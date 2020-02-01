// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TaskExtensions.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "TaskExtensions.cs" was last formatted by Protiguous on 2020/01/31 at 12:31 AM.

namespace LibrainianCore.Threading {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Exceptions;
    using Extensions;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Measurement.Time;
    using NLog;
    using Timer = System.Timers.Timer;

    /// <summary>Remember: Tasks are born "hot" unless created with "var task=new Task();".</summary>
    public static class TaskExtensions {

        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        /// <summary>Quietly consume the <paramref name="task" /> on a background thread. Fire & forget.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="anything"></param>
        /// <returns></returns>
        /// <remarks>I know, this is a "thread" consuming a "task", but oh well. Don't use it. :)</remarks>
        public static void Consume<T>( [NotNull] this Task<T> task, [CanBeNull] Object anything = default ) {
            if ( task is null ) {
                throw new ArgumentNullException( nameof( task ) );
            }

            var bgWorker = new BackgroundWorker();

            bgWorker.DoWork += async ( sender, args ) => {
                try {
                    await task.ConfigureAwait( false );
                }
                catch ( AggregateException exceptions ) {
                    exceptions.Log();
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            };

            bgWorker.RunWorkerCompleted += ( sender, args ) => { };

            bgWorker.RunWorkerAsync( anything );
        }

        /// <summary>
        ///     <para><see cref="Task.Delay(TimeSpan)" /> for <paramref name="milliseconds" />.</para>
        /// </summary>
        /// <param name="milliseconds"></param>
        [NotNull]
        public static Task Delay( this Int32 milliseconds ) => TimeSpan.FromMilliseconds( milliseconds ).Delay();

        /// <summary></summary>
        /// <param name="delay">How long to run the delay.</param>
        [NotNull]
        public static Task Delay( this TimeSpan delay ) => Task.Delay( delay );

        [NotNull]
        public static Task Delay( [NotNull] this IQuantityOfTime delay ) {
            if ( delay is null ) {
                throw new ArgumentNullException( nameof( delay ) );
            }

            return Task.Delay( delay.ToTimeSpan() );
        }

        /// <summary>Returns true if the <paramref name="task" /> is Completed, Cancelled, or Faulted.</summary>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <remarks>Just calls <see cref="IsDone" />.</remarks>
        [DebuggerStepThrough]
        public static Boolean Done( [NotNull] this Task task ) => task.IsDone();

        /// <summary>Invokes each <see cref="Action" /> in the given <paramref name="action" /> in a try/catch.</summary>
        /// <param name="action"></param>
        /// <param name="args">  </param>
        /// <returns></returns>
        public static void Execute( [NotNull] this Action action, [CanBeNull] Object[] args = null ) {
            foreach ( var method in action.GetInvocationList() ) {
                try {
                    if ( method.Target is ISynchronizeInvoke syncInvoke && syncInvoke.InvokeRequired ) {
                        syncInvoke.Invoke( method, args );
                    }
                    else {
                        method.Method.Invoke( method.Target, args );
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }
        }

        /// <summary>Invokes each action in the given <paramref name="action" /> in a try/catch.</summary>
        /// <param name="action"></param>
        /// <param name="args">  </param>
        /// <returns></returns>
        public static void Execute<T>( [NotNull] this Action<T> action, [CanBeNull] Object[] args = null ) {
            foreach ( var method in action.GetInvocationList() ) {
                try {
                    if ( method.Target is ISynchronizeInvoke syncInvoke && syncInvoke.InvokeRequired ) {
                        syncInvoke.Invoke( method, args );
                    }
                    else {
                        method.Method.Invoke( method.Target, args );
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }
        }

        public static void ExecuteAsync( [NotNull] this Action action, [CanBeNull] Object[] args = null ) {
            foreach ( var method in action.GetInvocationList() ) {
                try {
                    if ( method.Target is ISynchronizeInvoke syncInvoke ) {
                        syncInvoke.BeginInvoke( method, args );
                    }
                    else {
                        method.Method.Invoke( method.Target, args ); //run it sync anyways
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }
        }

        public static void ExecuteAsync<T>( [NotNull] this Action<T> action, [CanBeNull] Object[] args = null ) {
            foreach ( var method in action.GetInvocationList() ) {
                try {
                    if ( method.Target is ISynchronizeInvoke syncInvoke ) {
                        syncInvoke.BeginInvoke( method, args );
                    }
                    else {
                        method.Method.Invoke( method.Target, args ); //run it sync anyways
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
#pragma warning disable CA1030 // Use events where appropriate
        public static void FireAndForget( [CanBeNull] this Task task ) => task.Nop();

#pragma warning restore CA1030 // Use events where appropriate

        public static async Task<TResult> FromEvent<TDelegate, TResult>( [NotNull] Func<TaskCompletionSource<TResult>, TDelegate> createDelegate,
            [NotNull] Action<TDelegate> registerDelegate, [NotNull] Action<TDelegate> unregisterDelegate, TimeSpan timeout ) {
            if ( createDelegate is null ) {
                throw new ArgumentNullException( nameof( createDelegate ) );
            }

            if ( registerDelegate is null ) {
                throw new ArgumentNullException( nameof( registerDelegate ) );
            }

            if ( unregisterDelegate is null ) {
                throw new ArgumentNullException( nameof( unregisterDelegate ) );
            }

            var tcs = new TaskCompletionSource<TResult>( TaskCreationOptions.RunContinuationsAsynchronously );

            var cts = new CancellationTokenSource( timeout );
            cts.Token.Register( () => tcs.TrySetCanceled() );

            var del = createDelegate( tcs );

            try {
                registerDelegate( del );

                return await tcs.Task.ConfigureAwait( false );
            }
            finally {

                // Unsubscribe from event.
                unregisterDelegate( del );
            }
        }

        /// <summary>http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<Task<T>> InCompletionOrder<T>( [NotNull] this IEnumerable<Task<T>> source ) {
            if ( source is null ) {
                throw new ArgumentNullException( nameof( source ) );
            }

            var inputs = source.ToList();
            var boxes = inputs.Select( x => new TaskCompletionSource<T>( TaskCreationOptions.RunContinuationsAsynchronously ) ).ToList();
            var currentIndex = -1;

            foreach ( var task in inputs ) {
                task.ContinueWith( completed => {
                    var nextBox = boxes[ index: Interlocked.Increment( location: ref currentIndex ) ];
                    completed.PropagateResult( completionSource: nextBox );
                }, continuationOptions: TaskContinuationOptions.ExecuteSynchronously );
            }

            return boxes.Select( box => box.Task );
        }

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        /// <example>
        /// var tasks = new[] { Task.Delay(3000).ContinueWith(_ =&gt; 3), Task.Delay(1000).ContinueWith(_ =&gt; 1), Task.Delay(2000).ContinueWith(_ =&gt; 2),
        /// Task.Delay(5000).ContinueWith(_ =&gt; 5), Task.Delay(4000).ContinueWith(_ =&gt; 4), }; foreach (var bucket in Interleaved(tasks)) { var t = await bucket; int result = await t;
        /// Console.WriteLine("{0}: {1}", DateTime.Now, result); }
        /// </example>
        [NotNull]
        public static Task<Task<T>>[] Interleaved<T>( [NotNull] IEnumerable<Task<T>> tasks ) {
            if ( tasks is null ) {
                throw new ArgumentNullException( nameof( tasks ) );
            }

            var inputTasks = tasks.ToList();
            var buckets = new TaskCompletionSource<Task<T>>[ inputTasks.Count ];
            var results = new Task<Task<T>>[ buckets.Length ];

            for ( var i = 0; i < buckets.Length; i++ ) {
                buckets[ i ] = new TaskCompletionSource<Task<T>>( TaskCreationOptions.RunContinuationsAsynchronously );
                results[ i ] = buckets[ i ].Task;
            }

            var nextTaskIndex = -1;

            void Continuation( Task<T> completed ) {
                if ( completed is null ) {
                    throw new ArgumentNullException( nameof( completed ) );
                }

                var bucket = buckets[ Interlocked.Increment( location: ref nextTaskIndex ) ];
                bucket.TrySetResult( result: completed );
            }

            foreach ( var inputTask in inputTasks ) {
                inputTask.ContinueWith( continuationAction: Continuation, cancellationToken: CancellationToken.None,
                    continuationOptions: TaskContinuationOptions.ExecuteSynchronously, scheduler: TaskScheduler.Default );
            }

            return results;
        }

        /// <summary>Returns true if the <paramref name="task" /> is Completed, Cancelled, or Faulted.</summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Boolean IsDone( [NotNull] this Task task ) {
            if ( task is null ) {
                throw new ArgumentNullException( nameof( task ) );
            }

            return task.IsCompleted || task.IsCanceled || task.IsFaulted;
        }

        /// <summary>
        ///     <para>Shortcut for Task.ConfigureAwait(false).</para>
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static ConfiguredTaskAwaitable NoUI( [NotNull] this Task task ) => task.ConfigureAwait( false );

        /// <summary>
        ///     <para>Shortcut for Task.ConfigureAwait(false).</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static ConfiguredTaskAwaitable<T> NoUI<T>( [NotNull] this Task<T> task ) => task.ConfigureAwait( false );

        /// <summary>http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="completedTask">   </param>
        /// <param name="completionSource"></param>
        /// <exception cref="ArgumentEmptyException"></exception>
        public static void PropagateResult<T>( [NotNull] this Task<T> completedTask, [NotNull] TaskCompletionSource<T> completionSource ) {
            if ( completedTask is null ) {
                throw new ArgumentNullException( nameof( completedTask ) );
            }

            if ( completionSource is null ) {
                throw new ArgumentNullException( nameof( completionSource ) );
            }

            switch ( completedTask.Status ) {
                case TaskStatus.Canceled:
                    completionSource.TrySetCanceled();

                    break;

                case TaskStatus.Faulted:

                    if ( completedTask.Exception != null ) {
                        completionSource.TrySetException( exceptions: completedTask.Exception.InnerExceptions );
                    }

                    break;

                case TaskStatus.RanToCompletion:
                    completionSource.TrySetResult( result: completedTask.Result );

                    break;

                default: throw new ArgumentEmptyException( "Task was not completed." );
            }
        }

        public static void SetFromTask<T>( [NotNull] this TaskCompletionSource<T> tcs, [NotNull] Task<T> task ) {
            if ( tcs is null ) {
                throw new ArgumentNullException( nameof( tcs ) );
            }

            if ( task is null ) {
                throw new ArgumentNullException( nameof( task ) );
            }

            if ( !task.IsCompleted ) {
                throw new ArgumentException( "Task must be complete" );
            }

            if ( task.IsCanceled ) {
                tcs.TrySetCanceled();
            }
            else if ( task.IsFaulted ) {
                var ex = ( Exception )task.Exception ?? new InvalidOperationException( "Faulted Task" );
                tcs.TrySetException( ex );
            }
            else {
                tcs.TrySetResult( task.Result );
            }
        }

        /// <summary>
        ///     <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        [NotNull]
        public static async Task Then( this TimeSpan delay, [NotNull] Action job ) {
            if ( job is null ) {
                throw new ArgumentNullException( nameof( job ) );
            }

            await Task.Delay( delay: delay ).ConfigureAwait( false );
            await Task.Run( job ).ConfigureAwait( false );
        }

        /// <summary>
        ///     <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <param name="token"></param>
        /// <returns></returns>
        [NotNull]
        public static async Task Then( this TimeSpan delay, [NotNull] Action job, CancellationToken token ) {
            if ( job is null ) {
                throw new ArgumentNullException( nameof( job ) );
            }

            await Task.Delay( delay: delay, cancellationToken: token ).ConfigureAwait( false );
            await new Task( job, token, TaskCreationOptions.PreferFairness | TaskCreationOptions.RunContinuationsAsynchronously ).ConfigureAwait( false );
        }

        /// <summary>
        ///     <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        [NotNull]
        public static async Task Then( [CanBeNull] this SpanOfTime delay, [NotNull] Action job ) {
            if ( job is null ) {
                throw new ArgumentNullException( nameof( job ) );
            }

            await Task.Delay( delay: delay ).ConfigureAwait( false );
            await Task.Run( job ).ConfigureAwait( false );
        }

        /// <summary>
        ///     <para>Continue the <paramref name="second" /> task after running the <paramref name="first" /> task.</para>
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [NotNull]
        public static async Task Then( [NotNull] this Task first, [NotNull] Task second ) {
            if ( first is null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( second is null ) {
                throw new ArgumentNullException( nameof( second ) );
            }

            await first.ConfigureAwait( false );
            await second.ConfigureAwait( false );
        }

        /// <summary>
        ///     <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <param name="token"></param>
        /// <returns></returns>
        [NotNull]
        public static async Task Then( [CanBeNull] this SpanOfTime delay, [NotNull] Action job, CancellationToken token ) {
            if ( job is null ) {
                throw new ArgumentNullException( nameof( job ) );
            }

            await Task.Delay( delay: delay, cancellationToken: token ).ConfigureAwait( false );
            await Task.Run( job, token ).ConfigureAwait( false );
        }

        /// <summary>
        ///     <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <param name="token"></param>
        /// <returns></returns>
        [NotNull]
        public static async Task Then( [NotNull] this IQuantityOfTime delay, [NotNull] Action job, CancellationToken token ) {
            if ( delay is null ) {
                throw new ArgumentNullException( nameof( delay ) );
            }

            if ( job is null ) {
                throw new ArgumentNullException( nameof( job ) );
            }

            await Task.Delay( delay: delay.ToTimeSpan(), cancellationToken: token ).ConfigureAwait( false );
            await Task.Run( job, token ).ConfigureAwait( false );
        }

        [NotNull]
        public static Task Then( [NotNull] this Task first, [NotNull] Action next, CancellationToken? token ) {
            if ( first is null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( next is null ) {
                throw new ArgumentNullException( nameof( next ) );
            }

            var tcs = new TaskCompletionSource<Object>( TaskCreationOptions.RunContinuationsAsynchronously );

            first.ContinueWith( task => {
                if ( first.IsFaulted ) {
                    if ( first.Exception != null ) {
                        tcs.TrySetException( first.Exception.InnerExceptions );
                    }
                }
                else if ( first.IsCanceled ) {
                    tcs.TrySetCanceled();
                }
                else {
                    try {
                        next();
                        tcs.TrySetResult( null );
                    }
                    catch ( Exception exception ) {
                        tcs.TrySetException( exception );
                    }
                }
            }, token ?? CancellationToken.None );

            return tcs.Task;
        }

        [NotNull]
        public static Task<T2> Then<T2>( [NotNull] this Task first, [NotNull] Func<Task<T2>> next ) {
            if ( first is null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( next is null ) {
                throw new ArgumentNullException( nameof( next ) );
            }

            var tcs = new TaskCompletionSource<T2>( TaskCreationOptions.RunContinuationsAsynchronously ); //Tasks.FactorySooner.CreationOptions

            first.ContinueWith( obj => {
                if ( first.IsFaulted ) {
                    if ( first.Exception != null ) {
                        tcs.TrySetException( first.Exception.InnerExceptions );
                    }
                }
                else if ( first.IsCanceled ) {
                    tcs.TrySetCanceled();
                }
                else {
                    try {
                        var t = next();

                        if ( t is null ) {
                            tcs.TrySetCanceled();
                        }
                        else {
                            t.ContinueWith( obj1 => {
                                if ( t.IsFaulted ) {
                                    if ( t.Exception != null ) {
                                        tcs.TrySetException( t.Exception.InnerExceptions );
                                    }
                                }
                                else if ( t.IsCanceled ) {
                                    tcs.TrySetCanceled();
                                }
                                else {
                                    tcs.TrySetResult( t.Result );
                                }
                            }, TaskContinuationOptions.ExecuteSynchronously );
                        }
                    }
                    catch ( Exception exc ) {
                        tcs.TrySetException( exc );
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously );

            return tcs.Task;
        }

        [NotNull]
        public static Task Then<T1>( [NotNull] this Task<T1> first, [NotNull] Action<T1> next ) {
            if ( first is null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( next is null ) {
                throw new ArgumentNullException( nameof( next ) );
            }

            var tcs = new TaskCompletionSource<Object>( TaskCreationOptions.RunContinuationsAsynchronously );

            first.ContinueWith( task => {
                if ( first.IsFaulted ) {
                    if ( first.Exception != null ) {
                        tcs.TrySetException( first.Exception.InnerExceptions );
                    }
                }
                else if ( first.IsCanceled ) {
                    tcs.TrySetCanceled();
                }
                else {
                    try {
                        next( first.Result );
                        tcs.TrySetResult( null );
                    }
                    catch ( Exception ex ) {
                        tcs.TrySetException( ex );
                    }
                }
            }, TaskContinuationOptions.PreferFairness );

            return tcs.Task;
        }

        [NotNull]
        public static Task Then<T1>( [NotNull] this Task<T1> first, [NotNull] Func<T1, Task> next ) {
            if ( first is null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( next is null ) {
                throw new ArgumentNullException( nameof( next ) );
            }

            var tcs = new TaskCompletionSource<Object>( TaskCreationOptions.RunContinuationsAsynchronously ); //Tasks.FactorySooner.CreationOptions

            first.ContinueWith( obj => {
                if ( first.IsFaulted ) {
                    if ( first.Exception != null ) {
                        tcs.TrySetException( first.Exception.InnerExceptions );
                    }
                }
                else if ( first.IsCanceled ) {
                    tcs.TrySetCanceled();
                }
                else {
                    try {
                        var t = next( first.Result );

                        if ( t is null ) {
                            tcs.TrySetCanceled();
                        }
                        else {
                            t.ContinueWith( obj1 => {
                                if ( t.IsFaulted ) {
                                    if ( t.Exception != null ) {
                                        tcs.TrySetException( t.Exception.InnerExceptions );
                                    }
                                }
                                else if ( t.IsCanceled ) {
                                    tcs.TrySetCanceled();
                                }
                                else {
                                    tcs.TrySetResult( null );
                                }
                            }, TaskContinuationOptions.ExecuteSynchronously );
                        }
                    }
                    catch ( Exception exc ) {
                        tcs.TrySetException( exc );
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously );

            return tcs.Task;
        }

        [NotNull]
        public static Task<T2> Then<T1, T2>( [NotNull] this Task<T1> first, [NotNull] Func<T1, T2> next ) {
            if ( first is null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( next is null ) {
                throw new ArgumentNullException( nameof( next ) );
            }

            var tcs = new TaskCompletionSource<T2>( TaskCreationOptions.RunContinuationsAsynchronously ); //Tasks.FactorySooner.CreationOptions

            first.ContinueWith( obj => {
                if ( first.IsFaulted ) {
                    if ( first.Exception is null ) {
                        return;
                    }

                    tcs.TrySetException( first.Exception.InnerExceptions );
                }
                else if ( first.IsCanceled ) {
                    tcs.TrySetCanceled();
                }
                else {
                    try {
                        var result = next( first.Result );
                        tcs.TrySetResult( result );
                    }
                    catch ( Exception ex ) {
                        tcs.TrySetException( ex );
                    }
                }
            } );

            return tcs.Task;
        }

        [NotNull]
        public static Task<T2> Then<T1, T2>( [NotNull] this Task<T1> first, [NotNull] Func<T1, Task<T2>> next ) {
            if ( first is null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( next is null ) {
                throw new ArgumentNullException( nameof( next ) );
            }

            var tcs = new TaskCompletionSource<T2>( TaskCreationOptions.RunContinuationsAsynchronously ); //Tasks.FactorySooner.CreationOptions

            void ContinuationFunction( Task<T1> obj ) {
                if ( first.IsFaulted ) {
                    if ( first.Exception != null ) {
                        tcs.TrySetException( first.Exception.InnerExceptions );
                    }
                }
                else if ( first.IsCanceled ) {
                    tcs.TrySetCanceled();
                }
                else {
                    try {
                        var t = next( first.Result );

                        if ( t is null ) {
                            tcs.TrySetCanceled();
                        }
                        else {
                            void ContinuationAction( Task<T2> obj1 ) {
                                if ( t.IsFaulted ) {
                                    if ( t.Exception != null ) {
                                        tcs.TrySetException( t.Exception.InnerExceptions );
                                    }
                                }
                                else if ( t.IsCanceled ) {
                                    tcs.TrySetCanceled();
                                }
                                else {
                                    tcs.TrySetResult( t.Result );
                                }
                            }

                            t.ContinueWith( ContinuationAction, TaskContinuationOptions.ExecuteSynchronously );
                        }
                    }
                    catch ( Exception exc ) {
                        tcs.TrySetException( exc );
                    }
                }
            }

            first.ContinueWith( ContinuationFunction, TaskContinuationOptions.ExecuteSynchronously );

            return tcs.Task;
        }

        /// <summary>Keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="item">  </param>
        /// <param name="token"></param>
        public static async Task TryPost<T>( [NotNull] this ITargetBlock<T> target, [CanBeNull] T item, CancellationToken token ) {
            if ( target is null ) {
                throw new ArgumentNullException( nameof( target ) );
            }

            while ( true ) {
                var task = target.SendAsync( item, token );

                await task.ConfigureAwait( false );

                if ( task.IsDone() || token.IsCancellationRequested ) {
                    break;
                }

                await Task.Delay( 0, token ).ConfigureAwait( false );
            }
        }

        /// <summary>
        ///     <para>Automatically apply <see cref="Task.ConfigureAwait" /> to the <paramref name="task" />.</para>
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredTaskAwaitable UI( [NotNull] this Task task ) => task.ConfigureAwait( true );

        /// <summary>
        ///     <para>Automatically apply <see cref="Task.ConfigureAwait" /> to the <paramref name="task" />.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredTaskAwaitable<T> UI<T>( [NotNull] this Task<T> task ) => task.ConfigureAwait( true );

        /// <summary>
        ///     <para>Returns true if the task finished before the <paramref name="timeout" />.</para>
        ///     <para>Use this function if the Task does not have a built-in timeout.</para>
        ///     <para>Note: This function does not end the given <paramref name="task" /> if it does timeout.</para>
        /// </summary>
        /// <param name="task">   </param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<Boolean> Until( [NotNull] this Task task, TimeSpan timeout ) {
            if ( task is null ) {
                throw new ArgumentNullException( nameof( task ) );
            }

            var delay = Task.Delay( timeout );

            var whichTaskFinished = await Task.WhenAny( task, delay ).ConfigureAwait( false );

            var didOurTaskFinish = whichTaskFinished == task;

            if ( !didOurTaskFinish ) {

                //do we want to cancel? how to cancel the task?
            }

            return didOurTaskFinish;
        }

        /// <summary>Returns true if the task finished before the <paramref name="timeout" />.</summary>
        /// <param name="task">   </param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [NotNull]
        public static Task<Boolean> Until( this TimeSpan timeout, [NotNull] Task task ) => Until( task, timeout );

        public static async Task<Boolean> WaitAsync( [NotNull] this Task task, TimeSpan timeout ) {
            if ( task is null ) {
                throw new ArgumentNullException( nameof( task ) );
            }

            var canceler = new CancellationTokenSource();
            var delay = Task.Delay( timeout, canceler.Token );

            var completed = await Task.WhenAny( task, delay ).ConfigureAwait( false );

            if ( completed != task ) {
                return default;
            }

            canceler.Cancel();

            await completed.ConfigureAwait( false );

            return true;
        }

        /// <summary>Start a timer. When it fires, check the <paramref name="condition" />, and if true do the <paramref name="action" />.</summary>
        /// <param name="afterDelay"></param>
        /// <param name="action">    </param>
        /// <param name="condition"> </param>
        [CanBeNull]
        public static Timer When( this TimeSpan afterDelay, [NotNull] Func<Boolean> condition, [NotNull] Action action ) {
            if ( condition is null ) {
                throw new ArgumentNullException( nameof( condition ) );
            }

            if ( action is null ) {
                throw new ArgumentNullException( nameof( action ) );
            }

            try {
                return FluentTimer.Start( afterDelay.Create( () => {
                    if ( condition() ) {
                        action();
                    }
                } ).Once() );
            }
            catch ( Exception exception ) {
                exception.Log();

                return null;
            }
        }

        /// <summary>Returns when at least 1 of the tasks are marked as <see cref="Done" />.</summary>
        /// <param name="count">How many tasks to complete before returning.</param>
        /// <param name="token"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static async Task WhenCount( Int32 count, CancellationToken token, [CanBeNull] params Task[] tasks ) {
            if ( tasks?.Length.Any() != true ) {
                return;
            }

            if ( tasks.Length < count ) {
                count = tasks.Length;
            }

            if ( !count.Any() ) {
                return;
            }

            while ( tasks.Count( task => task?.IsDone() == true ) < count ) {
                if ( !token.IsCancellationRequested ) {
                    await Task.WhenAny( tasks ).ConfigureAwait( false );
                }
            }
        }

        /// <summary>await the <paramref name="task" /> with a <paramref name="timeout" /> and/or a <see cref="CancellationToken" />.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">             </param>
        /// <param name="timeout">          </param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">thrown when the task was cancelled?</exception>
        /// <exception cref="OperationCanceledException">thrown when <paramref name="timeout" /> happens?</exception>
        [ItemNotNull]
        public static async Task<Task> With<T>( [NotNull] this Task<T> task, TimeSpan timeout, CancellationToken token ) {
            if ( task is null ) {
                throw new ArgumentNullException( nameof( task ) );
            }

            if ( task.IsDone() ) {
                return task;
            }

            var delay = Task.Delay( timeout, token );
            var winning = await Task.WhenAny( task, delay ).ConfigureAwait( false );

            return winning == task ? task : Task.FromException( new OperationCanceledException( "cancelled" ) );
        }

        /// <summary>await the <paramref name="task" /> with a <see cref="CancellationToken" />.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">             </param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ItemNotNull]
        public static async Task<Task> With<T>( [NotNull] this Task<T> task, CancellationToken token ) {
            if ( task is null ) {
                throw new ArgumentNullException( nameof( task ) );
            }

            if ( task.IsDone() ) {
                return task;
            }

            var delay = Task.Delay( TimeSpan.MaxValue, token );
            var winning = await Task.WhenAny( task, delay ).ConfigureAwait( false );

            return winning == task ? task : Task.FromException( new OperationCanceledException( "cancelled" ) );
        }

        /// <summary>await the <paramref name="task" /> with a <paramref name="timeout" />.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">             </param>
        /// <param name="timeout">          </param>
        /// <returns></returns>
        [ItemNotNull]
        public static async Task<Task> With<T>( [NotNull] this Task<T> task, TimeSpan timeout ) {
            if ( task is null ) {
                throw new ArgumentNullException( nameof( task ) );
            }

            if ( task.IsDone() ) {
                return task;
            }

            var token = new CancellationTokenSource( timeout ).Token;
            var delay = Task.Delay( timeout, token );
            var winner = await Task.WhenAny( task, delay ).ConfigureAwait( false );

            return winner == task ? task : Task.FromException( new OperationCanceledException( "timeout" ) );
        }

        /// <summary>"you can even have a timeout using the following simple extension method"</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">             </param>
        /// <param name="timeout">          </param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException">on timeout</exception>
        /// <exception cref="ArgumentNullException"></exception>
        [ItemNotNull]
        public static async Task<Task> WithTimeout<T>( [NotNull] this Task<T> task, TimeSpan timeout ) {
            if ( task == await Task.WhenAny( task, Task.Delay( timeout ) ).ConfigureAwait( false ) ) {
                return task;
            }

            throw new OperationCanceledException( "timeout" );
        }

        /// <summary>Wrap 3 methods into one.</summary>
        /// <param name="pre">   </param>
        /// <param name="action"></param>
        /// <param name="post">  </param>
        /// <returns></returns>
        [NotNull]
        public static Action Wrap( [CanBeNull] this Action action, [CanBeNull] Action pre, [CanBeNull] Action post ) =>
            () => {
                try {
                    pre?.Invoke();
                }
                catch ( Exception exception ) {
                    exception.Log();
                }

                try {
                    action?.Invoke();
                }
                catch ( Exception exception ) {
                    exception.Log();
                }

                try {
                    post?.Invoke();
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            };

        /// <summary>var result = await Wrap( () =&gt; OldNonAsyncFunction( ) );</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        [NotNull]
        public static Task<T> Wrap<T>( [NotNull] this Func<T> selector ) => Task.Run( selector );

        /// <summary>var result = await Wrap( () =&gt; OldNonAsyncFunction( "hello world" ) );</summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="selector"></param>
        /// <param name="input">   </param>
        /// <returns></returns>
        [NotNull]
        public static Task<TOut> Wrap<TIn, TOut>( [NotNull] this Func<TIn, TOut> selector, [CanBeNull] TIn input ) => Task.Run( () => selector( input ) );

        public class ResourceLoader<T> : IResourceLoader<T> {

            public Int32 Available => this._semaphore.CurrentCount;

            public Int32 Count => this._count;

            public Int32 MaxConcurrency { get; }

            [NotNull]
            public Task<T> GetAsync( CancellationToken cancelToken = new CancellationToken() ) {
                lock ( this._lock ) {
                    return this.WaitAndLoadAsync( cancelToken );
                }
            }

            public Boolean TryGet( [CanBeNull] out Task<T> resource, CancellationToken cancelToken = new CancellationToken() ) {
                lock ( this._lock ) {
                    if ( this._semaphore.CurrentCount == 0 ) {
                        resource = null;

                        return default;
                    }

                    resource = this.WaitAndLoadAsync( cancelToken );

                    return true;
                }
            }

            private Int32 _count;

            [NotNull]
            private Func<CancellationToken, Task<T>> _loader { get; }

            [NotNull]
            private Object _lock { get; } = new Object();

            [NotNull]
            private SemaphoreSlim _semaphore { get; }

            public ResourceLoader( [NotNull] Func<CancellationToken, Task<T>> loader, Int32 maxConcurrency ) {
                this._loader = loader ?? throw new ArgumentNullException( nameof( loader ) );
                this._semaphore = new SemaphoreSlim( maxConcurrency, maxConcurrency );
                this.MaxConcurrency = maxConcurrency;
            }

            [ItemNotNull]
            private async Task<T> WaitAndLoadAsync( CancellationToken cancelToken ) {
                Interlocked.Increment( ref this._count );

                using ( await this._semaphore.UseWaitAsync( cancelToken ).ConfigureAwait( false ) ) {
                    return await this._loader( cancelToken ).ConfigureAwait( false );
                }
            }
        }
    }
}