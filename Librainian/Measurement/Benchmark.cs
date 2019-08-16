// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Benchmark.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Benchmark.cs" was last formatted by Protiguous on 2019/08/08 at 8:37 AM.

namespace Librainian.Measurement {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using Logging;
    using Time;

    /// <summary>
    ///     Originally based upon the idea from
    ///     <see cref="http://github.com/EBrown8534/Framework/blob/master/Evbpc.Framework/Utilities/BenchmarkResult.cs" />.
    /// </summary>
    /// <see cref="http://github.com/PerfDotNet/BenchmarkDotNet" />
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

            if ( runFor == null ) {
                runFor = Seconds.One;
            }

            try {
                try {
                    method.Invoke(); //jit per Eric Lippert (http://codereview.stackexchange.com/questions/125539/benchmarking-things-in-c)
                }
                catch ( Exception exception ) {
                    exception.Log();
                }

                var rounds = 0UL;

                var stopwatch = Stopwatch.StartNew();

                while ( stopwatch.Elapsed < runFor ) {
                    try {
                        method.Invoke();
                    }
                    catch ( Exception exception ) {
                        exception.Log();
                    }
                    finally {
                        rounds++;
                    }
                }

                return rounds;
            }
            finally {
                Process.GetCurrentProcess().PriorityClass = oldPriorityClass;
                Thread.CurrentThread.Priority = oldPriority;
            }
        }

        public static AorB WhichIsFaster( this Action methodA, Action methodB, TimeSpan? runfor = null ) {
            if ( null == runfor ) {
                runfor = Seconds.One;
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