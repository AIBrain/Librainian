// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TaggedSentence.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "TaggedSentence.cs" was last formatted by Protiguous on 2020/01/31 at 12:31 AM.

namespace LibrainianCore.Linguistics.PoS {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Collections.Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [JsonObject]
    public sealed class TaggedSentence : IEquatable<TaggedSentence>, IEnumerable<ITaggedWord> {

        [JsonProperty]
        [NotNull]
        public List<ITaggedWord> Tokens { get; } = new List<ITaggedWord>();

        public TaggedSentence( [NotNull] IEnumerable<ITaggedWord> words ) {
            if ( words is null ) {
                throw new ArgumentNullException( nameof( words ) );
            }

            this.Tokens.AddRange( words.Where( word => null != word ).Select( word => word ) );
        }

        [Pure]
        [NotNull]
        public static implicit operator String( [NotNull] TaggedSentence sentence ) => sentence.Tokens.ToStrings( " " );

        /// <summary>Returns a value that indicates whether two <see cref="TaggedSentence" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static Boolean operator !=( [CanBeNull] TaggedSentence left, [CanBeNull] TaggedSentence right ) => !Equals( left, right );

        /// <summary>Returns a value that indicates whether the values of two <see cref="TaggedSentence" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static Boolean operator ==( [CanBeNull] TaggedSentence left, [CanBeNull] TaggedSentence right ) => Equals( left, right );

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        [Pure]
        public Boolean Equals( TaggedSentence other ) {
            if ( other is null ) {
                return default;
            }

            return ReferenceEquals( this, other ) || this.Tokens.SequenceEqual( other.Tokens );
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override Boolean Equals( Object obj ) => Equals( this, obj as TaggedSentence );

        public IEnumerator<ITaggedWord> GetEnumerator() => this.Tokens.GetEnumerator();

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Tokens.GetHashCode();

        [Pure]
        [NotNull]
        public override String ToString() => this.Tokens.ToStrings( " " );

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}