// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentDictionaryFile.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
//
// ***  Project "Librainian"  ***
// File "ConcurrentDictionaryFile.cs" was last formatted by Protiguous on 2018/06/04 at 4:21 PM.

namespace Librainian.Persistence {

	using System;
	using System.Collections.Concurrent;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ComputerSystems.FileSystem;
	using JetBrains.Annotations;
	using Measurement.Time;
	using Newtonsoft.Json;
	using Threading;

	/// <summary>
	///     Persist a dictionary to and from a JSON formatted text document.
	/// </summary>
	[JsonObject]
	public class ConcurrentDictionaryFile<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, IDisposable {

		private volatile Boolean _isReading;

		private Boolean IsReading {
			get => this._isReading;
			set => this._isReading = value;
		}

		// ReSharper disable once NotNullMemberIsNotInitialized
		private ConcurrentDictionaryFile() => throw new NotImplementedException();

		protected virtual void Dispose( Boolean releaseManaged ) {
			if ( releaseManaged ) { this.Write().Wait( timeout: Minutes.One ); }

			GC.SuppressFinalize( this );
		}

		public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

		/// <summary>
		///     disallow constructor without a document/filename
		/// </summary>
		/// <summary>
		/// </summary>
		[JsonProperty]
		[NotNull]
		public Document Document { get; }

		/// <summary>
		///     Persist a dictionary to and from a JSON formatted text document.
		/// </summary>
		/// <param name="document"></param>
		/// <param name="preload"> </param>
		public ConcurrentDictionaryFile( [NotNull] Document document, Boolean preload = false ) {
			this.Document = document ?? throw new ArgumentNullException( nameof( document ) );

			if ( !this.Document.Folder.Exists() ) { this.Document.Folder.Create(); }

			if ( preload ) { this.Load().Wait(); }
		}

		/// <summary>
		///     Persist a dictionary to and from a JSON formatted text document.
		///     <para>Defaults to user\appdata\Local\productname\filename</para>
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="preload"> </param>
		public ConcurrentDictionaryFile( [NotNull] String filename, Boolean preload = false ) : this( document: new Document( fullPathWithFilename: filename ), preload: preload ) { }

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() => this.Dispose( releaseManaged: true );

		public async Task<Boolean> Load( CancellationToken cancellationToken = default ) {
			this.IsReading = true;

			try {
				var document = this.Document;

				if ( !document.Exists() ) { return false; }

				try {
					var data = await document.LoadJSONAsync<ConcurrentDictionary<TKey, TValue>>( this.CancellationTokenSource.Token ).NoUI();

					if ( data != null ) {
						var result = Parallel.ForEach( source: data.Keys.AsParallel(), body: key => this[key] = data[key] );

						return result.IsCompleted;
					}
				}
				catch ( JsonException exception ) { exception.More(); }
				catch ( IOException exception ) {

					//file in use by another app
					exception.More();
				}
				catch ( OutOfMemoryException exception ) {

					//file is huge
					exception.More();
				}

				return false;
			}
			finally { this.IsReading = false; }
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override String ToString() => $"{this.Keys.Count} keys, {this.Values.Count} values";

		[DebuggerStepThrough]
		public Boolean TryRemove( TKey key ) => key != null && this.TryRemove( key, out _ );

		/// <summary>
		///     Saves the data to the <see cref="Document" />.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Boolean> Write( CancellationToken cancellationToken = default ) =>
			await Task.Run( () => {
				var document = this.Document;

				if ( !document.Folder.Exists() ) { document.Folder.Create(); }

				if ( document.Exists() ) { document.Delete(); }

				return this.TrySave( document: document, overwrite: true, formatting: Formatting.Indented );
			}, cancellationToken: cancellationToken ).NoUI();
	}
}