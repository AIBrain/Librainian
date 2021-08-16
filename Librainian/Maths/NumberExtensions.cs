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
// File "NumberExtensions.cs" last formatted on 2020-08-14 at 8:36 PM.

#nullable enable

namespace Librainian.Maths {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Numerics;
	using System.Runtime.CompilerServices;
	using System.Text;
	using Exceptions;
	using JetBrains.Annotations;
	using Measurement.Time;
	using Numbers;
	using Rationals;

	public static class NumberExtensions {

		/// <summary>Table used for reversing bits.</summary>
		private static readonly Byte[] BitReverseTable256 = {
			0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90,
			0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0, 0x08, 0x88, 0x48, 0xC8,
			0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8,
			0x78, 0xF8, 0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4,
			0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4, 0x0C, 0x8C,
			0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC,
			0x3C, 0xBC, 0x7C, 0xFC, 0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2,
			0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2,
			0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A,
			0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA, 0x06, 0x86, 0x46, 0xC6,
			0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6,
			0x76, 0xF6, 0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE,
			0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE, 0x01, 0x81,
			0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1,
			0x31, 0xB1, 0x71, 0xF1, 0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9,
			0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9,
			0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95,
			0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5, 0x0D, 0x8D, 0x4D, 0xCD,
			0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD,
			0x7D, 0xFD, 0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3,
			0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3, 0x0B, 0x8B,
			0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB,
			0x3B, 0xBB, 0x7B, 0xFB, 0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7,
			0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7,
			0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F,
			0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF
		};

		private static readonly String[] SizeSuffixes = {
			"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
		};

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this SByte number ) => number > 0;

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Byte number ) => number > 0;

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Int16 number ) => number > 0;

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Int32 number ) => number > 0;

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Int64 number ) => number > 0;

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this UInt16 number ) => number > 0;

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this UInt32 number ) => number > 0;

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this UInt64 number ) => number > 0;

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this UInt64? number ) => number > 0;

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Int64? number ) => number > 0;

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Decimal number ) => number > Decimal.Zero;

		/// <summary>Returns true if <paramref name="number" /> is greater than 0.</summary>
		/// <param name="number"></param>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Double number ) => number > 0;

		/// <summary>Counts the number of set (bit = 1) bits in a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>Number of set (1) bits.</returns>
		[Pure]
		public static Int32 BitsOn( this Int64 value ) {
			Int32 i;

			for ( i = 0; value != 0; i++ ) {
				value &= value - 1;
			}

			return i;
		}

		/// <summary>Counts the number of set (bit = 1) bits in a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>Number of set (1) bits.</returns>
		[Pure]
		public static Int32 BitsOn( this UInt64 value ) {
			Int32 i;

			for ( i = 0; value != 0; i++ ) {
				value &= value - 1;
			}

			return i;
		}

		/// <summary>Counts the number of set (bit = 1) bits in a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>Number of set (1) bits.</returns>
		[Pure]
		public static Int32 BitsOn( this Int32 value ) {
			Int32 i;

			for ( i = 0; value != 0; i++ ) {
				value &= value - 1;
			}

			return i;
		}

		/// <summary>Counts the number of set (bit = 1) bits in a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>Number of set (1) bits.</returns>
		[Pure]
		public static Int32 BitsOn( this UInt32 value ) {
			Int32 i;

			for ( i = 0; value != 0; i++ ) {
				value &= value - 1;
			}

			return i;
		}

		/// <summary>Counts the number of set (bit = 1) bits in a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>Number of set (1) bits.</returns>
		[Pure]
		public static Int32 BitsOn( this Int16 value ) {
			Int32 i;

			for ( i = 0; value != 0; i++ ) {
				value &= ( Int16 )( value - 1 );
			}

			return i;
		}

		/// <summary>Counts the number of set (bit = 1) bits in a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>Number of set (1) bits.</returns>
		[Pure]
		public static Int32 BitsOn( this UInt16 value ) {
			Int32 i;

			for ( i = 0; value != 0; i++ ) {
				value &= ( UInt16 )( value - 1 );
			}

			return i;
		}

		/// <summary>Counts the number of set (bit = 1) bits in a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>Number of set (1) bits.</returns>
		[Pure]
		public static Int32 BitsOn( this Byte value ) {
			Int32 i;

			for ( i = 0; value != 0; i++ ) {
				value &= ( Byte )( value - 1 );
			}

			return i;
		}

		/// <summary>Counts the number of set (bit = 1) bits in a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>Number of set (1) bits.</returns>
		[Pure]
		public static Int32 BitsOn( this SByte value ) {
			Int32 i;

			for ( i = 0; value != 0; i++ ) {
				value &= ( SByte )( value - 1 );
			}

			return i;
		}

		public static TimeSpan GetStep( this DateTime from, DateTime to ) {
			var diff = from >= to ? from - to : to - from;

			if ( diff.TotalDays > 1 ) {
				return Days.One;
			}

			if ( diff.TotalHours > 1 ) {
				return Hours.One;
			}

			if ( diff.TotalMinutes > 1 ) {
				return Minutes.One;
			}

			if ( diff.TotalSeconds > 1 ) {
				return Seconds.One;
			}

			return Milliseconds.One;
		}

		/// <summary>Finds the parity of a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>True for even, False for odd.</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Boolean Parity( this Int64 value ) {
			Int64 i;

			for ( i = 0; value != 0; value >>= 1 ) {
				i += value & 1;
			}

			return i % 2 == 1;
		}

		/// <summary>Finds the parity of a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>True for even, False for odd.</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Boolean Parity( this UInt64 value ) {
			UInt64 i;

			for ( i = 0; value != 0; value >>= 1 ) {
				i += value & 1;
			}

			return i % 2 == 1;
		}

		/// <summary>Finds the parity of a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>True for even, False for odd.</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Boolean Parity( this Int32 value ) {
			Int32 i;

			for ( i = 0; value != 0; value >>= 1 ) {
				i += value & 1;
			}

			return i % 2 == 1;
		}

		/// <summary>Finds the parity of a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>True for even, False for odd.</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Boolean Parity( this UInt32 value ) {
			UInt32 i;

			for ( i = 0; value != 0; value >>= 1 ) {
				i += value & 1;
			}

			return i % 2 == 1;
		}

		/// <summary>Finds the parity of a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>True for even, False for odd.</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Boolean Parity( this Int16 value ) {
			Int32 i;

			for ( i = 0; value != 0; value >>= 1 ) {
				i += value & 1;
			}

			return i % 2 == 1;
		}

		/// <summary>Finds the parity of a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>True for even, False for odd.</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Boolean Parity( this UInt16 value ) {
			Int32 i;

			for ( i = 0; value != 0; value >>= 1 ) {
				i += value & 1;
			}

			return i % 2 == 1;
		}

		/// <summary>Finds the parity of a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>True for even, False for odd.</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Boolean Parity( this SByte value ) {
			Int32 i;

			for ( i = 0; value != 0; value >>= 1 ) {
				i += value & 1;
			}

			return i % 2 == 1;
		}

		/// <summary>Finds the parity of a given value.</summary>
		/// <param name="value">Value to check.</param>
		/// <returns>True for even, False for odd.</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Boolean Parity( this Byte value ) => ( ( ( ( UInt64 )( value * 0x0101010101010101 ) & 0x8040201008040201 ) % 0x1FF ) & 1 ) != 0;

		/// <summary>Reverses the bit order of a variable (ie: 0100 1000 becomes 0001 0010)</summary>
		/// <param name="source">Source value to reverse</param>
		/// <returns>Input value with reversed bits</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Byte ReverseBits( this Byte source ) => ( Byte )( ( ( ( ( source * 0x0802 ) & 0x22110 ) | ( ( source * 0x8020 ) & 0x88440 ) ) * 0x10101 ) >> 16 );

		/// <summary>Reverses the bit order of a variable (ie: 0100 1000 becomes 0001 0010)</summary>
		/// <param name="source">Source value to reverse</param>
		/// <returns>Input value with reversed bits</returns>
		public static Int32 ReverseBits( this Int32 source ) =>
			( BitReverseTable256[ source & 0xff ] << 24 ) | ( BitReverseTable256[ ( source >> 8 ) & 0xff ] << 16 ) | ( BitReverseTable256[ ( source >> 16 ) & 0xff ] << 8 ) |
			BitReverseTable256[ ( source >> 24 ) & 0xff ];

		/// <summary>Reverses the bit order of a variable (ie: 0100 1000 becomes 0001 0010)</summary>
		/// <param name="source">Source value to reverse</param>
		/// <returns>Input value with reversed bits</returns>
		public static UInt32 ReverseBits( this UInt32 source ) =>
			( UInt32 )( ( BitReverseTable256[ source & 0xff ] << 24 ) | ( BitReverseTable256[ ( source >> 8 ) & 0xff ] << 16 ) |
						( BitReverseTable256[ ( source >> 16 ) & 0xff ] << 8 ) | BitReverseTable256[ ( source >> 24 ) & 0xff ] );

		/// <summary>Reverses the bit order of a variable (ie: 0100 1000 becomes 0001 0010)</summary>
		/// <param name="source">Source value to reverse</param>
		/// <returns>Input value with reversed bits</returns>
		public static UInt16 ReverseBits( this UInt16 source ) {
			source = ( UInt16 )( ( ( source >> 1 ) & 0x5555 ) | ( ( source & 0x5555 ) << 1 ) );
			source = ( UInt16 )( ( ( source >> 2 ) & 0x3333 ) | ( ( source & 0x3333 ) << 2 ) );
			source = ( UInt16 )( ( ( source >> 4 ) & 0x0F0F ) | ( ( source & 0x0F0F ) << 4 ) );

			return ( UInt16 )( ( source >> 8 ) | ( source << 8 ) );
		}

		/// <summary>Reverses the bit order of a variable (ie: 0100 1000 becomes 0001 0010)</summary>
		/// <param name="source">Source value to reverse</param>
		/// <returns>Input value with reversed bits</returns>
		public static Int16 ReverseBits( this Int16 source ) {
			source = ( Int16 )( ( ( source >> 1 ) & 0x5555 ) | ( ( source & 0x5555 ) << 1 ) );
			source = ( Int16 )( ( ( source >> 2 ) & 0x3333 ) | ( ( source & 0x3333 ) << 2 ) );
			source = ( Int16 )( ( ( source >> 4 ) & 0x0F0F ) | ( ( source & 0x0F0F ) << 4 ) );

			return ( Int16 )( ( source >> 8 ) | ( source << 8 ) );
		}

		[DebuggerStepThrough]
		[Pure]
		public static String SizeSuffix( this Int64 value, Int32 decimalPlaces = 1 ) {
			if ( value < 0 ) {
				return "-" + SizeSuffix( -value );
			}

			if ( value == 0 ) {
				return "0 bytes";
			}

			// mag is 0 for bytes, 1 for KB, 2, for MB.
			var mag = ( Int32 )Math.Log( value, 1024 );

			// 1L << (mag * 10) == 2 ^ (10 * mag) [i.e. the number of bytes in the unit corresponding to mag]
			var adjustedSize = ( Decimal )value / ( 1L << ( mag * 10 ) );

			// make adjustment when the value is large enough that it would round up to 1000 or more
			if ( Math.Round( adjustedSize, decimalPlaces ) >= 1000 ) {
				mag += 1;
				adjustedSize /= 1024;
			}

			return String.Format( $"{{0:n{decimalPlaces}}} {{1}}", adjustedSize, SizeSuffixes[ mag ] );
		}

		public static IEnumerable<Int32> Through( this Int32 begin, Int32 end ) {
			Int32 offset;

			if ( begin < end ) {
				offset = 1;
			}
			else {
				offset = -1;
			}

			for ( var i = begin; i != end + offset; i += offset ) {
				yield return i;
			}
		}

		/// <summary>Example: foreach (var i in 102.To(204)) { Console.WriteLine(i); }</summary>
		/// <param name="start"></param>
		/// <param name="end">  </param>
		/// <param name="step"> </param>
		public static IEnumerable<Byte> To( this Byte start, Byte end, Byte step = 1 ) {
			if ( step == 0 ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			if ( start <= end ) {
				for ( var b = start; b <= end; b += step ) {
					yield return b;

					if ( b == Byte.MaxValue ) {
						yield break; //special case to deal with overflow
					}
				}
			}
			else {
				for ( var b = start; b >= end; b -= step ) {
					yield return b;

					if ( b == Byte.MinValue ) {
						yield break; //special case to deal with underflow
					}
				}
			}
		}

		/// <summary>Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }</summary>
		/// <param name="begin"></param>
		/// <param name="end">  </param>
		/// <param name="step"> </param>
		public static IEnumerable<UInt64> To( this Int32 begin, UInt64 end, UInt64 step = 1 ) {
			if ( step == 0 ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			var start = ( Decimal )begin;

			if ( start <= end ) {
				const Decimal maxValue = UInt64.MaxValue;

				for ( var value = start; value <= end; value += step ) {
					yield return ( UInt64 )value;   //TODO needs unit tested

					if ( value >= maxValue ) {
						yield break; //special case to deal with overflow
					}
				}
			}
			else {
				const Decimal minValue = UInt64.MinValue;

				for ( var ul = start; ul >= end; ul -= step ) {
					yield return ( UInt64 )ul; //TODO needs unit test

					if ( ul < minValue ) {
						yield break; //special case to deal with overflow
					}
				}
			}
		}

		/// <summary>Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }</summary>
		/// <param name="begin">inclusive</param>
		/// <param name="end">  inclusive</param>
		/// <param name="step"> </param>
		[Pure]
		public static IEnumerable<Int32> To( this Int32 begin, Int32 end, Int32 step = 1 ) {
			if ( step == 0 ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			if ( begin == end ) {
				/*no-op?*/
			}
			else if ( begin < end ) {
				for ( var i = begin; i < end; i += step ) {
					yield return i;
				}
			}
			else {
				var length = end - begin;

				for ( var i = length - 1; i >= 0; i-- ) {
					yield return i; //TODO needs a proper test, with many variations
				}
			}
		}

		/// <summary>Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }</summary>
		/// <param name="from"></param>
		/// <param name="end"> </param>
		/// <param name="step"></param>
		[Pure]
		public static IEnumerable<UInt64> To( this UInt64 from, UInt64 end, UInt64 step = 1 ) {
			if ( step == 0 ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			if ( from <= end ) {
				for ( var ul = from; ul <= end; ul += step ) {
					yield return ul;

					if ( ul == UInt64.MaxValue ) {
						yield break;
					} //special case to deal with overflow
				}
			}
			else {
				for ( var ul = from; ul >= end; ul -= step ) {
					yield return ul;

					if ( ul == UInt64.MinValue ) {
						yield break;
					} //special case to deal with overflow
				}
			}
		}

		/// <summary>Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }</summary>
		/// <param name="begin"></param>
		/// <param name="end"> </param>
		/// <param name="step"></param>
		[Pure]
		public static IEnumerable<Int64> To( this Int64 begin, Int64 end, Int64 step = 1 ) {
			if ( step == 0 ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			if ( begin <= end ) {
				for ( var ul = begin; ul <= end; ul += step ) {
					yield return ul;

					if ( ul == Int64.MaxValue ) {
						yield break;
					} //special case to deal with overflow
				}
			}
			else {
				for ( var ul = begin; ul >= end; ul -= step ) {
					yield return ul;

					if ( ul == Int64.MinValue ) {
						yield break;
					} //special case to deal with overflow
				}
			}
		}

		/// <summary>Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }</summary>
		/// <param name="from"></param>
		/// <param name="end">  </param>
		/// <param name="step"></param>
		[Pure]
		public static IEnumerable<BigInteger> To( this BigInteger from, BigInteger end, UInt64 step = 1 ) {
			if ( step == 0 ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			if ( from <= end ) {
				for ( var ul = from; ul <= end; ul += step ) {
					yield return ul;
				}
			}
			else {
				for ( var ul = from; ul >= end; ul -= step ) {
					yield return ul;
				}
			}
		}

		/// <summary>Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }</summary>
		/// <param name="begin"></param>
		/// <param name="end">  </param>
		/// <param name="step"></param>
		[Pure]
		public static IEnumerable<BigInteger> To( this Int64 begin, BigInteger end, UInt64 step = 1 ) {
			if ( step == 0 ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			BigInteger start = begin;

			if ( start <= end ) {
				for ( var ul = start; ul <= end; ul += step ) {
					yield return ul;
				}
			}
			else {
				for ( var ul = start; ul >= end; ul -= step ) {
					yield return ul;
				}
			}
		}

		/// <summary>Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }</summary>
		/// <param name="begin"></param>
		/// <param name="end">   </param>
		/// <param name="step"> </param>
		[Pure]
		public static IEnumerable<Rational> To( this Int32 begin, Rational end, Rational step ) {
			if ( step == 0 ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			Rational start = begin;

			if ( start <= end ) {
				for ( var ul = start; ul <= end; ul += step ) {
					yield return ul;
				}
			}
			else {
				for ( var ul = start; ul >= end; ul -= step ) {
					yield return ul;
				}
			}
		}

		/// <summary>
		///     Return each <see cref="DateTime" /> between <paramref name="from" /> and <paramref name="to" />, stepped by a
		///     <see cref="TimeSpan" /> ( <paramref name="step" />).
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to">  </param>
		/// <param name="step"></param>
		/// <remarks>//TODO Untested code!</remarks>
		/// <example>
		///     var now = DateTime.UtcNow; var then = now.AddMinutes( 10 ); var minutes = now.To( then, TimeSpan.FromMinutes( 1 )
		///     ); foreach ( var dateTime in minutes ) {
		///     Console.WriteLine( dateTime ); }
		/// </example>
		[Pure]
		public static IEnumerable<DateTime> To( this DateTime from, DateTime to, TimeSpan? step = null ) {
			step ??= from.GetStep( to );

			if ( step == TimeSpan.Zero ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			if ( from > to ) {
				for ( var dateTime = from; dateTime >= to; dateTime -= step.Value ) {
					yield return dateTime;
				}
			}
			else {
				for ( var dateTime = from; dateTime <= to; dateTime += step.Value ) {
					yield return dateTime;
				}
			}
		}

		[Pure]
		public static IEnumerable<Single> To( this Single start, Single end, Single step ) {
			if ( step == 0 ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			var count = end - start + 1.0f;

			for ( var idx = 0.0f; idx < count; idx += step ) {
				yield return start + idx;
			}
		}

		[Pure]
		public static IEnumerable<Double> To( this Double start, Double end, Single step ) {
			if ( step == 0 ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			if ( end >= start ) {
				for ( var i = start; i <= end; i += step ) {
					yield return i;
				}
			}
			else {
				for ( var i = start; i >= end; i -= step ) {
					yield return i;
				}
			}
		}

		[Pure]
		public static IEnumerable<Decimal> To( this Decimal start, Decimal end, Decimal step ) {
			if ( step == 0 ) {
				throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
			}

			if ( end >= start ) {
				for ( var i = start; i <= end; i += step ) {
					yield return i;
				}
			}
			else {
				for ( var i = start; i >= end; i -= step ) {
					yield return i;
				}
			}
		}

		[Pure]
		public static String ToHex( this IEnumerable<Byte> input ) {
			if ( input is null ) {
				throw new ArgumentEmptyException( nameof( input ) );
			}

			var result = new StringBuilder();

			foreach ( var b in input ) {
				result.Append( $"{result}{b:X2}" );
			}

			return result.ToString();
		}

		public static String ToHex( this UInt32 value ) => BitConverter.GetBytes( value ).Aggregate( String.Empty, ( current, b ) => current + b.ToString( "X2" ) );

		public static String ToHex( this UInt64 value ) => BitConverter.GetBytes( value ).Aggregate( String.Empty, ( current, b ) => current + b.ToString( "X2" ) );

		public static String ToHexNumberString( this IEnumerable<Byte> value ) => Bits.ToString( value.Reverse().ToArray() ).Replace( "-", "" ).ToLower();

		public static String ToHexNumberString( this UInt256 value ) => value.ToByteArray().ToHexNumberString();
	}
}