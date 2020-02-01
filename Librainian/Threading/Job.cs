// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Job.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Job.cs" was last formatted by Protiguous on 2020/01/31 at 12:31 AM.

namespace Librainian.Threading {

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Maths;

    /// <summary>A task with a maximum time to run.</summary>
    public class Job<T> {

        /// <summary>Set to cancel this job with the <see cref="MaxRunningTime" />.</summary>
        [NotNull]
        public CancellationTokenSource CTS { get; private set; }

        /// <summary>Query the <see cref="ETR" />.</summary>
        public TimeSpan EstimatedTimeRemaining => this.MaxRunningTime - this.Elapsed();

        public TimeSpan MaxRunningTime { get; private set; }

        [CanBeNull]
        public T Result { get; private set; }

        public DateTime Started { get; }

        public DateTime Stopped { get; private set; }

        /// <summary>Call await on this task.</summary>
        [NotNull]
        public Task TheTask { get; }

        private Job( TimeSpan maxRuntime ) {
            this.MaxRunningTime = maxRuntime;
            this.Started = DateTime.UtcNow;
            this.CTS = new CancellationTokenSource( this.MaxRunningTime );
        }

        public Job( [NotNull] Func<T> func, TimeSpan maxRuntime ) : this( maxRuntime ) {
            if ( func is null ) {
                throw new ArgumentNullException( nameof( func ) );
            }

            T result = default;

            this.TheTask = new Task( () => result = func.Trap(), this.CTS.Token, TaskCreationOptions.PreferFairness ).ContinueWith( task => this.Done( result ),
                TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously );
        }

        public Job( [NotNull] Action action, TimeSpan maxRuntime ) : this( maxRuntime ) {
            if ( action is null ) {
                throw new ArgumentNullException( nameof( action ) );
            }

            this.TheTask = new Task( () => action.Trap(), this.CTS.Token, TaskCreationOptions.PreferFairness ).ContinueWith( task => this.Done( default ),
                TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously );
        }

        private void Done( [CanBeNull] T result ) {
            this.Stopped = DateTime.UtcNow;
            $"Job (Task.Id={this.TheTask.Id}) is done.".Trace();
            this.Result = result;
        }

        [NotNull]
        public static implicit operator Task( [NotNull] Job<T> job ) {
            if ( job is null ) {
                throw new ArgumentNullException( nameof( job ) );
            }

            return job.TheTask;
        }

        /// <summary>Increase the <see cref="MaxRunningTime" /> by a <paramref name="timeSpan" />. Has no effect if the task is done.</summary>
        /// <param name="timeSpan"></param>
        public void AddRunningTime( TimeSpan timeSpan ) {
            if ( !this.IsDone() ) {
                this.MaxRunningTime += timeSpan;
                this.CTS = new CancellationTokenSource( this.MaxRunningTime - this.Elapsed() );
            }
        }

        /// <summary>Move the <see cref="EstimatedTimeRemaining" /> TODO Needs tested..am I thinking right about this? Has no effect if the task is done.</summary>
        /// <param name="timeSpan"></param>
        public void AdjustETR( TimeSpan timeSpan ) {
            if ( !this.IsDone() ) {
                this.MaxRunningTime = ( this.Elapsed() + timeSpan ).Half();
            }
        }

        /// <summary>Returns the time spent running the task so far, or total time if the task is done.</summary>
        /// <returns></returns>
        public TimeSpan Elapsed() {
            if ( this.TheTask.IsDone() ) {
                return this.Stopped - this.Started;
            }

            return DateTime.UtcNow - this.Started;
        }

        /// <summary>Query the <see cref="EstimatedTimeRemaining" />.</summary>
        /// <returns></returns>
        public TimeSpan ETR() => this.EstimatedTimeRemaining;

        public Boolean IsDone() => this.TheTask.IsDone();

        /// <summary>aka Run()</summary>
        /// <returns></returns>
        public async Task Task() {
            try {
                await this.TheTask.ConfigureAwait( false );
            }
            catch ( TaskCanceledException ) {
                this.Result = default;
            }
        }
    }
}