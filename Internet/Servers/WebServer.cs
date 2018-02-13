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
// "Librainian/WebServer.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Internet.Servers {

    using System;
    using System.Net;
    using System.Threading;
    using Magic;

    public class WebServer : ABetterClassDispose {

        private readonly AutoResetEvent _listenForNextRequest = new AutoResetEvent( false );
        private readonly HttpListener _httpListener;

        protected WebServer() => _httpListener = new HttpListener();

	    public Boolean IsRunning {
            get; private set;
        }

        public String Prefix {
            get; set;
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

        internal void Stop() {
			this._httpListener.Stop();
			this.IsRunning = false;
        }

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

		/// <summary>
		/// Dispose any disposable members.
		/// </summary>
		protected override void DisposeManaged() => this._listenForNextRequest?.Dispose();

	}
}