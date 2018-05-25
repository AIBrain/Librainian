// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Sentence.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Sentence.cs" was last formatted by Protiguous on 2018/05/24 at 7:19 PM.

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Collections;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;

    /// <summary>
    ///     A <see cref="Sentence" /> is an ordered sequence of <see cref="Word" /> .
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Sentence_(linguistics)"></seealso>
    /// <seealso cref="Paragraph"></seealso>
    [JsonObject]
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public sealed class Sentence : IEquatable<Sentence>, IEnumerable<Word>, IComparable<Sentence> {

        /// <summary></summary>
        public static readonly Sentence EndOfLine = new Sentence( "\0" );

        /// <summary></summary>
        [NotNull]
        [JsonProperty]
        private List<Word> Words { get; } = new List<Word>();

        public static Sentence Empty { get; } = new Sentence();

        private Sentence() { }

        /// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
        /// <param name="sentence"></param>
        public Sentence( [NotNull] String sentence ) : this( sentence.ToWords().Select( word => new Word( word ) ) ) { }

        /// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
        /// <param name="words"></param>
        public Sentence( [NotNull] IEnumerable<Word> words ) {
            if ( words is null ) { throw new ArgumentNullException( nameof( words ) ); }

            this.Words.AddRange( words.Where( word => word != null ) );
            this.Words.Fix();
        }

        //public static implicit operator String( Sentence sentence ) {return sentence != null ? sentence.Words.ToStrings( " " ) : String.Empty;}

        public Int32 CompareTo( Sentence other ) => String.Compare( this.ToString(), other.ToString(), StringComparison.Ordinal );

        public Boolean Equals( Sentence other ) {
            if ( other is null ) { return false; }

            return ReferenceEquals( this, other ) || this.SequenceEqual( other );
        }

        public IEnumerator<Word> GetEnumerator() => this.Words.GetEnumerator();

        public override Int32 GetHashCode() => this.Words.GetHashCode();

        public IEnumerable<Sentence> Possibles() => this.Words.ToArray().FastPowerSet().Select( words => new Sentence( words ) ).Where( sentence => !sentence.ToString().IsNullOrEmpty() );

        public override String ToString() => this.Words.ToStrings( " " );

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        //[NotNull]
        //public Word TakeFirst() {
        //    try {
        //        return this.Words.TakeFirst() ?? new Word( String.Empty );
        //    }
        //    finally {
        //        this.Words.Fix();
        //    }
        //}
    }
}