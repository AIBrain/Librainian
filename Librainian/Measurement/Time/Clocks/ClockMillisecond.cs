// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>A simple struct for a <see cref="ClockMillisecond" />.</summary>
    [JsonObject]
    [Immutable]
    public record ClockMillisecond : IClockPart {

        public const UInt16 MaximumValue = 999;

        public const UInt16 MinimumValue = 0;

        public ClockMillisecond( UInt16 value ) {
            if ( value > MaximumValue ) {
                value = MaximumValue; //or throw outofRangeException?
            }

            this.Value = value;
        }

        public static ClockMillisecond Maximum { get; } = new( MaximumValue );

        public static ClockMillisecond Minimum { get; } = new( MinimumValue );

        [JsonProperty]
        public UInt16 Value { get; }

        public static implicit operator UInt16( ClockMillisecond value ) => value.Value;

        public static implicit operator ClockMillisecond( UInt16 value ) => new( value );

        /// <summary>
        ///     Provide the next <see cref="ClockMillisecond" />.
        /// </summary>
        /// <param name="tocked">True when the <see cref="Value" /> went higher than <see cref="Maximum" />.</param>
        /// <returns></returns>
        public ClockMillisecond Next( out Boolean tocked ) {

            var next = this.Value + 1;

            if ( next > MaximumValue ) {
                tocked = true;

                return Minimum;
            }

            tocked = false;

            return ( ClockMillisecond )next;
        }

        /// <summary>Provide the previous <see cref="ClockMillisecond" />.</summary>
        public ClockMillisecond Previous( out Boolean tocked ) {

            var next = this.Value - 1;

            if ( next < MinimumValue ) {
                tocked = true;

                return Maximum;
            }

            tocked = false;

            return ( ClockMillisecond )next;
        }

    }

}