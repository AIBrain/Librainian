// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FileSizeFormatProviderExtensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;

    public static class FileSizeFormatProviderExtensions {
        private static readonly FileSizeFormatProvider FormatProvider = new FileSizeFormatProvider();

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" String.
        /// </summary>
        public static String ToFileSize( this Decimal bytes ) => String.Format( FormatProvider, "{0:fs}", bytes );

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" String.
        /// </summary>
        public static String ToFileSize( this UInt64 bytes ) => String.Format( FormatProvider, "{0:fs}", bytes );

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" String.
        /// </summary>
        public static String ToFileSize( this Int64 bytes ) => String.Format( FormatProvider, "{0:fs}", bytes );

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" String.
        /// </summary>
        public static String ToFileSize( this Int32 bytes ) => String.Format( FormatProvider, "{0:fs}", bytes );
    }
}