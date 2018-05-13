// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ConcurrentDictionaryFile.cs" was last cleaned by Protiguous on 2018/05/13 at 1:40 AM

namespace Librainian.Persistence {

    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FileSystem;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Newtonsoft.Json;

    /// <summary>
    /// Persist a dictionary to and from a JSON formatted text document.
    /// </summary>
    [JsonObject]
    public class ConcurrentDictionaryFile<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, IDisposable {

        private volatile Boolean _isReading;

        /// <summary>
        /// disallow constructor without a document/filename
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        private ConcurrentDictionaryFile() => throw new NotImplementedException();

        /// <summary>
        /// Persist a dictionary to and from a JSON formatted text document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="preload"> </param>
        public ConcurrentDictionaryFile( [NotNull] Document document, Boolean preload = false ) {
            this.Document = document ?? throw new ArgumentNullException( nameof( document ) );

            if ( !this.Document.Folder.Exists() ) {
                this.Document.Folder.Create();
            }

            if ( preload ) {
                this.Load().Wait();
            }
        }

        /// <summary>
        /// Persist a dictionary to and from a JSON formatted text document.
        /// <para>Defaults to user\appdata\Local\productname\filename</para>
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="preload"> </param>
        public ConcurrentDictionaryFile( [NotNull] String filename, Boolean preload = false ) : this( document: new Document( fullPathWithFilename: filename ), preload: preload ) { }

        private Boolean IsReading {
            get => this._isReading;
            set => this._isReading = value;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        [NotNull]
        public Document Document { get; }

        protected virtual void Dispose( Boolean releaseManaged ) {
            if ( releaseManaged ) {
                this.Write().Wait( timeout: Minutes.One );
            }

            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => this.Dispose( releaseManaged: true );

        public async Task<Boolean> Load( CancellationToken cancellationToken = default ) {
            this.IsReading = true;

            try {
                var document = this.Document;

                if ( !document.Exists() ) {
                    return false;
                }

                try {
                    var data = await document.LoadJSONAsync<ConcurrentDictionary<TKey, TValue>>().ConfigureAwait( false );

                    if ( data != null ) {
                        var result = Parallel.ForEach( source: data.Keys.AsParallel(), body: key => this[key] = data[key] );

                        return result.IsCompleted;
                    }
                }
                catch ( JsonException exception ) {
                    exception.More();
                }
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
            finally {
                this.IsReading = false;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => $"{Keys.Count} keys, {Values.Count} values";

        [DebuggerStepThrough]
        public Boolean TryRemove( TKey key ) => key != null && TryRemove( key, value: out _ );

        /// <summary>
        /// Saves the data to the <see cref="Document"/>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Boolean> Write( CancellationToken cancellationToken = default ) =>
            await Task.Run( () => {
                var document = this.Document;

                if ( !document.Folder.Exists() ) {
                    document.Folder.Create();
                }

                if ( document.Exists() ) {
                    document.Delete();
                }

                return this.Save( document: document, overwrite: true, formatting: Formatting.Indented );
            }, cancellationToken: cancellationToken ).ConfigureAwait( false );
    }
}