// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Milliseconds.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Milliseconds.cs" was last formatted by Protiguous on 2020/03/16 at 2:57 PM.

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

    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public struct Milliseconds : IComparable<Milliseconds>, IQuantityOfTime {

        /// <summary>1000</summary>
        public const UInt16 InOneSecond = 1000;

        public static Milliseconds Default { get; } = new Milliseconds( value: default );

        /// <summary>Ten <see cref="Milliseconds" /> s.</summary>
        public static Milliseconds Fifteen { get; } = new Milliseconds( value: 15 );

        /// <summary>Five <see cref="Milliseconds" /> s.</summary>
        public static Milliseconds Five { get; } = new Milliseconds( value: 5 );

        /// <summary>Five Hundred <see cref="Milliseconds" /> s.</summary>
        public static Milliseconds FiveHundred { get; } = new Milliseconds( value: 500 );

        /// <summary>111. 1 Hertz (9 <see cref="Milliseconds" />).</summary>
        public static Milliseconds Hertz111 { get; } = new Milliseconds( value: 9 );

        /// <summary>97 <see cref="Milliseconds" /> s.</summary>
        public static Milliseconds NinetySeven { get; } = new Milliseconds( value: 97 );

        /// <summary>One <see cref="Milliseconds" />.</summary>
        public static Milliseconds One { get; } = new Milliseconds( value: 1 );

        /// <summary>One <see cref="Milliseconds" /> s.</summary>
        public static Milliseconds OneHundred { get; } = new Milliseconds( value: 100 );

        /// <summary>One Thousand Nine <see cref="Milliseconds" /> (Prime).</summary>
        public static Milliseconds OneThousandNine { get; } = new Milliseconds( value: 1009 );

        /// <summary>Sixteen <see cref="Milliseconds" />.</summary>
        public static Milliseconds Sixteen { get; } = new Milliseconds( value: 16 );

        /// <summary>Ten <see cref="Milliseconds" /> s.</summary>
        public static Milliseconds Ten { get; } = new Milliseconds( value: 10 );

        /// <summary>Three <see cref="Milliseconds" /> s.</summary>
        public static Milliseconds Three { get; } = new Milliseconds( value: 3 );

        /// <summary>Three Three Three <see cref="Milliseconds" />.</summary>
        public static Milliseconds ThreeHundredThirtyThree { get; } = new Milliseconds( value: 333 );

        /// <summary>Two <see cref="Milliseconds" /> s.</summary>
        public static Milliseconds Two { get; } = new Milliseconds( value: 2 );

        /// <summary>Two Hundred <see cref="Milliseconds" />.</summary>
        public static Milliseconds TwoHundred { get; } = new Milliseconds( value: 200 );

        /// <summary>Two Hundred Eleven <see cref="Milliseconds" /> (Prime).</summary>
        public static Milliseconds TwoHundredEleven { get; } = new Milliseconds( value: 211 );

        /// <summary>Two Thousand Three <see cref="Milliseconds" /> (Prime).</summary>
        public static Milliseconds TwoThousandThree { get; } = new Milliseconds( value: 2003 );

        //faster WPM than a female (~240wpm)
        /// <summary>Zero <see cref="Milliseconds" />.</summary>
        public static Milliseconds Zero { get; } = new Milliseconds( value: 0 );

        [JsonProperty]
        public Rational Value { get; }

        //faster WPM than a female (~240wpm)
        public Milliseconds( Decimal value ) => this.Value = ( Rational )value;

        public Milliseconds( Rational value ) => this.Value = value;

        public Milliseconds( Int64 value ) => this.Value = value;

        public Milliseconds( BigInteger value ) => this.Value = value;

        public Milliseconds( Double value ) => this.Value = ( Rational )value;

        public static Milliseconds Combine( Milliseconds left, Rational milliseconds ) => new Milliseconds( value: left.Value + milliseconds );

        public static Milliseconds Combine( Milliseconds left, BigInteger milliseconds ) => new Milliseconds( value: left.Value + milliseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Milliseconds left, Milliseconds right ) => left.Value == right.Value;

        public static explicit operator Double( Milliseconds milliseconds ) => ( Double )milliseconds.Value;

        /// <summary>Implicitly convert the number of <paramref name="milliseconds" /> to <see cref="Microseconds" />.</summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static implicit operator Microseconds( Milliseconds milliseconds ) => milliseconds.ToMicroseconds();

        public static implicit operator Rational( Milliseconds milliseconds ) => milliseconds.Value;

        public static implicit operator Seconds( Milliseconds milliseconds ) => milliseconds.ToSeconds();

        [NotNull]
        public static implicit operator SpanOfTime( Milliseconds milliseconds ) => new SpanOfTime( seconds: milliseconds );

        public static implicit operator TimeSpan( Milliseconds milliseconds ) => TimeSpan.FromMilliseconds( value: ( Double )milliseconds.Value );

        public static Milliseconds operator -( Milliseconds milliseconds ) => new Milliseconds( value: milliseconds.Value * -1 );

        public static Milliseconds operator -( Milliseconds left, Milliseconds right ) => Combine( left: left, milliseconds: -right.Value );

        public static Milliseconds operator -( Milliseconds left, Decimal milliseconds ) => Combine( left: left, milliseconds: ( Rational )( -milliseconds ) );

        public static Boolean operator !=( Milliseconds left, Milliseconds right ) => !Equals( left: left, right: right );

        public static Milliseconds operator +( Milliseconds left, Milliseconds right ) => Combine( left: left, milliseconds: right.Value );

        public static Milliseconds operator +( Milliseconds left, Decimal milliseconds ) => Combine( left: left, milliseconds: ( Rational )milliseconds );

        public static Milliseconds operator +( Milliseconds left, BigInteger milliseconds ) => Combine( left: left, milliseconds: milliseconds );

        public static Boolean operator <( Milliseconds left, Milliseconds right ) => left.Value < right.Value;

        public static Boolean operator <( Milliseconds left, Seconds right ) => ( Seconds )left < right;

        public static Boolean operator ==( Milliseconds left, Milliseconds right ) => Equals( left: left, right: right );

        public static Boolean operator >( Milliseconds left, Milliseconds right ) => left.Value > right.Value;

        [Pure]
        public static Boolean operator >( Milliseconds left, Seconds right ) => ( Seconds )left > right;

        public Int32 CompareTo( Milliseconds other ) => this.Value.CompareTo( other: other.Value );

        public Boolean Equals( Milliseconds other ) => Equals( left: this, right: other );

        public override Boolean Equals( [CanBeNull] Object obj ) => Equals( left: this, right: obj is Milliseconds milliseconds ? milliseconds : default );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Microseconds ToMicroseconds() => new Microseconds( value: this.Value * Microseconds.InOneMillisecond );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( value: ( Rational )PlanckTimes.InOneMillisecond * this.Value );

        public Seconds ToSeconds() => new Seconds( value: this.Value / InOneSecond );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( singular: "millisecond" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( singular: "millisecond" )}";
        }

        public TimeSpan ToTimeSpan() => this;
    }
}