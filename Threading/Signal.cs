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
// "Librainian/Signal.cs" was last cleaned by Rick on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using System.Threading;
    using Magic;

    public class Signal : ABetterClassDispose {
        public readonly ManualResetEvent Event = new ManualResetEvent( false );

        private Object _lockobject = new Object();

        private Int64 _signalcount;

        /// <summary>Sets the ManualResetEvent to nonsignaled.</summary>
        public Signal() {
            this._signalcount = 0;
            this.Reset();
        }

        //~Signal() {
        //    this.Event?.Dispose();
        //    this._lockobject = null;
        //}

        public Boolean IsSignaled => Interlocked.Read( ref this._signalcount ) > 0;

		/// <summary>
		/// Dispose any disposable members.
		/// </summary>
		protected override void DisposeManaged() => this.Event.Dispose();

		public void Lower() {
            Interlocked.Add( ref this._signalcount, -1 );
            this.Event.Reset();
        }

        public void Raise() {
            Interlocked.Add( ref this._signalcount, 1 );
            this.Event.Set();
        }

        public void Reset() {
            lock ( this._lockobject ) {
                this.Event.Reset();
                if ( Interlocked.Read( ref this._signalcount ) > 0 ) {
                    Interlocked.Add( ref this._signalcount, -Interlocked.Read( ref this._signalcount ) );
                }
                if ( Interlocked.Read( ref this._signalcount ) < 0 ) {
                    Interlocked.Add( ref this._signalcount, -Interlocked.Read( ref this._signalcount ) );
                }
            }
        }
    }
}