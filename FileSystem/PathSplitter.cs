// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "PathSplitter.cs",
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
// "Librainian/Librainian/PathSplitter.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using Collections;
    using JetBrains.Annotations;

    public class PathSplitter {

        public PathSplitter( String fullpathandfilename ) {
            this.FileName = Path.GetFileName( fullpathandfilename );
            this.OriginalPath = Path.GetDirectoryName( fullpathandfilename );

            if ( default == this.OriginalPath ) { return; }

            var strings = this.OriginalPath.Split( new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries );
            this.Parts.AddRange( strings );
        }

        public PathSplitter( Document document ) : this( document.FullPathWithFileName ) {
            if ( document is null ) { throw new ArgumentNullException( nameof( document ) ); }
        }

        public PathSplitter( Folder folder ) : this( folder.FullName ) { }

        [NotNull]
        private List<String> Parts { get; } = new List<String>( 3 );

        public String FileName { get; }

        [CanBeNull]
        public String OriginalPath { get; }

        public Boolean InsertRoot( [NotNull] String path ) {
            if ( path is null ) { throw new ArgumentNullException( nameof( path ) ); }

            this.Parts.Insert( 1, path );

            if ( path[1] == ':' ) { this.Parts.RemoveAt( 0 ); }

            return true;
        }

        /// <summary>
        ///     Returns the reconstructed path and filename.
        /// </summary>
        /// <returns></returns>
        public Document Recombined() {
            var folder = new Folder( this.Parts.ToStrings( Path.DirectorySeparatorChar ) );

            return new Document( folder, this.FileName );
        }

        public Boolean SubstituteDrive( Char c ) {
            if ( this.Parts[0].Length != 2 && this.Parts[0].EndsWith( ":", StringComparison.Ordinal ) ) { return false; }

            this.Parts[0] = new String( new[] { c } ) + ":";

            return true;
        }
    }
}