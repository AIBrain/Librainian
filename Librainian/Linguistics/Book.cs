// Copyright � Protiguous. All Rights Reserved.
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
// File "Book.cs" last touched on 2021-03-07 at 3:59 PM by Protiguous.

namespace Librainian.Linguistics {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Exceptions;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>A <see cref="Book" /> is a sequence of <see cref="Page" /> .</para>
	/// </summary>
	[JsonObject]
	[Immutable]
	[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
	[Serializable]
	public record Book : IEnumerable<(Int32 pageid, Page page)>, IHasAuthors {
		private Book() { }

		public Book( IEnumerable<Page> pages, IEnumerable<Author>? authors = null ) {
			if ( pages is null ) {
				throw new ArgumentEmptyException( nameof( pages ) );
			}

			var pageNumber = 0;

			foreach ( var page in pages ) {
				this.Pages[ pageNumber++ ] = page;
			}

			if ( null != authors ) {
				this.Authors.AddRange( authors );
			}
		}

		[JsonProperty]
		public HashSet<Author> Authors { get; } = new();

		[JsonProperty]
		private Dictionary<Int32, Page> Pages { get; } = new();

		public static Book Empty { get; } = new();

		/// <summary>
		///     Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<(Int32, Page)> GetEnumerator() => this.GetPages().GetEnumerator();

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable )this.Pages ).GetEnumerator();

		/// <summary>
		///     static equality test, compare sequence of Pages
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Book? left, Book? right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return false;
			}

			return left.SequenceEqual( right ); //no authors?? No authors.
		}

		public IEnumerable<Author> GetAuthors() => this.Authors;

		public IEnumerable<(Int32, Page)> GetPages() => this.Pages.Select( pair => (pair.Key, pair.Value) );
	}
}