// Copyright � 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Months.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Months.cs" was last formatted by Protiguous on 2020/03/16 at 2:57 PM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using Rationals;

    [JsonObject]
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct Months : IComparable<Months>, IQuantityOfTime {

        /// <summary>12</summary>
        public const Byte InOneCommonYear = 12;

        /// <summary>One <see cref="Months" /> .</summary>
        public static readonly Months One = new Months( value: 1 );

        /// <summary></summary>
        public static readonly Months Ten = new Months( value: 10 );

        /// <summary></summary>
        public static readonly Months Thousand = new Months( value: 1000 );

        /// <summary>Zero <see cref="Months" /></summary>
        public static readonly Months Zero = new Months( value: 0 );

        [JsonProperty]
        public Rational Value { get; }

        private Months( Int32 value ) => this.Value = value;

        public Months( Decimal value ) => this.Value = ( Rational )value;

        public Months( Rational value ) => this.Value = value;

        public Months( BigInteger value ) => this.Value = value;

        public Months( Byte value ) => this.Value = value;

        public static Months Combine( Months left, Months right ) => Combine( left: left, months: right.Value );

        public static Months Combine( Months left, Rational months ) => new Months( value: left.Value + months );

        public static Months Combine( Months left, BigInteger months ) => new Months( value: left.Value + months );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Months left, Months right ) => left.Value == right.Value;

        [NotNull]
        public static implicit operator SpanOfTime( Months months ) => new SpanOfTime( months: months );

        public static implicit operator Weeks( Months months ) => months.ToWeeks();

        public static Months operator -( Months days ) => new Months( value: days.Value * -1 );

        public static Months operator -( Months left, Months right ) => Combine( left: left, right: -right );

        public static Months operator -( Months left, Decimal months ) => Combine( left: left, months: ( Rational )( -months ) );

        public static Boolean operator !=( Months left, Months right ) => !Equals( left: left, right: right );

        public static Months operator +( Months left, Months right ) => Combine( left: left, right: right );

        public static Months operator +( Months left, Rational months ) => Combine( left: left, months: months );

        public static Boolean operator <( Months left, Months right ) => left.Value < right.Value;

        public static Boolean operator ==( Months left, Months right ) => Equals( left: left, right: right );

        public static Boolean operator >( Months left, Months right ) => left.Value > right.Value;

        public Int32 CompareTo( Months other ) => this.Value.CompareTo( other: other.Value );

        public Boolean Equals( Months other ) => Equals( left: this, right: other );

        public override Boolean Equals( Object obj ) => obj is Months months && Equals( left: this, right: months );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( value: ( Rational )PlanckTimes.InOneMonth * this.Value );

        public Seconds ToSeconds() => new Seconds( value: this.Value * Seconds.InOneMonth );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( singular: "month" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( singular: "month" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

        //public static implicit operator Years( Months months ) => months.ToYears();

        public Weeks ToWeeks() => new Weeks( weeks: this.Value * ( Rational )Weeks.InOneMonth );
    }
}