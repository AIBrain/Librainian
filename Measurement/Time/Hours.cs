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
// "Librainian/Hours.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
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
    public struct Hours : IComparable< Hours > {
        /// <summary>
        ///     24
        /// </summary>
        public const Byte InOneDay = 24;

        /// <summary>
        ///     One <see cref="Hours" /> .
        /// </summary>
        public static readonly Hours One = new Hours( 1 );

        /// <summary>
        /// </summary>
        public static readonly Hours Ten = new Hours( 10 );

        /// <summary>
        /// </summary>
        public static readonly Hours Thousand = new Hours( 1000 );

        /// <summary>
        ///     Zero <see cref="Hours" />
        /// </summary>
        public static readonly Hours Zero = new Hours( 0 );

        /// <summary>
        ///     730 <see cref="Hours" /> in one month, according to WolframAlpha.
        /// </summary>
        /// <see cref="http://www.wolframalpha.com/input/?i=converts+1+month+to+hours" />
        public static BigInteger InOneMonth = 730;

        [DataMember] public readonly  Decimal Value;

        static Hours() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Days.One );
            One.Should().BeGreaterThan( Minutes.One );
        }

        public Hours(Decimal value ) {
            this.Value = value;
        }

        public Hours( long value ) {
            this.Value = value;
        }

        public Hours( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = (Decimal ) value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( Hours other ) {
            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( Hours other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Hours && this.Equals( ( Hours ) obj );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneHour, new BigInteger( this.Value ) );
        }

        public static Hours Combine( Hours left, Hours right ) {
            return Combine( left, right.Value );
        }

        public static Hours Combine( Hours left,Decimal hours ) {
            return new Hours( left.Value + hours );
        }

        public static Hours Combine( Hours left, BigInteger hours ) {
            return new Hours( ( BigInteger ) left.Value + hours );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="hours" /> to <see cref="Days" />.
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static implicit operator Days( Hours hours ) {
            return hours.ToDays();
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="hours" /> to <see cref="Minutes" />.
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static implicit operator Minutes( Hours hours ) {
            return hours.ToMinutes();
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Hours left, Hours right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( Hours left, Hours right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( Hours left, Hours right ) {
            return !Equals( left, right );
        }

        public static implicit operator Span( Hours hours ) {
            return new Span( hours );
        }

        public static implicit operator TimeSpan( Hours hours ) {
            return TimeSpan.FromHours( value: ( Double ) hours.Value );
        }

        public static Hours operator -( Hours hours ) {
            return new Hours( hours.Value*-1 );
        }

        public static Hours operator -( Hours left, Hours right ) {
            return Combine( left: left, right: -right );
        }

        public static Hours operator -( Hours left,Decimal hours ) {
            return Combine( left, -hours );
        }

        public static Hours operator +( Hours left, Hours right ) {
            return Combine( left, right );
        }

        public static Hours operator +( Hours left,Decimal hours ) {
            return Combine( left, hours );
        }

        public static Hours operator +( Hours left, BigInteger hours ) {
            return Combine( left, hours );
        }

        public static Boolean operator <( Hours left, Hours right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( Hours left, Minutes right ) {
            return left < ( Hours ) right;
        }

        public static Boolean operator >( Hours left, Minutes right ) {
            return left > ( Hours ) right;
        }

        public static Boolean operator >( Hours left, Hours right ) {
            return left.Value > right.Value;
        }

        public Days ToDays() {
            return new Days( this.Value/InOneDay );
        }

        [Pure]
        public Minutes ToMinutes() {
            return new Minutes( this.Value*Minutes.InOneHour );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public override string ToString() {
            return String.Format( "{0} {1}", this.Value, this.Value.PluralOf( "hour" ) );
        }
    }
}
