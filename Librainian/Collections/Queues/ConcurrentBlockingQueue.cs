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
// File "ConcurrentBlockingQueue.cs" last formatted on 2020-08-14 at 8:31 PM.

#nullable enable

namespace Librainian.Collections.Queues {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Threading;
	using JetBrains.Annotations;
	using Utilities;

	/// <summary>Represents a thread-safe blocking, first-in, first-out collection of objects.</summary>
	/// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
	public class ConcurrentBlockingQueue<T> : ABetterClassDispose {

		private Boolean _isCompleteAdding;

		private ConcurrentQueue<T> Queue { get; } = new();

		private AutoResetEvent WorkEvent { get; } = new( false );

		/// <summary>Adds the item to the queue.</summary>
		/// <param name="item">The item to be added.</param>
		public void Add( [CanBeNull] T item ) {
			// queue must not be marked as completed adding
			if ( this._isCompleteAdding ) {
				throw new InvalidOperationException();
			}

			// queue the item
			this.Queue.Enqueue( item );

			// notify the consuming enumerable
			this.WorkEvent.Set();
		}

		/// <summary>Marks the queue as complete for adding, no additional items may be added.</summary>
		/// <remarks>After adding has been completed, any consuming enumerables will complete once the queue is empty.</remarks>
		public void CompleteAdding() {
			// mark complete
			this._isCompleteAdding = true;

			// notify the consuming enumerable
			this.WorkEvent.Set();
		}

		public override void DisposeManaged() { }

		/// <summary>Provides a consuming enumerable of the items in the queue.</summary>
		/// <remarks>
		///     The consuming enumerable dequeues as many items as possible from the queue, and blocks when it is empty until
		///     additional items are added. The consuming enumerable will
		///     not return until the queue is complete for adding, and all items have been dequeued.
		/// </remarks>
		/// <returns>The consuming enumerable.</returns>
		public IEnumerable<T> GetConsumingEnumerable() {
			do {
				// dequeue and yield as many items as are available
				while ( this.Queue.TryDequeue( out var value ) ) {
					yield return value;
				}

				// once the queue is empty, check if adding is completed and return if so
				if ( this._isCompleteAdding && this.Queue.Count == 0 ) {
					// ensure all other consuming enumerables are unblocked when complete
					this.WorkEvent.Set();

					yield break;
				}

				// otherwise, wait for additional items to be added and continue
			} while ( this.WorkEvent.WaitOne() );
		}

	}

}