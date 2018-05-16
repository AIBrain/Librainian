// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Page.cs",
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
// "Librainian/Librainian/Page.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>A <see cref="Page" /> is a sequence of <see cref="Paragraph" /> .</para>
    /// </summary>
    /// <seealso cref="Book"></seealso>
    [JsonObject]
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public sealed class Page : IEquatable<Page>, IEnumerable<Paragraph> {

        private Page() { }

        public Page( [NotNull] IEnumerable<Paragraph> paragraphs ) {
            if ( paragraphs is null ) { throw new ArgumentNullException( nameof( paragraphs ) ); }

            this.Paragraphs.AddRange( paragraphs.Where( paragraph => paragraph != null ) );
        }

        [NotNull]
        [JsonProperty]
        private List<Paragraph> Paragraphs { get; } = new List<Paragraph>();

        public static Page Empty { get; } = new Page();

        public Boolean Equals( [CanBeNull] Page other ) {
            if ( other is null ) { return false; }

            return ReferenceEquals( this, other ) || this.Paragraphs.SequenceEqual( other.Paragraphs );
        }

        public IEnumerator<Paragraph> GetEnumerator() => this.Paragraphs.GetEnumerator();

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Paragraphs.GetHashCode();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}