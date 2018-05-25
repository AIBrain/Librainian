// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Int64Range.cs",
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
// "Librainian/Librainian/Int64Range.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

namespace Librainian.Maths.Ranges {

    using System;
    using Newtonsoft.Json;

    /// <summary>Represents a <see cref="UInt64" /> range with minimum and maximum values.</summary>
    /// <remarks>
    ///     <para>Modified from the AForge Library</para>
    ///     <para>Copyright © Andrew Kirillov, 2006, andrew.kirillov@gmail.com</para>
    /// </remarks>
    [JsonObject]
    public struct Int64Range {

        public static readonly Int64Range MinMax = new Int64Range( min: Int64.MinValue, max: Int64.MaxValue );

        /// <summary>Length of the range (difference between maximum and minimum values)</summary>
        [JsonProperty]
        public readonly Int64 Length;

        /// <summary>Maximum value</summary>
        [JsonProperty]
        public readonly Int64 Max;

        /// <summary>Minimum value</summary>
        [JsonProperty]
        public readonly Int64 Min;

        /// <summary>Initializes a new instance of the <see cref="Int64Range" /> class</summary>
        /// <param name="min">Minimum value of the range</param>
        /// <param name="max">Maximum value of the range</param>
        public Int64Range( Int64 min, Int64 max ) {
            this.Min = Math.Min( min, max );
            this.Max = Math.Max( min, max );
            this.Length = this.Max - this.Min;
        }

        /// <summary>Check if the specified range is inside this range</summary>
        /// <param name="range">Range to check</param>
        /// <returns>
        ///     <b>True</b> if the specified range is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( Int64Range range ) => this.IsInside( range.Min ) && this.IsInside( range.Max );

        /// <summary>Check if the specified value is inside this range</summary>
        /// <param name="x">Value to check</param>
        /// <returns>
        ///     <b>True</b> if the specified value is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( Int64 x ) => this.Min <= x && x <= this.Max;

        /// <summary>Check if the specified range overlaps with this range</summary>
        /// <param name="range">Range to check for overlapping</param>
        /// <returns>
        ///     <b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsOverlapping( Int64Range range ) => this.IsInside( range.Min ) || this.IsInside( range.Max );
    }
}