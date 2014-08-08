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
// "Librainian2/UInt64Range.cs" was last cleaned by Rick on 2014/08/08 at 2:28 PM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     Represents a <see cref="ulong" /> range with minimum and maximum values.
    /// </summary>
    /// <remarks>
    ///     <para>Modified from the AForge Library</para>
    ///     <para>Copyright © Andrew Kirillov, 2006, andrew.kirillov@gmail.com</para>
    ///     <para>Modified by Rick Harker, 2010, rick@aibrain.org</para>
    /// </remarks>
    [DataContract( IsReference = true )]
    public struct UInt64Range {
        public static readonly UInt64Range MinMax = new UInt64Range( min: UInt64.MinValue, max: UInt64.MaxValue );

        /// <summary>
        ///     Length of the range (difference between maximum and minimum values)
        /// </summary>
        [DataMember] [OptionalField] public readonly UInt64 Length;

        /// <summary>
        ///     Maximum value
        /// </summary>
        [DataMember] [OptionalField] public readonly UInt64 Max;

        /// <summary>
        ///     Minimum value
        /// </summary>
        [DataMember] [OptionalField] public readonly UInt64 Min;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UInt64Range" /> class
        /// </summary>
        /// <param name="min"> Minimum value of the range </param>
        /// <param name="max"> Maximum value of the range </param>
        public UInt64Range( UInt64 min, UInt64 max ) {
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
        public Boolean IsInside( UInt64Range range ) {
            return this.IsInside( range.Min ) && this.IsInside( range.Max );
        }

        /// <summary>
        ///     Check if the specified value is inside this range
        /// </summary>
        /// <param name="x"> Value to check </param>
        /// <returns>
        ///     <b>True</b> if the specified value is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( UInt64 x ) {
            return this.Min <= x && x <= this.Max;
        }

        /// <summary>
        ///     Check if the specified range overlaps with this range
        /// </summary>
        /// <param name="range"> Range to check for overlapping </param>
        /// <returns>
        ///     <b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsOverlapping( UInt64Range range ) {
            return this.IsInside( range.Min ) || this.IsInside( range.Max );
        }
    }
}
