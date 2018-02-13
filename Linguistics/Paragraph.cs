// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Paragraph.cs" was last cleaned by Rick on 2016/08/26 at 10:14 AM

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Collections;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;

    /// <summary>
    ///     <para>A <see cref="Paragraph" /> is a sequence of <see cref="Sentence" /> .</para>
    /// </summary>
    /// <seealso cref="Page"></seealso>
    [JsonObject]
    [Immutable]
    [DebuggerDisplay( "{ToString()}" )]
    [Serializable]
    public sealed class Paragraph : IEquatable<Paragraph>, IEnumerable<Sentence> {

        [NotNull]
        [JsonProperty]
        private readonly List<Sentence> Sentences = new List<Sentence>();

        /// <summary>A <see cref="Paragraph" /> is ordered sequence of sentences.</summary>
        /// <param name="paragraph"></param>
        public Paragraph( [CanBeNull] String paragraph ) : this( paragraph.ToSentences() ) { }

        /// <summary>A <see cref="Paragraph" /> is a collection of sentences.</summary>
        /// <param name="sentences"></param>
        public Paragraph( [CanBeNull] IEnumerable<Sentence> sentences ) {
            if ( sentences != null ) {
                this.Sentences.AddRange( sentences.Where( sentence => sentence != null ) );
            }
            this.Sentences.Fix();
        }

        private Paragraph() {
        }

        public static Paragraph Empty { get; } = new Paragraph();

        public static implicit operator String( Paragraph paragraph ) => paragraph.ToString();

        public Boolean Equals( [CanBeNull] Paragraph other ) {
            if ( ReferenceEquals( other, null ) ) {
                return false;
            }

            return ReferenceEquals( this, other ) || this.Sentences.SequenceEqual( other.Sentences );
        }

        public IEnumerator<Sentence> GetEnumerator() => this.Sentences.GetEnumerator();

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() {
            return this.Sentences.GetHashCode();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public override String ToString() {
            var sb = new StringBuilder();
            foreach ( var sentence in this.Sentences ) {
                sb.AppendLine( sentence.ToString() );
            }

            return sb.ToString();
        }
    }
}
