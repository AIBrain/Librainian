// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "PercentageNonTS.cs",
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
// "Librainian/Librainian/PercentageNonTS.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Threading;
    using Newtonsoft.Json;

    /// <summary>Restricts the value to between 0.0 and 1.0</summary>
    /// <remarks>Just wanted a threadsafe wrapper for Min and Max.</remarks>
    [JsonObject]
    public sealed class PercentageNonTs {

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _value;

        public const Double Epsilon = Double.Epsilon;

        public const Double MaxValue = 1D;

        public const Double MinValue = 0D;
        ///// <summary>
        /////   Initializes a random number between 0.0 and 1.0
        ///// </summary>
        //public Percentage() : this( Randem.NextDouble() ) { }

        public Double Value {
            get => Thread.VolatileRead( ref this._value );

            set => Thread.VolatileWrite( ref this._value, value >= MaxValue ? MaxValue : ( value <= MinValue ? MinValue : value ) );
        }

        /// <summary>Initializes and constrain the value to stay between 0.0 and 1.0.</summary>
        /// <param name="value"></param>
        public PercentageNonTs( Double value ) => this.Value = value;

        public PercentageNonTs( Double min, Double max ) : this( Randem.NextDouble( min: min, max: max ) ) { }

        public static implicit operator Double( PercentageNonTs special ) => special.Value;

        public static implicit operator PercentageNonTs( Double value ) => new PercentageNonTs( value );

        public static PercentageNonTs Parse( String value ) => new PercentageNonTs( Double.Parse( value ) );

        public void DropByAbsolute( PercentageNonTs percentage ) => this.Value -= percentage.Value;

        public void DropByRelative( PercentageNonTs percentage ) => this.Value -= percentage.Value * this.Value;

        public void RaiseByAbsolute( PercentageNonTs percentage ) => this.Value += percentage.Value;

        public void RaiseByRelative( PercentageNonTs percentage ) => this.Value += percentage.Value * this.Value;

        public override String ToString() => $"{this.Value:P1}";
    }

    //public static class Percentage_Extension {
    //    ///// <summary>
    //    /////   Bring the value closer to <see cref = "Percentage.MaxValue" />
    //    ///// </summary>
    //    ///// <param name = "percentage"></param>
    //    ///// <returns></returns>
    //    //public static Double Increment( this Percentage percentage ) {
    //    //    if ( null == percentage ) {
    //    //        throw new ArgumentNullException( "percentage" );
    //    //    }
    //    //    percentage.Value = Percentage.Combine( percentage, 1D );
    //    //    return percentage.Value;
    //    //}

    // /* ///
    // <summary>
    // /// Brings the <see cref="Percentage" /> closer to <paramref name="goal" />. /// <i>(defaults
    // to <seealso cref="Percentage.MaxValue" />)</i> ///
    // </summary>
    // ///
    // <param name="percentage"></param>
    // ///
    // <param name="goal"></param>
    // ///
    // <returns></returns>
    // public static void BringCloserTo( this Double percentage, Double goal = Percentage.MaxValue )
    // { percentage = ( percentage + goal ) / 2D; }
    // * /

    // ///
    // <summary>
    // /// Brings the <see cref="Percentage" /> closer to <paramref name="goal" />. /// <i>(defaults
    // to <seealso cref="Percentage.MaxValue" />)</i> ///
    // </summary>
    // ///
    // <param name="percentage"></param>
    // ///
    // <param name="goal"></param>
    // ///
    // <returns></returns>
    // public static void BringCloserTo( this Percentage percentage, Double goal =
    // Percentage.MaxValue ) { percentage.Value = ( percentage.Value + goal ) / 2D; //return
    // percentage; }

    //    /////// <summary>
    //    ///////   Brings the <see cref = "potential" /> closer to <paramref name = "goal" />.
    //    /////// </summary>
    //    /////// <param name="potential"></param>
    //    /////// <param name = "goal"></param>
    //    /////// <returns></returns>
    //    ////public static void BringCloserTo( this Potential potential, Double goal ) {
    //    ////    potential.Voltage = ( potential.Voltage + goal ) / 2D;
    //    ////    //return potential;
    //    ////}
    //}
}