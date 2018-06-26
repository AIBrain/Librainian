// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "WebServer.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "WebServer.cs" was last formatted by Protiguous on 2018/06/04 at 4:00 PM.

namespace Librainian.Internet.Servers {

	using System;
	using System.Net;
	using System.Threading;
	using Magic;

	public class WebServer : ABetterClassDispose {

		public Boolean IsRunning { get; private set; }

		public String Prefix { get; set; }

		private readonly HttpListener _httpListener;

		private readonly AutoResetEvent _listenForNextRequest = new AutoResetEvent( false );

		private static void ListenerCallback( IAsyncResult ar ) {

			//TODO
		}

		// Loop here to begin processing of new requests.
		private void Listen( Object state ) {
			while ( this._httpListener.IsListening ) {
				this._httpListener.BeginGetContext( ListenerCallback, this._httpListener );
				this._listenForNextRequest.WaitOne();
			}
		}

		internal void Stop() {
			this._httpListener.Stop();
			this.IsRunning = false;
		}

		/// <summary>
		///     Dispose any disposable members.
		/// </summary>
		public override void DisposeManaged() => this._listenForNextRequest?.Dispose();

		public void Start() {
			if ( String.IsNullOrEmpty( this.Prefix ) ) { throw new InvalidOperationException( "Specify prefix" ); }

			this._httpListener.Prefixes.Clear();
			this._httpListener.Prefixes.Add( this.Prefix );
			this._httpListener.Start();
			ThreadPool.QueueUserWorkItem( this.Listen );
		}

		protected WebServer() => this._httpListener = new HttpListener();

	}

}