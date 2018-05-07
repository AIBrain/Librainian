// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Pipeline.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Magic;

    public class Pipeline<TInput, TOutput> : ABetterClassDispose {

        /// <summary>the function to use</summary>
        private readonly Func<TInput, TOutput> _pipelineFunction;

        /// <summary>queue based blocking collection</summary>
        private BlockingCollection<ValueCallBackWrapper> _valueQueue;

        public Pipeline( [NotNull] Func<TInput, TOutput> function ) => this._pipelineFunction = function ?? throw new ArgumentNullException( nameof( function ) );

	    public Pipeline<TInput, TNewOutput> AddFunction<TNewOutput>( [NotNull] Func<TOutput, TNewOutput> newfunction ) {

            // create a composite function
            if ( newfunction is null ) {
                throw new ArgumentNullException( nameof( newfunction ) );
            }

	        TNewOutput CompositeFunction( TInput inputValue ) => newfunction( this._pipelineFunction( inputValue ) );

	        // return a new pipeline around the composite function
            return new Pipeline<TInput, TNewOutput>( CompositeFunction );
        }

        public void AddValue( TInput value, [NotNull] Action<TInput, TOutput> callback ) {

            // add the value to the queue for processing
            if ( callback is null ) {
                throw new ArgumentNullException( nameof( callback ) );
            }
            this._valueQueue.Add( new ValueCallBackWrapper { Value = value, Callback = callback } );
        }

        public Task StartProcessing() {

            // initialize the collection

            this._valueQueue = new BlockingCollection<ValueCallBackWrapper>();

            // create a parallel loop to consume

            // items from the collection

            return Task.Run( () => {
                Parallel.ForEach( this._valueQueue.GetConsumingEnumerable(), wrapper => wrapper.Callback( wrapper.Value, this._pipelineFunction( wrapper.Value ) ) );
            } );
        }

        public void StopProcessing() => this._valueQueue.CompleteAdding();

        private sealed class ValueCallBackWrapper {

            public Action<TInput, TOutput> Callback {
                get; set;
            }

            public TInput Value {
                get; set;
            }
        }

		/// <summary>
		/// Dispose any disposable members.
		/// </summary>
		protected override void DisposeManaged() => this._valueQueue.Dispose();

	}
}