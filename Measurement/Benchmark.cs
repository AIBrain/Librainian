// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Benchmark.cs",
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
// "Librainian/Librainian/Benchmark.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

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
                Logging.Break();
            }

            var rounds = 0UL;

            var stopwatch = Stopwatch.StartNew();

            while ( stopwatch.Elapsed < ( runFor ?? Seconds.One ) ) {
                try {
                    method.Invoke();
                    rounds++;
                }
                catch ( Exception exception ) {
                    Debug.WriteLine( exception.Message );
                    Logging.Break();
                }
            }

            stopwatch.Stop();

            Process.GetCurrentProcess().PriorityClass = oldPriorityClass;
            Thread.CurrentThread.Priority = oldPriority;

            return rounds;
        }

        public static AorB WhichIsFaster( this Action methodA, Action methodB, TimeSpan? runfor = null ) {
            if ( null == runfor ) { runfor = Milliseconds.FiveHundred; }

            var a = methodA.GetBenchmark( runfor );

            var b = methodB.GetBenchmark( runfor );

            if ( a > b ) { return AorB.MethodA; }

            if ( b > a ) { return AorB.MethodB; }

            return AorB.Same;
        }
    }
}