// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Idler.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Idler.cs" was last formatted by Protiguous on 2019/02/02 at 12:16 AM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Collections.Sets;
    using Extensions;
    using JetBrains.Annotations;
    using Logging;
    using Magic;
    using Parsing;
    using Persistence;

    public enum JobStatus {

        Exception = -1,

        Unknown = 0,

        Running = 1,

        Finished
    }

    public interface IIdler {

        /// <summary>
        ///     Add an <paramref name="action" /> as a job to be ran on the next <see cref="Application.Idle" /> event.
        /// </summary>
        /// <param name="action">  </param>
        /// <param name="jobName"> </param>
        void Add( [NotNull] Action action, params String[] jobName );

        Boolean Any();

        /// <summary>
        ///     Run any remaining jobs.
        ///     <para>Will exit while loop if <see cref="Idler.CancellationTokenSource" /> is signaled to cancel.</para>
        /// </summary>
        void Finish();

        void Start();

        void Stop();
    }

    public class Idler : ABetterClassDispose, IIdler {

        private Boolean Active { get; set; }

        [NotNull]
        private CancellationTokenSource CancellationTokenSource { get; }

        private String CurrentJobName { get; set; }

        private ConcurrentStack<String> InsideJobs { get; } = new ConcurrentStack<String>();

        [NotNull]
        private ConcurrentQueue<Job> Jobs { get; } = new ConcurrentQueue<Job>();

        [NotNull]
        private ConcurrentHashset<Task> Runners { get; } = new ConcurrentHashset<Task>();

        public Idler( [NotNull] CancellationTokenSource cancelSource ) {
            this.CancellationTokenSource = cancelSource ?? throw new ArgumentNullException( nameof( cancelSource ) );
            this.AddHandler();
        }

        private void AddHandler() => Application.Idle += this.OnIdle;

        /// <summary>
        ///     Pull next <see cref="Job" /> to run from the queue and execute it.
        /// </summary>
        private void NextJob() {
            if ( !this.Active ) {
                return;
            }

            if ( !this.Jobs.TryDequeue( out var job ) ) {
                TimeSpan.FromSeconds( 0.1 ).Delay().Wait(); //ugh

                return;
            }

            try {
                this.CurrentJobName = job.Name;
                this.InsideJobs.Push( job.Name );

                $"Idle(): Running next job \"{job.Name}\"...".Log();
                job.Action.Execute();
            }
            catch ( Exception exception ) {
                exception.Log();
            }
            finally {
                this.InsideJobs.TryPop( out _ );
                this.CurrentJobName = null;
                $"Idle(): Done with job {job.Name}.".Log();
            }
        }

        private void OnIdle( Object sender, EventArgs e ) => this.NextJob();

        private void RemoveHandler() => Application.Idle -= this.OnIdle;

        /// <summary>
        ///     Add an <paramref name="action" /> as a job to be ran on the next <see cref="Application.Idle" /> event.
        /// </summary>
        /// <param name="action">  </param>
        /// <param name="jobName"> </param>
        public void Add( [NotNull] Action action, [NotNull] params String[] jobName ) {
            if ( action == null ) {
                throw new ArgumentNullException( paramName: nameof( action ) );
            }

            if ( jobName == null ) {
                throw new ArgumentNullException( paramName: nameof( jobName ) );
            }

            var jobKey = Cache.BuildKey( jobName );

            if ( jobKey.Like( this.CurrentJobName ) ) {
                return; //job is curently running
            }

            if ( this.Jobs.Any( job => job.Name.Like( jobKey ) ) ) {
                return; //job is already in Q.
            }

            if ( this.InsideJobs.Contains( jobKey ) ) {
                return; //job is already running further up the stack.
            }

            this.Jobs.Enqueue( new Job( action, jobKey ) );
        }

        public Boolean Any() => this.Active && this.Jobs.Any();

        public override void DisposeManaged() {
            if ( !this.IsDisposed ) {
                this.RemoveHandler();
                this.CancellationTokenSource.CancelAfter( TimeSpan.FromMinutes( 1 ) );
            }

            base.DisposeManaged();
        }

        /// <summary>
        ///     Run any remaining jobs.
        ///     <para>Will exit prematurely if <see cref="CancellationTokenSource" /> is signaled to cancel.</para>
        /// </summary>
        public void Finish() {
            while ( this.Any() && !this.CancellationTokenSource.Token.IsCancellationRequested ) {
                this.NextJob();
            }
        }

        public void Start() => this.Active = true;

        public void Stop() => this.Active = false;

        public class Job {

            [NotNull]
            public Action Action { get; }

            [NotNull]
            public String Name { get; }

            public Job( [NotNull] Action action, [NotNull] String name ) {
                this.Action = action ?? throw new ArgumentNullException( nameof( action ) );
                this.Name = name.NullIf( String.Empty ) ?? throw new ArgumentNullException( nameof( name ) );
            }
        }
    }
}