// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "Blocks.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license has been
// overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Blocks.cs" was last cleaned by Protiguous on 2018/05/15 at 4:23 AM.

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

                if ( waitTime > TimeSpan.Zero ) { await Task.Delay( waitTime ); }

                lastItem = DateTime.UtcNow;

                return x;
            }, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 } );
        }

        public static class ManyProducers {

            /// <summary>
            /// Multiple producers consumed in smoothly ( <see cref="Environment.ProcessorCount"/> * <see cref="Environment.ProcessorCount"/> ).
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeEverything =
                new ExecutionDataflowBlockOptions { SingleProducerConstrained = false, MaxDegreeOfParallelism = Environment.ProcessorCount * Environment.ProcessorCount };

            /// <summary>
            /// Multiple producers consumed in smoothly (Environment.ProcessorCount - 1).
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeSensible =
                new ExecutionDataflowBlockOptions { SingleProducerConstrained = false, MaxDegreeOfParallelism = Environment.ProcessorCount > 1 ? Environment.ProcessorCount - 1 : 1 };

            /// <summary>
            /// Multiple producers consumed in serial (MaxDegreeOfParallelism = 1).
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeSerial = new ExecutionDataflowBlockOptions { SingleProducerConstrained = false, MaxDegreeOfParallelism = 1 };
        }

        public static class SingleProducer {

            /// <summary>
            /// <para>Single producer consumed in smoothly (Environment.ProcessorCount - 1).</para>
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeSensible =
                new ExecutionDataflowBlockOptions { SingleProducerConstrained = false, MaxDegreeOfParallelism = Environment.ProcessorCount > 1 ? Environment.ProcessorCount - 1 : 1 };

            /// <summary>
            /// <para>Single producer consumed in serial (one at a time).</para>
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeSerial = new ExecutionDataflowBlockOptions { SingleProducerConstrained = true, MaxDegreeOfParallelism = 1 };
        }
    }
}