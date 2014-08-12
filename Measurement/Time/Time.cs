#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian/Time.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM

#endregion License & Information

namespace Librainian.Measurement.Time {

    using System;
    using Clocks;

    /// <summary>
    /// </summary>
    public struct Time {

        /// <summary>
        /// </summary>
        public readonly Hour Hour;

        /// <summary>
        /// </summary>
        public readonly Millisecond Millisecond;
        
        /// <summary>
        /// </summary>
        public readonly Microsecond Microsecond;

        /// <summary>
        /// </summary>
        public readonly Minute Minute;

        /// <summary>
        /// </summary>
        public readonly Second Second;

        /// <summary>
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="millisecond"></param>
        /// <param name="microsecond"></param>
        public Time( Byte hour = 0, Byte minute = 0, Byte second = 0, Byte millisecond = 0, Byte microsecond = 0 )
            : this() {
            var span = new Span( hours: hour, minutes: minute, seconds: second, milliseconds: millisecond, microseconds: microsecond, normalize: true );
            this.Hour = new Hour( ( Byte )span.Hours.Value );
            this.Minute = new Minute( ( Byte )span.Minutes.Value );
            this.Second = new Second( ( Byte )span.Seconds.Value );
            this.Millisecond = new Millisecond( ( UInt16 )span.Milliseconds.Value );
            this.Microsecond = new Microsecond( ( UInt16 )span.Microseconds.Value );
        }

        /// <summary>
        /// </summary>
        /// <param name="dateTime"></param>
        public Time( DateTime dateTime )
            : this( hour: ( Byte )dateTime.Hour, minute: ( Byte )dateTime.Minute, second: ( Byte )dateTime.Second, millisecond: ( Byte )dateTime.Millisecond ) {
        }

        /// <summary>
        /// </summary>
        /// <param name="span"></param>
        public Time( Span span )
            : this( hour: ( Byte )span.Hours.Value, minute: ( Byte )span.Minutes.Value, second: ( Byte )span.Seconds.Value, millisecond: ( Byte )span.Milliseconds.Value, microsecond: ( Byte )span.Microseconds.Value ) {
        }

        /// <summary>
        /// Get the local system's computer time.
        /// </summary>
        public static Time Now {
            get {
                var now = DateTime.Now;
                return new Time( hour: ( Byte )now.Hour, minute: ( Byte )now.Minute, second: ( Byte )now.Second, millisecond: ( Byte )now.Millisecond );
            }
        }

        public static Time UtcNow {
            get {
                var now = DateTime.UtcNow;
                return new Time( hour: ( Byte )now.Hour, minute: ( Byte )now.Minute, second: ( Byte )now.Second, millisecond: ( Byte )now.Millisecond );
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static implicit operator DateTime( Time date ) {
            return new DateTime( year: DateTime.MinValue.Year, month: DateTime.MinValue.Month, day: DateTime.MinValue.Day, hour: date.Hour.Value, minute: date.Minute.Value, second: date.Second.Value, millisecond: date.Millisecond.Value );
        }
    }
}