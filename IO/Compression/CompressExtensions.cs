// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/CompressExtensions.cs" was last cleaned by Rick on 2014/12/09 at 5:55 AM

namespace Librainian.IO.Compression {
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// </summary>
    public static class CompressExtensions {

        /// <summary>
        /// Returns the string compressed (to base64).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async Task<String> CompressAsync( this String text ) {
            var buffer = Encoding.Unicode.GetBytes( text );
            using (var streamIn = new MemoryStream( buffer )) {
                using (var streamOut = new MemoryStream()) {
                    using (var zipStream = new GZipStream( streamOut, CompressionMode.Compress )) {
                        await streamIn.CopyToAsync( zipStream );
                    }
                    return Convert.ToBase64String( streamOut.ToArray() );
                }
            }
        }

        /// <summary>
        /// Returns the string decompressed (from base64).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async Task<String> DecompressAsync( this String text ) {
            var buffer = Convert.FromBase64String( text );
            using (var streamIn = new MemoryStream( buffer )) {
                using (var streamOut = new MemoryStream()) {
                    using (var gs = new GZipStream( streamIn, CompressionMode.Decompress )) {
                        await gs.CopyToAsync( streamOut );
                    }
                    return Encoding.Unicode.GetString( streamOut.ToArray() );
                }
            }
        }
    }
}