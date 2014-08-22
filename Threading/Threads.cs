#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/Threads.cs" was last cleaned by Rick on 2014/08/11 at 12:41 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Annotations;
    using Extensions;
    using Maths;
    using Measurement.Time;
    using Timer = System.Timers.Timer;

    public static class Threads {
        /// <summary>
        ///     <para>Holder for <see cref="Process.GetCurrentProcess" />.</para>
        /// </summary>
        public static readonly Process CurrentProcess = Process.GetCurrentProcess();

        /// <summary>
        ///     <para>Holder for <see cref="Environment.ProcessorCount" />.</para>
        /// </summary>
        public static readonly int ProcessorCount = Environment.ProcessorCount;

        public static TimeSpan? SliceAverageCache;

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
                        sizeInBytes = dictionary.Keys.Cast<object>().Aggregate( sizeInBytes, ( current, key ) => current + dictionary[ key ].CalcSizeInBytes() );
                    }
                }
                else if ( field.FieldType.IsSubclassOf( typeof( IEnumerable ) ) ) {
                    var enumerable = fv as IEnumerable;
                    if ( enumerable != null ) {
                        sizeInBytes = enumerable.Cast<object>().Aggregate( sizeInBytes, ( current, o ) => current + o.CalcSizeInBytes() );
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

        /// <summary>
        ///     Thread.EndThreadAffinity();
        ///     Thread.EndCriticalRegion();
        /// </summary>
        public static void End() {
            Thread.EndThreadAffinity();
            Thread.EndCriticalRegion();
        }

        /// <summary>
        ///     Gets the number of frames in the <see cref="StackTrace" />
        /// </summary>
        /// <param name="obj"> </param>
        /// <returns> </returns>
        public static int FrameCount( this Object obj ) {
            return ( new StackTrace( false ) ).FrameCount;
        }

        /// <summary>
        ///     Force a memory garbage collection on generation0 and generation1 objects.
        /// </summary>
        public static void Garbage() {
            var before = GC.GetTotalMemory( forceFullCollection: false );
            Wrap( () => GC.Collect( generation: 1, mode: GCCollectionMode.Optimized, blocking: false ) );
            var after = GC.GetTotalMemory( forceFullCollection: false );

            if ( after < before ) {
                String.Format( "{0} bytes freed by the GC.", before - after ).TimeDebug();
            }
        }

        public static IEnumerable<T> GetEnums<T>( this T hmm ) {
            return Enum.GetValues( typeof( T ) ).Cast<T>();
            //also: return ( T[] )Enum.GetValues( typeof( T ) );    //prob faster
        }

        public static int GetSizeOfPrimitives( this Type type ) {
            if ( type == typeof( Byte ) ) {
                return sizeof( Byte );
            }
            if ( type == typeof( UInt16 ) ) {
                return sizeof( UInt16 );
            }
            if ( type == typeof( UInt32 ) ) {
                return sizeof( UInt32 );
            }
            if ( type == typeof( UInt64 ) ) {
                return sizeof( UInt64 );
            }

            if ( type == typeof( SByte ) ) {
                return sizeof( SByte );
            }
            if ( type == typeof( Int16 ) ) {
                return sizeof( Int16 );
            }
            if ( type == typeof( Int32 ) ) {
                return sizeof( Int32 );
            }
            if ( type == typeof( Int64 ) ) {
                return sizeof( Int64 );
            }

            if ( type == typeof( Single ) ) {
                return sizeof( Single );
            }
            if ( type == typeof( Double ) ) {
                return sizeof( Double );
            }
            if ( type == typeof( Decimal ) ) {
                return sizeof( Decimal );
            }
            if ( type == typeof( Boolean ) ) {
                return sizeof( Boolean );
            }

            return sizeof( UInt64 ); //HACK assume 8 bytes..?
        }

        public static TimeSpan GetSlice( Boolean? setProcessorAffinity = null ) {
            if ( setProcessorAffinity.HasValue && setProcessorAffinity.Value ) {
                try {
                    var affinityMask = ( long )CurrentProcess.ProcessorAffinity;
                    affinityMask &= 0xFFFF; // use any of the available processors
                    CurrentProcess.ProcessorAffinity = ( IntPtr )affinityMask;
                }
                catch ( Win32Exception ) {
                    /*swallow*/
                }
                catch ( NotSupportedException ) {
                    /*swallow*/
                }
                catch ( InvalidOperationException ) {
                    /*swallow*/
                }
            }

            var stopwatch = Stopwatch.StartNew();
            Task.Run( () => Thread.Sleep( 1 ) ).Wait();
            return stopwatch.Elapsed;
        }

        public static TimeSpan GetSlicingAverage( Boolean useCache = true, Boolean? setProcessorAffinity = null ) {
            if ( useCache && SliceAverageCache.HasValue ) {
                return SliceAverageCache.Value;
            }

            if ( setProcessorAffinity.HasValue && setProcessorAffinity.Value ) {
                try {
                    var affinityMask = ( long )CurrentProcess.ProcessorAffinity;
                    affinityMask &= 0xFFFF; // use any of the available processors
                    CurrentProcess.ProcessorAffinity = ( IntPtr )affinityMask;
                }
                catch ( Win32Exception ) {
                    /*swallow*/
                }
                catch ( NotSupportedException ) {
                    /*swallow*/
                }
                catch ( InvalidOperationException ) {
                    /*swallow*/
                }
            }

            String.Format( "Performing {0} timeslice calibrations.", ProcessorCount ).TimeDebug();
            SliceAverageCache = new Milliseconds( ( decimal )( 1 + Math.Ceiling( 0.To( ProcessorCount ).Select( i => GetSlice() ).Average( span => span.TotalMilliseconds ) ) ) );
            String.Format( "Timeslice calibration is {0}.", SliceAverageCache.Value.Simpler() ).TimeDebug();
            return SliceAverageCache.Value;
        }

        /// <summary>
        ///     Accurate to within how many nanoseconds?
        /// </summary>
        /// <returns></returns>
        public static long GetTimerAccuracy() {
            return 1000000000L / Stopwatch.Frequency;
        }

        /// <summary>
        ///     TODO replace this with a proper IoC container.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        [DebuggerStepThrough]
        public static void Error( [CanBeNull] this Exception exception, [CanBeNull] String message = "", [CanBeNull] [CallerMemberName] String memberName = "", [CanBeNull] [CallerFilePath] String sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0 ) {

#if DEBUG
            if ( !String.IsNullOrEmpty( message ) ) {
                Debug.WriteLine( "[{0}]", message );
            }
            Debug.Indent();
            Debug.WriteLine( "[Method: {0}]", memberName );
            if ( exception != null ) {
                Debug.WriteLine( "[Exception: {0}]", exception.Message );
                Debug.WriteLine( "[In: {0}]", exception.Source );
                Debug.WriteLine( "[Msg: {0}]", exception.Message );
                Debug.WriteLine( "[Source: {0}]", sourceFilePath );
                Debug.WriteLine( "[Line: {0}]", sourceLineNumber );
            }
            Debug.Unindent();
#else

            if ( !String.IsNullOrEmpty( message ) ) {
                Trace.WriteLine( "[{0}]", message );
            }
            Trace.Indent();
            Trace.WriteLine( "[Method: {0}]", memberName );
            if ( exception != null ) {
                Trace.WriteLine( "[Exception: {0}]", exception.Message );
                Trace.WriteLine( "[In: {0}]", exception.Source );
                Trace.WriteLine( "[Msg: {0}]", exception.Message );
                Trace.WriteLine( "[Source: {0}]", sourceFilePath );
                Trace.WriteLine( "[Line: {0}]", sourceLineNumber );
            }
            Trace.Unindent();
#endif

            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }


        }

        //    e.GetObjectData( si, ctx );
        //    mgr.RegisterObject( e, 1, si );
        //    mgr.DoFixups();
        //}
        /// <summary>
        ///     returns Marshal.SizeOf( typeof( T ) );
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static int MarshalSizeOf<T>() where T : struct {
            return Marshal.SizeOf( typeof( T ) );
        }

        //public static void PreserveStackTrace( this Exception e ) {
        //    var ctx = new StreamingContext( StreamingContextStates.CrossAppDomain );
        //    var mgr = new ObjectManager( null, ctx );
        //    var si = new SerializationInfo( e.GetType(), new FormatterConverter() );
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
        ///     Gets a <b>horrible</b> ROUGH guesstimate of the memory consumed by an object by using
        ///     <seealso
        ///         cref="NetDataContractSerializer" />
        ///     .
        /// </summary>
        /// <param name="bob"> </param>
        /// <returns> </returns>
        public static long MemoryUsed( [NotNull] this Object bob ) {
            if ( bob == null ) {
                throw new ArgumentNullException( "bob" );
            }
            try {
                using ( var s = new NullStream() ) {
                    var serializer = new NetDataContractSerializer {
                        AssemblyFormat = FormatterAssemblyStyle.Full
                    };
                    serializer.WriteObject( stream: s, graph: bob );
                    return s.Length;
                }
            }
            catch ( InvalidDataContractException exception ) {
                exception.Error();
            }
            catch ( SerializationException exception ) {
                exception.Error();
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            return 0;
        }

        public static long MinifyXML( this XmlDocument xml ) {
            //TODO todo.
            throw new NotImplementedException();
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
            for ( var i = 0; i < Math.Abs( times ); i++ ) {
                action();
            }
        }

        public static void Repeat( [CanBeNull] this Action action, int times ) {
            if ( null == action ) {
                return;
            }
            for ( var i = 0; i < Math.Abs( times ); i++ ) {
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
                    tcs.TrySetException( first.Exception.InnerExceptions );
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
                                    tcs.TrySetException( t.Exception.InnerExceptions );
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
                    tcs.TrySetException( first.Exception.InnerExceptions );
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
                                    tcs.TrySetException( t.Exception.InnerExceptions );
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

        [DebuggerStepThrough]
        public static void TimeDebug( [CanBeNull] this String message, Boolean newline = true ) {
            if ( message == null ) {
                return;
            }
            if ( newline ) {
                Debug.WriteLine( "[{0:s}].({1}) {2}", DateTime.UtcNow, Thread.CurrentThread.ManagedThreadId, message );
            }
            else {
                Debug.Write( String.Format( "[] {0}", message ) );
            }
        }

        /// <summary>
        ///     Do a Take() on the top X percent
        /// </summary>
        /// <typeparam name="TSource"> </typeparam>
        /// <param name="source"> </param>
        /// <param name="x"> </param>
        /// <returns> </returns>
        public static IEnumerable<TSource> Top<TSource>( [NotNull] this IEnumerable<TSource> source, Double x ) {
            if ( source == null ) {
                throw new ArgumentNullException( "source" );
            }
            var sources = source as IList<TSource> ?? source.ToList();
            return sources.Take( ( int )( x * sources.Count() ) );
        }

        /// <summary>
        ///     Start a timer. When it fires, check the <paramref name="condition" />, and if true do the
        ///     <paramref name="action" />.
        /// </summary>
        /// <param name="afterDelay"></param>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        public static Timer When( this Span afterDelay, Func<Boolean> condition, Action action ) {
            if ( condition == null ) {
                throw new ArgumentNullException( "condition" );
            }
            if ( action == null ) {
                throw new ArgumentNullException( "action" );
            }
            try {
                return afterDelay.Create( () => {
                    if ( condition() ) {
                        action();
                    }
                } ).Once().AndStart();
            }
            catch ( Exception exception ) {
                exception.Error();
                return null;
            }
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
                Garbage();
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
        private static extern IntPtr GetCurrentThread();

        /// <summary>
        /// </summary>
        /// <param name="hThread"></param>
        /// <param name="dwThreadAffinityMask"></param>
        /// <returns></returns>
        /// <example>SetThreadAffinityMask( GetCurrentThread(), new IntPtr( 1 &lt;&lt; processor ) );</example>
        [DllImport( "kernel32.dll" )]
        private static extern IntPtr SetThreadAffinityMask( IntPtr hThread, IntPtr dwThreadAffinityMask );

        public sealed class ContextCallOnlyXTimes {
            public long CallsAllowed;

            public ContextCallOnlyXTimes( long times ) {
                if ( times <= 0 ) {
                    times = 0;
                }
                this.CallsAllowed = times;
            }
        }

        public static class Report {
            /// <summary>
            ///     TODO add in the threadID
            /// </summary>
            /// <param name="method"></param>
            /// <param name="FullMethodPath"></param>
            [DebuggerStepThrough]
            public static void Enter( [CallerMemberName] String method = "", [Custom] String FullMethodPath = "" ) {
                //if ( String.IsNullOrWhiteSpace( method ) ) {
                //    return;
                //}
                Debug.Indent();
                String.Format( "{0}: {1}  {2}", "enter", method, FullMethodPath ).TimeDebug();
            }

            /// <summary>
            ///     TODO add in the threadID
            /// </summary>
            /// <param name="method"></param>
            [DebuggerStepThrough]
            public static void Exit( [CallerMemberName] String method = "" ) {
                //if ( String.IsNullOrWhiteSpace( method ) ) {
                //    return;
                //}
                String.Format( "{0}: {1}", "exit", method ).TimeDebug();
                Debug.Unindent();
            }
        }
    }
}
