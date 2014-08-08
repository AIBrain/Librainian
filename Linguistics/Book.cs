#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/Book.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Linguistics {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using Knowledge;

    [DataContract( IsReference = true )]
    public sealed class Book : IEquatable< Book >, IEnumerable< Page > {
        public const int Level = Page.Level << 1;

        [NotNull] [DataMember] private readonly List< Author > _authors = new List< Author >();
        [NotNull] [DataMember] private readonly List< Page > _tokens = new List< Page >();

        static Book() {
            Level.Should().BeGreaterThan( Page.Level );
        }

        public Book( [NotNull] String text ) {
            if ( text == null ) {
                throw new ArgumentNullException( "text" );
            }
            this.Add( text );
        }

        public IEnumerator< Page > GetEnumerator() {
            return this._tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public Boolean Equals( [CanBeNull] Book other ) {
            if ( ReferenceEquals( other, null ) ) {
                return false;
            }
            return ReferenceEquals( this, other ) || this.SequenceEqual( other );
        }

        public Boolean Add( [NotNull] String text ) {
            if ( text == null ) {
                throw new ArgumentNullException( "text" );
            }
            this._tokens.Add( new Page( text ) ); //TODO //BUG this needs to add all pages
            return true;
        }
    }

    public class Author : Person { }
}
