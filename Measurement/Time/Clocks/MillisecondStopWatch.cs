// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/MillisecondStopWatch.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;

    /// <summary>
    /// Use with WindowsCE and Silverlight which don't have Stopwatch
    /// </summary>
    /// <remarks>Based on <seealso cref="http://github.com/amibar/SmartThreadPool/blob/master/SmartThreadPool/Stopwatch.cs"/></remarks>
    internal class MillisecondStopWatch {
        private const Decimal TicksPerMillisecond = 10000.0m;
        private UInt64 _elapsed;
        private UInt64 _startTimeStamp;

        public MillisecondStopWatch() => this.Reset();

        /// <summary>
        /// </summary>
        public Span Elapsed => new Span( milliseconds: this.GetElapsedDateTimeTicks() / TicksPerMillisecond );

        /// <summary>
        /// </summary>
        public Boolean IsRunning {
            get; private set;
        }

        private static UInt64 GetTimestamp() => ( UInt64 )DateTime.UtcNow.Ticks;

        private UInt64 GetElapsedDateTimeTicks() {
            var elapsed = this._elapsed;
            if ( !this.IsRunning ) {
                return elapsed;
            }
            var ticks = GetTimestamp() - this._startTimeStamp;
            elapsed += ticks;
            return elapsed;
        }

        public static MillisecondStopWatch StartNew() {
            var stopwatch = new MillisecondStopWatch();
            stopwatch.Start();
            return stopwatch;
        }

        public void Reset() {
            this._elapsed = 0;
            this._startTimeStamp = 0;
            this.IsRunning = false;
        }

        public void Start() {
            if ( this.IsRunning ) {
                return;
            }
            this._startTimeStamp = GetTimestamp();
            this.IsRunning = true;
        }

        public void Stop() {
            if ( !this.IsRunning ) {
                return;
            }
            var ticks = GetTimestamp() - this._startTimeStamp;
            this._elapsed += ticks;
            this.IsRunning = false;
        }
    }
}