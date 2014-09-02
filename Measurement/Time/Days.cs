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
// "Librainian/Days.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

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
    public struct Days : IComparable<Days>, IQuantityOfTime {

        /// <summary>
        ///     365
        /// </summary>
        public const UInt16 InOneCommonYear = 365;

        /// <summary>
        ///     7
        /// </summary>
        public const UInt16 InOneWeek = 7;

        /// <summary>
        ///     One <see cref="Days" /> .
        /// </summary>
        public static readonly Days One = new Days( 1 );

        /// <summary>
        /// </summary>
        public static readonly Days Ten = new Days( 10 );

        /// <summary>
        /// </summary>
        public static readonly Days Thousand = new Days( 1000 );

        /// <summary>
        ///     Zero <see cref="Days" />
        /// </summary>
        public static readonly Days Zero = new Days( 0 );

        [DataMember]
        public readonly Decimal Value;

        static Days() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Weeks.One );
            One.Should().BeGreaterThan( Hours.One );
        }

        public Days( Decimal value ) {
            this.Value = value;
        }

        public Days( long value ) {
            this.Value = value;
        }

        public Days( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal )value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay {
            get {
                return this.ToString();
            }
        }

        public const Byte InOneMonth = 31;

        public static Days Combine( Days left, Days right ) {
            return Combine( left, right.Value );
        }

        public static Days Combine( Days left, Decimal days ) {
            return new Days( left.Value + days );
        }

        public static Days Combine( Days left, BigInteger days ) {
            return new Days( ( BigInteger )left.Value + days );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Days left, Days right ) {
            return left.Value == right.Value;
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="days" /> to <see cref="Hours" />.
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static implicit operator Hours( Days days ) {
            return days.ToHours();
        }

        public static implicit operator Span( Days days ) {
            return new Span( days: days.Value );
        }

        public static implicit operator TimeSpan( Days days ) {
            return TimeSpan.FromDays( ( Double )days.Value );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="days" /> to <see cref="Weeks" />.
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static implicit operator Weeks( Days days ) {
            return days.ToWeeks();
        }

        public static Days operator -( Days days ) {
            return new Days( days.Value * -1 );
        }

        public static Days operator -( Days left, Days right ) {
            return Combine( left: left, right: -right );
        }

        public static Days operator -( Days left, Decimal days ) {
            return Combine( left, -days );
        }

        public static Boolean operator !=( Days left, Days right ) {
            return !Equals( left, right );
        }

        public static Days operator +( Days left, Days right ) {
            return Combine( left, right );
        }

        public static Days operator +( Days left, Decimal days ) {
            return Combine( left, days );
        }

        public static Days operator +( Days left, BigInteger days ) {
            return Combine( left, days );
        }

        public static Boolean operator <( Days left, Days right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( Days left, Hours right ) {
            return left < ( Days )right;
        }

        public static Boolean operator ==( Days left, Days right ) {
            return Equals( left, right );
        }

        public static Boolean operator >( Days left, Hours right ) {
            return left > ( Days )right;
        }

        public static Boolean operator >( Days left, Days right ) {
            return left.Value > right.Value;
        }

        public int CompareTo( Days other ) {
            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( Days other ) {
            return Equals( this.Value, other.Value );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Days && this.Equals( ( Days )obj );
        }

        [Pure]
        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        [Pure]
        public Hours ToHours() {
            return new Hours( this.Value * Hours.InOneDay );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneDay, new BigInteger( this.Value ) );
        }

        [Pure]
        public override string ToString() {
            return String.Format( "{0} {1}", this.Value, this.Value.PluralOf( "day" ) );
        }

        [Pure]
        public Weeks ToWeeks() {
            return new Weeks( this.Value / InOneWeek );
        }
    }
}