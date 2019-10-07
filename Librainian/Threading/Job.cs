// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Job.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", "Job.cs" was last formatted by Protiguous on 2019/10/03 at 5:08 PM.

namespace Librainian.Threading {

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;

    /// <summary>
    ///     A task with an active progress, and a timer to complete or cancel.
    /// </summary>
    public class Job<T> {

        public CancellationTokenSource CTS { get; }

        public TimeSpan EstimatedLengthOfJob { get; set; }

        public TimeSpan EstimatedTimeRemaining { get; set; }

        [CanBeNull]
        public T Result { get; private set; }

        public DateTime Started { get; }

        public DateTime Stopped { get; private set; }

        /// <summary>
        ///     Call await on this task.
        /// </summary>
        [NotNull]
        public Task TheTask { get; }

        private Job() => this.Started = DateTime.UtcNow;

        public Job( [NotNull] Func<T> func, TimeSpan maxRuntime ) : this() {
            if ( func == null ) {
                throw new ArgumentNullException( paramName: nameof( func ) );
            }

            this.CTS = new CancellationTokenSource( maxRuntime );

            this.TheTask = new Task( () => {
                var result = func.Trap();
                this.Done( result );
            }, this.CTS.Token );
        }

        public Job( [NotNull] Action action, TimeSpan maxRuntime ) : this() {
            if ( action == null ) {
                throw new ArgumentNullException( paramName: nameof( action ) );
            }

            this.CTS = new CancellationTokenSource( maxRuntime );

            this.TheTask = new Task( () => {
                action.Trap();
                this.Done();
            }, this.CTS.Token );
        }

        private void Done() {
            this.Stopped = DateTime.UtcNow;
            $"Job (Task.Id={this.TheTask.Id}) is done.".Log();
            this.Result = default; //action doesn't have a "result"
        }

        private void Done( T ante ) {
            this.Stopped = DateTime.UtcNow;
            $"Job (Task.Id={this.TheTask.Id}) is done.".Log();
            this.Result = ante;
        }

        [NotNull]
        public static implicit operator Task( Job<T> job ) => job.TheTask;

        /// <summary>
        ///     aka Run()
        /// </summary>
        /// <returns></returns>
        public async Task AwaitTask() {
            try {
                await this.TheTask.ConfigureAwait( false );
            }
            catch ( TaskCanceledException ) {
                this.Result = default;
            }
        }

        /// <summary>
        ///     Returns the time spent running the task so far, or total time if the task is done.
        /// </summary>
        /// <returns></returns>
        public TimeSpan TimeTaken() {
            if ( this.TheTask.IsDone() ) {
                return this.Stopped - this.Started;
            }

            return DateTime.UtcNow - this.Started;
        }

    }

}