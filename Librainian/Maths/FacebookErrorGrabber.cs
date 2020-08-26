// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "FacebookErrorGrabber.cs" last formatted on 2020-08-14 at 8:36 PM.

namespace Librainian.Maths {

	using System;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using Collections.Extensions;
	using Exceptions;
	using Internet;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	public static class FacebookErrorGrabber {

		/// <summary>See also <see cref="Randem" />.</summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[NotNull]
		public static Task<FaceBookRootObject> GetError( CancellationToken cancellationToken ) {
			var uri = new Uri( "http://graph.facebook.com/microsoft" );

			return uri.DeserializeJson<FaceBookRootObject>();
		}

		/// <summary>Pull another "random" number in a byte array via Facebook.</summary>
		/// <param name="fallbackByteCount">How many random bytes to fallback to when the facebook request fails.</param>
		/// <returns></returns>
		[ItemNotNull]
		public static async Task<Byte[]> NextDataAsync( Int32 fallbackByteCount = 16 ) {
			var rootObject = await GetError( CancellationToken.None ).ConfigureAwait( false );

			var data = rootObject.Error.FbtraceID;

			if ( data != null ) {
				var buffer = Encoding.UTF8.GetBytes( data );

				//mix up the response a bit with our own rng.
				foreach ( var _ in buffer ) {
					buffer.Swap( Randem.NextByte(), Randem.NextByte() );
				}

				return buffer;
			}

			if ( !fallbackByteCount.Any() ) {
				throw new OutOfRangeException( $"{nameof( fallbackByteCount )} must be greater than 0." );
			}

			var fallback = new Byte[fallbackByteCount];
			Randem.NextBytes( ref fallback );

			return fallback;
		}

		public static async Task<Int64> NxtInt32() {
			var bytes = await NextDataAsync( sizeof( Int32 ) ).ConfigureAwait( false );

			return BitConverter.ToInt64( bytes, 0 );
		}

		public static async Task<Int64> NxtInt64() {
			var bytes = await NextDataAsync( sizeof( Int64 ) ).ConfigureAwait( false );

			return BitConverter.ToInt64( bytes, 0 );
		}

		[JsonObject]
		public struct FaceBookError {

			[JsonProperty( "code" )]
			public Int32 Code { get; set; }

			[JsonProperty( "fbtrace_id" )]
			[CanBeNull]
			public String? FbtraceID { get; set; }

			[JsonProperty( "message" )]
			[CanBeNull]
			public String? Message { get; set; }

			[JsonProperty( "type" )]
			[CanBeNull]
			public String? Type { get; set; }

		}

		[JsonObject]
		public struct FaceBookRootObject {

			[JsonProperty( "error" )]
			public FaceBookError Error { get; }

		}

	}

}