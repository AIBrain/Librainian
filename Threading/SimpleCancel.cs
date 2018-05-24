// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "SimpleCancel.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/SimpleCancel.cs" was last formatted by Protiguous on 2018/05/21 at 10:56 PM.

namespace Librainian.Threading {

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Magic;
    using Microsoft.FSharp.Core;

    /// <summary>
    ///     A simpler threadsafe way to cancel a <see cref="Task" />. <seealso cref="CancellationToken" /> This version has the
    ///     Date and Time of the cancel request.
    /// </summary>
    [Experimental( "Somewhat untested" )]
    public sealed class SimpleCancel : ABetterClassDispose {

        /// <summary>
        /// </summary>
        private Int64 _cancelRequests;

        /// <summary>
        /// </summary>
        public SimpleCancel() => this.Reset();

        /// <summary>
        /// </summary>
        public DateTime? OldestCancelRequest { get; private set; }

        /// <summary>
        /// </summary>
        public DateTime? YoungestCancelRequest { get; private set; }

        /// <summary>
        ///     Returns true if the cancel request was approved.
        /// </summary>
        /// <param name="throwIfAlreadyRequested"></param>
        /// <param name="throwMessage">           </param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">Thrown if a cancellation has already been requested.</exception>
        public Boolean Cancel( Boolean throwIfAlreadyRequested = false, String throwMessage = "" ) => this.RequestCancel( throwIfAlreadyRequested: throwIfAlreadyRequested, throwMessage: throwMessage );

        public override void DisposeManaged() => this.RequestCancel( throwIfAlreadyRequested: false );

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Int64 GetCancelsRequestedCounter() => Interlocked.Read( location: ref this._cancelRequests );

        /// <summary>
        /// </summary>
        public Boolean HaveAnyCancellationsBeenRequested() => Interlocked.Read( location: ref this._cancelRequests ) > 0;

        /// <summary>
        ///     Returns true if the cancel request was approved.
        /// </summary>
        /// <param name="throwIfAlreadyRequested"></param>
        /// <param name="throwMessage">           </param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">Thrown if a cancellation has already been requested.</exception>
        [Experimental( "Untested" )]
        public Boolean RequestCancel( Boolean throwIfAlreadyRequested = false, String throwMessage = "" ) {
            if ( throwIfAlreadyRequested && this.HaveAnyCancellationsBeenRequested() ) { throw new TaskCanceledException( throwMessage ); }

            var now = DateTime.UtcNow;

            if ( !this.OldestCancelRequest.HasValue ) {

                //TODO name these better
                this.OldestCancelRequest = now; //TODO check logic here, might be backwards
            }

            if ( !this.YoungestCancelRequest.HasValue || this.YoungestCancelRequest.Value < now ) { this.YoungestCancelRequest = now; }

            Interlocked.Increment( location: ref this._cancelRequests );

            //this.CancelRequests.Enqueue( now );
            return true;
        }

        /// <summary>
        ///     Resets all requests back to starting values.
        /// </summary>
        public void Reset() => Interlocked.Add( location1: ref this._cancelRequests, -Interlocked.Read( location: ref this._cancelRequests ) );
    }
}