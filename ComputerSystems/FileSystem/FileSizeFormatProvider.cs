// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FileSizeFormatProvider.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/FileSizeFormatProvider.cs" was last formatted by Protiguous on 2018/05/24 at 7:02 PM.

namespace Librainian.ComputerSystems.FileSystem {

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