// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Library.cs" was last cleaned by Protiguous on 2016/08/26 at 10:19 AM

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
    /// <para>A <see cref="Library"/> is a cluster of <see cref="Book"/> s.</para>
    /// </summary>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public sealed class Library : IEquatable<Library>, IEnumerable<KeyValuePair<UDC, Book>> {

        public Library( [NotNull] UDC udc, [NotNull] Book book ) => this.Add( udc, book );

        [NotNull]
        [JsonProperty]
        private ConcurrentDictionary<UDC, Book> Books { get; } = new ConcurrentDictionary<UDC, Book>();

        /// <summary>
        /// Static equality test
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
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<UDC, Book>> GetEnumerator() => this.Books.GetEnumerator();

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Books.GetHashCode();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable )this.Books ).GetEnumerator();
    }
}