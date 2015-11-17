// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/WeightObsolete.cs" was last cleaned by Rick on 2015/11/09 at 10:56 PM

namespace Librainian.Maths {

    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using JetBrains.Annotations;
    using Threading;

    /// <summary>
    ///     <para>A Double number, constrained between <see cref="MinValue" /> and <see cref="MaxValue" />.</para>
    ///     <para>thread-safe by Interlocked</para>
    /// </summary>
    [Obsolete]
    [DataContract]
    public class WeightObsolete {

        /// <summary>ONLY used in the getter and setter.</summary>
        [DataMember] private Double _value;

        /// <summary>1 <see cref="MaxValue" /></summary>
        public const Double MaxValue = +1D;

        /// <summary>- 1 <see cref="MinValue" /></summary>
        public const Double MinValue = -1D;

        /// <summary>Initializes to a random number between 0.0 and 0.50D</summary>
        public WeightObsolete() {
            this.Value = Randem.NextDouble() * 0.25 + Randem.NextDouble() * 0.25;
        }

        /// <summary>A Double number, constrained between <see cref="MinValue" /> and <see cref="MaxValue" />.</summary>
        /// <param name="value"></param>
        public WeightObsolete( Double value ) {
            this.Value = value;
        }

        public Double Value {
            get { return Interlocked.Exchange( ref this._value, this._value ); }

            set {
                var correctedvalue = value;
                if ( value >= MaxValue ) {
                    correctedvalue = MaxValue;
                }
                else if ( value <= MinValue ) {
                    correctedvalue = MinValue;
                }
                Interlocked.Exchange( ref this._value, correctedvalue );
            }
        }

        //public object Clone() { return new Weight( this ); }

        public static Double Combine( Double value1, Double value2 ) => ( value1 + value2 ) / 2D;

        public static implicit operator Double( WeightObsolete special ) => special.Value;

        public static WeightObsolete Parse( [NotNull] String value ) {
            if ( value == null ) {
                throw new ArgumentNullException( nameof( value ) );
            }
            return new WeightObsolete( Double.Parse( value ) );
        }

        public void AdjustTowardsMax() => this.Value = ( this.Value + MaxValue ) / 2D;

        public void AdjustTowardsMin() => this.Value = ( this.Value + MinValue ) / 2D;

        public Boolean IsAgainst() => this.Value < 0.0D - Double.Epsilon;

        public Boolean IsFor() => this.Value > 0.0D + Double.Epsilon;

        public Boolean IsNeither() => !this.IsFor() && !this.IsAgainst();

        public override String ToString() => $"{this.Value:R}";

    }

}
