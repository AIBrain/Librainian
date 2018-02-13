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
// "Librainian/PathSplitter.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using Collections;
    using JetBrains.Annotations;

    public class PathSplitter {

        public PathSplitter( String fullpathandfilename ) {
            this.Parts = new List<String>();
            this.FileName = Path.GetFileName( fullpathandfilename );
            this.OriginalPath = Path.GetDirectoryName( fullpathandfilename );
            var originalPath = this.OriginalPath;
            if ( originalPath == null ) {
                return;
            }
            var strings = originalPath.Split( new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries );
            this.Parts.AddRange( strings );
        }

        public PathSplitter( Document document ) : this( document.FullPathWithFileName ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }
        }

        public PathSplitter( Folder folder ) : this( folder.FullName ) {
        }

        public String FileName {
            get;
        }

        [CanBeNull]
        public String OriginalPath {
            get;
        }

        [NotNull]
        private List<String> Parts {
            get;
        }

        public Boolean InsertRoot( [NotNull] String path ) {
            if ( path == null ) {
                throw new ArgumentNullException( nameof( path ) );
            }
            this.Parts.Insert( 1, path );
            if ( path[ 1 ] == ':' ) {
                this.Parts.RemoveAt( 0 );
            }
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
            if ( ( this.Parts[ 0 ].Length != 2 ) && this.Parts[ 0 ].EndsWith( ":", StringComparison.Ordinal ) ) {
                return false;
            }
            this.Parts[ 0 ] = new String( new[] { c } ) + ":";
            return true;
        }
    }
}