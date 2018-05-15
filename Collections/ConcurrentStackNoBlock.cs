// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "ConcurrentStackNoBlock.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original
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
// "Librainian/ConcurrentStackNoBlock.cs" was last cleaned by Protiguous on 2018/05/15 at 1:28 AM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Threading;

    public class ConcurrentNoBlockStackL<T> {

        private volatile Node _head;

        public ConcurrentNoBlockStackL() => this._head = new Node( item: default, next: this._head );

        public T Pop() {
            Node ret;

            do {
                ret = this._head;

                if ( ret.Next is null ) { throw new IndexOutOfRangeException( message: "Stack is empty" ); }
#pragma warning disable 420
            } while ( Interlocked.CompareExchange( location1: ref this._head, value: ret.Next, comparand: ret ) != ret );
#pragma warning restore 420
            return ret.Item;
        }

        public void Push( T item ) {
            var nodeNew = new Node { Item = item };

            Node tmp;

            do {
                tmp = this._head;
                nodeNew.Next = tmp;
#pragma warning disable 420
            } while ( Interlocked.CompareExchange( location1: ref this._head, value: nodeNew, comparand: tmp ) != tmp );
#pragma warning restore 420
        }

        internal sealed class Node {

            internal T Item;

            internal Node Next;

            public Node() { }

            public Node( T item, Node next ) {
                this.Item = item;
                this.Next = next;
            }
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <see cref="http://www.coderbag.com/Concurrent-Programming/Building-Concurrent-Stack"/>
    public class ConcurrentStackNoBlock<T> {

        private Node _head;

        public ConcurrentStackNoBlock() => this._head = new Node( item: default, next: this._head );

        public Int32 Count { get; private set; }

        public void Add( T item ) => this.Push( item: item );

        public void Add( IEnumerable<T> items ) => Parallel.ForEach( source: items, parallelOptions: ThreadingExtensions.CPUIntensive, body: this.Push );

        public void Add( ParallelQuery<T> items ) => items.ForAll( this.Push );

        public Int64 LongCount() => this.Count;

        public void Push( T item ) {
            if ( Equals( default, item ) ) { return; }

            var nodeNew = new Node { Item = item };

            Node tmp;

            do {
                tmp = this._head;
                nodeNew.Next = tmp;
#pragma warning disable 420
            } while ( Interlocked.CompareExchange( location1: ref this._head, value: nodeNew, comparand: tmp ) != tmp );
#pragma warning restore 420
            ++this.Count;
        }

        public Boolean TryPop( out T result ) {
            result = default;

            Node ret;

            do {
                ret = this._head;

                if ( ret.Next == default( Node ) ) {

                    //throw new IndexOutOfRangeException( "Stack is empty" );
                    return false;
                }
#pragma warning disable 420
            } while ( Interlocked.CompareExchange( location1: ref this._head, value: ret.Next, comparand: ret ) != ret );
#pragma warning restore 420
            --this.Count;
            result = ret.Item;

            return !Equals( result, default );
        }

        /// <summary>
        /// Attempt two <see cref="TryPop"/>
        /// </summary>
        /// <param name="itemOne"></param>
        /// <param name="itemTwo"></param>
        /// <returns></returns>
        public Boolean TryPopPop( out T itemOne, out T itemTwo ) {
            if ( !this.TryPop( result: out itemOne ) ) {
                itemTwo = default;

                return false;
            }

            if ( !this.TryPop( result: out itemTwo ) ) {
                this.Push( item: itemOne );

                return false;
            }

            return true;
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