// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "SimpleWebServer.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "SimpleWebServer.cs" was last formatted by Protiguous on 2018/07/10 at 9:11 PM.

namespace Librainian.Internet.Servers
{

    using FluentAssertions;
    using JetBrains.Annotations;
    using Magic;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;

    /// <summary>
    /// </summary>
    /// <remarks>Based upon the version by "David" @ "https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server.aspx"</remarks>
    /// <example>
    ///     WebServer ws = new WebServer(SendResponse, "http://localhost:8080/test/"); ws.Run(); Console.WriteLine("A
    ///     simple webserver. Press a key to quit."); Console.ReadKey(); ws.Stop();
    /// </example>
    /// <example>
    ///     public static string SendResponse(HttpListenerRequest request) { return string.Format("My web page",
    ///     DateTime.Now); }
    /// </example>
    [UsedImplicitly]
    public class SimpleWebServer : ABetterClassDispose
    {

        /// <summary>
        /// </summary>
        [NotNull]
        private readonly HttpListener _httpListener = new HttpListener();

        /// <summary>
        /// </summary>
        [CanBeNull]
        private readonly Func<HttpListenerRequest, String> _responderMethod;

        public Boolean IsReadyForRequests { get; private set; }

        public String NotReadyBecause { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="prefixes"></param>
        /// <param name="method">  </param>
        /// <exception cref="HttpListenerException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public SimpleWebServer(ICollection<String> prefixes, Func<HttpListenerRequest, String> method)
        {
            this.ImNotReady(String.Empty);

            this._httpListener.Should().NotBeNull("this._httpListener == null.");

            if (!HttpListener.IsSupported)
            {
                HttpListener.IsSupported.Should().BeTrue("Needs Windows XP SP2, Server 2003, or later.");
                this.ImNotReady(because: "HttpListener is not supported.");

                return;
            }

            prefixes.Should().NotBeNullOrEmpty("URI prefixes are required, for example http://localhost:8080/index/. ");

            if (prefixes?.Any() != true)
            {
                this.ImNotReady(because: "URI prefixes are required.");

                return;
            }

            method.Should().NotBeNull("A responder method is required");

            if (method == null)
            {
                this.ImNotReady(because: "A responder method is required");

                return;
            }

            foreach (var prefix in prefixes) { this._httpListener.Prefixes.Add(prefix); }

            this._responderMethod = method;

            try
            {
                this._httpListener.Start();
                this.IsReadyForRequests = true;
            }
            catch { this.ImNotReady(because: "The httpListener did not Start()."); }
        }

        public SimpleWebServer(Func<HttpListenerRequest, String> method, params String[] prefixes) : this(prefixes, method) { }

        private void ImNotReady(String because)
        {
            this.IsReadyForRequests = false;
            this.NotReadyBecause = because;
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this.Stop();

        /// <summary>
        ///     Start the http listener.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <see cref="Stop" />
        [NotNull]
        public Task Run(CancellationToken cancellationToken) =>
            Task.Run(async () =>
            {
                "Webserver running...".Info();

                try
                {
                    while (this._httpListener.IsListening)
                    {
                        Debug.WriteLine("Webserver listening..");

                        await Task.Run(async () =>
                        {
                            var listenerContext = await this._httpListener.GetContextAsync(); // Waits for an incoming request as an asynchronous operation.

                            if (listenerContext == null) { return; }

                            var responderMethod = this._responderMethod;

                            if (responderMethod == null)
                            {

                                //no responderMethod?!?
                                return;
                            }

                            try
                            {
                                var response = responderMethod(listenerContext.Request);
                                var buf = Encoding.UTF8.GetBytes(response);
                                listenerContext.Response.ContentLength64 = buf.Length;
                                listenerContext.Response.OutputStream.Write(buf, 0, buf.Length);
                            }

                            // ReSharper disable once EmptyGeneralCatchClause
                            catch
                            {

                                // suppress any exceptions
                            }
                            finally
                            {
                                listenerContext.Response.OutputStream.Close(); // always close the stream
                            }
                        }, cancellationToken);
                    }
                }

                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {

                    // suppress any exceptions
                }
            }, cancellationToken);

        public void Stop()
        {
            using (this._httpListener)
            {
                try
                {
                    if (this._httpListener.IsListening) { this._httpListener.Stop(); }
                }
                catch (ObjectDisposedException) { }

                this._httpListener.Close();
            }
        }
    }
}