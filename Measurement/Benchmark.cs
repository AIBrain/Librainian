// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Benchmark.cs" was last cleaned by Rick on 2016/06/18 at 10:55 PM

namespace Librainian.Measurement {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using Time;

    /// <summary>
    ///     Originally based upon the idea from
    ///     <see cref="http://github.com/EBrown8534/Framework/blob/master/Evbpc.Framework/Utilities/BenchmarkResult.cs" />.
    /// </summary>
    /// <seealso cref="http://github.com/PerfDotNet/BenchmarkDotNet" />
    public static class Benchmark {

        public enum AorB {
            Unknown,
            MethodA,
            MethodB,
            Same
        }

        /// <summary>
        ///     For benchmarking methods that are too fast for individual <see cref="Stopwatch" /> start and stops.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="runFor"></param>
        /// <returns>Returns how many rounds are ran in the time given.</returns>
        public static UInt64 GetBenchmark( this Action method, TimeSpan? runFor ) {
            GC.Collect();

            var oldPriorityClass = Process.GetCurrentProcess().PriorityClass;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            var oldPriority = Thread.CurrentThread.Priority;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            try {
                method.Invoke(); //jit per Eric Lippert (http://codereview.stackexchange.com/questions/125539/benchmarking-things-in-c)
            }
            catch ( Exception exception ) {
                Debug.WriteLine( exception.Message );
                if ( Debugger.IsAttached ) {
                    Debugger.Break();
                }
            }

            var rounds = 0UL;

            var stopwatch = Stopwatch.StartNew();
            while ( stopwatch.Elapsed < (runFor ?? Seconds.One) ) {
                try {
                    method.Invoke();
                    rounds++;
                }
                catch ( Exception exception ) {
                    Debug.WriteLine( exception.Message );
                    if ( Debugger.IsAttached ) {
                        Debugger.Break();
                    }
                }
            }
            stopwatch.Stop();

            Process.GetCurrentProcess().PriorityClass = oldPriorityClass;
            Thread.CurrentThread.Priority = oldPriority;

            return rounds;
        }

        public static AorB WhichIsFaster( this Action methodA, Action methodB, TimeSpan? runfor = null ) {
            if ( null == runfor ) {
                runfor = Milliseconds.FiveHundred;
            }

            var a = methodA.GetBenchmark( runfor );

            var b = methodB.GetBenchmark( runfor );

            if ( a > b ) {
                return AorB.MethodA;
            }
            if ( b > a ) {
                return AorB.MethodB;
            }
            return AorB.Same;
        }
    }
}