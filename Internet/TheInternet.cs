// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "TheInternet.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/TheInternet.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Media;
    using System.Net;
    using System.Threading.Tasks;
    using FileSystem;
    using JetBrains.Annotations;
    using Maths.Numbers;
    using Measurement.Time;
    using NUnit.Framework;
    using Threading;

    public static class TheInternet {

        /// <summary>
        ///     <para>Attempt to download the address to a local temp file.</para>
        ///     <para>Reports progress via <paramref name="reportProgress" /> by a <seealso cref="ZeroToOne" />.</para>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="reportProgress"></param>
        /// <param name="inProgress"></param>
        /// <param name="timeOut"></param>
        /// <param name="credentials"></param>
        /// <param name="onWebException"></param>
        /// <returns></returns>
        [ItemCanBeNull]
        public static async Task<Document> DownloadAsync( [NotNull] Uri address, TimeSpan timeOut, [CanBeNull] IProgress<ZeroToOne> reportProgress = null, VolatileBoolean inProgress = null,
            [CanBeNull] ICredentials credentials = null, [CanBeNull] Action<Uri, WebExceptionStatus> onWebException = null ) {
            if ( address is null ) { throw new ArgumentNullException( nameof( address ) ); }

            try {
                if ( inProgress != null ) { inProgress.Value = true; }

                reportProgress?.Report( ZeroToOne.MinValue );

                var tempDocument = Document.GetTempDocument();

                // ReSharper disable once UseObjectOrCollectionInitializer
                var webclient = new WebClient { Credentials = credentials };

                webclient.DownloadProgressChanged += ( sender, args ) => {
                    var progress = args.BytesReceived / ( Double )args.TotalBytesToReceive;
                    reportProgress?.Report( progress );
                };

                var timeoutTask = Task.Delay( timeOut );
                var downloadTask = webclient.DownloadFileTaskAsync( address, tempDocument.FullPathWithFileName );

                var task = await Task.WhenAny( timeoutTask, downloadTask );

                if ( task.Id == timeoutTask.Id ) { webclient.CancelAsync(); }

                return tempDocument;
            }
            catch ( WebException exception ) {
                try { onWebException?.Invoke( address, exception.Status ); }
                catch ( Exception exception2 ) { exception2.More(); }
            }
            catch ( Exception exception ) { exception.More(); }
            finally {
                reportProgress?.Report( ZeroToOne.MaxValue );

                if ( inProgress != null ) { inProgress.Value = false; }
            }

            return null;
        }

        public static IEnumerable<Document> FindFile( String filename, IEnumerable<String> locationClues ) {
            foreach ( var locationClue in locationClues ) {
                if ( !Uri.TryCreate( locationClue, UriKind.Absolute, out var internetAddress ) ) { continue; }

                //TODO this /totally/ is not finished yet.

                yield return new Document( internetAddress ); //should download file to a document in the user's temp folder.
            }
        }
    }

    public static class TheInternetTests {

        public static SoundPlayer Player { get; } = new SoundPlayer();

        private static void OnWebException( Uri uri, WebExceptionStatus webExceptionStatus ) {
            Console.WriteLine( uri );
            Console.WriteLine( webExceptionStatus );
        }

        [Test]
        public static void Test1() {
            var inprogress = new VolatileBoolean();
            var creds = new NetworkCredential( "AIBrain", @"hP&Y@bYsM5qT0tr" );
            var bob = TheInternet.DownloadAsync( new Uri( "https://www.freesound.org/people/BDWRekordings.com/sounds/98104/" ), Seconds.Ten, null, inprogress, creds, OnWebException ).Result;

            if ( null != bob ) {
                Player.Stream = File.OpenRead( bob.FullPathWithFileName );

                try { Player.PlaySync(); }
                catch ( Exception exception ) { exception.More(); }
            }
        }
    }
}