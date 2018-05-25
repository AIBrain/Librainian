// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Month.cs",
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
// "Librainian/Librainian/Month.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    ///     A simple struct for a <see cref="Month" />.
    /// </summary>
    [JsonObject]
    [Immutable]
    public struct Month : IComparable<Month>, IClockPart {

        public static readonly Byte[] ValidMonths = Enumerable.Range( 1, Months.InOneCommonYear + 1 ) //TODO //BUG ??
            .Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

        /// <summary>
        ///     12
        /// </summary>
        public static Byte Maximum { get; } = ValidMonths.Max();

        /// <summary>
        ///     1
        /// </summary>
        public static Byte Minimum { get; } = ValidMonths.Min();

        [JsonProperty]
        public Byte Value { get; }

        public Month( Byte value ) {
            if ( !ValidMonths.Contains( value ) ) { throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {Minimum} to {Maximum}." ); }

            this.Value = value;
        }

        public Int32 CompareTo( Month other ) => this.Value.CompareTo( other.Value );

        /// <summary>
        ///     Provide the next <see cref="Month" />.
        /// </summary>
        /// <param name="tocked"></param>
        /// <returns></returns>
        public Month Next( out Boolean tocked ) {
            tocked = false;
            var next = this.Value + 1;

            if ( next > Maximum ) {
                next = Minimum;
                tocked = true;
            }

            return new Month( ( Byte )next );
        }

        /// <summary>
        ///     Provide the previous <see cref="Month" />.
        /// </summary>
        public Month Previous( out Boolean tocked ) {
            tocked = false;
            var next = this.Value - 1;

            if ( next < Minimum ) {
                next = Maximum;
                tocked = true;
            }

            return new Month( ( Byte )next );
        }
    }
}