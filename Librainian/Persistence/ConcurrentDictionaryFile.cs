// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentDictionaryFile.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "ConcurrentDictionaryFile.cs" was last formatted by Protiguous on 2020/01/31 at 12:29 AM.

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

        [JsonProperty]
        [NotNull]
        public Document Document { get; }

        public Boolean IsLoading {
            get => this._isLoading;
            set => this._isLoading = value;
        }

        [NotNull]
        public CancellationTokenSource MainCTS { get; } = new CancellationTokenSource();

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

        /// <summary>Persist a dictionary to and from a JSON formatted text document.
        /// <para>Defaults to user\appdata\Local\productname\filename</para>
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="preload"> </param>
        public ConcurrentDictionaryFile( [NotNull] String filename, Boolean preload = false ) : this( document: new Document( filename ), preload: preload ) { }

        protected virtual void Dispose( Boolean releaseManaged ) {
            if ( releaseManaged ) {
                this.Save().Wait( timeout: Minutes.One );
            }

            GC.SuppressFinalize( this );
        }

        public void Dispose() {
            this.Dispose( releaseManaged: true );
            GC.SuppressFinalize( this );
        }

        public Boolean Flush() {
            var document = this.Document;

            if ( !document.ContainingingFolder().Exists() ) {
                document.ContainingingFolder().Create();
            }

            IDictionary<TKey, TValue> me = new Dictionary<TKey, TValue>( this.Count );

            foreach ( var pair in this ) {
                if ( !(pair.Key is null) ) {
                    me[ pair.Key ] = pair.Value;
                }
            }

            return me.TrySave( document: document, overwrite: true, formatting: Formatting.Indented );
        }

        public Boolean Load( CancellationToken token = default ) {
            var document = this.Document;

            if ( !document.Exists() ) {
                return default;
            }

            try {
                this.IsLoading = true;

                if ( token == default ) {
                    token = this.MainCTS.Token;
                }

                var dictionary = document.LoadJSON<ConcurrentDictionary<TKey, TValue>>();

                if ( dictionary != null ) {
                    var result = Parallel.ForEach( source: dictionary.Keys.AsParallel(), body: key => this[ key ] = dictionary[ key ], parallelOptions: new ParallelOptions {
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

            return Task.Run( this.Flush, cancellationToken: token );
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => $"{this.Keys.Count} keys, {this.Values.Count} values";

        [DebuggerStepThrough]
        public Boolean TryRemove( [CanBeNull] TKey key ) => !(key is null) && this.TryRemove( key, out _ );
    }
}