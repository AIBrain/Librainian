#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/Signal.cs" was last cleaned by Rick on 2014/08/11 at 12:41 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Threading;

    public class Signal : IDisposable {
        public readonly ManualResetEvent Event;

        private Object _lockobject = new Object();

        private long _signalcount;

        /// <summary>
        ///     Sets the ManualResetEvent to nonsignaled.
        /// </summary>
        public Signal() {
            this._signalcount = 0;
            this.Event = new ManualResetEvent( false );
            this.Reset();
        }

        public Boolean IsSignaled => Interlocked.Read( ref this._signalcount ) > 0;

        public void Dispose() => this.Event?.Dispose();

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

        ~Signal() {
            this.Event?.Dispose();
            this._lockobject = null;
        }

        public void Raise() {
            Interlocked.Add( ref this._signalcount, 1 );
            this.Event.Set();
        }

        public void Lower() {
            Interlocked.Add( ref this._signalcount, -1 );
            this.Event.Reset(); //humbug.
        }
    }
}
