// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Month.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "Month.cs" was last formatted by Protiguous on 2020/01/31 at 12:26 AM.

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>A simple struct for a <see cref="Month" />.</summary>
    [JsonObject]
    [Immutable]
    public struct Month : IComparable<Month>, IClockPart {

        public const SByte MaxValue = 12;

        public const SByte MinValue = 1;

        /// <summary>12</summary>
        public static Month Maximum { get; } = new Month( MaxValue );

        /// <summary>1</summary>
        public static Month Minimum { get; } = new Month( MinValue );

        [JsonProperty]
        public SByte Value { get; }

        public Month( SByte value ) {
            if ( value < MinValue || value > MaxValue ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinValue} to {MaxValue}." );
            }

            this.Value = value;
        }

        /// <summary>Allow this class to be read as a <see cref="Byte" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Byte( Month value ) => ( Byte )value.Value;

        public static implicit operator Month( SByte value ) => new Month( value );

        public Int32 CompareTo( Month other ) => this.Value.CompareTo( other.Value );

        /// <summary>Provide the next <see cref="Month" />.</summary>
        /// <param name="tocked"></param>
        /// <returns></returns>
        public Month Next( out Boolean tocked ) {
            tocked = false;
            var next = this.Value + 1;

            if ( next > Maximum.Value ) {
                next = Minimum.Value;
                tocked = true;
            }

            return ( Month )next;
        }

        /// <summary>Provide the previous <see cref="Month" />.</summary>
        public Month Previous( out Boolean tocked ) {
            tocked = false;
            var next = this.Value - 1;

            if ( next < Minimum.Value ) {
                next = Maximum.Value;
                tocked = true;
            }

            return ( Month )next;
        }
    }
}