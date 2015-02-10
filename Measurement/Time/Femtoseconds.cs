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
// "Librainian/Femtoseconds.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

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
    /// <seealso cref="http://wikipedia.org/wiki/Femtosecond" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    [Immutable]
    public struct Femtoseconds : IComparable<Femtoseconds>, IQuantityOfTime {

        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOnePicosecond = 1000;

        /// <summary>
        ///     Ten <see cref="Femtoseconds" />s.
        /// </summary>
        public static readonly Femtoseconds Fifteen = new Femtoseconds( value: 15 );

        /// <summary>
        ///     Five <see cref="Femtoseconds" />s.
        /// </summary>
        public static readonly Femtoseconds Five = new Femtoseconds( value: 5 );

        /// <summary>
        ///     Five Hundred <see cref="Femtoseconds" />s.
        /// </summary>
        public static readonly Femtoseconds FiveHundred = new Femtoseconds( value: 500 );

        /// <summary>
        ///     One <see cref="Femtoseconds" />.
        /// </summary>
        public static readonly Femtoseconds One = new Femtoseconds( value: 1 );

        /// <summary>
        ///     One Thousand Nine <see cref="Femtoseconds" /> (Prime).
        /// </summary>
        public static readonly Femtoseconds OneThousandNine = new Femtoseconds( value: 1009 );

        /// <summary>
        ///     Sixteen <see cref="Femtoseconds" />.
        /// </summary>
        public static readonly Femtoseconds Sixteen = new Femtoseconds( value: 16 );

        /// <summary>
        ///     Ten <see cref="Femtoseconds" />s.
        /// </summary>
        public static readonly Femtoseconds Ten = new Femtoseconds( value: 10 );

        /// <summary>
        ///     Three <see cref="Femtoseconds" />s.
        /// </summary>
        public static readonly Femtoseconds Three = new Femtoseconds( value: 3 );

        /// <summary>
        ///     Three Three Three <see cref="Femtoseconds" />.
        /// </summary>
        public static readonly Femtoseconds ThreeHundredThirtyThree = new Femtoseconds( value: 333 );

        /// <summary>
        ///     Two <see cref="Femtoseconds" />s.
        /// </summary>
        public static readonly Femtoseconds Two = new Femtoseconds( value: 2 );

        /// <summary>
        ///     Two Hundred <see cref="Femtoseconds" />.
        /// </summary>
        public static readonly Femtoseconds TwoHundred = new Femtoseconds( value: 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="Femtoseconds" /> (Prime).
        /// </summary>
        public static readonly Femtoseconds TwoHundredEleven = new Femtoseconds( value: 211 );

        /// <summary>
        ///     Two Thousand Three <see cref="Femtoseconds" /> (Prime).
        /// </summary>
        public static readonly Femtoseconds TwoThousandThree = new Femtoseconds( value: 2003 );

        /// <summary>
        ///     Zero <see cref="Femtoseconds" />.
        /// </summary>
        public static readonly Femtoseconds Zero = new Femtoseconds( value: 0 );

		[DataMember]
		public BigDecimal Value { get; }

		static Femtoseconds() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeGreaterThan( Attoseconds.One );
            One.Should().BeLessThan( Picoseconds.One );
        }

        public Femtoseconds( Decimal value ) {
            this.Value = value;
        }

        public Femtoseconds( BigDecimal value ) {
            this.Value = value;
        }

        public Femtoseconds( long value ) {
            this.Value = value;
        }

        public Femtoseconds( BigInteger value ) {
            this.Value = value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay => this.ToString();

        public static Femtoseconds Combine( Femtoseconds left, Femtoseconds right ) => Combine( left, right.Value );

        public static Femtoseconds Combine( Femtoseconds left, BigDecimal femtoseconds ) => new Femtoseconds( left.Value + femtoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Femtoseconds left, Femtoseconds right ) => left.Value == right.Value;

        public static implicit operator Attoseconds( Femtoseconds femtoseconds ) => femtoseconds.ToAttoseconds();

        public static implicit operator Picoseconds( Femtoseconds femtoseconds ) => femtoseconds.ToPicoseconds();

        public static implicit operator Span( Femtoseconds femtoseconds ) => new Span( femtoseconds: femtoseconds );

		public static Femtoseconds operator -( Femtoseconds femtoseconds ) => new Femtoseconds( femtoseconds.Value * -1 );

        public static Femtoseconds operator -( Femtoseconds left, Femtoseconds right ) => Combine( left, -right );

        public static Femtoseconds operator -( Femtoseconds left, Decimal femtoseconds ) => Combine( left, -femtoseconds );

        public static Boolean operator !=( Femtoseconds left, Femtoseconds right ) => !Equals( left, right );

        public static Femtoseconds operator +( Femtoseconds left, Femtoseconds right ) => Combine( left, right );

        public static Femtoseconds operator +( Femtoseconds left, Decimal femtoseconds ) => Combine( left, femtoseconds );

        public static Boolean operator <( Femtoseconds left, Femtoseconds right ) => left.Value < right.Value;

        public static Boolean operator ==( Femtoseconds left, Femtoseconds right ) => Equals( left, right );

        public static Boolean operator >( Femtoseconds left, Femtoseconds right ) => left.Value > right.Value;

        public int CompareTo( Femtoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Femtoseconds other ) => Equals( this, other );

        public override Boolean Equals( [CanBeNull] object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Femtoseconds && this.Equals( ( Femtoseconds )obj );
        }

        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>
        ///     Convert to a smaller unit.
        /// </summary>
        /// <returns></returns>
        public Attoseconds ToAttoseconds() => new Attoseconds( this.Value * Attoseconds.InOneFemtosecond );

        /// <summary>
        ///     Convert to a larger unit.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public Picoseconds ToPicoseconds() => new Picoseconds( this.Value / InOnePicosecond );

	    [ Pure ]
	    public PlanckTimes ToPlanckTimes() => new PlanckTimes( this.Value * PlanckTimes.InOneFemtosecond ) ;

        [Pure]
        public override String ToString() => String.Format( "{0} fs", this.Value );
    }
}