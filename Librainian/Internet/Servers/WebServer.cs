// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "WebServer.cs" last formatted on 2020-08-14 at 8:34 PM.

namespace Librainian.Internet.Servers {

	using System;
	using System.Net;
	using System.Threading;
	using Utilities;
	using Utilities.Disposables;

	public class WebServer : ABetterClassDispose {

		private readonly HttpListener _httpListener = new();

		private readonly AutoResetEvent _listenForNextRequest = new( false );

		public Boolean IsRunning { get; private set; }

		public String Prefix { get; set; }

		private static void ListenerCallback( IAsyncResult? ar ) {

			//TODO
		}

		// Loop here to begin processing of new requests.
		private void Listen( Object? state ) {
			while ( this._httpListener.IsListening ) {
				this._httpListener.BeginGetContext( ListenerCallback, this._httpListener );
				this._listenForNextRequest.WaitOne();
			}
		}

		internal void Stop() {
			this._httpListener.Stop();
			this.IsRunning = false;
		}

		/// <summary>Dispose any disposable members.</summary>
		public override void DisposeManaged() {
			using ( this._listenForNextRequest ) { }
		}

		public void Start() {
			if ( String.IsNullOrEmpty( this.Prefix ) ) {
				throw new InvalidOperationException( "Specify prefix" );
			}

			this._httpListener.Prefixes.Clear();
			this._httpListener.Prefixes.Add( this.Prefix );
			this._httpListener.Start();
			ThreadPool.QueueUserWorkItem( this.Listen );
		}
	}
}