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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks.Dataflow;
    using IO;
    using Linguistics;
    using Measurement.Time;
    using Parsing;
    using Threading;

    public class FactDatabase  {
        public readonly ConcurrentBag<FileInfo> KNBFiles = new ConcurrentBag<FileInfo>();

        public int FilesFound {
            get;
            set;
        }

        public int AddFile( FileInfo dataFile, ProgressChangedEventHandler feedback = null ) {
            if ( dataFile == null ) {
                throw new ArgumentNullException( "dataFile" );
            }
            var ext = Path.GetExtension( dataFile.FullName );
            if ( ext.Like( ".knb" ) ) {
                ++this.FilesFound;
                if ( feedback != null ) {
                    feedback( this, new ProgressChangedEventArgs( this.FilesFound, String.Format( "Found data file {0}", dataFile.Name ) ) );
                }

                //if ( !this.KNBFiles.Contains( dataFile ) ) {
                this.KNBFiles.Add( dataFile );

                //}
            }

            //TODO text, xml, csv, html, etc...

            return 0;
        }

        public void DoRandomEntry( ActionBlock<Sentence> action ) {
            Tasks.Spawn( ( () => {
                if ( null == action ) {
                    return;
                }

                //pick random line from random file
                var file = this.KNBFiles.OrderBy( o => Randem.Next() ).FirstOrDefault();
                if ( null == file ) {
                    return;
                }

                //pick random line
                var line = File.ReadLines( file.FullName ).Where( s => !String.IsNullOrWhiteSpace( s ) ).Where( s => Char.IsLetter( s[ 0 ] ) ).OrderBy( o => Randem.Next() ).FirstOrDefault();

                //TODO new ActionBlock<Action>( action: action => {
                //Threads.AIBrain().Input( line );
                if ( !String.IsNullOrEmpty( line ) ) {
                    action.TryPost( new Sentence( line ) );
                }
            } ) );
        }

        public void Initialize() {
            Seconds.One.Then( () => {
                Report.Enter();
                var cancellationToken = new CancellationToken();

                IEnumerable<String> fileSearchPatterns = new[] { "*.knb" };
                Action<FileInfo> onFindFile = file => this.AddFile( dataFile: file );
                new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.CommonDocuments ) ).FindFiles( fileSearchPatterns: fileSearchPatterns, cancellationToken: cancellationToken, onFindFile: onFindFile, onEachDirectory: null, searchStyle: SearchStyle.FilesFirst );

                IEnumerable<String> fileSearchPatterns1 = new[] { "*.knb" };
                Action<FileInfo> onFindFile1 = file => this.AddFile( dataFile: file );
                new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) ).FindFiles( fileSearchPatterns: fileSearchPatterns1, cancellationToken: cancellationToken, onFindFile: onFindFile1, onEachDirectory: null, searchStyle: SearchStyle.FilesFirst );

                if ( !this.KNBFiles.Any() ) {
                    IOExtensions.SearchAllDrives( fileSearchPatterns: new[] { "*.knb" }, onFindFile: file => this.AddFile( dataFile: file ), cancellationToken: new CancellationToken() );
                }
                Report.Exit();
            } );
        }

    }
}