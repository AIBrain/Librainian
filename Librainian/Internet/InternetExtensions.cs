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
// File "InternetExtensions.cs" last touched on 2021-04-25 at 6:05 PM by Protiguous.

namespace Librainian.Internet;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Exceptions;
using Logging;
using Newtonsoft.Json.Linq;

public static class InternetExtensions {

	private static Regex IP4ValidRegex { get; } = new( "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}", RegexOptions.Compiled | RegexOptions.Singleline );

	private static Regex ValidateURLRegex { get; } = new( @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Compiled );

	public static async Task<TextReader?> DoRequestAsync( this WebRequest request ) {
		if ( request is null ) {
			throw new ArgumentEmptyException( nameof( request ) );
		}

		var result = await request.GetResponseAsync().ConfigureAwait( false );

		//var result = await Task.Factory.FromAsync( ( asyncCallback, state ) => BeginGetResponse( state, asyncCallback ), EndGetResponse, request ).ConfigureAwait( false );

		await using var stream = result.GetResponseStream();

		return new StreamReader( stream );

		/*
		static IAsyncResult BeginGetResponse( Object? state, AsyncCallback asyncCallback ) {
			if ( state is HttpWebRequest httpWebRequest ) {
				return httpWebRequest.BeginGetResponse( asyncCallback, state );
			}

			throw new InvalidOperationException( $"Invalid {nameof( state )} object." );
		}

		static WebResponse EndGetResponse( IAsyncResult asyncResult ) {
			if ( asyncResult.AsyncState is HttpWebRequest httpWebRequest ) {
				return httpWebRequest.EndGetResponse( asyncResult );
			}
			throw new InvalidOperationException( $"Invalid {nameof( HttpWebRequest )} result." );
		}
		*/
	}

	/// <summary>Convert network bytes to a string</summary>
	/// <exception cref="ArgumentException"></exception>
	public static String FromNetworkBytes( this IEnumerable<Byte> data ) {
		var listData = data as IList<Byte> ?? data.ToList();

		var len = IPAddress.NetworkToHostOrder( BitConverter.ToInt16( listData.Take( 2 ).ToArray(), 0 ) );

		if ( listData.Count < 2 + len ) {
			throw new ArgumentException( "Too few bytes in packet" );
		}

		return Encoding.UTF8.GetString( listData.Skip( 2 ).Take( len ).ToArray() );
	}

	/// <summary>Return the machine's hostname</summary>
	public static String GetHostName() => Dns.GetHostName();

	public static JObject GetNonAsync( Uri? uri ) {
		var httpClient = new HttpClient();
		var content = httpClient.GetStringAsync( uri ).Result; //TODO bad

		return JObject.Parse( content );
	}

	public static async Task<String> GetString( this HttpClient httpClient, Uri url, CancellationToken cancellationToken, IDictionary<String, String>? headers = null ) {
		if ( httpClient == null ) {
			throw new ArgumentEmptyException( nameof( httpClient ) );
		}

		if ( headers?.Any() == true ) {
			foreach ( var item in headers ) {
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation( item.Key, item.Value );
			}
		}

		var response = await httpClient.GetAsync( url, cancellationToken ).ConfigureAwait( false );
		return await response.Content.ReadAsStringAsync( cancellationToken ).ConfigureAwait( false );
	}

	public static async Task<String?> GetWebPageAsync( this Uri url, CancellationToken cancellationToken ) {
		if ( url is null ) {
			throw new ArgumentEmptyException( nameof( url ) );
		}

		try {
			var request = new HttpClient();

			await using var response = await request.GetStreamAsync( url, cancellationToken ).ConfigureAwait( false );

			using var reader = new StreamReader( response );

			return await reader.ReadToEndAsync().ConfigureAwait( false );
		}
		catch {
			$"Unable to connect to {url}.".Error();
		}

		return default( String? );
	}

	public static Boolean IsValidIP4( this String ip ) {
		if ( !IP4ValidRegex.IsMatch( ip ) ) {
			return false;
		}

		var ips = ip.Split( '.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );

		return ips.Length is 4 && Int32.Parse( ips[0] ) < 256 && ( Int32.Parse( ips[1] ) < 256 ) && ( Int32.Parse( ips[2] ) < 256 ) && ( Int32.Parse( ips[3] ) < 256 );
	}

	public static Boolean IsValidUrl( this String text ) => ValidateURLRegex.IsMatch( text );

	public static IEnumerable<UriLinkItem> ParseLinks( Uri baseUri, String webpage ) {
		if ( baseUri == null ) {
			throw new ArgumentEmptyException( nameof( baseUri ) );
		}

		if ( String.IsNullOrWhiteSpace( webpage ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( webpage ) );
		}

		foreach ( Match match in Regex.Matches( webpage, @"(<a.*?>.*?</a>)", RegexOptions.Singleline ) ) {
			var value = match.Groups[1].Value;
			var m2 = Regex.Match( value, @"href=\""(.*?)\""", RegexOptions.Singleline );

			var i = new UriLinkItem( new Uri( baseUri, m2.Success ? m2.Groups[1].Value : String.Empty ),
				Regex.Replace( value, @"\s*<.*?>\s*", "", RegexOptions.Singleline ) );

			yield return i;
		}
	}

	/// <summary>Convert a string to network bytes</summary>
	public static IEnumerable<Byte> ToNetworkBytes( this String data ) {
		var bytes = Encoding.UTF8.GetBytes( data );

		var len = IPAddress.HostToNetworkOrder( ( Int16 )bytes.Length );

		return BitConverter.GetBytes( len ).Concat( bytes );
	}

	public static String ToQueryString( this NameValueCollection nvc ) => String.Join( "&", nvc.AllKeys.Select( key => $"{HttpUtility.UrlEncode( key )}={HttpUtility.UrlEncode( nvc[key] )}" ) );

	/*
	public static async Task<TextReader?> DoRequestAsync( this Uri uri ) {
		if ( uri is null ) {
			throw new ArgumentEmptyException( nameof( uri ) );
		}

		var request = new HttpClient( new HttpClientHandler(){} );

		//var request = WebRequest.CreateHttp( uri );

		//request.AllowReadStreamBuffering = true;

		//request.GetStringAsync(

		return await request.DoRequestAsync().ConfigureAwait( false );
	}
	*/

	/*
	public static async Task<T?> DoRequestJsonAsync<T>( this WebRequest request ) {
		if ( request is null ) {
			throw new ArgumentEmptyException( nameof( request ) );
		}

		using var reader = await DoRequestAsync( request ).ConfigureAwait( false );

		var response = await reader.ReadToEndAsync().ConfigureAwait( false );

		return JsonConvert.DeserializeObject<T>( response );
	}
	*/

	/*
	public static async Task<T?> DoRequestJsonAsync<T>( Uri uri ) {
		var reader = await DoRequestAsync( uri ).ConfigureAwait( false );

		var response = await reader.ReadToEndAsync().ConfigureAwait( false );

		return JsonConvert.DeserializeObject<T>( response );
	}
	*/
}