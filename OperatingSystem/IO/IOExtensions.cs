// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/IOExtensions.cs" was last cleaned by Rick on 2015/11/13 at 11:30 PM

namespace Librainian.OperatingSystem.IO {

    using System;
    using System.Collections.Concurrent;
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
    using Collections;
    using Controls;
    using FileSystem;
    using FluentAssertions;
    using global::Windows.Data.Xml.Dom;
    using global::Windows.UI.Notifications;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Time;
    using Microsoft.VisualBasic.Devices;
    using Microsoft.VisualBasic.FileIO;
    using NUnit.Framework;
    using Parsing;
    using Threading;
    using SearchOption = System.IO.SearchOption;

    public static class IOExtensions {

        public const Int32 FsctlSetCompression = 0x9C040;

        public static readonly HashSet< Folder > SystemFolders = new HashSet< Folder >();

        static IOExtensions() {
            SystemFolders.Add( new Folder( Environment.SpecialFolder.System ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.SystemX86 ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.AdminTools ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.CDBurning ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.Windows ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.Cookies ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.History ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.InternetCache ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.PrinterShortcuts ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.ProgramFiles ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.ProgramFilesX86 ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.Programs ) );
            SystemFolders.Add( new Folder( Environment.SpecialFolder.SendTo ) );
            SystemFolders.Add( new Folder( Path.GetTempPath() ) );

            //TODO foreach on Environment.SpecialFolder
        }

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern Boolean LockWorkStation();

        /// <summary>
        ///     Example: WriteTextAsync( fullPath: fullPath, text: message ).Wait();
        ///     Example: await WriteTextAsync( fullPath: fullPath, text: message );
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async void AppendTextAsync( this FileInfo fileInfo, String text ) {
            if ( fileInfo == null ) {
                throw new ArgumentNullException( nameof( fileInfo ) );
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
                exception.More();
            }
            catch ( ArgumentNullException exception ) {
                exception.More();
            }
            catch ( DirectoryNotFoundException exception ) {
                exception.More();
            }
            catch ( PathTooLongException exception ) {
                exception.More();
            }
            catch ( SecurityException exception ) {
                exception.More();
            }
            catch ( IOException exception ) {
                exception.More();
            }
        }

        /// <summary>
        ///     Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static IEnumerable< Byte > AsByteArray( [NotNull] this FileInfo fileInfo ) {
            if ( fileInfo == null ) {
                throw new ArgumentNullException( nameof( fileInfo ) );
            }

            if ( !fileInfo.Exists ) {
                yield break;
            }

            var stream = Try( () => new FileStream( path: fileInfo.FullName, mode: FileMode.Open, access: FileAccess.Read ), Seconds.Seven, CancellationToken.None );

            if ( null == stream ) {
                yield break;
            }

            if ( !stream.CanRead ) {
                throw new NotSupportedException( $"Cannot read from file {fileInfo.FullName}" );
            }

            using ( stream ) {
                using ( var buffered = new BufferedStream( stream ) ) {
                    do {
                        var b = buffered.ReadByte();
                        if ( b == -1 ) {
                            yield break;
                        }
                        yield return ( Byte ) b;
                    } while ( true );
                }
            }
        }

        /// <summary>
        ///     Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static IEnumerable< Byte > AsByteArray( [NotNull] this String filename ) {
            if ( filename == null ) {
                throw new ArgumentNullException( nameof( filename ) );
            }

            if ( !File.Exists( filename ) ) {
                yield break;
            }

            var stream = Try( () => new FileStream( path: filename, mode: FileMode.Open, access: FileAccess.Read ), Seconds.Seven, CancellationToken.None );

            if ( null == stream ) {
                yield break;
            }

            if ( !stream.CanRead ) {
                throw new NotSupportedException( $"Cannot read from file {filename}." );
            }

            using ( stream ) {
                using ( var buffered = new BufferedStream( stream ) ) {
                    do {
                        var b = buffered.ReadByte();
                        if ( b == -1 ) {
                            yield break;
                        }
                        yield return ( Byte ) b;
                    } while ( true );
                }
            }
        }

        /// <summary>
        ///     Retry the <paramref name="ioFunction" /> if an <see cref="IOException" /> occurs.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="ioFunction"></param>
        /// <param name="tryFor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        [CanBeNull]
        public static TResult Try< TResult >( [NotNull] this Func< TResult > ioFunction, TimeSpan tryFor, CancellationToken token ) {
            if ( ioFunction == null ) {
                throw new ArgumentNullException( nameof( ioFunction ) );
            }

            //var oneTenth = TimeSpan.FromMilliseconds( tryFor.TotalMilliseconds / 10 );
            var stopwatch = StopWatch.StartNew();
            TryAgain:
            if ( token.IsCancellationRequested ) {
                return default ( TResult );
            }
            try {
                Application.DoEvents();
                return ioFunction();
            }
            catch ( IOException exception ) {
                exception.Message.Error();
                if ( stopwatch.Elapsed > tryFor ) {
                    throw;
                }
                Thread.CurrentThread.Fraggle( Seconds.One );

                goto TryAgain;
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="ioAction"></param>
        ///// <param name="tryFor"></param>
        ///// <param name="cancel"></param>
        ///// <returns></returns>
        //[CanBeNull]
        //public static void Retry( [NotNull] this Action ioAction, TimeSpan tryFor, SimpleCancel cancel = null ) {
        //    if ( ioAction == null ) {
        //        throw new ArgumentNullException( nameof( ioAction ) );
        //    }
        //    var stopwatch = Stopwatch.StartNew();
        //TryAgain:
        //    try {
        //        Application.DoEvents();
        //        ioAction();
        //    }
        //    catch ( IOException) {
        //        if ( stopwatch.Elapsed > tryFor ) {
        //            throw;
        //        }
        //        if ( null != cancel && cancel.HaveAnyCancellationsBeenRequested() ) {
        //            return;
        //        }
        //        Thread.Yield();
        //        goto TryAgain;
        //    }
        //}

        /// <summary>
        ///     ask user for folder/network path where to store dictionary
        /// </summary>
        [CanBeNull]
        public static Folder AskUserForStorageFolder( String hint ) {
            var folderBrowserDialog = new FolderBrowserDialog {ShowNewFolderButton = true, Description = $"Please direct me to a storage folder for {hint}.", RootFolder = Environment.SpecialFolder.MyComputer};

            var owner = WindowWrapper.CreateWindowWrapper( Process.GetCurrentProcess()
                                                                  .MainWindowHandle );

            var dialog = folderBrowserDialog.ShowDialog( owner );

            if ( ( dialog != DialogResult.OK ) || folderBrowserDialog.SelectedPath.IsNullOrWhiteSpace() ) {
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
        public static IEnumerable< UInt16 > AsUInt16Array( [NotNull] this FileInfo fileInfo ) {
            if ( fileInfo == null ) {
                throw new ArgumentNullException( nameof( fileInfo ) );
            }
            if ( !fileInfo.Exists ) {
                fileInfo.Refresh(); //check one more time
                if ( !fileInfo.Exists ) {
                    yield break;
                }
            }

            using ( var stream = new FileStream( fileInfo.FullName, FileMode.Open ) ) {
                if ( !stream.CanRead ) {
                    throw new NotSupportedException( $"Cannot read from file {fileInfo.FullName}" );
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
        ///     No guarantee of return order. Also, because of the way the operating system works
        ///     (random-access), a directory may be created or deleted even after a search.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static IEnumerable< DirectoryInfo > BetterEnumerateDirectories( this DirectoryInfo target, String searchPattern = "*" ) {
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

        public static IEnumerable< FileInfo > BetterEnumerateFiles( [NotNull] this DirectoryInfo target, [NotNull] String searchPattern = "*" ) {
            if ( target == null ) {
                throw new ArgumentNullException( nameof( target ) );
            }
            if ( searchPattern == null ) {
                throw new ArgumentNullException( nameof( searchPattern ) );
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
        public static Int32 CalcHash( [NotNull] this FileInfo fileInfo ) {
            if ( fileInfo == null ) {
                throw new ArgumentNullException( nameof( fileInfo ) );
            }

            return fileInfo.AsByteArray()
                           .Aggregate( 0, ( current, b ) => current.GetHashMerge( b ) );
        }

        /// <summary>
        ///     poor mans crc
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Int32 CalcHash( [NotNull] this Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            var fileInfo = new FileInfo( document.FullPathWithFileName );
            if ( fileInfo == null ) {
                throw new NullReferenceException( "fileInfo" );
            }

            return fileInfo.AsByteArray()
                           .Aggregate( 0, ( current, b ) => current.GetHashMerge( b ) );
        }

        [CanBeNull]
        public static DirectoryInfo ChooseDirectoryDialog( this Environment.SpecialFolder startFolder, String path, String description = "Please select a folder." ) {
            using ( var folderDialog = new FolderBrowserDialog {Description = description, RootFolder = Environment.SpecialFolder.MyComputer, ShowNewFolderButton = false} ) {
                if ( folderDialog.ShowDialog() == DialogResult.OK ) {
                    return new DirectoryInfo( folderDialog.SelectedPath );
                }
            }
            return null;
        }

        public static Byte[] Compress( [NotNull] this Byte[] data ) {
            if ( data == null ) {
                throw new ArgumentNullException( nameof( data ) );
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
        public static Task Copy( this Document source, Document destination, Action< Double > progress, Action< TimeSpan > eta ) => Task.Run( () => {
                                                                                                                                                  var computer = new Computer();

                                                                                                                                                  //TODO file monitor/watcher?
                                                                                                                                                  computer.FileSystem.CopyFile( source.FullPathWithFileName, destination.FullPathWithFileName, UIOption.AllDialogs, UICancelOption.DoNothing );
                                                                                                                                              } );

        /// <summary>
        ///     Before: @"c:\hello\world".
        ///     After: @"c:\hello\world\23468923475634836.extension"
        /// </summary>
        /// <param name="info"></param>
        /// <param name="withExtension"></param>
        /// <param name="toBase"></param>
        /// <returns></returns>
        public static FileInfo DateAndTimeAsFile( this DirectoryInfo info, String withExtension, Int32 toBase = 16 ) {
            if ( info == null ) {
                throw new ArgumentNullException( nameof( info ) );
            }

            var now = Convert.ToString( value: DateTime.UtcNow.ToBinary(), toBase: toBase );
            var fileName = $"{now}{withExtension ?? info.Extension}";
            var path = Path.Combine( info.FullName, fileName );
            return new FileInfo( path );
        }

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
                throw new ArgumentNullException( nameof( directoryInfo ) );
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
                exception.More();
                return null;
            }
            return directoryInfo;
        }

        public static DateTime FileNameAsDateAndTime( this FileInfo info, DateTime? defaultValue = null ) {
            if ( info == null ) {
                throw new ArgumentNullException( nameof( info ) );
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

            Int64 data;

            if ( Int64.TryParse( fName, NumberStyles.AllowHexSpecifier, null, out data ) ) {
                return DateTime.FromBinary( data );
            }

            if ( Int64.TryParse( fName, NumberStyles.Any, null, out data ) ) {
                return DateTime.FromBinary( data );
            }

            return now;
        }

        /// <summary>
        /// </summary>
        /// <param name="startingFolder"></param>
        /// <param name="documentSearchPatterns"></param>
        /// <param name="onEachDocumentFound">Warning, this could OOM on a large folder structure.</param>
        /// <param name="cancellation"></param>
        /// <param name="progressFolders"></param>
        /// <param name="progressDocuments"></param>
        /// <returns></returns>
        public static Boolean GrabEntireTree( [NotNull] this Folder startingFolder, IEnumerable< String > documentSearchPatterns, [NotNull] Action< Document > onEachDocumentFound, IProgress< Int64 > progressFolders, IProgress< Int64 > progressDocuments, CancellationTokenSource cancellation ) {
            if ( startingFolder == null ) {
                throw new ArgumentNullException( nameof( startingFolder ) );
            }
            if ( onEachDocumentFound == null ) {
                throw new ArgumentNullException( nameof( onEachDocumentFound ) );
            }

            //if ( foldersFound == null ) {
            //    throw new ArgumentNullException( nameof( foldersFound ) );
            //}

            if ( cancellation.IsCancellationRequested ) {
                return false;
            }

            if ( !startingFolder.Exists() ) {
                return false;
            }

            //foldersFound.Add( startingFolder );
            var searchPatterns = documentSearchPatterns as IList< String > ?? documentSearchPatterns.ToList();

            Parallel.ForEach( startingFolder.GetFolders()
                                            .AsParallel(), folder => {
                                                               progressFolders.Report( 1 );
                                                               GrabEntireTree( folder, searchPatterns, onEachDocumentFound, progressFolders, progressDocuments, cancellation );
                                                               progressFolders.Report( -1 );
                                                           } );

            //var list = new List<FileInfo>();
            foreach ( var files in searchPatterns.Select( searchPattern => startingFolder.DirectoryInfo.EnumerateFiles( searchPattern )
                                                                                         .OrderBy( info => Randem.Next() ) ) ) {
                foreach ( var info in files ) {
                    progressDocuments.Report( 1 );
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

        /// <summary>
        ///     Warning, this could OOM on a large folder structure.
        /// </summary>
        /// <param name="startingFolder"></param>
        /// <param name="foldersFound">Warning, this could OOM on a *large* folder structure.</param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public static Boolean GrabAllFolders( [NotNull] this Folder startingFolder, [NotNull] ConcurrentBag< String > foldersFound, SimpleCancel cancellation ) {
            if ( startingFolder == null ) {
                throw new ArgumentNullException( nameof( startingFolder ) );
            }
            if ( foldersFound == null ) {
                throw new ArgumentNullException( nameof( foldersFound ) );
            }

            if ( cancellation.HaveAnyCancellationsBeenRequested() ) {
                return false;
            }

            if ( !startingFolder.Exists() ) {
                return false;
            }

            if ( startingFolder.Name.Like( "$OF" ) ) {
                return false;
            }

            foldersFound.Add( startingFolder.FullName );

            Parallel.ForEach( startingFolder.DirectoryInfo.EnumerateDirectories()
                                            .AsParallel(), info => GrabAllFolders( new Folder( info.FullName ), foldersFound, cancellation ) );

            return true;
        }

        /// <summary>
        ///     Search the <paramref name="startingFolder" /> for any files matching the
        ///     <paramref name="fileSearchPatterns" /> .
        /// </summary>
        /// <param name="startingFolder">The folder to start the search.</param>
        /// <param name="fileSearchPatterns">List of patterns to search for.</param>
        /// <param name="cancellation"></param>
        /// <param name="onFindFile"><see cref="Action" /> to perform when a file is found.</param>
        /// <param name="onEachDirectory"><see cref="Action" /> to perform on each folder found.</param>
        /// <param name="searchStyle"></param>
        public static void FindFiles( this DirectoryInfo startingFolder, IEnumerable< String > fileSearchPatterns, SimpleCancel cancellation, Action< FileInfo > onFindFile = null, Action< DirectoryInfo > onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
            if ( fileSearchPatterns == null ) {
                throw new ArgumentNullException( nameof( fileSearchPatterns ) );
            }
            if ( startingFolder == null ) {
                throw new ArgumentNullException( nameof( startingFolder ) );
            }
            try {
                var searchPatterns = fileSearchPatterns as IList< String > ?? fileSearchPatterns.ToList();
                searchPatterns.AsParallel()
                              .ForAll( searchPattern => {
                                           //#if DEEPDEBUG
                                           $"Searching folder {startingFolder.FullName} for {searchPattern}.".WriteLine();

                                           //#endif
                                           if ( cancellation.HaveAnyCancellationsBeenRequested() ) {
                                               return;
                                           }
                                           try {
                                               var folders = startingFolder.EnumerateDirectories( "*", SearchOption.TopDirectoryOnly );
                                               folders.AsParallel()
                                                      .ForAll( async folder => {
#if DEEPDEBUG
                                                                         $"Found folder {folder}.".WriteLine();
#endif
                                                                         if ( cancellation.HaveAnyCancellationsBeenRequested() ) {
                                                                             return;
                                                                         }
                                                                         try {
                                                                             onEachDirectory?.Invoke( folder );
                                                                         }
                                                                         catch ( Exception exception ) {
                                                                             exception.More();
                                                                         }
                                                                         if ( searchStyle == SearchStyle.FoldersFirst ) {
                                                                             folder.FindFiles( fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: onFindFile, onEachDirectory: onEachDirectory, searchStyle: SearchStyle.FoldersFirst ); //recurse
                                                                         }

                                                                         try {
                                                                             foreach ( var file in folder.EnumerateFiles( searchPattern, SearchOption.TopDirectoryOnly ) ) {
                                                                                 var info = file;
                                                                                 await Task.Run( () => info.InternalSearchFoundFile( onFindFile, cancellation ) );
                                                                             }

#if DEEPDEBUG
                                                                             $"Done searching {folder.Name} for {searchPattern}.".WriteLine();
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
                                                                                                   ex.More();
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
                                                                     ex.More();
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
                                      ex.More();
                                      return false;
                                  } );
            }
        }

        private static FileInfo InternalSearchFoundFile( this FileInfo info, Action< FileInfo > onFindFile, [CanBeNull] SimpleCancel cancellation ) {
            try {
                if ( ( cancellation != null ) && !cancellation.HaveAnyCancellationsBeenRequested() ) {
                    onFindFile?.Invoke( info );
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return info;
        }

        //public static DriveInfo GetDriveWithLargestAvailableFreeSpace() {
        //	return DriveInfo.GetDrives().AsParallel().Where( info => info.IsReady ).FirstOrDefault( driveInfo => driveInfo.AvailableFreeSpace >= DriveInfo.GetDrives().AsParallel().Max( info => info.AvailableFreeSpace ) );
        //}

        public static DriveInfo GetLargestEmptiestDrive() => DriveInfo.GetDrives()
                                                                      .AsParallel()
                                                                      .Where( info => info.IsReady )
                                                                      .OrderByDescending( info => info.AvailableFreeSpace )
                                                                      .FirstOrDefault();

        public static UInt32? GetFileSizeOnDisk( this Document document ) => GetFileSizeOnDisk( new FileInfo( document.FullPathWithFileName ) );

        [NotNull]
        public static String SimplifyFileName( [NotNull] this Document document /*, Folder hintFolder*/ ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension( document.FileName );

            TryAgain:

            //check for a double extension (image.jpg.tif), remove the 'fake' (.tif) extension
            if ( !Path.GetExtension( fileNameWithoutExtension )
                      .IsNullOrEmpty() ) {
                fileNameWithoutExtension = Path.GetFileNameWithoutExtension( fileNameWithoutExtension );
                goto TryAgain;
            }

            //TODO we have the document, see if we can just chop off down to a nonexisting filename.. just get rid of (3) or (2) or (1)

            var splitIntoWords = fileNameWithoutExtension.Split( new[] {' '}, StringSplitOptions.RemoveEmptyEntries )
                                                         .ToList();

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

            return $"{fileNameWithoutExtension}{document.Extension}";
        }

        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static UInt32? GetFileSizeOnDisk( this FileInfo info ) {
            UInt32 clusterSize = 1;
            if ( info.Directory != null ) {
                var driveLetter = info.Directory.Root.FullName.TrimEnd( '\\' );
                using ( var searcher = new ManagementObjectSearcher( $"select BlockSize,NumberOfBlocks from Win32_Volume WHERE DriveLetter = '{driveLetter}'" ) ) {
                    var bob = searcher.Get()
                                      .Cast< ManagementObject >()
                                      .First();
                    clusterSize = ( UInt32 ) bob[ "BlockSize" ];
                }
            }
            UInt32 hosize;
            var losize = NativeWin32.GetCompressedFileSizeW( info.FullName, out hosize );
            var size = ( hosize << 32 ) | losize;
            return ( size + clusterSize - 1 ) / clusterSize * clusterSize;
        }

        /// <summary>
        ///     <para>
        ///         The code above does not work properly on Windows Server 2008 or 2008 R2 or Windows 7 and
        ///         Vista based systems as cluster size is always zero (GetDiskFreeSpaceW and
        ///         GetDiskFreeSpace return -1 even with UAC disabled.)
        ///     </para>
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <seealso cref="http://stackoverflow.com/questions/3750590/get-size-of-file-on-disk" />
        public static UInt32? GetFileSizeOnDiskAlt( this FileInfo info ) {
            UInt32 sectorsPerCluster = 1;
            UInt32 bytesPerSector = 0;
            if ( info.Directory != null ) {
                UInt32 dummy;
                var result = NativeWin32.GetDiskFreeSpaceW( lpRootPathName: info.Directory.Root.FullName, lpSectorsPerCluster: out sectorsPerCluster, lpBytesPerSector: out bytesPerSector, lpNumberOfFreeClusters: out dummy, lpTotalNumberOfClusters: out dummy );
                if ( result == 0 ) {
                    throw new Win32Exception();
                }
            }
            var clusterSize = sectorsPerCluster * bytesPerSector;
            UInt32 sizeHigh;
            var losize = NativeWin32.GetCompressedFileSizeW( lpFileName: info.FullName, lpFileSizeHigh: out sizeHigh );
            var size = ( sizeHigh << 32 ) | losize;
            return ( size + clusterSize - 1 ) / clusterSize * clusterSize;
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
            var pickedfile = files.OrderBy( r => Randem.Next() )
                                  .FirstOrDefault();
            if ( ( pickedfile != null ) && File.Exists( pickedfile ) ) {
                return new FileInfo( pickedfile ).FullName;
            }
            return String.Empty;
        }

        public static Boolean IsDirectory( this NativeWin32.Win32FindData data ) => ( data.dwFileAttributes & FileAttributes.Directory ) == FileAttributes.Directory;

        public static Boolean IsFile( this NativeWin32.Win32FindData data ) => !IsDirectory( data );

        public static Boolean IsIgnoreFolder( this NativeWin32.Win32FindData data ) => data.cFileName.EndsLike( "$RECYCLE.BIN" ) || data.cFileName.Like( "TEMP" ) || data.cFileName.Like( "TMP" ) || SystemFolders.Contains( new Folder( data.cFileName ) );

        public static Boolean IsParentOrCurrent( this NativeWin32.Win32FindData data ) => ( data.cFileName == "." ) || ( data.cFileName == ".." );

        public static Boolean IsProtected( [NotNull] this FileSystemInfo fileSystemInfo ) {
            if ( fileSystemInfo == null ) {
                throw new ArgumentNullException( nameof( fileSystemInfo ) );
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
        public static Boolean IsReparsePoint( this NativeWin32.Win32FindData data ) => ( data.dwFileAttributes & FileAttributes.ReparsePoint ) == FileAttributes.ReparsePoint;

        /// <summary>
        ///     Opens a folder with Explorer.exe
        /// </summary>
        public static void OpenDirectoryWithExplorer( [CanBeNull] this DirectoryInfo folder ) {
            folder.Should()
                  .NotBeNull();
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
            windowsFolder.Should()
                         .NotBeNullOrWhiteSpace();
            if ( String.IsNullOrWhiteSpace( windowsFolder ) ) {
                return;
            }

            var proc = Process.Start( fileName: $"{windowsFolder}\\explorer.exe", arguments: $"/e,\"{folder.FullName}\"" );
            ( proc?.Responding ).Should()
                                .Be( true );
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
                throw new ArgumentNullException( nameof( info ) );
            }
            if ( info.Directory == null ) {
                throw new NullReferenceException( "info.directory" );
            }
            var now = Convert.ToString( value: DateTime.UtcNow.ToBinary(), toBase: 16 );
            var formatted = $"{Path.GetFileNameWithoutExtension( info.Name )} {now}{newExtension ?? info.Extension}";
            var path = Path.Combine( info.Directory.FullName, formatted );
            return new FileInfo( path );
        }

        /// <summary>
        ///     untested. is this written correctly? would it read from a *slow* media but not block the
        ///     calling function?
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bufferSize"></param>
        /// <param name="fileMissingRetries"></param>
        /// <param name="retryDelay"></param>
        /// <returns></returns>
        public static async Task< String > ReadTextAsync( String filePath, Int32? bufferSize = 4096, Int32? fileMissingRetries = 10, TimeSpan? retryDelay = null ) {
            if ( String.IsNullOrWhiteSpace( filePath ) ) {
                throw new ArgumentNullException( nameof( filePath ) );
            }

            if ( !bufferSize.HasValue ) {
                bufferSize = 4096;
            }
            if ( !retryDelay.HasValue ) {
                retryDelay = Seconds.One;
            }

            while ( fileMissingRetries.HasValue && ( fileMissingRetries.Value > 0 ) ) {
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
                        var buffer = new Byte[bufferSize.Value];
                        Int32 numRead;
                        while ( ( numRead = await sourceStream.ReadAsync( buffer, 0, buffer.Length ) ) != 0 ) {
                            var text = Encoding.Unicode.GetString( buffer, 0, numRead );
                            sb.Append( text );
                        }

                        return sb.ToString();
                    }
                }
                catch ( FileNotFoundException exception ) {
                    exception.More();
                }
            }
            return String.Empty;
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
            if ( ( left == null ) || ( right == null ) ) {
                return false;
            }

            if ( !left.Exists ) {
                return false;
            }
            if ( !right.Exists ) {
                return false;
            }
            if ( left.Length != right.Length ) {
                return false;
            }

            var lba = left.AsByteArray()
                          .ToArray();
            var rba = right.AsByteArray()
                           .ToArray();

            return lba.SequenceEqual( rba );
        }

        /// <summary>
        ///     <para>performs a byte by byte file comparison</para>
        /// </summary>
        /// <param name="leftFileName"></param>
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
            if ( ( leftFileName == null ) || ( rightFileName == null ) ) {
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

            var lba = leftFileName.AsByteArray()
                                  .ToArray();
            var rba = rightFileName.AsByteArray()
                                   .ToArray();

            return lba.SequenceEqual( rba );
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
            if ( ( left == null ) || ( right == null ) ) {
                return false;
            }

            return ( left.GetLength() == ( UInt64 ) right.Length ) && left.AsByteArray()
                                                                          .SequenceEqual( right.AsByteArray() );
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
            if ( ( left == null ) || ( right == null ) ) {
                return false;
            }

            return ( ( UInt64 ) left.Length == right.GetLength() ) && left.AsByteArray()
                                                                          .SequenceEqual( right.AsByteArray() );
        }

        /// <summary>
        ///     Search all possible drives for any files matching the
        ///     <paramref name="fileSearchPatterns" /> .
        /// </summary>
        /// <param name="fileSearchPatterns">List of patterns to search for.</param>
        /// <param name="cancellation"></param>
        /// <param name="onFindFile"><see cref="Action" /> to perform when a file is found.</param>
        /// <param name="onEachDirectory"><see cref="Action" /> to perform on each folder found.</param>
        /// <param name="searchStyle"></param>
        public static void SearchAllDrives( [NotNull] this IEnumerable< String > fileSearchPatterns, SimpleCancel cancellation, Action< FileInfo > onFindFile = null, Action< DirectoryInfo > onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
            if ( fileSearchPatterns == null ) {
                throw new ArgumentNullException( nameof( fileSearchPatterns ) );
            }
            try {
                DriveInfo.GetDrives()
                         .AsParallel()
                         .WithDegreeOfParallelism( 26 )
                         .WithExecutionMode( ParallelExecutionMode.ForceParallelism )
                         .ForAll( drive => {
                                      if ( !drive.IsReady || ( drive.DriveType == DriveType.NoRootDirectory ) || !drive.RootDirectory.Exists ) {
                                          return;
                                      }
                                      $"Scanning [{drive.VolumeLabel}]".WriteLine();
                                      drive.RootDirectory.FindFiles( fileSearchPatterns: fileSearchPatterns, cancellation: cancellation, onFindFile: onFindFile, onEachDirectory: onEachDirectory, searchStyle: searchStyle );
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
                                      ex.More();
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
                exception.More();
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
                exception.More();
            }
            return false;
        }

        public static ManagementPath ToManagementPath( this DirectoryInfo systemPath ) {
            var fullPath = systemPath.FullName;
            while ( fullPath.EndsWith( @"\", StringComparison.Ordinal ) ) {
                fullPath = fullPath.Substring( 0, fullPath.Length - 1 );
            }
            fullPath = "Win32_Directory.Name=\"" + fullPath.Replace( "\\", "\\\\" ) + "\"";
            var managed = new ManagementPath( fullPath );
            return managed;
        }

        public static IEnumerable< String > ToPaths( [NotNull] this DirectoryInfo directoryInfo ) {
            if ( directoryInfo == null ) {
                throw new ArgumentNullException( nameof( directoryInfo ) );
            }
            return directoryInfo.ToString()
                                .Split( Path.DirectorySeparatorChar );
        }

        public static MemoryStream TryCopyStream( String filePath, Boolean bePatient = true, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite ) {
            //TODO
            TryAgain:
            var memoryStream = new MemoryStream();
            try {
                if ( File.Exists( filePath ) ) {
                    using ( var fileStream = File.Open( path: filePath, mode: fileMode, access: fileAccess, share: fileShare ) ) {
                        var length = ( Int32 ) fileStream.Length;
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
                    if ( !Thread.Yield() ) {
                        Thread.Sleep( 0 );
                    }
                    goto TryAgain;
                }
            }
            return memoryStream;
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

                directoryInfo = new DirectoryInfo( path ); //try it anyways
                return true;
            }
            catch ( UriFormatException ) { }
            catch ( SecurityException ) { }
            catch ( PathTooLongException ) { }
            catch ( InvalidOperationException ) { }
            return false;
        }

        /// <summary>
        ///     Returns a temporary <see cref="Document" /> (but does not create the file in the file system).
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="document"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Boolean TryGetTempDocument( [NotNull] this Folder folder, [NotNull] out Document document, String extension = null ) {
            if ( folder == null ) {
                throw new ArgumentNullException( nameof( folder ) );
            }
            try {
                var randomFileName = Guid.NewGuid()
                                         .ToString();
                if ( String.IsNullOrWhiteSpace( extension ) ) {
                    randomFileName = Path.Combine( folder.FullName, Path.GetFileName( randomFileName ) );
                }
                else {
                    if ( !extension.StartsWith( "." ) ) {
                        extension = $".{extension}";
                    }
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
            document = default ( Document );
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
        ///     A valid FileStream object for the opened file, or null if the File could not be opened
        ///     after the required attempts
        /// </returns>
        [CanBeNull]
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

        [CanBeNull]
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
                if ( !bePatient ) {
                    return null;
                }
                if ( !Thread.Yield() ) {
                    Thread.Sleep( 0 );
                }
                goto TryAgain;
            }
            return null;
        }

        [CanBeNull]
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

        public static Int32? TurnOnCompression( [NotNull] this FileInfo info ) {
            if ( info == null ) {
                throw new ArgumentNullException( nameof( info ) );
            }
            if ( !info.Exists ) {
                info.Refresh();
                if ( !info.Exists ) {
                    return null;
                }
            }

            var lpBytesReturned = 0;
            Int16 compressionFormatDefault = 1;

            using ( var fileStream = File.Open( path: info.FullName, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.None ) ) {
                var success = false;

                try {
                    if ( fileStream.SafeFileHandle != null ) {
                        fileStream.SafeFileHandle.DangerousAddRef( success: ref success );

                        var result = NativeWin32.DeviceIoControl( hDevice: fileStream.SafeFileHandle.DangerousGetHandle(), dwIoControlCode: FsctlSetCompression, lpInBuffer: ref compressionFormatDefault, nInBufferSize: sizeof ( Int16 ), lpOutBuffer: IntPtr.Zero, nOutBufferSize: 0, lpBytesReturned: ref lpBytesReturned, lpOverlapped: IntPtr.Zero );
                    }
                }
                finally {
                    fileStream.SafeFileHandle?.DangerousRelease();
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

        /// <summary>
        /// </summary>
        /// <param name="heading"></param>
        /// <param name="body"></param>
        /// <param name="longDuration"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Note: All toast templates available in the Toast Template Catalog
        ///     (http://msdn.microsoft.com/en-us/Library/windows/apps/hh761494.aspx)
        /// </remarks>
        public static ToastNotification CreateTextOnlyToast( this String heading, String body, Boolean longDuration = false ) {
            // Using the ToastText02 toast template. Retrieve the content part of the toast so we
            // can change the text.
            var xml = ToastNotificationManager.GetTemplateContent( ToastTemplateType.ToastText02 );

            //Find the text component of the content
            var textElements = xml.GetElementsByTagName( "text" );

            // Set the text on the toast. The first line of text in the ToastText02 template is
            // treated as header text, and will be bold.
            textElements[ 0 ].AppendChild( xml.CreateTextNode( heading ) );
            textElements[ 1 ].AppendChild( xml.CreateTextNode( body ) );

            if ( longDuration ) {
                // Set the duration on the toast
                var toastNode = xml.SelectSingleNode( "/toast" ) as XmlElement;
                toastNode?.SetAttribute( "duration", "long" );
            }

            // Create the actual toast object using this toast specification.
            return new ToastNotification( xml );
        }

        /// <summary>
        ///     Where does this method belong?
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="longDuration"></param>
        /// <param name="popup"></param>
        /// <returns></returns>
        public static Boolean TryToast( this String applicationId, String header, String body, Boolean longDuration = false, Boolean popup = true ) {
            var notifier = ToastNotificationManager.CreateToastNotifier( applicationId );
            try {
                if ( notifier.Setting == NotificationSetting.Enabled ) {
                    var toast = CreateTextOnlyToast( header, body, longDuration );

                    // Set SuppressPopup = true on the toast in order to send it directly to action
                    // center without producing a popup.
                    toast.SuppressPopup = !popup;

                    // Send the toast.
                    notifier.Show( toast );

                    return true;
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return false;
        }

    }

}
