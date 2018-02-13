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
// "Librainian/BigIntegerRange.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Maths.Ranges {

    using System;
    using System.Numerics;
    using Newtonsoft.Json;

    /// <summary>Represents a <see cref="BigInteger" /> range with minimum and maximum values.</summary>
    [JsonObject]
    public struct BigIntegerRange {

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

        /// <summary>Length of the range (difference between maximum and minimum values).</summary>
        [JsonProperty]
        public BigInteger Length {
            get;
        }

        /// <summary>Maximum value</summary>
        [JsonProperty]
        public BigInteger Max {
            get;
        }

        /// <summary>Minimum value</summary>
        [JsonProperty]
        public BigInteger Min {
            get;
        }

        /// <summary>Check if the specified range is inside this range</summary>
        /// <param name="range">Range to check</param>
        /// <returns>
        ///     <b>True</b> if the specified range is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( BigIntegerRange range ) => this.IsInside( range.Min ) && this.IsInside( range.Max );

	    /// <summary>Check if the specified value is inside this range</summary>
        /// <param name="x">Value to check</param>
        /// <returns>
        ///     <b>True</b> if the specified value is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( BigInteger x ) => this.Min <= x && x <= this.Max;

        /// <summary>Check if the specified range overlaps with this range</summary>
        /// <param name="range">Range to check for overlapping</param>
        /// <returns>
        ///     <b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsOverlapping( BigIntegerRange range ) => this.IsInside( range.Min ) || this.IsInside( range.Max );
    }
}