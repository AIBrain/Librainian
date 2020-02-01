// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Paragraph.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "Paragraph.cs" was last formatted by Protiguous on 2020/01/31 at 12:31 AM.

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;

    /// <summary>
    ///     <para>A <see cref="Paragraph" /> is a sequence of <see cref="Sentence" /> .</para>
    /// </summary>
    /// <see cref="Page"></see>
    [JsonObject]
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public sealed class Paragraph : IEquatable<Paragraph>, IEnumerable<Sentence> {

        public IEnumerator<Sentence> GetEnumerator() => this.Sentences.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Boolean Equals( [CanBeNull] Paragraph other ) {
            if ( other is null ) {
                return default;
            }

            return ReferenceEquals( this, other ) || this.Sentences.SequenceEqual( other.Sentences );
        }

        [NotNull]
        [JsonProperty]
        private List<Sentence> Sentences { get; } = new List<Sentence>();

        public static Paragraph Empty { get; } = new Paragraph();

        private Paragraph() { }

        /// <summary>A <see cref="Paragraph" /> is ordered sequence of sentences.</summary>
        /// <param name="paragraph"></param>
        public Paragraph( [CanBeNull] String paragraph ) : this( paragraph.ToSentences() ) { }

        /// <summary>A <see cref="Paragraph" /> is a collection of sentences.</summary>
        /// <param name="sentences"></param>
        public Paragraph( [CanBeNull] IEnumerable<Sentence> sentences ) {
            if ( sentences != null ) {
                this.Sentences.AddRange( sentences.Where( sentence => sentence != null ) );
            }

            this.Sentences.TrimExcess();
        }

        [NotNull]
        public static implicit operator String( [NotNull] Paragraph paragraph ) => paragraph.ToString();

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Sentences.GetHashCode();

        public override String ToString() {
            var sb = new StringBuilder();

            foreach ( var sentence in this.Sentences ) {
                sb.AppendLine( sentence.ToString() );
            }

            return sb.ToString();
        }

    }

}