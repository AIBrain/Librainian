// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "IOExtensions.cs" last formatted on 2021-11-30 at 7:17 PM by Protiguous.

#nullable enable

namespace Librainian.FileSystem;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Collections.Extensions;
using Collections.Sets;
using Exceptions;
using JetBrains.Annotations;
using Logging;
using Maths;
using Measurement.Time;
using OperatingSystem;
using Parsing;
using Pri.LongPath;
using Path = System.IO.Path;

public static class IOExtensions {

	public const Int32 FsctlSetCompression = 0x9C040;

	private static async Task FindEachDocument( IFolder folder, Action<Document>? onFindFile, String searchPattern, CancellationToken cancelToken ) {
		if ( folder is null ) {
			throw new NullException( nameof( folder ) );
		}

		try {
			await foreach ( var file in folder.EnumerateDocuments( searchPattern, cancelToken ).ConfigureAwait( false ) ) {
				try {
					onFindFile?.Invoke( file );
				}
				catch ( Exception exception ) {
					exception.Log();
				}
			}
		}
		catch ( UnauthorizedAccessException ) { }
		catch ( FolderNotFoundException ) { }
		catch ( IOException ) { }
		catch ( SecurityException ) { }
		catch ( AggregateException aggregateException ) {
			aggregateException.Handle( exception => {
				switch ( exception ) {
					case UnauthorizedAccessException:
					case FolderNotFoundException:
					case IOException:
					case SecurityException:
						return true;
				}

				exception.Log();

				return false;
			} );
		}
	}

	private static Boolean MatchesFilter(
		FileAttributes fileAttributes,
		String name,
		String searchPattern,
		Boolean aDirectory,
		Boolean aHidden,
		Boolean aSystem,
		Boolean aReadOnly,
		Boolean aCompressed,
		Boolean aArchive,
		Boolean aReparsePoint,
		String filterMode
	) {

		// first make sure that the name matches the search pattern
		if ( NativeMethods.PathMatchSpec( name, searchPattern ) ) {

			// then we build our filter attributes enumeration
			var filterAttributes = new FileAttributes();

			if ( aDirectory ) {
				filterAttributes |= FileAttributes.Directory;
			}

			if ( aHidden ) {
				filterAttributes |= FileAttributes.Hidden;
			}

			if ( aSystem ) {
				filterAttributes |= FileAttributes.System;
			}

			if ( aReadOnly ) {
				filterAttributes |= FileAttributes.ReadOnly;
			}

			if ( aCompressed ) {
				filterAttributes |= FileAttributes.Compressed;
			}

			if ( aReparsePoint ) {
				filterAttributes |= FileAttributes.ReparsePoint;
			}

			if ( aArchive ) {
				filterAttributes |= FileAttributes.Archive;
			}

			// based on the filtermode, we match the file with our filter attributes a bit differently
			return filterMode switch {
				"Include" when ( fileAttributes & filterAttributes ) == filterAttributes => true,
				"Include" => false,
				"Exclude" when ( fileAttributes & filterAttributes ) != filterAttributes => true,
				"Exclude" => false,
				"Strict" when fileAttributes == filterAttributes => true,
				"Strict" => false,
				var _ => false
			};
		}

		return false;
	}

	private static async Task ScanForSubFolders(
		IFolder startingFolder,
		Action<Document>? onFindFile,
		Action<Folder>? onEachDirectory,
		SearchStyle searchStyle,
		IReadOnlyCollection<String> searchPatterns,
		CancellationToken cancelToken
	) {
		try {
			await foreach ( var subFolder in startingFolder.EnumerateFolders( "*.*", SearchOption.TopDirectoryOnly, cancelToken ).ConfigureAwait( false ) ) {
				try {
					onEachDirectory?.Invoke( subFolder );
				}
				catch ( Exception exception ) {
					exception.Log();
				}

				//recurse into
				await subFolder.FindFiles( searchPatterns, cancelToken, onFindFile, onEachDirectory, searchStyle ).ConfigureAwait( false );
			}
		}
		catch ( UnauthorizedAccessException ) { }
		catch ( FolderNotFoundException ) { }
		catch ( IOException ) { }
		catch ( SecurityException ) { }
		catch ( AggregateException aggregateException ) {
			aggregateException.Handle( exception => {
				switch ( exception ) {
					case UnauthorizedAccessException _:
					case FolderNotFoundException _:
					case IOException _:
					case SecurityException _:
						return true;
				}

				exception.Log();

				return false;
			} );
		}
	}

	/// <summary>
	/// Example: WriteTextAsync( fullPath: fullPath, text: message ).Wait(); Example: await WriteTextAsync( fullPath: fullPath,
	/// text: message );
	/// </summary>
	/// <param name="fileInfo"></param>
	/// <param name="text"></param>
	/// <param name="waitfor"></param>
	public static async Task<(Status, Exception?)> AppendTextAsync( this FileInfo fileInfo, String text, TimeSpan? waitfor = null ) {
		try {
			var buffer = Common.DefaultEncoding.GetBytes( text ).AsMemory( 0 );
			var length = buffer.Length;

			await using var sourceStream = ReTry( () => new FileStream( fileInfo.FullName, FileMode.Append, FileAccess.Write, FileShare.Write, length, true ),
				waitfor ?? Seconds.Seven, CancellationToken.None );

			if ( sourceStream is null ) {
				throw new InvalidOperationException( $"Could not open file {fileInfo.FullName} for reading." );
			}

			await sourceStream.WriteAsync( buffer ).ConfigureAwait( false );

			await sourceStream.FlushAsync().ConfigureAwait( false );

			return (Status.Success, default( Exception? ));
		}
		catch ( UnauthorizedAccessException exception ) {
			exception.Log();
			return (Status.Exception, exception);
		}
		catch ( NullException exception ) {
			exception.Log();
			return (Status.Exception, exception);
		}
		catch ( FolderNotFoundException exception ) {
			exception.Log();
			return (Status.Exception, exception);
		}
		catch ( PathTooLongException exception ) {
			exception.Log();
			return (Status.Exception, exception);
		}
		catch ( SecurityException exception ) {
			exception.Log();
			return (Status.Exception, exception);
		}
		catch ( IOException exception ) {
			exception.Log();
			return (Status.Exception, exception);
		}
	}

	/// <summary>Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.</summary>
	/// <param name="fileInfo"></param>
	/// <remarks>Don't use on large files obviously..</remarks>
	public static IEnumerable<Byte> AsBytes( this FileInfo fileInfo ) {
		if ( fileInfo is null ) {
			throw new NullException( nameof( fileInfo ) );
		}

		if ( !fileInfo.Exists ) {
			yield break;
		}

		using var stream = ReTry( () => new FileStream( fileInfo.FullName, FileMode.Open, FileAccess.Read ), Seconds.Seven, CancellationToken.None );

		if ( stream is null ) {
			throw new InvalidOperationException( $"Could not open file {fileInfo.FullName} for reading." );
		}

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file {fileInfo.FullName}." );
		}

		using var buffered = new BufferedStream( stream );

		do {
			var b = buffered.ReadByte();

			if ( b == -1 ) {
				yield break;
			}

			yield return ( Byte )b;
		} while ( true );
	}

	/// <summary>Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.</summary>
	/// <param name="filename"></param>
	public static IEnumerable<Byte> AsBytes( this String filename ) {
		if ( String.IsNullOrWhiteSpace( filename ) ) {
			throw new NullException( nameof( filename ) );
		}

		if ( !File.Exists( filename ) ) {
			yield break;
		}

		var stream = ReTry( () => new FileStream( filename, FileMode.Open, FileAccess.Read ), Seconds.Seven, CancellationToken.None );

		if ( stream is null ) {
			throw new InvalidOperationException( $"Could not open file {filename} for reading." );
		}

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file {filename}." );
		}

		using ( stream ) {
			using var buffered = new BufferedStream( stream );

			do {
				var b = buffered.ReadByte();

				if ( b == -1 ) {
					yield break;
				}

				yield return ( Byte )b;
			} while ( true );
		}
	}

	/// <summary>Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.</summary>
	/// <param name="fileInfo"></param>
	public static IEnumerable<UInt16> AsUInt16Array( this FileInfo fileInfo ) {

		// TODO this needs a unit test for endianness
		if ( fileInfo is null ) {
			throw new NullException( nameof( fileInfo ) );
		}

		if ( !fileInfo.Exists ) {
			fileInfo.Refresh(); //check one more time

			if ( !fileInfo.Exists ) {
				yield break;
			}
		}

		using var stream = new FileStream( fileInfo.FullName, FileMode.Open );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file {fileInfo.FullName}" );
		}

		using var buffered = new BufferedStream( stream );

		var low = buffered.ReadByte();

		if ( low == -1 ) {
			yield break;
		}

		var high = buffered.ReadByte();

		if ( high == -1 ) {
			yield return ( ( Byte )low ).CombineBytes( 0 );

			yield break;
		}

		yield return ( ( Byte )low ).CombineBytes( ( Byte )high );
	}

	/// <summary>
	/// No guarantee of return order. Also, because of the way the operating system works (random-access), a directory can be
	/// created or deleted after a search.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="searchPattern"></param>
	/// <param name="searchOption">Defaults to <see cref="SearchOption.AllDirectories" /></param>
	public static async IAsyncEnumerable<Folder> BetterEnumerateDirectories(
		this DirectoryInfo target,
		[EnumeratorCancellation] CancellationToken cancellationToken,
		String? searchPattern = "*",
		SearchOption searchOption = SearchOption.AllDirectories
	) {
		searchPattern ??= "*";

		var searchPath = Path.Combine( target.FullName, searchPattern );

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

			if ( !findData.IsDirectory() || findData.IsParentOrCurrent() || findData.IsReparsePoint() || findData.IsIgnoreFolder() ) {
				continue;
			}

			if ( findData.cFileName != null ) {

				// Fix with @"\\?\" +System.IO.PathTooLongException?
				if ( findData.cFileName.Length > PriNativeMethods.MAX_PATH ) {
					$"Found subfolder with length longer than {PriNativeMethods.MAX_PATH}. Debug and see if it works.".Break( "poor man's debug" );

					//continue; //BUG Needs unit tested for long paths.
				}

				var subFolder = target.FullName.CombinePaths( findData.cFileName );

				yield return new Folder( subFolder );

				switch ( searchOption ) {
					case SearchOption.AllDirectories: {
						var subInfo = new DirectoryInfo( subFolder );

						await foreach ( var info in subInfo.BetterEnumerateDirectories( cancellationToken, searchPattern ).ConfigureAwait( false ) ) {
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

			try {
				more = await Task.Run( () => hFindFile.FindNextFile( out findData ), cancellationToken ).ConfigureAwait( false );
			}
			catch ( Exception exception ) {
				exception.Log();
			}
		} while ( more );
	}

	/*
	public static List<FileInformation> FastFind( String path, String searchPattern, Boolean getFile, Boolean getDirectories, Boolean recurse, Int32? depth,
		Boolean parallel, Boolean suppressErrors, Boolean largeFetch, Boolean getHidden, Boolean getSystem, Boolean getReadOnly, Boolean getCompressed, Boolean getArchive,
		Boolean getReparsePoint, String filterMode ) {
		PriNativeMethods.FINDEX_ADDITIONAL_FLAGS additionalFlags = 0;
		if ( largeFetch ) {
			additionalFlags = PriNativeMethods.FINDEX_ADDITIONAL_FLAGS.FindFirstExLargeFetch;
		}

		// add prefix to allow for maximum path of up to 32,767 characters
		String prefixedPath;
		if ( path.StartsWith( @"\\" ) ) {
			prefixedPath = path.Replace( @"\\", Path.UNCLongPathPrefix );
		}
		else {
			prefixedPath = Path.LongPathPrefix + path;
		}

		var handle = PriNativeMethods.FindFirstFileExW( prefixedPath + @"\*", PriNativeMethods.FINDEX_INFO_LEVELS.FindExInfoBasic, out var lpFindFileData,
			PriNativeMethods.FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, additionalFlags );

		var resultList = new ConcurrentHashset<FileInformation>();
		var subDirectoryList = new ConcurrentHashset<PathInformation>();

		if ( !handle.IsInvalid ) {
			do {

				// skip "." and ".."
				if ( lpFindFileData.cFileName == "." || lpFindFileData.cFileName == ".." ) {
					continue;
				}

				// if directory...
				if ( getDirectories && recurse && ( lpFindFileData.dwFileAttributes & FileAttributes.Directory ) == FileAttributes.Directory ) {

					// ...and if we are performing a recursive search... ... populate the subdirectory list
					var fullName = Path.Combine( path, lpFindFileData.cFileName );
					subDirectoryList.Add( new PathInformation( fullName ) );
				}

				// skip folders if only the getFile parameter is used
				if ( getFile && !getDirectories ) {
					if ( ( lpFindFileData.dwFileAttributes & FileAttributes.Directory ) == FileAttributes.Directory ) {
						continue;
					}
				}

				// if file matches search pattern and attribute filter, add it to the result list
				if ( MatchesFilter( lpFindFileData.dwFileAttributes, lpFindFileData.cFileName, searchPattern, getDirectories, getHidden, getSystem, getReadOnly,
					getCompressed, getArchive, getReparsePoint, filterMode ) ) {
					Int64? thisFileSize = null;
					if ( ( lpFindFileData.dwFileAttributes & FileAttributes.Directory ) != FileAttributes.Directory ) {
						thisFileSize = lpFindFileData.nFileSizeHigh * ( 2 ^ 32 ) + lpFindFileData.nFileSizeLow;
					}

					var item = new FileInformation( lpFindFileData.cFileName, new PathInformation( Path.Combine( path, lpFindFileData.cFileName ) ) ) {
						Parent = new PathInformation( path ), Attributes = lpFindFileData.dwFileAttributes, FileSize = thisFileSize,
						CreationTime = lpFindFileData.ftCreationTime.ToDateTime(), LastAccessTime = lpFindFileData.ftLastAccessTime.ToDateTime(),
						LastWriteTime = lpFindFileData.ftLastWriteTime.ToDateTime()
					};

					resultList.Add( item );
				}
			} while ( PriNativeMethods.FindNextFile( handle, out lpFindFileData ) );

			// close the file handle
			handle.Dispose();

			// handle recursive search
			if ( recurse ) {

				// handle depth of recursion
				if ( depth > 0 ) {
					if ( parallel ) {
						subDirectoryList.AsParallel().ForAll( x => {
							List<FileInformation> resultSubDirectory = FastFind( x.Path, searchPattern, getFile, getDirectories, recurse, depth - 1, false, suppressErrors,
								largeFetch, getHidden, getSystem, getReadOnly, getCompressed, getArchive, getReparsePoint, filterMode );
							resultList.AddRange( resultSubDirectory );
						} );
					}
					else {
						foreach ( var directory in subDirectoryList ) {
							foreach ( var result in FastFind( directory.Path, searchPattern, getFile, getDirectories, recurse, depth - 1, false, suppressErrors,
								largeFetch, getHidden, getSystem, getReadOnly, getCompressed, getArchive, getReparsePoint, filterMode ) ) {
								resultList.Add( result );
							}
						}
					}
				}

				// if no depth are specified
				else if ( depth == null ) {
					if ( parallel ) {
						subDirectoryList.AsParallel().ForAll( x => {
							var resultSubDirectory = new List<FileInformation>();
							resultSubDirectory = FastFind( x.Path, searchPattern, getFile, getDirectories, recurse, null, false, suppressErrors, largeFetch, getHidden,
								getSystem, getReadOnly, getCompressed, getArchive, getReparsePoint, filterMode );
							lock ( resultListLock ) {
								resultList.AddRange( resultSubDirectory );
							}
						} );
					}
					else {
						foreach ( var directory in subDirectoryList ) {
							foreach ( var result in FastFind( directory.Path, searchPattern, getFile, getDirectories, recurse, null, false, suppressErrors, largeFetch,
								getHidden, getSystem, getReadOnly, getCompressed, getArchive, getReparsePoint, filterMode ) ) {
								resultList.Add( result );
							}
						}
					}
				}
			}
		}

		// error handling
		else if ( handle.IsInvalid && !suppressErrors ) {
			Int32 hr = Marshal.GetLastWin32Error();
			if ( hr != 2 && hr != 0x12 ) {

				//throw new Win32Exception(hr);
				Console.WriteLine( "{0}:  {1}", path, new Win32Exception( hr ).Message );
			}
		}

		return resultList;
	}
	*/

	// --------------------------- CopyStream ---------------------------
	/// <summary>Copies data from a source stream to a target stream.</summary>
	/// <param name="source">The source stream to copy from.</param>
	/// <param name="target">The destination stream to copy to.</param>
	public static void CopyStream( this Stream source, Stream target ) {
		if ( source is null ) {
			throw new NullException( nameof( source ) );
		}

		if ( target is null ) {
			throw new NullException( nameof( target ) );
		}

		if ( !source.CanRead ) {
			throw new Exception( $"Cannot read from {nameof( source )}" );
		}

		if ( !target.CanWrite ) {
			throw new Exception( $"Cannot write to {nameof( target )}" );
		}

		const Int32 size = 0xffff;
		var buffer = new Byte[ size ];
		Int32 bytesRead;

		while ( ( bytesRead = source.Read( buffer, 0, size ) ) > 0 ) {
			target.Write( buffer, 0, bytesRead );
		}
	}

	/// <summary>Before: @"c:\hello\world". After: @"c:\hello\world\23468923475634836.extension"</summary>
	/// <param name="info"></param>
	/// <param name="withExtension"></param>
	/// <param name="toBase"></param>
	public static FileInfo DateAndTimeAsFile( this DirectoryInfo info, String? withExtension, Int32 toBase = 16 ) {
		if ( info is null ) {
			throw new NullException( nameof( info ) );
		}

		var now = Convert.ToString( DateTime.UtcNow.ToBinary(), toBase );
		var fileName = $"{now}{withExtension ?? info.Extension}";
		var path = info.FullName.CombinePaths( fileName );

		return new FileInfo( path );
	}

	/// <summary>If the <paramref name="directoryInfo" /> does not exist, attempt to create it.</summary>
	/// <param name="directoryInfo"></param>
	/// <param name="requestReadAccess"></param>
	/// <param name="requestWriteAccess"></param>
	public static DirectoryInfo? Ensure( this DirectoryInfo directoryInfo, Boolean? requestReadAccess = null, Boolean? requestWriteAccess = null ) {
		if ( directoryInfo is null ) {
			throw new NullException( nameof( directoryInfo ) );
		}

		try {
			directoryInfo.Refresh();

			if ( !directoryInfo.Exists ) {
				directoryInfo.Create();
				directoryInfo.Refresh();
			}

			if ( requestReadAccess.HasValue ) {
				directoryInfo.Refresh();
			}

			if ( requestWriteAccess.HasValue ) {
				var temp = directoryInfo.FullName.CombinePaths( Path.GetRandomFileName() );
				File.WriteAllText( temp, "Delete Me!" );
				File.Delete( temp );
				directoryInfo.Refresh();
			}
		}
		catch ( Exception exception ) {
			exception.Log();

			return default( DirectoryInfo? );
		}

		return directoryInfo;
	}

	public static DateTime FileNameAsDateAndTime( this FileInfo info, DateTime? defaultValue = null ) {
		if ( info is null ) {
			throw new NullException( nameof( info ) );
		}

		defaultValue ??= DateTime.MinValue;

		var now = defaultValue.Value;
		var fName = info.Name.GetFileNameWithoutExtension();

		if ( String.IsNullOrWhiteSpace( fName ) ) {
			return now;
		}

		fName = fName.Trim();

		if ( String.IsNullOrWhiteSpace( fName ) ) {
			return now;
		}

		if ( Int64.TryParse( fName, NumberStyles.AllowHexSpecifier, null, out var data ) ) {
			return DateTime.FromBinary( data );
		}

		if ( Int64.TryParse( fName, NumberStyles.Any, null, out data ) ) {
			return DateTime.FromBinary( data );
		}

		return now;
	}

	//TODO This needs rewritten as a whole drive file searcher using tasks.

	/// <summary>
	/// Search the <paramref name="folder" /> for any files matching the <paramref name="fileSearchPatterns" /> .
	/// </summary>
	/// <param name="folder">The folder to start the search.</param>
	/// <param name="fileSearchPatterns">List of patterns to search for.</param>
	/// <param name="cancelToken"></param>
	/// <param name="onFindFile"><see cref="Action" /> to perform when a file is found.</param>
	/// <param name="onEachDirectory"><see cref="Action" /> to perform on each folder found.</param>
	/// <param name="searchStyle"></param>
	public static async Task FindFiles(
		this Folder folder,
		IReadOnlyCollection<String> fileSearchPatterns,
		CancellationToken cancelToken,
		Action<Document>? onFindFile = null,
		Action<Folder>? onEachDirectory = null,
		SearchStyle searchStyle = SearchStyle.FilesFirst
	) {
		if ( fileSearchPatterns is null ) {
			throw new NullException( nameof( fileSearchPatterns ) );
		}

		if ( folder is null ) {
			throw new NullException( nameof( folder ) );
		}

		var searchPatterns = fileSearchPatterns.ToList();

		await foreach ( var searchPattern in searchPatterns.ToAsyncEnumerable().WithCancellation( cancelToken ).ConfigureAwait( false ) ) {
			if ( String.IsNullOrWhiteSpace( searchPattern ) ) {
				continue;
			}

			if ( cancelToken.IsCancellationRequested ) {
				return;
			}

			await FindEachDocument( folder, onFindFile, searchPattern, cancelToken ).ConfigureAwait( false );

			await ScanForSubFolders( folder, onFindFile, onEachDirectory, searchStyle, searchPatterns, cancelToken ).ConfigureAwait( false );
		}
	}

	/// <summary>
	/// <para>
	/// The code does not work properly on Windows Server 2008 or 2008 R2 or Windows 7 and Vista based systems as cluster size
	/// is always zero (GetDiskFreeSpaceW and GetDiskFreeSpace return -1 even with UAC disabled.)
	/// </para>
	/// </summary>
	/// <param name="info"></param>
	/// <see cref="http://stackoverflow.com/questions/3750590/get-size-of-file-on-disk" />
	public static UInt64? GetFileSizeOnDiskAlt( this FileInfo info ) {
		var result = NativeMethods.GetDiskFreeSpaceW( info.Directory.Root.FullName, out var sectorsPerCluster, out var bytesPerSector, out var _, out var _ );

		if ( result == 0 ) {
			throw new Win32Exception();
		}

		var clusterSize = sectorsPerCluster * bytesPerSector;
		var losize = NativeMethods.GetCompressedFileSizeW( info.FullName, out var sizeHigh );
		var size = ( ( Int64 )sizeHigh << 32 ) | losize;

		return ( UInt64 )( ( size + clusterSize - 1 ) / clusterSize * clusterSize );
	}

	public static DriveInfo? GetLargestEmptiestDrive() =>
		DriveInfo.GetDrives().AsParallel().Where( info => info.IsReady ).OrderByDescending( info => info.AvailableFreeSpace ).FirstOrDefault();

	/// <summary>
	/// Given the <paramref name="path" /> and <paramref name="searchPattern" /> pick any one file and return the <see
	/// cref="FileSystemInfo.FullPath" /> .
	/// </summary>
	/// <param name="path"></param>
	/// <param name="searchPattern"></param>
	/// <param name="searchOption"></param>
	public static String GetRandomFile( String path, String searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly ) {
		if ( String.IsNullOrWhiteSpace( path ) ) {
			throw new NullException( nameof( path ) );
		}

		if ( String.IsNullOrWhiteSpace( searchPattern ) ) {
			throw new NullException( nameof( searchPattern ) );
		}

		var dir = new DirectoryInfo( path );

		if ( !dir.Exists ) {
			return String.Empty;
		}

		var files = Directory.EnumerateFiles( dir.FullName, searchPattern, searchOption );
		var pickedfile = files.OrderBy( r => Randem.Next() ).FirstOrDefault();

		if ( pickedfile != null && File.Exists( pickedfile ) ) {
			return new FileInfo( pickedfile ).FullName;
		}

		return String.Empty;
	}

	/// <summary>Warning, this could OOM on a large folder structure.</summary>
	/// <param name="startingFolder"></param>
	/// <param name="foldersFound">Warning, this could OOM on a *large* folder structure.</param>
	/// <param name="cancellationToken"></param>
	public static async Task<Boolean> GrabAllFolders( this Folder startingFolder, ConcurrentSet<Folder> foldersFound, CancellationToken cancellationToken ) {
		if ( startingFolder is null ) {
			throw new NullException( nameof( startingFolder ) );
		}

		if ( foldersFound is null ) {
			throw new NullException( nameof( foldersFound ) );
		}

		try {
			if ( cancellationToken.IsCancellationRequested ) {
				return false;
			}

			if ( !await startingFolder.Exists( cancellationToken ).ConfigureAwait( false ) ) {
				return false;
			}

			if ( foldersFound.Add( startingFolder ) ) {
				await foreach ( var subFolder in startingFolder.EnumerateFolders( "*.*", SearchOption.AllDirectories, cancellationToken ).ConfigureAwait( false ) ) {
					if ( foldersFound.Add( subFolder ) ) {

						//recurse into
						await GrabAllFolders( subFolder, foldersFound, cancellationToken ).ConfigureAwait( false );
					}
				}
			}

			return true;
		}
		catch ( OutOfMemoryException ) {
			GC.Collect( 2, GCCollectionMode.Forced, true, true );
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return false;
	}

	/*

	/// <param name="startingFolder"></param>
	/// <param name="documentSearchPatterns"></param>
	/// <param name="onEachDocumentFound">Warning, this could OOM on a large folder structure.</param>
	/// <param name="cancellation"></param>
	/// <param name="progressFolders"></param>
	/// <param name="progressDocuments"></param>
	/// <returns></returns>
	public static Boolean GrabEntireTree( [NotNull] this IFolder startingFolder, [CanBeNull] IEnumerable<String>? documentSearchPatterns,
		[NotNull] Action<Document> onEachDocumentFound, [CanBeNull] IProgress<Int64>? progressFolders, [CanBeNull] IProgress<Int64>? progressDocuments,
		[NotNull] CancellationTokenSource cancellation ) {
		if ( startingFolder is null ) {
			throw new NullException( nameof( startingFolder ) );
		}

		if ( onEachDocumentFound is null ) {
			throw new NullException( nameof( onEachDocumentFound ) );
		}

		//if ( foldersFound is null ) {
		//    throw new NullException( nameof( foldersFound ) );
		//}

		if ( cancellation.IsCancellationRequested ) {
			return false;
		}

		if ( !startingFolder.Exists() ) {
			return false;
		}

		//foldersFound.Add( startingFolder );
		var searchPatterns = documentSearchPatterns ?? new[] {
			"*.*"
		};

		Parallel.ForEach( startingFolder.GetFolders( "*" ).AsParallel(), folder => {
			progressFolders?.Report( 1 );
			GrabEntireTree( folder, searchPatterns, onEachDocumentFound, progressFolders, progressDocuments, cancellation );
			progressFolders?.Report( -1 );
		} );

		//var list = new List<FileInfo>();
		foreach ( var files in searchPatterns.Select( searchPattern => startingFolder.Info.EnumerateFiles( searchPattern ).OrderBy( info => Randem.Next() ) ) ) {
			foreach ( var info in files ) {
				progressDocuments?.Report( 1 );
				onEachDocumentFound( new Document( info ) );

				if ( cancellation.IsCancellationRequested ) {
					return false;
				}
			}
		}

		//if ( cancellation.HaveAnyCancellationsBeenRequested() ) {
		//    return documentsFound.Any();
		//}
		//foreach ( var folder in startingFolder.GetFolders() ) {
		//    GrabEntireTree( folder, searchPatterns, onEachDocumentFound, cancellation );
		//}

		return true;
	}
	*/

	[Pure]
	public static Boolean IsDirectory( this NativeMethods.Win32FindData data ) => data.dwFileAttributes.HasFlag( FileAttributes.Directory );

	[Pure]
	public static Boolean IsDirectory( this WIN32_FIND_DATA data ) => data.dwFileAttributes.HasFlag( FileAttributes.Directory );

	[Pure]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean IsExtended( this String path ) =>
		path.Length >= 4 && path[ 0 ] == PathInternal.Constants.Backslash && ( path[ 1 ] == PathInternal.Constants.Backslash || path[ 1 ] == '?' ) && path[ 2 ] == '?' &&
		path[ 3 ] == PathInternal.Constants.Backslash;

	[Pure]
	public static Boolean IsFile( this NativeMethods.Win32FindData data ) => !IsDirectory( data );

	[Pure]
	public static Boolean IsFile( this WIN32_FIND_DATA data ) => !IsDirectory( data );

	/// <summary>Hard coded folders to skip.</summary>
	/// <param name="data"></param>
	[Pure]
	public static Boolean IsIgnoreFolder( this NativeMethods.Win32FindData data ) =>
		data.cFileName.Like( "$RECYCLE.BIN" ) /*|| data.cFileName.Like( "TEMP" ) || data.cFileName.Like( "TMP" )*/ || data.cFileName.Like( "System Volume Information" );

	/// <summary>Hard coded folders to skip.</summary>
	/// <param name="data"></param>
	[Pure]
	public static Boolean IsIgnoreFolder( this WIN32_FIND_DATA data ) =>
		data.cFileName.Like( "$RECYCLE.BIN" ) /*|| data.cFileName.Like( "TEMP" ) || data.cFileName.Like( "TMP" )*/ || data.cFileName.Like( "System Volume Information" );

	[Pure]
	public static Boolean IsParentOrCurrent( this NativeMethods.Win32FindData data ) => data.cFileName is "." or "..";

	[Pure]
	public static Boolean IsParentOrCurrent( this WIN32_FIND_DATA data ) => data.cFileName is "." or "..";

	[Pure]
	public static Boolean IsReparsePoint( this NativeMethods.Win32FindData data ) => data.dwFileAttributes.HasFlag( FileAttributes.ReparsePoint );

	[Pure]
	public static Boolean IsReparsePoint( this WIN32_FIND_DATA data ) => data.dwFileAttributes.HasFlag( FileAttributes.ReparsePoint );

	/// <summary>Open with Explorer.exe</summary>
	/// <param name="folder">todo: describe folder parameter on OpenDirectoryWithExplorer</param>
	public static Boolean OpenWithExplorer( this DirectoryInfo folder ) {
		if ( folder is null ) {
			throw new NullException( nameof( folder ) );
		}

		var windows = Windows.WindowsSystem32Folder.Value;

		if ( windows is null ) {
			return false;
		}

		var process = Process.Start( $@"{windows.FullPath.CombinePaths( "explorer.exe" )}", $" /separate /select,{folder.FullName.DoubleQuote()} " );

		return process switch {
			null => false,
			var _ => process.Responding
		};
	}

	/// <summary>Open with Explorer.exe</summary>
	/// <param name="folder">todo: describe folder parameter on OpenDirectoryWithExplorer</param>
	public static Boolean OpenWithExplorer( this Folder folder ) {
		if ( folder is null ) {
			throw new NullException( nameof( folder ) );
		}

		var windows = Windows.WindowsSystem32Folder.Value;

		if ( windows is null ) {
			return false;
		}

		var proc = Process.Start( $@"{windows.FullPath.CombinePaths( "explorer.exe" )}", $" /separate /select,{folder.FullPath.DoubleQuote()} " );

		return proc switch {
			null => false,
			var _ => proc.Responding
		};
	}

	/// <summary>Open with Explorer.exe</summary>
	public static Boolean OpenWithExplorer( this Document document ) {
		if ( document is null ) {
			throw new NullException( nameof( document ) );
		}

		var windows = Windows.WindowsSystem32Folder.Value;

		if ( windows is null ) {
			return false;
		}

		var proc = Process.Start( $@"{windows.FullPath.CombinePaths( "explorer.exe" )}", $" /separate /select,{document.FullPath.DoubleQuote()} " );

		return proc switch {
			null => false,
			var _ => proc.Responding
		};
	}

	/// <summary>Before: "hello.txt". After: "hello 345680969061906730476346.txt"</summary>
	/// <param name="info"></param>
	/// <param name="newExtension"></param>
	public static FileInfo PlusDateTime( this FileInfo info, String? newExtension = null ) {
		if ( info is null ) {
			throw new NullException( nameof( info ) );
		}

		if ( info.Directory is null ) {
			throw new NullReferenceException( "info.directory" );
		}

		var now = Convert.ToString( DateTime.UtcNow.ToBinary(), 16 );
		var formatted = $"{info.Name.GetFileNameWithoutExtension()} {now}{newExtension ?? info.Extension}";
		var path = info.Directory.FullName.CombinePaths( formatted );

		return new FileInfo( path );
	}

	/// <summary>untested. is this written correctly? would it read from a *slow* media but not block the calling function?</summary>
	/// <param name="filePath"></param>
	/// <param name="bufferSize"></param>
	/// <param name="fileMissingRetries"></param>
	/// <param name="retryDelay"></param>
	public static async Task<String> ReadTextAsync( String filePath, Int32? bufferSize = 65536, Int32? fileMissingRetries = 10, TimeSpan? retryDelay = null ) {
		if ( String.IsNullOrWhiteSpace( filePath ) ) {
			throw new NullException( nameof( filePath ) );
		}

		bufferSize ??= 65536;

		while ( fileMissingRetries > 0 ) {
			if ( File.Exists( filePath ) ) {
				break;
			}

			await Task.Delay( retryDelay ?? Seconds.One ).ConfigureAwait( false );
			fileMissingRetries--;
		}

		if ( File.Exists( filePath ) ) {
			try {
				var sb = new StringBuilder( bufferSize.Value );
				var buffer = new Byte[ bufferSize.Value ];

				var sourceStream = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize.Value, true );
				await using var _ = sourceStream.ConfigureAwait( false );

				Int32 numRead;

				while ( ( numRead = await sourceStream.ReadAsync( buffer, 0, buffer.Length ).ConfigureAwait( false ) ) != 0 ) {
					var text = Encoding.Unicode.GetString( buffer, 0, numRead );
					sb.Append( text );
				}

				return sb.ToString();
			}
			catch ( FileNotFoundException exception ) {
				exception.Log();
			}
		}

		return String.Empty;
	}

	/// <summary>Retry the <paramref name="ioFunction" /> if an <see cref="IOException" /> occurs.</summary>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="ioFunction"></param>
	/// <param name="tryFor"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="IOException"></exception>
	public static TResult? ReTry<TResult>( this Func<TResult> ioFunction, TimeSpan tryFor, CancellationToken cancellationToken ) where TResult : class {
		var stopwatch = Stopwatch.StartNew();
		TryAgain:

		if ( cancellationToken.IsCancellationRequested ) {
			return default( TResult? );
		}

		try {
			return ioFunction();
		}
		catch ( IOException exception ) {
			exception.Message.Error();

			if ( stopwatch.Elapsed > tryFor ) {
				return default( TResult? );
			}

			goto TryAgain;
		}
	}

	/// <summary>Retry the <paramref name="ioFunction" /> if an <see cref="IOException" /> occurs.</summary>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="ioFunction"></param>
	/// <param name="tryFor"></param>
	/// <param name="result"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="IOException"></exception>
	public static Boolean ReTry<TResult>( this Func<TResult> ioFunction, TimeSpan tryFor, out TResult? result, CancellationToken cancellationToken ) where TResult : struct {
		var stopwatch = Stopwatch.StartNew();
		TryAgain:

		if ( cancellationToken.IsCancellationRequested ) {
			result = null;
			return false;
		}

		try {
			result = ioFunction();
			return true;
		}
		catch ( IOException exception ) {
			exception.Message.Error();

			if ( stopwatch.Elapsed > tryFor ) {
				result = null;
				return false;
			}

			goto TryAgain;
		}
	}

	/// <summary>
	/// <para>performs a byte by byte file comparison</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <exception cref="NullException"></exception>
	/// <exception cref="SecurityException"></exception>
	/// <exception cref="NullException"></exception>
	/// <exception cref="UnauthorizedAccessException"></exception>
	/// <exception cref="PathTooLongException"></exception>
	/// <exception cref="NotSupportedException"></exception>
	/// <exception cref="IOException"></exception>
	/// <exception cref="FolderNotFoundException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	public static Boolean SameContent( this FileInfo? left, FileInfo? right ) {
		if ( left is null || right is null ) {
			return false;
		}

		if ( !left.Exists || !right.Exists ) {
			return false;
		}

		if ( left.Length != right.Length ) {
			return false;
		}

		var lba = left.AsBytes(); //.ToArray();
		var rba = right.AsBytes(); //.ToArray();

		return lba.SequenceEqual( rba );
	}

	/// <summary>
	/// <para>performs a byte by byte file comparison</para>
	/// </summary>
	/// <param name="leftFileName"></param>
	/// <param name="rightFileName"></param>
	/// <exception cref="NullException"></exception>
	/// <exception cref="SecurityException"></exception>
	/// <exception cref="NullException"></exception>
	/// <exception cref="UnauthorizedAccessException"></exception>
	/// <exception cref="PathTooLongException"></exception>
	/// <exception cref="NotSupportedException"></exception>
	/// <exception cref="IOException"></exception>
	/// <exception cref="FolderNotFoundException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	public static Boolean SameContent( this String? leftFileName, String? rightFileName ) {
		if ( leftFileName is null || rightFileName is null ) {
			return false;
		}

		if ( !File.Exists( leftFileName ) ) {
			return false;
		}

		if ( !File.Exists( rightFileName ) ) {
			return false;
		}

		if ( leftFileName.Length != rightFileName.Length ) {
			return false;
		}

		var lba = leftFileName.AsBytes().ToArray();
		var rba = rightFileName.AsBytes().ToArray();

		return lba.SequenceEqual( rba );
	}

	/// <summary>
	/// <para>performs a byte by byte file comparison</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="rightFile"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NullException"></exception>
	/// <exception cref="SecurityException"></exception>
	/// <exception cref="NullException"></exception>
	/// <exception cref="UnauthorizedAccessException"></exception>
	/// <exception cref="PathTooLongException"></exception>
	/// <exception cref="NotSupportedException"></exception>
	/// <exception cref="IOException"></exception>
	/// <exception cref="FolderNotFoundException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	public static async Task<Boolean> SameContent( this Document? left, FileInfo? rightFile, CancellationToken cancellationToken ) {
		if ( left is null || rightFile is null ) {
			return false;
		}

		var right = new Document( rightFile.FullName );

		return await left.SameContent( right, cancellationToken ).ConfigureAwait( false );
	}

	/// <summary>
	/// <para>performs a byte by byte file comparison</para>
	/// </summary>
	/// <param name="right"></param>
	/// <param name="left"></param>
	/// <exception cref="NullException"></exception>
	/// <exception cref="SecurityException"></exception>
	/// <exception cref="NullException"></exception>
	/// <exception cref="UnauthorizedAccessException"></exception>
	/// <exception cref="PathTooLongException"></exception>
	/// <exception cref="NotSupportedException"></exception>
	/// <exception cref="IOException"></exception>
	/// <exception cref="FolderNotFoundException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	public static async Task<Boolean> SameContent( this FileInfo? leftFile, Document? right, CancellationToken cancellationToken ) {
		if ( leftFile is null || right is null ) {
			return false;
		}

		var left = new Document( leftFile.FullName );

		return await left.SameContent( right, cancellationToken ).ConfigureAwait( false );
	}

	/// <summary>Search all possible drives for any files matching the <paramref name="fileSearchPatterns" /> .</summary>
	/// <param name="fileSearchPatterns">List of patterns to search for.</param>
	/// <param name="cancellationToken"></param>
	/// <param name="onFindFile"><see cref="Action" /> to perform when a file is found.</param>
	/// <param name="onEachDirectory"><see cref="Action" /> to perform on each folder found.</param>
	/// <param name="searchStyle"></param>
	public static void SearchAllDrives(
		this IReadOnlyCollection<String> fileSearchPatterns,
		CancellationToken cancellationToken,
		Action<Document>? onFindFile = null,
		Action<Folder>? onEachDirectory = null,
		SearchStyle searchStyle = SearchStyle.FilesFirst
	) {
		if ( fileSearchPatterns is null ) {
			throw new NullException( nameof( fileSearchPatterns ) );
		}

		try {
			DriveInfo.GetDrives()
					 .AsParallel()
					 .WithDegreeOfParallelism( 26 )
					 .WithExecutionMode( ParallelExecutionMode.ForceParallelism )
					 .ForAll( async drive => {
						 if ( !drive.IsReady || drive.DriveType == DriveType.NoRootDirectory || !drive.RootDirectory.Exists ) {
							 return;
						 }

						 $"Scanning [{drive.VolumeLabel}]".Info();
						 var root = new Folder( drive.RootDirectory.FullName );
						 await root.FindFiles( fileSearchPatterns, cancellationToken, onFindFile, onEachDirectory, searchStyle ).ConfigureAwait( false );
					 } );
		}
		catch ( UnauthorizedAccessException ) { }
		catch ( FolderNotFoundException ) { }
		catch ( IOException ) { }
		catch ( SecurityException ) { }
		catch ( AggregateException exception ) {
			exception.Handle( ex => {
				switch ( ex ) {
					case UnauthorizedAccessException _:
					case FolderNotFoundException _:
					case IOException _:
					case SecurityException _: {
						return true;
					}
				}

				ex.Log();

				return false;
			} );
		}
	}

	public static String SimplifyFileName( this Document document ) {
		if ( document is null ) {
			throw new NullException( nameof( document ) );
		}

		var fileNameWithoutExtension = document.FileName.GetFileNameWithoutExtension();

		TryAgain:

		//check for a double extension (image.jpg.tif),
		//remove the fake .tif extension?
		//OR remove the fake .jpg extension?
		if ( !fileNameWithoutExtension.GetExtension().IsNullOrEmpty() ) {

			// ReSharper disable once AssignNullToNotNullAttribute
			fileNameWithoutExtension = fileNameWithoutExtension.GetFileNameWithoutExtension();

			goto TryAgain;
		}

		//TODO we have the document, see if we can just chop off down to a nonexistent filename.. just get rid of (3) then (2) then (1)

		var splitIntoWords = fileNameWithoutExtension.Split( ' ', StringSplitOptions.RemoveEmptyEntries ).ToList();

		if ( splitIntoWords.Count >= 2 ) {
			var list = splitIntoWords.ToList();
			var lastWord = list.TakeLast();

			//check for a copy indicator
			if ( lastWord.Like( "Copy" ) ) {
				fileNameWithoutExtension = list.ToStrings( " " );
				fileNameWithoutExtension = fileNameWithoutExtension.Trim();

				goto TryAgain;
			}

			//check for a trailing "-" or "_"
			if ( lastWord.Like( "-" ) || lastWord.Like( "_" ) ) {
				fileNameWithoutExtension = list.ToStrings( " " );
				fileNameWithoutExtension = fileNameWithoutExtension.Trim();

				goto TryAgain;
			}

			//check for duplicate "word word" at the string's ending.
			var nextlastWord = list.TakeLast();

			if ( lastWord.Like( nextlastWord ) ) {
				fileNameWithoutExtension = list.ToStrings( " " ) + " " + lastWord;
				fileNameWithoutExtension = fileNameWithoutExtension.Trim();

				goto TryAgain;
			}
		}

		return $"{fileNameWithoutExtension}{document.Extension()}";
	}

	public static IReadOnlyList<String> ToPaths( this DirectoryInfo directoryInfo ) {
		if ( directoryInfo is null ) {
			throw new NullException( nameof( directoryInfo ) );
		}

		return directoryInfo.FullName.Split( Path.DirectorySeparatorChar );
	}

	public static MemoryStream TryCopyStream(
		String filePath,
		Boolean bePatient = true,
		FileMode fileMode = FileMode.Open,
		FileAccess fileAccess = FileAccess.Read,
		FileShare fileShare = FileShare.ReadWrite
	) {
		if ( String.IsNullOrWhiteSpace( filePath ) ) {
			throw new NullException( nameof( filePath ) );
		}

		//TODO
		TryAgain:
		var memoryStream = new MemoryStream();

		try {
			if ( File.Exists( filePath ) ) {
				using var fileStream = File.Open( filePath, fileMode, fileAccess, fileShare );

				var length = ( Int32 )fileStream.Length;

				if ( length > 0 ) {
					fileStream.CopyTo( memoryStream, length ); //BUG int-long possible issue.
					memoryStream.Seek( 0, SeekOrigin.Begin );
				}
			}
		}
		catch ( IOException ) {

			// IOExcception is thrown if the file is in use by another process.
			if ( bePatient ) {
				if ( !Thread.Yield() ) {
					Thread.Sleep( 0 );
				}

				goto TryAgain;
			}
		}

		return memoryStream;
	}

	/// <summary>Returns a temporary <see cref="Document" /> (but does not create the file in the file system).</summary>
	/// <param name="folder"></param>
	/// <param name="extension">If no extension is given, a random <see cref="Guid" /> is used.</param>
	/// <param name="deleteAfterClose"></param>
	/// <exception cref="NullException"></exception>
	public static Document TryGetTempDocument( this Folder folder, String? extension = null, Boolean deleteAfterClose = false ) {
		if ( folder is null ) {
			throw new NullException( nameof( folder ) );
		}

		var randomFileName = Guid.NewGuid().ToString();
		extension = extension.Trimmed() ?? Guid.NewGuid().ToString();

		if ( !extension.StartsWith( ".", StringComparison.OrdinalIgnoreCase ) ) {
			extension = $".{extension}";
		}

		return new Document( folder.FullPath, $"{randomFileName}{extension}", deleteAfterClose );
	}

	/// <summary>Tries to open a file, with a user defined number of attempt and Sleep delay between attempts.</summary>
	/// <param name="filePath">The full file path to be opened</param>
	/// <param name="fileMode">Required file mode enum value(see MSDN documentation)</param>
	/// <param name="fileAccess">Required file access enum value(see MSDN documentation)</param>
	/// <param name="fileShare">Required file share enum value(see MSDN documentation)</param>
	/// <returns>
	/// A valid FileStream object for the opened file, or null if the File could not be opened after the required attempts
	/// </returns>
	public static FileStream? TryOpen( String? filePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare ) {

		//TODO
		try {
			return File.Open( filePath, fileMode, fileAccess, fileShare );
		}
		catch ( IOException ) {

			// IOExcception is thrown if the file is in use by another process.
		}

		return default( FileStream? );
	}

	public static FileStream? TryOpenForReading(
		String filePath,
		Boolean bePatient = true,
		FileMode fileMode = FileMode.Open,
		FileAccess fileAccess = FileAccess.Read,
		FileShare fileShare = FileShare.ReadWrite
	) {
		if ( String.IsNullOrWhiteSpace( filePath ) ) {
			throw new NullException( nameof( filePath ) );
		}

		//TODO
		TryAgain:

		try {
			if ( File.Exists( filePath ) ) {
				return File.Open( filePath, fileMode, fileAccess, fileShare );
			}
		}
		catch ( IOException ) {

			// IOExcception is thrown if the file is in use by another process.
			if ( !bePatient ) {
				return default( FileStream? );
			}

			if ( !Thread.Yield() ) {
				Thread.Sleep( 0 );
			}

			goto TryAgain;
		}

		return default( FileStream? );
	}

	public static FileStream? TryOpenForWriting(
		String? filePath,
		FileMode fileMode = FileMode.Create,
		FileAccess fileAccess = FileAccess.Write,
		FileShare fileShare = FileShare.ReadWrite
	) {

		//TODO
		try {
			return File.Open( filePath, fileMode, fileAccess, fileShare );
		}
		catch ( IOException ) {

			// IOExcception is thrown if the file is in use by another process.
		}

		return default( FileStream? );
	}

	public static Int32? TurnOnCompression( this FileInfo info ) {
		if ( info is null ) {
			throw new NullException( nameof( info ) );
		}

		if ( !info.Exists ) {
			info.Refresh();

			if ( !info.Exists ) {
				return default( Int32? );
			}
		}

		var lpBytesReturned = 0;
		Int16 compressionFormatDefault = 1;

		using var fileStream = File.Open( info.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None );

		var success = false;

		try {
			fileStream.SafeFileHandle.DangerousAddRef( ref success );

			NativeMethods.DeviceIoControl( fileStream.SafeFileHandle.DangerousGetHandle(), FsctlSetCompression, ref compressionFormatDefault, sizeof( Int16 ), IntPtr.Zero, 0,
				ref lpBytesReturned, IntPtr.Zero );
		}
		finally {
			fileStream.SafeFileHandle.DangerousRelease();
		}

		return lpBytesReturned;
	}

	/// <summary>(does not create path)</summary>
	/// <param name="basePath"></param>
	/// <param name="d"></param>
	public static DirectoryInfo WithShortDatePath( this DirectoryInfo basePath, DateTime d ) {
		var path = basePath.FullName.CombinePaths( d.Year.ToString(), d.DayOfYear.ToString(), d.Hour.ToString() );

		return new DirectoryInfo( path );
	}

	[Serializable]
	public record FileInformation( String Name, PathInformation Path ) {
		public FileAttributes Attributes { get; set; }

		public DateTime? CreationTime { get; set; }

		public Int64? FileSize { get; set; }

		public DateTime? LastAccessTime { get; set; }

		public DateTime? LastWriteTime { get; set; }

		public PathInformation? Parent { get; set; }

		public PathInformation Path { get; set; } = Path;
	}

	[Serializable]
	public record PathInformation( String Path ) {
		public FileAttributes Attributes { get; set; }

		public DateTime CreationTime { get; set; }

		public DateTime LastAccessTime { get; set; }

		public DateTime LastWriteTime { get; set; }

		public PathInformation? Parent { get; set; }
	}
}