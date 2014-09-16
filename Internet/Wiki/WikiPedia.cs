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
// "Librainian/WikiPedia.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Internet.Wiki {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Xml;
    using System.Xml.Linq;
    using Parsing;

    public class WikiPedia {
        private static XmlDocument BaseResponse { get { return "<?xml version=\"1.0\" ?><api /> ".ToXmlDoc(); } }

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

        //        http://en.wiktionary.org/w/api.php?action=query&format=xml&prop=info&list=search&titles=cat

        //        http://en.wiktionary.org/w/api.php?action=query&format=xml&prop=extlinks&titles=cat
        //        http://en.wiktionary.org/w/api.php?action=query&format=xml&prop=templates&titles=cat

        //        http://wikipedia.org/w/api.php?action=query&prop=revisions&titles=cat&rvprop=content&format=xml

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

        //        var webResponse = ( HttpWebResponse )webRequest.GetResponse();
        //        var responseStream = webResponse.GetResponseStream();
        //        var xmlreader = new XmlTextReader( responseStream );
        //        var xpathdoc = new XPathDocument( xmlreader );
        //        xmlreader.Close();
        //        webResponse.Close();
        //        var myXPathNavigator = xpathdoc.CreateNavigator();
        //        var nodesIt = myXPathNavigator.SelectDescendants( "text", "http://www.mediawiki.org/xml/export-0.4/", false );

        //        var rtWikiContent = new StringBuilder();

        //        while ( nodesIt.MoveNext() ) {
        //            rtWikiContent.Append( nodesIt.Current.InnerXml );
        //        }

        //        return rtWikiContent.ToString();
        //    }
        //    catch ( Exception ) {
        //        "Error while retrieve from Wikipedia. ".TimeDebug();
        //        return String.Empty;
        //    }
        //}

        public static IEnumerable< String > GetCategories( String titles ) {
            try {
                var uri = new Uri( String.Format( "http://wikipedia.org/w/api.php?action=query&prop=categories&format=xml&titles={0}", titles ) );
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
                    var results = new HashSet< String >();
                    foreach ( var xElement in cats ) {
                        results.Add( xElement.Value );
                    }
                    return results;

                    //using ( var reader = new XmlTextReader( responseStream ) ) {
                    //    var ds = new DataSet();
                    //    ds.ReadXml( reader );
                    //    return ds;
                    //}
                }
            }
            catch ( Exception Exception ) {
                Exception.Error();
                return null;
            }
        }

        public static XElement GetWikiData( String title ) {
            try {
                var webRequest = ( HttpWebRequest ) WebRequest.Create( "http://wikipedia.org/wiki/" + "Special:Export/" + title );
                webRequest.Credentials = CredentialCache.DefaultCredentials;
                webRequest.AllowAutoRedirect = true;
                webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)";
                webRequest.AutomaticDecompression = DecompressionMethods.GZip;
                webRequest.Accept = "text/xml";
                using ( var webResponse = webRequest.GetResponse() as HttpWebResponse ) {
                    if ( null == webResponse ) {
                        return null;
                    }

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
            catch ( Exception Exception ) {
                Exception.Error();
                return null;
            }
        }
    }
}
