#region License & Information

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
// "Librainian/Word.cs" was last cleaned by Rick on 2015/06/12 at 2:59 PM
#endregion License & Information

namespace Librainian.Linguistics {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Collections;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;

    /// <summary>A <see cref="Word" /> is a sequence of <see cref="Character" /> . <seealso cref="http://wikipedia.org/wiki/Truthbearer" /></summary>
    /// <seealso cref="Sentence"></seealso>
    [DataContract( IsReference = true )]
    [Immutable]
    public class Word : IEquatable<Word>, IEnumerable<Character> {
        public const UInt64 Level = Character.Level << 1;

        [NotNull]
        [DataMember]
        private readonly List<Character> _tokens = new List<Character>();

        static Word() {
            Level.Should().BeGreaterThan( Character.Level );
        }

        public Word([CanBeNull] String word) {
            if ( String.IsNullOrEmpty( word ) ) {
                word = String.Empty;
            }
            this._tokens.AddRange( word.Select( character => new Character( character ) ) );
            this._tokens.Fix();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        /// through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Character> GetEnumerator() => this._tokens.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
        /// through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter;
        /// otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Boolean Equals([CanBeNull] Word other) {
            if ( ReferenceEquals( other, null ) ) {
                return false;
            }
            return ReferenceEquals( this, other ) || this.SequenceEqual( other );
        }

        public static implicit operator String (Word word) => word._tokens.ToStrings( "" );

        public IEnumerable<Tuple<UInt64, String>> Possibles() {
            if ( !this._tokens.Any() ) {
                yield break;
            }

            foreach ( var character in this._tokens ) {
                yield return new Tuple<UInt64, String>( Level, character.ToString() );
            }
        }

        public override String ToString() => this._tokens.ToStrings( "" );
    }
}