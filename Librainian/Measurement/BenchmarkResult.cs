// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "BenchmarkResult.cs" last formatted on 2020-08-14 at 8:38 PM.

namespace Librainian.Measurement {

	using System;
	using System.Diagnostics;
	using JetBrains.Annotations;
	using Time;

	/// <summary>Represents the result of a benchmarking session.</summary>
	[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
	public struct BenchmarkResult {

		/// <summary>The average time for all the rounds.</summary>
		public TimeSpan AverageTime { get; }

		/// <summary>The total number of rounds ran.</summary>
		public Int64 RoundsRan { get; }

		/// <summary>
		///     The total amount of time taken for all the benchmarks. (Does not include statistic calculation time, or result
		///     verification time.)
		/// </summary>
		/// <remarks>
		///     Depending on the number of rounds and time taken for each, this value may not be entirely representful of the
		///     actual result, and may have rounded over. It should be used
		///     with caution on long-running methods that are run for long amounts of time, though that likely won't be a problem
		///     as that would result in the programmer having to wait for it to
		///     run. (It would take around 29,247 years for it to wrap around.)
		/// </remarks>
		public TimeSpan TotalTime { get; }

		public BenchmarkResult( Int64 roundsRan, TimeSpan averageTime, TimeSpan totalTime ) {
			this.RoundsRan = roundsRan;
			this.AverageTime = averageTime;
			this.TotalTime = totalTime;
		}

		/// <summary>Returns the fully qualified type name of this instance.</summary>
		/// <returns>A <see cref="String" /> containing a fully qualified type name.</returns>
		[NotNull]
		public override String ToString() => this.TotalTime.Simpler();

	}

}