// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/HttpServer.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Internet.Servers {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using Measurement.Time;

    public abstract class HttpServer {
        internal readonly List<Byte[]> LocalIPv4Addresses = new List<Byte[]>();

        /// <summary>If &gt; -1, the Server is listening for http connections on this port.</summary>
        protected readonly Int32 Port;

        /// <summary>If &gt; -1, the Server is listening for https connections on this port.</summary>
        protected readonly Int32 SecurePort;

        protected volatile Boolean StopRequested;
        private readonly X509Certificate2 _sslCertificate;
        private readonly Thread _thrHttp;
        private readonly Thread _thrHttps;
        private TcpListener _secureListener;
        private TcpListener _unsecureListener;

        /// <summary></summary>
        /// <param name="port">
        ///     The port number on which to accept regular http connections. If -1, the Server will not
        ///     listen for http connections.
        /// </param>
        /// <param name="httpsPort">
        ///     (Optional) The port number on which to accept https connections. If -1, the Server will
        ///     not listen for https connections.
        /// </param>
        /// <param name="cert">
        ///     (Optional) Certificate to use for https connections. If null and an httpsPort was
        ///     specified, a certificate is automatically created if necessary and loaded from
        ///     "SimpleHttpServer-SslCert.pfx" in the same directory that the current executable is
        ///     located in.
        /// </param>
        public HttpServer( Int32 port, Int32 httpsPort = -1, X509Certificate2 cert = null ) {
            this.Port = port;
            this.SecurePort = httpsPort;
            this._sslCertificate = cert;

            if ( ( this.Port > 65535 ) || ( this.Port < -1 ) ) {
                this.Port = -1;
            }
            if ( ( this.SecurePort > 65535 ) || ( this.SecurePort < -1 ) ) {
                this.SecurePort = -1;
            }

            if ( this.Port > -1 ) {
                this._thrHttp = new Thread( this.Listen ) { Name = "HttpServer Thread" };
            }

            //if ( this.SecurePort > -1 ) {
            //    if ( this._sslCertificate == null ) {
            //        String exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //        FileInfo fiExe = new FileInfo( exePath );
            //        FileInfo fiCert = new FileInfo( fiExe.Directory.FullName + "\\SimpleHttpServer-SslCert.pfx" );
            //        if ( fiCert.Exists )
            //            this._sslCertificate = new X509Certificate2( fiCert.FullName, "N0t_V3ry-S3cure#lol" );
            //        else {
            //            using ( Pluralsight.Crypto.CryptContext ctx = new Pluralsight.Crypto.CryptContext() ) {
            //                ctx.Open();

            // this._sslCertificate = ctx.CreateSelfSignedCertificate( new
            // Pluralsight.Crypto.SelfSignedCertProperties { IsPrivateKeyExportable = true,
            // KeyBitLength = 4096, Name = new X500DistinguishedName( "cn=localhost" ), ValidFrom =
            // DateTime.Today.AddDays( -1 ), ValidTo = DateTime.Today.AddYears( 100 ), } );

            //                byte[] certData = this._sslCertificate.Export( X509ContentType.Pfx, "N0t_V3ry-S3cure#lol" );
            //                File.WriteAllBytes( fiCert.FullName, certData );
            //            }
            //        }
            //    }
            //    this.thrHttps = new Thread( this.listen );
            //    this.thrHttps.Name = "HttpsServer Thread";
            //}

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach ( var addr in Dns.GetHostEntry( Dns.GetHostName() ).AddressList ) {
                if ( addr.AddressFamily == AddressFamily.InterNetwork ) {
                    var bytes = addr.GetAddressBytes();
                    if ( bytes.Length == 4 ) {
                        this.LocalIPv4Addresses.Add( bytes );
                    }
                }
            }
        }

        /// <summary>Handles an Http GET request.</summary>
        /// <param name="p">The HttpProcessor handling the request.</param>
        public abstract void HandleGetRequest( HttpProcessor p );

        /// <summary>Handles an Http POST request.</summary>
        /// <param name="p">The HttpProcessor handling the request.</param>
        /// <param name="inputData">
        ///     The input stream. If the request's MIME type was "application/x-www-form-urlencoded",
        ///     the StreamReader will be null and you can obtain the parameter values using
        ///     p.PostParams, p.GetPostParam(), p.GetPostIntParam(), etc.
        /// </param>
        public abstract void HandlePostRequest( HttpProcessor p, StreamReader inputData );

        /// <summary>
        ///     Blocks the calling thread until the http listening threads finish or the timeout
        ///     expires. Call this after calling Stop() if you need to wait for the listener to clean
        ///     up, such as if you intend to start another instance of the Server using the same port(s).
        /// </summary>
        /// <param name="timeoutMilliseconds">
        ///     Maximum number of milliseconds to wait for the HttpServer Threads to stop.
        /// </param>
        public void Join( Int32 timeoutMilliseconds = 2000 ) {
            var stopwatch = new StopWatch();
            var timeToWait = timeoutMilliseconds;
            stopwatch.Start();
            if ( timeToWait > 0 ) {
                try {
                    if ( ( this._thrHttp != null ) && this._thrHttp.IsAlive ) {
                        this._thrHttp.Join( timeToWait );
                    }
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.Log( ex );
                }
            }
            stopwatch.Stop();
            timeToWait = timeoutMilliseconds - ( Int32 )stopwatch.ElapsedMilliseconds;
            if ( timeToWait > 0 ) {
                try {
                    if ( ( this._thrHttps != null ) && this._thrHttps.IsAlive ) {
                        this._thrHttps.Join( timeToWait );
                    }
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.Log( ex );
                }
            }
        }

        /// <summary>Starts listening for connections.</summary>
        public void Start() {
            if ( this._thrHttp != null ) {
                this._thrHttp.Start( false );
            }
            if ( this._thrHttps != null ) {
                this._thrHttps.Start( true );
            }
        }

        /// <summary>Stops listening for connections.</summary>
        public void Stop() {
            if ( this.StopRequested ) {
                return;
            }
            this.StopRequested = true;
            if ( this._unsecureListener != null ) {
                try {
                    this._unsecureListener.Stop();
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.Log( ex );
                }
            }
            if ( this._secureListener != null ) {
                try {
                    this._secureListener.Stop();
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.Log( ex );
                }
            }
            if ( this._thrHttp != null ) {
                try {
                    this._thrHttp.Abort();
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.Log( ex );
                }
            }
            if ( this._thrHttps != null ) {
                try {
                    this._thrHttps.Abort();
                }
                catch ( Exception ex ) {
                    SimpleHttpLogger.Log( ex );
                }
            }
            try {
                this.StopServer();
            }
            catch ( Exception ex ) {
                SimpleHttpLogger.Log( ex );
            }
        }

        /// <summary>
        ///     This is called when the Server is stopping. Perform any cleanup work here.
        /// </summary>
        public abstract void StopServer();

        /// <summary>
        ///     Listens for connections, somewhat robustly. Does not return until the Server is stopped
        ///     or until more than 100 listener restarts occur in a single day.
        /// </summary>
        private void Listen( Object param ) {
            var isSecureListener = ( Boolean )param;

            var errorCount = 0;
            var lastError = DateTime.Now;

            TcpListener listener = null;

            while ( !this.StopRequested ) {
                var threwExceptionOuter = false;
                try {
                    listener = new TcpListener( IPAddress.Any, isSecureListener ? this.SecurePort : this.Port );
                    if ( isSecureListener ) {
                        this._secureListener = listener;
                    }
                    else {
                        this._unsecureListener = listener;
                    }
                    listener.Start();
                    while ( !this.StopRequested ) {
                        var innerErrorCount = 0;
                        var innerLastError = DateTime.Now;
                        try {
                            var s = listener.AcceptTcpClient();
                            Int32 workerThreads, completionPortThreads;
                            ThreadPool.GetAvailableThreads( out workerThreads, out completionPortThreads );

                            // Here is where we could enforce a minimum number of free pool threads,
                            // if we wanted to ensure better performance.
                            if ( workerThreads > 0 ) {
                                var processor = new HttpProcessor( s, this, isSecureListener ? this._sslCertificate : null );
                                ThreadPool.QueueUserWorkItem( processor.Process );
                            }
                            else {
                                try {
                                    var outputStream = new StreamWriter( s.GetStream() );
                                    outputStream.WriteLine( "HTTP/1.1 503 Service Unavailable" );
                                    outputStream.WriteLine( "Connection: close" );
                                    outputStream.WriteLine( "" );
                                    outputStream.WriteLine( "Server too busy" );
                                }
                                catch ( ThreadAbortException ex ) {
                                    throw ex;
                                }
                            }
                        }
                        catch ( ThreadAbortException ex ) {
                            throw ex;
                        }
                        catch ( Exception ex ) {
                            if ( ( DateTime.Now.Hour != innerLastError.Hour ) || ( DateTime.Now.DayOfYear != innerLastError.DayOfYear ) ) {

                                // ReSharper disable once RedundantAssignment
                                innerLastError = DateTime.Now;
                                innerErrorCount = 0;
                            }
                            if ( ++innerErrorCount > 10 ) {
                                throw ex;
                            }
                            SimpleHttpLogger.Log( ex, "Inner Error count this hour: " + innerErrorCount );
                            Thread.Sleep( 1 );
                        }
                    }
                }
                catch ( ThreadAbortException ) {
                    this.StopRequested = true;
                }
                catch ( Exception ex ) {
                    if ( ( DateTime.Now.DayOfYear != lastError.DayOfYear ) || ( DateTime.Now.Year != lastError.Year ) ) {
                        lastError = DateTime.Now;
                        errorCount = 0;
                    }
                    if ( ++errorCount > 100 ) {
                        throw ex;
                    }
                    SimpleHttpLogger.Log( ex, "Restarting listener. Error count today: " + errorCount );
                    threwExceptionOuter = true;
                }
                finally {
                    try {
                        if ( listener != null ) {
                            listener.Stop();
                            if ( threwExceptionOuter ) {
                                Thread.Sleep( 1000 );
                            }
                        }
                    }
                    catch ( ThreadAbortException ) {
                        this.StopRequested = true;
                    }
                    catch ( Exception ) { }
                }
            }
        }
    }
}