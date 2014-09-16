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
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Urls.cs" was last cleaned by Rick on 2014/09/05 at 3:31 PM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

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

        public static Boolean IsNameOnlyQueryString( this String res ) {
            return !String.IsNullOrEmpty( res ) && res[ 0 ] == '?';
        }

        public static String UrlDecode( this String input ) {
            return HttpUtility.UrlDecode( input );
        }

        /// <summary>
        ///     Uses Uri.EscapeDataString() based on recommendations on MSDN
        ///     http://blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
        /// </summary>
        public static String UrlEncode( this String input ) {
            if ( input == null ) {
                throw new ArgumentNullException( "input" );
            }

            const int maxLength = 32766;

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

        public static String HtmlDecode( this String input ) {
            return HttpUtility.HtmlDecode( input );
        }

        public static String HtmlEncode( this String input ) {
            return HttpUtility.HtmlEncode( input );
        }

        public static String HtmlAttributeEncode( this String input ) {
            return HttpUtility.HtmlAttributeEncode( input );
        }

        /// <summary>
        /// Check that a String is not null or empty
        /// </summary>
        /// <param name="input">String to check</param>
        /// <returns>bool</returns>
        public static bool HasValue( this String input ) {
            return !String.IsNullOrEmpty( input );
        }
    }
}
