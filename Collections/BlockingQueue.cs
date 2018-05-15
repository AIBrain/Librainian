// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "BlockingQueue.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license
// has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/BlockingQueue.cs" was last cleaned by Protiguous on 2018/05/15 at 1:28 AM.

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
                while ( this._head.Next is null ) { Monitor.Wait( this._lockObj ); }

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