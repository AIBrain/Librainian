// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Months.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Months.cs" was last formatted by Protiguous on 2018/06/04 at 4:15 PM.

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

	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Immutable]
	public struct Months : IComparable<Months>, IQuantityOfTime {

		[JsonProperty]
		public BigRational Value { get; }

		/// <summary>
		///     12
		/// </summary>
		public const Byte InOneCommonYear = 12;

		/// <summary>
		///     One <see cref="Months" /> .
		/// </summary>
		public static readonly Months One = new Months( 1 );

		/// <summary>
		/// </summary>
		public static readonly Months Ten = new Months( 10 );

		/// <summary>
		/// </summary>
		public static readonly Months Thousand = new Months( 1000 );

		/// <summary>
		///     Zero <see cref="Months" />
		/// </summary>
		public static readonly Months Zero = new Months( 0 );

		private Months( Int32 value ) => this.Value = value;

		public Months( Decimal value ) => this.Value = value;

		public Months( BigRational value ) => this.Value = value;

		public Months( BigInteger value ) => this.Value = value;

		public Months( Byte value ) => this.Value = value;

		public static Months Combine( Months left, Months right ) => Combine( left, right.Value );

		public static Months Combine( Months left, BigRational months ) => new Months( left.Value + months );

		public static Months Combine( Months left, BigInteger months ) => new Months( left.Value + months );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Months left, Months right ) => left.Value == right.Value;

		[NotNull]
		public static implicit operator SpanOfTime( Months months ) => new SpanOfTime( months: months.Value );

		public static implicit operator Weeks( Months months ) => months.ToWeeks();

		public static Months operator -( Months days ) => new Months( days.Value * -1 );

		public static Months operator -( Months left, Months right ) => Combine( left: left, right: -right );

		public static Months operator -( Months left, Decimal months ) => Combine( left, -months );

		public static Boolean operator !=( Months left, Months right ) => !Equals( left, right );

		public static Months operator +( Months left, Months right ) => Combine( left, right );

		public static Months operator +( Months left, BigRational months ) => Combine( left, months );

		public static Boolean operator <( Months left, Months right ) => left.Value < right.Value;

		public static Boolean operator ==( Months left, Months right ) => Equals( left, right );

		public static Boolean operator >( Months left, Months right ) => left.Value > right.Value;

		public Int32 CompareTo( Months other ) => this.Value.CompareTo( other.Value );

		public Boolean Equals( Months other ) => Equals( this, other );

		public override Boolean Equals( Object obj ) {
			if ( obj is null ) { return false; }

			return obj is Months months && this.Equals( months );
		}

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		[Pure]
		public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneMonth * this.Value );

		[Pure]
		public Seconds ToSeconds() => new Seconds( this.Value * Seconds.InOneMonth );

		[Pure]
		public override String ToString() {
			if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
				var whole = this.Value.GetWholePart();

				return $"{whole} {whole.PluralOf( "month" )}";
			}

			var dec = ( Decimal ) this.Value;

			return $"{dec} {dec.PluralOf( "month" )}";
		}

		//public static implicit operator Years( Months months ) => months.ToYears();

		[Pure]
		public Weeks ToWeeks() => new Weeks( this.Value * Weeks.InOneMonth );

	}

}