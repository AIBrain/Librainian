﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CommonExtensions.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "CommonExtensions.cs" was last formatted by Protiguous on 2018/11/02 at 11:03 PM.

namespace Librainian.Extensions {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using JetBrains.Annotations;
	using Maths;
	using Maths.Numbers;

	public static class CommonExtensions {

		/// <summary>
		///     Returns true if <paramref name="number" /> is greater than or equal to 1.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this SByte number ) => number >= 1;

		/// <summary>
		///     Returns true if <paramref name="number" /> is greater than or equal to 1.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Byte number ) => number >= 1;

		/// <summary>
		///     Returns true if <paramref name="number" /> is greater than or equal to 1.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Int16 number ) => number >= 1;

		/// <summary>
		///     Returns true if <paramref name="number" /> is greater than or equal to 1.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Int32 number ) => number >= 1;

		/// <summary>
		///     Returns true if <paramref name="number" /> is greater than or equal to 1.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Int64 number ) => number >= 1;

		/// <summary>
		///     Returns true if <paramref name="number" /> is greater than or equal to 1.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this UInt16 number ) => number >= 1;

		/// <summary>
		///     Returns true if <paramref name="number" /> is greater than or equal to 1.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this UInt32 number ) => number >= 1;

		/// <summary>
		///     Returns true if <paramref name="number" /> is greater than or equal to 1.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this UInt64 number ) => number >= 1;

		/// <summary>
		///     Returns true if <paramref name="number" /> is greater than or equal to 1.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Decimal number ) => number >= 1;

		/// <summary>
		///     Returns true if <paramref name="number" /> is greater than or equal to 1.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		[Pure]
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Any( this Double number ) => number >= 1;

		/// <summary>
		///     Return true if an <see cref="IComparable" /> value is <see cref="Between{T}" /> two inclusive values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">        </param>
		/// <param name="startInclusive"></param>
		/// <param name="endInclusive">  </param>
		/// <returns></returns>
		/// <example>5. Between(1, 10)</example>
		/// <example>5. Between(10, 1)</example>
		/// <example>5. Between(10, 6) == false</example>
		/// <example>5. Between(5, 5))</example>
		public static Boolean Between<T>( [NotNull] this T target, [NotNull] T startInclusive, T endInclusive ) where T : IComparable {
			if ( startInclusive.CompareTo( endInclusive ) == 1 ) {
				return target.CompareTo( startInclusive ) <= 0 && target.CompareTo( endInclusive ) >= 0;
			}

			return target.CompareTo( startInclusive ) >= 0 && target.CompareTo( endInclusive ) <= 0;
		}

		/// <summary>
		///     Returns a new <typeparamref name="T" /> that is the value of <paramref name="self" />, constrained between
		///     <paramref name="min" /> and <paramref name="max" />.
		/// </summary>
		/// <param name="self">The extended T.</param>
		/// <param name="min"> The minimum value of the <typeparamref name="T" /> that can be returned.</param>
		/// <param name="max"> The maximum value of the <typeparamref name="T" /> that can be returned.</param>
		/// <returns>The equivalent to: <c>this &lt; min ? min : this &gt; max ? max : this</c>.</returns>
		public static T Clamp<T>( [NotNull] this T self, T min, T max ) where T : IComparable<T> {
			if ( self.CompareTo( other: min ) < 0 ) {
				return min;
			}

			return self.CompareTo( other: max ) > 0 ? max : self;
		}

		public static IEnumerable<T> Concat<T>( [NotNull] this IEnumerable<T> first, T second ) {
			foreach ( var item in first ) {
				yield return item;
			}

			yield return second;
		}

		/// <summary>
		///     Just a no-op for setting a breakpoint on.
		/// </summary>
		[DebuggerStepThrough]
		public static void Nop<T>( this T obj ) { }

		/// <summary>
		///     <para>Works like the SQL "nullif" function.</para>
		///     <para>
		///         If <paramref name="left" /> is equal to <paramref name="right" /> then return null (or the default value for
		///         value types).
		///     </para>
		///     <para>Otherwise return <paramref name="left" />.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		[CanBeNull]
		[DebuggerStepThrough]
		public static T NullIf<T>( [NotNull] this T left, T right ) => left.Equals( right ) ? default : left;

		/// <summary>
		///     Swap.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public static void Swap<T>( ref T left, ref T right ) {
			var temp = left;
			left = right;
			right = temp;
		}

		/// <summary>
		///     Return (T right, T left).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static (T right, T left) Swap<T>( this T left, T right ) => (right, left);

		public static String ToHex( [NotNull] this IEnumerable<Byte> input ) {
			if ( input == null ) {
				throw new ArgumentNullException( nameof( input ) );
			}

			return input.Aggregate( "", ( current, b ) => current + b.ToString( "x2" ) );
		}

		public static String ToHex( this UInt32 value ) => BitConverter.GetBytes( value ).Aggregate( "", ( current, b ) => current + b.ToString( "x2" ) );

		public static String ToHex( this UInt64 value ) => BitConverter.GetBytes( value ).Aggregate( "", ( current, b ) => current + b.ToString( "x2" ) );

		[NotNull]
		public static String ToHexNumberString( [NotNull] this IEnumerable<Byte> value ) => Bits.ToString( value.Reverse().ToArray() ).Replace( "-", "" ).ToLower();

		[NotNull]
		public static String ToHexNumberString( this UInt256 value ) => value.ToByteArray().ToHexNumberString();
	}
}