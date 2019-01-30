// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// this entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// this source code contained in "Internet.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Internet.cs" was last formatted by Protiguous on 2018/12/25 at 7:44 AM.

namespace Librainian.Internet {

    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Cache;
    using System.Threading;
    using System.Threading.Tasks;
    using ComputerSystem.FileSystem;
    using JetBrains.Annotations;
    using Logging;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;

    public static class Internet {

        private static ConcurrentDictionary<Guid, IDownloader> DownloadRequests { get; } = new ConcurrentDictionary<Guid, IDownloader>();

        internal static ThreadLocal<WebClientWithTimeout> WebClients { get; } = new ThreadLocal<WebClientWithTimeout>( () => new WebClientWithTimeout(), true );

        public enum ResponseCode {

            Error = -1,

            Unknown = 0,

            Success = 1
        }

        public interface IDownloader {

            [CanBeNull]
            ICredentials Credentials { get; }

            /// <summary>
            ///     When downloading data, this will be the destination.
            /// </summary>
            [CanBeNull]
            Byte[] DestinationBuffer { get; }

            /// <summary>
            ///     When downloading a file, this will be the destination.
            /// </summary>
            [CanBeNull]
            Document DestinationDocument { get; }

            /// <summary>
            ///     The amount of time passed since the download was started. See also: <seealso cref="WhenStarted" />.
            /// </summary>
            Stopwatch Elasped { get; set; }

            [CanBeNull]
            Action OnCancelled { get; set; }

            [CanBeNull]
            Action OnCompleted { get; set; }

            [CanBeNull]
            Action OnFailure { get; set; }

            [CanBeNull]
            Action OnTimeout { get; set; }

            [NotNull]
            Uri Source { get; }

            Task Task { get; set; }

            /// <summary>
            ///     The length of time to wait before the download is cancelled. See also:
            ///     <seealso cref="UnderlyingDownloader.Forever" />.
            /// </summary>
            TimeSpan Timeout { get; set; }

            /// <summary>
            ///     The UTC date & time when the download was started.
            /// </summary>
            DateTime WhenStarted { get; set; }

            Boolean Cancel();

            /// <summary>
            ///     Returns true if the web request is in progress.
            /// </summary>
            /// <returns></returns>
            Boolean IsBusy();

            Boolean Start();

            /// <summary>
            ///     this blocks until <see cref="UnderlyingDownloader.Downloaded" /> signals.
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="forHowLong" /> must be -1 milliseconds or greater.</exception>
            /// <exception cref="ObjectDisposedException"></exception>
            /// <exception cref="AbandonedMutexException">An abandoned mutex often indicates a serious coding error.</exception>
            /// <exception cref="Exception"></exception>
            Boolean Wait( TimeSpan forHowLong );
        }

        public class FileDownloader : UnderlyingDownloader {

            /// <summary>
            ///     Download a file. Call <see cref="Start" /> to begin the <see cref="UnderlyingDownloader.Task" />.
            /// </summary>
            /// <param name="source">The url of the data source to download.</param>
            /// <param name="destination">The local document to download to.</param>
            /// <param name="waitifBusy">
            ///     If true and <see cref="WebClient.IsBusy" /> is true, when block until false. Otherwise
            ///     <see cref="InvalidOperationException" /> will be thrown.
            /// </param>
            /// <param name="timeout">Time to wait before a download is cancelled.</param>
            /// <param name="autoStart">If true, the download will begin now.</param>
            /// <param name="credentials">Username and password, if needed otherwise null.</param>
            public FileDownloader( [NotNull] Uri source, [NotNull] Document destination, Boolean waitifBusy, TimeSpan timeout, Boolean autoStart = true,
                [CanBeNull] ICredentials credentials = null ) : base( source, destination, waitifBusy, timeout, credentials ) {
                $"{nameof( FileDownloader )} created with {nameof( this.Id )} of {this.Id}.".Log();

                DownloadRequests[ this.Id ] = this;

                if ( autoStart ) {
                    this.Start();
                }
            }

            public sealed override Boolean Start() {
                this.Downloaded.Reset();

                this.AttachedToWebClient.DownloadFileCompleted += ( sender, args ) => {
                    this.Downloaded.Set();

                    try {
                        this.OnCompleted?.Invoke();
                    }
                    catch ( Exception exception ) {
                        exception.Log();
                    }
                    finally {
                        DownloadRequests.TryRemove( this.Id, out _ );
                    }
                };

                this.AttachedToWebClient.Timeout = this.Timeout;

                this.Task = this.AttachedToWebClient.DownloadFileTaskAsync( this.Source, this.DestinationDocument.FullPath );

                return base.Start();
            }
        }

        public abstract class UnderlyingDownloader : ReactiveObject, IDownloader {

            public static RequestCachePolicy DefaultCachePolicy { get; } = new HttpRequestCachePolicy( HttpRequestCacheLevel.Default );

            /// <summary>
            ///     -1 milliseconds
            /// </summary>
            public static TimeSpan Forever { get; } = TimeSpan.FromMilliseconds( -1 );

            [NotNull]
            [Reactive]
            public WebClientWithTimeout AttachedToWebClient { get; }

            [CanBeNull]
            [Reactive]
            public ICredentials Credentials { get; }

            [CanBeNull]
            [Reactive]
            public Byte[] DestinationBuffer { get; }

            [NotNull]
            [Reactive]
            public Document DestinationDocument { get; }

            public AutoResetEvent Downloaded { get; } = new AutoResetEvent( false );

            /// <summary>
            ///     The amount of time passed since the download was started. See also: <seealso cref="WhenStarted" />.
            /// </summary>
            [Reactive]
            public Stopwatch Elasped { get; set; }

            /// <summary>
            ///     The unique identifier assigned to this download.
            /// </summary>
            [Reactive]
            public Guid Id { get; }

            [CanBeNull]
            [Reactive]
            public Action OnCancelled { get; set; }

            [CanBeNull]
            [Reactive]
            public Action OnCompleted { get; set; }

            [CanBeNull]
            [Reactive]
            public Action OnFailure { get; set; }

            [CanBeNull]
            [Reactive]
            public Action OnTimeout { get; set; }

            [NotNull]
            [Reactive]
            public Uri Source { get; }

            [CanBeNull]
            public Task Task { get; set; }

            /// <summary>
            ///     The length of time to wait before the download is cancelled. See also: <seealso cref="Forever" />.
            /// </summary>
            [Reactive]
            public TimeSpan Timeout { get; set; }

            /// <summary>
            ///     The UTC date & time when the download was started.
            /// </summary>
            [Reactive]
            public DateTime WhenStarted { get; set; }

            /// <summary>
            ///     ctor
            /// </summary>
            /// <param name="source"></param>
            /// <param name="destination"></param>
            /// <param name="waitIfBusy"></param>
            /// <param name="timeout"></param>
            /// <param name="credentials"></param>
            /// <exception cref="InvalidOperationException">Thrown when the <see cref="WebClient" /> is busy.</exception>
            protected UnderlyingDownloader( [NotNull] Uri source, [NotNull] Document destination, Boolean waitIfBusy, TimeSpan timeout,
                [CanBeNull] ICredentials credentials = null ) {
                var web = WebClients.Value;

                if ( web.IsBusy ) {
                    if ( waitIfBusy ) {
                        this.Wait( timeout );
                    }
                    else {
                        throw new InvalidOperationException( $"WebClient is already being used. Unable to download \"{this.Source}\"." );
                    }
                }

                this.AttachedToWebClient = web;
                this.Source = source ?? throw new ArgumentNullException( nameof( source ) );
                this.DestinationDocument = destination ?? throw new ArgumentNullException( nameof( destination ) );
                this.Timeout = timeout;
                this.Id = Guid.NewGuid();
                this.Credentials = credentials;

                this.AttachedToWebClient.Credentials = this.Credentials;
                this.AttachedToWebClient.CachePolicy = DefaultCachePolicy;
                this.DestinationBuffer = destination.AsBytes() as Byte[]; //can we do this??
            }

            public virtual Boolean Cancel() {
                try {
                    this.AttachedToWebClient.CancelAsync();
                }
                catch ( Exception exception ) {
                    exception.Log();
                }

                return this.IsBusy();
            }

            public (ResponseCode responseCode, Int64 fileLength) GetContentLength() {

                if ( WebRequest.Create( this.Source ) is HttpWebRequest request ) {
                    request.Method = "HEAD";

                    using ( var response = request.GetResponse() ) {
                        return (ResponseCode.Success, response.ContentLength);
                    }
                }

                return (ResponseCode.Error, default);
            }

            /// <summary>
            ///     Returns true if the web request is in progress.
            /// </summary>
            /// <returns></returns>
            public Boolean IsBusy() => this.AttachedToWebClient.IsBusy;

            public virtual Boolean Start() {
                this.WhenStarted = DateTime.UtcNow;
                this.Elasped = Stopwatch.StartNew();

                return true;
            }

            /// <summary>
            ///     this blocks until <see cref="Downloaded" /> signals.
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="forHowLong" /> must be -1 milliseconds or greater.</exception>
            /// <exception cref="ObjectDisposedException"></exception>
            /// <exception cref="AbandonedMutexException">An abandoned mutex often indicates a serious coding error.</exception>
            /// <exception cref="Exception"></exception>
            public Boolean Wait( TimeSpan forHowLong ) {
                try {
                    if ( forHowLong < Forever ) {
                        forHowLong = Forever;
                    }

                    return this.Downloaded.WaitOne( forHowLong );
                }
                catch ( ArgumentOutOfRangeException exception ) {
                    exception.Log();

                    throw;
                }
                catch ( ObjectDisposedException exception ) {
                    exception.Log();

                    throw;
                }
                catch ( AbandonedMutexException exception ) {
                    exception.Log();

                    throw;
                }
                catch ( InvalidOperationException exception ) {
                    exception.Log();

                    throw;
                }
                catch ( Exception exception ) {
                    exception.Log();

                    throw;
                }
            }
        }

        public class WebClientWithTimeout : WebClient {

            /// <summary>
            ///     The <see cref="WebRequest" /> instance.
            /// </summary>
            [CanBeNull]
            public WebRequest Request { get; private set; }

            public TimeSpan Timeout { get; set; }

            public WebClientWithTimeout() : this( UnderlyingDownloader.Forever ) { }

            public WebClientWithTimeout( TimeSpan timeout ) => this.Timeout = timeout;

            protected override WebRequest GetWebRequest( Uri address ) {
                this.Request = base.GetWebRequest( address );

                var webRequest = this.Request;

                if ( webRequest != null ) {
                    webRequest.Timeout = ( Int32 )this.Timeout.TotalMilliseconds;
                }

                return webRequest;
            }
        }
    }
}