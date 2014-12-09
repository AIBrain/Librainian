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
    using Librainian.Extensions;
    using Parsing;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Immutable]
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
        /// 10
        /// </summary>
        public static readonly Minutes Ten = new Minutes( value: 10 );

        /// <summary>
        /// 15
        /// </summary>
        public static readonly Minutes Fifteen = new Minutes( value: 15 );

        /// <summary>
        /// 30
        /// </summary>
        public static readonly Minutes Thirty = new Minutes( value: 30 );

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
        private String DebuggerDisplay {
            get {
                return this.ToString();
            }
        }

        public static Minutes Combine( Minutes left, Minutes right ) => Combine( left, right.Value );

        public static Minutes Combine( Minutes left, Decimal minutes ) => new Minutes( left.Value + minutes );

        public static Minutes Combine( Minutes left, BigInteger minutes ) => new Minutes( ( BigInteger )left.Value + minutes );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Minutes left, Minutes right ) => left.Value == right.Value;

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to <see cref="Hours" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static implicit operator Hours( Minutes minutes ) => minutes.ToHours();

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to <see cref="Seconds" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static implicit operator Seconds( Minutes minutes ) => minutes.ToSeconds();

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to a <see cref="Span" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static implicit operator Span( Minutes minutes ) => new Span( minutes );

        public static implicit operator TimeSpan( Minutes minutes ) => TimeSpan.FromMinutes( ( Double )minutes.Value );

        public static Minutes operator -( Minutes minutes ) => new Minutes( minutes.Value * -1 );

        public static Minutes operator -( Minutes left, Minutes right ) => Combine( left: left, right: -right );

        public static Minutes operator -( Minutes left, Decimal minutes ) => Combine( left, -minutes );

        public static Boolean operator !=( Minutes left, Minutes right ) => !Equals( left, right );

        public static Minutes operator +( Minutes left, Minutes right ) => Combine( left, right );

        public static Minutes operator +( Minutes left, Decimal minutes ) => Combine( left, minutes );

        public static Minutes operator +( Minutes left, BigInteger minutes ) => Combine( left, minutes );

        public static Boolean operator <( Minutes left, Minutes right ) => left.Value < right.Value;

        public static Boolean operator <( Minutes left, Hours right ) => ( Hours )left < right;

        public static Boolean operator <( Minutes left, Seconds right ) => left < ( Minutes )right;

        public static Boolean operator ==( Minutes left, Minutes right ) => Equals( left, right );

        public static Boolean operator >( Minutes left, Hours right ) => ( Hours )left > right;

        public static Boolean operator >( Minutes left, Minutes right ) => left.Value > right.Value;

        public static Boolean operator >( Minutes left, Seconds right ) => left > ( Minutes )right;

        public int CompareTo( Minutes other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Minutes other ) => Equals( this, other );

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Minutes && this.Equals( ( Minutes )obj );
        }

        public override int GetHashCode() => this.Value.GetHashCode();

        public Hours ToHours() => new Hours( this.Value / InOneHour );

        [Pure]
        public BigInteger ToPlanckTimes() => BigInteger.Multiply( PlanckTimes.InOneMinute, new BigInteger( this.Value ) );

        [Pure]
        public Seconds ToSeconds() => new Seconds( this.Value * Seconds.InOneMinute );

        [Pure]
        public override String ToString() => String.Format( "{0} {1}", this.Value, this.Value.PluralOf( "minute" ) );
    }
}