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
// "Librainian/Fuzzy.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Runtime.Serialization;
    using Annotations;
    using Collections;
    using Threading;

    public enum LowMiddleHigh {
        Low,

        Middle,

        High
    }

    /// <summary>
    ///     A Double number, constrained between 0 and 1.
    ///     Kinda thread-safe by Interlocked
    /// </summary>
    [DataContract]
    public struct Fuzzy : ICloneable {
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
        [DataMember]
        private AtomicDouble _value;

        //private static readonly Fuzzy Truer = Fuzzy.Combine( Undecided, MaxValue );
        //private static readonly Fuzzy Falser = Fuzzy.Combine( Undecided, MinValue );
        //private static readonly Fuzzy UndecidedUpper = Combine( Undecided, Truer);
        //private static readonly Fuzzy UndecidedLower = Combine( Undecided, Falser );
        /// <summary>
        ///     Initializes a random number between 0 and 1
        /// </summary>
        public Fuzzy( Double? value = null )
            : this() {
            if ( value.HasValue ) {
                this.Value = value.Value;
            }
            else {
                this.Randomize();
            }
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
                this._value.Value = correctedvalue;
            }
        }

        public object Clone() => new Fuzzy( this.Value );

        public static Double Combine( Double value1, Double value2 ) => ( value1 + value2 ) / 2.0D;

        public static Fuzzy Parse( [CanBeNull] String value ) {
            if ( String.IsNullOrWhiteSpace( value ) ) {
                throw new ArgumentNullException( "value" );
            }
            return new Fuzzy( Double.Parse( value ) );
        }

        public void AdjustTowardsMax() => this.Value = ( this.Value + MaxValue ) / 2D;

        //public Boolean IsUndecided( Fuzzy anotherFuzzy ) { return !IsTruer( anotherFuzzy ) && !IsFalser( anotherFuzzy ); }
        [UsedImplicitly]
        public void AdjustTowardsMin() => this.Value = ( this.Value + MinValue ) / 2D;

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
                            } while ( this.Value < MinValue || this.Value > 0.25D );
                            break;

                        case LowMiddleHigh.Middle:
                            do {
                                this.Value = Randem.NextDouble( 0.25D, 0.75D );
                            } while ( this.Value < 0.25D || this.Value > 0.75D );
                            break;

                        case LowMiddleHigh.High:
                            do {
                                this.Value = Randem.NextDouble();
                            } while ( this.Value < 0.75D || this.Value > MaxValue );
                            break;

                        default:
                            this.Value = Randem.NextDouble();
                            break;
                    }
                    break;
            }
        }

        //public static implicit operator Double( Fuzzy special ) {
        //    return special.Value;
        //}
        //public static Fuzzy Combine( Fuzzy value1, Fuzzy value2 ) { return new Fuzzy( ( value1 + value2 ) / 2D ); }

        //public static Fuzzy Combine( Fuzzy value1, Double value2 ) { return new Fuzzy( ( value1 + value2 ) / 2D ); }

        //public static Fuzzy Combine( Double value1, Fuzzy value2 ) { return new Fuzzy( ( value1 + value2 ) / 2D ); }
        public override String ToString() => String.Format( "{0:R}", this.Value );

        ///// <summary>
        ///// Returns true if this Fuzzy has a higher probability than the fuzzy being compared.
        ///// </summary>
        ///// <param name="anotherFuzzy"></param>
        ///// <returns></returns>
        //public Boolean IsTruer( Fuzzy anotherFuzzy ) { return this.Value > anotherFuzzy.Value; }

        //public Boolean IsFalser( Fuzzy anotherFuzzy ) { return this.Value < anotherFuzzy.Value; }
    }
}