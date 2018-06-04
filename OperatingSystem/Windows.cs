// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Windows.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Windows.cs" was last formatted by Protiguous on 2018/06/04 at 4:19 PM.

namespace Librainian.OperatingSystem {

	using System;
	using System.Collections.Concurrent;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Management;
	using System.ServiceProcess;
	using System.Threading.Tasks;
	using Collections;
	using ComputerSystems.FileSystem;
	using Extensions;
	using JetBrains.Annotations;
	using Measurement.Time;
	using Parsing;
	using TimeoutException = System.ServiceProcess.TimeoutException;

	public static class Windows {

		/// <summary>
		///     Cleans and sorts the Windows <see cref="Environment" /> path variable.
		/// </summary>
		/// <returns></returns>
		public static void CleanUpPATH( Boolean reportToConsole = false ) {
			if ( reportToConsole ) { "Attempting to verify and fix the PATH environment.".WriteLine(); }

			var currentPath = GetCurrentPATH().Trim();

			if ( String.IsNullOrWhiteSpace( currentPath ) ) {
				"Unable to obtain the current PATH variable.".Error();

				if ( reportToConsole ) { "Exiting subroutine. No changes have been made to the PATH variable.".WriteLine(); }

				return;
			}

			var justpaths = currentPath.Split( separator: PathSeparator, options: StringSplitOptions.RemoveEmptyEntries ).ToHashSet();

			if ( reportToConsole ) { $"Found PATH list with {justpaths.Count} entries.".WriteLine(); }

			var pathsData = new ConcurrentDictionary<String, Folder>( concurrencyLevel: Environment.ProcessorCount, capacity: justpaths.Count );

			foreach ( var s in justpaths ) { pathsData[ s ] = new Folder( fullPath: s ); }

			if ( reportToConsole ) { "Examining entries...".WriteLine(); }

			foreach ( var pair in pathsData.Where( pair => !pair.Value.Exists() ) ) {
				if ( pathsData.TryRemove( pair.Key, out var dummy ) && reportToConsole ) { $"Removing nonexistent folder `{dummy.FullName}` from PATH".WriteLine(); }
			}

			foreach ( var pair in pathsData.Where( pair => !pair.Value.GetFolders().Any() && !pair.Value.GetDocuments().Any() ) ) {
				if ( pathsData.TryRemove( pair.Key, out var dummy ) && reportToConsole ) { $"Removing empty folder {dummy.FullName} from PATH".WriteLine(); }
			}

			if ( reportToConsole ) { "Rebuilding PATH entries...".WriteLine(); }

			var rebuiltPath = pathsData.Values.OrderByDescending( info => info.FullName.Length ).Select( info => info.FullName ).ToStrings( ";" );

			if ( reportToConsole ) { "Applying new PATH entries...".WriteLine(); }

			Environment.SetEnvironmentVariable( variable: PATH, rebuiltPath, EnvironmentVariableTarget.Machine );
		}

		public static Boolean CreateRestorePoint( String description = null ) {
			try {
				if ( String.IsNullOrWhiteSpace( description ) ) { description = DateTime.Now.ToLongDateTime(); }

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

			return false;
		}

		[NotNull]
		public static Task<Process> ExecuteCommandPrompt( String arguments, Boolean newConsoleWindow = true ) =>
			Task.Run( () => {
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

					$"Running command '{proc.Arguments}'...".WriteLineColor( foreColor: ConsoleColor.White, backColor: ConsoleColor.Blue );

					return Process.Start( startInfo: proc );
				}
				catch ( Exception exception ) { exception.More(); }

				return null;
			} );

		[CanBeNull]
		public static Task<Process> ExecuteExplorer( String arguments ) =>
			Task.Run( () => {
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

					$"Running command '{proc.Arguments}'...".WriteLineColor( foreColor: ConsoleColor.White, backColor: ConsoleColor.Cyan );

					return Process.Start( startInfo: proc );
				}
				catch ( Exception exception ) { exception.More(); }

				return null;
			} );

		[NotNull]
		public static Task<Boolean> ExecutePowershellCommand( String arguments, Boolean elevated = false ) =>
			Task.Run( () => {
				try {
					var startInfo = new ProcessStartInfo {
						UseShellExecute = false,
						WorkingDirectory = PowerShellFolder.Value.FullName,
						FileName = PowerShell.Value.FullPathWithFileName,
						Verb = elevated ? "runas" : null, //demand elevated permissions
						Arguments = $"-EncodedCommand {arguments.ToBase64()}",
						CreateNoWindow = false,
						ErrorDialog = true,
						WindowStyle = ProcessWindowStyle.Normal
					};

					$"Running PowerShell command '{arguments}'...".WriteLineColor( foreColor: ConsoleColor.White, backColor: ConsoleColor.Green );

					var process = Process.Start( startInfo: startInfo );

					if ( null == process ) {
						"failure.".WriteLine();

						return false;
					}

					process.WaitForExit( milliseconds: ( Int32 ) Minutes.One.ToSeconds().ToMilliseconds().Value );
					"success.".WriteLine();

					return true;
				}
				catch ( Exception exception ) { exception.More(); }

				return false;
			} );

		[NotNull]
		public static Task<Process> ExecuteProcess( Document filename, Folder workingFolder, String arguments, Boolean elevate ) =>
			Task.Run( () => {
				try {
					var proc = new ProcessStartInfo {
						UseShellExecute = false,
						WorkingDirectory = filename.Folder.FullName,
						FileName = filename.FullPathWithFileName,
						Verb = elevate ? null : "runas", //demand elevated permissions
						Arguments = arguments,
						CreateNoWindow = false,
						ErrorDialog = true,
						WindowStyle = ProcessWindowStyle.Normal
					};

					$"Running process '{filename} {proc.Arguments}'...".WriteLineColor( foreColor: ConsoleColor.White, backColor: ConsoleColor.Blue );

					return Process.Start( startInfo: proc );
				}
				catch ( Exception exception ) { exception.More(); }

				return null;
			} );

		[CanBeNull]
		public static Document FindDocument( String fullname, String okayMessage = null, String errorMessage = null ) {
			if ( !String.IsNullOrEmpty( okayMessage ) ) { $"Finding {fullname}...".Write(); }

			var mainDocument = new Document( fullPathWithFilename: fullname );

			if ( mainDocument.Exists() ) {
				if ( !String.IsNullOrEmpty( okayMessage ) ) { okayMessage.WriteLine(); }

				return mainDocument;
			}

			errorMessage.Error();

			return null;
		}

		[CanBeNull]
		public static Folder FindFolder( [NotNull] String fullname, String okayMessage = null, String errorMessage = null ) {
			if ( String.IsNullOrWhiteSpace( fullname ) ) { throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fullname ) ); }

			if ( !String.IsNullOrEmpty( okayMessage ) ) { $"Finding {fullname}...".Write(); }

			var mainFolder = new Folder( fullPath: fullname );

			if ( !mainFolder.Exists() ) {
				errorMessage.Error();

				return null;
			}

			if ( !String.IsNullOrEmpty( okayMessage ) ) { okayMessage.WriteLine(); }

			return mainFolder;
		}

		[NotNull]
		public static String GetCurrentPATH() => Environment.GetEnvironmentVariable( variable: PATH, EnvironmentVariableTarget.Machine ) ?? String.Empty;

		public static async Task<Boolean> MirrorFolderStructure( Folder folder, Folder baseFolder ) => await ExecutePowershellCommand( arguments: $"xcopy.exe \"{folder.FullName}\" \"{baseFolder.FullName}\" /E /T" );

		public static async Task<Boolean> RestartService( String serviceName, TimeSpan timeout ) =>
			await StartService( serviceName: serviceName, timeout: timeout ) && await StopService( serviceName: serviceName, timeout: timeout );

		public static async Task<Boolean> StartService( String serviceName, TimeSpan timeout ) {
			try {
				return await Task.Run( () => {
					using ( var service = new ServiceController( name: serviceName ) ) {
						if ( service.Status != ServiceControllerStatus.Running ) { service.Start(); }

						service.WaitForStatus( desiredStatus: ServiceControllerStatus.Running, timeout: timeout );

						return service.Status == ServiceControllerStatus.Running;
					}
				} );
			}
			catch ( TimeoutException exception ) { exception.More(); }

			return false;
		}

		public static async Task<Boolean> StopService( String serviceName, TimeSpan timeout ) {
			try {
				return await Task.Run( () => {
					using ( var service = new ServiceController( name: serviceName ) ) {
						service.Stop();
						service.WaitForStatus( desiredStatus: ServiceControllerStatus.Stopped, timeout: timeout );

						return service.Status == ServiceControllerStatus.Stopped;
					}
				} );
			}
			catch ( TimeoutException exception ) { exception.More(); }

			return false;
		}

		[CanBeNull]
		public static Task<Process> TryConvert_WithIrfanview( Document inDocument, Document outDocument ) =>
			Task.Run( () => {
				if ( IrfanView64 is null ) { return null; }

				if ( !IrfanView64.Value.Exists() ) { return null; }

				try {
					var arguments = $" {inDocument.FullPathWithFileName.Quoted()} /convert={outDocument.FullPathWithFileName.Quoted()} ";

					var proc = new ProcessStartInfo {
						UseShellExecute = false,
						WorkingDirectory = IrfanView64.Value.Folder.FullName,
						FileName = IrfanView64.Value.FullPathWithFileName,

						//Verb = "runas", //demand elevated permissions
						Arguments = arguments,
						CreateNoWindow = true,
						ErrorDialog = false,
						WindowStyle = ProcessWindowStyle.Normal
					};

					$"Running irfanview command '{proc.Arguments}'...".WriteLine();

					return Process.Start( startInfo: proc );
				}
				catch ( Exception exception ) { exception.More(); }

				return null;
			} );

		private const String PATH = "PATH";

		[CanBeNull]
		public static readonly Lazy<Document> CommandPrompt = new Lazy<Document>( () => FindDocument( fullname: Path.Combine( path1: WindowsSystem32Folder.Value.FullName, path2: "cmd.exe" ) ), isThreadSafe: true );

		[CanBeNull]
		public static readonly Lazy<Document> IrfanView64 =
			new Lazy<Document>( () => FindDocument( fullname: Path.Combine( path1: Environment.GetFolderPath( folder: Environment.SpecialFolder.ProgramFiles ) + @"\IrfanView\", path2: "i_view64.exe" ) ),
				isThreadSafe: true );

		public static readonly Char[] PathSeparator = { ';' };

		[NotNull]
		public static readonly Lazy<Document> PowerShell = new Lazy<Document>( () => {
			var document = FindDocument( fullname: Path.Combine( path1: PowerShellFolder.Value.FullName, path2: "powershell.exe" ) );

			if ( null == document ) { throw new FileNotFoundException( "Unable to locate powershell.exe." ); }

			return document;
		}, isThreadSafe: true );

		[NotNull]
		public static readonly Lazy<Folder> PowerShellFolder = new Lazy<Folder>( () => {
			var folder = FindFolder( fullname: Path.Combine( path1: WindowsSystem32Folder.Value.FullName, path2: @"WindowsPowerShell\v1.0" ) );

			if ( null == folder ) { throw new DirectoryNotFoundException( "Unable to locate Windows PowerShell folder." ); }

			return folder;
		}, isThreadSafe: true );

		[NotNull]
		public static readonly Lazy<Folder> WindowsFolder = new Lazy<Folder>( () => {
			var folder = FindFolder( fullname: Environment.GetFolderPath( folder: Environment.SpecialFolder.Windows ) );

			return folder;
		}, isThreadSafe: true );

		[NotNull]
		public static readonly Lazy<Folder> WindowsSystem32Folder = new Lazy<Folder>( () => FindFolder( fullname: Path.Combine( path1: WindowsFolder.Value.FullName, path2: "System32" ) ), isThreadSafe: true );

	}

}