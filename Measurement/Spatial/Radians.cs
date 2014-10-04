namespace Librainian.Measurement.Spatial {
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using Annotations;
    using Extensions;

    /// <summary>
    /// The radian is the standard unit of angular measure.
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Radian"/>
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    [Immutable]
    public struct Radians : IComparable<Radians> {
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
            const Double radToDegFactor = 180.0d / Math.PI;
            return new Degrees( radians.Value * radToDegFactor );
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

        public int CompareTo( Radians other ) {
            return this.Value.CompareTo( other.Value );
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
    }
}