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
// File "Chapter.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Linguistics;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Exceptions;
using Newtonsoft.Json;

/// <summary>
/// <para>A <see cref="Page" /> is a sequence of <see cref="Paragraph" /> .</para>
/// </summary>
/// <see cref="Book"></see>
[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
[Serializable]
public record Chapter : IEnumerable<Page>, IHasAuthors {
	private Chapter() { }

	public Chapter( IEnumerable<Page> pages ) {
		if ( pages is null ) {
			throw new NullException( nameof( pages ) );
		}

		this.Add( pages );
	}

	[JsonProperty]
	private List<Page> Pages { get; } = new();

	public static Chapter Empty { get; } = new();

	public IEnumerator<Page> GetEnumerator() => this.Pages.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

	public HashSet<Author> Authors { get; } = new();

	public static Boolean Equals( Chapter? left, Chapter? right ) =>
		left is not null && right is not null && ( ReferenceEquals( left, right ) || left.Pages.SequenceEqual( right.Pages ) );

	public void Add( Page page ) => this.Pages.Add( page );

	public void Add( IEnumerable<Page> pages ) => this.Pages.AddRange( pages );
}