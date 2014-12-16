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

        public bool IsRunning {
            get;
            private set;
        }

        public string Prefix {
            get;
            set;
        }

        public void Start() {
            if ( String.IsNullOrEmpty( Prefix ) )
                throw new InvalidOperationException( "Specify prefix" );
            _httpListener.Prefixes.Clear();
            _httpListener.Prefixes.Add( Prefix );
            _httpListener.Start();
            ThreadPool.QueueUserWorkItem( Listen );
        }

        internal void Stop() {
            _httpListener.Stop();
            IsRunning = false;
        }

        // Loop here to begin processing of new requests. 
        private void Listen( object state ) {
            while ( _httpListener.IsListening ) {
                _httpListener.BeginGetContext( ListenerCallback, _httpListener );
                ListenForNextRequest.WaitOne();
            }
        }

        private void ListenerCallback( IAsyncResult ar ) {
            //TODO

        }
    }
}