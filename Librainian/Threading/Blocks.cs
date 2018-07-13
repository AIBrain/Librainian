// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Blocks.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Blocks.cs" was last formatted by Protiguous on 2018/07/13 at 1:39 AM.

namespace Librainian.Threading {

	using System;
	using System.Threading.Tasks;
	using System.Threading.Tasks.Dataflow;
	using JetBrains.Annotations;
	using Measurement.Time;

	public static class Blocks {

		[NotNull]
		public static IPropagatorBlock<T, T> CreateDelayBlock<T>( SpanOfTime delay ) {
			var lastItem = DateTime.MinValue;

			return new TransformBlock<T, T>( async x => {
				var waitTime = lastItem + delay - DateTime.UtcNow;

				if ( waitTime > TimeSpan.Zero ) { await Task.Delay( waitTime ); }

				lastItem = DateTime.UtcNow;

				return x;
			}, new ExecutionDataflowBlockOptions {
				BoundedCapacity = 1
			} );
		}

		public static class ManyProducers {

			/// <summary>
			///     Multiple producers consumed in smoothly ( <see cref="Environment.ProcessorCount" /> *
			///     <see cref="Environment.ProcessorCount" /> ).
			/// </summary>
			public static readonly ExecutionDataflowBlockOptions ConsumeEverything = new ExecutionDataflowBlockOptions {
				SingleProducerConstrained = false,
				MaxDegreeOfParallelism = Environment.ProcessorCount * Environment.ProcessorCount
			};

			/// <summary>
			///     Multiple producers consumed in smoothly (Environment.ProcessorCount - 1).
			/// </summary>
			public static readonly ExecutionDataflowBlockOptions ConsumeSensible = new ExecutionDataflowBlockOptions {
				SingleProducerConstrained = false,
				MaxDegreeOfParallelism = Environment.ProcessorCount > 1 ? Environment.ProcessorCount - 1 : 1
			};

			/// <summary>
			///     Multiple producers consumed in serial (MaxDegreeOfParallelism = 1).
			/// </summary>
			public static readonly ExecutionDataflowBlockOptions ConsumeSerial = new ExecutionDataflowBlockOptions {
				SingleProducerConstrained = false,
				MaxDegreeOfParallelism = 1
			};
		}

		public static class SingleProducer {

			/// <summary>
			///     <para>Single producer consumed in smoothly (Environment.ProcessorCount - 1).</para>
			/// </summary>
			public static readonly ExecutionDataflowBlockOptions ConsumeSensible = new ExecutionDataflowBlockOptions {
				SingleProducerConstrained = false,
				MaxDegreeOfParallelism = Environment.ProcessorCount > 1 ? Environment.ProcessorCount - 1 : 1
			};

			/// <summary>
			///     <para>Single producer consumed in serial (one at a time).</para>
			/// </summary>
			public static readonly ExecutionDataflowBlockOptions ConsumeSerial = new ExecutionDataflowBlockOptions {
				SingleProducerConstrained = true,
				MaxDegreeOfParallelism = 1
			};
		}
	}
}