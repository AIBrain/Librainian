// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Radians.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "Radians.cs" was last formatted by Protiguous on 2020/01/31 at 12:26 AM.

namespace LibrainianCore.Measurement.Spatial {

    using System;
    using System.Diagnostics;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>The radian is the standard unit of angular measure.</summary>
    /// <see cref="http://wikipedia.org/wiki/Radian" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public struct Radians : IComparable<Radians> {

        [JsonProperty]
        private Single _value;

        public const Single MaximumValue = 360.0f;

        public const Single MinimumValue = 0.0f;

        //TODO is this correct?
        /// <summary>180 / Math.PI</summary>
        public const Single RadiansToDegreesFactor = ( Single )( 180 / Math.PI );

        /// <summary>One <see cref="Radians" />.</summary>
        public static readonly Radians One = new Radians( 1 );

        public Single Value {
            get => this._value;

            set {
                while ( value < MinimumValue ) {
                    value += MaximumValue;
                }

                while ( value >= MaximumValue ) {
                    value -= MaximumValue;
                }

                this._value = value;
            }
        }

        public Radians( Single value ) : this() => this.Value = value;

        public static Radians Combine( Radians left, Single radians ) => new Radians( left.Value + radians );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Radians left, Radians right ) => Math.Abs( left.Value - right.Value ) < Double.Epsilon;

        public static implicit operator Decimal( Radians radians ) => ( Decimal )radians.Value;

        public static implicit operator Degrees( Radians radians ) => ToDegrees( radians );

        public static implicit operator Double( Radians radians ) => radians.Value;

        public static implicit operator Single( Radians radians ) => radians.Value;

        public static Radians operator -( Radians radians ) => new Radians( radians.Value * -1 );

        public static Radians operator -( Radians left, Radians right ) => Combine( left, -right.Value );

        public static Radians operator -( Radians left, Single radians ) => Combine( left, -radians );

        public static Boolean operator !=( Radians left, Radians right ) => !Equals( left, right );

        public static Radians operator +( Radians left, Radians right ) => Combine( left, right.Value );

        public static Radians operator +( Radians left, Single radians ) => Combine( left, radians );

        public static Boolean operator <( Radians left, Radians right ) => left.Value < right.Value;

        public static Boolean operator ==( Radians left, Radians right ) => Equals( left, right );

        public static Boolean operator >( Radians left, Radians right ) => left.Value > right.Value;

        public static Degrees ToDegrees( Single radians ) => new Degrees( radians * RadiansToDegreesFactor );

        public static Degrees ToDegrees( Double radians ) => new Degrees( ( Single )( radians * RadiansToDegreesFactor ) );

        public static Degrees ToDegrees( Radians radians ) => new Degrees( radians.Value * RadiansToDegreesFactor );

        public Int32 CompareTo( Radians other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Radians other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is Radians radians && Equals( this, radians );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public override String ToString() => $"{this.Value} ㎭";
    }
}