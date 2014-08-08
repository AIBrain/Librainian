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
// "Librainian2/Wiktionary.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Internet.Wiki {
    using System;
    using System.Xml;
    using Parsing;

    public class Wiktionary {
        private static DateTime lastWikiResponse = DateTime.MinValue;

        static Wiktionary() {
            if ( DoesWikiRespond ) {
                //AIBrain.Brain.BlackBoxClass.Diagnostic( String.Format( "Wiktionary responded at {0}.", LastWikiResponse ) );
            }
        }

        /// <summary>
        ///     Returns true if Wiki has responded within the past 15 minutes.
        /// </summary>
        public static Boolean DoesWikiRespond {
            get {
                if ( ( DateTime.UtcNow - lastWikiResponse ).TotalMinutes <= 15 ) {
                    return true;
                }
                var Response = Http.Get( String.Format( BaseQuery, "wiki" ) );
                if ( Response.Contains( "Definition from Wiktionary" ) ) {
                    lastWikiResponse = DateTime.UtcNow;
                    return true;
                }
                return false;
            }
        }

        private static XmlDocument BaseXMLResponse { get { return "<?xml version=\"1.0\" ?><api /> ".ToXmlDoc(); } }

        /// <summary>
        ///     Use String.Format to enter the search parameter.
        /// </summary>
        private static String BaseQuery { get { return @"http://en.wiktionary.org/wiki/Special:Search?search={0}&go=Go"; } }

        /// <summary>
        ///     Pull the HTML for the Wiktionary entry on the base word.
        /// </summary>
        /// <param name="baseWord"></param>
        /// <returns></returns>
        public static String Wiki( String baseWord ) {
            if ( String.IsNullOrEmpty( baseWord ) ) {
                return String.Empty;
            }
            if ( !DoesWikiRespond ) {
                return String.Empty;
            }

            var wiki = Http.Get( String.Format( BaseQuery, baseWord ) );
            if ( !wiki.Contains( "Definition from Wiktionary" ) ) {
                return String.Empty;
            }
            lastWikiResponse = DateTime.UtcNow;

            return wiki;
        }

        /*
        public static String Lookup( String BaseWord ) {
            var wiki = Wiki( BaseWord );
            if ( String.IsNullOrEmpty( wiki ) ) {
                return String.Empty;
            }

            return String.Empty;
        }
        */
    }
}
