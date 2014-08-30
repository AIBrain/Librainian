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
// "Librainian/IOExtensions.cs" was last cleaned by Rick on 2014/08/28 at 4:04 PM
#endregion

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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Annotations;
    using Controls;
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

        [DllImport( "kernel32.dll" )]
        public static extern int DeviceIoControl( IntPtr hDevice, int dwIoControlCode, ref short lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped );

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

                    var result = DeviceIoControl( hDevice: fileStream.SafeFileHandle.DangerousGetHandle(), dwIoControlCode: FSCTL_SET_COMPRESSION, lpInBuffer: ref compressionFormatDefault, nInBufferSize: sizeof ( short ), lpOutBuffer: IntPtr.Zero, nOutBufferSize: 0, lpBytesReturned: ref lpBytesReturned, lpOverlapped: IntPtr.Zero );
                }
                finally {
                    fileStream.SafeFileHandle.DangerousRelease();
                }

                return lpBytesReturned;
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

            return left.Length == ( UInt64 ) right.Length && left.AsByteArray().SequenceEqual( right.AsByteArray() );
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

            return ( UInt64 ) left.Length == right.Length && left.AsByteArray().SequenceEqual( right.AsByteArray() );
        }

        /// <summary>
        ///     Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static IEnumerable< byte > AsByteArray( [NotNull] this FileInfo fileInfo ) {
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
                    yield return ( Byte ) b;
                }
            }
        }

        /// <summary>
        ///     Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        // TODO this needs a unit test for endianness
        public static IEnumerable< ushort > AsUInt16Array( [NotNull] this FileInfo fileInfo ) {
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
                        yield return ( ( Byte ) low ).CombineBytes( high: 0 );
                        yield break;
                    }

                    yield return ( ( Byte ) low ).CombineBytes( high: ( Byte ) high );
                }
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
        public static Task Copy( Document source, Document destination, Action< double > progress, Action< TimeSpan > eta ) {
            return Task.Run( () => {
                                 var computer = new Computer();
                                 //TODO file monitor/watcher?
                                 computer.FileSystem.CopyFile( source.FullPathWithFileName, destination.FullPathWithFileName, UIOption.AllDialogs, UICancelOption.DoNothing );
                             } );
        }

        public static MemoryStream TryCopyStream( String filePath, Boolean bePatient = true, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite ) {
            //TODO
            TryAgain:
            var memoryStream = new MemoryStream();
            try {
                if ( File.Exists( filePath ) ) {
                    using ( var fileStream = File.Open( path: filePath, mode: fileMode, access: fileAccess, share: fileShare ) ) {
                        var length = ( int ) fileStream.Length;
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
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) {
                // IOExcception is thrown if the file is in use by another process.
                if ( bePatient && stopwatch.Elapsed <= Seconds.Five ) {
                    if ( !Thread.Yield() ) {
                        Thread.Sleep( Milliseconds.ThreeHundredThirtyThree );
                    }
                    goto TryAgain;
                }
            }
            catch ( UnauthorizedAccessException ) { }
            catch ( ArgumentNullException ) { }
            finally {
                stopwatch.Stop();
            }
            return null;
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
            var clusterSize = sectorsPerCluster*bytesPerSector;
            uint sizeHigh;
            var losize = WindowsAPI.GetCompressedFileSizeW( lpFileName: info.FullName, lpFileSizeHigh: out sizeHigh );
            var size = sizeHigh << 32 | losize;
            return ( ( size + clusterSize - 1 )/clusterSize )*clusterSize;
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
                var bob = searcher.Get().Cast< ManagementObject >().First();
                clusterSize = ( uint ) bob[ "BlockSize" ];
            }
            uint hosize;
            var losize = WindowsAPI.GetCompressedFileSizeW( info.FullName, out hosize );
            var size = hosize << 32 | losize;
            return ( ( size + clusterSize - 1 )/clusterSize )*clusterSize;
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

        [Test]
        public static Boolean TestEmptyDocument() {
            return true;
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
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) { }
            catch ( NotSupportedException ) { }
            catch ( UnauthorizedAccessException ) { }
            // ReSharper disable once AssignNullToNotNullAttribute
            document = default( Document );
            return false;
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
            catch ( UriFormatException ) { }
            catch ( SecurityException ) { }
            catch ( PathTooLongException ) { }
            catch ( InvalidOperationException ) { }
            return false;
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

            var owner = WindowWrapper.CreateWindowWrapper( Threads.CurrentProcess.MainWindowHandle );

            var dialog = folderBrowserDialog.ShowDialog( owner );

            if ( dialog != DialogResult.OK || folderBrowserDialog.SelectedPath.IsNullOrWhiteSpace() ) {
                return null;
            }
            return new Folder( folderBrowserDialog.SelectedPath );
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
        public static void SearchAllDrives( [NotNull] IEnumerable< String > fileSearchPatterns, CancellationToken cancellationToken, Action< FileInfo > onFindFile = null, Action< DirectoryInfo > onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
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
            catch ( UnauthorizedAccessException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( IOException ) { }
            catch ( SecurityException ) { }
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
        public static void FindFiles( IEnumerable< String > fileSearchPatterns, DirectoryInfo startingFolder, CancellationToken cancellationToken, Action< FileInfo > onFindFile = null, Action< DirectoryInfo > onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
            if ( fileSearchPatterns == null ) {
                throw new ArgumentNullException( "fileSearchPatterns" );
            }
            if ( startingFolder == null ) {
                throw new ArgumentNullException( "startingFolder" );
            }
            try {
                var searchPatterns = fileSearchPatterns as IList< String > ?? fileSearchPatterns.ToList();
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
                                                                                                                                                       catch ( UnauthorizedAccessException ) { }
                                                                                                                                                       catch ( DirectoryNotFoundException ) { }
                                                                                                                                                       catch ( IOException ) { }
                                                                                                                                                       catch ( SecurityException ) { }
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
                                                                                     catch ( UnauthorizedAccessException ) { }
                                                                                     catch ( DirectoryNotFoundException ) { }
                                                                                     catch ( IOException ) { }
                                                                                     catch ( SecurityException ) { }
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
            catch ( UnauthorizedAccessException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( IOException ) { }
            catch ( SecurityException ) { }
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
    }
}
