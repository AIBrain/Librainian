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
// "Librainian/FileInfos.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Extensions {
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
