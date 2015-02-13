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
// "Librainian/Yoctoseconds.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

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

	/// <summary>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Yoctosecond" />
    [DataContract( IsReference = true )]
	// ReSharper disable once UseNameofExpression
	[DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Immutable]
    public struct Yoctoseconds : IComparable<Yoctoseconds>, IQuantityOfTime {

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

	    [ DataMember ]
	    public BigDecimal Value { get; }

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

		public Yoctoseconds( BigDecimal value ) {
            this.Value = value;
        }

        public Yoctoseconds( long value ) {
            this.Value = value;
        }

        public Yoctoseconds( BigInteger value ) {
            this.Value = value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay => this.ToString();

        public static Yoctoseconds Combine( Yoctoseconds left, Yoctoseconds right ) => Combine( left, right.Value );

        public static Yoctoseconds Combine( Yoctoseconds left, BigDecimal yoctoseconds ) => new Yoctoseconds( left.Value + yoctoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Yoctoseconds left, Yoctoseconds right ) => left.Value == right.Value;

        /// <summary>
        ///     Implicitly convert  the number of <paramref name="yoctoseconds" /> to <see cref="PlanckTimes" />.
        /// </summary>
        /// <param name="yoctoseconds"></param>
        /// <returns></returns>
        public static implicit operator PlanckTimes( Yoctoseconds yoctoseconds ) => ToPlanckTimes( yoctoseconds );

        public static implicit operator Span( Yoctoseconds yoctoseconds ) => new Span( yoctoseconds: yoctoseconds );

        /// <summary>
        ///     Implicitly convert the number of <paramref name="yoctoseconds" /> to <see cref="Zeptoseconds" />.
        /// </summary>
        /// <param name="yoctoseconds"></param>
        /// <returns></returns>
        public static implicit operator Zeptoseconds( Yoctoseconds yoctoseconds ) => yoctoseconds.ToZeptoseconds();

        public static Yoctoseconds operator -( Yoctoseconds yoctoseconds ) => new Yoctoseconds( yoctoseconds.Value * -1 );

        public static Yoctoseconds operator -( Yoctoseconds left, Yoctoseconds right ) => Combine( left: left, right: -right );

        public static Yoctoseconds operator -( Yoctoseconds left, Decimal seconds ) => Combine( left, -seconds );

        public static Boolean operator !=( Yoctoseconds left, Yoctoseconds right ) => !Equals( left, right );

        public static Yoctoseconds operator +( Yoctoseconds left, Yoctoseconds right ) => Combine( left, right );

        public static Yoctoseconds operator +( Yoctoseconds left, Decimal yoctoseconds ) => Combine( left, yoctoseconds );

        public static Boolean operator <( Yoctoseconds left, Yoctoseconds right ) => left.Value < right.Value;

        public static Boolean operator ==( Yoctoseconds left, Yoctoseconds right ) => Equals( left, right );

        public static Boolean operator >( Yoctoseconds left, Yoctoseconds right ) => left.Value > right.Value;

		public static PlanckTimes ToPlanckTimes( Yoctoseconds yoctoseconds ) => new PlanckTimes( PlanckTimes.InOneYoctosecond * yoctoseconds.Value );

        public int CompareTo( Yoctoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Yoctoseconds other ) => Equals( this, other );

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Yoctoseconds && this.Equals( ( Yoctoseconds )obj );
        }

        [Pure]
        public override int GetHashCode() => this.Value.GetHashCode();

	    [ Pure ]
	    public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneYoctosecond *  this.Value );

		//TODO
		//[Pure]public Seconds ToSeconds() => new Seconds( this.Value / Seconds. );


		[Pure]
        public override String ToString() => String.Format( "{0} ys", this.Value );

        [Pure]
        public Zeptoseconds ToZeptoseconds() => new Zeptoseconds( this.Value / InOneZeptosecond );
    }
}