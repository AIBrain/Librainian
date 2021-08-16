// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Internet.cs" last formatted on 2020-08-14 at 8:34 PM.

namespace Librainian.Internet {

	using System;
	using System.Collections.Concurrent;
	using System.Diagnostics;
	using System.Net;
	using System.Net.Cache;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions;
	using FileSystem;
	using Logging;

	public static class Internet {

		private static ConcurrentDictionary<Guid, IDownloader> DownloadRequests { get; } = new();

		internal static ThreadLocal<WebClientWithTimeout> WebClients { get; } = new( () => new WebClientWithTimeout(), true );

		public interface IDownloader {

			ICredentials? Credentials { get; }

			/// <summary>When downloading data, this will be the destination.</summary>
			Byte[]? DestinationBuffer { get; }

			/// <summary>When downloading a file, this will be the destination.</summary>
			Document? DestinationDocument { get; }

			/// <summary>The amount of time passed since the download was started. See also: <seealso cref="WhenStarted" />.</summary>
			Stopwatch? Elasped { get; set; }

			Action? OnCancelled { get; set; }

			Action? OnCompleted { get; set; }

			Action? OnFailure { get; set; }

			Action? OnTimeout { get; set; }

			Uri Source { get; }

			Task? Task { get; set; }

			/// <summary>
			///     The length of time to wait before the download is cancelled. See also:
			///     <seealso cref="UnderlyingDownloader.Forever" />.
			/// </summary>
			TimeSpan Timeout { get; set; }

			/// <summary>The UTC date & time when the download was started.</summary>
			DateTime WhenStarted { get; set; }

			Boolean Cancel();

			/// <summary>Returns true if the web request is in progress.</summary>
			Boolean IsBusy();

			Boolean Start();

			/// <summary>this blocks until <see cref="UnderlyingDownloader.Downloaded" /> signals.</summary>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="forHowLong" /> must be -1 milliseconds or greater.</exception>
			/// <exception cref="ObjectDisposedException"></exception>
			/// <exception cref="AbandonedMutexException">An abandoned mutex often indicates a serious coding error.</exception>
			/// <exception cref="Exception"></exception>
			Boolean Wait( TimeSpan forHowLong, CancellationToken cancellationToken );
		}

		public class FileDownloader : UnderlyingDownloader {

			/// <summary>Download a file. Call <see cref="Start" /> to begin the <see cref="UnderlyingDownloader.Task" />.</summary>
			/// <param name="source">The url of the data source to download.</param>
			/// <param name="destination">The local document to download to.</param>
			/// <param name="waitifBusy">
			///     If true and <see cref="WebClient.IsBusy" /> is true, when block until false. Otherwise
			///     <see cref="InvalidOperationException" /> will be thrown.
			/// </param>
			/// <param name="timeout">Time to wait before a download is cancelled.</param>
			/// <param name="cancellationToken"></param>
			/// <param name="autoStart">If true, the download will begin now.</param>
			/// <param name="credentials">Username and password, if needed otherwise null.</param>
			public FileDownloader(
				Uri source,
				Document destination,
				Boolean waitifBusy,
				TimeSpan timeout,
				 CancellationToken cancellationToken,
				Boolean autoStart = true,
				ICredentials? credentials = null
			) : base( source, destination, waitifBusy, timeout, cancellationToken, credentials ) {
				$"{nameof( FileDownloader )} created with {nameof( this.Id )} of {this.Id}.".Log();

				DownloadRequests[ this.Id ] = this;

				if ( autoStart ) {
					this.Start();
				}
			}

			public sealed override Boolean Start() {
				this.Downloaded.Reset();

				this.Client.DownloadFileCompleted += ( sender, args ) => {
					this.Downloaded.Set();

					try {
						this.OnCompleted?.Invoke();
					}
					catch ( Exception exception ) {
						exception.Log();
					}
					finally {
						DownloadRequests.TryRemove( this.Id, out var _ );
					}
				};

				this.Client.Timeout = this.Timeout;

				this.Task = this.Client.DownloadFileTaskAsync( this.Source, this.DestinationDocument.FullPath );

				return base.Start();
			}
		}

		public abstract class UnderlyingDownloader : IDownloader {

			public static RequestCachePolicy DefaultCachePolicy { get; } = new HttpRequestCachePolicy( HttpRequestCacheLevel.Default );

			/// <summary>-1 milliseconds</summary>
			public static TimeSpan Forever { get; } = TimeSpan.FromMilliseconds( -1 );

			public WebClientWithTimeout Client { get; }

			public ICredentials? Credentials { get; set; }

			public Byte[]? DestinationBuffer { get; set; }

			public Document DestinationDocument { get; set; }

			public ManualResetEventSlim Downloaded { get; } = new( false );

			/// <summary>The amount of time passed since the download was started. See also: <seealso cref="WhenStarted" />.</summary>
			public Stopwatch? Elasped { get; set; }

			/// <summary>The unique identifier assigned to this download.</summary>
			public Guid Id { get; }

			public Action? OnCancelled { get; set; }

			public Action? OnCompleted { get; set; }

			public Action? OnFailure { get; set; }

			public Action? OnTimeout { get; set; }

			public Uri Source { get; set; }

			public Task? Task { get; set; }

			/// <summary>The length of time to wait before the download is cancelled. See also: <seealso cref="Forever" />.</summary>
			public TimeSpan Timeout { get; set; }

			/// <summary>The UTC date & time when the download was started.</summary>
			public DateTime WhenStarted { get; set; }

			/// <summary>ctor</summary>
			/// <param name="source"></param>
			/// <param name="destination"></param>
			/// <param name="waitIfBusy"></param>
			/// <param name="timeout"></param>
			/// <param name="cancellationToken"></param>
			/// <param name="credentials"></param>
			/// <exception cref="InvalidOperationException">Thrown when the <see cref="WebClient" /> is busy.</exception>
			protected UnderlyingDownloader(
				Uri source,
				Document destination,
				Boolean waitIfBusy,
				TimeSpan timeout,
				 CancellationToken cancellationToken,
				ICredentials? credentials = null
			) {
				var web = WebClients.Value;

				if ( web.IsBusy ) {
					if ( waitIfBusy ) {
						this.Wait( timeout, cancellationToken );
					}
					else {
						throw new InvalidOperationException( $"WebClient is already being used. Unable to download \"{this.Source}\"." );
					}
				}

				this.Client = web;
				this.Source = source ?? throw new ArgumentEmptyException( nameof( source ) );
				this.DestinationDocument = destination ?? throw new ArgumentEmptyException( nameof( destination ) );
				this.Timeout = timeout;
				this.Id = Guid.NewGuid();
				this.Credentials = credentials;

				this.Client.Credentials = this.Credentials;
				this.Client.CachePolicy = DefaultCachePolicy;

				//TODO what???
				//this.DestinationBuffer = destination.AsBytes(cancellationToken).ToArrayAsync(cancellationToken); //can we do this??
			}

			public virtual Boolean Cancel() {
				try {
					this.Client.CancelAsync();
				}
				catch ( Exception exception ) {
					exception.Log();
				}

				return this.IsBusy();
			}

			public (Status responseCode, Int64 fileLength) GetContentLength() {
				if ( WebRequest.Create( this.Source ) is HttpWebRequest request ) {
					request.Method = "HEAD";

					using var response = request.GetResponse();

					return (Status.Success, response.ContentLength);
				}

				return (Status.Error, default( Int64 ));
			}

			/// <summary>Returns true if the web request is in progress.</summary>
			public Boolean IsBusy() => this.Client.IsBusy;

			public virtual Boolean Start() {
				this.WhenStarted = DateTime.UtcNow;
				this.Elasped = Stopwatch.StartNew();

				return true;
			}

			/// <summary>this blocks until <see cref="Downloaded" /> signals.</summary>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="forHowLong" /> must be -1 milliseconds or greater.</exception>
			/// <exception cref="ObjectDisposedException"></exception>
			/// <exception cref="AbandonedMutexException">An abandoned mutex often indicates a serious coding error.</exception>
			/// <exception cref="Exception"></exception>
			public Boolean Wait( TimeSpan forHowLong, CancellationToken cancellationToken ) {
				try {
					if ( forHowLong < Forever ) {
						forHowLong = Forever;
					}

					return this.Downloaded.Wait( forHowLong, cancellationToken );
				}
				catch ( Exception exception ) {
					switch ( exception ) {
						case ArgumentOutOfRangeException _: {
							return false;
						}

						case ObjectDisposedException _: {
							return false;
						}

						case AbandonedMutexException _: {
							return false;
						}

						case InvalidOperationException _: {
							return false;
						}

						default: {
							exception.Log();

							throw;
						}
					}
				}
			}
		}
	}
}