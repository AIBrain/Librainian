// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "ThreadingExtensions.cs" last formatted on 2020-10-12 at 4:26 PM.

#nullable enable

namespace Librainian.Threading {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Logging;
	using Maths;

	public static class ThreadingExtensions {
		
		public static Boolean IsRunningFromNUnit { get; } =
			AppDomain.CurrentDomain.GetAssemblies().Any( assembly => assembly.FullName?.ToLowerInvariant().StartsWith( "nunit.framework" ) == true );

		/// <summary>Only allow a delegate to run X times.</summary>
		/// <param name="action">      </param>
		/// <param name="callsAllowed"></param>
		/// <returns></returns>
		/// <example>var barWithBarrier = ThreadingExtensions.ActionBarrier(Bar, remainingCallsAllowed: 2 );</example>
		/// <remarks>Calling the delegate more often than <paramref name="callsAllowed" /> should just NOP.</remarks>
		[NotNull]
		public static Action ActionBarrier( [NotNull] this Action action, Int64? callsAllowed = null ) {
			var context = new ContextCallOnlyXTimes( callsAllowed ?? 1 );

			return () => {
				if ( Interlocked.Decrement( ref context.CallsAllowed ) >= 0 ) {
					action();
				}
			};
		}

		/// <summary>Only allow a delegate to run X times.</summary>
		/// <param name="action">      </param>
		/// <param name="parameter">   </param>
		/// <param name="callsAllowed"></param>
		/// <returns></returns>
		/// <example>var barWithBarrier = ThreadingExtensions.ActionBarrier(Bar, remainingCallsAllowed: 2 );</example>
		/// <remarks>Calling the delegate more often than <paramref name="callsAllowed" /> should just NOP.</remarks>
		[NotNull]
		public static Action ActionBarrier<T>( [NotNull] this Action<T> action, [CanBeNull] T parameter, Int64? callsAllowed = null ) {
			var context = new ContextCallOnlyXTimes( callsAllowed ?? 1 );

			return () => {
				if ( Interlocked.Decrement( ref context.CallsAllowed ) >= 0 ) {
					action( parameter );
				}
			};
		}

		/// <summary>
		///     <para>Thread.BeginThreadAffinity();</para>
		///     <para>Thread.BeginCriticalRegion();</para>
		///     <para>...</para>
		///     <see cref="End" />
		/// </summary>
		public static void Begin( Boolean lowPriority = true ) {
			Thread.BeginThreadAffinity();
			Thread.BeginCriticalRegion();

			if ( lowPriority && Thread.CurrentThread.Priority != ThreadPriority.Lowest ) {
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			}
		}

		/// <summary>About X bytes by polling the object's fields.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static UInt64 CalcSizeInBytes<T>( [CanBeNull] this T obj ) {
			if ( obj is null ) {
				return 0;
			}

			var sizeInBytes = obj.GetSizeOfPrimitive();

			if ( sizeInBytes.Any() ) {
				return sizeInBytes;
			}

			var fields = obj.GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );

			foreach ( var value in from field in fields select field.GetValue( obj ) ) {
				switch ( value ) {
					case Array array:
						foreach ( var o in array ) {
							sizeInBytes += CalcSizeInBytes( o );
						}
						continue;
					case IList list:
						foreach ( var o in list ) {
							sizeInBytes += CalcSizeInBytes( o );
						}

						continue;
					case IDictionary dictionary:
						foreach ( DictionaryEntry o in dictionary ) {
							sizeInBytes += CalcSizeInBytes( o.Key );
							sizeInBytes += CalcSizeInBytes( o.Value );
						}

						continue;
					case IEnumerable enumerable:
						foreach ( var o in enumerable ) {
							sizeInBytes += CalcSizeInBytes( o );
						}

						continue;
					default:
						sizeInBytes += value.CalcSizeInBytes();
						break;
				}
			}

			return sizeInBytes;
		}

		/// <summary>
		///     <para>
		///         <see cref="Begin" />
		///     </para>
		///     <para>...</para>
		///     <para>Thread.EndThreadAffinity();</para>
		///     <para>Thread.EndCriticalRegion();</para>
		/// </summary>
		public static void End() {
			Thread.EndThreadAffinity();
			Thread.EndCriticalRegion();
		}

		public static Int32 GetMaximumActiveWorkerThreads() {
			ThreadPool.GetMaxThreads( out var _, out var maxPortThreads );

			return maxPortThreads;
		}

		public static UInt64 GetSizeOfPrimitive<T>( [CanBeNull] this T obj ) =>
			( UInt64 )( obj switch {
				Boolean => sizeof( Boolean ),
				Byte => sizeof( Byte ),
				SByte => sizeof( SByte ),
				Char => sizeof( Char ),
				Int16 => sizeof( Int16 ),
				UInt16 => sizeof( UInt16 ),
				Int32 => sizeof( Int32 ),
				UInt32 => sizeof( UInt32 ),
				Int64 => sizeof( Int64 ),
				UInt64 => sizeof( UInt64 ),
				Single => sizeof( Single ),
				Double => sizeof( Double ),
				Decimal => sizeof( Decimal ),
				String s => sizeof( Char ) * s.Length,
				{ } => sizeof( Int32 ),	//BUG 4 ?? 8. sizeof(Pointer)
				var _ => 0
			} );

		/// <summary>returns Marshal.SizeOf( typeof( T ) );</summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static Int32 MarshalSizeOf<T>() where T : struct => Marshal.SizeOf( typeof( T ) );

		/// <summary>boxed returns Marshal.SizeOf( obj )</summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static Int32 MarshalSizeOf( [NotNull] this Object obj ) => Marshal.SizeOf( obj );

		/// <summary>generic returns Marshal.SizeOf( obj )</summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static Int32 MarshalSizeOf<T>( [NotNull] this T obj ) => Marshal.SizeOf( obj );

		/// <summary>Repeat the <paramref name="action" /><paramref name="times" />.
		/// <para>Swallows <see cref="Exception"/>.</para>
		/// </summary>
		/// <param name="times"> </param>
		/// <param name="action"></param>
		public static void Repeat( this Int32 times, [NotNull] Action action ) {
			for ( var i = 0; i < Math.Abs( times ); i++ ) {
				try {
					action();
				}
				catch ( Exception ) { }
			}
		}

		/// <summary>
		/// <para>Swallows <see cref="Exception"/>.</para>
		/// </summary>
		/// <param name="action"></param>
		/// <param name="times"></param>
		public static void Repeat( [NotNull] this Action action, Int32 times ) {
			for ( var i = 0; i < Math.Abs( times ); i++ ) {
				try {
					action();
				}
				catch ( Exception ) { }
			}
		}

		/// <summary>
		/// <para>Swallows <see cref="Exception"/>.</para>
		/// </summary>
		/// <param name="action"></param>
		/// <param name="counter"></param>
		public static void RepeatAction( [NotNull] this Action action, Int32 counter ) => Parallel.For( 1, counter, i => {
			try {
				action();
			}
			catch ( Exception ) { }
		} );

		/// <summary>
		///     Run each <see cref="Action" />, optionally in parallel (defaults to true), optionally printing feedback
		///     through an action.
		/// </summary>
		/// <param name="actions">      </param>
		/// <param name="output">     </param>
		/// <param name="description"></param>
		/// <param name="inParallel"> </param>
		/// <returns></returns>
		public static Boolean Run( [NotNull] this IEnumerable<Action> actions, [CanBeNull] Action<String>? output = null, [CanBeNull] String? description = null,
			Boolean inParallel = true ) {
			if ( actions is null ) {
				throw new ArgumentNullException( nameof( actions ) );
			}

			if ( output != null && !String.IsNullOrWhiteSpace( description ) ) {
				output( description );
			}

			if ( inParallel ) {
				var result = Parallel.ForEach( actions.AsParallel(), action => action() );

				return result.IsCompleted;
			}

			foreach ( var action in actions ) {
				action();
			}

			return true;
		}

		/// <summary>Run each <see cref="Func{Boolean}" /> in parallel, optionally printing feedback through an action.</summary>
		/// <param name="functions">      </param>
		/// <param name="output">     </param>
		/// <param name="description"></param>
		/// <param name="inParallel"> </param>
		/// <returns></returns>
		public static Boolean Run( [NotNull] this IEnumerable<Func<Boolean>> functions, [CanBeNull] Action<String>? output = null, [CanBeNull] String? description = null,
			Boolean inParallel = true ) {
			if ( functions is null ) {
				throw new ArgumentNullException( nameof( functions ) );
			}

			if ( output != null && !String.IsNullOrWhiteSpace( description ) ) {
				output( description );
			}

			if ( inParallel ) {
				var result = Parallel.ForEach( functions.AsParallel(), function => {
					try {
						function();
					}
					catch ( Exception exception ) {
						exception.Log();
					}
				} );

				return result.IsCompleted;
			}

			foreach ( var function in functions ) {
				try {
					function();
				}
				catch ( Exception exception ) {
					exception.Log();
				}
			}

			return true;
		}

		public sealed class ContextCallOnlyXTimes {
			public Int64 CallsAllowed;

			public ContextCallOnlyXTimes( Int64 times ) {
				if ( times <= 0 ) {
					times = 0;
				}

				this.CallsAllowed = times;
			}
		}

		/*

				/// <summary>
				/// Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.
				/// </summary>
				/// <typeparam name="TResult">Specifies the type of data contained in the task.</typeparam>
				/// <param name="task">   The task.</param>
				/// <param name="timeout">The timeout.</param>
				/// <returns>The new Task that may time out.</returns>
				/// <see cref="http://stackoverflow.com/a/20639723/956364"/>
				public static Task<TResult> WithTimeout<TResult>( this Task<TResult> task, TimeSpan timeout ) {
					var result = new TaskCompletionSource<TResult>( task.AsyncState );
					var timer = new Timer( state =>
									( ( TaskCompletionSource<TResult> )state ).TrySetCanceled(),
									result, timeout, TimeSpan.FromMilliseconds( -1 ) );
					task.ContinueWith( t => {
						timer.Dispose();
						result.TrySetFromTask( t );
					}, TaskContinuationOptions.ExecuteSynchronously );
					return result.Task;
				}
		*/
		/*

				/// <summary>
				/// a fire-and-forget wrapper for an <see cref="Action"/>.
				/// </summary>
				/// <param name="action"></param>
				/// <param name="next">  </param>
				/// <returns></returns>
				public static void Then( this Action action, Action next ) {
					if ( action is null ) {
						throw new ArgumentNullException( "action" );
					}
					action.Spawn(); //does this even make sense?
					next.Spawn();
				}
		*/
	}
}