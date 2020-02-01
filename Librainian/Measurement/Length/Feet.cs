// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Feet.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Feet.cs" was last formatted by Protiguous on 2020/01/31 at 12:27 AM.

namespace Librainian.Measurement.Length {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;
    using Rationals;

    /// <summary>
    ///     <para>A foot (plural: feet) is a unit of length.</para>
    ///     <para>Since 1960 the term has usually referred to the international foot,</para>
    ///     <para>defined as being one third of a yard, making it 0.3048 meters exactly.</para>
    ///     <para>The foot is subdivided into 12 inches.</para>
    /// </summary>
    /// <see cref="http://wikipedia.org/wiki/Foot_(unit)" />
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
        public readonly Rational Value;

        public Feet( Rational value ) => this.Value = value;

        public Feet( Int64 value ) => this.Value = value;

        public Feet( BigInteger value ) => this.Value = value;

        public static Feet Combine( Feet left, Rational feet ) => new Feet( left.Value + feet );

        public static Feet Combine( Feet left, BigInteger seconds ) => new Feet( left.Value + seconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Feet left, Feet right ) => left.Value == right.Value;

        public static Feet operator -( Feet feet ) => new Feet( feet.Value * -1 );

        public static Feet operator -( Feet left, Feet right ) => Combine( left, -right.Value );

        public static Feet operator -( Feet left, Decimal seconds ) => Combine( left, ( Rational )( -seconds ) );

        public static Boolean operator !=( Feet left, Feet right ) => !Equals( left, right );

        public static Feet operator +( Feet left, Feet right ) => Combine( left, right.Value );

        public static Feet operator +( Feet left, Decimal seconds ) => Combine( left, ( Rational )seconds );

        public static Feet operator +( Feet left, BigInteger seconds ) => Combine( left, seconds );

        public static Boolean operator <( Feet left, Feet right ) => left.Value < right.Value;

        public static Boolean operator ==( Feet left, Feet right ) => Equals( left, right );

        public static Boolean operator >( Feet left, Feet right ) => left.Value > right.Value;

        public Int32 CompareTo( Feet other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Feet other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is Feet feet && this.Equals( feet );
        }

        [Pure]
        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Rational ToMeters() => throw new NotImplementedException();

        public override String ToString() => $"{this.Value} {this.Value.PluralOf( "foot" )}";
    }
}