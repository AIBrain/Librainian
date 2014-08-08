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
// "Librainian2/Paragraph.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Linguistics {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using Annotations;
    using Collections;
    using FluentAssertions;
    using Parsing;

    /// <summary>
    ///     A <see cref="Paragraph" /> is a sequence of <see cref="Sentence" />.
    /// </summary>
    [DataContract( IsReference = true )]
    public sealed class Paragraph : IEquatable< Paragraph >, IEnumerable< Sentence > {
        public const int Level = Sentence.Level << 1;

        [NotNull] [DataMember] public readonly List< Sentence > Tokens = new List< Sentence >();

        static Paragraph() {
            Level.Should().BeGreaterThan( Sentence.Level );
        }

        /// <summary>
        ///     A <see cref="Paragraph" /> is ordered sequence of sentences.
        /// </summary>
        /// <param name="paragraph"></param>
        public Paragraph( [CanBeNull] String paragraph ) : this( paragraph.ToSentences() ) { }

        /// <summary>
        ///     A <see cref="Paragraph" /> is a collection of sentences.
        /// </summary>
        /// <param name="sentences"></param>
        public Paragraph( [CanBeNull] IEnumerable< Sentence > sentences ) {
            if ( sentences != null ) {
                this.Tokens.AddRange( sentences );
            }
            this.Tokens.Fix();
        }

        public IEnumerator< Sentence > GetEnumerator() {
            return this.Tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public Boolean Equals( [CanBeNull] Paragraph other ) {
            if ( ReferenceEquals( other, null ) ) {
                return false;
            }
            return ReferenceEquals( this, other ) || this.Tokens.SequenceEqual( other.Tokens );
        }

        public IEnumerable< Tuple< int, String > > Possibles() {
            foreach ( var sentence in this ) {
                yield return new Tuple< int, string >( Level, sentence );

                foreach ( var possible in sentence.Possibles() ) {
                    yield return possible;
                }
            }
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach ( var sentence in this.Tokens ) {
                sb.AppendLine( sentence );
            }
            return sb.ToString();
        }

        public static implicit operator String( Paragraph paragraph ) {
            return paragraph.ToString();
        }
    }
}
