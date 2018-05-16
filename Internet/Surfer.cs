// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Surfer.cs",
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
// "Librainian/Librainian/Surfer.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

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
    using Magic;

    public class Surfer : ABetterClassDispose {

        private readonly ReaderWriterLockSlim _downloadInProgressAccess = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        private readonly ConcurrentBag<Uri> _pastUrls = new ConcurrentBag<Uri>();

        private readonly ConcurrentQueue<Uri> _urls = new ConcurrentQueue<Uri>();

        /// <remarks>Not thread safe.</remarks>
        private readonly WebClient _webclient;

        private Boolean _downloadInProgressStatus;

        public Surfer( Action<DownloadStringCompletedEventArgs> onDownloadStringCompleted ) {
            this._webclient = new WebClient { CachePolicy = new RequestCachePolicy( RequestCacheLevel.Default ) };

            if ( null != onDownloadStringCompleted ) { this._webclient.DownloadStringCompleted += ( sender, e ) => onDownloadStringCompleted( e ); }
            else { this._webclient.DownloadStringCompleted += this.webclient_DownloadStringCompleted; }

            //System.Net.WebUtility
            //System.HttpStyleUriParser
            /*
            var urls = new[] { "http://www.google.com", "http://www.yahoo.com" };

            Task.Factory.ContinueWhenAll(
                urls.Select( url => Task.Factory.StartNew( u => {
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

        /// <summary>
        ///     Returns True if a download is currently in progress
        /// </summary>
        public Boolean DownloadInProgress {
            get {
                try {
                    this._downloadInProgressAccess.EnterReadLock();

                    return this._downloadInProgressStatus;
                }
                finally { this._downloadInProgressAccess.ExitReadLock(); }
            }

            private set {
                try {
                    this._downloadInProgressAccess.EnterWriteLock();
                    this._downloadInProgressStatus = value;
                }
                finally { this._downloadInProgressAccess.ExitWriteLock(); }
            }
        }

        public static IEnumerable<UriLinkItem> ParseLinks( Uri baseUri, String webpage ) {

            // ReSharper disable LoopCanBeConvertedToQuery
#pragma warning disable IDE0007 // Use implicit type
            foreach ( Match match in Regex.Matches( webpage, @"(<a.*?>.*?</a>)", RegexOptions.Singleline ) ) {
#pragma warning restore IDE0007 // Use implicit type

                // ReSharper restore LoopCanBeConvertedToQuery
                var value = match.Groups[1].Value;
                var m2 = Regex.Match( value, @"href=\""(.*?)\""", RegexOptions.Singleline );

                var i = new UriLinkItem { Text = Regex.Replace( value, @"\s*<.*?>\s*", "", RegexOptions.Singleline ), Href = new Uri( baseUri: baseUri, relativeUri: m2.Success ? m2.Groups[1].Value : String.Empty ) };

                yield return i;
            }
        }

        /// <summary>
        ///     Returns True if the address was successfully added to the queue to be downloaded.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Boolean Surf( String address ) {
            if ( String.IsNullOrWhiteSpace( address ) ) { return false; }

            try {
                var uri = new Uri( uriString: address );

                return this.Surf( address: uri );
            }
            catch ( UriFormatException ) {
                String.Format( format: "Surf(): Unable to parse address {0}", arg0: address ).WriteLine();

                return false;
            }
        }

        /// <summary>
        ///     Returns True if the address was successfully added to the queue to be downloaded.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Boolean Surf( Uri address ) {
            if ( null == address ) { return false; }

            try {
                if ( this._pastUrls.Contains( address ) ) { return false; }

                this._urls.Enqueue( item: address );

                return true;
            }
            finally { this.StartNextDownload(); }
        }

        internal void webclient_DownloadStringCompleted( Object sender, DownloadStringCompletedEventArgs e ) {
            if ( e.UserState is Uri userState ) {
                String.Format( format: "Surf(): Download completed on {0}", arg0: userState ).WriteLine();
                this._pastUrls.Add( userState );
                this.DownloadInProgress = false;
            }

            this.StartNextDownload();
        }

        private void StartNextDownload() =>
            Task.Factory.StartNew( () => {
                Thread.Yield();

                if ( this.DownloadInProgress ) { return; }

                if ( !this._urls.TryDequeue( result: out var address ) ) { return; }

                this.DownloadInProgress = true;
                $"Surf(): Starting download: {address.AbsoluteUri}".WriteLine();
                this._webclient.DownloadStringAsync( address: address, userToken: address );
            } ).ContinueWith( t => {
                if ( this._urls.Any() ) { this.StartNextDownload(); }
            } );

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this._downloadInProgressAccess.Dispose();
    }
}