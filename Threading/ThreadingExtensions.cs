// Copyright 2018 Protiguous.
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
// "Librainian/ThreadingExtensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

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
    using Maths;
    using Measurement.Time;
    using static LanguageExt.Prelude;

    public static class ThreadingExtensions {

        /// <summary>
        /// <para>Sets the <see cref="System.Threading.Tasks.ParallelOptions.MaxDegreeOfParallelism"/> of a <see cref="System.Threading.Tasks.ParallelOptions"/> to <see cref="Environment.ProcessorCount"/>.</para>
        /// <para>1 core to 1</para>
        /// <para>2 cores to 2</para>
        /// <para>4 cores to 4</para>
        /// <para>8 cores to 8</para>
        /// <para>n cores to n</para>
        /// </summary>
        [NotNull]
        public static readonly ParallelOptions AllCPU = new ParallelOptions { MaxDegreeOfParallelism = Math.Max( 1, Environment.ProcessorCount ) };

        /// <summary>
        /// <para>Sets the <see cref="System.Threading.Tasks.ParallelOptions.MaxDegreeOfParallelism"/> of a <see cref="System.Threading.Tasks.ParallelOptions"/> to <see cref="Environment.ProcessorCount"/>-1.</para>
        /// <para>1 core to 1</para>
        /// <para>2 cores to 1</para>
        /// <para>4 cores to 3</para>
        /// <para>8 cores to 7</para>
        /// <para>n cores to n-1</para>
        /// </summary>
        [NotNull]
        public static readonly ParallelOptions CPUIntensive = new ParallelOptions {
            MaxDegreeOfParallelism = Math.Max( 1, Environment.ProcessorCount - 1 ) //leave the OS a little wiggle room
        };

        /// <summary>
        /// <para>Sets the <see cref="System.Threading.Tasks.ParallelOptions.MaxDegreeOfParallelism"/> of a <see cref="System.Threading.Tasks.ParallelOptions"/> to half of <see cref="Environment.ProcessorCount"/>.</para>
        /// <para>1 core to 1?</para>
        /// <para>2 cores to 1</para>
        /// <para>4 cores to 2</para>
        /// <para>8 cores to 4</para>
        /// <para>n cores to n/2</para>
        /// </summary>
        [NotNull]
        public static readonly ParallelOptions CPULight = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount / 2 };

        /// <summary>
        /// <para>Sets the <see cref="System.Threading.Tasks.ParallelOptions.MaxDegreeOfParallelism"/> of a <see cref="System.Threading.Tasks.ParallelOptions"/> to <see cref="Environment.ProcessorCount"/> * 2.</para>
        /// <para>1 core to 2</para>
        /// <para>2 cores to 4</para>
        /// <para>4 cores to 8</para>
        /// <para>8 cores to 16</para>
        /// <para>n cores to 2n</para>
        /// </summary>
        [NotNull]
        public static readonly ParallelOptions DiskIntensive = new ParallelOptions { MaxDegreeOfParallelism = Math.Max( 1, Environment.ProcessorCount * 2 ) };

        public static readonly Boolean IsRunningFromNUnit = AppDomain.CurrentDomain.GetAssemblies().Any( assembly => assembly.FullName.ToLowerInvariant().StartsWith( "nunit.framework" ) );

        /// <summary>
        /// Only allow a delegate to run X times.
        /// </summary>
        /// <param name="action">      </param>
        /// <param name="callsAllowed"></param>
        /// <returns></returns>
        /// <example>
        /// var barWithBarrier = ThreadingExtensions.ActionBarrier(Bar,
        /// remainingCallsAllowed: 2 );
        /// </example>
        /// <remarks>Calling the delegate more often than <paramref name="callsAllowed"/> should just NOP.</remarks>
        public static Action ActionBarrier( [CanBeNull] this Action action, Int64? callsAllowed = null ) {
            var context = new ContextCallOnlyXTimes( callsAllowed ?? 1 );
            return () => {
                if ( Interlocked.Decrement( ref context.CallsAllowed ) >= 0 ) {
                    action?.Invoke();
                }
            };
        }

        /// <summary>
        /// Only allow a delegate to run X times.
        /// </summary>
        /// <param name="action">      </param>
        /// <param name="parameter">   </param>
        /// <param name="callsAllowed"></param>
        /// <returns></returns>
        /// <example>
        /// var barWithBarrier = ThreadingExtensions.ActionBarrier(Bar,
        /// remainingCallsAllowed: 2 );
        /// </example>
        /// <remarks>Calling the delegate more often than <paramref name="callsAllowed"/> should just NOP.</remarks>
        public static Action ActionBarrier<T1>( [CanBeNull] this Action<T1> action, T1 parameter, Int64? callsAllowed = null ) {
            var context = new ContextCallOnlyXTimes( callsAllowed ?? 1 );
            return () => {
                if ( Interlocked.Decrement( ref context.CallsAllowed ) >= 0 ) {
                    action?.Invoke( parameter );
                }
            };
        }

        /// <summary>
        /// <para>Thread.BeginThreadAffinity();</para>
        /// <para>Thread.BeginCriticalRegion();</para>
        /// <para>...</para>
        /// <see cref="End"/>
        /// </summary>
        public static void Begin( Boolean lowPriority = true ) {
            Thread.BeginThreadAffinity();
            Thread.BeginCriticalRegion();
            if ( !lowPriority ) {
                return;
            }

            if ( Thread.CurrentThread.Priority != ThreadPriority.Lowest ) {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            }
        }

        /// <summary>
        /// About X bytes by polling the object's fields.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static UInt64 CalcSizeInBytes<T>( this T obj ) {
            if ( Equals( obj, default ) ) {
                return 0;
            }

            if ( obj.GetSizeOfPrimitives( out var sizeInBytes ) ) {
                return sizeInBytes;
            }

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
                    if ( !( value is IList list ) ) {
                        continue;
                    }
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
                    if ( value is IEnumerable enumerable ) {
                        sizeInBytes = enumerable.Cast<Object>().Aggregate( sizeInBytes, ( current, o ) => current + o.CalcSizeInBytes() );
                    }
                    continue;
                }

                if ( field.FieldType.IsArray ) {
                    var bob = field.GetValue( obj );
                    if ( null == bob ) {
                        continue;
                    }
                    var list = List( bob );
                    foreach ( var o in list ) {
                        sizeInBytes += o.CalcSizeInBytes();
                    }
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
        /// Has attributes <see cref="MethodImplOptions.NoInlining"/> and <see cref="MethodImplOptions.NoOptimization"/> .
        /// </summary>
        [MethodImpl( MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization )]
        public static void DoNothing() {
        }

        /// <summary>
        /// <para><see cref="Begin"/></para>
        /// <para>...</para>
        /// <para>Thread.EndThreadAffinity();</para>
        /// <para>Thread.EndCriticalRegion();</para>
        /// </summary>
        public static void End() {
            Thread.EndThreadAffinity();
            Thread.EndCriticalRegion();
        }

        /// <summary>
        /// Split the given <paramref name="timeSpan"/> into tenths, alternating between <see cref="Thread.Sleep(TimeSpan)"/> and <see cref="Thread.Yield"/>
        /// </summary>
        /// <param name="thread">  </param>
        /// <param name="timeSpan"></param>
        public static void Fraggle( [NotNull] this Thread thread, TimeSpan timeSpan ) {
            if ( null == thread ) {
                throw new ArgumentNullException( nameof( thread ) );
            }
            var stopwatch = StopWatch.StartNew();
            var tenth = TimeSpan.FromMilliseconds( timeSpan.TotalMilliseconds / 10.0 );
            if ( tenth > Seconds.One ) {
                tenth = Seconds.One;
            }
            var toggle = true;
            do {
                Application.DoEvents();
                toggle = !toggle;
                if ( toggle ) {
                    Thread.Sleep( tenth );
                }
                else {
                    Thread.Yield();
                }
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
                total = 0;  //TODO recurse all fields
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

        public static Boolean IsNotRunning( this Task task ) => task.IsCompleted || task.IsCanceled || task.IsFaulted;

        /// <summary>
        /// returns Marshal.SizeOf( typeof( T ) );
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Int32 MarshalSizeOf<T>() where T : struct => Marshal.SizeOf( typeof( T ) );

        /// <summary>
        /// boxed returns Marshal.SizeOf( obj )
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Int32 MarshalSizeOf( [NotNull] this Object obj ) {
            if ( obj is null ) {
                throw new ArgumentNullException( nameof( obj ) );
            }
            return Marshal.SizeOf( obj );
        }

        /// <summary>
        /// generic returns Marshal.SizeOf( obj )
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Int32 MarshalSizeOf<T>( this T obj ) => Marshal.SizeOf( obj );

        /// <summary>
        /// Repeat the <paramref name="action"/><paramref name="times"/> .
        /// </summary>
        /// <param name="times"> </param>
        /// <param name="action"></param>
        public static void Repeat( this Int32 times, [CanBeNull] Action action ) {
            if ( action is null ) {
                return;
            }
            for ( var i = 0; i < Math.Abs( times ); i++ ) {
                action();
            }
        }

        public static void Repeat( [CanBeNull] this Action action, Int32 times ) {
            if ( action is null ) {
                return;
            }
            for ( var i = 0; i < Math.Abs( times ); i++ ) {
                action();
            }
        }

        public static void RepeatAction( this Int32 counter, Action action ) {
            if ( null == action ) {
                return;
            }
            Parallel.For( 1, counter, i => action() );
        }

        /// <summary>
        /// Run each task, optionally in parallel, optionally printing feedback through an action.
        /// </summary>
        /// <param name="tasks">      </param>
        /// <param name="output">     </param>
        /// <param name="description"></param>
        /// <param name="inParallel"> </param>
        /// <returns></returns>
        public static Boolean Run( this IEnumerable<Action> tasks, Action<String> output = null, String description = null, Boolean inParallel = true ) {
            if ( Equals( tasks, null ) ) {
                return false;
            }
            if ( !Equals( output, null ) && !String.IsNullOrWhiteSpace( description ) ) {
                output( description );
            }
            if ( inParallel ) {
                var result = Parallel.ForEach( tasks, task => task?.Invoke() );
                return result.IsCompleted;
            }
            foreach ( var task in tasks ) {
                task();
            }
            return true;
        }

        /// <summary>
        /// Run each task in parallel, optionally printing feedback through an action.
        /// </summary>
        /// <param name="tasks">      </param>
        /// <param name="output">     </param>
        /// <param name="description"></param>
        /// <param name="inParallel"> </param>
        /// <returns></returns>
        public static Boolean Run( this IEnumerable<Func<Boolean>> tasks, Action<String> output = null, String description = null, Boolean inParallel = true ) {
            if ( Equals( tasks, null ) ) {
                return false;
            }
            if ( !Equals( output, null ) && !String.IsNullOrWhiteSpace( description ) ) {
                output( description );
            }
            var notnull = tasks.Where( task => !Equals( task, default ) );
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
        public static Task<T2> Then<T2>( this Task first, Func<Task<T2>> next ) {
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

        public static Task Then<T1>( this Task<T1> first, Action<T1> next ) {
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

        public static Task Then<T1>( this Task<T1> first, Func<T1, Task> next ) {
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

        public static Task<T2> Then<T1, T2>( this Task<T1> first, Func<T1, T2> next ) {
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

        public static Task<T2> Then<T1, T2>( this Task<T1> first, Func<T1, Task<T2>> next ) {
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
        /// <para>Returns true if the task finished before the <paramref name="timeout"/>.</para>
        /// <para>Use this function if the Task does not have a built-in timeout.</para>
        /// <para>This function does not end the given <paramref name="task"/> if it does timeout.</para>
        /// </summary>
        /// <param name="task">   </param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<Boolean> Until( this Task task, TimeSpan timeout ) {
            var delay = Task.Delay( timeout );

            var whichTaskFinished = await Task.WhenAny( task, delay );

            var didOurTaskFinish = whichTaskFinished == task;

            return didOurTaskFinish;
        }

        /// <summary>
        /// Returns true if the task finished before the <paramref name="timeout"/>.
        /// </summary>
        /// <param name="task">   </param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<Boolean> Until( this TimeSpan timeout, Task task ) => await Until( task, timeout );

        /// <summary>
        /// “I have an async operation that’s not cancelable. How do I cancel it?”
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">             </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <copyright>
        ///     http: //blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx
        /// </copyright>
        public static async Task<T> WithCancellation<T>( this Task<T> task, CancellationToken cancellationToken ) {
            var tcs = new TaskCompletionSource<Boolean>();
            using ( cancellationToken.Register( o => ( ( TaskCompletionSource<Boolean> )o ).TrySetResult( true ), tcs ) ) {
                if ( task != await Task.WhenAny( task, tcs.Task ) ) {
                    throw new OperationCanceledException( cancellationToken );
                }
                return await task;
            }
        }

        /// <summary>
        /// "you can even have a timeout using the following simple extension method"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">             </param>
        /// <param name="timeout">          </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T> WithCancellation<T>( this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken ) {
            var t = await Task.WhenAny( task, Task.Delay( timeout, cancellationToken ) );

            cancellationToken.ThrowIfCancellationRequested();

            if ( t != task ) {
                throw new OperationCanceledException( "timeout" );
            }

            return task.Result;
        }

        /// <summary>
        /// var result = await Wrap( () =&gt; OldNonAsyncFunction( ) );
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Task<T> Wrap<T>( [NotNull] this Func<T> selector ) {
            if ( selector is null ) {
                throw new ArgumentNullException( nameof( selector ) );
            }
            return Task.Run( selector );
        }

        /// <summary>
        /// var result = await Wrap( () =&gt; OldNonAsyncFunction( "hello world" ) );
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="selector"></param>
        /// <param name="input">   </param>
        /// <returns></returns>
        public static Task<TOut> Wrap<TIn, TOut>( [NotNull] this Func<TIn, TOut> selector, TIn input ) {
            if ( selector is null ) {
                throw new ArgumentNullException( nameof( selector ) );
            }
            return Task.Run( () => selector( input ) );
        }

        /// <summary>
        /// Just a try/catch wrapper for methods.
        /// </summary>
        /// <param name="action">           </param>
        /// <param name="timeAction">       </param>
        /// <param name="andTookLongerThan"></param>
        /// <param name="onException">      </param>
        /// <param name="callerMemberName"> </param>
        /// <returns></returns>
        public static Boolean Wrap( this Action action, Boolean timeAction = true, TimeSpan? andTookLongerThan = null, Action onException = null, [CallerMemberName] String callerMemberName = "" ) {
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