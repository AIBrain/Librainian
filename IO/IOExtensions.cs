#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/IOExtensions.cs" was last cleaned by Rick on 2014/09/06 at 7:28 AM

#endregion License & Information

namespace Librainian.IO {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Annotations;
    using Controls;
    using Extensions;
    using FluentAssertions;
    using Maths;
    using Measurement.Time;
    using Microsoft.Scripting.Math;
    using Microsoft.VisualBasic.Devices;
    using Microsoft.VisualBasic.FileIO;
    using NUnit.Framework;
    using Parsing;
    using Threading;
    using SearchOption = System.IO.SearchOption;

    public static class IOExtensions {
        public const int FSCTL_SET_COMPRESSION = 0x9C040;
        public static readonly HashSet<DirectoryInfo> SystemFolders = new HashSet<DirectoryInfo>();

        static IOExtensions() {
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.System ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.SystemX86 ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.AdminTools ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.CDBurning ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.Windows ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.Cookies ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.History ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.InternetCache ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.PrinterShortcuts ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86 ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.Programs ) ) );
            SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.SendTo ) ) );
            SystemFolders.Add( new DirectoryInfo( Path.GetTempPath() ) );

            //TODO foreach on Environment.SpecialFolder
        }

        /// <summary>
        ///     Example: WriteTextAsync( fullPath: fullPath, text: message ).Wait();
        ///     Example: await WriteTextAsync( fullPath: fullPath, text: message );
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async void AppendTextAsync( this FileInfo fileInfo, String text ) {
            if ( fileInfo == null ) {
                throw new ArgumentNullException( "fileInfo" );
            }
            if ( String.IsNullOrWhiteSpace( fileInfo.FullName ) || String.IsNullOrWhiteSpace( text ) ) {
                return;
            }
            try {

                //using ( var str = new StreamWriter( fileInfo.FullName, true, Encoding.Unicode ) ) { return str.WriteLineAsync( text ); }
                var encodedText = Encoding.Unicode.GetBytes( text );
                var length = encodedText.Length;

                //hack
                //using ( var bob = File.Create( fileInfo.FullName, length, FileOptions.Asynchronous | FileOptions.RandomAccess | FileOptions.WriteThrough  ) ) {
                //    bob.WriteAsync
                //}

                using ( var sourceStream = new FileStream( path: fileInfo.FullName, mode: FileMode.Append, access: FileAccess.Write, share: FileShare.Write, bufferSize: length, useAsync: true ) ) {
                    await sourceStream.WriteAsync( buffer: encodedText, offset: 0, count: length );
                    await sourceStream.FlushAsync();
                }
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.Error();
            }
            catch ( ArgumentNullException exception ) {
                exception.Error();
            }
            catch ( DirectoryNotFoundException exception ) {
                exception.Error();
            }
            catch ( PathTooLongException exception ) {
                exception.Error();
            }
            catch ( SecurityException exception ) {
                exception.Error();
            }
            catch ( IOException exception ) {
                exception.Error();
            }
        }

        /// <summary>
        ///     Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static IEnumerable<byte> AsByteArray( [NotNull] this FileInfo fileInfo ) {
            if ( fileInfo == null ) {
                throw new ArgumentNullException( "fileInfo" );
            }
            if ( !fileInfo.Exists ) {
                fileInfo.Refresh(); //check one more time
                if ( !fileInfo.Exists ) {
                    yield break;
                }
            }

            using ( var stream = new FileStream( fileInfo.FullName, FileMode.Open ) ) {
                if ( !stream.CanRead ) {
                    throw new NotSupportedException( String.Format( "Cannot read from file {0}", fileInfo.FullName ) );
                }

                using ( var buffered = new BufferedStream( stream ) ) {
                    var b = buffered.ReadByte();
                    if ( b == -1 ) {
                        yield break;
                    }
                    yield return ( Byte )b;
                }
            }
        }

        /// <summary>
        ///     ask user for folder/network path where to store dictionary
        /// </summary>
        [CanBeNull]
        public static Folder AskUserForStorageFolder( String hint ) {
            var folderBrowserDialog = new FolderBrowserDialog {
                ShowNewFolderButton = true,
                Description = String.Format( "Please direct me to a storage folder for {0}.", hint ),
                RootFolder = Environment.SpecialFolder.MyComputer
            };

            var owner = WindowWrapper.CreateWindowWrapper( Diagnostical.CurrentProcess.MainWindowHandle );

            var dialog = folderBrowserDialog.ShowDialog( owner );

            if ( dialog != DialogResult.OK || folderBrowserDialog.SelectedPath.IsNullOrWhiteSpace() ) {
                return null;
            }
            return new Folder( folderBrowserDialog.SelectedPath );
        }

        /// <summary>
        ///     Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        // TODO this needs a unit test for endianness
        public static IEnumerable<ushort> AsUInt16Array( [NotNull] this FileInfo fileInfo ) {
            if ( fileInfo == null ) {
                throw new ArgumentNullException( "fileInfo" );
            }
            if ( !fileInfo.Exists ) {
                fileInfo.Refresh(); //check one more time
                if ( !fileInfo.Exists ) {
                    yield break;
                }
            }

            using ( var stream = new FileStream( fileInfo.FullName, FileMode.Open ) ) {
                if ( !stream.CanRead ) {
                    throw new NotSupportedException( String.Format( "Cannot read from file {0}", fileInfo.FullName ) );
                }

                using ( var buffered = new BufferedStream( stream ) ) {
                    var low = buffered.ReadByte();
                    if ( low == -1 ) {
                        yield break;
                    }

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
        ///     No guarantee of return order. Also, because of the way the operating system works
        ///     (random-access), a directory may be created or deleted even after a search.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> BetterEnumerateDirectories( this DirectoryInfo target, String searchPattern = "*" ) {
            if ( null == target ) {
                yield break;
            }
            var searchPath = Path.Combine( target.FullName, searchPattern );
            NativeWin32.Win32FindData findData;
            using ( var hFindFile = NativeWin32.FindFirstFile( searchPath, out findData ) ) {
                do {
                    if ( hFindFile.IsInvalid ) {
                        break;
                    }

                    if ( IsParentOrCurrent( findData ) ) {
                        continue;
                    }

                    if ( IsReparsePoint( findData ) ) {
                        continue;
                    }

                    if ( !IsDirectory( findData ) ) {
                        continue;
                    }

                    if ( IsIgnoreFolder( findData ) ) {
                        continue;
                    }

                    var subFolder = Path.Combine( target.FullName, findData.cFileName );

                    // @"\\?\" +System.IO.PathTooLongException
                    if ( subFolder.Length >= 260 ) {
                        continue; //HACK
                    }

                    var subInfo = new DirectoryInfo( subFolder );

                    if ( IsProtected( subInfo ) ) {
                        continue;
                    }

                    yield return subInfo;

                    foreach ( var info in subInfo.BetterEnumerateDirectories( searchPattern ) ) {
                        yield return info;
                    }
                } while ( NativeWin32.FindNextFile( hFindFile, out findData ) );
            }
        }

        public static IEnumerable<FileInfo> BetterEnumerateFiles( [NotNull] this DirectoryInfo target, [NotNull] String searchPattern = "*" ) {
            if ( target == null ) {
                throw new ArgumentNullException( "target" );
            }
            if ( searchPattern == null ) {
                throw new ArgumentNullException( "searchPattern" );
            }

            //if ( null == target ) {
            //    yield break;
            //}
            var searchPath = Path.Combine( target.FullName, searchPattern );
            NativeWin32.Win32FindData findData;
            using ( var hFindFile = NativeWin32.FindFirstFile( searchPath, out findData ) ) {
                do {

                    //Application.DoEvents();

                    if ( hFindFile.IsInvalid ) {
                        break;
                    }

                    if ( IsParentOrCurrent( findData ) ) {
                        continue;
                    }
                    if ( IsReparsePoint( findData ) ) {
                        continue;
                    }

                    if ( !IsFile( findData ) ) {
                        continue;
                    }

                    var newfName = Path.Combine( target.FullName, findData.cFileName );
                    yield return new FileInfo( newfName );
                } while ( NativeWin32.FindNextFile( hFindFile, out findData ) );
            }
        }

        /// <summary>
        ///     poor mans crc
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static int CalcHash( [NotNull] this FileInfo fileInfo ) {
            if ( fileInfo == null ) {
                throw new ArgumentNullException( "fileInfo" );
            }

            return fileInfo.AsByteArray().Aggregate( 0, ( current, b ) => current.GetHashMerge( b ) );
        }

        /// <summary>
        ///     poor mans crc
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static int CalcHash( [NotNull] this Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( "document" );
            }

            var fileInfo = new FileInfo( document.FullPathWithFileName );
            if ( fileInfo == null ) {
                throw new NullReferenceException( "fileInfo" );
            }

            return fileInfo.AsByteArray().Aggregate( 0, ( current, b ) => current.GetHashMerge( b ) );
        }

        [CanBeNull]
        public static DirectoryInfo ChooseDirectoryDialog( this Environment.SpecialFolder startFolder, String path, String description = "Please select a folder." ) {
            using ( var folderDialog = new FolderBrowserDialog {
                Description = description,
                RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = false
            } ) {
                if ( folderDialog.ShowDialog() == DialogResult.OK ) {
                    return new DirectoryInfo( folderDialog.SelectedPath );
                }
            }
            return null;
        }

        public static byte[] Compress( [NotNull] this byte[] data ) {
            if ( data == null ) {
                throw new ArgumentNullException( "data" );
            }
            using ( var output = new MemoryStream() ) {
                using ( var compress = new GZipStream( output, CompressionMode.Compress ) ) {
                    compress.Write( data, 0, data.Length );
                }
                return output.ToArray();
            }
        }

        /// <summary>
        ///     Starts a task to copy a file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="progress"></param>
        /// <param name="eta"></param>
        /// <returns></returns>
        public static Task Copy( Document source, Document destination, Action<double> progress, Action<TimeSpan> eta ) {
            return Task.Run( () => {
                var computer = new Computer();

                //TODO file monitor/watcher?
                computer.FileSystem.CopyFile( source.FullPathWithFileName, destination.FullPathWithFileName, UIOption.AllDialogs, UICancelOption.DoNothing );
            } );
        }

        /// <summary>
        ///     Before: @"c:\hello\world".
        ///     After: @"c:\hello\world\23468923475634836.extension"
        /// </summary>
        /// <param name="info"></param>
        /// <param name="withExtension"></param>
        /// <param name="toBase"></param>
        /// <returns></returns>
        public static FileInfo DateAndTimeAsFile( this DirectoryInfo info, String withExtension, int toBase = 16 ) {
            if ( info == null ) {
                throw new ArgumentNullException( "info" );
            }

            var now = Convert.ToString( value: DateTime.UtcNow.ToBinary(), toBase: toBase );
            var fileName = String.Format( "{0}{1}", now, withExtension ?? info.Extension );
            var path = Path.Combine( info.FullName, fileName );
            return new FileInfo( path );
        }

        [DllImport( "kernel32.dll" )]
        public static extern int DeviceIoControl( IntPtr hDevice, int dwIoControlCode, ref short lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped );

        /// <summary>
        ///     If the <paramref name="directoryInfo" /> does not exist, attempt to create it.
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="changeCompressionTo">
        ///     Suggest if folder comperssion be Enabled or Disabled. Defaults to null.
        /// </param>
        /// <param name="requestReadAccess"></param>
        /// <param name="requestWriteAccess"></param>
        /// <returns></returns>
        public static DirectoryInfo Ensure( this DirectoryInfo directoryInfo, Boolean? changeCompressionTo = null, Boolean? requestReadAccess = null, Boolean? requestWriteAccess = null ) {
            Assert.NotNull( directoryInfo );
            if ( directoryInfo == null ) {
                throw new ArgumentNullException( "directoryInfo" );
            }
            try {
                Assert.False( String.IsNullOrWhiteSpace( directoryInfo.FullName ) );
                directoryInfo.Refresh();
                if ( !directoryInfo.Exists ) {
                    directoryInfo.Create();
                    directoryInfo.Refresh();
                }

                if ( changeCompressionTo.HasValue ) {
                    directoryInfo.SetCompression( changeCompressionTo.Value );
                    directoryInfo.Refresh();
                }

                if ( requestReadAccess.HasValue ) {
                    directoryInfo.Refresh();
                }

                if ( requestWriteAccess.HasValue ) {
                    var temp = Path.Combine( directoryInfo.FullName, Path.GetRandomFileName() );
                    File.WriteAllText( temp, "Delete Me!" );
                    File.Delete( temp );
                    directoryInfo.Refresh();
                }
                Assert.True( directoryInfo.Exists );
            }
            catch ( Exception exception ) {
                exception.Error();
                return null;
            }
            return directoryInfo;
        }

        public static DateTime FileNameAsDateAndTime( this FileInfo info, DateTime? defaultValue = null ) {
            if ( info == null ) {
                throw new ArgumentNullException( "info" );
            }

            if ( null == defaultValue ) {
                defaultValue = DateTime.MinValue;
            }

            var now = defaultValue.Value;
            var fName = Path.GetFileNameWithoutExtension( info.Name );

            if ( String.IsNullOrWhiteSpace( fName ) ) {
                return now;
            }

            fName = fName.Trim();
            if ( String.IsNullOrWhiteSpace( fName ) ) {
                return now;
            }

            long data;

            if ( Int64.TryParse( fName, NumberStyles.AllowHexSpecifier, null, out data ) ) {
                return DateTime.FromBinary( data );
            }

            if ( Int64.TryParse( fName, NumberStyles.Any, null, out data ) ) {
                return DateTime.FromBinary( data );
            }

            return now;
        }

        /// <summary>
        ///     Search the <paramref name="startingFolder" /> for any files matching the
        ///     <paramref
        ///         name="fileSearchPatterns" />
        ///     .
        /// </summary>
        /// <param name="fileSearchPatterns">List of patterns to search for.</param>
        /// <param name="startingFolder">The folder to start the search.</param>
        /// <param name="cancellationToken"></param>
        /// <param name="onFindFile"><see cref="Action" /> to perform when a file is found.</param>
        /// <param name="onEachDirectory"><see cref="Action" /> to perform on each folder found.</param>
        /// <param name="searchStyle"></param>
        public static void FindFiles( IEnumerable<string> fileSearchPatterns, DirectoryInfo startingFolder, CancellationToken cancellationToken, Action<FileInfo> onFindFile = null, Action<DirectoryInfo> onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
            if ( fileSearchPatterns == null ) {
                throw new ArgumentNullException( "fileSearchPatterns" );
            }
            if ( startingFolder == null ) {
                throw new ArgumentNullException( "startingFolder" );
            }
            try {
                var searchPatterns = fileSearchPatterns as IList<string> ?? fileSearchPatterns.ToList();
                searchPatterns.AsParallel().WithDegreeOfParallelism( 1 ).ForAll( searchPattern => {
#if DEEPDEBUG
                    String.Format( "Searching folder {0} for {1}.", startingFolder.FullName, searchPattern ).TimeDebug();
#endif
                    if ( cancellationToken.IsCancellationRequested ) {
                        return;
                    }
                    try {
                        var folders = startingFolder.EnumerateDirectories( "*", SearchOption.TopDirectoryOnly );
                        folders.AsParallel().WithDegreeOfParallelism( 1 ).ForAll( folder => {
#if DEEPDEBUG

                            String.Format( "Found folder {0}.", folder ).TimeDebug();
#endif
                            if ( cancellationToken.IsCancellationRequested ) {
                                return;
                            }
                            try {
                                if ( onEachDirectory != null ) {
                                    onEachDirectory( folder );
                                }
                            }
                            catch ( Exception exception ) {
                                exception.Error();
                            }
                            if ( searchStyle == SearchStyle.FoldersFirst ) {
                                FindFiles( fileSearchPatterns: searchPatterns, cancellationToken: cancellationToken, startingFolder: folder, onFindFile: onFindFile, onEachDirectory: onEachDirectory, searchStyle: searchStyle ); //recurse
                            }

                            try {
                                var files = folder.EnumerateFiles( searchPattern, SearchOption.TopDirectoryOnly );
                                files.AsParallel().WithDegreeOfParallelism( 1 ).ForAll( file => {

                                    //String.Format( "Found file {0}.", file ).TimeDebug();
                                    if ( cancellationToken.IsCancellationRequested ) {
                                        return;
                                    }
                                    try {
                                        if ( onFindFile != null ) {
                                            onFindFile( file );
                                        }
                                    }
                                    catch ( Exception exception ) {
                                        exception.Error();
                                    }
                                } );
#if DEEPDEBUG
                                String.Format( "Done searching {0} for {1}.", folder.Name, searchPattern ).TimeDebug();
#endif
                            }
                            catch ( UnauthorizedAccessException ) {
                            }
                            catch ( DirectoryNotFoundException ) {
                            }
                            catch ( IOException ) {
                            }
                            catch ( SecurityException ) {
                            }
                            catch ( AggregateException exception ) {
                                exception.Handle( ex => {
                                    if ( ex is UnauthorizedAccessException ) {
                                        return true;
                                    }
                                    if ( ex is DirectoryNotFoundException ) {
                                        return true;
                                    }
                                    if ( ex is IOException ) {
                                        return true;
                                    }
                                    if ( ex is SecurityException ) {
                                        return true;
                                    }
                                    ex.Error();
                                    return false;
                                } );
                            }

                            if ( searchStyle == SearchStyle.FilesFirst ) {
                                FindFiles( fileSearchPatterns: searchPatterns, cancellationToken: cancellationToken, startingFolder: folder, onFindFile: onFindFile, onEachDirectory: onEachDirectory, searchStyle: searchStyle ); //recurse
                            }
                            else {
                                FindFiles( fileSearchPatterns: searchPatterns, cancellationToken: cancellationToken, startingFolder: folder, onFindFile: onFindFile, onEachDirectory: onEachDirectory, searchStyle: searchStyle ); //recurse
                            }
                        } );
                    }
                    catch ( UnauthorizedAccessException ) {
                    }
                    catch ( DirectoryNotFoundException ) {
                    }
                    catch ( IOException ) {
                    }
                    catch ( SecurityException ) {
                    }
                    catch ( AggregateException exception ) {
                        exception.Handle( ex => {
                            if ( ex is UnauthorizedAccessException ) {
                                return true;
                            }
                            if ( ex is DirectoryNotFoundException ) {
                                return true;
                            }
                            if ( ex is IOException ) {
                                return true;
                            }
                            if ( ex is SecurityException ) {
                                return true;
                            }
                            ex.Error();
                            return false;
                        } );
                    }
                } );
            }
            catch ( UnauthorizedAccessException ) {
            }
            catch ( DirectoryNotFoundException ) {
            }
            catch ( IOException ) {
            }
            catch ( SecurityException ) {
            }
            catch ( AggregateException exception ) {
                exception.Handle( ex => {
                    if ( ex is UnauthorizedAccessException ) {
                        return true;
                    }
                    if ( ex is DirectoryNotFoundException ) {
                        return true;
                    }
                    if ( ex is IOException ) {
                        return true;
                    }
                    if ( ex is SecurityException ) {
                        return true;
                    }
                    ex.Error();
                    return false;
                } );
            }
        }

        public static DriveInfo GetDriveWithLargestAvailableFreeSpace() {
            return DriveInfo.GetDrives().AsParallel().Where( info => info.IsReady ).FirstOrDefault( driveInfo => driveInfo.AvailableFreeSpace >= DriveInfo.GetDrives().AsParallel().Where( info => info.IsReady ).Max( info => info.AvailableFreeSpace ) );
        }

        public static uint? GetFileSizeOnDisk( Document document ) {
            return GetFileSizeOnDisk( new FileInfo( document.FullPathWithFileName ) );
        }

        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static uint? GetFileSizeOnDisk( this FileInfo info ) {
            uint clusterSize;
            var driveLetter = info.Directory.Root.FullName.TrimEnd( '\\' );
            using ( var searcher = new ManagementObjectSearcher( String.Format( "select BlockSize,NumberOfBlocks from Win32_Volume WHERE DriveLetter = '{0}'", driveLetter ) ) ) {
                var bob = searcher.Get().Cast<ManagementObject>().First();
                clusterSize = ( uint )bob[ "BlockSize" ];
            }
            uint hosize;
            var losize = WindowsAPI.GetCompressedFileSizeW( info.FullName, out hosize );
            var size = hosize << 32 | losize;
            return ( ( size + clusterSize - 1 ) / clusterSize ) * clusterSize;
        }

        /// <summary>
        ///     <para>
        ///         The code above does not work properly on Windows Server 2008 or 2008 R2 or Windows 7 and Vista based systems
        ///         as cluster size is always zero (GetDiskFreeSpaceW and GetDiskFreeSpace return -1 even with UAC disabled.)
        ///     </para>
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <seealso cref="http://stackoverflow.com/questions/3750590/get-size-of-file-on-disk" />
        public static uint? GetFileSizeOnDiskAlt( this FileInfo info ) {
            uint dummy;
            uint sectorsPerCluster;
            uint bytesPerSector;
            var result = WindowsAPI.GetDiskFreeSpaceW( lpRootPathName: info.Directory.Root.FullName, lpSectorsPerCluster: out sectorsPerCluster, lpBytesPerSector: out bytesPerSector, lpNumberOfFreeClusters: out dummy, lpTotalNumberOfClusters: out dummy );
            if ( result == 0 ) {
                throw new Win32Exception();
            }
            var clusterSize = sectorsPerCluster * bytesPerSector;
            uint sizeHigh;
            var losize = WindowsAPI.GetCompressedFileSizeW( lpFileName: info.FullName, lpFileSizeHigh: out sizeHigh );
            var size = sizeHigh << 32 | losize;
            return ( ( size + clusterSize - 1 ) / clusterSize ) * clusterSize;
        }

        /// <summary>
        ///     Given the <paramref name="path" /> and <paramref name="searchPattern" /> pick any one
        ///     file and return the <see cref="FileSystemInfo.FullName" /> .
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static String GetRandomFile( String path, String searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly ) {
            if ( !Directory.Exists( path ) ) {
                return String.Empty;
            }
            var dir = new DirectoryInfo( path );
            if ( !dir.Exists ) {
                return String.Empty;
            }
            var files = Directory.EnumerateFiles( path: dir.FullName, searchPattern: searchPattern, searchOption: searchOption );
            var pickedfile = files.OrderBy( r => Randem.Next() ).FirstOrDefault();
            if ( pickedfile != null && File.Exists( pickedfile ) ) {
                return new FileInfo( pickedfile ).FullName;
            }
            return String.Empty;
        }

        public static Boolean IsDirectory( this NativeWin32.Win32FindData data ) {
            return ( data.dwFileAttributes & FileAttributes.Directory ) == FileAttributes.Directory;
        }

        public static Boolean IsFile( this NativeWin32.Win32FindData data ) {
            return !IsDirectory( data );
        }

        public static Boolean IsIgnoreFolder( this NativeWin32.Win32FindData data ) {
            return data.cFileName.EndsLike( "$RECYCLE.BIN" ) || data.cFileName.Like( "TEMP" ) || data.cFileName.Like( "TMP" ) || SystemFolders.Contains( new DirectoryInfo( data.cFileName ) );
        }

        public static Boolean IsParentOrCurrent( this NativeWin32.Win32FindData data ) {
            return data.cFileName == "." || data.cFileName == "..";
        }

        public static Boolean IsProtected( [NotNull] this FileSystemInfo fileSystemInfo ) {
            if ( fileSystemInfo == null ) {
                throw new ArgumentNullException( "fileSystemInfo" );
            }
            if ( !fileSystemInfo.Exists ) {
                return false;
            }
            var ds = new DirectorySecurity( fileSystemInfo.FullName, AccessControlSections.Access );
            if ( !ds.AreAccessRulesProtected ) {
                return false;
            }
            using ( var wi = WindowsIdentity.GetCurrent() ) {
                if ( wi == null ) {
                    return false;
                }
                var wp = new WindowsPrincipal( wi );
                var isProtected = !wp.IsInRole( WindowsBuiltInRole.Administrator ); // Not running as admin
                return isProtected;
            }
        }

        [Pure]
        public static Boolean IsReparsePoint( this NativeWin32.Win32FindData data ) {
            return ( data.dwFileAttributes & FileAttributes.ReparsePoint ) == FileAttributes.ReparsePoint;
        }

        /// <summary>
        ///     Opens a folder with Explorer.exe
        /// </summary>
        public static void OpenDirectoryWithExplorer( [CanBeNull] this DirectoryInfo folder ) {
            folder.Should().NotBeNull();
            if ( null == folder ) {
                return;
            }
            if ( !folder.Exists ) {
                folder.Refresh();
                if ( !folder.Exists ) {
                    return;
                }
            }

            var windowsFolder = Environment.GetEnvironmentVariable( "SystemRoot" );
            windowsFolder.Should().NotBeNullOrWhiteSpace();
            if ( String.IsNullOrWhiteSpace( windowsFolder ) ) {
                return;
            }

            var proc = Process.Start( fileName: String.Format( "{0}\\explorer.exe", windowsFolder ), arguments: String.Format( "/e,\"{0}\"", folder.FullName ) );
            proc.Responding.Should().Be( true );
        }

        /// <summary>
        ///     Before: "hello.txt".
        ///     After: "hello 345680969061906730476346.txt"
        /// </summary>
        /// <param name="info"></param>
        /// <param name="newExtension"></param>
        /// <returns></returns>
        public static FileInfo PlusDateTime( this FileInfo info, String newExtension = null ) {
            if ( info == null ) {
                throw new ArgumentNullException( "info" );
            }
            if ( info.Directory == null ) {
                throw new NullReferenceException( "info.directory" );
            }
            var now = Convert.ToString( value: DateTime.UtcNow.ToBinary(), toBase: 16 );
            var formatted = String.Format( "{0} {1}{2}", Path.GetFileNameWithoutExtension( info.Name ), now, newExtension ?? info.Extension );
            var path = Path.Combine( info.Directory.FullName, formatted );
            return new FileInfo( path );
        }

        /// <summary>
        ///     untested. is this written correctly? would it read from a *slow* media but not block the calling function?
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bufferSize"></param>
        /// <param name="fileMissingRetries"></param>
        /// <param name="retryDelay"></param>
        /// <returns></returns>
        public static async Task<string> ReadTextAsync( String filePath, int? bufferSize = 4096, int? fileMissingRetries = 10, TimeSpan? retryDelay = null ) {
            if ( String.IsNullOrWhiteSpace( filePath ) ) {
                throw new ArgumentNullException( "filePath" );
            }

            if ( !bufferSize.HasValue ) {
                bufferSize = 4096;
            }
            if ( !retryDelay.HasValue ) {
                retryDelay = Seconds.One;
            }

            while ( fileMissingRetries.HasValue && fileMissingRetries.Value > 0 ) {
                if ( File.Exists( filePath ) ) {
                    break;
                }
                await Task.Delay( retryDelay.Value );
                fileMissingRetries--;
            }

            if ( File.Exists( filePath ) ) {
                try {
                    using ( var sourceStream = new FileStream( path: filePath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: bufferSize.Value, useAsync: true ) ) {
                        var sb = new StringBuilder( bufferSize.Value );
                        var buffer = new byte[ bufferSize.Value ];
                        int numRead;
                        while ( ( numRead = await sourceStream.ReadAsync( buffer, 0, buffer.Length ) ) != 0 ) {
                            var text = Encoding.Unicode.GetString( buffer, 0, numRead );
                            sb.Append( text );
                        }

                        return sb.ToString();
                    }
                }
                catch ( FileNotFoundException exception ) {
                    exception.Error();
                }
            }
            return String.Empty;
        }

        /// <summary>
        ///     <para>Performs a byte by byte file comparison, but ignores the <see cref="Document" /> file names.</para>
        /// </summary>
        /// <param name="left"></param>
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
        public static Boolean SameContent( [CanBeNull] this Document left, [CanBeNull] Document right ) {
            if ( left == null || right == null ) {
                return false;
            }
            var linfo = new FileInfo( left.FullPathWithFileName );
            var rinfo = new FileInfo( right.FullPathWithFileName );
            return linfo.SameContent( rinfo );
        }

        /// <summary>
        ///     <para>performs a byte by byte file comparison</para>
        /// </summary>
        /// <param name="left"></param>
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
            if ( left == null || right == null ) {
                return false;
            }

            return left.Length == right.Length && left.AsByteArray().SequenceEqual( right.AsByteArray() );
        }

        /// <summary>
        ///     <para>performs a byte by byte file comparison</para>
        /// </summary>
        /// <param name="left"></param>
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
            if ( left == null || right == null ) {
                return false;
            }

            return left.GetLength() == ( UInt64 )right.Length && left.AsByteArray().SequenceEqual( right.AsByteArray() );
        }

        /// <summary>
        ///     <para>performs a byte by byte file comparison</para>
        /// </summary>
        /// <param name="left"></param>
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
        public static Boolean SameContent( [CanBeNull] this FileInfo left, [CanBeNull] Document right ) {
            if ( left == null || right == null ) {
                return false;
            }

            return ( UInt64 )left.Length == right.GetLength() && left.AsByteArray().SequenceEqual( right.AsByteArray() );
        }

        /// <summary>
        ///     Search all possible drives for any files matching the
        ///     <paramref
        ///         name="fileSearchPatterns" />
        ///     .
        /// </summary>
        /// <param name="fileSearchPatterns">List of patterns to search for.</param>
        /// <param name="cancellationToken"></param>
        /// <param name="onFindFile"><see cref="Action" /> to perform when a file is found.</param>
        /// <param name="onEachDirectory"><see cref="Action" /> to perform on each folder found.</param>
        /// <param name="searchStyle"></param>
        public static void SearchAllDrives( [NotNull] IEnumerable<string> fileSearchPatterns, CancellationToken cancellationToken, Action<FileInfo> onFindFile = null, Action<DirectoryInfo> onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
            if ( fileSearchPatterns == null ) {
                throw new ArgumentNullException( "fileSearchPatterns" );
            }
            try {
                DriveInfo.GetDrives().AsParallel().WithDegreeOfParallelism( 26 ).WithExecutionMode( ParallelExecutionMode.ForceParallelism ).ForAll( drive => {
                    if ( !drive.IsReady || drive.DriveType == DriveType.NoRootDirectory || !drive.RootDirectory.Exists ) {
                        return;
                    }
                    String.Format( "Scanning [{0}]", drive.VolumeLabel ).TimeDebug();
                    FindFiles( fileSearchPatterns: fileSearchPatterns, cancellationToken: cancellationToken, startingFolder: drive.RootDirectory, onFindFile: onFindFile, onEachDirectory: onEachDirectory, searchStyle: searchStyle );
                } );
            }
            catch ( UnauthorizedAccessException ) {
            }
            catch ( DirectoryNotFoundException ) {
            }
            catch ( IOException ) {
            }
            catch ( SecurityException ) {
            }
            catch ( AggregateException exception ) {
                exception.Handle( ex => {
                    if ( ex is UnauthorizedAccessException ) {
                        return true;
                    }
                    if ( ex is DirectoryNotFoundException ) {
                        return true;
                    }
                    if ( ex is IOException ) {
                        return true;
                    }
                    if ( ex is SecurityException ) {
                        return true;
                    }
                    ex.Error();
                    return false;
                } );
            }
        }

        public static Boolean SetCompression( this DirectoryInfo directoryInfo, Boolean compressed = true ) {
            try {
                if ( directoryInfo.Exists ) {
                    using ( var dir = new ManagementObject( directoryInfo.ToManagementPath() ) ) {
                        var outParams = dir.InvokeMethod( compressed ? "Compress" : "Uncompress", null, null );
                        if ( null == outParams ) {
                            return false;
                        }
                        var result = Convert.ToUInt32( outParams.Properties[ "ReturnValue" ].Value );
                        return result == 0;
                    }
                }
            }
            catch ( ManagementException exception ) {
                exception.Error();
            }
            return false;
        }

        public static Boolean SetCompression( this String folderPath, Boolean compressed = true ) {
            if ( String.IsNullOrWhiteSpace( folderPath ) ) {
                return false;
            }

            try {
                var dirInfo = new DirectoryInfo( folderPath );
                return dirInfo.SetCompression( compressed );
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            return false;
        }

        public static ManagementPath ToManagementPath( this DirectoryInfo systemPath ) {
            var fullPath = systemPath.FullName;
            while ( fullPath.EndsWith( "\\" ) ) {
                fullPath = fullPath.Substring( 0, fullPath.Length - 1 );
            }
            fullPath = String.Format( "Win32_Directory.Name=\"{0}\"", fullPath.Replace( "\\", "\\\\" ) );
            var managed = new ManagementPath( fullPath );
            return managed;
        }

        public static IEnumerable<string> ToPaths( [NotNull] this DirectoryInfo directoryInfo ) {
            if ( directoryInfo == null ) {
                throw new ArgumentNullException( "directoryInfo" );
            }
            return directoryInfo.ToString().Split( Path.DirectorySeparatorChar );
        }

        public static MemoryStream TryCopyStream( String filePath, Boolean bePatient = true, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite ) {

        //TODO
        TryAgain:
            var memoryStream = new MemoryStream();
            try {
                if ( File.Exists( filePath ) ) {
                    using ( var fileStream = File.Open( path: filePath, mode: fileMode, access: fileAccess, share: fileShare ) ) {
                        var length = ( int )fileStream.Length;
                        if ( length > 0 ) {
                            fileStream.CopyTo( memoryStream, length ); //int-long possible issue.
                            memoryStream.Seek( 0, SeekOrigin.Begin );
                        }
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

        /// <summary>
        ///     <para>Returns true if the <see cref="Document" /> no longer seems to exist.</para>
        ///     <para>Returns null if existance cannot be determined.</para>
        /// </summary>
        /// <param name="document"></param>
        /// <param name="bePatient">The delete will retry for a default of <see cref="Seconds.Five" />.</param>
        /// <returns></returns>
        public static Boolean? TryDeleting( this Document document, Boolean bePatient = true ) {
            var stopwatch = Stopwatch.StartNew();
        TryAgain:
            try {
                if ( !document.Exists() ) {
                    return true;
                }
                File.Delete( path: document.FullPathWithFileName );
                return !File.Exists( document.FullPathWithFileName );
            }
            catch ( DirectoryNotFoundException ) {
            }
            catch ( PathTooLongException ) {
            }
            catch ( IOException ) {

                // IOExcception is thrown if the file is in use by another process.
                if ( bePatient && stopwatch.Elapsed <= Seconds.Five ) {
                    if ( !Thread.Yield() ) {
                        Thread.Sleep( Milliseconds.ThreeHundredThirtyThree );
                    }
                    goto TryAgain;
                }
            }
            catch ( UnauthorizedAccessException ) {
            }
            catch ( ArgumentNullException ) {
            }
            finally {
                stopwatch.Stop();
            }
            return null;
        }

        public static Boolean TryGetFolderFromPath( String path, [CanBeNull] out DirectoryInfo directoryInfo, [CanBeNull] out Uri uri ) {
            directoryInfo = null;
            uri = null;
            try {
                if ( String.IsNullOrWhiteSpace( path ) ) {
                    return false;
                }
                path = path.Trim();
                if ( String.IsNullOrWhiteSpace( path ) ) {
                    return false;
                }
                if ( Uri.TryCreate( path, UriKind.Absolute, out uri ) ) {
                    directoryInfo = new DirectoryInfo( uri.LocalPath );
                    return true;
                }
            }
            catch ( UriFormatException ) {
            }
            catch ( SecurityException ) {
            }
            catch ( PathTooLongException ) {
            }
            catch ( InvalidOperationException ) {
            }
            return false;
        }

        /// <summary>
        ///     Returns a temporary <see cref="Document" />, but does not create it in the file system.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Boolean TryGetTempDocument( [NotNull] this Folder folder, [NotNull] out Document document ) {
            if ( folder == null ) {
                throw new ArgumentNullException( "folder" );
            }
            try {
                string randomFile = null;
                try {
                    randomFile = Path.GetTempFileName();
                }
                finally {
                    if ( randomFile != null ) {
                        File.Delete( randomFile );
                    }
                }

                var randomFileName = Path.Combine( folder.FullName, Path.GetFileName( randomFile ) );

                document = new Document( randomFileName );
                return true;
            }
            catch ( DirectoryNotFoundException ) {
            }
            catch ( PathTooLongException ) {
            }
            catch ( IOException ) {
            }
            catch ( NotSupportedException ) {
            }
            catch ( UnauthorizedAccessException ) {
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            document = default( Document );
            return false;
        }

        /// <summary>
        ///     Tries to open a file, with a user defined number of attempt and Sleep delay between attempts.
        /// </summary>
        /// <param name="filePath">The full file path to be opened</param>
        /// <param name="fileMode">Required file mode enum value(see MSDN documentation)</param>
        /// <param name="fileAccess">Required file access enum value(see MSDN documentation)</param>
        /// <param name="fileShare">Required file share enum value(see MSDN documentation)</param>
        /// <returns>
        ///     A valid FileStream object for the opened file, or null if the File could not be opened after the required
        ///     attempts
        /// </returns>
        public static FileStream TryOpen( String filePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare ) {

            //TODO
            try {
                return File.Open( path: filePath, mode: fileMode, access: fileAccess, share: fileShare );
            }
            catch ( IOException ) {

                // IOExcception is thrown if the file is in use by another process.
            }
            return null;
        }

        public static FileStream TryOpenForReading( String filePath, Boolean bePatient = true, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite ) {

        //TODO
        TryAgain:
            try {
                if ( File.Exists( filePath ) ) {
                    return File.Open( path: filePath, mode: fileMode, access: fileAccess, share: fileShare );
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
            return null;
        }

        public static FileStream TryOpenForWriting( String filePath, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.Write, FileShare fileShare = FileShare.ReadWrite ) {

            //TODO
            try {
                return File.Open( path: filePath, mode: fileMode, access: fileAccess, share: fileShare );
            }
            catch ( IOException ) {

                // IOExcception is thrown if the file is in use by another process.
            }
            return null;
        }

        public static int? TurnOnCompression( [NotNull] this FileInfo info ) {
            if ( info == null ) {
                throw new ArgumentNullException( "info" );
            }
            if ( !info.Exists ) {
                info.Refresh();
                if ( !info.Exists ) {
                    return null;
                }
            }

            var lpBytesReturned = 0;
            short compressionFormatDefault = 1;

            BigInteger bob;

            using ( var fileStream = File.Open( path: info.FullName, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.None ) ) {
                var success = false;

                try {
                    fileStream.SafeFileHandle.DangerousAddRef( success: ref success );

                    var result = DeviceIoControl( hDevice: fileStream.SafeFileHandle.DangerousGetHandle(), dwIoControlCode: FSCTL_SET_COMPRESSION, lpInBuffer: ref compressionFormatDefault, nInBufferSize: sizeof( short ), lpOutBuffer: IntPtr.Zero, nOutBufferSize: 0, lpBytesReturned: ref lpBytesReturned, lpOverlapped: IntPtr.Zero );
                }
                finally {
                    fileStream.SafeFileHandle.DangerousRelease();
                }

                return lpBytesReturned;
            }
        }

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

                    if ( DateTime.TryParseExact( value, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result ) ) {
                        return result;
                    }

                    return null;
                }
        */

        /// <summary>
        ///     (does not create path)
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DirectoryInfo WithShortDatePath( this DirectoryInfo basePath, DateTime d ) {
            var path = Path.Combine( basePath.FullName, d.Year.ToString(), d.DayOfYear.ToString(), d.Hour.ToString() );
            return new DirectoryInfo( path );
        }
    }
}