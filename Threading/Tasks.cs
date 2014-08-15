#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Tasks.cs" was last cleaned by Rick on 2014/08/15 at 10:39 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Annotations;
    using Measurement.Time;
    using Timer = System.Timers.Timer;

    public class PriorityBlock<T> {
        [NotNull]
        private readonly BufferBlock<T> _lowestPriorityTarget = new BufferBlock<T>( new DataflowBlockOptions { BoundedCapacity = 1 } );
        [NotNull]
        private readonly BufferBlock<T> _lowerPriorityTarget = new BufferBlock<T>( new DataflowBlockOptions { BoundedCapacity = 1 } );
        [NotNull]
        private readonly BufferBlock<T> _normalPriorityTarget = new BufferBlock<T>( new DataflowBlockOptions { BoundedCapacity = 1 } );
        [NotNull]
        private readonly BufferBlock<T> _higherPriorityTarget = new BufferBlock<T>( new DataflowBlockOptions { BoundedCapacity = 1 } );
        [NotNull]
        private readonly BufferBlock<T> _highestPriorityTarget = new BufferBlock<T>( new DataflowBlockOptions { BoundedCapacity = 1 } );

        private readonly BufferBlock<T> _source;

        /// <summary>
        /// </summary>
        /// <example>
        ///     b1.LinkTo(priorityBlock.HighPriorityTarget);
        ///     b2.LinkTo(priorityBlock.LowPriorityTarget);
        ///     priorityBlock.Source.LinkTo(a);
        /// </example>
        public PriorityBlock() {
            this._source = new BufferBlock<T>( new DataflowBlockOptions { BoundedCapacity = 1 } );
            this.TriageTask = Task.Run( () => this.Triage() );
        }

        public Task TriageTask { get; private set; }

        [NotNull]
        public ITargetBlock<T> HighestPriorityTarget { get { return this._highestPriorityTarget; } }

        [NotNull]
        public ITargetBlock<T> HigherPriorityTarget { get { return this._higherPriorityTarget; } }

        [NotNull]
        public ITargetBlock<T> NormalPriorityTarget { get { return this._normalPriorityTarget; } }

        [NotNull]
        public ITargetBlock<T> LowerPriorityTarget { get { return this._lowerPriorityTarget; } }

        [NotNull]
        public ITargetBlock<T> LowestPriorityTarget { get { return this._lowestPriorityTarget; } }

        public ISourceBlock<T> Source { get { return this._source; } }

        private async Task Triage() {
            while ( true ) {
                await Task.WhenAny(
                    this._highestPriorityTarget.OutputAvailableAsync(),
                    this._higherPriorityTarget.OutputAvailableAsync(),
                    this._normalPriorityTarget.OutputAvailableAsync(),
                    this._lowerPriorityTarget.OutputAvailableAsync(),
                    this._lowestPriorityTarget.OutputAvailableAsync()
                    );

                T item;

                if ( this._highestPriorityTarget.TryReceive( out item ) ) { await this._source.SendAsync( item ); }
                else if ( this._higherPriorityTarget.TryReceive( out item ) ) { await this._source.SendAsync( item ); }
                else if ( this._normalPriorityTarget.TryReceive( out item ) ) { await this._source.SendAsync( item ); }
                else if ( this._lowestPriorityTarget.TryReceive( out item ) ) { await this._source.SendAsync( item ); }
                else if ( this._lowestPriorityTarget.TryReceive( out item ) ) { await this._source.SendAsync( item ); }
                else {
                    // both input blocks must be completed
                    this._source.Complete();
                    return;
                }
            }
        }
    }

    public static class Tasks {
        public enum Priority : byte {
            Lowest,
            Low,
            Normal,
            High,
            Highest
        }

        public static long Counter;

        public static readonly TaskFactory Factory = new TaskFactory( creationOptions: TaskCreationOptions.PreferFairness, continuationOptions: TaskContinuationOptions.PreferFairness );

        public static readonly TaskFactory FactoryLater = new TaskFactory( creationOptions: TaskCreationOptions.None, continuationOptions: TaskContinuationOptions.None );

        private static readonly BufferBlock<Job> JobsBlock = new BufferBlock<Job>( dataflowBlockOptions: Blocks.ManyProducers.ConsumeSensible );

        public static readonly TransformBlock<Job, Job> PriorityBlock = new TransformBlock<Job, Job>();

        /// <summary>
        ///     dataflowBlockOptions: <see cref="Blocks.ManyProducers.ConsumeSensible" />
        /// </summary>
        private static readonly ActionBlock<Action> FireAndForget = new ActionBlock<Action>( action: action => {
            if ( null == action ) {
                return;
            }
            try {
                if ( !CancelJobs ) {
                    action();
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
            finally {
                Interlocked.Decrement( ref spawnCounter );
            }
        }, dataflowBlockOptions: Blocks.ManyProducers.ConsumeSensible );

        private static long spawnCounter;

        static Tasks() {
            JobsBlock.LinkTo();
        }

        /// <summary>
        ///     <para>Cancel has been requested. Don't queue or start any more spawns.</para>
        /// </summary>
        /// <remarks>Please don't set this to true.</remarks>
        public static Boolean CancelJobs { get; set; }

        public static long GetSpawnsWaiting() {
            return Interlocked.Read( ref spawnCounter );
        }

        private static void IncrementAndThenFireAndForget( Action job, Span? delay = null, Priority priority = Priority.Normal ) {
            if ( CancelJobs ) {
                return;
            }
            Interlocked.Increment( ref spawnCounter );
            if ( delay.HasValue ) {
                FireAndForget.TryPost( item: job, delay: delay.Value );
            }
            else {
                FireAndForget.TryPost( item: job );
            }
        }

        //TODO make priorty Spawns

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        /// <example>
        ///     var tasks = new[] {
        ///     Task.Delay(3000).ContinueWith(_ => 3),
        ///     Task.Delay(1000).ContinueWith(_ => 1),
        ///     Task.Delay(2000).ContinueWith(_ => 2),
        ///     Task.Delay(5000).ContinueWith(_ => 5),
        ///     Task.Delay(4000).ContinueWith(_ => 4),
        ///     };
        ///     foreach (var bucket in Interleaved(tasks)) {
        ///     var t = await bucket;
        ///     int result = await t;
        ///     Console.WriteLine("{0}: {1}", DateTime.Now, result);
        ///     }
        /// </example>
        public static Task<Task<T>>[] Interleaved<T>( [NotNull] IEnumerable<Task<T>> tasks ) {
            if ( tasks == null ) {
                throw new ArgumentNullException( "tasks" );
            }

            var inputTasks = tasks.ToList();

            var buckets = new TaskCompletionSource<Task<T>>[ inputTasks.Count ];

            var results = new Task<Task<T>>[ buckets.Length ];

            for ( var i = 0; i < buckets.Length; i++ ) {
                buckets[ i ] = new TaskCompletionSource<Task<T>>();
                results[ i ] = buckets[ i ].Task;
            }

            var nextTaskIndex = -1;
            Action<Task<T>> continuation = completed => {
                var bucket = buckets[ Interlocked.Increment( ref nextTaskIndex ) ];
                bucket.TrySetResult( completed );
            };

            foreach ( var inputTask in inputTasks ) {
                inputTask.ContinueWith( continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default );
            }

            return results;
        }

        public static int GetMaximumActiveWorkerThreads() {
            int maxWorkerThreads, maxPortThreads;
            ThreadPool.GetMaxThreads( workerThreads: out maxWorkerThreads, completionPortThreads: out maxPortThreads );
            return maxPortThreads;
        }

        public static Func<object> NewInstanceByLambda( [NotNull] this Type type ) {
            if ( type == null ) {
                throw new ArgumentNullException( "type" );
            }
            return Expression.Lambda<Func<object>>( Expression.New( type ) ).Compile();
        }

        /// <summary>
        ///     <para>Post the <paramref name="job" /> to the <see cref="FireAndForget" /> dataflow.</para>
        /// </summary>
        /// <param name="job"></param>
        /// <param name="delay"></param>
        /// <param name="priority"></param>
        public static void Spawn( [NotNull] this Action job, Span? delay = null, Priority priority = Priority.Normal ) {
            if ( job == null ) {
                throw new ArgumentNullException( "job" );
            }
            IncrementAndThenFireAndForget( job: job, delay: delay );
        }

        public static Func<object> NewInstanceByCreate( [NotNull] this Type type ) {
            if ( type == null ) {
                throw new ArgumentNullException( "type" );
            }
            var localType = type; // create a local copy to prevent adverse effects of closure
            Func<object> func = ( () => Activator.CreateInstance( localType ) ); // curry the localType
            return func;
        }

        /// <summary>
        ///     Do the <paramref name="job" /> with a <see cref="FireAndForget" /> dataflow after a
        ///     <see cref="System.Threading.Timer" />
        ///     .
        /// </summary>
        /// <param name="delay"> </param>
        /// <param name="job"> </param>
        /// <returns> </returns>
        public static void Then( this TimeSpan delay, [NotNull] Action job ) {
            if ( job == null ) {
                throw new ArgumentNullException( "job" );
            }
            IncrementAndThenFireAndForget( job: job, delay: delay );
        }

        /// <summary>
        ///     Do the <paramref name="job" /> with a <see cref="FireAndForget" /> dataflow after a
        ///     <see cref="System.Threading.Timer" />
        ///     .
        /// </summary>
        /// <param name="delay"> </param>
        /// <param name="job"> </param>
        /// <returns> </returns>
        public static void Then( this Milliseconds delay, [NotNull] Action job ) {
            if ( job == null ) {
                throw new ArgumentNullException( "job" );
            }
            IncrementAndThenFireAndForget( job: job, delay: delay );
        }

        /// <summary>
        ///     Do the <paramref name="job" /> with a <see cref="FireAndForget" /> dataflow after a
        ///     <see cref="System.Threading.Timer" /> fires off.
        ///     .
        /// </summary>
        /// <param name="delay"> </param>
        /// <param name="job"> </param>
        /// <returns> </returns>
        public static void Then( this Seconds delay, [NotNull] Action job ) {
            if ( job == null ) {
                throw new ArgumentNullException( "job" );
            }
            IncrementAndThenFireAndForget( job: job, delay: delay );
        }

        /// <summary>
        ///     Do the <paramref name="job" /> with a <see cref="FireAndForget" /> dataflow after a
        ///     <see cref="System.Threading.Timer" />
        ///     .
        /// </summary>
        /// <param name="delay"> </param>
        /// <param name="job"> </param>
        /// <returns> </returns>
        public static void Then( this Minutes delay, [NotNull] Action job ) {
            if ( job == null ) {
                throw new ArgumentNullException( "job" );
            }
            IncrementAndThenFireAndForget( job: job, delay: delay );
        }

        /// <summary>
        ///     Wrap 3 methods into one.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pre"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        public static Action Wrap( [CanBeNull] this Action action, [CanBeNull] Action pre, [CanBeNull] Action post ) {
            return () => {
                if ( pre != null ) {
                    pre();
                }
                if ( action != null ) {
                    action();
                }
                if ( post != null ) {
                    post();
                }
            };
        }

        ///// <summary>
        /////   This is untested.
        ///// </summary>
        //private static void SetThreadingMode() {
        //    SetMinimumWorkerThreads();

        //    SetMaximumActiveWorkerThreads();
        //}

        //private static void SetMaximumActiveWorkerThreads() {
        //    int maxWorkerThreads, maxPortThreads;
        //    ThreadPool.GetMaxThreads( out maxWorkerThreads, out maxPortThreads );
        //    maxWorkerThreads = 1 + Threads.ProcessorCount + .Parameters.DefaultSprouts;
        //    if ( ThreadPool.SetMaxThreads( workerThreads: maxWorkerThreads, completionPortThreads: maxPortThreads ) ) {
        //        Generic.Report( String.Format( "Successfully set max threads to {0} and I/O threads to {1}", maxWorkerThreads, maxPortThreads ) );
        //    }
        //}

        //private static void SetMinimumWorkerThreads() {
        //    int minWorkerThreads, minPortThreads;
        //    ThreadPool.GetMinThreads( out minWorkerThreads, out minPortThreads );
        //    minWorkerThreads = Threads.ProcessorCount;
        //    if ( ThreadPool.SetMinThreads( workerThreads: minWorkerThreads, completionPortThreads: minPortThreads ) ) {
        //        Generic.Report( String.Format( "Successfully set min threads to {0} and I/O threads to {1}", minWorkerThreads, minPortThreads ) );
        //    }
        //}

        //public static void SetGovernorTimeout( TimeSpan timeout ) { GovernorTimeout = timeout; }

        ///// <summary>
        /////   Start ASAP.
        ///// </summary>
        ///// <param name="job"> </param>
        ///// <param name="delay"> </param>
        ///// <returns> </returns>
        //public static Task Start( Action job, TimeSpan? delay = null ) {
        //    if ( job == null ) {
        //        throw new ArgumentNullException( "job" );
        //    }

        //    if ( delay.HasValue ) {
        //        return Task.Delay( delay.Value ).ContinueWith( task1 => job(), TaskContinuationOptions.PreferFairness );
        //    }
        //    return FactorySooner.StartNew( job );
        //}

        ///// <summary>
        /////     Start a timer. ContinueWith the job().
        ///// </summary>
        ///// <param name="func"> </param>
        ///// <returns> </returns>
        //[Obsolete]
        //public static async Task<Func<T>> Spawn<T>( this Func<T> func ) {
        //    //if ( func == null ) {
        //    //    return default (T);
        //    //}
        //    await Task.Delay( TaskModerator );
        //    var result = Factory.StartNew( () => func );
        //    return result.Result;
        //}

        ///// <summary>
        /////     just returns the task. Doesn't await() it.
        ///// </summary>
        ///// <returns> </returns>
        //[Obsolete]
        //public static Task Job( this  Action job ) {
        //    if ( job == null ) {
        //        throw new ArgumentNullException( "job" );
        //    }
        //    var task = Factory.StartNew( action: job );
        //    return task;
        //}

        //private static readonly Stopwatch LastSpawnCreatedReading = new Stopwatch();
        //private static Stopwatch LastSpawnConsumedReading = new Stopwatch();

        ///// <summary>
        /////   Start a timer. ContinueWith the job().
        ///// </summary>
        ///// <param name="delay"> </param>
        ///// <param name="task"> </param>
        ///// <returns> </returns>
        //public static async void Then( Action<Task> task, TimeSpan? delay = null ) {
        //    if ( task == null ) {
        //        throw new ArgumentNullException( "task" );
        //    }
        //    if ( !delay.HasValue ) {
        //        delay = Millisecond.One;
        //    }
        //    //Task.Delay( delay ).ContinueWith( task1 => job() /*, TaskContinuationOptions.PreferFairness */);
        //    await Task.Delay( delay.Value );
        //    await FactorySooner.StartNew( task );
        //}

        ///// <summary>
        /////   Start whenever. No hurry.
        ///// </summary>
        ///// <param name="job"> </param>
        ///// <param name="delay"> </param>
        //public static Task Then( this Action job, TimeSpan? delay = null ) {
        //    if ( job == null ) {
        //        throw new ArgumentNullException( "job" );
        //    }
        //    if ( !delay.HasValue ) { delay = Millisecond.TwoHundredEleven; }

        //    if ( delay.HasValue ) {
        //        return Task.Delay( delay.Value ).ContinueWith( task1 => job(), TaskContinuationOptions.None );  //possibly try LongRunning ??
        //    }
        //    return FactoryLater.StartNew( job );
        //    //Thread.Yield();
        //}

        //public static readonly TaskScheduler Scheduler = new TaskScheduler();
        //public static readonly TaskFactory Slowies = new TaskFactory( TaskCreationOptions.LongRunning, TaskContinuationOptions.LongRunning );

        //public static void RunQuickTask( Action action, Action before = null, Action after = null, TaskCreationOptions creationOptions = TaskCreationOptions.None ) {
        //    var task = Quickies.StartNew( action: () => {
        //        if ( null != before ) {
        //            try { before(); }
        //            catch { }
        //        }
        //        if ( null != action ) {
        //            try { action(); }
        //            catch { }
        //        }
        //        if ( null == after ) { return; }
        //        try { after(); }
        //        catch { }
        //    }, creationOptions: creationOptions );
        //    ActiveTasks.AddOrUpdate( task, DateTime.UtcNow, ( t, i ) => DateTime.UtcNow );
        //    DateTime dummy;
        //    task.ContinueWith( continuationAction: aftertask => ActiveTasks.TryRemove( task, out dummy ) );
        //}

        //public static Task Run( this IEnumerable<Action> tasks ) { return Run( delay: TimeSpan.Zero, tasks: tasks.ToArray() ); }

        //public static Task<Task> Run( Action action ) { return Run( delay: TimeSpan.Zero, tasks: action ); }

        //[Obsolete( "Use Tasks.Factory.StartNew() now." )]public static Task OldRun( params Action[] tasks ) { return OldRun( delay: TimeSpan.Zero, tasks: tasks ); }

        ///// <summary>
        ///// Nonblocking. Schedules the params-tasks to be ran.
        ///// </summary>
        ///// <param name="delay"> </param>
        ///// <param name="tasks"> </param>
        //[Obsolete( "Use Tasks.Factory.StartNew() now." )]
        //public static Task OldRun( TimeSpan delay, params Action[] tasks ) {
        //    if ( null == tasks ) { tasks = new Action[] { () => { } }; }

        //    var continueTask = default( Task );
        //    var mainTask = default( Task );
        //    foreach ( var action in tasks.Where( action => null != action ) ) {
        //        if ( default( Task ) == mainTask ) {
        //            if ( delay.Ticks > 0 ) { doDelay( delay ); }
        //            mainTask = Factory.StartNew( action: action );
        //            continueTask = mainTask;
        //            //ActiveTasks.AddOrUpdate( task, DateTime.UtcNow, ( id, dateTime ) => DateTime.UtcNow );    //so the task doesn't get GC'd before it runs..
        //        }
        //        else {
        //            continueTask = continueTask.ContinueWith( task2 => action() );
        //        }
        //    }
        //    //if ( default( Task ) == mainTask ) { return default( Task ); }
        //    //*task =*/ task.ContinueWith( key => {
        //    //    DateTime value;
        //    //    //ActiveTasks.TryRemove( key, out value );
        //    //    //var been = ActiveTasks.Where( pair => pair.Key.IsCompleted );
        //    //    //Parallel.ForEach( been, pair => ActiveTasks.TryRemove( pair.Key, out value ) );

        //    //} );
        //    return mainTask;
        //}

        //private static async void doDelay( TimeSpan delay ) {
        //    await Task.Delay( delay );
        //}

        //[Obsolete]
        //public static void Barrier( Barrier barrier, Action action ) {
        //    if ( barrier != null ) {
        //        barrier.AddParticipant();
        //    }
        //    OldRun( action, () => {
        //        try {
        //            if ( barrier != null ) {
        //                barrier.SignalAndWait();
        //            }
        //        }
        //        catch ( BarrierPostPhaseException exception ) {
        //            exception.Log();
        //        }
        //    } );
        //}
        /// <summary>
        ///     Keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="item"></param>
        public static void TryPost<T>( this ITargetBlock<T> target, T item ) {
            if ( target == null ) {
#if DEBUG
                throw new ArgumentNullException( "target" );
#else
                return;
#endif
            }

            if ( !target.Post( item ) ) {
                //var bob = target as IDataflowBlock;
                //if ( bob.Completion.IsCompleted  )
                TryPost( target: target, item: item, delay: Threads.GetSlicingAverage() ); //retry
            }
        }

        /// <summary>
        ///     After a delay, keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="item"></param>
        /// <param name="delay"></param>
        public static Timer TryPost<T>( this ITargetBlock<T> target, T item, Span delay ) {
            if ( target == null ) {
                throw new ArgumentNullException( "target" );
            }

            try {
                if ( delay < Milliseconds.One ) {
                    delay = Milliseconds.One;
                }
                return delay.Create( () => target.TryPost( item ) ).AndStart();
            }
            catch ( Exception exception ) {
                exception.Log();
                throw;
            }
        }

        public struct Job {
            public Action Action;
            public Priority Priority;
        }
    }
}
