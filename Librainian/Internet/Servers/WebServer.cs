﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "WebServer.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "WebServer.cs" was last formatted by Protiguous on 2020/03/18 at 10:23 AM.

namespace Librainian.Internet.Servers {

    using System;
    using System.Net;
    using System.Threading;
    using JetBrains.Annotations;
    using Utilities;

    public class WebServer : ABetterClassDispose {

        [NotNull]
        private readonly HttpListener _httpListener = new HttpListener();

        [NotNull]
        private readonly AutoResetEvent _listenForNextRequest = new AutoResetEvent( false );

        public Boolean IsRunning { get; private set; }

        public String Prefix { get; set; }

        private static void ListenerCallback( [CanBeNull] IAsyncResult ar ) {

            //TODO
        }

        // Loop here to begin processing of new requests.
        private void Listen( [CanBeNull] Object state ) {
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