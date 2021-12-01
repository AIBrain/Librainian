// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "WebServer.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

#nullable enable

namespace Librainian.Internet.Servers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Logging;
using Utilities.Disposables;

/// <remarks>Based upon the version by "David" @ "https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server.aspx"</remarks>
/// <example>
/// <code>
///WebServer ws = new WebServer(SendResponse, "http://localhost:8080/test/");
///ws.Run();
///Console.WriteLine("A simple webserver. Press a key to quit.");
///Console.ReadKey();
///ws.Stop();
/// </code>
/// </example>
/// <example>
/// <code>
///public static string SendResponse(HttpListenerRequest request) { return string.Format("My web page", DateTime.Now); }
/// </code>
/// </example>
[UsedImplicitly]
public class WebServer : ABetterClassDispose {

	private readonly HttpListener _httpListener = new();

	private readonly Func<HttpListenerRequest, String>? _responderResponderMethod;

	/// <param name="prefixes"></param>
	/// <param name="responderMethod"></param>
	/// <exception cref="HttpListenerException"></exception>
	/// <exception cref="ObjectDisposedException"></exception>
	public WebServer( ICollection<String>? prefixes, Func<HttpListenerRequest, String>? responderMethod ) : base( nameof( WebServer ) ) {
		this.ImNotReady( String.Empty );

		if ( !HttpListener.IsSupported ) {
			this.ImNotReady( "HttpListener is not supported." );

			return;
		}

		if ( prefixes?.Any() != true ) {
			this.ImNotReady( "URI prefixes are required." );

			return;
		}

		if ( responderMethod is null ) {
			this.ImNotReady( "A responder method is required" );

			return;
		}

		foreach ( var prefix in prefixes ) {
			this._httpListener.Prefixes.Add( prefix );
		}

		this._responderResponderMethod = responderMethod;

		try {
			this._httpListener.Start();
			this.IsReadyForRequests = true;
		}
		catch {
			this.ImNotReady( "The httpListener did not Start()." );
		}
	}

	public WebServer( Func<HttpListenerRequest, String>? method, params String[]? prefixes ) : this( prefixes, method ) {
	}

	public Boolean IsReadyForRequests { get; private set; }

	public String? NotReadyBecause { get; private set; }

	private void ImNotReady( String? because ) {
		this.IsReadyForRequests = false;
		this.NotReadyBecause = because;
	}

	/// <summary>Dispose any disposable members.</summary>
	public override void DisposeManaged() => this.Stop();

	/// <summary>Start the http listener.</summary>
	/// <param name="cancellationToken"></param>
	/// <see cref="Stop" />
	public async Task Run( CancellationToken cancellationToken ) {
		"Webserver running...".Verbose();

		try {
			while ( this._httpListener.IsListening ) {
				"Webserver listening..".Verbose();

				var listenerContext = await this._httpListener.GetContextAsync().ConfigureAwait( false );

				if ( this._responderResponderMethod is null ) {

					//no responderMethod?!?
					return;
				}

				try {
					var response = this._responderResponderMethod( listenerContext.Request );
					var buffer = Encoding.UTF8.GetBytes( response );
					listenerContext.Response.ContentLength64 = buffer.Length;
					await listenerContext.Response.OutputStream.WriteAsync( buffer, cancellationToken ).ConfigureAwait( false );
				}
				catch ( Exception exception ) {
					exception.Log( BreakOrDontBreak.DontBreak );
				}
				finally {
					listenerContext.Response.OutputStream.Close();
				}
			}
		}
		catch ( Exception exception ) {
			exception.Log( BreakOrDontBreak.DontBreak );
		}
	}

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