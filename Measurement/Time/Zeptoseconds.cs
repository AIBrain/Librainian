#region License & Information

// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Zeptoseconds.cs" was last cleaned by Rick on 2015/06/12 at 3:03 PM
#endregion License & Information

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Numerics;
    using NUnit.Framework;

    /// <summary></summary>
    /// <seealso cref="http://wikipedia.org/wiki/Zeptosecond" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{ToString(),nq}" )]
    [Immutable]
    public struct Zeptoseconds : IComparable<Zeptoseconds>, IQuantityOfTime {

        /// <summary>1000</summary>
        public const UInt16 InOneAttosecond = 1000;

        /// <summary><see cref="Five" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Five = new Zeptoseconds( 5 );

        /// <summary><see cref="One" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds One = new Zeptoseconds( 1 );

        /// <summary><see cref="Seven" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Seven = new Zeptoseconds( 7 );

        /// <summary><see cref="Ten" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Ten = new Zeptoseconds( 10 );

        /// <summary><see cref="Thirteen" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Thirteen = new Zeptoseconds( 13 );

        /// <summary><see cref="Thirty" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Thirty = new Zeptoseconds( 30 );

        /// <summary><see cref="Three" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Three = new Zeptoseconds( 3 );

        /// <summary><see cref="Two" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Two = new Zeptoseconds( 2 );

        /// <summary></summary>
        public static readonly Zeptoseconds Zero = new Zeptoseconds( 0 );

        public Zeptoseconds(Decimal value) {
            this.Value = value;
        }

        public Zeptoseconds( BigRational value ) {
            this.Value = value;
        }

        public Zeptoseconds(Int64 value) {
            this.Value = value;
        }

        public Zeptoseconds(BigInteger value) {
            this.Value = value;
        }

        [DataMember]
        public BigRational Value {
            get;
        }

        public Int32 CompareTo(Zeptoseconds other) => this.Value.CompareTo( other.Value );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneZeptosecond * this.Value );

        [Pure]
        public override String ToString() {
            return this.Value > Decimal.MaxValue ? $"{this.Value.GetWholePart()} zs" : $"{( Decimal )this.Value} zs";
        }

        public static Zeptoseconds Combine(Zeptoseconds left, Zeptoseconds right) => Combine( left, right.Value );

        public static Zeptoseconds Combine(Zeptoseconds left, BigRational zeptoseconds ) => new Zeptoseconds( left.Value + zeptoseconds );

        /// <summary>
        /// <para>static equality test</para></summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals(Zeptoseconds left, Zeptoseconds right) => left.Value == right.Value;

        /// <summary>
        /// Implicitly convert the number of <paramref name="zeptoseconds" /> to <see cref="Milliseconds" />.
        /// </summary>
        /// <param name="zeptoseconds"></param>
        /// <returns></returns>
        public static implicit operator Attoseconds(Zeptoseconds zeptoseconds) => zeptoseconds.ToAttoseconds();

        public static implicit operator Span(Zeptoseconds zeptoseconds) => new Span( zeptoseconds: zeptoseconds.Value );

        public static implicit operator TimeSpan(Zeptoseconds zeptoseconds) => TimeSpan.FromSeconds( value: ( Double )zeptoseconds.Value );

        /// <summary>
        /// Implicitly convert the number of <paramref name="zeptoseconds" /> to <see cref="Yoctoseconds" />.
        /// </summary>
        /// <param name="zeptoseconds"></param>
        /// <returns></returns>
        public static implicit operator Yoctoseconds(Zeptoseconds zeptoseconds) => zeptoseconds.ToYoctoseconds();

        public static Zeptoseconds operator -(Zeptoseconds zeptoseconds) => new Zeptoseconds( zeptoseconds.Value * -1 );

        public static Zeptoseconds operator -(Zeptoseconds left, Zeptoseconds right) => Combine( left: left, right: -right );

        public static Zeptoseconds operator -(Zeptoseconds left, Decimal zeptoseconds) => Combine( left, -zeptoseconds );

        public static Boolean operator !=(Zeptoseconds left, Zeptoseconds right) => !Equals( left, right );

        public static Zeptoseconds operator +(Zeptoseconds left, Zeptoseconds right) => Combine( left, right );

        public static Zeptoseconds operator +(Zeptoseconds left, Decimal zeptoseconds) => Combine( left, zeptoseconds );

        public static Boolean operator <(Zeptoseconds left, Zeptoseconds right) => left.Value < right.Value;

        public static Boolean operator <(Zeptoseconds left, Yoctoseconds right) => left < ( Zeptoseconds )right;

        public static Boolean operator ==(Zeptoseconds left, Zeptoseconds right) => Equals( left, right );

        public static Boolean operator >(Zeptoseconds left, Yoctoseconds right) => left > ( Zeptoseconds )right;

        public static Boolean operator >(Zeptoseconds left, Zeptoseconds right) => left.Value > right.Value;

        public Boolean Equals(Zeptoseconds other) => Equals( this, other );

        public override Boolean Equals(Object obj) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Zeptoseconds && this.Equals( ( Zeptoseconds )obj );
        }

        [Test]
        public void Test() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().BeGreaterThan( Yoctoseconds.One );
            One.Should().Be( One );
            One.Should().BeLessThan( Attoseconds.One );
        }

        /// <summary>
        /// <para>Convert to a larger unit.</para></summary>
        /// <returns></returns>
        [Pure]
        public Attoseconds ToAttoseconds() => new Attoseconds( this.Value / InOneAttosecond );

        /// <summary>
        /// <para>Convert to a smaller unit.</para></summary>
        /// <returns></returns>
        public Yoctoseconds ToYoctoseconds() => new Yoctoseconds( this.Value * Yoctoseconds.InOneZeptosecond );
    }
}