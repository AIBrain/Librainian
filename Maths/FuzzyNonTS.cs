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
// "Librainian/FuzzyNonTS.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Runtime.Serialization;
    using Threading;

    /// <summary>
    ///     A Double number, constrained between 0 and 1.
    ///     Not thread safe!
    /// </summary>
    [DataContract( IsReference = true )]
    public sealed class FuzzyNonTS {
        public const Double MinValue = 0D;

        public const Double MaxValue = 1D;

        //private static readonly Random rnd = new Random( ( int ) DateTime.UtcNow.Ticks );

        public static readonly FuzzyNonTS Undecided = new FuzzyNonTS( 0.5D );

        public static readonly FuzzyNonTS Truer = new FuzzyNonTS( Undecided + ( Undecided/2 ) );

        public static readonly FuzzyNonTS Falser = new FuzzyNonTS( Undecided - ( Undecided/2 ) );

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember] [OptionalField] private Double _value;

        public FuzzyNonTS( Double value ) {
            this.Value = value;
        }

        public FuzzyNonTS( Fuzzy.LowMiddleHigh lmh = Fuzzy.LowMiddleHigh.Middle ) {
            this.Randomize( lmh );
        }

        public Double Value {
            get { return this._value; }
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

        /// <summary>
        ///     Initializes a random number between 0 and 1 within a range, defaulting to Middle
        /// </summary>
        public void Randomize( Fuzzy.LowMiddleHigh lmh = Fuzzy.LowMiddleHigh.Middle ) {
            switch ( lmh ) {
                case Fuzzy.LowMiddleHigh.Low:
                    this.Value = Randem.NextDouble()/10;
                    break;
                case Fuzzy.LowMiddleHigh.Middle:
                    this.Value = ( 1 - ( Randem.NextDouble()/10 ) )/2;
                    break;
                case Fuzzy.LowMiddleHigh.High:
                    this.Value = 1 - ( Randem.NextDouble()/10 );
                    break;
                default:
                    this.Value = Randem.NextDouble();
                    break;
            }
        }

        public static implicit operator Double( FuzzyNonTS special ) {
            return special.Value;
        }

        public static FuzzyNonTS Parse( String value ) {
            return new FuzzyNonTS( Double.Parse( value ) );
        }

        public static FuzzyNonTS Combine( FuzzyNonTS value1, FuzzyNonTS value2 ) {
            return new FuzzyNonTS( ( value1 + value2 )/2D );
        }

        public static FuzzyNonTS Combine( FuzzyNonTS value1, Double value2 ) {
            return new FuzzyNonTS( ( value1 + value2 )/2D );
        }

        public static FuzzyNonTS Combine( Double value1, FuzzyNonTS value2 ) {
            return new FuzzyNonTS( ( value1 + value2 )/2D );
        }

        public static Double Combine( Double value1, Double value2 ) {
            return ( value1 + value2 )/2D;
        }

        public static Boolean IsTruer( FuzzyNonTS special ) {
            return null != special && special.Value >= Truer.Value;
        }

        public static Boolean IsFalser( FuzzyNonTS special ) {
            return null != special && special.Value <= Falser.Value;
        }

        public static Boolean IsUndecided( FuzzyNonTS special ) {
            return !IsTruer( special ) && !IsFalser( special );
        }

        public void MoreLikely( FuzzyNonTS towards = null ) {
            this.Value = ( this.Value + ( towards ?? MaxValue ) )/2D;
        }

        public void MoreLikely( Double towards ) {
            this.Value = ( this.Value + ( towards >= MinValue ? towards : MaxValue ) )/2D;
        }

        public void LessLikely() {
            this.Value = ( this.Value + MinValue )/2D;
        }

        public override String ToString() {
            return String.Format( "{0:R}", this.Value );
        }

        //public override Boolean Equals( object obj ) {
        //    var other = obj as FuzzyNonTS;
        //    if ( ReferenceEquals( null, other ) ) {
        //        return false;
        //    }
        //    return ReferenceEquals( this, other ) || other.Value.Equals( this.Value );
        //}

        //public override int GetHashCode() { return base.GetHashCode(); }
    }
}
