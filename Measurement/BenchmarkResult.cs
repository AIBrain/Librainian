// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/BenchmarkResult.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

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
        public TimeSpan AverageTime {
            get;
        }

        /// <summary>
        ///     The total number of rounds ran.
        /// </summary>
        public Int64 RoundsRan {
            get;
        }

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
        public TimeSpan TotalTime {
            get;
        }

        /// <summary>
        ///     Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        public override String ToString() => this.TotalTime.Simpler();

    }
}