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
// "Librainian/Radians.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM
#endregion License & Information

namespace Librainian.Measurement.Spatial {
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using Extensions;
    using JetBrains.Annotations;

    /// <summary>The radian is the standard unit of angular measure.</summary>
    /// <seealso cref="http://wikipedia.org/wiki/Radian" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    [Immutable]
    public struct Radians : IComparable<Radians> {
        public const Double MaximumValue = 360.0d; //TODO is this correct?
        public const Double MinimumValue = 0.0d;

        /// <summary>180 / Math.PI</summary>
        public const Double RadiansToDegreesFactor = 180 / Math.PI;

        /// <summary>One <see cref="Radians" />.</summary>
        public static readonly Radians One = new Radians( 1.0d );

        [DataMember]
        private Double _value;

        public Radians(Double value) : this() {
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

        public Int32 CompareTo(Radians other) => this.Value.CompareTo( other.Value );

        public static Radians Combine(Radians left, Double radians) => new Radians( left.Value + radians );

        /// <summary>
        /// <para>static equality test</para></summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals(Radians left, Radians right) => Math.Abs( left.Value - right.Value ) < Double.Epsilon;

        public static implicit operator Single(Radians radians) => ( Single )radians.Value;

        public static implicit operator Double(Radians radians) => radians.Value;

        public static implicit operator Degrees(Radians radians) => ToDegrees( radians );

        public static Degrees ToDegrees(Single radians) => new Degrees( radians * RadiansToDegreesFactor );

        public static Degrees ToDegrees(Double radians) => new Degrees( radians * RadiansToDegreesFactor );

        public static Degrees ToDegrees(Radians radians) => new Degrees( radians.Value * RadiansToDegreesFactor );

        public static implicit operator Decimal(Radians radians) => ( Decimal )radians.Value;

        public static Radians operator -(Radians radians) => new Radians( radians.Value * -1.0d );

        public static Radians operator -(Radians left, Radians right) => Combine( left, -right.Value );

        public static Radians operator -(Radians left, Double radians) => Combine( left, -radians );

        public static Boolean operator !=(Radians left, Radians right) => !Equals( left, right );

        public static Radians operator +(Radians left, Radians right) => Combine( left, right.Value );

        public static Radians operator +(Radians left, Double radians) => Combine( left, radians );

        public static Boolean operator <(Radians left, Radians right) => left.Value < right.Value;

        public static Boolean operator ==(Radians left, Radians right) => Equals( left, right );

        public static Boolean operator >(Radians left, Radians right) => left.Value > right.Value;

        public Boolean Equals(Radians other) => Equals( this, other );

        public override Boolean Equals(Object obj) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Radians && Equals( this, ( Radians )obj );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public override String ToString() => $"{this.Value} ㎭";
    }
}