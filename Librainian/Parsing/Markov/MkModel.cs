// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "MkModel.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "MkModel.cs" was last formatted by Protiguous on 2020/01/31 at 12:28 AM.

namespace Librainian.Parsing.Markov {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Maths;

    public class MkModel {

        public readonly String Name;

        [NotNull]
        private ConcurrentDictionary<String, List<String>> _markovChains { get; } = new ConcurrentDictionary<String, List<String>>();

        private MkModel() => throw new NotImplementedException();

        public MkModel( [NotNull] String name ) {
            if ( String.IsNullOrWhiteSpace( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( name ) );
            }

            this.Name = name;
        }

        [NotNull]
        public String GenerateRandomCorpus( Int32 numberOfWords ) {
            if ( !this._markovChains.Any() ) {
                return String.Empty;
            }

            var word = this._markovChains.OrderBy( o => Randem.Next() ).First().Key;
            var corpus = new StringBuilder( numberOfWords * 128 ); //just using 128 as a max avg word length..

            while ( numberOfWords.Any() ) {

                //var word = startWord;
                var randomChain = this.Nexts( word ).Where( w => !String.IsNullOrEmpty( w ) ).OrderBy( o => Randem.Next() );

                foreach ( var w in randomChain ) {
                    corpus.Append( $"{w}{Symbols.Singlespace}" );

                    word = w;
                    numberOfWords -= 1;
                }
            }

            return corpus.ToString().TrimEnd();
        }

        /// <summary>Return the list of strings found after this <paramref name="word" />.</summary>
        /// <param name="word"></param>
        /// <returns></returns>
        [NotNull]
        public IEnumerable<String> Nexts( [CanBeNull] String word ) {
            if ( !( word is null ) && this._markovChains.ContainsKey( word ) ) {
                return this._markovChains[ word ];
            }

            return Enumerable.Empty<String>().ToList();
        }

        public void Train( [CanBeNull] String corpus, Int32 level = 3 ) {
            var words = corpus.ToWords();

            Parallel.For( 0, words.Length, ( i, state ) => this._markovChains.TryAdd( words[ i ], words.Skip( i + 1 ).Take( level ).ToList() ) );
        }

    }

}