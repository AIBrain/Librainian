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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "BigInt.cs" last touched on 2021-10-13 at 4:27 PM by Protiguous.

namespace Librainian.Maths.Numbers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions;

/// <summary>http://codereview.stackexchange.com/a/99085/26303</summary>
[Immutable]
public class BigInt {

	public BigInt( String number ) => this.Integer = CalculateBigInteger( number );

	public BigInt( List<Int32>? list ) => this.Integer = list;

	public List<Int32> Integer { get; }

	private static List<Int32> CalculateBigInteger( String number ) => number.Reverse().Select( chararcter => Int32.Parse( chararcter.ToString() ) ).ToList();

	private static Int32 NumberAdd( Int32 value1, Int32 value2, ref Int32 carryOver ) {
		var addResult = value1 + value2 + carryOver;
		carryOver = addResult / 10;
		var addValue = addResult % 10;

		return addValue;
	}

	public static BigInt Add( BigInt int1, BigInt int2 ) {
		var result = new List<Int32>();

		var carryOver = 0;

		using IEnumerator<Int32> enumerator1 = int1.Integer.GetEnumerator();
		using IEnumerator<Int32> enumerator2 = int2.Integer.GetEnumerator();

		enumerator1.MoveNext();
		enumerator2.MoveNext();

		var hasNext1 = true;
		var hasNext2 = true;

		while ( hasNext1 || hasNext2 ) {
			var value = NumberAdd( enumerator1.Current, enumerator2.Current, ref carryOver );
			result.Add( value );

			hasNext1 = enumerator1.MoveNext();
			hasNext2 = enumerator2.MoveNext();
		}

		if ( carryOver != 0 ) {
			result.Add( carryOver );
		}

		return new BigInt( result );
	}

	public override String ToString() {
		var sb = new StringBuilder();

		foreach ( var number in this.Integer ) {
			sb.Append( number.ToString() );
		}

		var reverseString = sb.ToString().ToCharArray();
		Array.Reverse( reverseString );

		return new String( reverseString );
	}

}