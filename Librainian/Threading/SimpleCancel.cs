// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "SimpleCancel.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "SimpleCancel.cs" was last formatted by Protiguous on 2018/07/13 at 1:41 AM.

namespace Librainian.Threading {

	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Collections;
	using Extensions;
	using Magic;

	/// <summary>
	///     A simpler threadsafe way to cancel a <see cref="Task" />. <seealso cref="CancellationToken" />
	/// </summary>
	[Experimental( "Somewhat untested. Should work though." )]
	public sealed class SimpleCancel : ABetterClassDispose {

		/// <summary>
		/// </summary>
		private Int64 _cancelRequests;

		public Boolean IsCancellationRequested => this.HaveAnyCancellationsBeenRequested();

		/// <summary>
		/// </summary>
		public SimpleCancel() => this.Reset();

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
		public Boolean HaveAnyCancellationsBeenRequested() => this.GetCancelsRequestedCounter().Any();

		/// <summary>
		///     Returns true if the cancel request was approved.
		/// </summary>
		/// <param name="throwIfAlreadyRequested"></param>
		/// <param name="throwMessage">           </param>
		/// <returns></returns>
		/// <exception cref="TaskCanceledException">Thrown if a cancellation has already been requested.</exception>
		public Boolean RequestCancel( Boolean throwIfAlreadyRequested = false, String throwMessage = "" ) {
			if ( throwIfAlreadyRequested && this.HaveAnyCancellationsBeenRequested() ) { throw new TaskCanceledException( throwMessage ); }

			Interlocked.Increment( location: ref this._cancelRequests );

			return true;
		}

		/// <summary>
		///     Resets all requests back to starting values.
		/// </summary>
		public void Reset() => Interlocked.Add( location1: ref this._cancelRequests, -Interlocked.Read( location: ref this._cancelRequests ) );
	}
}