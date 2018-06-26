// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "WikiPedia.cs" belongs to Rick@AIBrain.org and
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "WikiPedia.cs" was last formatted by Protiguous on 2018/06/04 at 4:00 PM.

namespace Librainian.Internet.Wiki {

	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Xml;
	using System.Xml.Linq;
	using JetBrains.Annotations;
	using Parsing;

	public class WikiPedia {

		private static XmlDocument BaseResponse => "<?xml version=\"1.0\" ?><api /> ".ToXmlDoc();

		//public static Boolean doesWikiRespond {
		//    get {
		//        XmlDocument doc = AIBrain.Internet.Http.Get( BaseQuery ).ToXmlDoc();
		//        return BaseResponse.OuterXml.Equals( doc.OuterXml );
		//    }
		//}

		//private static String BaseQuery {
		//    get {
		//        return String.Empty;
		//        /*
		//        http://wikipedia.org/w/api.php?action=query&format=xml&prop=info&titles=cat

		// http: //en.wiktionary.org/w/api.php?action=query&format=xml&prop=info&list=search&titles=cat

		// http: //en.wiktionary.org/w/api.php?action=query&format=xml&prop=extlinks&titles=cat
		// http: //en.wiktionary.org/w/api.php?action=query&format=xml&prop=templates&titles=cat

		// http: //wikipedia.org/w/api.php?action=query&prop=revisions&titles=cat&rvprop=content&format=xml

		//        return @"http://wikipedia.org/w/api.php?action=query&format=xml&prop=info&titles=cat";
		//        //return @"http://wikipedia.org/w/api.php?action                =query&format=xml&list=categorymembers&cmlimit=500&cmtitle=cat
		//        //                                 api.php?action                =query&format=xml&prop=info&titles=
		//        */
		//    }
		//}

		///// <summary>
		///// From http://www.fryan0911.com/2009/05/how-to-retrieve-content-from-wikipedia.html
		///// </summary>
		///// <param name="query"></param>
		//public static String Retrieve( String query ) {
		//    try {
		//        var uri = new Uri( String.Format( "http://wikipedia.org/wiki/Special:Export/{0}", query ) );
		//        var webRequest = ( HttpWebRequest )WebRequest.Create( uri );
		//        webRequest.Credentials = CredentialCache.DefaultCredentials;
		//        webRequest.AllowAutoRedirect = true;
		//        webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)";
		//        webRequest.Accept = "text/xml";

		// var webResponse = ( HttpWebResponse )webRequest.GetResponse(); var responseStream =
		// webResponse.GetResponseStream(); var xmlreader = new XmlTextReader( responseStream ); var
		// xpathdoc = new XPathDocument( xmlreader ); xmlreader.Close(); webResponse.Close(); var
		// myXPathNavigator = xpathdoc.CreateNavigator(); var nodesIt =
		// myXPathNavigator.SelectDescendants( "text", "http://www.mediawiki.org/xml/export-0.4/",
		// false );

		// var rtWikiContent = new StringBuilder();

		// while ( nodesIt.MoveNext() ) { rtWikiContent.Append( nodesIt.Current.InnerXml ); }

		//        return rtWikiContent.ToString();
		//    }
		//    catch ( Exception ) {
		//        "Error while retrieve from Wikipedia. ".TimeDebug();
		//        return String.Empty;
		//    }
		//}

		[CanBeNull]
		public static IEnumerable<String> GetCategories( String titles ) {
			try {
				var uri = new Uri( $"http://wikipedia.org/w/api.php?action=query&prop=categories&format=xml&titles={titles}" );
				var webRequest = ( HttpWebRequest ) WebRequest.Create( uri );
				webRequest.Credentials = CredentialCache.DefaultCredentials;
				webRequest.AllowAutoRedirect = true;
				webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)";
				webRequest.AutomaticDecompression = DecompressionMethods.GZip;
				webRequest.Accept = "text/xml";

				using ( var webResponse = ( HttpWebResponse ) webRequest.GetResponse() ) {

					//var responseStream = webResponse.GetResponseStream();
					var alltext = webResponse.StringFromResponse();
					var alldata = XElement.Parse( alltext, LoadOptions.None );
					var cats = alldata.Elements( "categories" );
					var results = new HashSet<String>();

					foreach ( var xElement in cats ) { results.Add( xElement.Value ); }

					return results;

					//using ( var reader = new XmlTextReader( responseStream ) ) {
					//    var ds = new DataSet();
					//    ds.ReadXml( reader );
					//    return ds;
					//}
				}
			}
			catch ( Exception exception ) {
				exception.More();

				return null;
			}
		}

		[CanBeNull]
		public static XElement GetWikiData( String title ) {
			try {
				var webRequest = ( HttpWebRequest ) WebRequest.Create( "http://wikipedia.org/wiki/" + "Special:Export/" + title );
				webRequest.Credentials = CredentialCache.DefaultCredentials;
				webRequest.AllowAutoRedirect = true;
				webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)";
				webRequest.AutomaticDecompression = DecompressionMethods.GZip;
				webRequest.Accept = "text/xml";

				using ( var webResponse = webRequest.GetResponse() as HttpWebResponse ) {
					if ( null == webResponse ) { return null; }

					//var responseStream = webResponse.GetResponseStream();
					var alltext = webResponse.StringFromResponse();
					var data = XElement.Parse( alltext, LoadOptions.None );

					return data;

					//using ( var reader = new XmlTextReader( responseStream ) ) {
					//    var ds = new DataSet();
					//    ds.ReadXml( reader );
					//    return ds;
					//}
				}
			}
			catch ( Exception exception ) {
				exception.More();

				return null;
			}
		}

	}

}