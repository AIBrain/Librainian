// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "CompressExtensions.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

#nullable enable

namespace Librainian.OperatingSystem.Compression;

using System;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Exceptions;
using Maths;
using Microsoft.IO;
using Utilities;

public static class CompressExtensions {

	private static RecyclableMemoryStreamManager MemoryStreamManager { get; } = new( MathConstants.Sizes.OneMegaByte, MathConstants.Sizes.OneGigaByte );

	/// <summary>Compresses the data by using <see cref="GZipStream" />.</summary>
	/// <param name="data"></param>
	/// <param name="compressionLevel"></param>
	public static Byte[] Compress( this Byte[] data, CompressionLevel compressionLevel = CompressionLevel.Optimal ) {
		if ( data is null ) {
			throw new ArgumentEmptyException( nameof( data ) );
		}

		using var output = MemoryStreamManager.GetStream();

		using var compress = new GZipStream( output, compressionLevel );

		compress.Write( data, 0, data.Length );

		return output.ToArray();
	}

	public static Byte[] Compress( this String text, Encoding? encoding = null ) {
		if ( text is null ) {
			throw new ArgumentEmptyException( nameof( text ) );
		}

		return ( encoding ?? Common.DefaultEncoding ).GetBytes( text ).Compress();
	}

	/// <summary>Returns the string, Gzip compressed and then converted to base64.</summary>
	/// <param name="text"></param>
	/// <param name="encoding"></param>
	public static async Task<String> CompressAsync( this String text, Encoding? encoding = null ) {
		var buffer = ( encoding ?? Common.DefaultEncoding ).GetBytes( text );

		await using var streamIn = MemoryStreamManager.GetStream( buffer );

		await using var streamOut = MemoryStreamManager.GetStream();

		await using var zipStream = new GZipStream( streamOut, CompressionMode.Compress );
		var task = streamIn.CopyToAsync( zipStream );

		await task.ConfigureAwait( false );

		return Convert.ToBase64String( streamOut.ToArray() );
	}

	public static Byte[] Decompress( this Byte[] data ) {
		if ( data is null ) {
			throw new ArgumentEmptyException( nameof( data ) );
		}

		using var memoryStream = MemoryStreamManager.GetStream( data );

		using var decompress = new GZipStream( memoryStream, CompressionMode.Decompress );

		using var output = MemoryStreamManager.GetStream();

		decompress.CopyTo( output );

		return output.ToArray();
	}

	/// <summary>Returns the string decompressed (from base64).</summary>
	/// <param name="text"></param>
	/// <param name="encoding"></param>
	public static async Task<String> DecompressAsync( this String text, Encoding? encoding = null ) {
		var buffer = Convert.FromBase64String( text );

		await using var streamIn = MemoryStreamManager.GetStream( buffer );
		await using var streamOut = MemoryStreamManager.GetStream();

		await using var gs = new GZipStream( streamIn, CompressionMode.Decompress );
		await gs.CopyToAsync( streamOut ).ConfigureAwait( false );

		return ( encoding ?? Common.DefaultEncoding ).GetString( streamOut.ToArray() );
	}

	public static String DecompressToString( this Byte[] data, Encoding? encoding = null ) {
		if ( data is null ) {
			throw new ArgumentEmptyException( nameof( data ) );
		}

		return ( encoding ?? Common.DefaultEncoding ).GetString( data.Decompress() );
	}

	/// <summary>Returns the string decompressed (from base64).</summary>
	/// <param name="text"></param>
	/// <param name="encoding"></param>
	public static String FromCompressedBase64( this String text, Encoding? encoding = null ) {
		if ( text is null ) {
			throw new ArgumentEmptyException( nameof( text ) );
		}

		var buffer = Convert.FromBase64String( text );

		using var streamIn = MemoryStreamManager.GetStream( buffer );

		using var gs = new GZipStream( streamIn, CompressionMode.Decompress );

		using var streamOut = MemoryStreamManager.GetStream();

		gs.CopyTo( streamOut );

		return ( encoding ?? Common.DefaultEncoding ).GetString( streamOut.ToArray() );
	}

	/// <summary>Returns the string compressed (and then returned as a base64 string).</summary>
	/// <param name="text"></param>
	/// <param name="encoding"></param>
	public static String ToCompressedBase64( this String text, Encoding? encoding = null ) {
		using var incoming = MemoryStreamManager.GetStream( ( encoding ?? Common.DefaultEncoding ).GetBytes( text ) );

		using var streamOut = MemoryStreamManager.GetStream();

		using var destination = new GZipStream( streamOut, CompressionLevel.Fastest );

		incoming.CopyTo( destination );

		return Convert.ToBase64String( streamOut.ToArray() );
	}
}