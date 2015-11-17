namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Extensions;
    using FluentAssertions;
    using Numerics;
    using Parsing;

    /// <summary>
    /// https://en.wikipedia.org/wiki/Second
    /// </summary>
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{ToString(),nq}" )]
    [Serializable]
    [Immutable]
    public struct CaesiumPeriod : IComparable<CaesiumPeriod>, IQuantityOfTime {

        /// <summary>9192631770</summary>
        public const UInt64 InOneSecond = 9192631770;

        /// <summary>Ten <see cref="CaesiumPeriod" /> s.</summary>
        public static readonly CaesiumPeriod Fifteen = new CaesiumPeriod( 15 );

        /// <summary>Five <see cref="CaesiumPeriod" /> s.</summary>
        public static readonly CaesiumPeriod Five = new CaesiumPeriod( 5 );

        /// <summary>Five Hundred <see cref="CaesiumPeriod" /> s.</summary>
        public static readonly CaesiumPeriod FiveHundred = new CaesiumPeriod( 500 );

        /// <summary>One <see cref="CaesiumPeriod" />.</summary>
        public static readonly CaesiumPeriod One = new CaesiumPeriod( 1 );

        /// <summary>One Thousand Nine <see cref="CaesiumPeriod" /> (Prime).</summary>
        public static readonly CaesiumPeriod OneThousandNine = new CaesiumPeriod( 1009 );

        /// <summary>Sixteen <see cref="CaesiumPeriod" />.</summary>
        public static readonly CaesiumPeriod Sixteen = new CaesiumPeriod( 16 );

        /// <summary>Ten <see cref="CaesiumPeriod" /> s.</summary>
        public static readonly CaesiumPeriod Ten = new CaesiumPeriod( 10 );

        /// <summary>Three <see cref="CaesiumPeriod" /> s.</summary>
        public static readonly CaesiumPeriod Three = new CaesiumPeriod( 3 );

        /// <summary>Three Three Three <see cref="CaesiumPeriod" />.</summary>
        public static readonly CaesiumPeriod ThreeHundredThirtyThree = new CaesiumPeriod( 333 );

        /// <summary>Two <see cref="CaesiumPeriod" /> s.</summary>
        public static readonly CaesiumPeriod Two = new CaesiumPeriod( 2 );

        /// <summary>Two Hundred <see cref="CaesiumPeriod" />.</summary>
        public static readonly CaesiumPeriod TwoHundred = new CaesiumPeriod( 200 ); //faster WPM than a female (~240wpm)

        /// <summary>Two Hundred Eleven <see cref="CaesiumPeriod" /> (Prime).</summary>
        public static readonly CaesiumPeriod TwoHundredEleven = new CaesiumPeriod( 211 ); //faster WPM than a female (~240wpm)

        /// <summary>Two Thousand Three <see cref="CaesiumPeriod" /> (Prime).</summary>
        public static readonly CaesiumPeriod TwoThousandThree = new CaesiumPeriod( 2003 );

        /// <summary>Zero <see cref="CaesiumPeriod" />.</summary>
        public static readonly CaesiumPeriod Zero = new CaesiumPeriod( 0 );

        static CaesiumPeriod() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            //One.Should().BeLessThan( Seconds.One );
            //One.Should().BeLessThan( Microseconds.One );
        }

        public CaesiumPeriod( Decimal value ) {
            this.Value = value;
        }

        public CaesiumPeriod( BigRational value ) {
            this.Value = value;
        }

        public CaesiumPeriod( Int64 value ) {
            this.Value = value;
        }

        public CaesiumPeriod( BigInteger value ) {
            this.Value = value;
        }

        public CaesiumPeriod( Double value ) {
            this.Value = value;
        }

        [DataMember]
        public BigRational Value {
            get;
        }

        public Int32 CompareTo( CaesiumPeriod other ) => this.Value.CompareTo( other.Value );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public PlanckTimes ToPlanckTimes() {
            return new PlanckTimes( PlanckTimes.InOneMillisecond * this.Value );    //BUG InOneMillisecond is wrong
        }

        [Pure]
        public override String ToString() => $"{this.Value} {this.Value.PluralOf( "millisecond" )}";

        public static CaesiumPeriod Combine( CaesiumPeriod left, BigRational milliseconds ) => new CaesiumPeriod( left.Value + milliseconds );

        public static CaesiumPeriod Combine( CaesiumPeriod left, BigInteger milliseconds ) => new CaesiumPeriod( ( BigInteger )left.Value + milliseconds );

        /// <summary>
        /// <para>static equality test</para></summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( CaesiumPeriod left, CaesiumPeriod right ) => left.Value == right.Value;

        /// <summary>I don't prefer implicits to Double.. oh well.</summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static explicit operator Double( CaesiumPeriod milliseconds ) => ( Double )milliseconds.Value;

        public static implicit operator BigRational( CaesiumPeriod milliseconds ) => milliseconds.Value;

        /// <summary>
        /// Implicitly convert the number of <paramref name="milliseconds" /> to <see cref="Microseconds" />.
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static implicit operator Microseconds( CaesiumPeriod milliseconds ) => milliseconds.ToMicroseconds();

        public static implicit operator Seconds( CaesiumPeriod milliseconds ) => milliseconds.ToSeconds();

        public static implicit operator Span( CaesiumPeriod milliseconds ) => new Span( milliseconds: milliseconds );

        public static CaesiumPeriod operator -( CaesiumPeriod milliseconds ) => new CaesiumPeriod( milliseconds.Value * -1 );

        public static CaesiumPeriod operator -( CaesiumPeriod left, CaesiumPeriod right ) => Combine( left, -right.Value );

        public static CaesiumPeriod operator -( CaesiumPeriod left, Decimal milliseconds ) => Combine( left, -milliseconds );

        public static Boolean operator !=( CaesiumPeriod left, CaesiumPeriod right ) => !Equals( left, right );

        public static CaesiumPeriod operator +( CaesiumPeriod left, CaesiumPeriod right ) => Combine( left, right.Value );

        public static CaesiumPeriod operator +( CaesiumPeriod left, Decimal milliseconds ) => Combine( left, milliseconds );

        public static CaesiumPeriod operator +( CaesiumPeriod left, BigInteger milliseconds ) => Combine( left, milliseconds );

        public static Boolean operator <( CaesiumPeriod left, CaesiumPeriod right ) => left.Value < right.Value;

        public static Boolean operator <( CaesiumPeriod left, Seconds right ) => ( Seconds )left < right;

        public static Boolean operator ==( CaesiumPeriod left, CaesiumPeriod right ) => Equals( left, right );

        public static Boolean operator >( CaesiumPeriod left, CaesiumPeriod right ) => left.Value > right.Value;

        public static Boolean operator >( CaesiumPeriod left, Seconds right ) => ( Seconds )left > right;

        public Boolean Equals( CaesiumPeriod other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is CaesiumPeriod && this.Equals( ( CaesiumPeriod )obj );
        }

        public Microseconds ToMicroseconds() => new Microseconds( this.Value * Microseconds.InOneMillisecond );

        [Pure]
        public Seconds ToSeconds() => new Seconds( this.Value / InOneSecond );

        public static implicit operator TimeSpan( CaesiumPeriod milliseconds ) => TimeSpan.FromMilliseconds( value: ( Double )milliseconds.Value );
    }

}