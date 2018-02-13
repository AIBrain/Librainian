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
// "Librainian/SimpleWebServer.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Internet.Servers {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Magic;

    /// <summary></summary>
    /// <remarks>Based upon the version by "David" @ "https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server.aspx"</remarks>
    /// <example>
    ///     WebServer ws = new WebServer(SendResponse, "http://localhost:8080/test/"); ws.Run();
    ///     Console.WriteLine("A simple webserver. Press a key to quit."); Console.ReadKey(); ws.Stop();
    /// </example>
    /// <example>
    ///     public static string SendResponse(HttpListenerRequest request) { return string.Format("My
    ///     web page", DateTime.Now); }
    /// </example>
    [UsedImplicitly]
    public class SimpleWebServer : ABetterClassDispose {

        /// <summary></summary>
        [NotNull]
        private readonly HttpListener _httpListener = new HttpListener();

        /// <summary></summary>
        [CanBeNull]
        private readonly Func<HttpListenerRequest, String> _responderMethod;

        /// <summary></summary>
        /// <param name="prefixes"></param>
        /// <param name="method"></param>
        /// <exception cref="HttpListenerException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public SimpleWebServer( ICollection<String> prefixes, Func<HttpListenerRequest, String> method ) {
            this.ImNotReady( String.Empty );

            this._httpListener.Should().NotBeNull( "this._httpListener is null." );

            if ( !HttpListener.IsSupported ) {
                HttpListener.IsSupported.Should().BeTrue( "Needs Windows XP SP2, Server 2003, or later." );
                this.ImNotReady( because: "HttpListener is not supported." );
                return;
            }

            prefixes.Should().NotBeNullOrEmpty( "URI prefixes are required, for example http://localhost:8080/index/. " );
            if ( prefixes == null || !prefixes.Any() ) {
                this.ImNotReady( because: "URI prefixes are required." );
                return;
            }

            method.Should().NotBeNull( "A responder method is required" );
            if ( method == null ) {
                this.ImNotReady( because: "A responder method is required" );
                return;
            }

            foreach ( var prefix in prefixes ) {
                this._httpListener.Prefixes.Add( prefix );
            }

            this._responderMethod = method;

            try {
                this._httpListener.Start();
                this.IsReadyForRequests = true;
            }
            catch {
                this.ImNotReady( because: "The httpListener did not Start()." );
            }
        }

        public SimpleWebServer( Func<HttpListenerRequest, String> method, params String[] prefixes ) : this( prefixes, method ) {
        }

        public Boolean IsReadyForRequests {
            get; private set;
        }

        public String NotReadyBecause {
            get; private set;
        }

		/// <summary>
		/// Dispose any disposable members.
		/// </summary>
		protected override void DisposeManaged() => this.Stop();

		/// <summary>Start the http listener.</summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <seealso cref="Stop" />
		public Task Run( CancellationToken cancellationToken ) => Task.Run( async () => {
            "Webserver running...".Info();
            try {
                while ( this._httpListener.IsListening ) {
                    Debug.WriteLine( "Webserver listening.." );
                    await Task.Run( async () => {
                        var listenerContext = await this._httpListener.GetContextAsync(); // Waits for an incoming request as an asynchronous operation.
                        if ( listenerContext == null ) {
                            return;
                        }
                        var responderMethod = this._responderMethod;
                        if ( responderMethod == null ) {

                            //no responderMethod?!?
                            return;
                        }
                        try {
                            var response = responderMethod( listenerContext.Request );
                            var buf = Encoding.UTF8.GetBytes( response );
                            listenerContext.Response.ContentLength64 = buf.Length;
                            listenerContext.Response.OutputStream.Write( buf, 0, buf.Length );
                        }

                        // ReSharper disable once EmptyGeneralCatchClause
                        catch {

                            // suppress any exceptions
                        }
                        finally {
                            listenerContext.Response.OutputStream.Close(); // always close the stream
                        }
                    }, cancellationToken );
                }
            }

            // ReSharper disable once EmptyGeneralCatchClause
            catch {

                // suppress any exceptions
            }
        }, cancellationToken );

        public void Stop() {
            using ( this._httpListener ) {
                try {
                    if ( this._httpListener.IsListening ) {
                        this._httpListener.Stop();
                    }
                }
                catch ( ObjectDisposedException ) { }
                this._httpListener.Close();
            }
        }

        private void ImNotReady( String because ) {
            this.IsReadyForRequests = false;
            this.NotReadyBecause = because;
        }
    }
}