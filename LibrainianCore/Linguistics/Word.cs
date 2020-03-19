// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Word.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "LibrainianCore", File: "Word.cs" was last formatted by Protiguous on 2020/03/16 at 3:06 PM.

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;

    /// <summary>One word. Case-sensitive <see cref="Equals(Word,Word)" /></summary>
    /// <see cref="Sentence"></see>
    [JsonObject]
    [Immutable]
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public class Word : IEquatable<Word>, IEnumerable<Char>, IComparable<Word> {

        [NotNull]
        [JsonProperty]
        private String value { get; }

        [NotNull]
        public static Word Empty { get; } = new Word( word: String.Empty );

        private Word() => this.value = Empty;

        public Word( [NotNull] String word ) {
            word = word.Trimmed();
            this.value = String.IsNullOrEmpty( value: word ) ? String.Empty : word;
        }

        public static Boolean Equals( [CanBeNull] Word left, [CanBeNull] Word right ) {
            if ( ReferenceEquals( objA: left, objB: right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            return left.value.Is( right: right.value );
        }

        [NotNull]
        public static implicit operator String( [NotNull] Word word ) => word.value;

        public Int32 CompareTo( [NotNull] Word other ) => String.Compare( strA: this.value, strB: other.value, comparisonType: StringComparison.Ordinal );

        public Boolean Equals( [CanBeNull] Word other ) => Equals( left: this, right: other );

        public IEnumerator<Char> GetEnumerator() => this.value.GetEnumerator();

        public override Int32 GetHashCode() => this.value.GetHashCode();

        [NotNull]
        public override String ToString() => this.value;

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        //[NotNull]public Char[][] Possibles() => this.Chars.ToArray().FastPowerSet();
    }
}