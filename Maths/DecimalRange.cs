// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/DecimalRange.cs" was last cleaned by Rick on 2015/06/12 at 3:00 PM

namespace Librainian.Maths {

    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>Represents a Decimal range with minimum and maximum values</summary>
    /// <remarks>
    /// Modified from the AForge Library Copyright © Andrew Kirillov, 2006,
    /// andrew.kirillov@gmail.com Modified by Rick Harker, 2010, rick@aibrain.org
    /// </remarks>
    [DataContract( IsReference = true )]
    public class DecimalRange {
        public static readonly DecimalRange ZeroToOne = new DecimalRange( 0.0M, 1.0M );

        /// <summary>Maximum value</summary>
        [DataMember]
        [OptionalField]
        public readonly Decimal Max;

        /// <summary>Minimum value</summary>
        [DataMember]
        [OptionalField]
        public readonly Decimal Min;

        /// <summary>The difference between maximum and minimum values.</summary>
        [DataMember]
        [OptionalField]
        public readonly Decimal Range;

        /// <summary>Initializes a new instance of the <see cref="DecimalRange" /> class</summary>
        /// <param name="min">Minimum value of the range</param>
        /// <param name="max">Maximum value of the range</param>
        public DecimalRange(Decimal min, Decimal max) {
            this.Min = Math.Min( min, max );
            this.Max = Math.Max( min, max );
            this.Range = this.Max - this.Min;
        }

        /// <summary>Check if the specified value is inside this range</summary>
        /// <param name="x">Value to check</param>
        /// <returns>
        /// <b>True</b> if the specified value is inside this range or <b>false</b> otherwise.
        /// </returns>
        [Pure]
        public Boolean IsInside(Decimal x) => ( this.Min <= x ) && ( x <= this.Max );

        /// <summary>Check if the specified range is inside this range</summary>
        /// <param name="range">Range to check</param>
        /// <returns>
        /// <b>True</b> if the specified range is inside this range or <b>false</b> otherwise.
        /// </returns>
        [Pure]
        public Boolean IsInside(DecimalRange range) => this.IsInside( range.Min ) && this.IsInside( range.Max );

        /// <summary>Check if the specified range overlaps with this range</summary>
        /// <param name="range">Range to check for overlapping</param>
        /// <returns>
        /// <b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.
        /// </returns>
        [Pure]
        public Boolean IsOverlapping(DecimalRange range) => this.IsInside( range.Min ) || this.IsInside( range.Max );
    }
}