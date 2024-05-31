// Copyright © Protiguous. All Rights Reserved.
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
// File "Library.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Linguistics;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Exceptions;
using Newtonsoft.Json;

/// <summary>
/// <para>A <see cref="Library" /> is a cluster of <see cref="Book" /> s.</para>
/// </summary>
[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
[Serializable]
public record Library : IEnumerable<(UDC udc, Book book)> {
	public Library( UDC udc, Book book ) => this.Add( udc, book );

	[JsonProperty]
	private ConcurrentDictionary<UDC, Book> Books { get; } = new();

	IEnumerator<(UDC udc, Book book)> IEnumerable<(UDC udc, Book book)>.GetEnumerator() => ( IEnumerator<(UDC udc, Book book)> )this.GetEnumerator();

	public IEnumerator GetEnumerator() => ( ( IEnumerable )this.Books ).GetEnumerator();

	public virtual Boolean Equals( Library? other ) => Equals( this, other );

	/// <summary>Static equality test</summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equals( Library? left, Library? right ) {
		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		if ( left is null || right is null ) {
			return false;
		}

		//shouldn't this be more of a set-type comparison? If all A are contained in B or all B are contained in A then true?
		return left.OrderBy( pair => pair.udc ).SequenceEqual( right.OrderBy( pair => pair.udc ) );
	}

	public Boolean Add( UDC udc, Book book ) {
		if ( udc is null ) {
			throw new NullException( nameof( udc ) );
		}

		if ( book is null ) {
			throw new NullException( nameof( book ) );
		}

		return this.Books.TryAdd( udc, book );
	}

	/// <summary>Serves as the default hash function.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override Int32 GetHashCode() => this.Books.GetHashCode();
}