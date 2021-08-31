// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
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
// File "Surfer.cs" last touched on 2021-08-01 at 3:47 PM by Protiguous.

namespace Librainian.Internet {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Cache;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Threading.Tasks;
	using Logging;
	using Utilities.Disposables;

	public class Surfer : ABetterClassDispose {

		private readonly ReaderWriterLockSlim _downloadInProgressAccess = new( LockRecursionPolicy.SupportsRecursion );

		private readonly ConcurrentBag<Uri> _pastUrls = new();

		private readonly ConcurrentQueue<Uri> _urls = new();

		/// <remarks>Not thread safe.</remarks>
		private readonly WebClient _webclient;

		private Boolean _downloadInProgressStatus;

		/// <summary>Returns True if a download is currently in progress</summary>
		public Boolean DownloadInProgress {
			get {
				try {
					this._downloadInProgressAccess.EnterReadLock();

					return this._downloadInProgressStatus;
				}
				finally {
					this._downloadInProgressAccess.ExitReadLock();
				}
			}

			private set {
				try {
					this._downloadInProgressAccess.EnterWriteLock();
					this._downloadInProgressStatus = value;
				}
				finally {
					this._downloadInProgressAccess.ExitWriteLock();
				}
			}
		}

		public Surfer( Action<DownloadStringCompletedEventArgs>? onDownloadStringCompleted ) {
			this._webclient = new WebClient {
				CachePolicy = new RequestCachePolicy( RequestCacheLevel.Default )
			};

			if ( onDownloadStringCompleted != null ) {
				this._webclient.DownloadStringCompleted += ( sender, e ) => onDownloadStringCompleted( e );
			}
			else {
				this._webclient.DownloadStringCompleted += this.webclient_DownloadStringCompleted;
			}

			//System.Net.WebUtility
			//System.HttpStyleUriParser
			/*
            var urls = new[] { "http://www.google.com", "http://www.yahoo.com" };

            Task.Factory.ContinueWhenAll(
                urls.Select( url => Task.Run( u => {
                    using ( var client = new WebClient() ) {
                        return client.DownloadString( ( String )u );
                    }
                }, url ) ).ToArray(), tasks => {
                    var results = tasks.Select( t => t.Result );
                    foreach ( var html in results ) {
                        Console.WriteLine( html );
                    }
                } );
            */
		}

		private void StartNextDownload() =>
			Task.Run( () => {
				Thread.Yield();

				if ( this.DownloadInProgress ) {
					return;
				}

				if ( !this._urls.TryDequeue( out var address ) ) {
					return;
				}

				this.DownloadInProgress = true;
				$"Surf(): Starting download: {address.AbsoluteUri}".Info();
				this._webclient.DownloadStringAsync( address, address );
			} )
				.ContinueWith( t => {
					if ( this._urls.Any() ) {
						this.StartNextDownload();
					}
				} );

		internal void webclient_DownloadStringCompleted( Object? sender, DownloadStringCompletedEventArgs e ) {
			if ( e.UserState is Uri userState ) {
				$"Surf(): Download completed on {userState}".Info();
				this._pastUrls.Add( userState );
				this.DownloadInProgress = false;
			}

			this.StartNextDownload();
		}

		public static IEnumerable<UriLinkItem> ParseLinks( Uri? baseUri, String webpage ) {
			foreach ( Match match in Regex.Matches( webpage, @"(<a.*?>.*?</a>)", RegexOptions.Singleline ) ) {
				var value = match.Groups[1].Value;
				var m2 = Regex.Match( value, @"href=\""(.*?)\""", RegexOptions.Singleline );

				var i = new UriLinkItem( new Uri( baseUri, m2.Success ? m2.Groups[1].Value : String.Empty ),
					Regex.Replace( value, @"\s*<.*?>\s*", "", RegexOptions.Singleline ) );

				yield return i;
			}
		}

		/// <summary>Dispose any disposable members.</summary>
		public override void DisposeManaged() => this._downloadInProgressAccess.Dispose();

		/// <summary>Returns True if the address was successfully added to the queue to be downloaded.</summary>
		/// <param name="address"></param>
		public Boolean Surf( String? address ) {
			if ( String.IsNullOrWhiteSpace( address ) ) {
				return false;
			}

			try {
				var uri = new Uri( address );

				return this.Surf( uri );
			}
			catch ( UriFormatException ) {
				$"Surf(): Unable to parse address {address}".Info();

				return false;
			}
		}

		/// <summary>Returns True if the address was successfully added to the queue to be downloaded.</summary>
		/// <param name="address"></param>
		public Boolean Surf( Uri? address ) {
			if ( address is null ) {
				return false;
			}

			try {
				if ( this._pastUrls.Contains( address ) ) {
					return false;
				}

				this._urls.Enqueue( address );

				return true;
			}
			finally {
				this.StartNextDownload();
			}
		}
	}
}