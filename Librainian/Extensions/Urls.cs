// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Urls.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Urls.cs" was last formatted by Protiguous on 2019/08/08 at 7:22 AM.

namespace Librainian.Extensions {

    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using JetBrains.Annotations;

    public static class Urls {

        public static String GetSuggestedNameFromUrl( String url, String defaultValue ) {
            var res = Path.GetFileNameWithoutExtension( url );

            //check if there is no file name, i.e. just folder name + query String
            if ( !String.IsNullOrEmpty( res ) && !res.IsNameOnlyQueryString() ) {
                return defaultValue;
            }

            res = Path.GetFileName( Path.GetDirectoryName( url ) );

            return String.IsNullOrEmpty( res ) ? defaultValue : Regex.Replace( res, @"[^\w]", "_", RegexOptions.Singleline ).Substring( 0, 50 );
        }

        /// <summary>
        ///     Check that a String is not null or empty
        /// </summary>
        /// <param name="input">String to check</param>
        /// <returns>Boolean</returns>
        public static Boolean HasValue( [CanBeNull] this String input ) => !String.IsNullOrEmpty( input );

        [CanBeNull]
        public static String HtmlAttributeEncode( [NotNull] this String input ) => HttpUtility.HtmlAttributeEncode( input );

        [CanBeNull]
        public static String HtmlDecode( this String input ) => HttpUtility.HtmlDecode( input );

        [CanBeNull]
        public static String HtmlEncode( [NotNull] this String input ) => HttpUtility.HtmlEncode( input );

        public static Boolean IsNameOnlyQueryString( [CanBeNull] this String res ) => !String.IsNullOrEmpty( res ) && res[ 0 ] == '?';

        [CanBeNull]
        public static String UrlDecode( this String input ) => HttpUtility.UrlDecode( input );

        /// <summary>
        ///     Uses Uri.EscapeDataString() based on recommendations on MSDN http:
        ///     //blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
        /// </summary>
        [NotNull]
        public static String UrlEncode( [NotNull] this String input ) {
            if ( input is null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            const Int32 maxLength = 32766;

            if ( input.Length <= maxLength ) {
                return Uri.EscapeDataString( input );
            }

            var sb = new StringBuilder( input.Length * 2 );
            var index = 0;

            while ( index < input.Length ) {
                var length = Math.Min( input.Length - index, maxLength );
                var subString = input.Substring( index, length );
                sb.Append( Uri.EscapeDataString( subString ) );
                index += subString.Length;
            }

            return sb.ToString();
        }
    }
}