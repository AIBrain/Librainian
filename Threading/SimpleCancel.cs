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
// "Librainian 2015/SimpleCancel.cs" was last cleaned by aibra_000 on 2015/03/23 at 5:24 PM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;

    /// <summary>
    ///     Don't like the <see cref="CancellationTokenSource" /> throwing exceptions after <see cref="CancellationTokenSource.Cancel()" /> is called.
    ///     I understand why, I just don't like it. Plus, this version has the Dates and Times of the cancel requests.
    /// </summary>
    public sealed class SimpleCancel : IDisposable {
        public SimpleCancel() {
            this.Reset();
        }

        public ConcurrentQueue< DateTime > CancelRequests { get; } = new ConcurrentQueue< DateTime >();

        /// <summary>
        /// </summary>
        public DateTime? OldestCancelRequest => this.CancelRequests.OrderBy( dateTime => dateTime ).FirstOrDefault();

        public DateTime? YoungestCancelRequest => this.CancelRequests.OrderBy( dateTime => dateTime ).LastOrDefault();

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => this.RequestCancel( throwIfAlreadyRequested: false );

        /// <summary>
        /// </summary>
        public Boolean HaveAnyCancellationsBeenRequested() => this.CancelRequests.Any();

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
        public UInt64 GetCancelsRequestedCounter() => ( UInt64 ) this.CancelRequests.LongCount();

        /// <summary>
        ///     Returns true if the cancel request was approved.
        /// </summary>
        /// <param name="throwIfAlreadyRequested"></param>
        /// <param name="throwMessage"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">Thrown if a cancellation has already been requested.</exception>
        public Boolean RequestCancel( Boolean throwIfAlreadyRequested = false, String throwMessage = "" ) {
            if ( throwIfAlreadyRequested && this.HaveAnyCancellationsBeenRequested() ) {
                throw new TaskCanceledException( throwMessage );
            }
            this.CancelRequests.Enqueue( DateTime.UtcNow );
            return true;
        }

        /// <summary>
        ///     Resets all requests back to starting values.
        /// </summary>
        public void Reset() => this.CancelRequests.Clear();
    }
}
