﻿// Copyright © Protiguous. All Rights Reserved.
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
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Parsing;

    public static class LinguisticExtensions {

        public static Grammar.SentenceType GuessSentenceType( this String sentence ) {

            //TODO Replace this with a real PoS-style tagger/engine ie Google lol

            if ( sentence.EndsWith( "!", StringComparison.CurrentCultureIgnoreCase ) ) {
                return Grammar.SentenceType.Exclamatory; //TODO Imperative
            }

            if ( sentence.EndsWith( "?", StringComparison.CurrentCultureIgnoreCase ) ) {
                return Grammar.SentenceType.Interrogative;
            }

            return Grammar.SentenceType.Declarative;
        }

        /// <summary>
        ///     <para>Remove duplicate words ONLY if the previous word was the same word.</para>
        /// </summary>
        /// <example>Example: "My cat cat likes likes to to to eat food." Should become "My cat likes to eat food."</example>
        /// <param name="s"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String RemoveDoubleWords( [CanBeNull] this String? s ) {
            if ( String.IsNullOrEmpty( s ) ) {
                return String.Empty;
            }

            var words = s.ToWords().ToList();
            var result = new List<String>( words.Count );

            String? previous = default;

            foreach ( var word in words ) {
                if ( !String.Equals( word, previous, StringComparison.Ordinal ) ) {
                    result.Add( word );
                }

                previous = word;
            }

            return result.ToStrings( ParsingConstants.Strings.Singlespace );
        }

    }

}