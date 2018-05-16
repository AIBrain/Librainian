// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Microseconds.cs",
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
// "Librainian/Librainian/Microseconds.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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
    public struct Microseconds : IComparable<Microseconds>, IQuantityOfTime {

        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOneMillisecond = 1000;

        /// <summary>
        ///     Ten <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds Fifteen = new Microseconds( 15 );

        /// <summary>
        ///     Five <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds Five = new Microseconds( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds FiveHundred = new Microseconds( 500 );

        /// <summary>
        ///     One <see cref="Microseconds" />.
        /// </summary>
        public static readonly Microseconds One = new Microseconds( 1 );

        /// <summary>
        ///     One Thousand Nine <see cref="Microseconds" /> (Prime).
        /// </summary>
        public static readonly Microseconds OneThousandNine = new Microseconds( 1009 );

        /// <summary>
        ///     Sixteen <see cref="Microseconds" />.
        /// </summary>
        public static readonly Microseconds Sixteen = new Microseconds( 16 );

        /// <summary>
        ///     Ten <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds Ten = new Microseconds( 10 );

        /// <summary>
        ///     Three <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds Three = new Microseconds( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Microseconds" />.
        /// </summary>
        public static readonly Microseconds ThreeHundredThirtyThree = new Microseconds( 333 );

        /// <summary>
        ///     Two <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds Two = new Microseconds( 2 );

        /// <summary>
        ///     Two Hundred <see cref="Microseconds" />.
        /// </summary>
        public static readonly Microseconds TwoHundred = new Microseconds( 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="Microseconds" /> (Prime).
        /// </summary>
        public static readonly Microseconds TwoHundredEleven = new Microseconds( 211 );

        /// <summary>
        ///     Two Thousand Three <see cref="Microseconds" /> (Prime).
        /// </summary>
        public static readonly Microseconds TwoThousandThree = new Microseconds( 2003 );

        /// <summary>
        ///     Zero <see cref="Microseconds" />.
        /// </summary>
        public static readonly Microseconds Zero = new Microseconds( 0 );

        public Microseconds( Decimal value ) => this.Value = value;

        public Microseconds( BigRational value ) => this.Value = value;

        public Microseconds( Int64 value ) => this.Value = value;

        public Microseconds( BigInteger value ) => this.Value = value;

        [JsonProperty]
        public BigRational Value { get; }

        public static Microseconds Combine( Microseconds left, Microseconds right ) => Combine( left, right.Value );

        public static Microseconds Combine( Microseconds left, BigRational microseconds ) => new Microseconds( left.Value + microseconds );

        public static Microseconds Combine( Microseconds left, BigInteger microseconds ) => new Microseconds( left.Value + microseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Microseconds left, Microseconds right ) => left.Value == right.Value;

        public static implicit operator Milliseconds( Microseconds microseconds ) => microseconds.ToMilliseconds();

        public static implicit operator Nanoseconds( Microseconds microseconds ) => microseconds.ToNanoseconds();

        public static implicit operator TimeSpan( Microseconds microseconds ) => TimeSpan.FromMilliseconds( ( Double )microseconds.Value );

        public static Microseconds operator -( Microseconds milliseconds ) => new Microseconds( milliseconds.Value * -1 );

        public static Microseconds operator -( Microseconds left, Microseconds right ) => Combine( left, -right );

        public static Microseconds operator -( Microseconds left, Decimal microseconds ) => Combine( left, -microseconds );

        public static Boolean operator !=( Microseconds left, Microseconds right ) => !Equals( left, right );

        public static Microseconds operator +( Microseconds left, Microseconds right ) => Combine( left, right );

        public static Microseconds operator +( Microseconds left, Decimal microseconds ) => Combine( left, microseconds );

        public static Microseconds operator +( Microseconds left, BigInteger microseconds ) => Combine( left, microseconds );

        public static Boolean operator <( Microseconds left, Microseconds right ) => left.Value < right.Value;

        public static Boolean operator <( Microseconds left, Milliseconds right ) => ( Milliseconds )left < right;

        public static Boolean operator ==( Microseconds left, Microseconds right ) => Equals( left, right );

        public static Boolean operator >( Microseconds left, Microseconds right ) => left.Value > right.Value;

        public static Boolean operator >( Microseconds left, Milliseconds right ) => ( Milliseconds )left > right;

        public Int32 CompareTo( Microseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Microseconds other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) { return false; }

            return obj is Microseconds microseconds && this.Equals( microseconds );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public Milliseconds ToMilliseconds() => new Milliseconds( this.Value / InOneMillisecond );

        [Pure]
        public Nanoseconds ToNanoseconds() => new Nanoseconds( this.Value * Nanoseconds.InOneMicrosecond );

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneMicrosecond * this.Value );

        [Pure]
        public override String ToString() {
            if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();

                return $"{whole} {whole.PluralOf( "µs" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "µs" )}";
        }
    }
}