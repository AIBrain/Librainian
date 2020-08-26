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
// File "BlockingQueue.cs" last formatted on 2020-08-14 at 8:31 PM.

namespace Librainian.Collections.Queues {

	using System;
	using System.Threading;
	using JetBrains.Annotations;

	public class BlockingQueue<T> {

		public BlockingQueue() {
			this.LockObj = new Object();
			this.Head = this.Tail = new Node<T>( default, null );
		}

		private Object LockObj { get; }

		public Node<T> Head { get; set; }

		public Node<T> Tail { get; set; }

		[CanBeNull]
		public T Dequeue() {
			lock ( this.LockObj ) {
				while ( this.Head.Next is null ) {
					Monitor.Wait( this.LockObj );
				}

				var retItem = this.Head.Next.Item;
				this.Head = this.Head.Next;

				return retItem;
			}
		}

		public void Enqueue( [CanBeNull] T item ) {
			var newNode = new Node<T>( item, null );

			lock ( this.LockObj ) {
				this.Tail.Next = newNode;
				this.Tail = newNode;

				Monitor.Pulse( this.LockObj );
			}
		}

	}

	public class Node<T> {

		public Node() { }

		public Node( [CanBeNull] T item, [CanBeNull] Node<T> next ) {
			this.Item = item;
			this.Next = next;
		}

		internal T Item { get; }

		internal Node<T> Next { get; set; }

	}

}