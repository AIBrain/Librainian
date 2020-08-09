#nullable enable
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
				throw new ArgumentNullException( nameof( data ) );
			}

			using var output = new MemoryStream();

			using var compress = new GZipStream( output, compressionLevel );

			compress.Write( data, 0, data.Length );

			return output.ToArray();
		}

		[NotNull]
		public static Byte[] Compress( [NotNull] this String text, [CanBeNull] Encoding? encoding = null ) {
			if ( text is null ) {
				throw new ArgumentNullException( nameof( text ) );
			}

			return ( encoding ?? Common.DefaultEncoding ).GetBytes( text ).Compress();
		}

		/// <summary>Returns the string, Gzip compressed and then converted to base64.</summary>
		/// <param name="text"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		[ItemNotNull]
		public static async Task<String> CompressAsync( [NotNull] this String text, [CanBeNull] Encoding? encoding = null ) {
			var buffer = ( encoding ?? Common.DefaultEncoding ).GetBytes( text );

#if !NET48
			await
#endif
				using var streamIn = new MemoryStream( buffer );

#if !NET48
			await
#endif
				using var streamOut = new MemoryStream();

#if !NET48
			await
#endif
				using ( var zipStream = new GZipStream( streamOut, CompressionMode.Compress ) ) {
				var task = streamIn.CopyToAsync( zipStream );

				if ( task != null ) {
					await task.ConfigureAwait( false );
				}
			}

			return Convert.ToBase64String( streamOut.ToArray() );
		}

		[NotNull]
		public static Byte[] Decompress( [NotNull] this Byte[] data ) {
			if ( data is null ) {
				throw new ArgumentNullException( nameof( data ) );
			}

			using var memoryStream = new MemoryStream( data );

			using var decompress = new GZipStream( memoryStream, CompressionMode.Decompress );

			using var output = new MemoryStream();

			decompress.CopyTo( output );

			return output.ToArray();
		}

		/// <summary>Returns the string decompressed (from base64).</summary>
		/// <param name="text"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		[ItemNotNull]
		public static async Task<String> DecompressAsync( [NotNull] this String text, [CanBeNull] Encoding? encoding = null ) {
			var buffer = Convert.FromBase64String( text );

#if NET48
			using var streamIn = new MemoryStream( buffer );
			using var streamOut = new MemoryStream();

			using var gs = new GZipStream( streamIn, CompressionMode.Decompress );

			await gs.CopyToAsync( streamOut ).ConfigureAwait( false );

#else
			await using var streamIn = new MemoryStream( buffer );
			await using var streamOut = new MemoryStream();

			await using ( var gs = new GZipStream( streamIn, CompressionMode.Decompress ) ) {
				await gs.CopyToAsync( streamOut ).ConfigureAwait( false );
			}
#endif

			return ( encoding ?? Common.DefaultEncoding ).GetString( streamOut.ToArray() );
		}

		[NotNull]
		public static String DecompressToString( [NotNull] this Byte[] data, [CanBeNull] Encoding? encoding = null ) {
			if ( data is null ) {
				throw new ArgumentNullException( nameof( data ) );
			}

			return ( encoding ?? Common.DefaultEncoding ).GetString( data.Decompress() );
		}

		/// <summary>Returns the string decompressed (from base64).</summary>
		/// <param name="text"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		[NotNull]
		public static String FromCompressedBase64( [NotNull] this String text, [CanBeNull] Encoding? encoding = null ) {
			var buffer = Convert.FromBase64String( text );

			using var streamIn = new MemoryStream( buffer );

			using var gs = new GZipStream( streamIn, CompressionMode.Decompress );

			using var streamOut = new MemoryStream();

			gs.CopyTo( streamOut );

			return ( encoding ?? Common.DefaultEncoding ).GetString( streamOut.ToArray() );
		}

		/// <summary>Returns the string compressed (and then returned as a base64 string).</summary>
		/// <param name="text"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		[NotNull]
		public static String ToCompressedBase64( [NotNull] this String text, [CanBeNull] Encoding? encoding = null ) {
			using var incoming = new MemoryStream( ( encoding ?? Common.DefaultEncoding ).GetBytes( text ), false );

			using var streamOut = new MemoryStream();

			using var destination = new GZipStream( streamOut, CompressionLevel.Fastest );

			incoming.CopyTo( destination );

			return Convert.ToBase64String( streamOut.ToArray() );
		}

	}

}