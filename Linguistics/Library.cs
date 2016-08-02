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
// "Librainian/Library.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>A <see cref="Library" /> is a cluster of <see cref="Book" /> .</para>
    /// </summary>
    [JsonObject]
    public sealed class Library : IEquatable<Library>, IEnumerable<Book> {

        /// <summary></summary>
        public const UInt64 Level = Book.Level << 1;

        [NotNull]
        [JsonProperty]
        private readonly ConcurrentDictionary<Udc, Book> _tokens = new ConcurrentDictionary<Udc, Book>();

        static Library() {
            Level.Should().BeGreaterThan( Book.Level );
        }

        public Library( [NotNull] Udc udc, [NotNull] Book book ) {
            this.Add( udc, book );
        }

        public Boolean Add( [NotNull] Udc udc, [NotNull] Book book ) {
            if ( book == null ) {
                throw new ArgumentNullException( nameof( book ) );
            }
            this._tokens.TryAdd( udc, book );
            return true;
        }

        public Boolean Equals( [CanBeNull] Library other ) {
            if ( ReferenceEquals( other, null ) ) {
                return false;
            }
            return ReferenceEquals( this, other ) || this.SequenceEqual( other );
        }

        public IEnumerator<Book> GetEnumerator() => this._tokens.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}