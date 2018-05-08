// Copyright 2018 Protiguous.
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
// "Librainian/ConcurrentDictionaryFile.cs" was last cleaned by Protiguous on 2018/02/03 at 4:26 PM

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
    using Threading;

    /// <summary>
    ///     Persist a dictionary to and from a JSON formatted text document.
    /// </summary>
    [JsonObject]
    public class ConcurrentDictionaryFile<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, IDisposable {
        private volatile Boolean _isReading;

        /// <summary>
        ///     Persist a dictionary to and from a JSON formatted text document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="autoload"></param>
        public ConcurrentDictionaryFile( [NotNull] Document document, Boolean autoload = false ) {
            this.Document = document ?? throw new ArgumentNullException( paramName: nameof( document ) );

            if ( !this.Document.Folder.Exists() ) {
                this.Document.Folder.Create();
            }

            if ( autoload ) {
                this.Load().Wait();
            }
        }

        /// <summary>
        ///     Persist a dictionary to and from a JSON formatted text document.
        ///     <para>Defaults to user\appdata\Local\productname\filename</para>
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="autoload"></param>
        public ConcurrentDictionaryFile( [NotNull] String filename, Boolean autoload = false ) : this( document: new Document( fullPathWithFilename: filename ), autoload: autoload ) { }

        /// <summary>
        ///     disallow constructor without a document/filename
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        private ConcurrentDictionaryFile() => throw new NotImplementedException();

        private Boolean isReading {
            get => this._isReading;
            set => this._isReading = value;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        [NotNull]
        public Document Document { get; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => this.Dispose( releaseManaged: true );

        protected virtual void Dispose( Boolean releaseManaged ) {
            if ( releaseManaged ) {
                this.Write().Wait( timeout: Minutes.One );
            }

            GC.SuppressFinalize( this );
        }

        public async Task<(Boolean loaded, UInt64 bytesRead)> Load( CancellationToken cancellationToken = default ) {
            this.isReading = true;
            try {
                var document = this.Document;

                if ( !document.Exists() ) {
                    return (false, UInt64.MinValue);
                }

                try {
                    var data = await document.LoadJSONAsync<ConcurrentDictionary<TKey, TValue>>();
                    if ( data != null ) {
                        var result = Parallel.ForEach( source: data.Keys.AsParallel(), parallelOptions: ThreadingExtensions.CPUIntensive, body: key => { this[  key] = data[  key]; } );
                        return (result.IsCompleted, 0);
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

                return (false, 0);
            }
            finally {
                this.isReading = false;
            }
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override String ToString() => $"{this.Keys.Count} keys, {this.Values.Count} values";

        [DebuggerStepThrough]
        public Boolean TryRemove( TKey key ) {
            if ( key is null ) {
                throw new ArgumentNullException( paramName: nameof( key ) );
            }

            return this.TryRemove(key, value: out var value );
        }

        /// <summary>
        ///     Saves the data to the <see cref="Document" />.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Boolean> Write( CancellationToken cancellationToken = default ) =>
            Task.Run( () => {
                var document = this.Document;

                if ( !document.Folder.Exists() ) {
                    document.Folder.Create();
                }

                if ( document.Exists() ) {
                    document.Delete();
                }

                return this.Save( document: document, overwrite: true, formatting: Formatting.Indented );
            }, cancellationToken: cancellationToken );
    }
}