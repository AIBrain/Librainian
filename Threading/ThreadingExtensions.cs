// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ThreadingExtensions.cs" belongs to Rick@AIBrain.org and
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
//
// ***  Project "Librainian"  ***
// File "ThreadingExtensions.cs" was last formatted by Protiguous on 2018/06/03 at 5:01 AM.

namespace Librainian.Threading {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using JetBrains.Annotations;
	using Measurement.Time;
	using static LanguageExt.Prelude;

	public static class ThreadingExtensions {

		/// <summary>
		///     <para>
		///         Sets the <see cref="System.Threading.Tasks.ParallelOptions.MaxDegreeOfParallelism" /> of a
		///         <see cref="System.Threading.Tasks.ParallelOptions" /> to <see cref="Environment.ProcessorCount" />.
		///     </para>
		///     <para>1 core to 1</para>
		///     <para>2 cores to 2</para>
		///     <para>4 cores to 4</para>
		///     <para>8 cores to 8</para>
		///     <para>n cores to n</para>
		/// </summary>
		[NotNull]
		public static ParallelOptions AllCPU { get; } = new ParallelOptions { MaxDegreeOfParallelism = Math.Max( 1, Environment.ProcessorCount ) };

		/// <summary>
		///     <para>
		///         Sets the <see cref="System.Threading.Tasks.ParallelOptions.MaxDegreeOfParallelism" /> of a
		///         <see cref="System.Threading.Tasks.ParallelOptions" /> to <see cref="Environment.ProcessorCount" />-1.
		///     </para>
		///     <para>1 core to 1</para>
		///     <para>2 cores to 1</para>
		///     <para>4 cores to 3</para>
		///     <para>8 cores to 7</para>
		///     <para>n cores to n-1</para>
		/// </summary>
		[NotNull]
		public static ParallelOptions CPUIntensive { get; } = new ParallelOptions {
			MaxDegreeOfParallelism = Math.Max( 1, Environment.ProcessorCount - 1 ) //leave the OS a little wiggle room
		};

		/// <summary>
		///     <para>
		///         Sets the <see cref="System.Threading.Tasks.ParallelOptions.MaxDegreeOfParallelism" /> of a
		///         <see cref="System.Threading.Tasks.ParallelOptions" /> to half of <see cref="Environment.ProcessorCount" />.
		///     </para>
		///     <para>1 core to 1?</para>
		///     <para>2 cores to 1</para>
		///     <para>4 cores to 2</para>
		///     <para>8 cores to 4</para>
		///     <para>n cores to n/2</para>
		/// </summary>
		[NotNull]
		public static ParallelOptions CPULight { get; } = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount / 2 };

		/// <summary>
		///     <para>
		///         Sets the <see cref="System.Threading.Tasks.ParallelOptions.MaxDegreeOfParallelism" /> of a
		///         <see cref="System.Threading.Tasks.ParallelOptions" /> to <see cref="Environment.ProcessorCount" /> * 2.
		///     </para>
		///     <para>1 core to 2</para>
		///     <para>2 cores to 4</para>
		///     <para>4 cores to 8</para>
		///     <para>8 cores to 16</para>
		///     <para>n cores to 2n</para>
		/// </summary>
		[NotNull]
		public static ParallelOptions DiskIntensive { get; } = new ParallelOptions { MaxDegreeOfParallelism = Math.Max( 1, Environment.ProcessorCount * 2 ) };

		public static Boolean IsRunningFromNUnit { get; } = AppDomain.CurrentDomain.GetAssemblies().Any( assembly => assembly.FullName.ToLowerInvariant().StartsWith( "nunit.framework" ) );

		/// <summary>
		///     Only allow a delegate to run X times.
		/// </summary>
		/// <param name="action">      </param>
		/// <param name="callsAllowed"></param>
		/// <returns></returns>
		/// <example>
		///     var barWithBarrier = ThreadingExtensions.ActionBarrier(Bar,
		///     remainingCallsAllowed: 2 );
		/// </example>
		/// <remarks>Calling the delegate more often than <paramref name="callsAllowed" /> should just NOP.</remarks>
		public static Action ActionBarrier( [CanBeNull] this Action action, Int64? callsAllowed = null ) {
			var context = new ContextCallOnlyXTimes( callsAllowed ?? 1 );

			return () => {
				if ( Interlocked.Decrement( ref context.CallsAllowed ) >= 0 ) { action?.Invoke(); }
			};
		}

		/// <summary>
		///     Only allow a delegate to run X times.
		/// </summary>
		/// <param name="action">      </param>
		/// <param name="parameter">   </param>
		/// <param name="callsAllowed"></param>
		/// <returns></returns>
		/// <example>
		///     var barWithBarrier = ThreadingExtensions.ActionBarrier(Bar,
		///     remainingCallsAllowed: 2 );
		/// </example>
		/// <remarks>Calling the delegate more often than <paramref name="callsAllowed" /> should just NOP.</remarks>
		public static Action ActionBarrier<T1>( [CanBeNull] this Action<T1> action, T1 parameter, Int64? callsAllowed = null ) {
			var context = new ContextCallOnlyXTimes( callsAllowed ?? 1 );

			return () => {
				if ( Interlocked.Decrement( ref context.CallsAllowed ) >= 0 ) { action?.Invoke( parameter ); }
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

			if ( !lowPriority ) { return; }

			if ( Thread.CurrentThread.Priority != ThreadPriority.Lowest ) { Thread.CurrentThread.Priority = ThreadPriority.Lowest; }
		}

		/// <summary>
		///     About X bytes by polling the object's fields.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static UInt64 CalcSizeInBytes<T>( this T obj ) {
			if ( Equals( obj, default ) ) { return 0; }

			if ( obj.GetSizeOfPrimitives( out var sizeInBytes ) ) { return sizeInBytes; }

			var fields = obj.GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );

			foreach ( var field in fields ) {

				//UInt64 localsize;
				//if ( GetSizeOfPrimitives( field.FieldType, out localsize ) ) {
				//    sizeInBytes += localsize;
				//    continue;
				//}
				//if ( field.is ) {
				//TODO check for array in GetValue
				//}
				var value = field.GetValue( obj );

				// http://rogeralsing.com/2008/02/28/linq-expressions-creating-objects/
				//// Make a NewExpression that calls the ctor with the args we just created
				//NewExpression newExp = Expression.New( ctor, argsExp );

				//// Create a lambda with the New expression as body and our param object[] as arg
				//LambdaExpression lambda = Expression.Lambda( typeof( ObjectActivator ), newExp, param );

				//// Compile it
				//ObjectActivator compiled = ( ObjectActivator )lambda.Compile();

				if ( field.FieldType.IsSubclassOf( typeof( IList ) ) ) {
					if ( !( value is IList list ) ) { continue; }

					sizeInBytes = list.Cast<Object>().Aggregate( sizeInBytes, ( current, o ) => current + o.CalcSizeInBytes() );

					continue;
				}

				if ( field.FieldType.IsSubclassOf( typeof( IDictionary ) ) ) {
					if ( value is IDictionary dictionary ) {
						foreach ( var key in dictionary.Keys ) {
							sizeInBytes += key.CalcSizeInBytes(); //TODO could optimize this out of the loop
							sizeInBytes += dictionary[key].CalcSizeInBytes();
						}
					}

					continue;
				}

				if ( field.FieldType.IsSubclassOf( typeof( IEnumerable ) ) ) {
					if ( value is IEnumerable enumerable ) { sizeInBytes = enumerable.Cast<Object>().Aggregate( sizeInBytes, ( current, o ) => current + o.CalcSizeInBytes() ); }

					continue;
				}

				if ( field.FieldType.IsArray ) {
					var bob = field.GetValue( obj );

					if ( null == bob ) { continue; }

					var list = List( bob );

					foreach ( var o in list ) { sizeInBytes += o.CalcSizeInBytes(); }

					continue;
				}

				sizeInBytes += value.CalcSizeInBytes();

				//if ( value.GetType().IsSerializable ) {
				//    try {
				//        using ( var ms = new MemoryStream() ) {
				//            var bf = new BinaryFormatter();
				//            bf.Serialize( ms, value );
				//            sizeInBytes += ( UInt64 )ms.Position;
				//        }
				//    }
				//    catch ( Exception exception ) {
				//        continue;
				//    }
				//}

				////sizeInBytes += ( UInt64 ) new StringInfo( value.Serialize() ).LengthInTextElements;
				//sizeInBytes += value.GetType().CalcSizeInBytes();
			}

			return sizeInBytes;
		}

		/// <summary>
		///     Has attributes <see cref="MethodImplOptions.NoInlining" /> and <see cref="MethodImplOptions.NoOptimization" /> .
		/// </summary>
		[MethodImpl( MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization )]
		public static void DoNothing() { }

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

		/// <summary>
		///     Split the given <paramref name="timeSpan" /> into tenths, alternating between <see cref="Thread.Sleep(TimeSpan)" />
		///     and <see cref="Thread.Yield" />
		/// </summary>
		/// <param name="thread">  </param>
		/// <param name="timeSpan"></param>
		public static void Fraggle( [NotNull] this Thread thread, TimeSpan timeSpan ) {
			if ( null == thread ) { throw new ArgumentNullException( nameof( thread ) ); }

			var stopwatch = StopWatch.StartNew();
			var tenth = TimeSpan.FromMilliseconds( timeSpan.TotalMilliseconds / 10.0 );

			if ( tenth > Seconds.One ) { tenth = Seconds.One; }

			var toggle = true;

			do {
				Application.DoEvents();
				toggle = !toggle;

				if ( toggle ) { Thread.Sleep( tenth ); }
				else { Thread.Yield(); }
			} while ( stopwatch.Elapsed < timeSpan );
		}

		public static Int32 GetMaximumActiveWorkerThreads() {
			ThreadPool.GetMaxThreads( workerThreads: out _, completionPortThreads: out var maxPortThreads );

			return maxPortThreads;
		}

		public static Boolean GetSizeOfPrimitives<T>( this T obj, out UInt64 total ) {
			if ( obj is String s1 ) {
				total = ( UInt64 )s1.Length;

				return true;
			}

			var type = typeof( T );

			if ( !type.IsPrimitive ) {
				total = 0; //TODO recurse all fields

				return false;
			}

			switch ( obj ) {
				case UInt32 _:
					total = sizeof( UInt32 );

					return true;

				case Int32 _:
					total = sizeof( Int32 );

					return true;

				case UInt64 _:
					total = sizeof( UInt64 );

					return true;

				case Int64 _:
					total = sizeof( Int64 );

					return true;

				case Decimal _:
					total = sizeof( Decimal );

					return true;

				case Double _:
					total = sizeof( Double );

					return true;

				case UInt16 _:
					total = sizeof( UInt16 );

					return true;

				case Byte _:
					total = sizeof( Byte );

					return true;

				case SByte _:
					total = sizeof( SByte );

					return true;

				case Int16 _:
					total = sizeof( Int16 );

					return true;

				case Single _:
					total = sizeof( Single );

					return true;

				case Boolean _:
					total = sizeof( Boolean );

					return true;
			}

			total = 0;

			return false; //unknown type
		}

		/// <summary>
		///     returns Marshal.SizeOf( typeof( T ) );
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static Int32 MarshalSizeOf<T>() where T : struct => Marshal.SizeOf( typeof( T ) );

		/// <summary>
		///     boxed returns Marshal.SizeOf( obj )
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static Int32 MarshalSizeOf( [NotNull] this Object obj ) {
			if ( obj is null ) { throw new ArgumentNullException( nameof( obj ) ); }

			return Marshal.SizeOf( obj );
		}

		/// <summary>
		///     generic returns Marshal.SizeOf( obj )
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static Int32 MarshalSizeOf<T>( this T obj ) => Marshal.SizeOf( obj );

		/// <summary>
		///     Repeat the <paramref name="action" /><paramref name="times" /> .
		/// </summary>
		/// <param name="times"> </param>
		/// <param name="action"></param>
		public static void Repeat( this Int32 times, [CanBeNull] Action action ) {
			if ( action is null ) { return; }

			for ( var i = 0; i < Math.Abs( times ); i++ ) { action(); }
		}

		public static void Repeat( [CanBeNull] this Action action, Int32 times ) {
			if ( action is null ) { return; }

			for ( var i = 0; i < Math.Abs( times ); i++ ) { action(); }
		}

		public static void RepeatAction( this Int32 counter, Action action ) {
			if ( null == action ) { return; }

			Parallel.For( 1, counter, i => action() );
		}

		/// <summary>
		///     Run each <see cref="Action" />, optionally in parallel (defaults to true), optionally printing feedback through an
		///     action.
		/// </summary>
		/// <param name="actions">      </param>
		/// <param name="output">     </param>
		/// <param name="description"></param>
		/// <param name="inParallel"> </param>
		/// <returns></returns>
		public static Boolean Run( [NotNull] this IEnumerable<Action> actions, Action<String> output = null, String description = null, Boolean inParallel = true ) {
			if ( actions == null ) { throw new ArgumentNullException( paramName: nameof( actions ) ); }

			if ( output != null && !String.IsNullOrWhiteSpace( description ) ) { output( description ); }

			if ( inParallel ) {
				var result = Parallel.ForEach( actions, action => action?.Invoke() );

				return result.IsCompleted;
			}

			foreach ( var action in actions ) {
				action?.Invoke();
			}

			return true;
		}

		/// <summary>
		///     Run each <see cref="Func{Boolean}" /> in parallel, optionally printing feedback through an action.
		/// </summary>
		/// <param name="functions">      </param>
		/// <param name="output">     </param>
		/// <param name="description"></param>
		/// <param name="inParallel"> </param>
		/// <returns></returns>
		public static Boolean Run( [NotNull] this IEnumerable<Func<Boolean>> functions, Action<String> output = null, String description = null, Boolean inParallel = true ) {
			if ( functions == null ) { throw new ArgumentNullException( paramName: nameof( functions ) ); }

			if ( output != null && !String.IsNullOrWhiteSpace( description ) ) { output( description ); }

			if ( inParallel ) {
				var result = Parallel.ForEach( functions, func => func?.Invoke() );

				return result.IsCompleted;
			}

			foreach ( var function in functions ) { function?.Invoke(); }

			return true;
		}

		public sealed class ContextCallOnlyXTimes {

			public Int64 CallsAllowed;

			public ContextCallOnlyXTimes( Int64 times ) {
				if ( times <= 0 ) { times = 0; }

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
                /// <seealso cref="http://stackoverflow.com/a/20639723/956364"/>
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