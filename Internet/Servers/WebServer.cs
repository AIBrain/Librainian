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

    public class WebServer {
        private static readonly AutoResetEvent ListenForNextRequest = new AutoResetEvent( false );
        private readonly HttpListener _httpListener;

        protected WebServer() {
            _httpListener = new HttpListener();
        }

        public Boolean IsRunning {
            get; private set;
        }

        public String Prefix {
            get; set;
        }

        public void Start() {
            if ( String.IsNullOrEmpty( Prefix ) ) {
                throw new InvalidOperationException( "Specify prefix" );
            }
            _httpListener.Prefixes.Clear();
            _httpListener.Prefixes.Add( Prefix );
            _httpListener.Start();
            ThreadPool.QueueUserWorkItem( Listen );
        }

        internal void Stop() {
            _httpListener.Stop();
            IsRunning = false;
        }

        private static void ListenerCallback( IAsyncResult ar ) {

            //TODO
        }

        // Loop here to begin processing of new requests.
        private void Listen( Object state ) {
            while ( _httpListener.IsListening ) {
                _httpListener.BeginGetContext( ListenerCallback, _httpListener );
                ListenForNextRequest.WaitOne();
            }
        }
    }
}