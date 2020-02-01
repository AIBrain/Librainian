// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TimeSpanRange.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "TimeSpanRange.cs" was last formatted by Protiguous on 2020/01/31 at 12:27 AM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
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

        [NotNull]
        public IEnumerable<Days> AllDays() => this.Min.TotalDays.To( this.Max.TotalDays, 1 ).Select( second => Days.One );

        [NotNull]
        public IEnumerable<Hours> AllHours() => this.Min.TotalHours.To( this.Max.TotalHours, 1 ).Select( second => Hours.One );

        [NotNull]
        public IEnumerable<Milliseconds> AllMilliseconds() => this.Min.TotalMilliseconds.To( this.Max.TotalMilliseconds, 1 ).Select( millsecond => Milliseconds.One );

        [NotNull]
        public IEnumerable<Minutes> AllMinutes() => this.Min.TotalMinutes.To( this.Max.TotalMinutes, 1 ).Select( minutes => Minutes.One );

        [NotNull]
        public IEnumerable<Seconds> AllSeconds() => this.Min.TotalSeconds.To( this.Max.TotalSeconds, 1 ).Select( second => Seconds.One );

        /// <summary>Check if the specified range is inside this range</summary>
        /// <param name="range">Range to check</param>
        /// <returns><b>True</b> if the specified range is inside this range or <b>false</b> otherwise.</returns>
        public Boolean IsInside( TimeSpanRange range ) => this.IsInside( range.Min ) && this.IsInside( range.Max );

        /// <summary>Check if the specified value is inside this range</summary>
        /// <param name="x">Value to check</param>
        /// <returns><b>True</b> if the specified value is inside this range or <b>false</b> otherwise.</returns>
        public Boolean IsInside( TimeSpan x ) => this.Min <= x && x <= this.Max;

        /// <summary>Check if the specified range overlaps with this range</summary>
        /// <param name="range">Range to check for overlapping</param>
        /// <returns><b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.</returns>
        public Boolean IsOverlapping( TimeSpanRange range ) => this.IsInside( range.Min ) || this.IsInside( range.Max );

        public TimeSpan Random() => this.Min.NextTimeSpan( this.Max );
    }
}