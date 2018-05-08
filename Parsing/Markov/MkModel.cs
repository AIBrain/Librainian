// Copyright 2016 Protiguous.
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
// "Librainian/MkModel.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

namespace Librainian.Parsing.Markov {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Collections;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Persistence;

    public class MkModel {
        public readonly String Name;
        private readonly ConcurrentDictionary<String, List<String>> _markovChains = new ConcurrentDictionary<String, List<String>>();

        public MkModel() => throw new NotImplementedException();

	    public MkModel( String name ) => this.Name = name;

	    public String GenerateRandomCorpus( Int32 numberOfWords ) {
            if ( !this._markovChains.Any() ) {
                return String.Empty;
            }
            var startWord = this._markovChains.OrderBy( o => Randem.Next() ).FirstOrDefault().Key;
            var newCorpus = new StringBuilder( startWord );

            while ( numberOfWords > 0 ) {
                var word = startWord;
                var randomChain = this.Nexts( word: word ).OrderBy( o => Randem.Next() );

                foreach ( var w in randomChain ) {
                    newCorpus.Append( $"{w} " );
                    if ( String.IsNullOrEmpty( w ) ) {
                        continue;
                    }
                    startWord = w;
                    numberOfWords -= 1;
                }
            }
            return newCorpus.ToString();
        }

        public Boolean Load() => this.Name.Loader<MkModel>( source => source.DeepClone( destination: this ) );

        /// <summary>Return the list of strings found after this <paramref name="word" />.</summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public IEnumerable<String> Nexts( [CanBeNull] String word ) {
            if ( word is null ) {
                return Enumerable.Empty<String>();
            }

            if ( this._markovChains.ContainsKey(word ) ) {
                return this._markovChains[word ];
            }
            return Enumerable.Empty<String>();
        }

        public Boolean Save() => this.Saver( this.Name );

        public void Train( String corpus, Int32 level = 3 ) {

            var words = corpus.ToWords().AsParallel().ToArray();

            Parallel.For( 0, words.Length, ( i, state ) => this._markovChains.TryAdd(words[ i ], value: words.Skip( i + 1 ).Take( level ).ToList() ) );
        }
    }
}