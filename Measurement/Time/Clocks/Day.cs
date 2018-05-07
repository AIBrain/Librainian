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
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Day.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

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
            if ( !ValidDays.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
            }
            this.Value = value;
        }

        /// <summary>should be 1</summary>
        public static Byte MinimumValue { get; } = ValidDays.Min();

        /// <summary>should be 31</summary>
        public static Byte MaximumValue { get; } = ValidDays.Max();

        public static Day Maximum { get; } = new Day( MaximumValue );

        public static Day Minimum { get; } = new Day( MinimumValue );

        [JsonProperty]
        public Byte Value {
            get;
        }

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