// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "PotentialF.cs",
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
// "Librainian/Librainian/PotentialF.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

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

        /// <summary></summary>
        /// <remarks>ONLY used in the getter and setter.</remarks>
        [JsonProperty]
        private Single _value = MinValue;

        /// <summary>1</summary>
        public const Single MaxValue = 1.0f;

        /// <summary>
        ///     <para>0.000000000000000000000000000000000000000000001401298</para>
        ///     <para>"1.401298E-45"</para>
        /// </summary>
        public const Single MinValue = 0.0f;

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

        /// <summary>Initializes a random number between <see cref="MinValue" /> and <see cref="MaxValue" /></summary>
        public PotentialF( Boolean randomValue ) {
            if ( randomValue ) { this.Value = Randem.NextFloat( MinValue, MaxValue ); }
        }

        /// <summary>Initializes with <paramref name="initialValue" />.</summary>
        /// <param name="initialValue"></param>
        public PotentialF( Single initialValue ) => this.Value = initialValue;

        public PotentialF( Single min, Single max ) : this( Randem.NextFloat( min: min, max: max ) ) { }

        public static implicit operator Single( PotentialF special ) => special.Value;

        public static PotentialF Parse( String value ) => new PotentialF( Single.Parse( value ) );

        public void Add( Single amount ) => this.Value += amount;

        public void Divide( Single amount ) => this.Value /= amount;

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public void Multiply( Single amount ) => this.Value *= amount;

        public override String ToString() => $"{this.Value:P3}";
    }
}