namespace Librainian.Threading {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// Task.Factory.StartNew(() => {
    ///     //everything here will be executed in a thread whose priority is BelowNormal
    /// }, null, TaskCreationOptions.None, PriorityScheduler.BelowNormal);
    /// </example>
    /// <seealso cref="http://stackoverflow.com/questions/3836584/lowering-priority-of-task-factory-startnew-thread"/>
    public class PriorityScheduler : TaskScheduler {
        public static PriorityScheduler AboveNormal = new PriorityScheduler( ThreadPriority.AboveNormal );
        public static PriorityScheduler BelowNormal = new PriorityScheduler( ThreadPriority.BelowNormal );
        public static PriorityScheduler Lowest = new PriorityScheduler( ThreadPriority.Lowest );

        private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        private Thread[] _threads;
        private readonly ThreadPriority _priority;
        private readonly int _maximumConcurrencyLevel = Math.Max( 1, Environment.ProcessorCount );

        public PriorityScheduler( ThreadPriority priority ) {
            this._priority = priority;
        }

        public override int MaximumConcurrencyLevel {
            get {
                return this._maximumConcurrencyLevel;
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks() {
            return this._tasks;
        }

        protected override void QueueTask( Task task ) {
            this._tasks.Add( task );

            if ( this._threads != null ) {
                return;
            }
            this._threads = new Thread[ this._maximumConcurrencyLevel ];
            for ( var i = 0 ; i < this._threads.Length ; i++ ) {
                this._threads[ i ] = new Thread( () => {
                                                     foreach ( var t in this._tasks.GetConsumingEnumerable() )
                                                         this.TryExecuteTask( t );
                                                 } ) {
                                                         Name = String.Format( "PriorityScheduler: ", i ),
                                                         Priority = this._priority,
                                                         IsBackground = true
                                                     };
                this._threads[ i ].Start();
            }
        }

        protected override bool TryExecuteTaskInline( Task task, bool taskWasPreviouslyQueued ) {
            return false; // we might not want to execute task that should schedule as high or low priority inline
        }
    }
}