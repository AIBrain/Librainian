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
	using Exceptions;
	using Extensions;
	using FileSystem;
	using Logging;
	using Maths;
	using Measurement.Time;
	using Parsing;

	public static class Windows {

		private const String PATH = "PATH";

		public static readonly Char[] PathSeparator = {
			';'
		};

		public static readonly Lazy<Folder> WindowsFolder = new( () => {
			var folder = FluffFolder( Environment.GetFolderPath( Environment.SpecialFolder.Windows ) );

			if ( folder is null ) {
				throw new DirectoryNotFoundException( "Unable to locate Windows folder." );
			}

			return folder;
		}, true );

		public static readonly Lazy<Folder?> WindowsSystem32Folder = new( () => FluffFolder( Path.Combine( WindowsFolder.Value.FullPath, "System32" ) ), true );

		public static Lazy<Document?> CommandPrompt { get; } = new( () => FluffDocument( Path.Combine( WindowsSystem32Folder.Value.FullPath, "cmd.exe" ) ), true );

		/// <summary>Cleans and sorts the Windows <see cref="Environment" /> path variable.</summary>
		public static async Task CleanUpPath( Boolean reportToConsole, CancellationToken cancellationToken ) {
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

			foreach ( var pair in pathsData.Where( pair => !pair.Value.ExistsSync() ) ) {
				if ( pathsData.TryRemove( pair.Key, out var dummy ) && reportToConsole ) {
					$"Removing nonexistent folder `{dummy.FullPath}` from PATH".Info();
				}
			}

			foreach ( var pair in pathsData ) {
				if ( !await pair.Value.EnumerateFolders( "*", SearchOption.TopDirectoryOnly, cancellationToken ).AnyAsync( cancellationToken ).ConfigureAwait( false ) ) {
					if ( !await pair.Value.EnumerateDocuments( "*.*", cancellationToken ).AnyAsync( cancellationToken ).ConfigureAwait( false ) ) {
						if ( pathsData.TryRemove( pair.Key, out var dummy ) && reportToConsole ) {
							$"Removing empty folder {dummy.FullPath} from PATH".Info();
						}
					}
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

		public static Task<Process?> ExecuteCommandPromptAsync( String? arguments ) =>
			Task.Run( () => {
				try {
					var proc = new ProcessStartInfo {
						UseShellExecute = false,
						WorkingDirectory = WindowsSystem32Folder.Value.FullPath,
						FileName = CommandPrompt.Value.FullPath,
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
					exception.Log();
				}

				return default( Process? );
			} );

		public static Task<Boolean> ExecutePowershellCommandAsync( String? arguments = null, Boolean elevated = false ) =>
			Task.Run( () => {
				try {
					var startInfo = new ProcessStartInfo {
						UseShellExecute = false,

						//WorkingDirectory = PowerShellFolder.Value.FullPath,
						FileName = "powershell.exe",
						Verb = elevated ? "runas" : String.Empty, //demand elevated permissions?
						Arguments = $"-EncodedCommand {arguments.ToBase64()}",
						CreateNoWindow = false,
						ErrorDialog = true,
						WindowStyle = ProcessWindowStyle.Normal
					};

					$"Running PowerShell command '{arguments}'...".WriteLineColor( ConsoleColor.White, ConsoleColor.Green );

					var process = Process.Start( startInfo );

					if ( process == null ) {
						"failure.".Info();

						return false;
					}

					process.WaitForExit( ( Int32 )Minutes.One.ToSeconds().ToMilliseconds().Value );
					"success.".Info();

					return true;
				}
				catch ( Exception exception ) {
					exception.Log();
				}

				return false;
			} );

		public static Task<Process?> ExecuteProcessAsync( Document filename, Folder workingFolder, String? arguments, Boolean elevate ) {
			if ( filename == null ) {
				throw new ArgumentEmptyException( nameof( filename ) );
			}

			if ( workingFolder == null ) {
				throw new ArgumentEmptyException( nameof( workingFolder ) );
			}

			return Task.Run( () => {
				try {
					var processStartInfo = new ProcessStartInfo {
						UseShellExecute = false,
						WorkingDirectory = workingFolder.FullPath,
						FileName = filename.FullPath,
						Verb = elevate ? null : "runas", //demand elevated permissions
						Arguments = arguments ?? String.Empty,
						CreateNoWindow = false,
						ErrorDialog = true,
						WindowStyle = ProcessWindowStyle.Normal
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

		public static Document? FluffDocument( String fullname, String? okayMessage = null, String? errorMessage = null ) {
			if ( !String.IsNullOrEmpty( okayMessage ) ) {
				$"Finding {fullname}...".Info();
			}

			using var mainDocument = new Document( fullname );

			if ( mainDocument.GetExists() ) {
				okayMessage.Info();

				return mainDocument;
			}

			errorMessage.Error();

			return default( Document? );
		}

		public static Folder? FluffFolder( String fullname, String? okayMessage = null, String? errorMessage = null ) {
			if ( String.IsNullOrWhiteSpace( fullname ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fullname ) );
			}

			if ( !String.IsNullOrEmpty( okayMessage ) ) {
				$"Finding {fullname}...".Info();
			}

			var document = new Document( fullname );
			if ( document.ContainingingFolder() is Folder folder ) {
				return folder;
			}

			var mainFolder = new Folder( fullname );

			if ( mainFolder.GetExists() ) {
				if ( !String.IsNullOrEmpty( okayMessage ) ) {
					okayMessage.Info();
				}

				return mainFolder;
			}

			errorMessage.Error();

			return default( Folder? );
		}

		public static String GetCurrentPATH() => Environment.GetEnvironmentVariable( PATH, EnvironmentVariableTarget.Machine ) ?? String.Empty;

		public static Boolean IsServer() => GCSettings.IsServerGC;

		public static Boolean IsWorkStation() => !GCSettings.IsServerGC;

		public static Task<Boolean> MirrorFolderStructureAsync( Folder folder, Folder baseFolder ) =>
			ExecutePowershellCommandAsync( $"xcopy.exe \"{folder.FullPath}\" \"{baseFolder.FullPath}\" /E /T" );

		public static Process? OpenWithExplorer( String? value ) {
			try {

				//Verb = "runas", //demand elevated permissions
				var proc = new ProcessStartInfo {
					UseShellExecute = false,
					WorkingDirectory = Environment.CurrentDirectory,
					FileName = Path.Combine( WindowsSystem32Folder.Value.FullPath, "explorer.exe" ),
					Arguments = $" /separate /select,\"{value}\" ",
					CreateNoWindow = false,
					ErrorDialog = true,
					WindowStyle = ProcessWindowStyle.Normal
				};

				$"Running command '{proc.Arguments}'...".WriteLineColor( ConsoleColor.White, ConsoleColor.Cyan );

				return Process.Start( proc );
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default( Process? );
		}

		public static void Yield() {
			if ( Randem.NextBoolean() ) {
				Thread.Yield();
			}
		}

		public static class Utilities {

			public static Lazy<Document?> IrfanView64 { get; } =
				new( () => FluffDocument( Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles ) + @"\IrfanView\", "i_view64.exe" ) ), true );

			public static async Task<Process?>? TryConvert_WithIrfanviewAsync( Document inDocument, Document outDocument ) {
				if ( inDocument == null ) {
					throw new ArgumentEmptyException( nameof( inDocument ) );
				}

				if ( outDocument == null ) {
					throw new ArgumentEmptyException( nameof( outDocument ) );
				}

				var irfan = IrfanView64.Value;
				if ( irfan is null ) {
					return default( Process? );
				}

				if ( await irfan.Exists( CancellationToken.None ).ConfigureAwait( false ) != true ) {
					return default( Process? );
				}

				try {
					var arguments = $" {inDocument.FullPath.Quoted()} /convert={outDocument.FullPath.Quoted()} ";

					var proc = new ProcessStartInfo {
						UseShellExecute = false,
						WorkingDirectory = Folder.GetTempFolder().FullPath,
						FileName = irfan.FullPath,

						//Verb = "runas", //demand elevated permissions
						Arguments = arguments,
						CreateNoWindow = true,
						ErrorDialog = false,
						WindowStyle = ProcessWindowStyle.Normal
					};

					$"Running irfanview command '{proc.Arguments}'...".Info();

					return Process.Start( proc );
				}
				catch ( Exception exception ) {
					exception.Log();
				}

				return default( Process? );
			}
		}
	}
}