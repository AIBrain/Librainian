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
// "Librainian/Picoseconds.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
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
    public struct Picoseconds : IComparable< Picoseconds >, IQuantityOfTime {
        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOneNanosecond = 1000;

        /// <summary>
        ///     Ten <see cref="Picoseconds" />s.
        /// </summary>
        public static readonly Picoseconds Fifteen = new Picoseconds( 15 );

        /// <summary>
        ///     Five <see cref="Picoseconds" />s.
        /// </summary>
        public static readonly Picoseconds Five = new Picoseconds( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Picoseconds" />s.
        /// </summary>
        public static readonly Picoseconds FiveHundred = new Picoseconds( 500 );

        /// <summary>
        ///     One <see cref="Picoseconds" />.
        /// </summary>
        public static readonly Picoseconds One = new Picoseconds( 1 );

        /// <summary>
        ///     One Thousand Nine <see cref="Picoseconds" /> (Prime).
        /// </summary>
        public static readonly Picoseconds OneThousandNine = new Picoseconds( 1009 );

        /// <summary>
        ///     Sixteen <see cref="Picoseconds" />.
        /// </summary>
        public static readonly Picoseconds Sixteen = new Picoseconds( 16 );

        /// <summary>
        ///     Ten <see cref="Picoseconds" />s.
        /// </summary>
        public static readonly Picoseconds Ten = new Picoseconds( 10 );

        /// <summary>
        ///     Three <see cref="Picoseconds" />s.
        /// </summary>
        public static readonly Picoseconds Three = new Picoseconds( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Picoseconds" />.
        /// </summary>
        public static readonly Picoseconds ThreeHundredThirtyThree = new Picoseconds( 333 );

        /// <summary>
        ///     Two <see cref="Picoseconds" />s.
        /// </summary>
        public static readonly Picoseconds Two = new Picoseconds( 2 );

        /// <summary>
        ///     Two Hundred <see cref="Picoseconds" />.
        /// </summary>
        public static readonly Picoseconds TwoHundred = new Picoseconds( 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="Picoseconds" /> (Prime).
        /// </summary>
        public static readonly Picoseconds TwoHundredEleven = new Picoseconds( 211 );

        /// <summary>
        ///     Two Thousand Three <see cref="Picoseconds" /> (Prime).
        /// </summary>
        public static readonly Picoseconds TwoThousandThree = new Picoseconds( 2003 );

        /// <summary>
        ///     Zero <see cref="Picoseconds" />.
        /// </summary>
        public static readonly Picoseconds Zero = new Picoseconds( 0 );

        [DataMember] public readonly Decimal Value;

        static Picoseconds() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Nanoseconds.One );
            One.Should().BeGreaterThan( Femtoseconds.One );
        }

        public Picoseconds( Decimal value ) {
            this.Value = value;
        }

        public Picoseconds( long value ) {
            this.Value = value;
        }

        public Picoseconds( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal ) value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( Picoseconds other ) {
            return this.Value.CompareTo( other.Value );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOnePicosecond, new BigInteger( this.Value ) );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public override string ToString() {
            return String.Format( "{0:R} ps", this.Value );
        }

        public Boolean Equals( Picoseconds other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Picoseconds && this.Equals( ( Picoseconds ) obj );
        }

        public static Picoseconds Combine( Picoseconds left, Picoseconds right ) {
            return Combine( left, right.Value );
        }

        public static Picoseconds Combine( Picoseconds left, Decimal picoseconds ) {
            return new Picoseconds( left.Value + picoseconds );
        }

        public static implicit operator Femtoseconds( Picoseconds picoseconds ) {
            return ToFemtoseconds( picoseconds );
        }

        public static implicit operator Nanoseconds( Picoseconds picoseconds ) {
            return ToNanoseconds( picoseconds );
        }

        public static Picoseconds operator -( Picoseconds nanoseconds ) {
            return new Picoseconds( nanoseconds.Value*-1 );
        }

        public static Picoseconds operator -( Picoseconds left, Picoseconds right ) {
            return Combine( left, -right );
        }

        public static Picoseconds operator -( Picoseconds left, Decimal nanoseconds ) {
            return Combine( left, -nanoseconds );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Picoseconds left, Picoseconds right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( Picoseconds left, Picoseconds right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( Picoseconds left, Picoseconds right ) {
            return !Equals( left, right );
        }

        public static Picoseconds operator +( Picoseconds left, Picoseconds right ) {
            return Combine( left, right );
        }

        public static Picoseconds operator +( Picoseconds left, Decimal nanoseconds ) {
            return Combine( left, nanoseconds );
        }

        public static Boolean operator <( Picoseconds left, Picoseconds right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator >( Picoseconds left, Picoseconds right ) {
            return left.Value > right.Value;
        }

        public static Femtoseconds ToFemtoseconds( Picoseconds picoseconds ) {
            return new Femtoseconds( picoseconds.Value*Femtoseconds.InOnePicosecond );
        }

        public static Nanoseconds ToNanoseconds( Picoseconds picoseconds ) {
            return new Nanoseconds( picoseconds.Value/InOneNanosecond );
        }
    }
}
