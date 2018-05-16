// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FactDatabase.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/FactDatabase.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

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
    using Threading;

    [JsonObject]
    public class FactDatabase {

        [JsonProperty]
        [NotNull]
        public readonly ConcurrentBag<Document> KnbFiles = new ConcurrentBag<Document>();

        [JsonProperty]
        public Int32 FilesFound;

        public Int32 AddFile( Document dataFile, ProgressChangedEventHandler feedback = null ) {
            if ( dataFile is null ) { throw new ArgumentNullException( nameof( dataFile ) ); }

            if ( !dataFile.Extension().Like( ".knb" ) ) { return 0; }

            Interlocked.Increment( ref this.FilesFound );
            feedback?.Invoke( this, new ProgressChangedEventArgs( this.FilesFound, $"Found data file {dataFile.FileName()}" ) );

            if ( !this.KnbFiles.Contains( dataFile ) ) { this.KnbFiles.Add( dataFile ); }

            //TODO text, xml, csv, html, etc...

            return 0;
        }

        public async Task ReadRandomFact( Action<String> action ) {
            if ( null == action ) { return; }

            await Task.Run( () => {

                //pick random line from random file
                var file = this.KnbFiles.OrderBy( o => Randem.Next() ).FirstOrDefault();

                if ( null == file ) { return; }

                try {

                    //pick random line
                    var line = File.ReadLines( file.FullPathWithFileName ).Where( s => !String.IsNullOrWhiteSpace( s ) ).Where( s => Char.IsLetter( s[0] ) ).OrderBy( o => Randem.Next() ).FirstOrDefault();
                    action( line );
                }
                catch ( Exception exception ) { exception.More(); }
            } );
        }

        public String SearchForFactFiles( SimpleCancel cancellation ) {
            Logging.Enter();

            try {
                var searchPatterns = new[] { "*.knb" };

                var folder = new Folder( Path.Combine( Path.GetDirectoryName( Application.ExecutablePath ) ) );

                folder.Info.FindFiles( fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: file => this.AddFile( dataFile: new Document( file ) ), onEachDirectory: null,
                    searchStyle: SearchStyle.FilesFirst );

                if ( !this.KnbFiles.Any() ) {
                    folder = new Folder( Environment.SpecialFolder.CommonDocuments );

                    folder.Info.FindFiles( fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: file => this.AddFile( dataFile: new Document( file ) ), onEachDirectory: null,
                        searchStyle: SearchStyle.FilesFirst );
                }

                if ( !this.KnbFiles.Any() ) { searchPatterns.SearchAllDrives( onFindFile: file => this.AddFile( dataFile: new Document( file ) ), cancellation: cancellation ); }

                return $"Found {this.KnbFiles.Count} KNB files";
            }
            finally { Logging.Exit(); }
        }
    }
}