// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "PriorityScheduler.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/PriorityScheduler.cs" was last cleaned by Protiguous on 2018/05/15 at 10:50 PM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// </summary>
    /// <example>
    ///     Task.Factory.StartNew(() =&gt; { //everything here will be executed in a thread whose priority is BelowNormal
    ///     }, null, TaskCreationOptions.None, PriorityScheduler.BelowNormal);
    /// </example>
    /// <seealso cref="http://stackoverflow.com/questions/3836584/lowering-priority-of-task-factory-startnew-thread" />
    public class PriorityScheduler : TaskScheduler, IDisposable {

        private readonly Int32 _maximumConcurrencyLevel = Math.Max( 1, Environment.ProcessorCount );
        private readonly ThreadPriority _priority;
        private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        private Thread[] _threads;
        public static PriorityScheduler AboveNormal = new PriorityScheduler( ThreadPriority.AboveNormal );

        public static PriorityScheduler BelowNormal = new PriorityScheduler( ThreadPriority.BelowNormal );

        public static PriorityScheduler Lowest = new PriorityScheduler( ThreadPriority.Lowest );

        public PriorityScheduler( ThreadPriority priority ) => this._priority = priority;

        public override Int32 MaximumConcurrencyLevel => this._maximumConcurrencyLevel;

        protected virtual void Dispose( Boolean sdfsss ) {
            if ( sdfsss ) { this._tasks.Dispose(); }

            GC.SuppressFinalize( this );
        }

        protected override IEnumerable<Task> GetScheduledTasks() => this._tasks;

        protected override void QueueTask( Task task ) {
            this._tasks.Add( task );

            if ( this._threads != null ) { return; }

            this._threads = new Thread[this._maximumConcurrencyLevel];

            for ( var i = 0; i < this._threads.Length; i++ ) {
                this._threads[i] = new Thread( () => {
                    foreach ( var t in this._tasks.GetConsumingEnumerable() ) { this.TryExecuteTask( t ); }
                } ) { Name = $"PriorityScheduler: {i}", Priority = this._priority, IsBackground = true };

                this._threads[i].Start();
            }
        }

        protected override Boolean TryExecuteTaskInline( Task task, Boolean taskWasPreviouslyQueued ) => false;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => this.Dispose( true );
    }
}