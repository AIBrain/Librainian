// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ConcurrentListFile.cs" was last cleaned by Rick on 2016/06/18 at 10:56 PM

namespace Librainian.Persistence {

    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Collections;
    using FileSystem;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;

    /// <summary>
    ///     Persist a dictionary to and from a JSON formatted text document.
    /// </summary>
    [JsonObject]
    public class ConcurrentListFile<TValue> : ConcurrentList<TValue> {

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        protected override void DisposeManaged() {
            this.Write().Wait();
            base.DisposeManaged();
        }

        /// <summary>
        ///     Persist a dictionary to and from a JSON formatted text document.
        /// </summary>
        /// <param name="document"></param>
        public ConcurrentListFile( [NotNull] Document document ) {
			this.Document = document ?? throw new ArgumentNullException( nameof( document ) );
            this.Read().Wait();
        }

        /// <summary>
        ///     Persist a dictionary to and from a JSON formatted text document.
        ///     <para>Defaults to user\appdata\Local\productname\filename</para>
        /// </summary>
        /// <param name="filename"></param>
        public ConcurrentListFile( [NotNull] String filename ) {
            if ( filename.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( filename ) );
            }
            var folder = new Folder( Environment.SpecialFolder.LocalApplicationData, Application.ProductName );
            if ( !folder.Exists() ) {
                folder.Create();
            }
            this.Document = new Document( folder, filename );
            this.Read().Wait();
        }

        /// <summary>
        ///     disallow constructor without a document/filename
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        private ConcurrentListFile() => throw new NotImplementedException();

	    /// <summary>
        /// </summary>
        [JsonProperty]
        [NotNull]
        public Document Document {
            get; set;
        }


        public async Task<Boolean> Read( CancellationToken cancellationToken = default ) {
            if ( !this.Document.Exists() ) {
                return false;
            }

            try {
                var data = this.Document.LoadJSON<ConcurrentList<TValue>>();
                if ( data != null ) {
                    await this.AddRangeAsync( data );
                    return true;
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

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override String ToString() => $"{this.Count} items";

	    /// <summary>
        ///     Saves the data to the <see cref="Document" />.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Boolean> Write( CancellationToken cancellationToken = default ) {
            var document = this.Document;

            return Task.Run( () => {
                if ( !document.Folder.Exists() ) {
                    document.Folder.Create();
                }

                if ( document.Exists() ) {
                    document.Delete();
                }

                return this.Save( document, true, Formatting.Indented );
            }, cancellationToken );
        }
    }
}