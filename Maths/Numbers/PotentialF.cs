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
// "Librainian/PotentialF.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Maths.Numbers {

    using System;
    using System.Threading;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>Restricts the value to between 0.0 and 1.0.</para>
    /// </summary>
    /// <remarks>
    ///     <para>Just wanted a threadsafe wrapper for Min and Max.</para>
    /// </remarks>
    [JsonObject]
    public sealed class PotentialF {

        /// <summary>1</summary>
        public const Single MaxValue = 1.0f;

        /// <summary>
        ///     <para>0.000000000000000000000000000000000000000000001401298</para>
        ///     <para>"1.401298E-45"</para>
        /// </summary>
        public const Single MinValue = 0.0f;

        /// <summary></summary>
        /// <remarks>ONLY used in the getter and setter.</remarks>
        [JsonProperty]
        private Single _value = MinValue;

        /// <summary>Initializes a random number between <see cref="MinValue" /> and <see cref="MaxValue" /></summary>
        public PotentialF( Boolean randomValue ) {
            if ( randomValue ) {
                this.Value = Randem.NextFloat( MinValue, MaxValue );
            }
        }

        /// <summary>Initializes with <paramref name="initialValue" />.</summary>
        /// <param name="initialValue"></param>
        public PotentialF( Single initialValue ) => this.Value = initialValue;

	    public PotentialF( Single min, Single max ) : this( Randem.NextFloat( min: min, max: max ) ) {
        }

        /// <summary>
        ///     <para>Thread-safe getter and setter.</para>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Constrains the value to stay between <see cref="MinValue" /> and <see cref="MaxValue" /> .
        ///     </para>
        /// </remarks>
        public Single Value {
            get => Thread.VolatileRead( ref this._value );

	        private set => Thread.VolatileWrite( ref this._value, value >= MaxValue ? MaxValue : ( value <= MinValue ? MinValue : value ) );
        }

        public static implicit operator Single( PotentialF special ) => special.Value;

        public static PotentialF Parse( String value ) => new PotentialF( Single.Parse( value ) );

        public void Add( Single amount ) => this.Value += amount;

        public void Divide( Single amount ) => this.Value /= amount;

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public void Multiply( Single amount ) => this.Value *= amount;

        public override String ToString() => $"{this.Value:P3}";
    }
}