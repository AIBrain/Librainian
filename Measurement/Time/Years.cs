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
// "Librainian/Years.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

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
    public struct Years : IComparable<Years> {
        /// <summary>
        ///     One <see cref="Years" /> .
        /// </summary>
        public static readonly Years One = new Years( 1 );

        /// <summary>
        /// </summary>
        public static readonly Years Ten = new Years( 10 );

        /// <summary>
        /// </summary>
        public static readonly Years Thousand = new Years( 1000 );

        /// <summary>
        ///     Zero <see cref="Years" />
        /// </summary>
        public static readonly Years Zero = new Years( 0 );

        [DataMember]
        public readonly Decimal Value;

        static Years() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeGreaterThan( Months.One );
        }

        public Years( Decimal value ) {
            this.Value = value;
        }

        public Years( long value ) {
            this.Value = value;
        }

        public Years( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal )value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( Years other ) {
            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( Years other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Years && this.Equals( ( Years )obj );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneYear, new BigInteger( this.Value ) );
        }

        public static Years Combine( Years left, Years right ) {
            return Combine( left, right.Value );
        }

        public static Years Combine( Years left, Decimal years ) {
            return new Years( left.Value + years );
        }

        public static Years Combine( Years left, BigInteger years ) {
            return new Years( ( BigInteger )left.Value + years );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Years left, Years right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( Years left, Years right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( Years left, Years right ) {
            return !Equals( left, right );
        }

        public static implicit operator Months( Years years ) {
            return ToMonths( years );
        }

        public static implicit operator Span( Years years ) {
            return new Span( years: years.Value );
        }

        public static Years operator -( Years days ) {
            return new Years( days.Value * -1 );
        }

        public static Years operator -( Years left, Years right ) {
            return Combine( left: left, right: -right );
        }

        public static Years operator -( Years left, Decimal years ) {
            return Combine( left, -years );
        }

        public static Years operator +( Years left, Years right ) {
            return Combine( left, right );
        }

        public static Years operator +( Years left, Decimal years ) {
            return Combine( left, years );
        }

        public static Years operator +( Years left, BigInteger years ) {
            return Combine( left, years );
        }

        public static Boolean operator <( Years left, Years right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator >( Years left, Years right ) {
            return left.Value > right.Value;
        }

        public static Months ToMonths( Years years ) {
            var months = years.Value * Months.InOneYear;
            return new Months( months );
        }

        public static BigInteger ToPlanckTimes( Years years ) {
            return BigInteger.Multiply( PlanckTimes.InOneYear, new BigInteger( years.Value ) );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public override string ToString() {
            return String.Format( "{0:R} {1}", this.Value, this.Value.PluralOf( "year" ) );
        }
    }
}
