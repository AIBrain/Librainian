// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Nanoseconds.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Nanoseconds.cs" was last formatted by Protiguous on 2019/08/08 at 9:07 AM.

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

    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public class Nanoseconds : IComparable<Nanoseconds>, IQuantityOfTime {

        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOneMicrosecond = 1000;

        /// <summary>
        ///     Ten <see cref="Nanoseconds" /> s.
        /// </summary>
        public static Nanoseconds Fifteen = new Nanoseconds( 15 );

        /// <summary>
        ///     Five <see cref="Nanoseconds" /> s.
        /// </summary>
        public static Nanoseconds Five = new Nanoseconds( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Nanoseconds" /> s.
        /// </summary>
        public static Nanoseconds FiveHundred = new Nanoseconds( 500 );

        /// <summary>
        ///     One <see cref="Nanoseconds" />.
        /// </summary>
        public static Nanoseconds One = new Nanoseconds( 1 );

        /// <summary>
        ///     One Thousand Nine <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static Nanoseconds OneThousandNine = new Nanoseconds( 1009 );

        /// <summary>
        ///     Sixteen <see cref="Nanoseconds" />.
        /// </summary>
        public static Nanoseconds Sixteen = new Nanoseconds( 16 );

        /// <summary>
        ///     Ten <see cref="Nanoseconds" /> s.
        /// </summary>
        public static Nanoseconds Ten = new Nanoseconds( 10 );

        /// <summary>
        ///     Three <see cref="Nanoseconds" /> s.
        /// </summary>
        public static Nanoseconds Three = new Nanoseconds( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Nanoseconds" />.
        /// </summary>
        public static Nanoseconds ThreeHundredThirtyThree = new Nanoseconds( 333 );

        /// <summary>
        ///     Two <see cref="Nanoseconds" /> s.
        /// </summary>
        public static Nanoseconds Two = new Nanoseconds( 2 );

        /// <summary>
        ///     Two Hundred <see cref="Nanoseconds" />.
        /// </summary>
        public static Nanoseconds TwoHundred = new Nanoseconds( 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static Nanoseconds TwoHundredEleven = new Nanoseconds( 211 );

        /// <summary>
        ///     Two Thousand Three <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static Nanoseconds TwoThousandThree = new Nanoseconds( 2003 );

        /// <summary>
        ///     Zero <see cref="Nanoseconds" />.
        /// </summary>
        public static Nanoseconds Zero = new Nanoseconds( 0 );

        [JsonProperty]
        public Rational Value { get; }

        public Nanoseconds( Decimal value ) => this.Value = ( Rational )value;

        public Nanoseconds( Rational value ) => this.Value = value;

        public Nanoseconds( Int64 value ) => this.Value = value;

        public Nanoseconds( BigInteger value ) => this.Value = value;

        [CanBeNull]
        public static Nanoseconds Combine( [CanBeNull] Nanoseconds left, Nanoseconds right ) => Combine( left, right.Value );

        [NotNull]
        public static Nanoseconds Combine( Nanoseconds left, Rational nanoseconds ) => new Nanoseconds( left.Value + nanoseconds );

        [NotNull]
        public static Nanoseconds Combine( Nanoseconds left, BigInteger nanoseconds ) => new Nanoseconds( left.Value + nanoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Nanoseconds left, Nanoseconds right ) => left.Value == right.Value;

        [CanBeNull]
        public static implicit operator Microseconds( Nanoseconds nanoseconds ) => nanoseconds.ToMicroseconds();

        [CanBeNull]
        public static implicit operator Picoseconds( Nanoseconds nanoseconds ) => nanoseconds.ToPicoseconds();

        [NotNull]
        public static implicit operator SpanOfTime( [CanBeNull] Nanoseconds nanoseconds ) => new SpanOfTime( nanoseconds: nanoseconds );

        [NotNull]
        public static Nanoseconds operator -( Nanoseconds nanoseconds ) => new Nanoseconds( nanoseconds.Value * -1 );

        [CanBeNull]
        public static Nanoseconds operator -( [CanBeNull] Nanoseconds left, [CanBeNull] Nanoseconds right ) => Combine( left, -right );

        [NotNull]
        public static Nanoseconds operator -( [CanBeNull] Nanoseconds left, Decimal nanoseconds ) => Combine( left, ( Rational )( -nanoseconds ) );

        public static Boolean operator !=( [CanBeNull] Nanoseconds left, [CanBeNull] Nanoseconds right ) => !Equals( left, right );

        [CanBeNull]
        public static Nanoseconds operator +( [CanBeNull] Nanoseconds left, [CanBeNull] Nanoseconds right ) => Combine( left, right );

        [NotNull]
        public static Nanoseconds operator +( [CanBeNull] Nanoseconds left, Decimal nanoseconds ) => Combine( left, ( Rational )nanoseconds );

        [NotNull]
        public static Nanoseconds operator +( [CanBeNull] Nanoseconds left, BigInteger nanoseconds ) => Combine( left, nanoseconds );

        public static Boolean operator <( Nanoseconds left, Nanoseconds right ) => left.Value < right.Value;

        public static Boolean operator <( [CanBeNull] Nanoseconds left, [CanBeNull] Microseconds right ) => ( Microseconds )left < right;

        public static Boolean operator ==( [CanBeNull] Nanoseconds left, [CanBeNull] Nanoseconds right ) => Equals( left, right );

        public static Boolean operator >( Nanoseconds left, Nanoseconds right ) => left.Value > right.Value;

        public static Boolean operator >( [CanBeNull] Nanoseconds left, [CanBeNull] Microseconds right ) => ( Microseconds )left > right;

        public Int32 CompareTo( Nanoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( [CanBeNull] Nanoseconds other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }

            return obj is Nanoseconds nanoseconds && this.Equals( nanoseconds );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [NotNull]
        public Microseconds ToMicroseconds() => new Microseconds( this.Value / InOneMicrosecond );

        [NotNull]
        public Picoseconds ToPicoseconds() => new Picoseconds( this.Value * Picoseconds.InOneNanosecond );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( ( Rational )PlanckTimes.InOneNanosecond * this.Value );

        public Seconds ToSeconds() => throw new NotImplementedException();

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "ns" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "ns" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();
    }
}