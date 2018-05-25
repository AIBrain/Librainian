// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentListFile.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/ConcurrentListFile.cs" was last formatted by Protiguous on 2018/05/21 at 11:18 PM.

namespace Librainian.Persistence {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Collections;
    using ComputerSystems.FileSystem;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;
    using Threading;

    /// <summary>
    ///     Persist a list to and from a JSON formatted text document.
    /// </summary>
    [JsonObject]
    public class ConcurrentListFile<TValue> : ConcurrentList<TValue> {

        /// <summary>
        ///     disallow constructor without a document/filename
        /// </summary>

        /// <summary>
        /// </summary>
        [JsonProperty]
        [NotNull]
        public Document Document { get; set; }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private ConcurrentListFile() => throw new NotImplementedException();

        /// <summary>
        ///     Persist a dictionary to and from a JSON formatted text document.
        /// </summary>
        /// <param name="document"></param>
        public ConcurrentListFile( [NotNull] Document document ) {
            this.Document = document ?? throw new ArgumentNullException( nameof( document ) );
            this.Read().Wait(); //TODO I don't like this here.
        }

        /// <summary>
        ///     Persist a dictionary to and from a JSON formatted text document.
        ///     <para>Defaults to user\appdata\Local\productname\filename</para>
        /// </summary>
        /// <param name="filename"></param>
        public ConcurrentListFile( [NotNull] String filename ) {
            if ( filename.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( filename ) ); }

            var folder = new Folder( Environment.SpecialFolder.LocalApplicationData, Application.ProductName );

            if ( !folder.Exists() ) { folder.Create(); }

            this.Document = new Document( folder, filename );
            this.Read().Wait();
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() {
            this.Write().Wait();
            base.DisposeManaged();
        }

        public async Task<Boolean> Read( CancellationToken cancellationToken = default ) {
            if ( !this.Document.Exists() ) { return false; }

            try {
                var data = this.Document.LoadJSON<IEnumerable<TValue>>();

                if ( data != null ) {
                    await this.AddRangeAsync( data ).NoUI();

                    return true;
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

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => $"{this.Count} items";

        /// <summary>
        ///     Saves the data to the <see cref="Document" />.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Boolean> Write( CancellationToken cancellationToken = default ) {
            var document = this.Document;

            return Task.Run( () => {
                if ( !document.Folder.Exists() ) { document.Folder.Create(); }

                if ( document.Exists() ) { document.Delete(); }

                return this.TrySave( document, true, Formatting.Indented );
            }, cancellationToken );
        }
    }
}