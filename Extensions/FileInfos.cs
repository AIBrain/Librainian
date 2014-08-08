#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/FileInfos.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM

#endregion License & Information

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading;
    using Annotations;
    using Threading;

    public static class FileInfos {
        /*
                [Obsolete]
                public static readonly ActionBlock<Search> Searcher = new ActionBlock<Search>( search => {

                    //TODO each 'class/method/proc' needs to request its own Searcher

                    var folder = new DirectoryInfo( search.Path );
                    if ( !folder.Exists ) {
                        return;
                    }

                    //First: react to any files found.
                    folder.BetterEnumerateFiles( search.Pattern )
                          .AsParallel()
                          .ForAll( fileInfo => {
                              if ( null != search.OnFindFile ) {
                                  Tasks.Spawn( () => search.OnFindFile( fileInfo ) );
                              }
                          } );

                    //Second: search down into any subfolders.
                    folder.BetterEnumerateDirectories()
                          .AsParallel()
                          .ForAll( directoryInfo => {

                              //String.Format( "Scanning [{0}]", directoryInfo.FullName ).TimeDebug();
                              if ( null != search.OnFindFolder ) {
                                  Tasks.Spawn( () => search.OnFindFolder( directoryInfo ) );
                              }

                              var newSearch = new Search {
                                  Path = directoryInfo.FullName,
                                  Pattern = search.Pattern,
                                  OnFindFile = search.OnFindFile,
                                  OnFindFolder = search.OnFindFolder
                              };
                              Searcher.TryPost( newSearch );
                          } );
                }, Blocks.ManyProducers.ConsumeSerial );
        */

        // public static DateTime ImageCreationBestGuess( this FileSystemInfo info ) { var bestGuess
        // = DateTime.Now;
        //
        // var fileCreation = File.GetCreationTime( info.FullName ); if ( fileCreation <= bestGuess
        // ) { bestGuess = fileCreation; }
        //
        // var lastWrite = File.GetLastWriteTime( info.FullName ); if ( lastWrite <= bestGuess ) {
        // bestGuess = lastWrite; }
        //
        // try { using ( var image = Image.FromFile( info.FullName, false ) ) { if (
        // image.PropertyIdList.Contains( 36868 ) ) { //DateTimeDigitized var asDateTime =
        // GetProperteryAsDateTime( image.GetPropertyItem( 36868 ) ); if ( asDateTime.HasValue &&
        // asDateTime.Value < bestGuess ) { bestGuess = asDateTime.Value; } }
        //
        // if ( image.PropertyIdList.Contains( 36867 ) ) { //DateTimeOriginal var asDateTime =
        // GetProperteryAsDateTime( image.GetPropertyItem( 36867 ) ); if ( asDateTime.HasValue &&
        // asDateTime.Value < bestGuess ) { bestGuess = asDateTime.Value; } }
        //
        // if ( image.PropertyIdList.Contains( 306 ) ) { //PropertyTagDateTime var asDateTime =
        // GetProperteryAsDateTime( image.GetPropertyItem( 306 ) ); if ( asDateTime.HasValue &&
        // asDateTime.Value < bestGuess ) { bestGuess = asDateTime.Value; } } } } catch (
        // OutOfMemoryException ) { /*swallow bad images*/ }
        //
        // return bestGuess; }

        public enum SearchStyle {

            /// <summary>
            /// </summary>
            FilesFirst,

            /// <summary>
            /// </summary>
            FoldersFirst,
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

        /*

                /// <summary>
                /// No guarantee of <paramref name="onFind" /> order.
                /// </summary>
                /// <param name="searchPattern"></param>
                /// <param name="onFind"></param>
                /// <param name="startingFolder"></param>
                /// <param name="specialFolder"></param>
                /// <returns></returns>
                [Obsolete]
                public static void FindALLFiles( String searchPattern, Action<FileInfo> onFind, Environment.SpecialFolder? specialFolder = null, DirectoryInfo startingFolder = null ) {
                    if ( onFind == null ) {
                        throw new ArgumentNullException( "onFind" );
                    }
                    if ( String.IsNullOrWhiteSpace( searchPattern ) ) {
                        throw new ArgumentNullException( "searchPattern" );
                    }

                    var stopwatch = Stopwatch.StartNew();

                    if ( null == startingFolder && !specialFolder.HasValue ) {
                        DriveInfo.GetDrives()
                                 .AsParallel()
                                 .ForAll( drive => {
                                     try {
                                         if ( !drive.IsReady || drive.DriveType == DriveType.NoRootDirectory || !drive.RootDirectory.Exists ) {
                                             return;
                                         }
                                         String.Format( "Scanning [{0}]", drive.VolumeLabel )
                                               .TimeDebug();
                                         Searcher.TryPost( new Search {
                                             Pattern = searchPattern,
                                             Path = drive.RootDirectory.FullName,
                                             OnFindFile = onFind
                                         } );
                                     }
                                     catch ( Exception exception ) {
                                         exception.Log();
                                     }
                                 } );
                    }

                    if ( null != startingFolder ) {
                        Searcher.TryPost( new Search {
                            Pattern = searchPattern,
                            Path = startingFolder.FullName,
                            OnFindFile = onFind
                        } );
                    }

                    if ( specialFolder.HasValue ) {
                        Searcher.TryPost( new Search {
                            Pattern = searchPattern,
                            Path = Environment.GetFolderPath( specialFolder.Value ),
                            OnFindFile = onFind
                        } );
                    }

                    var timeTaken = stopwatch.Elapsed;
                    String.Format( "{0}", timeTaken.Simpler() )
                          .TimeDebug();
                }
        */

        /// <summary>
        /// Search all possible drives for any files matching the <paramref
        /// name="fileSearchPatterns" />.
        /// </summary>
        /// <param name="fileSearchPatterns">List of patterns to search for.</param>
        /// <param name="cancellationToken"></param>
        /// <param name="onFindFile"><see cref="Action" /> to perform when a file is found.</param>
        /// <param name="onEachDirectory"><see cref="Action" /> to perform on each folder found.</param>
        /// <param name="searchStyle"></param>
        public static void FindFiles( [NotNull] IEnumerable<String> fileSearchPatterns, CancellationToken cancellationToken, Action<FileInfo> onFindFile = null, Action<DirectoryInfo> onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
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
                    ex.Log();
                    return false;
                } );
            }
        }

        /// <summary>
        /// Search the <paramref name="startingFolder" /> for any files matching the <paramref
        /// name="fileSearchPatterns" />.
        /// </summary>
        /// <param name="fileSearchPatterns">List of patterns to search for.</param>
        /// <param name="startingFolder">The folder to start the search.</param>
        /// <param name="cancellationToken"></param>
        /// <param name="onFindFile"><see cref="Action" /> to perform when a file is found.</param>
        /// <param name="onEachDirectory"><see cref="Action" /> to perform on each folder found.</param>
        /// <param name="searchStyle"></param>
        public static void FindFiles( IEnumerable<String> fileSearchPatterns, DirectoryInfo startingFolder, CancellationToken cancellationToken, Action<FileInfo> onFindFile = null, Action<DirectoryInfo> onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
            if ( fileSearchPatterns == null ) {
                throw new ArgumentNullException( "fileSearchPatterns" );
            }
            if ( startingFolder == null ) {
                throw new ArgumentNullException( "startingFolder" );
            }
            try {
                var searchPatterns = fileSearchPatterns as IList<String> ?? fileSearchPatterns.ToList();
                searchPatterns.AsParallel().WithDegreeOfParallelism( 1 ).ForAll( searchPattern => {
#if DEEPDEBUG

    //String.Format( "Searching folder {0} for {1}.", startingFolder.FullName, searchPattern ).TimeDebug();
#endif
                    if ( cancellationToken.IsCancellationRequested ) {
                        return;
                    }
                    try {
                        var folders = startingFolder.EnumerateDirectories( "*", SearchOption.TopDirectoryOnly );
                        folders.AsParallel().WithDegreeOfParallelism( 1 ).ForAll( folder => {
#if DEEPDEBUG

    //String.Format( "Found folder {0}.", folder ).TimeDebug();
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
                                exception.Log();
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
                                        exception.Log();
                                    }
                                } );

                                //String.Format( "Done searching {0} for {1}.", folder.Name, searchPattern ).TimeDebug();
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
                                    ex.Log();
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
                    ex.Log();
                    return false;
                } );
            }
        }

        /// <summary>
        /// Search the <paramref name="startingFolder" /> for any files matching the <paramref
        /// name="fileSearchPatterns" />.
        /// </summary>
        /// <param name="fileSearchPatterns">List of patterns to search for.</param>
        /// <param name="startingFolder">The folder to start the search.</param>
        /// <param name="cancellationToken"></param>
        /// <param name="onFindFile"><see cref="Action" /> to perform when a file is found.</param>
        /// <param name="onEachDirectory"><see cref="Action" /> to perform on each folder found.</param>
        /// <param name="searchStyle"></param>
        public static void FindFiles( IEnumerable<String> fileSearchPatterns, Environment.SpecialFolder startingFolder, CancellationToken cancellationToken, Action<FileInfo> onFindFile = null, Action<DirectoryInfo> onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
            if ( fileSearchPatterns == null ) {
                throw new ArgumentNullException( "fileSearchPatterns" );
            }
            FindFiles( fileSearchPatterns: fileSearchPatterns, cancellationToken: cancellationToken, startingFolder: new DirectoryInfo( Environment.GetFolderPath( startingFolder ) ), onFindFile: onFindFile, onEachDirectory: onEachDirectory, searchStyle: searchStyle );
        }

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

        /// <summary>
        /// Before: "hello.txt".
        /// After: "hello 345680969061906730476346.txt"
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

        /*
                public static void FindFiles( IEnumerable<String> searchPatterns, DirectoryInfo startingFolder, Action<FileInfo> onFindFile ) {
                    try {
                        searchPatterns.AsParallel()
                                      .ForAll( searchPattern => {
                                          String.Format( "Searching {0} for {1}.", startingFolder.Name, searchPattern )
                                                .TimeDebug();
                                          var files = startingFolder.EnumerateFiles( searchPattern, SearchOption.AllDirectories );
                                          try {
                                              files.AsParallel()
                                                   .ForAll( file => {

                                                       //String.Format( "Found {0}.", file ).TimeDebug();
                                                       try {
                                                           onFindFile( file );
                                                       }
                                                       catch ( Exception exception ) {
                                                           exception.Log();
                                                       }
                                                   } );
                                              String.Format( "Done searching {0} for {1}.", startingFolder.Name, searchPattern )
                                                    .TimeDebug();
                                          }
                                          catch ( AggregateException exception ) {
                                              exception.Handle( ex => {
                                                  if ( ex is UnauthorizedAccessException ) {
                                                      return true;
                                                  }
                                                  if ( ex is DirectoryNotFoundException ) {
                                                      return true;
                                                  }
                                                  if ( ex is SecurityException ) {
                                                      return true;
                                                  }
                                                  ex.Log();
                                                  return false;
                                              } );
                                          }
                                      } );
                    }
                    catch ( AggregateException exception ) {
                        exception.Handle( ex => {
                            if ( ex is UnauthorizedAccessException ) {
                                return true;
                            }
                            if ( ex is DirectoryNotFoundException ) {
                                return true;
                            }
                            if ( ex is SecurityException ) {
                                return true;
                            }
                            ex.Log();
                            return false;
                        } );
                    }
                }
        */
    }
}