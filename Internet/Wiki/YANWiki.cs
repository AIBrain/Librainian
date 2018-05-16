// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "YANWiki.cs",
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
// "Librainian/Librainian/YANWiki.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Internet.Wiki {

    using System;
    using System.Xml;
    using Parsing;

    public class YanWiki {

        private static String BaseQuery => @"http://en.wiktionary.org/w/api.php?action=query&format=xml&prop=info&search=";

        private static XmlDocument BaseResponse => "<?xml version=\"1.0\" ?><api /> ".ToXmlDoc();

        public static Boolean DoesWikiRespond {
            get {
                var doc = Http.Get( BaseQuery ).ToXmlDoc();

                return BaseResponse.OuterXml.Equals( doc.OuterXml );
            }
        }
    }
}