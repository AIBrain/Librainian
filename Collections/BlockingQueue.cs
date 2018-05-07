// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/BlockingQueue.cs" was last cleaned by Protiguous on 2018/05/06 at 9:30 PM

namespace Librainian.Collections {

    using System;
    using System.Threading;

    public class BlockingQueue<T> {
        private readonly Object _lockObj;

        private Node _head;

        private Node _tail;

        public BlockingQueue() {
            this._lockObj = new Object();
            this._head = this._tail = new Node( item: default, next: null );
        }

        public T Dequeue() {
            lock ( this._lockObj ) {
                while ( this._head.Next is null ) {
                    Monitor.Wait( this._lockObj );
                }

                var retItem = this._head.Next.Item;
                this._head = this._head.Next;

                return retItem;
            }
        }

        public void Enqueue( T item ) {
            var newNode = new Node( item: item, next: null );

            lock ( this._lockObj ) {
                this._tail.Next = newNode;
                this._tail = newNode;

                Monitor.Pulse( this._lockObj );
            }
        }

        internal class Node {
            internal T Item;
            internal Node Next;

            public Node() { }

            public Node( T item, Node next ) {
                this.Item = item;
                this.Next = next;
            }
        }
    }
}