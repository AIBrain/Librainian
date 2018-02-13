// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Windows.cs" was last cleaned by Rick on 2016/07/29 at 8:42 AM

namespace Librainian.OperatingSystem {

    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using Collections;
    using Extensions;
    using FileSystem;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Parsing;
    using TimeoutException = System.ServiceProcess.TimeoutException;

    public static class Windows {

        [NotNull]
        public static readonly Lazy<Document> CommandPrompt = new Lazy<Document>( () => {
            Document document;
            FindDocument( Path.Combine( WindowsSystem32Folder.Value.FullName, "cmd.exe" ), out document );
            return document;
        }, true );

        [CanBeNull]
        public static readonly Lazy<Document> IrfanView64 = new Lazy<Document>( () => {
            Document document;
            FindDocument( Path.Combine( @"C:\Program Files\IrfanView\", "i_view64.exe" ), out document );
            return document;
        }, true );

        public static readonly Char[] PathSeparator = { ';' };

        [NotNull]
        public static readonly Lazy<Document> PowerShellV1 = new Lazy<Document>( () => {
            Document document;
            if ( !FindDocument( Path.Combine( PowerShellV1Folder.Value.FullName, "powershell.exe" ), out document ) ) {
                throw new FileNotFoundException( "Unable to locate v1 of powershell.exe." );
            }

            return document;
        }, true );

        [NotNull]
        public static readonly Lazy<Folder> PowerShellV1Folder = new Lazy<Folder>( () => {
            Folder folder;
            if ( !FindFolder( Path.Combine( WindowsSystem32Folder.Value.FullName, @"WindowsPowerShell\v1.0" ), out folder ) ) {
                throw new DirectoryNotFoundException( "Unable to locate Windows PowerShell v1 folder." );
            }

            return folder;
        }, true );

        [NotNull]
        public static readonly Lazy<Folder> WindowsFolder = new Lazy<Folder>( () => {
            Folder folder;
            FindFolder( Environment.GetFolderPath( Environment.SpecialFolder.Windows ), out folder );
            return folder;
        }, true );

        [NotNull]
        public static readonly Lazy<Folder> WindowsSystem32Folder = new Lazy<Folder>( () => {
            Folder folder;
            FindFolder( Path.Combine( WindowsFolder.Value.FullName, "System32" ), out folder );
            return folder;
        }, true );

        private const String PATH = "PATH";

        /// <summary>
        ///     Cleans and sorts the Windows <see cref="Environment" /> path variable.
        /// </summary>
        /// <returns></returns>
        public static void CleanUpPATH( Boolean reportToConsole = false ) {
            if ( reportToConsole ) {
                "Attempting to verify and fix the PATH environment.".WriteLine();
            }

            var currentPath = GetCurrentPATH().Trim();
            if ( String.IsNullOrWhiteSpace( currentPath ) ) {
                "Unable to obtain the current PATH variable.".Error();
                if ( reportToConsole ) {
                    "Exiting subroutine. No changes have been made to the PATH variable.".WriteLine();
                }
                return;
            }

            var justpaths = currentPath.Split( PathSeparator, StringSplitOptions.RemoveEmptyEntries ).ToHashSet();

            if ( reportToConsole ) {
                $"Found PATH list with {justpaths.Count} entries.".WriteLine();
            }

            var pathsData = new ConcurrentDictionary<String, Folder>( Environment.ProcessorCount, justpaths.Count );
            foreach ( var s in justpaths ) {
                pathsData[ s ] = new Folder( s );
            }

            if ( reportToConsole ) {
                "Examining entries...".WriteLine();
            }
            foreach ( var pair in pathsData.Where( pair => !pair.Value.Exists() ) ) {
                Folder dummy;
                if ( pathsData.TryRemove( pair.Key, out dummy ) && reportToConsole ) {
                    $"Removing nonexistent folder `{dummy.FullName}` from PATH".WriteLine();
                }
            }

            foreach ( var pair in pathsData.Where( pair => !pair.Value.GetFolders().Any() && !pair.Value.GetDocuments().Any() ) ) {
                Folder dummy;
                if ( pathsData.TryRemove( pair.Key, out dummy ) && reportToConsole ) {
                    $"Removing empty folder {dummy.FullName} from PATH".WriteLine();
                }
            }

            if ( reportToConsole ) {
                "Rebuilding PATH entries...".WriteLine();
            }
            var rebuiltPath = pathsData.Values.OrderByDescending( info => info.FullName.Length ).Select( info => info.FullName ).ToStrings( ";" );

            if ( reportToConsole ) {
                "Applying new PATH entries...".WriteLine();
            }
            Environment.SetEnvironmentVariable( PATH, rebuiltPath, EnvironmentVariableTarget.Machine );
        }

        [NotNull]
        public static Task<Process> ExecuteCommandPrompt( String arguments, Boolean newConsoleWindow = true ) {
            return Task.Run( () => {
                try {
                    var proc = new ProcessStartInfo {
                        UseShellExecute = newConsoleWindow,
                        WorkingDirectory = WindowsSystem32Folder.Value.FullName,
                        FileName = CommandPrompt.Value.FullPathWithFileName,
                        Verb = "runas", //demand elevated permissions
                        Arguments = $"/C \"{arguments}\"",
                        CreateNoWindow = false,
                        ErrorDialog = true,
                        WindowStyle = ProcessWindowStyle.Normal
                    };

                    $"Running command '{proc.Arguments}'...".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );
                    return Process.Start( proc );
                }
                catch ( Exception exception ) {
                    exception.More();
                }

                return null;
            } );
        }

        [NotNull]
        public static Task<Process> ExecuteProcess( Document filename, String arguments, Boolean elevate ) {
            return Task.Run( () => {
                try {
                    var proc = new ProcessStartInfo {
                        UseShellExecute = false,
                        WorkingDirectory = filename.Folder().FullName,
                        FileName = filename.FullPathWithFileName,
                        Verb = elevate ? null : "runas", //demand elevated permissions
                        Arguments = arguments,
                        CreateNoWindow = false,
                        ErrorDialog = true,
                        WindowStyle = ProcessWindowStyle.Normal
                    };

                    $"Running process '{filename} {proc.Arguments}'...".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );
                    return Process.Start( proc );
                }
                catch ( Exception exception ) {
                    exception.More();
                }

                return null;
            } );
        }

        [CanBeNull]
        public static Task<Process> ExecuteExplorer( String arguments ) {
            return Task.Run( () => {
                try {
                    var proc = new ProcessStartInfo {
                        UseShellExecute = false,
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName = "explorer.exe",

                        //Verb = "runas", //demand elevated permissions
                        Arguments = $@"""{arguments}""",
                        CreateNoWindow = false,
                        ErrorDialog = true,
                        WindowStyle = ProcessWindowStyle.Normal
                    };

                    $"Running command '{proc.Arguments}'...".WriteLineColor( ConsoleColor.White, ConsoleColor.Cyan );
                    return Process.Start( proc );
                }
                catch ( Exception exception ) {
                    exception.More();
                }

                return null;
            } );
        }

        [NotNull]
        public static Task<Boolean> ExecutePowershellV1Command( String arguments, Boolean elevated = false ) {
            return Task.Run( () => {
                try {
                    var startInfo = new ProcessStartInfo {
                        UseShellExecute = false,
                        WorkingDirectory = PowerShellV1Folder.Value.FullName,
                        FileName = PowerShellV1.Value.FullPathWithFileName,
                        Verb = elevated ? "runas" : null, //demand elevated permissions
                        Arguments = $"-EncodedCommand {arguments.ToBase64()}",
                        CreateNoWindow = false,
                        ErrorDialog = true,
                        WindowStyle = ProcessWindowStyle.Normal
                    };
                    $"Running PowerShell command '{arguments}'...".WriteLineColor( ConsoleColor.White, ConsoleColor.Green );

                    var process = Process.Start( startInfo );
                    if ( null == process ) {
                        "failure.".WriteLine();
                        return false;
                    }

                    process.WaitForExit( ( Int32 )Minutes.One.ToSeconds().ToMilliseconds().Value );
                    "success.".WriteLine();
                    return true;
                }
                catch ( Exception exception ) {
                    exception.More();
                }

                return false;
            } );
        }

        public static Boolean FindDocument( String fullname, [CanBeNull] out Document mainDocument, String okayMessage = null, String errorMessage = null ) {
            if ( !String.IsNullOrEmpty( okayMessage ) ) {
                $"Finding {fullname}...".Write();
            }
            mainDocument = new Document( fullname );
            if ( !mainDocument.Exists() ) {
                errorMessage.Error();
                return false;
            }

            if ( !String.IsNullOrEmpty( okayMessage ) ) {
                okayMessage.WriteLine();
            }
            return true;
        }

        public static Boolean FindFolder( String fullname, out Folder mainFolder, String okayMessage = null, String errorMessage = null ) {
            if ( !String.IsNullOrEmpty( okayMessage ) ) {
                $"Finding {fullname}...".Write();
            }

            mainFolder = new Folder( fullname );
            if ( !mainFolder.Exists() ) {
                errorMessage.Error();
                return false;
            }

            if ( !String.IsNullOrEmpty( okayMessage ) ) {
                okayMessage.WriteLine();
            }
            return true;
        }

        [NotNull]
        public static String GetCurrentPATH() => Environment.GetEnvironmentVariable( PATH, EnvironmentVariableTarget.Machine ) ?? String.Empty;

        public static async Task<Boolean> MirrorFolderStructure( Folder folder, Folder baseFolder ) {
            return await ExecutePowershellV1Command( $"xcopy.exe \"{folder.FullName}\" \"{baseFolder.FullName}\" /E /T" );
        }

        public static async Task<Boolean> RestartService( String serviceName, TimeSpan timeout ) {
            return await StartService( serviceName, timeout ) && await StopService( serviceName, timeout );
        }

        public static async Task<Boolean> StartService( String serviceName, TimeSpan timeout ) {
            try {
                return await Task.Run( () => {
                    using ( var service = new ServiceController( serviceName ) ) {
                        if ( service.Status != ServiceControllerStatus.Running ) {
                            service.Start();
                        }
                        service.WaitForStatus( ServiceControllerStatus.Running, timeout );
                        return service.Status == ServiceControllerStatus.Running;
                    }
                } );
            }
            catch ( TimeoutException exception ) {
                exception.More();
            }

            return false;
        }

        public static async Task<Boolean> StopService( String serviceName, TimeSpan timeout ) {
            try {
                return await Task.Run( () => {
                    using ( var service = new ServiceController( serviceName ) ) {
                        service.Stop();
                        service.WaitForStatus( ServiceControllerStatus.Stopped, timeout );
                        return service.Status == ServiceControllerStatus.Stopped;
                    }
                } );
            }
            catch ( TimeoutException exception ) {
                exception.More();
            }

            return false;
        }

        [CanBeNull]
        public static Task<Process> TryConvert_WithIrfanview( Document inDocument, Document outDocument ) {
            return Task.Run( () => {
                if ( IrfanView64 == null ) {
                    return null;
                }
                if ( !IrfanView64.Value.Exists() ) {
                    return null;
                }

                try {
                    var arguments = $" {inDocument.FullPathWithFileName.Quoted()} /convert={outDocument.FullPathWithFileName.Quoted()} ";

                    var proc = new ProcessStartInfo {
                        UseShellExecute = false,
                        WorkingDirectory = IrfanView64.Value.Folder().FullName,
                        FileName = IrfanView64.Value.FullPathWithFileName,

                        //Verb = "runas", //demand elevated permissions
                        Arguments = arguments,
                        CreateNoWindow = true,
                        ErrorDialog = false,
                        WindowStyle = ProcessWindowStyle.Normal
                    };

                    $"Running irfanview command '{proc.Arguments}'...".WriteLine();
                    return Process.Start( proc );
                }
                catch ( Exception exception ) {
                    exception.More();
                }

                return null;
            } );
        }
    }
}