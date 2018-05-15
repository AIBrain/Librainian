// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "ConcurrentBlockingQueue.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original
// license has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/ConcurrentBlockingQueue.cs" was last cleaned by Protiguous on 2018/05/15 at 1:28 AM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using Magic;

    /// <summary>
    /// Represents a thread-safe blocking, first-in, first-out collection of objects.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    public class ConcurrentBlockingQueue<T> : ABetterClassDispose {

        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        private readonly AutoResetEvent _workEvent = new AutoResetEvent( initialState: false );

        private Boolean _isCompleteAdding;

        /// <summary>
        /// Adds the item to the queue.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void Add( T item ) {

            // queue must not be marked as completed adding
            if ( this._isCompleteAdding ) { throw new InvalidOperationException(); }

            // queue the item
            this._queue.Enqueue( item: item );

            // notify the consuming enumerable
            this._workEvent.Set();
        }

        /// <summary>
        /// Marks the queue as complete for adding, no additional items may be added.
        /// </summary>
        /// <remarks>After adding has been completed, any consuming enumerables will complete once the queue is empty.</remarks>
        public void CompleteAdding() {

            // mark complete
            this._isCompleteAdding = true;

            // notify the consuming enumerable
            this._workEvent.Set();
        }

        public override void DisposeManaged() { }

        /// <summary>
        /// Provides a consuming enumerable of the items in the queue.
        /// </summary>
        /// <remarks>
        /// The consuming enumerable dequeues as many items as possible from the queue, and blocks when it is empty until additional items are added. The consuming enumerable will not return until the queue is complete
        /// for adding, and all items have been dequeued.
        /// </remarks>
        /// <returns>The consuming enumerable.</returns>
        public IEnumerable<T> GetConsumingEnumerable() {
            do {

                // dequeue and yield as many items as are available
                while ( this._queue.TryDequeue( result: out var value ) ) { yield return value; }

                // once the queue is empty, check if adding is completed and return if so
                if ( this._isCompleteAdding && this._queue.Count == 0 ) {

                    // ensure all other consuming enumerables are unblocked when complete
                    this._workEvent.Set();

                    yield break;
                }

                // otherwise, wait for additional items to be added and continue
            } while ( this._workEvent.WaitOne() );
        }
    }
}