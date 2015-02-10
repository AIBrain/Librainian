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
// "Librainian/Months.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Librainian.Extensions;
    using Maths;
    using Parsing;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Immutable]
    public struct Months : IComparable<Months>, IQuantityOfTime {

        /// <summary>
        ///     12
        /// </summary>
        public const Byte InOneCommonYear = 12;

        /// <summary>
        ///     One <see cref="Months" /> .
        /// </summary>
        public static readonly Months One = new Months( 1 );

        /// <summary>
        /// </summary>
        public static readonly Months Ten = new Months( 10 );

        /// <summary>
        /// </summary>
        public static readonly Months Thousand = new Months( 1000 );

        /// <summary>
        ///     Zero <see cref="Months" />
        /// </summary>
        public static readonly Months Zero = new Months( 0 );

	    [ DataMember ]
	    public BigDecimal Value { get; }

	    static Months() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Years.One );
        }

        public Months( Decimal value ) {
            this.Value = value;
        }

        public Months( BigDecimal value ) {
            this.Value = value;
        }

        public Months( BigInteger value ) {
            this.Value = value;
        }

        private Months( int value ) {
            this.Value = value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay => this.ToString();

        public static Months Combine( Months left, Months right ) => Combine( left, right.Value );

        public static Months Combine( Months left, BigDecimal months ) => new Months( left.Value + months );

        public static Months Combine( Months left, BigInteger months ) => new Months( left.Value + months );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Months left, Months right ) => left.Value == right.Value;

        public static implicit operator Span( Months months ) => new Span( months: months.Value );

        public static implicit operator Weeks( Months months ) => months.ToWeeks();

        public static Months operator -( Months days ) => new Months( days.Value * -1 );

        public static Months operator -( Months left, Months right ) => Combine( left: left, right: -right );

        public static Months operator -( Months left, Decimal months ) => Combine( left, -months );

        public static Boolean operator !=( Months left, Months right ) => !Equals( left, right );

        public static Months operator +( Months left, Months right ) => Combine( left, right );

        public static Months operator +( Months left, BigDecimal months ) => Combine( left, months );

        public static Boolean operator <( Months left, Months right ) => left.Value < right.Value;

        public static Boolean operator ==( Months left, Months right ) => Equals( left, right );

        public static Boolean operator >( Months left, Months right ) => left.Value > right.Value;

        public int CompareTo( Months other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Months other ) => Equals( this, other );

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Months && this.Equals( ( Months )obj );
        }

        public override int GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public PlanckTimes ToPlanckTimes() =>  new PlanckTimes( PlanckTimes.InOneMonth *  this.Value )  ;

		[Pure]
		public Seconds ToSeconds() => new Seconds( this.Value * Seconds.InOneMonth );

		[Pure]
        public override String ToString() => String.Format( "{0} {1}", this.Value, this.Value.PluralOf( "month" ) );

        //public static implicit operator Years( Months months ) => months.ToYears();

        [Pure]public Weeks ToWeeks() => new Weeks( this.Value * Weeks.InOneMonth );

        //[Pure]public Years ToYears() => new Years( this.Value / InOneCommonYear );
    }
}