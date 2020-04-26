// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "MathExtensions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "MathExtensions.cs" was last formatted by Protiguous on 2020/03/18 at 10:24 AM.

namespace Librainian.Maths {

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
    using JetBrains.Annotations;
    using Measurement.Time;
    using Numbers;
    using Parsing;
    using Rationals;

    public static class MathExtensions {

        public delegate Int32 FibonacciCalculator( Int32 n );

        // you may want to pass this and use generics to allow more or less bits
        /// <summary>Store the complete list of values that will fit in a 32-bit unsigned integer without overflow.</summary>
        [NotNull]
        private static UInt32[] FibonacciLookup { get; } = {
            1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711, 28657, 46368, 75025, 121393, 196418, 317811, 514229, 832040,
            1346269, 2178309, 3524578, 5702887, 9227465, 14930352, 24157817, 39088169, 63245986, 102334155, 165580141, 267914296, 433494437, 701408733, 1134903170, 1836311903,
            2971215073
        };

        /// <summary>
        ///     <para>Return the smallest possible value above <see cref="Decimal.Zero" /> for a <see cref="Decimal" />.</para>
        /// </summary>
        public const Decimal EpsilonDecimal = 0.0000000000000000000000000001m;

        /// <summary>Add two <see cref="UInt64" />.</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="overflowed">True if the two values added to more than <see cref="UInt64.MaxValue" /></param>
        /// <returns></returns>
        public static UBigInteger Add( this UInt64 left, UInt64 right, out Boolean overflowed ) {
            var result = new UBigInteger( left ) + new UBigInteger( right );
            overflowed = result > UInt64.MaxValue;

            return result;
        }

        /// <summary>Allow <paramref name="left" /> to increase or decrease by a signed number;</summary>
        /// <param name="left">      </param>
        /// <param name="right">     </param>
        /// <param name="overflowed">True if the two values added to more than <see cref="UInt64.MaxValue" /></param>
        /// <returns></returns>
        public static BigInteger Add( this UInt64 left, Int64 right, out Boolean overflowed ) {
            var result = new BigInteger( left ) + new BigInteger( right );
            overflowed = result > UInt64.MaxValue;

            return result;
        }

        /// <summary>
        ///     <para>Add <paramref name="tax" /> of <paramref name="number" /> to <paramref name="number" />.</para>
        ///     <para>If the tax is 6% on $50, then you would call this function like this:</para>
        ///     <code>var withTax = 50.AddTax( 0.06 );</code> <code>Assert( withTax == 53.00 );</code>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="tax">   </param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Decimal AddTax( this Decimal number, Decimal tax ) => number * ( 1.0m + tax );

        /// <summary>
        ///     <para>Add <paramref name="percentTax" /> of <paramref name="number" /> to <paramref name="number" />.</para>
        ///     <para>If the tax is 6% on $50, then you would call this function like this:</para>
        ///     <code>var withTax = AddTaxPercent( 50.00, 6.0 );</code> <code>Assert( withTax == 53.00 );</code>
        /// </summary>
        /// <param name="number">    </param>
        /// <param name="percentTax"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Decimal AddTaxPercent( this Decimal number, Decimal percentTax ) => number * ( 1.0m + ( percentTax / 100.0m ) );

        /// <summary>Combine two <see cref="UInt32" /> values into one <see cref="UInt64" /> value. Use Split() for the reverse.</summary>
        /// <param name="high"></param>
        /// <param name="low"> </param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static UInt64 Combine( this UInt32 high, UInt32 low ) => ( ( UInt64 ) high << 32 ) | low;

        /// <summary>Combine two bytes into one <see cref="UInt16" />.</summary>
        /// <param name="low"> </param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static UInt16 CombineBytes( this Byte low, Byte high ) =>
            BitConverter.ToUInt16( BitConverter.IsLittleEndian ?
                new[] {
                    high, low
                } :
                new[] {
                    low, high
                }, 0 );

        /// <summary>Combine two bytes into one <see cref="UInt16" /> with little endianess.</summary>
        /// <param name="low"> </param>
        /// <param name="high"></param>
        /// <returns></returns>
        /// <see cref="CombineTwoBytesLittleEndianess" />
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static UInt16 CombineTwoBytesHighEndianess( this Byte low, Byte high ) => ( UInt16 ) ( high + ( low << 8 ) );

        /// <summary>Combine two bytes into one <see cref="UInt16" /> with little endianess.</summary>
        /// <param name="low"> </param>
        /// <param name="high"></param>
        /// <returns></returns>
        /// <see cref="CombineTwoBytesHighEndianess" />
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static UInt16 CombineTwoBytesLittleEndianess( this Byte low, Byte high ) => ( UInt16 ) ( low + ( high << 8 ) );

        /// <summary>Combine two byte arrays into one byte array.
        /// <para>Warning: this allocates a new array.</para>
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [NotNull]
        public static Byte[] Concat( [NotNull] this Byte[] first, [NotNull] Byte[] second ) {
            var buffer = new Byte[ first.Length + second.Length ];
            Buffer.BlockCopy( first, 0, buffer, 0, first.Length );
            Buffer.BlockCopy( second, 0, buffer, first.Length, second.Length );

            return buffer;
        }

        /// <summary>Add a byte onto the end of the <paramref name="first" /> array.
        /// <para>Warning: this allocates a new array.</para>
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [NotNull]
        public static Byte[] Concat( [NotNull] this Byte[] first, Byte second ) {
            var buffer = new Byte[ first.Length + 1 ];
            Buffer.BlockCopy( first, 0, buffer, 0, first.Length );
            buffer[ ^1 ] = second;

            return buffer;
        }

        /// <summary>ConvertBigIntToBcd</summary>
        /// <param name="numberToConvert"></param>
        /// <param name="howManyBytes">   </param>
        /// <returns></returns>
        /// <see cref="http://github.com/mkadlec/ConvertBigIntToBcd/blob/master/ConvertBigIntToBcd.cs" />
        [NotNull]
        public static Byte[] ConvertBigIntToBcd( this Int64 numberToConvert, Int32 howManyBytes ) {
            var convertedNumber = new Byte[ howManyBytes ];
            var strNumber = numberToConvert.ToString();
            var currentNumber = String.Empty;

            for ( var i = 0; i < howManyBytes; i++ ) {
                convertedNumber[ i ] = 0xff;
            }

            for ( var i = 0; i < strNumber.Length; i++ ) {
                currentNumber += strNumber[ i ].ToString();

                if ( ( i == ( strNumber.Length - 1 ) ) && ( ( i % 2 ) == 0 ) ) {
                    convertedNumber[ i / 2 ] = 0xf;
                    convertedNumber[ i / 2 ] |= ( Byte ) ( ( Int32.Parse( currentNumber ) % 10 ) << 4 );
                }

                if ( ( i % 2 ) == 0 ) {
                    continue;
                }

                var value = Int32.Parse( currentNumber );
                convertedNumber[ ( i - 1 ) / 2 ] = ( Byte ) ( value % 10 );
                convertedNumber[ ( i - 1 ) / 2 ] |= ( Byte ) ( ( value / 10 ) << 4 );
                currentNumber = String.Empty;
            }

            return convertedNumber;
        }

        /// <summary>Remove everything after the decimal point.</summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Double Crop( this Double x ) => Math.Truncate( x * 100.0D ) / 100.0D;

        /// <summary>Remove everything after the decimal point.</summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Single Crop( this Single x ) => ( Single ) ( Math.Truncate( x * 100.0f ) / 100.0f );

        /// <summary>Return the cube (^3) of the number.</summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Single Cubed( this Single number ) => number * number * number;

        /// <summary>Return the cube (^3) of the number.</summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Double Cubed( this Double number ) => number * number * number;

        /// <summary>Return the cube (^3) of the number.</summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Decimal Cubed( this Decimal number ) => number * number * number;

        /// <summary>Needs tested.</summary>
        /// <param name="d"></param>
        /// <returns></returns>
        [NotNull]
        public static String Decimal2Packed( this Decimal d ) {
            var output = new Boolean[ 10 ];
            var input = new Boolean[ 12 ];

            for ( var i = 0; i < 3; i++ ) {
                var a = ( Int32 ) ( ( Int32 ) d / Math.Pow( 10, i ) ) % 10;

                for ( var j = 0; j < 4; j++ ) {
                    input[ j + ( i * 4 ) ] = ( a & ( 1 << j ) ) != 0;
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

        /// <summary>
        ///     <para>Return the smallest possible value above <see cref="Decimal.Zero" /> for a <see cref="Decimal" />.</para>
        ///     <para>1E-28</para>
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]

        // ReSharper disable once UnusedParameter.Global
        public static Decimal Epsilon( this Decimal _ ) => EpsilonDecimal;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
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
            var t = 1.0 / ( 1.0 + ( p * x ) );
            var y = 1.0 - ( ( ( ( ( ( ( ( ( a5 * t ) + a4 ) * t ) + a3 ) * t ) + a2 ) * t ) + a1 ) * t * Math.Exp( -x * x ) );

            return sign * y;
        }

        /// <summary>Compute fibonacci series up to Max (&gt; 1). Example: foreach (int i in Fib(10)) { Console.WriteLine(i); }</summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static IEnumerable<Int32> Fib( Int32 max ) {
            var a = 0;
            var b = 1;

            yield return 1;

            for ( var i = 0; i < ( max - 1 ); i++ ) {
                var c = a + b;

                yield return c;

                a = b;
                b = c;
            }
        }

        public static UInt32 FibonacciSequence( this UInt32 n ) => FibonacciLookup[ n ];

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Double FiftyPercentOf( this Double x ) {
            var result = x * 0.5;

            return result < 1.0 ? 1 : result;
        }

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Int32 FiftyPercentOf( this Int32 x ) {
            var result = x * 0.5;

            return result < 1.0 ? 1 : ( Int32 ) result;
        }

        public static Int32 FlipBit( this Int32 value, Byte bitToFlip ) => value ^ bitToFlip;

        public static Int64 FlipBit( this Int64 value, Byte bitToFlip ) => value ^ bitToFlip;

        public static UInt64 FlipBit( this UInt64 value, Byte bitToFlip ) => value ^ bitToFlip;

        public static Byte FlipBit( this Byte value, Byte bitToFlip ) => ( Byte ) ( value ^ bitToFlip );

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Double ForceBounds( this Double thisDouble, Double minLimit, Double maxLimit ) => Math.Max( Math.Min( thisDouble, maxLimit ), minLimit );

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Int32 FractionOf( this Int32 x, Double top, Double bottom ) {
            var result = ( top * x ) / bottom;

            return result < 1.0 ? 1 : ( Int32 ) result;
        }

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Double FractionOf( this Double x, Double top, Double bottom ) => ( top * x ) / bottom;

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Single FractionOf( this Single x, Single top, Single bottom ) => ( top * x ) / bottom;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UInt64 FractionOf( this UInt64 x, UInt64 top, UInt64 bottom ) => ( top * x ) / bottom;

        /// <summary>Greatest Common Divisor for int</summary>
        /// <remarks>Uses recursion, passing a remainder each time.</remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Int32 gcd( this Int32 x, Int32 y ) {
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
        /// <returns></returns>
        public static Int64 gcd( this Int64 x, Int64 y ) {
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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

        [NotNull]
        public static UInt16[] GetBitFields( UInt32 packedBits, [NotNull] Byte[] bitFields ) {
            const Int32 maxBits = 32;
            var fields = bitFields.Length - 1;     // number of fields to unpack
            var retArr = new UInt16[ fields + 1 ]; // init return array
            var curPos = 0;                        // current field bit position (start)

            for ( var f = fields; f >= 0; f-- ) // loop from last
            {
                var lastEnd = curPos;     // position where last field ended
                curPos += bitFields[ f ]; // we get where the current value starts

                var leftShift = maxBits - curPos; // we figure how much left shift we gotta apply for the other numbers to overflow into oblivion

                retArr[ f ] = ( UInt16 ) ( ( packedBits << leftShift ) >> ( leftShift + lastEnd ) ); // we do magic
            }

            return retArr;
        }

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Single Half( this Single number ) => number / 2.0f;

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Double Half( this Double number ) => number / 2.0d;

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Byte Half( this Byte number ) => ( Byte ) ( number / 2 );

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static TimeSpan Half( this TimeSpan timeSpan ) => TimeSpan.FromTicks( timeSpan.Ticks.Half() );

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Int32 Half( this Int32 number ) => number / 2;

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Int16 Half( this Int16 number ) => ( Int16 ) ( number / 2 );

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static UInt16 Half( this UInt16 number ) => ( UInt16 ) ( number / 2 );

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static UInt32 Half( this UInt32 number ) => number / 2;

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static UInt64 Half( this UInt64 number ) => number / 2;

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Int64 Half( this Int64 number ) => number / 2;

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Decimal Half( this Decimal number ) => number / 2.0m;

        /// <summary>
        ///     <para>If the <paramref name="number" /> is less than <see cref="Decimal.Zero" />, then return <see cref="Decimal.Zero" />.</para>
        ///     <para>Otherwise return the <paramref name="number" />.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Decimal IfLessThanZeroThenZero( this Decimal number ) => number < Decimal.Zero ? Decimal.Zero : number;

        /// <summary>
        ///     <para>If the <paramref name="number" /> is less than <see cref="BigInteger.Zero" />, then return <see cref="Decimal.Zero" />.</para>
        ///     <para>Otherwise return the <paramref name="number" />.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static BigInteger IfLessThanZeroThenZero( this BigInteger number ) => number < BigInteger.Zero ? BigInteger.Zero : number;

        /// <summary>
        ///     <para>If the <paramref name="number" /> is less than <see cref="Rational.Zero" />, then return <see cref="Decimal.Zero" />.</para>
        ///     <para>Otherwise return the <paramref name="number" />.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Rational IfLessThanZeroThenZero( this Rational number ) => number < Rational.Zero ? Rational.Zero : number;

        /// <summary>
        ///     <para>If the <paramref name="number" /> is less than <see cref="Rational.Zero" />, then return <see cref="Decimal.Zero" />.</para>
        ///     <para>Otherwise return the <paramref name="number" />.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Rational IfLessThanZeroThenZero( this Rational? number ) {
            if ( !number.HasValue || ( number <= Rational.Zero ) ) {
                return Rational.Zero;
            }

            return number.Value;
        }

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsEven( this Int32 value ) => 0 == ( value % 2 );

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsEven( this Int64 value ) => 0 == ( value % 2 );

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsNegative( this Single value ) => value < 0.0f;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsNumber( this Single value ) => !Single.IsNaN( value ) && !Single.IsInfinity( value );

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsNumber( this Double value ) {
            if ( Double.IsNaN( value ) ) {
                return default;
            }

            return !Double.IsInfinity( value );
        }

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsOdd( this Int32 value ) => 0 != ( value % 2 );

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsOdd( this Int64 value ) => 0 != ( value % 2 );

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsPositive( this Single value ) => value > 0.0f;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsPowerOfTwo( this Int32 number ) => ( number & -number ) == number;

        /// <summary>Linearly interpolates between two values.</summary>
        /// <param name="source">Source value.</param>
        /// <param name="target">Target value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Single Lerp( this Single source, Single target, Single amount ) => source + ( ( target - source ) * amount );

        /// <summary>Linearly interpolates between two values.</summary>
        /// <param name="source">Source value.</param>
        /// <param name="target">Target value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Double Lerp( this Double source, Double target, Single amount ) => source + ( ( target - source ) * amount );

        /// <summary>Linearly interpolates between two values.</summary>
        /// <param name="source">Source value.</param>
        /// <param name="target">Target value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UInt64 Lerp( this UInt64 source, UInt64 target, Single amount ) => ( UInt64 ) ( source + ( ( target - source ) * amount ) );

        /// <summary>Linearly interpolates between two values.</summary>
        /// <param name="source">Source value.</param>
        /// <param name="target">Target value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UInt32 Lerp( this UInt32 source, UInt32 target, Single amount ) => ( UInt32 ) ( source + ( ( target - source ) * amount ) );

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Double LogFactorial( this Int32 n ) {
            if ( n < 0 ) {
                throw new ArgumentOutOfRangeException();
            }

            if ( n <= 254 ) {
                return MathConstants.Logfactorialtable[ n ];
            }

            var x = n + 1d;

            return ( ( ( x - 0.5 ) * Math.Log( x ) ) - x ) + ( 0.5 * Math.Log( 2 * Math.PI ) ) + ( 1.0 / ( 12.0 * x ) );
        }

        /// <summary>compute log(1+x) without losing precision for small values of x</summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Double LogOnePlusX( this Double x ) {
            if ( x <= -1.0 ) {
                throw new ArgumentOutOfRangeException( nameof( x ), $"Invalid input argument: {x}" );
            }

            if ( Math.Abs( x ) > 1e-4 ) {

                // x is large enough that the obvious evaluation is OK
                return Math.Log( 1.0 + x );
            }

            // Use Taylor approx. log(1 + x) = x - x^2/2 with error roughly x^3/3 since |x| < 10^-4, |x|^3 < 10^-12, relative error less than 10^-8
            return ( ( -0.5 * x ) + 1.0 ) * x;
        }

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Near( this Double number, Double target ) => Math.Abs( number - target ) <= Double.Epsilon;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Near( this Int32 number, Int32 target ) => Math.Abs( number - target ) <= Double.Epsilon;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Near( this Single number, Single target ) => Math.Abs( number - target ) <= Single.Epsilon;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Near( this Point here, Point there ) => here.X.Near( there.X ) && here.Y.Near( there.Y );

        /*
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Near( this Point3D here, Point3D there ) => here.X.Near( there.X ) && here.Y.Near( there.Y ) && here.Z.Near( there.Z );
        */

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Near( this Decimal number, Decimal target ) => Math.Abs( number - target ) <= EpsilonDecimal;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Near( this Rational number, Rational target ) {
            var difference = number - target;

            if ( difference < Rational.Zero ) {
                difference = -difference;
            }

            return difference <= ( Rational ) EpsilonDecimal;
        }

        public static Boolean Near( this PointF here, PointF there ) => here.X.Near( there.X ) && here.Y.Near( there.Y );

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Near( this BigInteger number, BigInteger target ) {
            var difference = number - target;

            return BigInteger.Zero == difference;
        }

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Near( this UInt64 number, UInt64 target ) => ( number - target ) <= UInt64.MinValue;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Near( this Int64 number, Int64 target ) => ( number - target ) <= Int64.MinValue;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Near( this UBigInteger number, UBigInteger target ) => ( number - target ) <= UBigInteger.Epsilon;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Double Nested( this Double x ) => Math.Sqrt( x * 100.0 ) / 100.0d;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Single Nested( this Single x ) => ( Single ) ( Math.Sqrt( x * 100.0 ) / 100.0f );

        /// <summary>Remove all the trailing zeros from the decimal</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Decimal Normalize( this Decimal value ) => value / 1.000000000000000000000000000000000m;

        /// <summary></summary>
        /// <param name="baseValue"></param>
        /// <param name="n">        </param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/a/18363540/956364" />
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Decimal NthRoot( this Decimal baseValue, Int32 n ) {
            if ( n == 1 ) {
                return baseValue;
            }

            Decimal deltaX;
            var x = 0.1M;

            do {
                deltaX = ( ( baseValue / x.Pow( n - 1 ) ) - x ) / n;
                x += deltaX;
            } while ( Math.Abs( deltaX ) > 0 );

            return x;
        }

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UInt64 OneHundreth( this UInt64 x ) => x / 100;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UInt64 OneQuarter( this UInt64 x ) => x / 4;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Single OneQuarter( this Single x ) => x / 4.0f;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UInt64 OneTenth( this UInt64 x ) => x / 10;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Single OneThird( this Single x ) => x / 3.0f;

        public static UInt32 PackBitFields( [NotNull] UInt16[] values, [NotNull] Byte[] bitFields ) {
            if ( bitFields is null ) {
                throw new ArgumentNullException( nameof( bitFields ) );
            }

            UInt32 retVal = values[ 0 ]; //we set the first value right away

            for ( var f = 1; f < values.Length; f++ ) {
                retVal <<= bitFields[ f ]; //we shift the previous value
                retVal += values[ f ];     //and add our current value //on some processors | (pipe) will be faster here
            }

            return retVal;
        }

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
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
            var t = 1.0 / ( 1.0 + ( p * x ) );
            var y = 1.0 - ( ( ( ( ( ( ( ( ( a5 * t ) + a4 ) * t ) + a3 ) * t ) + a2 ) * t ) + a1 ) * t * Math.Exp( -x * x ) );

            return 0.5 * ( 1.0 + ( sign * y ) );
        }

        [DebuggerStepThrough]
        [Pure]
        public static Decimal Pow( this Decimal baseValue, Int32 n ) {
            for ( var i = 0; i < ( n - 1 ); i++ ) {
                baseValue *= baseValue;
            }

            return baseValue;
        }

        /// <summary><see cref="Decimal" /> raised to the nth power.</summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <returns></returns>
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

                if ( found.TakeWhile( prime => ( prime * prime ) <= candidate1 ).All( prime => ( candidate2 % prime ) != 0 ) ) {
                    found.Add( candidate );

                    yield return candidate;
                }

                candidate += 2;
            }
        }

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Decimal Quarter( this Decimal number ) => number / 4.0m;

        public static Double Root( this Double x, Double root ) => Math.Pow( x, 1.0 / root );

        public static Double Root( this Decimal x, Decimal root ) => Math.Pow( ( Double ) x, ( Double ) ( 1.0m / root ) );

        public static UInt64 RotateLeft( this UInt64 original, Int32 bits ) => ( original << bits ) | ( original >> ( 64 - bits ) );

        public static UInt64 RotateRight( this UInt64 original, Int32 bits ) => ( original >> bits ) | ( original << ( 64 - bits ) );

        /// <summary>Truncate, don't round. Just chop it off.</summary>
        /// <param name="number">       </param>
        /// <param name="decimalPlaces"></param>
        /// <returns>Bitcoin ftw!</returns>
        public static Decimal Sanitize( this Decimal number, UInt16 decimalPlaces = 8 ) {
            number *= ( Decimal ) Math.Pow( 10, decimalPlaces );

            number = Math.Truncate( number ); //Truncate, don't round. Just chop it off.

            number *= ( Decimal ) Math.Pow( 10, -decimalPlaces );

            return number;
        }

        /// <summary>Smooths a value to between 0 and 1.</summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Double Sigmoid0To1( this Double x ) => 1.0D / ( 1.0D + Math.Exp( -x ) );

        /// <summary>Smooths a value to between 0 and 1.</summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Decimal Sigmoid0To1( this Decimal x ) => 1.0M / ( 1.0M + ( Decimal ) Math.Exp( ( Double ) ( -x ) ) );

        /// <summary>Smooths a value to between -1 and 1.</summary>
        /// <param name="x"></param>
        /// <returns></returns>
        /// <see cref="http://www.wolframalpha.com/input/?i=1+-+%28+2+%2F+%281+%2B+Exp%28+v+%29+%29+%29%2C+v+from+-10+to+10" />
        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Double SigmoidNeg1To1( this Double x ) => 1.0D - ( 2.0D / ( 1.0D + Math.Exp( x ) ) );

        public static Double Slope( [NotNull] this List<TimeProgression> data ) {
            if ( data is null ) {
                throw new ArgumentNullException( nameof( data ) );
            }

            var averageX = data.Average( d => d.MillisecondsPassed );
            var averageY = data.Average( d => d.Progress );

            var a = data.Sum( d => ( d.MillisecondsPassed - averageX ) * ( d.Progress - averageY ) );
            var b = data.Sum( d => Math.Pow( d.MillisecondsPassed - averageX, 2 ) );

            return a / b;
        }

        /// <summary>Return the integer part and the fraction parts of a <see cref="Decimal" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [NotNull]
        public static Tuple<Decimal, Decimal> Split( this Decimal value ) {
            var parts = value.ToString( "R" ).Split( '.' );
            var result = new Tuple<Decimal, Decimal>( Decimal.Parse( parts[ 0 ] ), Decimal.Parse( "0." + parts[ 1 ] ) );

            return result;
        }

        /// <summary>Return the integer part and the fraction parts of a <see cref="Double" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [NotNull]
        public static Tuple<Double, Double> Split( this Double value ) {
            var parts = value.ToString( "R" ).Split( '.' );

            return new Tuple<Double, Double>( Double.Parse( parts[ 0 ] ), Double.Parse( "0." + parts[ 1 ] ) );
        }

        /// <summary>Split one <see cref="UInt64" /> value into two <see cref="UInt32" /> values. Use <see cref="Combine" /> for the reverse.</summary>
        /// <param name="value"></param>
        /// <param name="high"> </param>
        /// <param name="low">  </param>
        public static void Split( this UInt64 value, out UInt32 high, out UInt32 low ) {
            high = ( UInt32 ) ( value >> 32 );
            low = ( UInt32 ) ( value & UInt32.MaxValue );
        }

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Single Squared( this Single number ) => number * number;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Double Squared( this Double number ) => number * number;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Decimal Squared( this Decimal number ) => number * number;

        public static Double SquareRootOfProducts( [NotNull] this IEnumerable<Double> data ) {
            var sorted = new List<Double>( data.Where( d => Math.Abs( d ) >= Double.Epsilon ).OrderBy( d => d ) );

            var aggregate = Rational.One;

            while ( sorted.Any() ) {
                sorted.TakeFirst( out var smallest );

                if ( !sorted.TakeLast( out var largest ) ) {
                    largest = 1;
                }

                aggregate *= ( Rational ) smallest;
                aggregate *= ( Rational ) largest;

                //aggregate.Should().NotBe( Double.NaN );
                //aggregate.Should().NotBe( Double.NegativeInfinity );
                //aggregate.Should().NotBe( Double.PositiveInfinity );
            }

            //foreach ( Double d in data ) {aggregate = aggregate * d;}
            return Math.Sqrt( ( Double ) aggregate );
        }

        public static Decimal SquareRootOfProducts( [NotNull] this IEnumerable<Decimal> data ) {
            var aggregate = data.Aggregate( 1.0m, ( current, d ) => current * d );

            return ( Decimal ) Math.Sqrt( ( Double ) aggregate );
        }

        /// <summary>Subtract <paramref name="tax" /> of <paramref name="total" /> from <paramref name="total" />.
        /// <para>If the tax was 6% on $53, then you would call this function like this:</para>
        /// <para>var withTax = SubtractTax( 53.00, 0.06 );</para>
        /// <para>Assert( withTax == 50.00 );</para>
        /// </summary>
        /// <param name="total"></param>
        /// <param name="tax">  </param>
        /// <returns></returns>
        public static Decimal SubtractTax( this Decimal total, Decimal tax ) {
            var taxed = total / ( 1.0m + tax );

            return taxed;
        }

        /// <summary>
        /// Subtract <paramref name="right" /> away from <paramref name="left" /> without the chance of "throw new ArgumentOutOfRangeException( "amount", String.Format( "Values {0}
        /// and {1} are loo small to handle.", amount, uBigInteger ) );"
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        public static UInt64 SubtractWithoutUnderFlow( this UInt64 left, UInt64 right ) {
            var integer = new UBigInteger( left ) - new UBigInteger( right );

            if ( integer < new UBigInteger( UInt64.MinValue ) ) {
                return UInt64.MinValue;
            }

            return ( UInt64 ) integer;
        }

        /// <summary>
        ///     <para>Returns the sum of all <see cref="BigInteger" />.</para>
        /// </summary>
        /// <param name="bigIntegers"></param>
        /// <returns></returns>
        public static BigInteger Sum( [NotNull] this IEnumerable<BigInteger> bigIntegers ) =>
            bigIntegers.Aggregate( BigInteger.Zero, ( current, bigInteger ) => current + bigInteger );

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Double TendTo( this Double number, Double goal ) => ( number + goal ).Half();

        [DebuggerStepThrough]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static TimeSpan TendTo( this TimeSpan time, TimeSpan goal ) => ( time + goal ).Half();

        public static Int32 ThreeFourths( this Int32 x ) {
            var result = ( 3.0 * x ) / 4.0;

            return result < 1.0 ? 1 : ( Int32 ) result;
        }

        public static UInt64 ThreeQuarters( this UInt64 x ) => ( 3 * x ) / 4;

        public static Single ThreeQuarters( this Single x ) => ( 3.0f * x ) / 4.0f;

        public static Double ThreeQuarters( this Double x ) => ( 3.0d * x ) / 4.0d;

        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static TimeSpan Thrice( this TimeSpan timeSpan ) => TimeSpan.FromTicks( timeSpan.Ticks.Thrice() );

        public static Int64 Thrice( this Int64 number ) => number * 3L;

        ///// <summary>
        /////     Creates an enumerable that iterates the range [fromInclusive, toExclusive).
        ///// </summary>
        ///// <param name="fromInclusive">The lower bound, inclusive.</param>
        ///// <param name="toExclusive">The upper bound, exclusive.</param>
        ///// <returns>The enumerable of the range.</returns>
        //public static IEnumerable<BigInteger> To( this BigInteger fromInclusive, BigInteger toExclusive ) {
        //    for ( var i = fromInclusive; i < toExclusive; i++ ) {
        //        yield return i;
        //    }
        //}

        /// <summary>
        ///     <see cref="http://stackoverflow.com/questions/17575375/how-do-i-convert-an-int-to-a-String-in-c-sharp-without-using-tostring" />
        /// </summary>
        /// <param name="number">   </param>
        /// <param name="base">     </param>
        /// <param name="minDigits"></param>
        /// <returns></returns>
        [NotNull]
        public static String ToStringWithBase( this Int32 number, Int32 @base, Int32 minDigits = 1 ) {
            if ( minDigits < 1 ) {
                minDigits = 1;
            }

            if ( number == 0 ) {
                return new String( '0', minDigits );
            }

            var s = "";

            if ( ( @base < 2 ) || ( @base > MathConstants.NumberBaseChars.Length ) ) {
                return s;
            }

            var neg = false;

            if ( ( @base == 10 ) && ( number < 0 ) ) {
                neg = true;
                number = -number;
            }

            var n = ( UInt32 ) number;
            var b = ( UInt32 ) @base;

            while ( ( n > 0 ) | ( minDigits-- > 0 ) ) {
                s = MathConstants.NumberBaseChars[ ( Int32 ) ( n % b ) ] + s;
                n /= b;
            }

            if ( neg ) {
                s = "-" + s;
            }

            return s;
        }

        public static UInt64? ToUInt64( [CanBeNull] this String text ) => UInt64.TryParse( text, out var result ) ? ( UInt64? ) result : null;

        public static UInt64 ToUInt64( [NotNull] this Byte[] bytes, Int32 pos ) =>
            ( UInt64 ) ( bytes[ pos++ ] | ( bytes[ pos++ ] << 8 ) | ( bytes[ pos++ ] << 16 ) | ( bytes[ pos ] << 24 ) );

        public static Int64 Truncate( this Single number ) => ( Int64 ) number;

        public static Int64 Truncate( this Double number ) => ( Int64 ) number;

        /// <summary>
        ///     <para>Attempt to parse a fraction from a String.</para>
        /// </summary>
        /// <example>" 1234 / 346 "</example>
        /// <param name="numberString"></param>
        /// <param name="result">      </param>
        /// <returns></returns>
        public static Boolean TryParse( [CanBeNull] this String numberString, out Rational result ) {
            result = Rational.Zero;

            if ( null == numberString ) {
                return default;
            }

            numberString = numberString.Trim();

            if ( numberString.IsNullOrEmpty() ) {
                return default;
            }

            var parts = numberString.Split( '/' ).ToList();

            if ( parts.Count != 2 ) {
                return default;
            }

            var top = parts.TakeFirst();

            if ( top.IsNullOrWhiteSpace() ) {
                return default;
            }

            top = top.Trim();

            var bottom = parts.TakeLast();

            if ( String.IsNullOrWhiteSpace( bottom ) ) {
                return default;
            }

            if ( parts.Count > 0 ) {
                return default;
            }

            BigInteger.TryParse( top, out var numerator );

            BigInteger.TryParse( bottom, out var denominator );

            result = new Rational( numerator, denominator );

            return true;
        }

        public static Boolean TrySplitDecimal( this Decimal value, out BigInteger beforeDecimalPoint, out BigInteger afterDecimalPoint ) {
            var theString = value.ToString( "R" );

            if ( !theString.Contains( "." ) ) {
                theString += ".0";
            }

            var split = theString.Split( '.' );

            afterDecimalPoint = BigInteger.Zero;

            return BigInteger.TryParse( split[ 0 ], out beforeDecimalPoint ) && BigInteger.TryParse( split[ 1 ], out afterDecimalPoint );
        }

        public static Int32 TurnBitsOff( this Int32 value, Byte bitToTurnOff ) => value & ~bitToTurnOff;

        public static Int64 TurnBitsOff( this Int64 value, Byte bitToTurnOff ) => value & ~bitToTurnOff;

        public static UInt64 TurnBitsOff( this UInt64 value, Byte bitToTurnOff ) => value & ( UInt64 ) ~bitToTurnOff;

        public static Byte TurnBitsOff( this Byte value, Byte bitToTurnOff ) => ( Byte ) ( value & ~bitToTurnOff );

        public static Int32 TurnBitsOn( this Int32 value, Byte bitToTurnOn ) => value | bitToTurnOn;

        public static Int64 TurnBitsOn( this Int64 value, Byte bitToTurnOn ) => value | bitToTurnOn;

        public static UInt64 TurnBitsOn( this UInt64 value, Byte bitToTurnOn ) => value | bitToTurnOn;

        public static Byte TurnBitsOn( this Byte value, Byte bitToTurnOn ) => ( Byte ) ( value | bitToTurnOn );

        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static TimeSpan Twice( this TimeSpan timeSpan ) => TimeSpan.FromTicks( timeSpan.Ticks.Twice() );

        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Single Twice( this Single x ) => x * 2.0f;

        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Double Twice( this Double number ) => number * 2d;

        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Decimal Twice( this Decimal number ) => number * 2m;

        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Int64 Twice( this Int64 number ) => number * 2L;

    }

}