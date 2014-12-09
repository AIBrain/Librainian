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
// "Librainian/Weight.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using Annotations;
    using Threading;

    /// <summary>
    ///     <para>A Double number, constrained between <see cref="MinValue" /> and <see cref="MaxValue" />.</para>
    ///     <para>thread-safe by Interlocked</para>
    /// </summary>
    [DataContract]
    public class Weight {

        /// <summary>
        ///     1 <see cref="MaxValue" />
        /// </summary>
        public const Double MaxValue = +1D;

        /// <summary>
        ///     -1 <see cref="MinValue" />
        /// </summary>
        public const Double MinValue = -1D;

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember]
        [OptionalField]
        private Double _value;

        /// <summary>
        ///     Initializes to a random number between 0.0 and 0.50D
        /// </summary>
        public Weight() {
            this.Value = ( Randem.NextDouble() * 0.25D ) + ( Randem.NextDouble() * 0.25D );
        }

        /// <summary>
        ///     A Double number, constrained between <see cref="MinValue" /> and <see cref="MaxValue" />.
        /// </summary>
        /// <param name="value"></param>
        public Weight( Double value ) {
            this.Value = value;
        }

        public Double Value {
            get {
                return Interlocked.Exchange( ref this._value, this._value );
            }

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

        public static implicit operator Double( Weight special ) => special.Value;

        public static Weight Parse( [NotNull] String value ) {
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }
            return new Weight( Double.Parse( value ) );
        }

        public void AdjustTowardsMax() {
            this.Value = ( this.Value + MaxValue ) / 2D;

            //return this;
        }

        public void AdjustTowardsMin() {
            this.Value = ( this.Value + MinValue ) / 2D;

            //return this;
        }

        public Boolean IsAgainst() => this.Value < ( 0.0D - Double.Epsilon );

        public Boolean IsFor() => this.Value > ( 0.0D + Double.Epsilon );

        public Boolean IsNeither() => !this.IsFor() && !this.IsAgainst();

        public override String ToString() => String.Format( "{0:R}", this.Value );
    }
}