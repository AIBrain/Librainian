// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "PercentageNonTS.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "PercentageNonTS.cs" was last formatted by Protiguous on 2018/07/13 at 1:19 AM.

namespace Librainian.Maths.Numbers {

	using System;
	using System.Threading;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>Restricts the value to between 0.0 and 1.0</summary>
	/// <remarks>Just wanted a threadsafe wrapper for Min and Max.</remarks>
	[JsonObject]
	public sealed class PercentageNonTs {

		/// <summary>ONLY used in the getter and setter.</summary>
		[JsonProperty]
		private Double _value;

		public Double Value {
			get => Thread.VolatileRead( ref this._value );

			set => Thread.VolatileWrite( ref this._value, value >= MaxValue ? MaxValue : ( value <= MinValue ? MinValue : value ) );
		}

		public const Double Epsilon = Double.Epsilon;

		public const Double MaxValue = 1D;

		public const Double MinValue = 0D;

		/// <summary>Initializes and constrain the value to stay between 0.0 and 1.0.</summary>
		/// <param name="value"></param>
		public PercentageNonTs( Double value ) => this.Value = value;

		public PercentageNonTs( Double min, Double max ) : this( Randem.NextDouble( min: min, max: max ) ) { }

		///// <summary>
		/////   Initializes a random number between 0.0 and 1.0
		///// </summary>
		//public Percentage() : this( Randem.NextDouble() ) { }
		public static implicit operator Double( [NotNull] PercentageNonTs special ) => special.Value;

		[NotNull]
		public static implicit operator PercentageNonTs( Double value ) => new PercentageNonTs( value );

		[NotNull]
		public static PercentageNonTs Parse( [NotNull] String value ) => new PercentageNonTs( Double.Parse( value ) );

		public void DropByAbsolute( [NotNull] PercentageNonTs percentage ) => this.Value -= percentage.Value;

		public void DropByRelative( [NotNull] PercentageNonTs percentage ) => this.Value -= percentage.Value * this.Value;

		public void RaiseByAbsolute( [NotNull] PercentageNonTs percentage ) => this.Value += percentage.Value;

		public void RaiseByRelative( [NotNull] PercentageNonTs percentage ) => this.Value += percentage.Value * this.Value;

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