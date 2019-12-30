﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TheInternet.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "TheInternet.cs" was last formatted by Protiguous on 2019/11/07 at 2:05 PM.

namespace LibrainianCore.Internet {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Threading.Tasks;
    using Logging;
    using Maths.Numbers;
    using OperatingSystem.FileSystem;
    using Threading;

    public static class TheInternet {

        /// <summary>
        ///     <para>Attempt to download the address to a local temp file.</para>
        ///     <para>Reports progress via <paramref name="reportProgress" /> by a <see cref="ZeroToOne" />.</para>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="reportProgress"></param>
        /// <param name="inProgress"></param>
        /// <param name="timeOut"></param>
        /// <param name="credentials"></param>
        /// <param name="onWebException"></param>
        /// <returns></returns>
        [ItemCanBeNull]
        public static async Task<IDocument> DownloadAsync( [NotNull] Uri address, TimeSpan timeOut, [CanBeNull] IProgress<ZeroToOne> reportProgress = null,
            VolatileBoolean inProgress = default, [CanBeNull] ICredentials credentials = null, [CanBeNull] Action<Uri, WebExceptionStatus> onWebException = null ) {
            if ( address is null ) {
                throw new ArgumentNullException( nameof( address ) );
            }

            try {

                inProgress.Value = true;

                reportProgress?.Report( ZeroToOne.MinValue );

                var tempDocument = Document.GetTempDocument();

                // ReSharper disable once UseObjectOrCollectionInitializer
                var webclient = new WebClient {
                    Credentials = credentials
                };

                webclient.DownloadProgressChanged += ( sender, args ) => {
                    var progress = args.BytesReceived / ( Double )args.TotalBytesToReceive;
                    reportProgress?.Report( progress );
                };

                var timeoutTask = Task.Delay( timeOut );
                var downloadTask = webclient.DownloadFileTaskAsync( address, tempDocument.FullPath );

                var task = await Task.WhenAny( timeoutTask, downloadTask ).ConfigureAwait( false );

                if ( task.Id == timeoutTask.Id ) {
                    webclient.CancelAsync();
                }

                return tempDocument;
            }
            catch ( WebException exception ) {
                try {
                    onWebException?.Invoke( address, exception.Status );
                }
                catch ( Exception exception2 ) {
                    exception2.Log();
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
            finally {
                reportProgress?.Report( ZeroToOne.MaxValue );

                //inProgress = false;
            }

            return null;
        }

        public static IEnumerable<Document> FindFile( [NotNull] String filename, [NotNull] IEnumerable<String> locationClues ) {
            if ( locationClues is null ) {
                throw new ArgumentNullException( paramName: nameof( locationClues ) );
            }

            if ( String.IsNullOrWhiteSpace( value: filename ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( filename ) );
            }

            foreach ( var locationClue in locationClues ) {
                if ( !Uri.TryCreate( locationClue, UriKind.Absolute, out var internetAddress ) ) {
                    continue;
                }

                //TODO this /totally/ is not finished yet.

                yield return new Document( internetAddress.ToString() ); //should download file to a document in the user's temp folder.
            }
        }
    }
}