// Copyright 2017 Protiguous.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/SimpleCancel.cs" was last cleaned by Protiguous on 2017/10/08 at 10:33 PM

namespace Librainian.Threading {

	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Magic;
	using Microsoft.FSharp.Core;

	/// <summary>
	///     A simpler threadsafe way to cancel a <see cref="Task" />.
	///     <seealso cref="CancellationToken" />
	///     This version has the Date and Time of the cancel request.
	/// </summary>
	[ Experimental( "Untested" ) ]
	public sealed class SimpleCancel : ABetterClassDispose {

		/// <summary>
		/// </summary>
		private Int64 _cancelRequests;

		/// <summary>
		/// </summary>
		public SimpleCancel() => this.Reset();

		/// <summary></summary>
		public DateTime? OldestCancelRequest { get; private set; }

		/// <summary>
		/// </summary>
		public DateTime? YoungestCancelRequest { get; private set; }

        protected override void DisposeManaged() => this.RequestCancel( throwIfAlreadyRequested: false );

        /// <summary>Returns true if the cancel request was approved.</summary>
        /// <param name="throwIfAlreadyRequested"></param>
        /// <param name="throwMessage"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">
        ///     Thrown if a cancellation has already been requested.
        /// </exception>
        public Boolean Cancel( Boolean throwIfAlreadyRequested = false, String throwMessage = "" ) => this.RequestCancel( throwIfAlreadyRequested: throwIfAlreadyRequested, throwMessage: throwMessage );

		/// <summary></summary>
		/// <returns></returns>
		public Int64 GetCancelsRequestedCounter() => Interlocked.Read( location: ref this._cancelRequests );

		/// <summary></summary>
		public Boolean HaveAnyCancellationsBeenRequested() => Interlocked.Read( location: ref this._cancelRequests ) > 0;

		/// <summary>Returns true if the cancel request was approved.</summary>
		/// <param name="throwIfAlreadyRequested"></param>
		/// <param name="throwMessage"></param>
		/// <returns></returns>
		/// <exception cref="TaskCanceledException">
		///     Thrown if a cancellation has already been requested.
		/// </exception>
		[Experimental( "Untested" )]
		public Boolean RequestCancel( Boolean throwIfAlreadyRequested = false, String throwMessage = "" ) {
			if ( throwIfAlreadyRequested && this.HaveAnyCancellationsBeenRequested() ) {
				throw new TaskCanceledException( message: throwMessage );
			}

			var now = DateTime.UtcNow;
			if ( !this.OldestCancelRequest.HasValue ) {
				//TODO name these better
				this.OldestCancelRequest = now; //TODO check logic here, might be backwards
			}
			if ( !this.YoungestCancelRequest.HasValue || this.YoungestCancelRequest.Value < now ) {
				this.YoungestCancelRequest = now;
			}
			Interlocked.Increment( location: ref this._cancelRequests );

			//this.CancelRequests.Enqueue( now );
			return true;
		}

		/// <summary>Resets all requests back to starting values.</summary>
		public void Reset() => Interlocked.Add( location1: ref this._cancelRequests, value: -Interlocked.Read( location: ref this._cancelRequests ) );

	}

}
