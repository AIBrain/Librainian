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
// "Librainian/Urls.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    public static class Urls {
        public static String GetSuggestedNameFromUrl( String url, String defaultValue ) {
            var res = Path.GetFileNameWithoutExtension( url );

            //check if there is no file name, i.e. just folder name + query String
            if ( !String.IsNullOrEmpty( res ) && !IsNameOnlyQueryString( res ) ) {
                return defaultValue;
            }
            res = Path.GetFileName( Path.GetDirectoryName( url ) );

            return String.IsNullOrEmpty( res ) ? defaultValue : Regex.Replace( res, @"[^\w]", "_", RegexOptions.Singleline ).Substring( 0, 50 );
        }

        private static Boolean IsNameOnlyQueryString( String res ) {
            return !String.IsNullOrEmpty( res ) && res[ 0 ] == '?';
        }
    }
}
