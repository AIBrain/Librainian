// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/HttpProcessor.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Internet.Servers {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Web;
    using Magic;
    using Maths;

    public class HttpProcessor : ABetterClassDispose {
        private const Int32 BufSize = 4096;

        private static readonly Int32 MaxPostSize = 10 * ( Int32 )Constants.Sizes.OneMegaByte;

        // 10MB
        private readonly X509Certificate2 _sslCertificate;

        private Stream _inputStream;

        private Int32 _isLanConnection = -1;

        private String _remoteIpAddress;

        private Byte[] _remoteIpAddressBytes;

        /// <summary>
        /// A flag that is set when WriteSuccess(), WriteFailure(), or WriteRedirect() is called. If the
        /// </summary>
        private Boolean _responseWritten;

        /// <summary>
        /// A Dictionary mapping http header names to values. Names are all converted to lower case before being added to this Dictionary.
        /// </summary>
        public readonly Dictionary<String, String> HttpHeaders = new Dictionary<String, String>();

        /// <summary>
        /// The cookies to send to the remote client.
        /// </summary>
        public readonly Cookies ResponseCookies = new Cookies();

        public readonly Boolean SecureHttps;

        /// <summary>
        /// The HttpServer instance that accepted this request.
        /// </summary>
        public readonly HttpServer Srv;

        /// <summary>
        /// The underlying tcpClient which handles the network connection.
        /// </summary>
        public readonly TcpClient TcpClient;

        /// <summary>
        /// The base Uri for this Server, containing its host name and port.
        /// </summary>
        public Uri BaseUriThisServer;

        /// <summary>
        /// The Http method used. i.e. "POST" or "GET"
        /// </summary>
        public String HttpMethod;

        /// <summary>
        /// The protocol version String sent by the client. e.g. "HTTP/1.1"
        /// </summary>
        public String HttpProtocolVersionstring;

        /// <summary>
        /// Be careful to flush each output stream before using a different one!! This stream is for writing text data.
        /// </summary>
        public StreamWriter OutputStream;

        /// <summary>
        /// A String array containing the directories and the page name. For example, if the URL was "/articles/science/moon.html?date=2011-10-21", pathParts would be { "articles", "science", "moon.html" }
        /// </summary>
        public String[] PathParts;

        /// <summary>
        /// A SortedList mapping lower-case keys to values of parameters. This list is populated if and only if the request was a POST request with mimetype "application/x-www-form-urlencoded".
        /// </summary>
        public SortedList<String, String> PostParams = new SortedList<String, String>();

        /// <summary>
        /// A SortedList mapping lower-case keys to values of parameters. This list is populated parameters that were appended to the url (the query String). e.g. if the url is "mypage.html?arg1=value1&amp;arg2=value2",
        /// then there will be two parameters ("arg1" with value "value1" and "arg2" with value "value2"
        /// </summary>
        public SortedList<String, String> QueryString = new SortedList<String, String>();

        /// <summary>
        /// Be careful to flush each output stream before using a different one!! This stream is for writing binary data.
        /// </summary>
        public BufferedStream RawOutputStream;

        /// <summary>
        /// A SortedList mapping keys to values of parameters. No character case conversion is applied in this list. This list is populated if and only if the request was a POST request with mimetype "application/x-www-form-urlencoded".
        /// </summary>
        public SortedList<String, String> RawPostParams = new SortedList<String, String>();

        /// <summary>
        /// A SortedList mapping keys to values of parameters. No character case conversion is applied in this list. This list is populated parameters that were appended to the url (the query String). e.g. if the url is
        /// "mypage.html?arg1=value1&amp;arg2=value2", then there will be two parameters ("arg1" with value "value1" and "arg2" with value "value2"
        /// </summary>
        public SortedList<String, String> RawQueryString = new SortedList<String, String>();

        /// <summary>
        /// The cookies sent by the remote client.
        /// </summary>
        public Cookies RequestCookies;

        /// <summary>
        /// The path to and name of the requested page, not including the first '/' For example, if the URL was "/articles/science/moon.html?date=2011-10-21", requestedPage would be "articles/science/moon.html"
        /// </summary>
        public String RequestedPage;

        /// <summary>
        /// The requested url.
        /// </summary>
        public Uri RequestUrl;

        public HttpProcessor( TcpClient s, HttpServer srv, X509Certificate2 sslCertificate = null ) {
            this._sslCertificate = sslCertificate;
            this.SecureHttps = sslCertificate != null;
            this.TcpClient = s;
            this.BaseUriThisServer = new Uri( "http" + ( this.SecureHttps ? "s" : "" ) + "://" + s.Client.LocalEndPoint, UriKind.Absolute );
            this.Srv = srv;
        }

        private Byte[] RemoteIpAddressBytes {
            get {
                if ( this._remoteIpAddressBytes != null ) {
                    return this._remoteIpAddressBytes;
                }
                try {
                    if ( this.TcpClient != null ) {
                        if ( IPAddress.TryParse( this.RemoteIpAddress, out var remoteAddress ) && remoteAddress.AddressFamily == AddressFamily.InterNetwork ) {
                            this._remoteIpAddressBytes = remoteAddress.GetAddressBytes();
                        }
                    }
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.Log( ex );
                }
                return this._remoteIpAddressBytes;
            }
        }

        /// <summary>
        /// Returns the remote client's IP address, or an empty String if the remote IP address is somehow not available.
        /// </summary>
        public String RemoteIpAddress {
            get {
                if ( !String.IsNullOrEmpty( this._remoteIpAddress ) ) {
                    return this._remoteIpAddress;
                }
                try {
                    if ( this.TcpClient != null ) {
                        this._remoteIpAddress = this.TcpClient.Client.RemoteEndPoint.ToString().Split( ':' )[0];
                    }
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.Log( ex );
                }
                return this._remoteIpAddress;
            }
        }

        /// <summary>
        /// Parses the specified query String and returns a sorted list containing the arguments found in the specified query String. Can also be used to parse the POST request body if the mimetype is "application/x-www-form-urlencoded".
        /// </summary>
        /// <param name="queryString">             </param>
        /// <param name="requireQuestionMark">     </param>
        /// <param name="preserveKeyCharacterCase"></param>
        /// <returns></returns>
        private static SortedList<String, String> ParseQueryStringArguments( String queryString, Boolean requireQuestionMark = true, Boolean preserveKeyCharacterCase = false ) {
            var arguments = new SortedList<String, String>();
            var idx = queryString.IndexOf( '?' );
            if ( idx > -1 ) {
                queryString = queryString.Substring( idx + 1 );
            }
            else if ( requireQuestionMark ) {
                return arguments;
            }
            idx = queryString.LastIndexOf( '#' );
            String hash = null;
            if ( idx > -1 ) {
                hash = queryString.Substring( idx + 1 );
                queryString = queryString.Remove( idx );
            }
            var parts = queryString.Split( '&' );
            foreach ( var t in parts ) {
                var argument = t.Split( '=' );
                if ( argument.Length == 2 ) {
                    var key = HttpUtility.UrlDecode( argument[0] );
                    if ( null != key ) {
                        if ( !preserveKeyCharacterCase ) {
                            key = key.ToLower();
                        }
                        if ( arguments.TryGetValue( key, out var existingValue ) ) {
                            arguments[key] += "," + HttpUtility.UrlDecode( argument[1] );
                        }
                        else {
                            arguments[key] = HttpUtility.UrlDecode( argument[1] );
                        }
                    }
                }
            }
            if ( hash != null ) {
                arguments["#"] = hash;
            }
            return arguments;
        }

        //    }
        //    return new NetworkCredential();
        //}
        /// <summary>
        /// Asks the HttpServer to handle this request as a GET request. If the HttpServer does not write a response code header, this will write a generic failure header.
        /// </summary>
        private void HandleGetRequest() {
            try {
                this.Srv.HandleGetRequest( this );
            }
            finally {
                if ( !this._responseWritten ) {
                    this.WriteFailure();
                }
            }
        }

        // The following function was the start of an attempt to support basic authentication, but I have since decided against it as basic authentication is very insecure.
        //private NetworkCredential ParseAuthorizationCredentials()
        //{
        //    String auth = this.httpHeaders["Authorization"].ToString();
        //    if (auth != null && auth.StartsWith("Basic "))
        //    {
        //        byte[] bytes =  System.Convert.FromBase64String(auth.Substring(6));
        //        String creds = ASCIIEncoding.ASCII.GetString(bytes);
        /// <summary>
        /// This post data processing just reads everything into a memory stream. This is fine for smallish things, but for large stuff we should really hand an input stream to the request processor. However, the input
        /// stream we hand to the user's code needs to see the "end of the stream" at this content length, because otherwise it won't know where the end is! If the HttpServer does not write a response code header, this
        /// will write a generic failure header.
        /// </summary>
        private void HandlePostRequest() {
            try {
                var ms = new MemoryStream();
                var contentLengthStr = this.GetHeaderValue( "Content-Length" );
                if ( !String.IsNullOrWhiteSpace( contentLengthStr ) ) {
                    if ( Int32.TryParse( contentLengthStr, out var contentLen ) ) {
                        if ( contentLen > MaxPostSize ) {
                            this.WriteFailure( "413 Request Entity Too Large", "Request Too Large" );
                            SimpleHttpLogger.LogVerbose( "POST Content-Length(" + contentLen + ") too big for this simple Server.  Server can handle up to " + MaxPostSize );
                            return;
                        }
                        var buf = new Byte[BufSize];
                        var toRead = contentLen;
                        while ( toRead > 0 ) {
                            var numread = this._inputStream.Read( buf, 0, Math.Min( BufSize, toRead ) );
                            if ( numread == 0 ) {
                                if ( toRead == 0 ) {
                                    break;
                                }
                                SimpleHttpLogger.LogVerbose( "client disconnected during post" );
                                return;
                            }
                            toRead -= numread;
                            ms.Write( buf, 0, numread );
                        }
                        ms.Seek( 0, SeekOrigin.Begin );
                    }
                }
                else {
                    this.WriteFailure( "411 Length Required", "The request did not specify the length of its content." );
                    SimpleHttpLogger.LogVerbose( "The request did not specify the length of its content.  This Server requires that all POST requests include a Content-Length header." );
                    return;
                }

                var contentType = this.GetHeaderValue( "Content-Type" );
                if ( contentType != null && contentType.Contains( "application/x-www-form-urlencoded" ) ) {
                    var sr = new StreamReader( ms );
                    var all = sr.ReadToEnd();
                    sr.Close();

                    this.RawPostParams = ParseQueryStringArguments( all, false, true );
                    this.PostParams = ParseQueryStringArguments( all, false );

                    this.Srv.HandlePostRequest( this, null );
                }
                else {
                    this.Srv.HandlePostRequest( this, new StreamReader( ms ) );
                }
            }
            finally {
                try {
                    if ( !this._responseWritten ) {
                        this.WriteFailure();
                    }
                }
                catch ( Exception ) {

                    // ignored
                }
            }
        }

        /// <summary>
        /// Parses the first line of the http request to get the request method, url, and protocol version.
        /// </summary>
        private void ParseRequest() {
            var request = StreamReadLine( this._inputStream );
            var tokens = request.Split( ' ' );
            if ( tokens.Length != 3 ) {
                throw new Exception( "invalid http request line: " + request );
            }
            this.HttpMethod = tokens[0].ToUpper();

            if ( tokens[1].StartsWith( "http://" ) || tokens[1].StartsWith( "https://" ) ) {
                this.RequestUrl = new Uri( tokens[1] );
            }
            else {
                this.RequestUrl = new Uri( this.BaseUriThisServer, tokens[1] );
            }

            this.HttpProtocolVersionstring = tokens[2];
        }

        /// <summary>
        /// Parses the http headers
        /// </summary>
        private void ReadHeaders() {
            String line;
            while ( ( line = StreamReadLine( this._inputStream ) ) != "" ) {
                var separator = line.IndexOf( ':' );
                if ( separator == -1 ) {
                    throw new Exception( "invalid http header line: " + line );
                }
                var name = line.Substring( 0, separator );
                var pos = separator + 1;
                while ( pos < line.Length && line[pos] == ' ' ) {
                    pos++; // strip any spaces
                }

                var value = line.Substring( pos, line.Length - pos );
                this.HttpHeaders[name.ToLower()] = value;
            }
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        protected override void DisposeManaged() {
            this.OutputStream?.Dispose();
            this.RawOutputStream?.Dispose();
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        internal void Process( Object objParameter ) {
            Stream tcpStream = null;
            try {
                tcpStream = this.TcpClient.GetStream();
                if ( this.SecureHttps ) {
                    try {
                        tcpStream = new SslStream( tcpStream, false, null, null );
                        ( ( SslStream )tcpStream ).AuthenticateAsServer( this._sslCertificate );
                    }
                    catch ( Exception ex ) {
                        SimpleHttpLogger.LogVerbose( ex );
                        return;
                    }
                }
                this._inputStream = new BufferedStream( tcpStream );
                this.RawOutputStream = new BufferedStream( tcpStream );
                this.OutputStream = new StreamWriter( this.RawOutputStream );
                try {
                    this.ParseRequest();
                    this.ReadHeaders();
                    this.RawQueryString = ParseQueryStringArguments( this.RequestUrl.Query, preserveKeyCharacterCase: true );
                    this.QueryString = ParseQueryStringArguments( this.RequestUrl.Query );
                    this.RequestCookies = Cookies.FromString( this.GetHeaderValue( "Cookie", "" ) );
                    try {
                        if ( this.HttpMethod.Equals( "GET" ) ) {
                            this.HandleGetRequest();
                        }
                        else if ( this.HttpMethod.Equals( "POST" ) ) {
                            this.HandlePostRequest();
                        }
                    }
                    catch ( Exception e ) {
                        if ( !IsOrdinaryDisconnectException( e ) ) {
                            SimpleHttpLogger.Log( e );
                        }
                        this.WriteFailure( "500 Internal Server Error" );
                    }
                }
                catch ( Exception e ) {
                    if ( !IsOrdinaryDisconnectException( e ) ) {
                        SimpleHttpLogger.LogVerbose( e );
                    }
                    this.WriteFailure( "400 Bad Request", "The request cannot be fulfilled due to bad syntax." );
                }
                this.OutputStream.Flush();
                this.RawOutputStream.Flush();
                this._inputStream = null;
                this.OutputStream = null;
                this.RawOutputStream = null;
            }
            catch ( Exception ex ) {
                if ( !IsOrdinaryDisconnectException( ex ) ) {
                    SimpleHttpLogger.LogVerbose( ex );
                }
            }
            finally {
                try {
                    this.TcpClient?.Close();
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.LogVerbose( ex );
                }
                try {
                    tcpStream?.Close();
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.LogVerbose( ex );
                }
            }
        }

        /// <summary>
        /// Returns true if the remote client's ipv4 address is in the same class C range as any of the Server's ipv4 addresses.
        /// </summary>
        /// <param name="httpProcessor"></param>
        public static Boolean GetIsLanConnection( HttpProcessor httpProcessor ) {
            if ( httpProcessor._isLanConnection != -1 ) {
                return httpProcessor._isLanConnection == 1;
            }
            var remoteBytes = httpProcessor.RemoteIpAddressBytes;
            if ( remoteBytes is null || remoteBytes.Length != 4 ) {
                httpProcessor._isLanConnection = 0;
            }
            else if ( remoteBytes[0] == 127 && remoteBytes[1] == 0 && remoteBytes[2] == 0 && remoteBytes[3] == 1 ) {
                httpProcessor._isLanConnection = 1;
            }
            else {

                // If the first 3 bytes of any local address matches the first 3 bytes of the local address, then the remote address is in the same class C range as this address.
                foreach ( var localBytes in httpProcessor.Srv.LocalIPv4Addresses ) {
                    var addressIsMatch = true;
                    for ( var i = 0; i < 3; i++ ) {
                        if ( localBytes[i] != remoteBytes[i] ) {
                            addressIsMatch = false;
                            break;
                        }
                    }
                    if ( addressIsMatch ) {
                        httpProcessor._isLanConnection = 1;
                        break;
                    }
                }
                if ( httpProcessor._isLanConnection != 1 ) {
                    httpProcessor._isLanConnection = 0;
                }
            }
            return httpProcessor._isLanConnection == 1;
        }

        public static Boolean IsOrdinaryDisconnectException( Exception ex ) {
            if ( ex is IOException ) {
                if ( ex.InnerException is SocketException ) {
                    if ( ex.InnerException.Message.Contains( "An established connection was aborted by the software in your host machine" ) || ex.InnerException.Message.Contains( "An existing connection was forcibly closed by the remote host" ) || ex.InnerException.Message.Contains( "The socket has been shut down" ) /* Mono/Linux */ ) {
                        return true; // Connection aborted.  This happens often enough that reporting it can be excessive.
                    }
                }
            }
            return false;
        }

        public static String StreamReadLine( Stream inputStream ) {
            var data = new StringBuilder();
            while ( true ) {
                var nextChar = inputStream.ReadByte();
                if ( nextChar == '\n' ) {
                    break;
                }
                if ( nextChar == '\r' ) {
                    continue;
                }
                if ( nextChar == -1 ) {
                    break;
                }
                data.Append( Convert.ToChar( nextChar ) );
            }
            return data.ToString();
        }

        /// <summary>
        /// Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>
        /// The value of the key, or [defaultValue] if the key does not exist or has no suitable value. This function interprets a value of "1" or "true" (case insensitive) as being true. Any other parameter value is
        /// interpreted as false.
        /// </returns>
        public Boolean GetBoolParam( String key ) => this.GetQsBoolParam( key );

        /// <summary>
        /// Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">         A case insensitive key.</param>
        /// <param name="defaultValue"></param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public Double GetDoubleParam( String key, Int32 defaultValue = 0 ) => this.GetQsDoubleParam( key, defaultValue );

        /// <summary>
        /// Gets the value of the header, or null if the header does not exist. The name is case insensitive.
        /// </summary>
        /// <param name="name">        The case insensitive name of the header to get the value of.</param>
        /// <param name="defaultValue"></param>
        /// <returns>The value of the header, or null if the header did not exist.</returns>
        public String GetHeaderValue( String name, String defaultValue = null ) {
            name = name.ToLower();
            if ( !this.HttpHeaders.TryGetValue( name, out var value ) ) {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">         A case insensitive key.</param>
        /// <param name="defaultValue"></param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public Int32 GetIntParam( String key, Int32 defaultValue = 0 ) => this.GetQsIntParam( key, defaultValue );

        /// <summary>
        /// Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or empty String if the key does not exist or has no value.</returns>
        public String GetParam( String key ) => this.GetQsParam( key );

        /// <summary>
        /// Returns the value of a parameter sent via POST with MIME type "application/x-www-form-urlencoded".
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>
        /// The value of the key, or [defaultValue] if the key does not exist or has no suitable value. This function interprets a value of "1" or "true" (case insensitive) as being true. Any other parameter value is
        /// interpreted as false.
        /// </returns>
        public Boolean GetPostBoolParam( String key ) {
            var param = this.GetPostParam( key );
            if ( param == "1" || param.ToLower() == "true" ) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the value of a parameter sent via POST with MIME type "application/x-www-form-urlencoded".
        /// </summary>
        /// <param name="key">         A case insensitive key.</param>
        /// <param name="defaultValue"></param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public Double GetPostDoubleParam( String key, Double defaultValue = 0 ) {
            if ( key is null ) {
                return defaultValue;
            }
            if ( Double.TryParse( this.GetPostParam( key.ToLower() ), out var value ) ) {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Returns the value of a parameter sent via POST with MIME type "application/x-www-form-urlencoded".
        /// </summary>
        /// <param name="key">         A case insensitive key.</param>
        /// <param name="defaultValue"></param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public Int32 GetPostIntParam( String key, Int32 defaultValue = 0 ) {
            if ( key is null ) {
                return defaultValue;
            }
            if ( Int32.TryParse( this.GetPostParam( key.ToLower() ), out var value ) ) {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Returns the value of a parameter sent via POST with MIME type "application/x-www-form-urlencoded".
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or empty String if the key does not exist or has no value.</returns>
        public String GetPostParam( String key ) {
            if ( key is null ) {
                return "";
            }
            if ( this.PostParams.TryGetValue( key.ToLower(), out var value ) ) {
                return value;
            }
            return "";
        }

        /// <summary>
        /// Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>
        /// The value of the key, or [defaultValue] if the key does not exist or has no suitable value. This function interprets a value of "1" or "true" (case insensitive) as being true. Any other parameter value is
        /// interpreted as false.
        /// </returns>
        public Boolean GetQsBoolParam( String key ) {
            var param = this.GetQsParam( key );
            if ( param == "1" || param.ToLower() == "true" ) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">         A case insensitive key.</param>
        /// <param name="defaultValue"></param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public Double GetQsDoubleParam( String key, Double defaultValue = 0 ) {
            if ( key is null ) {
                return defaultValue;
            }
            return Double.TryParse( this.GetQsParam( key.ToLower() ), out var value ) ? value : defaultValue;
        }

        /// <summary>
        /// Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">         A case insensitive key.</param>
        /// <param name="defaultValue"></param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public Int32 GetQsIntParam( String key, Int32 defaultValue = 0 ) {
            if ( key is null ) {
                return defaultValue;
            }
            if ( Int32.TryParse( this.GetQsParam( key.ToLower() ), out var value ) ) {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or empty String if the key does not exist or has no value.</returns>
        public String GetQsParam( String key ) {
            if ( key is null ) {
                return "";
            }
            if ( this.QueryString.TryGetValue( key.ToLower(), out var value ) ) {
                return value;
            }
            return "";
        }

        /// <summary>
        /// Writes a failure response header. Call this one time to return an error response.
        /// </summary>
        /// <param name="code">       (OPTIONAL) The http error code (including explanation entity). For example: "404 Not Found" where 404 is the error code and "Not Found" is the explanation.</param>
        /// <param name="description">
        /// (OPTIONAL) A description String to send after the headers as the response. This is typically shown to the remote user in his browser. If null, the code String is sent here. If "", no response body is sent by
        /// this function, and you may or may not want to write your own.
        /// </param>
        public void WriteFailure( String code = "404 Not Found", String description = null ) {
            this._responseWritten = true;
            this.OutputStream.WriteLine( "HTTP/1.1 " + code );
            this.OutputStream.WriteLine( "Connection: close" );
            this.OutputStream.WriteLine( "" );
            if ( description is null ) {
                this.OutputStream.WriteLine( code );
            }
            else if ( description != "" ) {
                this.OutputStream.WriteLine( description );
            }
        }

        /// <summary>
        /// Writes a redirect header instructing the remote user's browser to load the URL you specify. Call this one time and do not write any other data to the response stream.
        /// </summary>
        /// <param name="redirectToUrl">URL to redirect to.</param>
        public void WriteRedirect( String redirectToUrl ) {
            this._responseWritten = true;
            this.OutputStream.WriteLine( "HTTP/1.1 302 Found" );
            this.OutputStream.WriteLine( "Location: " + redirectToUrl );
            this.OutputStream.WriteLine( "Connection: close" );
            this.OutputStream.WriteLine( "" );
        }

        /// <summary>
        /// Writes the response headers for a successful response. Call this one time before writing your response, after you have determined that the request is valid.
        /// </summary>
        /// <param name="contentType">      The MIME type of your response.</param>
        /// <param name="contentLength">    (OPTIONAL) The length of your response, in bytes, if you know it.</param>
        /// <param name="responseCode">     </param>
        /// <param name="additionalHeaders"></param>
        public void WriteSuccess( String contentType = "text/html", Int64 contentLength = -1, String responseCode = "200 OK", List<KeyValuePair<String, String>> additionalHeaders = null ) {
            this._responseWritten = true;
            this.OutputStream.WriteLine( "HTTP/1.1 " + responseCode );
            if ( !String.IsNullOrEmpty( contentType ) ) {
                this.OutputStream.WriteLine( "Content-Type: " + contentType );
            }
            if ( contentLength > -1 ) {
                this.OutputStream.WriteLine( "Content-Length: " + contentLength );
            }
            var cookieStr = this.ResponseCookies.ToString();
            if ( !String.IsNullOrEmpty( cookieStr ) ) {
                this.OutputStream.WriteLine( cookieStr );
            }
            if ( additionalHeaders != null ) {
                foreach ( var header in additionalHeaders ) {
                    this.OutputStream.WriteLine( header.Key + ": " + header.Value );
                }
            }
            this.OutputStream.WriteLine( "Connection: close" );
            this.OutputStream.WriteLine( "" );
        }
    }
}