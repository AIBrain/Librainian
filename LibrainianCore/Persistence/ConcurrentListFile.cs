// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentListFile.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "ConcurrentListFile.cs" was last formatted by Protiguous on 2019/08/08 at 9:27 AM.

namespace LibrainianCore.Persistence {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Mime;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Collections.Lists;
    using Logging;
    using OperatingSystem.FileSystem;

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
            if ( document is null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            var folder = new Folder( Environment.SpecialFolder.LocalApplicationData, MediaTypeNames.Application.ProductName );

            if ( !folder.Exists() ) {
                folder.Create();
            }

            this.Document = document ?? throw new ArgumentNullException( nameof( document ) );
            this.Read().Wait(); //TODO I don't like this Wait being here.
        }

        /// <summary>
        ///     Persist a dictionary to and from a JSON formatted text document.
        ///     <para>Defaults to user\appdata\Local\productname\filename</para>
        /// </summary>
        /// <param name="filename"></param>
        public ConcurrentListFile( [NotNull] String filename ) : this( new Document( filename ) ) {
        }

        public async Task<Boolean> Read( CancellationToken token = default ) {
            if ( this.Document.Exists() == false ) {
                return false;
            }

            try {
                var data = this.Document.LoadJSON<IEnumerable<TValue>>();

                if ( data != null ) {
                    await this.AddRangeAsync( data, token ).ConfigureAwait( false );

                    return true;
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

                //file is huge
                exception.Log();
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
        /// <param name="token"></param>
        /// <returns></returns>
        [NotNull]
        public Task<Boolean> Write( CancellationToken token = default ) {
            var document = this.Document;

            return Task.Run( () => {
                if ( !document.ContainingingFolder().Exists() ) {
                    document.ContainingingFolder().Create();
                }

                if ( document.Exists() ) {
                    document.Delete();
                }

                return this.TrySave( document, true, Formatting.Indented );
            }, token );
        }
    }
}