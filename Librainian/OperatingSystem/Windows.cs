// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Windows.cs" last formatted on 2020-08-14 at 8:41 PM.

#nullable enable
namespace Librainian.OperatingSystem {

	using System;
	using System.Collections.Concurrent;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Runtime;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Librainian.Extensions;
	using Librainian.FileSystem;
	using Logging;
	using Maths;
	using Measurement.Time;
	using Parsing;

	public static class Windows {

		private const String PATH = "PATH";

		public static readonly Char[] PathSeparator = {
			';'
		};

		[NotNull]
		public static readonly Lazy<Folder> WindowsFolder = new Lazy<Folder>( () => {
			var folder = FindFolder( Environment.GetFolderPath( Environment.SpecialFolder.Windows ) );

			if ( folder is null ) {
				throw new DirectoryNotFoundException( "Unable to locate Windows folder." );
			}

			return folder;
		}, true );

		[NotNull]
		public static readonly Lazy<Folder?> WindowsSystem32Folder = new Lazy<Folder?>( () => FindFolder( Path.Combine( WindowsFolder.Value.FullPath, "System32" ) ), true );

		[NotNull]
		public static Lazy<Document?> CommandPrompt { get; } =
			new Lazy<Document?>( () => FindDocument( Path.Combine( WindowsSystem32Folder.Value.FullPath, "cmd.exe" ) ), true );

		[NotNull]
		public static Lazy<Document?> IrfanView64 { get; } =
			new Lazy<Document?>( () => FindDocument( Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles ) + @"\IrfanView\", "i_view64.exe" ) ),
								 true );

		/// <summary>Cleans and sorts the Windows <see cref="Environment" /> path variable.</summary>
		/// <returns></returns>
		public static void CleanUpPath( Boolean reportToConsole = false ) {
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

			var justpaths = currentPath.Split( PathSeparator, StringSplitOptions.RemoveEmptyEntries ).ToHashSet();

			if ( reportToConsole ) {
				$"Found PATH list with {justpaths.Count} entries.".Info();
			}

			var pathsData = new ConcurrentDictionary<String, Folder>( Environment.ProcessorCount, justpaths.Count );

			foreach ( var s in justpaths ) {
				pathsData[s] = new Folder( s );
			}

			if ( reportToConsole ) {
				"Examining entries...".Info();
			}

			foreach ( var pair in pathsData.Where( pair => !pair.Value.Exists() ) ) {
				if ( pathsData.TryRemove( pair.Key, out var dummy ) && reportToConsole ) {
					$"Removing nonexistent folder `{dummy.FullPath}` from PATH".Info();
				}
			}

			foreach ( var pair in pathsData.Where( pair => !pair.Value.GetFolders( "*" ).Any() && !pair.Value.GetDocuments().Any() ) ) {
				if ( pathsData.TryRemove( pair.Key, out var dummy ) && reportToConsole ) {
					$"Removing empty folder {dummy.FullPath} from PATH".Info();
				}
			}

			if ( reportToConsole ) {
				"Rebuilding PATH entries...".Info();
			}

			var rebuiltPath = pathsData.Values.OrderByDescending( info => info.FullPath.Length ).Select( info => info.FullPath ).ToStrings( ";" );

			if ( reportToConsole ) {
				"Applying new PATH entries...".Info();
			}

			Environment.SetEnvironmentVariable( PATH, rebuiltPath, EnvironmentVariableTarget.Machine );
		}

		[NotNull]
		public static Task<Process> ExecuteCommandPromptAsync( [CanBeNull] String? arguments ) =>
			Task.Run( () => {
				try {
					var proc = new ProcessStartInfo {
						UseShellExecute = false, WorkingDirectory = WindowsSystem32Folder.Value.FullPath, FileName = CommandPrompt.Value.FullPath,
						Verb = "runas", //demand elevated permissions
						Arguments = $"/C \"{arguments}\"", CreateNoWindow = false, ErrorDialog = true, WindowStyle = ProcessWindowStyle.Normal
					};

					$"Running command '{proc.Arguments}'...".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );

					return Process.Start( proc );
				}
				catch ( Exception exception ) {
					exception.Log();
				}

				return default( Process? );
			} );

		[NotNull]
		public static Task<Boolean> ExecutePowershellCommandAsync( [CanBeNull] String? arguments = null, Boolean elevated = false ) =>
			Task.Run( () => {
				try {
					var startInfo = new ProcessStartInfo {
						UseShellExecute = false,

						//WorkingDirectory = PowerShellFolder.Value.FullPath,
						FileName = "powershell.exe", Verb = elevated ? "runas" : null, //demand elevated permissions?
						Arguments = $"-EncodedCommand {arguments.ToBase64()}", CreateNoWindow = false, ErrorDialog = true, WindowStyle = ProcessWindowStyle.Normal
					};

					$"Running PowerShell command '{arguments}'...".WriteLineColor( ConsoleColor.White, ConsoleColor.Green );

					var process = Process.Start( startInfo );

					if ( null == process ) {
						"failure.".Info();

						return default( Boolean );
					}

					process.WaitForExit( ( Int32 )Minutes.One.ToSeconds().ToMilliseconds().Value );
					"success.".Info();

					return true;
				}
				catch ( Exception exception ) {
					exception.Log();
				}

				return default( Boolean );
			} );

		[CanBeNull]
		public static Task<Process?> ExecuteProcessAsync( [NotNull] Document filename, [NotNull] Folder workingFolder, [CanBeNull] String? arguments, Boolean elevate ) {
			if ( filename == null ) {
				throw new ArgumentNullException( nameof( filename ) );
			}

			if ( workingFolder == null ) {
				throw new ArgumentNullException( nameof( workingFolder ) );
			}

			return Task.Run( () => {
				try {
					var processStartInfo = new ProcessStartInfo {
						UseShellExecute = false, WorkingDirectory = workingFolder.FullPath, FileName = filename.FullPath,
						Verb = elevate ? null : "runas", //demand elevated permissions
						Arguments = arguments ?? String.Empty, CreateNoWindow = false, ErrorDialog = true, WindowStyle = ProcessWindowStyle.Normal
					};

					$"Running process '{filename} {processStartInfo.Arguments}'...".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );

					var bob = Process.Start( processStartInfo );

					if ( bob != null ) {
						return bob;
					}
				}
				catch ( Exception exception ) {
					exception.Log();
				}

				return default( Process? );
			} );
		}

		[CanBeNull]
		public static Document? FindDocument( [NotNull] String fullname, [CanBeNull] String? okayMessage = null, [CanBeNull] String? errorMessage = null ) {
			if ( !String.IsNullOrEmpty( okayMessage ) ) {
				$"Finding {fullname}...".Info();
			}

			using var mainDocument = new Document( fullname );

			if ( mainDocument.Exists() ) {
				okayMessage.Info();

				return mainDocument;
			}

			errorMessage.Error();

			return default( Document? );
		}

		[CanBeNull]
		public static Folder? FindFolder( [NotNull] String fullname, [CanBeNull] String? okayMessage = null, [CanBeNull] String? errorMessage = null ) {
			if ( String.IsNullOrWhiteSpace( fullname ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fullname ) );
			}

			if ( !String.IsNullOrEmpty( okayMessage ) ) {
				$"Finding {fullname}...".Info();
			}

			var mainFolder = new Folder( fullname );

			if ( !mainFolder.Exists() ) {
				errorMessage.Error();

				return default( Folder? );
			}

			if ( !String.IsNullOrEmpty( okayMessage ) ) {
				okayMessage.Info();
			}

			return mainFolder;
		}

		[NotNull]
		public static String GetCurrentPATH() => Environment.GetEnvironmentVariable( PATH, EnvironmentVariableTarget.Machine ) ?? String.Empty;

		public static Boolean IsServer() => GCSettings.IsServerGC;

		public static Boolean IsWorkStation() => !GCSettings.IsServerGC;

		[NotNull]
		public static Task<Boolean> MirrorFolderStructureAsync( [NotNull] Folder folder, [NotNull] Folder baseFolder ) =>
			ExecutePowershellCommandAsync( $"xcopy.exe \"{folder.FullPath}\" \"{baseFolder.FullPath}\" /E /T" );

		[CanBeNull]
		public static Process? OpenWithExplorer( [CanBeNull] String? value ) {
			try {
				//Verb = "runas", //demand elevated permissions
				var proc = new ProcessStartInfo {
					UseShellExecute = false, WorkingDirectory = Environment.CurrentDirectory, FileName = Path.Combine( WindowsSystem32Folder.Value.FullPath, "explorer.exe" ),
					Arguments = $" /separate /select,\"{value}\" ", CreateNoWindow = false, ErrorDialog = true, WindowStyle = ProcessWindowStyle.Normal
				};

				$"Running command '{proc.Arguments}'...".WriteLineColor( ConsoleColor.White, ConsoleColor.Cyan );

				return Process.Start( proc );
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default( Process? );
		}

		[CanBeNull]
		public static Task<Process?> TryConvert_WithIrfanviewAsync( [NotNull] Document inDocument, [NotNull] Document outDocument ) {
			if ( inDocument == null ) {
				throw new ArgumentNullException( nameof( inDocument ) );
			}

			if ( outDocument == null ) {
				throw new ArgumentNullException( nameof( outDocument ) );
			}

			return Task.Run( () => {
				if ( IrfanView64.Value?.Exists() != true ) {
					return default( Process? );
				}

				try {
					var arguments = $" {inDocument.FullPath.Quoted()} /convert={outDocument.FullPath.Quoted()} ";

					var proc = new ProcessStartInfo {
						UseShellExecute = false, WorkingDirectory = Folder.GetTempFolder().FullPath, FileName = IrfanView64.Value.FullPath,

						//Verb = "runas", //demand elevated permissions
						Arguments = arguments, CreateNoWindow = true, ErrorDialog = false, WindowStyle = ProcessWindowStyle.Normal
					};

					$"Running irfanview command '{proc.Arguments}'...".Info();

					return Process.Start( proc );
				}
				catch ( Exception exception ) {
					exception.Log();
				}

				return default( Process? );
			} );
		}

		public static void Yield() {
			if ( Randem.NextBoolean() ) {
				Thread.Yield();
			}
		}

	}

}