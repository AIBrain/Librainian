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
// "Librainian2/Scraper.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Internet {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Cache;
    using System.Net.Security;
    using System.Runtime.Serialization;
    using System.Threading;
    using Parsing;
    using Threading;

    [DataContract]
    [Obsolete]
    public static class Scraper {
        [DataMember] [OptionalField] private static ReaderWriterLockSlim mAccess = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        /// <summary>
        ///     TODO: concurrentbag
        /// </summary>
        [DataMember] [OptionalField] private static List< WebSite > mWebsites = new List< WebSite >();

        [DataMember] [OptionalField] private static CookieContainer cookies = new CookieContainer();

        public static List< WebSite > ScrapedSites {
            get {
                try {
                    mAccess.EnterReadLock();
                    return mWebsites.Where( w => w.ResponseCount > 0 ) as List< WebSite >;
                }
                finally {
                    mAccess.ExitReadLock();
                }
            }
        }

        public static void AddSiteToScrape( String url, Action< WebSite > responseaction ) {
            try {
                Uri uri;
                if ( Uri.TryCreate( url, UriKind.RelativeOrAbsolute, out uri ) ) {
                    AddSiteToScrape( uri, responseaction );
                }
            }
            catch ( Exception Exception ) {
                Exception.Log();
            }
        }

        public static void AddSiteToScrape( Uri uri, Action< WebSite > responseaction ) {
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
                    mAccess.EnterWriteLock();
                    mWebsites.Add( web );
                }
                finally {
                    mAccess.ExitWriteLock();
                }
            }
            else {
                try {
                    mAccess.EnterWriteLock();
                    mWebsites.Where( w => w.Location.Equals( uri ) ).ForEach( r => { r.RequestCount++; } );
                }
                finally {
                    mAccess.ExitWriteLock();
                }
            }

            StartNextScrape();
        }

        public static Boolean IsSiteQueued( Uri uri ) {
            try {
                mAccess.EnterReadLock();
                return mWebsites.Exists( w => w.Location.Equals( uri ) );
            }
            finally {
                mAccess.ExitReadLock();
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
                        mAccess.EnterWriteLock();
                        web.Request = WebRequest.Create( web.Location ) as HttpWebRequest;
                        if ( web.Request != null ) {
                            web.Request.AllowAutoRedirect = true;
                            web.Request.AllowWriteStreamBuffering = true;
                            web.Request.AuthenticationLevel = AuthenticationLevel.None;
                            web.Request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                            web.Request.CachePolicy = new RequestCachePolicy( RequestCacheLevel.Default );
                            web.Request.CookieContainer = cookies;
                            web.Request.KeepAlive = true;
                            web.Request.MaximumAutomaticRedirections = 300;
                            web.Request.Method = "GET";
                            web.Request.Pipelined = true;
                            web.Request.SendChunked = true;
                            var now = DateTime.Now;
                            web.Request.UserAgent = String.Format( "AIBrain/{0}.{1}", now.Year, now.Month );
                        }
                        web.WhenRequestStarted = DateTime.UtcNow;
                    }
                    finally {
                        mAccess.ExitWriteLock();
                    }
                }
                if ( web.Request != null ) {
                    web.Request.BeginGetResponse( RespCallback, web );
                }
            }
            catch ( Exception Exception ) {
                Exception.Log();
            }
        }

        private static WebSite GetNextToScrape() {
            try {
                mAccess.EnterReadLock();
                return mWebsites.FirstOrDefault( w => w.WhenRequestStarted.Equals( DateTime.MinValue ) );
            }
            finally {
                mAccess.ExitReadLock();
            }
        }

        private static void RespCallback( IAsyncResult asynchronousResult ) {
            try {
                if ( !( asynchronousResult.AsyncState is WebSite ) ) {
                    return;
                }
                var web = asynchronousResult.AsyncState as WebSite;
                var response = web.Request.EndGetResponse( asynchronousResult );
                var document = response.StringFromResponse();

                Debug.WriteLineIf( response.IsFromCache, String.Format( "from cache {0}", web.Location ) );

                mAccess.EnterWriteLock();
                web.ResponseCount++;
                web.WhenResponseCame = DateTime.UtcNow;
                web.Document = document;
                if ( !web.Location.Equals( response.ResponseUri ) ) {
                    web.Location = response.ResponseUri;
                    //AddSiteToScrape( response.ResponseUri, web.ResponseAction );
                }
                mAccess.ExitWriteLock();

                //TODO
                //if ( web.ResponseAction is Action<WebSite> ) {
                //    web.ResponseAction.FiredAndForgotten( web );
                //    //web.ResponseAction( web );
                //}
            }
            catch ( WebException ) { }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

        //public static void ParseWikipedia( HTMLDocument document, Action<String> sentenceaction ) {
        //    try {
        //        if ( null == document ) {
        //            return;
        //        }
        //        if ( null == document.body ) {
        //            return;
        //        }
        //        if ( null == document.body.innerText ) {
        //            return;
        //        }

        //        var website = new Uri( document.url );

        //        //mshtml.IHTMLElement toctitle = document.getElementById( "toctitle" );
        //        var body = document.body.innerText;
        //        //body = body.ToEnglishFromHTML();
        //        body = body.ReplaceHTML( "\r\n" );
        //        foreach ( var sent in Sentence.Sentences( body ) ) {
        //            sentenceaction( sent );
        //        }

        //        //XmlDocument doc = new 
        //        //document.getElementsByTagName(

        //        //also have document.images

        //        /*
        //        List<Uri> links = new List<Uri>();
        //        foreach ( mshtml.HTMLAnchorElement link in document.links ) {
        //            Uri uri = new Uri( link.href );
        //            if ( uri.DnsSafeHost.Equals( website.DnsSafeHost ) && uri.AbsolutePath.StartsWith( website.AbsolutePath ) ) {
        //                System.Diagnostics.Debug.WriteLine( String.Format( "link: {0}", uri ) );
        //                links.Add( uri );
        //            }
        //            //Info( item.innerText );
        //        }
        //        */

        //        //foreach ( var word in Parsing.Sentence.ToWords( website.Document ) ) {           }
        //    }
        //    catch ( Exception ex ) {
        //        Utility.LogException( ex );
        //    }
        //}
    }
}
