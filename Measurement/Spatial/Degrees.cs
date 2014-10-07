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
// "Librainian/Degrees.cs" was last cleaned by Rick on 2014/10/04 at 9:57 AM

#endregion License & Information

namespace Librainian.Measurement.Spatial {

    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using Annotations;
    using Extensions;

    /// <summary>
    ///     A degree is a measurement of plane angle, representing 1⁄360 of a full rotation.
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Degree_(angle)" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    [Immutable]
    public struct Degrees : IComparable< Degrees > {
        public const Double MaximumValue = ( double ) CardinalDirections.FullNorth;
        public const Double MinimumValue = 0.0f;

        /// <summary>
        ///     One <see cref="Degrees" />.
        /// </summary>
        public static readonly Degrees One = new Degrees( 1d );

        [DataMember]
        private Double _value;

        public Degrees( Double value )
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

        public int CompareTo( Degrees other ) {
            return this.Value.CompareTo( other.Value );
        }

        public static Degrees Combine( Degrees left, Double degrees ) {
            return new Degrees( left.Value + degrees );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Degrees left, Degrees right ) {
            return Math.Abs( left.Value - right.Value ) < Double.Epsilon;
        }

        public static implicit operator Double( Degrees degrees ) {
            return degrees.Value;
        }

        public static implicit operator Radians( Degrees degrees ) {
            return ToRadians( degrees );
        }

        public static Radians ToRadians( Degrees degrees ) {
            return new Radians( degrees.Value * DegreesToRadiansFactor );
        }

        public static implicit operator Decimal( Degrees degrees ) {
            return ( Decimal )degrees.Value;
        }

        public static Degrees operator -( Degrees degrees ) {
            return new Degrees( degrees.Value * -1f );
        }

        public static Degrees operator -( Degrees left, Degrees right ) {
            return Combine( left, -right.Value );
        }

        public static Degrees operator -( Degrees left, Double degrees ) {
            return Combine( left, -degrees );
        }

        public static Boolean operator !=( Degrees left, Degrees right ) {
            return !Equals( left, right );
        }

        public static Degrees operator +( Degrees left, Degrees right ) {
            return Combine( left, right.Value );
        }

        public static Degrees operator +( Degrees left, Double degrees ) {
            return Combine( left, degrees );
        }

        public static Boolean operator <( Degrees left, Degrees right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator ==( Degrees left, Degrees right ) {
            return Equals( left, right );
        }

        public static Boolean operator >( Degrees left, Degrees right ) {
            return left.Value > right.Value;
        }

        public Boolean Equals( Degrees other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Degrees && Equals( this, ( Degrees )obj );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        [Pure]
        public override String ToString() {
            return String.Format( "{0} °", this.Value );
        }

        /// <summary>
        /// Math.PI / 180
        /// </summary>
        public const double DegreesToRadiansFactor = Math.PI / 180.0d;
    }
}