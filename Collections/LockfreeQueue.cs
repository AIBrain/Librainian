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
// "Librainian/LockfreeQueue.cs" was last cleaned by Rick on 2014/08/11 at 12:36 AM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    ///     Represents a lock-free, thread-safe, first-in, first-out collection of objects.
    /// </summary>
    /// <typeparam name="T">specifies the type of the elements in the queue</typeparam>
    /// <remarks>Enumeration and clearing are not thread-safe.</remarks>
    public class LockfreeQueue< T > : IEnumerable< T > where T : class {
        private int _count;

        private SingleLinkNode< T > _head = new SingleLinkNode< T >();

        private SingleLinkNode< T > _tail;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public LockfreeQueue() {
            this._tail = this._head;
        }

        public LockfreeQueue( IEnumerable< T > items ) : this() {
            foreach ( var item in items ) {
                this.Enqueue( item );
            }
        }

        /// <summary>
        ///     Gets the number of elements contained in the queue.
        /// </summary>
        public int Count => Thread.VolatileRead( ref this._count );

        /// <summary>
        ///     Returns an enumerator that iterates through the queue.
        /// </summary>
        /// <returns>an enumerator for the queue</returns>
        public IEnumerator< T > GetEnumerator() {
            var currentNode = this._head;

            do {
                if ( currentNode.Item == null ) {
                    yield break;
                }
                yield return currentNode.Item;
            } while ( ( currentNode = currentNode.Next ) != null );
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the queue.
        /// </summary>
        /// <returns>an enumerator for the queue</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        ///     Clears the queue.
        /// </summary>
        /// <remarks>This method is not thread-safe.</remarks>
        public void Clear() {
            var currentNode = this._head;

            while ( currentNode != null ) {
                var tempNode = currentNode;
                currentNode = currentNode.Next;

                tempNode.Item = default( T );
                tempNode.Next = null;
            }

            this._head = new SingleLinkNode< T >();
            this._tail = this._head;
            this._count = 0;
        }

        /// <summary>
        ///     Removes and returns the object at the beginning of the queue.
        /// </summary>
        /// <returns>the object that is removed from the beginning of the queue</returns>
        public T Dequeue() {
            T result;

            if ( !this.TryDequeue( out result ) ) {
                throw new InvalidOperationException( "the queue is empty" );
            }

            return result;
        }

        /// <summary>
        ///     Adds an object to the end of the queue.
        /// </summary>
        /// <param name="item">the object to add to the queue</param>
        public void Enqueue( T item ) {
            SingleLinkNode< T > oldTail = null;

            var newNode = new SingleLinkNode< T > {
                                                      Item = item
                                                  };

            var newNodeWasAdded = false;

            while ( !newNodeWasAdded ) {
                oldTail = this._tail;
                var oldTailNext = oldTail.Next;

                if ( this._tail != oldTail ) {
                    continue;
                }
                if ( oldTailNext == null ) {
                    newNodeWasAdded = Interlocked.CompareExchange( ref this._tail.Next, newNode, null ) == null;
                }
                else {
                    Interlocked.CompareExchange( ref this._tail, oldTailNext, oldTail );
                }
            }

            Interlocked.CompareExchange( ref this._tail, newNode, oldTail );
            Interlocked.Increment( ref this._count );
        }

        public T TryDequeue() {
            T item;
            this.TryDequeue( out item );
            return item;
        }

        /// <summary>
        ///     Removes and returns the object at the beginning of the queue.
        /// </summary>
        /// <param name="item">
        ///     when the method returns, contains the object removed from the beginning of the queue, if
        ///     the queue is not empty; otherwise it is the default value for the element type
        /// </param>
        /// <returns>
        ///     true if an object from removed from the beginning of the queue; false if the queue is empty
        /// </returns>
        public Boolean TryDequeue( out T item ) {
            item = default( T );

            var haveAdvancedHead = false;
            while ( !haveAdvancedHead ) {
                var oldTail = this._tail;
                var oldHead = this._head;

                if ( oldHead != this._head ) {
                    continue;
                }

                var oldHeadNext = this._head.Next;

                if ( oldHead == oldTail ) {
                    if ( oldHeadNext == null ) {
                        return false;
                    }

                    Interlocked.CompareExchange( ref this._tail, oldHeadNext, oldTail );
                }
                else {
                    item = oldHeadNext.Item;
                    haveAdvancedHead = Interlocked.CompareExchange( ref this._head, oldHeadNext, oldHead ) == oldHead;
                }
            }

            Interlocked.Decrement( ref this._count );
            return true;
        }
    }
}
