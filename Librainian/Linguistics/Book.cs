// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Book.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Book.cs" was last formatted by Protiguous on 2019/08/08 at 8:08 AM.

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>A <see cref="Book" /> is a sequence of <see cref="Page" /> .</para>
    /// </summary>
    [JsonObject]
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public sealed class Book : IEquatable<Book>, IEnumerable<KeyValuePair<Int32, Page>> {

        [NotNull]
        [JsonProperty]
        private HashSet<Author> Authors { get; } = new HashSet<Author>();

        [NotNull]
        [JsonProperty]
        private Dictionary<Int32, Page> Pages { get; } = new Dictionary<Int32, Page>();

        public static Book Empty { get; } = new Book();

        private Book() { }

        public Book( [ItemCanBeNull] [NotNull] IEnumerable<Page> pages, [ItemCanBeNull] [CanBeNull] IEnumerable<Author> authors = null ) {
            if ( pages is null ) {
                throw new ArgumentNullException( nameof( pages ) );
            }

            var pageNumber = 0;

            foreach ( var page in pages.Where( page => page != null ) ) {
                pageNumber++;
                this.Pages[ pageNumber ] = page;
            }

            if ( null != authors ) {
                this.Authors.AddRange( authors.Where( author => null != author ) );
            }
        }

        /// <summary>
        ///     static equality test, compare sequence of Books
        /// </summary>
        /// <param name="left"></param>
        /// <param name="rhs"> </param>
        /// <returns></returns>
        public static Boolean Equals( Book left, Book rhs ) {
            if ( ReferenceEquals( left, rhs ) ) {
                return true;
            }

            if ( left is null ) {
                return false;
            }

            if ( rhs is null ) {
                return false;
            }

            return left.SequenceEqual( rhs ); //no authors??
        }

        public Boolean Equals( [CanBeNull] Book other ) => Equals( this, other );

        [NotNull]
        public IEnumerable<Author> GetAuthors() => this.Authors;

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<Int32, Page>> GetEnumerator() => this.Pages.GetEnumerator();

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Pages.GetHashCode();

        [NotNull]
        public IEnumerable<KeyValuePair<Int32, Page>> GetPages() => this.Pages;

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable )this.Pages ).GetEnumerator();
    }
}