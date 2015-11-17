// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/FactDatabase.cs" was last cleaned by Rick on 2015/06/12 at 2:59 PM

namespace Librainian.Knowledge {

    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using System.Windows.Forms;
    using JetBrains.Annotations;
    using Linguistics;
    using OperatingSystem.FileSystem;
    using OperatingSystem.IO;
    using Parsing;
    using Threading;

    public class FactDatabase {

        /// <summary></summary>
        [NotNull]
        public readonly ConcurrentBag<Document> KnbFiles = new ConcurrentBag<Document>();

        public Int32 FilesFound {
            get; private set;
        }

        public Int32 AddFile(Document dataFile, ProgressChangedEventHandler feedback = null) {
            if ( dataFile == null ) {
                throw new ArgumentNullException( nameof( dataFile ) );
            }

            if ( !dataFile.Extension.Like( ".knb" ) ) {
                return 0;
            }

            ++this.FilesFound;
            feedback?.Invoke( this, new ProgressChangedEventArgs( this.FilesFound, $"Found data file {dataFile.FileName}" ) );

            if ( !this.KnbFiles.Contains( dataFile ) ) {
                this.KnbFiles.Add( dataFile );
            }

            //TODO text, xml, csv, html, etc...

            return 0;
        }

        public async Task DoRandomEntryAsync(ActionBlock<Sentence> action, SimpleCancel cancellation) {
            if ( null == action ) {
                return;
            }

            await Task.Run( () => {
                if ( cancellation.HaveAnyCancellationsBeenRequested() ) {
                    return;
                }

                //pick random line from random file
                var file = this.KnbFiles.OrderBy( o => Randem.Next() ).FirstOrDefault();
                if ( null == file ) {
                    return;
                }

                if ( cancellation.HaveAnyCancellationsBeenRequested() ) {
                    return;
                }

                try {

                    //pick random line
                    var line = File.ReadLines( file.FullPathWithFileName ).Where( s => !String.IsNullOrWhiteSpace( s ) ).Where( s => Char.IsLetter( s[ 0 ] ) ).OrderBy( o => Randem.Next() ).FirstOrDefault();

                    //TODO new ActionBlock<Action>( action: action => {
                    //Threads.AIBrain().Input( line );
                    if ( !String.IsNullOrEmpty( line ) && !cancellation.HaveAnyCancellationsBeenRequested() ) {
                        action.TryPost( new Sentence( line ) );
                    }
                }
                catch ( Exception exception ) {
                    exception.More();
                }
            } );
        }

        public void SearchForFactFiles(SimpleCancel cancellation) {
            Log.Enter();

            var searchPatterns = new[] { "*.knb" };

            var folder = new Folder( Path.Combine( Path.GetDirectoryName( Application.ExecutablePath ) ) );

            folder.DirectoryInfo.FindFiles( fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: file => this.AddFile( dataFile: new Document( file ) ), onEachDirectory: null, searchStyle: SearchStyle.FilesFirst );

            folder = new Folder( Environment.SpecialFolder.CommonDocuments );
            folder.DirectoryInfo.FindFiles( fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: file => this.AddFile( dataFile: new Document( file ) ), onEachDirectory: null, searchStyle: SearchStyle.FilesFirst );

            //folder.DirectoryInfo.FindFiles( fileSearchPatterns: searchPatterns, cancellationToken: cancellationToken, onFindFile: file => this.AddFile( dataFile: new Document( file ) ), onEachDirectory: null, searchStyle: SearchStyle.FilesFirst );

            //folder = new Folder( Environment.SpecialFolder.MyDocuments );
            //folder.DirectoryInfo.FindFiles( fileSearchPatterns: searchPatterns, cancellationToken: cancellationToken, onFindFile: file => this.AddFile( dataFile: new Document( file ) ), onEachDirectory: null, searchStyle: SearchStyle.FilesFirst );

            //if ( !this.KNBFiles.Any() ) {
            //    searchPatterns.SearchAllDrives( onFindFile: file => this.AddFile( dataFile: new Document( file ) ), cancellationToken: new CancellationToken() );
            //}
            Log.Exit();
        }
    }
}