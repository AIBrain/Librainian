#region License & Information

// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Microseconds.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

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

    [DataContract(IsReference = true)]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [Serializable]
    [Immutable]
    public struct Microseconds : IComparable<Microseconds>, IQuantityOfTime {

        /// <summary>
        /// 1000
        /// </summary>
        public const UInt16 InOneMillisecond = 1000;

        /// <summary>
        /// Ten <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds Fifteen = new Microseconds( 15 );

        /// <summary>
        /// Five <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds Five = new Microseconds( 5 );

        /// <summary>
        /// Five Hundred <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds FiveHundred = new Microseconds( 500 );

        /// <summary>
        /// One <see cref="Microseconds" />.
        /// </summary>
        public static readonly Microseconds One = new Microseconds( 1 );

        /// <summary>
        /// One Thousand Nine <see cref="Microseconds" /> (Prime).
        /// </summary>
        public static readonly Microseconds OneThousandNine = new Microseconds( 1009 );

        /// <summary>
        /// Sixteen <see cref="Microseconds" />.
        /// </summary>
        public static readonly Microseconds Sixteen = new Microseconds( 16 );

        /// <summary>
        /// Ten <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds Ten = new Microseconds( 10 );

        /// <summary>
        /// Three <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds Three = new Microseconds( 3 );

        /// <summary>
        /// Three Three Three <see cref="Microseconds" />.
        /// </summary>
        public static readonly Microseconds ThreeHundredThirtyThree = new Microseconds( 333 );

        /// <summary>
        /// Two <see cref="Microseconds" /> s.
        /// </summary>
        public static readonly Microseconds Two = new Microseconds( 2 );

        /// <summary>
        /// Two Hundred <see cref="Microseconds" />.
        /// </summary>
        public static readonly Microseconds TwoHundred = new Microseconds( 200 );

        /// <summary>
        /// Two Hundred Eleven <see cref="Microseconds" /> (Prime).
        /// </summary>
        public static readonly Microseconds TwoHundredEleven = new Microseconds( 211 );

        /// <summary>
        /// Two Thousand Three <see cref="Microseconds" /> (Prime).
        /// </summary>
        public static readonly Microseconds TwoThousandThree = new Microseconds( 2003 );

        /// <summary>
        /// Zero <see cref="Microseconds" />.
        /// </summary>
        public static readonly Microseconds Zero = new Microseconds( 0 );

        [DataMember]
        public BigDecimal Value { get; }

        static Microseconds( ) {
            Zero.Should( ).BeLessThan( One );
            One.Should( ).BeGreaterThan( Zero );
            One.Should( ).Be( One );
            One.Should( ).BeLessThan( Milliseconds.One );
            One.Should( ).BeGreaterThan( Nanoseconds.One );
        }

        public Microseconds( Decimal value ) {
            this.Value = value;
        }

        public Microseconds( BigDecimal value ) {
            this.Value = value;
        }

        public Microseconds( long value ) {
            this.Value = value;
        }

        public Microseconds( BigInteger value ) {
            this.Value = value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay => this.ToString( );

        public static Microseconds Combine( Microseconds left, Microseconds right ) => Combine( left, right.Value );

        public static Microseconds Combine( Microseconds left, BigDecimal microseconds ) => new Microseconds( left.Value + microseconds );

        public static Microseconds Combine( Microseconds left, BigInteger microseconds ) => new Microseconds( left.Value + microseconds );

        /// <summary>
        /// <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Microseconds left, Microseconds right ) => left.Value == right.Value;

        public static implicit operator Milliseconds( Microseconds microseconds ) => microseconds.ToMilliseconds( );

        public static implicit operator Nanoseconds( Microseconds microseconds ) => microseconds.ToNanoseconds( );

        public static implicit operator TimeSpan( Microseconds microseconds ) => TimeSpan.FromMilliseconds( value: ( double )microseconds.Value );

        public static Microseconds operator -( Microseconds milliseconds ) => new Microseconds( milliseconds.Value * -1 );

        public static Microseconds operator -( Microseconds left, Microseconds right ) => Combine( left, -right );

        public static Microseconds operator -( Microseconds left, Decimal microseconds ) => Combine( left, -microseconds );

        public static Boolean operator !=( Microseconds left, Microseconds right ) => !Equals( left, right );

        public static Microseconds operator +( Microseconds left, Microseconds right ) => Combine( left, right );

        public static Microseconds operator +( Microseconds left, Decimal microseconds ) => Combine( left, microseconds );

        public static Microseconds operator +( Microseconds left, BigInteger microseconds ) => Combine( left, microseconds );

        public static Boolean operator <( Microseconds left, Microseconds right ) => left.Value < right.Value;

        public static Boolean operator <( Microseconds left, Milliseconds right ) => ( Milliseconds )left < right;

        public static Boolean operator ==( Microseconds left, Microseconds right ) => Equals( left, right );

        public static Boolean operator >( Microseconds left, Microseconds right ) => left.Value > right.Value;

        public static Boolean operator >( Microseconds left, Milliseconds right ) => ( Milliseconds )left > right;

        public int CompareTo( Microseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Microseconds other ) => Equals( this, other );

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Microseconds && this.Equals( ( Microseconds )obj );
        }

        public override int GetHashCode( ) => this.Value.GetHashCode( );

        [Pure]
        public Milliseconds ToMilliseconds( ) => new Milliseconds( this.Value / InOneMillisecond );

        [Pure]
        public Nanoseconds ToNanoseconds( ) => new Nanoseconds( this.Value * Nanoseconds.InOneMicrosecond );

        [Pure]
        public PlanckTimes ToPlanckTimes( ) => new PlanckTimes( PlanckTimes.InOneMicrosecond * this.Value );

        [Pure]
        public override String ToString( ) => String.Format( "{0} µs", this.Value );
    }
}