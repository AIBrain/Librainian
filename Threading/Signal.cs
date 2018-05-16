// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Signal.cs",
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
// "Librainian/Librainian/Signal.cs" was last cleaned by Protiguous on 2018/05/15 at 10:50 PM.

namespace Librainian.Threading {

    using System;
    using System.Threading;
    using Magic;

    public class Signal : ABetterClassDispose {

        private readonly Object _lockobject = new Object();

        private Int64 _signalcount;
        public readonly ManualResetEvent Event = new ManualResetEvent( false );

        /// <summary>
        ///     Sets the ManualResetEvent to nonsignaled.
        /// </summary>
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
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this.Event.Dispose();

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

                if ( Interlocked.Read( ref this._signalcount ) > 0 ) { Interlocked.Add( ref this._signalcount, -Interlocked.Read( ref this._signalcount ) ); }

                if ( Interlocked.Read( ref this._signalcount ) < 0 ) { Interlocked.Add( ref this._signalcount, -Interlocked.Read( ref this._signalcount ) ); }
            }
        }
    }
}