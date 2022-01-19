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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Tasks.cs" last formatted on 2022-12-22 at 5:21 PM by Protiguous.

namespace Librainian.Threading;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Logging;
using Measurement.Time;

/// <summary>
///     Execute an <see cref="Action" /> on a <see cref="Timer" />.
/// </summary>
public static class Tasks {

	/// <summary>
	///     http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="completedTask"></param>
	/// <param name="completionSource"></param>
	private static void PropagateResult<T>( Task<T> completedTask, TaskCompletionSource<T>? completionSource ) {
		switch ( completedTask.Status ) {
			case TaskStatus.Canceled:
				completionSource?.TrySetCanceled();

				break;

			case TaskStatus.Faulted:
				if ( completedTask.Exception != null ) {
					completionSource?.TrySetException( completedTask.Exception.InnerExceptions );
				}

				break;

			case TaskStatus.RanToCompletion:
				completionSource?.TrySetResult( completedTask.Result );

				break;

			default:
				throw new ArgumentException( "Task was not completed." );
		}
	}

	/// <summary>
	///     http://stackoverflow.com/questions/35247862/is-there-a-reason-to-prefer-one-of-these-implementations-over-the-other
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	public static IEnumerable<Task<T>> InCompletionOrder<T>( this IEnumerable<Task<T>> source ) {
		var inputs = source.ToList();
		var boxes = inputs.ConvertAll( _ => new TaskCompletionSource<T>() );
		var currentIndex = -1;

		foreach ( var task in inputs ) {
			task.ContinueWith( completed => {
				var nextBox = boxes[ Interlocked.Increment( ref currentIndex ) ];
				PropagateResult( completed, nextBox );
			}, TaskContinuationOptions.ExecuteSynchronously );
		}

		return boxes.Select( box => box.Task );
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="tasks"></param>
	/// <example>
	///     var tasks = new[] { Task.Delay(3000).ContinueWith(_ =&gt; 3), Task.Delay(1000).ContinueWith(_ =&gt; 1),
	///     Task.Delay(2000).ContinueWith(_ =&gt; 2), Task.Delay(5000).ContinueWith(_ =&gt; 5), Task.Delay(4000).ContinueWith(_
	///     =&gt; 4), }; foreach (var bucket in Interleaved(tasks)) { var t = await bucket; int result = await t;
	///     Console.WriteLine("{0}: {1}", DateTime.Now, result); }
	/// </example>
	public static Task<Task<T>>[] Interleaved<T>( IEnumerable<Task<T>> tasks ) {
		if ( tasks is null ) {
			throw new ArgumentEmptyException( nameof( tasks ) );
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
			var bucket = buckets[ Interlocked.Increment( ref nextTaskIndex ) ];
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
	//public static void Spawn( [NeedsTesting] this Action job, Span? delay = null, Single priority ) {
	//    if ( job is null ) {
	//        throw new ArgumentEmptyException( "job" );
	//    }
	//    AddThenFireAndForget( job: job, delay: delay );
	//}

	public static Task Multitude( params Action[] actions ) => Task.Run( () => Parallel.Invoke( actions ) );

	/// <summary>
	///     Do the <paramref name="job" /> with a dataflow after a <see cref="Timer" /> .
	/// </summary>
	/// <param name="delay"></param>
	/// <param name="job"></param>
	public static async Task Then( this TimeSpan delay, Action job ) {
		if ( job is null ) {
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
		if ( job is null ) {
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
		if ( job is null ) {
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
		if ( job is null ) {
			throw new ArgumentEmptyException( nameof( job ) );
		}

		await Task.Delay( delay ).ConfigureAwait( false );

		await Task.Run( job ).ConfigureAwait( false );
	}

	/// <summary>
	///     Wrap 3 methods into one.
	/// </summary>
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

}