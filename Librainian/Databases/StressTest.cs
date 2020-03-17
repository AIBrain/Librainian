﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "StressTest.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "StressTest.cs" was last formatted by Protiguous on 2020/03/16 at 4:39 PM.

namespace Librainian.Databases {

    using System;
    using System.Data;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Measurement.Time;

    public static class StressTest {

        /// <summary>How high can this computer count in one second?</summary>
        /// <returns></returns>
        public static UInt64 PerformBaselineCounting() {
            TimeSpan forHowLong = Seconds.One;
            var stopwatch = Stopwatch.StartNew();
            var counter = 0UL;

            do {
                counter++;

                if ( stopwatch.Elapsed >= forHowLong ) {
                    break;
                }
            } while ( true );

            return counter;
        }

        /// <summary>How high can this database count in one second?</summary>
        /// <param name="database">   </param>
        /// <param name="forHowLong"> </param>
        /// <param name="multithread"></param>
        /// <returns></returns>
        public static UInt64 PerformDatabaseCounting( [NotNull] IDatabase database, out TimeSpan forHowLong, Boolean multithread = false ) {
            if ( database is null ) {
                throw new ArgumentNullException( paramName: nameof( database ) );
            }

            if ( multithread ) {
                throw new NotImplementedException( message: "yet" );
            }

            forHowLong = Seconds.One;
            var stopwatch = Stopwatch.StartNew();
            var counter = 0UL;

            do {
                counter = database.ExecuteScalar<UInt64>( query: $"select {counter} + cast(1 as bigint)  as [Result];", commandType: CommandType.Text );

                if ( stopwatch.Elapsed >= forHowLong ) {
                    break;
                }
            } while ( true );

            return counter;
        }

    }

}