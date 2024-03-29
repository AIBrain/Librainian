﻿// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code
//  (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting.
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
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "TaskExtensions.cs" last formatted on 2021-02-23 at 10:56 PM.

#nullable enable

namespace Librainian.Threading {

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
	using JetBrains.Annotations;
	using Logging;
	using Maths;
	using Measurement.Time;
	using PooledAwait;

	/// <summary>
	///     Remember: Tasks are born "hot" unless created with "var task=new Task();".
	/// </summary>
	public static class TaskExtensions {

		/// <summary>
		/// Throws <see cref="OperationCanceledException"></see> if any tokens that have been cancelled.
		/// </summary>
		/// <param name="tokens"></param>
		/// <exception cref="OperationCanceledException"></exception>
		public static void ThrowAnyCancelledTokens( [NotNull] params CancellationToken[] tokens ) {
			if ( tokens == null ) {
				throw new ArgumentNullException( nameof( tokens ) );
			}

			foreach ( var cancellationToken in tokens ) {
				cancellationToken.ThrowIfCancellationRequested();
			}
		}

		/// <summary>Quietly consume the <paramref name="task" /> on a background thread. Fire & forget.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task"></param>
		/// <param name="anything"></param>
		/// <returns></returns>
		[Obsolete( "Don't use it. Unless you know what/why it does." )]
		public static void Consume<T>( [NotNull] this Task<T> task, [CanBeNull] Object? anything = default ) {
			if ( task is null ) {
				throw new ArgumentNullException( nameof( task ) );
			}

			using var bgWorker = new BackgroundWorker();
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
			bgWorker.DoWork += async ( _, _ ) => {
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
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
			bgWorker.RunWorkerCompleted += ( _, _ ) => { };
			bgWorker.RunWorkerAsync( anything );
		}

		/// <summary>
		///     <para><see cref="Task.Delay(TimeSpan)" /> for <paramref name="milliseconds" />.</para>
		/// </summary>
		/// <param name="milliseconds"></param>
		[NotNull]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Task Delay( this Int32 milliseconds ) => TimeSpan.FromMilliseconds( milliseconds ).Delay();

		/// <summary>
		///     Just a wrapper for <see cref="Task.Delay(Int32)" />.
		/// </summary>
		/// <param name="delay"></param>
		/// <returns></returns>
		[NotNull]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Task Delay( this TimeSpan delay ) => Task.Delay( delay );

		/// <summary>
		///     Just a wrapper for <see cref="Task.Delay(Int32)" />.
		/// </summary>
		/// <param name="delay"></param>
		/// <returns></returns>
		[NotNull]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Task Delay( [NotNull] this IQuantityOfTime delay ) => Task.Delay( delay.ToTimeSpan() );

		/// <summary>
		///     Invokes each <see cref="Action" /> in the given <paramref name="action" /> in a try/catch.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="args">  </param>
		/// <returns></returns>
		public static void Execute( [NotNull] this Action action, [CanBeNull] Object[]? args = default ) {
			foreach ( var method in action.GetInvocationList() ) {
				try {
					if ( method.Target is ISynchronizeInvoke { InvokeRequired: true } syncInvoke ) {
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
		public static void Execute<T>( [NotNull] this Action<T> action, [CanBeNull] Object[]? args = null ) {
			foreach ( var method in action.GetInvocationList() ) {
				try {
					if ( method.Target is ISynchronizeInvoke { InvokeRequired: true } syncInvoke ) {
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

		public static void ExecuteAsync( [NotNull] this Action action, [CanBeNull] Object[]? args = null ) {
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

		public static void ExecuteAsync<T>( [NotNull] this Action<T> action, [CanBeNull] Object[]? args = null ) {
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

			using var cts = new CancellationTokenSource( timeout );
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

		/// <summary>
		///     Required for await support.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="lazy"></param>
		/// <returns></returns>
		public static Awaiter<T> GetAwaiter<T>( this Lazy<T> lazy ) => new( lazy );

		/// <summary>
		///     http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<Task<T>> InCompletionOrder<T>( [NotNull] this IEnumerable<Task<T>> source ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			var inputs = source.ToList();
			var boxes = inputs.Select( _ => new TaskCompletionSource<T>( TaskCreationOptions.RunContinuationsAsynchronously ) ).ToList();
			var currentIndex = -1;

			foreach ( var task in inputs ) {
				task.ContinueWith( completed => {
					var nextBox = boxes[ Interlocked.Increment( ref currentIndex ) ];
					completed.PropagateResult( nextBox );
				}, TaskContinuationOptions.ExecuteSynchronously );
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
		///     Task.Delay(2000).ContinueWith(_ =&gt; 2),
		///     Task.Delay(5000).ContinueWith(_ =&gt; 5), Task.Delay(4000).ContinueWith(_ =&gt; 4), }; foreach (var bucket in
		///     Interleaved(tasks)) { var t = await bucket; int
		///     result = await t; Console.WriteLine("{0}: {1}", DateTime.Now, result); }
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

				var bucket = buckets[ Interlocked.Increment( ref nextTaskIndex ) ];
				bucket.TrySetResult( completed );
			}

			foreach ( var inputTask in inputTasks ) {
				inputTask.ContinueWith( Continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default );
			}

			return results;
		}

		/// <summary>
		///     Returns true if the <paramref name="task" /> is Completed, Cancelled, or Faulted.
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static Boolean IsDone( [NotNull] this Task task ) => task.IsCompleted || task.IsCanceled || task.IsFaulted;

		public static CancellationToken LinkTo( this CancellationToken firstToken, CancellationToken secondToken ) {
			if ( firstToken == secondToken ) {
				return firstToken; //could throw here, but why?
			}

			return CancellationTokenSource.CreateLinkedTokenSource( firstToken, secondToken ).Token;
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
		///     http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other
		/// </summary>
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
						completionSource.TrySetException( completedTask.Exception.InnerExceptions );
					}

					break;

				case TaskStatus.RanToCompletion:
					completionSource.TrySetResult( completedTask.Result );

					break;

				default:
					throw new ArgumentEmptyException( "Task was not completed." );
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
				var ex = task.Exception as Exception ?? new InvalidOperationException( "Faulted Task" );
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
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[NotNull]
		public static async Task Then( this TimeSpan delay, [NotNull] Action job, CancellationToken cancellationToken ) {
			if ( job is null ) {
				throw new ArgumentNullException( nameof( job ) );
			}

			await Task.Delay( delay, cancellationToken ).ConfigureAwait( false );
			await new Task( job, cancellationToken, TaskCreationOptions.PreferFairness | TaskCreationOptions.RunContinuationsAsynchronously ).ConfigureAwait( false );
		}

		/// <summary>
		///     <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
		/// </summary>
		/// <param name="delay"></param>
		/// <param name="job">  </param>
		/// <returns></returns>
		[NotNull]
		public static async Task Then( this SpanOfTime delay, [NotNull] Action job ) {
			if ( job is null ) {
				throw new ArgumentNullException( nameof( job ) );
			}

			await Task.Delay( delay ).ConfigureAwait( false );
			await Task.Run( job ).ConfigureAwait( false );
		}

		/// <summary>
		///     <para>Continue the <paramref name="second" /> task after running the <paramref name="first" /> task.</para>
		/// </summary>
		/// <param name="first"> </param>
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
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[NotNull]
		public static async Task Then( this SpanOfTime delay, [NotNull] Action job, CancellationToken cancellationToken ) {
			if ( job is null ) {
				throw new ArgumentNullException( nameof( job ) );
			}

			await Task.Delay( delay, cancellationToken ).ConfigureAwait( false );
			await Task.Run( job, cancellationToken ).ConfigureAwait( false );
		}

		/// <summary>
		///     <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
		/// </summary>
		/// <param name="delay"></param>
		/// <param name="job">  </param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[NotNull]
		public static async Task Then( [NotNull] this IQuantityOfTime delay, [NotNull] Action job, CancellationToken cancellationToken ) {
			if ( delay is null ) {
				throw new ArgumentNullException( nameof( delay ) );
			}

			if ( job is null ) {
				throw new ArgumentNullException( nameof( job ) );
			}

			await Task.Delay( delay.ToTimeSpan(), cancellationToken ).ConfigureAwait( false );
			await Task.Run( job, cancellationToken ).ConfigureAwait( false );
		}

		[NotNull]
		public static Task Then( [NotNull] this Task first, [NotNull] Action next, CancellationToken? cancellationToken ) {
			if ( first is null ) {
				throw new ArgumentNullException( nameof( first ) );
			}

			if ( next is null ) {
				throw new ArgumentNullException( nameof( next ) );
			}

			var tcs = new TaskCompletionSource<Object?>( TaskCreationOptions.RunContinuationsAsynchronously );

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
			}, cancellationToken ?? CancellationToken.None );

			return tcs.Task;
		}

		[NotNull]
		public static Task<T2> Then<T2>( [NotNull] this Task first, [NotNull] Func<Task<T2>?> next ) {
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

			var tcs = new TaskCompletionSource<Object?>( TaskCreationOptions.RunContinuationsAsynchronously );

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
		public static Task Then<T1>( [NotNull] this Task<T1> first, [NotNull] Func<T1, Task?> next ) {
			if ( first is null ) {
				throw new ArgumentNullException( nameof( first ) );
			}

			if ( next is null ) {
				throw new ArgumentNullException( nameof( next ) );
			}

			var tcs = new TaskCompletionSource<Object?>( TaskCreationOptions.RunContinuationsAsynchronously ); //Tasks.FactorySooner.CreationOptions

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
		public static Task<T2> Then<T1, T2>( [NotNull] this Task<T1> first, [NotNull] Func<T1, Task<T2>?> next ) {
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

		/// <summary>
		///     Keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target"></param>
		/// <param name="item">  </param>
		/// <param name="cancellationToken"> </param>
		public static async Task TryPost<T>( [NotNull] this ITargetBlock<T> target, [CanBeNull] T item, CancellationToken cancellationToken ) {
			if ( target is null ) {
				throw new ArgumentNullException( nameof( target ) );
			}

			while ( true ) {
				var task = target.SendAsync( item, cancellationToken );

				await task.ConfigureAwait( false );

				if ( task.IsDone() || cancellationToken.IsCancellationRequested ) {
					break;
				}

				await Task.Delay( 0, cancellationToken ).ConfigureAwait( false );
			}
		}

		/// <summary>
		///     Return each item in <paramref name="self" /> until <paramref name="timeSpan" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self">    </param>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		[NotNull]
		public static async IAsyncEnumerable<T> Until<T>( [NotNull] this IEnumerable<T> self, TimeSpan timeSpan ) {
			var watch = Stopwatch.StartNew();

			await foreach ( var row in self.ToAsyncEnumerable().TakeWhile( _ => watch.Elapsed <= timeSpan ) ) {
				yield return row;
			}
		}

		/// <summary>
		///     Return each item in <paramref name="self" /> until <paramref name="timeSpan" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self">    </param>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		[NotNull]
		public static async IAsyncEnumerable<T> Until<T>( [NotNull] this IAsyncEnumerable<T> self, TimeSpan timeSpan ) {
			var watch = Stopwatch.StartNew();

			await foreach ( var row in self.TakeWhile( _ => watch.Elapsed <= timeSpan ) ) {
				yield return row;
			}
		}

		/// <summary>
		///     Return each item in <paramref name="self" /> until <paramref name="when" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self">    </param>
		/// <param name="when"></param>
		/// <returns></returns>
		[NotNull]
		public static IAsyncEnumerable<T> Until<T>( [NotNull] this IEnumerable<T> self, DateTime when ) => self.ToAsyncEnumerable().TakeWhile( _ => DateTime.UtcNow < when );

		/// <summary>
		///     Return each item in <paramref name="self" /> until <paramref name="when" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self">    </param>
		/// <param name="when"></param>
		/// <returns></returns>
		[NotNull]
		public static IAsyncEnumerable<T> Until<T>( [NotNull] this IAsyncEnumerable<T> self, DateTime when ) => self.TakeWhile( _ => DateTime.UtcNow < when );

		/// <summary>
		///     Returns true if the task finished before the <paramref name="timeout" />.
		/// </summary>
		/// <param name="task">   </param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		[NotNull]
		public static Task<Boolean> Until( this TimeSpan timeout, [NotNull] Task task ) => UntilTimeout( task, timeout );

		/// <summary>
		///     <para>Returns true if the task finished before the <paramref name="timeout" />.</para>
		///     <para>Use this function if the Task does not have a built-in timeout.</para>
		///     <para>Note: This function does not end the given <paramref name="task" /> if it does timeout.</para>
		/// </summary>
		/// <param name="task">   </param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public static async Task<Boolean> UntilTimeout( [NotNull] this Task task, TimeSpan timeout ) {
			if ( task is null ) {
				throw new ArgumentNullException( nameof( task ) );
			}

			var delay = Task.Delay( timeout );

			var whichTaskFinished = await Task.WhenAny( task, delay ).ConfigureAwait( false );

			var didOurTaskFinish = whichTaskFinished == task;

			return didOurTaskFinish;
		}

		public static async Task<Boolean> WaitAsync( [NotNull] this Task task, TimeSpan timeout ) {
			if ( task is null ) {
				throw new ArgumentNullException( nameof( task ) );
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
		public static FluentTimer When( this TimeSpan afterDelay, [NotNull] Func<Boolean> condition, [NotNull] Action action ) {
			if ( condition is null ) {
				throw new ArgumentNullException( nameof( condition ) );
			}

			if ( action is null ) {
				throw new ArgumentNullException( nameof( action ) );
			}

			var timer = afterDelay.CreateTimer( () => {
				if ( condition() ) {
					try {
						action();
					}
					catch ( Exception exception ) {
						exception.Log();
					}
				}
			} ).Once().Start();

			return timer;
		}

		/// <summary>
		///     Return when at least <paramref name="count" /> of the <paramref name="tasks" /> are marked as <see cref="IsDone" />
		///     .
		/// </summary>
		/// <param name="count">How many tasks to complete before returning.</param>
		/// <param name="cancellationToken"></param>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static async PooledTask WhenCount( this Int32 count, CancellationToken cancellationToken, [CanBeNull] params Task[]? tasks ) {
			if ( !count.Any() ) {
				return;
			}

			if ( tasks?.Any() == true ) {
				if ( tasks.Length < count ) {
					count = tasks.Length;
				}

				while ( tasks.Count( task => task.IsDone() ) < count ) {
					if ( cancellationToken.IsCancellationRequested ) {
						return;
					}

					await Task.WhenAny( tasks.Where( task => !task.IsDone() ) ).ConfigureAwait( false );
				}
			}
		}

		/// <summary>
		///     await the <paramref name="task" /> with a <paramref name="timeout" /> and/or a <see cref="CancellationToken" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">   </param>
		/// <param name="timeout"></param>
		/// <param name="cancellationToken">  </param>
		/// <returns></returns>
		/// <exception cref="TaskCanceledException">thrown when the task was cancelled?</exception>
		/// <exception cref="OperationCanceledException">thrown when <paramref name="timeout" /> happens?</exception>
		[ItemNotNull]
		public static async Task<Task> With<T>( [NotNull] this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken ) {
			if ( task is null ) {
				throw new ArgumentNullException( nameof( task ) );
			}

			if ( task.IsDone() ) {
				return task;
			}

			var delay = Task.Delay( timeout, cancellationToken );
			var winning = await Task.WhenAny( task, delay ).ConfigureAwait( false );

			return winning == task ? task : Task.FromException( new OperationCanceledException( "cancelled" ) );
		}

		/// <summary>
		///     await the <paramref name="task" /> with a <see cref="CancellationToken" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task"> </param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[ItemNotNull]
		public static async Task<Task> With<T>( [NotNull] this Task<T> task, CancellationToken cancellationToken ) {
			if ( task is null ) {
				throw new ArgumentNullException( nameof( task ) );
			}

			if ( task.IsDone() ) {
				return task;
			}

			var delay = Task.Delay( TimeSpan.MaxValue, cancellationToken );
			var winning = await Task.WhenAny( task, delay ).ConfigureAwait( false );

			return winning == task ? task : Task.FromException( new OperationCanceledException( "cancelled" ) );
		}

		/// <summary>
		///     await the <paramref name="task" /> with a <paramref name="timeout" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">   </param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		[ItemNotNull]
		public static async Task<Task> With<T>( [NotNull] this Task<T> task, TimeSpan timeout ) {
			if ( task is null ) {
				throw new ArgumentNullException( nameof( task ) );
			}

			if ( task.IsDone() ) {
				return task;
			}

			using var cts = new CancellationTokenSource( timeout );
			var delay = Task.Delay( timeout, cts.Token );
			var winner = await Task.WhenAny( task, delay ).ConfigureAwait( false );

			return winner == task ? task : Task.FromException( new OperationCanceledException( "timeout" ) );
		}

		/// <summary>
		///     "you can even have a timeout using the following simple extension method"
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">   </param>
		/// <param name="timeout"></param>
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
		///     "you can even have a timeout using the following simple extension method"
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">   </param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		/// <exception cref="OperationCanceledException">on timeout</exception>
		/// <exception cref="ArgumentNullException"></exception>
		[ItemNotNull]
		public static async Task<Task> WithTimeout<T>( [NotNull] this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken ) {
			if ( task == await Task.WhenAny( task, Task.Delay( timeout, cancellationToken ) ).ConfigureAwait( false ) ) {
				return task;
			}

			throw new OperationCanceledException( "timeout" );
		}

		/// <summary>
		///     Wrap 3 actions into one <see cref="Action" />.
		/// </summary>
		/// <param name="pre">   </param>
		/// <param name="action"></param>
		/// <param name="post">  </param>
		/// <returns></returns>
		[NotNull]
		public static Action Wrap( [CanBeNull] this Action? action, [CanBeNull] Action? pre, [CanBeNull] Action? post ) =>
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
		public static Task<TOut> Wrap<TIn, TOut>( [NotNull] this Func<TIn, TOut> selector, [CanBeNull] TIn input ) => Task.Run( () => selector( input ) );

		/// <summary>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <see cref="https://www.codeproject.com/Articles/5274659/How-to-Use-the-Csharp-Await-Keyword-On-Anything" />
		public readonly struct Awaiter<T> : INotifyCompletion {

			private readonly Lazy<T> _lazy;

			public Boolean IsCompleted => this._lazy.IsValueCreated;

			public Awaiter( Lazy<T> lazy ) => this._lazy = lazy;

			public T GetResult() => this._lazy.Value;

			public void OnCompleted( [NotNull] Action continuation ) {
				Task.Run( continuation );
			}
		}

		public class ResourceLoader<T> : IResourceLoader<T> {

			private Int32 _count;

			public ResourceLoader( [NotNull] Func<CancellationToken, Task<T>> loader, Int32 maxConcurrency ) {
				this._loader = loader ?? throw new ArgumentNullException( nameof( loader ) );
				this._semaphore = new SemaphoreSlim( maxConcurrency, maxConcurrency );
				this.MaxConcurrency = maxConcurrency;
			}

			[NotNull]
			private Func<CancellationToken, Task<T>> _loader { get; }

			[NotNull]
			private Object _lock { get; } = new();

			[NotNull]
			private SemaphoreSlim _semaphore { get; }

			public Int32 Available => this._semaphore.CurrentCount;

			public Int32 Count => this._count;

			public Int32 MaxConcurrency { get; }

			[NotNull]
			public Task<T> GetAsync( CancellationToken cancelToken = new() ) {
				lock ( this._lock ) {
					return this.WaitAndLoadAsync( cancelToken );
				}
			}

			public Boolean TryGet( [CanBeNull] out Task<T>? resource, CancellationToken cancelToken = default ) {
				lock ( this._lock ) {
					if ( this._semaphore.CurrentCount == 0 ) {
						resource = null;

						return false;
					}

					resource = this.WaitAndLoadAsync( cancelToken );

					return true;
				}
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