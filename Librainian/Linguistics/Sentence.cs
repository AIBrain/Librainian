// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Sentence.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Sentence.cs" was last formatted by Protiguous on 2019/08/08 at 8:11 AM.

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Collections.Extensions;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;

    /// <summary>
    ///     A <see cref="Sentence" /> is an ordered sequence of <see cref="Word" /> .
    /// </summary>
    /// <see cref="http://wikipedia.org/wiki/Sentence_(linguistics)"></see>
    /// <see cref="Paragraph"></see>
    [JsonObject( MemberSerialization.Fields )]
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public sealed class Sentence : IEquatable<Sentence>, IEnumerable<Word>, IComparable<Sentence> {

        public Int32 CompareTo( [NotNull] Sentence other ) => String.Compare( this.ToString(), other.ToString(), StringComparison.Ordinal );

        public IEnumerator<Word> GetEnumerator() => this.Words.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Boolean Equals( [CanBeNull] Sentence other ) => Equals( this, other );

        /// <summary></summary>
        [NotNull]
        [JsonProperty]
        private Queue<Word> Words { get; } = new Queue<Word>();

        [NotNull]
        public static Sentence Empty { get; } = new Sentence();

        private Sentence() { }

        /// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
        /// <param name="sentence"></param>
        private Sentence( [NotNull] String sentence ) : this( sentence.ToWords().Select( word => new Word( word ) ) ) { }

        [NotNull]
        public static Sentence Parse( String sentence ) => new Sentence( sentence );

        /// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
        /// <param name="words"></param>
        public Sentence( [NotNull] IEnumerable<Word> words ) {
            if ( words == null ) {
                throw new ArgumentNullException( nameof( words ) );
            }

            foreach ( var word in words.Where( word => word != null ) ) {
                this.Words.Enqueue( word );
            }
        }

        public static Boolean Equals( [CanBeNull] Sentence left, [CanBeNull] Sentence right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return false;
            }

            return left.Words.SequenceEqual( right.Words );
        }

        public override Int32 GetHashCode() => this.Words.GetHashCode();

        public override String ToString() => this.Words.ToStrings( Symbols.Singlespace );

        //[NotNull]public IEnumerable<Sentence> Possibles() => this.Words.ToArray().FastPowerSet().Select( words => new Sentence( words ) ).Where( sentence => !sentence.ToString().IsNullOrEmpty() );
    }
}