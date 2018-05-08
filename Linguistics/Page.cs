// Copyright 2018 Protiguous.
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
// "Librainian/Page.cs" was last cleaned by Protiguous on 2016/08/26 at 10:14 AM

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

        public Page( [NotNull] IEnumerable<Paragraph> paragraphs ) {
            if ( paragraphs is null ) {
                throw new ArgumentNullException( nameof( paragraphs ) );
            }

            this.Paragraphs.AddRange( paragraphs.Where( paragraph => paragraph != null ) );
        }

        private Page() {
        }

        public static Page Empty { get; } = new Page();

        [NotNull]
        [JsonProperty]
        private List<Paragraph> Paragraphs { get; } = new List<Paragraph>();

        public Boolean Equals( [CanBeNull] Page other ) {
            if ( other is null ) {
                return false;
            }

            return ReferenceEquals( this, other ) || this.Paragraphs.SequenceEqual( other.Paragraphs );
        }

        public IEnumerator<Paragraph> GetEnumerator() => this.Paragraphs.GetEnumerator();

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Paragraphs.GetHashCode();

	    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
