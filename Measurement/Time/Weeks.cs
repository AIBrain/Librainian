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
// "Librainian/Weeks.cs" was last cleaned by Rick on 2015/06/12 at 3:03 PM
#endregion License & Information

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Numerics;
    using NUnit.Framework;
    using Parsing;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{ToString(),nq}" )]
    public struct Weeks : IComparable<Weeks>, IQuantityOfTime {

        /// <summary>52</summary>
        public const Decimal InOneCommonYear = 52m;

        /// <summary>4. 345</summary>
        public const Decimal InOneMonth = 4.345m;

        /// <summary>One <see cref="Weeks" /> .</summary>
        public static readonly Weeks One = new Weeks( 1 );

        /// <summary></summary>
        public static readonly Weeks Ten = new Weeks( 10 );

        /// <summary></summary>
        public static readonly Weeks Thousand = new Weeks( 1000 );

        /// <summary>Zero <see cref="Weeks" /></summary>
        public static readonly Weeks Zero = new Weeks( 0 );

        [Test]
        public static void TestWeeks() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );

            One.Should().BeLessThan( Months.One );
            One.Should().BeGreaterThan( Days.One );
        }

        public Weeks(Decimal weeks) {
            this.Value = weeks;
        }

        public Weeks( BigRational weeks ) {
            this.Value = weeks;
        }

        public Weeks(Int64 value) {
            this.Value = value;
        }

        public Weeks(BigInteger value) {
            this.Value = value;
        }

        [DataMember]
        public BigRational Value {
            get;
        }

        public Int32 CompareTo(Weeks other) => this.Value.CompareTo( other.Value );

        [Pure]
        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneWeek * this.Value );

        [Pure]
        public override String ToString() => $"{this.Value} {this.Value.PluralOf( "week" )}";

        public static Weeks Combine(Weeks left, Weeks right) => new Weeks( left.Value + right.Value );

        public static Weeks Combine(Weeks left, BigRational weeks ) => new Weeks( left.Value + weeks );

        public static Weeks Combine(Weeks left, BigInteger weeks) => new Weeks( left.Value + weeks );

        /// <summary>
        /// <para>static equality test</para></summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals(Weeks left, Weeks right) => left.Value == right.Value;

        /// <summary>Implicitly convert the number of <paramref name="weeks" /> to <see cref="Days" />.</summary>
        /// <param name="weeks"></param>
        /// <returns></returns>
        public static implicit operator Days(Weeks weeks) => weeks.ToDays();

        public static implicit operator Months(Weeks weeks) => weeks.ToMonths();

        public static implicit operator Span(Weeks weeks) => new Span( weeks: weeks.Value );

        public static Weeks operator -(Weeks days) => new Weeks( days.Value * -1 );

        public static Weeks operator -(Weeks left, Weeks right) => Combine( left: left, right: -right );

        public static Boolean operator !=(Weeks left, Weeks right) => !Equals( left, right );

        public static Weeks operator +(Weeks left, Weeks right) => Combine( left, right );

        public static Weeks operator +(Weeks left, Decimal weeks) => Combine( left, weeks );

        public static Weeks operator +(Weeks left, BigInteger weeks) => Combine( left, weeks );

        public static Boolean operator <(Weeks left, Weeks right) => left.Value < right.Value;

        public static Boolean operator <(Weeks left, Days right) => left < ( Weeks )right;

        public static Boolean operator <(Weeks left, Months right) => ( Months )left < right;

        public static Boolean operator ==(Weeks left, Weeks right) => Equals( left, right );

        public static Boolean operator >(Weeks left, Months right) => ( Months )left > right;

        public static Boolean operator >(Weeks left, Days right) => left > ( Weeks )right;

        public static Boolean operator >(Weeks left, Weeks right) => left.Value > right.Value;

        public Boolean Equals(Weeks other) => Equals( this, other );

        public override Boolean Equals(Object obj) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Weeks && this.Equals( ( Weeks )obj );
        }

        [Pure]
        public Days ToDays() => new Days( this.Value * Days.InOneWeek );

        [Pure]
        public Months ToMonths() => new Months( this.Value / InOneMonth );
    }
}