// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Windows.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Windows.cs" was last formatted by Protiguous on 2020/01/31 at 12:28 AM.

namespace Librainian.OperatingSystem {

    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Runtime;
    using System.ServiceProcess;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Collections.Extensions;
    using Extensions;
    using FileSystem;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Measurement.Time;
    using Parsing;
    using TimeoutException = System.ServiceProcess.TimeoutException;

    public static class Windows {

        private const String PATH = "PATH";

        [CanBeNull]
        public static readonly Lazy<Document> CommandPrompt =
            new Lazy<Document>( () => FindDocument( fullname: Path.Combine( path1: WindowsSystem32Folder.Value.FullName, path2: "cmd.exe" ) ), isThreadSafe: true );

        [CanBeNull]
        public static readonly Lazy<Document> IrfanView64 =
            new Lazy<Document>(
                () => FindDocument( fullname: Path.Combine( path1: Environment.GetFolderPath( folder: Environment.SpecialFolder.ProgramFiles ) + @"\IrfanView\",
                    path2: "i_view64.exe" ) ), isThreadSafe: true );

        public static readonly Char[] PathSeparator = {
            ';'
        };

        [NotNull]
        public static readonly Lazy<Document> PowerShell = new Lazy<Document>( () => {
            var document = FindDocument( fullname: Path.Combine( path1: PowerShellFolder.Value.FullName, path2: "powershell.exe" ) );

            if ( null == document ) {
                throw new FileNotFoundException( "Unable to locate powershell.exe." );
            }

            return document;
        }, isThreadSafe: true );

        [NotNull]
        public static readonly Lazy<Folder> PowerShellFolder = new Lazy<Folder>( () => {
            var folder = FindFolder( fullname: Path.Combine( path1: WindowsSystem32Folder.Value.FullName, path2: @"WindowsPowerShell\v1.0" ) );

            if ( null == folder ) {
                throw new DirectoryNotFoundException( "Unable to locate Windows PowerShell folder." );
            }

            return folder;
        }, isThreadSafe: true );

        [NotNull]
        public static readonly Lazy<Folder> WindowsFolder = new Lazy<Folder>( () => {
            var folder = FindFolder( fullname: Environment.GetFolderPath( folder: Environment.SpecialFolder.Windows ) );

            return folder;
        }, isThreadSafe: true );

        [NotNull]
        public static readonly Lazy<Folder> WindowsSystem32Folder =
            new Lazy<Folder>( () => FindFolder( fullname: Path.Combine( path1: WindowsFolder.Value.FullName, path2: "System32" ) ), isThreadSafe: true );

        /// <summary>Cleans and sorts the Windows <see cref="Environment" /> path variable.</summary>
        /// <returns></returns>
        public static void CleanUpPATH( Boolean reportToConsole = false ) {
            if ( reportToConsole ) {
                "Attempting to verify and fix the PATH environment.".Info();
            }

            var currentPath = GetCurrentPATH().Trim();

            if ( String.IsNullOrWhiteSpace( currentPath ) ) {
                "Unable to obtain the current PATH variable.".Log();

                if ( reportToConsole ) {
                    "Exiting subroutine. No changes have been made to the PATH variable.".Info();
                }

                return;
            }

            var justpaths = currentPath.Split( separator: PathSeparator, options: StringSplitOptions.RemoveEmptyEntries ).ToHashSet();

            if ( reportToConsole ) {
                $"Found PATH list with {justpaths.Count} entries.".Info();
            }

            var pathsData = new ConcurrentDictionary<String, Folder>( concurrencyLevel: Environment.ProcessorCount, capacity: justpaths.Count );

            foreach ( var s in justpaths ) {
                pathsData[ s ] = new Folder( fullPath: s );
            }

            if ( reportToConsole ) {
                "Examining entries...".Info();
            }

            foreach ( var pair in pathsData.Where( pair => !pair.Value.Exists() ) ) {
                if ( pathsData.TryRemove( pair.Key, out var dummy ) && reportToConsole ) {
                    $"Removing nonexistent folder `{dummy.FullName}` from PATH".Info();
                }
            }

            foreach ( var pair in pathsData.Where( pair => !pair.Value.GetFolders( "*" ).Any() && !pair.Value.GetDocuments().Any() ) ) {
                if ( pathsData.TryRemove( pair.Key, out var dummy ) && reportToConsole ) {
                    $"Removing empty folder {dummy.FullName} from PATH".Info();
                }
            }

            if ( reportToConsole ) {
                "Rebuilding PATH entries...".Info();
            }

            var rebuiltPath = pathsData.Values.OrderByDescending( info => info.FullName.Length ).Select( info => info.FullName ).ToStrings( ";" );

            if ( reportToConsole ) {
                "Applying new PATH entries...".Info();
            }

            Environment.SetEnvironmentVariable( variable: PATH, rebuiltPath, EnvironmentVariableTarget.Machine );
        }

        public static Boolean CreateRestorePoint( String description = null ) {
            try {
                if ( String.IsNullOrWhiteSpace( description ) ) {
                    description = DateTime.Now.ToLongDateTime();
                }

                var oScope = new ManagementScope( @"\\localhost\root\default" );
                var oPath = new ManagementPath( "SystemRestore" );
                var oGetOp = new ObjectGetOptions();

                using ( var oProcess = new ManagementClass( scope: oScope, oPath, options: oGetOp ) ) {
                    var oInParams = oProcess.GetMethodParameters( "CreateRestorePoint" );
                    oInParams[ "Description" ] = description;
                    oInParams[ "RestorePointType" ] = 12; // MODIFY_SETTINGS
                    oInParams[ "EventType" ] = 100;

                    var oOutParams = oProcess.InvokeMethod( "CreateRestorePoint", inParameters: oInParams, options: null );

                    return oOutParams != null;
                }
            }
            catch ( Exception ) { }

            return default;
        }

        [NotNull]
        public static Task<Process> ExecuteCommandPromptAsync( [CanBeNull] String arguments ) =>
            Task.Run( () => {
                try {
                    if ( CommandPrompt != null ) {
                        var proc = new ProcessStartInfo {
                            UseShellExecute = false,
                            WorkingDirectory = WindowsSystem32Folder.Value.FullName,
                            FileName = CommandPrompt.Value.FullPath,
                            Verb = "runas", //demand elevated permissions
                            Arguments = $"/C \"{arguments}\"",
                            CreateNoWindow = false,
                            ErrorDialog = true,
                            WindowStyle = ProcessWindowStyle.Normal
                        };

                        $"Running command '{proc.Arguments}'...".WriteLineColor( foreColor: ConsoleColor.White, backColor: ConsoleColor.Blue );

                        return Process.Start( startInfo: proc );
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();
                }

                return null;
            } );

        [NotNull]
        public static Task<Boolean> ExecutePowershellCommandAsync( [CanBeNull] String arguments, Boolean elevated = false ) =>
            Task.Run( () => {
                try {
                    var startInfo = new ProcessStartInfo {
                        UseShellExecute = false,
                        WorkingDirectory = PowerShellFolder.Value.FullName,
                        FileName = PowerShell.Value.FullPath,
                        Verb = elevated ? "runas" : null, //demand elevated permissions
                        Arguments = $"-EncodedCommand {arguments.ToBase64()}",
                        CreateNoWindow = false,
                        ErrorDialog = true,
                        WindowStyle = ProcessWindowStyle.Normal
                    };

                    $"Running PowerShell command '{arguments}'...".WriteLineColor( foreColor: ConsoleColor.White, backColor: ConsoleColor.Green );

                    var process = Process.Start( startInfo: startInfo );

                    if ( null == process ) {
                        "failure.".Info();

                        return default;
                    }

                    process.WaitForExit( milliseconds: ( Int32 )Minutes.One.ToSeconds().ToMilliseconds().Value );
                    "success.".Info();

                    return true;
                }
                catch ( Exception exception ) {
                    exception.Log();
                }

                return default;
            } );

        [NotNull]
        public static Task<Process> ExecuteProcessAsync( [NotNull] Document filename, [NotNull] Folder workingFolder, [CanBeNull] String arguments, Boolean elevate ) {
            if ( filename == null ) {
                throw new ArgumentNullException( paramName: nameof( filename ) );
            }

            if ( workingFolder == null ) {
                throw new ArgumentNullException( paramName: nameof( workingFolder ) );
            }

            return Task.Run( () => {
                try {
                    var proc = new ProcessStartInfo {
                        UseShellExecute = false,
                        WorkingDirectory = workingFolder.FullName,
                        FileName = filename.FullPath,
                        Verb = elevate ? null : "runas", //demand elevated permissions
                        Arguments = arguments ?? String.Empty,
                        CreateNoWindow = false,
                        ErrorDialog = true,
                        WindowStyle = ProcessWindowStyle.Normal
                    };

                    $"Running process '{filename} {proc.Arguments}'...".WriteLineColor( foreColor: ConsoleColor.White, backColor: ConsoleColor.Blue );

                    return Process.Start( startInfo: proc );
                }
                catch ( Exception exception ) {
                    exception.Log();
                }

                return null;
            } );
        }

        [CanBeNull]
        public static Document FindDocument( [NotNull] String fullname, [CanBeNull] String okayMessage = null, [CanBeNull] String errorMessage = null ) {
            if ( !String.IsNullOrEmpty( okayMessage ) ) {
                $"Finding {fullname}...".Info();
            }

            var mainDocument = new Document( fullname );

            if ( mainDocument.Exists() ) {
                okayMessage.Info();

                return mainDocument;
            }

            errorMessage.Error();

            return null;
        }

        [CanBeNull]
        public static Folder FindFolder( [NotNull] String fullname, [CanBeNull] String okayMessage = null, [CanBeNull] String errorMessage = null ) {
            if ( String.IsNullOrWhiteSpace( fullname ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fullname ) );
            }

            if ( !String.IsNullOrEmpty( okayMessage ) ) {
                $"Finding {fullname}...".Info();
            }

            var mainFolder = new Folder( fullPath: fullname );

            if ( !mainFolder.Exists() ) {
                errorMessage.Error();

                return null;
            }

            if ( !String.IsNullOrEmpty( okayMessage ) ) {
                okayMessage.Info();
            }

            return mainFolder;
        }

        [NotNull]
        public static String GetCurrentPATH() => Environment.GetEnvironmentVariable( variable: PATH, EnvironmentVariableTarget.Machine ) ?? String.Empty;

        public static Boolean IsServer() => GCSettings.IsServerGC;

        public static Boolean IsWorkStation() => !GCSettings.IsServerGC;

        [NotNull]
        public static Task<Boolean> MirrorFolderStructureAsync( [NotNull] Folder folder, [NotNull] Folder baseFolder ) =>
            ExecutePowershellCommandAsync( arguments: $"xcopy.exe \"{folder.FullName}\" \"{baseFolder.FullName}\" /E /T" );

        [CanBeNull]
        public static Process OpenWithExplorer( [CanBeNull] String value ) {
            try {

                //Verb = "runas", //demand elevated permissions
                var proc = new ProcessStartInfo {
                    UseShellExecute = false,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Path.Combine( WindowsSystem32Folder.Value.FullName, "explorer.exe" ),
                    Arguments = $" /separate /select,\"{value}\" ",
                    CreateNoWindow = false,
                    ErrorDialog = true,
                    WindowStyle = ProcessWindowStyle.Normal
                };

                $"Running command '{proc.Arguments}'...".WriteLineColor( foreColor: ConsoleColor.White, backColor: ConsoleColor.Cyan );

                return Process.Start( startInfo: proc );
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        public static async Task<Boolean> RestartServiceAsync( [NotNull] String serviceName, TimeSpan timeout ) {
            if ( String.IsNullOrWhiteSpace( value: serviceName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( serviceName ) );
            }

            return await StartServiceAsync( serviceName: serviceName, timeout: timeout ).ConfigureAwait( false ) &&
                   await StopServiceAsync( serviceName: serviceName, timeout: timeout ).ConfigureAwait( false );
        }

        public static async Task<Boolean> StartServiceAsync( [NotNull] String serviceName, TimeSpan timeout ) {
            if ( String.IsNullOrWhiteSpace( value: serviceName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( serviceName ) );
            }

            try {
                return await Task.Run( () => {
                    using ( var service = new ServiceController( serviceName ) ) {
                        if ( service.Status != ServiceControllerStatus.Running ) {
                            service.Start();
                        }

                        service.WaitForStatus( desiredStatus: ServiceControllerStatus.Running, timeout: timeout );

                        return service.Status == ServiceControllerStatus.Running;
                    }
                } ).ConfigureAwait( false );
            }
            catch ( TimeoutException exception ) {
                exception.Log();
            }

            return default;
        }

        public static async Task<Boolean> StopServiceAsync( [NotNull] String serviceName, TimeSpan timeout ) {
            if ( String.IsNullOrWhiteSpace( value: serviceName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( serviceName ) );
            }

            try {
                return await Task.Run( () => {
                    using ( var service = new ServiceController( serviceName ) ) {
                        service.Stop();
                        service.WaitForStatus( desiredStatus: ServiceControllerStatus.Stopped, timeout: timeout );

                        return service.Status == ServiceControllerStatus.Stopped;
                    }
                } ).ConfigureAwait( false );
            }
            catch ( TimeoutException exception ) {
                exception.Log();
            }

            return default;
        }

        [CanBeNull]
        public static Task<Process> TryConvert_WithIrfanviewAsync( [NotNull] Document inDocument, [NotNull] Document outDocument ) {
            if ( inDocument == null ) {
                throw new ArgumentNullException( paramName: nameof( inDocument ) );
            }

            if ( outDocument == null ) {
                throw new ArgumentNullException( paramName: nameof( outDocument ) );
            }

            return Task.Run( () => {

                if ( IrfanView64?.Value.Exists() != true ) {
                    return null;
                }

                try {
                    var arguments = $" {inDocument.FullPath.Quoted()} /convert={outDocument.FullPath.Quoted()} ";

                    var proc = new ProcessStartInfo {
                        UseShellExecute = false,
                        WorkingDirectory = Folder.GetTempFolder().FullName,
                        FileName = IrfanView64.Value.FullPath,

                        //Verb = "runas", //demand elevated permissions
                        Arguments = arguments,
                        CreateNoWindow = true,
                        ErrorDialog = false,
                        WindowStyle = ProcessWindowStyle.Normal
                    };

                    $"Running irfanview command '{proc.Arguments}'...".Info();

                    return Process.Start( startInfo: proc );
                }
                catch ( Exception exception ) {
                    exception.Log();
                }

                return null;
            } );
        }

        /// <summary><see cref="Application.DoEvents()" /> and then <see cref="Thread.Yield()" />.</summary>
        public static void Yield() {
            Application.DoEvents();

            if ( Randem.NextBooleanFast() ) {
                Thread.Yield();
            }
        }
    }
}