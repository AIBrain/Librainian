// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "LockfreeQueue.cs" belongs to Rick@AIBrain.org and
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
// File "LockfreeQueue.cs" was last formatted by Protiguous on 2018/06/04 at 3:43 PM.

namespace Librainian.Collections {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using JetBrains.Annotations;

	/// <summary>
	///     Represents a lock-free, thread-safe, first-in, first-out collection of objects.
	/// </summary>
	/// <typeparam name="T">specifies the type of the elements in the queue</typeparam>
	/// <remarks>Enumeration and clearing are not thread-safe.</remarks>
	public class LockfreeQueue<T> : IEnumerable<T> where T : class {

		/// <summary>
		///     Returns an enumerator that iterates through the queue.
		/// </summary>
		/// <returns>an enumerator for the queue</returns>
		public IEnumerator<T> GetEnumerator() {
			var currentNode = this._head;

			do {
				if ( currentNode.Item is null ) { yield break; }

				yield return currentNode.Item;
			} while ( ( currentNode = currentNode.Next ) != null );
		}

		/// <summary>
		///     Returns an enumerator that iterates through the queue.
		/// </summary>
		/// <returns>an enumerator for the queue</returns>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		/// <summary>
		///     Gets the number of elements contained in the queue.
		/// </summary>
		public Int32 Count => Thread.VolatileRead( address: ref this._count );

		private Int32 _count;

		private SingleLinkNode<T> _head = new SingleLinkNode<T>();

		private SingleLinkNode<T> _tail;

		/// <summary>
		///     Clears the queue.
		/// </summary>
		/// <remarks>This method is not thread-safe.</remarks>
		public void Clear() {
			var currentNode = this._head;

			while ( currentNode != null ) {
				var tempNode = currentNode;
				currentNode = currentNode.Next;

				tempNode.Item = default;
				tempNode.Next = null;
			}

			this._head = new SingleLinkNode<T>();
			this._tail = this._head;
			this._count = 0;
		}

		/// <summary>
		///     Removes and returns the object at the beginning of the queue.
		/// </summary>
		/// <returns>the object that is removed from the beginning of the queue</returns>
		public T Dequeue() {
			if ( !this.TryDequeue( item: out var result ) ) { throw new InvalidOperationException( "the queue is empty" ); }

			return result;
		}

		/// <summary>
		///     Adds an object to the end of the queue.
		/// </summary>
		/// <param name="item">the object to add to the queue</param>
		public void Enqueue( T item ) {
			SingleLinkNode<T> oldTail = null;

			var newNode = new SingleLinkNode<T> { Item = item };

			var newNodeWasAdded = false;

			while ( !newNodeWasAdded ) {
				oldTail = this._tail;
				var oldTailNext = oldTail.Next;

				if ( this._tail != oldTail ) { continue; }

				if ( oldTailNext is null ) { newNodeWasAdded = Interlocked.CompareExchange( location1: ref this._tail.Next, newNode, comparand: null ) == null; }
				else { Interlocked.CompareExchange( location1: ref this._tail, oldTailNext, comparand: oldTail ); }
			}

			Interlocked.CompareExchange( location1: ref this._tail, newNode, comparand: oldTail );
			Interlocked.Increment( location: ref this._count );
		}

		public T TryDequeue() {
			this.TryDequeue( item: out var item );

			return item;
		}

		/// <summary>
		///     Removes and returns the object at the beginning of the queue.
		/// </summary>
		/// <param name="item">
		///     when the method returns, contains the object removed from the beginning of the queue, if the queue
		///     is not empty; otherwise it is the default value for the element type
		/// </param>
		/// <returns>true if an object from removed from the beginning of the queue; false if the queue is empty</returns>
		public Boolean TryDequeue( out T item ) {
			item = default;

			var haveAdvancedHead = false;

			while ( !haveAdvancedHead ) {
				var oldTail = this._tail;
				var oldHead = this._head;

				if ( oldHead != this._head ) { continue; }

				var oldHeadNext = this._head.Next;

				if ( oldHead == oldTail ) {
					if ( oldHeadNext is null ) { return false; }

					Interlocked.CompareExchange( location1: ref this._tail, oldHeadNext, comparand: oldTail );
				}
				else {
					item = oldHeadNext.Item;
					haveAdvancedHead = Interlocked.CompareExchange( location1: ref this._head, oldHeadNext, comparand: oldHead ) == oldHead;
				}
			}

			Interlocked.Decrement( location: ref this._count );

			return true;
		}

		/// <summary>
		///     Default constructor.
		/// </summary>
		public LockfreeQueue() => this._tail = this._head;

		public LockfreeQueue( [NotNull] IEnumerable<T> items ) : this() {
			foreach ( var item in items ) { this.Enqueue( item: item ); }
		}

	}

}