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
// Project: "Librainian", "Idler.cs" was last formatted by Protiguous on 2019/08/08 at 9:36 AM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Collections.Sets;
    using JetBrains.Annotations;
    using Logging;
    using Magic;
    using Measurement.Time;
    using Error = System.Error;

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
        /// <param name="name"></param>
        /// <param name="action"> </param>
        void Add( [NotNull] String name, [NotNull] Action action );

        Boolean Any();

        /// <summary>
        ///     Run any remaining jobs.
        ///     <para>
        ///         Will exit while loop if <see cref="Librainian.Threading.Idler.CancellationTokenSource" /> is signaled to
        ///         cancel.
        ///     </para>
        /// </summary>
        void Finish();
    }

    public class Idler : ABetterClassDispose, IIdler {

        /// <summary>
        ///     Add an <paramref name="action" /> as a job to be ran on the next <see cref="Application.Idle" /> event.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"> </param>
        public void Add( [NotNull] String name, [NotNull] Action action ) {
            if ( action == null ) {
                throw new ArgumentNullException( paramName: nameof( action ) );
            }

            if ( String.IsNullOrWhiteSpace( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( name ) );
            }

            Error.Trap( () => this.Jobs.TryAdd( name, action ) );
        }

        public Boolean Any() => this.Jobs.Any();

        /// <summary>
        ///     Run any remaining jobs.
        ///     <para>Will exit prematurely if <see cref="CancellationTokenSource" /> is signaled to cancel.</para>
        /// </summary>
        public void Finish() {
            while ( this.Any() && !this.CancellationTokenSource.Token.IsCancellationRequested ) {
                this.NextJob();
            }
        }

        [NotNull]
        private CancellationTokenSource CancellationTokenSource { get; }

        [NotNull]
        private ConcurrentDictionary<String, Action> Jobs { get; } = new ConcurrentDictionary<String, Action>();

        [NotNull]
        private ConcurrentHashset<Task> Runners { get; } = new ConcurrentHashset<Task>();

        public Idler( [NotNull] CancellationTokenSource cancelSource ) {
            this.CancellationTokenSource = cancelSource ?? throw new ArgumentNullException( nameof( cancelSource ) );

            //this.Jobs.CollectionChanged += ( sender, args ) => this.NextJob();
            this.AddHandler();
        }

        private void AddHandler() => Application.Idle += this.OnIdle;

        /// <summary>
        ///     Pull next <see cref="Action" /> to run from the queue and execute it.
        /// </summary>
        private void NextJob() {
            if ( !this.Any() ) {
                return;
            }

            var job = this.Jobs.Keys.FirstOrDefault();

            if ( job == null || !this.Jobs.TryRemove( job, out var jack ) ) {
                return;
            }

            try {
                "Idle(): Running next job...".Trace();
                jack.Execute();
            }
            catch ( Exception exception ) {
                exception.Log();
            }
            finally {
                "Idle(): Done with job.".Trace();
            }
        }

        private void OnIdle( Object sender, EventArgs e ) => this.NextJob();

        private void RemoveHandler() => Application.Idle -= this.OnIdle;

        public override void DisposeManaged() {
            if ( !this.IsDisposed() ) {
                this.RemoveHandler();
                this.CancellationTokenSource.CancelAfter( Seconds.One );
            }
        }
    }
}