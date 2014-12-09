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
// "Librainian/TaggedSentence.cs" was last cleaned by Rick on 2014/10/21 at 5:02 AM

namespace Librainian.Linguistics.PoS {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Annotations;
    using Collections;

    [DataContract( IsReference = true )]
    public sealed class TaggedSentence : IEquatable<TaggedSentence>, IEnumerable<ITaggedWord> {

        [DataMember]
        public readonly List<ITaggedWord> Tokens = new List<ITaggedWord>();

        public TaggedSentence( [NotNull] IEnumerable<ITaggedWord> words ) {
            if ( words == null ) {
                throw new ArgumentNullException( "words" );
            }
        }

        [Pure]
        public static implicit operator String( TaggedSentence sentence ) => sentence.Tokens.ToStrings( " " );

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter;
        /// otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        [Pure]
        public Boolean Equals( [NotNull] TaggedSentence other ) {
            if ( other == null ) {
                throw new ArgumentNullException( "other" );
            }
            return ReferenceEquals( this, other ) || this.Tokens.SequenceEqual( other.Tokens );
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        /// through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ITaggedWord> GetEnumerator() => this.Tokens.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
        /// through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [Pure]
        public override String ToString() => this.Tokens.ToStrings( " " );
    }
}