// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/CompressExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:55 PM

namespace Librainian.OperatingSystem.Compression {

    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>
    /// </summary>
    public static class CompressExtensions {

        /// <summary>
        ///     Compresses the data by using <see cref="GZipStream" />.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="compressionLevel"></param>
        /// <returns></returns>
        public static Byte[] Compress( [NotNull] this Byte[] data, CompressionLevel compressionLevel = CompressionLevel.Optimal ) {
            if ( data == null ) {
                throw new ArgumentNullException( nameof( data ) );
            }

            var output = new MemoryStream();
            using ( var compress = new GZipStream( output, compressionLevel ) ) {
                compress.Write( data, 0, data.Length );
            }
            return output.ToArray();

        }

        /// <summary>
        ///     Pulled from https://bitbucket.org/jpbochi/jplabscode/src/e1bb20c8f273/Extensions/CompressionExt.cs
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Byte[] Compress( [NotNull] this String text ) {
            if ( text == null ) {
                throw new ArgumentNullException( nameof( text ) );
            }
            return Compress( text, Encoding.Default );
        }

        public static Byte[] Compress( [NotNull] this String text, [NotNull] Encoding encoding ) {
            if ( text == null ) {
                throw new ArgumentNullException( nameof( text ) );
            }
            if ( encoding == null ) {
                throw new ArgumentNullException( nameof( encoding ) );
            }
            return encoding.GetBytes( text ).Compress();
        }

        /// <summary>
        ///     Returns the string compressed (to base64).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async Task<String> CompressAsync( this String text ) {
            var buffer = Encoding.Unicode.GetBytes( text );
            using ( var streamIn = new MemoryStream( buffer ) ) {
                using ( var streamOut = new MemoryStream() ) {
                    using ( var zipStream = new GZipStream( streamOut, CompressionMode.Compress ) ) {
                        await streamIn.CopyToAsync( zipStream );
                    }
                    return Convert.ToBase64String( streamOut.ToArray() );
                }
            }
        }

        public static Byte[] Decompress( [NotNull] this Byte[] data ) {
            if ( data == null ) {
                throw new ArgumentNullException( nameof( data ) );
            }
            using ( var decompress = new GZipStream( new MemoryStream( data ), CompressionMode.Decompress ) ) {
                using ( var output = new MemoryStream() ) {
                    decompress.CopyTo( output );
                    return output.ToArray();
                }
            }
        }

        /// <summary>
        ///     Returns the string decompressed (from base64).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async Task<String> DecompressAsync( this String text ) {
            var buffer = Convert.FromBase64String( text );
            using ( var streamIn = new MemoryStream( buffer ) ) {
                using ( var streamOut = new MemoryStream() ) {
                    using ( var gs = new GZipStream( streamIn, CompressionMode.Decompress ) ) {
                        await gs.CopyToAsync( streamOut );
                    }
                    return Encoding.Unicode.GetString( streamOut.ToArray() );
                }
            }
        }

        public static String DecompressToString( [NotNull] this Byte[] data ) {
            if ( data == null ) {
                throw new ArgumentNullException( nameof( data ) );
            }
            return DecompressToString( data, Encoding.Default );
        }

        public static String DecompressToString( [NotNull] this Byte[] data, [NotNull] Encoding encoding ) {
            if ( data == null ) {
                throw new ArgumentNullException( nameof( data ) );
            }
            if ( encoding == null ) {
                throw new ArgumentNullException( nameof( encoding ) );
            }
            return encoding.GetString( data.Decompress() );
        }

        /// <summary>
        ///     Returns the string decompressed (from base64).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static String FromCompressedBase64( this String text ) {
            var buffer = Convert.FromBase64String( text );
            using ( var streamOut = new MemoryStream() ) {
	            using ( var streamIn = new MemoryStream( buffer ) ) {
		            using ( var gs = new GZipStream( streamIn, CompressionMode.Decompress ) ) {
			            gs.CopyTo( streamOut );
		            }
	            }
	            return Encoding.Unicode.GetString( streamOut.ToArray() );
            }
        }

        /// <summary>
        ///     Returns the string compressed (to base64).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static String ToCompressedBase64( this String text ) {
            var buffer = Encoding.Unicode.GetBytes( text );
            using ( var streamIn = new MemoryStream( buffer: buffer ) ) {
	            using ( var streamOut = new MemoryStream() ) {
		            using ( var zipStream = new GZipStream( stream: streamOut, compressionLevel: CompressionLevel.Fastest ) ) {
			            streamIn.CopyTo( zipStream );
		            }
		            return Convert.ToBase64String( streamOut.ToArray() );
	            }
            }
        }
    }

}