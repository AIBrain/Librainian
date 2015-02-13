// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Library.cs" was last cleaned by Rick on 2014/10/21 at 5:02 AM

namespace Librainian.Linguistics {
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using JetBrains.Annotations;

    /// <summary>
    /// <para>A <see cref="Library" /> is a cluster of <see cref="Book" /> .</para>
    /// </summary>
    [DataContract( IsReference = true )]
    public sealed class Library : IEquatable<Library>, IEnumerable<Book> {

        /// <summary>
        /// </summary>
        public const UInt64 Level = Book.Level << 1;

        [NotNull]
        [DataMember]
        private readonly ConcurrentDictionary<UDC, Book> _tokens = new ConcurrentDictionary<UDC, Book>();

        static Library() {
            Level.Should().BeGreaterThan( Book.Level );
        }

        public Library( [NotNull] UDC udc, [NotNull] Book book ) {
            this.Add( udc, book );
        }

        public Boolean Add( [NotNull] UDC udc, [NotNull] Book book ) {
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