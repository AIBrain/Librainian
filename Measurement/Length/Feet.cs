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
// "Librainian/Feet.cs" was last cleaned by Rick on 2014/08/23 at 12:57 AM

#endregion License & Information

namespace Librainian.Measurement.Length {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using Maths;
    using Parsing;

    /// <summary>
    ///     <para>A foot (plural: feet) is a unit of length.</para>
    ///     <para>Since 1960 the term has usually referred to the international foot,</para>
    ///     <para>defined as being one third of a yard, making it 0.3048 meters exactly.</para>
    ///     <para>The foot is subdivided into 12 inches.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Foot_(unit)" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public struct Feet : IComparable<Feet>, IQuantityOfDistance {

        /// <summary>
        ///     60
        /// </summary>
        public const Byte InOneYard = 60;

        /// <summary>
        ///     <see cref="Five" /> <see cref="Feet" />.
        /// </summary>
        public static readonly Feet Five = new Feet( 5 );

        /// <summary>
        ///     <see cref="One" /> <see cref="Feet" />.
        /// </summary>
        public static readonly Feet One = new Feet( 1 );

        /// <summary>
        ///     <see cref="Seven" /> <see cref="Feet" />.
        /// </summary>
        public static readonly Feet Seven = new Feet( 7 );

        /// <summary>
        ///     <see cref="Ten" /> <see cref="Feet" />.
        /// </summary>
        public static readonly Feet Ten = new Feet( 10 );

        /// <summary>
        ///     <see cref="Thirteen" /> <see cref="Feet" />.
        /// </summary>
        public static readonly Feet Thirteen = new Feet( 13 );

        /// <summary>
        ///     <see cref="Thirty" /> <see cref="Feet" />.
        /// </summary>
        public static readonly Feet Thirty = new Feet( 30 );

        /// <summary>
        ///     <see cref="Three" /> <see cref="Feet" />.
        /// </summary>
        public static readonly Feet Three = new Feet( 3 );

        /// <summary>
        ///     <see cref="Two" /> <see cref="Feet" />.
        /// </summary>
        public static readonly Feet Two = new Feet( 2 );

        /// <summary>
        /// </summary>
        public static readonly Feet Zero = new Feet( 0 );

        [DataMember]
        public readonly BigDecimal Value;

        static Feet() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );

            //One.Should().BeLessThan( Yards.One );
            //One.Should().BeGreaterThan( Inches.One );
        }

        public Feet( BigDecimal value ) {
            this.Value = value;
        }

        public Feet( long value ) {
            this.Value = value;
        }

        public Feet( BigInteger value ) {
            this.Value = value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay => this.ToString();

        public static Feet Combine( Feet left, BigDecimal feet ) => new Feet( left.Value + feet );

        public static Feet Combine( Feet left, BigInteger seconds ) => new Feet( ( BigInteger )left.Value + seconds );

        //public static Feet Combine( Feet left, Feet right ) {
        //    return Combine( ( Feet ) left, right.Value );
        //}
        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Feet left, Feet right ) => left.Value == right.Value;

        public static Feet operator -( Feet feet ) => new Feet( feet.Value * -1 );

        public static Feet operator -( Feet left, Feet right ) => Combine( left, -right.Value );

        public static Feet operator -( Feet left, Decimal seconds ) => Combine( left, -seconds );

        public static Boolean operator !=( Feet left, Feet right ) => !Equals( left, right );

        public static Feet operator +( Feet left, Feet right ) => Combine( left, right.Value );

        public static Feet operator +( Feet left, Decimal seconds ) => Combine( left, seconds );

        public static Feet operator +( Feet left, BigInteger seconds ) => Combine( left, seconds );

        public static Boolean operator <( Feet left, Feet right ) => left.Value < right.Value;

        public static Boolean operator ==( Feet left, Feet right ) => Equals( left, right );

        public static Boolean operator >( Feet left, Feet right ) => left.Value > right.Value;

        public int CompareTo( Feet other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Feet other ) => Equals( this, other );

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Feet && this.Equals( ( Feet )obj );
        }

        [Pure]
        public override int GetHashCode() => this.Value.GetHashCode();

        public BigDecimal ToMeters() {
            throw new NotImplementedException();
        }

        public override String ToString() => String.Format( "{0} {1}", this.Value, this.Value.PluralOf( "second" ) );

        //[Pure]
        //public Millimeters ToMillimeters() {
        //    return new Millimeters( this.Value*Millimeters.InOneFoot );
        //}

        //[Pure]
        //public Minutes ToMinutes() {
        //    return new Minutes( value: this.Value/InOneMinute );
        //}
    }
}