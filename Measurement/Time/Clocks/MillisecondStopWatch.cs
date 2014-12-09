#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Class1.cs" was last cleaned by Rick on 2014/11/30 at 2:50 PM
#endregion

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

        public MillisecondStopWatch() {
            Reset();
        }

        /// <summary>
        /// </summary>
        public Span Elapsed { get { return new Span( milliseconds: GetElapsedDateTimeTicks() / TicksPerMillisecond ); } }

        /// <summary>
        /// </summary>
        public bool IsRunning { get; private set; }

        private UInt64 GetElapsedDateTimeTicks() {
            var elapsed = _elapsed;
            if ( !IsRunning ) {
                return elapsed;
            }
            var ticks = GetTimestamp() - _startTimeStamp;
            elapsed += ticks;
            return elapsed;
        }

        private static UInt64 GetTimestamp() => ( UInt64 ) DateTime.UtcNow.Ticks;

        public void Reset() {
            _elapsed = 0;
            _startTimeStamp = 0;
            IsRunning = false;
        }

        public void Start() {
            if ( IsRunning ) {
                return;
            }
            _startTimeStamp = GetTimestamp();
            IsRunning = true;
        }

        public static MillisecondStopWatch StartNew() {
            var stopwatch = new MillisecondStopWatch();
            stopwatch.Start();
            return stopwatch;
        }

        public void Stop() {
            if ( !IsRunning ) {
                return;
            }
            var ticks = GetTimestamp() - _startTimeStamp;
            _elapsed += ticks;
            IsRunning = false;
        }
    }
}
