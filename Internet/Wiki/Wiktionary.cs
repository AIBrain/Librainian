// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Wiktionary.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Wiktionary.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Internet.Wiki {

    using System;
    using System.Xml;
    using Parsing;

    public class Wiktionary {

        private static DateTime _lastWikiResponse = DateTime.MinValue;

        /// <summary>Use String.Format to enter the search parameter.</summary>
        private static String BaseQuery => @"http://en.wiktionary.org/wiki/Special:Search?search={0}&go=Go";

        private static XmlDocument BaseXMLResponse => "<?xml version=\"1.0\" ?><api /> ".ToXmlDoc();

        /// <summary>Returns true if Wiki has responded within the past 15 minutes.</summary>
        public static Boolean DoesWikiRespond {
            get {
                if ( ( DateTime.UtcNow - _lastWikiResponse ).TotalMinutes <= 15 ) { return true; }

                var response = Http.Get( String.Format( BaseQuery, "wiki" ) );

                if ( response.Contains( "Definition from Wiktionary" ) ) {
                    _lastWikiResponse = DateTime.UtcNow;

                    return true;
                }

                return false;
            }
        }

        static Wiktionary() {
            if ( DoesWikiRespond ) {

                //AIBrain.Brain.BlackBoxClass.Diagnostic( String.Format( "Wiktionary responded at {0}.", LastWikiResponse ) );
            }
        }

        /// <summary>Pull the HTML for the Wiktionary entry on the base word.</summary>
        /// <param name="baseWord"></param>
        /// <returns></returns>
        public static String Wiki( String baseWord ) {
            if ( String.IsNullOrEmpty( baseWord ) ) { return String.Empty; }

            if ( !DoesWikiRespond ) { return String.Empty; }

            var wiki = Http.Get( String.Format( BaseQuery, baseWord ) );

            if ( !wiki.Contains( "Definition from Wiktionary" ) ) { return String.Empty; }

            _lastWikiResponse = DateTime.UtcNow;

            return wiki;
        }
    }
}