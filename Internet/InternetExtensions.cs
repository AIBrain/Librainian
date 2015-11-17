// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/InternetExtensions.cs" was last cleaned by Rick on 2015/06/12 at 2:56 PM

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Web.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class InternetExtensions {

        /// <summary>Convert network bytes to a string</summary>
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

        /// <summary>Convert a string to network bytes</summary>
        public static IEnumerable<Byte> ToNetworkBytes( this String data ) {
            var bytes = Encoding.UTF8.GetBytes( data );

            var len = IPAddress.HostToNetworkOrder( ( Int16 )bytes.Length );

            return BitConverter.GetBytes( len ).Concat( bytes );
        }

        public static String GetWebPage( this String url ) {
            try {
                var request = WebRequest.Create( url );
                request.Proxy = null;
                request.Credentials = CredentialCache.DefaultCredentials;

                using ( var response = request.GetResponse() as HttpWebResponse ) {
                    if ( response != null ) {
                        using ( var dataStream = response.GetResponseStream() ) {
                            if ( dataStream != null ) {
                                using ( var reader = new StreamReader( dataStream ) ) {
                                    var responseFromServer = reader.ReadToEnd();
                                    return responseFromServer;
                                }
                            }
                        }
                        response.Close();
                    }
                }
            }
            catch {
                throw new Exception( $"Unable to connect to {url}." );
            }
            return null;
        }

        public static async Task<String> GetWebPageAsync( this Uri url ) {
            try {
                var request = WebRequest.Create( url );
                request.Proxy = null;
                request.Credentials = CredentialCache.DefaultCredentials;
                using ( var response = await request.GetResponseAsync() ) {
                    using ( var dataStream = response.GetResponseStream() ) {
                        if ( dataStream != null ) {
                            using ( var reader = new StreamReader( dataStream ) ) {
                                var responseFromServer = reader.ReadToEnd();
                                return responseFromServer;
                            }
                        }
                    }
                }
            }
            catch {
                $"Unable to connect to {url}.".Error();
            }
            return null;
        }

        public static async Task<TextReader> DoRequestAsync( this WebRequest request ) {
            if ( request == null ) {
                throw new ArgumentNullException( nameof( request ) );
            }
            var result = await Task.Factory.FromAsync(
                ( asyncCallback, state ) => (
                ( HttpWebRequest )state ).BeginGetResponse( asyncCallback, state ),
                asyncResult => ( ( HttpWebRequest )asyncResult.AsyncState ).EndGetResponse( asyncResult ), request );
            var stream = result.GetResponseStream();
            return stream != null ? new StreamReader( stream ) : TextReader.Null;
        }

        public static async Task<TextReader> DoRequestAsync( this Uri uri ) {
            if ( uri == null ) {
                throw new ArgumentNullException( nameof( uri ) );
            }
            var request = WebRequest.CreateHttp( uri );
            //request.AllowReadStreamBuffering = true;
            var textReader = await request.DoRequestAsync();
            return textReader;
        }

        public static async Task<T> DoRequestJsonAsync<T>( this WebRequest request ) {
            if ( request == null ) {
                throw new ArgumentNullException( nameof( request ) );
            }
            var reader = await DoRequestAsync( request );
            var response = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>( response );
        }

        public static async Task<T> DoRequestJsonAsync<T>( Uri uri ) {
            var reader = await DoRequestAsync( uri );
            var response = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>( response );
        }

        public static JObject GetNonAsync( Uri uri ) {
            var httpClient = new HttpClient( );
            var content = httpClient.GetStringAsync( uri ).GetResults();
            return JObject.Parse( content );
        }
    }
}