﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "ThreadSafeEnumerable.cs" last formatted on 2021-11-30 at 7:23 PM by Protiguous.

#nullable enable

namespace Librainian.Threadsafe;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Exceptions;

public class ThreadSafeEnumerable<T> : IEnumerable<T> {

	public ThreadSafeEnumerable( IEnumerable<T> original ) => this.Original = original ?? throw new NullException( nameof( original ) );

	private IEnumerable<T> Original { get; }

	public IEnumerator<T> GetEnumerator() => new ThreadSafeEnumerator( this.Original.GetEnumerator() );

	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

	private sealed class ThreadSafeEnumerator : IEnumerator<T> {

		internal ThreadSafeEnumerator( IEnumerator<T> original ) => this.original = original ?? throw new NullException( nameof( original ) );

		private ThreadLocal<T?> current { get; } = new();

		private IEnumerator<T> original { get; }

		private Object padlock { get; } = new();

		public T? Current => this.current.Value;

		Object? IEnumerator.Current => this.Current;

		public void Dispose() {
			using ( this.original ) { }

			using ( this.current ) { }
		}

		public Boolean MoveNext() {
			lock ( this.padlock ) {
				var next = this.original.MoveNext();

				if ( next ) {
					this.current.Value = this.original.Current;
				}

				return next;
			}
		}

		public void Reset() => throw new NotSupportedException();
	}
}