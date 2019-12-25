// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Degrees.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Degrees.cs" was last formatted by Protiguous on 2019/08/08 at 8:51 AM.

namespace LibrainianCore.Measurement.Spatial {

    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using Extensions;

    /// <summary>
    ///     A degree is a measurement of plane angle, representing 1⁄360 of a full rotation.
    /// </summary>
    /// <see cref="http://wikipedia.org/wiki/Degree_(angle)" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public struct Degrees : IComparable<Degrees> {

        [JsonProperty]
        private volatile Single _value;

        /// <summary>Math.PI / 180</summary>
        public const Single DegreesToRadiansFactor = ( Single )( Math.PI / 180.0f );

        /// <summary>
        ///     360
        /// </summary>
        public const Single MaximumValue = ( Single )CardinalDirection.FullNorth;

        /// <summary>
        ///     Just above Zero. Not Zero. Zero is <see cref="CardinalDirection.FullNorth" />.
        /// </summary>
        public const Single MinimumValue = Single.Epsilon;

        /// <summary>One <see cref="Degrees" />.</summary>
        public static readonly Degrees One = new Degrees( 1 );

        public Single Value {
            get => this._value;

            set {
                while ( value < MinimumValue ) {
                    value += MaximumValue; //BUG use math instead, is this even correct?
                }

                while ( value >= MaximumValue ) {
                    value -= MaximumValue; //BUG use math instead, is this even correct?
                }

                this._value = value;
            }
        }

        public Degrees( Single value ) : this() => this.Value = value;

        //public Boolean SetValue( Single degrees ) {
        //    this.Value = degrees;
        //    return true;
        //}
        public static Degrees Combine( Degrees left, Single degrees ) => new Degrees( left.Value + degrees );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Degrees left, Degrees right ) => Math.Abs( left.Value - right.Value ) < Double.Epsilon;

        public static implicit operator Decimal( Degrees degrees ) => ( Decimal )degrees.Value;

        public static implicit operator Double( Degrees degrees ) => degrees.Value;

        public static implicit operator Radians( Degrees degrees ) => ToRadians( degrees );

        public static implicit operator Single( Degrees degrees ) => degrees.Value;

        public static Degrees operator -( Degrees degrees ) => new Degrees( degrees.Value * -1f );

        public static Degrees operator -( Degrees left, Degrees right ) => Combine( left, -right.Value );

        public static Degrees operator -( Degrees left, Single degrees ) => Combine( left, -degrees );

        public static Boolean operator !=( Degrees left, Degrees right ) => !Equals( left, right );

        public static Degrees operator +( Degrees left, Degrees right ) => Combine( left, right.Value );

        public static Degrees operator +( Degrees left, Single degrees ) => Combine( left, degrees );

        public static Boolean operator <( Degrees left, Degrees right ) => left.Value < right.Value;

        public static Boolean operator ==( Degrees left, Degrees right ) => Equals( left, right );

        public static Boolean operator >( Degrees left, Degrees right ) => left.Value > right.Value;

        public static Radians ToRadians( Degrees degrees ) => new Radians( degrees.Value * DegreesToRadiansFactor );

        public static Radians ToRadians( Single degrees ) => new Radians( degrees * DegreesToRadiansFactor );

        public Int32 CompareTo( Degrees other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Degrees other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }

            return obj is Degrees degrees && Equals( this, degrees );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Radians ToRadians() => ToRadians( this );

        [Pure]
        public override String ToString() => $"{this.Value} °";
    }
}