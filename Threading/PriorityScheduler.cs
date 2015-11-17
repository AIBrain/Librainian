// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/PriorityScheduler.cs" was last cleaned by Rick on 2015/06/12 at 3:14 PM

namespace Librainian.Threading {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary></summary>
    /// <example>
    /// Task.Factory.StartNew(() =&gt; { //everything here will be executed in a thread whose
    /// priority is BelowNormal }, null, TaskCreationOptions.None, PriorityScheduler.BelowNormal);
    /// </example>
    /// <seealso cref="http://stackoverflow.com/questions/3836584/lowering-priority-of-task-factory-startnew-thread" />
    public class PriorityScheduler : TaskScheduler {
        public static PriorityScheduler AboveNormal = new PriorityScheduler( ThreadPriority.AboveNormal );
        public static PriorityScheduler BelowNormal = new PriorityScheduler( ThreadPriority.BelowNormal );
        public static PriorityScheduler Lowest = new PriorityScheduler( ThreadPriority.Lowest );
        private readonly Int32 _maximumConcurrencyLevel = Math.Max( 1, Environment.ProcessorCount );
        private readonly ThreadPriority _priority;
        private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        private Thread[] _threads;

        public override Int32 MaximumConcurrencyLevel => this._maximumConcurrencyLevel;

        public PriorityScheduler(ThreadPriority priority) {
            this._priority = priority;
        }

        protected override IEnumerable<Task> GetScheduledTasks() => this._tasks;

        protected override void QueueTask(Task task) {
            this._tasks.Add( task );

            if ( this._threads != null ) {
                return;
            }
            this._threads = new Thread[ this._maximumConcurrencyLevel ];
            for ( var i = 0; i < this._threads.Length; i++ ) {
                this._threads[ i ] = new Thread( () => {
                    foreach ( var t in this._tasks.GetConsumingEnumerable() ) {
                        this.TryExecuteTask( t );
                    }
                } ) {
                    Name = $"PriorityScheduler: {i}",
                    Priority = this._priority,
                    IsBackground = true
                };
                this._threads[ i ].Start();
            }
        }

        protected override Boolean TryExecuteTaskInline(Task task, Boolean taskWasPreviouslyQueued) => false;
    }
}