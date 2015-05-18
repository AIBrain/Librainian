namespace Librainian.Maths {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     Represents a <see cref="UInt64" /> range with minimum and maximum values.
    /// </summary>
    /// <remarks>
    ///     <para>Modified from the AForge Library</para>
    ///     <para>Copyright © Andrew Kirillov, 2006, andrew.kirillov@gmail.com</para>
    ///     <para>Modified by Rick Harker, 2014, rick@aibrain.org</para>
    /// </remarks>
    [DataContract( IsReference = true )]
    public struct Int64Range {
        public static readonly Int64Range MinMax = new Int64Range( min: Int64.MinValue, max: Int64.MaxValue );

        /// <summary>
        ///     Length of the range (difference between maximum and minimum values)
        /// </summary>
        [DataMember]
        public readonly Int64 Length;

        /// <summary>
        ///     Maximum value
        /// </summary>
        [DataMember]
        public readonly Int64 Max;

        /// <summary>
        ///     Minimum value
        /// </summary>
        [DataMember]
        public readonly Int64 Min;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int64Range" /> class
        /// </summary>
        /// <param name="min"> Minimum value of the range </param>
        /// <param name="max"> Maximum value of the range </param>
        public Int64Range( Int64 min, Int64 max ) {
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
        public Boolean IsInside( Int64Range range ) => this.IsInside( range.Min ) && this.IsInside( range.Max );

        /// <summary>
        ///     Check if the specified value is inside this range
        /// </summary>
        /// <param name="x"> Value to check </param>
        /// <returns>
        ///     <b>True</b> if the specified value is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( Int64 x ) => this.Min <= x && x <= this.Max;

        /// <summary>
        ///     Check if the specified range overlaps with this range
        /// </summary>
        /// <param name="range"> Range to check for overlapping </param>
        /// <returns>
        ///     <b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsOverlapping( Int64Range range ) => this.IsInside( range.Min ) || this.IsInside( range.Max );
    }
}