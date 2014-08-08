#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/DoubleRange.cs" was last cleaned by Rick on 2014/08/08 at 2:28 PM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     Represents a Double range with minimum and maximum values
    /// </summary>
    /// <remarks>
    ///     Modified from the AForge Library
    ///     Copyright © Andrew Kirillov, 2006, andrew.kirillov@gmail.com
    ///     Modified by Rick Harker, 2010, rick@aibrain.org
    /// </remarks>
    [DataContract( IsReference = true )]
    public struct DoubleRange {
        public static readonly DoubleRange ZeroToOne = new DoubleRange( 0, 1 );

        /// <summary>
        ///     Length of the range (difference between maximum and minimum values)
        /// </summary>
        [DataMember] [OptionalField] public readonly Double Length;

        /// <summary>
        ///     Maximum value
        /// </summary>
        [DataMember] [OptionalField] public readonly Double Max;

        /// <summary>
        ///     Minimum value
        /// </summary>
        [DataMember] [OptionalField] public readonly Double Min;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DoubleRange" /> class
        /// </summary>
        /// <param name="min"> Minimum value of the range </param>
        /// <param name="max"> Maximum value of the range </param>
        public DoubleRange( Double min, Double max ) {
            this.Min = Math.Min( min, max );
            this.Max = Math.Max( min, max );
            this.Length = this.Max - this.Min;
        }

        /// <summary>
        ///     Check if the specified range is inside this range
        /// </summary>
        /// <param name="range"> Range to check </param>
        /// <returns>
        ///     <b>True</b> if the specified range is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( DoubleRange range ) {
            return this.IsInside( range.Min ) && this.IsInside( range.Max );
        }

        /// <summary>
        ///     Check if the specified value is inside this range
        /// </summary>
        /// <param name="x"> Value to check </param>
        /// <returns>
        ///     <b>True</b> if the specified value is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( Double x ) {
            return this.Min <= x && x <= this.Max;
        }

        /// <summary>
        ///     Check if the specified range overlaps with this range
        /// </summary>
        /// <param name="range"> Range to check for overlapping </param>
        /// <returns>
        ///     <b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsOverlapping( DoubleRange range ) {
            return this.IsInside( range.Min ) || this.IsInside( range.Max );
        }
    }
}
