// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "FacebookErrorGrabber.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "FacebookErrorGrabber.cs" was last formatted by Protiguous on 2018/06/04 at 4:03 PM.

namespace Librainian.Maths {

	using System;
	using System.Text;
	using System.Threading.Tasks;
	using Collections;
	using Exceptions;
	using Extensions;
	using Internet;
	using Newtonsoft.Json;
	using Threading;

	public static class FacebookErrorGrabber {

		[JsonObject]
		public struct FaceBookError {

			[JsonProperty( propertyName: "code" )]
			public Int32 Code { get; set; }

			[JsonProperty( propertyName: "fbtrace_id" )]
			public String FbtraceID { get; set; }

			[JsonProperty( propertyName: "message" )]
			public String Message { get; set; }

			[JsonProperty( propertyName: "type" )]
			public String Type { get; set; }

		}

		[JsonObject]
		public struct FaceBookRootObject {

			[JsonProperty( propertyName: "error" )]
			internal FaceBookError Error { get; }

		}

		/// <summary>
		///     See also <seealso cref="Randem" />.
		/// </summary>
		/// <returns></returns>
		public static async Task<FaceBookRootObject> GetError() {
			var uri = new Uri( uriString: "http://graph.facebook.com/microsoft" );

			return await uri.DeserializeJson<FaceBookRootObject>().NoUI();
		}

		/// <summary>
		///     Pull another "random" number in a byte array via Facebook.
		/// </summary>
		/// <param name="fallbackByteCount">How many random bytes to fallback to when the facebook request fails.</param>
		/// <returns></returns>
		public static async Task<Byte[]> NextData( Int32 fallbackByteCount = 16 ) {
			var rootObject = await GetError().NoUI();

			var data = rootObject.Error.FbtraceID;

			if ( data != null ) {
				var buffer = Encoding.UTF8.GetBytes( data );

				//mix up the response a bit with our own rng.
				foreach ( var _ in buffer ) { buffer.Swap( Randem.NextByte(), Randem.NextByte() ); }

				return buffer;
			}

			if ( !fallbackByteCount.Any() ) { throw new OutOfRangeException( $"{nameof( fallbackByteCount )} must be greater than 0." ); }

			var fallback = new Byte[ fallbackByteCount ];
			Randem.Instance.NextBytes( fallback );

			return fallback;
		}

		public static async Task<Int64> NxtInt32() {
			var bytes = await NextData( sizeof( Int32 ) ).NoUI();

			return BitConverter.ToInt64( bytes, 0 );
		}

		public static async Task<Int64> NxtInt64() {
			var bytes = await NextData( sizeof( Int64 ) ).NoUI();

			return BitConverter.ToInt64( bytes, 0 );
		}

	}

	public static partial class Randem { }

}