// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "TaskExtensions.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "TaskExtensions.cs" was last formatted by Protiguous on 2018/06/26 at 1:44 AM.

namespace Librainian.Threading {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Threading.Tasks.Dataflow;
	using Extensions;
	using JetBrains.Annotations;
	using Measurement.Time;
	using Timer = System.Timers.Timer;

	/// <summary>
	///     Execute an <see cref="Action{T}" /> on a <see cref="System.Timers.Timer" />.
	/// </summary>
	public static class TaskExtensions {

		/// <summary>
		///     http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="completedTask">   </param>
		/// <param name="completionSource"></param>
		private static void PropagateResult<T>( [NotNull] Task<T> completedTask, TaskCompletionSource<T> completionSource ) {
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
		///     "you can even have a timeout using the following simple extension method"
		/// </summary>
		/// <param name="task">             </param>
		/// <param name="token"></param>
		/// <returns></returns>
		[Obsolete( "Does not work as intended." )]
		public static Task AddCancellation( [NotNull] this Task task, CancellationToken token ) {
			if ( task == null ) {
				throw new ArgumentNullException( paramName: nameof( task ) );
			}

			return Task.WhenAny( task, Task.Delay( TimeSpan.MaxValue, token ) );
		}

		/// <summary>
		///     "you can even have a timeout using the following simple extension method"
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">             </param>
		/// <param name="token"></param>
		/// <returns></returns>
		[Obsolete( "Does not work as intended." )]
		[ItemNotNull]
		public static async Task<T> AddCancellation<T>( [NotNull] this Task<T> task, CancellationToken token ) {
			if ( task == null ) {
				throw new ArgumentNullException( paramName: nameof( task ) );
			}

			var t = await Task.WhenAny( task, Task.Delay( TimeSpan.MaxValue, token ) ).NoUI();

			if ( t != task ) {
				throw new OperationCanceledException( "timeout" );
			}

			return task.Result;
		}

		/// <summary>
		///     "you can even have a timeout using the following simple extension method"
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">             </param>
		/// <param name="timeout">          </param>
		/// <param name="token"></param>
		/// <returns></returns>
		/// <exception cref="TaskCanceledException">thrown when the task was cancelled?</exception>
		/// <exception cref="OperationCanceledException">thrown when <paramref name="timeout" /> happens?</exception>
		[Obsolete( "Does not work as intended." )]
		public static async Task<T> AddCancellation<T>( [NotNull] this Task<T> task, TimeSpan timeout, CancellationToken token ) {
			if ( task is null ) {
				throw new ArgumentNullException( paramName: nameof( task ) );
			}

			try {
				var winning = await Task.WhenAny( task, Task.Delay( timeout, token ) ).NoUI();

				if ( winning != task ) {
					throw new OperationCanceledException( "cancelled" );
				}

				return task.Result;
			}
			catch ( TaskCanceledException ) {
				return Task.FromCanceled<T>( token ).Result; //cancelled?
			}
			catch ( OperationCanceledException exception ) {
				return Task.FromException<T>( exception ).Result; //timeout?
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
					PropagateResult( completedTask: completed, completionSource: nextBox );
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
				inputTask.ContinueWith( continuationAction: Continuation, cancellationToken: CancellationToken.None, continuationOptions: TaskContinuationOptions.ExecuteSynchronously,
					scheduler: TaskScheduler.Default );
			}

			return results;
		}

		/// <summary>
		///     Returns true if the <paramref name="task" /> is Completed, Cancelled, or Faulted.
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static Boolean IsTaskDone( [NotNull] this Task task ) => task.IsCompleted || task.IsCanceled || task.IsFaulted;

		/// <summary>
		///     <para>Automatically apply <see cref="Task{TResult}.ConfigureAwait" /> to the <paramref name="task" />.</para>
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable NoUI( [NotNull] this Task task ) {
			if ( task == null ) {
				throw new ArgumentNullException( paramName: nameof( task ) );
			}

			return task.ConfigureAwait( false );
		}

		/// <summary>
		///     <para>Automatically apply <see cref="Task.ConfigureAwait" /> to the <paramref name="task" />.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable<T> NoUI<T>( [NotNull] this Task<T> task ) {
			if ( task == null ) {
				throw new ArgumentNullException( paramName: nameof( task ) );
			}

			return task.ConfigureAwait( false );
		}

		/// <summary>
		///     Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" /> .
		/// </summary>
		/// <param name="delay"></param>
		/// <param name="job">  </param>
		/// <returns></returns>
		public static async Task Then( this TimeSpan delay, [NotNull] Action job ) {
			if ( job is null ) {
				throw new ArgumentNullException( nameof( job ) );
			}

			await Task.Delay( delay: delay );
			await Task.Run( job );
		}

		/// <summary>
		///     <para>Do the <paramref name="job" /> with a dataflow after a <see cref="System.Threading.Timer" />.</para>
		/// </summary>
		/// <param name="delay"></param>
		/// <param name="job">  </param>
		/// <returns></returns>
		public static async Task Then( this SpanOfTime delay, [NotNull] Action job ) {
			if ( job is null ) {
				throw new ArgumentNullException( nameof( job ) );
			}

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
			if ( job is null ) {
				throw new ArgumentNullException( nameof( job ) );
			}

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
			if ( job is null ) {
				throw new ArgumentNullException( nameof( job ) );
			}

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
			if ( job is null ) {
				throw new ArgumentNullException( nameof( job ) );
			}

			await Task.Delay( delay: delay ).NoUI();
			await Task.Run( job ).NoUI();
		}

		public static Task Then( [NotNull] this Task first, [NotNull] Action next ) {

			//if ( next is null ) {
			//    throw new ArgumentNullException( "next" );
			//}

			if ( first is null ) {
				throw new ArgumentNullException( nameof( first ) );
			}

			if ( next is null ) {
				throw new ArgumentNullException( nameof( next ) );
			}

			var tcs = new TaskCompletionSource<Object>(); //Tasks.FactorySooner.CreationOptions

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
					catch ( Exception ex ) {
						tcs.TrySetException( ex );
					}
				}
			} );

			return tcs.Task;
		}

		[Obsolete( "use continuewith", true )]
		public static Task<T2> Then<T2>( [NotNull] this Task first, [NotNull] Func<Task<T2>> next ) {
			if ( first is null ) {
				throw new ArgumentNullException( nameof( first ) );
			}

			if ( next is null ) {
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

		public static Task Then<T1>( [NotNull] this Task<T1> first, [NotNull] Action<T1> next ) {
			if ( first is null ) {
				throw new ArgumentNullException( nameof( first ) );
			}

			if ( next is null ) {
				throw new ArgumentNullException( nameof( next ) );
			}

			var tcs = new TaskCompletionSource<Object>(); //Tasks.FactorySooner.CreationOptions

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
			if ( first is null ) {
				throw new ArgumentNullException( nameof( first ) );
			}

			if ( next is null ) {
				throw new ArgumentNullException( nameof( next ) );
			}

			var tcs = new TaskCompletionSource<Object>(); //Tasks.FactorySooner.CreationOptions

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
						var t = next( first.Result );

						if ( t is null ) {
							tcs.TrySetCanceled();
						}
						else {
							t.ContinueWith( delegate {
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
			if ( first is null ) {
				throw new ArgumentNullException( nameof( first ) );
			}

			if ( next is null ) {
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
			if ( first is null ) {
				throw new ArgumentNullException( nameof( first ) );
			}

			if ( next is null ) {
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
						var t = next( first.Result );

						if ( t is null ) {
							tcs.TrySetCanceled();
						}
						else {
							t.ContinueWith( delegate {
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

		/// <summary>
		///     Keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target"></param>
		/// <param name="item">  </param>
		/// <param name="token"></param>
		public static async Task TryPost<T>( [NotNull] this ITargetBlock<T> target, T item, CancellationToken token ) {
			if ( target is null ) {
				throw new ArgumentNullException( nameof( target ) );
			}

			while ( true ) {

				if ( await target.SendAsync( item, token ).NoUI() || token.IsCancellationRequested ) {
					break;
				}

				await Task.Delay( 16, token ).NoUI();
			}
		}

		/// <summary>
		///     <para>Automatically apply <see cref="Task.ConfigureAwait" /> to the <paramref name="task" />.</para>
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable UI( [NotNull] this Task task ) {
			if ( task == null ) {
				throw new ArgumentNullException( paramName: nameof( task ) );
			}

			return task.ConfigureAwait( true );
		}

		/// <summary>
		///     <para>Automatically apply <see cref="Task.ConfigureAwait" /> to the <paramref name="task" />.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable<T> UI<T>( [NotNull] this Task<T> task ) {
			if ( task == null ) {
				throw new ArgumentNullException( paramName: nameof( task ) );
			}

			return task.ConfigureAwait( true );
		}

		/// <summary>
		///     <para>Returns true if the task finished before the <paramref name="timeout" />.</para>
		///     <para>Use this function if the Task does not have a built-in timeout.</para>
		///     <para>This function does not end the given <paramref name="task" /> if it does timeout.</para>
		/// </summary>
		/// <param name="task">   </param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public static async Task<Boolean> Until( this Task task, TimeSpan timeout ) {
			var delay = Task.Delay( timeout );

			var whichTaskFinished = await Task.WhenAny( task, delay ).NoUI();

			var didOurTaskFinish = whichTaskFinished == task;

			return didOurTaskFinish;
		}

		/// <summary>
		///     Returns true if the task finished before the <paramref name="timeout" />.
		/// </summary>
		/// <param name="task">   </param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public static async Task<Boolean> Until( this TimeSpan timeout, Task task ) => await Until( task, timeout ).NoUI();

		/// <summary>
		///     Start a timer. When it fires, check the <paramref name="condition" />, and if true do the
		///     <paramref name="action" />.
		/// </summary>
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
				return afterDelay.CreateTimer( () => {
					if ( condition() ) {
						action();
					}
				} ).Once().AndStart();
			}
			catch ( Exception exception ) {
				exception.More();

				return null;
			}
		}

		/// <summary>
		///     "you can even have a timeout using the following simple extension method"
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">             </param>
		/// <param name="timeout">          </param>
		/// <returns></returns>
		/// <exception cref="OperationCanceledException">on timeout</exception>
		[Obsolete( "Does not work as intended." )]
		[ItemNotNull]
		public static async Task<T> WithTimeout<T>( [NotNull] this Task<T> task, TimeSpan timeout ) {
			if ( task == null ) {
				throw new ArgumentNullException( paramName: nameof( task ) );
			}

			var t = await Task.WhenAny( task, Task.Delay( timeout ) ).NoUI();

			if ( t != task ) {
				throw new OperationCanceledException( "timeout" );
			}

			return task.Result;
		}

		/// <summary>
		///     Wrap 3 methods into one.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="pre">   </param>
		/// <param name="post">  </param>
		/// <returns></returns>
		[NotNull]
		public static Action Wrap( [CanBeNull] this Action action, [CanBeNull] Action pre, [CanBeNull] Action post ) =>
			() => {
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

		/* Concept class

		/// <summary>
		///     After a delay, keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target"></param>
		/// <param name="item">  </param>
		/// <param name="delay"> </param>
		public static Timer TryPost<T>( this ITargetBlock<T> target, T item, TimeSpan delay ) {
			if ( target is null ) {
				throw new ArgumentNullException( nameof( target ) );
			}

			try {
				if ( delay < Milliseconds.One ) {
					delay = Milliseconds.One;
				}

				return delay.CreateTimer( () => target.Post( item ) ).AndStart();
			}
			catch ( Exception exception ) {
				exception.More();

				throw;
			}
		}
		*/

		/// <summary>
		///     var result = await Wrap( () =&gt; OldNonAsyncFunction( ) );
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public static async Task<T> Wrap<T>( [NotNull] this Func<T> selector ) {
			if ( selector is null ) {
				throw new ArgumentNullException( nameof( selector ) );
			}

			return await Task.Run( selector ).NoUI();
		}

		/// <summary>
		///     var result = await Wrap( () =&gt; OldNonAsyncFunction( "hello world" ) );
		/// </summary>
		/// <typeparam name="TIn"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="selector"></param>
		/// <param name="input">   </param>
		/// <returns></returns>
		public static async Task<TOut> Wrap<TIn, TOut>( [NotNull] this Func<TIn, TOut> selector, TIn input ) {
			if ( selector is null ) {
				throw new ArgumentNullException( nameof( selector ) );
			}

			return await Task.Run( () => selector( input ) ).NoUI();
		}

		/// <summary>
		///     Just a try/catch wrapper for methods.
		/// </summary>
		/// <param name="action">           </param>
		/// <param name="timeAction">       </param>
		/// <param name="andTookLongerThan"></param>
		/// <param name="onException">      </param>
		/// <param name="callerMemberName"> </param>
		/// <returns></returns>
		public static Boolean Wrap( [CanBeNull] this Action action, Boolean timeAction = true, TimeSpan? andTookLongerThan = null, [CanBeNull] Action onException = null,
			[CallerMemberName] String callerMemberName = "" ) {
			var attempts = 1;
			TryAgain:

			try {
				if ( null != action ) {
					if ( timeAction ) {
						var stopwatch = Stopwatch.StartNew();
						action();

						if ( stopwatch.Elapsed > ( andTookLongerThan ?? Milliseconds.ThreeHundredThirtyThree ) ) {
							$"{callerMemberName} took {stopwatch.Elapsed.Simpler()}.".WriteLine();
						}
					}
					else {
						action();
					}

					return true;
				}
			}
			catch ( OutOfMemoryException lowMemory ) {
				lowMemory.More();
				Logging.Garbage();
				attempts--;

				if ( attempts.Any() ) {
					goto TryAgain;
				}
			}
			catch ( Exception exception ) {
				if ( null != onException ) {
					return onException.Wrap();
				}

				exception.More();
			}

			return false;
		}
	}
}