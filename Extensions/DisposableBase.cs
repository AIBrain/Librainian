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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/DisposableBase.cs" was last cleaned by Rick on 2014/11/21 at 7:16 AM

#endregion License & Information

namespace Librainian.Extensions {

    using System;
    using System.Diagnostics;

    public abstract class DisposableBase : IDisposable {

        /// <summary>
        /// </summary>
        protected DisposableBase() {
            this.IsDisposed = false;
        }

        public bool IsDisposed {
            get;
            private set;
        }

        public void Dispose() {
            this.Dispose( true );

            this.IsDisposed = true;
            GC.SuppressFinalize( this );

            if ( this.Disposed != null ) {
                this.Disposed( this, EventArgs.Empty );
            }
        }

        /// <summary>
        ///     <see cref="DisposableBase" /> was not properly disposed!
        /// </summary>
        ~DisposableBase() {
            this.Dispose( false );
            if ( Debugger.IsAttached ) {
                Debugger.Break(); //
            }
        }

        public event EventHandler<EventArgs> Disposed;

        protected virtual void VerifyNotDisposed() {
            if ( this.IsDisposed ) {
                throw new ObjectDisposedException( this.ToString() );
            }
        }

        protected abstract void Dispose( bool disposing );
    }
}