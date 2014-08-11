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
// "Librainian/Yoctoseconds.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;

    /// <summary>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Yoctosecond" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public struct Yoctoseconds : IComparable< Yoctoseconds >, IQuantityOfTime {
        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOneZeptosecond = 1000;

        /// <summary>
        ///     <see cref="Five" /> <see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Five = new Yoctoseconds( 5 );

        /// <summary>
        ///     <see cref="One" /> <see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds One = new Yoctoseconds( 1 );

        /// <summary>
        ///     <see cref="Seven" /> <see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Seven = new Yoctoseconds( 7 );

        /// <summary>
        ///     <see cref="Ten" /> <see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Ten = new Yoctoseconds( 10 );

        /// <summary>
        ///     <see cref="Thirteen" /> <see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Thirteen = new Yoctoseconds( 13 );

        /// <summary>
        ///     <see cref="Thirty" /> <see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Thirty = new Yoctoseconds( 30 );

        /// <summary>
        ///     <see cref="Three" /> <see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Three = new Yoctoseconds( 3 );

        /// <summary>
        ///     <see cref="Two" /> <see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Two = new Yoctoseconds( 2 );

        /// <summary>
        /// </summary>
        public static readonly Yoctoseconds Zero = new Yoctoseconds( 0 );

        [DataMember] public readonly Decimal Value;

        static Yoctoseconds() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeGreaterThan( PlanckTimes.One );
            One.Should().BeLessThan( Zeptoseconds.One );
        }

        public Yoctoseconds( Decimal value ) {
            this.Value = value;
        }

        public Yoctoseconds( long value ) {
            this.Value = value;
        }

        public Yoctoseconds( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal ) value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( Yoctoseconds other ) {
            return this.Value.CompareTo( other.Value );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneYoctosecond, new BigInteger( this.Value ) );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public override string ToString() {
            return String.Format( "{0:R} ys", this.Value );
        }

        public Boolean Equals( Yoctoseconds other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Yoctoseconds && this.Equals( ( Yoctoseconds ) obj );
        }

        public static Yoctoseconds Combine( Yoctoseconds left, Yoctoseconds right ) {
            return Combine( left, right.Value );
        }

        public static Yoctoseconds Combine( Yoctoseconds left, Decimal yoctoseconds ) {
            return new Yoctoseconds( left.Value + yoctoseconds );
        }

        /// <summary>
        ///     Implicitly convert  the number of <paramref name="yoctoseconds" /> to <see cref="PlanckTimes" />.
        /// </summary>
        /// <param name="yoctoseconds"></param>
        /// <returns></returns>
        public static implicit operator PlanckTimes( Yoctoseconds yoctoseconds ) {
            return ToPlanckTimes( yoctoseconds );
        }

        public static implicit operator Span( Yoctoseconds yoctoseconds ) {
            return new Span( yoctoseconds );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="yoctoseconds" /> to <see cref="Zeptoseconds" />.
        /// </summary>
        /// <param name="yoctoseconds"></param>
        /// <returns></returns>
        public static implicit operator Zeptoseconds( Yoctoseconds yoctoseconds ) {
            return ToZeptoseconds( yoctoseconds );
        }

        public static Yoctoseconds operator -( Yoctoseconds yoctoseconds ) {
            return new Yoctoseconds( yoctoseconds.Value*-1 );
        }

        public static Yoctoseconds operator -( Yoctoseconds left, Yoctoseconds right ) {
            return Combine( left: left, right: -right );
        }

        public static Yoctoseconds operator -( Yoctoseconds left, Decimal seconds ) {
            return Combine( left, -seconds );
        }

        public static Yoctoseconds operator +( Yoctoseconds left, Yoctoseconds right ) {
            return Combine( left, right );
        }

        public static Yoctoseconds operator +( Yoctoseconds left, Decimal yoctoseconds ) {
            return Combine( left, yoctoseconds );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Yoctoseconds left, Yoctoseconds right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( Yoctoseconds left, Yoctoseconds right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( Yoctoseconds left, Yoctoseconds right ) {
            return !Equals( left, right );
        }

        public static Boolean operator <( Yoctoseconds left, Yoctoseconds right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator >( Yoctoseconds left, Yoctoseconds right ) {
            return left.Value > right.Value;
        }

        public static PlanckTimes ToPlanckTimes( Yoctoseconds yoctoseconds ) {
            return new PlanckTimes( BigInteger.Multiply( PlanckTimes.InOneYoctosecond, new BigInteger( yoctoseconds.Value ) ) );
        }

        public static Zeptoseconds ToZeptoseconds( Yoctoseconds yoctoseconds ) {
            return new Zeptoseconds( yoctoseconds.Value/InOneZeptosecond );
        }
    }
}
