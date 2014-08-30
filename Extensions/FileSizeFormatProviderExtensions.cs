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
// "Librainian/FileSizeFormatProviderExtensions.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Extensions {
    using System;

    public static class FileSizeFormatProviderExtensions {
        private static readonly FileSizeFormatProvider FormatProvider = new FileSizeFormatProvider();

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" string.
        /// </summary>
        public static String ToFileSize( this  Decimal @bytes ) {
            return String.Format( FormatProvider, "{0:fs}", @bytes );
        }

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" string.
        /// </summary>
        public static String ToFileSize( this UInt64 @bytes ) {
            return String.Format( FormatProvider, "{0:fs}", @bytes );
        }

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" string.
        /// </summary>
        public static String ToFileSize( this long @bytes ) {
            return String.Format( FormatProvider, "{0:fs}", @bytes );
        }

        /// <summary>
        ///     Return the number supplied into its "TB, GB, MB, KB, or Bytes" string.
        /// </summary>
        public static String ToFileSize( this int @bytes ) {
            return String.Format( FormatProvider, "{0:fs}", @bytes );
        }
    }
}
