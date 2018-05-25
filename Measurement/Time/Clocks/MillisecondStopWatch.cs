// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "MillisecondStopWatch.cs",
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
// "Librainian/Librainian/MillisecondStopWatch.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Time.Clocks {

    using System;

    /// <summary>
    ///     Use with WindowsCE and Silverlight which don't have Stopwatch
    /// </summary>
    /// <remarks>
    ///     Based on <seealso cref="http://github.com/amibar/SmartThreadPool/blob/master/SmartThreadPool/Stopwatch.cs" />
    /// </remarks>
    internal class MillisecondStopWatch {

        private const Decimal TicksPerMillisecond = 10000.0m;

        private UInt64 _elapsed;

        private UInt64 _startTimeStamp;

        /// <summary>
        /// </summary>
        public Span Elapsed => new Span( milliseconds: this.GetElapsedDateTimeTicks() / TicksPerMillisecond );

        /// <summary>
        /// </summary>
        public Boolean IsRunning { get; private set; }

        public MillisecondStopWatch() => this.Reset();

        private static UInt64 GetTimestamp() => ( UInt64 )DateTime.UtcNow.Ticks;

        private UInt64 GetElapsedDateTimeTicks() {
            var elapsed = this._elapsed;

            if ( !this.IsRunning ) { return elapsed; }

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
            if ( this.IsRunning ) { return; }

            this._startTimeStamp = GetTimestamp();
            this.IsRunning = true;
        }

        public void Stop() {
            if ( !this.IsRunning ) { return; }

            var ticks = GetTimestamp() - this._startTimeStamp;
            this._elapsed += ticks;
            this.IsRunning = false;
        }
    }
}