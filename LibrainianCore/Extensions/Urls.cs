// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Urls.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "LibrainianCore", File: "Urls.cs" was last formatted by Protiguous on 2020/03/16 at 3:04 PM.

namespace Librainian.Extensions {

    using System;
    using System.Text;
    using System.Web;
    using JetBrains.Annotations;

    public static class Urls {

        /// <summary>Check that a String is not null or empty</summary>
        /// <param name="input">String to check</param>
        /// <returns>Boolean</returns>
        public static Boolean HasValue( [CanBeNull] this String input ) => !String.IsNullOrEmpty( value: input );

        [CanBeNull]
        public static String HtmlAttributeEncode( [NotNull] this String input ) => HttpUtility.HtmlAttributeEncode( s: input );

        [CanBeNull]
        public static String HtmlDecode( [CanBeNull] this String input ) => HttpUtility.HtmlDecode( s: input );

        [CanBeNull]
        public static String HtmlEncode( [NotNull] this String input ) => HttpUtility.HtmlEncode( s: input );

        public static Boolean IsNameOnlyQueryString( [CanBeNull] this String res ) => !String.IsNullOrEmpty( value: res ) && res[ index: 0 ] == '?';

        [CanBeNull]
        public static Uri UrlDecode( [NotNull] this String input ) {
            if ( String.IsNullOrWhiteSpace( value: input ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( input ) );
            }

            return new Uri( uriString: HttpUtility.UrlDecode( str: input ) );
        }

        /// <summary>
        /// Uses Uri.EscapeDataString() based on recommendations on MSDN http:
        /// //blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
        /// </summary>
        [NotNull]
        public static Uri UrlEncode( [NotNull] this String input ) {
            if ( input is null ) {
                throw new ArgumentNullException( paramName: nameof( input ) );
            }

            const Int32 maxLength = 32766;

            if ( input.Length <= maxLength ) {
                return new Uri( uriString: Uri.EscapeDataString( stringToEscape: input ) );
            }

            var sb = new StringBuilder( capacity: input.Length * 2 );
            var index = 0;

            while ( index < input.Length ) {
                var length = Math.Min( val1: input.Length - index, val2: maxLength );
                var subString = input.Substring( startIndex: index, length: length );
                sb.Append( value: Uri.EscapeDataString( stringToEscape: subString ) );
                index += subString.Length;
            }

            return new Uri( uriString: sb.ToString() );
        }
    }
}