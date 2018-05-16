// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Nanoseconds.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Nanoseconds.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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
    public struct Nanoseconds : IComparable<Nanoseconds>, IQuantityOfTime {

        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOneMicrosecond = 1000;

        /// <summary>
        ///     Ten <see cref="Nanoseconds" /> s.
        /// </summary>
        public static readonly Nanoseconds Fifteen = new Nanoseconds( 15 );

        /// <summary>
        ///     Five <see cref="Nanoseconds" /> s.
        /// </summary>
        public static readonly Nanoseconds Five = new Nanoseconds( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Nanoseconds" /> s.
        /// </summary>
        public static readonly Nanoseconds FiveHundred = new Nanoseconds( 500 );

        /// <summary>
        ///     One <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds One = new Nanoseconds( 1 );

        /// <summary>
        ///     One Thousand Nine <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static readonly Nanoseconds OneThousandNine = new Nanoseconds( 1009 );

        /// <summary>
        ///     Sixteen <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds Sixteen = new Nanoseconds( 16 );

        /// <summary>
        ///     Ten <see cref="Nanoseconds" /> s.
        /// </summary>
        public static readonly Nanoseconds Ten = new Nanoseconds( 10 );

        /// <summary>
        ///     Three <see cref="Nanoseconds" /> s.
        /// </summary>
        public static readonly Nanoseconds Three = new Nanoseconds( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds ThreeHundredThirtyThree = new Nanoseconds( 333 );

        /// <summary>
        ///     Two <see cref="Nanoseconds" /> s.
        /// </summary>
        public static readonly Nanoseconds Two = new Nanoseconds( 2 );

        /// <summary>
        ///     Two Hundred <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds TwoHundred = new Nanoseconds( 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static readonly Nanoseconds TwoHundredEleven = new Nanoseconds( 211 );

        /// <summary>
        ///     Two Thousand Three <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static readonly Nanoseconds TwoThousandThree = new Nanoseconds( 2003 );

        /// <summary>
        ///     Zero <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds Zero = new Nanoseconds( 0 );

        public Nanoseconds( Decimal value ) => this.Value = value;

        public Nanoseconds( BigRational value ) => this.Value = value;

        public Nanoseconds( Int64 value ) => this.Value = value;

        public Nanoseconds( BigInteger value ) => this.Value = value;

        [JsonProperty]
        public BigRational Value { get; }

        public static Nanoseconds Combine( Nanoseconds left, Nanoseconds right ) => Combine( left, right.Value );

        public static Nanoseconds Combine( Nanoseconds left, BigRational nanoseconds ) => new Nanoseconds( left.Value + nanoseconds );

        public static Nanoseconds Combine( Nanoseconds left, BigInteger nanoseconds ) => new Nanoseconds( left.Value + nanoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Nanoseconds left, Nanoseconds right ) => left.Value == right.Value;

        public static implicit operator Microseconds( Nanoseconds nanoseconds ) => nanoseconds.ToMicroseconds();

        public static implicit operator Picoseconds( Nanoseconds nanoseconds ) => nanoseconds.ToPicoseconds();

        public static implicit operator Span( Nanoseconds nanoseconds ) => new Span( nanoseconds: nanoseconds );

        public static Nanoseconds operator -( Nanoseconds nanoseconds ) => new Nanoseconds( nanoseconds.Value * -1 );

        public static Nanoseconds operator -( Nanoseconds left, Nanoseconds right ) => Combine( left, -right );

        public static Nanoseconds operator -( Nanoseconds left, Decimal nanoseconds ) => Combine( left, -nanoseconds );

        public static Boolean operator !=( Nanoseconds left, Nanoseconds right ) => !Equals( left, right );

        public static Nanoseconds operator +( Nanoseconds left, Nanoseconds right ) => Combine( left, right );

        public static Nanoseconds operator +( Nanoseconds left, Decimal nanoseconds ) => Combine( left, nanoseconds );

        public static Nanoseconds operator +( Nanoseconds left, BigInteger nanoseconds ) => Combine( left, nanoseconds );

        public static Boolean operator <( Nanoseconds left, Nanoseconds right ) => left.Value < right.Value;

        public static Boolean operator <( Nanoseconds left, Microseconds right ) => ( Microseconds )left < right;

        public static Boolean operator ==( Nanoseconds left, Nanoseconds right ) => Equals( left, right );

        public static Boolean operator >( Nanoseconds left, Nanoseconds right ) => left.Value > right.Value;

        public static Boolean operator >( Nanoseconds left, Microseconds right ) => ( Microseconds )left > right;

        public Int32 CompareTo( Nanoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Nanoseconds other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) { return false; }

            return obj is Nanoseconds nanoseconds && this.Equals( nanoseconds );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public Microseconds ToMicroseconds() => new Microseconds( this.Value / InOneMicrosecond );

        [Pure]
        public Picoseconds ToPicoseconds() => new Picoseconds( this.Value * Picoseconds.InOneNanosecond );

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneNanosecond * this.Value );

        [Pure]
        public override String ToString() {
            if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();

                return $"{whole} {whole.PluralOf( "ns" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "ns" )}";
        }
    }
}