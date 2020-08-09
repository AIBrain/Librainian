// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Internet.Servers {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Logging;
	using Utilities;

	/// <summary></summary>
	/// <remarks>Based upon the version by "David" @ "https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server.aspx"</remarks>
	/// <example>
	/// WebServer ws = new WebServer(SendResponse, "http://localhost:8080/test/"); ws.Run(); Console.WriteLine("A simple webserver. Press a key to quit."); Console.ReadKey();
	/// ws.Stop();
	/// </example>
	/// <example>public static string SendResponse(HttpListenerRequest request) { return string.Format("My web page", DateTime.Now); }</example>
	[UsedImplicitly]
	public class SimpleWebServer : ABetterClassDispose {

		/// <summary></summary>
		[NotNull]
		private readonly HttpListener _httpListener = new HttpListener();

		/// <summary></summary>
		[CanBeNull]
		private readonly Func<HttpListenerRequest, String> _responderMethod;

		public Boolean IsReadyForRequests { get; private set; }

		public String NotReadyBecause { get; private set; }

		/// <summary></summary>
		/// <param name="prefixes"></param>
		/// <param name="method">  </param>
		/// <exception cref="HttpListenerException"></exception>
		/// <exception cref="ObjectDisposedException"></exception>
		public SimpleWebServer( [CanBeNull] ICollection<String> prefixes, [CanBeNull] Func<HttpListenerRequest, String> method ) {
			this.ImNotReady( String.Empty );

			if ( !HttpListener.IsSupported ) {
				this.ImNotReady( "HttpListener is not supported." );

				return;
			}

			if ( prefixes?.Any() != true ) {
				this.ImNotReady( "URI prefixes are required." );

				return;
			}

			if ( method is null ) {
				this.ImNotReady( "A responder method is required" );

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
				this.ImNotReady( "The httpListener did not Start()." );
			}
		}

		public SimpleWebServer( [CanBeNull] Func<HttpListenerRequest, String> method, [CanBeNull] params String[] prefixes ) : this( prefixes, method ) { }

		private void ImNotReady( [CanBeNull] String because ) {
			this.IsReadyForRequests = false;
			this.NotReadyBecause = because;
		}

		/// <summary>Dispose any disposable members.</summary>
		public override void DisposeManaged() => this.Stop();

		/// <summary>Start the http listener.</summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <see cref="Stop" />
		[NotNull]
		public Task Run( CancellationToken cancellationToken ) =>
			Task.Run( async () => {
				"Webserver running...".Info();

				try {
					while ( this._httpListener.IsListening ) {
						Debug.WriteLine( "Webserver listening.." );

						await Task.Run( async () => {
							var listenerContext =
								await this._httpListener.GetContextAsync().ConfigureAwait( false ); // Waits for an incoming request as an asynchronous operation.

							if ( listenerContext is null ) {
								return;
							}

							var responderMethod = this._responderMethod;

							if ( responderMethod is null ) {

								//no responderMethod?!?
								return;
							}

							try {
								var response = responderMethod( listenerContext.Request );
								var buf = Encoding.UTF8.GetBytes( response );
								listenerContext.Response.ContentLength64 = buf.Length;
								await listenerContext.Response.OutputStream.WriteAsync( buf, 0, buf.Length, cancellationToken ).ConfigureAwait(false);
							}

							// ReSharper disable once EmptyGeneralCatchClause
							catch {

								// suppress any exceptions
							}
							finally {
								listenerContext.Response.OutputStream.Close(); // always close the stream
							}
						}, cancellationToken ).ConfigureAwait( false );
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
	}
}