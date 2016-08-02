// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Nanoseconds.cs" was last cleaned by Rick on 2016/06/18 at 10:54 PM

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

    [DebuggerDisplay( "{ToString(),nq}" )]
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
        public static readonly Nanoseconds Fifteen = new Nanoseconds( value: 15 );

        /// <summary>
        ///     Five <see cref="Nanoseconds" /> s.
        /// </summary>
        public static readonly Nanoseconds Five = new Nanoseconds( value: 5 );

        /// <summary>
        ///     Five Hundred <see cref="Nanoseconds" /> s.
        /// </summary>
        public static readonly Nanoseconds FiveHundred = new Nanoseconds( value: 500 );

        /// <summary>
        ///     One <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds One = new Nanoseconds( value: 1 );

        /// <summary>
        ///     One Thousand Nine <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static readonly Nanoseconds OneThousandNine = new Nanoseconds( value: 1009 );

        /// <summary>
        ///     Sixteen <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds Sixteen = new Nanoseconds( value: 16 );

        /// <summary>
        ///     Ten <see cref="Nanoseconds" /> s.
        /// </summary>
        public static readonly Nanoseconds Ten = new Nanoseconds( value: 10 );

        /// <summary>
        ///     Three <see cref="Nanoseconds" /> s.
        /// </summary>
        public static readonly Nanoseconds Three = new Nanoseconds( value: 3 );

        /// <summary>
        ///     Three Three Three <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds ThreeHundredThirtyThree = new Nanoseconds( value: 333 );

        /// <summary>
        ///     Two <see cref="Nanoseconds" /> s.
        /// </summary>
        public static readonly Nanoseconds Two = new Nanoseconds( value: 2 );

        /// <summary>
        ///     Two Hundred <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds TwoHundred = new Nanoseconds( value: 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static readonly Nanoseconds TwoHundredEleven = new Nanoseconds( value: 211 );

        /// <summary>
        ///     Two Thousand Three <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static readonly Nanoseconds TwoThousandThree = new Nanoseconds( value: 2003 );

        /// <summary>
        ///     Zero <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds Zero = new Nanoseconds( value: 0 );

        public Nanoseconds( Decimal value ) {
            this.Value = value;
        }

        public Nanoseconds( BigRational value ) {
            this.Value = value;
        }

        public Nanoseconds( Int64 value ) {
            this.Value = value;
        }

        public Nanoseconds( BigInteger value ) {
            this.Value = value;
        }

        [JsonProperty]
        public BigRational Value {
            get;
        }

        public static Nanoseconds Combine( Nanoseconds left, Nanoseconds right ) => Combine( left, right.Value );

        public static Nanoseconds Combine( Nanoseconds left, BigRational nanoseconds ) => new Nanoseconds( left.Value + nanoseconds );

        public static Nanoseconds Combine( Nanoseconds left, BigInteger nanoseconds ) => new Nanoseconds( left.Value + nanoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
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
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Nanoseconds && this.Equals( ( Nanoseconds )obj );
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
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();
                return $"{whole} {whole.PluralOf( "ns" )}";
            }
            var dec = ( Decimal )this.Value;
            return $"{dec} {dec.PluralOf( "ns" )}";
        }
    }
}