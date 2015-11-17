// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Page.cs" was last cleaned by Rick on 2015/06/12 at 2:59 PM

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;

    /// <summary>
    /// <para>A <see cref="Page" /> is a sequence of <see cref="Paragraph" /> .</para></summary>
    /// <seealso cref="Book"></seealso>
    [Immutable]
    [DataContract( IsReference = true )]
    public sealed class Page : IEquatable<Page>, IEnumerable<Paragraph> {
        public const UInt64 Level = Paragraph.Level << 1;

        [NotNull]
        [DataMember]
        private readonly List<Paragraph> _tokens = new List<Paragraph>();

        static Page() {
            Level.Should().BeGreaterThan( Paragraph.Level );
        }

        public Page([NotNull] String text) {
            if ( text == null ) {
                throw new ArgumentNullException( nameof( text ) );
            }
            this.Add( text );
        }

        public Boolean Equals([CanBeNull] Page other) {
            if ( ReferenceEquals( other, null ) ) {
                return false;
            }
            return ReferenceEquals( this, other ) || this._tokens.SequenceEqual( other._tokens );
        }

        public IEnumerator<Paragraph> GetEnumerator() => this._tokens.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Boolean Add([NotNull] String text) {
            if ( text == null ) {
                throw new ArgumentNullException( nameof( text ) );
            }
            this._tokens.Add( new Paragraph( text ) ); //TODO //BUG this needs to add all paragraphs
            return true;
        }
    }
}