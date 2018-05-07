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
// "Librainian/Wiktionary.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Internet.Wiki {

    using System;
    using System.Xml;
    using Parsing;

    public class Wiktionary {
        private static DateTime _lastWikiResponse = DateTime.MinValue;

        static Wiktionary() {
            if ( DoesWikiRespond ) {

                //AIBrain.Brain.BlackBoxClass.Diagnostic( String.Format( "Wiktionary responded at {0}.", LastWikiResponse ) );
            }
        }

        /// <summary>Returns true if Wiki has responded within the past 15 minutes.</summary>
        public static Boolean DoesWikiRespond {
            get {
                if ( ( DateTime.UtcNow - _lastWikiResponse ).TotalMinutes <= 15 ) {
                    return true;
                }
                var response = Http.Get( String.Format( BaseQuery, "wiki" ) );
                if ( response.Contains( "Definition from Wiktionary" ) ) {
                    _lastWikiResponse = DateTime.UtcNow;
                    return true;
                }
                return false;
            }
        }

        /// <summary>Use String.Format to enter the search parameter.</summary>
        private static String BaseQuery => @"http://en.wiktionary.org/wiki/Special:Search?search={0}&go=Go";

        private static XmlDocument BaseXMLResponse => "<?xml version=\"1.0\" ?><api /> ".ToXmlDoc();

        /// <summary>Pull the HTML for the Wiktionary entry on the base word.</summary>
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
            _lastWikiResponse = DateTime.UtcNow;

            return wiki;
        }
    }
}