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
// "Librainian/Nanoseconds.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    public struct Nanoseconds : IComparable< Nanoseconds >, IQuantityOfTime {
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

        [DataMember] public readonly Decimal Value;

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
            this.Value = ( Decimal ) value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( Nanoseconds other ) {
            return this.Value.CompareTo( other.Value );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneNanosecond, new BigInteger( this.Value ) );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public override string ToString() {
            return String.Format( "{0} ns", this.Value );
        }

        public Boolean Equals( Nanoseconds other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Nanoseconds && this.Equals( ( Nanoseconds ) obj );
        }

        public static Nanoseconds Combine( Nanoseconds left, Nanoseconds right ) {
            return Combine( left, right.Value );
        }

        public static Nanoseconds Combine( Nanoseconds left, Decimal nanoseconds ) {
            return new Nanoseconds( left.Value + nanoseconds );
        }

        public static implicit operator Microseconds( Nanoseconds nanoseconds ) {
            return new Microseconds( nanoseconds.Value/InOneMicrosecond );
        }

        public static implicit operator Picoseconds( Nanoseconds nanoseconds ) {
            return nanoseconds.ToPicoseconds();
        }

        public static Nanoseconds operator -( Nanoseconds nanoseconds ) {
            return new Nanoseconds( nanoseconds.Value*-1 );
        }

        public static Nanoseconds operator -( Nanoseconds left, Nanoseconds right ) {
            return Combine( left, -right );
        }

        public static Nanoseconds operator -( Nanoseconds left, Decimal nanoseconds ) {
            return Combine( left, -nanoseconds );
        }

        public static Nanoseconds operator +( Nanoseconds left, Nanoseconds right ) {
            return Combine( left, right );
        }

        public static Nanoseconds operator +( Nanoseconds left, Decimal nanoseconds ) {
            return Combine( left, nanoseconds );
        }

        public static Nanoseconds operator +( Nanoseconds left, BigInteger nanoseconds ) {
            return Combine( left, nanoseconds );
        }

        public static Nanoseconds Combine( Nanoseconds left, BigInteger nanoseconds ) {
            return new Nanoseconds( ( BigInteger ) left.Value + nanoseconds );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Nanoseconds left, Nanoseconds right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( Nanoseconds left, Nanoseconds right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( Nanoseconds left, Nanoseconds right ) {
            return !Equals( left, right );
        }

        public static Boolean operator <( Nanoseconds left, Nanoseconds right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( Nanoseconds left, Microseconds right ) {
            return ( Microseconds ) left < right;
        }

        public static Boolean operator >( Nanoseconds left, Nanoseconds right ) {
            return left.Value > right.Value;
        }

        public static Boolean operator >( Nanoseconds left, Microseconds right ) {
            return ( Microseconds ) left > right;
        }

        public Picoseconds ToPicoseconds() {
            return new Picoseconds( Value*Picoseconds.InOneNanosecond );
        }

        public Microseconds ToMicroseconds() {
            return new Microseconds( Value/InOneMicrosecond );
        }
    }
}
