// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Feet.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Length {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Numerics;
    using NUnit.Framework;
    using Parsing;

    /// <summary>
    ///     <para>A foot (plural: feet) is a unit of length.</para>
    ///     <para>Since 1960 the term has usually referred to the international foot,</para>
    ///     <para>defined as being one third of a yard, making it 0.3048 meters exactly.</para>
    ///     <para>The foot is subdivided into 12 inches.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Foot_(unit)" />
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public struct Feet : IComparable<Feet>, IQuantityOfDistance {

        /// <summary>60</summary>
        public const Byte InOneYard = 3;

        /// <summary><see cref="Five" /> .</summary>
        public static readonly Feet Five = new Feet( 5 );

        /// <summary><see cref="One" /> .</summary>
        public static readonly Feet One = new Feet( 1 );

        /// <summary><see cref="Seven" /> .</summary>
        public static readonly Feet Seven = new Feet( 7 );

        /// <summary><see cref="Ten" /> .</summary>
        public static readonly Feet Ten = new Feet( 10 );

        /// <summary><see cref="Thirteen" /> .</summary>
        public static readonly Feet Thirteen = new Feet( 13 );

        /// <summary><see cref="Thirty" /> .</summary>
        public static readonly Feet Thirty = new Feet( 30 );

        /// <summary><see cref="Three" /> .</summary>
        public static readonly Feet Three = new Feet( 3 );

        /// <summary><see cref="Two" /> .</summary>
        public static readonly Feet Two = new Feet( 2 );

        /// <summary></summary>
        public static readonly Feet Zero = new Feet( 0 );

        [JsonProperty]
        public readonly BigRational Value;

        public Feet( BigRational value ) => this.Value = value;

	    public Feet( Int64 value ) => this.Value = value;

	    public Feet( BigInteger value ) => this.Value = value;

	    public static Feet Combine( Feet left, BigRational feet ) => new Feet( left.Value + feet );

        public static Feet Combine( Feet left, BigInteger seconds ) => new Feet( ( BigInteger )left.Value + seconds );

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

        public Int32 CompareTo( Feet other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Feet other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }
            return obj is Feet feet && this.Equals( feet );
        }

        [Pure]
        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public BigRational ToMeters() => throw new NotImplementedException();

	    public override String ToString() => $"{this.Value} {this.Value.PluralOf( "foot" )}";

    }

    [TestFixture]
    public static class TestFeets {

        [Test]
        public static void TestFeet() {
            Feet.Zero.Should().BeLessThan( Feet.One );
            Feet.One.Should().BeGreaterThan( Feet.Zero );
            Feet.One.Should().Be( Feet.One );

            //TODO
            //One.Should().BeLessThan( Yards.One );
            //One.Should().BeGreaterThan( Inches.One );
        }

    }

}