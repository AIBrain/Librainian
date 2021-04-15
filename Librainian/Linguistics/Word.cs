// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;

    /// <summary>One word. Should be case-sensitive? <see cref="Equals(Word,Word)" /></summary>
    /// <see cref="Sentence"></see>
    [JsonObject]
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public record Word : IEnumerable<Char>, IComparable<Word> {

        private Word() : this( String.Empty ) { }

        public Word( [NotNull] String word ) => this.Value = word.Trimmed() ?? String.Empty;

        [NotNull]
        [JsonProperty]
        private String Value { get; init; }

        [NotNull]
        public static Word Empty { get; } = new();

        public Int32 CompareTo( [NotNull] Word? other ) => String.Compare( this.Value, other?.Value, StringComparison.Ordinal );

        public IEnumerator<Char> GetEnumerator() => this.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public static Boolean Equals( Word? left, Word? right ) => String.Equals( left?.Value, right?.Value, StringComparison.Ordinal );

        //public virtual Boolean Equals( Word? other ) => String.Equals( this.Value, other?.Value, StringComparison.Ordinal );

        [NotNull]
        public static implicit operator String( [NotNull] Word word ) => word.Value;

        [NotNull]
        public override String ToString() => this.Value;

        //[NotNull]public Char[][] Possibles() => this.Chars.ToArray().FastPowerSet();

        public override Int32 GetHashCode() => this.Value.GetHashCode();

    }

}