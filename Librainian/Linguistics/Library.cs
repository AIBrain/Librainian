// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Library.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "Library.cs" was last formatted by Protiguous on 2018/06/26 at 1:15 AM.

namespace Librainian.Linguistics {

	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>A <see cref="Library" /> is a cluster of <see cref="Book" /> s.</para>
	/// </summary>
	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
	[Serializable]
	public sealed class Library : IEquatable<Library>, IEnumerable<KeyValuePair<UDC, Book>> {

		[NotNull]
		[JsonProperty]
		private ConcurrentDictionary<UDC, Book> Books { get; } = new ConcurrentDictionary<UDC, Book>();

		public Library( [NotNull] UDC udc, [NotNull] Book book ) => this.Add( udc, book );

		/// <summary>
		///     Static equality test
		/// </summary>
		/// <param name="left"></param>
		/// <param name="rhs"> </param>
		/// <returns></returns>
		public static Boolean Equals( Library left, Library rhs ) {
			if ( ReferenceEquals( left, rhs ) ) {
				return true;
			}

			if ( left is null ) {
				return false;
			}

			if ( rhs is null ) {
				return false;
			}

			return left.OrderBy( pair => pair.Key ).SequenceEqual( rhs.OrderBy( pair => pair.Key ) );
		}

		public Boolean Add( [NotNull] UDC udc, [NotNull] Book book ) {
			if ( udc is null ) {
				throw new ArgumentNullException( nameof( udc ) );
			}

			if ( book is null ) {
				throw new ArgumentNullException( nameof( book ) );
			}

			this.Books.TryAdd( udc, book );

			return true;
		}

		public Boolean Equals( [CanBeNull] Library other ) => Equals( this, other );

		/// <summary>
		///     Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<KeyValuePair<UDC, Book>> GetEnumerator() => this.Books.GetEnumerator();

		/// <summary>
		///     Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override Int32 GetHashCode() => this.Books.GetHashCode();

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable ) this.Books ).GetEnumerator();
	}
}