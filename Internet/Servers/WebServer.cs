// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "WebServer.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/WebServer.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Internet.Servers {

    using System;
    using System.Net;
    using System.Threading;
    using Magic;

    public class WebServer : ABetterClassDispose {

        private readonly HttpListener _httpListener;

        private readonly AutoResetEvent _listenForNextRequest = new AutoResetEvent( false );

        protected WebServer() => this._httpListener = new HttpListener();

        public Boolean IsRunning { get; private set; }

        public String Prefix { get; set; }

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
    }
}