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
// "Librainian/Library.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Linguistics {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;

    [DataContract( IsReference = true )]
    public sealed class Library : IEquatable< Library >, IEnumerable< Book > {
        /// <summary>
        /// </summary>
        public const int Level = Book.Level << 1;

        [NotNull] [DataMember] private readonly List< Book > _tokens = new List< Book >();

        static Library() {
            Level.Should().BeGreaterThan( Book.Level );
        }

        public Library( [NotNull] Book book ) {
            this.Add( book );
        }

        public IEnumerator< Book > GetEnumerator() {
            return this._tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public Boolean Equals( [CanBeNull] Library other ) {
            if ( ReferenceEquals( other, null ) ) {
                return false;
            }
            return ReferenceEquals( this, other ) || this.SequenceEqual( other );
        }

        public Boolean Add( [NotNull] Book book ) {
            if ( book == null ) {
                throw new ArgumentNullException( "book" );
            }
            this._tokens.Add( book );
            return true;
        }
    }
}
