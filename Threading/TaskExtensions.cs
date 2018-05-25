// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TaskExtensions.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/TaskExtensions.cs" was last formatted by Protiguous on 2018/05/22 at 4:40 PM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Timer = System.Timers.Timer;

    /// <summary>
    ///     Execute an <see cref="Action{T}" /> on a <see cref="System.Timers.Timer" />.
    /// </summary>
    public static class TaskExtensions {

        /// <summary>
        ///     <para>Automatically apply <see cref="Task{TResult}.ConfigureAwait" /> to the <paramref name="task" />.</para>
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredTaskAwaitable NoUI( this Task task ) => task.ConfigureAwait( false );

        /// <summary>
        ///     <para>Automatically apply <see cref="Task.ConfigureAwait" /> to the <paramref name="task" />.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredTaskAwaitable<T> NoUI<T>( this Task<T> task ) => task.ConfigureAwait( false );

        /// <summary>
        ///     <para>Automatically apply <see cref="Task.ConfigureAwait" /> to the <paramref name="task" />.</para>
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredTaskAwaitable UI( this Task task ) => task.ConfigureAwait( true );

        /// <summary>
        ///     <para>Automatically apply <see cref="Task.ConfigureAwait" /> to the <paramref name="task" />.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredTaskAwaitable<T> UI<T>( this Task<T> task ) => task.ConfigureAwait( true );

        /// <summary>
        ///     http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<Task<T>> InCompletionOrder<T>( this IEnumerable<Task<T>> source ) {
            var inputs = source.ToList();
            var boxes = inputs.Select( x => new TaskCompletionSource<T>() ).ToList();
            var currentIndex = -1;

            foreach ( var task in inputs ) {
                task.ContinueWith( completed => {
                    var nextBox = boxes[index: Interlocked.Increment( location: ref currentIndex )];
                    PropagateResult( completedTask: completed, completionSource: nextBox );
                }, continuationOptions: TaskContinuationOptions.ExecuteSynchronously );
            }

            return boxes.Select( box => box.Task );
        }

        /// <summary>
        ///     http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="completedTask">   </param>
        /// <param name="completionSource"></param>
        private static void PropagateResult<T>( Task<T> completedTask, TaskCompletionSource<T> completionSource ) {
            switch ( completedTask.Status ) {
                case TaskStatus.Canceled:
                    completionSource.TrySetCanceled();

                    break;

                case TaskStatus.Faulted:

                    if ( completedTask.Exception != null ) { completionSource.TrySetException( exceptions: completedTask.Exception.InnerExceptions ); }

                    break;

                case TaskStatus.RanToCompletion:
                    completionSource.TrySetResult( result: completedTask.Result );

                    break;

                default: throw new ArgumentException( "Task was not completed." );
            }
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
        public static Task<Task<T>>[] Interleaved<T>( [NotNull] IEnumerable<Task<T>> tasks ) {
            if ( tasks is null ) { throw new ArgumentNullException( nameof( tasks ) ); }

            var inputTasks = tasks.ToList();
            var buckets = new TaskCompletionSource<Task<T>>[inputTasks.Count];
            var results = new Task<Task<T>>[buckets.Length];

            for ( var i = 0; i < buckets.Length; i++ ) {
                buckets[i] = new TaskCompletionSource<Task<T>>();
                results[i] = buckets[i].Task;
            }

            var nextTaskIndex = -1;

            void Continuation( Task<T> completed ) {
                var bucket = buckets[Interlocked.Increment( location: ref nextTaskIndex )];
                bucket.TrySetResult( result: completed );
            }

            foreach ( var inputTask in inputTasks ) {
                inputTask.ContinueWith( continuationAction: Continuation, cancellationToken: CancellationToken.None, continuationOptions: TaskContinuationOptions.ExecuteSynchronously, scheduler: TaskScheduler.Default );
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
        //    if ( job is null ) {
        //        throw new ArgumentNullException( "job" );
        //    }
        //    AddThenFireAndForget( job: job, delay: delay );
        //}
        /// <summary>
        ///     Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" /> .
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        public static async Task Then( this TimeSpan delay, [NotNull] Action job ) {
            if ( job is null ) { throw new ArgumentNullException( nameof( job ) ); }

            await Task.Delay( delay: delay );
            await Task.Run( job );
        }

        /// <summary>
        ///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" />.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        public static async Task Then( this Span delay, [NotNull] Action job ) {
            if ( job is null ) { throw new ArgumentNullException( nameof( job ) ); }

            await Task.Delay( delay: delay ).NoUI();
            await Task.Run( job ).NoUI();
        }

        /// <summary>
        ///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" />.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        public static async Task Then( this Milliseconds delay, [NotNull] Action job ) {
            if ( job is null ) { throw new ArgumentNullException( nameof( job ) ); }

            await Task.Delay( delay: delay ).NoUI();
            await Task.Run( job ).NoUI();
        }

        /// <summary>
        ///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" />.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        public static async Task Then( this Seconds delay, [NotNull] Action job ) {
            if ( job is null ) { throw new ArgumentNullException( nameof( job ) ); }

            await Task.Delay( delay: delay ).NoUI();
            await Task.Run( job ).NoUI();
        }

        /// <summary>
        ///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" />.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        public static async Task Then( this Minutes delay, [NotNull] Action job ) {
            if ( job is null ) { throw new ArgumentNullException( nameof( job ) ); }

            await Task.Delay( delay: delay ).NoUI();
            await Task.Run( job ).NoUI();
        }

        /// <summary>
        ///     Wrap 3 methods into one.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pre">   </param>
        /// <param name="post">  </param>
        /// <returns></returns>
        public static Action Wrap( [CanBeNull] this Action action, [CanBeNull] Action pre, [CanBeNull] Action post ) =>
            () => {
                try { pre?.Invoke(); }
                catch ( Exception exception ) { exception.More(); }

                try { action?.Invoke(); }
                catch ( Exception exception ) { exception.More(); }

                try { post?.Invoke(); }
                catch ( Exception exception ) { exception.More(); }
            };

        /// <summary>
        ///     Keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="item">  </param>
        public static void TryPost<T>( this ITargetBlock<T> target, T item ) {
            if ( target is null ) {
#if DEBUG
                throw new ArgumentNullException( nameof( target ) );
#else
                return;
#endif
            }

            if ( !target.Post( item: item ) ) {
                target.TryPost( item: item, delay: TimeExtensions.GetAverageDateTimePrecision() ); //retry
            }
        }

        /// <summary>
        ///     After a delay, keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="item">  </param>
        /// <param name="delay"> </param>
        public static Timer TryPost<T>( this ITargetBlock<T> target, T item, TimeSpan delay ) {
            if ( target is null ) { throw new ArgumentNullException( nameof( target ) ); }

            try {
                if ( delay < Milliseconds.One ) { delay = Milliseconds.One; }

                return delay.CreateTimer( () => target.TryPost( item: item ) ).AndStart();
            }
            catch ( Exception exception ) {
                exception.More();

                throw;
            }
        }

        /// <summary>
        ///     Start a timer. When it fires, check the <paramref name="condition" />, and if true do the
        ///     <paramref name="action" />.
        /// </summary>
        /// <param name="afterDelay"></param>
        /// <param name="action">    </param>
        /// <param name="condition"> </param>
        [CanBeNull]
        public static Timer When( this TimeSpan afterDelay, Func<Boolean> condition, Action action ) {
            if ( condition is null ) { throw new ArgumentNullException( nameof( condition ) ); }

            if ( action is null ) { throw new ArgumentNullException( nameof( action ) ); }

            try {
                return afterDelay.CreateTimer( () => {
                    if ( condition() ) { action(); }
                } ).Once().AndStart();
            }
            catch ( Exception exception ) {
                exception.More();

                return null;
            }
        }
    }
}