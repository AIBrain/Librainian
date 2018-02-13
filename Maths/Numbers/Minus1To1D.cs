// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Minus1To1D.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Maths.Numbers {

    using System;
    using Newtonsoft.Json;

    /// <summary>Restricts the value to between -1.0 and 1.0</summary>
    [JsonObject]
    public struct Minus1To1D {
        public const Double MaxValue = 1D;
        public const Double MinValue = -1D;
        public const Double NeutralValue = 0D;

        /// <summary>ONLY used in the getter and setter.</summary>
        private Double _value;

        /// <summary>
        ///     <para>Initializes a random number between -1.0 and 1.0</para>
        ///     <para>Restricts the value to between -1.0 and 1.0</para>
        /// </summary>
        /// <param name="value"></param>
        public Minus1To1D( Double? value = null ) : this() {
            if ( !value.HasValue ) {
                value = Randem.NextDouble( MinValue, MaxValue );
            }
            this.Value = value.Value;
        }

        public Double Value {
            get => this._value;

	        set => this._value = value > MaxValue ? MaxValue : ( value < MinValue ? MinValue : value );
        }

        /// <summary>
        ///     Return a new <see cref="Minus1To1D" /> with the value of <paramref name="value1" />
        ///     moved closer to the value of <paramref name="value2" /> .
        /// </summary>
        /// <param name="value1">The current value.</param>
        /// <param name="value2">The value to move closer towards.</param>
        /// <returns>
        ///     Returns a new <see cref="Minus1To1D" /> with the value of <paramref name="value1" />
        ///     moved closer to the value of <paramref name="value2" /> .
        /// </returns>
        public static Minus1To1D Combine( Minus1To1D value1, Minus1To1D value2 ) => new Minus1To1D( ( value1 + value2 ) / 2D );

        public static implicit operator Double( Minus1To1D special ) => special.Value;

        public static implicit operator Minus1To1D( Double value ) => new Minus1To1D( value );

        public static Minus1To1D Parse( String value ) => new Minus1To1D( Double.Parse( value ) );

        public override String ToString() => $"{this.Value:P}";
    }
}