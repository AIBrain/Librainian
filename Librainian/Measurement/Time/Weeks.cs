// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Weeks.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Weeks.cs" was last formatted by Protiguous on 2018/07/13 at 1:31 AM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;
	using Numerics;
	using Parsing;

	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	public struct Weeks : IComparable<Weeks>, IQuantityOfTime {

		/// <summary>
		///     52
		/// </summary>
		public const Decimal InOneCommonYear = 52m;

		/// <summary>
		///     4. 345
		/// </summary>
		public const Decimal InOneMonth = 4.345m;

		/// <summary>
		///     One <see cref="Weeks" /> .
		/// </summary>
		public static readonly Weeks One = new Weeks( 1 );

		/// <summary>
		/// </summary>
		public static readonly Weeks Ten = new Weeks( 10 );

		/// <summary>
		/// </summary>
		public static readonly Weeks Thousand = new Weeks( 1000 );

		/// <summary>
		///     Zero <see cref="Weeks" />
		/// </summary>
		public static readonly Weeks Zero = new Weeks( 0 );

		[JsonProperty]
		public BigRational Value { get; }

		public Weeks( Decimal weeks ) => this.Value = weeks;

		public Weeks( BigRational weeks ) => this.Value = weeks;

		public Weeks( Int64 value ) => this.Value = value;

		public Weeks( BigInteger value ) => this.Value = value;

		public static Weeks Combine( Weeks left, Weeks right ) => new Weeks( left.Value + right.Value );

		public static Weeks Combine( Weeks left, BigRational weeks ) => new Weeks( left.Value + weeks );

		public static Weeks Combine( Weeks left, BigInteger weeks ) => new Weeks( left.Value + weeks );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Weeks left, Weeks right ) => left.Value == right.Value;

		/// <summary>
		///     Implicitly convert the number of <paramref name="weeks" /> to <see cref="Days" />.
		/// </summary>
		/// <param name="weeks"></param>
		/// <returns></returns>
		public static implicit operator Days( Weeks weeks ) => weeks.ToDays();

		public static implicit operator Months( Weeks weeks ) => weeks.ToMonths();

		[NotNull]
		public static implicit operator SpanOfTime( Weeks weeks ) => new SpanOfTime( weeks: weeks.Value );

		public static Weeks operator -( Weeks days ) => new Weeks( days.Value * -1 );

		public static Weeks operator -( Weeks left, Weeks right ) => Combine( left: left, right: -right );

		public static Boolean operator !=( Weeks left, Weeks right ) => !Equals( left, right );

		public static Weeks operator +( Weeks left, Weeks right ) => Combine( left, right );

		public static Weeks operator +( Weeks left, Decimal weeks ) => Combine( left, weeks );

		public static Weeks operator +( Weeks left, BigInteger weeks ) => Combine( left, weeks );

		public static Boolean operator <( Weeks left, Weeks right ) => left.Value < right.Value;

		public static Boolean operator <( Weeks left, Days right ) => left < ( Weeks ) right;

		public static Boolean operator <( Weeks left, Months right ) => ( Months ) left < right;

		public static Boolean operator ==( Weeks left, Weeks right ) => Equals( left, right );

		public static Boolean operator >( Weeks left, Months right ) => ( Months ) left > right;

		public static Boolean operator >( Weeks left, Days right ) => left > ( Weeks ) right;

		public static Boolean operator >( Weeks left, Weeks right ) => left.Value > right.Value;

		public Int32 CompareTo( Weeks other ) => this.Value.CompareTo( other.Value );

		public Boolean Equals( Weeks other ) => Equals( this, other );

		public override Boolean Equals( Object obj ) {
			if ( obj is null ) { return false; }

			return obj is Weeks weeks && this.Equals( weeks );
		}

		[Pure]
		public override Int32 GetHashCode() => this.Value.GetHashCode();

		[Pure]
		public Days ToDays() => new Days( this.Value * Days.InOneWeek );

		[Pure]
		public Months ToMonths() => new Months( this.Value / InOneMonth );

		[Pure]
		public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneWeek * this.Value );

		[Pure]
		public override String ToString() {
			if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
				var whole = this.Value.GetWholePart();

				return $"{whole} {whole.PluralOf( "week" )}";
			}

			var dec = ( Decimal ) this.Value;

			return $"{dec} {dec.PluralOf( "week" )}";
		}
	}
}