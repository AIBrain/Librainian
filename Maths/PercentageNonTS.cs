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
// "Librainian/PercentageNonTS.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using Threading;

    /// <summary>
    ///     Restricts the value to between 0.0 and 1.0
    /// </summary>
    /// <remarks>
    ///     Just wanted a threadsafe wrapper for Min and Max.
    /// </remarks>
    [DataContract( IsReference = true )]
    public sealed class PercentageNonTS {
        public const Double Epsilon = Double.Epsilon;
        public const Double MinValue = 0D;

        public const Double MaxValue = 1D;

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember] [OptionalField] private Double _value;

        ///// <summary>
        /////   Initializes a random number between 0.0 and 1.0
        ///// </summary>
        //public Percentage() : this( Randem.NextDouble() ) { }

        /// <summary>
        ///     Initializes and constrain the value to stay between 0.0 and 1.0.
        /// </summary>
        /// <param name="value"></param>
        public PercentageNonTS( Double value ) {
            this.Value = value;
        }

        public PercentageNonTS( Double min, Double max ) : this( Randem.NextDouble( min: min, max: max ) ) { }

        public Double Value { get { return Thread.VolatileRead( ref this._value ); } set { Thread.VolatileWrite( ref this._value, value >= MaxValue ? MaxValue : ( value <= MinValue ? MinValue : value ) ); } }

        public static PercentageNonTS Parse( String value ) {
            return new PercentageNonTS( Double.Parse( value ) );
        }

        public override String ToString() {
            return String.Format( "{0:P1}", this.Value );
        }

        public void DropByRelative( PercentageNonTS percentage ) {
            this.Value -= percentage.Value*this.Value;
        }

        public void DropByAbsolute( PercentageNonTS percentage ) {
            this.Value -= percentage.Value;
        }

        public void RaiseByRelative( PercentageNonTS percentage ) {
            this.Value += percentage.Value*this.Value;
        }

        public void RaiseByAbsolute( PercentageNonTS percentage ) {
            this.Value += percentage.Value;
        }

        public static implicit operator Double( PercentageNonTS special ) {
            return special.Value;
        }

        public static implicit operator PercentageNonTS( Double value ) {
            return new PercentageNonTS( value );
        }

        //public override int GetHashCode() { return this.HashCode; }
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

    //    /*
    //    /// <summary>
    //    ///   Brings the <see cref = "Percentage" /> closer to <paramref name = "goal" />.
    //    /// <i>(defaults to <seealso cref="Percentage.MaxValue"/>)</i>
    //    /// </summary>
    //    /// <param name = "percentage"></param>
    //    /// <param name = "goal"></param>
    //    /// <returns></returns>
    //    public static void BringCloserTo( this Double percentage, Double goal = Percentage.MaxValue ) {
    //        percentage = ( percentage + goal ) / 2D;
    //    }
    //    */

    //    /// <summary>
    //    ///   Brings the <see cref = "Percentage" /> closer to <paramref name = "goal" />.
    //    ///   <i>(defaults to <seealso cref = "Percentage.MaxValue" />)</i>
    //    /// </summary>
    //    /// <param name = "percentage"></param>
    //    /// <param name = "goal"></param>
    //    /// <returns></returns>
    //    public static void BringCloserTo( this Percentage percentage, Double goal = Percentage.MaxValue ) {
    //        percentage.Value = ( percentage.Value + goal ) / 2D;
    //        //return percentage;
    //    }

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
