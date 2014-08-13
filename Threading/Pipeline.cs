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
// "Librainian/Pipeline.cs" was last cleaned by Rick on 2014/08/11 at 12:41 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Annotations;

    public class Pipeline< TInput, TOutput > {
        /// <summary>
        ///     the function to use
        /// </summary>
        private readonly Func< TInput, TOutput > _pipelineFunction;

        /// <summary>
        ///     queue based blocking collection
        /// </summary>
        private BlockingCollection< ValueCallBackWrapper > _valueQueue;

        public Pipeline( [NotNull] Func< TInput, TOutput > function ) {
            if ( function == null ) {
                throw new ArgumentNullException( "function" );
            }
            this._pipelineFunction = function;
        }

        public Pipeline< TInput, TNewOutput > AddFunction< TNewOutput >( [NotNull] Func< TOutput, TNewOutput > newfunction ) {
            // create a composite function
            if ( newfunction == null ) {
                throw new ArgumentNullException( "newfunction" );
            }
            Func< TInput, TNewOutput > compositeFunction = ( inputValue => newfunction( this._pipelineFunction( inputValue ) ) );

            // return a new pipeline around the composite function
            return new Pipeline< TInput, TNewOutput >( compositeFunction );
        }

        public void AddValue( TInput value, [NotNull] Action< TInput, TOutput > callback ) {
            // add the value to the queue for processing
            if ( callback == null ) {
                throw new ArgumentNullException( "callback" );
            }
            this._valueQueue.Add( new ValueCallBackWrapper {
                                                               Value = value,
                                                               Callback = callback
                                                           } );
        }

        public Task StartProcessing() {
            // initialize the collection

            this._valueQueue = new BlockingCollection< ValueCallBackWrapper >();

            // create a parallel loop to consume

            // items from the collection

           return Task.Run( () => { Parallel.ForEach( this._valueQueue.GetConsumingEnumerable(), wrapper => wrapper.Callback( wrapper.Value, this._pipelineFunction( wrapper.Value ) ) ); } );
        }

        public void StopProcessing() {
            // signal to the collection that no further values will be added
            this._valueQueue.CompleteAdding();
        }

        private sealed class ValueCallBackWrapper {
            public Action< TInput, TOutput > Callback;

            public TInput Value;
        }
    }
}
