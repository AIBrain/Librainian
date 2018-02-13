// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Book.cs" was last cleaned by Rick on 2016/08/26 at 10:14 AM

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
    [DebuggerDisplay( "{ToString()}" )]
    [Serializable]
    public sealed class Book : IEquatable<Book>, IEnumerable<KeyValuePair<Int32, Page>> {

        public Book( [ItemCanBeNull] [NotNull] IEnumerable<Page> pages, [ItemCanBeNull] [CanBeNull] IEnumerable<Author> authors = null ) {
            if ( pages == null ) {
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

        private Book() {
        }

        public static Book Empty { get; } = new Book();

        [NotNull]
        [JsonProperty]
        private HashSet<Author> Authors { get; } = new HashSet<Author>();

        [NotNull]
        [JsonProperty]
        private Dictionary<Int32, Page> Pages { get; } = new Dictionary<Int32, Page>();

        /// <summary>
        ///     static equality test, compare sequence of Books
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean Equals( Book lhs, Book rhs ) {
            if ( ReferenceEquals( lhs, rhs ) ) {
                return true;
            }
            if ( ReferenceEquals( lhs, null ) ) {
                return false;
            }
            if ( ReferenceEquals( null, rhs ) ) {
                return false;
            }

            return lhs.SequenceEqual( rhs ); //no authors??
        }

        public Boolean Equals( [CanBeNull] Book other ) {
            return Equals( this, other );
        }

        public IEnumerable<Author> GetAuthors() => this.Authors;

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<Int32, Page>> GetEnumerator() {
            return this.Pages.GetEnumerator();
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() {
            return this.Pages.GetHashCode();
        }

        public IEnumerable<KeyValuePair<Int32, Page>> GetPages() {
            return this.Pages;
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return ( ( IEnumerable )this.Pages ).GetEnumerator();
        }
    }
}
