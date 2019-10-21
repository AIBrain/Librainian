// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Word.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Word.cs" was last formatted by Protiguous on 2019/08/08 at 8:12 AM.

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     One word. Case-sensitive <see cref="Equals(Word,Word)" />
    /// </summary>
    /// <see cref="Sentence"></see>
    [JsonObject]
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public class Word : IEquatable<Word>, IEnumerable<Char>, IComparable<Word> {

        public Int32 CompareTo( [NotNull] Word other ) => String.Compare( this.value, other.value, StringComparison.Ordinal );

        public IEnumerator<Char> GetEnumerator() => this.value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Boolean Equals( [CanBeNull] Word other ) => Equals( this, other );

        [NotNull]
        [JsonProperty]
        private String value { get; }

        [NotNull]
        public static Word Empty { get; } = new Word( String.Empty );

        private Word() => this.value = Empty;

        public Word( [NotNull] String word ) => this.value = String.IsNullOrEmpty( value: word ) ? String.Empty : word;

        public static Boolean Equals( [CanBeNull] Word left, [CanBeNull] Word right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return false;
            }

            return String.Equals( left.value, right.value, StringComparison.Ordinal );
        }

        [NotNull]
        public static implicit operator String( [NotNull] Word word ) => word.value;

        public override Int32 GetHashCode() => this.value.GetHashCode();

        public override String ToString() => this.value;

        //[NotNull]public Char[][] Possibles() => this.Chars.ToArray().FastPowerSet();
    }
}