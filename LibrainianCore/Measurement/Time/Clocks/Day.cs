// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Day.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "LibrainianCore", File: "Day.cs" was last formatted by Protiguous on 2020/03/16 at 3:07 PM.

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>A simple struct for a Day of the month.</summary>
    [JsonObject]
    [Immutable]
    public struct Day : IClockPart {

        public const SByte MaxValue = 31;

        public const SByte MinValue = 1;

        /// <summary>31</summary>
        public static Day Maximum { get; } = new Day( value: MaxValue );

        /// <summary>1</summary>
        public static Day Minimum { get; } = new Day( value: MinValue );

        [JsonProperty]
        public SByte Value { get; }

        public Day( SByte value ) : this() {
            if ( value < MinValue || value > MaxValue ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( value ),
                    message: $"The specified value ({value}) is out of the valid range of {MinValue} to {MaxValue}." );
            }

            this.Value = value;
        }

        public static explicit operator Byte( Day value ) => ( Byte ) value.Value;

        public static implicit operator SByte( Day value ) => value.Value;

        /// <summary>Provide the next <see cref="Day" />.</summary>
        public Day Next( out Boolean tocked ) {
            tocked = false;
            var next = ( SByte ) ( this.Value + 1 );

            if ( next > Maximum ) {
                next = Minimum;
                tocked = true;
            }

            return new Day( value: next );
        }

        /// <summary>Provide the previous <see cref="Day" />.</summary>
        public Day Previous( out Boolean tocked ) {
            tocked = false;
            var next = ( SByte ) ( this.Value - 1 );

            if ( next < Minimum ) {
                next = Maximum;
                tocked = true;
            }

            return new Day( value: next );
        }

    }

}