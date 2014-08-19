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
// "Librainian/Http.cs" was last cleaned by Rick on 2014/08/19 at 1:27 PM
#endregion

namespace Librainian.Internet {
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Net.Cache;
    using System.Threading;
    using System.Windows.Forms;
    using Extensions;

    public class Http {
        /*
        public class HtmlDocument : Uri {
            public HtmlDocument( String url )
                : base( url ) {
            }

            private String _document = String.Empty;
            public String Document {
                get { return this._document; }
                set {
                    this._document = value;
                    this.LastGet = DateTime.UtcNow;
                }
            }

            public DateTime LastGet { get; set; }
        }
        */

        private static readonly Object Synch = new Object();

        private static Hashtable _urls;

        static Http() {
            Urls = new Hashtable( 100 );
        }

        private static Hashtable Urls {
            get {
                lock ( Synch ) {
                    return _urls;
                }
            }
            set {
                lock ( Synch ) {
                    _urls = value;
                }
            }
        }

        /// <summary>
        ///     Starts an asynchronous http request. It can be checked by Peek( url )
        ///     Each Poke starts another request.
        ///     The order of responses is undeterminitic (can be out-of-order).
        ///     This is by design.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IAsyncResult Poke( String url ) {
            var uri = new Uri( url );
            if ( !uri.IsWellFormedOriginalString() ) {
                return null;
            }
            if ( Urls.ContainsKey( uri ) ) {
                Urls.Remove( uri );
            }
            return GetAsynch( url );
        }

        /// <summary>
        ///     Returns the document for the address specified or String.Empty if nothing has been captured yet.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static String Peek( String url ) {
            return Peek( new Uri( url ) );
        }

        public static String Peek( Uri uri ) {
            if ( !uri.IsWellFormedOriginalString() ) {
                return String.Empty;
            }
            if ( !Urls.ContainsKey( uri ) ) {
                return String.Empty;
            }
            var Document = Urls[ uri ].ToString();
            return Document;
        }

        public static IAsyncResult GetAsynch( String url ) {
            return GetAsynch( new Uri( url ) );
        }

        public static IAsyncResult GetAsynch( Uri uri ) {
            uri.IsWellFormedOriginalString().DebugAssert();
            if ( !uri.IsWellFormedOriginalString() ) {
                return null;
            }

            var request = WebRequest.Create( uri ) as HttpWebRequest;
            request.AllowAutoRedirect = true;
            request.UserAgent = "AIBrain Engine";
            request.CachePolicy = new RequestCachePolicy( RequestCacheLevel.Default );
            request.KeepAlive = true;
            request.SendChunked = true;
            //AIBrain.Brain.BlackBoxClass.Diagnostic( String.Format( "Starting HTTP request for {0}", uri.AbsoluteUri ) );
            return request.BeginGetResponse( GetAsynchCallback, request );
        }

        private static void GetAsynchCallback( IAsyncResult result ) {
            if ( !result.IsCompleted ) {
                return;
            }

            ( result.AsyncState is HttpWebRequest ).DebugAssert(); //heh
            var Request = ( HttpWebRequest ) result.AsyncState;

            var Response = ( HttpWebResponse ) Request.GetResponse();
            if ( Response.StatusCode != HttpStatusCode.OK ) {
                //AIBrain.Brain.BlackBoxClass.Diagnostic( String.Format( "HTTP request returned StatusCode {1} for {0}", Request.RequestUri.AbsoluteUri, Response.StatusCode ) );
                return;
            }

            var tempresp = Response.GetResponseStream();
            if ( tempresp == null ) {
                return;
            }
            var Document = new StreamReader( tempresp ).ReadToEnd();
            if ( String.IsNullOrEmpty( Document ) ) {
                return;
            }
            //AIBrain.Brain.BlackBoxClass.Diagnostic( String.Format( "HttpManager.Response.Uri={0} Cached:{1} Length:{2}.", Response.ResponseUri.AbsoluteUri, Response.IsFromCache, Document.Length ) );

            if ( !Urls.ContainsKey( Request.RequestUri ) ) {
                Urls.Add( Request.RequestUri, Document );
            }
            else {
                Urls[ Request.RequestUri ] = Document;
            }

            if ( Response.ResponseUri.AbsoluteUri.Equals( Request.RequestUri.AbsoluteUri ) ) {
                return;
            }

            if ( !Urls.ContainsKey( Response.ResponseUri ) ) {
                Urls.Add( Response.ResponseUri, Document );
            }
            else {
                Urls[ Response.ResponseUri ] = Document;
            }
        }

        /// <summary>
        ///     From: http://www.albahari.com/threading/part3.aspx#_Asynch_Delegates
        ///     Use:
        ///     DownloadString http1 = new WebClient().DownloadString;
        ///     IAsyncResult cookie1 = download1.BeginInvoke( uri, null, null);
        ///     ...
        ///     String s1 = download1.EndInvoke( cookie1 );
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static IAsyncResult GetStart( Uri uri ) {
            uri.IsWellFormedOriginalString().DebugAssert();
            if ( !uri.IsWellFormedOriginalString() ) {
                return null;
            }

            //TODO
            //DownloadString http1 = new WebClient().DownloadStringAsync;

            var request = ( HttpWebRequest ) WebRequest.Create( uri );
            request.AllowAutoRedirect = true;
            request.UserAgent = "AIBrain Engine v2009.03";
            request.CachePolicy = new RequestCachePolicy( RequestCacheLevel.Default );
            request.KeepAlive = true;
            request.SendChunked = true;
            //AIBrain.Brain.BlackBoxClass.Diagnostic( String.Format( "Starting HTTP request for {0}", uri.AbsoluteUri ) );
            return request.BeginGetResponse( GetAsynchCallback, request );
        }

        /// <summary>
        ///     Pump messages while waiting forever for a response to be populated for this url.
        /// </summary>
        /// <param name="url"></param>
        public static void Wait( String url ) {
            Wait( new Uri( url ) );
        }

        /// <summary>
        ///     Pump messages while waiting forever for a response to be populated for this uri.
        /// </summary>
        /// <param name="uri"></param>
        public static void Wait( Uri uri ) {
            uri.IsWellFormedOriginalString().DebugAssert();
            if ( !uri.IsWellFormedOriginalString() ) {
                return;
            }

            while ( String.IsNullOrEmpty( Peek( uri ) ) ) {
                Thread.Yield();
                Application.DoEvents();
            }
        }

        public static String Get( String url ) {
            return Get( new Uri( url ) );
        }

        public static String Get( Uri uri ) {
            uri.IsWellFormedOriginalString().DebugAssert();
            if ( !uri.IsWellFormedOriginalString() ) {
                return null;
            }

            var peek = Peek( uri ); //Got the result in our cache already?
            if ( !String.IsNullOrEmpty( peek ) ) {
                //yes?
                GetAsynch( uri ); //start a refresh 
                return peek; //but return what we have already.
            }

            var Request = ( HttpWebRequest ) WebRequest.Create( uri );
            Request.AllowAutoRedirect = true;
            Request.UserAgent = "AIBrain Engine v2010.04";
            Request.CachePolicy = new RequestCachePolicy( RequestCacheLevel.Default );
            Request.KeepAlive = true;
            Request.SendChunked = true;

            var response = Request.GetResponse() as HttpWebResponse;
            if ( response != null && response.StatusCode == HttpStatusCode.OK ) {
                var respstrm = response.GetResponseStream();
                if ( respstrm != null ) {
                    var Document = new StreamReader( respstrm ).ReadToEnd();
                    //AIBrain.Brain.BlackBoxClass.Diagnostic( String.Format( "HttpManager.Response.Uri={0} Cached:{1} Length:{2}.", Response.ResponseUri.AbsoluteUri, Response.IsFromCache, Document.Length ) );

                    if ( Urls.ContainsKey( Request.RequestUri ) ) {
                        Urls[ Request.RequestUri ] = Document;
                    }
                    else {
                        Urls.Add( Request.RequestUri, Document );
                    }

                    if ( !response.ResponseUri.AbsoluteUri.Equals( Request.RequestUri.AbsoluteUri ) ) {
                        if ( Urls.ContainsKey( response.ResponseUri ) ) {
                            Urls[ response.ResponseUri ] = Document;
                        }
                        else {
                            Urls.Add( response.ResponseUri, Document );
                        }
                    }

                    return Document;
                }
                return String.Empty;
            }

            //AIBrain.Brain.BlackBoxClass.Diagnostic( String.Format( "HTTP request returned StatusCode {1} for {0}", Request.RequestUri.AbsoluteUri, Response.StatusCode ) );
            return String.Empty;
        }
    }
}
