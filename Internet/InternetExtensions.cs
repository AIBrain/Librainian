// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "InternetExtensions.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// 
// "Librainian/Librainian/InternetExtensions.cs" was last formatted by Protiguous on 2018/05/23 at 9:19 PM.

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Threading;

    public static class InternetExtensions {

        public static async Task<TextReader> DoRequestAsync( this WebRequest request ) {
            if ( request is null ) { throw new ArgumentNullException( nameof( request ) ); }

            var result = await Task.Factory.FromAsync( beginMethod: ( asyncCallback, state ) => ( ( HttpWebRequest ) state ).BeginGetResponse( callback: asyncCallback, state: state ),
                endMethod: asyncResult => ( ( HttpWebRequest ) asyncResult.AsyncState ).EndGetResponse( asyncResult: asyncResult ), state: request );

            var stream = result.GetResponseStream();

            return stream != null ? new StreamReader( stream: stream ) : TextReader.Null;
        }

        public static async Task<TextReader> DoRequestAsync( this Uri uri ) {
            if ( uri is null ) { throw new ArgumentNullException( nameof( uri ) ); }

            var request = WebRequest.CreateHttp( requestUri: uri );

            //request.AllowReadStreamBuffering = true;
            var textReader = await request.DoRequestAsync();

            return textReader;
        }

        public static async Task<T> DoRequestJsonAsync<T>( this WebRequest request ) {
            if ( request is null ) { throw new ArgumentNullException( nameof( request ) ); }

            var reader = await DoRequestAsync( request: request ).NoUI();
            var response = await reader.ReadToEndAsync().NoUI();

            return JsonConvert.DeserializeObject<T>( response );
        }

        public static async Task<T> DoRequestJsonAsync<T>( Uri uri ) {
            var reader = await DoRequestAsync( uri: uri ).NoUI();
            var response = await reader.ReadToEndAsync().NoUI();

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

        /*
        public static JObject GetNonAsync( Uri uri ) {
            var httpClient = new HttpClient();
            var content = httpClient.GetStringAsync( uri ).Result;

            return JObject.Parse( json: content );
        }
        */

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
                        finally { dataStream.Dispose(); }
                    }
                }
            }
            catch { throw new Exception( $"Unable to connect to {url}." ); }

            return null;
        }

        public static async Task<String> GetWebPageAsync( this Uri uri ) {
            try {
                var request = WebRequest.Create( requestUri: uri );
                request.Proxy = null;
                request.Credentials = CredentialCache.DefaultCredentials;

                using ( var response = await request.GetResponseAsync().NoUI() ) {
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
            catch { $"Unable to connect to {uri}.".Error(); }

            return null;
        }

        /// <summary>Convert a string to network bytes</summary>
        public static IEnumerable<Byte> ToNetworkBytes( this String data ) {
            var bytes = Encoding.UTF8.GetBytes( s: data );

            var len = IPAddress.HostToNetworkOrder( host: ( Int16 ) bytes.Length );

            return BitConverter.GetBytes( len ).Concat( second: bytes );
        }

    }

}