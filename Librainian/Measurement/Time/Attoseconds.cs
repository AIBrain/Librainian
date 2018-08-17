// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Attoseconds.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Attoseconds.cs" was last formatted by Protiguous on 2018/07/13 at 1:25 AM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Extensions;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;
	using Numerics;
	using Parsing;

	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	[Immutable]
	public struct Attoseconds : IComparable<Attoseconds>, IQuantityOfTime {

		/// <summary>
		///     1000
		/// </summary>
		/// <see cref="Femtoseconds" />
		public const UInt16 InOneFemtosecond = 1000;

		/// <summary>
		///     Ten <see cref="Attoseconds" /> s.
		/// </summary>
		public static Attoseconds Fifteen { get; } = new Attoseconds( 15 );

		/// <summary>
		///     Five <see cref="Attoseconds" /> s.
		/// </summary>
		public static Attoseconds Five { get; } = new Attoseconds( 5 );

		/// <summary>
		///     Five Hundred <see cref="Attoseconds" /> s.
		/// </summary>
		public static Attoseconds FiveHundred { get; } = new Attoseconds( 500 );

		/// <summary>
		///     111. 1 Hertz <see cref="Attoseconds" />.
		/// </summary>
		public static Attoseconds Hertz111 { get; } = new Attoseconds( 9 );

		/// <summary>
		///     One <see cref="Attoseconds" />.
		/// </summary>
		/// <remarks>the time it takes for light to travel the length of three hydrogen atoms</remarks>
		public static Attoseconds One { get; } = new Attoseconds( 1 );

		/// <summary>
		///     <see cref="OneHundred" /><see cref="Attoseconds" />.
		/// </summary>
		/// <remarks>fastest ever view of molecular motion</remarks>
		public static Attoseconds OneHundred { get; } = new Attoseconds( 100 );

		/// <summary>
		///     One Thousand Nine <see cref="Attoseconds" /> (Prime).
		/// </summary>
		public static Attoseconds OneThousandNine { get; } = new Attoseconds( 1009 );

		/// <summary>
		///     Sixteen <see cref="Attoseconds" />.
		/// </summary>
		public static Attoseconds Sixteen { get; } = new Attoseconds( 16 );

		/// <summary>
		///     <see cref="SixtySeven" /><see cref="Attoseconds" />.
		/// </summary>
		/// <remarks>the shortest pulses of laser light yet created</remarks>
		public static Attoseconds SixtySeven { get; } = new Attoseconds( 67 );

		/// <summary>
		///     Ten <see cref="Attoseconds" /> s.
		/// </summary>
		public static Attoseconds Ten { get; } = new Attoseconds( 10 );

		/// <summary>
		///     Three <see cref="Attoseconds" /> s.
		/// </summary>
		public static Attoseconds Three { get; } = new Attoseconds( 3 );

		/// <summary>
		///     Three Three Three <see cref="Attoseconds" />.
		/// </summary>
		public static Attoseconds ThreeHundredThirtyThree { get; } = new Attoseconds( 333 );

		/// <summary>
		///     <see cref="ThreeHundredTwenty" /><see cref="Attoseconds" />.
		/// </summary>
		/// <remarks>estimated time it takes electrons to transfer between atoms</remarks>
		public static Attoseconds ThreeHundredTwenty { get; } = new Attoseconds( 320 );

		/// <summary>
		///     <see cref="Twelve" /><see cref="Attoseconds" />.
		/// </summary>
		/// <remarks>record for shortest time interval measured as of 12 May 2010</remarks>
		public static Attoseconds Twelve { get; } = new Attoseconds( 12 );

		/// <summary>
		///     <see cref="TwentyFour" /><see cref="Attoseconds" />.
		/// </summary>
		/// <remarks>the atomic unit of time</remarks>
		public static Attoseconds TwentyFour { get; } = new Attoseconds( 24 );

		/// <summary>
		///     Two <see cref="Attoseconds" /> s.
		/// </summary>
		public static Attoseconds Two { get; } = new Attoseconds( 2 );

		/// <summary>
		///     <see cref="TwoHundred" /><see cref="Attoseconds" />.
		/// </summary>
		/// <remarks>
		///     (approximately) – half-life of beryllium-8, maximum time available for the triple-alpha process for the
		///     synthesis of carbon and heavier elements in stars
		/// </remarks>
		public static Attoseconds TwoHundred { get; } = new Attoseconds( 200 );

		/// <summary>
		///     Two Hundred Eleven <see cref="Attoseconds" /> (Prime).
		/// </summary>
		public static Attoseconds TwoHundredEleven { get; } = new Attoseconds( 211 );

		/// <summary>
		///     Two Thousand Three <see cref="Attoseconds" /> (Prime).
		/// </summary>
		public static Attoseconds TwoThousandThree { get; } = new Attoseconds( 2003 );

		/// <summary>
		///     Zero <see cref="Attoseconds" />.
		/// </summary>
		public static Attoseconds Zero { get; } = new Attoseconds( 0 );

		[JsonProperty]
		public BigRational Value { get; }

		public Attoseconds( Decimal value ) => this.Value = value;

		public Attoseconds( BigRational value ) => this.Value = value;

		public Attoseconds( Int64 value ) => this.Value = value;

		public Attoseconds( BigInteger value ) => this.Value = value;

		public static Attoseconds Combine( Attoseconds left, Attoseconds right ) => new Attoseconds( left.Value + right.Value );

		public static Attoseconds Combine( Attoseconds left, Decimal attoseconds ) => new Attoseconds( left.Value + attoseconds );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Attoseconds left, Attoseconds right ) => left.Value == right.Value;

		public static implicit operator Femtoseconds( Attoseconds attoseconds ) => attoseconds.ToFemtoseconds();

		public static implicit operator SpanOfTime( Attoseconds attoseconds ) => new SpanOfTime( planckTimes: attoseconds.ToPlanckTimes().Value );

		public static implicit operator Zeptoseconds( Attoseconds attoseconds ) => attoseconds.ToZeptoseconds();

		public static Attoseconds operator -( Attoseconds left, Decimal attoseconds ) => Combine( left, -attoseconds );

		public static Boolean operator !=( Attoseconds left, Attoseconds right ) => !Equals( left, right );

		public static Attoseconds operator +( Attoseconds left, Attoseconds right ) => Combine( left, right );

		public static Attoseconds operator +( Attoseconds left, Decimal attoseconds ) => Combine( left, attoseconds );

		public static Boolean operator <( Attoseconds left, Attoseconds right ) => left.Value < right.Value;

		public static Boolean operator ==( Attoseconds left, Attoseconds right ) => Equals( left, right );

		public static Boolean operator >( Attoseconds left, Attoseconds right ) => left.Value > right.Value;

		public Int32 CompareTo( Attoseconds other ) => this.Value.CompareTo( other.Value );

		public Boolean Equals( Attoseconds other ) => Equals( this, other );

		public override Boolean Equals( [CanBeNull] Object obj ) => obj is Attoseconds attoseconds && this.Equals( attoseconds );

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		/// <summary>
		///     Convert to a larger unit.
		/// </summary>
		/// <returns></returns>
		[Pure]
		public Femtoseconds ToFemtoseconds() => new Femtoseconds( this.Value / InOneFemtosecond );

		[Pure]
		public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneAttosecond * this.Value );

		public Seconds ToSeconds() => throw new NotImplementedException();

		[Pure]
		public override String ToString() {
			if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
				var whole = this.Value.GetWholePart();

				return $"{whole} {whole.PluralOf( "as" )}";
			}

			var dec = ( Decimal )this.Value;

			return $"{dec} {dec.PluralOf( "as" )}";
		}

		/// <summary>
		///     Convert to a smaller unit.
		/// </summary>
		/// <returns></returns>
		public Zeptoseconds ToZeptoseconds() => new Zeptoseconds( this.Value * Zeptoseconds.InOneAttosecond );
	}
}