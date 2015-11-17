// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/FileSizeFormatProviderExtensions.cs" was last cleaned by Rick on 2015/11/13 at 11:30 PM

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using IO;

    public static class FileSizeFormatProviderExtensions {

        private static readonly FileSizeFormatProvider FormatProvider = new FileSizeFormatProvider();

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" String.
        /// </summary>
        public static String ToFileSize( this Decimal @bytes ) => String.Format( FormatProvider, "{0:fs}", @bytes );

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" String.
        /// </summary>
        public static String ToFileSize( this UInt64 @bytes ) => String.Format( FormatProvider, "{0:fs}", @bytes );

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" String.
        /// </summary>
        public static String ToFileSize( this Int64 @bytes ) => String.Format( FormatProvider, "{0:fs}", @bytes );

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" String.
        /// </summary>
        public static String ToFileSize( this Int32 @bytes ) => String.Format( FormatProvider, "{0:fs}", @bytes );

    }

}
