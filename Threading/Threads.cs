// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Threads.cs" was last cleaned by Rick on 2015/06/12 at 3:14 PM

namespace Librainian.Threading {

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Maths;
    using Measurement.Time;

    public static class Threads {

        /// <summary>The fastest time a task context switch should take.</summary>
        private static TimeSpan? _averageTimerPrecision;

        public static TimeSpan GetAverageTimerPrecision( Boolean useCache = true ) {
            if ( useCache && _averageTimerPrecision.HasValue ) {
                return _averageTimerPrecision.Value;
            }

            $"Performing {Environment.ProcessorCount} timeslice calibrations.".WriteLine();
            _averageTimerPrecision = new Milliseconds( 0.To( Environment.ProcessorCount ).Select( i => GetTimerPrecision() ).Average( span => span.TotalMilliseconds ) );
            if ( _averageTimerPrecision < Milliseconds.One ) {
                _averageTimerPrecision = Milliseconds.One;
            }
            $"Average timer precision is {_averageTimerPrecision.Value.Simpler()}.".WriteLine();
            return _averageTimerPrecision.Value;
        }

        /// <summary>Accurate to within how many nanoseconds?</summary>
        /// <returns></returns>
        public static Int64 GetTimerAccuracy() => 1000000000L / Stopwatch.Frequency;

        public static TimeSpan GetTimerPrecision() {
            var then = DateTime.UtcNow.Ticks;
            var now = DateTime.UtcNow.Ticks;
            while ( then == now ) {
                now = DateTime.UtcNow.Ticks;
            }

            var result = new Milliseconds( TimeSpan.FromTicks( now - then ).TotalMilliseconds );
            return result;
        }

        public static TimeSpan TimeATaskWait() {
            var stopwatch = Stopwatch.StartNew();
            Task.Run( () => Task.Delay( 0 ).Wait() ).Wait();
            return stopwatch.Elapsed;
        }

        public static TimeSpan TimeAThreadSwitch() {
            var stopwatch = Stopwatch.StartNew();
            Thread.Sleep( 1 );
            return stopwatch.Elapsed;
        }
    }
}