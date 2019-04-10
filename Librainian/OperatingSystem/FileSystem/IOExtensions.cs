// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// this entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// this source code contained in "IOExtensions.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "IOExtensions.cs" was last formatted by Protiguous on 2018/07/10 at 8:55 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Security;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Collections.Extensions;
    using Controls;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Measurement.Time;
    using OperatingSystem;
    using Parsing;
    using Threading;

    public static class IOExtensions {

        public const Int32 FsctlSetCompression = 0x9C040;

        private static FileInfo InternalSearchFoundFile( this FileInfo info, [CanBeNull] Action<FileInfo> onFindFile, [CanBeNull] SimpleCancel cancellation ) {
            try {
                if ( cancellation?.HaveAnyCancellationsBeenRequested() == false ) { onFindFile?.Invoke( info ); }
            }
            catch ( Exception exception ) { exception.Log(); }

            return info;
        }

        /// <summary>
        ///     Example: WriteTextAsync( fullPath: fullPath, text: message ).Wait();
        ///     Example: await WriteTextAsync( fullPath: fullPath, text: message );
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="text">    </param>
        /// <returns></returns>
        public static async Task AppendTextAsync( [NotNull] this FileInfo fileInfo, [CanBeNull] String text ) {
            if ( fileInfo == null ) { throw new ArgumentNullException( nameof( fileInfo ) ); }

            if ( String.IsNullOrWhiteSpace( fileInfo.FullName ) || String.IsNullOrWhiteSpace( text ) ) { return; }

            try {
                var encodedText = Encoding.Unicode.GetBytes( text );
                var length = encodedText.Length;

                using ( var sourceStream = new FileStream( fileInfo.FullName, mode: FileMode.Append, access: FileAccess.Write, share: FileShare.Write, bufferSize: length, useAsync: true ) ) {
                    await sourceStream.WriteAsync( buffer: encodedText, offset: 0, count: length ).ConfigureAwait( false );
                    await sourceStream.FlushAsync().ConfigureAwait( false );
                }
            }
            catch ( UnauthorizedAccessException exception ) { exception.Log(); }
            catch ( ArgumentNullException exception ) { exception.Log(); }
            catch ( DirectoryNotFoundException exception ) { exception.Log(); }
            catch ( PathTooLongException exception ) { exception.Log(); }
            catch ( SecurityException exception ) { exception.Log(); }
            catch ( IOException exception ) { exception.Log(); }
        }

        /// <summary>
        ///     Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static IEnumerable<Byte> AsBytes( [NotNull] this FileInfo fileInfo ) {
            if ( fileInfo == null ) { throw new ArgumentNullException( nameof( fileInfo ) ); }

            if ( !fileInfo.Exists ) { yield break; }

            var stream = ReTry( () => new FileStream( fileInfo.FullName, mode: FileMode.Open, access: FileAccess.Read ), Seconds.Seven, CancellationToken.None );

            if ( stream == null ) { yield break; }

            if ( !stream.CanRead ) { throw new NotSupportedException( $"Cannot read from file {fileInfo.FullName}" ); }

            using ( stream ) {
                using ( var buffered = new BufferedStream( stream ) ) {
                    do {
                        var b = buffered.ReadByte();

                        if ( b == -1 ) { yield break; }

                        yield return ( Byte )b;
                    } while ( true );
                }
            }
        }

        /// <summary>
        ///     Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static IEnumerable<Byte> AsBytes( [NotNull] this String filename ) {
            if ( filename == null ) { throw new ArgumentNullException( nameof( filename ) ); }

            if ( !File.Exists( filename ) ) { yield break; }

            var stream = ReTry( () => new FileStream( filename, mode: FileMode.Open, access: FileAccess.Read ), Seconds.Seven, CancellationToken.None );

            if ( stream == null ) { yield break; }

            if ( !stream.CanRead ) { throw new NotSupportedException( $"Cannot read from file {filename}." ); }

            using ( stream ) {
                using ( var buffered = new BufferedStream( stream ) ) {
                    do {
                        var b = buffered.ReadByte();

                        if ( b == -1 ) { yield break; }

                        yield return ( Byte )b;
                    } while ( true );
                }
            }
        }

        /// <summary>
        ///     ask user for folder/network path where to store dictionary
        /// </summary>
        /// <param name="hint">todo: describe hint parameter on AskUserForStorageFolder</param>
        [CanBeNull]
        public static Folder AskUserForStorageFolder( String hint ) {
            using ( var folderBrowserDialog = new FolderBrowserDialog {
                ShowNewFolderButton = true,
                Description = $"Please direct me to a storage folder for {hint}.",
                RootFolder = Environment.SpecialFolder.MyComputer
            } ) {
                var owner = WindowWrapper.CreateWindowWrapper( Process.GetCurrentProcess().MainWindowHandle );

                var dialog = folderBrowserDialog.ShowDialog( owner );

                if ( dialog != DialogResult.OK || folderBrowserDialog.SelectedPath.IsNullOrWhiteSpace() ) { return null; }

                return new Folder( folderBrowserDialog.SelectedPath );
            }
        }

        /// <summary>
        ///     Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>

        // TODO this needs a unit test for endianness
        public static IEnumerable<UInt16> AsUInt16Array( [NotNull] this FileInfo fileInfo ) {
            if ( fileInfo == null ) { throw new ArgumentNullException( nameof( fileInfo ) ); }

            if ( !fileInfo.Exists ) {
                fileInfo.Refresh(); //check one more time

                if ( !fileInfo.Exists ) { yield break; }
            }

            using ( var stream = new FileStream( fileInfo.FullName, FileMode.Open ) ) {
                if ( !stream.CanRead ) { throw new NotSupportedException( $"Cannot read from file {fileInfo.FullName}" ); }

                using ( var buffered = new BufferedStream( stream ) ) {
                    var low = buffered.ReadByte();

                    if ( low == -1 ) { yield break; }

                    var high = buffered.ReadByte();

                    if ( high == -1 ) {
                        yield return ( ( Byte )low ).CombineBytes( high: 0 );

                        yield break;
                    }

                    yield return ( ( Byte )low ).CombineBytes( high: ( Byte )high );
                }
            }
        }

        /// <summary>
        ///     No guarantee of return order. Also, because of the way the operating system works (random-access), a directory may
        ///     be created or deleted after a search.
        /// </summary>
        /// <param name="target">       </param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"> Defaults to <see cref="SearchOption.AllDirectories" /></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> BetterEnumerateDirectories( [CanBeNull] this DirectoryInfo target, String searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories ) {
            if ( target == null ) { yield break; }

            var searchPath = Path.Combine( target.FullName, searchPattern );

            using ( var hFindFile = NativeMethods.FindFirstFile( searchPath, out var findData ) ) {
                do {
                    if ( hFindFile.IsInvalid ) { break; }

                    if ( !IsDirectory( findData ) ) { continue; }

                    if ( IsParentOrCurrent( findData ) ) { continue; }

                    if ( IsReparsePoint( findData ) ) { continue; }

                    if ( IsIgnoreFolder( findData ) ) { continue; }

                    var subFolder = Path.Combine( target.FullName, findData.cFileName );

                    // Fix with @"\\?\" +System.IO.PathTooLongException?
                    if ( subFolder.Length >= 260 ) {
                        continue; //HACK
                    }

                    var subInfo = new DirectoryInfo( subFolder );

                    yield return subInfo;

                    switch ( searchOption ) {
                        case SearchOption.AllDirectories:

                            foreach ( var info in subInfo.BetterEnumerateDirectories( searchPattern ) ) { yield return info; }

                            break;
                    }
                } while ( NativeMethods.FindNextFile( hFindFile, out findData ) );
            }
        }

        public static IEnumerable<FileInfo> BetterEnumerateFiles( [NotNull] this DirectoryInfo target, [NotNull] String searchPattern = "*" ) {
            if ( target == null ) { throw new ArgumentNullException( nameof( target ) ); }

            if ( searchPattern == null ) { throw new ArgumentNullException( nameof( searchPattern ) ); }

            var searchPath = Path.Combine( target.FullName, searchPattern );

            using ( var hFindFile = NativeMethods.FindFirstFile( searchPath, out var findData ) ) {
                do {
                    if ( hFindFile.IsInvalid ) { break; }

                    if ( IsParentOrCurrent( findData ) ) { continue; }

                    if ( IsReparsePoint( findData ) ) { continue; }

                    if ( !IsFile( findData ) ) { continue; }

                    var newfName = Path.Combine( target.FullName, findData.cFileName );

                    yield return new FileInfo( newfName );
                } while ( NativeMethods.FindNextFile( hFindFile, out findData ) );
            }
        }

        [CanBeNull]
        public static DirectoryInfo ChooseDirectoryDialog( this Environment.SpecialFolder startFolder, String description = "Please select a folder." ) {
            using ( var folderDialog = new FolderBrowserDialog {
                Description = description,
                RootFolder = startFolder,
                ShowNewFolderButton = false
            } ) {
                if ( folderDialog.ShowDialog() == DialogResult.OK ) { return new DirectoryInfo( folderDialog.SelectedPath ); }
            }

            return null;
        }

        // --------------------------- CopyStream ---------------------------
        /// <summary>
        ///     Copies data from a source stream to a target stream.
        /// </summary>
        /// <param name="source">The source stream to copy from.</param>
        /// <param name="target">The destination stream to copy to.</param>
        public static void CopyStream( [NotNull] this Stream source, [NotNull] Stream target ) {
            if ( !source.CanRead ) { throw new Exception( $"Cannot read from {nameof( source )}" ); }

            if ( !target.CanWrite ) { throw new Exception( $"Cannot write to {nameof( target )}" ); }

            const Int32 size = 0xffff;
            var buffer = new Byte[ size ];
            Int32 bytesRead;

            while ( ( bytesRead = source.Read( buffer, 0, size ) ) > 0 ) { target.Write( buffer, 0, bytesRead ); }
        }

        /// <summary>
        ///     Before: @"c:\hello\world".
        ///     After: @"c:\hello\world\23468923475634836.extension"
        /// </summary>
        /// <param name="info">         </param>
        /// <param name="withExtension"></param>
        /// <param name="toBase">       </param>
        /// <returns></returns>
        [NotNull]
        public static FileInfo DateAndTimeAsFile( [NotNull] this DirectoryInfo info, [CanBeNull] String withExtension, Int32 toBase = 16 ) {
            if ( info == null ) { throw new ArgumentNullException( nameof( info ) ); }

            var now = Convert.ToString( DateTime.UtcNow.ToBinary(), toBase: toBase );
            var fileName = $"{now}{withExtension ?? info.Extension}";
            var path = Path.Combine( info.FullName, fileName );

            return new FileInfo( path );
        }

        /// <summary>
        ///     If the <paramref name="directoryInfo" /> does not exist, attempt to create it.
        /// </summary>
        /// <param name="directoryInfo">      </param>
        /// <param name="changeCompressionTo">Suggest if folder comperssion be Enabled or Disabled. Defaults to null.</param>
        /// <param name="requestReadAccess">  </param>
        /// <param name="requestWriteAccess"> </param>
        /// <returns></returns>
        [CanBeNull]
        public static DirectoryInfo Ensure( [NotNull] this DirectoryInfo directoryInfo, Boolean? changeCompressionTo = null, Boolean? requestReadAccess = null, Boolean? requestWriteAccess = null ) {

            if ( directoryInfo == null ) { throw new ArgumentNullException( nameof( directoryInfo ) ); }

            try {
                directoryInfo.Refresh();

                if ( !directoryInfo.Exists ) {
                    directoryInfo.Create();
                    directoryInfo.Refresh();
                }

                if ( changeCompressionTo.HasValue ) {
                    directoryInfo.SetCompression( changeCompressionTo.Value );
                    directoryInfo.Refresh();
                }

                if ( requestReadAccess.HasValue ) { directoryInfo.Refresh(); }

                if ( requestWriteAccess.HasValue ) {
                    var temp = Path.Combine( directoryInfo.FullName, Path.GetRandomFileName() );
                    File.WriteAllText( temp, "Delete Me!" );
                    File.Delete( temp );
                    directoryInfo.Refresh();
                }
            }
            catch ( Exception exception ) {
                exception.Log();

                return null;
            }

            return directoryInfo;
        }

        public static DateTime FileNameAsDateAndTime( [NotNull] this FileInfo info, DateTime? defaultValue = null ) {
            if ( info == null ) { throw new ArgumentNullException( nameof( info ) ); }

            if ( null == defaultValue ) { defaultValue = DateTime.MinValue; }

            var now = defaultValue.Value;
            var fName = Path.GetFileNameWithoutExtension( info.Name );

            if ( String.IsNullOrWhiteSpace( fName ) ) { return now; }

            fName = fName.Trim();

            if ( String.IsNullOrWhiteSpace( fName ) ) { return now; }

            if ( Int64.TryParse( fName, NumberStyles.AllowHexSpecifier, null, out var data ) ) { return DateTime.FromBinary( data ); }

            if ( Int64.TryParse( fName, NumberStyles.Any, null, out data ) ) { return DateTime.FromBinary( data ); }

            return now;
        }

        /// <summary>
        ///     Search the <paramref name="startingFolder" /> for any files matching the <paramref name="fileSearchPatterns" /> .
        /// </summary>
        /// <param name="startingFolder">    The folder to start the search.</param>
        /// <param name="fileSearchPatterns">List of patterns to search for.</param>
        /// <param name="cancellation">      </param>
        /// <param name="onFindFile">        <see cref="Action" /> to perform when a file is found.</param>
        /// <param name="onEachDirectory">   <see cref="Action" /> to perform on each folder found.</param>
        /// <param name="searchStyle">       </param>
        public static void FindFiles( [NotNull] this DirectoryInfo startingFolder, [NotNull] IEnumerable<String> fileSearchPatterns, SimpleCancel cancellation, [CanBeNull] Action<FileInfo> onFindFile = null,
            [CanBeNull] Action<DirectoryInfo> onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
            if ( fileSearchPatterns == null ) { throw new ArgumentNullException( nameof( fileSearchPatterns ) ); }

            if ( startingFolder == null ) { throw new ArgumentNullException( nameof( startingFolder ) ); }

            try {
                var searchPatterns = fileSearchPatterns as IList<String> ?? fileSearchPatterns.ToList();

                searchPatterns.AsParallel().ForAll( searchPattern => {
                    if ( cancellation.HaveAnyCancellationsBeenRequested() ) { return; }

                    try {
                        var folders = startingFolder.BetterEnumerateDirectories( "*" /*, SearchOption.TopDirectoryOnly*/ );

                        folders.AsParallel().ForAll( folder => {
                            if ( cancellation.HaveAnyCancellationsBeenRequested() ) { return; }

                            try { onEachDirectory?.Invoke( folder ); }
                            catch ( Exception exception ) { exception.Log(); }

                            if ( searchStyle == SearchStyle.FoldersFirst ) {
                                folder.FindFiles( fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: onFindFile, onEachDirectory: onEachDirectory,
                                    searchStyle: SearchStyle.FoldersFirst ); //recurse
                            }

                            try {
                                foreach ( var file in folder.BetterEnumerateFiles( searchPattern /*, SearchOption.TopDirectoryOnly*/ ) ) { file.InternalSearchFoundFile( onFindFile, cancellation ); }
                            }
                            catch ( UnauthorizedAccessException ) { }
                            catch ( DirectoryNotFoundException ) { }
                            catch ( IOException ) { }
                            catch ( SecurityException ) { }
                            catch ( AggregateException exception ) {
                                exception.Handle( ex => {
                                    switch ( ex ) {
                                        case UnauthorizedAccessException _:
                                        case DirectoryNotFoundException _:
                                        case IOException _:
                                        case SecurityException _:

                                            return true;
                                    }

                                    ex.Log();

                                    return false;
                                } );
                            }

                            folder.FindFiles( fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: onFindFile, onEachDirectory: onEachDirectory, searchStyle: searchStyle ); //recurse
                        } );
                    }
                    catch ( UnauthorizedAccessException ) { }
                    catch ( DirectoryNotFoundException ) { }
                    catch ( IOException ) { }
                    catch ( SecurityException ) { }
                    catch ( AggregateException exception ) {
                        exception.Handle( ex => {
                            switch ( ex ) {
                                case UnauthorizedAccessException _:
                                case DirectoryNotFoundException _:
                                case IOException _:
                                case SecurityException _:

                                    return true;
                            }

                            ex.Log();

                            return false;
                        } );
                    }
                } );
            }
            catch ( UnauthorizedAccessException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( IOException ) { }
            catch ( SecurityException ) { }
            catch ( AggregateException exception ) {
                exception.Handle( ex => {
                    switch ( ex ) {
                        case UnauthorizedAccessException _:
                        case DirectoryNotFoundException _:
                        case IOException _:
                        case SecurityException _:

                            return true;
                    }

                    ex.Log();

                    return false;
                } );
            }
        }

        public static UInt32? GetFileSizeOnDisk( [NotNull] this Document document ) => GetFileSizeOnDisk( new FileInfo( document.FullPath ) );

        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static UInt32? GetFileSizeOnDisk( [NotNull] this FileInfo info ) {
            UInt32 clusterSize;
            var driveLetter = info.Directory.Root.FullName.TrimEnd( '\\' );

            using ( var searcher = new ManagementObjectSearcher( $"select BlockSize,NumberOfBlocks from Win32_Volume WHERE DriveLetter = '{driveLetter}'" ) ) {
                var bob = searcher.Get().Cast<ManagementObject>().First();
                clusterSize = ( UInt32 )bob[ "BlockSize" ];
            }

            var losize = NativeMethods.GetCompressedFileSizeW( info.FullName, out var hosize );
            var size = hosize << 32 | losize;

            return ( size + clusterSize - 1 ) / clusterSize * clusterSize;
        }

        /// <summary>
        ///     <para>
        ///         The code does not work properly on Windows Server 2008 or 2008 R2 or Windows 7 and Vista based systems as
        ///         cluster size is always zero (GetDiskFreeSpaceW and GetDiskFreeSpace return -1 even with UAC disabled.)
        ///     </para>
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/questions/3750590/get-size-of-file-on-disk" />
        public static UInt32? GetFileSizeOnDiskAlt( [NotNull] this FileInfo info ) {
            var result = NativeMethods.GetDiskFreeSpaceW( lpRootPathName: info.Directory?.Root.FullName, lpSectorsPerCluster: out var sectorsPerCluster, lpBytesPerSector: out var bytesPerSector,
                lpNumberOfFreeClusters: out var dummy, lpTotalNumberOfClusters: out _ );

            if ( result == 0 ) { throw new Win32Exception(); }

            var clusterSize = sectorsPerCluster * bytesPerSector;
            var losize = NativeMethods.GetCompressedFileSizeW( lpFileName: info.FullName, lpFileSizeHigh: out var sizeHigh );
            var size = sizeHigh << 32 | losize;

            return ( size + clusterSize - 1 ) / clusterSize * clusterSize;
        }

        [CanBeNull]
        public static DriveInfo GetLargestEmptiestDrive() => DriveInfo.GetDrives().AsParallel().Where( info => info.IsReady ).OrderByDescending( info => info.AvailableFreeSpace ).FirstOrDefault();

        /// <summary>
        ///     Given the <paramref name="path" /> and <paramref name="searchPattern" /> pick any one file and return the
        ///     <see cref="FileSystemInfo.FullName" /> .
        /// </summary>
        /// <param name="path">         </param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"> </param>
        /// <returns></returns>
        [NotNull]
        public static String GetRandomFile( String path, String searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly ) {
            if ( !Directory.Exists( path ) ) { return String.Empty; }

            var dir = new DirectoryInfo( path );

            if ( !dir.Exists ) { return String.Empty; }

            var files = Directory.EnumerateFiles( dir.FullName, searchPattern: searchPattern, searchOption: searchOption );
            var pickedfile = files.OrderBy( r => Randem.Next() ).FirstOrDefault();

            if ( pickedfile != null && File.Exists( pickedfile ) ) { return new FileInfo( pickedfile ).FullName; }

            return String.Empty;
        }

        /// <summary>
        ///     Warning, this could OOM on a large folder structure.
        /// </summary>
        /// <param name="startingFolder"></param>
        /// <param name="foldersFound">  Warning, this could OOM on a *large* folder structure.</param>
        /// <param name="cancellation">  </param>
        /// <returns></returns>
        public static Boolean GrabAllFolders( [NotNull] this Folder startingFolder, [NotNull] ConcurrentBag<String> foldersFound, SimpleCancel cancellation ) {
            if ( startingFolder == null ) { throw new ArgumentNullException( nameof( startingFolder ) ); }

            if ( foldersFound == null ) { throw new ArgumentNullException( nameof( foldersFound ) ); }

            try {
                if ( cancellation.HaveAnyCancellationsBeenRequested() ) { return false; }

                if ( !startingFolder.Exists() ) { return false; }

                //if ( startingFolder.Name.Like( "$OF" ) ) {return false;}

                /* quick little trick, but doesn't handle unicode properly
                var tempfile = Document.GetTempDocument();

                var bob = Windows.ExecuteCommandPrompt( $"DIR {startingFolder.FullName} /B /S /AD > {tempfile.FullPathWithFileName}" )
                       .Result;
                bob.WaitForExit();

                var lines = File.ReadLines( tempfile.FullPathWithFileName );
                foreach ( var line in lines ) {
                    foldersFound.Add( line );
                }

                tempfile.Delete();
                */

                //foldersFound.Add( startingFolder.FullName );

                //Parallel.ForEach( startingFolder.Info.BetterEnumerateDirectories().AsParallel(), ThreadingExtensions.CPUIntensive, info => GrabAllFolders( new Folder( info.FullName ), foldersFound, cancellation ) );
                foreach ( var info in startingFolder.Info.EnumerateDirectories( "*.*", SearchOption.AllDirectories ) ) {

                    //GrabAllFolders( new Folder( info.FullName ), foldersFound, cancellation );
                    foldersFound.Add( info.FullName );
                }

                return true;
            }
            catch ( OutOfMemoryException ) { GC.Collect(); }
            catch ( Exception exception ) { exception.Log(); }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="startingFolder">        </param>
        /// <param name="documentSearchPatterns"></param>
        /// <param name="onEachDocumentFound">   Warning, this could OOM on a large folder structure.</param>
        /// <param name="cancellation">          </param>
        /// <param name="progressFolders">       </param>
        /// <param name="progressDocuments">     </param>
        /// <returns></returns>
        public static Boolean GrabEntireTree( [NotNull] this IFolder startingFolder, IEnumerable<String> documentSearchPatterns, [NotNull] Action<Document> onEachDocumentFound, IProgress<Int64> progressFolders,
            IProgress<Int64> progressDocuments, [NotNull] CancellationTokenSource cancellation ) {
            if ( startingFolder == null ) { throw new ArgumentNullException( nameof( startingFolder ) ); }

            if ( onEachDocumentFound == null ) { throw new ArgumentNullException( nameof( onEachDocumentFound ) ); }

            //if ( foldersFound == null ) {
            //    throw new ArgumentNullException( nameof( foldersFound ) );
            //}

            if ( cancellation.IsCancellationRequested ) { return false; }

            if ( !startingFolder.Exists() ) { return false; }

            //foldersFound.Add( startingFolder );
            var searchPatterns = documentSearchPatterns as IList<String> ?? documentSearchPatterns.ToList();

            Parallel.ForEach( startingFolder.GetFolders( "*" ).AsParallel(), folder => {
                progressFolders.Report( 1 );
                GrabEntireTree( folder, searchPatterns, onEachDocumentFound, progressFolders, progressDocuments, cancellation );
                progressFolders.Report( -1 );
            } );

            //var list = new List<FileInfo>();
            foreach ( var files in searchPatterns.Select( searchPattern => startingFolder.Info.EnumerateFiles( searchPattern ).OrderBy( info => Randem.Next() ) ) ) {
                foreach ( var info in files ) {
                    progressDocuments.Report( 1 );
                    onEachDocumentFound( new Document( info ) );

                    if ( cancellation.IsCancellationRequested ) { return false; }
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

        [Pure]
        public static Boolean IsDirectory( this NativeMethods.Win32FindData data ) => data.dwFileAttributes.HasFlag( FileAttributes.Directory );

        [Pure]
        public static Boolean IsFile( this NativeMethods.Win32FindData data ) => !IsDirectory( data );

        /// <summary>
        /// Hard coded folders to skip.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean IsIgnoreFolder( this NativeMethods.Win32FindData data ) =>
            data.cFileName.EndsLike( "$RECYCLE.BIN" ) /*|| data.cFileName.Like( "TEMP" ) || data.cFileName.Like( "TMP" )*/ || data.cFileName.Like( "System Volume Information" );

        [Pure]
        public static Boolean IsParentOrCurrent( this NativeMethods.Win32FindData data ) => data.cFileName == "." || data.cFileName == "..";

        [Pure]
        public static Boolean IsProtected( [NotNull] this FileSystemInfo fileSystemInfo ) {
            if ( fileSystemInfo == null ) { throw new ArgumentNullException( nameof( fileSystemInfo ) ); }

            if ( !fileSystemInfo.Exists ) { return false; }

            DirectorySecurity ds;

            try { ds = new DirectorySecurity( fileSystemInfo.FullName, AccessControlSections.Access ); }
            catch ( UnauthorizedAccessException ) { return true; }

            if ( !ds.AreAccessRulesProtected ) { return false; }

            using ( var windowsIdentity = WindowsIdentity.GetCurrent() ) {
                var windowsPrincipal = new WindowsPrincipal( windowsIdentity );
                var isProtected = !windowsPrincipal.IsInRole( WindowsBuiltInRole.Administrator ); // Not running as admin

                return isProtected;
            }
        }

        [Pure]
        public static Boolean IsReparsePoint( this NativeMethods.Win32FindData data ) => data.dwFileAttributes.HasFlag( FileAttributes.ReparsePoint );

        /// <summary>
        ///     Open with Explorer.exe
        /// </summary>
        /// <param name="folder">todo: describe folder parameter on OpenDirectoryWithExplorer</param>
        public static Boolean OpenWithExplorer( [NotNull] this DirectoryInfo folder ) {
            if ( folder == null ) {
                throw new ArgumentNullException( paramName: nameof( folder ) );
            }

            var proc = Process.Start( fileName: $@"{Path.Combine( Windows.WindowsSystem32Folder.Value.FullName, "explorer.exe" )}",
                arguments: $" /separate /select,\"{folder.FullName}\" " );

            return proc?.Responding == true;
        }

        /// <summary>
        ///     Open with Explorer.exe
        /// </summary>
        /// <param name="folder">todo: describe folder parameter on OpenDirectoryWithExplorer</param>
        public static Boolean OpenWithExplorer( [NotNull] this Folder folder ) {
            if ( folder == null ) {
                throw new ArgumentNullException( paramName: nameof( folder ) );
            }

            var proc = Process.Start( fileName: $@"{Path.Combine( Windows.WindowsSystem32Folder.Value.FullName, "explorer.exe" )}",
                arguments: $" /separate /select,\"{folder.FullName}\" " );

            return proc?.Responding == true;
        }

        /// <summary>
        ///     Open with Explorer.exe
        /// </summary>
        public static Boolean OpenWithExplorer( [NotNull] this Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            var proc = Process.Start( fileName: $@"{Path.Combine( Windows.WindowsSystem32Folder.Value.FullName, "explorer.exe" )}",
                arguments: $" /separate /select,\"{document.FullPath}\" " );

            return proc?.Responding == true;
        }

        /// <summary>
        ///     Before: "hello.txt".
        ///     After: "hello 345680969061906730476346.txt"
        /// </summary>
        /// <param name="info">        </param>
        /// <param name="newExtension"></param>
        /// <returns></returns>
        [NotNull]
        public static FileInfo PlusDateTime( [NotNull] this FileInfo info, [CanBeNull] String newExtension = null ) {
            if ( info == null ) { throw new ArgumentNullException( nameof( info ) ); }

            if ( info.Directory == null ) { throw new NullReferenceException( "info.directory" ); }

            var now = Convert.ToString( DateTime.UtcNow.ToBinary(), toBase: 16 );
            var formatted = $"{Path.GetFileNameWithoutExtension( info.Name )} {now}{newExtension ?? info.Extension}";
            var path = Path.Combine( info.Directory.FullName, formatted );

            return new FileInfo( path );
        }

        /// <summary>
        ///     untested. is this written correctly? would it read from a *slow* media but not block the calling function?
        /// </summary>
        /// <param name="filePath">          </param>
        /// <param name="bufferSize">        </param>
        /// <param name="fileMissingRetries"></param>
        /// <param name="retryDelay">        </param>
        /// <returns></returns>
        [ItemNotNull]
        public static async Task<String> ReadTextAsync( [NotNull] String filePath, Int32? bufferSize = 4096, Int32? fileMissingRetries = 10, TimeSpan? retryDelay = null ) {
            if ( String.IsNullOrWhiteSpace( filePath ) ) { throw new ArgumentNullException( nameof( filePath ) ); }

            if ( !bufferSize.HasValue ) { bufferSize = 4096; }

            while ( fileMissingRetries.HasValue && fileMissingRetries.Value > 0 ) {
                if ( File.Exists( filePath ) ) { break; }

                await Task.Delay( retryDelay ?? Seconds.One );
                fileMissingRetries--;
            }

            if ( File.Exists( filePath ) ) {
                try {
                    using ( var sourceStream = new FileStream( filePath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: bufferSize.Value, useAsync: true ) ) {
                        var sb = new StringBuilder( bufferSize.Value );
                        var buffer = new Byte[ bufferSize.Value ];
                        Int32 numRead;

                        while ( ( numRead = await sourceStream.ReadAsync( buffer, 0, buffer.Length ) ) != 0 ) {
                            var text = Encoding.Unicode.GetString( buffer, 0, numRead );
                            sb.Append( text );
                        }

                        return sb.ToString();
                    }
                }
                catch ( FileNotFoundException exception ) { exception.Log(); }
            }

            return String.Empty;
        }

        /// <summary>
        ///     Retry the <paramref name="ioFunction" /> if an <see cref="IOException" /> occurs.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="ioFunction"></param>
        /// <param name="tryFor">    </param>
        /// <param name="token">     </param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        [CanBeNull]
        public static TResult ReTry<TResult>( [NotNull] this Func<TResult> ioFunction, TimeSpan tryFor, CancellationToken token ) {
            if ( ioFunction == null ) { throw new ArgumentNullException( nameof( ioFunction ) ); }

            //var oneTenth = TimeSpan.FromMilliseconds( tryFor.TotalMilliseconds / 10 );
            var stopwatch = Stopwatch.StartNew();
            TryAgain:

            if ( token.IsCancellationRequested ) { return default; }

            try {
                Application.DoEvents();

                return ioFunction();
            }
            catch ( IOException exception ) {
                exception.Message.Error();

                if ( stopwatch.Elapsed > tryFor ) { return default; }

                Thread.CurrentThread.Fraggle( Seconds.One );

                goto TryAgain;
            }
        }

        /// <summary>
        ///     <para>performs a byte by byte file comparison</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static Boolean SameContent( [CanBeNull] this FileInfo left, [CanBeNull] FileInfo right ) {
            if ( left == null || right == null ) { return false; }

            if ( !left.Exists ) { return false; }

            if ( !right.Exists ) { return false; }

            if ( left.Length != right.Length ) { return false; }

            var lba = left.AsBytes(); //.ToArray();
            var rba = right.AsBytes(); //.ToArray();

            return lba.SequenceEqual( rba );
        }

        /// <summary>
        ///     <para>performs a byte by byte file comparison</para>
        /// </summary>
        /// <param name="leftFileName"> </param>
        /// <param name="rightFileName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static Boolean SameContent( [CanBeNull] this String leftFileName, [CanBeNull] String rightFileName ) {
            if ( leftFileName == null || rightFileName == null ) { return false; }

            if ( !File.Exists( leftFileName ) ) { return false; }

            if ( !File.Exists( rightFileName ) ) { return false; }

            if ( leftFileName.Length != rightFileName.Length ) { return false; }

            var lba = leftFileName.AsBytes().ToArray();
            var rba = rightFileName.AsBytes().ToArray();

            return lba.SequenceEqual( rba );
        }

        /// <summary>
        ///     <para>performs a byte by byte file comparison</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static Boolean SameContent( [CanBeNull] this Document left, [CanBeNull] FileInfo right ) {
            if ( left == null || right == null ) { return false; }

            if ( left.Exists() == false ) {
                return false;
            }

            var leftLength = left.Length;
            if ( !leftLength.HasValue ) {
                return false;
            }

            if ( !right.Exists ) {
                right.Refresh();

                if ( !right.Exists ) {
                    return false;
                }
            }

            var rightLength = ( UInt64 )right.Length;

            if ( !rightLength.Any() ) {
                return false;
            }

            return leftLength.Value == rightLength && left.AsBytes().SequenceEqual( right.AsBytes() );
        }

        /// <summary>
        ///     <para>performs a byte by byte file comparison</para>
        /// </summary>
        /// <param name="right"> </param>
        /// <param name="left"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static Boolean SameContent( [CanBeNull] this FileInfo left, [CanBeNull] Document right ) {
            if ( left == default || right == default ) { return false; }

            if ( !left.Exists ) {
                left.Refresh();

                if ( !left.Exists ) {
                    return false;
                }
            }

            var rightLength = ( UInt64 )left.Length;

            if ( !rightLength.Any() ) {
                return false;
            }

            if ( right.Exists() == false ) {
                return false;
            }

            var leftLength = right.Length;
            if ( !leftLength.HasValue ) {
                return false;
            }

            return leftLength.Value == rightLength && right.AsBytes().SequenceEqual( left.AsBytes() );
        }

        /// <summary>
        ///     Search all possible drives for any files matching the <paramref name="fileSearchPatterns" /> .
        /// </summary>
        /// <param name="fileSearchPatterns">List of patterns to search for.</param>
        /// <param name="cancellation">      </param>
        /// <param name="onFindFile">        <see cref="Action" /> to perform when a file is found.</param>
        /// <param name="onEachDirectory">   <see cref="Action" /> to perform on each folder found.</param>
        /// <param name="searchStyle">       </param>
        public static void SearchAllDrives( [NotNull] this IEnumerable<String> fileSearchPatterns, SimpleCancel cancellation, [CanBeNull] Action<FileInfo> onFindFile = null,
            [CanBeNull] Action<DirectoryInfo> onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
            if ( fileSearchPatterns == null ) { throw new ArgumentNullException( nameof( fileSearchPatterns ) ); }

            try {
                DriveInfo.GetDrives().AsParallel().WithDegreeOfParallelism( 26 ).WithExecutionMode( ParallelExecutionMode.ForceParallelism ).ForAll( drive => {
                    if ( !drive.IsReady || drive.DriveType == DriveType.NoRootDirectory || !drive.RootDirectory.Exists ) { return; }

                    $"Scanning [{drive.VolumeLabel}]".Info();
                    drive.RootDirectory.FindFiles( fileSearchPatterns: fileSearchPatterns, cancellation: cancellation, onFindFile: onFindFile, onEachDirectory: onEachDirectory, searchStyle: searchStyle );
                } );
            }
            catch ( UnauthorizedAccessException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( IOException ) { }
            catch ( SecurityException ) { }
            catch ( AggregateException exception ) {
                exception.Handle( ex => {
                    switch ( ex ) {
                        case UnauthorizedAccessException _:
                        case DirectoryNotFoundException _:
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

        public static Boolean SetCompression( this DirectoryInfo directoryInfo, Boolean compressed = true ) {
            try {
                if ( directoryInfo.Exists ) {
                    using ( var dir = new ManagementObject( directoryInfo.ToManagementPath() ) ) {
                        var outParams = dir.InvokeMethod( compressed ? "Compress" : "Uncompress", null, null );

                        if ( null == outParams ) { return false; }

                        var result = Convert.ToUInt32( outParams.Properties[ "ReturnValue" ].Value );

                        return result == 0;
                    }
                }
            }
            catch ( ManagementException exception ) { exception.Log(); }

            return false;
        }

        public static Boolean SetCompression( [CanBeNull] this String folderPath, Boolean compressed = true ) {
            if ( String.IsNullOrWhiteSpace( folderPath ) ) { return false; }

            try {
                var dirInfo = new DirectoryInfo( folderPath );

                return dirInfo.SetCompression( compressed );
            }
            catch ( Exception exception ) { exception.Log(); }

            return false;
        }

        //public static DriveInfo GetDriveWithLargestAvailableFreeSpace() {
        //	return DriveInfo.GetDrives().AsParallel().Where( info => info.IsReady ).FirstOrDefault( driveInfo => driveInfo.AvailableFreeSpace >= DriveInfo.GetDrives().AsParallel().Max( info => info.AvailableFreeSpace ) );
        //}

        [NotNull]
        public static String SimplifyFileName( [NotNull] this Document document /*, Folder hintFolder*/ ) {
            if ( document == null ) { throw new ArgumentNullException( nameof( document ) ); }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension( document.FileName );

            TryAgain:

            //check for a double extension (image.jpg.tif), remove the 'fakeish' (.tif) extension
            if ( !Path.GetExtension( fileNameWithoutExtension ).IsNullOrEmpty() ) {
                fileNameWithoutExtension = Path.GetFileNameWithoutExtension( fileNameWithoutExtension );

                goto TryAgain;
            }

            //TODO we have the document, see if we can just chop off down to a nonexistent filename.. just get rid of (3) or (2) or (1)

            var splitIntoWords = fileNameWithoutExtension.Split( new[] {
                ' '
            }, StringSplitOptions.RemoveEmptyEntries ).ToList();

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

        [NotNull]
        public static ManagementPath ToManagementPath( [NotNull] this DirectoryInfo systemPath ) {
            var fullPath = systemPath.FullName;

            while ( fullPath.EndsWith( @"\", StringComparison.Ordinal ) ) { fullPath = fullPath.Substring( 0, fullPath.Length - 1 ); }

            fullPath = "Win32_Directory.Name=\"" + fullPath.Replace( "\\", "\\\\" ) + "\"";
            var managed = new ManagementPath( fullPath );

            return managed;
        }

        [NotNull]
        public static IEnumerable<String> ToPaths( [NotNull] this DirectoryInfo directoryInfo ) {
            if ( directoryInfo == null ) { throw new ArgumentNullException( nameof( directoryInfo ) ); }

            return directoryInfo.ToString().Split( Path.DirectorySeparatorChar );
        }

        [NotNull]
        public static MemoryStream TryCopyStream( String filePath, Boolean bePatient = true, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite ) {

            //TODO
            TryAgain:
            var memoryStream = new MemoryStream();

            try {
                if ( File.Exists( filePath ) ) {
                    using ( var fileStream = File.Open( filePath, mode: fileMode, access: fileAccess, share: fileShare ) ) {
                        var length = ( Int32 )fileStream.Length;

                        if ( length > 0 ) {
                            fileStream.CopyTo( memoryStream, length ); //BUG int-long possible issue.
                            memoryStream.Seek( 0, SeekOrigin.Begin );
                        }
                    }
                }
            }
            catch ( IOException ) {

                // IOExcception is thrown if the file is in use by another process.
                if ( bePatient ) {
                    if ( !Thread.Yield() ) { Thread.Sleep( 0 ); }

                    goto TryAgain;
                }
            }

            return memoryStream;
        }

        public static Boolean TryGetFolderFromPath( this TrimmedString path, [CanBeNull] out DirectoryInfo directoryInfo, [CanBeNull] out Uri uri ) => TryGetFolderFromPath( path.Value, out directoryInfo, out uri );

        public static Boolean TryGetFolderFromPath( [CanBeNull] this String path, [CanBeNull] out DirectoryInfo directoryInfo, [CanBeNull] out Uri uri ) {

            directoryInfo = null;
            uri = null;

            try {
                if ( String.IsNullOrWhiteSpace( value: path ) ) {
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

        /// <summary>
        ///     Returns a temporary <see cref="Document" /> (but does not create the file in the file system).
        /// </summary>
        /// <param name="folder">   </param>
        /// <param name="document"> </param>
        /// <param name="extension"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Boolean TryGetTempDocument( [NotNull] this Folder folder, [NotNull] out Document document, String extension = null ) {
            if ( folder == null ) { throw new ArgumentNullException( nameof( folder ) ); }

            try {
                var randomFileName = Guid.NewGuid().ToString();

                if ( String.IsNullOrWhiteSpace( extension ) ) { randomFileName = Path.Combine( folder.FullName, Path.GetFileName( randomFileName ) ); }
                else {
                    if ( !extension.StartsWith( "." ) ) { extension = $".{extension}"; }

                    randomFileName = Path.Combine( folder.FullName, Path.GetFileNameWithoutExtension( randomFileName ) + extension );
                }

                document = new Document( randomFileName );

                return true;
            }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) { }
            catch ( NotSupportedException ) { }
            catch ( UnauthorizedAccessException ) { }

            // ReSharper disable once AssignNullToNotNullAttribute
            document = default;

            return false;
        }

        /// <summary>
        ///     Tries to open a file, with a user defined number of attempt and Sleep delay between attempts.
        /// </summary>
        /// <param name="filePath">  The full file path to be opened</param>
        /// <param name="fileMode">  Required file mode enum value(see MSDN documentation)</param>
        /// <param name="fileAccess">Required file access enum value(see MSDN documentation)</param>
        /// <param name="fileShare"> Required file share enum value(see MSDN documentation)</param>
        /// <returns>
        ///     A valid FileStream object for the opened file, or null if the File could not be opened after the required
        ///     attempts
        /// </returns>
        [CanBeNull]
        public static FileStream TryOpen( String filePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare ) {

            //TODO
            try { return File.Open( filePath, mode: fileMode, access: fileAccess, share: fileShare ); }
            catch ( IOException ) {

                // IOExcception is thrown if the file is in use by another process.
            }

            return null;
        }

        [CanBeNull]
        public static FileStream TryOpenForReading( String filePath, Boolean bePatient = true, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite ) {

            //TODO
            TryAgain:

            try {
                if ( File.Exists( filePath ) ) { return File.Open( filePath, mode: fileMode, access: fileAccess, share: fileShare ); }
            }
            catch ( IOException ) {

                // IOExcception is thrown if the file is in use by another process.
                if ( !bePatient ) { return null; }

                if ( !Thread.Yield() ) { Thread.Sleep( 0 ); }

                goto TryAgain;
            }

            return null;
        }

        [CanBeNull]
        public static FileStream TryOpenForWriting( String filePath, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.Write, FileShare fileShare = FileShare.ReadWrite ) {

            //TODO
            try { return File.Open( filePath, mode: fileMode, access: fileAccess, share: fileShare ); }
            catch ( IOException ) {

                // IOExcception is thrown if the file is in use by another process.
            }

            return null;
        }

        public static Int32? TurnOnCompression( [NotNull] this FileInfo info ) {
            if ( info == null ) { throw new ArgumentNullException( nameof( info ) ); }

            if ( !info.Exists ) {
                info.Refresh();

                if ( !info.Exists ) { return null; }
            }

            var lpBytesReturned = 0;
            Int16 compressionFormatDefault = 1;

            using ( var fileStream = File.Open( info.FullName, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.None ) ) {
                var success = false;

                try {
                    if ( fileStream.SafeFileHandle != null ) {
                        fileStream.SafeFileHandle.DangerousAddRef( success: ref success );

                        NativeMethods.DeviceIoControl( fileStream.SafeFileHandle.DangerousGetHandle(), FsctlSetCompression, ref compressionFormatDefault, sizeof( Int16 ), IntPtr.Zero, nOutBufferSize: 0,
                            lpBytesReturned: ref lpBytesReturned, lpOverlapped: IntPtr.Zero );
                    }
                }
                finally { fileStream.SafeFileHandle?.DangerousRelease(); }

                return lpBytesReturned;
            }
        }

        /// <summary>
        ///     (does not create path)
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="d">       </param>
        /// <returns></returns>
        [NotNull]
        public static DirectoryInfo WithShortDatePath( [NotNull] this DirectoryInfo basePath, DateTime d ) {
            var path = Path.Combine( basePath.FullName, d.Year.ToString(), d.DayOfYear.ToString(), d.Hour.ToString() );

            return new DirectoryInfo( path );
        }

        // return false; } exception.Log(); catch ( Exception exception ) { } }

        // return true; notifier.Show( toast );

        // // Send the toast. toast.SuppressPopup = !popup; // center without producing a popup.

        //            // Set SuppressPopup = true on the toast in order to send it directly to action
        //            var toast = CreateTextOnlyToast( header, body, longDuration );
        //        if ( notifier.Setting == NotificationSetting.Enabled ) {
        //    try {
        //    var notifier = ToastNotificationManager.CreateToastNotifier( applicationId );
        //public static Boolean TryToast( this String applicationId, String header, String body, Boolean longDuration = false, Boolean popup = true ) {
        ///// <returns></returns>
        ///// <param name="popup"></param>
        ///// <param name="longDuration"></param>
        ///// <param name="body"></param>
        ///// <param name="header"></param>
        ///// <param name="applicationId"></param>
        ///// </summary>
        /////     Where does this method belong?

        ///// <summary>
        //}
        //    return new ToastNotification( xml );

        // // Create the actual toast object using this toast specification. } toastNode?.SetAttribute( "duration", "long" ); var toastNode = xml.SelectSingleNode( "/toast" ) as XmlElement; // Set the duration on the toast

        // if ( longDuration ) { textElements[ 1 ].AppendChild( xml.CreateTextNode( body ) ); textElements[ 0 ].AppendChild( xml.CreateTextNode( heading ) ); // treated as header text, and will be bold.

        // // Set the text on the toast. The first line of text in the ToastText02 template is var textElements = xml.GetElementsByTagName( "text" );

        //    //Find the text component of the content
        //    var xml = ToastNotificationManager.GetTemplateContent( ToastTemplateType.ToastText02 );
        //    // can change the text.
        //    // Using the ToastText02 toast template. Retrieve the content part of the toast so we
        //public static ToastNotification CreateTextOnlyToast( this String heading, String body, Boolean longDuration = false ) {
        ///// </remarks>
        /////     (http://msdn.microsoft.com/en-us/Library/windows/apps/hh761494.aspx)
        /////     Note: All toast templates available in the Toast Template Catalog
        ///// <remarks>
        ///// <returns></returns>
        ///// <param name="longDuration"></param>
        ///// <param name="body"></param>
        ///// <param name="heading"></param>

        /*
                public static DateTime? GetProperteryAsDateTime( [CanBeNull] this PropertyItem item ) {
                    if ( null == item ) {
                        return null;
                    }

                    var value = Encoding.ASCII.GetString( item.Value );
                    if ( value.EndsWith( "\0" ) ) {
                        value = value.Replace( "\0", String.Empty );
                    }

                    if ( value == "0000:00:00 00:00:00" ) {
                        return null;
                    }

                    DateTime result;
                    if ( DateTime.TryParse( value, out result ) ) {
                        return result;
                    }

                    if ( DateTime.TryParseExact( value, "yyyy:MM:dd HH:mm:ss", CultureInfo.Ordinal, DateTimeStyles.AllowWhiteSpaces, out result ) ) {
                        return result;
                    }

                    return null;
                }
        */

        //}
        ///// </summary>
    }
}