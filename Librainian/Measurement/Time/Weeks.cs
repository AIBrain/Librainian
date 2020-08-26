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
// File "Weeks.cs" last formatted on 2020-08-14 at 8:38 PM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Maths;
	using Newtonsoft.Json;
	using Parsing;
	using Rationals;

	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	public struct Weeks : IComparable<Weeks>, IQuantityOfTime {

		/// <summary>52</summary>
		public const Decimal InOneCommonYear = 52m;

		/// <summary>4. 345</summary>
		public const Decimal InOneMonth = 4.345m;

		/// <summary>One <see cref="Weeks" /> .</summary>
		public static readonly Weeks One = new Weeks( 1 );

		/// <summary></summary>
		public static readonly Weeks Ten = new Weeks( 10 );

		/// <summary></summary>
		public static readonly Weeks Thousand = new Weeks( 1000 );

		/// <summary>Zero <see cref="Weeks" /></summary>
		public static readonly Weeks Zero = new Weeks( 0 );

		[JsonProperty]
		public Rational Value { get; }

		public Weeks( Decimal weeks ) => this.Value = ( Rational )weeks;

		public Weeks( Rational weeks ) => this.Value = weeks;

		public Weeks( Int64 value ) => this.Value = value;

		public Weeks( BigInteger value ) => this.Value = value;

		public static Weeks Combine( Weeks left, Weeks right ) => new Weeks( left.Value + right.Value );

		public static Weeks Combine( Weeks left, Rational weeks ) => new Weeks( left.Value + weeks );

		public static Weeks Combine( Weeks left, BigInteger weeks ) => new Weeks( left.Value + weeks );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Weeks left, Weeks right ) => left.Value == right.Value;

		/// <summary>Implicitly convert the number of <paramref name="weeks" /> to <see cref="Days" />.</summary>
		/// <param name="weeks"></param>
		/// <returns></returns>
		public static implicit operator Days( Weeks weeks ) => weeks.ToDays();

		public static implicit operator Months( Weeks weeks ) => weeks.ToMonths();

		public static implicit operator SpanOfTime( Weeks weeks ) => new SpanOfTime( weeks: weeks );

		public static Weeks operator -( Weeks days ) => new Weeks( days.Value * -1 );

		public static Weeks operator -( Weeks left, Weeks right ) => Combine( left, -right );

		public static Boolean operator !=( Weeks left, Weeks right ) => !Equals( left, right );

		public static Weeks operator +( Weeks left, Weeks right ) => Combine( left, right );

		public static Weeks operator +( Weeks left, Decimal weeks ) => Combine( left, ( Rational )weeks );

		public static Weeks operator +( Weeks left, BigInteger weeks ) => Combine( left, weeks );

		public static Boolean operator <( Weeks left, Weeks right ) => left.Value < right.Value;

		public static Boolean operator <( Weeks left, Days right ) => left < ( Weeks )right;

		public static Boolean operator <( Weeks left, Months right ) => ( Months )left < right;

		public static Boolean operator ==( Weeks left, Weeks right ) => Equals( left, right );

		public static Boolean operator >( Weeks left, Months right ) => ( Months )left > right;

		public static Boolean operator >( Weeks left, Days right ) => left > ( Weeks )right;

		public static Boolean operator >( Weeks left, Weeks right ) => left.Value > right.Value;

		public Int32 CompareTo( Weeks other ) => this.Value.CompareTo( other.Value );

		public Boolean Equals( Weeks other ) => Equals( this, other );

		public override Boolean Equals( Object obj ) {
			if ( obj is null ) {
				return default;
			}

			return obj is Weeks weeks && this.Equals( weeks );
		}

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		public Days ToDays() => new Days( this.Value * Days.InOneWeek );

		public Months ToMonths() => new Months( this.Value / ( Rational )InOneMonth );

		public PlanckTimes ToPlanckTimes() => new PlanckTimes( this.Value * ( Rational )PlanckTimes.InOneWeek );

		public Seconds ToSeconds() => new Seconds( this.Value * Seconds.InOneWeek );

		public override String ToString() {
			if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
				var whole = this.Value.WholePart;

				return $"{whole} {whole.PluralOf( "week" )}";
			}

			var dec = ( Decimal )this.Value;

			return $"{dec} {dec.PluralOf( "week" )}";
		}

		public TimeSpan ToTimeSpan() => this.ToSeconds();

	}

}