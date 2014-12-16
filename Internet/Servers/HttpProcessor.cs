#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/HttpProcessor.cs" was last cleaned by Rick on 2014/09/08 at 3:57 AM

#endregion License & Information

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
    using Maths;

    public class HttpProcessor {

        /// <summary>
        ///     The cookies to send to the remote client.
        /// </summary>
        public readonly Cookies responseCookies = new Cookies();

        public readonly bool secure_https;

        /// <summary>
        ///     The HttpServer instance that accepted this request.
        /// </summary>
        public readonly HttpServer srv;

        /// <summary>
        ///     The underlying tcpClient which handles the network connection.
        /// </summary>
        public readonly TcpClient TcpClient;

        /// <summary>
        ///     The base Uri for this server, containing its host name and port.
        /// </summary>
        public Uri base_uri_this_server;

        /// <summary>
        ///     The Http method used.  i.e. "POST" or "GET"
        /// </summary>
        public String http_method;

        /// <summary>
        ///     The protocol version String sent by the client.  e.g. "HTTP/1.1"
        /// </summary>
        public String http_protocol_versionstring;

        /// <summary>
        ///     A Dictionary mapping http header names to values. Names are all converted to lower case before being added to this
        ///     Dictionary.
        /// </summary>
        public Dictionary<String, String> httpHeaders = new Dictionary<String, String>();

        /// <summary>
        ///     Be careful to flush each output stream before using a different one!!
        ///     This stream is for writing text data.
        /// </summary>
        public StreamWriter outputStream;

        /// <summary>
        ///     A String array containing the directories and the page name.
        ///     For example, if the URL was "/articles/science/moon.html?date=2011-10-21", pathParts would be { "articles",
        ///     "science", "moon.html" }
        /// </summary>
        public String[] pathParts;

        /// <summary>
        ///     A SortedList mapping lower-case keys to values of parameters.  This list is populated if and only if the request
        ///     was a POST request with mimetype "application/x-www-form-urlencoded".
        /// </summary>
        public SortedList<String, String> PostParams = new SortedList<String, String>();

        /// <summary>
        ///     A SortedList mapping lower-case keys to values of parameters.  This list is populated parameters that were appended
        ///     to the url (the query String).  e.g. if the url is "mypage.html?arg1=value1&arg2=value2", then there will be two
        ///     parameters ("arg1" with value "value1" and "arg2" with value "value2"
        /// </summary>
        public SortedList<String, String> QueryString = new SortedList<String, String>();

        /// <summary>
        ///     Be careful to flush each output stream before using a different one!!
        ///     This stream is for writing binary data.
        /// </summary>
        public BufferedStream rawOutputStream;

        /// <summary>
        ///     A SortedList mapping keys to values of parameters.  No character case conversion is applied in this list.  This
        ///     list is populated if and only if the request was a POST request with mimetype "application/x-www-form-urlencoded".
        /// </summary>
        public SortedList<String, String> RawPostParams = new SortedList<String, String>();

        /// <summary>
        ///     A SortedList mapping keys to values of parameters.  No character case conversion is applied in this list.  This
        ///     list is populated parameters that were appended to the url (the query String).  e.g. if the url is
        ///     "mypage.html?arg1=value1&arg2=value2", then there will be two parameters ("arg1" with value "value1" and "arg2"
        ///     with value "value2"
        /// </summary>
        public SortedList<String, String> RawQueryString = new SortedList<String, String>();

        /// <summary>
        ///     The requested url.
        /// </summary>
        public Uri request_url;

        /// <summary>
        ///     The cookies sent by the remote client.
        /// </summary>
        public Cookies requestCookies;

        /// <summary>
        ///     The path to and name of the requested page, not including the first '/'
        ///     For example, if the URL was "/articles/science/moon.html?date=2011-10-21", requestedPage would be
        ///     "articles/science/moon.html"
        /// </summary>
        public String requestedPage;

        private const int BUF_SIZE = 4096;
        private static int MAX_POST_SIZE = 10 * ( int )MathExtensions.OneMegaByte; // 10MB
        private readonly X509Certificate2 ssl_certificate;
        private Stream inputStream;
        private int isLanConnection = -1;
        private String remoteIPAddress;
        private byte[] remoteIPAddressBytes;

        /// <summary>
        ///     A flag that is set when WriteSuccess(), WriteFailure(), or WriteRedirect() is called.  If the
        /// </summary>
        private bool responseWritten;

        public HttpProcessor( TcpClient s, HttpServer srv, X509Certificate2 ssl_certificate = null ) {
            this.ssl_certificate = ssl_certificate;
            this.secure_https = ssl_certificate != null;
            this.TcpClient = s;
            this.base_uri_this_server = new Uri( "http" + ( this.secure_https ? "s" : "" ) + "://" + s.Client.LocalEndPoint, UriKind.Absolute );
            this.srv = srv;
        }

        /// <summary>
        ///     Returns the remote client's IP address, or an empty String if the remote IP address is somehow not available.
        /// </summary>
        public String RemoteIPAddress {
            get {
                if ( !String.IsNullOrEmpty( this.remoteIPAddress ) ) {
                    return this.remoteIPAddress;
                }
                try {
                    if ( this.TcpClient != null ) {
                        this.remoteIPAddress = this.TcpClient.Client.RemoteEndPoint.ToString().Split( ':' )[ 0 ];
                    }
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.Log( ex );
                }
                return this.remoteIPAddress;
            }
        }

        private byte[] RemoteIPAddressBytes {
            get {
                if ( this.remoteIPAddressBytes != null ) {
                    return this.remoteIPAddressBytes;
                }
                try {
                    if ( this.TcpClient != null ) {
                        IPAddress remoteAddress;
                        if ( IPAddress.TryParse( this.RemoteIPAddress, out remoteAddress ) && remoteAddress.AddressFamily == AddressFamily.InterNetwork ) {
                            this.remoteIPAddressBytes = remoteAddress.GetAddressBytes();
                        }
                    }
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.Log( ex );
                }
                return this.remoteIPAddressBytes;
            }
        }

        /// <summary>
        ///     Returns true if the remote client's ipv4 address is in the same class C range as any of the server's ipv4
        ///     addresses.
        /// </summary>
        /// <param name="httpProcessor"></param>
        public static bool GetIsLanConnection( HttpProcessor httpProcessor ) {
            if ( httpProcessor.isLanConnection != -1 ) {
                return httpProcessor.isLanConnection == 1;
            }
            var remoteBytes = httpProcessor.RemoteIPAddressBytes;
            if ( remoteBytes == null || remoteBytes.Length != 4 ) {
                httpProcessor.isLanConnection = 0;
            }
            else if ( remoteBytes[ 0 ] == 127 && remoteBytes[ 1 ] == 0 && remoteBytes[ 2 ] == 0 && remoteBytes[ 3 ] == 1 ) {
                httpProcessor.isLanConnection = 1;
            }
            else {

                // If the first 3 bytes of any local address matches the first 3 bytes of the local address, then the remote address is in the same class C range as this address.
                foreach ( var localBytes in httpProcessor.srv.LocalIPv4Addresses ) {
                    var addressIsMatch = true;
                    for ( var i = 0 ; i < 3 ; i++ ) {
                        if ( localBytes[ i ] != remoteBytes[ i ] ) {
                            addressIsMatch = false;
                            break;
                        }
                    }
                    if ( addressIsMatch ) {
                        httpProcessor.isLanConnection = 1;
                        break;
                    }
                }
                if ( httpProcessor.isLanConnection != 1 ) {
                    httpProcessor.isLanConnection = 0;
                }
            }
            return httpProcessor.isLanConnection == 1;
        }

        public static bool IsOrdinaryDisconnectException( Exception ex ) {
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
        ///     Gets the value of the header, or null if the header does not exist.  The name is case insensitive.
        /// </summary>
        /// <param name="name">The case insensitive name of the header to get the value of.</param>
        /// <returns>The value of the header, or null if the header did not exist.</returns>
        public String GetHeaderValue( String name, String defaultValue = null ) {
            name = name.ToLower();
            String value;
            if ( !this.httpHeaders.TryGetValue( name, out value ) ) {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        ///     Writes a failure response header.  Call this one time to return an error response.
        /// </summary>
        /// <param name="code">
        ///     (OPTIONAL) The http error code (including explanation entity).  For example: "404 Not Found" where
        ///     404 is the error code and "Not Found" is the explanation.
        /// </param>
        /// <param name="description">
        ///     (OPTIONAL) A description String to send after the headers as the response.  This is typically
        ///     shown to the remote user in his browser.  If null, the code String is sent here.  If "", no response body is sent
        ///     by this function, and you may or may not want to write your own.
        /// </param>
        public void writeFailure( String code = "404 Not Found", String description = null ) {
            this.responseWritten = true;
            this.outputStream.WriteLine( "HTTP/1.1 " + code );
            this.outputStream.WriteLine( "Connection: close" );
            this.outputStream.WriteLine( "" );
            if ( description == null ) {
                this.outputStream.WriteLine( code );
            }
            else if ( description != "" ) {
                this.outputStream.WriteLine( description );
            }
        }

        /// <summary>
        ///     Writes a redirect header instructing the remote user's browser to load the URL you specify.  Call this one time and
        ///     do not write any other data to the response stream.
        /// </summary>
        /// <param name="redirectToUrl">URL to redirect to.</param>
        public void writeRedirect( String redirectToUrl ) {
            this.responseWritten = true;
            this.outputStream.WriteLine( "HTTP/1.1 302 Found" );
            this.outputStream.WriteLine( "Location: " + redirectToUrl );
            this.outputStream.WriteLine( "Connection: close" );
            this.outputStream.WriteLine( "" );
        }

        /// <summary>
        ///     Writes the response headers for a successful response.  Call this one time before writing your response, after you
        ///     have determined that the request is valid.
        /// </summary>
        /// <param name="contentType">The MIME type of your response.</param>
        /// <param name="contentLength">(OPTIONAL) The length of your response, in bytes, if you know it.</param>
        public void writeSuccess( String contentType = "text/html", long contentLength = -1, String responseCode = "200 OK", List<KeyValuePair<String, String>> additionalHeaders = null ) {
            this.responseWritten = true;
            this.outputStream.WriteLine( "HTTP/1.1 " + responseCode );
            if ( !String.IsNullOrEmpty( contentType ) ) {
                this.outputStream.WriteLine( "Content-Type: " + contentType );
            }
            if ( contentLength > -1 ) {
                this.outputStream.WriteLine( "Content-Length: " + contentLength );
            }
            var cookieStr = this.responseCookies.ToString();
            if ( !String.IsNullOrEmpty( cookieStr ) ) {
                this.outputStream.WriteLine( cookieStr );
            }
            if ( additionalHeaders != null ) {
                foreach ( var header in additionalHeaders ) {
                    this.outputStream.WriteLine( header.Key + ": " + header.Value );
                }
            }
            this.outputStream.WriteLine( "Connection: close" );
            this.outputStream.WriteLine( "" );
        }

        /// <summary>
        ///     Processes the request.
        /// </summary>
        internal void Process( object objParameter ) {
            Stream tcpStream = null;
            try {
                tcpStream = this.TcpClient.GetStream();
                if ( this.secure_https ) {
                    try {
                        tcpStream = new SslStream( tcpStream, false, null, null );
                        ( ( SslStream )tcpStream ).AuthenticateAsServer( this.ssl_certificate );
                    }
                    catch ( Exception ex ) {
                        SimpleHttpLogger.LogVerbose( ex );
                        return;
                    }
                }
                this.inputStream = new BufferedStream( tcpStream );
                this.rawOutputStream = new BufferedStream( tcpStream );
                this.outputStream = new StreamWriter( this.rawOutputStream );
                try {
                    this.ParseRequest();
                    this.ReadHeaders();
                    this.RawQueryString = ParseQueryStringArguments( this.request_url.Query, preserveKeyCharacterCase: true );
                    this.QueryString = ParseQueryStringArguments( this.request_url.Query );
                    this.requestCookies = Cookies.FromString( this.GetHeaderValue( "Cookie", "" ) );
                    try {
                        if ( this.http_method.Equals( "GET" ) ) {
                            this.handleGETRequest();
                        }
                        else if ( this.http_method.Equals( "POST" ) ) {
                            this.handlePOSTRequest();
                        }
                    }
                    catch ( Exception e ) {
                        if ( !IsOrdinaryDisconnectException( e ) ) {
                            SimpleHttpLogger.Log( e );
                        }
                        this.writeFailure( "500 Internal Server Error" );
                    }
                }
                catch ( Exception e ) {
                    if ( !IsOrdinaryDisconnectException( e ) ) {
                        SimpleHttpLogger.LogVerbose( e );
                    }
                    this.writeFailure( "400 Bad Request", "The request cannot be fulfilled due to bad syntax." );
                }
                this.outputStream.Flush();
                this.rawOutputStream.Flush();
                this.inputStream = null;
                this.outputStream = null;
                this.rawOutputStream = null;
            }
            catch ( Exception ex ) {
                if ( !IsOrdinaryDisconnectException( ex ) ) {
                    SimpleHttpLogger.LogVerbose( ex );
                }
            }
            finally {
                try {
                    if ( this.TcpClient != null ) {
                        this.TcpClient.Close();
                    }
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.LogVerbose( ex );
                }
                try {
                    if ( tcpStream != null ) {
                        tcpStream.Close();
                    }
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.LogVerbose( ex );
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

        //    }
        //    return new NetworkCredential();
        //}

        /// <summary>
        ///     Asks the HttpServer to handle this request as a GET request.  If the HttpServer does not write a response code
        ///     header, this will write a generic failure header.
        /// </summary>
        private void handleGETRequest() {
            try {
                this.srv.handleGETRequest( this );
            }
            finally {
                if ( !this.responseWritten ) {
                    this.writeFailure();
                }
            }
        }

        /// <summary>
        ///     This post data processing just reads everything into a memory stream.
        ///     This is fine for smallish things, but for large stuff we should really
        ///     hand an input stream to the request processor. However, the input stream
        ///     we hand to the user's code needs to see the "end of the stream" at this
        ///     content length, because otherwise it won't know where the end is!
        ///     If the HttpServer does not write a response code header, this will write a generic failure header.
        /// </summary>
        private void handlePOSTRequest() {
            try {
                var ms = new MemoryStream();
                var content_length_str = this.GetHeaderValue( "Content-Length" );
                if ( !String.IsNullOrWhiteSpace( content_length_str ) ) {
                    int content_len;
                    if ( int.TryParse( content_length_str, out content_len ) ) {
                        if ( content_len > MAX_POST_SIZE ) {
                            this.writeFailure( "413 Request Entity Too Large", "Request Too Large" );
                            SimpleHttpLogger.LogVerbose( "POST Content-Length(" + content_len + ") too big for this simple server.  Server can handle up to " + MAX_POST_SIZE );
                            return;
                        }
                        var buf = new byte[ BUF_SIZE ];
                        var to_read = content_len;
                        while ( to_read > 0 ) {
                            var numread = this.inputStream.Read( buf, 0, Math.Min( BUF_SIZE, to_read ) );
                            if ( numread == 0 ) {
                                if ( to_read == 0 ) {
                                    break;
                                }
                                SimpleHttpLogger.LogVerbose( "client disconnected during post" );
                                return;
                            }
                            to_read -= numread;
                            ms.Write( buf, 0, numread );
                        }
                        ms.Seek( 0, SeekOrigin.Begin );
                    }
                }
                else {
                    this.writeFailure( "411 Length Required", "The request did not specify the length of its content." );
                    SimpleHttpLogger.LogVerbose( "The request did not specify the length of its content.  This server requires that all POST requests include a Content-Length header." );
                    return;
                }

                var contentType = this.GetHeaderValue( "Content-Type" );
                if ( contentType != null && contentType.Contains( "application/x-www-form-urlencoded" ) ) {
                    var sr = new StreamReader( ms );
                    var all = sr.ReadToEnd();
                    sr.Close();

                    this.RawPostParams = ParseQueryStringArguments( all, false, true );
                    this.PostParams = ParseQueryStringArguments( all, false );

                    this.srv.handlePOSTRequest( this, null );
                }
                else {
                    this.srv.handlePOSTRequest( this, new StreamReader( ms ) );
                }
            }
            finally {
                try {
                    if ( !this.responseWritten ) {
                        this.writeFailure();
                    }
                }
                catch ( Exception ) {
                }
            }
        }

        /// <summary>
        ///     Parses the first line of the http request to get the request method, url, and protocol version.
        /// </summary>
        private void ParseRequest() {
            var request = StreamReadLine( this.inputStream );
            var tokens = request.Split( ' ' );
            if ( tokens.Length != 3 ) {
                throw new Exception( "invalid http request line: " + request );
            }
            this.http_method = tokens[ 0 ].ToUpper();

            if ( tokens[ 1 ].StartsWith( "http://" ) || tokens[ 1 ].StartsWith( "https://" ) ) {
                this.request_url = new Uri( tokens[ 1 ] );
            }
            else {
                this.request_url = new Uri( this.base_uri_this_server, tokens[ 1 ] );
            }

            this.http_protocol_versionstring = tokens[ 2 ];
        }

        /// <summary>
        ///     Parses the http headers
        /// </summary>
        private void ReadHeaders() {
            String line;
            while ( ( line = StreamReadLine( this.inputStream ) ) != "" ) {
                var separator = line.IndexOf( ':' );
                if ( separator == -1 ) {
                    throw new Exception( "invalid http header line: " + line );
                }
                var name = line.Substring( 0, separator );
                var pos = separator + 1;
                while ( pos < line.Length && line[ pos ] == ' ' ) {
                    pos++; // strip any spaces
                }

                var value = line.Substring( pos, line.Length - pos );
                this.httpHeaders[ name.ToLower() ] = value;
            }
        }

        #region Parameter parsing

        /// <summary>
        ///     Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>
        ///     The value of the key, or [defaultValue] if the key does not exist or has no suitable value. This function
        ///     interprets a value of "1" or "true" (case insensitive) as being true.  Any other parameter value is interpreted as
        ///     false.
        /// </returns>
        public bool GetBoolParam( String key ) => this.GetQSBoolParam( key );

        /// <summary>
        ///     Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public Double GetDoubleParam( String key, int defaultValue = 0 ) => this.GetQSDoubleParam( key, defaultValue );

        /// <summary>
        ///     Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public int GetIntParam( String key, int defaultValue = 0 ) => this.GetQSIntParam( key, defaultValue );

        /// <summary>
        ///     Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or empty String if the key does not exist or has no value.</returns>
        public String GetParam( String key ) => this.GetQSParam( key );

        /// <summary>
        ///     Returns the value of a parameter sent via POST with MIME type "application/x-www-form-urlencoded".
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>
        ///     The value of the key, or [defaultValue] if the key does not exist or has no suitable value. This function
        ///     interprets a value of "1" or "true" (case insensitive) as being true.  Any other parameter value is interpreted as
        ///     false.
        /// </returns>
        public bool GetPostBoolParam( String key ) {
            var param = this.GetPostParam( key );
            if ( param == "1" || param.ToLower() == "true" ) {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Returns the value of a parameter sent via POST with MIME type "application/x-www-form-urlencoded".
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public Double GetPostDoubleParam( String key, Double defaultValue = 0 ) {
            if ( key == null ) {
                return defaultValue;
            }
            Double value;
            if ( Double.TryParse( this.GetPostParam( key.ToLower() ), out value ) ) {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        ///     Returns the value of a parameter sent via POST with MIME type "application/x-www-form-urlencoded".
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public int GetPostIntParam( String key, int defaultValue = 0 ) {
            if ( key == null ) {
                return defaultValue;
            }
            int value;
            if ( int.TryParse( this.GetPostParam( key.ToLower() ), out value ) ) {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        ///     Returns the value of a parameter sent via POST with MIME type "application/x-www-form-urlencoded".
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or empty String if the key does not exist or has no value.</returns>
        public String GetPostParam( String key ) {
            if ( key == null ) {
                return "";
            }
            String value;
            if ( this.PostParams.TryGetValue( key.ToLower(), out value ) ) {
                return value;
            }
            return "";
        }

        /// <summary>
        ///     Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>
        ///     The value of the key, or [defaultValue] if the key does not exist or has no suitable value. This function
        ///     interprets a value of "1" or "true" (case insensitive) as being true.  Any other parameter value is interpreted as
        ///     false.
        /// </returns>
        public bool GetQSBoolParam( String key ) {
            var param = this.GetQSParam( key );
            if ( param == "1" || param.ToLower() == "true" ) {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public Double GetQSDoubleParam( String key, Double defaultValue = 0 ) {
            if ( key == null ) {
                return defaultValue;
            }
            Double value;
            if ( Double.TryParse( this.GetQSParam( key.ToLower() ), out value ) ) {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        ///     Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or [defaultValue] if the key does not exist or has no suitable value.</returns>
        public int GetQSIntParam( String key, int defaultValue = 0 ) {
            if ( key == null ) {
                return defaultValue;
            }
            int value;
            if ( int.TryParse( this.GetQSParam( key.ToLower() ), out value ) ) {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        ///     Returns the value of the Query String parameter with the specified key.
        /// </summary>
        /// <param name="key">A case insensitive key.</param>
        /// <returns>The value of the key, or empty String if the key does not exist or has no value.</returns>
        public String GetQSParam( String key ) {
            if ( key == null ) {
                return "";
            }
            String value;
            if ( this.QueryString.TryGetValue( key.ToLower(), out value ) ) {
                return value;
            }
            return "";
        }

        /// <summary>
        ///     Parses the specified query String and returns a sorted list containing the arguments found in the specified query
        ///     String.  Can also be used to parse the POST request body if the mimetype is "application/x-www-form-urlencoded".
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="requireQuestionMark"></param>
        /// <returns></returns>
        private static SortedList<String, String> ParseQueryStringArguments( String queryString, bool requireQuestionMark = true, bool preserveKeyCharacterCase = false ) {
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
            foreach ( string t in parts ) {
                var argument = t.Split( '=' );
                if ( argument.Length == 2 ) {
                    var key = HttpUtility.UrlDecode( argument[ 0 ] );
                    if ( !preserveKeyCharacterCase ) {
                        if ( key != null ) {
                            key = key.ToLower();
                        }
                    }
                    String existingValue;
                    if ( arguments.TryGetValue( key, out existingValue ) ) {
                        arguments[ key ] += "," + HttpUtility.UrlDecode( argument[ 1 ] );
                    }
                    else {
                        arguments[ key ] = HttpUtility.UrlDecode( argument[ 1 ] );
                    }
                }
            }
            if ( hash != null ) {
                arguments[ "#" ] = hash;
            }
            return arguments;
        }

        #endregion Parameter parsing
    }
}