// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Fuzzy.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Maths {

    using System;
    using Collections;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Numbers;

    public enum LowMiddleHigh {
        Low,

        Middle,

        High
    }

    /// <summary>
    ///     A Double number, constrained between 0 and 1. Kinda thread-safe by Interlocked
    /// </summary>
    [JsonObject]
    public struct Fuzzy : ICloneable {

        public static readonly Fuzzy Empty;

        public const Double HalfValue = ( MinValue + MaxValue ) / 2D;

        /// <summary>
        ///     1
        /// </summary>
        public const Double MaxValue = 1D;

        /// <summary>
        ///     0
        /// </summary>
        public const Double MinValue = 0D;

        /// <summary>
        ///     ~25 to 75% probability.
        /// </summary>
        private static readonly PairOfDoubles Undecided = new PairOfDoubles( low: Combine( MinValue, HalfValue ), high: Combine( HalfValue, MaxValue ) );

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [JsonProperty]
        private AtomicDouble _value;

        //private static readonly Fuzzy Truer = Fuzzy.Combine( Undecided, MaxValue );
        //private static readonly Fuzzy Falser = Fuzzy.Combine( Undecided, MinValue );
        //private static readonly Fuzzy UndecidedUpper = Combine( Undecided, Truer);
        //private static readonly Fuzzy UndecidedLower = Combine( Undecided, Falser );

        /// <summary>
        ///     Initializes a random number between 0 and 1
        /// </summary>
        public Fuzzy( Double? value = null ) : this() {
            if ( value.HasValue ) {
                this.Value = value.Value;
            }
            else {
                this.Randomize();
            }
        }

        public Double Value {
            get => this._value;

	        set {
                if ( value > MaxValue ) {
                    value = MaxValue;
                }
                else if ( value < MinValue ) {
                    value = MinValue;
                }
                this._value.Value = value;
            }
        }

        public static Double Combine( Double lhs, Double rhs ) {
            if ( !lhs.IsNumber()  ) {
                throw new ArgumentOutOfRangeException( nameof( lhs ) );
            }
            if ( !rhs.IsNumber() ) {
                throw new ArgumentOutOfRangeException( nameof( rhs ) );
            }

            return ( lhs + rhs ) / 2D;
        }

        public static Fuzzy Parse( [CanBeNull] String value ) {
            if ( String.IsNullOrWhiteSpace( value ) ) {
                throw new ArgumentNullException( nameof( value ) );
            }

			if ( Double.TryParse( value, out var result ) ) {
				return new Fuzzy( result );
			}

			return Empty;
        }

        public void AdjustTowardsMax() => this.Value = ( this.Value + MaxValue ) / 2D;

        public void AdjustTowardsMin() => this.Value = ( this.Value + MinValue ) / 2D;

        public Object Clone() => new Fuzzy( this.Value );

        //public Boolean IsUndecided( Fuzzy anotherFuzzy ) { return !IsTruer( anotherFuzzy ) && !IsFalser( anotherFuzzy ); }
        public Boolean IsFalseish() => this.Value < Undecided.Low;

        public Boolean IsTrueish() => this.Value > Undecided.High;

        public Boolean IsUndecided() => !this.IsTrueish() && !this.IsFalseish();

        /// <summary>
        ///     Initializes a random number between 0 and 1 within a range, defaulting to Middle range (~0.50)
        /// </summary>
        public void Randomize( LowMiddleHigh? lmh = LowMiddleHigh.Middle ) {
            switch ( lmh ) {
                case null:
                    this.Value = Randem.NextDouble();
                    break;

                default:
                    switch ( lmh.Value ) {
                        case LowMiddleHigh.Low:
                            do {
                                this.Value = Randem.NextDouble( 0.0D, 0.25D );
                            } while ( this.Value < Fuzzy.MinValue || this.Value > 0.25D );
                            break;

                        case LowMiddleHigh.Middle:
                            do {
                                this.Value = Randem.NextDouble( 0.25D, 0.75D );
                            } while ( this.Value < 0.25D || this.Value > 0.75D );
                            break;

                        case LowMiddleHigh.High:
                            do {
                                this.Value = Randem.NextDouble();
                            } while ( this.Value < 0.75D || this.Value > Fuzzy.MaxValue );
                            break;

                        default:
                            this.Value = Randem.NextDouble();
                            break;
                    }
                    break;
            }
        }

        public override String ToString() => $"{this.Value:R}";
    }
}