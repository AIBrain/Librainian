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
// File "CPU.cs" last formatted on 2020-08-14 at 8:46 PM.

namespace Librainian.Threading {

	using System;
	using System.Diagnostics;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Logging;

	public static class CPU {

		/// <summary>
		///     <para>
		///         Sets the <see cref="ParallelOptions.MaxDegreeOfParallelism" /> of a <see cref="ParallelOptions" /> to
		///         <see cref="Environment.ProcessorCount" />.
		///     </para>
		///     <para>1 core to 1</para>
		///     <para>2 cores to 2</para>
		///     <para>4 cores to 4</para>
		///     <para>8 cores to 8</para>
		///     <para>n cores to n</para>
		/// </summary>
		[NotNull]
		public static ParallelOptions AllCPU { get; } = new() {
			MaxDegreeOfParallelism = Math.Max( 1, Environment.ProcessorCount )
		};

		/// <summary>
		///     <para>
		///         Sets the <see cref="ParallelOptions.MaxDegreeOfParallelism" /> of a <see cref="ParallelOptions" /> to
		///         <see cref="Environment.ProcessorCount" />-1.
		///     </para>
		///     <para>1 core to 1</para>
		///     <para>2 cores to 1</para>
		///     <para>4 cores to 3</para>
		///     <para>8 cores to 7</para>
		///     <para>n cores to n-1</para>
		/// </summary>
		[NotNull]
		public static ParallelOptions AllExceptOne { get; } = new() {
			MaxDegreeOfParallelism = Math.Max( 1, Environment.ProcessorCount - 1 ) //leave the OS a little wiggle room on one CPU
		};

		/// <summary>
		///     <para>
		///         Sets the <see cref="ParallelOptions.MaxDegreeOfParallelism" /> of a <see cref="ParallelOptions" /> to half of
		///         <see cref="Environment.ProcessorCount" />.
		///     </para>
		///     <para>1 core to 1?</para>
		///     <para>2 cores to 1</para>
		///     <para>4 cores to 2</para>
		///     <para>8 cores to 4</para>
		///     <para>n cores to n/2</para>
		/// </summary>
		[NotNull]
		public static ParallelOptions CPULight { get; } = new() {
			MaxDegreeOfParallelism = Environment.ProcessorCount / 2
		};
		
		/// <summary>
		/// Set MaxDegreeOfParallelism to half of maximum CPU processors.
		/// </summary>
		[NotNull]
		public static ParallelOptions HalfOfCPU { get; } = new() {
			MaxDegreeOfParallelism = Environment.ProcessorCount / 2
		};

		/// <summary>
		///     <para>
		///         Sets the <see cref="ParallelOptions.MaxDegreeOfParallelism" /> of a <see cref="ParallelOptions" /> to
		///         <see cref="Environment.ProcessorCount" /> * 2.
		///     </para>
		///     <para>1 core to 2</para>
		///     <para>2 cores to 4</para>
		///     <para>4 cores to 8</para>
		///     <para>8 cores to 16</para>
		///     <para>n cores to 2n</para>
		/// </summary>
		[NotNull]
		public static ParallelOptions DiskIntensive { get; } = new() {
			MaxDegreeOfParallelism = Math.Max( 1, Environment.ProcessorCount * 2 )
		};

		/// <summary>Set the Ideal Processor core to use. (For ALL threads in this process).</summary>
		/// <remarks>Untested. When would you want this??</remarks>
		/// <remarks>The primary thread is not necessarily at index zero in the thread array.</remarks>
		public static void IdealProcessor( this Byte core ) {
			try {
				if ( core > Environment.ProcessorCount ) {
					core = ( Byte )Environment.ProcessorCount;
				}

				var processThreads = Process.GetCurrentProcess().Threads;

				foreach ( ProcessThread processThread in processThreads ) {
					try {
						if ( processThread != null ) {
							processThread.IdealProcessor = core;
						}
					}
					catch ( Exception exception ) {
						exception.Log();
					}
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}
		}

	}

}