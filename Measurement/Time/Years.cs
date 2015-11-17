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
// "Librainian/Years.cs" was last cleaned by Rick on 2015/06/12 at 3:03 PM
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
    using Parsing;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{ToString(),nq}" )]
    [Immutable]
    public struct Years : IComparable<Years>, IQuantityOfTime {

        /// <summary>One <see cref="Years" /> .</summary>
        public static readonly Years One = new Years( 1 );

        /// <summary></summary>
        public static readonly Years Ten = new Years( 10 );

        /// <summary></summary>
        public static readonly Years Thousand = new Years( 1000 );

        /// <summary>Zero <see cref="Years" /></summary>
        public static readonly Years Zero = new Years( 0 );

        [Test]
        public static void TestYears() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );

            //One.Should().BeGreaterThan( Months.One );
        }

        public Years( Decimal value ) {
            this.Value = value;
        }

        public Years( BigRational value ) {
            this.Value = value;
        }

        public Years( Int64 value ) {
            this.Value = value;
        }

        public Years( BigInteger value ) {
            this.Value = value;
        }

        [DataMember]
        public BigRational Value {
            get;
        }

        public Int32 CompareTo( Years other ) => this.Value.CompareTo( other.Value );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneYear * this.Value );

        [Pure]
        public override String ToString() => $"{this.Value.GetWholePart()} {this.Value.PluralOf( "year" )}";

        public static Years Combine( Years left, Years right ) => Combine( left, right.Value );

        public static Years Combine( Years left, Decimal years ) => new Years( left.Value + years );

        public static Years Combine( Years left, BigRational years ) => new Years( left.Value + years );

        /// <summary>
        /// <para>static equality test</para></summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Years left, Years right ) => left.Value == right.Value;

        public static implicit operator Months( Years years ) => years.ToMonths();

        public static implicit operator Span( Years years ) => new Span( years: years.Value );

        public static Years operator -( Years days ) => new Years( days.Value * -1 );

        public static Years operator -( Years left, Years right ) => Combine( left: left, right: -right );

        public static Years operator -( Years left, Decimal years ) => Combine( left, -years );

        public static Boolean operator !=( Years left, Years right ) => !Equals( left, right );

        public static Years operator +( Years left, Years right ) => Combine( left, right );

        public static Years operator +( Years left, Decimal years ) => Combine( left, years );

        public static Years operator +( Years left, BigInteger years ) => Combine( left, years );

        public static Boolean operator <( Years left, Years right ) => left.Value < right.Value;

        public static Boolean operator ==( Years left, Years right ) => Equals( left, right );

        public static Boolean operator >( Years left, Years right ) => left.Value > right.Value;

        public Boolean Equals( Years other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Years && this.Equals( ( Years )obj );
        }

        [Pure]
        public Days ToDays() => new Days( this.Value * Days.InOneCommonYear );

        [Pure]
        public Months ToMonths() => new Months( this.Value * Months.InOneCommonYear );

        [Pure]
        public Seconds ToSeconds() => new Seconds( this.Value * Seconds.InOneCommonYear );

        [Pure]
        public Weeks ToWeeks() => new Weeks( this.Value * Weeks.InOneCommonYear );
    }
}