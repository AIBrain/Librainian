// Copyright 2016 Protiguous.
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
// "Librainian/Extensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Length {

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

        public static Int32 Comparison( this Millimeters lhs, Millimeters rhs ) => lhs.Value.CompareTo( rhs.Value );

        //public static Int32 Comparison( this Millimeters millimeters, Centimeters centimeters ) {
        //    var lhs = new Centimeters( millimeters: millimeters ).Value; //upconvert. less likely to overflow.
        //    var rhs = centimeters.Value;
        //    return lhs.CompareTo( rhs );
        //}
    }
}