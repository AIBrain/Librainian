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
// File "D.cs" last formatted on 2021-11-30 at 7:22 PM by Protiguous.

#nullable enable

namespace Librainian.Persistence;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Exceptions;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Parsing;

/// <summary>
/// <para>[D]ata([K]ey=[V]alue)</para>
/// <para>[K] is not mutable, and can be an empty string, and contain whitespace.</para>
/// <para>[V] is mutable, and can be a null string.</para>
/// </summary>
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Serializable]
[JsonObject( MemberSerialization.OptIn, IsReference = false, ItemIsReference = false, /*ItemNullValueHandling = NullValueHandling.Ignore,*/
	ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore )]
public class D : IEqualityComparer<D> {

	public D() {
		this.K = String.Empty;
		this.V = null;
	}

	public D( String key ) => this.K = key ?? throw new NullException( nameof( key ) );

	public D( String key, String? value ) {
		this.K = key ?? throw new NullException( nameof( key ) );
		this.V = value;
	}

	/// <summary>The key.</summary>
	[JsonProperty( IsReference = false, ItemIsReference = false )]
	public String K { get; }

	/// <summary>The value.</summary>
	[JsonProperty( IsReference = false, ItemIsReference = false )]
	public String? V { get; set; }

	/// <summary>
	/// <para>Static equality test.</para>
	/// <para>Return true if: K and K have the same value, and V and V have the same value.</para>
	/// <para>Two nulls should be equal.</para>
	/// <para>Comparison is by <see cref="StringComparison.Ordinal" />.</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equals( D? left, D? right ) {
		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		if ( left is null || right is null ) {
			return false;
		}

		if ( !left.K.Equals( right.K, StringComparison.Ordinal ) ) {
			return false;
		}

		if ( ReferenceEquals( left.V, right.V ) ) {
			return true;
		}

		if ( left.V is null || right.V is null ) {
			return false;
		}

		return left.V.Equals( right.V, StringComparison.Ordinal );
	}

	public override Boolean Equals( Object? obj ) => Equals( this, obj as D );

	public Int32 GetHashCode( D d ) => d.K.GetHashCode();

	Boolean IEqualityComparer<D>.Equals( D? x, D? y ) => Equals( x, y );

	public override Int32 GetHashCode() => this.K.GetHashCode();

	public override String ToString() {
		String keypart;

		if ( this.K.Length > 22 ) {
			var left = Strings.Left( this.K, 10 );
			var right = Strings.Right( this.K, 10 );

			keypart = $"{left}..{right}";
		}
		else {
			keypart = this.K;
		}

		if ( this.V is null ) {
			return $"{keypart}={Symbols.Null}";
		}

		var valuepart = String.Empty;

		if ( this.V.Length > 22 ) {
			var left = Strings.Left( this.V, 10 );
			var right = Strings.Right( this.V, 10 );

			valuepart = $"{left}..{right}";
		}

		return $"{keypart}={valuepart}";
	}
}