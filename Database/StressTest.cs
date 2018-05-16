// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "StressTest.cs",
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
// "Librainian/Librainian/StressTest.cs" was last cleaned by Protiguous on 2018/05/15 at 10:39 PM.

namespace Librainian.Database {

    using System;
    using System.Data;
    using JetBrains.Annotations;
    using Measurement.Time;

    public static class StressTest {

        /// <summary>
        ///     How high can this computer count in one second?
        /// </summary>
        /// <returns></returns>
        public static UInt64 PerformBaselineCounting() {
            TimeSpan forHowLong = Seconds.One;
            var stopwatch = StopWatch.StartNew();
            var counter = 0UL;

            do {
                counter++;

                if ( stopwatch.Elapsed >= forHowLong ) { break; }
            } while ( true );

            return counter;
        }

        /// <summary>
        ///     How high can this database count in one second?
        /// </summary>
        /// <param name="database">   </param>
        /// <param name="forHowLong"> </param>
        /// <param name="multithread"></param>
        /// <returns></returns>
        public static UInt64 PerformDatabaseCounting( [NotNull] IDatabase database, out TimeSpan forHowLong, Boolean multithread = false ) {
            if ( database is null ) { throw new ArgumentNullException( nameof( database ) ); }

            if ( multithread ) { throw new NotImplementedException( "yet" ); }

            forHowLong = Seconds.One;
            var stopwatch = StopWatch.StartNew();
            var counter = 0UL;

            do {
                counter = database.ExecuteScalar<UInt64>( $"select {counter} + cast(1 as bigint)  as [Result];", CommandType.Text );

                if ( stopwatch.Elapsed >= forHowLong ) { break; }
            } while ( true );

            return counter;
        }
    }
}