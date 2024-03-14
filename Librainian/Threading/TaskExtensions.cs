// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "TaskExtensions.cs" last formatted on 2021-11-30 at 7:22 PM by Protiguous.

#nullable enable

namespace Librainian.Threading;

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
using Utilities;

/// <summary>Remember: Tasks are born "hot" unless created with "var task=new Task();".</summary>
public static class TaskExtensions {

	/// <summary>
	/// Start a timer. When it fires, check the <paramref name="condition" />, and if true do the <paramref name="action" />.
	/// </summary>
	/// <param name="delay"></param>
	/// <param name="action"></param>
	/// <param name="condition"></param>
	public static FluentTimer After( this TimeSpan delay, Func<Boolean> condition, Action action ) {
		if ( condition is null ) {
			throw new NullException( nameof( condition ) );
		}

		if ( action is null ) {
			throw new NullException( nameof( action ) );
		}

		return FluentTimer.Create( delay, () => {
			if ( condition() ) {
				try {
					action();
				}
				catch ( Exception exception ) {
					exception.Log();
				}
			}
		} )
						  .Once()
						  .Start();
	}

	/// <summary>Quietly consume the <paramref name="task" /> on a background thread. Fire & forget.</summary> <typeparam
	/// name="T"></typeparam> <param name="task"></param> <param name="anything"></param>
	[NeedsTesting]
	[Obsolete( "Don't use it. Unless you know what&why it does." )]
	public static void Consume<T>( this Task<T> task, Object? anything = default ) {
		if ( task is null ) {
			throw new NullException( nameof( task ) );
		}

		using var bgWorker = new BackgroundWorker();
		bgWorker.DoWork += async ( _, _ ) => {
			try {
				_ = await task.ConfigureAwait( false );
			}
			catch ( AggregateException exceptions ) {
				exceptions.Log();
			}
			catch ( Exception exception ) {
				exception.Log();
			}
		};
		bgWorker.RunWorkerCompleted += ( _, _ ) => { };
		bgWorker.RunWorkerAsync( anything );
	}

	/// <summary>
	/// <para><see cref="Task.Delay(TimeSpan)" /> for <paramref name="milliseconds" />.</para>
	/// </summary>
	/// <param name="milliseconds"></param>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Task Delay( this Int32 milliseconds ) => TimeSpan.FromMilliseconds( milliseconds ).Delay();

	/// <summary>Just a wrapper for <see cref="Task.Delay(Int32)" />.</summary>
	/// <param name="delay"></param>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Task Delay( this TimeSpan delay ) => Task.Delay( delay );

	/// <summary>Just a wrapper for <see cref="Task.Delay(Int32)" />.</summary>
	/// <param name="delay"></param>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Task Delay( this IQuantityOfTime delay ) => Task.Delay( delay.ToTimeSpan() );

	/// <summary>Invokes each <see cref="Action" /> in the given <paramref name="action" /> in a try/catch.</summary>
	/// <param name="action"></param>
	/// <param name="args"></param>
	public static void Execute( this Action action, Object[]? args = default ) {
		foreach ( var method in action.GetInvocationList() ) {
			try {
				if ( method.Target is ISynchronizeInvoke {
					InvokeRequired: true
				} syncInvoke ) {
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
	/// <param name="args"></param>
	public static void Execute<T>( this Action<T> action, Object[]? args = null ) {
		foreach ( var method in action.GetInvocationList() ) {
			try {
				if ( method.Target is ISynchronizeInvoke {
					InvokeRequired: true
				} syncInvoke ) {
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

	public static void ExecuteAsync( this Action action, Object[]? args = null ) {
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

	public static void ExecuteAsync<T>( this Action<T> action, Object[]? args = null ) {
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

	public static async Task<TResult> FromEvent<TDelegate, TResult>(
		Func<TaskCompletionSource<TResult>, TDelegate> createDelegate,
		Action<TDelegate> registerDelegate,
		Action<TDelegate> unregisterDelegate,
		TimeSpan timeout
	) {
		if ( createDelegate is null ) {
			throw new NullException( nameof( createDelegate ) );
		}

		if ( registerDelegate is null ) {
			throw new NullException( nameof( registerDelegate ) );
		}

		if ( unregisterDelegate is null ) {
			throw new NullException( nameof( unregisterDelegate ) );
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

	/// <summary>Required for await support.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="lazy"></param>
	public static Awaiter<T> GetAwaiter<T>( this Lazy<T> lazy ) => new( lazy );

	/// <summary>http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	public static IEnumerable<Task<T>> InCompletionOrder<T>( this IEnumerable<Task<T>> source ) {
		if ( source is null ) {
			throw new NullException( nameof( source ) );
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

	/// <summary>Return tasks in order of completion.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static async Task<IEnumerable<Task<Task<T>>>> InOrderOfCompletion<T>( this IEnumerable<Task<T>> tasks ) {
		var enumerable = tasks as Task<T>[] ?? tasks.ToArray();

		var inputTasks = enumerable.ToAsyncEnumerable();

		var buckets = new TaskCompletionSource<Task<T>>[ enumerable.Length ];
		var results = new Task<Task<T>>[ buckets.Length ];

		for ( var i = 0; i < buckets.Length; i++ ) {
			buckets[ i ] = new TaskCompletionSource<Task<T>>();
			results[ i ] = buckets[ i ].Task;
		}

		var nextTaskIndex = -1;

		void Continuation( Task<T> completed ) {
			var bucket = buckets[ Interlocked.Increment( ref nextTaskIndex ) ];
			bucket.TrySetResult( completed );
		}

		await foreach ( var inputTask in inputTasks.ConfigureAwait( false ) ) {
			await inputTask.ContinueWith( Continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default ).ConfigureAwait( false );
		}

		return results;
	}

	/// <summary>Return tasks in order of completion.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static async Task<IEnumerable<Task<Task<T>>>> InOrderOfCompletion<T>( this IDictionary<TimeSpan, Task<T>> tasks ) {
		var buckets = new TaskCompletionSource<Task<T>>[ tasks.Count ];
		var results = new Task<Task<T>>[ buckets.Length ];

		for ( var i = 0; i < buckets.Length; i++ ) {
			buckets[ i ] = new TaskCompletionSource<Task<T>>();
			results[ i ] = buckets[ i ].Task;
		}

		var nextTaskIndex = -1;

		void Continuation( Task<T> completed ) {
			var bucket = buckets[ Interlocked.Increment( ref nextTaskIndex ) ];
			bucket.TrySetResult( completed );
		}

		await foreach ( var inputTask in tasks.ToAsyncEnumerable().ConfigureAwait( false ) ) {
			await inputTask.Value.ContinueWith( Continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default )
						   .ConfigureAwait( false );
		}

		return results;
	}

	/// <summary>Return tasks in order of completion.</summary>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static async Task<IEnumerable<Task<Task>>> InOrderOfCompletionAsync( this IEnumerable<Task> tasks ) {
		var inputTasks = tasks.ToList();

		var buckets = new TaskCompletionSource<Task>[ inputTasks.Count ];
		var results = new Task<Task>[ buckets.Length ];

		for ( var i = 0; i < buckets.Length; i++ ) {
			buckets[ i ] = new TaskCompletionSource<Task>();
			results[ i ] = buckets[ i ].Task;
		}

		var nextTaskIndex = -1;

		void Continuation( Task completed ) {
			var bucket = buckets[ Interlocked.Increment( ref nextTaskIndex ) ];
			bucket.TrySetResult( completed );
		}

		foreach ( var inputTask in inputTasks ) {
			await inputTask.ContinueWith( Continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default ).ConfigureAwait( false );
		}

		return results;
	}

	/// <summary></summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="tasks"></param>
	/// <example>
	/// var tasks = new[] { Task.Delay(3000).ContinueWith(_ =&gt; 3), Task.Delay(1000).ContinueWith(_ =&gt; 1),
	/// Task.Delay(2000).ContinueWith(_ =&gt; 2), Task.Delay(5000).ContinueWith(_ =&gt; 5), Task.Delay(4000).ContinueWith(_
	/// =&gt; 4), }; foreach (var bucket in Interleaved(tasks)) { var t = await bucket; int result = await t;
	/// Console.WriteLine("{0}: {1}", DateTime.Now, result); }
	/// </example>
	public static Task<Task<T>>[] Interleaved<T>( IEnumerable<Task<T>> tasks ) {
		if ( tasks is null ) {
			throw new NullException( nameof( tasks ) );
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
				throw new NullException( nameof( completed ) );
			}

			var bucket = buckets[ Interlocked.Increment( ref nextTaskIndex ) ];
			bucket.TrySetResult( completed );
		}

		foreach ( var inputTask in inputTasks ) {
			inputTask.ContinueWith( Continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default );
		}

		return results;
	}

	/// <summary>Returns true if the <paramref name="task" /> is Completed, Cancelled, or Faulted.</summary>
	/// <param name="task"></param>
	[DebuggerStepThrough]
	public static Boolean IsDone( this Task task ) => task.IsCompleted || task.IsCanceled || task.IsFaulted;

	public static CancellationToken LinkTo( this CancellationToken firstToken, CancellationToken secondToken ) {
		if ( firstToken == secondToken ) {
			return firstToken; //could throw here, but why?
		}

		return CancellationTokenSource.CreateLinkedTokenSource( firstToken, secondToken ).Token;
	}

	/// <summary>
	/// <para>Shortcut for Task.ConfigureAwait(false).</para>
	/// </summary>
	/// <param name="task"></param>
	[DebuggerStepThrough]
	public static ConfiguredTaskAwaitable NoUI( this Task task ) => task.ConfigureAwait( false );

	/// <summary>
	/// <para>Shortcut for Task.ConfigureAwait(false).</para>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="task"></param>
	[DebuggerStepThrough]
	public static ConfiguredTaskAwaitable<T> NoUI<T>( this Task<T> task ) => task.ConfigureAwait( false );

	/// <summary>http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="completedTask"></param>
	/// <param name="completionSource"></param>
	/// <exception cref="NullException"></exception>
	public static void PropagateResult<T>( this Task<T> completedTask, TaskCompletionSource<T> completionSource ) {
		if ( completedTask is null ) {
			throw new NullException( nameof( completedTask ) );
		}

		if ( completionSource is null ) {
			throw new NullException( nameof( completionSource ) );
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
				throw new NullException( "Task was not completed." );
		}
	}

	public static void SetFromTask<T>( this TaskCompletionSource<T> tcs, Task<T> task ) {
		if ( tcs is null ) {
			throw new NullException( nameof( tcs ) );
		}

		if ( task is null ) {
			throw new NullException( nameof( task ) );
		}

		if ( !task.IsCompleted ) {
			throw new NullException( "Task must be complete" );
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
	/// <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
	/// </summary>
	/// <param name="delay"></param>
	/// <param name="job"></param>
	/// <param name="cancellationToken"></param>
	public static async Task Then( this TimeSpan delay, Action job, CancellationToken cancellationToken ) {
		if ( job is null ) {
			throw new NullException( nameof( job ) );
		}

		await Task.Delay( delay, cancellationToken ).ConfigureAwait( false );
		await new Task( job, cancellationToken, TaskCreationOptions.PreferFairness | TaskCreationOptions.RunContinuationsAsynchronously ).ConfigureAwait( false );
	}

	/// <summary>
	/// <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
	/// </summary>
	/// <param name="delay"></param>
	/// <param name="job"></param>
	public static async Task Then( this SpanOfTime delay, Action job ) {
		if ( job is null ) {
			throw new NullException( nameof( job ) );
		}

		await Task.Delay( delay ).ConfigureAwait( false );
		await Task.Run( job ).ConfigureAwait( false );
	}

	/// <summary>
	/// <para>Continue the <paramref name="second" /> task after running the <paramref name="first" /> task.</para>
	/// </summary>
	/// <param name="first"></param>
	/// <param name="second"></param>
	public static async Task Then( this Task first, Task second ) {
		if ( first is null ) {
			throw new NullException( nameof( first ) );
		}

		if ( second is null ) {
			throw new NullException( nameof( second ) );
		}

		await first.ConfigureAwait( false );
		await second.ConfigureAwait( false );
	}

	/// <summary>
	/// <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
	/// </summary>
	/// <param name="delay"></param>
	/// <param name="job"></param>
	/// <param name="cancellationToken"></param>
	public static async Task Then( this SpanOfTime delay, Action job, CancellationToken cancellationToken ) {
		if ( job is null ) {
			throw new NullException( nameof( job ) );
		}

		await Task.Delay( delay, cancellationToken ).ConfigureAwait( false );
		await Task.Run( job, cancellationToken ).ConfigureAwait( false );
	}

	/// <summary>
	/// <para>Continue the task with the <paramref name="job" /> after a <paramref name="delay" />.</para>
	/// </summary>
	/// <param name="delay"></param>
	/// <param name="job"></param>
	/// <param name="cancellationToken"></param>
	public static async Task Then( this IQuantityOfTime delay, Action job, CancellationToken cancellationToken ) {
		if ( delay is null ) {
			throw new NullException( nameof( delay ) );
		}

		if ( job is null ) {
			throw new NullException( nameof( job ) );
		}

		await Task.Delay( delay.ToTimeSpan(), cancellationToken ).ConfigureAwait( false );
		await Task.Run( job, cancellationToken ).ConfigureAwait( false );
	}

	/*
	public static Task Then( this Task first, Action next, CancellationToken? cancellationToken ) {
		if ( first is null ) {
			throw new NullException( nameof( first ) );
		}

		if ( next is null ) {
			throw new NullException( nameof( next ) );
		}

		var tcs = new TaskCompletionSource<Object?>( TaskCreationOptions.RunContinuationsAsynchronously );

		first.ContinueWith( _ => {
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
	*/

	/*
	public static Task<T2> Then<T2>( this Task first, Func<Task<T2>?> next ) {
		if ( first is null ) {
			throw new NullException( nameof( first ) );
		}

		if ( next is null ) {
			throw new NullException( nameof( next ) );
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
	*/

	/*
	public static Task Then<T1>( this Task<T1> first, Action<T1> next ) {
		if ( first is null ) {
			throw new NullException( nameof( first ) );
		}

		if ( next is null ) {
			throw new NullException( nameof( next ) );
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
	*/

	/*
	public static Task Then<T1>( this Task<T1> first, Func<T1, Task?> next ) {
		if ( first is null ) {
			throw new NullException( nameof( first ) );
		}

		if ( next is null ) {
			throw new NullException( nameof( next ) );
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
	*/

	/*
	public static Task<T2> Then<T1, T2>( this Task<T1> first, Func<T1, T2> next ) {
		if ( first is null ) {
			throw new NullException( nameof( first ) );
		}

		if ( next is null ) {
			throw new NullException( nameof( next ) );
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
	*/

	/*
	public static Task<T2> Then<T1, T2>( this Task<T1> first, Func<T1, Task<T2>?> next ) {
		if ( first is null ) {
			throw new NullException( nameof( first ) );
		}

		if ( next is null ) {
			throw new NullException( nameof( next ) );
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
	*/

	/// <summary>Throws <see cref="OperationCanceledException"></see> if any tokens that have been cancelled.</summary>
	/// <param name="tokens"></param>
	/// <exception cref="OperationCanceledException"></exception>
	[NeedsTesting]
	public static void ThrowAnyCancelledTokens( params CancellationToken[] tokens ) {
		if ( tokens is null ) {
			throw new NullException( nameof( tokens ) );
		}

		foreach ( var cancellationToken in tokens ) {
			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	/// <summary>Keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="target"></param>
	/// <param name="item"></param>
	/// <param name="cancellationToken"></param>
	public static async Task TryPost<T>( this ITargetBlock<T?> target, T? item, CancellationToken cancellationToken ) {
		if ( target is null ) {
			throw new NullException( nameof( target ) );
		}

		while ( true ) {
			if ( cancellationToken.IsCancellationRequested ) {
				break;
			}

			await target.SendAsync( item, cancellationToken ).ConfigureAwait( false );
		}
	}

	/// <summary>Return items in <paramref name="self" /> until <paramref name="timeSpan" /> elapses.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	/// <param name="timeSpan"></param>
	public static async IAsyncEnumerable<T> Until<T>( this IEnumerable<T> self, TimeSpan timeSpan ) {
		var watch = Stopwatch.StartNew();

		await foreach ( var row in self.ToAsyncEnumerable().TakeWhile( _ => watch.Elapsed <= timeSpan ).ConfigureAwait( false ) ) {
			yield return row;
		}
	}

	/// <summary>Return each item in <paramref name="self" /> until <paramref name="timeSpan" /> elapses.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	/// <param name="timeSpan"></param>
	public static async IAsyncEnumerable<T> Until<T>( this IAsyncEnumerable<T> self, TimeSpan timeSpan ) {
		var watch = Stopwatch.StartNew();

		await foreach ( var row in self.TakeWhile( _ => watch.Elapsed <= timeSpan ).ConfigureAwait( false ) ) {
			yield return row;
		}
	}

	/// <summary>Return each item in <paramref name="self" /> until <paramref name="when" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	/// <param name="when"></param>
	public static IAsyncEnumerable<T> Until<T>( this IEnumerable<T> self, DateTime when ) => self.ToAsyncEnumerable().TakeWhile( _ => DateTime.UtcNow < when );

	/// <summary>Return each item in <paramref name="self" /> until <paramref name="when" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	/// <param name="when"></param>
	public static IAsyncEnumerable<T> Until<T>( this IAsyncEnumerable<T> self, DateTime when ) => self.TakeWhile( _ => DateTime.UtcNow < when );

	/// <summary>Returns true if the task finished before the <paramref name="timeout" />.</summary>
	/// <param name="task"></param>
	/// <param name="timeout"></param>
	public static Task<Boolean> Until( this TimeSpan timeout, Task task ) => UntilTimeout( task, timeout );

	/// <summary>
	/// <para>Returns true if the task finished before the <paramref name="timeout" />.</para>
	/// <para>Use this function if the Task does not have a built-in timeout.</para>
	/// <para>Note: This function does not end the given <paramref name="task" /> if it does timeout.</para>
	/// </summary>
	/// <param name="task"></param>
	/// <param name="timeout"></param>
	public static async Task<Boolean> UntilTimeout( this Task task, TimeSpan timeout ) {
		if ( task is null ) {
			throw new NullException( nameof( task ) );
		}

		var delay = Task.Delay( timeout );

		var whichTaskFinished = await Task.WhenAny( task, delay ).ConfigureAwait( false );

		var didOurTaskFinish = whichTaskFinished == task;

		return didOurTaskFinish;
	}

	public static async Task<Boolean> WaitAsync( this Task task, TimeSpan timeout ) {
		if ( task is null ) {
			throw new NullException( nameof( task ) );
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
	/// Return when at least <paramref name="count" /> of the <paramref name="tasks" /> are marked as <see cref="IsDone" /> .
	/// </summary>
	/// <param name="count">How many tasks to complete before returning.</param>
	/// <param name="cancellationToken"></param>
	/// <param name="tasks"></param>
	public static async PooledTask WhenCount( this Int32 count, CancellationToken cancellationToken, params Task[]? tasks ) {
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
	/// await the <paramref name="task" /> with a <paramref name="timeout" /> and/or a <see cref="CancellationToken" />.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="task"></param>
	/// <param name="timeout"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="TaskCanceledException">thrown when the task was cancelled?</exception>
	/// <exception cref="OperationCanceledException">thrown when <paramref name="timeout" /> happens?</exception>
	public static async Task<Task> With<T>( this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken ) {
		if ( task is null ) {
			throw new NullException( nameof( task ) );
		}

		if ( task.IsDone() ) {
			return task;
		}

		var delay = Task.Delay( timeout, cancellationToken );
		var winning = await Task.WhenAny( task, delay ).ConfigureAwait( false );

		return winning == task ? task : Task.FromException( new OperationCanceledException( "cancelled" ) );
	}

	/// <summary>await the <paramref name="task" /> with a <see cref="CancellationToken" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="task"></param>
	/// <param name="cancellationToken"></param>
	public static async Task<Task> With<T>( this Task<T> task, CancellationToken cancellationToken ) {
		if ( task is null ) {
			throw new NullException( nameof( task ) );
		}

		if ( task.IsDone() ) {
			return task;
		}

		var delay = Task.Delay( TimeSpan.MaxValue, cancellationToken );
		var winning = await Task.WhenAny( task, delay ).ConfigureAwait( false );

		return winning == task ? task : Task.FromException( new OperationCanceledException( "cancelled" ) );
	}

	/// <summary>await the <paramref name="task" /> with a <paramref name="timeout" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="task"></param>
	/// <param name="timeout"></param>
	public static async Task<Task> With<T>( this Task<T> task, TimeSpan timeout ) {
		if ( task is null ) {
			throw new NullException( nameof( task ) );
		}

		if ( task.IsDone() ) {
			return task;
		}

		using var cts = new CancellationTokenSource( timeout );
		var delay = Task.Delay( timeout, cts.Token );
		var winner = await Task.WhenAny( task, delay ).ConfigureAwait( false );

		return winner == task ? task : Task.FromException( new OperationCanceledException( "timeout" ) );
	}

	/// <summary>"you can even have a timeout using the following simple extension method"</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="task"></param>
	/// <param name="timeout"></param>
	/// <exception cref="TaskCanceledException">on timeout</exception>
	/// <exception cref="NullException"></exception>
	public static async Task<Task> WithTimeout<T>( this Task<T> task, TimeSpan timeout ) {
		if ( task == await Task.WhenAny( task, Task.Delay( timeout ) ).ConfigureAwait( false ) ) {
			return task;
		}

		throw new TaskCanceledException( "timeout" );
	}

	/// <summary>"you can even have a timeout using the following simple extension method"</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="task"></param>
	/// <param name="timeout"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="TaskCanceledException">on timeout</exception>
	/// <exception cref="NullException"></exception>
	public static async Task<Task> WithTimeout<T>( this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken ) {
		if ( task == await Task.WhenAny( task, Task.Delay( timeout, cancellationToken ) ).ConfigureAwait( false ) ) {
			return task;
		}

		throw new TaskCanceledException( "timeout" );
	}

	/// <summary>Wrap 3 actions into one <see cref="Action" />.</summary>
	/// <param name="pre"></param>
	/// <param name="action"></param>
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

	/// <summary>var result = await Wrap( () =&gt; OldNonAsyncFunction( ) );</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="selector"></param>
	public static Task<T> Wrap<T>( this Func<T> selector ) => Task.Run( selector );

	/// <summary>var result = await Wrap( () =&gt; OldNonAsyncFunction( "hello world" ) );</summary>
	/// <typeparam name="TIn"></typeparam>
	/// <typeparam name="TOut"></typeparam>
	/// <param name="selector"></param>
	/// <param name="input"></param>
	public static Task<TOut> Wrap<TIn, TOut>( this Func<TIn?, TOut> selector, TIn? input ) => Task.Run( () => selector( input ) );

	/// <summary></summary>
	/// <typeparam name="T"></typeparam>
	/// <see cref="http://www.codeproject.com/Articles/5274659/How-to-Use-the-Csharp-Await-Keyword-On-Anything" />
	public readonly struct Awaiter<T> : INotifyCompletion {

		private readonly Lazy<T> _lazy;

		public Awaiter( Lazy<T> lazy ) => this._lazy = lazy;

		public Boolean IsCompleted => this._lazy.IsValueCreated;

		public T GetResult() => this._lazy.Value;

		public void OnCompleted( Action continuation ) => Task.Run( continuation );
	}

	public class ResourceLoader<T> : IResourceLoader<T> {

		private Int32 _count;

		public ResourceLoader( Func<CancellationToken, Task<T>> loader, Int32 maxConcurrency ) {
			this.Loader = loader ?? throw new NullException( nameof( loader ) );
			this.Semaphore = new SemaphoreSlim( maxConcurrency, maxConcurrency );
			this.MaxConcurrency = maxConcurrency;
		}

		private Func<CancellationToken, Task<T>> Loader { get; }

		private Object Lock { get; } = new();

		private SemaphoreSlim Semaphore { get; }

		public Int32 Available => this.Semaphore.CurrentCount;

		public Int32 Count => this._count;

		public Int32 MaxConcurrency { get; }

		[ItemNotNull]
		private async Task<T> WaitAndLoadAsync( CancellationToken cancelToken ) {
			Interlocked.Increment( ref this._count );

			using ( await this.Semaphore.UseWaitAsync( cancelToken ).ConfigureAwait( false ) ) {
				return await this.Loader( cancelToken ).ConfigureAwait( false );
			}
		}

		public Task<T> GetAsync( CancellationToken cancelToken = new() ) {
			lock ( this.Lock ) {
				return this.WaitAndLoadAsync( cancelToken );
			}
		}

		public Boolean TryGet( out Task<T>? resource, CancellationToken cancelToken = default ) {
			lock ( this.Lock ) {
				if ( this.Semaphore.CurrentCount == 0 ) {
					resource = null;

					return false;
				}

				resource = this.WaitAndLoadAsync( cancelToken );

				return true;
			}
		}
	}
}