// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Pipeline.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Pipeline.cs" was last formatted by Protiguous on 2018/07/13 at 1:40 AM.

namespace Librainian.Threading {

	using System;
	using System.Collections.Concurrent;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Magic;

	public class Pipeline<TInput, TOutput> : ABetterClassDispose {

		/// <summary>
		///     the function to use
		/// </summary>
		private readonly Func<TInput, TOutput> _pipelineFunction;

		/// <summary>
		///     queue based blocking collection
		/// </summary>
		private BlockingCollection<ValueCallBackWrapper> _valueQueue;

		public Pipeline( [NotNull] Func<TInput, TOutput> function ) => this._pipelineFunction = function ?? throw new ArgumentNullException( nameof( function ) );

		[NotNull]
		public Pipeline<TInput, TNewOutput> AddFunction<TNewOutput>( [NotNull] Func<TOutput, TNewOutput> newfunction ) {

			// create a composite function
			if ( newfunction is null ) { throw new ArgumentNullException( nameof( newfunction ) ); }

			TNewOutput CompositeFunction( TInput inputValue ) => newfunction( this._pipelineFunction( inputValue ) );

			// return a new pipeline around the composite function
			return new Pipeline<TInput, TNewOutput>( CompositeFunction );
		}

		public void AddValue( TInput value, [NotNull] Action<TInput, TOutput> callback ) {

			// add the value to the queue for processing
			if ( callback is null ) { throw new ArgumentNullException( nameof( callback ) ); }

			this._valueQueue.Add( new ValueCallBackWrapper {
				Value = value,
				Callback = callback
			} );
		}

		public override void DisposeManaged() => this._valueQueue.Dispose();

		[NotNull]
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

			public Action<TInput, TOutput> Callback { get; set; }

			public TInput Value { get; set; }
		}
	}
}