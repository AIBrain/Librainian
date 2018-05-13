// Copyright 2018 Protiguous
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/InternetExtensions.cs" was last cleaned by Protiguous on 2018/05/06 at 3:46 PM

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class InternetExtensions {

        public static async Task<TextReader> DoRequestAsync( this WebRequest request ) {
            if ( request is null ) {
                throw new ArgumentNullException(nameof( request ) );
            }

            var result = await Task.Factory.FromAsync( beginMethod: ( asyncCallback, state ) => ( ( HttpWebRequest )state ).BeginGetResponse( callback: asyncCallback, state: state ),
                endMethod: asyncResult => ( ( HttpWebRequest )asyncResult.AsyncState ).EndGetResponse( asyncResult: asyncResult ), state: request );
            var stream = result.GetResponseStream();
            return stream != null ? new StreamReader( stream: stream ) : TextReader.Null;
        }

        public static async Task<TextReader> DoRequestAsync( this Uri uri ) {
            if ( uri is null ) {
                throw new ArgumentNullException(nameof( uri ) );
            }

            var request = WebRequest.CreateHttp( requestUri: uri );

            //request.AllowReadStreamBuffering = true;
            var textReader = await request.DoRequestAsync();
            return textReader;
        }

        public static async Task<T> DoRequestJsonAsync<T>( this WebRequest request ) {
            if ( request is null ) {
                throw new ArgumentNullException(nameof( request ) );
            }

            var reader = await DoRequestAsync( request: request ).ConfigureAwait( false );
            var response = await reader.ReadToEndAsync().ConfigureAwait( false );
            return JsonConvert.DeserializeObject<T>( value: response );
        }

        public static async Task<T> DoRequestJsonAsync<T>( Uri uri ) {
            var reader = await DoRequestAsync( uri: uri ).ConfigureAwait( false );
            var response = await reader.ReadToEndAsync().ConfigureAwait( false );
            return JsonConvert.DeserializeObject<T>( value: response );
        }

        /// <summary>Convert network bytes to a string</summary>
        /// <exception cref="ArgumentException"></exception>
        public static String FromNetworkBytes( this IEnumerable<Byte> data ) {
            var listData = data as IList<Byte> ?? data.ToList();

            var len = IPAddress.NetworkToHostOrder( network: BitConverter.ToInt16( value: listData.Take( count: 2 ).ToArray(), startIndex: 0 ) );
            if ( listData.Count < 2 + len ) {
                throw new ArgumentException( message: "Too few bytes in packet" );
            }

            return Encoding.UTF8.GetString( bytes: listData.Skip( count: 2 ).Take( count: len ).ToArray() );
        }

        /// <summary>Return the machine's hostname</summary>
        public static String GetHostName() => Dns.GetHostName();

        public static JObject GetNonAsync( Uri uri ) {
            var httpClient = new HttpClient();
            var content = httpClient.GetStringAsync( requestUri: uri ).Result;
            return JObject.Parse( json: content );
        }

        [CanBeNull]
        public static String GetWebPage( this String url ) {
            try {
                var request = WebRequest.Create( requestUriString: url );
                request.Proxy = null;
                request.Credentials = CredentialCache.DefaultCredentials;

                using ( var response = request.GetResponse() as HttpWebResponse ) {
                    var dataStream = response?.GetResponseStream();
                    if ( dataStream != null ) {
                        try {
                            using ( var reader = new StreamReader( stream: dataStream ) ) {
                                var responseFromServer = reader.ReadToEnd();
                                return responseFromServer;
                            }
                        }
                        finally {
                            dataStream.Dispose();
                        }
                    }
                }
            }
            catch {
                throw new Exception( message: $"Unable to connect to {url}." );
            }

            return null;
        }

        public static async Task<String> GetWebPageAsync( this Uri uri ) {
            try {
                var request = WebRequest.Create( requestUri: uri );
                request.Proxy = null;
                request.Credentials = CredentialCache.DefaultCredentials;
                using ( var response = await request.GetResponseAsync().ConfigureAwait( false ) ) {
                    using ( var dataStream = response.GetResponseStream() ) {
                        if ( dataStream != null ) {
                            using ( var reader = new StreamReader( stream: dataStream ) ) {
                                var responseFromServer = reader.ReadToEnd();
                                return responseFromServer;
                            }
                        }
                    }
                }
            }
            catch {
                $"Unable to connect to {uri}.".Error();
            }

            return null;
        }

        /// <summary>Convert a string to network bytes</summary>
        public static IEnumerable<Byte> ToNetworkBytes( this String data ) {
            var bytes = Encoding.UTF8.GetBytes( s: data );

            var len = IPAddress.HostToNetworkOrder( host: ( Int16 )bytes.Length );

            return BitConverter.GetBytes( value: len ).Concat( second: bytes );
        }
    }
}