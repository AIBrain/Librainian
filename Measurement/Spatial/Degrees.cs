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
// "Librainian/Degrees.cs" was last cleaned by Rick on 2014/09/29 at 1:47 PM

#endregion License & Information

namespace Librainian.Measurement.Spatial {

    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using Annotations;
    using Extensions;
    using Parsing;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    [Immutable]
    public struct Degrees : IComparable<Degrees> {
        public const Single MaximumValue = 360.0f;
        public const Single MinimumValue = 0.0f;

        /// <summary>
        ///     One <see cref="Degrees" />.
        /// </summary>
        public static readonly Degrees One = new Degrees( 1 );

        [DataMember]
        private float _value;

        public Degrees( Single value )
            : this() {
            this.Value = value;
        }

        public Degrees( Double value ) : this( ( Single )value ) {
        }

        public Single Value {
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

        public static Degrees Combine( Degrees left, Single degrees ) {
            return new Degrees( left.Value + degrees );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Degrees left, Degrees right ) {
            return Math.Abs( left.Value - right.Value ) < Single.Epsilon;
        }

        public static implicit operator Double( Degrees degrees ) {
            return degrees.Value;
        }

        public static implicit operator Decimal( Degrees degrees ) {
            return ( Decimal )degrees.Value;
        }

        public static Degrees operator -( Degrees degrees ) {
            return new Degrees( degrees.Value * -1 );
        }

        public static Degrees operator -( Degrees left, Degrees right ) {
            return Combine( left, -right.Value );
        }

        public static Degrees operator -( Degrees left, Single degrees ) {
            return Combine( left, -degrees );
        }

        public static Boolean operator !=( Degrees left, Degrees right ) {
            return !Equals( left, right );
        }

        public static Degrees operator +( Degrees left, Degrees right ) {
            return Combine( left, right.Value );
        }

        public static Degrees operator +( Degrees left, Single degrees ) {
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

        public int CompareTo( Degrees other ) {
            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( Degrees other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Degrees && this.Equals( ( Degrees )obj );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        [Pure]
        public override String ToString() {
            return String.Format( "{0} {1}", this.Value, this.Value.PluralOf( "degree" ) );
        }
    }
}