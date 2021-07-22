// Copyright © Protiguous. All Rights Reserved.
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
// File "Folder.cs" last touched on 2021-07-14 at 4:41 AM by Protiguous.

namespace Librainian.FileSystem {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Security;
	using System.Security.AccessControl;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Threading.Tasks;
	using ComputerSystem.Devices;
	using Exceptions;
	using Extensions;
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

		private UInt16? _levelsDeep;

		/// <summary></summary>
		/// <param name="fullPath"></param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( String? fullPath ) {
			if ( String.IsNullOrWhiteSpace( fullPath = CleanPath( fullPath ) ) ) {
				throw new NullException( nameof( fullPath ) );
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
		public Folder( Environment.SpecialFolder specialFolder, String subFolder ) : this( Environment.GetFolderPath( specialFolder ).CombinePaths( subFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( Environment.SpecialFolder specialFolder, String? applicationName, String subFolder ) : this( Environment.GetFolderPath( specialFolder )
			.CombinePaths( applicationName ?? AppDomain.CurrentDomain.FriendlyName, subFolder ) ) { }

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
		public Folder( Environment.SpecialFolder specialFolder, String? companyName, String? applicationName, params String[] subFolders ) : this( Environment
			.GetFolderPath( specialFolder )
			.CombinePaths( companyName ?? throw new InvalidOperationException( $"Empty {nameof( companyName )}." ),
				applicationName ?? throw new InvalidOperationException( $"Empty {nameof( applicationName )}." ), subFolders.ToStrings( @"\" ) ) ) { }

		[DebuggerStepThrough]
		public Folder( Environment.SpecialFolder specialFolder, params String[] subFolders ) : this( Environment.GetFolderPath( specialFolder )
		                                                                                                        .CombinePaths( subFolders
			                                                                                                        .Select( fullpath => CleanPath( fullpath ) )
			                                                                                                        .ToStrings( FolderSeparatorChar ) ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		[DebuggerStepThrough]
		public Folder( String fullPath, String subFolder ) : this( fullPath.CombinePaths( subFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		[DebuggerStepThrough]
		public Folder( IFolder folder, String subFolder ) : this( folder.FullPath.CombinePaths( subFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		[DebuggerStepThrough]
		public Folder( IDocument document, String subFolder ) : this( document.ContainingingFolder().FullPath.CombinePaths( subFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		[DebuggerStepThrough]
		public Folder( FileSystemInfo fileSystemInfo ) : this( fileSystemInfo.FullName ) { }

		/// <summary>
		///     String of invalid characters in a path or filename.
		/// </summary>
		private static String InvalidPathCharacters { get; } = new(Path.GetInvalidPathChars());

		private static Regex RegexForInvalidPathCharacters { get; } = new($"[{Regex.Escape( InvalidPathCharacters )}]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		//BUG Will the '\0' create a partially null-string?
		/// <summary>"/"</summary>
		[JsonIgnore]
		public static String FolderAltSeparator { get; } = new(new[] {
			Path.AltDirectorySeparatorChar
		});

		/// <summary>"\"</summary>
		[JsonIgnore]
		public static String FolderSeparator { get; } = new(new[] {
			Path.DirectorySeparatorChar
		});

		[JsonIgnore]
		public static Char FolderSeparatorChar { get; } = Path.DirectorySeparatorChar;

		[JsonIgnore]
		public String FullPath => this.Info.FullName;

		/// <summary>The <see cref="IFolder" /> class is built around <see cref="DirectoryInfo" />.</summary>
		[JsonProperty]
		public DirectoryInfo Info { get; }

		[JsonIgnore]
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

		/// <summary>
		///     <para>Returns an enumerable collection of <see cref="Document" /> in the current directory.</para>
		/// </summary>
		/// <param name="searchPattern"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async IAsyncEnumerable<Document> EnumerateDocuments( String? searchPattern, [EnumeratorCancellation] CancellationToken cancellationToken ) {
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

		public async IAsyncEnumerable<Document> EnumerateDocuments( IEnumerable<String> searchPatterns, [EnumeratorCancellation] CancellationToken cancelToken ) {
			foreach ( var searchPattern in searchPatterns ) {
				await foreach ( var document in this.EnumerateDocuments( searchPattern, cancelToken ) ) {
					yield return document;
				}
			}
		}

		/// <summary>
		///     No guarantee of return order. Also, because of the way the operating system works, a <see cref="Folder" /> can
		///     be created or deleted after a search.
		/// </summary>
		/// <param name="searchPattern"></param>
		/// <param name="searchOption"> Defaults to <see cref="SearchOption.AllDirectories" /></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async IAsyncEnumerable<Folder> EnumerateFolders(
			String? searchPattern,
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

		public Disk GetDrive() => new(this.Info.Root.FullName);

		/// <summary>
		///     Synchronous version.
		/// </summary>
		/// <returns></returns>
		public Boolean GetExists() {
			this.Info.Refresh();
			return this.Info.Exists;
		}

		public override Int32 GetHashCode() => this.FullPath.GetHashCode();

		public IFolder GetParent() => new Folder( this.Info.Parent ?? throw new NullException( nameof( this.Info.Parent ) ) );

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
			if ( cancellationToken.IsCancellationRequested ) {
				return default( PooledValueTask<DirectoryInfo> );
			}

			this.Info.Refresh();
			if ( cancellationToken.IsCancellationRequested ) {
				return default( PooledValueTask<DirectoryInfo> );
			}

			return new PooledValueTask<DirectoryInfo>( this.Info );
		}

		/// <summary>
		///     <para>Shorten the full path with "..."</para>
		/// </summary>
		/// <returns></returns>
		public String ToCompactFormat() {
			var length = this.FullPath.Length;
			var sb = new StringBuilder( length, length );

			NativeMethods.PathCompactPathEx( sb, this.FullPath, length, 0 );

			return sb.ToString();
		}

		/// <summary>Returns a String that represents the current object.</summary>
		/// <returns>A String that represents the current object.</returns>
		public override String ToString() => this.FullPath;

		/// <summary>
		///     Returns the path with any invalid characters replaced with <paramref name="replacement" /> and then
		///     <see cref="String.Trim()" /> the result.
		///     <para>Passing in a null string will return <see cref="String.Empty" /></para>
		///     <para>Defaults to <see cref="String.Empty" /> />.</para>
		/// </summary>
		/// <param name="fullpath"></param>
		/// <param name="replacement"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static String CleanPath( String? fullpath, String? replacement = null ) {
			if ( fullpath is null ) {
				return String.Empty;
			}

			var path = RegexForInvalidPathCharacters.Replace( fullpath, replacement ?? String.Empty ).Trim();

			CouldBeMore:
			while ( path.EndsWith( FolderSeparator, StringComparison.OrdinalIgnoreCase ) ) {
				path = path.RemoveLastCharacter();
			}

			if ( path.EndsWith( FolderAltSeparator, StringComparison.OrdinalIgnoreCase ) ) {
				path = path.RemoveLastCharacter();
				goto CouldBeMore;
			}

			return path.Trim();
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
		public static Boolean Equals( IFolder? left, IFolder? right ) {
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
		public static IFolder GetTempFolder() => new Folder( Path.GetTempPath() );

		public static implicit operator DirectoryInfo( Folder folder ) => folder.Info;

		/// <summary>Opens a folder in file explorer.</summary>
		public static void OpenWithExplorer( IFolder? folder ) {
			if ( folder is null ) {
				throw new ArgumentEmptyException( nameof( folder ) );
			}

			var windowsFolder = Environment.GetFolderPath( Environment.SpecialFolder.Windows );

			Process.Start( $@"{windowsFolder}\explorer.exe", $"/e,\"{folder.FullPath}\"" );
		}

		[DebuggerStepThrough]
		public static Boolean TryGetFolderFromPath( TrimmedString path, out DirectoryInfo? directoryInfo, out Uri? uri ) =>
			TryGetFolderFromPath( path.Value, out directoryInfo, out uri );

		[DebuggerStepThrough]
		public static Boolean TryGetFolderFromPath( String? path, out DirectoryInfo? directoryInfo, out Uri? uri ) {
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

		public static Boolean TryParse( String? path, out IFolder? folder ) {
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

		public static Boolean CheckFolderPermission( String folder ) {
			try {
				new DirectoryInfo( folder ).GetAccessControl( AccessControlSections.All );
				return true;
			}
			catch ( PrivilegeNotHeldException ) {
				return false;
			}
		}

		public static Boolean CheckFolderPermission( DirectoryInfo folder ) {
			try {
				folder.GetAccessControl( AccessControlSections.All );
				return true;
			}
			catch ( PrivilegeNotHeldException ) {
				return false;
			}
		}

		/// <summary>
		///     Return how many [sub]folders are in this folder's path.
		/// </summary>
		/// <returns></returns>
		public UInt16 LevelsDeep() {
			this._levelsDeep ??= ( UInt16? )this.FullPath.Count( c => c == FolderSeparatorChar );

			return this._levelsDeep.Value;
		}

		/// <summary>
		///     <see cref="op_Implicit" />
		/// </summary>
		/// <returns></returns>
		public DirectoryInfo ToDirectoryInfo() => this;

	}

}