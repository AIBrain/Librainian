// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "CompressExtensions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", File: "CompressExtensions.cs" was last formatted by Protiguous on 2020/03/16 at 2:59 PM.

namespace Librainian.OperatingSystem.Compression {

    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary></summary>
    public static class CompressExtensions {

        /// <summary>Compresses the data by using <see cref="GZipStream" />.</summary>
        /// <param name="data"></param>
        /// <param name="compressionLevel"></param>
        /// <returns></returns>
        [NotNull]
        public static Byte[] Compress( [NotNull] this Byte[] data, CompressionLevel compressionLevel = CompressionLevel.Optimal ) {
            if ( data is null ) {
                throw new ArgumentNullException( paramName: nameof( data ) );
            }

            var output = new MemoryStream();

            using ( var compress = new GZipStream( stream: output, compressionLevel: compressionLevel ) ) {
                compress.Write( array: data, offset: 0, count: data.Length );
            }

            return output.ToArray();
        }

        /// <summary></summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <see cref="http://bitbucket.org/jpbochi/jplabscode/src/e1bb20c8f273/Extensions/CompressionExt.cs" />
        [NotNull]
        public static Byte[] Compress( [NotNull] this String text ) {
            if ( text is null ) {
                throw new ArgumentNullException( paramName: nameof( text ) );
            }

            return Compress( text: text, encoding: Encoding.Default );
        }

        [NotNull]
        public static Byte[] Compress( [NotNull] this String text, [NotNull] Encoding encoding ) {
            if ( text is null ) {
                throw new ArgumentNullException( paramName: nameof( text ) );
            }

            if ( encoding is null ) {
                throw new ArgumentNullException( paramName: nameof( encoding ) );
            }

            return encoding.GetBytes( s: text ).Compress();
        }

        /// <summary>Returns the string, Gzip compressed and then converted to base64.</summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [ItemNotNull]
        public static async Task<String> CompressAsync( [NotNull] this String text ) {
            var buffer = Encoding.Unicode.GetBytes( s: text );

            using ( var streamIn = new MemoryStream( buffer: buffer ) ) {
                using ( var streamOut = new MemoryStream() ) {
                    using ( var zipStream = new GZipStream( stream: streamOut, mode: CompressionMode.Compress ) ) {
                        await streamIn.CopyToAsync( destination: zipStream ).ConfigureAwait( continueOnCapturedContext: false );
                    }

                    return Convert.ToBase64String( inArray: streamOut.ToArray() );
                }
            }
        }

        [NotNull]
        public static Byte[] Decompress( [NotNull] this Byte[] data ) {
            if ( data is null ) {
                throw new ArgumentNullException( paramName: nameof( data ) );
            }

            using ( var decompress = new GZipStream( stream: new MemoryStream( buffer: data ), mode: CompressionMode.Decompress ) ) {
                using ( var output = new MemoryStream() ) {
                    decompress.CopyTo( destination: output );

                    return output.ToArray();
                }
            }
        }

        /// <summary>Returns the string decompressed (from base64).</summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [ItemNotNull]
        public static async Task<String> DecompressAsync( [NotNull] this String text ) {
            var buffer = Convert.FromBase64String( s: text );

            using ( var streamIn = new MemoryStream( buffer: buffer ) ) {
                using ( var streamOut = new MemoryStream() ) {
                    using ( var gs = new GZipStream( stream: streamIn, mode: CompressionMode.Decompress ) ) {
                        await gs.CopyToAsync( destination: streamOut ).ConfigureAwait( continueOnCapturedContext: false );
                    }

                    return Encoding.Unicode.GetString( bytes: streamOut.ToArray() );
                }
            }
        }

        [NotNull]
        public static String DecompressToString( [NotNull] this Byte[] data ) {
            if ( data is null ) {
                throw new ArgumentNullException( paramName: nameof( data ) );
            }

            return DecompressToString( data: data, encoding: Encoding.Default );
        }

        [NotNull]
        public static String DecompressToString( [NotNull] this Byte[] data, [NotNull] Encoding encoding ) {
            if ( data is null ) {
                throw new ArgumentNullException( paramName: nameof( data ) );
            }

            if ( encoding is null ) {
                throw new ArgumentNullException( paramName: nameof( encoding ) );
            }

            return encoding.GetString( bytes: data.Decompress() );
        }

        /// <summary>Returns the string decompressed (from base64).</summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        [NotNull]
        public static String FromCompressedBase64( [NotNull] this String text, [CanBeNull] Encoding encoding = null ) {

            using ( var streamOut = new MemoryStream() ) {
                var buffer = Convert.FromBase64String( s: text );

                using ( var streamIn = new MemoryStream( buffer: buffer ) ) {
                    using ( var gs = new GZipStream( stream: streamIn, mode: CompressionMode.Decompress ) ) {
                        gs.CopyTo( destination: streamOut );
                    }
                }

                if ( encoding is null ) {
                    encoding = Encoding.Unicode;
                }

                return encoding.GetString( bytes: streamOut.ToArray() );
            }
        }

        /// <summary>Returns the string compressed (and then returned as a base64 string).</summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        [NotNull]
        public static String ToCompressedBase64( [NotNull] this String text, [CanBeNull] Encoding encoding = null ) {
            if ( encoding is null ) {
                encoding = Encoding.Unicode;
            }

            using ( var incoming = new MemoryStream( buffer: encoding.GetBytes( s: text ), writable: false ) ) {
                using ( var streamOut = new MemoryStream() ) {
                    using ( var destination = new GZipStream( stream: streamOut, compressionLevel: CompressionLevel.Fastest ) ) {
                        incoming.CopyTo( destination: destination );
                    }

                    return Convert.ToBase64String( inArray: streamOut.ToArray() );
                }
            }
        }
    }
}