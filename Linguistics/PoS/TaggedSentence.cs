// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "TaggedSentence.cs",
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
// "Librainian/Librainian/TaggedSentence.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

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
            if ( words is null ) { throw new ArgumentNullException( nameof( words ) ); }

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
            if ( other is null ) { return false; }

            return ReferenceEquals( this, other ) || this.Tokens.SequenceEqual( other.Tokens );
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        ///     through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ITaggedWord> GetEnumerator() => this.Tokens.GetEnumerator();

        [Pure]
        public override String ToString() => this.Tokens.ToStrings( " " );

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
        ///     through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}