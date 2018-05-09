// Copyright 2017 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Tasks.cs" was last cleaned by Protiguous on 2017/04/02 at 10:13 PM

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

    /// <summary>
    /// Execute an <see cref="Action"/> on a <see cref="Timer"/>.
    /// </summary>
    public static class Tasks {

        /// <summary>
        /// http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other
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
        /// http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other
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
                    if ( completedTask.Exception != null ) {
                        completionSource.TrySetException( exceptions: completedTask.Exception.InnerExceptions );
                    }

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
        /// var tasks = new[] { Task.Delay(3000).ContinueWith(_ =&gt; 3), Task.Delay(1000).ContinueWith(_ =&gt; 1), Task.Delay(2000).ContinueWith(_ =&gt; 2), Task.Delay(5000).ContinueWith(_ =&gt; 5),
        /// Task.Delay(4000).ContinueWith(_ =&gt; 4), }; foreach (var bucket in Interleaved(tasks)) { var t = await bucket; int result = await t; Console.WriteLine("{0}: {1}", DateTime.Now, result); }
        /// </example>
        public static Task<Task<T>>[] Interleaved<T>( [NotNull] IEnumerable<Task<T>> tasks ) {
            if ( tasks is null ) {
                throw new ArgumentNullException( paramName: nameof( tasks ) );
            }

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
        /// Do the <paramref name="job"/> with a dataflow after a <see cref="System.Threading.Timer"/> .
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        public static async Task Then( this TimeSpan delay, [NotNull] Action job ) {
            if ( job is null ) {
                throw new ArgumentNullException( paramName: nameof( job ) );
            }

            await Task.Delay( delay: delay );

            await Task.Run( job );
        }

        /// <summary>
        /// <para>Do the <paramref name="job"/> with a dataflow after a <see cref="System.Threading.Timer"/>.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        public static async Task Then( this Span delay, [NotNull] Action job ) {
            if ( job is null ) {
                throw new ArgumentNullException( paramName: nameof( job ) );
            }

            await Task.Delay( delay: delay ).ConfigureAwait( false );

            await Task.Run( job ).ConfigureAwait( false );
        }

        /// <summary>
        /// <para>Do the <paramref name="job"/> with a dataflow after a <see cref="System.Threading.Timer"/>.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        public static async Task Then( this Milliseconds delay, [NotNull] Action job ) {
            if ( job is null ) {
                throw new ArgumentNullException( paramName: nameof( job ) );
            }

            await Task.Delay( delay: delay ).ConfigureAwait( false );

            await Task.Run( job ).ConfigureAwait( false );
        }

        /// <summary>
        /// <para>Do the <paramref name="job"/> with a dataflow after a <see cref="System.Threading.Timer"/>.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        public static async Task Then( this Seconds delay, [NotNull] Action job ) {
            if ( job is null ) {
                throw new ArgumentNullException( paramName: nameof( job ) );
            }

            await Task.Delay( delay: delay ).ConfigureAwait( false );

            await Task.Run( job ).ConfigureAwait( false );
        }

        /// <summary>
        /// <para>Do the <paramref name="job"/> with a dataflow after a <see cref="System.Threading.Timer"/>.</para>
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="job">  </param>
        /// <returns></returns>
        public static async Task Then( this Minutes delay, [NotNull] Action job ) {
            if ( job is null ) {
                throw new ArgumentNullException( paramName: nameof( job ) );
            }

            await Task.Delay( delay: delay ).ConfigureAwait( false );

            await Task.Run( job ).ConfigureAwait( false );
        }

        /// <summary>
        /// Wrap 3 methods into one.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pre">   </param>
        /// <param name="post">  </param>
        /// <returns></returns>
        public static Action Wrap( [CanBeNull] this Action action, [CanBeNull] Action pre, [CanBeNull] Action post ) => () => {
            try {
                pre?.Invoke();
            }
            catch ( Exception exception ) {
                exception.More();
            }

            try {
                action?.Invoke();
            }
            catch ( Exception exception ) {
                exception.More();
            }

            try {
                post?.Invoke();
            }
            catch ( Exception exception ) {
                exception.More();
            }
        };

        /// <summary>
        /// Keep posting to the <see cref="ITargetBlock{TInput}"/> until it posts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="item">  </param>
        public static void TryPost<T>( this ITargetBlock<T> target, T item ) {
            if ( target is null ) {
#if DEBUG
                throw new ArgumentNullException( paramName: nameof( target ) );
#else
                return;
#endif
            }

            if ( !target.Post( item: item ) ) {
                target.TryPost( item: item, delay: TimeExtensions.GetAverageDateTimePrecision() ); //retry
            }
        }

        /// <summary>
        /// After a delay, keep posting to the <see cref="ITargetBlock{TInput}"/> until it posts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="item">  </param>
        /// <param name="delay"> </param>
        public static System.Timers.Timer TryPost<T>( this ITargetBlock<T> target, T item, TimeSpan delay ) {
            if ( target is null ) {
                throw new ArgumentNullException( paramName: nameof( target ) );
            }

            try {
                if ( delay < Milliseconds.One ) {
                    delay = Milliseconds.One;
                }

                return delay.CreateTimer( () => target.TryPost( item: item ) ).AndStart();
            }
            catch ( Exception exception ) {
                exception.More();
                throw;
            }
        }

        /// <summary>
        /// Start a timer. When it fires, check the <paramref name="condition"/>, and if true do the <paramref name="action"/>.
        /// </summary>
        /// <param name="afterDelay"></param>
        /// <param name="action">    </param>
        /// <param name="condition"> </param>
        public static System.Timers.Timer When( this TimeSpan afterDelay, Func<Boolean> condition, Action action ) {
            if ( condition is null ) {
                throw new ArgumentNullException( paramName: nameof( condition ) );
            }
            if ( action is null ) {
                throw new ArgumentNullException( paramName: nameof( action ) );
            }
            try {
                return afterDelay.CreateTimer( () => {
                    if ( condition() ) {
                        action();
                    }
                } )
                                 .Once()
                                 .AndStart();
            }
            catch ( Exception exception ) {
                exception.More();
                return null;
            }
        }

        public static ConfiguredTaskAwaitable Multitude( params Action[] actions ) => Task.Run( () => { Parallel.Invoke( actions ); } ).ConfigureAwait( false );
    }
}