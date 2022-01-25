// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "StringTable.cs" last formatted on 2022-12-22 at 5:14 PM by Protiguous.

namespace Librainian.Collections;

using System;
using System.Collections.Generic;
using Exceptions;
using FileSystem;
using Newtonsoft.Json;
using Persistence;

[JsonObject]
public class StringTable {

	public StringTable( Folder commonName ) {
		if ( commonName is null ) {
			throw new ArgumentEmptyException( nameof( commonName ) );
		}

		this.Ints = new PersistTable<UInt64, String>( new Folder( commonName, nameof( this.Ints ) ), true );
		this.Words = new PersistTable<String, UInt64>( new Folder( commonName, nameof( this.Words ) ), true );
	}

	[JsonProperty]
	public PersistTable<UInt64, String> Ints { get; }

	[JsonProperty]
	public PersistTable<String, UInt64> Words { get; }

	/// <summary>Get or set the <paramref name="key" /> for this word.</summary>
	/// <param name="key"></param>
	public UInt64 this[ String key ] {
		get => this.Words.TryGetValue( key, out var result ) ? result : default( UInt64 );

		set {
			if ( String.IsNullOrEmpty( key ) ) {
				return;
			}

			this.Words[ key ] = value;
			this.Ints[ value ] = key;
		}
	}

	/// <summary>Get or set the word for this guid.</summary>
	/// <param name="key"></param>
	public String? this[ UInt64 key ] {
		get => this.Ints[ key ];

		set {
			if ( value != null ) {
				this.Words[ value ] = key;
			}

			this.Ints[ key ] = value;
		}
	}

	public void Clear() {
		this.Words.Clear();
		this.Ints.Clear();
	}

	/// <summary>Returns true if the word is contained in the collections.</summary>
	/// <param name="word"></param>
	public Boolean Contains( String? word ) {
		if ( String.IsNullOrEmpty( word ) ) {
			return false;
		}

		return this.Words.TryGetValue( word, out var _ );
	}

	/// <summary>Returns true if the guid is contained in the collection.</summary>
	/// <param name="key"></param>
	public Boolean Contains( UInt64 key ) => this.Ints.TryGetValue( key, out var _ );

	public ICollection<UInt64> EachInt() => this.Ints.Keys;

	public ICollection<String> EachWord() => this.Words.Keys;
}