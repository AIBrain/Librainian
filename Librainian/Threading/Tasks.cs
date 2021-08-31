// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Tasks.cs" last touched on 2021-04-25 at 10:33 AM by Protiguous.

namespace Librainian.Threading {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions;
	using Logging;
	using Measurement.Time;

	/// <summary>Execute an <see cref="Action" /> on a <see cref="Timer" />.</summary>
	public static class Tasks {

		/// <summary>http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="completedTask"></param>
		/// <param name="completionSource"></param>
		private static void PropagateResult<T>( Task<T> completedTask, TaskCompletionSource<T>? completionSource ) {
			switch ( completedTask.Status ) {
				case TaskStatus.Canceled:
					completionSource.TrySetCanceled();

					break;

				case TaskStatus.Faulted:
					if ( completedTask.Exception != null ) {
						completionSource.TrySetException( completedTask.Exception.InnerExceptions );
					}

					break;

				case TaskStatus.RanToCompletion:
					completionSource.TrySetResult( completedTask.Result );

					break;

				default:
					throw new ArgumentException( "Task was not completed." );
			}
		}

		/// <summary>http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		public static IEnumerable<Task<T>> InCompletionOrder<T>( this IEnumerable<Task<T>> source ) {
			var inputs = source.ToList();
			var boxes = inputs.Select( x => new TaskCompletionSource<T>() ).ToList();
			var currentIndex = -1;

			foreach ( var task in inputs ) {
				task.ContinueWith( completed => {
					var nextBox = boxes[Interlocked.Increment( ref currentIndex )];
					PropagateResult( completed, nextBox );
				}, TaskContinuationOptions.ExecuteSynchronously );
			}

			return boxes.Select( box => box.Task );
		}

		/// <summary></summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="tasks"></param>
		/// <example>
		///     var tasks = new[] { Task.Delay(3000).ContinueWith(_ =&gt; 3), Task.Delay(1000).ContinueWith(_ =&gt; 1),
		///     Task.Delay(2000).ContinueWith(_ =&gt; 2),
		///     Task.Delay(5000).ContinueWith(_ =&gt; 5), Task.Delay(4000).ContinueWith(_ =&gt; 4), }; foreach (var bucket in
		///     Interleaved(tasks)) { var t = await bucket; int result = await t;
		///     Console.WriteLine("{0}: {1}", DateTime.Now, result); }
		/// </example>
		public static Task<Task<T>>[] Interleaved<T>( IEnumerable<Task<T>> tasks ) {
			if ( tasks == null ) {
				throw new ArgumentEmptyException( nameof( tasks ) );
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
				var bucket = buckets[Interlocked.Increment( ref nextTaskIndex )];
				bucket.TrySetResult( completed );
			}

			foreach ( var inputTask in inputTasks ) {
				inputTask.ContinueWith( Continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default );
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
		//        throw new ArgumentEmptyException( "job" );
		//    }
		//    AddThenFireAndForget( job: job, delay: delay );
		//}

		public static Task Multitude( params Action[]? actions ) => Task.Run( () => Parallel.Invoke( actions ) );

		/// <summary>Do the <paramref name="job" /> with a dataflow after a <see cref="Timer" /> .</summary>
		/// <param name="delay"></param>
		/// <param name="job"></param>
		public static async Task Then( this TimeSpan delay, Action job ) {
			if ( job == null ) {
				throw new ArgumentEmptyException( nameof( job ) );
			}

			await Task.Delay( delay ).ConfigureAwait( false );

			await Task.Run( job ).ConfigureAwait( false );
		}

		/// <summary>
		///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="Timer" />.</para>
		/// </summary>
		/// <param name="delay"></param>
		/// <param name="job"></param>
		public static async Task Then( this Milliseconds delay, Action job ) {
			if ( job == null ) {
				throw new ArgumentEmptyException( nameof( job ) );
			}

			await Task.Delay( delay ).ConfigureAwait( false );

			await Task.Run( job ).ConfigureAwait( false );
		}

		/// <summary>
		///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="Timer" />.</para>
		/// </summary>
		/// <param name="delay"></param>
		/// <param name="job"></param>
		public static async Task Then( this Seconds delay, Action job ) {
			if ( job == null ) {
				throw new ArgumentEmptyException( nameof( job ) );
			}

			await Task.Delay( delay ).ConfigureAwait( false );

			await Task.Run( job ).ConfigureAwait( false );
		}

		/// <summary>
		///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="Timer" />.</para>
		/// </summary>
		/// <param name="delay"></param>
		/// <param name="job"></param>
		public static async Task Then( this Minutes delay, Action job ) {
			if ( job == null ) {
				throw new ArgumentEmptyException( nameof( job ) );
			}

			await Task.Delay( delay ).ConfigureAwait( false );

			await Task.Run( job ).ConfigureAwait( false );
		}

		/// <summary>Wrap 3 methods into one.</summary>
		/// <param name="action"></param>
		/// <param name="pre"></param>
		/// <param name="post"></param>
		public static Action Wrap( this Action? action, Action? pre, Action? post ) =>
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

		///// <summary>
		/////   This is untested.
		///// </summary>
		//private static void SetThreadingMode() {
		//    SetMinimumWorkerThreads();

		//    SetMaximumActiveWorkerThreads();
		//}

		//private static void SetMaximumActiveWorkerThreads() {
		//	ThreadPool.GetMaxThreads( out var maxWorkerThreads, out var maxPortThreads );
		//	maxWorkerThreads = 1 + Environment.ProcessorCount;
		//	if ( ThreadPool.SetMaxThreads( maxWorkerThreads, maxPortThreads ) ) {
		//		$"Successfully set max threads to {maxWorkerThreads} and I/O threads to {maxPortThreads}".Verbose();
		//	}
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
		//        throw new ArgumentEmptyException( "job" );
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
		//        throw new ArgumentEmptyException( "job" );
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
		//        throw new ArgumentEmptyException( "task" );
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
		//        throw new ArgumentEmptyException( "job" );
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

		// var continueTask = default( Task ); var mainTask = default( Task ); foreach ( var action
		// in tasks.Where( action => null != action ) ) { if ( default( Task ) == mainTask ) { if (
		// delay.Ticks > 0 ) { doDelay( delay ); } mainTask = Factory.StartNew( action: action );
		// continueTask = mainTask; //ActiveTasks.AddOrUpdate( task, DateTime.UtcNow, ( id, dateTime
		// ) => DateTime.UtcNow ); //so the task doesn't get GC'd before it runs.. } else {
		// continueTask = continueTask.ContinueWith( task2 => action() ); } } //if ( default( Task )
		// == mainTask ) { return default( Task ); } //*task =*/ task.ContinueWith( key => { //
		// DateTime value; // //ActiveTasks.TryRemove( key, out value ); // //var been =
		// ActiveTasks.Where( pair => pair.Key.IsCompleted ); // //Parallel.ForEach( been, pair =>
		// ActiveTasks.TryRemove( pair.Key, out value ) );

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
	}
}