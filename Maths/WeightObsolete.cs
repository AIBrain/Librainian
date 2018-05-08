// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/WeightObsolete.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Maths {

    using System;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>A Double number, constrained between <see cref="MinValue" /> and <see cref="MaxValue" />.</para>
    ///     <para>thread-safe by Interlocked</para>
    /// </summary>
    [Obsolete]
    [JsonObject]
    public class WeightObsolete {

        /// <summary>1 <see cref="MaxValue" /></summary>
        public const Double MaxValue = +1D;

        /// <summary>- 1 <see cref="MinValue" /></summary>
        public const Double MinValue = -1D;

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _value;

        /// <summary>Initializes to a random number between 0.0 and 0.50D</summary>
        public WeightObsolete() => this.Value = Randem.NextDouble() * 0.25 + Randem.NextDouble() * 0.25;

	    /// <summary>A Double number, constrained between <see cref="MinValue" /> and <see cref="MaxValue" />.</summary>
        /// <param name="value"></param>
        public WeightObsolete( Double value ) => this.Value = value;

	    public Double Value {
            get => Interlocked.Exchange( ref this._value, this._value );

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
            if ( value is null ) {
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