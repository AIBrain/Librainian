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
    using System.Threading.Tasks;
    using Maths;
    using Measurement.Time;

    public static class Threads {

        /// <summary>
        /// The average time a task context switch takes.
        /// </summary>
        public static TimeSpan? SliceAverageCache;

        public static TimeSpan GetSlice( /*Boolean? setProcessorAffinity = null*/ ) {
            //if ( setProcessorAffinity.HasValue && setProcessorAffinity.Value ) {
            //    try {
            //        var currentProcess = Process.GetCurrentProcess();
            //        var affinityMask = ( long )currentProcess.ProcessorAffinity;
            //        affinityMask &= 0xFFFF; // use any of the available processors
            //        currentProcess.ProcessorAffinity = ( IntPtr )affinityMask;
            //    }
            //    catch ( Win32Exception ) {
            //    }
            //    catch ( NotSupportedException ) {
            //    }
            //    catch ( InvalidOperationException ) {
            //    }
            //}

            var stopwatch = Stopwatch.StartNew();
            Task.Run( () => Task.Delay( 1 ).Wait() ).Wait();
            return stopwatch.Elapsed;
        }

        public static TimeSpan GetSlicingAverage( Boolean useCache = true/*, Boolean? setProcessorAffinity = null*/ ) {
            if ( useCache && SliceAverageCache.HasValue ) {
                return SliceAverageCache.Value;
            }

            //if ( setProcessorAffinity.HasValue && setProcessorAffinity.Value ) {
            //    try {
            //        var currentProcess = Process.GetCurrentProcess();

            //        var affinityMask = ( long )currentProcess.ProcessorAffinity;
            //        affinityMask &= 0xFFFF; // use any of the available processors

            //        currentProcess.ProcessorAffinity = ( IntPtr )affinityMask;
            //    }
            //    catch ( Win32Exception ) {
            //        /*swallow*/
            //    }
            //    catch ( NotSupportedException ) {
            //        /*swallow*/
            //    }
            //    catch ( InvalidOperationException ) {
            //        /*swallow*/
            //    }
            //}

            var times = ( UInt64 ) Math.Pow( Environment.ProcessorCount, Environment.ProcessorCount );
            String.Format( "Performing {0} timeslice calibrations.", times).WriteLine();
            SliceAverageCache = new Milliseconds( 0.To( times ).Select( i => GetSlice() ).Average( span => span.TotalMilliseconds ) ) ;
            if ( SliceAverageCache < Milliseconds.One ) {
                SliceAverageCache = Milliseconds.One;
            }
            String.Format( "Timeslice calibration is {0}.", SliceAverageCache.Value.Simpler() ).WriteLine();
            return SliceAverageCache.Value;
        }

        /// <summary>
        ///     Accurate to within how many nanoseconds?
        /// </summary>
        /// <returns></returns>
        public static long GetTimerAccuracy() {
            return 1000000000L / Stopwatch.Frequency;
        }

    }
}