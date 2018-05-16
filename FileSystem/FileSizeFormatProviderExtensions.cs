// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FileSizeFormatProviderExtensions.cs",
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
// "Librainian/Librainian/FileSizeFormatProviderExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

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