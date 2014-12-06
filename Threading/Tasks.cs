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

#endregion License & Information

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Annotations;
    using Measurement.Time;

    /// <summary>
    /// Execute an <see cref="Action"/> on a <see cref="Timer"/>.
    /// </summary>
    public static class Tasks {
        //private static readonly BufferBlock<OneJob> JobsBlock = new BufferBlock<OneJob>( dataflowBlockOptions: Blocks.ManyProducers.ConsumeSensible );

        //public static readonly TransformBlock<OneJob, OneJob> PriorityBlock = new TransformBlock<OneJob, OneJob>();

        ///// <summary>
        /////     dataflowBlockOptions: <see cref="Blocks.ManyProducers.ConsumeSensible" />
        ///// </summary>
        //private static readonly ActionBlock<Action> FireAndForget = new ActionBlock<Action>( action: action => {
        //    if ( null == action ) {
        //        return;
        //    }
        //    try {
        //        if ( !CancelJobs ) {
        //            action();
        //        }
        //    }
        //    catch ( Exception exception ) {
        //        exception.Error();
        //    }
        //    finally {
        //        Interlocked.Decrement( ref spawnCounter );
        //    }
        //}, dataflowBlockOptions: Blocks.ManyProducers.ConsumeSensible );

        //private static long spawnCounter;

        /// <summary>
        ///     <para>Cancel has been requested. Don't queue or start any more spawns. If we're in a method, try to check the token.</para>
        /// </summary>
        public static readonly CancellationTokenSource CancelJobsTokenSource = new CancellationTokenSource(  );

/*
        public static readonly PriorityBlock JobPriorityBlock = new PriorityBlock( CancelAllJobsToken );
*/

/*
        /// <summary>
        ///     <para>Cancel has been requested. Don't queue any more spawns.</para>
        /// </summary>
        public static readonly CancellationToken CancelNewJobsToken = new CancellationToken( false );
*/

        //public static UInt64 GetSpawnsWaiting() {
        //    return ( UInt64 )Interlocked.Read( ref spawnCounter );
        //}

/*
        [Obsolete( "use Task.Run()" )]
        public static void Spawn( this Action job, Single priority = 0.50f, Span? delay = null ) {
            if ( CancelNewJobsToken.IsCancellationRequested ) {
                return;
            }
            if ( !delay.HasValue ) {
                var onejob = new OneJob( priority, job );
                JobPriorityBlock.Add( onejob );
            }
            else {
                delay.Value.Create( () => job.Spawn( priority ) ).AndStart();
            }
        }
*/

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

            for ( var i = 0 ; i < buckets.Length ; i++ ) {
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

        ///// <summary>
        /////     <para>Post the <paramref name="job" /> to the <see cref="FireAndForget" /> dataflow.</para>
        ///// </summary>
        ///// <param name="job"></param>
        ///// <param name="delay"></param>
        ///// <param name="priority"></param>
        //public static void Spawn( [NotNull] this Action job, Span? delay = null, Single priority ) {
        //    if ( job == null ) {
        //        throw new ArgumentNullException( "job" );
        //    }
        //    AddThenFireAndForget( job: job, delay: delay );
        //}

        /// <summary>
        ///     Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" />     .
        /// </summary>
        /// <param name="delay"> </param>
        /// <param name="job"> </param>
        /// <returns> </returns>
        public static async Task Then( this TimeSpan delay, [NotNull] Action job ) {
            if ( job == null ) {
                throw new ArgumentNullException( "job" );
            }

            await Task.Delay( delay );

            await Task.Run( job );
        }

        /// <summary>
        ///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" />.</para>
        /// </summary>
        /// <param name="delay"> </param>
        /// <param name="job"> </param>
        /// <returns> </returns>
        public static async Task Then( this Span delay, [NotNull] Action job ) {
            if ( job == null ) {
                throw new ArgumentNullException( "job" );
            }
            await Task.Delay( delay );

            await Task.Run( job );
        }

        /// <summary>
        ///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" />.</para>
        /// </summary>
        /// <param name="delay"> </param>
        /// <param name="job"> </param>
        /// <returns> </returns>
        public static async Task Then( this Milliseconds delay, [NotNull] Action job ) {
            if ( job == null ) {
                throw new ArgumentNullException( "job" );
            }
            await Task.Delay( delay );

            await Task.Run( job );
        }

        /// <summary>
        ///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" />.</para>
        /// </summary>
        /// <param name="delay"> </param>
        /// <param name="job"> </param>
        /// <returns> </returns>
        public static async Task Then( this Seconds delay, [NotNull] Action job ) {
            if ( job == null ) {
                throw new ArgumentNullException( "job" );
            }
            await Task.Delay( delay );

            await Task.Run( job );
        }

        /// <summary>
        ///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" />.</para>
        /// </summary>
        /// <param name="delay"> </param>
        /// <param name="job"> </param>
        /// <returns> </returns>
        public static async void Then( this Minutes delay, [NotNull] Action job ) {
            if ( job == null ) {
                throw new ArgumentNullException( "job" );
            }
            await Task.Delay( delay );

            await Task.Run( job );
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
        //            exception.Error();
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
                TryPost( target: target, item: item, delay: ( Span )Threads.GetSlicingAverage() ); //retry
            }
        }

        /// <summary>
        ///     After a delay, keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="item"></param>
        /// <param name="delay"></param>
        public static System.Timers.Timer TryPost<T>( this ITargetBlock<T> target, T item, Span delay ) {
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
                exception.Debug();
                throw;
            }
        }

        /// <summary>
        ///     Start a timer. When it fires, check the <paramref name="condition" />, and if true do the
        ///     <paramref name="action" />.
        /// </summary>
        /// <param name="afterDelay"></param>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        public static System.Timers.Timer When( this Span afterDelay, Func<Boolean> condition, Action action ) {
            if ( condition == null ) {
                throw new ArgumentNullException( "condition" );
            }
            if ( action == null ) {
                throw new ArgumentNullException( "action" );
            }
            try {
                return afterDelay.Create( () => {
                    if ( condition() ) {
                        action();
                    }
                } ).Once().AndStart();
            }
            catch ( Exception exception ) {
                exception.Debug();
                return null;
            }
        }
    }

}