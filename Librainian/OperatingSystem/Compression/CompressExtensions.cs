// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "CompressExtensions.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "CompressExtensions.cs" was last formatted by Protiguous on 2018/06/26 at 1:34 AM.

namespace Librainian.OperatingSystem.Compression {

	using System;
	using System.IO;
	using System.IO.Compression;
	using System.Text;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Threading;

	/// <summary>
	/// </summary>
	public static class CompressExtensions {

		/// <summary>
		///     Compresses the data by using <see cref="GZipStream" />.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="compressionLevel"></param>
		/// <returns></returns>
		[NotNull]
		public static Byte[] Compress( [NotNull] this Byte[] data, CompressionLevel compressionLevel = CompressionLevel.Optimal ) {
			if ( data is null ) {
				throw new ArgumentNullException( nameof( data ) );
			}

			var output = new MemoryStream();

			using ( var compress = new GZipStream( output, compressionLevel ) ) {
				compress.Write( data, 0, data.Length );
			}

			return output.ToArray();
		}

		/// <summary>
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		/// <see cref="http://bitbucket.org/jpbochi/jplabscode/src/e1bb20c8f273/Extensions/CompressionExt.cs" />
		public static Byte[] Compress( [NotNull] this String text ) {
			if ( text is null ) {
				throw new ArgumentNullException( nameof( text ) );
			}

			return Compress( text, Encoding.Default );
		}

		[NotNull]
		public static Byte[] Compress( [NotNull] this String text, [NotNull] Encoding encoding ) {
			if ( text is null ) {
				throw new ArgumentNullException( nameof( text ) );
			}

			if ( encoding is null ) {
				throw new ArgumentNullException( nameof( encoding ) );
			}

			return encoding.GetBytes( text ).Compress();
		}

		/// <summary>
		///     Returns the string, Gzip compressed and then converted to base64.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		[ItemNotNull]
		public static async Task<String> CompressAsync( [NotNull] this String text ) {
			var buffer = Encoding.Unicode.GetBytes( text );

			using ( var streamIn = new MemoryStream( buffer ) ) {
				using ( var streamOut = new MemoryStream() ) {
					using ( var zipStream = new GZipStream( streamOut, CompressionMode.Compress ) ) {
						await streamIn.CopyToAsync( zipStream ).NoUI();
					}

					return Convert.ToBase64String( streamOut.ToArray() );
				}
			}
		}

		[NotNull]
		public static Byte[] Decompress( [NotNull] this Byte[] data ) {
			if ( data is null ) {
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
		[ItemNotNull]
		public static async Task<String> DecompressAsync( [NotNull] this String text ) {
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
			if ( data is null ) {
				throw new ArgumentNullException( nameof( data ) );
			}

			return DecompressToString( data, Encoding.Default );
		}

		[NotNull]
		public static String DecompressToString( [NotNull] this Byte[] data, [NotNull] Encoding encoding ) {
			if ( data is null ) {
				throw new ArgumentNullException( nameof( data ) );
			}

			if ( encoding is null ) {
				throw new ArgumentNullException( nameof( encoding ) );
			}

			return encoding.GetString( data.Decompress() );
		}

		/// <summary>
		///     Returns the string decompressed (from base64).
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		[NotNull]
		public static String FromCompressedBase64( [NotNull] this String text ) {

			using ( var streamOut = new MemoryStream() ) {
				var buffer = Convert.FromBase64String( text );

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
		[NotNull]
		public static String ToCompressedBase64( [NotNull] this String text ) {
			using ( var streamOut = new MemoryStream() ) {
				var buffer = Encoding.Unicode.GetBytes( text );

				using ( var streamIn = new MemoryStream( buffer: buffer ) ) {
					using ( var zipStream = new GZipStream( stream: streamOut, compressionLevel: CompressionLevel.Fastest ) ) {
						streamIn.CopyTo( zipStream );
					}

					return Convert.ToBase64String( streamOut.ToArray() );
				}
			}
		}
	}
}