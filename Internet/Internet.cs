// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Internet.cs" belongs to Rick@AIBrain.org and
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
// File "Internet.cs" was last formatted by Protiguous on 2018/06/04 at 3:59 PM.

namespace Librainian.Internet {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Cache;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Measurement.Time;
	using Newtonsoft.Json;
	using Threading;

	public static class Internet {

		public static async Task<T> DeserializeJson<T>( [NotNull] this Uri uri, TimeSpan? timeout = null ) {
			if ( uri == null ) { throw new ArgumentNullException( paramName: nameof( uri ) ); }

			if ( timeout == null ) { timeout = TimeSpan.Zero + Seconds.Seven; }

			var response = await uri.GetWebPageAsync( timeout.Value ).NoUI();

			return JsonConvert.DeserializeObject<T>( response );
		}

		/// <summary>Convert network bytes to a string</summary>
		/// <exception cref="ArgumentException"></exception>
		public static String FromNetworkBytes( this IEnumerable<Byte> data ) {
			var listData = data as IList<Byte> ?? data.ToList();

			var len = IPAddress.NetworkToHostOrder( network: BitConverter.ToInt16( listData.Take( count: 2 ).ToArray(), startIndex: 0 ) );

			if ( listData.Count < 2 + len ) { throw new ArgumentException( "Too few bytes in packet" ); }

			return Encoding.UTF8.GetString( bytes: listData.Skip( count: 2 ).Take( count: len ).ToArray() );
		}

		/// <summary>Return the machine's hostname</summary>
		public static String GetHostName() => Dns.GetHostName();

		[CanBeNull]
		public static String GetWebPage( this String url ) {
			try {
				if ( Uri.TryCreate( url, UriKind.Absolute, out var uri ) ) {
					using ( var client = new WebClient { Encoding = Encoding.Unicode, CachePolicy = new RequestCachePolicy( RequestCacheLevel.NoCacheNoStore ) } ) { return client.DownloadString( uri ); }
				}

				throw new Exception( $"Unable to connect to {url}." );
			}
			catch ( Exception exception ) { exception.More(); }

			return null;
		}

		[CanBeNull]
		public static async Task<String> GetWebPageAsync( [NotNull] this String url, TimeSpan timeout ) {
			if ( String.IsNullOrWhiteSpace( value: url ) ) { throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( url ) ); }

			try {
				if ( Uri.TryCreate( url, UriKind.Absolute, out var uri ) ) { return await uri.GetWebPageAsync( timeout ).NoUI(); }

				throw new Exception( $"Unable to parse the url:{url}." );
			}
			catch ( Exception exception ) { exception.More(); }

			return null;
		}

		[CanBeNull]
		public static async Task<String> GetWebPageAsync( [NotNull] this Uri uri, TimeSpan timeout ) {
			if ( uri == null ) { throw new ArgumentNullException( paramName: nameof( uri ) ); }

			try {
				using ( var client = new WebClient { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy( RequestCacheLevel.NoCacheNoStore ) } ) {
					var result = await client.DownloadStringTaskAsync( uri ).WithCancellation( timeout, CancellationToken.None ).NoUI();

					return result;
				}
			}
			catch ( Exception exception ) { exception.More(); }

			return null;
		}

		/// <summary>Convert a string to network bytes</summary>
		public static IEnumerable<Byte> ToNetworkBytes( [NotNull] this String data ) {
			if ( String.IsNullOrEmpty( value: data ) ) { throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( data ) ); }

			var bytes = Encoding.UTF8.GetBytes( s: data );

			var len = IPAddress.HostToNetworkOrder( host: ( Int16 ) bytes.Length );

			return BitConverter.GetBytes( len ).Concat( second: bytes );
		}

	}

}