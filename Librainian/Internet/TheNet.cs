#nullable enable

namespace Librainian.Internet {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Cache;
	using System.Text;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Logging;
	using Measurement.Time;
	using Newtonsoft.Json;

	public static class TheNet {

		[ItemCanBeNull]
		public static async Task<T> DeserializeJson<T>( [NotNull] this Uri uri, TimeSpan? timeout = null ) {
			if ( uri is null ) {
				throw new ArgumentNullException( nameof( uri ) );
			}

			timeout ??= Seconds.Seven;

			var response = await GetWebPageAsync( uri, timeout ).ConfigureAwait( false );

			return JsonConvert.DeserializeObject<T>( response );
		}

		/// <summary>Convert network bytes to a string</summary>
		/// <exception cref="ArgumentException"></exception>
		[NotNull]
		public static String FromNetworkBytes( [NotNull] this IEnumerable<Byte> data ) {
			var listData = data as IList<Byte> ?? data.ToList();

			var len = IPAddress.NetworkToHostOrder( BitConverter.ToInt16( listData.Take( 2 ).ToArray(), 0 ) );

			if ( listData.Count < 2 + len ) {
				throw new ArgumentException( "Too few bytes in packet" );
			}

			return Encoding.UTF8.GetString( listData.Skip( 2 ).Take( len ).ToArray() );
		}

		/// <summary>Return the machine's hostname</summary>
		[NotNull]
		public static String GetHostName() => Dns.GetHostName();

		[CanBeNull]
		public static String? GetWebPage( [CanBeNull] this String? url ) {
			try {
				if ( Uri.TryCreate( url, UriKind.Absolute, out var uri ) ) {
					using var client = new WebClient {
						Encoding = Encoding.Unicode,
						CachePolicy = new RequestCachePolicy( RequestCacheLevel.NoCacheNoStore )
					};

					return client.DownloadString( uri );
				}

				throw new Exception( $"Unable to connect to {url}." );
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		[CanBeNull]
		public static async Task<String?> GetWebPageAsync( [NotNull] this String url, TimeSpan timeout ) {
			if ( String.IsNullOrWhiteSpace( url ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( url ) );
			}

			try {
				if ( Uri.TryCreate( url, UriKind.Absolute, out var uri ) ) {
					return await uri.GetWebPageAsync( timeout ).ConfigureAwait( false );
				}

				throw new Exception( $"Unable to parse the url:{url}." );
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		[CanBeNull]
		public static Task<String> GetWebPageAsync( [NotNull] this Uri uri, TimeSpan? timeout = null ) {
			if ( uri is null ) {
				throw new ArgumentNullException( nameof( uri ) );
			}

			try {
				using var client = new WebClientWithTimeout {
					Encoding = Encoding.UTF8,
					CachePolicy = new RequestCachePolicy( RequestCacheLevel.NoCacheNoStore )
				};

				if ( timeout.HasValue ) {
					client.SetTimeout( timeout.Value );
				}

				return client.DownloadStringTaskAsync( uri );
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return Task.FromResult( String.Empty );
		}

		/// <summary>Convert a string to network bytes</summary>
		[NotNull]
		public static IEnumerable<Byte> ToNetworkBytes( [NotNull] this String data ) {
			if ( String.IsNullOrEmpty( data ) ) {
				throw new ArgumentException( "Value cannot be null or empty.", nameof( data ) );
			}

			var bytes = Encoding.UTF8.GetBytes( data );

			var hostToNetworkOrder = IPAddress.HostToNetworkOrder( ( Int16 )bytes.Length );

			return BitConverter.GetBytes( hostToNetworkOrder ).Concat( bytes );
		}

	}

}