// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Paragraph.cs",
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
// "Librainian/Librainian/Paragraph.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

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
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public sealed class Paragraph : IEquatable<Paragraph>, IEnumerable<Sentence> {

        [NotNull]
        [JsonProperty]
        private readonly List<Sentence> Sentences = new List<Sentence>();

        private Paragraph() { }

        /// <summary>A <see cref="Paragraph" /> is ordered sequence of sentences.</summary>
        /// <param name="paragraph"></param>
        public Paragraph( [CanBeNull] String paragraph ) : this( paragraph.ToSentences() ) { }

        /// <summary>A <see cref="Paragraph" /> is a collection of sentences.</summary>
        /// <param name="sentences"></param>
        public Paragraph( [CanBeNull] IEnumerable<Sentence> sentences ) {
            if ( sentences != null ) { this.Sentences.AddRange( sentences.Where( sentence => sentence != null ) ); }

            this.Sentences.Fix();
        }

        public static Paragraph Empty { get; } = new Paragraph();

        public static implicit operator String( Paragraph paragraph ) => paragraph.ToString();

        public Boolean Equals( [CanBeNull] Paragraph other ) {
            if ( other is null ) { return false; }

            return ReferenceEquals( this, other ) || this.Sentences.SequenceEqual( other.Sentences );
        }

        public IEnumerator<Sentence> GetEnumerator() => this.Sentences.GetEnumerator();

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Sentences.GetHashCode();

        public override String ToString() {
            var sb = new StringBuilder();

            foreach ( var sentence in this.Sentences ) { sb.AppendLine( sentence.ToString() ); }

            return sb.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}