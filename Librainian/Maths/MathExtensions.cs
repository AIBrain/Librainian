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
// File "MathExtensions.cs" last formatted on 2021-11-30 at 7:19 PM by Protiguous.

#nullable enable

namespace Librainian.Maths;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Collections.Extensions;
using Exceptions;
using ExtendedNumerics;
using JetBrains.Annotations;
using Numbers;

public static class MathExtensions {

	/// <summary>Fake!</summary>
	public const Decimal EpsilonBigDecimal = 0.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001m;

	/// <summary>
	/// <para>Return the smallest possible value above <see cref="Decimal.Zero" /> for a <see cref="Decimal" />.</para>
	/// </summary>
	public const Decimal EpsilonDecimal = 0.0000000000000000000000000001m;

	// you may want to pass this and use generics to allow more or less bits
	/// <summary>Store the complete list of values that will fit in a 32-bit unsigned integer without overflow.</summary>
	private static UInt32[] FibonacciLookup { get; } = {
		1, 1, 2, 3, 5, 8, 13, 21, 34, 55,
		89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765,
		10946, 17711, 28657, 46368, 75025, 121393, 196418, 317811, 514229, 832040,
		1346269, 2178309, 3524578, 5702887, 9227465, 14930352, 24157817, 39088169, 63245986, 102334155,
		165580141, 267914296, 433494437, 701408733, 1134903170, 1836311903, 2971215073
	};

	/// <summary>Add two <see cref="UInt64" />.</summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="overflowed">True if the two values added to more than <see cref="UInt64.MaxValue" /></param>
	public static UBigInteger Add( this UInt64 left, UInt64 right, out Boolean overflowed ) {
		var result = new UBigInteger( left ) + new UBigInteger( right );
		overflowed = result > UInt64.MaxValue;

		return result;
	}

	/// <summary>Allow <paramref name="left" /> to increase or decrease by a signed number;</summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="overflowed">True if the two values added to more than <see cref="UInt64.MaxValue" /></param>
	public static BigInteger Add( this UInt64 left, Int64 right, out Boolean overflowed ) {
		var result = new BigInteger( left ) + new BigInteger( right );
		overflowed = result > UInt64.MaxValue;

		return result;
	}

	/// <summary>
	/// <para>Add <paramref name="tax" /> of <paramref name="number" /> to <paramref name="number" />.</para>
	/// <para>If the tax is 6% on $50, then you would call this function like this:</para>
	/// <code>var withTax = 50.AddTax( 0.06 );</code>
	/// <code>Assert( withTax == 53.00 );</code>
	/// </summary>
	/// <param name="number"></param>
	/// <param name="tax"></param>
	[Pure]
	public static Decimal AddTax( this Decimal number, Decimal tax ) => number * ( 1.0m + tax );

	/// <summary>
	/// <para>Add <paramref name="percentTax" /> of <paramref name="number" /> to <paramref name="number" />.</para>
	/// <para>If the tax is 6% on $50, then you would call this function like this:</para>
	/// <code>var withTax = AddTaxPercent( 50.00, 6.0 );</code>
	/// <code>Assert( withTax == 53.00 );</code>
	/// </summary>
	/// <param name="number"></param>
	/// <param name="percentTax"></param>
	[Pure]
	public static Decimal AddTaxPercent( this Decimal number, Decimal percentTax ) => number * ( 1.0m + percentTax / 100.0m );

	public static BigDecimal BigEpsilon( this BigDecimal _ ) => EpsilonBigDecimal;

	/// <summary>Combine two <see cref="UInt32" /> values into one <see cref="UInt64" /> value. Use Split() for the reverse.</summary>
	/// <param name="high"></param>
	/// <param name="low"></param>
	[Pure]
	public static UInt64 Combine( this UInt32 high, UInt32 low ) => ( ( UInt64 )high << 32 ) | low;

	/// <summary>Combine two bytes into one <see cref="UInt16" />.</summary>
	/// <param name="low"></param>
	/// <param name="high"></param>
	public static UInt16 CombineBytes( this Byte low, Byte high ) =>
		BitConverter.ToUInt16( BitConverter.IsLittleEndian ? new[] {
			high, low
		} : new[] {
			low, high
		}, 0 );

	/// <summary>Combine two bytes into one <see cref="UInt16" /> with little endianess.</summary>
	/// <param name="low"></param>
	/// <param name="high"></param>
	/// <see cref="CombineTwoBytesLittleEndianess" />
	[Pure]
	public static UInt16 CombineTwoBytesHighEndianess( this Byte low, Byte high ) => ( UInt16 )( high + ( low << 8 ) );

	/// <summary>Combine two bytes into one <see cref="UInt16" /> with little endianess.</summary>
	/// <param name="low"></param>
	/// <param name="high"></param>
	/// <see cref="CombineTwoBytesHighEndianess" />
	[Pure]
	public static UInt16 CombineTwoBytesLittleEndianess( this Byte low, Byte high ) => ( UInt16 )( low + ( high << 8 ) );

	/// <summary>
	/// Combine two byte arrays into one byte array.
	/// <para>Warning: this allocates a new array.</para>
	/// </summary>
	/// <param name="first"></param>
	/// <param name="second"></param>
	public static Byte[] Concat( this Byte[] first, Byte[] second ) {
		var buffer = new Byte[ first.Length + second.Length ];
		Buffer.BlockCopy( first, 0, buffer, 0, first.Length );
		Buffer.BlockCopy( second, 0, buffer, first.Length, second.Length );

		return buffer;
	}

	/// <summary>
	/// Add a byte onto the end of the <paramref name="first" /> array.
	/// <para>Warning: this allocates a new array.</para>
	/// </summary>
	/// <param name="first"></param>
	/// <param name="second"></param>
	public static Byte[] Concat( this Byte[] first, Byte second ) {
		var buffer = new Byte[ first.Length + 1 ];
		Buffer.BlockCopy( first, 0, buffer, 0, first.Length );
		buffer[ ^1 ] = second;

		return buffer;
	}

	/// <summary>ConvertBigIntToBcd</summary>
	/// <param name="numberToConvert"></param>
	/// <param name="howManyBytes"></param>
	/// <see cref="http://github.com/mkadlec/ConvertBigIntToBcd/blob/master/ConvertBigIntToBcd.cs" />
	public static Byte[] ConvertBigIntToBcd( this Int64 numberToConvert, Int32 howManyBytes ) {
		var convertedNumber = new Byte[ howManyBytes ];
		var strNumber = numberToConvert.ToString();
		var currentNumber = String.Empty;

		for ( var i = 0; i < howManyBytes; i++ ) {
			convertedNumber[ i ] = 0xff;
		}

		for ( var i = 0; i < strNumber.Length; i++ ) {
			currentNumber += strNumber[ i ].ToString();

			if ( i == strNumber.Length - 1 && i % 2 == 0 ) {
				convertedNumber[ i / 2 ] = 0xf;
				convertedNumber[ i / 2 ] |= ( Byte )( ( Int32.Parse( currentNumber ) % 10 ) << 4 );
			}

			if ( i % 2 == 0 ) {
				continue;
			}

			var value = Int32.Parse( currentNumber );
			convertedNumber[ ( i - 1 ) / 2 ] = ( Byte )( value % 10 );
			convertedNumber[ ( i - 1 ) / 2 ] |= ( Byte )( ( value / 10 ) << 4 );
			currentNumber = String.Empty;
		}

		return convertedNumber;
	}

	/// <summary>Remove everything after the decimal point.</summary>
	/// <param name="x"></param>
	[Pure]
	public static Double Crop( this Double x ) => Math.Truncate( x * 100.0D ) / 100.0D;

	/// <summary>Remove everything after the decimal point.</summary>
	/// <param name="x"></param>
	[Pure]
	public static Single Crop( this Single x ) => ( Single )( Math.Truncate( x * 100.0f ) / 100.0f );

	/// <summary>Return the cube (^3) of the number.</summary>
	/// <param name="number"></param>
	[Pure]
	public static Single Cubed( this Single number ) => number * number * number;

	/// <summary>Return the cube (^3) of the number.</summary>
	/// <param name="number"></param>
	[Pure]
	public static Double Cubed( this Double number ) => number * number * number;

	/// <summary>Return the cube (^3) of the number.</summary>
	/// <param name="number"></param>
	[Pure]
	public static Decimal Cubed( this Decimal number ) => number * number * number;

	/// <summary>Needs tested.</summary>
	/// <param name="d"></param>
	public static String Decimal2Packed( this Decimal d ) {
		var output = new Boolean[ 10 ];
		var input = new Boolean[ 12 ];

		for ( var i = 0; i < 3; i++ ) {
			var a = ( Int32 )( ( Int32 )d / Math.Pow( 10, i ) ) % 10;

			for ( var j = 0; j < 4; j++ ) {
				input[ j + i * 4 ] = ( a & ( 1 << j ) ) != 0;
			}
		}

		output[ 0 ] = input[ 0 ];
		output[ 1 ] = input[ 7 ] | ( input[ 11 ] & input[ 3 ] ) | false;
		output[ 2 ] = input[ 11 ] | ( input[ 7 ] & input[ 3 ] ) | false;
		output[ 3 ] = input[ 11 ] | input[ 7 ] | input[ 3 ];
		output[ 4 ] = input[ 4 ];
		output[ 5 ] = input[ 5 ] | false | ( input[ 11 ] & input[ 3 ] );
		output[ 6 ] = false | ( input[ 7 ] & input[ 3 ] );
		output[ 7 ] = input[ 8 ];
		output[ 8 ] = input[ 9 ] | ( input[ 11 ] & input[ 1 ] ) | false;
		output[ 9 ] = input[ 10 ] | ( input[ 11 ] & input[ 2 ] ) | false;

		var sb = new StringBuilder();

		for ( var i = 9; i >= 0; i-- ) {
			sb.Append( output[ i ] ? '1' : '0' );
		}

		return sb.ToString();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Pure]
	public static Int32 Dubble( this Int32 number ) => number * 2;

	/// <summary>
	/// <para>Return the smallest possible value above <see cref="Decimal.Zero" /> for a <see cref="Decimal" />.</para>
	/// <para>1E-28</para>
	/// </summary>
	/// <param name="_"></param>
	[Pure]
	public static Decimal Epsilon( this Decimal _ ) => EpsilonDecimal;

	[Pure]
	public static Double Erf( this Double x ) {

		// constants
		const Double a1 = 0.254829592;
		const Double a2 = -0.284496736;
		const Double a3 = 1.421413741;
		const Double a4 = -1.453152027;
		const Double a5 = 1.061405429;
		const Double p = 0.3275911;

		// Save the sign of x
		var sign = x < 0 ? -1 : 1;
		x = Math.Abs( x );

		// A&S formula 7.1.26
		var t = 1.0 / ( 1.0 + p * x );
		var y = 1.0 - ( ( ( ( a5 * t + a4 ) * t + a3 ) * t + a2 ) * t + a1 ) * t * Math.Exp( -x * x );

		return sign * y;
	}

	/// <summary>
	/// Compute fibonacci series up to Max (&gt; 1). Example: foreach (int i in Fib(10)) { Console.WriteLine(i); }
	/// </summary>
	/// <param name="max"></param>
	public static IEnumerable<Int32> Fib( Int32 max ) {
		var a = 0;
		var b = 1;

		yield return 1;

		for ( var i = 0; i < max - 1; i++ ) {
			var c = a + b;

			yield return c;

			a = b;
			b = c;
		}
	}

	public static UInt32 FibonacciSequence( this UInt32 n ) => FibonacciLookup[ n ];

	[DebuggerStepThrough]
	[Pure]
	public static Double FiftyPercentOf( this Double x ) {
		var result = x * 0.5;

		return result < 1.0 ? 1 : result;
	}

	[DebuggerStepThrough]
	[Pure]
	public static Int32 FiftyPercentOf( this Int32 x ) {
		var result = x * 0.5;

		return result < 1.0 ? 1 : ( Int32 )result;
	}

	public static Int32 FlipBit( this Int32 value, Byte bitToFlip ) => value ^ bitToFlip;

	public static Int64 FlipBit( this Int64 value, Byte bitToFlip ) => value ^ bitToFlip;

	public static UInt64 FlipBit( this UInt64 value, Byte bitToFlip ) => value ^ bitToFlip;

	public static Byte FlipBit( this Byte value, Byte bitToFlip ) => ( Byte )( value ^ bitToFlip );

	[DebuggerStepThrough]
	[Pure]
	public static Byte ForceBounds( this Byte value, Byte minLimit, Byte maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static SByte ForceBounds( this SByte value, SByte minLimit, SByte maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static Int16 ForceBounds( this Int16 value, Int16 minLimit, Int16 maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static UInt16 ForceBounds( this UInt16 value, UInt16 minLimit, UInt16 maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static Int32 ForceBounds( this Int32 value, Int32 minLimit, Int32 maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static UInt32 ForceBounds( this UInt32 value, UInt32 minLimit, UInt32 maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static Single ForceBounds( this Single value, Single minLimit, Single maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static Double ForceBounds( this Double value, Double minLimit, Double maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static Int64 ForceBounds( this Int64 value, Int64 minLimit, Int64 maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static UInt64 ForceBounds( this UInt64 value, UInt64 minLimit, UInt64 maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static BigInteger ForceBounds( this BigInteger value, BigInteger minLimit, BigInteger maxLimit ) => BigInteger.Max( BigInteger.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static Decimal ForceBounds( this Decimal value, Decimal minLimit, Decimal maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static nint ForceBounds( this nint value, nint minLimit, nint maxLimit ) => Math.Max( Math.Min( value, maxLimit ), minLimit );

	[DebuggerStepThrough]
	[Pure]
	public static Int32 FractionOf( this Int32 x, Double top, Double bottom ) {
		var result = top * x / bottom;

		return result < 1.0 ? 1 : ( Int32 )result;
	}

	[DebuggerStepThrough]
	[Pure]
	public static Double FractionOf( this Double x, Double top, Double bottom ) => top * x / bottom;

	[DebuggerStepThrough]
	[Pure]
	public static Single FractionOf( this Single x, Single top, Single bottom ) => top * x / bottom;

	[DebuggerStepThrough]
	[Pure]
	public static UInt64 FractionOf( this UInt64 x, UInt64 top, UInt64 bottom ) => top * x / bottom;

	/// <summary>Greatest Common Divisor for int</summary>
	/// <remarks>Uses recursion, passing a remainder each time.</remarks>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static Int32 Gcd( this Int32 x, Int32 y ) {
		while ( true ) {
			if ( y == 0 ) {
				return x;
			}

			var x1 = x;
			x = y;
			y = x1 % y;
		}
	}

	/// <summary>Greatest Common Divisor for long</summary>
	/// <remarks>Uses recursion, passing a remainder each time.</remarks>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static Int64 Gcd( this Int64 x, Int64 y ) {
		while ( true ) {
			if ( y == 0 ) {
				return x;
			}

			var x1 = x;
			x = y;
			y = x1 % y;
		}
	}

	/// <summary>Greatest Common Divisor for int</summary>
	/// <remarks>Uses a while loop and remainder.</remarks>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public static Int32 GCD( Int32 a, Int32 b ) {
		while ( b != 0 ) {
			var remainder = a % b;
			a = b;
			b = remainder;
		}

		return a;
	}

	/// <summary>Greatest Common Divisor for long</summary>
	/// <remarks>Uses a while loop and remainder.</remarks>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public static Int64 GCD( Int64 a, Int64 b ) {
		while ( b != 0 ) {
			var remainder = a % b;
			a = b;
			b = remainder;
		}

		return a;
	}

	/// <summary>Greatest Common Divisor for int</summary>
	/// <remarks>More like the ancient greek Euclid originally devised. It uses a while loop with subtraction.</remarks>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static Int32 GCD2( Int32 x, Int32 y ) {
		while ( x != y ) {
			if ( x > y ) {
				x -= y;
			}
			else {
				y -= x;
			}
		}

		return x;
	}

	/// <summary>Greatest Common Divisor for long</summary>
	/// <remarks>More like the ancient greek Euclid originally devised. It uses a while loop with subtraction.</remarks>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static Int64 GCD2( Int64 x, Int64 y ) {
		while ( x != y ) {
			if ( x > y ) {
				x -= y;
			}
			else {
				y -= x;
			}
		}

		return x;
	}

	/// <summary>Get the value of a bit</summary>
	/// <param name="b">The byte value</param>
	/// <param name="position">The position of the bit</param>
	/// <returns>The value of the bit</returns>
	public static Boolean GetBit( this Byte b, Byte position ) => ( b & ( Byte )( 1 << position ) ) != 0;

	public static UInt16[] GetBitFields( UInt32 packedBits, Byte[] bitFields ) {
		const Int32 maxBits = 32;
		var fields = bitFields.Length - 1; // number of fields to unpack
		var retArr = new UInt16[ fields + 1 ]; // init return array
		var curPos = 0; // current field bit position (start)

		for ( var f = fields; f >= 0; f-- ) // loop from last
		{
			var lastEnd = curPos; // position where last field ended
			curPos += bitFields[ f ]; // we get where the current value starts

			var leftShift = maxBits - curPos; // we figure how much left shift we gotta apply for the other numbers to overflow into oblivion

			retArr[ f ] = ( UInt16 )( ( packedBits << leftShift ) >> ( leftShift + lastEnd ) ); // we do magic
		}

		return retArr;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Pure]
	public static Single Half( this Single number ) => number / 2.0f;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Pure]
	public static Double Half( this Double number ) => number / 2.0d;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Pure]
	public static Byte Half( this Byte number ) => ( Byte )( number / 2 );

	[DebuggerStepThrough]
	[Pure]
	public static TimeSpan Half( this TimeSpan timeSpan ) => TimeSpan.FromTicks( timeSpan.Ticks.Half() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Pure]
	public static Int32 Half( this Int32 number ) => number / 2;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Pure]
	public static Int16 Half( this Int16 number ) => ( Int16 )( number / 2 );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Pure]
	public static UInt16 Half( this UInt16 number ) => ( UInt16 )( number / 2 );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Pure]
	public static UInt32 Half( this UInt32 number ) => number / 2;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Pure]
	public static UInt64 Half( this UInt64 number ) => number / 2;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Pure]
	public static Int64 Half( this Int64 number ) => number / 2;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Pure]
	public static Decimal Half( this Decimal number ) => number / 2.0m;

	/// <summary>
	/// <para>
	/// If the <paramref name="number" /> is less than <see cref="Decimal.Zero" />, then return <see cref="Decimal.Zero" />.
	/// </para>
	/// <para>Otherwise return the <paramref name="number" />.</para>
	/// </summary>
	/// <param name="number"></param>
	[DebuggerStepThrough]
	[Pure]
	public static Decimal IfLessThanZeroThenZero( this Decimal number ) => number < Decimal.Zero ? Decimal.Zero : number;

	/// <summary>
	/// <para>
	/// If the <paramref name="number" /> is less than <see cref="BigInteger.Zero" />, then return <see cref="Decimal.Zero" />.
	/// </para>
	/// <para>Otherwise return the <paramref name="number" />.</para>
	/// </summary>
	/// <param name="number"></param>
	[DebuggerStepThrough]
	[Pure]
	public static BigInteger IfLessThanZeroThenZero( this BigInteger number ) => number < BigInteger.Zero ? BigInteger.Zero : number;

	/*

	/// <summary>
	/// <para>
	/// If the <paramref name="number" /> is less than <see cref="BigDecimal.Zero" />, then return <see cref="Decimal.Zero" />.
	/// </para>
	/// <para>Otherwise return the <paramref name="number" />.</para>
	/// </summary>
	/// <param name="number"></param>
	[DebuggerStepThrough]
	[Pure]
	public static BigDecimal IfLessThanZeroThenZero( this BigDecimal number ) => number <= BigDecimal.Zero ? BigDecimal.Zero : number;
	*/

	/// <summary>
	/// <para>
	/// If the <paramref name="number" /> is less than <see cref="BigDecimal.Zero" />, then return <see cref="Decimal.Zero" />.
	/// </para>
	/// <para>Otherwise return the <paramref name="number" />.</para>
	/// </summary>
	/// <param name="number"></param>
	[DebuggerStepThrough]
	[Pure]
	public static BigDecimal IfLessThanZeroThenZero( this BigDecimal? number ) =>
		number is null ? BigDecimal.Zero :
		number <= BigDecimal.Zero ? BigDecimal.Zero : number;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean IsEven( this Int32 value ) => value % 2 == 0;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean IsEven( this Decimal value ) => value % 2 == 0;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean IsEven( this Int64 value ) => value % 2 == 0;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean IsNegative( this Single value ) => value < 0.0f;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean IsNumber( this Single value ) => !Single.IsNaN( value ) && !Single.IsInfinity( value );

	[DebuggerStepThrough]
	[Pure]
	public static Boolean IsNumber( this Double value ) {
		if ( Double.IsNaN( value ) ) {
			return false;
		}

		return !Double.IsInfinity( value );
	}

	[DebuggerStepThrough]
	[Pure]
	public static Boolean IsOdd( this Int32 value ) => value % 2 != 0;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean IsOdd( this Int64 value ) => value % 2 != 0;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean IsOdd( this Decimal value ) => value % 2 != 0;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean IsPositive( this Single value ) => value > 0.0f;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean IsPowerOfTwo( this Int32 number ) => ( number & -number ) == number;

	[DebuggerStepThrough]
	public static void LeftLeftLeftRightLeft( ref this UInt64 ul, Int32 l1, Int32 l2, Int32 l3, Int32 r1, Int32 l4 ) {
		ul.Rol( l1 );
		ul.Rol( l2 );
		ul.Rol( l3 );
		ul.Ror( r1 );
		ul.Rol( l4 );
	}

	/// <summary>Linearly interpolates between two values.</summary>
	/// <param name="source">Source value.</param>
	/// <param name="target">Target value.</param>
	/// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
	[DebuggerStepThrough]
	[Pure]
	public static Single Lerp( this Single source, Single target, Single amount ) => source + ( target - source ) * amount;

	/// <summary>Linearly interpolates between two values.</summary>
	/// <param name="source">Source value.</param>
	/// <param name="target">Target value.</param>
	/// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
	[DebuggerStepThrough]
	[Pure]
	public static Double Lerp( this Double source, Double target, Single amount ) => source + ( target - source ) * amount;

	/// <summary>Linearly interpolates between two values.</summary>
	/// <param name="source">Source value.</param>
	/// <param name="target">Target value.</param>
	/// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
	[DebuggerStepThrough]
	[Pure]
	public static Decimal Lerp( this Decimal source, Decimal target, Decimal amount ) => source + ( target - source ) * amount;

	/// <summary>Linearly interpolates between two values.</summary>
	/// <param name="source">Source value.</param>
	/// <param name="target">Target value.</param>
	/// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
	[DebuggerStepThrough]
	[Pure]
	public static UInt64 Lerp( this UInt64 source, UInt64 target, Single amount ) => ( UInt64 )( source + ( target - source ) * amount );

	/// <summary>Linearly interpolates between two values.</summary>
	/// <param name="source">Source value.</param>
	/// <param name="target">Target value.</param>
	/// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
	[DebuggerStepThrough]
	[Pure]
	public static UInt32 Lerp( this UInt32 source, UInt32 target, Single amount ) => ( UInt32 )( source + ( target - source ) * amount );

	[DebuggerStepThrough]
	[Pure]
	public static Double LogFactorial( this Int32 n ) {
		switch ( n ) {
			case < 0: {
				throw new ArgumentOutOfRangeException( nameof( n ) );
			}
			case <= 254: {
				return MathConstants.Logfactorialtable[ n ];
			}
			default: {
				var x = n + 1d;

				return ( x - 0.5 ) * Math.Log( x ) - x + 0.5 * Math.Log( 2 * Math.PI ) + 1.0 / ( 12.0 * x );
			}
		}
	}

	/// <summary>compute log(1+x) without losing precision for small values of x</summary>
	/// <param name="x"></param>
	[DebuggerStepThrough]
	[Pure]
	public static Double LogOnePlusX( this Double x ) {
		if ( x <= -1.0 ) {
			throw new ArgumentOutOfRangeException( nameof( x ), $"Invalid input argument: {x}" );
		}

		if ( Math.Abs( x ) > 1e-4 ) {

			// x is large enough that the obvious evaluation is OK
			return Math.Log( 1.0 + x );
		}

		// Use Taylor approx. log(1 + x) = x - x^2/2 with error roughly x^3/3 since |x| < 10^-4, |x|^3 < 10^-12, relative error
		// less than 10^-8
		return ( -0.5 * x + 1.0 ) * x;
	}

	[DebuggerStepThrough]
	[Pure]
	public static Boolean Near( this Double number, Double target ) => Math.Abs( number - target ) <= Double.Epsilon;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean Near( this Int32 number, Int32 target ) => Math.Abs( number - target ) <= Double.Epsilon;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean Near( this Single number, Single target ) => Math.Abs( number - target ) <= Single.Epsilon;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean Near( this Point here, Point there ) => here.X.Near( there.X ) && here.Y.Near( there.Y );

	[DebuggerStepThrough]
	[Pure]
	public static Boolean Near( this Decimal number, Decimal target ) => Math.Abs( number - target ) <= EpsilonDecimal;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean Near( this BigDecimal number, BigDecimal target ) {
		var difference = number - target;

		if ( difference < BigDecimal.Zero ) {
			difference = -difference;
		}

		return difference <= EpsilonDecimal;
	}

	public static Boolean Near( this PointF here, PointF there ) => here.X.Near( there.X ) && here.Y.Near( there.Y );

	[DebuggerStepThrough]
	[Pure]
	public static Boolean Near( this BigInteger number, BigInteger target ) {
		var difference = number - target;

		return BigInteger.Zero == difference;
	}

	[DebuggerStepThrough]
	[Pure]
	public static Boolean Near( this UInt64 number, UInt64 target ) => number - target <= UInt64.MinValue;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean Near( this Int64 number, Int64 target ) => number - target <= Int64.MinValue;

	[DebuggerStepThrough]
	[Pure]
	public static Boolean Near( this UBigInteger number, UBigInteger target ) => number - target <= UBigInteger.Epsilon;

	[DebuggerStepThrough]
	[Pure]
	public static Double Nested( this Double x ) => Math.Sqrt( x * 100.0 ) / 100.0d;

	[DebuggerStepThrough]
	[Pure]
	public static Single Nested( this Single x ) => ( Single )( Math.Sqrt( x * 100.0 ) / 100.0f );

	/// <summary>Remove all the trailing zeros from the decimal</summary>
	/// <param name="value"></param>
	[DebuggerStepThrough]
	[Pure]
	public static Decimal Normalize( this Decimal value ) => value / 1.000000000000000000000000000000000m;

	/// <summary></summary>
	/// <param name="baseValue"></param>
	/// <param name="n"></param>
	/// <see cref="http://stackoverflow.com/a/18363540/956364" />
	[DebuggerStepThrough]
	[Pure]
	public static Decimal NthRoot( this Decimal baseValue, Int32 n ) {
		if ( n == 1 ) {
			return baseValue;
		}

		Decimal deltaX;
		var x = 0.1M;

		do {
			deltaX = ( baseValue / x.Pow( n - 1 ) - x ) / n;
			x += deltaX;
		} while ( Math.Abs( deltaX ) > 0 );

		return x;
	}

	[DebuggerStepThrough]
	[Pure]
	public static UInt64 OneHundreth( this UInt64 x ) => x / 100;

	[DebuggerStepThrough]
	[Pure]
	public static UInt64 OneQuarter( this UInt64 x ) => x / 4;

	[DebuggerStepThrough]
	[Pure]
	public static Single OneQuarter( this Single x ) => x / 4.0f;

	[DebuggerStepThrough]
	[Pure]
	public static UInt64 OneTenth( this UInt64 x ) => x / 10;

	[DebuggerStepThrough]
	[Pure]
	public static Single OneThird( this Single x ) => x / 3.0f;

	public static UInt32 PackBitFields( UInt16[] values, Byte[] bitFields ) {
		if ( bitFields is null ) {
			throw new NullException( nameof( bitFields ) );
		}

		UInt32 retVal = values[ 0 ]; //we set the first value right away

		for ( var f = 1; f < values.Length; f++ ) {
			retVal <<= bitFields[ f ]; //we shift the previous value
			retVal += values[ f ]; //and add our current value //on some processors | (pipe) will be faster here
		}

		return retVal;
	}

	[DebuggerStepThrough]
	[Pure]
	public static Double Phi( this Double x ) {

		// constants
		const Double a1 = 0.254829592;
		const Double a2 = -0.284496736;
		const Double a3 = 1.421413741;
		const Double a4 = -1.453152027;
		const Double a5 = 1.061405429;
		const Double p = 0.3275911;

		// Save the sign of x
		var sign = x < 0 ? -1 : 1;
		x = Math.Abs( x ) / Math.Sqrt( 2.0 );

		// A&S formula 7.1.26
		var t = 1.0 / ( 1.0 + p * x );
		var y = 1.0 - ( ( ( ( a5 * t + a4 ) * t + a3 ) * t + a2 ) * t + a1 ) * t * Math.Exp( -x * x );

		return 0.5 * ( 1.0 + sign * y );
	}

	[Pure]
	public static UInt64 Positive( this Int16 value ) => value < 0 ? 0 : Convert.ToUInt64( value );

	[Pure]
	public static UInt64 Positive( this Int32 value ) => value < 0 ? 0 : Convert.ToUInt64( value );

	[Pure]
	public static UInt64 Positive( this Int64 value ) => value < 0 ? 0 : Convert.ToUInt64( value );

	[DebuggerStepThrough]
	[Pure]
	public static Decimal Pow( this Decimal baseValue, Int32 n ) {
		for ( var i = 0; i < n - 1; i++ ) {
			baseValue *= baseValue;
		}

		return baseValue;
	}

	/// <summary><see cref="Decimal" /> raised to the nth power.</summary>
	/// <param name="x"></param>
	/// <param name="n"></param>
	/// <see cref="http://stackoverflow.com/questions/429165/raising-a-Decimal-to-a-power-of-Decimal" />
	[DebuggerStepThrough]
	[Pure]
	public static Decimal Pow( this Decimal x, UInt32 n ) {
		var a = 1m;
		var e = new BitArray( BitConverter.GetBytes( n ) );

		for ( var i = e.Count - 1; i >= 0; --i ) {
			a *= a;

			if ( e[ i ] ) {
				a *= x;
			}
		}

		return a;
	}

	public static IEnumerable<Int32> Primes( this Int32 max ) {
		yield return 2;

		var found = new List<Int32> {
			3
		};

		var candidate = 3;

		while ( candidate <= max ) {
			var candidate1 = candidate;
			var candidate2 = candidate;

			if ( found.TakeWhile( prime => prime * prime <= candidate1 ).All( prime => candidate2 % prime != 0 ) ) {
				found.Add( candidate );

				yield return candidate;
			}

			candidate += 2;
		}
	}

	[DebuggerStepThrough]
	[Pure]
	public static Decimal Quarter( this Decimal number ) => number / 4.0m;

	[DebuggerStepThrough]
	public static void Rol( ref this UInt64 ul ) => ul = ( ul << 1 ) | ( ul >> 63 );

	[DebuggerStepThrough]
	public static void Rol( ref this UInt64 ul, Int32 n ) => ul = ( ul << n ) | ( ul >> ( 64 - n ) );

	[DebuggerStepThrough]
	public static Double Root( this Double x, Double root ) => Math.Pow( x, 1.0 / root );

	[DebuggerStepThrough]
	public static Double Root( this Decimal x, Decimal root ) => Math.Pow( ( Double )x, ( Double )( 1.0m / root ) );

	[DebuggerStepThrough]
	public static void Ror( ref this UInt64 ul ) => ul = ( ul << 63 ) | ( ul >> 1 );

	[DebuggerStepThrough]
	public static void Ror( ref this UInt64 ul, Int32 n ) => ul = ( ul << ( 64 - n ) ) | ( ul >> n );

	[DebuggerStepThrough]
	public static void RotateLeft( ref this UInt64 original, Int32 bits ) => original = ( original << bits ) | ( original >> ( 64 - bits ) );

	[DebuggerStepThrough]
	public static void RotateRight( ref this UInt64 original, Int32 bits ) => original = ( original >> bits ) | ( original << ( 64 - bits ) );

	/// <summary>Truncate, don't round. Just chop it off after <paramref name="decimalPlaces" />.</summary>
	/// <param name="number"></param>
	/// <param name="decimalPlaces"></param>
	public static Decimal Sanitize( this Decimal number, UInt16 decimalPlaces = 8 ) {
		number *= ( Decimal )Math.Pow( 10, decimalPlaces );

		number = Math.Truncate( number ); //Don't round. Just Truncate.

		number *= ( Decimal )Math.Pow( 10, -decimalPlaces );

		return number;
	}

	/// <summary>Set a bit to [newBitValue]</summary>
	/// <param name="b">The byte value</param>
	/// <param name="position">The position (1-8) of the bit</param>
	/// <param name="newBitValue">The new value of the bit in position [position]</param>
	/// <returns>The new byte value</returns>
	public static Byte SetBit( Byte b, Byte position, Boolean newBitValue ) {
		var mask = ( Byte )( 1 << position );

		if ( newBitValue ) {
			return ( Byte )( b | mask );
		}

		return ( Byte )( b & ~mask );
	}

	/// <summary>Smooths a value to between 0 and 1.</summary>
	/// <param name="x"></param>
	[DebuggerStepThrough]
	[Pure]
	public static Double Sigmoid0To1( this Double x ) => 1.0D / ( 1.0D + Math.Exp( -x ) );

	/// <summary>Smooths a value to between 0 and 1.</summary>
	/// <param name="x"></param>
	[DebuggerStepThrough]
	[Pure]
	public static Decimal Sigmoid0To1( this Decimal x ) => 1.0M / ( 1.0M + ( Decimal )Math.Exp( ( Double )( -x ) ) );

	/// <summary>Smooths a value to between -1 and 1.</summary>
	/// <param name="x"></param>
	/// <see cref="http://www.wolframalpha.com/input/?i=1+-+%28+2+%2F+%281+%2B+Exp%28+v+%29+%29+%29%2C+v+from+-10+to+10" />
	[DebuggerStepThrough]
	[Pure]
	public static Double SigmoidNeg1To1( this Double x ) => 1.0D - 2.0D / ( 1.0D + Math.Exp( x ) );

	/// <summary>Return the integer part and the fraction parts of a <see cref="Decimal" />.</summary>
	/// <param name="value"></param>
	public static Tuple<Decimal, Decimal> Split( this Decimal value ) {
		var parts = value.ToString( "R" ).Split( '.' );
		var result = new Tuple<Decimal, Decimal>( Decimal.Parse( parts[ 0 ] ), Decimal.Parse( "0." + parts[ 1 ] ) );

		return result;
	}

	/// <summary>Return the integer part and the fraction parts of a <see cref="Double" />.</summary>
	/// <param name="value"></param>
	public static Tuple<Double, Double> Split( this Double value ) {
		var parts = value.ToString( "R" ).Split( '.' );

		return new Tuple<Double, Double>( Double.Parse( parts[ 0 ] ), Double.Parse( "0." + parts[ 1 ] ) );
	}

	/// <summary>
	/// Split one <see cref="UInt64" /> value into two <see cref="UInt32" /> values. Use <see cref="Combine" /> for the reverse.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="high"></param>
	/// <param name="low"></param>
	public static void Split( this UInt64 value, out UInt32 high, out UInt32 low ) {
		high = ( UInt32 )( value >> 32 );
		low = ( UInt32 )( value & UInt32.MaxValue );
	}

	[DebuggerStepThrough]
	[Pure]
	public static Single Squared( this Single number ) => number * number;

	[DebuggerStepThrough]
	[Pure]
	public static Double Squared( this Double number ) => number * number;

	[DebuggerStepThrough]
	[Pure]
	public static Decimal Squared( this Decimal number ) => number * number;

	public static Double SquareRootOfProducts( this IEnumerable<Double> data ) {
		var sorted = new List<Double>( data.Where( d => Math.Abs( d ) >= Double.Epsilon ).OrderBy( d => d ) );

		var aggregate = BigDecimal.One;

		while ( sorted.Any() ) {
			sorted.TakeFirst( out var smallest );

			if ( !sorted.TakeLast( out var largest ) ) {
				largest = 1;
			}

			aggregate *= smallest;
			aggregate *= largest;

			//aggregate.Should().NotBe( Double.NaN );
			//aggregate.Should().NotBe( Double.NegativeInfinity );
			//aggregate.Should().NotBe( Double.PositiveInfinity );
		}

		//foreach ( Double d in data ) {aggregate = aggregate * d;}
		return Math.Sqrt( ( Double )aggregate );
	}

	public static Decimal SquareRootOfProducts( this IEnumerable<Decimal> data ) {
		var aggregate = data.Aggregate( 1.0m, ( current, d ) => current * d );

		return ( Decimal )Math.Sqrt( ( Double )aggregate );
	}

	/// <summary>
	/// Subtract <paramref name="tax" /> of <paramref name="total" /> from <paramref name="total" />.
	/// <para>If the tax was 6% on $53, then you would call this function like this:</para>
	/// <para>var withTax = SubtractTax( 53.00, 0.06 );</para>
	/// <para>Assert( withTax == 50.00 );</para>
	/// </summary>
	/// <param name="total"></param>
	/// <param name="tax"></param>
	public static Decimal SubtractTax( this Decimal total, Decimal tax ) {
		var taxed = total / ( 1.0m + tax );

		return taxed;
	}

	/// <summary>
	/// Subtract <paramref name="right" /> away from <paramref name="left" /> without the chance of "throw new
	/// ArgumentOutOfRangeException( "amount", String.Format( "Values {0} and {1} are loo small to handle.", amount,
	/// uBigInteger ) );"
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static UInt64 SubtractWithoutUnderFlow( this UInt64 left, UInt64 right ) {
		var integer = new UBigInteger( left ) - new UBigInteger( right );

		if ( integer < new UBigInteger( UInt64.MinValue ) ) {
			return UInt64.MinValue;
		}

		return ( UInt64 )integer;
	}

	/// <summary>
	/// <para>Returns the sum of all <see cref="BigInteger" />.</para>
	/// </summary>
	/// <param name="bigIntegers"></param>
	public static BigInteger Sum( this IEnumerable<BigInteger> bigIntegers ) => bigIntegers.Aggregate( BigInteger.Zero, ( current, bigInteger ) => current + bigInteger );

	[DebuggerStepThrough]
	[Pure]
	public static Double TendTo( this Double number, Double goal ) => ( number + goal ).Half();

	[DebuggerStepThrough]
	[Pure]
	public static TimeSpan TendTo( this TimeSpan time, TimeSpan goal ) => ( time + goal ).Half();

	[DebuggerStepThrough]
	[Pure]
	public static Int32 ThreeFourths( this Int32 x ) => ( Int32 )( 3.0 * x / 4.0 );

	[DebuggerStepThrough]
	[Pure]
	public static UInt64 ThreeQuarters( this UInt64 x ) => 3 * x / 4;

	[DebuggerStepThrough]
	[Pure]
	public static Single ThreeQuarters( this Single x ) => 3.0f * x / 4.0f;

	[DebuggerStepThrough]
	[Pure]
	public static Double ThreeQuarters( this Double x ) => 3.0d * x / 4.0d;

	[DebuggerStepThrough]
	[Pure]
	public static TimeSpan Thrice( this TimeSpan timeSpan ) => TimeSpan.FromTicks( timeSpan.Ticks.Thrice() );

	[DebuggerStepThrough]
	[Pure]
	public static Int64 Thrice( this Int64 number ) => number * 3L;

	/// <summary>
	/// <see
	/// cref="http://stackoverflow.com/questions/17575375/how-do-i-convert-an-int-to-a-String-in-c-sharp-without-using-tostring" />
	/// </summary>
	/// <param name="number"></param>
	/// <param name="base"></param>
	/// <param name="minDigits"></param>
	public static String ToStringWithBase( this Int32 number, Int32 @base, Int32 minDigits = 1 ) {
		if ( minDigits < 1 ) {
			minDigits = 1;
		}

		if ( number == 0 ) {
			return new String( '0', minDigits );
		}

		var s = "";

		if ( @base < 2 || @base > MathConstants.NumberBaseChars.Length ) {
			return s;
		}

		var neg = false;

		if ( @base == 10 && number < 0 ) {
			neg = true;
			number = -number;
		}

		var n = ( UInt32 )number;
		var b = ( UInt32 )@base;

		while ( ( n > 0 ) | ( minDigits-- > 0 ) ) {
			s = MathConstants.NumberBaseChars[ ( Int32 )( n % b ) ] + s;
			n /= b;
		}

		if ( neg ) {
			s = "-" + s;
		}

		return s;
	}

	public static UInt64? ToUInt64( this String? text ) => UInt64.TryParse( text, out var result ) ? result : null;

	public static UInt64 ToUInt64( this Byte[] bytes, Int32 pos ) =>
		( UInt64 )( bytes[ pos++ ] | ( bytes[ pos++ ] << 8 ) | ( bytes[ pos++ ] << 16 ) | ( bytes[ pos ] << 24 ) );

	public static Int64 Truncate( this Single number ) => ( Int64 )number;

	public static Int64 Truncate( this Double number ) => ( Int64 )number;

	/*

	/// <summary>
	/// <para>Attempt to parse a fraction from a String.</para>
	/// </summary>
	/// <example>" 1234 / 346 "</example>
	/// <param name="numberString"></param>
	/// <param name="result"></param>
	/// <returns></returns>
	public static Boolean TryParse( [CanBeNull] this String? numberString, out BigDecimal result ) {
		result = BigDecimal.Zero;

		if ( null == numberString ) {
			return false;
		}

		numberString = numberString.Trim();

		if ( numberString.IsNullOrEmpty() ) {
			return false;
		}

		var parts = numberString.Split( '/' ).ToList();

		if ( parts.Count != 2 ) {
			return false;
		}

		var top = parts.TakeFirst();

		if ( String.IsNullOrWhiteSpace( top ) ) {
			return false;
		}

		top = top.Trim();

		var bottom = parts.TakeLast();

		if ( String.IsNullOrWhiteSpace( bottom ) ) {
			return false;
		}

		if ( parts.Count > 0 ) {
			return false;
		}

		if ( BigInteger.TryParse( top, out var numerator ) ) {
			if ( BigInteger.TryParse( bottom, out var denominator ) ) {
				result = new BigDecimal( numerator, denominator );

				return true;
			}

			throw new OutOfRangeException( "Couldn't parse denominator" );
		}

		throw new OutOfRangeException( "Couldn't parse numerator" );
	}
	*/

	public static Boolean TrySplitDecimal( this BigDecimal value, out BigInteger beforeDecimalPoint, out BigInteger afterDecimalPoint ) {
		var theString = value.ToString();

		if ( !theString.Contains( ".", StringComparison.CurrentCulture ) ) {
			theString += ".0";
		}

		var split = theString.Split( '.' );

		afterDecimalPoint = BigInteger.Zero;

		return BigInteger.TryParse( split[ 0 ], out beforeDecimalPoint ) && BigInteger.TryParse( split[ 1 ], out afterDecimalPoint );
	}

	public static Int32 TurnBitsOff( this Int32 value, Byte bitToTurnOff ) => value & ~bitToTurnOff;

	public static Int64 TurnBitsOff( this Int64 value, Byte bitToTurnOff ) => value & ~bitToTurnOff;

	public static UInt64 TurnBitsOff( this UInt64 value, Byte bitToTurnOff ) => value & ( UInt64 )~bitToTurnOff;

	public static Byte TurnBitsOff( this Byte value, Byte bitToTurnOff ) => ( Byte )( value & ~bitToTurnOff );

	public static Int32 TurnBitsOn( this Int32 value, Byte bitToTurnOn ) => value | bitToTurnOn;

	public static Int64 TurnBitsOn( this Int64 value, Byte bitToTurnOn ) => value | bitToTurnOn;

	public static UInt64 TurnBitsOn( this UInt64 value, Byte bitToTurnOn ) => value | bitToTurnOn;

	public static Byte TurnBitsOn( this Byte value, Byte bitToTurnOn ) => ( Byte )( value | bitToTurnOn );

	[Pure]
	public static TimeSpan Twice( this TimeSpan timeSpan ) => TimeSpan.FromTicks( timeSpan.Ticks.Twice() );

	[Pure]
	public static Single Twice( this Single x ) => x * 2.0f;

	[Pure]
	public static Double Twice( this Double number ) => number * 2d;

	[Pure]
	public static Decimal Twice( this Decimal number ) => number * 2m;

	[Pure]
	public static Int64 Twice( this Int64 number ) => number * 2L;
}