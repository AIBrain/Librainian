// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "BigIntegerRange.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "BigIntegerRange.cs" was last formatted by Protiguous on 2020/01/31 at 12:26 AM.

namespace LibrainianCore.Maths.Ranges {

    using System;
    using System.Numerics;
    using Newtonsoft.Json;

    /// <summary>Represents a <see cref="BigInteger" /> range with minimum and maximum values.</summary>
    [JsonObject]
    public struct BigIntegerRange {

        /// <summary>Length of the range (difference between maximum and minimum values).</summary>
        [JsonProperty]
        public BigInteger Length { get; }

        /// <summary>Maximum value</summary>
        [JsonProperty]
        public BigInteger Max { get; }

        /// <summary>Minimum value</summary>
        [JsonProperty]
        public BigInteger Min { get; }

        /// <summary>Initializes a new instance of the <see cref="BigIntegerRange" /> class</summary>
        /// <param name="min">Minimum value of the range</param>
        /// <param name="max">Maximum value of the range</param>
        public BigIntegerRange( BigInteger min, BigInteger max ) {
            if ( min <= max ) {
                this.Min = min;
                this.Max = max;
            }
            else {
                this.Min = max;
                this.Max = min;
            }

            this.Length = this.Max - this.Min;
        }

        /// <summary>Check if the specified range is inside this range</summary>
        /// <param name="range">Range to check</param>
        /// <returns><b>True</b> if the specified range is inside this range or <b>false</b> otherwise.</returns>
        public Boolean IsInside( BigIntegerRange range ) => this.IsInside( range.Min ) && this.IsInside( range.Max );

        /// <summary>Check if the specified value is inside this range</summary>
        /// <param name="x">Value to check</param>
        /// <returns><b>True</b> if the specified value is inside this range or <b>false</b> otherwise.</returns>
        public Boolean IsInside( BigInteger x ) => this.Min <= x && x <= this.Max;

        /// <summary>Check if the specified range overlaps with this range</summary>
        /// <param name="range">Range to check for overlapping</param>
        /// <returns><b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.</returns>
        public Boolean IsOverlapping( BigIntegerRange range ) => this.IsInside( range.Min ) || this.IsInside( range.Max );
    }
}