﻿// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting.
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "MkModel.cs" last formatted on 2021-02-03 at 4:40 PM.

namespace Librainian.Parsing.Markov {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Linguistics;
	using Maths;

	public class MkModel {

		public readonly String Name;

		private MkModel() => throw new NotImplementedException();

		public MkModel( [NotNull] String name ) {
			if ( String.IsNullOrWhiteSpace( name ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( name ) );
			}

			this.Name = name;
		}

		[NotNull]
		private ConcurrentDictionary<Word, List<Word>> _markovChains { get; } = new();

		[NotNull]
		public Task<String> GenerateRandomCorpus( Int32 numberOfWords ) =>
			Task.Run( () => {
				if ( !this._markovChains.Any() ) {
					return String.Empty;
				}

				var word = this._markovChains.OrderBy( _ => Randem.Next() ).First().Key; //pick a random starting word
				var corpus = new StringBuilder( numberOfWords * 128 ); //just using 128 as a max avg word length..

				foreach ( var _ in 0.To( numberOfWords ) ) {
					var randomChain = this.Nexts( word ).Where( w => !String.IsNullOrEmpty( w ) ).OrderBy( o => Randem.Next() );

					foreach ( var w in randomChain ) {
						corpus.Append( $"{w}{ParsingConstants.Strings.Singlespace}" );

						word = w;
					}
				}

				return corpus.ToString().TrimEnd();
			} );

		/// <summary>
		///     Return the list of strings found after this <paramref name="word" />.
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		[NotNull]
		public IEnumerable<Word> Nexts( [CanBeNull] Word? word ) {
			if ( word is null ) {
				return Enumerable.Empty<Word>();
			}

			if ( !this._markovChains.ContainsKey( word ) ) {
				return Enumerable.Empty<Word>();
			}

			return this._markovChains[word];
		}

		public Task Train( [CanBeNull] String? corpus, Int32 level = 3 ) =>
			Task.Run( () => {
				var words = corpus.ToWords().ToList();

				foreach ( var (word, index) in words.Select( ( word, i ) => ( word, i ) ) ) {
					var chain = words.Skip( index + 1 ).Take( level );
					this._markovChains.TryAdd( word, chain.ToList() );

					//TODO What will this do with duplicate Word in sentence?
				}
			} );

	}
}