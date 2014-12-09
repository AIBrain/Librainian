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
// "Librainian/SimpleCancel.cs" was last cleaned by Rick on 2014/12/06 at 7:23 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Maths;

    /// <summary>
    ///     I don't like the <see cref="CancellationTokenSource" /> throwing exceptions after
    ///     <see cref="CancellationTokenSource.Cancel()" /> is called.
    /// </summary>
    public class SimpleCancel : IDisposable {
        //public event Action OnCanelRequestedEvent { add { this._onCanelRequestedAction += value; } remove { this._onCanelRequestedAction -= value; } }

        public enum RequestState : byte {
            Unrequested = 0,
            CancelRequested = 1
        }

        private volatile UInt32 _cancelRequestCounter;
        private volatile Int32 _dateOfFirstCancelRequest;
        private volatile Int32 _dateOfRecentCancelRequest;
        //[NotNull] private Action _onCanelRequestedAction = () => { };

        private volatile Byte _state;
        private volatile Int32 _timeOfFirstCancelRequest;
        private volatile Int32 _timeOfRecentCancelRequest;

        public SimpleCancel() {
            this.Reset();
        }

        public Boolean IsCancellationRequested => this.HasCancelBeenRequested();

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

        public UInt32 GetCancelsRequestedCounter() => this._cancelRequestCounter;

        /// <summary>
        ///     Returns the <see cref="DateTime" /> of the oldest cancel request.
        /// </summary>
        /// <returns></returns>
        public DateTime GetOldestCancelRequest() {
            var translate = new Translate64( this._dateOfFirstCancelRequest, this._timeOfFirstCancelRequest );
            var firstCancelRequest = new DateTime( translate.SignedValue );
            return firstCancelRequest;
        }

        /// <summary>
        ///     Returns the <see cref="DateTime" /> of the youngest cancel request.
        /// </summary>
        /// <returns></returns>
        public DateTime GetYoungestCancelRequest() {
            var translate = new Translate64( this._dateOfRecentCancelRequest, this._timeOfRecentCancelRequest );
            var firstCancelRequest = new DateTime( translate.SignedValue );
            return firstCancelRequest;
        }

        public Boolean HasCancelBeenRequested() => GetCancelsRequestedCounter() > 0;

        /// <summary>
        ///     Returns true if the cancel request was approved.
        /// </summary>
        /// <param name="throwIfAlreadyRequested"></param>
        /// <param name="throwMessage"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">Thrown if a cancellation has already been requested.</exception>
        public Boolean RequestCancel( Boolean throwIfAlreadyRequested = false, String throwMessage = "" ) {
            ++this._cancelRequestCounter;

            var now = DateTime.UtcNow;
            var translate64 = new Translate64( now.Ticks );

            if ( this._dateOfFirstCancelRequest < 0 ) {
                this._dateOfFirstCancelRequest = translate64.SignedHigh;
                this._timeOfFirstCancelRequest = translate64.SignedLow;
            }

            this._dateOfRecentCancelRequest = translate64.SignedHigh;
            this._timeOfRecentCancelRequest = translate64.SignedLow;

            switch ( this.GetState() ) {
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

            _dateOfFirstCancelRequest = -1;
            _timeOfFirstCancelRequest = -1;

            _dateOfRecentCancelRequest = -1;
            _timeOfRecentCancelRequest = -1;
        }

        private RequestState GetState() => ( RequestState ) this._state;

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
