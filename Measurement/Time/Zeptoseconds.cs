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
// "Librainian/Zeptoseconds.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using Librainian.Extensions;
    using NUnit.Framework;

    /// <summary>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Zeptosecond" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Immutable]
    public struct Zeptoseconds : IComparable<Zeptoseconds>, IQuantityOfTime {

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

        [DataMember]
        public readonly Decimal Value;

        public Zeptoseconds( Decimal value ) {
            this.Value = value;
        }

        public Zeptoseconds( long value ) {
            this.Value = value;
        }

        public Zeptoseconds( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal )value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay {
            get {
                return this.ToString();
            }
        }

        public static Zeptoseconds Combine( Zeptoseconds left, Zeptoseconds right ) => Combine( left, right.Value );

        public static Zeptoseconds Combine( Zeptoseconds left, Decimal zeptoseconds ) => new Zeptoseconds( left.Value + zeptoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Zeptoseconds left, Zeptoseconds right ) => left.Value == right.Value;

        /// <summary>
        ///     Implicitly convert the number of <paramref name="zeptoseconds" /> to <see cref="Milliseconds" />.
        /// </summary>
        /// <param name="zeptoseconds"></param>
        /// <returns></returns>
        public static implicit operator Attoseconds( Zeptoseconds zeptoseconds ) => zeptoseconds.ToAttoseconds();

        public static implicit operator Span( Zeptoseconds zeptoseconds ) => new Span( zeptoseconds: zeptoseconds.Value );

        public static implicit operator TimeSpan( Zeptoseconds zeptoseconds ) => TimeSpan.FromSeconds( value: ( Double )zeptoseconds.Value );

        /// <summary>
        ///     Implicitly convert  the number of <paramref name="zeptoseconds" /> to <see cref="Yoctoseconds" />.
        /// </summary>
        /// <param name="zeptoseconds"></param>
        /// <returns></returns>
        public static implicit operator Yoctoseconds( Zeptoseconds zeptoseconds ) => zeptoseconds.ToYoctoseconds();

        public static Zeptoseconds operator -( Zeptoseconds zeptoseconds ) => new Zeptoseconds( zeptoseconds.Value * -1 );

        public static Zeptoseconds operator -( Zeptoseconds left, Zeptoseconds right ) => Combine( left: left, right: -right );

        public static Zeptoseconds operator -( Zeptoseconds left, Decimal zeptoseconds ) => Combine( left, -zeptoseconds );

        public static Boolean operator !=( Zeptoseconds left, Zeptoseconds right ) => !Equals( left, right );

        public static Zeptoseconds operator +( Zeptoseconds left, Zeptoseconds right ) => Combine( left, right );

        public static Zeptoseconds operator +( Zeptoseconds left, Decimal zeptoseconds ) => Combine( left, zeptoseconds );

        public static Boolean operator <( Zeptoseconds left, Zeptoseconds right ) => left.Value < right.Value;

        public static Boolean operator <( Zeptoseconds left, Yoctoseconds right ) => left < ( Zeptoseconds )right;

        public static Boolean operator ==( Zeptoseconds left, Zeptoseconds right ) => Equals( left, right );

        public static Boolean operator >( Zeptoseconds left, Yoctoseconds right ) => left > ( Zeptoseconds )right;

        public static Boolean operator >( Zeptoseconds left, Zeptoseconds right ) => left.Value > right.Value;

        public int CompareTo( Zeptoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Zeptoseconds other ) => Equals( this, other );

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Zeptoseconds && this.Equals( ( Zeptoseconds )obj );
        }

        public override int GetHashCode() => this.Value.GetHashCode();

        [Test]
        public void Test() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().BeGreaterThan( Yoctoseconds.One );
            One.Should().Be( One );
            One.Should().BeLessThan( Attoseconds.One );
        }

        /// <summary>
        ///     <para>Convert to a larger unit.</para>
        /// </summary>
        /// <returns></returns>
        [Pure]
        public Attoseconds ToAttoseconds() => new Attoseconds( this.Value / InOneAttosecond );

        [Pure]
        public BigInteger ToPlanckTimes() => BigInteger.Multiply( PlanckTimes.InOneZeptosecond, new BigInteger( this.Value ) );

        [Pure]
        public override String ToString() => String.Format( "{0} zs", this.Value );

        /// <summary>
        ///     <para>Convert to a smaller unit.</para>
        /// </summary>
        /// <returns></returns>
        public Yoctoseconds ToYoctoseconds() => new Yoctoseconds( this.Value * Yoctoseconds.InOneZeptosecond );
    }
}