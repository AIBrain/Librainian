// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Minus1to1TS.cs",
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
// "Librainian/Librainian/Minus1to1TS.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Threading;
    using Newtonsoft.Json;

    /// <summary>
    ///     Uses Interlocked to ensure thread safety and restricts the value to between -1 and 1.
    /// </summary>
    [JsonObject]
    public class Minus1To1Ts : ICloneable {

        private const Double NaNValue = 2D;
        private static readonly Random Rand = new Random( ( Int32 )DateTime.UtcNow.Ticks );

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _value;

        public const Double MaxValue = 1D;

        public const Double MinValue = -1D;

        public Double Value {
            get => Interlocked.CompareExchange( ref this._value, this._value, NaNValue );

            set {
                if ( value > MaxValue ) { value = MaxValue; }
                else if ( value < MinValue ) { value = MinValue; }

                Interlocked.CompareExchange( ref this._value, value, this._value );
            }
        }

        /// <summary>Initialize the value to a random value between -1 and 1.</summary>
        public Minus1To1Ts() => this.Value = Rand.NextDouble() - Rand.NextDouble();

        /// <summary>Initialize the value to between -1 and 1.</summary>
        /// <param name="value"></param>
        public Minus1To1Ts( Double value ) => this.Value = value;

        public static implicit operator Double( Minus1To1Ts special ) => special.Value;

        public static implicit operator Minus1To1Ts( Double value ) => new Minus1To1Ts( value );

        public Object Clone() => new Minus1To1Ts( this.Value );

        public override String ToString() => $"{this.Value:R}";
    }
}