// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "StringTable.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "StringTable.cs" was last formatted by Protiguous on 2018/06/26 at 12:52 AM.

namespace Librainian.Collections {

	using System;
	using System.Collections.Generic;
	using ComputerSystem.FileSystem;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Persistence;

	[JsonObject]
	public class StringTable {

		[JsonProperty]
		public PersistTable<UInt64, String> Ints { get; }

		[JsonProperty]
		public PersistTable<String, UInt64> Words { get; }

		/// <summary>
		///     Get or set the <paramref name="key" /> for this word.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public UInt64 this[ [NotNull] String key ] {
			get => this.Words.TryGetValue( key, out var result ) ? result : default;

			set {
				if ( String.IsNullOrEmpty( key ) ) {
					return;
				}

				this.Words[ key ] = value;
				this.Ints[ value ] = key;
			}
		}

		/// <summary>
		///     Get or set the word for this guid.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[CanBeNull]
		public String this[ UInt64 key ] {
			get => this.Ints[ key ];

			set {
				this.Words[ value ] = key;
				this.Ints[ key ] = value;
			}
		}

		public StringTable( Folder commonName ) {
			this.Ints = new PersistTable<UInt64, String>( folder: new Folder( folder: commonName, subFolder: nameof( this.Ints ) ), testForReadWriteAccess: true );
			this.Words = new PersistTable<String, UInt64>( folder: new Folder( folder: commonName, subFolder: nameof( this.Words ) ), testForReadWriteAccess: true );
		}

		public void Clear() {
			this.Words.Clear();
			this.Ints.Clear();
		}

		/// <summary>
		///     Returns true if the word is contained in the collections.
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		public Boolean Contains( [CanBeNull] String word ) {
			if ( String.IsNullOrEmpty( word ) ) {
				return false;
			}

			return this.Words.TryGetValue( word, out _ );
		}

		/// <summary>
		///     Returns true if the guid is contained in the collection.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Boolean Contains( UInt64 key ) => this.Ints.TryGetValue( key, out _ );

		[NotNull]
		public IEnumerable<UInt64> EachInt() => this.Ints.Keys;

		[NotNull]
		public IEnumerable<String> EachWord() => this.Words.Keys;
	}
}