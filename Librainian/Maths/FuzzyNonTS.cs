// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "FuzzyNonTS.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "FuzzyNonTS.cs" was last formatted by Protiguous on 2020/03/16 at 2:56 PM.

namespace Librainian.Maths {

    using System;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>A Double number, constrained between 0 and 1. Not thread safe!</summary>
    [JsonObject]
    public sealed class FuzzyNonTs {

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _value;

        public const Double MaxValue = 1D;

        public const Double MinValue = 0D;

        public static FuzzyNonTs Falser { get; } = new FuzzyNonTs( value: new FuzzyNonTs( value: 0.5D ) - new FuzzyNonTs( value: 0.5D ) / 2 );

        public static FuzzyNonTs Truer { get; } = new FuzzyNonTs( value: new FuzzyNonTs( value: 0.5D ) + new FuzzyNonTs( value: 0.5D ) / 2 );

        public static FuzzyNonTs Undecided { get; } = new FuzzyNonTs( value: 0.5D );

        public Double Value {
            get => this._value;

            set {
                var correctedvalue = value;

                if ( value > MaxValue ) {
                    correctedvalue = MaxValue;
                }
                else if ( value < MinValue ) {
                    correctedvalue = MinValue;
                }

                this._value = correctedvalue;
            }
        }

        public FuzzyNonTs( Double value ) => this.Value = value;

        public FuzzyNonTs( LowMiddleHigh lmh = LowMiddleHigh.Middle ) => this.Randomize( lmh: lmh );

        [NotNull]
        public static FuzzyNonTs Combine( [CanBeNull] FuzzyNonTs value1, [CanBeNull] FuzzyNonTs value2 ) => new FuzzyNonTs( value: ( value1 + value2 ) / 2D );

        [NotNull]
        public static FuzzyNonTs Combine( [CanBeNull] FuzzyNonTs value1, Double value2 ) => new FuzzyNonTs( value: ( value1 + value2 ) / 2D );

        [NotNull]
        public static FuzzyNonTs Combine( Double value1, [CanBeNull] FuzzyNonTs value2 ) => new FuzzyNonTs( value: ( value1 + value2 ) / 2D );

        public static Double Combine( Double value1, Double value2 ) => ( value1 + value2 ) / 2D;

        public static implicit operator Double( [NotNull] FuzzyNonTs special ) => special.Value;

        public static Boolean IsFalser( [CanBeNull] FuzzyNonTs special ) => null != special && special.Value <= Falser.Value;

        public static Boolean IsTruer( [CanBeNull] FuzzyNonTs special ) => null != special && special.Value >= Truer.Value;

        public static Boolean IsUndecided( [CanBeNull] FuzzyNonTs special ) => !IsTruer( special: special ) && !IsFalser( special: special );

        [NotNull]
        public static FuzzyNonTs Parse( [NotNull] String value ) => new FuzzyNonTs( value: Double.Parse( s: value ) );

        public void LessLikely() => this.Value = ( this.Value + MinValue ) / 2D;

        public void MoreLikely( [CanBeNull] FuzzyNonTs towards = null ) => this.Value = ( this.Value + ( towards ?? MaxValue ) ) / 2D;

        public void MoreLikely( Double towards ) => this.Value = ( this.Value + ( towards >= MinValue ? towards : MaxValue ) ) / 2D;

        /// <summary>Initializes a random number between 0 and 1 within a range, defaulting to Middle</summary>
        public void Randomize( LowMiddleHigh lmh = LowMiddleHigh.Middle ) {
            switch ( lmh ) {
                case LowMiddleHigh.Low:
                    this.Value = Randem.NextDouble() / 10;

                    break;

                case LowMiddleHigh.Middle:
                    this.Value = ( 1 - Randem.NextDouble() / 10 ) / 2;

                    break;

                case LowMiddleHigh.High:
                    this.Value = 1 - Randem.NextDouble() / 10;

                    break;

                default:
                    this.Value = Randem.NextDouble();

                    break;
            }
        }

        public override String ToString() => $"{this.Value:R}";
    }
}