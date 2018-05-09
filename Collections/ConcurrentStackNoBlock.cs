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
// "Librainian/ConcurrentStackNoBlock.cs" was last cleaned by Protiguous on 2018/05/06 at 9:31 PM

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
                if ( ret.Next is null ) {
                    throw new IndexOutOfRangeException( message: "Stack is empty" );
                }
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
            if ( Equals( default, item ) ) {
                return;
            }

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