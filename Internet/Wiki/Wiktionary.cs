// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Wiktionary.cs" belongs to Rick@AIBrain.org and
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
// File "Wiktionary.cs" was last formatted by Protiguous on 2018/06/04 at 4:00 PM.

namespace Librainian.Internet.Wiki {

	using System;
	using System.Xml;
	using Parsing;

	public class Wiktionary {

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

		/// <summary>Use String.Format to enter the search parameter.</summary>
		private static String BaseQuery => @"http://en.wiktionary.org/wiki/Special:Search?search={0}&go=Go";

		private static XmlDocument BaseXMLResponse => "<?xml version=\"1.0\" ?><api /> ".ToXmlDoc();

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

		private static DateTime _lastWikiResponse = DateTime.MinValue;

		static Wiktionary() {
			if ( DoesWikiRespond ) {

				//AIBrain.Brain.BlackBoxClass.Diagnostic( String.Format( "Wiktionary responded at {0}.", LastWikiResponse ) );
			}
		}

	}

}