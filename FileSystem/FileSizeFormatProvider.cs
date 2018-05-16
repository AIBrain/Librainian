// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FileSizeFormatProvider.cs",
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
// "Librainian/Librainian/FileSizeFormatProvider.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

    using System;
    using Maths;

    public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter {

        private const String FileSizeFormat = "fs";

        private static String DefaultFormat( String format, Object arg, IFormatProvider formatProvider ) {
            var formattableArg = arg as IFormattable;

            return formattableArg?.ToString( format, formatProvider ) ?? arg.ToString();
        }

        public String Format( String format, Object arg, IFormatProvider formatProvider ) {
            if ( format is null || !format.StartsWith( FileSizeFormat ) ) { return DefaultFormat( format, arg, formatProvider ); }

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
}