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
// "Librainian/Minus1To1.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Runtime.Serialization;
    using Threading;

    /// <summary>
    ///     Restricts the value to between -1.0 and 1.0
    /// </summary>
    [DataContract( IsReference = true )]
    public struct Minus1To1 {
        public const Double MaxValue = 1D;
        public const Double MinValue = -1D;

        public const Double NeutralValue = 0D;

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        private Double _value;

        /// <summary>
        ///     <para>Initializes a random number between -1.0 and 1.0</para>
        ///     <para>Restricts the value to between -1.0 and 1.0</para>
        /// </summary>
        /// <param name="value"></param>
        public Minus1To1( Double? value = null )
            : this() {
            if ( !value.HasValue ) {
                value = Randem.NextDouble( MinValue, MaxValue );
            }
            this.Value = value.Value;
        }

        public Double Value {
            get {
                return this._value;
            }

            set {
                if ( value > MaxValue ) {
                    this._value = MaxValue;
                }
                else if ( value < MinValue ) {
                    this._value = MinValue;
                }
                else {
                    this._value = value;
                }
            }
        }

        /// <summary>
        ///     Return a new <see cref="Minus1To1" /> with the value of <paramref name="value1" /> moved closer to the value of
        ///     <paramref
        ///         name="value2" />
        ///     .
        /// </summary>
        /// <param name="value1">The current value.</param>
        /// <param name="value2">The value to move closer towards.</param>
        /// <returns>
        ///     Returns a new <see cref="Minus1To1" /> with the value of <paramref name="value1" /> moved closer to the value of
        ///     <paramref
        ///         name="value2" />
        ///     .
        /// </returns>
        public static Minus1To1 Combine( Minus1To1 value1, Minus1To1 value2 ) {
            return new Minus1To1( ( value1 + value2 ) / 2D );
        }

        public static implicit operator Double( Minus1To1 special ) {
            return special.Value;
        }

        public static implicit operator Minus1To1( Double value ) {
            return new Minus1To1( value );
        }

        public static Minus1To1 Parse( String value ) {
            return new Minus1To1( Double.Parse( value ) );
        }

        public override String ToString() {
            return String.Format( "{0:P}", this.Value );
        }
    }
}