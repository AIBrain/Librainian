#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Surfer.cs" was last cleaned by Rick on 2014/08/19 at 1:27 PM
#endregion

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
    using Threading;

    public class Surfer {
        private readonly ConcurrentBag< Uri > PastUrls = new ConcurrentBag< Uri >();

        private readonly ConcurrentQueue< Uri > Urls = new ConcurrentQueue< Uri >();

        private readonly ReaderWriterLockSlim _DownloadInProgressAccess = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        /// <remarks>
        ///     Not thread safe.
        /// </remarks>
        private readonly WebClient webclient;

        private Boolean _DownloadInProgressStatus;

        public Surfer( Action< DownloadStringCompletedEventArgs > onDownloadStringCompleted ) {
            this.webclient = new WebClient {
                                               CachePolicy = new RequestCachePolicy( RequestCacheLevel.Default )
                                           };

            if ( null != onDownloadStringCompleted ) {
                this.webclient.DownloadStringCompleted += ( sender, e ) => onDownloadStringCompleted( e );
            }
            else {
                this.webclient.DownloadStringCompleted += this.webclient_DownloadStringCompleted;
            }

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
                    this._DownloadInProgressAccess.EnterReadLock();
                    return this._DownloadInProgressStatus;
                }
                finally {
                    this._DownloadInProgressAccess.ExitReadLock();
                }
            }
            private set {
                try {
                    this._DownloadInProgressAccess.EnterWriteLock();
                    this._DownloadInProgressStatus = value;
                }
                finally {
                    this._DownloadInProgressAccess.ExitWriteLock();
                }
            }
        }

        /// <summary>
        ///     Returns True if the address was successfully added to the queue to be downloaded.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Boolean Surf( String address ) {
            if ( String.IsNullOrWhiteSpace( value: address ) ) {
                return false;
            }
            try {
                var uri = new Uri( uriString: address );
                return this.Surf( address: uri );
            }
            catch ( UriFormatException ) {
                String.Format( format: "Surf(): Unable to parse address {0}", arg0: address ).TimeDebug();
                return false;
            }
        }

        /// <summary>
        ///     Returns True if the address was successfully added to the queue to be downloaded.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Boolean Surf( Uri address ) {
            if ( null == address ) {
                return false;
            }
            try {
                if ( this.PastUrls.Contains( address ) ) {
                    return false;
                }
                this.Urls.Enqueue( item: address );
                return true;
            }
            finally {
                this.StartNextDownload();
            }
        }

        private void StartNextDownload() {
            Task.Factory.StartNew( () => {
                                       Thread.Yield();
                                       if ( this.DownloadInProgress ) {
                                           return;
                                       }
                                       Uri address;
                                       if ( !this.Urls.TryDequeue( result: out address ) ) {
                                           return;
                                       }

                                       this.DownloadInProgress = true;
                                       String.Format( "Surf(): Starting download: {0}", address.AbsoluteUri ).TimeDebug();
                                       this.webclient.DownloadStringAsync( address: address, userToken: address );
                                   } ).ContinueWith( t => {
                                                         if ( this.Urls.Any() ) {
                                                             this.StartNextDownload();
                                                         }
                                                     } );
        }

        internal void webclient_DownloadStringCompleted( object sender, DownloadStringCompletedEventArgs e ) {
            if ( e.UserState is Uri ) {
                String.Format( format: "Surf(): Download completed on {0}", arg0: e.UserState as Uri ).TimeDebug();
                this.PastUrls.Add( e.UserState as Uri );
                this.DownloadInProgress = false;
            }
            this.StartNextDownload();
        }

        public static IEnumerable< LinkItem > ParseLinks( Uri baseUri, String webpage ) {
// ReSharper disable LoopCanBeConvertedToQuery
            foreach ( Match match in Regex.Matches( webpage, @"(<a.*?>.*?</a>)", RegexOptions.Singleline ) ) {
// ReSharper restore LoopCanBeConvertedToQuery
                var value = match.Groups[ 1 ].Value;
                var m2 = Regex.Match( value, @"href=\""(.*?)\""", RegexOptions.Singleline );

                var i = new LinkItem {
                                         Text = Regex.Replace( value, @"\s*<.*?>\s*", "", RegexOptions.Singleline ),
                                         Href = new Uri( baseUri: baseUri, relativeUri: m2.Success ? m2.Groups[ 1 ].Value : String.Empty )
                                     };

                yield return i;
            }
        }

        public struct LinkItem {
            public Uri Href;

            public String Text;

            public override String ToString() {
                return String.Format( "{0}  {1}", this.Text, this.Href );
            }
        }
    }
}
