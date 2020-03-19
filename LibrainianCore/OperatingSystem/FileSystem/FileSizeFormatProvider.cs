// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "FileSizeFormatProvider.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "FileSizeFormatProvider.cs" was last formatted by Protiguous on 2020/03/16 at 3:09 PM.

namespace Librainian.OperatingSystem.FileSystem {

    /*

    /// <summary>
    /// //TODO I think there is a nuget package to do this better.
    /// </summary>
    [Obsolete("I think there is a nuget package to do this better")]
    public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter {

        private const String FileSizeFormat = "fs";

        private static String DefaultFormat( String format, Object arg, IFormatProvider formatProvider ) {
            var formattableArg = arg as IFormattable;

            return formattableArg?.ToString( format, formatProvider ) ?? arg.ToString();
        }

        public String Format( String format, Object arg, IFormatProvider formatProvider ) {
            if ( format?.StartsWith( FileSizeFormat ) != true ) { return DefaultFormat( format, arg, formatProvider ); }

            if ( arg is String ) { return DefaultFormat( format, arg, formatProvider ); }

            UInt64 size;

            try { size = Convert.ToUInt64( arg ); }
            catch ( InvalidCastException ) { return DefaultFormat( format, arg, formatProvider ); }

            var suffix = "n/a";

            if ( size.Between( Constants.Sizes.OneTeraByte, UInt64.MaxValue ) ) {
                size /= Constants.Sizes.OneTeraByte;
                suffix = "TB";
            }
            else if ( size.Between( Constants.Sizes.OneGigaByte, Constants.Sizes.OneTeraByte ) ) {
                size /= Constants.Sizes.OneGigaByte;
                suffix = "GB";
            }
            else if ( size.Between( Constants.Sizes.OneMegaByte, Constants.Sizes.OneGigaByte ) ) {
                size /= Constants.Sizes.OneMegaByte;
                suffix = "MB";
            }
            else if ( size.Between( Constants.Sizes.OneKiloByte, Constants.Sizes.OneMegaByte ) ) {
                size /= Constants.Sizes.OneKiloByte;
                suffix = "kB";
            }
            else if ( size.Between( UInt64.MinValue, Constants.Sizes.OneKiloByte ) ) { suffix = "Bytes"; }

            return $"{size:N0} {suffix}";
        }

        public Object GetFormat( Type formatType ) => formatType == typeof( ICustomFormatter ) ? this : null;
    }
    */

}