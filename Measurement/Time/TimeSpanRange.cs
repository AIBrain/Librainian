// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "TimeSpanRange.cs",
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
// "Librainian/Librainian/TimeSpanRange.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maths;
    using Newtonsoft.Json;

    /// <summary>Represents a Single range with minimum and maximum values</summary>
    [JsonObject]
    public struct TimeSpanRange {

        /// <summary>Length of the range (difference between maximum and minimum values).</summary>
        [JsonProperty]
        public readonly TimeSpan Length;

        /// <summary>Maximum value</summary>
        [JsonProperty]
        public readonly TimeSpan Max;

        /// <summary>Minimum value</summary>
        [JsonProperty]
        public readonly TimeSpan Min;

        /// <summary>Initializes a new instance of the <see cref="TimeSpanRange" /> class</summary>
        /// <param name="min">Minimum value of the range</param>
        /// <param name="max">Maximum value of the range</param>
        public TimeSpanRange( TimeSpan min, TimeSpan max ) {
            if ( min < max ) {
                this.Min = min;
                this.Max = max;
            }
            else {
                this.Min = max;
                this.Max = min;
            }

            var δ = this.Max - this.Min;
            this.Length = δ;
        }

        public IEnumerable<Days> AllDays() => this.Min.TotalDays.To( this.Max.TotalDays, 1 ).Select( second => Days.One );

        public IEnumerable<Hours> AllHours() => this.Min.TotalHours.To( this.Max.TotalHours, 1 ).Select( second => Hours.One );

        public IEnumerable<Milliseconds> AllMilliseconds() => this.Min.TotalMilliseconds.To( this.Max.TotalMilliseconds, 1 ).Select( millsecond => Milliseconds.One );

        public IEnumerable<Minutes> AllMinutes() => this.Min.TotalMinutes.To( this.Max.TotalMinutes, 1 ).Select( minutes => Minutes.One );

        public IEnumerable<Seconds> AllSeconds() => this.Min.TotalSeconds.To( this.Max.TotalSeconds, 1 ).Select( second => Seconds.One );

        /// <summary>Check if the specified range is inside this range</summary>
        /// <param name="range">Range to check</param>
        /// <returns>
        ///     <b>True</b> if the specified range is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( TimeSpanRange range ) => this.IsInside( range.Min ) && this.IsInside( range.Max );

        /// <summary>Check if the specified value is inside this range</summary>
        /// <param name="x">Value to check</param>
        /// <returns>
        ///     <b>True</b> if the specified value is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( TimeSpan x ) => this.Min <= x && x <= this.Max;

        /// <summary>Check if the specified range overlaps with this range</summary>
        /// <param name="range">Range to check for overlapping</param>
        /// <returns>
        ///     <b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsOverlapping( TimeSpanRange range ) => this.IsInside( range.Min ) || this.IsInside( range.Max );

        public TimeSpan Random() => this.Min.NextTimeSpan( this.Max );
    }
}