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
// "Librainian/Sentence.cs" was last cleaned by Rick on 2016/08/26 at 10:14 AM

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

        /// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
        /// <param name="sentence"></param>
        public Sentence( [NotNull] String sentence ) : this( sentence.ToWords().Select( word => new Word( word ) ) ) { }

        /// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
        /// <param name="words"></param>
        public Sentence( [NotNull] IEnumerable<Word> words ) {
            if ( words is null ) {
                throw new ArgumentNullException( nameof( words ) );
            }

            this.Words.AddRange( words.Where( word => word != null ) );
            this.Words.Fix();
        }

        private Sentence() {
        }

        public static Sentence Empty { get; } = new Sentence();

        /// <summary></summary>
        [NotNull]
        [JsonProperty]
        private List<Word> Words { get; } = new List<Word>();

        //public static implicit operator String( Sentence sentence ) {return sentence != null ? sentence.Words.ToStrings( " " ) : String.Empty;}

        public Int32 CompareTo( Sentence other ) => String.Compare( this.ToString(), other.ToString(), StringComparison.Ordinal );

	    public Boolean Equals( Sentence other ) {
            if ( other is null ) {
                return false;
            }

            return ReferenceEquals( this, other ) || this.SequenceEqual( other );
        }

        public IEnumerator<Word> GetEnumerator() => this.Words.GetEnumerator();

        public override Int32 GetHashCode() => this.Words.GetHashCode();

	    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public IEnumerable<Sentence> Possibles() => this.Words.ToArray().FastPowerSet().Select( words => new Sentence( words ) ).Where( sentence => !sentence.ToString().IsNullOrEmpty() );

		//[NotNull]
		//public Word TakeFirst() {
		//    try {
		//        return this.Words.TakeFirst() ?? new Word( String.Empty );
		//    }
		//    finally {
		//        this.Words.Fix();
		//    }
		//}

		public override String ToString() => this.Words.ToStrings( " " );
    }
}
