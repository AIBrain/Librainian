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
// "Librainian/Radians.cs" was last cleaned by Rick on 2014/10/04 at 9:57 AM

#endregion License & Information

namespace Librainian.Measurement.Spatial {

    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using Annotations;
    using Extensions;

    /// <summary>
    ///     The radian is the standard unit of angular measure.
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Radian" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    [Immutable]
    public struct Radians : IComparable< Radians > {
        public const Double MaximumValue = 360.0d;
        public const Double MinimumValue = 0.0d;

        /// <summary>
        ///     One <see cref="Radians" />.
        /// </summary>
        public static readonly Radians One = new Radians( 1.0d );

        [DataMember]
        private Double _value;

        public Radians( Double value )
            : this() {
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

        [UsedImplicitly]
        private String DebuggerDisplay {
            get {
                return this.ToString();
            }
        }

        public int CompareTo( Radians other ) {
            return this.Value.CompareTo( other.Value );
        }

        public static Radians Combine( Radians left, Double radians ) {
            return new Radians( left.Value + radians );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Radians left, Radians right ) {
            return Math.Abs( left.Value - right.Value ) < Double.Epsilon;
        }

        public static implicit operator Double( Radians radians ) {
            return radians.Value;
        }

        public static implicit operator Degrees( Radians radians ) {
            return ToDegrees( radians );
        }

        public static Degrees ToDegrees( Radians radians ) {
            return new Degrees( radians.Value * RadiansToDegreesFactor );
        }

        public static implicit operator Decimal( Radians radians ) {
            return ( Decimal )radians.Value;
        }

        public static Radians operator -( Radians radians ) {
            return new Radians( radians.Value * -1.0d );
        }

        public static Radians operator -( Radians left, Radians right ) {
            return Combine( left, -right.Value );
        }

        public static Radians operator -( Radians left, Double radians ) {
            return Combine( left, -radians );
        }

        public static Boolean operator !=( Radians left, Radians right ) {
            return !Equals( left, right );
        }

        public static Radians operator +( Radians left, Radians right ) {
            return Combine( left, right.Value );
        }

        public static Radians operator +( Radians left, Double radians ) {
            return Combine( left, radians );
        }

        public static Boolean operator <( Radians left, Radians right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator ==( Radians left, Radians right ) {
            return Equals( left, right );
        }

        public static Boolean operator >( Radians left, Radians right ) {
            return left.Value > right.Value;
        }

        public Boolean Equals( Radians other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Radians && Equals( this, ( Radians )obj );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        [Pure]
        public override String ToString() {
            return String.Format( "{0} ㎭", this.Value );
        }

        /// <summary>
        /// 180 / Math.PI
        /// </summary>
        public const double RadiansToDegreesFactor = 180 / Math.PI;
    }
}