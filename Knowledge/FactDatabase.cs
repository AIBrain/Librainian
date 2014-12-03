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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FactDatabase.cs" was last cleaned by Rick on 2014/10/21 at 5:01 AM

namespace Librainian.Knowledge {
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using System.Windows.Forms;
    using Annotations;
    using IO;
    using Linguistics;
    using Parsing;
    using Threading;

    public class FactDatabase {

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public readonly ConcurrentBag<Document> KNBFiles = new ConcurrentBag<Document>();

        public int FilesFound {
            get;
            private set;
        }

        public int AddFile( Document dataFile, ProgressChangedEventHandler feedback = null ) {
            if ( dataFile == null ) {
                throw new ArgumentNullException( "dataFile" );
            }

            if ( dataFile.Extension.Like( ".knb" ) ) {
                ++this.FilesFound;
                if ( feedback != null ) {
                    feedback( this, new ProgressChangedEventArgs( this.FilesFound, String.Format( "Found data file {0}", dataFile.FileName ) ) );
                }

                if ( !this.KNBFiles.Contains( dataFile ) ) {
                    this.KNBFiles.Add( dataFile );

                }
            }

            //TODO text, xml, csv, html, etc...

            return 0;
        }

        public async Task DoRandomEntryAsync( ActionBlock<Sentence> action, SimpleCancel cancellation ) {

            if ( null == action ) {
                return;
            }

            await Task.Run( () => {

                if ( cancellation.IsCancellationRequested ) {
                    return;
                }

                //pick random line from random file
                var file = this.KNBFiles.OrderBy( o => Randem.Next() ).FirstOrDefault();
                if ( null == file ) {
                    return;
                }

                if ( cancellation.IsCancellationRequested ) {
                    return;
                }

                try {
                    //pick random line
                    var line = File.ReadLines( file.FullPathWithFileName ).Where( s => !String.IsNullOrWhiteSpace( s ) ).Where( s => Char.IsLetter( s[ 0 ] ) ).OrderBy( o => Randem.Next() ).FirstOrDefault();

                    //TODO new ActionBlock<Action>( action: action => {
                    //Threads.AIBrain().Input( line );
                    if ( !String.IsNullOrEmpty( line ) && !cancellation.IsCancellationRequested ) {
                        action.TryPost( new Sentence( line ) );
                    }
                }
                catch ( Exception exception ) {
                    exception.Error();
                }
            } );
        }

        public void SearchForFactFiles( SimpleCancel cancellation ) {
            Report.Enter();

            var searchPatterns = new[] { "*.knb" };

            var folder = new Folder( Path.Combine( Path.GetDirectoryName( Application.ExecutablePath) ));

            folder.DirectoryInfo.FindFiles( fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: file => this.AddFile( dataFile: new Document( file ) ), onEachDirectory: null, searchStyle: SearchStyle.FilesFirst );


            //folder = new Folder( Environment.SpecialFolder.CommonDocuments );

            //folder.DirectoryInfo.FindFiles( fileSearchPatterns: searchPatterns, cancellationToken: cancellationToken, onFindFile: file => this.AddFile( dataFile: new Document( file ) ), onEachDirectory: null, searchStyle: SearchStyle.FilesFirst );

            //folder = new Folder( Environment.SpecialFolder.MyDocuments );
            //folder.DirectoryInfo.FindFiles( fileSearchPatterns: searchPatterns, cancellationToken: cancellationToken, onFindFile: file => this.AddFile( dataFile: new Document( file ) ), onEachDirectory: null, searchStyle: SearchStyle.FilesFirst );

            //if ( !this.KNBFiles.Any() ) {
            //    searchPatterns.SearchAllDrives( onFindFile: file => this.AddFile( dataFile: new Document( file ) ), cancellationToken: new CancellationToken() );
            //}
            Report.Exit();
        }

    }
}