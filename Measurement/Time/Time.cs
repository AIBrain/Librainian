// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Time.cs",
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
// "Librainian/Librainian/Time.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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
    public struct Time {

        public static Time Zero = new Time( Hour.Minimum, Minute.Minimum, Second.Minimum, Millisecond.Minimum );

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Hour Hour { get; }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Microsecond Microsecond { get; }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Millisecond Millisecond { get; }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Minute Minute { get; }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Second Second { get; }

        /// <summary>
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="millisecond"></param>
        /// <param name="microsecond"></param>
        public Time( Byte hour = 0, Byte minute = 0, Byte second = 0, UInt16 millisecond = 0, UInt16 microsecond = 0 ) : this() {
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.Millisecond = millisecond;
            this.Microsecond = microsecond;
        }

        /// <summary>
        /// </summary>
        /// <param name="dateTime"></param>
        public Time( DateTime dateTime ) : this( hour: ( Byte )dateTime.Hour, minute: ( Byte )dateTime.Minute, second: ( Byte )dateTime.Second, millisecond: ( UInt16 )dateTime.Millisecond ) { }

        /// <summary>
        /// </summary>
        /// <param name="span"></param>
        public Time( Span span ) : this( hour: ( Byte )span.Hours.Value, minute: ( Byte )span.Minutes.Value, second: ( Byte )span.Seconds.Value, millisecond: ( UInt16 )span.Milliseconds.Value,
            microsecond: ( UInt16 )span.Microseconds.Value ) { }

        public static explicit operator Time( DateTime dateTime ) => new Time( ( Byte )dateTime.Hour, ( Byte )dateTime.Minute, ( Byte )dateTime.Second, ( UInt16 )dateTime.Millisecond );

        /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static implicit operator DateTime( Time date ) =>
            new DateTime( year: DateTime.MinValue.Year, month: DateTime.MinValue.Month, day: DateTime.MinValue.Day, hour: date.Hour.Value, minute: date.Minute.Value, second: date.Second.Value,
                millisecond: date.Millisecond.Value );

        /// <summary>
        ///     Get the local system's computer time.
        /// </summary>
        public static Time Now() {
            var now = DateTime.Now;

            return new Time( hour: ( Byte )now.Hour, minute: ( Byte )now.Minute, second: ( Byte )now.Second, millisecond: ( UInt16 )now.Millisecond );
        }

        public static Time UtcNow() {
            var now = DateTime.UtcNow;

            return new Time( hour: ( Byte )now.Hour, minute: ( Byte )now.Minute, second: ( Byte )now.Second, millisecond: ( UInt16 )now.Millisecond );
        }
    }
}