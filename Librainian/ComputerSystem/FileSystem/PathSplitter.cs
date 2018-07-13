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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "PathSplitter.cs" was last formatted by Protiguous on 2018/07/10 at 8:55 PM.

namespace Librainian.ComputerSystem.FileSystem {

	using System;
	using System.Collections.Generic;
	using System.IO;
	using Collections;
	using JetBrains.Annotations;

	public class PathSplitter {

		[NotNull]
		private List<String> Parts { get; } = new List<String>( 3 );

		public String FileName { get; }

		[CanBeNull]
		public String OriginalPath { get; }

		public PathSplitter( String fullpathandfilename ) {
			this.FileName = Path.GetFileName( fullpathandfilename );
			this.OriginalPath = Path.GetDirectoryName( fullpathandfilename );

			if ( default == this.OriginalPath ) { return; }

			var strings = this.OriginalPath.Split( new[] {
				Path.DirectorySeparatorChar
			}, StringSplitOptions.RemoveEmptyEntries );

			this.Parts.AddRange( strings );
		}

		public PathSplitter( [NotNull] Document document ) : this( document.FullPathWithFileName ) {
			if ( document is null ) { throw new ArgumentNullException( nameof( document ) ); }
		}

		public PathSplitter( [NotNull] Folder folder ) : this( folder.FullName ) { }

		public Boolean InsertRoot( [NotNull] String path ) {
			if ( path is null ) { throw new ArgumentNullException( nameof( path ) ); }

			this.Parts.Insert( 1, path );

			if ( path[ 1 ] == ':' ) { this.Parts.RemoveAt( 0 ); }

			return true;
		}

		/// <summary>
		///     Returns the reconstructed path and filename.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public Document Recombined() {
			var folder = new Folder( this.Parts.ToStrings( Path.DirectorySeparatorChar ) );

			return new Document( folder, this.FileName );
		}

		public Boolean SubstituteDrive( Char c ) {
			if ( this.Parts[ 0 ].Length != 2 && this.Parts[ 0 ].EndsWith( ":", StringComparison.Ordinal ) ) { return false; }

			this.Parts[ 0 ] = new String( new[] {
				c
			} ) + ":";

			return true;
		}
	}
}