﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
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
// Project: "Librainian", "Blocks.cs" was last formatted by Protiguous on 2019/11/24 at 2:30 PM.

namespace LibrainianCore.Threading {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Measurement.Time;

    public static class Blocks {

        [NotNull]
        public static IPropagatorBlock<T, T> CreateDelayBlock<T>( [NotNull] SpanOfTime delay ) {
            if ( delay is null ) {
                throw new ArgumentNullException( paramName: nameof( delay ) );
            }

            var lastItem = DateTime.MinValue;

            return new TransformBlock<T, T>( async x => {
                var waitTime = lastItem + delay - DateTime.UtcNow;

                if ( waitTime > TimeSpan.Zero ) {
                    await Task.Delay( waitTime ).ConfigureAwait( false );
                }

                lastItem = DateTime.UtcNow;

                return x;
            }, new ExecutionDataflowBlockOptions {
                BoundedCapacity = 1
            } );
        }

        public static class ManyProducers {

            /// <summary>Multiple producers consumed in smoothly ( <see cref="Environment.ProcessorCount" /> * <see cref="Environment.ProcessorCount" /> ).</summary>
            [NotNull]
            public static ExecutionDataflowBlockOptions ConsumeEverything( CancellationToken? token ) =>
                new ExecutionDataflowBlockOptions {
                    SingleProducerConstrained = false,
                    MaxDegreeOfParallelism = Environment.ProcessorCount * Environment.ProcessorCount,
                    EnsureOrdered = true,
                    CancellationToken = token ?? CancellationToken.None
                };

            /// <summary>Multiple producers consumed in smoothly (Environment.ProcessorCount - 1).</summary>
            [NotNull]
            public static ExecutionDataflowBlockOptions ConsumeSensible( CancellationToken? token ) =>
                new ExecutionDataflowBlockOptions {
                    SingleProducerConstrained = false,
                    MaxDegreeOfParallelism = Environment.ProcessorCount > 2 ? Environment.ProcessorCount - 2 : 1,
                    EnsureOrdered = true,
                    CancellationToken = token ?? CancellationToken.None
                };

            /// <summary>Multiple producers consumed in serial (MaxDegreeOfParallelism = 1).</summary>
            [NotNull]
            public static ExecutionDataflowBlockOptions ConsumeSerial( CancellationToken? token ) =>
                new ExecutionDataflowBlockOptions {
                    SingleProducerConstrained = false, MaxDegreeOfParallelism = 1, EnsureOrdered = true, CancellationToken = token ?? CancellationToken.None
                };

        }

        public static class SingleProducer {

            /// <summary>
            ///     <para>Single producer consumed in smoothly (Environment.ProcessorCount - 1).</para>
            /// </summary>
            [NotNull]
            public static ExecutionDataflowBlockOptions ConsumeSensible( CancellationToken? token ) =>
                new ExecutionDataflowBlockOptions {
                    SingleProducerConstrained = false,
                    MaxDegreeOfParallelism = Environment.ProcessorCount > 2 ? Environment.ProcessorCount - 2 : 1,
                    EnsureOrdered = true,
                    CancellationToken = token ?? CancellationToken.None
                };

            /// <summary>
            ///     <para>Single producer consumed in serial (one at a time).</para>
            /// </summary>
            [NotNull]
            public static ExecutionDataflowBlockOptions ConsumeSerial( CancellationToken? token ) =>
                new ExecutionDataflowBlockOptions {
                    SingleProducerConstrained = true, MaxDegreeOfParallelism = 1, EnsureOrdered = true, CancellationToken = token ?? CancellationToken.None
                };

        }

    }

}