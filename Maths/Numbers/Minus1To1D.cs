// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Minus1To1D.cs",
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
// "Librainian/Librainian/Minus1To1D.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using Newtonsoft.Json;

    /// <summary>Restricts the value to between -1.0 and 1.0</summary>
    [JsonObject]
    public struct Minus1To1D {

        /// <summary>ONLY used in the getter and setter.</summary>
        private Double _value;

        public const Double MaxValue = 1D;

        public const Double MinValue = -1D;

        public const Double NeutralValue = 0D;

        /// <summary>
        ///     <para>Initializes a random number between -1.0 and 1.0</para>
        ///     <para>Restricts the value to between -1.0 and 1.0</para>
        /// </summary>
        /// <param name="value"></param>
        public Minus1To1D( Double? value = null ) : this() {
            if ( !value.HasValue ) { value = Randem.NextDouble( MinValue, MaxValue ); }

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