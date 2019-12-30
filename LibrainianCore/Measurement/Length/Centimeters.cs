// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Centimeters.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Centimeters.cs" was last formatted by Protiguous on 2019/08/08 at 8:45 AM.

namespace LibrainianCore.Measurement.Length {

    using System;

    [JsonObject]
    public struct Centimeters {

        /// <summary>One <see cref="Centimeters" /> .</summary>
        public static readonly Centimeters One = new Centimeters( centimeters: 1 );

        /// <summary>Two <see cref="Centimeters" /> .</summary>
        public static readonly Centimeters Two = new Centimeters( centimeters: 2 );

        [JsonProperty]
        public readonly Rational Value;

        public Centimeters( Decimal centimeters ) => this.Value = ( Rational ) centimeters;

        //public Centimeters( Millimeters millimeters ) {
        //    var val = millimeters.Value / Extensions.MillimetersInSingleCentimeter;
        //    this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        //}

        //public Centimeters( Meters meters ) {
        //    var val = meters.Value / Extensions.CentimetersinSingleMeter;
        //    this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        //}

        public override Int32 GetHashCode() => this.Value.GetHashCode();
    }
}