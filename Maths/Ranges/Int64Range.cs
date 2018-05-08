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
// "Librainian/Int64Range.cs" was last cleaned by Protiguous on 2016/07/08 at 10:42 AM

namespace Librainian.Maths.Ranges {

    using System;
    using Newtonsoft.Json;

	/// <summary>Represents a <see cref="UInt64" /> range with minimum and maximum values.</summary>
	/// <remarks>
	///     <para>Modified from the AForge Library</para>
	///     <para>Copyright © Andrew Kirillov, 2006, andrew.kirillov@gmail.com</para>
	/// </remarks>
	[ JsonObject ]
    public struct Int64Range {

        public static readonly Int64Range MinMax = new Int64Range( min: Int64.MinValue, max: Int64.MaxValue );

        /// <summary>Length of the range (difference between maximum and minimum values)</summary>
        [ JsonProperty ]
        public readonly Int64 Length;

        /// <summary>Maximum value</summary>
        [ JsonProperty ]
        public readonly Int64 Max;

        /// <summary>Minimum value</summary>
        [ JsonProperty ]
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
