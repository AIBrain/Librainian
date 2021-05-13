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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Feet.cs" last formatted on 2021-01-01 at 9:38 AM.

namespace Librainian.Measurement.Length {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Parsing;
	using Rationals;

	/// <summary>
	///     <para>A foot (plural: feet) is a unit of length.</para>
	///     <para>Since 1960 the term has usually referred to the international foot,</para>
	///     <para>defined as being one third of a yard, making it 0.3048 meters exactly.</para>
	///     <para>The foot is subdivided into 12 inches.</para>
	/// </summary>
	/// <see cref="http://wikipedia.org/wiki/Foot_(unit)" />
	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	public record Feet : IQuantityOfDistance, IComparable<Feet> {

		/// <summary>60</summary>
		public const Byte InOneYard = 3;

		public const Decimal FeetPerMeter = 3.28084m;

		public Feet( Rational value ) => this.Value = value;

		public Feet( Int64 value ) => this.Value = value;

		public Feet( BigInteger value ) => this.Value = value;

		/// <summary><see cref="Five" /> .</summary>
		public static Feet Five { get; } = new( 5 );

		/// <summary><see cref="One" /> .</summary>
		public static Feet One { get; } = new( 1 );

		/// <summary><see cref="Seven" /> .</summary>
		public static Feet Seven { get; } = new( 7 );

		/// <summary><see cref="Ten" /> .</summary>
		public static Feet Ten { get; } = new( 10 );

		/// <summary><see cref="Thirteen" /> .</summary>
		public static Feet Thirteen { get; } = new( 13 );

		/// <summary><see cref="Thirty" /> .</summary>
		public static Feet Thirty { get; } = new( 30 );

		/// <summary><see cref="Three" /> .</summary>
		public static Feet Three { get; } = new( 3 );

		/// <summary><see cref="Two" /> .</summary>
		public static Feet Two { get; } = new( 2 );

		/// <summary></summary>
		public static Feet Zero { get; } = new( 0 );

		[JsonProperty]
		public Rational Value { get; init; }

		[Pure]
		public override Int32 GetHashCode() => this.Value.GetHashCode();

		public Rational ToMeters() => this.Value / ( Rational )FeetPerMeter;

		[NotNull]
		public override String ToString() => $"{this.Value} {this.Value.PluralOf( "foot" )}";

		public static Feet Combine( Feet left, Rational feet ) => new( left.Value + feet );

		public static Feet Combine( Feet left, BigInteger seconds ) => new( left.Value + seconds );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Feet left, Feet right ) => left.Value == right.Value;

		public static Feet operator -( Feet feet ) => new( feet.Value * -1 );

		public static Feet operator -( Feet left, Feet right ) => Combine( left, -right.Value );

		public static Feet operator -( Feet left, Decimal seconds ) => Combine( left, ( Rational )( -seconds ) );

		public static Feet operator +( Feet left, Feet right ) => Combine( left, right.Value );

		public static Feet operator +( Feet left, Decimal seconds ) => Combine( left, ( Rational )seconds );

		public static Feet operator +( Feet left, BigInteger seconds ) => Combine( left, seconds );

		public static Boolean operator <( Feet left, Feet right ) => left.Value < right.Value;

		public static Boolean operator >( Feet left, Feet right ) => left.Value > right.Value;

		public Int32 CompareTo( Feet? other ) {
			if ( ReferenceEquals( this, other ) ) {
				return Order.Same;
			}

			if ( other is null ) {
				return Order.After;
			}

			return this.Value.CompareTo( other.Value );
		}
	}
}