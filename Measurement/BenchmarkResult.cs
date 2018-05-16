// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "BenchmarkResult.cs",
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
// "Librainian/Librainian/BenchmarkResult.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

namespace Librainian.Measurement {

    using System;
    using System.Diagnostics;
    using Time;

    /// <summary>
    ///     Represents the result of a benchmarking session.
    /// </summary>
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    public struct BenchmarkResult {

        public BenchmarkResult( Int64 roundsRan, TimeSpan averageTime, TimeSpan totalTime ) {
            this.RoundsRan = roundsRan;
            this.AverageTime = averageTime;
            this.TotalTime = totalTime;
        }

        /// <summary>
        ///     The average time for all the rounds.
        /// </summary>
        public TimeSpan AverageTime { get; }

        /// <summary>
        ///     The total number of rounds ran.
        /// </summary>
        public Int64 RoundsRan { get; }

        /// <summary>
        ///     The total amount of time taken for all the benchmarks. (Does not include statistic calculation time, or result
        ///     verification time.)
        /// </summary>
        /// <remarks>
        ///     Depending on the number of rounds and time taken for each, this value may not be entirely representful of the
        ///     actual result, and may have rounded over. It should be used with caution on long-running methods that are run for
        ///     long amounts of time, though that likely won't be a problem as that would result in the programmer having to wait
        ///     for it to run. (It would take around 29,247 years for it to wrap around.)
        /// </remarks>
        public TimeSpan TotalTime { get; }

        /// <summary>
        ///     Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        public override String ToString() => this.TotalTime.Simpler();
    }
}