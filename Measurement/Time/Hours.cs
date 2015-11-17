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
// "Librainian/Hours.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM
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
    using Parsing;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Immutable]
    public struct Hours : IComparable<Hours>, IQuantityOfTime {

        /// <summary>24</summary>
        public const Byte InOneDay = 24;

        /// <summary>One <see cref="Hours" /> .</summary>
        public static readonly Hours One = new Hours( 1 );

        /// <summary>Eight <see cref="Hours" /> .</summary>
        public static readonly Hours Eight = new Hours( 8 );

        /// <summary></summary>
        public static readonly Hours Ten = new Hours( 10 );

        /// <summary></summary>
        public static readonly Hours Thousand = new Hours( 1000 );

        /// <summary>Zero <see cref="Hours" /></summary>
        public static readonly Hours Zero = new Hours( 0 );

        /// <summary>730 <see cref="Hours" /> in one month, according to WolframAlpha.</summary>
        /// <see cref="http://www.wolframalpha.com/input/?i=converts+1+month+to+hours" />
        public static BigInteger InOneMonth = 730;

        static Hours() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Days.One );
            One.Should().BeGreaterThan( Minutes.One );
        }

        public Hours(Decimal value) {
            this.Value = value;
        }

        public Hours( BigRational value ) {
            this.Value = value;
        }

        public Hours(Int64 value) {
            this.Value = value;
        }

        public Hours(BigInteger value) {
            this.Value = value;
        }

        [DataMember]
        public BigRational Value {
            get;
        }

        private String DebuggerDisplay => this.ToString();

        public Int32 CompareTo(Hours other) => this.Value.CompareTo( other.Value );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneHour * this.Value );

        [Pure]
        public override String ToString() => $"{this.Value} {this.Value.PluralOf( "hour" )}";

        public static Hours Combine(Hours left, Hours right) => Combine( left, right.Value );

        public static Hours Combine(Hours left, BigRational hours ) => new Hours( left.Value + hours );

        public static Hours Combine(Hours left, BigInteger hours) => new Hours( ( BigInteger )left.Value + hours );

        /// <summary>
        /// <para>static equality test</para></summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals(Hours left, Hours right) => left.Value == right.Value;

        /// <summary>Implicitly convert the number of <paramref name="hours" /> to <see cref="Days" />.</summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static implicit operator Days(Hours hours) => hours.ToDays();

        /// <summary>Implicitly convert the number of <paramref name="hours" /> to <see cref="Minutes" />.</summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static implicit operator Minutes(Hours hours) => hours.ToMinutes();

        public static implicit operator Span(Hours hours) => new Span( hours );

        public static implicit operator TimeSpan(Hours hours) => TimeSpan.FromHours( value: ( Double )hours.Value );

        public static Hours operator -(Hours hours) => new Hours( hours.Value * -1 );

        public static Hours operator -(Hours left, Hours right) => Combine( left: left, right: -right );

        public static Hours operator -(Hours left, Decimal hours) => Combine( left, -hours );

        public static Boolean operator !=(Hours left, Hours right) => !Equals( left, right );

        public static Hours operator +(Hours left, Hours right) => Combine( left, right );

        public static Hours operator +(Hours left, Decimal hours) => Combine( left, hours );

        public static Hours operator +(Hours left, BigInteger hours) => Combine( left, hours );

        public static Boolean operator <(Hours left, Hours right) => left.Value < right.Value;

        public static Boolean operator <(Hours left, Minutes right) => left < ( Hours )right;

        public static Boolean operator ==(Hours left, Hours right) => Equals( left, right );

        public static Boolean operator >(Hours left, Minutes right) => left > ( Hours )right;

        public static Boolean operator >(Hours left, Hours right) => left.Value > right.Value;

        public Boolean Equals(Hours other) => Equals( this, other );

        public override Boolean Equals(Object obj) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Hours && this.Equals( ( Hours )obj );
        }

        [Pure]
        public Days ToDays() => new Days( this.Value / InOneDay );

        [Pure]
        public Minutes ToMinutes() => new Minutes( this.Value * Minutes.InOneHour );
    }
}