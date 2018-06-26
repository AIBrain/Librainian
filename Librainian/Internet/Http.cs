// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Http.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "Http.cs" was last formatted by Protiguous on 2018/06/26 at 1:11 AM.

namespace Librainian.Internet {

	using System;
	using System.Collections;
	using System.IO;
	using System.Net;
	using System.Net.Cache;
	using System.Threading;
	using System.Windows.Forms;
	using JetBrains.Annotations;

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

		static Http() => Urls = new Hashtable( 100 );

		private static void GetAsynchCallback( [NotNull] IAsyncResult result ) {
			if ( !result.IsCompleted ) {
				return;
			}

			( result.AsyncState is HttpWebRequest ).BreakIfFalse(); //heh
			var request = ( HttpWebRequest ) result.AsyncState;

			var response = ( HttpWebResponse ) request.GetResponse();

			if ( response.StatusCode != HttpStatusCode.OK ) {
				return;
			}

			var tempresp = response.GetResponseStream();

			if ( tempresp is null ) {
				return;
			}

			var document = new StreamReader( tempresp ).ReadToEnd();

			if ( String.IsNullOrEmpty( document ) ) {
				return;
			}

			if ( !Urls.ContainsKey( request.RequestUri ) ) {
				Urls.Add( request.RequestUri, document );
			}
			else {
				Urls[ request.RequestUri ] = document;
			}

			if ( response.ResponseUri.AbsoluteUri.Equals( request.RequestUri.AbsoluteUri ) ) {
				return;
			}

			if ( !Urls.ContainsKey( response.ResponseUri ) ) {
				Urls.Add( response.ResponseUri, document );
			}
			else {
				Urls[ response.ResponseUri ] = document;
			}
		}

		public static String Get( [NotNull] String url ) => Get( new Uri( url ) );

		[CanBeNull]
		public static String Get( [NotNull] Uri uri ) {
			uri.IsWellFormedOriginalString().BreakIfFalse();

			if ( !uri.IsWellFormedOriginalString() ) {
				return null;
			}

			var peek = Peek( uri ); //Got the result in our cache already?

			if ( !String.IsNullOrEmpty( peek ) ) {

				//yes?
				GetAsync( uri ); //start a refresh

				return peek; //but return what we have already.
			}

			var request = ( HttpWebRequest ) WebRequest.Create( uri );
			request.AllowAutoRedirect = true;
			request.UserAgent = "AIBrain Engine v2010.04";
			request.CachePolicy = new RequestCachePolicy( RequestCacheLevel.Default );
			request.KeepAlive = true;
			request.SendChunked = true;

			if ( request.GetResponse() is HttpWebResponse response && response.StatusCode == HttpStatusCode.OK ) {
				var respstrm = response.GetResponseStream();

				if ( respstrm != null ) {
					var document = new StreamReader( respstrm ).ReadToEnd();

					if ( Urls.ContainsKey( request.RequestUri ) ) {
						Urls[ request.RequestUri ] = document;
					}
					else {
						Urls.Add( request.RequestUri, document );
					}

					if ( !response.ResponseUri.AbsoluteUri.Equals( request.RequestUri.AbsoluteUri ) ) {
						if ( Urls.ContainsKey( response.ResponseUri ) ) {
							Urls[ response.ResponseUri ] = document;
						}
						else {
							Urls.Add( response.ResponseUri, document );
						}
					}

					return document;
				}

				return String.Empty;
			}

			return String.Empty;
		}

		[CanBeNull]
		public static IAsyncResult GetAsync( [NotNull] String url ) => GetAsync( new Uri( url ) );

		[CanBeNull]
		public static IAsyncResult GetAsync( [NotNull] Uri uri ) {
			uri.IsWellFormedOriginalString().BreakIfFalse();

			if ( !uri.IsWellFormedOriginalString() ) {
				return null;
			}

			if ( WebRequest.Create( uri ) is HttpWebRequest request ) {
				request.AllowAutoRedirect = true;
				request.UserAgent = "AIBrain Engine";
				request.CachePolicy = new RequestCachePolicy( RequestCacheLevel.Default );
				request.KeepAlive = true;
				request.SendChunked = true;

				return request.BeginGetResponse( GetAsynchCallback, request );
			}

			return null;
		}

		/// <summary>
		///     From: http://www.albahari.com/threading/part3.aspx#_Asynch_Delegates Use: DownloadString
		///     http1 = new WebClient().DownloadString; IAsyncResult cookie1 =
		///     download1.BeginInvoke( uri, null, null); ... String s1 = download1.EndInvoke(
		///     cookie1 );
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		[CanBeNull]
		public static IAsyncResult GetStart( [NotNull] Uri uri ) {
			uri.IsWellFormedOriginalString().BreakIfFalse();

			if ( !uri.IsWellFormedOriginalString() ) {
				return null;
			}

			//TODO
			//DownloadString http1 = new WebClient().DownloadStringAsync;

			var request = ( HttpWebRequest ) WebRequest.Create( uri );
			request.AllowAutoRedirect = true;
			request.UserAgent = "AIBrain Engine v" + DateTime.Now.Year + "." + DateTime.Now.Month;
			request.CachePolicy = new RequestCachePolicy( RequestCacheLevel.Default );
			request.KeepAlive = true;
			request.SendChunked = true;

			return request.BeginGetResponse( GetAsynchCallback, request );
		}

		/// <summary>
		///     Returns the document for the address specified or String.Empty if nothing has been
		///     captured yet.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static String Peek( [NotNull] String url ) => Peek( new Uri( url ) );

		[NotNull]
		public static String Peek( [NotNull] Uri uri ) {
			if ( !uri.IsWellFormedOriginalString() ) {
				return String.Empty;
			}

			if ( !Urls.ContainsKey( uri ) ) {
				return String.Empty;
			}

			var document = Urls[ uri ].ToString();

			return document;
		}

		/// <summary>
		///     Starts an asynchronous http request. It can be checked by Peek( url ) Each Poke starts
		///     another request. The order of responses is undeterminitic (can be out-of-order). This is
		///     by design.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		[CanBeNull]
		public static IAsyncResult Poke( [NotNull] String url ) {
			var uri = new Uri( url );

			if ( !uri.IsWellFormedOriginalString() ) {
				return null;
			}

			if ( Urls.ContainsKey( uri ) ) {
				Urls.Remove( uri );
			}

			return GetAsync( url );
		}

		/// <summary>
		///     Pump messages while waiting forever for a response to be populated for this url.
		/// </summary>
		/// <param name="url"></param>
		public static void Wait( [NotNull] String url ) => Wait( new Uri( url ) );

		/// <summary>
		///     Pump messages while waiting forever for a response to be populated for this uri.
		/// </summary>
		/// <param name="uri"></param>
		public static void Wait( [NotNull] Uri uri ) {
			uri.IsWellFormedOriginalString().BreakIfFalse();

			if ( !uri.IsWellFormedOriginalString() ) {
				return;
			}

			while ( String.IsNullOrEmpty( Peek( uri ) ) ) {
				Thread.Yield();
				Application.DoEvents();
			}
		}
	}
}