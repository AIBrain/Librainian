// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/Windows.cs" was last cleaned by Rick on 2015/11/13 at 11:30 PM

namespace Librainian.OperatingSystem {

    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using Collections;
    using FileSystem;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Parsing;
    using TimeoutException = System.ServiceProcess.TimeoutException;

    public static class Windows {

        private const String PATH = "PATH";

        [NotNull] public static readonly Lazy< Document > CommandPrompt = new Lazy< Document >( () => {
                                                                                                    Document commandPrompt;
                                                                                                    FindDocument( Path.Combine( WindowsSystem32Folder.Value.FullName, "cmd.exe" ), out commandPrompt );
                                                                                                    return commandPrompt;
                                                                                                }, true );

        public static readonly Char[] PathSeparator = {';'};

        [NotNull] public static readonly Lazy< Document > PowerShellV1 = new Lazy< Document >( () => {
                                                                                                   Document powerShell;
                                                                                                   if ( !FindDocument( Path.Combine( PowerShellV1Folder.Value.FullName, "powershell.exe" ), out powerShell ) ) {
                                                                                                       throw new FileNotFoundException( "Unable to locate v1 of powershell.exe." );
                                                                                                   }
                                                                                                   return powerShell;
                                                                                               }, true );

        [NotNull] public static readonly Lazy< Folder > PowerShellV1Folder = new Lazy< Folder >( () => {
                                                                                                     Folder powerShellFolder;
                                                                                                     if ( !FindFolder( Path.Combine( WindowsSystem32Folder.Value.FullName, @"WindowsPowerShell\v1.0" ), out powerShellFolder ) ) {
                                                                                                         throw new DirectoryNotFoundException( "Unable to locate Windows powershell v1 folder." );
                                                                                                     }
                                                                                                     return powerShellFolder;
                                                                                                 }, true );

        [NotNull] public static readonly Lazy< Folder > WindowsFolder = new Lazy< Folder >( () => {
                                                                                                Folder windowsFolder;
                                                                                                FindFolder( Environment.GetFolderPath( Environment.SpecialFolder.Windows ), out windowsFolder );
                                                                                                return windowsFolder;
                                                                                            }, true );

        [NotNull] public static readonly Lazy< Folder > WindowsSystem32Folder = new Lazy< Folder >( () => {
                                                                                                        Folder windowsSystem32Folder;
                                                                                                        FindFolder( Path.Combine( WindowsFolder.Value.FullName, "System32" ), out windowsSystem32Folder );
                                                                                                        return windowsSystem32Folder;
                                                                                                    }, true );

        /// <summary>
        ///     Cleans and sorts the Windows <see cref="Environment" /> path variable.
        /// </summary>
        /// <returns></returns>
        public static async Task< Boolean > CleanUpPATH( Boolean reportToConsole = false ) {
            return await Task.Run( () => {
                                       if ( reportToConsole ) {
                                           "Attempting to verify and fix the PATH environment.".WriteLine();
                                       }

                                       var currentPath = GetCurrentPATH()
                                           .Trim();
                                       if ( String.IsNullOrWhiteSpace( currentPath ) ) {
                                           "Unable to obtain the current PATH variable.".Error();
                                           if ( reportToConsole ) {
                                               "Exiting subroutine. No changes have been made to the PATH variable.".WriteLine();
                                           }
                                           return false;
                                       }

                                       var justpaths = currentPath.Split( PathSeparator, StringSplitOptions.RemoveEmptyEntries )
                                                                  .OrderByDescending( s => s.Length )
                                                                  .ToList();
                                       var pathsData = new ConcurrentDictionary< String, Folder >();

                                       if ( reportToConsole ) {
                                           $"Found PATH list with {justpaths.Count} entries.".WriteLine();
                                       }
                                       foreach ( var s in justpaths ) {
                                           pathsData[ s ] = new Folder( s );
                                           pathsData[ s ].Refresh();
                                       }

                                       if ( reportToConsole ) {
                                           "Examining entries...".WriteLine();
                                       }
                                       foreach ( var pair in pathsData.Where( pair => !pair.Value.Exists() ) ) {
                                           Folder dummy;
                                           if ( !pathsData.TryRemove( pair.Key, out dummy ) ) {
                                               continue;
                                           }
                                           if ( reportToConsole ) {
                                               $"Removing nonexisting folder `{dummy.FullName}` from PATH".WriteLine();
                                           }
                                       }

                                       foreach ( var pair in pathsData.Where( pair => !pair.Value.GetFolders()
                                                                                           .Any() && !pair.Value.GetDocuments()
                                                                                                          .Any() ) ) {
                                           Folder dummy;
                                           if ( !pathsData.TryRemove( pair.Key, out dummy ) ) {
                                               continue;
                                           }
                                           if ( reportToConsole ) {
                                               $"Removing empty folder {dummy.FullName} from PATH".WriteLine();
                                           }
                                       }

                                       if ( reportToConsole ) {
                                           "Rebuilding PATH entries...".WriteLine();
                                       }
                                       var rebuiltPath = pathsData.Values.OrderByDescending( info => info.FullName.Length )
                                                                  .Select( info => info.FullName )
                                                                  .ToStrings( ";" );

                                       if ( reportToConsole ) {
                                           "Applying new PATH entries...".WriteLine();
                                       }
                                       SetCurrentPATH( rebuiltPath );

                                       return true;
                                   } );
        }

        [NotNull]
        public static Task< Process > ExecuteCommandPrompt( String arguments ) {
            return Task.Run( () => {
                                 try {
                                     var proc = new ProcessStartInfo {UseShellExecute = false, WorkingDirectory = WindowsSystem32Folder.Value.FullName, FileName = CommandPrompt.Value.FullPathWithFileName, Verb = "runas", //demand elevated permissions
                                         Arguments = $"/C \"{arguments}\"", CreateNoWindow = false, ErrorDialog = true, WindowStyle = ProcessWindowStyle.Normal};

                                     $"Running command '{proc.Arguments}'...".WriteLine();
                                     return Process.Start( proc );
                                 }
                                 catch ( Exception exception ) {
                                     exception.More();
                                 }
                                 return null;
                             } );
        }

        [NotNull]

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static Task< Process > ExecuteExplorer( String arguments ) {
            return Task.Run( () => {
                                 try {
                                     var proc = new ProcessStartInfo {UseShellExecute = false, WorkingDirectory = Environment.CurrentDirectory, FileName = "explorer.exe",

                                         //Verb = "runas", //demand elevated permissions
                                         Arguments = $@"""{arguments}""", CreateNoWindow = false, ErrorDialog = true, WindowStyle = ProcessWindowStyle.Normal};

                                     $"Running shell command '{proc.Arguments}'...".WriteLine();
                                     return Process.Start( proc );
                                 }
                                 catch ( Exception exception ) {
                                     exception.More();
                                 }
                                 return null;
                             } );
        }

        [NotNull]
        public static Task< Boolean > ExecutePowershellV1Command( String arguments, Boolean elevated = false ) {
            return Task.Run( () => {
                                 try {
                                     var startInfo = new ProcessStartInfo {UseShellExecute = false, WorkingDirectory = PowerShellV1Folder.Value.FullName, FileName = PowerShellV1.Value.FullPathWithFileName, Verb = elevated ? "runas" : null, //demand elevated permissions
                                         Arguments = $"-EncodedCommand {arguments.ToBase64()}", CreateNoWindow = false, ErrorDialog = true, WindowStyle = ProcessWindowStyle.Normal};
                                     $"Running powershell command '{arguments}' with a ten minute timeout...".WriteLine();

                                     var process = Process.Start( startInfo );
                                     if ( null == process ) {
                                         return false;
                                     }
                                     process.WaitForExit( ( Int32 ) Minutes.Ten.ToSeconds()
                                                                           .ToMilliseconds()
                                                                           .Value );
                                     return true;
                                 }
                                 catch ( Exception exception ) {
                                     exception.More();
                                 }
                                 return false;
                             } );
        }

        public static Boolean FindDocument( String fullname, out Document mainDocument, String okayMessage = null, String errorMessage = null ) {
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

        public static async Task< Boolean > MirrorFolderStructure( Folder folder, Folder baseFolder ) {
            return await ExecutePowershellV1Command( $"xcopy \"{folder.FullName}\" \"{baseFolder.FullName}\" /E /T" );
        }

        public static async Task< Boolean > RestartService( String serviceName, TimeSpan timeout ) {
            return await StartService( serviceName, timeout ) && await StopService( serviceName, timeout );
        }

        public static void SetCurrentPATH( [NotNull] String path ) {
            if ( path == null ) {
                throw new ArgumentNullException( nameof( path ) );
            }
            Environment.SetEnvironmentVariable( PATH, path, EnvironmentVariableTarget.Machine );
        }

        public static async Task< Boolean > StartService( String serviceName, TimeSpan timeout ) {
            try {
                return await Task.Run( () => {
                                           using ( var service = new ServiceController( serviceName ) ) {
                                               if ( service.Status == ServiceControllerStatus.Stopped ) {
                                                   service.Start();
                                                   service.WaitForStatus( ServiceControllerStatus.Running, timeout );
                                               }
                                               return service.Status == ServiceControllerStatus.Running;
                                           }
                                       } );
            }
            catch ( TimeoutException exception ) {
                exception.More();
            }
            return false;
        }

        public static async Task< Boolean > StopService( String serviceName, TimeSpan timeout ) {
            try {
                return await Task.Run( () => {
                                           using ( var service = new ServiceController( serviceName ) ) {
                                               service.Stop();
                                               service.WaitForStatus( ServiceControllerStatus.Stopped, timeout );
                                           }
                                           return true;
                                       } );
            }
            catch ( TimeoutException exception ) {
                exception.More();
            }
            return false;
        }

    }

}
