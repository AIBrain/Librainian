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
// "Librainian/FuzzyNonTS.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

namespace Librainian.Maths {

    using System;
    using Newtonsoft.Json;
    using Threading;

    /// <summary>A Double number, constrained between 0 and 1. Not thread safe!</summary>
    [JsonObject]
    public sealed class FuzzyNonTs {
        public const Double MaxValue = 1D;
        public const Double MinValue = 0D;

        //private static readonly Random rnd = new Random( ( int ) DateTime.UtcNow.Ticks );

        public static readonly FuzzyNonTs Undecided = new FuzzyNonTs( 0.5D );
        public static readonly FuzzyNonTs Falser = new FuzzyNonTs( Undecided - Undecided / 2 );
        public static readonly FuzzyNonTs Truer = new FuzzyNonTs( Undecided + Undecided / 2 );

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _value;

        public FuzzyNonTs( Double value ) {
            this.Value = value;
        }

        public FuzzyNonTs( LowMiddleHigh lmh = LowMiddleHigh.Middle ) {
            this.Randomize( lmh );
        }

        public Double Value {
            get {
                return this._value;
            }

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

        public static FuzzyNonTs Combine( FuzzyNonTs value1, FuzzyNonTs value2 ) => new FuzzyNonTs( ( value1 + value2 ) / 2D );

        public static FuzzyNonTs Combine( FuzzyNonTs value1, Double value2 ) => new FuzzyNonTs( ( value1 + value2 ) / 2D );

        public static FuzzyNonTs Combine( Double value1, FuzzyNonTs value2 ) => new FuzzyNonTs( ( value1 + value2 ) / 2D );

        public static Double Combine( Double value1, Double value2 ) => ( value1 + value2 ) / 2D;

        public static implicit operator Double( FuzzyNonTs special ) => special.Value;

        public static Boolean IsFalser( FuzzyNonTs special ) => ( null != special ) && ( special.Value <= Falser.Value );

        public static Boolean IsTruer( FuzzyNonTs special ) => ( null != special ) && ( special.Value >= Truer.Value );

        public static Boolean IsUndecided( FuzzyNonTs special ) => !IsTruer( special ) && !IsFalser( special );

        public static FuzzyNonTs Parse( String value ) => new FuzzyNonTs( Double.Parse( value ) );

        public void LessLikely() => this.Value = ( this.Value + MinValue ) / 2D;

        public void MoreLikely( FuzzyNonTs towards = null ) => this.Value = ( this.Value + ( towards ?? MaxValue ) ) / 2D;

        public void MoreLikely( Double towards ) => this.Value = ( this.Value + ( towards >= MinValue ? towards : MaxValue ) ) / 2D;

        /// <summary>
        ///     Initializes a random number between 0 and 1 within a range, defaulting to Middle
        /// </summary>
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