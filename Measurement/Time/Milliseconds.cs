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
// "Librainian/Milliseconds.cs" was last cleaned by Rick on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Numerics;
    using Extensions;
    using Maths;
    using Newtonsoft.Json;
    using Numerics;
    using Parsing;

    [DebuggerDisplay( "{ToString(),nq}" )]
    [JsonObject]
    [Immutable]
    public struct Milliseconds : IComparable<Milliseconds>, IQuantityOfTime {

        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOneSecond = 1000;

        /// <summary>
        ///     Ten <see cref="Milliseconds" /> s.
        /// </summary>
        public static readonly Milliseconds Fifteen = new Milliseconds( 15 );

        /// <summary>
        ///     Five <see cref="Milliseconds" /> s.
        /// </summary>
        public static readonly Milliseconds Five = new Milliseconds( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Milliseconds" /> s.
        /// </summary>
        public static readonly Milliseconds FiveHundred = new Milliseconds( 500 );

        /// <summary>
        ///     111. 1 Hertz (9 <see cref="Milliseconds" />).
        /// </summary>
        public static readonly Milliseconds Hertz111 = new Milliseconds( 9 );

        /// <summary>
        ///     97 <see cref="Milliseconds" /> s.
        /// </summary>
        public static readonly Milliseconds NinetySeven = new Milliseconds( 97 );

        /// <summary>
        ///     One <see cref="Milliseconds" />.
        /// </summary>
        public static readonly Milliseconds One = new Milliseconds( 1 );

        /// <summary>
        ///     One <see cref="Milliseconds" /> s.
        /// </summary>
        public static readonly Milliseconds OneHundred = new Milliseconds( 100 );

        /// <summary>
        ///     One Thousand Nine <see cref="Milliseconds" /> (Prime).
        /// </summary>
        public static readonly Milliseconds OneThousandNine = new Milliseconds( 1009 );

        /// <summary>
        ///     Sixteen <see cref="Milliseconds" />.
        /// </summary>
        public static readonly Milliseconds Sixteen = new Milliseconds( 16 );

        /// <summary>
        ///     Ten <see cref="Milliseconds" /> s.
        /// </summary>
        public static readonly Milliseconds Ten = new Milliseconds( 10 );

        /// <summary>
        ///     Three <see cref="Milliseconds" /> s.
        /// </summary>
        public static readonly Milliseconds Three = new Milliseconds( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Milliseconds" />.
        /// </summary>
        public static readonly Milliseconds ThreeHundredThirtyThree = new Milliseconds( 333 );

        /// <summary>
        ///     Two <see cref="Milliseconds" /> s.
        /// </summary>
        public static readonly Milliseconds Two = new Milliseconds( 2 );

        /// <summary>
        ///     Two Hundred <see cref="Milliseconds" />.
        /// </summary>
        public static readonly Milliseconds TwoHundred = new Milliseconds( 200 ); //faster WPM than a female (~240wpm)

        /// <summary>
        ///     Two Hundred Eleven <see cref="Milliseconds" /> (Prime).
        /// </summary>
        public static readonly Milliseconds TwoHundredEleven = new Milliseconds( 211 ); //faster WPM than a female (~240wpm)

        /// <summary>
        ///     Two Thousand Three <see cref="Milliseconds" /> (Prime).
        /// </summary>
        public static readonly Milliseconds TwoThousandThree = new Milliseconds( 2003 );

        /// <summary>
        ///     Zero <see cref="Milliseconds" />.
        /// </summary>
        public static readonly Milliseconds Zero = new Milliseconds( 0 );

        public Milliseconds( Decimal value ) {
            this.Value = value;
        }

        public Milliseconds( BigRational value ) {
            this.Value = value;
        }

        public Milliseconds( Int64 value ) {
            this.Value = value;
        }

        public Milliseconds( BigInteger value ) {
            this.Value = value;
        }

        public Milliseconds( Double value ) {
            this.Value = value;
        }

        [JsonProperty]
        public BigRational Value {
            get;
        }

        public static Milliseconds Combine( Milliseconds left, BigRational milliseconds ) => new Milliseconds( left.Value + milliseconds );

        public static Milliseconds Combine( Milliseconds left, BigInteger milliseconds ) => new Milliseconds( ( BigInteger )left.Value + milliseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Milliseconds left, Milliseconds right ) => left.Value == right.Value;

        public static explicit operator Double( Milliseconds milliseconds ) => ( Double )milliseconds.Value;

        public static implicit operator BigRational( Milliseconds milliseconds ) => milliseconds.Value;

        /// <summary>
        ///     Implicitly convert the number of <paramref name="milliseconds" /> to <see cref="Microseconds" />.
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static implicit operator Microseconds( Milliseconds milliseconds ) => milliseconds.ToMicroseconds();

        public static implicit operator Seconds( Milliseconds milliseconds ) => milliseconds.ToSeconds();

        public static implicit operator Span( Milliseconds milliseconds ) => new Span( milliseconds: milliseconds );

        public static implicit operator TimeSpan( Milliseconds milliseconds ) => TimeSpan.FromMilliseconds( value: ( Double )milliseconds.Value );

        public static Milliseconds operator -( Milliseconds milliseconds ) => new Milliseconds( milliseconds.Value * -1 );

        public static Milliseconds operator -( Milliseconds left, Milliseconds right ) => Combine( left, -right.Value );

        public static Milliseconds operator -( Milliseconds left, Decimal milliseconds ) => Combine( left, -milliseconds );

        public static Boolean operator !=( Milliseconds left, Milliseconds right ) => !Equals( left, right );

        public static Milliseconds operator +( Milliseconds left, Milliseconds right ) => Combine( left, right.Value );

        public static Milliseconds operator +( Milliseconds left, Decimal milliseconds ) => Combine( left, milliseconds );

        public static Milliseconds operator +( Milliseconds left, BigInteger milliseconds ) => Combine( left, milliseconds );

        public static Boolean operator <( Milliseconds left, Milliseconds right ) => left.Value < right.Value;

        public static Boolean operator <( Milliseconds left, Seconds right ) => ( Seconds )left < right;

        public static Boolean operator ==( Milliseconds left, Milliseconds right ) => Equals( left, right );

        public static Boolean operator >( Milliseconds left, Milliseconds right ) => left.Value > right.Value;

        public static Boolean operator >( Milliseconds left, Seconds right ) => ( Seconds )left > right;

        public Int32 CompareTo( Milliseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Milliseconds other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Milliseconds && this.Equals( ( Milliseconds )obj );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Microseconds ToMicroseconds() => new Microseconds( this.Value * Microseconds.InOneMillisecond );

        [Pure]
        public PlanckTimes ToPlanckTimes() {
            return new PlanckTimes( PlanckTimes.InOneMillisecond * this.Value );
        }

        [Pure]
        public Seconds ToSeconds() => new Seconds( this.Value / InOneSecond );

        [Pure]
        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();
                return $"{whole} {whole.PluralOf( "millisecond" )}";
            }
            var dec = ( Decimal )this.Value;
            return $"{dec} {dec.PluralOf( "millisecond" )}";
        }
    }
}