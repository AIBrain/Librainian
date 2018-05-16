// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Day.cs",
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
// "Librainian/Librainian/Day.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using Extensions;
    using Maths;
    using Newtonsoft.Json;

    /// <summary>A simple struct for a Day of the month.</summary>
    [JsonObject]
    [Immutable]
    public struct Day : IClockPart {

        public static readonly Byte[] ValidDays = 1.To( 31 ).Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

        public Day( Byte value ) : this() {
            if ( !ValidDays.Contains( value ) ) { throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." ); }

            this.Value = value;
        }

        public static Day Maximum { get; } = new Day( MaximumValue );

        /// <summary>should be 31</summary>
        public static Byte MaximumValue { get; } = ValidDays.Max();

        public static Day Minimum { get; } = new Day( MinimumValue );

        /// <summary>should be 1</summary>
        public static Byte MinimumValue { get; } = ValidDays.Min();

        [JsonProperty]
        public Byte Value { get; }

        public static explicit operator SByte( Day value ) => ( SByte )value.Value;

        public static implicit operator Byte( Day value ) => value.Value;

        /// <summary>Provide the next <see cref="Day" />.</summary>
        public Day Next( out Boolean tocked ) {
            tocked = false;
            var next = this.Value + 1;

            if ( next > MaximumValue ) {
                next = MinimumValue;
                tocked = true;
            }

            return new Day( ( Byte )next );
        }

        /// <summary>Provide the previous <see cref="Day" />.</summary>
        public Day Previous( out Boolean tocked ) {
            tocked = false;
            var next = this.Value - 1;

            if ( next < MinimumValue ) {
                next = MaximumValue;
                tocked = true;
            }

            return new Day( ( Byte )next );
        }
    }
}