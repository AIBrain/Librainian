// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "WeightObsolete.cs",
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
// "Librainian/Librainian/WeightObsolete.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

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

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _value;

        /// <summary>1 <see cref="MaxValue" /></summary>
        public const Double MaxValue = +1D;

        /// <summary>- 1 <see cref="MinValue" /></summary>
        public const Double MinValue = -1D;

        /// <summary>Initializes to a random number between 0.0 and 0.50D</summary>
        public WeightObsolete() => this.Value = Randem.NextDouble() * 0.25 + Randem.NextDouble() * 0.25;

        /// <summary>A Double number, constrained between <see cref="MinValue" /> and <see cref="MaxValue" />.</summary>
        /// <param name="value"></param>
        public WeightObsolete( Double value ) => this.Value = value;

        public Double Value {
            get => Interlocked.Exchange( ref this._value, this._value );

            set {
                var correctedvalue = value;

                if ( value >= MaxValue ) { correctedvalue = MaxValue; }
                else if ( value <= MinValue ) { correctedvalue = MinValue; }

                Interlocked.Exchange( ref this._value, correctedvalue );
            }
        }

        //public object Clone() { return new Weight( this ); }

        public static Double Combine( Double value1, Double value2 ) => ( value1 + value2 ) / 2D;

        public static implicit operator Double( WeightObsolete special ) => special.Value;

        public static WeightObsolete Parse( [NotNull] String value ) {
            if ( value is null ) { throw new ArgumentNullException( nameof( value ) ); }

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