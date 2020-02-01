// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Millisecond.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "Millisecond.cs" was last formatted by Protiguous on 2020/01/31 at 12:26 AM.

namespace LibrainianCore.Measurement.Time.Clocks {

    using System;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>A simple struct for a <see cref="Millisecond" />.</summary>
    [JsonObject]
    [Immutable]
    public struct Millisecond : IClockPart {

        public const Int32 MaxValue = 999;

        public const Int32 MinValue = 0;

        public static Millisecond Maximum { get; } = new Millisecond( MaxValue );

        public static Millisecond Minimum { get; } = new Millisecond( MinValue );

        [JsonProperty]
        public Int16 Value { get; }

        public Millisecond( Int16 value ) {
            if ( value < MinValue || value > MaxValue ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinValue} to {MaxValue}." );
            }

            this.Value = value;
        }

        public static explicit operator UInt16( Millisecond value ) => ( UInt16 )value.Value;

        /// <summary>Allow this class to be visibly cast to an <see cref="Int16" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Int16( Millisecond value ) => value.Value;

        public static implicit operator Millisecond( Int16 value ) => new Millisecond( value );

        /// <summary>Provide the next <see cref="Millisecond" />.</summary>
        public Millisecond Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;

            if ( next > Maximum ) {
                next = Minimum;
                ticked = true;
            }

            return ( Int16 )next;
        }

        /// <summary>Provide the previous <see cref="Millisecond" />.</summary>
        public Millisecond Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;

            if ( next < Minimum ) {
                next = Maximum;
                ticked = true;
            }

            return ( Int16 )next;
        }
    }
}