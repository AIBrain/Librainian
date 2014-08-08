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
// "Librainian2/Minus1to1TS.cs" was last cleaned by Rick on 2014/08/08 at 2:28 PM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Runtime.Serialization;
    using System.Threading;

    /// <summary>
    ///     Uses Interlocked to ensure thread safety and restricts the value to between -1 and 1.
    /// </summary>
    [DataContract]
    public class Minus1to1TS : ICloneable {
        public const Double MinValue = -1D;

        public const Double MaxValue = 1D;

        private const Double NaNValue = 2D;

        private static readonly Random rand = new Random( ( int ) DateTime.UtcNow.Ticks );

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember] [OptionalField] private Double _value;

        /// <summary>
        ///     Initialize the value to a random value between -1 and 1.
        /// </summary>
        public Minus1to1TS() {
            this.Value = rand.NextDouble() - rand.NextDouble();
        }

        /// <summary>
        ///     Initialize the value to between -1 and 1.
        /// </summary>
        /// <param name="Value"></param>
        public Minus1to1TS( Double Value ) {
            this.Value = Value;
        }

        public Double Value {
            get { return Interlocked.CompareExchange( ref this._value, this._value, NaNValue ); }
            set {
                if ( value > MaxValue ) {
                    value = MaxValue;
                }
                else if ( value < MinValue ) {
                    value = MinValue;
                }
                Interlocked.CompareExchange( ref this._value, value, this._value );
            }
        }

        #region ICloneable Members
        public object Clone() {
            return new Minus1to1TS( this.Value );
        }
        #endregion

        public override String ToString() {
            return String.Format( "{0:R}", this.Value );
        }

        public static implicit operator Double( Minus1to1TS special ) {
            return special.Value;
        }

        public static implicit operator Minus1to1TS( Double value ) {
            return new Minus1to1TS( value );
        }
    }
}
