namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using Librainian.Extensions;
    using Parsing;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Immutable]
    public struct BillionYears : IComparable< BillionYears > {
        ///// <summary>
        /////     10
        ///// </summary>
        //public const Byte InOneMillenium = 10;

        /// <summary>
        ///     One <see cref="BillionYears" /> .
        /// </summary>
        public static readonly BillionYears One = new BillionYears( 1 );

        /// <summary>
        /// </summary>
        public static readonly BillionYears Ten = new BillionYears( 10 );

        /// <summary>
        /// </summary>
        public static readonly BillionYears Thousand = new BillionYears( 1000 );

        /// <summary>
        ///     Zero <see cref="BillionYears" />
        /// </summary>
        public static readonly BillionYears Zero = new BillionYears( 0 );

        [DataMember] public readonly Decimal Value;

        static BillionYears() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeGreaterThan( Years.One );
            One.Should().BeGreaterThan( Milleniums.One );
        }

        public BillionYears( long value ) {
            this.Value = value;
        }

        public BillionYears( Decimal value ) {
            this.Value = value;
        }

        public BillionYears( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal ) value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( BillionYears other ) {
            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( BillionYears other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( [CanBeNull] object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is BillionYears && this.Equals( ( BillionYears ) obj );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneCentury, new BigInteger( this.Value ) );
        }

        public static BillionYears Combine( BillionYears left, BillionYears right ) {
            return Combine( ( BillionYears ) left, right.Value );
        }

        public static BillionYears Combine( BillionYears left, Decimal years ) {
            return new BillionYears( left.Value + years );
        }

        public static BillionYears Combine( BillionYears left, BigInteger centuries ) {
            return new BillionYears( ( BigInteger ) left.Value + centuries );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="centuries" /> to <see cref="Milleniums" />.
        /// </summary>
        /// <param name="centuries"></param>
        /// <returns></returns>
        public static implicit operator Milleniums( BillionYears centuries ) {
            return ToMilleniums( centuries );
        }

        public static implicit operator Span( BillionYears centuries ) {
            return new Span( centuries: centuries.Value );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="centuries" /> to <see cref="Years" />.
        /// </summary>
        /// <param name="centuries"></param>
        /// <returns></returns>
        public static implicit operator Years( BillionYears centuries ) {
            return ToYears( centuries );
        }

        public static BillionYears operator -( BillionYears days ) {
            return new BillionYears( days.Value*-1 );
        }

        public static BillionYears operator -( BillionYears left, BillionYears right ) {
            return Combine( left: left, right: -right );
        }

        public static BillionYears operator -( BillionYears left, Decimal centuries ) {
            return Combine( left, -centuries );
        }

        public static BillionYears operator +( BillionYears left, BigInteger centuries ) {
            return Combine( left, centuries );
        }

        public static BillionYears operator +( BillionYears left, BillionYears right ) {
            return Combine( left, right );
        }

        public static BillionYears operator +( BillionYears left, Decimal centuries ) {
            return Combine( left, centuries );
        }

        public static Boolean operator <( BillionYears left, BillionYears right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( BillionYears left, Years years ) {
            return left <  years.ToBillionYears();
        }

        public static Boolean operator >( BillionYears left, Years years ) {
            return left > ( BillionYears ) years;
        }

        public static Boolean operator >( BillionYears left, BillionYears right ) {
            return left.Value > right.Value;
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( BillionYears left, BillionYears right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( BillionYears left, BillionYears right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( BillionYears left, BillionYears right ) {
            return !Equals( left, right );
        }

        public static Milleniums ToMilleniums( BillionYears centuries ) {
            return new Milleniums( centuries.Value/InOneMillenium );
        }

        public static BigInteger ToPlanckTimes( BillionYears centuries ) {
            return BigInteger.Multiply( PlanckTimes.InOneCentury, new BigInteger( centuries.Value ) );
        }

        public static Years ToYears( BillionYears centuries ) {
            return new Years( centuries.Value*Years.InOneCentury );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public override string ToString() {
            return this.Value.PluralOf( "century" );
        }
    }
}