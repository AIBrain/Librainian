
namespace Librainian.Threading {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Annotations;
    using Measurement.Time;

    public static class ThreadingExtensions {
        /// <summary>
        ///     <para>Holder for <see cref="Environment.ProcessorCount" />.</para>
        /// </summary>
        public static readonly int ProcessorCount = Environment.ProcessorCount;

        /// <summary>
        /// <para> Sets the <see cref="ParallelOptions.MaxDegreeOfParallelism" /> of a <see
        /// cref="ParallelOptions" /> to <see cref="ProcessorCount" />. </para>
        /// </summary>
        [NotNull]
        public static readonly ParallelOptions Parallelism = new ParallelOptions {
            MaxDegreeOfParallelism = ProcessorCount
        };

        [NotNull]
        public static readonly ThreadLocal<SHA256Managed> ThreadLocalSHA256Managed = new ThreadLocal<SHA256Managed>( valueFactory: () => new SHA256Managed(), trackAllValues: false );

        public static int GetMaximumActiveWorkerThreads() {
            int maxWorkerThreads, maxPortThreads;
            ThreadPool.GetMaxThreads( workerThreads: out maxWorkerThreads, completionPortThreads: out maxPortThreads );
            return maxPortThreads;
        }

        /// <summary>
        /// Has attributes <see cref="MethodImplOptions.NoInlining"/> and <see cref="MethodImplOptions.NoOptimization"/>.
        /// </summary>
        [MethodImpl( MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization )]
        public static void DoNothing() {
        }


/*
        /// <summary>Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.</summary>
        /// <typeparam name="TResult">Specifies the type of data contained in the task.</typeparam>
        /// <param name="task">The task.</param>
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

        /// <summary>
        ///     About X bytes by polling just and ONLY fields.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int CalcSizeInBytes<T>( this T obj ) {
            if ( Equals( obj, default( T ) ) ) {
                return 0;
            }

            var type = typeof( T );
            var sizeInBytes = GetSizeOfPrimitives( type );
            var fields = type.GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );

            foreach ( var field in fields ) {
                sizeInBytes += GetSizeOfPrimitives( field.FieldType ); //the size of the field itself

                var fv = field.GetValue( obj );

                if ( Equals( obj, default( T ) ) ) {
                    continue;
                }
                if ( field.FieldType == typeof( String ) ) {
                    var s = fv as String ?? String.Empty;
                    sizeInBytes += Encoding.Unicode.GetByteCount( s );
                }
                else if ( field.FieldType.IsSubclassOf( typeof( IList ) ) ) {
                    var list = fv as IList;
                    if ( list != null ) {
                        var count = list.Count;
                        sizeInBytes += count * list[ 0 ].CalcSizeInBytes();
                    }
                }
                else if ( field.FieldType.IsSubclassOf( typeof( IDictionary ) ) ) {
                    var dictionary = fv as IDictionary;
                    if ( dictionary != null ) {
                        sizeInBytes = dictionary.Keys.Cast<object>().Aggregate<object, int>( sizeInBytes, ( current, key ) => current + dictionary[ key ].CalcSizeInBytes() );
                    }
                }
                else if ( field.FieldType.IsSubclassOf( typeof( IEnumerable ) ) ) {
                    var enumerable = fv as IEnumerable;
                    if ( enumerable != null ) {
                        sizeInBytes = enumerable.Cast<object>().Aggregate<object, int>( sizeInBytes, ( current, o ) => current + o.CalcSizeInBytes() );
                    }
                }
                else if ( field.FieldType.IsArray ) {
                    var list = fv as IList;
                    if ( list != null ) {
                        var count = list.Count;
                        sizeInBytes += count * list[ 0 ].CalcSizeInBytes();
                    }
                }
            }
            return sizeInBytes;
        }

        public static IEnumerable<T> GetEnums<T>( this T hmm ) {
            return Enum.GetValues( typeof( T ) ).Cast<T>();
            //also: return ( T[] )Enum.GetValues( typeof( T ) );    //prob faster
        }

        public static int GetSizeOfPrimitives<T>( this T type ) {
            var total = 0;
            if ( type is UInt64 ) {
                total += sizeof( UInt64 );
            }
            if ( type is Int64 ) {
                total += sizeof( Int64 );
            }
            if ( type is Decimal ) {
                total += sizeof( Decimal );
            }
            if ( type is Double ) {
                total += sizeof( Double );
            }

            if ( type is UInt32 ) {
                total += sizeof( UInt32 );
            }

            if ( type is UInt16 ) {
                total += sizeof( UInt16 );
            }
            if ( type is Byte ) {
                total += sizeof( Byte );
            }

            if ( type is SByte ) {
                total += sizeof( SByte );
            }
            if ( type is Int16 ) {
                total += sizeof( Int16 );
            }
            if ( type is Int32 ) {
                total += sizeof( Int32 );
            }

            if ( type is Single ) {
                total += sizeof( Single );
            }
            if ( type is Boolean ) {
                total += sizeof( Boolean );
            }

            var s = type as String;
            if ( s != null ) {
                total += s.Length;
            }

            return total;
        }

        /// <summary>
        ///     Thread.BeginThreadAffinity();
        ///     Thread.BeginCriticalRegion();
        /// </summary>
        public static void Begin( Boolean lowPriority = true ) {
            Thread.BeginThreadAffinity();
            Thread.BeginCriticalRegion();
            if ( !lowPriority ) {
                return;
            }
            // ReSharper disable RedundantCheckBeforeAssignment
            if ( Thread.CurrentThread.Priority != ThreadPriority.Lowest ) {
                // ReSharper restore RedundantCheckBeforeAssignment
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            }
        }

        /// <summary>
        ///     Thread.EndThreadAffinity();
        ///     Thread.EndCriticalRegion();
        /// </summary>
        public static void End() {
            Thread.EndThreadAffinity();
            Thread.EndCriticalRegion();
        }

        /// <summary>
        ///     returns Marshal.SizeOf( typeof( T ) );
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static int MarshalSizeOf<T>() where T : struct {
            return Marshal.SizeOf( typeof( T ) );
        }

        /// <summary>
        ///     boxed returns Marshal.SizeOf( obj )
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int MarshalSizeOf( [NotNull] this object obj ) {
            if ( obj == null ) {
                throw new ArgumentNullException( "obj" );
            }
            return Marshal.SizeOf( obj );
        }

        /// <summary>
        ///     generic returns Marshal.SizeOf( obj )
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int MarshalSizeOf<T>( this T obj ) {
            return Marshal.SizeOf( obj );
        }

        /// <summary>
        ///     Only allow a delegate to run X times.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callsAllowed"></param>
        /// <returns></returns>
        /// <example>var barWithBarrier = ThreadingExtensions.ActionBarrier( action: Bar, remainingCallsAllowed: 2 );</example>
        /// <remarks>Calling the delegate more often than <paramref name="callsAllowed" /> should just NOP.</remarks>
        public static Action ActionBarrier( [CanBeNull] this Action action, long? callsAllowed = null ) {
            var context = new ContextCallOnlyXTimes( callsAllowed ?? 1 );
            return () => {
                if ( Interlocked.Decrement( ref context.CallsAllowed ) >= 0 ) {
                    if ( action != null ) {
                        action();
                    }
                }
            };
        }

        /// <summary>
        ///     Only allow a delegate to run X times.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameter"></param>
        /// <param name="callsAllowed"></param>
        /// <returns></returns>
        /// <example>var barWithBarrier = ThreadingExtensions.ActionBarrier( action: Bar, remainingCallsAllowed: 2 );</example>
        /// <remarks>Calling the delegate more often than <paramref name="callsAllowed" /> should just NOP.</remarks>
        public static Action ActionBarrier<T1>( [CanBeNull] this Action<T1> action, T1 parameter, long? callsAllowed = null ) {
            var context = new ContextCallOnlyXTimes( callsAllowed ?? 1 );
            return () => {
                if ( Interlocked.Decrement( ref context.CallsAllowed ) >= 0 && action != null ) {
                    action( parameter );
                }
            };
        }

        /// <summary>
        ///     Repeat the <paramref name="action" /> <paramref name="times" /> .
        /// </summary>
        /// <param name="times"> </param>
        /// <param name="action"> </param>
        public static void Repeat( this int times, [CanBeNull] Action action ) {
            if ( null == action ) {
                return;
            }
            for ( var i = 0 ; i < Math.Abs( times ) ; i++ ) {
                action();
            }
        }

        public static void Repeat( [CanBeNull] this Action action, int times ) {
            if ( null == action ) {
                return;
            }
            for ( var i = 0 ; i < Math.Abs( times ) ; i++ ) {
                action();
            }
        }

        public static void RepeatAction( this int counter, Action action ) {
            if ( null == action ) {
                return;
            }
            Parallel.For( 1, counter, i => action() );
        }

        /// <summary>
        ///     Run each task, optionally in parallel, optionally printing feedback through an action.
        /// </summary>
        /// <param name="tasks"> </param>
        /// <param name="output"> </param>
        /// <param name="description"> </param>
        /// <param name="inParallel"> </param>
        /// <returns> </returns>
        public static Boolean Run( this IEnumerable<Action> tasks, Action<String> output = null, String description = null, Boolean inParallel = true ) {
            if ( Equals( tasks, null ) ) {
                return false;
            }
            if ( !Equals( output, null ) && !String.IsNullOrWhiteSpace( description ) ) {
                output( description );
            }
            if ( inParallel ) {
                var result = Parallel.ForEach( tasks, task => {
                    if ( task != null ) {
                        task();
                    }
                } );
                return result.IsCompleted;
            }
            foreach ( var task in tasks ) {
                task();
            }
            return true;
        }

        /// <summary>
        ///     Run each task in parallel, optionally printing feedback through an action.
        /// </summary>
        /// <param name="tasks"> </param>
        /// <param name="output"> </param>
        /// <param name="description"> </param>
        /// <param name="inParallel"> </param>
        /// <returns> </returns>
        public static Boolean Run( this IEnumerable<Func<Boolean>> tasks, Action<String> output = null, String description = null, Boolean inParallel = true ) {
            if ( Equals( tasks, null ) ) {
                return false;
            }
            if ( !Equals( output, null ) && !String.IsNullOrWhiteSpace( description ) ) {
                output( description );
            }
            var notnull = tasks.Where( task => !Equals( task, default( Action ) ) );
            if ( inParallel ) {
                var result = Parallel.ForEach( notnull, task => task() );
                return result.IsCompleted;
            }
            foreach ( var task in notnull ) {
                task();
            }
            return true;
        }

        public static Task Then( [NotNull] this Task first, [NotNull] Action next ) {
            //if ( next == null ) {
            //    throw new ArgumentNullException( "next" );
            //}

            if ( first == null ) {
                throw new ArgumentNullException( "first" );
            }
            if ( next == null ) {
                throw new ArgumentNullException( "next" );
            }
            var tcs = new TaskCompletionSource<object>(); //Tasks.FactorySooner.CreationOptions

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
        public static Task<T2> Then<T2>( this Task first, Func<Task<T2>> next ) {
            if ( first == null ) {
                throw new ArgumentNullException( "first" );
            }
            if ( next == null ) {
                throw new ArgumentNullException( "next" );
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

        [Obsolete( "use continuewith", true )]
        public static Task Then<T1>( this Task<T1> first, Action<T1> next ) {
            if ( first == null ) {
                throw new ArgumentNullException( "first" );
            }
            if ( next == null ) {
                throw new ArgumentNullException( "next" );
            }

            var tcs = new TaskCompletionSource<object>(); //Tasks.FactorySooner.CreationOptions

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

        [Obsolete( "use continuewith", true )]
        public static Task Then<T1>( this Task<T1> first, Func<T1, Task> next ) {
            if ( first == null ) {
                throw new ArgumentNullException( "first" );
            }
            if ( next == null ) {
                throw new ArgumentNullException( "next" );
            }

            var tcs = new TaskCompletionSource<object>(); //Tasks.FactorySooner.CreationOptions
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
                        if ( t == null ) {
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

        [Obsolete( "use continuewith", true )]
        public static Task<T2> Then<T1, T2>( this Task<T1> first, Func<T1, T2> next ) {
            if ( first == null ) {
                throw new ArgumentNullException( "first" );
            }
            if ( next == null ) {
                throw new ArgumentNullException( "next" );
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

        [Obsolete( "use continuewith", true )]
        public static Task<T2> Then<T1, T2>( this Task<T1> first, Func<T1, Task<T2>> next ) {
            if ( first == null ) {
                throw new ArgumentNullException( "first" );
            }
            if ( next == null ) {
                throw new ArgumentNullException( "next" );
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
                        if ( t == null ) {
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
        ///     Just a fire-and-forget wrapper for an <see cref="Action" />.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public static void Then( this Action action, Action next ) {
            if ( action == null ) {
                throw new ArgumentNullException( "action" );
            }
            action.Spawn();
            next.Spawn();
        }

        /// <summary>
        ///     var result = await Wrap( () => OldNonAsyncFunction( ) );
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Task<T> Wrap<T>( [NotNull] this Func<T> selector ) {
            if ( selector == null ) {
                throw new ArgumentNullException( "selector" );
            }
            return Task.Run( selector );
        }

        /// <summary>
        ///     var result = await Wrap( () => OldNonAsyncFunction( "hello world" ) );
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="selector"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Task<TOut> Wrap<TIn, TOut>( [NotNull] this Func<TIn, TOut> selector, TIn input ) {
            if ( selector == null ) {
                throw new ArgumentNullException( "selector" );
            }
            return Task.Run( () => selector( input ) );
        }

        /// <summary>
        ///     Just a try/catch wrapper for methods.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timeAction"></param>
        /// <param name="andTookLongerThan"></param>
        /// <param name="onException"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public static Boolean Wrap( this Action action, Boolean timeAction = true, TimeSpan? andTookLongerThan = null, Action onException = null, [CallerMemberName] String callerMemberName = "" ) {
            try {
                if ( null != action ) {
                    if ( timeAction ) {
                        if ( null == andTookLongerThan ) {
                            andTookLongerThan = Milliseconds.ThreeHundredThirtyThree;
                        }
                        var stopwatch = Stopwatch.StartNew();
                        action();
                        if ( stopwatch.Elapsed > andTookLongerThan.Value ) {
                            String.Format( "{0} took {1}.", callerMemberName, stopwatch.Elapsed.Simpler() ).TimeDebug();
                        }
                    }
                    else {
                        action();
                    }
                    return true;
                }
            }
            catch ( OutOfMemoryException lowMemory ) {
                lowMemory.Error();
                Diagnostical.Garbage();
            }
            catch ( Exception exception ) {
                if ( null != onException ) {
                    return onException.Wrap();
                }
                exception.Error();
            }
            return false;
        }

        [DllImport( "kernel32.dll" )]
        public static extern IntPtr GetCurrentThread();

        /// <summary>
        /// </summary>
        /// <param name="hThread"></param>
        /// <param name="dwThreadAffinityMask"></param>
        /// <returns></returns>
        /// <example>SetThreadAffinityMask( GetCurrentThread(), new IntPtr( 1 &lt;&lt; processor ) );</example>
        [DllImport( "kernel32.dll" )]
        public static extern IntPtr SetThreadAffinityMask( IntPtr hThread, IntPtr dwThreadAffinityMask );

        public sealed class ContextCallOnlyXTimes {
            public long CallsAllowed;

            public ContextCallOnlyXTimes( long times ) {
                if ( times <= 0 ) {
                    times = 0;
                }
                this.CallsAllowed = times;
            }
        }
    }
}
