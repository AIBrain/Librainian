// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "InternetExtensions.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "InternetExtensions.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class InternetExtensions {

        private static Regex ValidateURLRegex { get; } = new Regex( @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Compiled );

        [ItemCanBeNull]
        public static async Task<TextReader> DoRequestAsync( [NotNull] this WebRequest request ) {
            if ( request is null ) {
                throw new ArgumentNullException( nameof( request ) );
            }

            var result = await Task.Factory.FromAsync( ( asyncCallback, state ) => ( ( HttpWebRequest )state ).BeginGetResponse( asyncCallback, state ),
                asyncResult => ( ( HttpWebRequest )asyncResult.AsyncState ).EndGetResponse( asyncResult ), request ).ConfigureAwait( false );

            using var stream = result.GetResponseStream();

            return stream != null ? new StreamReader( stream ) : TextReader.Null;
        }

        [ItemCanBeNull]
        public static async Task<TextReader> DoRequestAsync( [NotNull] this Uri uri ) {
            if ( uri is null ) {
                throw new ArgumentNullException( nameof( uri ) );
            }

            var request = WebRequest.CreateHttp( uri );

            //request.AllowReadStreamBuffering = true;
            var textReader = await request.DoRequestAsync().ConfigureAwait( false );

            return textReader;
        }

        [ItemCanBeNull]
        public static async Task<T> DoRequestJsonAsync<T>( [NotNull] this WebRequest request ) {
            if ( request is null ) {
                throw new ArgumentNullException( nameof( request ) );
            }

            using var reader = await DoRequestAsync( request ).ConfigureAwait( false );

            if ( reader != null ) {
                var response = await reader.ReadToEndAsync().ConfigureAwait( false );

                return JsonConvert.DeserializeObject<T>( response );
            }

            return default;
        }

        [ItemCanBeNull]
        public static async Task<T> DoRequestJsonAsync<T>( [NotNull] Uri uri ) {
            var reader = await DoRequestAsync( uri ).ConfigureAwait( false );

            if ( reader != default ) {
                var response = await reader.ReadToEndAsync().ConfigureAwait( false );

                return JsonConvert.DeserializeObject<T>( response );
            }

            return default;
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
        public static JObject GetNonAsync( [CanBeNull] Uri uri ) {
            var httpClient = new HttpClient();
            var content = httpClient.GetStringAsync( uri ).Result; //TODO bad

            return JObject.Parse( content );
        }

        [ItemCanBeNull]
        public static async Task<String> GetWebPageAsync( [NotNull] this Uri url ) {
            if ( url is null ) {
                throw new ArgumentNullException( paramName: nameof( url ) );
            }

            try {
                var request = WebRequest.Create( url );
                request.Proxy = null;
                request.Credentials = CredentialCache.DefaultCredentials;

                using var response = await request.GetResponseAsync().ConfigureAwait( false );

                using var dataStream = response.GetResponseStream();

                if ( dataStream != null ) {
                    using var reader = new StreamReader( dataStream );

                    var responseFromServer = await reader.ReadToEndAsync().ConfigureAwait( false );

                    return responseFromServer;
                }
            }
            catch {
                $"Unable to connect to {url}.".Error();
            }

            return null;
        }

        public static Boolean IsValidIp( this String ip ) {
            if ( !Regex.IsMatch( ip, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}" ) ) {
                return default; //TODO precompile this regex
            }

            var ips = ip.Split( '.' );

            if ( ips.Length == 4 || ips.Length == 6 ) {
                return Int32.Parse( ips[ 0 ] ) < 256 && ( Int32.Parse( ips[ 1 ] ) < 256 ) & ( Int32.Parse( ips[ 2 ] ) < 256 ) & ( Int32.Parse( ips[ 3 ] ) < 256 );
            }

            return default;
        }

        public static Boolean IsValidUrl( this String text ) => ValidateURLRegex.IsMatch( text );

        public static IEnumerable<UriLinkItem> ParseLinks( [NotNull] Uri baseUri, [NotNull] String webpage ) {
            if ( baseUri == null ) {
                throw new ArgumentNullException( paramName: nameof( baseUri ) );
            }

            if ( String.IsNullOrWhiteSpace( value: webpage ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( webpage ) );
            }

            foreach ( Match match in Regex.Matches( webpage, @"(<a.*?>.*?</a>)", RegexOptions.Singleline ) ) {

                var value = match.Groups[ 1 ].Value;
                var m2 = Regex.Match( value, @"href=\""(.*?)\""", RegexOptions.Singleline );

                var i = new UriLinkItem {
                    Text = Regex.Replace( value, @"\s*<.*?>\s*", "", RegexOptions.Singleline ),
                    Href = new Uri( baseUri: baseUri, relativeUri: m2.Success ? m2.Groups[ 1 ].Value : String.Empty )
                };

                yield return i;
            }
        }

        /// <summary>Convert a string to network bytes</summary>
        [NotNull]
        public static IEnumerable<Byte> ToNetworkBytes( [NotNull] this String data ) {
            var bytes = Encoding.UTF8.GetBytes( data );

            var len = IPAddress.HostToNetworkOrder( ( Int16 )bytes.Length );

            return BitConverter.GetBytes( len ).Concat( bytes );
        }
    }
}