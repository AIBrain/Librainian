// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentStackNoBlock.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "ConcurrentStackNoBlock.cs" was last formatted by Protiguous on 2019/08/08 at 6:38 AM.

namespace Librainian.Collections.Stacks {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Threading;

    public class ConcurrentNoBlockStackL<T> {

        private volatile Node _head;

        public ConcurrentNoBlockStackL() => this._head = new Node( item: default, next: this._head );

        [CanBeNull]
        public T Pop() {
            Node ret;

            do {
                ret = this._head;

                if ( ret.Next is null ) {
                    throw new IndexOutOfRangeException( "Stack is empty" );
                }
            } while ( Interlocked.CompareExchange( location1: ref this._head, ret.Next, comparand: ret ) != ret );

            return ret.Item;
        }

        public void Push( [CanBeNull] T item ) {
            var nodeNew = new Node {
                Item = item
            };

            Node tmp;

            do {
                tmp = this._head;
                nodeNew.Next = tmp;
            } while ( Interlocked.CompareExchange( location1: ref this._head, nodeNew, comparand: tmp ) != tmp );
        }

        internal sealed class Node {

            internal T Item;

            internal Node Next;

            public Node() { }

            public Node( [CanBeNull] T item, [CanBeNull] Node next ) {
                this.Item = item;
                this.Next = next;
            }
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>http://www.coderbag.com/Concurrent-Programming/Building-Concurrent-Stack</remarks>
    public class ConcurrentStackNoBlock<T> {

        private Node _head;

        public Int32 Count { get; private set; }

        public ConcurrentStackNoBlock() => this._head = new Node( item: default, next: this._head );

        public void Add( [CanBeNull] T item ) => this.Push( item: item );

        public void Add( [NotNull] IEnumerable<T> items ) => Parallel.ForEach( source: items, parallelOptions: CPU.AllCPUExceptOne, body: this.Push );

        public void Add( [NotNull] ParallelQuery<T> items ) => items.ForAll( this.Push );

        public Int64 LongCount() => this.Count;

        public void Push( [CanBeNull] T item ) {
            if ( Equals( default, item ) ) {
                return;
            }

            var nodeNew = new Node {
                Item = item
            };

            Node tmp;

            do {
                tmp = this._head;
                nodeNew.Next = tmp;
            } while ( Interlocked.CompareExchange( location1: ref this._head, nodeNew, comparand: tmp ) != tmp );

            ++this.Count;
        }

        public Boolean TryPop( [CanBeNull] out T result ) {
            result = default;

            Node ret;

            do {
                ret = this._head;

                if ( ret.Next == default( Node ) ) {

                    //throw new IndexOutOfRangeException( "Stack is empty" );
                    return default;
                }
            } while ( Interlocked.CompareExchange( location1: ref this._head, ret.Next, comparand: ret ) != ret );

            --this.Count;
            result = ret.Item;

            return !Equals( result, default );
        }

        /// <summary>
        ///     Attempt two <see cref="TryPop" />
        /// </summary>
        /// <param name="itemOne"></param>
        /// <param name="itemTwo"></param>
        /// <returns></returns>
        public Boolean TryPopPop( [CanBeNull] out T itemOne, [CanBeNull] out T itemTwo ) {
            if ( !this.TryPop( result: out itemOne ) ) {
                itemTwo = default;

                return default;
            }

            if ( !this.TryPop( result: out itemTwo ) ) {
                this.Push( item: itemOne );

                return default;
            }

            return true;
        }

        internal class Node {

            internal T Item;

            internal Node Next;

            public Node() { }

            public Node( [CanBeNull] T item, [CanBeNull] Node next ) {
                this.Item = item;
                this.Next = next;
            }
        }
    }
}