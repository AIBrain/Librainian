// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Minus1to1TS.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", File: "Minus1to1TS.cs" was last formatted by Protiguous on 2020/03/16 at 2:56 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>Uses Interlocked to ensure thread safety and restricts the value to between -1 and 1.</summary>
    [JsonObject]
    public class Minus1To1Ts : ICloneable {

        private const Double NaNValue = 2D;

        private static readonly Random Rand = new Random( Seed: ( Int32 )DateTime.UtcNow.Ticks );

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _value;

        public const Double MaxValue = 1D;

        public const Double MinValue = -1D;

        public Double Value {
            get => Interlocked.CompareExchange( location1: ref this._value, value: this._value, comparand: NaNValue );

            set {
                if ( value > MaxValue ) {
                    value = MaxValue;
                }
                else if ( value < MinValue ) {
                    value = MinValue;
                }

                Interlocked.CompareExchange( location1: ref this._value, value: value, comparand: this._value );
            }
        }

        /// <summary>Initialize the value to a random value between -1 and 1.</summary>
        public Minus1To1Ts() => this.Value = Rand.NextDouble() - Rand.NextDouble();

        /// <summary>Initialize the value to between -1 and 1.</summary>
        /// <param name="value"></param>
        public Minus1To1Ts( Double value ) => this.Value = value;

        public static implicit operator Double( [NotNull] Minus1To1Ts special ) => special.Value;

        [NotNull]
        public static implicit operator Minus1To1Ts( Double value ) => new Minus1To1Ts( value: value );

        public Object Clone() => new Minus1To1Ts( value: this.Value );

        public override String ToString() => $"{this.Value:R}";
    }
}