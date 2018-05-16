// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Degrees.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Degrees.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Spatial {

    using System;
    using System.Diagnostics;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     A degree is a measurement of plane angle, representing 1⁄360 of a full rotation.
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Degree_(angle)" />
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

        public Degrees( Single value ) : this() => this.Value = value;

        //public Boolean SetValue( Single degrees ) {
        //    this.Value = degrees;
        //    return true;
        //}

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
            if ( obj is null ) { return false; }

            return obj is Degrees degrees && Equals( this, degrees );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Radians ToRadians() => ToRadians( this );

        [Pure]
        public override String ToString() => $"{this.Value} °";
    }
}