// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ConcurrentStackNoBlock.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "ConcurrentStackNoBlock.cs" was last formatted by Protiguous on 2018/06/04 at 3:43 PM.

namespace Librainian.Collections {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Threading;

	public class ConcurrentNoBlockStackL<T> {

		private volatile Node _head;

		public T Pop() {
			Node ret;

			do {
				ret = this._head;

				if ( ret.Next is null ) { throw new IndexOutOfRangeException( "Stack is empty" ); }
			} while ( Interlocked.CompareExchange( location1: ref this._head, ret.Next, comparand: ret ) != ret );

			return ret.Item;
		}

		public void Push( T item ) {
			var nodeNew = new Node { Item = item };

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

			public Node( T item, Node next ) {
				this.Item = item;
				this.Next = next;
			}

		}

		public ConcurrentNoBlockStackL() => this._head = new Node( item: default, next: this._head );

	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>http://www.coderbag.com/Concurrent-Programming/Building-Concurrent-Stack</remarks>
	public class ConcurrentStackNoBlock<T> {

		public Int32 Count { get; private set; }

		private Node _head;

		public void Add( T item ) => this.Push( item: item );

		public void Add( [NotNull] IEnumerable<T> items ) => Parallel.ForEach( source: items, parallelOptions: ThreadingExtensions.CPUIntensive, body: this.Push );

		public void Add( [NotNull] ParallelQuery<T> items ) => items.ForAll( this.Push );

		public Int64 LongCount() => this.Count;

		public void Push( T item ) {
			if ( Equals( default, item ) ) { return; }

			var nodeNew = new Node { Item = item };

			Node tmp;

			do {
				tmp = this._head;
				nodeNew.Next = tmp;
			} while ( Interlocked.CompareExchange( location1: ref this._head, nodeNew, comparand: tmp ) != tmp );

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

		public ConcurrentStackNoBlock() => this._head = new Node( item: default, next: this._head );

	}

}