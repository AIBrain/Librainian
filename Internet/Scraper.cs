// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Scraper.cs" belongs to Rick@AIBrain.org and
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Scraper.cs" was last formatted by Protiguous on 2018/05/24 at 7:15 PM.

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
                finally { MAccess.ExitReadLock(); }
            }
        }

        private static WebSite GetNextToScrape() {
            try {
                MAccess.EnterReadLock();

                return MWebsites.FirstOrDefault( w => w.WhenRequestStarted.Equals( DateTime.MinValue ) );
            }
            finally { MAccess.ExitReadLock(); }
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
            catch ( Exception exception ) { exception.More(); }
        }

        private static void StartNextScrape() {
            try {
                var web = GetNextToScrape();

                if ( null == web ) { return; }

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
                    finally { MAccess.ExitWriteLock(); }
                }

                web.Request?.BeginGetResponse( RespCallback, web );
            }
            catch ( Exception exception ) { exception.More(); }
        }

        public static void AddSiteToScrape( String url, Action<WebSite> responseaction ) {
            try {
                if ( Uri.TryCreate( url, UriKind.RelativeOrAbsolute, out var uri ) ) { AddSiteToScrape( uri, responseaction ); }
            }
            catch ( Exception exception ) { exception.More(); }
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
                finally { MAccess.ExitWriteLock(); }
            }
            else {
                try {
                    MAccess.EnterWriteLock();
                    MWebsites.Where( w => w.Location.Equals( uri ) ).ForEach( r => r.RequestCount++ );
                }
                finally { MAccess.ExitWriteLock(); }
            }

            StartNextScrape();
        }

        public static Boolean IsSiteQueued( Uri uri ) {
            try {
                MAccess.EnterReadLock();

                return MWebsites.Exists( w => w.Location.Equals( uri ) );
            }
            finally { MAccess.ExitReadLock(); }
        }
    }
}