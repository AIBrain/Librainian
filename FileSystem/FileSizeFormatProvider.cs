// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FileSizeFormatProvider.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using Maths;

    public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter {
        private const String FileSizeFormat = "fs";

        public String Format( String format, Object arg, IFormatProvider formatProvider ) {
            if ( format is null || !format.StartsWith( FileSizeFormat ) ) {
                return DefaultFormat( format, arg, formatProvider );
            }

            if ( arg is String ) {
                return DefaultFormat( format, arg, formatProvider );
            }

            UInt64 size;
            try {
                size = Convert.ToUInt64( arg );
            }
            catch ( InvalidCastException ) {
                return DefaultFormat( format, arg, formatProvider );
            }

            var suffix = "n/a";
            if ( size.Between( MathConstants.OneTeraByte, UInt64.MaxValue ) ) {
                size /= MathConstants.OneTeraByte;
                suffix = "TB";
            }
            else if ( size.Between( MathConstants.OneGigaByte, MathConstants.OneTeraByte ) ) {
                size /= MathConstants.OneGigaByte;
                suffix = "GB";
            }
            else if ( size.Between( MathConstants.OneMegaByte, MathConstants.OneGigaByte ) ) {
                size /= MathConstants.OneMegaByte;
                suffix = "MB";
            }
            else if ( size.Between( MathConstants.OneKiloByte, MathConstants.OneMegaByte ) ) {
                size /= MathConstants.OneKiloByte;
                suffix = "kB";
            }
            else if ( size.Between( UInt64.MinValue, MathConstants.OneKiloByte ) ) {
                suffix = "Bytes";
            }

            return $"{size:N0} {suffix}";
        }

        public Object GetFormat( Type formatType ) => formatType == typeof( ICustomFormatter ) ? this : null;

        private static String DefaultFormat( String format, Object arg, IFormatProvider formatProvider ) {
            var formattableArg = arg as IFormattable;
            return formattableArg?.ToString( format, formatProvider ) ?? arg.ToString();
        }
    }
}