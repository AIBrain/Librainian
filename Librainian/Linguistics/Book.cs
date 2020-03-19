// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Book.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "Book.cs" was last formatted by Protiguous on 2020/03/18 at 10:31 AM.

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
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public class Book : IEquatable<Book>, IEnumerable<(Int32, Page)> {

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<(Int32, Page)> GetEnumerator() => this.GetPages().GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable ) this.Pages ).GetEnumerator();

        public Boolean Equals( [CanBeNull] Book other ) => Equals( left: this, right: other );

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
                throw new ArgumentNullException( paramName: nameof( pages ) );
            }

            var pageNumber = 0;

            foreach ( var page in pages.Where( predicate: page => page != null ) ) {
                this.Pages[ key: pageNumber++ ] = page;
            }

            if ( null != authors ) {
                this.Authors.AddRange( range: authors.Where( predicate: author => null != author ) );
            }
        }

        /// <summary>static equality test, compare sequence of Pages</summary>
        /// <param name="left"></param>
        /// <param name="right"> </param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Book left, [CanBeNull] Book right ) {
            if ( ReferenceEquals( objA: left, objB: right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            return left.SequenceEqual( second: right ); //no authors?? No authors.
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override Boolean Equals( Object obj ) => Equals( left: this, right: obj as Book );

        [NotNull]
        public IEnumerable<Author> GetAuthors() => this.Authors;

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Pages.GetHashCode();

        [NotNull]
        public IEnumerable<(Int32, Page)> GetPages() => this.Pages.Select( selector: pair => ( pair.Key, pair.Value ) );

    }

}