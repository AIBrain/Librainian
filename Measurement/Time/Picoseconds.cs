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
// "Librainian/Picoseconds.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM
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

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{ToString(),nq}" )]
    [Serializable]
    [Immutable]
    public struct Picoseconds : IComparable<Picoseconds>, IQuantityOfTime {

        /// <summary>1000</summary>
        public const UInt16 InOneNanosecond = 1000;

        /// <summary>Ten <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds Fifteen = new Picoseconds( 15 );

        /// <summary>Five <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds Five = new Picoseconds( 5 );

        /// <summary>Five Hundred <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds FiveHundred = new Picoseconds( 500 );

        /// <summary>One <see cref="Picoseconds" />.</summary>
        public static readonly Picoseconds One = new Picoseconds( 1 );

        /// <summary>One Thousand Nine <see cref="Picoseconds" /> (Prime).</summary>
        public static readonly Picoseconds OneThousandNine = new Picoseconds( 1009 );

        /// <summary>Sixteen <see cref="Picoseconds" />.</summary>
        public static readonly Picoseconds Sixteen = new Picoseconds( 16 );

        /// <summary>Ten <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds Ten = new Picoseconds( 10 );

        /// <summary>Three <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds Three = new Picoseconds( 3 );

        /// <summary>Three Three Three <see cref="Picoseconds" />.</summary>
        public static readonly Picoseconds ThreeHundredThirtyThree = new Picoseconds( 333 );

        /// <summary>Two <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds Two = new Picoseconds( 2 );

        /// <summary>Two Hundred <see cref="Picoseconds" />.</summary>
        public static readonly Picoseconds TwoHundred = new Picoseconds( 200 );

        /// <summary>Two Hundred Eleven <see cref="Picoseconds" /> (Prime).</summary>
        public static readonly Picoseconds TwoHundredEleven = new Picoseconds( 211 );

        /// <summary>Two Thousand Three <see cref="Picoseconds" /> (Prime).</summary>
        public static readonly Picoseconds TwoThousandThree = new Picoseconds( 2003 );

        /// <summary>Zero <see cref="Picoseconds" />.</summary>
        public static readonly Picoseconds Zero = new Picoseconds( 0 );

        static Picoseconds() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Nanoseconds.One );
            One.Should().BeGreaterThan( Femtoseconds.One );
        }

        public Picoseconds(Decimal value) {
            this.Value = value;
        }

        public Picoseconds( BigRational value ) {
            this.Value = value;
        }

        public Picoseconds(Int64 value) {
            this.Value = value;
        }

        public Picoseconds(BigInteger value) {
            this.Value = value;
        }

        [DataMember]
        public BigRational Value {
            get;
        }

        public Int32 CompareTo(Picoseconds other) => this.Value.CompareTo( other.Value );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOnePicosecond * this.Value );

        [Pure]
        public override String ToString() {
            return this.Value > Decimal.MaxValue ? $"{this.Value.GetWholePart()} ps" : $"{( Decimal )this.Value} ps";
        }

        public static Picoseconds Combine(Picoseconds left, Picoseconds right) => Combine( left, right.Value );

        public static Picoseconds Combine(Picoseconds left, BigRational picoseconds ) => new Picoseconds( left.Value + picoseconds );

        /// <summary>
        /// <para>static equality test</para></summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals(Picoseconds left, Picoseconds right) => left.Value == right.Value;

        public static implicit operator Femtoseconds(Picoseconds picoseconds) => picoseconds.ToFemtoseconds();

        public static implicit operator Nanoseconds(Picoseconds picoseconds) => picoseconds.ToNanoseconds();

        public static Picoseconds operator -(Picoseconds nanoseconds) => new Picoseconds( nanoseconds.Value * -1 );

        public static Picoseconds operator -(Picoseconds left, Picoseconds right) => Combine( left, -right );

        public static Picoseconds operator -(Picoseconds left, Decimal nanoseconds) => Combine( left, -nanoseconds );

        public static Boolean operator !=(Picoseconds left, Picoseconds right) => !Equals( left, right );

        public static Picoseconds operator +(Picoseconds left, Picoseconds right) => Combine( left, right );

        public static Picoseconds operator +(Picoseconds left, Decimal nanoseconds) => Combine( left, nanoseconds );

        public static Boolean operator <(Picoseconds left, Picoseconds right) => left.Value < right.Value;

        public static Boolean operator ==(Picoseconds left, Picoseconds right) => Equals( left, right );

        public static Boolean operator >(Picoseconds left, Picoseconds right) => left.Value > right.Value;

        public Boolean Equals(Picoseconds other) => Equals( this, other );

        public override Boolean Equals(Object obj) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Picoseconds && this.Equals( ( Picoseconds )obj );
        }

        public Femtoseconds ToFemtoseconds() => new Femtoseconds( this.Value * Femtoseconds.InOnePicosecond );

        public Nanoseconds ToNanoseconds() => new Nanoseconds( this.Value / InOneNanosecond );
    }
}