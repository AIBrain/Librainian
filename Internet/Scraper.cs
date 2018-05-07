// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Scraper.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Cache;
    using System.Net.Security;
    using System.Threading;
    using Collections;
    using Newtonsoft.Json;
    using Parsing;

    [JsonObject]
    [Obsolete]
    public static class Scraper {

        [JsonProperty]
        private static readonly CookieContainer Cookies = new CookieContainer();

        [JsonProperty]
        private static readonly ReaderWriterLockSlim MAccess = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        /// <summary>TODO: concurrentbag</summary>
        [JsonProperty]
        private static readonly List<WebSite> MWebsites = new List<WebSite>();

        public static List<WebSite> ScrapedSites {
            get {
                try {
                    MAccess.EnterReadLock();
                    return MWebsites.Where( w => w.ResponseCount > 0 ) as List<WebSite>;
                }
                finally {
                    MAccess.ExitReadLock();
                }
            }
        }

        public static void AddSiteToScrape( String url, Action<WebSite> responseaction ) {
            try {
				if ( Uri.TryCreate( url, UriKind.RelativeOrAbsolute, out var uri ) ) {
					AddSiteToScrape( uri, responseaction );
				}
			}
            catch ( Exception exception ) {
                exception.More();
            }
        }

        public static void AddSiteToScrape( Uri uri, Action<WebSite> responseaction ) {
            if ( !IsSiteQueued( uri ) ) {
                var web = new WebSite {
                    Location = uri,
                    Document = String.Empty,
                    RequestCount = 0,
                    ResponseCount = 0,
                    WhenAddedToQueue = DateTime.UtcNow,
                    WhenRequestStarted = DateTime.MinValue,
                    WhenResponseCame = DateTime.MinValue

                    //ResponseAction = responseaction
                };
                try {
                    MAccess.EnterWriteLock();
                    MWebsites.Add( web );
                }
                finally {
                    MAccess.ExitWriteLock();
                }
            }
            else {
                try {
                    MAccess.EnterWriteLock();
                    MWebsites.Where( w => w.Location.Equals( uri ) ).ForEach( r => r.RequestCount++ );
                }
                finally {
                    MAccess.ExitWriteLock();
                }
            }

            StartNextScrape();
        }

        public static Boolean IsSiteQueued( Uri uri ) {
            try {
                MAccess.EnterReadLock();
                return MWebsites.Exists( w => w.Location.Equals( uri ) );
            }
            finally {
                MAccess.ExitReadLock();
            }
        }

        private static WebSite GetNextToScrape() {
            try {
                MAccess.EnterReadLock();
                return MWebsites.FirstOrDefault( w => w.WhenRequestStarted.Equals( DateTime.MinValue ) );
            }
            finally {
                MAccess.ExitReadLock();
            }
        }

        private static void RespCallback( IAsyncResult asynchronousResult ) {
            try {
                if ( asynchronousResult.AsyncState is WebSite web ) {
                    var response = web.Request.EndGetResponse( asynchronousResult );
                    var document = response.StringFromResponse();

                    Debug.WriteLineIf( response.IsFromCache, $"from cache {web.Location}" );

                    MAccess.EnterWriteLock();
                    web.ResponseCount++;
                    web.WhenResponseCame = DateTime.UtcNow;
                    web.Document = document;
                    if ( !web.Location.Equals( response.ResponseUri ) ) {
                        web.Location = response.ResponseUri;

                        //AddSiteToScrape( response.ResponseUri, web.ResponseAction );
                    }

                    MAccess.ExitWriteLock();
                }

                //TODO
                //if ( web.ResponseAction is Action<WebSite> ) {
                //    web.ResponseAction.FiredAndForgotten( web );
                //    //web.ResponseAction( web );
                //}
            }
            catch ( WebException ) { }
            catch ( Exception exception ) {
                exception.More();
            }
        }

        private static void StartNextScrape() {
            try {
                var web = GetNextToScrape();
                if ( null == web ) {
                    return;
                }

                if ( null == web.Request ) {
                    try {
                        MAccess.EnterWriteLock();
                        web.Request = WebRequest.Create( web.Location ) as HttpWebRequest;
                        if ( web.Request != null ) {
                            web.Request.AllowAutoRedirect = true;
                            web.Request.AllowWriteStreamBuffering = true;
                            web.Request.AuthenticationLevel = AuthenticationLevel.None;
                            web.Request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                            web.Request.CachePolicy = new RequestCachePolicy( RequestCacheLevel.Default );
                            web.Request.CookieContainer = Cookies;
                            web.Request.KeepAlive = true;
                            web.Request.MaximumAutomaticRedirections = 300;
                            web.Request.Method = "GET";
                            web.Request.Pipelined = true;
                            web.Request.SendChunked = true;
                            var now = DateTime.Now;
                            web.Request.UserAgent = $"AIBrain/{now.Year}.{now.Month}.{now.Day}";
                        }
                        web.WhenRequestStarted = DateTime.UtcNow;
                    }
                    finally {
                        MAccess.ExitWriteLock();
                    }
                }
                web.Request?.BeginGetResponse( RespCallback, web );
            }
            catch ( Exception exception ) {
                exception.More();
            }
        }
    }
}