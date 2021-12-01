// Copyright © Protiguous. All Rights Reserved.
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
// File "BigIntegerHelper.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Maths.Bigger;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Exceptions;

/// <summary>Sqrt and NRoot acquired from http://mjs5.com/2016/01/20/c-biginteger-helper-constructors</summary>
public static class BigIntegerHelper {

	public static BigInteger Clone( this BigInteger source ) => new( source.ToByteArray() );

	public static BigInteger GCD( IEnumerable<BigInteger> numbers ) {
		if ( numbers == null ) {
			throw new NullException( nameof( numbers ) );
		}

		return numbers.Aggregate( GCD );
	}

	public static BigInteger GCD( BigInteger value1, BigInteger value2 ) {
		var absValue1 = BigInteger.Abs( value1 );
		var absValue2 = BigInteger.Abs( value2 );

		while ( absValue1 != 0 && absValue2 != 0 ) {
			if ( absValue1 > absValue2 ) {
				absValue1 %= absValue2;
			}
			else {
				absValue2 %= absValue1;
			}
		}

		return BigInteger.Max( absValue1, absValue2 );
	}

	public static Int32 GetLength( this BigInteger source ) {
		var result = 0;
		var copy = source.Clone();
		while ( copy > 0 ) {
			copy /= 10;
			result++;
		}

		return result;
	}

	public static IEnumerable<BigInteger> GetRange( BigInteger min, BigInteger max ) {
		var counter = min;

		while ( counter < max ) {
			yield return counter;
			counter++;
		}
	}

	public static Boolean IsCoprime( BigInteger value1, BigInteger value2 ) => GCD( value1, value2 ) == 1;

	public static BigInteger LCM( IEnumerable<BigInteger> numbers ) {
		if ( numbers == null ) {
			throw new NullException( nameof( numbers ) );
		}

		return numbers.Aggregate( LCM );
	}

	public static BigInteger LCM( BigInteger num1, BigInteger num2 ) {
		var absValue1 = BigInteger.Abs( num1 );
		var absValue2 = BigInteger.Abs( num2 );
		return absValue1 * absValue2 / GCD( absValue1, absValue2 );
	}

	// Returns the NTHs root of a BigInteger with Remainder. The root must be greater than or equal to 1 or value must be a
	// positive integer.
	public static BigInteger NthRoot( this BigInteger value, Int32 root, ref BigInteger remainder ) {
		if ( root < 1 ) {
			throw new Exception( "root must be greater than or equal to 1" );
		}

		if ( value.Sign == -1 ) {
			throw new Exception( "value must be a positive integer" );
		}

		if ( value == BigInteger.One ) {
			remainder = 0;
			return BigInteger.One;
		}

		if ( value == BigInteger.Zero ) {
			remainder = 0;
			return BigInteger.Zero;
		}

		if ( root == 1 ) {
			remainder = 0;
			return value;
		}

		var upperbound = value;
		var lowerbound = BigInteger.Zero;

		while ( true ) {
			var nval = ( upperbound + lowerbound ) >> 1;
			var tstsq = BigInteger.Pow( nval, root );
			if ( tstsq > value ) {
				upperbound = nval;
			}

			if ( tstsq < value ) {
				lowerbound = nval;
			}

			if ( tstsq == value ) {
				lowerbound = nval;
				break;
			}

			if ( lowerbound == upperbound - 1 ) {
				break;
			}
		}

		remainder = value - BigInteger.Pow( lowerbound, root );
		return lowerbound;
	}

	public static BigInteger Square( this BigInteger input ) => input * input;

	public static BigInteger SquareRoot( BigInteger input ) {
		if ( input.IsZero ) {
			return new BigInteger( 0 );
		}

		var n = new BigInteger( 0 );
		var p = new BigInteger( 0 );
		var low = new BigInteger( 0 );
		var high = BigInteger.Abs( input );

		while ( high > low + 1 ) {
			n = ( high + low ) >> 1;
			p = n * n;
			if ( input < p ) {
				high = n;
			}
			else if ( input > p ) {
				low = n;
			}
			else {
				break;
			}
		}

		return input == p ? n : low;
	}
}