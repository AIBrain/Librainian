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
// "Librainian2/Zeptoseconds.cs" was last cleaned by Rick on 2014/08/08 at 2:30 PM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using NUnit.Framework;

    /// <summary>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Zeptosecond" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public struct Zeptoseconds : IComparable< Zeptoseconds >, IQuantityOfTime {
        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOneAttosecond = 1000;

        /// <summary>
        ///     <see cref="Five" /> <see cref="Zeptoseconds" />.
        /// </summary>
        public static readonly Zeptoseconds Five = new Zeptoseconds( 5 );

        /// <summary>
        ///     <see cref="One" /> <see cref="Zeptoseconds" />.
        /// </summary>
        public static readonly Zeptoseconds One = new Zeptoseconds( 1 );

        /// <summary>
        ///     <see cref="Seven" /> <see cref="Zeptoseconds" />.
        /// </summary>
        public static readonly Zeptoseconds Seven = new Zeptoseconds( 7 );

        /// <summary>
        ///     <see cref="Ten" /> <see cref="Zeptoseconds" />.
        /// </summary>
        public static readonly Zeptoseconds Ten = new Zeptoseconds( 10 );

        /// <summary>
        ///     <see cref="Thirteen" /> <see cref="Zeptoseconds" />.
        /// </summary>
        public static readonly Zeptoseconds Thirteen = new Zeptoseconds( 13 );

        /// <summary>
        ///     <see cref="Thirty" /> <see cref="Zeptoseconds" />.
        /// </summary>
        public static readonly Zeptoseconds Thirty = new Zeptoseconds( 30 );

        /// <summary>
        ///     <see cref="Three" /> <see cref="Zeptoseconds" />.
        /// </summary>
        public static readonly Zeptoseconds Three = new Zeptoseconds( 3 );

        /// <summary>
        ///     <see cref="Two" /> <see cref="Zeptoseconds" />.
        /// </summary>
        public static readonly Zeptoseconds Two = new Zeptoseconds( 2 );

        /// <summary>
        /// </summary>
        public static readonly Zeptoseconds Zero = new Zeptoseconds( 0 );

        [DataMember] public readonly Decimal Value;

        public Zeptoseconds( Decimal value ) {
            this.Value = value;
        }

        public Zeptoseconds( long value ) {
            this.Value = value;
        }

        public Zeptoseconds( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal ) value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( Zeptoseconds other ) {
            return this.Value.CompareTo( other.Value );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneZeptosecond, new BigInteger( this.Value ) );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public override string ToString() {
            return String.Format( "{0:R} zs", this.Value );
        }

        public Boolean Equals( Zeptoseconds other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Zeptoseconds && this.Equals( ( Zeptoseconds ) obj );
        }

        [Test]
        public void Test() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().BeGreaterThan( Yoctoseconds.One );
            One.Should().Be( One );
            One.Should().BeLessThan( Attoseconds.One );
        }

        public static Zeptoseconds Combine( Zeptoseconds left, Zeptoseconds right ) {
            return Combine( left, right.Value );
        }

        public static Zeptoseconds Combine( Zeptoseconds left, Decimal zeptoseconds ) {
            return new Zeptoseconds( left.Value + zeptoseconds );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Zeptoseconds left, Zeptoseconds right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( Zeptoseconds left, Zeptoseconds right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( Zeptoseconds left, Zeptoseconds right ) {
            return !Equals( left, right );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="zeptoseconds" /> to <see cref="Milliseconds" />.
        /// </summary>
        /// <param name="zeptoseconds"></param>
        /// <returns></returns>
        public static implicit operator Attoseconds( Zeptoseconds zeptoseconds ) {
            return ToAttoseconds( zeptoseconds );
        }

        public static implicit operator Span( Zeptoseconds zeptoseconds ) {
            return new Span( seconds: zeptoseconds.Value );
        }

        public static implicit operator TimeSpan( Zeptoseconds zeptoseconds ) {
            return TimeSpan.FromSeconds( value: ( Double ) zeptoseconds.Value );
        }

        /// <summary>
        ///     Implicitly convert  the number of <paramref name="zeptoseconds" /> to <see cref="Yoctoseconds" />.
        /// </summary>
        /// <param name="zeptoseconds"></param>
        /// <returns></returns>
        public static implicit operator Yoctoseconds( Zeptoseconds zeptoseconds ) {
            return ToYoctoseconds( zeptoseconds );
        }

        public static Zeptoseconds operator -( Zeptoseconds zeptoseconds ) {
            return new Zeptoseconds( zeptoseconds.Value*-1 );
        }

        public static Zeptoseconds operator -( Zeptoseconds left, Zeptoseconds right ) {
            return Combine( left: left, right: -right );
        }

        public static Zeptoseconds operator -( Zeptoseconds left, Decimal zeptoseconds ) {
            return Combine( left, -zeptoseconds );
        }

        public static Zeptoseconds operator +( Zeptoseconds left, Zeptoseconds right ) {
            return Combine( left, right );
        }

        public static Zeptoseconds operator +( Zeptoseconds left, Decimal zeptoseconds ) {
            return Combine( left, zeptoseconds );
        }

        public static Boolean operator <( Zeptoseconds left, Zeptoseconds right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( Zeptoseconds left, Yoctoseconds right ) {
            return left < ( Zeptoseconds ) right;
        }

        public static Boolean operator >( Zeptoseconds left, Yoctoseconds right ) {
            return left > ( Zeptoseconds ) right;
        }

        public static Boolean operator >( Zeptoseconds left, Zeptoseconds right ) {
            return left.Value > right.Value;
        }

        /// <summary>
        ///     <para>Convert to a larger unit.</para>
        /// </summary>
        /// <param name="zeptoseconds"></param>
        /// <returns></returns>
        public static Femtoseconds ToAttoseconds( Zeptoseconds zeptoseconds ) {
            return new Attoseconds( zeptoseconds.Value/InOneAttosecond );
        }

        /// <summary>
        ///     <para>Convert to a smaller unit.</para>
        /// </summary>
        /// <param name="zeptoseconds"></param>
        /// <returns></returns>
        public static Yoctoseconds ToYoctoseconds( Zeptoseconds zeptoseconds ) {
            return new Yoctoseconds( zeptoseconds.Value*Yoctoseconds.InOneZeptosecond );
        }
    }
}
