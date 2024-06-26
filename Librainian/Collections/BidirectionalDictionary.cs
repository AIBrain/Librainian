﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "BidirectionalDictionary.cs" last formatted on 2021-11-30 at 7:16 PM by Protiguous.

#nullable enable

namespace Librainian.Collections;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

/// <summary>
/// This class provide service for both the singularization and pluralization, it takes the word pairs in the ctor following
/// the rules that the first one is singular and the second one is plural.
/// </summary>
public class BidirectionalDictionary<TSingle, TPlural> where TSingle : class where TPlural : class {

	protected BidirectionalDictionary( IDictionary<TSingle, TPlural> firstToSecondDictionary ) {
		foreach ( var pair in firstToSecondDictionary ) {
			this.AddValue( pair );
		}
	}

	public BidirectionalDictionary() {
	}

	private ConcurrentDictionary<TPlural, TSingle?> PluralToSingle { get; } = new();

	private ConcurrentDictionary<TSingle, TPlural?> SingleToPlural { get; } = new();

	public void AddValue( TSingle single, TPlural plural ) {
		this.SingleToPlural[ single ] = plural;

		this.PluralToSingle[ plural ] = single;
	}

	public void AddValue( (TSingle single, TPlural plural) values ) => this.AddValue( values.single, values.plural );

	public void AddValue( KeyValuePair<TSingle, TPlural> pair ) => this.AddValue( pair.Key, pair.Value );

	public virtual Boolean ExistsInPlural( TPlural value ) => this.PluralToSingle.ContainsKey( value );

	public virtual Boolean ExistsInSingle( TSingle value ) => this.SingleToPlural.ContainsKey( value );

	public virtual TPlural? GetPlural( TSingle value ) => this.SingleToPlural.TryGetValue( value, out var second ) ? second : default( TPlural? );

	public virtual TSingle? GetSingle( TPlural value ) => this.PluralToSingle.TryGetValue( value, out var first ) ? first : default( TSingle? );
}