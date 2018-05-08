// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/TaggedSentence.cs" was last cleaned by Protiguous on 2016/08/26 at 10:14 AM

namespace Librainian.Linguistics.PoS {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Collections;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [JsonObject]
    public sealed class TaggedSentence : IEquatable<TaggedSentence>, IEnumerable<ITaggedWord> {

        [JsonProperty]
        public readonly List<ITaggedWord> Tokens = new List<ITaggedWord>();

        public TaggedSentence( [NotNull] IEnumerable<ITaggedWord> words ) {
            if ( words is null ) {
                throw new ArgumentNullException( nameof( words ) );
            }
	        this.Tokens.AddRange( words.Where( word => null != word ).Select( word => word ) );
        }

        [Pure]
        public static implicit operator String( TaggedSentence sentence ) => sentence.Tokens.ToStrings( " " );

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter;
        ///     otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        [Pure]
        public Boolean Equals( TaggedSentence other ) {
            if ( other is null ) {
                return false;
            }

            return ReferenceEquals( this, other ) || this.Tokens.SequenceEqual( other.Tokens );
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        ///     through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ITaggedWord> GetEnumerator() => this.Tokens.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
        ///     through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [Pure]
        public override String ToString() => this.Tokens.ToStrings( " " );
    }
}
