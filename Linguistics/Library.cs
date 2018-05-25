// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Library.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Library.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

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
            if ( ReferenceEquals( left, rhs ) ) { return true; }

            if ( left is null ) { return false; }

            if ( rhs is null ) { return false; }

            return left.OrderBy( pair => pair.Key ).SequenceEqual( rhs.OrderBy( pair => pair.Key ) );
        }

        public Boolean Add( [NotNull] UDC udc, [NotNull] Book book ) {
            if ( udc is null ) { throw new ArgumentNullException( nameof( udc ) ); }

            if ( book is null ) { throw new ArgumentNullException( nameof( book ) ); }

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
        IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable )this.Books ).GetEnumerator();
    }
}