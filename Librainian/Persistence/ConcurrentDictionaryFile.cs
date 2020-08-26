// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ConcurrentDictionaryFile.cs" last formatted on 2020-08-14 at 8:44 PM.

namespace Librainian.Persistence {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Logging;
	using Measurement.Time;
	using Newtonsoft.Json;
	using OperatingSystem.FileSystem;

	/// <summary>Persist a dictionary to and from a JSON formatted text document.</summary>
	[JsonObject]
	public class ConcurrentDictionaryFile<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, IDisposable {

		private volatile Boolean _isLoading;

		// ReSharper disable once NotNullMemberIsNotInitialized
		private ConcurrentDictionaryFile() => throw new NotImplementedException();

		/// <summary>Disallow constructor without a document/filename</summary>
		/// <summary></summary>
		/// <summary>Persist a dictionary to and from a JSON formatted text document.</summary>
		/// <param name="document"></param>
		/// <param name="preload"> </param>
		public ConcurrentDictionaryFile( [NotNull] Document document, Boolean preload = false ) {
			this.Document = document ?? throw new ArgumentNullException( nameof( document ) );

			if ( !this.Document.ContainingingFolder().Exists() ) {
				this.Document.ContainingingFolder().Create();
			}

			if ( preload ) {
				this.Load();
			}
		}

		/// <summary>
		///     Persist a dictionary to and from a JSON formatted text document.
		///     <para>Defaults to user\appdata\Local\productname\filename</para>
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="preload"> </param>
		public ConcurrentDictionaryFile( [NotNull] String filename, Boolean preload = false ) : this( new Document( filename ), preload ) { }

		[JsonProperty]
		[NotNull]
		public Document Document { get; }

		public Boolean IsLoading {
			get => this._isLoading;
			set => this._isLoading = value;
		}

		public CancellationTokenSource MainCTS { get; } = new CancellationTokenSource();

		public void Dispose() {
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( Boolean releaseManaged ) {
			if ( releaseManaged ) {
				this.Save().Wait( Minutes.One );
			}

			GC.SuppressFinalize( this );
		}

		public Boolean Flush() {
			var document = this.Document;

			if ( !document.ContainingingFolder().Exists() ) {
				document.ContainingingFolder().Create();
			}

			IDictionary<TKey, TValue> me = new Dictionary<TKey, TValue>( this.Count );

			foreach ( var pair in this ) {
				me[pair.Key] = pair.Value;
			}

			return me.TrySave( document, true, Formatting.Indented );
		}

		public Boolean Load( CancellationToken token = default ) {
			var document = this.Document;

			if ( document.Exists() == false ) {
				return default;
			}

			try {
				this.IsLoading = true;

				if ( token == default ) {
					token = this.MainCTS.Token;
				}

				var dictionary = document.LoadJSON<ConcurrentDictionary<TKey, TValue>>();

				if ( dictionary != null ) {
					var result = Parallel.ForEach( dictionary.Keys.AsParallel(), body: key => this[key] = dictionary[key], parallelOptions: new ParallelOptions {
						CancellationToken = token
					} );

					return result.IsCompleted;
				}
			}
			catch ( JsonException exception ) {
				exception.Log();
			}
			catch ( IOException exception ) {
				//file in use by another app
				exception.Log();
			}
			catch ( OutOfMemoryException exception ) {
				//file is huge (too big to load into memory).
				exception.Log();
			}
			finally {
				this.IsLoading = false;
			}

			return default;
		}

		/// <summary>Saves the data to the <see cref="Document" />.</summary>
		/// <param name="token"></param>
		/// <returns></returns>
		[NotNull]
		public Task<Boolean> Save( CancellationToken token = default ) {
			if ( token == default ) {
				token = this.MainCTS.Token;
			}

			return Task.Run( this.Flush, token );
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		[NotNull]
		public override String ToString() => $"{this.Keys.Count} keys, {this.Values.Count} values";

		[DebuggerStepThrough]
		public Boolean TryRemove( [CanBeNull] TKey key ) => this.TryRemove( key, out _ );

	}

}