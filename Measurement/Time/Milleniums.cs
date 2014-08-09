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
// "Librainian2/Milleniums.cs" was last cleaned by Rick on 2014/08/08 at 2:29 PM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using Parsing;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public struct Milleniums : IComparable< Milleniums > {
        /// <summary>
        ///     One <see cref="Milleniums" /> .
        /// </summary>
        public static readonly Milleniums One = new Milleniums( BigInteger.One );

        /// <summary>
        ///     Zero <see cref="Milleniums" />
        /// </summary>
        public static readonly Milleniums Zero = new Milleniums( BigInteger.Zero );

        [DataMember] public readonly Decimal Value;

        static Milleniums() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero ).And.Be( One ).And.BeGreaterThan( Centuries.One );
        }

        public Milleniums( long value ) {
            this.Value = value;
        }

        public Milleniums( Decimal value ) {
            this.Value = value;
        }

        public Milleniums( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal ) value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public const UInt16 InOneBillion = 1000;

        public int CompareTo( Milleniums other ) {
            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( Milleniums other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Milleniums && this.Equals( ( Milleniums ) obj );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneMillenium, new BigInteger( this.Value ) );
        }

        public static Milleniums Combine( Milleniums left, Milleniums right ) {
            return Combine( left, right.Value );
        }

        public static Milleniums Combine( Milleniums left, Decimal milleniums ) {
            return new Milleniums( left.Value + milleniums );
        }

        public static Milleniums Combine( Milleniums left, BigInteger milleniums ) {
            return new Milleniums( ( BigInteger ) left.Value + milleniums );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="milleniums" /> to <see cref="Centuries" />.
        /// </summary>
        /// <param name="milleniums"></param>
        /// <returns></returns>
        public static implicit operator Centuries( Milleniums milleniums ) {
            return ToCenturies( milleniums );
        }

        public static implicit operator Span( Milleniums milleniums ) {
            return new Span( milleniums: milleniums.Value );
        }

        public static Milleniums operator -( Milleniums days ) {
            return new Milleniums( days.Value*-1 );
        }

        public static Milleniums operator -( Milleniums left, Milleniums right ) {
            return Combine( left: left, right: -right );
        }

        public static Milleniums operator +( Milleniums left, Milleniums right ) {
            return Combine( left, right );
        }

        public static Milleniums operator +( Milleniums left, Decimal milleniums ) {
            return Combine( left, milleniums );
        }

        public static Milleniums operator +( Milleniums left, BigInteger milleniums ) {
            return Combine( left, milleniums );
        }

        public static Boolean operator <( Milleniums left, Milleniums right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( Milleniums left, Centuries centuries ) {
            return left < ( Milleniums ) centuries;
        }

        public static Boolean operator >( Milleniums left, Centuries centuries ) {
            return left > ( Milleniums ) centuries;
        }

        public static Boolean operator >( Milleniums left, Milleniums right ) {
            return left.Value > right.Value;
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Milleniums left, Milleniums right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( Milleniums left, Milleniums right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( Milleniums left, Milleniums right ) {
            return !Equals( left, right );
        }

        public static Centuries ToCenturies( Milleniums milleniums ) {
            return new Centuries( milleniums.Value*Centuries.InOneMillenium );
        }

        public static BigInteger ToPlanckTimes( Milleniums milleniums ) {
            return BigInteger.Multiply( PlanckTimes.InOneMillenium, new BigInteger( milleniums.Value ) );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public override string ToString() {
            return this.Value.PluralOf( "millenium" );
        }
    }
}
