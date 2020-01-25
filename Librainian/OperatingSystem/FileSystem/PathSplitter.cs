// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "PathSplitter.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "PathSplitter.cs" was last formatted by Protiguous on 2019/12/02 at 3:58 AM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Collections.Extensions;
    using JetBrains.Annotations;
    using Parsing;
    //using LanguageExt.Prelude;

    public class PathSplitter {

        [NotNull]
        [ItemNotNull]
        private List<String> Parts { get; } = new List<String>();

        [NotNull]
        public String FileName { get; }

        /// <summary>
        /// Null when equal to (is) the root folder.
        /// </summary>
        [CanBeNull]
        public String OriginalPath { get; }

        [NotNull]
        private static IEnumerable<String> Split( [NotNull] String path ) {
            if ( String.IsNullOrWhiteSpace( value: path ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.",  nameof( path ) );
            }

            return path.Split( new[] {
                Path.DirectorySeparatorChar
            }, StringSplitOptions.RemoveEmptyEntries );
        }

        public PathSplitter( [NotNull] Folder folder ) : this( new Document(folder.FullName), default ) { }

        public PathSplitter( [NotNull] IDocument document, String newExtension = default ) {
            if ( document == null ) {
                throw new ArgumentNullException(  nameof( document ) );
            }

            newExtension = newExtension.Trimmed() ?? document.Extension() ?? String.Empty;

            if ( !newExtension.StartsWith( "." ) ) {
                newExtension = $".{newExtension}";
            }

            this.FileName = $"{document.JustName()}{newExtension}";

            this.OriginalPath = Path.GetDirectoryName( document.FullPath );

            this.Parts.Clear();
            if ( !String.IsNullOrEmpty( this.OriginalPath ) ) {
                this.Parts.AddRange( Split( this.OriginalPath ) );
            }
            this.Parts.TrimExcess();
        }

        //[DebuggerStepThrough]
        public Boolean InsertRoot( [NotNull] Folder path ) {
            if ( path is null ) {
                throw new ArgumentNullException( nameof( path ) );
            }

            this.Parts.Insert( 1, path.FullName );

            if ( path.FullName[ 1 ] == ':' ) {
                this.Parts.RemoveAt( 0 );   //inserting a drive:\folder? remove the original drive:\folder part
            }

            return true;
        }

        /// <summary>
        /// Replace the original path, with <paramref name="replacement"/> path, not changing the filename.
        /// </summary>
        /// <param name="replacement"></param>
        /// <returns></returns>
        //[DebuggerStepThrough]
        public Boolean ReplacePath( [NotNull] IFolder replacement ) {
            if ( replacement == null ) {
                throw new ArgumentNullException(  nameof( replacement ) );
            }

            this.Parts.Clear();
            this.Parts.AddRange( Split( replacement.FullName ) );
            this.Parts.TrimExcess();

            return true;
        }

        public Boolean AddSubFolder( [NotNull] String subfolder ) {
            subfolder = Folder.CleanPath( subfolder.Trim() );

            if ( String.IsNullOrWhiteSpace( value: subfolder ) ) {
                return false;
            }

            this.Parts.Add( subfolder );

            return true;
        }

        /// <summary>Returns the reconstructed path and filename.</summary>
        /// <returns></returns>
        [NotNull]
        [DebuggerStepThrough]
        public Document Recombined() {
            var folder = new Folder( this.Parts.ToStrings( Path.DirectorySeparatorChar ) );

            return new Document( folder, this.FileName );
        }

        [DebuggerStepThrough]
        public Boolean SubstituteDrive( Char d ) {
            var s = this.Parts[ 0 ] ?? String.Empty;

            if ( s.Length != 2 || !s.EndsWith( ":", StringComparison.Ordinal ) ) {
                return false;
            }

            this.Parts[ 0 ] = $"{d}:";

            return true;
        }

    }

}