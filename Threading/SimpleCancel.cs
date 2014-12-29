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
// "Librainian 2015/SimpleCancel.cs" was last cleaned by Rick on 2014/12/29 at 8:15 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     I don't like the <see cref="CancellationTokenSource" /> throwing exceptions after
    ///     <see cref="CancellationTokenSource.Cancel()" /> is called.
    /// </summary>
    public class SimpleCancel : IDisposable {
        public enum RequestState : byte {
            Unrequested = 0,
            CancelRequested = 1
        }

        private long _cancelRequestCounter;
        private volatile Byte _state;

        public SimpleCancel() {
            this.Reset();
        }

        /// <summary>
        /// </summary>
        public DateTime? FirstCancelRequest { get; private set; }

        /// <summary>
        /// </summary>
        public Boolean IsCancellationRequested => this.GetCancelsRequestedCounter() > 0;

        public DateTime? LastCancelRequest { get; private set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => this.RequestCancel( throwIfAlreadyRequested: false );

        /// <summary>
        ///     Returns true if the cancel request was approved.
        /// </summary>
        /// <param name="throwIfAlreadyRequested"></param>
        /// <param name="throwMessage"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">Thrown if a cancellation has already been requested.</exception>
        public Boolean Cancel( Boolean throwIfAlreadyRequested = false, String throwMessage = "" ) => RequestCancel( throwIfAlreadyRequested, throwMessage );

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public UInt64 GetCancelsRequestedCounter() => ( UInt64 ) this._cancelRequestCounter;

        /// <summary>
        ///     Returns true if the cancel request was approved.
        /// </summary>
        /// <param name="throwIfAlreadyRequested"></param>
        /// <param name="throwMessage"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">Thrown if a cancellation has already been requested.</exception>
        public Boolean RequestCancel( Boolean throwIfAlreadyRequested = false, String throwMessage = "" ) {
            Interlocked.Increment( ref this._cancelRequestCounter );

            var now = DateTime.UtcNow;

            if ( !this.FirstCancelRequest.HasValue ) {
                this.FirstCancelRequest = now;
            }
            this.LastCancelRequest = now;

            switch ( ( RequestState ) this._state ) {
                case RequestState.Unrequested:
                    this.SetState( RequestState.CancelRequested );
                    return true;

                case RequestState.CancelRequested:
                    this.SetState( RequestState.CancelRequested );
                    if ( throwIfAlreadyRequested ) {
                        throw new TaskCanceledException( throwMessage );
                    }
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Resets all values, counters, and requests back to starting values.
        /// </summary>
        public void Reset() {
            this._state = ( Byte ) RequestState.Unrequested;
            this._cancelRequestCounter = 0;
            this.LastCancelRequest = null;
            this.FirstCancelRequest = null;
        }

        private Boolean SetState( RequestState state ) {
            switch ( state ) {
                case RequestState.CancelRequested:
                    this._state = ( Byte ) RequestState.CancelRequested;
                    return true;

                case RequestState.Unrequested:
                    this._state = ( Byte ) RequestState.Unrequested;
                    return true;
            }
            return false;
        }
    }
}
