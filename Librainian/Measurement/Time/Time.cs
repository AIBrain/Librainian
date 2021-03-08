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

namespace Librainian.Measurement.Time {

    using System;
    using Clocks;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para></para>
    /// </summary>
    [JsonObject]
    [Immutable]
    public record Time {

        public Time( Byte hour = 0, Byte minute = 0, Byte second = 0, UInt16 millisecond = 0, UInt16 microsecond = 0 ) {
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.Millisecond = millisecond;
            this.Microsecond = microsecond;
        }

        /// <summary></summary>
        /// <param name="dateTime"></param>
        public Time( DateTime dateTime ) : this( ( Byte )dateTime.Hour, ( Byte )dateTime.Minute, ( Byte )dateTime.Second, ( UInt16 )dateTime.Millisecond ) { }

        /// <summary></summary>
        /// <param name="spanOfTime"></param>
        public Time( SpanOfTime spanOfTime ) : this( ( Byte )spanOfTime.Hours.Value, ( Byte )spanOfTime.Minutes.Value, ( Byte )spanOfTime.Seconds.Value,
            ( UInt16 )spanOfTime.Milliseconds.Value, ( UInt16 )spanOfTime.Microseconds.Value ) { }

        public static Time Minimum { get; } = new( ClockHour.Minimum, ClockMinute.Minimum, ClockSecond.Minimum, ClockMillisecond.Minimum );

        public static Time Maximum { get; } = new( ClockHour.Maximum, ClockMinute.Maximum, ClockSecond.Maximum, ClockMillisecond.Maximum );

        /// <summary></summary>
        [JsonProperty]
        public ClockHour Hour { get; init; }

        /// <summary></summary>
        [JsonProperty]
        public ClockMicrosecond Microsecond { get; init; }

        /// <summary></summary>
        [JsonProperty]
        public ClockMillisecond Millisecond { get; init; }

        /// <summary></summary>
        [JsonProperty]
        public ClockMinute Minute { get; init; }

        /// <summary></summary>
        [JsonProperty]
        public ClockSecond Second { get; init; }

        public static implicit operator Time( DateTime dateTime ) =>
            new( ( Byte )dateTime.Hour, ( Byte )dateTime.Minute, ( Byte )dateTime.Second, ( UInt16 )dateTime.Millisecond );

        /// <summary></summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static implicit operator DateTime( Time date ) =>
            new( DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, date.Hour.Value, date.Minute.Value, date.Second.Value, date.Millisecond.Value );

        /// <summary>Get the local system's computer time.</summary>
        public static Time Now() {
            var now = DateTime.Now;

            return new Time( ( Byte )now.Hour, ( Byte )now.Minute, ( Byte )now.Second, ( UInt16 )now.Millisecond );
        }

        public static Time UtcNow() {
            var now = DateTime.UtcNow;

            return new Time( ( Byte )now.Hour, ( Byte )now.Minute, ( Byte )now.Second, ( UInt16 )now.Millisecond );
        }

    }

}