// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "ConcurrentStackNoBlock.cs" last formatted on 2022-12-22 at 5:14 PM by Protiguous.

namespace Librainian.Collections.Stacks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Threading;
using Utilities;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>http://www.coderbag.com/Concurrent-Programming/Building-Concurrent-Stack</remarks>
[NeedsTesting]
public class ConcurrentStackNoBlock<T> {

	private Node? _head;

	public ConcurrentStackNoBlock() => this._head = new Node( default( T ), this._head );

	public Int32 Count { get; private set; }

	public void Add( T? item ) => this.Push( item );

	public void Add( IEnumerable<T> items ) => Parallel.ForEach( items, CPU.AllExceptOne, this.Push );

	public void Add( ParallelQuery<T> items ) => items.ForAll( this.Push );

	public Int64 LongCount() => this.Count;

	public void Push( T? item ) {
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

	public Boolean TryPop( out T? result ) {
		result = default( T );

		Node? ret;

		do {
			ret = this._head;

			if ( ret?.Next is null ) {

				//throw new IndexOutOfRangeException( "Stack is empty" );
				return false;
			}
		} while ( Interlocked.CompareExchange( ref this._head, ret.Next, ret ) != ret );

		--this.Count;
		result = ret.Item;

		return !Equals( result, default( Object? ) );
	}

	/// <summary>Attempt two <see cref="TryPop" /></summary>
	/// <param name="itemOne"></param>
	/// <param name="itemTwo"></param>
	public Boolean TryPopPop( out T? itemOne, out T? itemTwo ) {
		if ( !this.TryPop( out itemOne ) ) {
			itemTwo = default( T );

			return false;
		}

		if ( !this.TryPop( out itemTwo ) ) {
			this.Push( itemOne );

			return false;
		}

		return true;
	}

	internal class Node {

		internal readonly T? Item;

		internal Node? Next;

		public Node( T? item, Node? next = default ) {
			this.Item = item;
			this.Next = next;
		}
	}
}