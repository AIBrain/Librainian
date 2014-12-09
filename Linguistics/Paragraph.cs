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
// "Librainian/Paragraph.cs" was last cleaned by Rick on 2014/10/21 at 5:02 AM

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using Annotations;
    using Collections;
    using Extensions;
    using FluentAssertions;
    using Parsing;

    /// <summary>
    /// <para>A <see cref="Paragraph" /> is a sequence of <see cref="Sentence" /> .</para>
    /// </summary>
    /// <seealso cref="Page"></seealso>
    [DataContract(IsReference = true)]
    [Immutable]
    public sealed class Paragraph : IEquatable<Paragraph>, IEnumerable<Sentence> {
        public const UInt64 Level = Sentence.Level << 1;

        [NotNull]
        [DataMember]
        private readonly List<Sentence> _tokens = new List<Sentence>();

        static Paragraph() {
            Level.Should().BeGreaterThan( Sentence.Level );
        }

        /// <summary>
        /// A <see cref="Paragraph" /> is ordered sequence of sentences.
        /// </summary>
        /// <param name="paragraph"></param>
        public Paragraph( [CanBeNull] String paragraph ) : this( paragraph.ToSentences() ) {
        }

        /// <summary>
        /// A <see cref="Paragraph" /> is a collection of sentences.
        /// </summary>
        /// <param name="sentences"></param>
        public Paragraph( [CanBeNull] IEnumerable<Sentence> sentences ) {
            if ( sentences != null ) {
                this._tokens.AddRange( sentences );
            }
            this._tokens.Fix();
        }

        public static implicit operator String( Paragraph paragraph ) => paragraph.ToString();

        public Boolean Equals( [CanBeNull] Paragraph other ) {
            if ( ReferenceEquals( other, null ) ) {
                return false;
            }
            return ReferenceEquals( this, other ) || this._tokens.SequenceEqual( other._tokens );
        }

        public IEnumerator<Sentence> GetEnumerator() => this._tokens.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /*
                public IEnumerable<Tuple<UInt64, String>> Possibles() {
                    foreach ( var sentence in this ) {
                        yield return new Tuple<UInt64, String>( Level, sentence );

                        foreach ( var possible in sentence.Possibles() ) {
                            yield return possible;
                        }
                    }
                }
        */

        public override String ToString() {
            var sb = new StringBuilder();
            foreach ( var sentence in this._tokens ) {
                sb.AppendLine( sentence );
            }
            return sb.ToString();
        }
    }
}