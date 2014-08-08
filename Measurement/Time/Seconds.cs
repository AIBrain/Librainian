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
// "Librainian2/Seconds.cs" was last cleaned by Rick on 2014/08/08 at 2:29 PM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using Parsing;

    /// <summary>
    ///     <para>
    ///         Under the International System of Units, since 1967 the second has been defined as the duration of 9192631770
    ///         periods of the radiation corresponding to the transition between the two hyperfine levels of the ground state
    ///         of the caesium 133 atom.
    ///     </para>
    /// </summary>
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public struct Seconds : IComparable< Seconds > {
        /// <summary>
        ///     60
        /// </summary>
        public const Byte InOneMinute = 60;

        /// <summary>
        ///     <see cref="Five" /> <see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Five = new Seconds( value: 5 );

        /// <summary>
        ///     <see cref="One" /> <see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds One = new Seconds( value: 1 );

        /// <summary>
        ///     <see cref="Seven" /> <see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Seven = new Seconds( value: 7 );

        /// <summary>
        ///     <see cref="Ten" /> <see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Ten = new Seconds( value: 10 );

        /// <summary>
        ///     <see cref="Thirteen" /> <see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Thirteen = new Seconds( value: 13 );

        /// <summary>
        ///     <see cref="Thirty" /> <see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Thirty = new Seconds( value: 30 );

        /// <summary>
        ///     <see cref="Three" /> <see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Three = new Seconds( value: 3 );

        /// <summary>
        ///     <see cref="Two" /> <see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Two = new Seconds( value: 2 );

        /// <summary>
        /// </summary>
        public static readonly Seconds Zero = new Seconds( value: 0 );

        [DataMember] public readonly Decimal Value;

        static Seconds() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Minutes.One );
            One.Should().BeGreaterThan( Milliseconds.One );
        }

        public Seconds( Decimal value ) {
            this.Value = value;
        }

        public Seconds( long value ) {
            this.Value = value;
        }

        public Seconds( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal ) value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( Seconds other ) {
            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( Seconds other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Seconds && this.Equals( ( Seconds ) obj );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneSecond, new BigInteger( this.Value ) );
        }

        public static Seconds Combine( Seconds left, Seconds right ) {
            return Combine( left, right.Value );
        }

        public static Seconds Combine( Seconds left, Decimal seconds ) {
            return new Seconds( left.Value + seconds );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Seconds left, Seconds right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( Seconds left, Seconds right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( Seconds left, Seconds right ) {
            return !Equals( left, right );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="seconds" /> to <see cref="Milliseconds" />.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static implicit operator Milliseconds( Seconds seconds ) {
            return ToMilliseconds( seconds );
        }

        /// <summary>
        ///     Implicitly convert  the number of <paramref name="seconds" /> to <see cref="Minutes" />.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static implicit operator Minutes( Seconds seconds ) {
            return ToMinutes( seconds );
        }

        public static implicit operator Span( Seconds seconds ) {
            return new Span( seconds );
        }

        public static implicit operator TimeSpan( Seconds seconds ) {
            return TimeSpan.FromSeconds( ( Double ) seconds.Value );
        }

        public static Seconds operator -( Seconds seconds ) {
            return new Seconds( seconds.Value*-1 );
        }

        public static Seconds operator -( Seconds left, Seconds right ) {
            return Combine( left: left, right: -right );
        }

        public static Seconds operator -( Seconds left, Decimal seconds ) {
            return Combine( left, -seconds );
        }

        public static Seconds operator +( Seconds left, Seconds right ) {
            return Combine( left, right );
        }

        public static Seconds operator +( Seconds left, Decimal seconds ) {
            return Combine( left, seconds );
        }

        public static Seconds operator +( Seconds left, BigInteger seconds ) {
            return Combine( left, seconds );
        }

        public static Seconds Combine( Seconds left, BigInteger seconds ) {
            return new Seconds( ( BigInteger ) left.Value + seconds );
        }

        public static Boolean operator <( Seconds left, Seconds right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( Seconds left, Milliseconds right ) {
            return left < ( Seconds ) right;
        }

        public static Boolean operator <( Seconds left, Minutes right ) {
            return ( Minutes ) left < right;
        }

        public static Boolean operator >( Seconds left, Minutes right ) {
            return ( Minutes ) left > right;
        }

        public static Boolean operator >( Seconds left, Seconds right ) {
            return left.Value > right.Value;
        }

        public static Boolean operator >( Seconds left, Milliseconds right ) {
            return left > ( Seconds ) right;
        }

        public static Milliseconds ToMilliseconds( Seconds seconds ) {
            return new Milliseconds( seconds.Value*Milliseconds.InOneSecond );
        }

        public static Minutes ToMinutes( Seconds seconds ) {
            return new Minutes( value: seconds.Value/InOneMinute );
        }

        public static BigInteger ToPlanckTimes( Seconds seconds ) {
            return BigInteger.Multiply( PlanckTimes.InOneSecond, new BigInteger( seconds.Value ) );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public override string ToString() {
            return this.Value.PluralOf( "second" );
        }
    }
}
