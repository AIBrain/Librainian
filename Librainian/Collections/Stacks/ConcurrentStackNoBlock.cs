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
// File "ConcurrentStackNoBlock.cs" last formatted on 2020-08-14 at 8:31 PM.

namespace Librainian.Collections.Stacks {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Threading;

	public class ConcurrentNoBlockStackL<T> {

		[CanBeNull]
		private volatile Node? _head;

		public ConcurrentNoBlockStackL() => this._head = new Node( default( T ), this._head );

		[CanBeNull]
		public T Pop() {
			Node? ret;

			do {
				ret = this._head;

				if ( ret?.Next is null ) {
					throw new IndexOutOfRangeException( "Stack is empty" );
				}
			} while ( Interlocked.CompareExchange( ref this._head, ret.Next, ret ) != ret );

			return ret.Item;
		}

		public void Push( [CanBeNull] T item ) {
			var nodeNew = new Node( item );

			Node? tmp;

			do {
				tmp = this._head;
				nodeNew.Next = tmp;
			} while ( Interlocked.CompareExchange( ref this._head, nodeNew, tmp ) != tmp );
		}

		internal sealed class Node {

			internal readonly T Item;

			internal Node? Next;

			public Node( [CanBeNull] T item, [CanBeNull] Node? next = null ) {
				this.Item = item;
				this.Next = next;
			}

		}

	}

	/// <summary></summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>http://www.coderbag.com/Concurrent-Programming/Building-Concurrent-Stack</remarks>
	public class ConcurrentStackNoBlock<T> {

		private Node? _head;

		public ConcurrentStackNoBlock() => this._head = new Node( default( T ), this._head );

		public Int32 Count { get; private set; }

		public void Add( [CanBeNull] T item ) => this.Push( item );

		public void Add( [NotNull] IEnumerable<T> items ) => Parallel.ForEach( items, CPU.AllExceptOne, this.Push );

		public void Add( [NotNull] ParallelQuery<T> items ) => items.ForAll( this.Push );

		public Int64 LongCount() => this.Count;

		public void Push( [CanBeNull] T item ) {
			if ( Equals( default( Object? ), item ) ) {
				return;
			}

			var nodeNew = new Node( item );

			Node? tmp;

			do {
				tmp = this._head;
				nodeNew.Next = tmp;
			} while ( Interlocked.CompareExchange( ref this._head, nodeNew, tmp ) != tmp );

			++this.Count;
		}

		public Boolean TryPop( [CanBeNull] out T result ) {
			result = default( T )!;

			Node? ret;

			do {
				ret = this._head;

				if ( ret?.Next is null ) {
					//throw new IndexOutOfRangeException( "Stack is empty" );
					return default( Boolean );
				}
			} while ( Interlocked.CompareExchange( ref this._head, ret.Next, ret ) != ret );

			--this.Count;
			result = ret.Item;

			return !Equals( result, default( Object? ) );
		}

		/// <summary>Attempt two <see cref="TryPop" /></summary>
		/// <param name="itemOne"></param>
		/// <param name="itemTwo"></param>
		/// <returns></returns>
		public Boolean TryPopPop( [CanBeNull] out T itemOne, [CanBeNull] out T itemTwo ) {
			if ( !this.TryPop( out itemOne ) ) {
				itemTwo = default( T );

				return default( Boolean );
			}

			if ( !this.TryPop( out itemTwo ) ) {
				this.Push( itemOne );

				return default( Boolean );
			}

			return true;
		}

		internal class Node {

			internal T Item;

			internal Node? Next;

			public Node( [CanBeNull] T item, [CanBeNull] Node? next = default ) {
				this.Item = item;
				this.Next = next;
			}

		}

	}

}