// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Idler.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "Idler.cs" was last formatted by Protiguous on 2020/03/16 at 3:02 PM.

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
    using Utilities;
    using Error = System.Error;

    public enum JobStatus {

        Exception = -1,

        Unknown = 0,

        Running = 1,

        Finished
    }

    public interface IIdler {

        /// <summary>Add an <paramref name="action" /> as a job to be ran on the next <see cref="Application.Idle" /> event.</summary>
        /// <param name="name"></param>
        /// <param name="action"> </param>
        void Add( [NotNull] String name, [NotNull] Action action );

        Boolean Any();

        /// <summary>Run any remaining jobs.
        /// <para>Will exit while loop if <see cref="CancellationToken" /> is signaled to cancel.</para>
        /// </summary>
        void Finish();
    }

    public class Idler : ABetterClassDispose, IIdler {

        [NotNull]
        private ConcurrentDictionary<String, Action> Jobs { get; } = new ConcurrentDictionary<String, Action>();

        [NotNull]
        private ConcurrentHashset<Task> Runners { get; } = new ConcurrentHashset<Task>();

        private CancellationToken Token { get; }

        public Idler( CancellationToken token ) {
            this.Token = token;

            //this.Jobs.CollectionChanged += ( sender, args ) => this.NextJob();
            this.AddHandler();
        }

        private void AddHandler() => Application.Idle += this.OnIdle;

        /// <summary>Pull next <see cref="Action" /> to run from the queue and execute it.</summary>
        private void NextJob() {
            if ( !this.Any() || this.Token.IsCancellationRequested ) {
                return;
            }

            var job = this.Jobs.Keys.FirstOrDefault();

            if ( job is null || !this.Jobs.TryRemove( key: job, value: out var jack ) ) {
                return;
            }

            try {

                //"Idle(): Running next job...".Verbose();
                jack.Execute();
            }
            catch ( Exception exception ) {
                exception.Log();
            }
            finally {
                this.Nop();

                //"Idle(): Done with job.".Verbose();
            }
        }

        private void OnIdle( [CanBeNull] Object sender, [CanBeNull] EventArgs e ) => this.NextJob();

        private void RemoveHandler() {
            if ( !this.IsDisposed ) {
                Application.Idle -= this.OnIdle;
            }
        }

        /// <summary>Add an <paramref name="action" /> as a job to be ran on the next <see cref="Application.Idle" /> event.</summary>
        /// <param name="name"></param>
        /// <param name="action"> </param>
        public void Add( [NotNull] String name, [NotNull] Action action ) {
            if ( action is null ) {
                throw new ArgumentNullException( paramName: nameof( action ) );
            }

            if ( String.IsNullOrWhiteSpace( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( name ) );
            }

            Error.Trap( func: () => this.Jobs.TryAdd( key: name, value: action ) );
        }

        public Boolean Any() => this.Jobs.Any();

        public override void DisposeManaged() => this.RemoveHandler();

        /// <summary>Run any remaining jobs.
        /// <para>Will exit prematurely if <see cref="Token" /> is signaled to cancel.</para>
        /// </summary>
        public void Finish() {
            while ( this.Any() && !this.Token.IsCancellationRequested ) {
                this.NextJob();
            }
        }
    }
}