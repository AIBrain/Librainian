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
// "Librainian/Threads.cs" was last cleaned by Rick on 2014/08/22 at 11:46 PM

#endregion License & Information

namespace Librainian.Threading {
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Maths;
    using Measurement.Time;

    public static class Threads {

        /// <summary>
        /// The average time a task context switch takes.
        /// </summary>
        public static TimeSpan? AverageTimerPrecision;

        public static TimeSpan GetTimerPrecision() {

            long now;
            var then = DateTime.UtcNow.Ticks;
            while ( true ) {
                now = DateTime.UtcNow.Ticks;
                if ( now - then > 0 ) {
                    break;
                }
            }

            return new Milliseconds( now - then );
        }

        public static TimeSpan TimeAThreadSwitch() {
            var stopwatch = Stopwatch.StartNew();
            Thread.Sleep( 1 );
            return stopwatch.Elapsed;
        }

        public static TimeSpan TimeATaskWait() {
            var stopwatch = Stopwatch.StartNew();
            Task.Run( () => Task.Delay( 0 ).Wait() ).Wait();
            return stopwatch.Elapsed;
        }

        public static TimeSpan GetAverageTimerPrecision( Boolean useCache = true ) {
            if ( useCache && AverageTimerPrecision.HasValue ) {
                return AverageTimerPrecision.Value;
            }

            $"Performing {Environment.ProcessorCount} timeslice calibrations.".WriteLine();
            AverageTimerPrecision = new Milliseconds( 0.To( Environment.ProcessorCount ).Select( i => GetTimerPrecision() ).Average( span => span.TotalMilliseconds ) );
            if ( AverageTimerPrecision < Milliseconds.One ) {
                AverageTimerPrecision = Milliseconds.One;
            }
            $"Average timer precision is {AverageTimerPrecision.Value.Simpler()}.".WriteLine();
            return AverageTimerPrecision.Value;
        }

        /// <summary>
        ///     Accurate to within how many nanoseconds?
        /// </summary>
        /// <returns></returns>
        public static long GetTimerAccuracy() => 1000000000L / Stopwatch.Frequency;
    }
}