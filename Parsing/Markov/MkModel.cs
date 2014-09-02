#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/MkModel.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Parsing.Markov {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Annotations;
    using Collections;
    using Persistence;
    using Threading;

    public class MkModel {
        public readonly String Name;

        private readonly ConcurrentDictionary< String, List< String > > _markovChains = new ConcurrentDictionary< String, List< String > >();

        public MkModel() {
            throw new NotImplementedException();
        }

        public MkModel( String name ) {
            this.Name = name;
        }

        public String GenerateRandomCorpus( int numberOfWords ) {
            if ( !this._markovChains.Any() ) {
                return String.Empty;
            }
            var startWord = this._markovChains.OrderBy( o => Randem.Next() ).FirstOrDefault().Key;
            var newCorpus = new StringBuilder( startWord );

            while ( numberOfWords > 0 ) {
                var word = startWord;
                var randomChain = this.Nexts( word: word ).OrderBy( o => Randem.Next() );

                foreach ( var w in randomChain ) {
                    newCorpus.AppendFormat( "{0} ", w );
                    if ( String.IsNullOrEmpty( w ) ) {
                        continue;
                    }
                    startWord = w;
                    numberOfWords -= 1;
                }
            }
            return newCorpus.ToString();
        }

        public Boolean Load() {
            return this.Name.Loader< MkModel >( source => source.DeepClone( destination: this ) );
        }

        /// <summary>
        ///     Return the list of strings found after this <paramref name="word" />.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public IEnumerable< String > Nexts( [CanBeNull] String word ) {
            if ( word == null ) {
                return CollectionExtensions.EmptyList;
            }

            return this._markovChains.ContainsKey( key: word ) ? this._markovChains[ key: word ] : CollectionExtensions.EmptyList;
        }

        public Boolean Save() {
            return this.Saver( this.Name );
        }

        public void Train( String corpus, int level = 3 ) {
            //return Task.Run( () => {
            var words = corpus.ToWords().AsParallel().ToArray();

            Parallel.For( 0, words.Length, ( i, state ) => this._markovChains.TryAdd( key: words[ i ], value: words.Skip( i + 1 ).Take( level ).ToList() ) );

            //for ( var i = 0; i < words.Length; i++ ) {
            //    this._markovChains.TryAdd( key: words[ i ], value: words.Skip( i + 1 ).Take( level ).ToList() );
            //}
            //} );
        }
    }
}
