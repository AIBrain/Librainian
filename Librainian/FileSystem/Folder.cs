// Copyright � Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
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
// File "Folder.cs" last touched on 2021-03-07 at 9:14 AM by Protiguous.

namespace Librainian.FileSystem {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Security;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Threading.Tasks;
	using ComputerSystem.Devices;
	using Extensions;
	using JetBrains.Annotations;
	using Logging;
	using Newtonsoft.Json;
	using OperatingSystem;
	using Parsing;
	using PooledAwait;
	using Pri.LongPath;
	using Path = System.IO.Path;

	[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
	[JsonObject]
	[Immutable]
	[Serializable]
	public class Folder : IFolder {

		[DebuggerStepThrough]
		public static Boolean TryGetFolderFromPath( TrimmedString path, [CanBeNull] out DirectoryInfo? directoryInfo, [CanBeNull] out Uri? uri ) => TryGetFolderFromPath( path.Value, out directoryInfo, out uri );

		[DebuggerStepThrough]
		public static Boolean TryGetFolderFromPath( [CanBeNull] String? path, [CanBeNull] out DirectoryInfo? directoryInfo, [CanBeNull] out Uri? uri ) {
			directoryInfo = null;
			uri = null;

			try {
				if ( String.IsNullOrWhiteSpace( path ) ) {
					return false;
				}

				if ( Uri.TryCreate( path, UriKind.Absolute, out uri ) ) {
					directoryInfo = new DirectoryInfo( uri.LocalPath );

					return true;
				}

				directoryInfo = new DirectoryInfo( path ); //try it anyways

				return true;
			}
			catch ( ArgumentException ) { }
			catch ( UriFormatException ) { }
			catch ( SecurityException ) { }
			catch ( PathTooLongException ) { }
			catch ( InvalidOperationException ) { }

			return false;
		}

		private Byte? _levelsDeep;

		/// <summary></summary>
		/// <param name="fullPath"></param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( String? fullPath ) {
			if ( String.IsNullOrWhiteSpace( fullPath ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fullPath ) );
			}

			if ( String.IsNullOrWhiteSpace( fullPath = CleanPath( fullPath ) ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fullPath ) );
			}

			/*
			if ( !TryGetFolderFromPath( fullPath, out DirectoryInfo? directoryInfo, out var _ ) ) {
				throw new InvalidOperationException( $"Unable to parse a valid path from `{fullPath}`" );
			}
			*/

			var directoryInfo = new DirectoryInfo( fullPath );

			this.Info = directoryInfo ?? throw new InvalidOperationException( $"Unable to parse a valid path from `{fullPath}`" );
		}

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( Environment.SpecialFolder specialFolder ) : this( Environment.GetFolderPath( specialFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( Environment.SpecialFolder specialFolder, [NotNull] String subFolder ) : this( Environment.GetFolderPath( specialFolder ).CombinePaths( subFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( Environment.SpecialFolder specialFolder, [CanBeNull] String? applicationName, [NotNull] String subFolder ) : this(
			Environment.GetFolderPath( specialFolder ).CombinePaths( applicationName ?? AppDomain.CurrentDomain.FriendlyName, subFolder ) ) { }

		/// <summary>
		///     <para>Pass null to automatically fill in <paramref name="companyName" /> and <paramref name="applicationName" /> .</para>
		/// </summary>
		/// <param name="specialFolder">  </param>
		/// <param name="companyName">    </param>
		/// <param name="applicationName"></param>
		/// <param name="subFolders"></param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		[DebuggerStepThrough]
		public Folder( Environment.SpecialFolder specialFolder, [CanBeNull] String? companyName, [CanBeNull] String? applicationName, [NotNull] params String[] subFolders ) :
			this( Environment.GetFolderPath( specialFolder ).CombinePaths( companyName ?? throw new InvalidOperationException( $"Empty {nameof( companyName )}." ),
				applicationName ?? throw new InvalidOperationException( $"Empty {nameof( applicationName )}." ), subFolders.ToStrings( @"\" ) ) ) { }

		[DebuggerStepThrough]
		public Folder( Environment.SpecialFolder specialFolder, [NotNull] params String[] subFolders ) : this( Environment.GetFolderPath( specialFolder )
			.CombinePaths( subFolders.Select( fullpath => CleanPath( fullpath ) ).ToStrings( FolderSeparatorChar ) ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		[DebuggerStepThrough]
		public Folder( [NotNull] String fullPath, [NotNull] String subFolder ) : this( fullPath.CombinePaths( subFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		[DebuggerStepThrough]
		public Folder( [NotNull] IFolder folder, [NotNull] String subFolder ) : this( folder.FullPath.CombinePaths( subFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		[DebuggerStepThrough]
		public Folder( [NotNull] IDocument document, [NotNull] String subFolder ) : this( document.ContainingingFolder().FullPath.CombinePaths( subFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		[DebuggerStepThrough]
		public Folder( [NotNull] FileSystemInfo fileSystemInfo ) : this( fileSystemInfo.FullName ) { }

		/// <summary>
		///     String of invalid characters in a path or filename.
		/// </summary>
		[NotNull]
		private static String InvalidPathCharacters { get; } = new(Path.GetInvalidPathChars()); //BUG Will the '\0' create a partially null-string?

		[NotNull]
		private static Regex RegexForInvalidPathCharacters { get; } = new($"[{Regex.Escape( InvalidPathCharacters )}]", RegexOptions.Compiled);

		/// <summary>"/"</summary>
		[JsonIgnore]
		[NotNull]
		public static String FolderAltSeparator { get; } = new(new[] {
			Path.AltDirectorySeparatorChar
		});

		/// <summary>"\"</summary>
		[JsonIgnore]
		[NotNull]
		public static String FolderSeparator { get; } = new(new[] {
			Path.DirectorySeparatorChar
		});

		[JsonIgnore]
		public static Char FolderSeparatorChar { get; } = Path.DirectorySeparatorChar;

		/// <summary>The <see cref="IFolder" /> class is built around <see cref="DirectoryInfo" />.</summary>
		[JsonProperty]
		[NotNull]
		public DirectoryInfo Info { get; }

		[JsonIgnore]
		[NotNull]
		public String FullPath => this.Info.FullName;

		[JsonIgnore]
		[NotNull]
		public String Name => this.Info.Name;

		/// <summary>
		///     <para>Returns True if the folder exists.</para>
		/// </summary>
		/// <returns></returns>
		/// See also:
		/// <see cref="Delete"></see>
		public async PooledValueTask<Boolean> Create( CancellationToken cancellationToken ) {
			try {
				if ( await this.Exists( cancellationToken ).ConfigureAwait( false ) ) {
					return true;
				}

				try {
					var parent = new Folder( this.Info.Parent.FullName );

					if ( !await parent.Exists( cancellationToken ).ConfigureAwait( false ) ) {
						await parent.Create( cancellationToken ).ConfigureAwait( false );
					}
				}
				catch ( Exception exception ) {
					exception.Log();
				}

				this.Info.Create();

				return await this.Exists( cancellationToken ).ConfigureAwait( false );
			}
			catch ( IOException ) {
				return false;
			}
		}

		/// <summary>
		///     <para>Returns True if the folder no longer exists.</para>
		/// </summary>
		/// <returns></returns>
		/// <see cref="Create"></see>
		public async PooledValueTask<Boolean> Delete( CancellationToken cancellationToken ) {
			try {
				if ( await this.IsEmpty( cancellationToken ).ConfigureAwait( false ) ) {
					this.Info.Delete();
				}

				return !await this.Exists( cancellationToken ).ConfigureAwait( false );
			}
			catch ( IOException ) { }

			return false;
		}

		public Boolean Equals( IFolder? other ) => Equals( this, other );

		/// <summary>Returns true if the <see cref="IFolder" /> currently exists.</summary>
		/// <exception cref="IOException"></exception>
		/// <exception cref="SecurityException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		public async PooledValueTask<Boolean> Exists( CancellationToken cancellationToken ) {
			await this.Refresh( cancellationToken ).ConfigureAwait( false );

			return this.Info.Exists;
		}

		/// <summary>Returns true if the <see cref="IFolder" /> currently exists.</summary>
		/// <exception cref="IOException"></exception>
		/// <exception cref="SecurityException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		public Boolean ExistsSync() {
			this.Info.Refresh();

			return this.Info.Exists;
		}

		/// <summary>Free space available to the current user.</summary>
		/// <returns></returns>
		public PooledValueTask<UInt64> GetAvailableFreeSpace() => new(( UInt64 )new DriveInfo( this.GetDrive().ToString() ).AvailableFreeSpace);

		/// <summary>
		///     <para>Returns an enumerable collection of <see cref="Document" /> in the current directory.</para>
		/// </summary>
		/// <param name="searchPattern"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[NotNull]
		public async IAsyncEnumerable<Document> EnumerateDocuments( [CanBeNull] String? searchPattern, [EnumeratorCancellation] CancellationToken cancellationToken ) {
			searchPattern = searchPattern.NullIfEmptyOrWhiteSpace() ?? "*.*";

			var searchPath = this.FullPath.CombinePaths( searchPattern );

			var findData = default( WIN32_FIND_DATA );

			var hFindFile = default( NativeMethods.SafeFindHandle );

			try {
				hFindFile = await Task.Run( () => PriNativeMethods.FindFirstFile( searchPath, out findData ), cancellationToken ).ConfigureAwait( false );
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			var more = false;

			do {
				if ( cancellationToken.IsCancellationRequested ) {
					break;
				}

				if ( hFindFile?.IsInvalid != false ) {
					//BUG or == true ?
					break;
				}

				if ( findData.IsParentOrCurrent() || findData.IsReparsePoint() || !findData.IsFile() ) {
					continue;
				}

				if ( findData.cFileName != null ) {
					yield return new Document( this, findData.cFileName );
				}

				try {
					more = await Task.Run( () => hFindFile.FindNextFile( out findData ), cancellationToken ).ConfigureAwait( false );
				}
				catch ( Exception exception ) {
					exception.Log( false );
				}
			} while ( more );
		}

		[NotNull]
		public async IAsyncEnumerable<Document> EnumerateDocuments( [NotNull] IEnumerable<String> searchPatterns, [EnumeratorCancellation] CancellationToken cancelToken ) {
			foreach ( var searchPattern in searchPatterns ) {
				await foreach ( var document in this.EnumerateDocuments( searchPattern, cancelToken ) ) {
					yield return document;
				}
			}
		}

		[NotNull]
		public Disk GetDrive() => new(this.Info.Root.FullName);

		public override Int32 GetHashCode() => this.FullPath.GetHashCode();

		[CanBeNull]
		public IFolder GetParent() => new Folder( this.Info.Parent );

		/// <summary>
		///     <para>Check if this <see cref="Folder" /> contains any <see cref="Folder" /> or any <see cref="Document" /> .</para>
		/// </summary>
		/// <returns></returns>
		public async PooledValueTask<Boolean> IsEmpty( CancellationToken cancellationToken ) =>
			!await this.EnumerateFolders( "*.*", SearchOption.TopDirectoryOnly, cancellationToken ).AnyAsync( cancellationToken ).ConfigureAwait( false ) &&
			!await this.EnumerateDocuments( "*.*", cancellationToken ).AnyAsync( cancellationToken ).ConfigureAwait( false );

		public void OpenWithExplorer() {
			using var _ = Windows.OpenWithExplorer( this.FullPath );
		}

		public PooledValueTask<DirectoryInfo> Refresh( CancellationToken cancellationToken ) {
			this.Info.Refresh();
			return new PooledValueTask<DirectoryInfo>( this.Info );
		}

		/// <summary>
		///     <para>Shorten the full path with "..."</para>
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public String ToCompactFormat() {
			var sb = new StringBuilder();

			NativeMethods.PathCompactPathEx( sb, this.FullPath, this.FullPath.Length, 0 ); //TODO untested. //HACK may be buggy on extensions also

			return sb.ToString();
		}

		/// <summary>Returns a String that represents the current object.</summary>
		/// <returns>A String that represents the current object.</returns>
		[NotNull]
		public override String ToString() => this.FullPath;

		/// <summary>
		///     No guarantee of return order. Also, because of the way the operating system works, a <see cref="Folder" /> can
		///     be created or deleted after a search.
		/// </summary>
		/// <param name="searchPattern"></param>
		/// <param name="searchOption"> Defaults to <see cref="SearchOption.AllDirectories" /></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[NotNull]
		public async IAsyncEnumerable<Folder> EnumerateFolders(
			[CanBeNull] String? searchPattern,
			SearchOption searchOption,
			[EnumeratorCancellation] CancellationToken cancellationToken
		) {
			searchPattern ??= "*";

			var searchPath = this.FullPath.CombinePaths( searchPattern );

			var findData = default( WIN32_FIND_DATA );

			var hFindFile = default( NativeMethods.SafeFindHandle );

			try {
				hFindFile = await Task.Run( () => PriNativeMethods.FindFirstFile( searchPath, out findData ), cancellationToken ).ConfigureAwait( false );
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			var more = true;

			do {
				if ( cancellationToken.IsCancellationRequested ) {
					break;
				}

				if ( hFindFile?.IsInvalid != false ) {
					//BUG or == true ?
					break;
				}

				if ( findData.IsDirectory() && !findData.IsParentOrCurrent() && !findData.IsReparsePoint() && !findData.IsIgnoreFolder() ) {
					if ( findData.cFileName != null ) {
						// Fix with @"\\?\" +System.IO.PathTooLongException?
						if ( findData.cFileName.Length > PriNativeMethods.MAX_PATH ) {
							$"Found subfolder with length longer than {PriNativeMethods.MAX_PATH}. Debug and see if it works.".BreakIfDebug( "poor man's debug" );
							//continue; //BUG Needs unit tested for long paths.
						}

						var subFolder = new Folder( this, findData.cFileName );

						yield return subFolder;

						switch ( searchOption ) {
							case SearchOption.AllDirectories: {
								await foreach ( var info in subFolder.EnumerateFolders( searchPattern, searchOption, cancellationToken ) ) {
									yield return info;
								}

								break;
							}

							case SearchOption.TopDirectoryOnly: {
								break;
							}
							default: {
								throw new ArgumentOutOfRangeException( nameof( searchOption ), searchOption, null );
							}
						}
					}
				}

				try {
					more = await Task.Run( () => hFindFile.FindNextFile( out findData ), cancellationToken ).ConfigureAwait( false );
				}
				catch ( Exception exception ) {
					exception.Log( false );
				}
			} while ( more );
		}

		/// <summary>
		///     Synchronous version.
		/// </summary>
		/// <returns></returns>
		public Boolean GetExists() {
			this.Info.Refresh();
			return this.Info.Exists;
		}

		/// <summary>
		///     Returns the path with any invalid characters replaced with <paramref name="replacement" /> and then
		///     <see cref="String.Trim()" /> the result.
		///     (Defaults to <see cref="String.Empty" /> />.)
		/// </summary>
		/// <param name="fullpath"></param>
		/// <param name="replacement"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[NotNull]
		public static String CleanPath( [NotNull] String fullpath, [CanBeNull] String? replacement = null ) {
			if ( fullpath is null ) {
				throw new ArgumentNullException( nameof( fullpath ) );
			}

			var path = RegexForInvalidPathCharacters.Replace( fullpath, replacement ?? String.Empty ).Trim();

			/*
			while ( path.Right( 1 ) == FolderSeparator ) {
				path = path?.Left( ( UInt32 )( path.Length - 1 ) );
			}
			*/

			return path;
		}

		///// <summary>
		/////     <para>
		/////         Pass null to automatically fill in <paramref name="companyName" /> and
		/////         <paramref name="applicationName" /> .
		/////     </para>
		///// </summary>
		///// <param name="specialFolder"></param>
		///// <param name="companyName"></param>
		///// <param name="applicationName"></param>
		///// <param name="subFolder"></param>
		///// <param name="subSubfolder"></param>
		//public Folder( Environment.SpecialFolder specialFolder, String companyName, String applicationName, String subFolder, String subSubfolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder, subSubfolder ) ) {
		//}
		///// <summary>
		/////     <para>
		/////         Pass null to automatically fill in <paramref name="companyName" /> and
		/////         <paramref name="applicationName" /> .
		/////     </para>
		///// </summary>
		///// <param name="specialFolder"></param>
		///// <param name="companyName"></param>
		///// <param name="applicationName"></param>
		///// <param name="subFolder"></param>
		//public Folder( Environment.SpecialFolder specialFolder, String companyName, String applicationName, String subFolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder ) ) {
		//}
		/// <summary>
		///     <para>Static comparison of the folder names (case sensitive) for equality.</para>
		///     <para>
		///         To compare the path of two <see cref="IFolder" /> use
		///         <param name="left">todo: describe left parameter on Equals</param>
		///         <param name="right">todo: describe right parameter on Equals</param>
		///         <seealso /> .
		///     </para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] IFolder? left, [CanBeNull] IFolder? right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return false;
			}

			return left.FullPath.Is( right.FullPath );
		}

		/// <summary>Throws Exception if unable to obtain the Temp path.</summary>
		/// <returns></returns>
		[NotNull]
		public static IFolder GetTempFolder() => new Folder( Path.GetTempPath() );

		[NotNull]
		public static implicit operator DirectoryInfo( [NotNull] Folder folder ) => folder.Info;

		/// <summary>Opens a folder in file explorer.</summary>
		public static void OpenWithExplorer( [CanBeNull] IFolder folder ) {
			if ( folder is null ) {
				throw new ArgumentNullException( nameof( folder ) );
			}

			var windowsFolder = Environment.GetFolderPath( Environment.SpecialFolder.Windows );

			Process.Start( $@"{windowsFolder}\explorer.exe", $"/e,\"{folder.FullPath}\"" );
		}

		public static Boolean TryParse( [CanBeNull] String? path, [CanBeNull] out IFolder? folder ) {
			folder = null;

			try {
				if ( String.IsNullOrWhiteSpace( path ) ) {
					return false;
				}

				path = CleanPath( path );

				if ( String.IsNullOrEmpty( path ) ) {
					return false;
				}

				DirectoryInfo dirInfo;

				if ( Uri.TryCreate( path, UriKind.Absolute, out var uri ) ) {
					dirInfo = new DirectoryInfo( uri.LocalPath );
					folder = new Folder( dirInfo );

					return true;
				}

				dirInfo = new DirectoryInfo( path ); //try it anyways
				folder = new Folder( dirInfo ); //eh? //TODO

				return true;
			}
			catch ( ArgumentException ) { }
			catch ( UriFormatException ) { }
			catch ( SecurityException ) { }
			catch ( PathTooLongException ) { }
			catch ( InvalidOperationException ) { }

			return false;
		}

		public Boolean Explore() => this.Info.OpenWithExplorer();

		/// <summary>
		///     <see cref="op_Implicit" />
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public DirectoryInfo ToDirectoryInfo() => this;

		/// <summary>
		///     Return how many [sub]folders are in this folder's path.
		/// </summary>
		/// <returns></returns>
		public Byte LevelsDeep() {
			this._levelsDeep ??= ( Byte? )this.FullPath.Count( c => c == FolderSeparatorChar );

			return this._levelsDeep.Value;
		}

	}

}