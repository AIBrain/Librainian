#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/Milliseconds.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using Librainian.Extensions;
    using Parsing;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    [Immutable]
    public struct Milliseconds : IComparable< Milliseconds >, IQuantityOfTime {
        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOneSecond = 1000;

        /// <summary>
        ///     Ten <see cref="Milliseconds" />s.
        /// </summary>
        public static readonly Milliseconds Fifteen = new Milliseconds( 15 );

        /// <summary>
        ///     Five <see cref="Milliseconds" />s.
        /// </summary>
        public static readonly Milliseconds Five = new Milliseconds( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Milliseconds" />s.
        /// </summary>
        public static readonly Milliseconds FiveHundred = new Milliseconds( 500 );

        /// <summary>
        ///     111.1 Hertz <see cref="Milliseconds" />.
        /// </summary>
        public static readonly Milliseconds Hertz111 = new Milliseconds( 9 );

        /// <summary>
        ///     One <see cref="Milliseconds" />.
        /// </summary>
        public static readonly Milliseconds One = new Milliseconds( 1 );

        /// <summary>
        ///     One Thousand Nine <see cref="Milliseconds" /> (Prime).
        /// </summary>
        public static readonly Milliseconds OneThousandNine = new Milliseconds( 1009 );

        /// <summary>
        ///     Sixteen <see cref="Milliseconds" />.
        /// </summary>
        public static readonly Milliseconds Sixteen = new Milliseconds( 16 );

        /// <summary>
        ///     Ten <see cref="Milliseconds" />s.
        /// </summary>
        public static readonly Milliseconds Ten = new Milliseconds( 10 );

        /// <summary>
        ///     Three <see cref="Milliseconds" />s.
        /// </summary>
        public static readonly Milliseconds Three = new Milliseconds( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Milliseconds" />.
        /// </summary>
        public static readonly Milliseconds ThreeHundredThirtyThree = new Milliseconds( 333 );

        /// <summary>
        ///     Two <see cref="Milliseconds" />s.
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

        [DataMember] public readonly Decimal Value;

        static Milliseconds() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Seconds.One );
            One.Should().BeGreaterThan( Microseconds.One );
        }

        public Milliseconds( Decimal value ) {
            this.Value = value;
        }

        public Milliseconds( long value ) {
            this.Value = value;
        }

        public Milliseconds( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal ) value;
        }

        public Milliseconds( Double value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal ) value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( Milliseconds other ) {
            return this.Value.CompareTo( other.Value );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneMillisecond, new BigInteger( this.Value ) );
        }

        public override string ToString() {
            return String.Format( "{0} {1}", this.Value, this.Value.PluralOf( "millisecond" ) );
        }

        public static Milliseconds Combine( Milliseconds left, Decimal milliseconds ) {
            return new Milliseconds( left.Value + milliseconds );
        }

        public static Milliseconds Combine( Milliseconds left, BigInteger milliseconds ) {
            return new Milliseconds( ( BigInteger ) left.Value + milliseconds );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Milliseconds left, Milliseconds right ) {
            return left.Value == right.Value;
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="milliseconds" /> to <see cref="Microseconds" />.
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static implicit operator Microseconds( Milliseconds milliseconds ) {
            return milliseconds.ToMicroseconds();
        }

        public static implicit operator Seconds( Milliseconds milliseconds ) {
            return milliseconds.ToSeconds();
        }

        public static implicit operator Span( Milliseconds milliseconds ) {
            return new Span( milliseconds: milliseconds );
        }

        public static implicit operator TimeSpan( Milliseconds milliseconds ) {
            return TimeSpan.FromMilliseconds( value: ( Double ) milliseconds.Value );
        }

        /// <summary>
        /// I don't prefer implicits to Double.. oh well.
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static explicit operator Double( Milliseconds milliseconds ) {
            return ( Double ) milliseconds.Value;
        }

        public static implicit operator Decimal( Milliseconds milliseconds ) {
            return milliseconds.Value;
        }

        public static Milliseconds operator -( Milliseconds milliseconds ) {
            return new Milliseconds( milliseconds.Value*-1 );
        }

        public static Milliseconds operator -( Milliseconds left, Milliseconds right ) {
            return Combine( left, -right.Value );
        }

        public static Milliseconds operator -( Milliseconds left, Decimal milliseconds ) {
            return Combine( left, -milliseconds );
        }

        public static Boolean operator !=( Milliseconds left, Milliseconds right ) {
            return !Equals( left, right );
        }

        public static Milliseconds operator +( Milliseconds left, Milliseconds right ) {
            return Combine( left, right.Value );
        }

        public static Milliseconds operator +( Milliseconds left, Decimal milliseconds ) {
            return Combine( left, milliseconds );
        }

        public static Milliseconds operator +( Milliseconds left, BigInteger milliseconds ) {
            return Combine( left, milliseconds );
        }

        public static Boolean operator <( Milliseconds left, Milliseconds right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( Milliseconds left, Seconds right ) {
            return ( Seconds ) left < right;
        }

        public static Boolean operator ==( Milliseconds left, Milliseconds right ) {
            return Equals( left, right );
        }

        public static Boolean operator >( Milliseconds left, Milliseconds right ) {
            return left.Value > right.Value;
        }

        public static Boolean operator >( Milliseconds left, Seconds right ) {
            return ( Seconds ) left > right;
        }

        public Microseconds ToMicroseconds() {
            return new Microseconds( Value*Microseconds.InOneMillisecond );
        }

        [Pure]
        public Seconds ToSeconds() {
            return new Seconds( Value/InOneSecond );
        }

        public Boolean Equals( Milliseconds other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Milliseconds && this.Equals( ( Milliseconds ) obj );
        }
    }
}
