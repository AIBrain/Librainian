namespace Librainian.Measurement.Length {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using Maths;
    using Parsing;
    using Time;

    /// <summary>
    ///     <para>
    ///         Under the International System of Units, since 1967 the second has been defined as the duration of 9192631770
    ///         periods of the radiation corresponding to the transition between the two hyperfine levels of the ground state
    ///         of the caesium 133 atom.
    ///     </para>
    /// </summary>
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public struct Foot : IComparable<Foot>, IQuantityOfDistance {

        /// <summary>
        ///     60
        /// </summary>
        public const Byte InOneYard = 60;

        /// <summary>
        ///     <see cref="Five" /> <see cref="Foot" />.
        /// </summary>
        public static readonly Foot Five = new Foot(  5 );

        /// <summary>
        ///     <see cref="One" /> <see cref="Foot" />.
        /// </summary>
        public static readonly Foot One = new Foot(  1 );

        /// <summary>
        ///     <see cref="Seven" /> <see cref="Foot" />.
        /// </summary>
        public static readonly Foot Seven = new Foot(  7 );

        /// <summary>
        ///     <see cref="Ten" /> <see cref="Foot" />.
        /// </summary>
        public static readonly Foot Ten = new Foot(  10 );

        /// <summary>
        ///     <see cref="Thirteen" /> <see cref="Foot" />.
        /// </summary>
        public static readonly Foot Thirteen = new Foot(  13 );

        /// <summary>
        ///     <see cref="Thirty" /> <see cref="Foot" />.
        /// </summary>
        public static readonly Foot Thirty = new Foot(  30 );

        /// <summary>
        ///     <see cref="Three" /> <see cref="Foot" />.
        /// </summary>
        public static readonly Foot Three = new Foot(  3 );

        /// <summary>
        ///     <see cref="Two" /> <see cref="Foot" />.
        /// </summary>
        public static readonly Foot Two = new Foot(  2 );

        /// <summary>
        /// </summary>
        public static readonly Foot Zero = new Foot(  0 );

        [DataMember]
        public readonly Decimal Value;

        static Foot() {
            Zero.Should< Foot >().BeLessThan( One );
            One.Should< Foot >().BeGreaterThan( Zero );
            One.Should< Foot >().Be( One );
            One.Should< Foot >().BeLessThan( Minutes.One );
            One.Should< Foot >().BeGreaterThan( Milliseconds.One );
        }

        public Foot( Decimal value ) {
            this.Value = value;
        }

        public Foot( long value ) {
            this.Value = value;
        }

        public Foot( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal )value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( Foot other ) {
            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( Foot other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Foot && this.Equals( ( Foot )obj );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            var seconds = new BigDecimal( this.Value ); //avoid overflow?
            seconds *= PlanckTimes.InOneSecond;
            return ( BigInteger ) seconds;  //gets truncated here. oh well.
        }

        public static Foot Combine( Foot left, Foot right ) {
            return Combine( ( Foot ) left, right.Value );
        }

        public static Foot Combine( Foot left, Decimal seconds ) {
            return new Foot( left.Value + seconds );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Foot left, Foot right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( Foot left, Foot right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( Foot left, Foot right ) {
            return !Equals( left, right );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="seconds" /> to <see cref="Milliseconds" />.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static implicit operator Milliseconds( Foot seconds ) {
            return seconds.ToMilliseconds();
        }

        /// <summary>
        ///     Implicitly convert  the number of <paramref name="seconds" /> to <see cref="Minutes" />.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static implicit operator Minutes( Foot seconds ) {
            return seconds.ToMinutes();
        }

        public static implicit operator Span( Foot seconds ) {
            return new Span( planckTimes: seconds.ToPlanckTimes() );
        }

        public static implicit operator TimeSpan( Foot seconds ) {
            return TimeSpan.FromSeconds( ( Double )seconds.Value );
        }

        public static Foot operator -( Foot seconds ) {
            return new Foot( seconds.Value * -1 );
        }

        public static Foot operator -( Foot left, Foot right ) {
            return Combine( left: left, right: -right );
        }

        public static Foot operator -( Foot left, Decimal seconds ) {
            return Combine( left, -seconds );
        }

        public static Foot operator +( Foot left, Foot right ) {
            return Combine( left, right );
        }

        public static Foot operator +( Foot left, Decimal seconds ) {
            return Combine( left, seconds );
        }

        public static Foot operator +( Foot left, BigInteger seconds ) {
            return Combine( ( Foot ) left, seconds );
        }

        public static Foot Combine( Foot left, BigInteger seconds ) {
            return new Foot( ( BigInteger )left.Value + seconds );
        }

        public static Boolean operator <( Foot left, Foot right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( Foot left, Milliseconds right ) {
            return left < ( Foot )right;
        }

        public static Boolean operator <( Foot left, Minutes right ) {
            return ( Minutes )left < right;
        }

        public static Boolean operator >( Foot left, Minutes right ) {
            return ( Minutes )left > right;
        }

        public static Boolean operator >( Foot left, Foot right ) {
            return left.Value > right.Value;
        }

        public static Boolean operator >( Foot left, Millimeters right ) {
            return left > ( Foot )right;
        }

        [Pure]
        public Millimeters ToMillimeters() {
            return new Millimeters( this.Value * Millimeters.InOneFoot );
        }

        [Pure]
        public Minutes ToMinutes() {
            return new Minutes( value: this.Value / InOneMinute );
        }

        [Pure]
        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public override string ToString() {
            return String.Format( "{0} {1}", this.Value, this.Value.PluralOf( "second" ) );
        }
    }

    public interface IQuantityOfDistance {
    
            int GetHashCode();

            [Pure]
            BigInteger ToPlanckTimes();

            string ToString();
        
    }
}