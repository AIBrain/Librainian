// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "OAuthBase.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "OAuthBase.cs" was last formatted by Protiguous on 2020/03/16 at 2:55 PM.

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using JetBrains.Annotations;

    public class OAuthBase {

        protected const String Hmacsha1SignatureType = "HMAC-SHA1";

        protected const String OAuthCallbackKey = "oauth_callback";

        //
        // List of know and used oauth parameters' names
        //
        protected const String OAuthConsumerKeyKey = "oauth_consumer_key";

        protected const String OAuthNonceKey = "oauth_nonce";

        protected const String OAuthParameterPrefix = "oauth_";

        protected const String OAuthSignatureKey = "oauth_signature";

        protected const String OAuthSignatureMethodKey = "oauth_signature_method";

        protected const String OAuthTimestampKey = "oauth_timestamp";

        protected const String OAuthTokenKey = "oauth_token";

        protected const String OAuthTokenSecretKey = "oauth_token_secret";

        protected const String OAuthVersion = "1.0";

        protected const String OAuthVersionKey = "oauth_version";

        protected const String PlainTextSignatureType = "PLAINTEXT";

        protected const String Rsasha1SignatureType = "RSA-SHA1";

        protected const String UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        protected readonly Random Random = new Random();

        /// <summary>Provides a predefined set of algorithms that are supported officially by the protocol</summary>
        public enum SignatureTypes {

            Hmacsha1,

            Plaintext,

            Rsasha1
        }

        /// <summary>Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")</summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        [NotNull]
        private static List<QueryParameter> GetQueryParameters( String parameters ) {
            if ( parameters.StartsWith( value: "?" ) ) {
                parameters = parameters.Remove( startIndex: 0, count: 1 );
            }

            var result = new List<QueryParameter>();

            if ( !String.IsNullOrEmpty( value: parameters ) ) {
                var p = parameters.Split( '&' );

                foreach ( var s in p.Where( predicate: s => !String.IsNullOrEmpty( value: s ) && !s.StartsWith( value: OAuthParameterPrefix ) ) ) {
                    if ( s.Contains( value: "=" ) ) {
                        var temp = s.Split( '=' );
                        result.Add( item: new QueryParameter( name: temp[ 0 ], value: temp[ 1 ] ) );
                    }
                    else {
                        result.Add( item: new QueryParameter( name: s, value: String.Empty ) );
                    }
                }
            }

            return result;
        }

        /// <summary>Helper function to compute a hash value</summary>
        /// <param name="hashAlgorithm">
        /// The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it
        /// to this function
        /// </param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        [NotNull]
        private String ComputeHash( [NotNull] HashAlgorithm hashAlgorithm, [NotNull] String data ) {
            if ( hashAlgorithm is null ) {
                throw new ArgumentNullException( paramName: nameof( hashAlgorithm ) );
            }

            if ( String.IsNullOrEmpty( value: data ) ) {
                throw new ArgumentNullException( paramName: nameof( data ) );
            }

            var dataBuffer = Encoding.ASCII.GetBytes( s: data );
            var hashBytes = hashAlgorithm.ComputeHash( buffer: dataBuffer );

            return Convert.ToBase64String( inArray: hashBytes );
        }

        /// <summary>Normalizes the request parameters according to the spec</summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        [NotNull]
        protected static String NormalizeRequestParameters( [NotNull] IList<QueryParameter> parameters ) {
            var sb = new StringBuilder();

            for ( var i = 0; i < parameters.Count; i++ ) {
                var p = parameters[ index: i ];
                sb.Append( value: $"{p.Name}={p.Value}" );

                if ( i < parameters.Count - 1 ) {
                    sb.Append( value: "&" );
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case. While this is not a problem with the percent encoding
        /// spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        [NotNull]
        public static String UrlEncode( [NotNull] String value ) {
            var result = new StringBuilder( capacity: value.Length * 3 );

            foreach ( var symbol in value ) {
                if ( UnreservedChars.IndexOf( value: symbol ) != -1 ) {
                    result.Append( value: symbol );
                }
                else {
                    result.Append( value: $"{'%'}{( Int32 )symbol:X2}" );
                }
            }

            return result.ToString();
        }

        /// <summary>Generate a nonce</summary>
        /// <returns></returns>
        [NotNull]
        public virtual String GenerateNonce() => this.Random.Next( minValue: 123400, maxValue: 9999999 ).ToString();

        /// <summary>Generates a signature using the HMAC-SHA1 algorithm</summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="timeStamp"></param>
        /// <param name="nonce"></param>
        /// <param name="normalizedUrl"></param>
        /// <param name="normalizedRequestParameters"></param>
        /// <returns>A base64 string of the hash value</returns>
        [NotNull]
        public String GenerateSignature( [NotNull] Uri url, [NotNull] String consumerKey, [NotNull] String consumerSecret, [CanBeNull] String? token,
            [CanBeNull] String? tokenSecret, [NotNull] String httpMethod, [CanBeNull] String? timeStamp, [CanBeNull] String? nonce, [CanBeNull] out String normalizedUrl,
            [CanBeNull] out String normalizedRequestParameters ) {
            if ( url == null ) {
                throw new ArgumentNullException( paramName: nameof( url ) );
            }

            if ( String.IsNullOrEmpty( value: consumerKey ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( consumerKey ) );
            }

            if ( String.IsNullOrEmpty( value: consumerSecret ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( consumerSecret ) );
            }

            if ( String.IsNullOrWhiteSpace( value: httpMethod ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( httpMethod ) );
            }

            return this.GenerateSignature( url: url, consumerKey: consumerKey, consumerSecret: consumerSecret, token: token, tokenSecret: tokenSecret, httpMethod: httpMethod,
                timeStamp: timeStamp, nonce: nonce, signatureType: SignatureTypes.Hmacsha1, normalizedUrl: out normalizedUrl,
                normalizedRequestParameters: out normalizedRequestParameters );
        }

        /// <summary>Generates a signature using the specified signatureType</summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce"></param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <param name="timeStamp"></param>
        /// <param name="normalizedUrl"></param>
        /// <param name="normalizedRequestParameters"></param>
        /// <returns>A base64 string of the hash value</returns>
        [NotNull]
        public String GenerateSignature( [NotNull] Uri url, [NotNull] String consumerKey, [NotNull] String consumerSecret, [CanBeNull] String? token,
            [CanBeNull] String? tokenSecret, [NotNull] String httpMethod, [CanBeNull] String? timeStamp, [CanBeNull] String? nonce, SignatureTypes signatureType,
            [CanBeNull] out String normalizedUrl, [CanBeNull] out String normalizedRequestParameters ) {
            if ( url == null ) {
                throw new ArgumentNullException( paramName: nameof( url ) );
            }

            if ( String.IsNullOrEmpty( value: consumerKey ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( consumerKey ) );
            }

            if ( String.IsNullOrEmpty( value: consumerSecret ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( consumerSecret ) );
            }

            if ( String.IsNullOrWhiteSpace( value: httpMethod ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( httpMethod ) );
            }

            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch ( signatureType ) {
                case SignatureTypes.Plaintext: return HttpUtility.UrlEncode( str: $"{consumerSecret}&{tokenSecret}" );

                case SignatureTypes.Hmacsha1:

                    var signatureBase = this.GenerateSignatureBase( url: url, consumerKey: consumerKey, token: token, tokenSecret: tokenSecret, httpMethod: httpMethod,
                        timeStamp: timeStamp, nonce: nonce, signatureType: Hmacsha1SignatureType, normalizedUrl: out normalizedUrl,
                        normalizedRequestParameters: out normalizedRequestParameters );

                    var hmacsha1 = new HMACSHA1 {
                        Key = Encoding.ASCII.GetBytes(
                            s: $"{UrlEncode( value: consumerSecret )}&{( String.IsNullOrEmpty( value: tokenSecret ) ? "" : UrlEncode( value: tokenSecret ) )}" )
                    };

                    return this.GenerateSignatureUsingHash( signatureBase: signatureBase, hash: hmacsha1 );

                case SignatureTypes.Rsasha1: throw new NotImplementedException();
                default: throw new ArgumentException( message: "Unknown signature type", paramName: nameof( signatureType ) );
            }
        }

        /// <summary>Generate the signature base that is used to produce the signature</summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce"></param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
        /// <param name="timeStamp"></param>
        /// <param name="normalizedUrl"></param>
        /// <param name="normalizedRequestParameters"></param>
        /// <returns>The signature base</returns>
        [NotNull]
        public String GenerateSignatureBase( [NotNull] Uri url, [NotNull] String consumerKey, String token, [CanBeNull] String? tokenSecret, [NotNull] String httpMethod,
            [CanBeNull] String? timeStamp, [CanBeNull] String? nonce, [NotNull] String signatureType, [NotNull] out String normalizedUrl,
            [NotNull] out String normalizedRequestParameters ) {
            if ( token is null ) {
                token = String.Empty;
            }

            if ( tokenSecret is null ) {

                //tokenSecret = String.Empty;
            }

            if ( String.IsNullOrEmpty( value: consumerKey ) ) {
                throw new ArgumentNullException( paramName: nameof( consumerKey ) );
            }

            if ( String.IsNullOrEmpty( value: httpMethod ) ) {
                throw new ArgumentNullException( paramName: nameof( httpMethod ) );
            }

            if ( String.IsNullOrEmpty( value: signatureType ) ) {
                throw new ArgumentNullException( paramName: nameof( signatureType ) );
            }

            var parameters = GetQueryParameters( parameters: url.Query );
            parameters.Add( item: new QueryParameter( name: OAuthVersionKey, value: OAuthVersion ) );
            parameters.Add( item: new QueryParameter( name: OAuthNonceKey, value: nonce ) );
            parameters.Add( item: new QueryParameter( name: OAuthTimestampKey, value: timeStamp ) );
            parameters.Add( item: new QueryParameter( name: OAuthSignatureMethodKey, value: signatureType ) );
            parameters.Add( item: new QueryParameter( name: OAuthConsumerKeyKey, value: consumerKey ) );

            if ( !String.IsNullOrEmpty( value: token ) ) {
                parameters.Add( item: new QueryParameter( name: OAuthTokenKey, value: token ) );
            }

            parameters.Sort( comparer: new QueryParameterComparer() );

            normalizedUrl = $"{url.Scheme}://{url.Host}";

            if ( !( url.Scheme == "http" && url.Port == 80 || url.Scheme == "https" && url.Port == 443 ) ) {
                normalizedUrl += $":{url.Port}";
            }

            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters( parameters: parameters );

            var signatureBase = new StringBuilder();
            signatureBase.Append( value: $"{httpMethod.ToUpper()}&" );
            signatureBase.Append( value: $"{UrlEncode( value: normalizedUrl )}&" );
            signatureBase.Append( value: $"{UrlEncode( value: normalizedRequestParameters )}" );

            return signatureBase.ToString();
        }

        /// <summary>Generate the signature value based on the given signature base and hash algorithm</summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 string of the hash value</returns>
        [NotNull]
        public String GenerateSignatureUsingHash( [NotNull] String signatureBase, [NotNull] HashAlgorithm hash ) =>
            this.ComputeHash( hashAlgorithm: hash, data: signatureBase );

        /// <summary>Generate the timestamp for the signature</summary>
        /// <returns></returns>
        [NotNull]
        public virtual String GenerateTimeStamp() {

            // Default implementation of UNIX time of the current UTC time
            var ts = DateTime.UtcNow - new DateTime( year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, millisecond: 0 );

            return Convert.ToInt64( value: ts.TotalSeconds ).ToString();
        }

        /// <summary>Provides an internal structure to sort the query parameter</summary>
        protected class QueryParameter {

            public String Name { get; }

            public String Value { get; }

            public QueryParameter( [CanBeNull] String? name, [CanBeNull] String? value ) {
                this.Name = name;
                this.Value = value;
            }
        }

        /// <summary>Comparer class used to perform the sorting of the query parameters</summary>
        protected class QueryParameterComparer : IComparer<QueryParameter> {

            public Int32 Compare( QueryParameter x, QueryParameter y ) =>
                x?.Name == y?.Name ? String.CompareOrdinal( strA: x?.Value, strB: y?.Value ) : String.CompareOrdinal( strA: x?.Name, strB: y?.Name );
        }
    }
}