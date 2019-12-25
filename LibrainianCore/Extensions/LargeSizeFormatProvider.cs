// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "LargeSizeFormatProvider.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "LargeSizeFormatProvider.cs" was last formatted by Protiguous on 2019/08/08 at 7:16 AM.

namespace LibrainianCore.Extensions {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using Maths;

    public class LargeSizeFormatProvider : IFormatProvider, ICustomFormatter {

        private const String FileSizeFormat = "fs";

        [NotNull]
        private static String DefaultFormat( String format, [NotNull] Object arg, IFormatProvider formatProvider ) {
            var formattableArg = arg as IFormattable;

            return formattableArg?.ToString( format, formatProvider ) ?? arg.ToString();
        }

        [NotNull]
        public String Format( [CanBeNull] String format, Object arg, IFormatProvider formatProvider ) {
            if ( format?.StartsWith( FileSizeFormat ) != true ) {
                return DefaultFormat( format, arg, formatProvider );
            }

            if ( arg is String ) {
                return DefaultFormat( format, arg, formatProvider );
            }

            Single size;

            try {
                size = Convert.ToUInt64( arg );
            }
            catch ( InvalidCastException ) {
                return DefaultFormat( format, arg, formatProvider );
            }

            var suffix = "n/a";

            if ( size.Between( MathConstants.Sizes.OneTeraByte, UInt64.MaxValue ) ) {
                size /= MathConstants.Sizes.OneTeraByte;
                suffix = "trillion";
            }
            else if ( size.Between( MathConstants.Sizes.OneGigaByte, MathConstants.Sizes.OneTeraByte ) ) {
                size /= MathConstants.Sizes.OneGigaByte;
                suffix = "billion";
            }
            else if ( size.Between( MathConstants.Sizes.OneMegaByte, MathConstants.Sizes.OneGigaByte ) ) {
                size /= MathConstants.Sizes.OneMegaByte;
                suffix = "million";
            }
            else if ( size.Between( MathConstants.Sizes.OneKiloByte, MathConstants.Sizes.OneMegaByte ) ) {
                size /= MathConstants.Sizes.OneKiloByte;
                suffix = "thousand";
            }
            else if ( size.Between( UInt64.MinValue, MathConstants.Sizes.OneKiloByte ) ) {
                suffix = "";
            }

            return $"{size:N3} {suffix}";
        }

        public Object GetFormat( [NotNull] Type formatType ) {
            if ( formatType is null ) {
                throw new ArgumentNullException( nameof( formatType ) );
            }

            return formatType == typeof( ICustomFormatter ) ? this : null;
        }
    }
}