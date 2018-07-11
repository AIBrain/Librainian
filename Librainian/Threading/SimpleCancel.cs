// ---------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

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
			if ( throwIfAlreadyRequested && this.HaveAnyCancellationsBeenRequested() ) {
				throw new TaskCanceledException( throwMessage );
			}

			Interlocked.Increment( location: ref this._cancelRequests );

			return true;
		}

		/// <summary>
		///     Resets all requests back to starting values.
		/// </summary>
		public void Reset() => Interlocked.Add( location1: ref this._cancelRequests, -Interlocked.Read( location: ref this._cancelRequests ) );

	}

}