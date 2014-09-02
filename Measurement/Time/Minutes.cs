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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Minutes.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using Parsing;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public struct Minutes : IComparable<Minutes>, IQuantityOfTime {

        /// <summary>
        ///     60
        /// </summary>
        public const Byte InOneHour = 60;

        /// <summary>
        ///     One <see cref="Minutes" /> .
        /// </summary>
        public static readonly Minutes One = new Minutes( value: 1 );

        /// <summary>
        /// </summary>
        public static readonly Minutes Ten = new Minutes( value: 10 );

        /// <summary>
        /// </summary>
        public static readonly Minutes Thousand = new Minutes( value: 1000 );

        /// <summary>
        ///     Zero <see cref="Minutes" />
        /// </summary>
        public static readonly Minutes Zero = new Minutes( value: 0 );

        [DataMember]
        public readonly Decimal Value;

        static Minutes() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Hours.One );
            One.Should().BeGreaterThan( Seconds.One );
        }

        public Minutes( Decimal value ) {
            this.Value = value;
        }

        public Minutes( long value ) {
            this.Value = value;
        }

        public Minutes( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal )value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay {
            get {
                return this.ToString();
            }
        }

        public static Minutes Combine( Minutes left, Minutes right ) {
            return Combine( left, right.Value );
        }

        public static Minutes Combine( Minutes left, Decimal minutes ) {
            return new Minutes( left.Value + minutes );
        }

        public static Minutes Combine( Minutes left, BigInteger minutes ) {
            return new Minutes( ( BigInteger )left.Value + minutes );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Minutes left, Minutes right ) {
            return left.Value == right.Value;
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to <see cref="Hours" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static implicit operator Hours( Minutes minutes ) {
            return minutes.ToHours();
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to <see cref="Seconds" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static implicit operator Seconds( Minutes minutes ) {
            return minutes.ToSeconds();
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to a <see cref="Span" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static implicit operator Span( Minutes minutes ) {
            return new Span( minutes );
        }

        public static implicit operator TimeSpan( Minutes minutes ) {
            return TimeSpan.FromMinutes( ( Double )minutes.Value );
        }

        public static Minutes operator -( Minutes minutes ) {
            return new Minutes( minutes.Value * -1 );
        }

        public static Minutes operator -( Minutes left, Minutes right ) {
            return Combine( left: left, right: -right );
        }

        public static Minutes operator -( Minutes left, Decimal minutes ) {
            return Combine( left, -minutes );
        }

        public static Boolean operator !=( Minutes left, Minutes right ) {
            return !Equals( left, right );
        }

        public static Minutes operator +( Minutes left, Minutes right ) {
            return Combine( left, right );
        }

        public static Minutes operator +( Minutes left, Decimal minutes ) {
            return Combine( left, minutes );
        }

        public static Minutes operator +( Minutes left, BigInteger minutes ) {
            return Combine( left, minutes );
        }

        public static Boolean operator <( Minutes left, Minutes right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( Minutes left, Hours right ) {
            return ( Hours )left < right;
        }

        public static Boolean operator <( Minutes left, Seconds right ) {
            return left < ( Minutes )right;
        }

        public static Boolean operator ==( Minutes left, Minutes right ) {
            return Equals( left, right );
        }

        public static Boolean operator >( Minutes left, Hours right ) {
            return ( Hours )left > right;
        }

        public static Boolean operator >( Minutes left, Minutes right ) {
            return left.Value > right.Value;
        }

        public static Boolean operator >( Minutes left, Seconds right ) {
            return left > ( Minutes )right;
        }

        public int CompareTo( Minutes other ) {
            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( Minutes other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Minutes && this.Equals( ( Minutes )obj );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public Hours ToHours() {
            return new Hours( this.Value / InOneHour );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneMinute, new BigInteger( this.Value ) );
        }

        [Pure]
        public Seconds ToSeconds() {
            return new Seconds( this.Value * Seconds.InOneMinute );
        }

        [Pure]
        public override string ToString() {
            return String.Format( "{0} {1}", this.Value, this.Value.PluralOf( "minute" ) );
        }
    }
}