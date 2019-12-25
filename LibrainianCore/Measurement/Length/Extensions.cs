// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Extensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Extensions.cs" was last formatted by Protiguous on 2019/08/08 at 8:45 AM.

namespace LibrainianCore.Measurement.Length {

    using System;

    public static class Extensions {

        /// <summary>
        ///     How many <see cref="Centimeters" /> are in a single <see cref="Meters" /> ? (100)
        /// </summary>
        public const Decimal CentimetersinSingleMeter = 100m;

        /// <summary>
        ///     How many <see cref="Millimeters" /> are in a single <see cref="Centimeters" /> ? (10)
        /// </summary>
        public const Decimal MillimetersInSingleCentimeter = 10m;

        /// <summary>
        ///     How many <see cref="Millimeters" /> are in a single <see cref="Inches" /> ? (25.4)
        /// </summary>
        public const Decimal MillimetersInSingleInch = 25.4m;

        /// <summary>
        ///     How many <see cref="Millimeters" /> are in a single <see cref="Meters" /> ? (1000)
        /// </summary>
        public const Decimal MillimetersInSingleMeter = CentimetersinSingleMeter * MillimetersInSingleCentimeter;

        public static Int32 Comparison( this Millimeters left, Millimeters rhs ) => left.Value.CompareTo( rhs.Value );

        //public static Int32 Comparison( this Millimeters millimeters, Centimeters centimeters ) {
        //    var left = new Centimeters( millimeters: millimeters ).Value; //upconvert. less likely to overflow.
        //    var rhs = centimeters.Value;
        //    return left.CompareTo( rhs );
        //}
    }
}