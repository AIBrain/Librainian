#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Blocks.cs" was last cleaned by Rick on 2014/08/15 at 9:58 AM

#endregion License & Information

namespace Librainian.Threading {
    using System;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Measurement.Time;

    public static class Blocks {

        public static IPropagatorBlock<T, T> CreateDelayBlock<T>( Span delay ) {
            var lastItem = DateTime.MinValue;
            return new TransformBlock<T, T>( async x => {
                var waitTime = lastItem + delay - DateTime.UtcNow;
                if ( waitTime > TimeSpan.Zero ) {
                    await Task.Delay( waitTime );
                }

                lastItem = DateTime.UtcNow;

                return x;
            }, new ExecutionDataflowBlockOptions {
                BoundedCapacity = 1
            } );
        }

        public static class ManyProducers {

            /// <summary>
            ///     Multiple producers consumed in smoothly (MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded).
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeSensible = new ExecutionDataflowBlockOptions {
                SingleProducerConstrained = false,
                MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
            };

            /// <summary>
            ///     Multiple producers consumed in parallel (MaxDegreeOfParallelism = Threads.ProcessorCount).
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeParallel = new ExecutionDataflowBlockOptions {
                SingleProducerConstrained = false,
                MaxDegreeOfParallelism = Environment.ProcessorCount
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
            ///     <para>Single producer consumed in serial (one at a time).</para>
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeSerial = new ExecutionDataflowBlockOptions {
                SingleProducerConstrained = true,
                MaxDegreeOfParallelism = 1
            };

            /// <summary>
            ///     <para>Single producer consumed in smoothly (MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded).</para>
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeSensible = new ExecutionDataflowBlockOptions {
                SingleProducerConstrained = false,
                MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
            };

            /// <summary>
            ///     <para>Single producer consumed in smoothly (MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded).</para>
            /// </summary>
            public static ExecutionDataflowBlockOptions ConsumeParallel = new ExecutionDataflowBlockOptions {
                SingleProducerConstrained = true,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
        }
    }
}