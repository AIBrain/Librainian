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
// "Librainian2/Centuries.cs" was last cleaned by Rick on 2014/08/08 at 2:29 PM
#endregion

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
    public struct Centuries : IComparable< Centuries > {
        /// <summary>
        ///     10
        /// </summary>
        public const Byte InOneMillenium = 10;

        /// <summary>
        ///     One <see cref="Centuries" /> .
        /// </summary>
        public static readonly Centuries One = new Centuries( 1 );

        /// <summary>
        /// </summary>
        public static readonly Centuries Ten = new Centuries( 10 );

        /// <summary>
        /// </summary>
        public static readonly Centuries Thousand = new Centuries( 1000 );

        /// <summary>
        ///     Zero <see cref="Centuries" />
        /// </summary>
        public static readonly Centuries Zero = new Centuries( 0 );

        [DataMember] public readonly Decimal Value;

        static Centuries() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Milleniums.One );
            One.Should().BeGreaterThan( Years.One );
        }

        public Centuries( long value ) {
            this.Value = value;
        }

        public Centuries( Decimal value ) {
            this.Value = value;
        }

        public Centuries( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal ) value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( Centuries other ) {
            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( Centuries other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Centuries && this.Equals( ( Centuries ) obj );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneCentury, new BigInteger( this.Value ) );
        }

        public static Centuries Combine( Centuries left, Centuries right ) {
            return Combine( left, right.Value );
        }

        public static Centuries Combine( Centuries left, Decimal years ) {
            return new Centuries( left.Value + years );
        }

        public static Centuries Combine( Centuries left, BigInteger centuries ) {
            return new Centuries( ( BigInteger ) left.Value + centuries );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="centuries" /> to <see cref="Milleniums" />.
        /// </summary>
        /// <param name="centuries"></param>
        /// <returns></returns>
        public static implicit operator Milleniums( Centuries centuries ) {
            return ToMilleniums( centuries );
        }

        public static implicit operator Span( Centuries centuries ) {
            return new Span( centuries: centuries.Value );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="centuries" /> to <see cref="Years" />.
        /// </summary>
        /// <param name="centuries"></param>
        /// <returns></returns>
        public static implicit operator Years( Centuries centuries ) {
            return ToYears( centuries );
        }

        public static Centuries operator -( Centuries days ) {
            return new Centuries( days.Value*-1 );
        }

        public static Centuries operator -( Centuries left, Centuries right ) {
            return Combine( left: left, right: -right );
        }

        public static Centuries operator -( Centuries left, Decimal centuries ) {
            return Combine( left, -centuries );
        }

        public static Centuries operator +( Centuries left, BigInteger centuries ) {
            return Combine( left, centuries );
        }

        public static Centuries operator +( Centuries left, Centuries right ) {
            return Combine( left, right );
        }

        public static Centuries operator +( Centuries left, Decimal centuries ) {
            return Combine( left, centuries );
        }

        public static Boolean operator <( Centuries left, Centuries right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( Centuries left, Years years ) {
            return left < ( Centuries ) years;
        }

        public static Boolean operator >( Centuries left, Years years ) {
            return left > ( Centuries ) years;
        }

        public static Boolean operator >( Centuries left, Centuries right ) {
            return left.Value > right.Value;
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Centuries left, Centuries right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( Centuries left, Centuries right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( Centuries left, Centuries right ) {
            return !Equals( left, right );
        }

        public static Milleniums ToMilleniums( Centuries centuries ) {
            return new Milleniums( centuries.Value/InOneMillenium );
        }

        public static BigInteger ToPlanckTimes( Centuries centuries ) {
            return BigInteger.Multiply( PlanckTimes.InOneCentury, new BigInteger( centuries.Value ) );
        }

        public static Years ToYears( Centuries centuries ) {
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
