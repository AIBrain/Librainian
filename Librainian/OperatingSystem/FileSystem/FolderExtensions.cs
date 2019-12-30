// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FolderExtensions.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "FolderExtensions.cs" was last formatted by Protiguous on 2019/08/08 at 9:16 AM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Permissions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using ComputerSystem.Devices;
    using JetBrains.Annotations;
    using Parsing;
    using Threading;

    // ReSharper disable RedundantUsingDirective
    using Path = Pri.LongPath.Path;
    using Directory = Pri.LongPath.Directory;
    using DirectoryInfo = Pri.LongPath.DirectoryInfo;
    using File = Pri.LongPath.File;
    using FileSystemInfo = Pri.LongPath.FileSystemInfo;
    using FileInfo = Pri.LongPath.FileInfo;
    // ReSharper restore RedundantUsingDirective


    public static class FolderExtensions {

        /*
        public static Char[] InvalidPathChars {
            get;
        } = Path.GetInvalidPathChars();
        */

        /*
        [NotNull]
        public static String CleanupForFolder([NotNull] this String foldername) {
            if (String.IsNullOrWhiteSpace(foldername)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(foldername));
            }

            var sb = new StringBuilder(foldername.Length, UInt16.MaxValue / 2);

            foreach (var c in foldername) {
                if (!InvalidPathChars.Contains(c)) {
                    sb.Append(c);
                }
            }

   //         var idx = foldername.IndexOfAny( InvalidPathChars );

			//while ( idx.Any() ) {
   //             if ( idx.Any() ) {
   //                 foldername = foldername.Remove( idx, 1 );
   //             }
			//	idx = foldername.IndexOfAny( InvalidPathChars );
			//}
   //         return foldername.Trim();

            return sb.ToString().Trim();
        }
        */

        /// <summary>
        ///     Returns a list of all files copied.
        /// </summary>
        /// <param name="sourceFolder">                 </param>
        /// <param name="destinationFolder">            </param>
        /// <param name="searchPatterns">               </param>
        /// <param name="overwriteDestinationDocuments"></param>
        /// <param name="crc">                          Calculate the CRC64 of source and destination documents.</param>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<DocumentCopyStatistics> CopyFiles( [NotNull] this Folder sourceFolder, [NotNull] Folder destinationFolder,
            IEnumerable<String> searchPatterns, Boolean overwriteDestinationDocuments = true, Boolean crc = true ) {
            if ( sourceFolder is null ) {
                throw new ArgumentNullException( nameof( sourceFolder ) );
            }

            if ( destinationFolder is null ) {
                throw new ArgumentNullException( nameof( destinationFolder ) );
            }

            var documentCopyStatistics = new ConcurrentBag<DocumentCopyStatistics>();

            if ( !sourceFolder.HavePermission( FileIOPermissionAccess.Read ) ) {
                return documentCopyStatistics;
            }

            if ( !destinationFolder.HavePermission( FileIOPermissionAccess.Write ) ) {
                return documentCopyStatistics;
            }

            var sourceFiles = sourceFolder.GetDocuments( searchPatterns );

            Parallel.ForEach( sourceFiles.AsParallel(), CPU.AllCPUExceptOne, sourceDocument => {
                try {
                    var beginTime = DateTime.UtcNow;

                    var statistics = new DocumentCopyStatistics {
                        TimeStarted = beginTime,
                        SourceDocument = sourceDocument
                    };

                    if ( crc ) {
                        statistics.SourceDocumentCRC64 = sourceDocument.CRC64Hex();
                    }

                    var destinationDocument = new Document( destinationFolder, sourceDocument.FileName );

                    if ( overwriteDestinationDocuments && destinationDocument.Exists() ) {
                        destinationDocument.Delete();
                    }

                    File.Copy( sourceDocument.FullPath, destinationDocument.FullPath );

                    if ( crc ) {
                        statistics.DestinationDocumentCRC64 = destinationDocument.CRC64Hex();
                    }

                    var endTime = DateTime.UtcNow;

                    if ( destinationDocument.Exists() == false ) {
                        return;
                    }

                    statistics.BytesCopied = destinationDocument.Size().GetValueOrDefault( 0 );

                    if ( crc ) {
                        statistics.BytesCopied *= 2;
                    }

                    statistics.TimeTaken = endTime - beginTime;
                    statistics.DestinationDocument = destinationDocument;
                    documentCopyStatistics.Add( statistics );
                }
                catch ( Exception ) {

                    //swallow any errors
                }
            } );

            return documentCopyStatistics;
        }

        public static IEnumerable<IFolder> FindFolder( [NotNull] this String folderName ) {
            if ( folderName is null ) {
                throw new ArgumentNullException( nameof( folderName ) );
            }

            //First check across all known drives.
            var found = false;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach ( var drive in DriveInfo.GetDrives() ) {
                var path = Path.Combine( drive.RootDirectory.FullName, folderName );
                var asFolder = new Folder( path );

                if ( asFolder.Exists() ) {
                    found = true;

                    yield return asFolder;
                }
            }

            if ( found ) {
                yield break;
            }

            //Next, check subfolders, beginning with the first drive.
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach ( var drive in Disk.GetDrives() ) {
                var folders = drive.GetFolders();

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach ( var folder in folders ) {
                    var parts = SplitPath( ( Folder )folder ); //TODO fix this cast

                    if ( parts.Any( s => s.Like( folderName ) ) ) {
                        found = true;

                        yield return folder;
                    }
                }
            }

            if ( !found ) { }
        }

        /// <summary>
        ///     <see cref="PathSplitter" />.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<String> SplitPath( [NotNull] String path ) {
            if ( String.IsNullOrWhiteSpace( value: path ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( path ) );
            }

            return path.Split( Folder.FolderSeparatorChar ).Where( s => !s.IsNullOrWhiteSpace() );
        }

        /// <summary>
        ///     <see cref="PathSplitter" />.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<String> SplitPath( [NotNull] this DirectoryInfo info ) {
            if ( info is null ) {
                throw new ArgumentNullException( nameof( info ) );
            }

            return SplitPath( info.FullName );
        }

        /// <summary>
        ///     <para>Returns true if the <see cref="Document" /> no longer seems to exist.</para>
        ///     <para>Returns null if existence cannot be determined.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="tryFor"></param>
        /// <returns></returns>
        public static Boolean? TryDeleting( this Folder folder, TimeSpan tryFor ) {
            var stopwatch = Stopwatch.StartNew();
            TryAgain:

            try {
                if ( !folder.Exists() ) {
                    return true;
                }

                Directory.Delete( folder.FullName );

                return !Directory.Exists( folder.FullName );
            }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) {

                // IOExcception is thrown when the file is in use by any process.
                if ( stopwatch.Elapsed <= tryFor ) {
                    Thread.Yield();
                    Application.DoEvents();

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
    }
}