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
// "Librainian/Radians.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Spatial {

    using System;
    using System.Diagnostics;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>The radian is the standard unit of angular measure.</summary>
    /// <seealso cref="http://wikipedia.org/wiki/Radian" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public struct Radians : IComparable<Radians> {
        public const Single MaximumValue = 360.0f; //TODO is this correct?
        public const Single MinimumValue = 0.0f;

        /// <summary>180 / Math.PI</summary>
        public const Single RadiansToDegreesFactor = ( Single )( 180 / Math.PI );

        /// <summary>One <see cref="Radians" />.</summary>
        public static readonly Radians One = new Radians( 1 );

        [JsonProperty]
        private Single _value;

        public Radians( Single value ) : this() => this.Value = value;

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
                return false;
            }
            return obj is Radians radians && Equals( this, radians );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public override String ToString() => $"{this.Value} ㎭";
    }
}