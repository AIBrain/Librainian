// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FactDatabase.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Knowledge {

    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using FileSystem;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using Threading;

    [JsonObject]
    public class FactDatabase {

        [JsonProperty]
        [NotNull]
        public readonly ConcurrentBag<Document> KnbFiles = new ConcurrentBag<Document>();

        [JsonProperty]
        public Int32 FilesFound;

        public Int32 AddFile( Document dataFile, ProgressChangedEventHandler feedback = null ) {
            if ( dataFile == null ) {
                throw new ArgumentNullException( nameof( dataFile ) );
            }

            if ( !dataFile.Extension().Like( ".knb" ) ) {
                return 0;
            }

            Interlocked.Increment( ref this.FilesFound );
            feedback?.Invoke( this, new ProgressChangedEventArgs( this.FilesFound, $"Found data file {dataFile.FileName()}" ) );

            if ( !this.KnbFiles.Contains( dataFile ) ) {
                this.KnbFiles.Add( dataFile );
            }

            //TODO text, xml, csv, html, etc...

            return 0;
        }

        public async Task ReadRandomFact( Action<String> action ) {
            if ( null == action ) {
                return;
            }

            await Task.Run( () => {

                //pick random line from random file
                var file = this.KnbFiles.OrderBy( o => Randem.Next() ).FirstOrDefault();
                if ( null == file ) {
                    return;
                }

                try {

                    //pick random line
                    var line = File.ReadLines( file.FullPathWithFileName ).Where( s => !String.IsNullOrWhiteSpace( s ) ).Where( s => Char.IsLetter( s[ 0 ] ) ).OrderBy( o => Randem.Next() ).FirstOrDefault();
                    action( line );
                }
                catch ( Exception exception ) {
                    exception.More();
                }
            } );
        }

        public String SearchForFactFiles( SimpleCancel cancellation ) {
            Log.Enter();

            try {
                var searchPatterns = new[] { "*.knb" };

                var folder = new Folder( Path.Combine( Path.GetDirectoryName( Application.ExecutablePath ) ) );

                folder.Info.FindFiles( fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: file => this.AddFile( dataFile: new Document( file ) ), onEachDirectory: null, searchStyle: SearchStyle.FilesFirst );

                if ( !this.KnbFiles.Any() ) {
                    folder = new Folder( Environment.SpecialFolder.CommonDocuments );
                    folder.Info.FindFiles( fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: file => this.AddFile( dataFile: new Document( file ) ), onEachDirectory: null, searchStyle: SearchStyle.FilesFirst );
                }

                if ( !this.KnbFiles.Any() ) {
                    searchPatterns.SearchAllDrives( onFindFile: file => this.AddFile( dataFile: new Document( file ) ), cancellation: new SimpleCancel() );
                }
                return $"Found {this.KnbFiles.Count} KNB files";
            }
            finally {
                Log.Exit();
            }
        }
    }
}