// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Job.cs" last touched on 2021-06-18 at 5:40 PM by Protiguous.

namespace Librainian.Threading {

	using System;
	using System.Diagnostics;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions;
	using Extensions;
	using Logging;
	using Maths;
	using Measurement.Frequency;
	using Utilities;
	using Utilities.Disposables;

	/// <summary>
	///     <para>
	///         A task with a maximum time to run before a cancel is thrown (<see cref="TaskCanceledException" /> which
	///         sometimes can be thrown as <see cref="OperationCanceledException" />, I think).
	///     </para>
	/// </summary>
	[NeedsTesting]
	public class Job<T> : ABetterClassDispose {

		private Stopwatch _stopwatch { get; }

		/// <summary>Set to cancel this job with the <see cref="MaxRunningTime" />.</summary>
		public CancellationTokenSource CTS { get; }

		/// <summary>Query the <see cref="ETR" />.</summary>
		public TimeSpan EstimatedTimeRemaining() => this.MaxRunningTime - this.Elapsed();

		public TimeSpan MaxRunningTime { get; private set; }

		public IProgress<Single> Progress { get; }

		public T? Result { get; private set; }

		/// <summary>Call await on this task.</summary>
		public Task TheTask { get; } = null!;

		/// <summary>
		/// </summary>
		/// <param name="maxRuntime"></param>
		/// <param name="progress">Update this progress after each <see cref="Step" />.</param>
		private Job( TimeSpan maxRuntime, IProgress<Single> progress ) : base( nameof( Job<T> ) ) {
			this.MaxRunningTime = maxRuntime;
			this.Progress = progress;
			this.CTS = new CancellationTokenSource( this.MaxRunningTime );
			this._stopwatch = Stopwatch.StartNew();
			this.Timer = FluentTimer.Create( Fps.Sixty ).AutoReset().AndStart();
		}

		private FluentTimer Timer { get; set; }

		public Job( Func<T> func, TimeSpan maxRuntime, IProgress<Single> progress ) : this( maxRuntime, progress ) {
			if ( func is null ) {
				throw new ArgumentEmptyException( nameof( func ) );
			}

			T? result = default;

			this.TheTask = new Task( () => result = func.Trap(), this.CTS.Token, TaskCreationOptions.PreferFairness ).ContinueWith( _ => this.Done( result ),
				TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously );
		}

		public Job( Action action, TimeSpan maxRuntime, IProgress<Single> progress ) : this( maxRuntime, progress ) {
			if ( action is null ) {
				throw new ArgumentEmptyException( nameof( action ) );
			}

			this.TheTask = new Task( () => action.Trap(), this.CTS.Token, TaskCreationOptions.PreferFairness ).ContinueWith( _ => this.Done( default( T ) ),
				TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously );
		}

		private void Done( T? result ) {
			this._stopwatch.Stop();
			$"Job (Task.Id={this.TheTask.Id}) is done.".Verbose();

			this.Result = result;
		}

		public static implicit operator Task( Job<T> job ) {
			if ( job is null ) {
				throw new ArgumentEmptyException( nameof( job ) );
			}

			return job.TheTask;
		}

		/// <summary>
		///     Move the <see cref="EstimatedTimeRemaining" /> TODO Needs tested..am I thinking right about this? Has no
		///     effect if the task is <see cref="IsDone" />.
		/// </summary>
		/// <param name="timeSpan"></param>
		public void AdjustETR( TimeSpan timeSpan ) {
			if ( !this.IsDone() ) {
				this.MaxRunningTime = ( this.Elapsed() + timeSpan ).Half();
			}
		}

		/// <summary>
		///     Increase the <see cref="MaxRunningTime" /> by a <paramref name="amountOfTime" />. Has no effect if the task is
		///     done.
		/// </summary>
		/// <param name="amountOfTime"></param>
		public void ChangeRemainingTime( TimeSpan amountOfTime ) {
			if ( !this.IsDone() ) {
				this.MaxRunningTime += amountOfTime;

				//var stillLeft = this.Elapsed() - this.MaxRunningTime;
				this.CTS.CancelAfter( this.MaxRunningTime );
			}
		}

		[MethodImpl( MethodImplOptions.Synchronized )]
		public override void DisposeManaged() {
			base.DisposeManaged();
			if ( !this.CTS.IsCancellationRequested ) {
				this.CTS.Cancel( true );
			}

			this.Timer.Stop();
		}

		/// <summary>
		///     How long the <see cref="Job{T}" /> has been running.
		/// </summary>
		public TimeSpan Elapsed() => this._stopwatch.Elapsed;

		/// <summary>Query the <see cref="EstimatedTimeRemaining" />.</summary>
		public TimeSpan ETR() => this.EstimatedTimeRemaining();

		public Boolean IsDone() => this.TheTask.IsDone();

		/// <summary>aka Run()</summary>
		public async Task Task() {
			try {
				await this.TheTask.ConfigureAwait( false );
			}
			catch ( TaskCanceledException ) {
				this.Result = default( T );
			}
		}
	}
}