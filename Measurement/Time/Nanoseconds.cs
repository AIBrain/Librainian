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
// "Librainian/Nanoseconds.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Librainian.Extensions;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    [Immutable]
    public struct Nanoseconds : IComparable<Nanoseconds>, IQuantityOfTime {

        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOneMicrosecond = 1000;

        /// <summary>
        ///     Ten <see cref="Nanoseconds" />s.
        /// </summary>
        public static readonly Nanoseconds Fifteen = new Nanoseconds( value: 15 );

        /// <summary>
        ///     Five <see cref="Nanoseconds" />s.
        /// </summary>
        public static readonly Nanoseconds Five = new Nanoseconds( value: 5 );

        /// <summary>
        ///     Five Hundred <see cref="Nanoseconds" />s.
        /// </summary>
        public static readonly Nanoseconds FiveHundred = new Nanoseconds( value: 500 );

        /// <summary>
        ///     One <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds One = new Nanoseconds( value: 1 );

        /// <summary>
        ///     One Thousand Nine <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static readonly Nanoseconds OneThousandNine = new Nanoseconds( value: 1009 );

        /// <summary>
        ///     Sixteen <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds Sixteen = new Nanoseconds( value: 16 );

        /// <summary>
        ///     Ten <see cref="Nanoseconds" />s.
        /// </summary>
        public static readonly Nanoseconds Ten = new Nanoseconds( value: 10 );

        /// <summary>
        ///     Three <see cref="Nanoseconds" />s.
        /// </summary>
        public static readonly Nanoseconds Three = new Nanoseconds( value: 3 );

        /// <summary>
        ///     Three Three Three <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds ThreeHundredThirtyThree = new Nanoseconds( value: 333 );

        /// <summary>
        ///     Two <see cref="Nanoseconds" />s.
        /// </summary>
        public static readonly Nanoseconds Two = new Nanoseconds( value: 2 );

        /// <summary>
        ///     Two Hundred <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds TwoHundred = new Nanoseconds( value: 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static readonly Nanoseconds TwoHundredEleven = new Nanoseconds( value: 211 );

        /// <summary>
        ///     Two Thousand Three <see cref="Nanoseconds" /> (Prime).
        /// </summary>
        public static readonly Nanoseconds TwoThousandThree = new Nanoseconds( value: 2003 );

        /// <summary>
        ///     Zero <see cref="Nanoseconds" />.
        /// </summary>
        public static readonly Nanoseconds Zero = new Nanoseconds( value: 0 );

        [DataMember]
        public readonly Decimal Value;

        static Nanoseconds() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Microseconds.One );
            One.Should().BeGreaterThan( Picoseconds.One );
        }

        public Nanoseconds( Decimal value ) {
            this.Value = value;
        }

        public Nanoseconds( long value ) {
            this.Value = value;
        }

        public Nanoseconds( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal )value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay => this.ToString();

        public static Nanoseconds Combine( Nanoseconds left, Nanoseconds right ) => Combine( left, right.Value );

        public static Nanoseconds Combine( Nanoseconds left, Decimal nanoseconds ) => new Nanoseconds( left.Value + nanoseconds );

        public static Nanoseconds Combine( Nanoseconds left, BigInteger nanoseconds ) => new Nanoseconds( ( BigInteger )left.Value + nanoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Nanoseconds left, Nanoseconds right ) => left.Value == right.Value;

        public static implicit operator Microseconds( Nanoseconds nanoseconds ) => new Microseconds( nanoseconds.Value / InOneMicrosecond );

        public static implicit operator Picoseconds( Nanoseconds nanoseconds ) => nanoseconds.ToPicoseconds();

        public static Nanoseconds operator -( Nanoseconds nanoseconds ) => new Nanoseconds( nanoseconds.Value * -1 );

        public static Nanoseconds operator -( Nanoseconds left, Nanoseconds right ) => Combine( left, -right );

        public static Nanoseconds operator -( Nanoseconds left, Decimal nanoseconds ) => Combine( left, -nanoseconds );

        public static Boolean operator !=( Nanoseconds left, Nanoseconds right ) => !Equals( left, right );

        public static Nanoseconds operator +( Nanoseconds left, Nanoseconds right ) => Combine( left, right );

        public static Nanoseconds operator +( Nanoseconds left, Decimal nanoseconds ) => Combine( left, nanoseconds );

        public static Nanoseconds operator +( Nanoseconds left, BigInteger nanoseconds ) => Combine( left, nanoseconds );

        public static Boolean operator <( Nanoseconds left, Nanoseconds right ) => left.Value < right.Value;

        public static Boolean operator <( Nanoseconds left, Microseconds right ) => ( Microseconds )left < right;

        public static Boolean operator ==( Nanoseconds left, Nanoseconds right ) => Equals( left, right );

        public static Boolean operator >( Nanoseconds left, Nanoseconds right ) => left.Value > right.Value;

        public static Boolean operator >( Nanoseconds left, Microseconds right ) => ( Microseconds )left > right;

        public int CompareTo( Nanoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Nanoseconds other ) => Equals( this, other );

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Nanoseconds && this.Equals( ( Nanoseconds )obj );
        }

        public override int GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public Microseconds ToMicroseconds() => new Microseconds( this.Value / InOneMicrosecond );

        public Picoseconds ToPicoseconds() => new Picoseconds( this.Value * Picoseconds.InOneNanosecond );

        [Pure]
        public BigInteger ToPlanckTimes() => BigInteger.Multiply( PlanckTimes.InOneNanosecond, new BigInteger( this.Value ) );

        [Pure]
        public override String ToString() => String.Format( "{0} ns", this.Value );
    }
}