#region License & Information

// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Degrees.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM
#endregion License & Information

namespace Librainian.Measurement.Spatial {
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using Extensions;
    using JetBrains.Annotations;

    /// <summary>
    /// A degree is a measurement of plane angle, representing 1⁄360 of a full rotation.
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Degree_(angle)" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    [Immutable]
    public struct Degrees : IComparable<Degrees> {
        public const Double MaximumValue = ( Double )CardinalDirection.FullNorth;
        public const Double MinimumValue = 0.0f;

        /// <summary>Math.PI / 180</summary>
        public const Double DegreesToRadiansFactor = Math.PI / 180.0d;

        /// <summary>One <see cref="Degrees" />.</summary>
        public static readonly Degrees One = new Degrees( 1d );

        [DataMember]
        private Double _value;

        public Degrees(Double value) : this() {
            this.Value = value;
        }

        public Double Value {
            get {
                return this._value;
            }

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

        private String DebuggerDisplay => this.ToString();

        public Int32 CompareTo(Degrees other) => this.Value.CompareTo( other.Value );

        public static Degrees Combine(Degrees left, Double degrees) => new Degrees( left.Value + degrees );

        /// <summary>
        /// <para>static equality test</para></summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals(Degrees left, Degrees right) => Math.Abs( left.Value - right.Value ) < Double.Epsilon;

        public static implicit operator Single(Degrees degrees) => ( Single )degrees.Value;

        public static implicit operator Double(Degrees degrees) => degrees.Value;

        public static implicit operator Radians(Degrees degrees) => ToRadians( degrees );

        public Radians ToRadians() => ToRadians( this );

        public static Radians ToRadians(Degrees degrees) => new Radians( degrees.Value * DegreesToRadiansFactor );

        public static Radians ToRadians(Double degrees) => new Radians( degrees * DegreesToRadiansFactor );

        public static implicit operator Decimal(Degrees degrees) => ( Decimal )degrees.Value;

        public static Degrees operator -(Degrees degrees) => new Degrees( degrees.Value * -1f );

        public static Degrees operator -(Degrees left, Degrees right) => Combine( left, -right.Value );

        public static Degrees operator -(Degrees left, Double degrees) => Combine( left, -degrees );

        public static Boolean operator !=(Degrees left, Degrees right) => !Equals( left, right );

        public static Degrees operator +(Degrees left, Degrees right) => Combine( left, right.Value );

        public static Degrees operator +(Degrees left, Double degrees) => Combine( left, degrees );

        public static Boolean operator <(Degrees left, Degrees right) => left.Value < right.Value;

        public static Boolean operator ==(Degrees left, Degrees right) => Equals( left, right );

        public static Boolean operator >(Degrees left, Degrees right) => left.Value > right.Value;

        public Boolean Equals(Degrees other) => Equals( this, other );

        public override Boolean Equals(Object obj) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Degrees && Equals( this, ( Degrees )obj );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public override String ToString() => $"{this.Value} °";
    }
}