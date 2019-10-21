// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TaskExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "TaskExtensions.cs" was last formatted by Protiguous on 2019/08/08 at 9:38 AM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Exceptions;
    using JetBrains.Annotations;
    using Logging;
    using Measurement.Time;
    using NLog;
    using Timer = System.Timers.Timer;

    /// <summary>
    ///     Remember: Tasks are born "hot" unless created with "var task=new Task();".
    /// </summary>
    [SuppressMessage( "ReSharper", "AsyncConverter.AsyncMethodNamingHighlighting" )]
    public static class TaskExtensions {

        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="completedTask">   </param>
        /// <param name="completionSource"></param>
        /// <exception cref="ArgumentEmptyException"></exception>
        public static void PropagateResult<T>( [NotNull] this Task<T> completedTask, TaskCompletionSource<T> completionSource ) {

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

        /// <summary>
        ///     Quietly consume the <paramref name="task" /> on a background thread. Fire & forget.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="anything"></param>
        /// <returns></returns>
        /// <remarks>I know, this is a "thread" consuming a "task", but oh well. Don't use it. :)</remarks>
        public static void Consume<T>( [NotNull] this Task<T> task, [CanBeNull] Object anything = default ) {
            if ( task == null ) {
                throw new ArgumentNullException( paramName: nameof( task ) );
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

            bgWorker.RunWorkerCompleted += ( sender, args ) => {
                
            };

            bgWorker.RunWorkerAsync( anything );
        }

        /// <summary>
        ///     <para><see cref="Task.Delay(System.TimeSpan)" /> for <paramref name="milliseconds" />.</para>
        /// </summary>
        /// <param name="milliseconds"></param>
        public static async Task Delay( this Int32 milliseconds ) => await TimeSpan.FromMilliseconds( milliseconds ).Delay();

        /// <summary>
        /// </summary>
        /// <param name="delay">How long to run the delay.</param>
        public static async Task Delay( this TimeSpan delay ) {
            try {
                await Task.Delay( delay ).ConfigureAwait( false );
            }
            catch ( Exception exception ) {
                exception.Break();
            }
        }

        /// <summary>
        ///     Invokes each <see cref="Action" /> in the given <paramref name="action" /> in a try/catch.
        /// </summary>
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

        /// <summary>
        ///     Invokes each action in the given <paramref name="action" /> in a try/catch.
        /// </summary>
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

        /// <summary>
        ///     http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<Task<T>> InCompletionOrder<T>( [NotNull] this IEnumerable<Task<T>> source ) {
            if ( source == null ) {
                throw new ArgumentNullException( paramName: nameof( source ) );
            }

            var inputs = source.ToList();
            var boxes = inputs.Select( x => new TaskCompletionSource<T>() ).ToList();
            var currentIndex = -1;

            foreach ( var task in inputs ) {
                task.ContinueWith( completed => {
                    var nextBox = boxes[ index: Interlocked.Increment( location: ref currentIndex ) ];
                    completed.PropagateResult( completionSource: nextBox );
                }, continuationOptions: TaskContinuationOptions.ExecuteSynchronously );
            }

            return boxes.Select( box => box.Task );
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        /// <example>
        ///     var tasks = new[] { Task.Delay(3000).ContinueWith(_ =&gt; 3), Task.Delay(1000).ContinueWith(_ =&gt; 1),
        ///     Task.Delay(2000).ContinueWith(_ =&gt; 2), Task.Delay(5000).ContinueWith(_ =&gt; 5),
        ///     Task.Delay(4000).ContinueWith(_ =&gt; 4), }; foreach (var bucket in Interleaved(tasks)) { var t = await bucket; int
        ///     result = await t; Console.WriteLine("{0}: {1}", DateTime.Now, result); }
        /// </example>
        [NotNull]
        public static Task<Task<T>>[] Interleaved<T>( [NotNull] IEnumerable<Task<T>> tasks ) {
            if ( tasks == null ) {
                throw new ArgumentNullException( paramName: nameof( tasks ) );
            }

            var inputTasks = tasks.ToList();
            var buckets = new TaskCompletionSource<Task<T>>[ inputTasks.Count ];
            var results = new Task<Task<T>>[ buckets.Length ];

            for ( var i = 0; i < buckets.Length; i++ ) {
                buckets[ i ] = new TaskCompletionSource<Task<T>>();
                results[ i ] = buckets[ i ].Task;
            }

            var nextTaskIndex = -1;

            void Continuation( Task<T> completed ) {
                if ( completed == null ) {
                    throw new ArgumentNullException( paramName: nameof( completed ) );
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

        /// <summary>
        ///     Returns true if the <paramref name="task" /> is Completed, Cancelled, or Faulted.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Boolean IsDone( [NotNull] this Task task ) {
            if ( task == null ) {
                throw new ArgumentNullException( paramName: nameof( task ) );
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

        /// <summary>
        ///     <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        [NotNull]
        public static async Task Then( this TimeSpan delay, [NotNull] Action job ) {
            if ( job == null ) {
                throw new ArgumentNullException( paramName: nameof( job ) );
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
            if ( job == null ) {
                throw new ArgumentNullException( paramName: nameof( job ) );
            }

            await Task.Delay( delay: delay, cancellationToken: token ).ConfigureAwait( false );
            await Task.Run( job, token ).ConfigureAwait( false );
        }

        /// <summary>
        ///     <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        [NotNull]
        public static async Task Then( this SpanOfTime delay, [NotNull] Action job ) {
            if ( job == null ) {
                throw new ArgumentNullException( paramName: nameof( job ) );
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
            if ( first == null ) {
                throw new ArgumentNullException( paramName: nameof( first ) );
            }

            if ( second == null ) {
                throw new ArgumentNullException( paramName: nameof( second ) );
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
        public static async Task Then( this SpanOfTime delay, [NotNull] Action job, CancellationToken token ) {
            if ( job == null ) {
                throw new ArgumentNullException( paramName: nameof( job ) );
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
            if ( delay == null ) {
                throw new ArgumentNullException( paramName: nameof( delay ) );
            }

            if ( job == null ) {
                throw new ArgumentNullException( paramName: nameof( job ) );
            }

            await Task.Delay( delay: delay.ToTimeSpan(), cancellationToken: token ).ConfigureAwait( false );
            await Task.Run( job, token ).ConfigureAwait( false );
        }

        public static Task Then( [NotNull] this Task first, [NotNull] Action next, CancellationToken? token ) {
            if ( first == null ) {
                throw new ArgumentNullException( paramName: nameof( first ) );
            }

            if ( next == null ) {
                throw new ArgumentNullException( paramName: nameof( next ) );
            }

            var tcs = new TaskCompletionSource<Object>();

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

        public static Task<T2> Then<T2>( [NotNull] this Task first, [NotNull] Func<Task<T2>> next ) {
            if ( first == null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( next == null ) {
                throw new ArgumentNullException( nameof( next ) );
            }

            var tcs = new TaskCompletionSource<T2>(); //Tasks.FactorySooner.CreationOptions

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

                        if ( t == null ) {
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

        public static Task Then<T1>( [NotNull] this Task<T1> first, [NotNull] Action<T1> next ) {
            if ( first == null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( next == null ) {
                throw new ArgumentNullException( nameof( next ) );
            }

            var tcs = new TaskCompletionSource<Object>();

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
            } );

            return tcs.Task;
        }

        public static Task Then<T1>( [NotNull] this Task<T1> first, [NotNull] Func<T1, Task> next ) {
            if ( first == null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( next == null ) {
                throw new ArgumentNullException( nameof( next ) );
            }

            var tcs = new TaskCompletionSource<Object>(); //Tasks.FactorySooner.CreationOptions

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

                        if ( t == null ) {
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

        public static Task<T2> Then<T1, T2>( [NotNull] this Task<T1> first, [NotNull] Func<T1, T2> next ) {
            if ( first == null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( next == null ) {
                throw new ArgumentNullException( nameof( next ) );
            }

            var tcs = new TaskCompletionSource<T2>(); //Tasks.FactorySooner.CreationOptions

            first.ContinueWith( delegate {
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

        public static Task<T2> Then<T1, T2>( [NotNull] this Task<T1> first, [NotNull] Func<T1, Task<T2>> next ) {
            if ( first == null ) {
                throw new ArgumentNullException( nameof( first ) );
            }

            if ( next == null ) {
                throw new ArgumentNullException( nameof( next ) );
            }

            var tcs = new TaskCompletionSource<T2>(); //Tasks.FactorySooner.CreationOptions

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

                        if ( t == null ) {
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

        /// <summary>
        ///     Keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="item">  </param>
        /// <param name="token"></param>
        public static async Task TryPost<T>( [NotNull] this ITargetBlock<T> target, T item, CancellationToken token ) {
            if ( target == null ) {
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
            if ( task == null ) {
                throw new ArgumentNullException( paramName: nameof( task ) );
            }

            var delay = Task.Delay( timeout );

            var whichTaskFinished = await Task.WhenAny( task, delay ).ConfigureAwait( false );

            var didOurTaskFinish = whichTaskFinished == task;

            if ( !didOurTaskFinish ) {
                //do we want to cancel? how to cancel the task?
            }

            return didOurTaskFinish;
        }

        /// <summary>
        ///     Returns true if the task finished before the <paramref name="timeout" />.
        /// </summary>
        /// <param name="task">   </param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [NotNull]
        public static Task<Boolean> Until( this TimeSpan timeout, [NotNull] Task task ) => Until( task, timeout );

        public static async Task<Boolean> WaitAsync( [NotNull] this Task task, TimeSpan timeout ) {
            if ( task == null ) {
                throw new ArgumentNullException( paramName: nameof( task ) );
            }

            var canceler = new CancellationTokenSource();
            var delay = Task.Delay( timeout, canceler.Token );

            var completed = await Task.WhenAny( task, delay ).ConfigureAwait( false );

            if ( completed != task ) {
                return false;
            }

            canceler.Cancel();

            await completed.ConfigureAwait( false );

            return true;
        }

        /// <summary>
        ///     Start a timer. When it fires, check the <paramref name="condition" />, and if true do the
        ///     <paramref name="action" />.
        /// </summary>
        /// <param name="afterDelay"></param>
        /// <param name="action">    </param>
        /// <param name="condition"> </param>
        [CanBeNull]
        public static Timer When( this TimeSpan afterDelay, [NotNull] Func<Boolean> condition, [NotNull] Action action ) {
            if ( condition == null ) {
                throw new ArgumentNullException( nameof( condition ) );
            }

            if ( action == null ) {
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

        /// <summary>
        ///     await the <paramref name="task" /> with a <paramref name="timeout" /> and/or a <see cref="CancellationToken" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">             </param>
        /// <param name="timeout">          </param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">thrown when the task was cancelled?</exception>
        /// <exception cref="OperationCanceledException">thrown when <paramref name="timeout" /> happens?</exception>
        [ItemNotNull]
        public static async Task<Task> With<T>( [NotNull] this Task<T> task, TimeSpan timeout, CancellationToken token ) {
            if ( task == null ) {
                throw new ArgumentNullException( paramName: nameof( task ) );
            }

            if ( task.IsDone() ) {
                return task;
            }

            var delay = Task.Delay( timeout, token );
            var winning = await Task.WhenAny( task, delay ).ConfigureAwait( false );

            return winning == task ? task : Task.FromException( new OperationCanceledException( "cancelled" ) );
        }

        /// <summary>
        ///     await the <paramref name="task" /> with a <see cref="CancellationToken" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">             </param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ItemNotNull]
        public static async Task<Task> With<T>( [NotNull] this Task<T> task, CancellationToken token ) {
            if ( task == null ) {
                throw new ArgumentNullException( paramName: nameof( task ) );
            }

            if ( task.IsDone() ) {
                return task;
            }

            var delay = Task.Delay( TimeSpan.MaxValue, token );
            var winning = await Task.WhenAny( task, delay ).ConfigureAwait( false );

            return winning == task ? task : Task.FromException( new OperationCanceledException( "cancelled" ) );
        }

        /// <summary>
        ///     await the <paramref name="task" /> with a <paramref name="timeout" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">             </param>
        /// <param name="timeout">          </param>
        /// <returns></returns>
        [ItemNotNull]
        public static async Task<Task> With<T>( [NotNull] this Task<T> task, TimeSpan timeout ) {
            if ( task == null ) {
                throw new ArgumentNullException( paramName: nameof( task ) );
            }

            if ( task.IsDone() ) {
                return task;
            }

            var token = new CancellationTokenSource( timeout ).Token;
            var delay = Task.Delay( timeout, token );
            var winner = await Task.WhenAny( task, delay ).ConfigureAwait( false );

            return winner == task ? task : Task.FromException( new OperationCanceledException( "timeout" ) );
        }

        /// <summary>
        ///     "you can even have a timeout using the following simple extension method"
        /// </summary>
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

        /// <summary>
        ///     Wrap 3 methods into one.
        /// </summary>
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

        /// <summary>
        ///     var result = await Wrap( () =&gt; OldNonAsyncFunction( ) );
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        [NotNull]
        public static Task<T> Wrap<T>( [NotNull] this Func<T> selector ) => Task.Run( selector );

        /// <summary>
        ///     var result = await Wrap( () =&gt; OldNonAsyncFunction( "hello world" ) );
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="selector"></param>
        /// <param name="input">   </param>
        /// <returns></returns>
        [NotNull]
        public static Task<TOut> Wrap<TIn, TOut>( [NotNull] this Func<TIn, TOut> selector, TIn input ) => Task.Run( () => selector( input ) );
    }
}