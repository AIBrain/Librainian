// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Binary.cs" last formatted on 2020-08-14 at 8:35 PM.

#nullable enable

namespace Librainian.Maths.Numbers;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

/// <summary>Based from Hamming code found at http://maciejlis.com/hamming-code-algorithm-c-sharp/</summary>
[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
public class Binary : IEnumerable<Boolean> {

	private List<Boolean> Booleans { get; }

	public Int32 Length => this.Booleans.Count;

	public Boolean this[Int32 index] {
		get => this.Booleans[index];

		set => this.Booleans[index] = value;
	}

	public IReadOnlyCollection<Boolean> GetBooleans() => this.Booleans;

	public Binary( IReadOnlyCollection<Boolean> booleans ) {
		this.Booleans = booleans.ToList();
		this.Booleans.Capacity = this.Booleans.Count;
	}

	public Binary( IEnumerable<Boolean> binary ) : this( ( IReadOnlyCollection<Boolean> )binary ) { }

	public Binary( Int32 value ) : this( ConvertToBoolean( value ) ) { }

	public Binary( Int32 value, Int32 minSize ) : this( ConvertToBoolean( value, minSize ) ) { }

	public static Binary Concatenate( Binary a, Binary b ) {
		var result = new Binary( new Boolean[a.Length + b.Length] );
		var n = 0;

		foreach ( var bit in a ) {
			result[n] = bit;
			++n;
		}

		foreach ( var bit in b ) {
			result[n] = bit;
			++n;
		}

		return result;
	}

	public static IEnumerable<Boolean> ConvertToBoolean( Int32 value ) {
		return Convert.ToString( value, 2 ).Select( c => c == '1' );
	}

	public static IEnumerable<Boolean> ConvertToBoolean( Int32 value, Int32 minSize ) {
		var toBinary = new List<Boolean>( ConvertToBoolean( value ) );

		while ( toBinary.Count != minSize ) {
			toBinary.Insert( 0, false );
		}

		toBinary.TrimExcess();

		return toBinary;
	}

	public static Binary operator &( Binary a, Binary b ) {
		if ( a.Length != b.Length ) {
			throw new ArgumentException( "Binaries must have same length" );
		}

		var result = new Boolean[a.Length];

		for ( var i = 0; i < a.Length; i++ ) {
			result[i] = a[i] && b[i];
		}

		return new Binary( result );
	}

	public static Binary operator ^( Binary a, Binary b ) => Xor( a, b );

	public static Binary Xor( Binary a, Binary b ) {
		if ( a.Length != b.Length ) {
			throw new ArgumentException( "Binaries must have same length" );
		}

		var result = new Boolean[a.Length];

		for ( var i = 0; i < a.Length; i++ ) {
			result[i] = a[i] ^ b[i];
		}

		return new Binary( result );
	}

	public Int32 CountOnes() => this.Booleans.Count( bit => bit );

	public Int32 CountZeroes() => this.Booleans.Count( bit => !bit );

	public IEnumerator<Boolean> GetEnumerator() => this.Booleans.GetEnumerator();

	public override String ToString() {
		var stringBuilder = new StringBuilder( this.Length, this.Length );

		foreach ( var bit in this.Booleans ) {
			stringBuilder.Append( bit ? '1' : '0' );
		}

		return stringBuilder.ToString();
	}

	IEnumerator IEnumerable.GetEnumerator() => this.Booleans.GetEnumerator();
}