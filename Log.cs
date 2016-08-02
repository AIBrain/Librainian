// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Log.cs" was last cleaned by Rick on 2016/06/18 at 10:58 PM

namespace Librainian {

    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Runtime;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using Extensions;
    using JetBrains.Annotations;
    using Measurement.Frequency;
    using Measurement.Time;
    using OperatingSystem;
    using Parsing;
    using Threading;

    public enum LoggingLevel {
        Critical = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Verbose = 4,
        Debug = 5
    }

    /// <summary>
    ///     A class to help with exception handling and plain ol' simple time+logging to the Console.
    ///     <para>I feel like this is a rereinvented wheel..</para>
    /// </summary>
    public static class Log {
        private static readonly ConsoleListenerWithTimePrefix ConsoleListener;

        static Log() {
            ConsoleListener = new ConsoleListenerWithTimePrefix();
        }

        public static Boolean HasConsoleBeenAllocated {
            get; private set;
        }

        [DebuggerStepThrough]
        public static void After( [CallerMemberName] String method = "" ) {
            $"After - {method ?? String.Empty}".WriteLine();
            ConsoleListener.IndentLevel--;
        }

        [DebuggerStepThrough]
        public static void Before( [CallerMemberName] String method = "" ) {
            ConsoleListener.IndentLevel++;
            $"Before - {method ?? String.Empty}".WriteLine();
        }

        [DebuggerStepThrough]
        public static void BreakIfFalse( this Boolean condition, String message = "" ) {
            if ( condition ) {
                return;
            }
            if ( !String.IsNullOrEmpty( message ) ) {
                Debug.WriteLine( message );
            }
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        [DebuggerStepThrough]
        public static void BreakIfTrue( this Boolean condition, String message = "" ) {
            if ( !condition ) {
                return;
            }
            if ( !String.IsNullOrEmpty( message ) ) {
                Debug.WriteLine( message );
            }
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        /// <summary>
        ///     See also <seealso cref="Log.Exit" />.
        /// </summary>
        /// <param name="method"></param>
        [DebuggerStepThrough]
        public static void Enter( [CallerMemberName] String method = null ) {
            ConsoleListener.IndentLevel++;
            $"Entering {method ?? String.Empty}".WriteLine();
        }

        /// <summary>
        ///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
        ///     <para>
        ///         See also: <seealso cref="Message" />, <seealso cref="Info" />,
        ///         <seealso cref="Warning" />, and <seealso cref="Error" />.
        ///     </para>
        /// </summary>
        /// <param name="message"></param>
        [DebuggerStepThrough]
        public static void Error( [CanBeNull] this String message ) {
            if ( message != null ) {
                $"Error: {message}".WriteLine();
            }
        }

        [DebuggerStepThrough]
        public static void Exit( [CallerMemberName] String method = "" ) {
            $"Exited {method ?? String.Empty}".WriteLine();
            ConsoleListener.IndentLevel--;
        }

        [DebuggerStepThrough]
        public static void Finalized( [CallerMemberName] String method = "" ) => $"{"Finalized"}: {method ?? String.Empty}".WriteLine();

        /// <summary>
        ///     Gets the number of frames in the <see cref="StackTrace" />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedParameter.Global
        public static Int32 FrameCount( this Object obj ) => new StackTrace( false ).FrameCount;

        /// <summary>
        ///     Force a memory garbage collection on generation0 and generation1 objects.
        /// </summary>
        /// <seealso
        ///     cref="http://programmers.stackexchange.com/questions/276585/when-is-it-a-good-idea-to-force-garbage-collection" />
        public static void Garbage() {
            var before = GC.GetTotalMemory( forceFullCollection: false );
            GC.Collect( generation: 1, mode: GCCollectionMode.Forced, blocking: true );
            var after = GC.GetTotalMemory( forceFullCollection: false );

            if ( after < before ) {
                $"{before - after} bytes freed by the GC.".Info();
            }
        }

        /// <summary>
        ///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
        ///     <para>
        ///         See also: <seealso cref="Message" />, <seealso cref="Info" />,
        ///         <seealso cref="Warning" />, and <seealso cref="Error" />.
        ///     </para>
        /// </summary>
        /// <param name="message"></param>
        [DebuggerStepThrough]
        public static String Info( [CanBeNull] this String message ) {
            $"Info: {message ?? String.Empty}".WriteLine();
            return message ?? String.Empty;
        }

        /// <summary>
        ///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
        ///     <para>
        ///         See also: <seealso cref="Message" />, <seealso cref="Info" />,
        ///         <seealso cref="Warning" />, and <seealso cref="Error" />.
        ///     </para>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="method"></param>
        [DebuggerStepThrough]
        public static String Message( [CanBeNull] this String message, [CallerMemberName] String method = "" ) {
            $"{method.NullIfEmpty() ?? "?"}: {message}".WriteLine();
            return message ?? String.Empty;
        }

        /// <summary>
        ///     "Bring out yer' dead! Bring out yer' dead!"
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="method"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        [DebuggerStepThrough]
        public static void More( [NotNull] this Exception exception, [CanBeNull] [CallerMemberName] String method = "", [CanBeNull] [CallerFilePath] String sourceFilePath = "", [CallerLineNumber] Int32 sourceLineNumber = 0 ) {
            var message = new StringBuilder();
            message.AppendFormat( " [Exception: {0}]\r\n", exception.Message );
            message.AppendFormat( " [In: {0}]\r\n", exception.Source );
            message.AppendFormat( " [When: {0}]\r\n", DateTime.Now );
            message.AppendFormat( " [Msg: {0}]\r\n", exception.Message );
            message.AppendFormat( " [Source: {0}]\r\n", sourceFilePath );
            message.AppendFormat( " [Line: {0}]\r\n", sourceLineNumber );
            ConsoleListener.Fail( method, message.ToString() );
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        public static void Shutdown( Boolean linger = true ) {
            if ( !HasConsoleBeenAllocated ) {
                return;
            }
            "Shutting down".WriteColor( ConsoleColor.White, ConsoleColor.Blue );
            if ( linger ) {
                var stopwatch = StopWatch.StartNew();
                while ( stopwatch.Elapsed < Milliseconds.FiveHundred ) {
                    Thread.Sleep( Hertz.OneHundredTwenty );
                    ".".WriteColor( ConsoleColor.White, ConsoleColor.Blue );
                }
                stopwatch.Stop();
            }
            NativeWin32.FreeConsole();
        }

        public static Boolean Startup() {
            try {
                HasConsoleBeenAllocated = NativeWin32.AllocConsole();

                Debug.Listeners.Add( ConsoleListener );

                Contract.ContractFailed += ( sender, e ) => {
                    var message = $"Caught Uncaught Contract Failure:\r\nCondition:{e.Condition}\r\nFailureKind:{e.FailureKind}\r\nHandled:{e.Handled}\r\nMessage:{e.Message}";
                    Debugger.IsAttached.BreakIfTrue( message );
                    e.OriginalException.More();
                };

                ProfileOptimization.SetProfileRoot( Application.ExecutablePath );
                ProfileOptimization.StartProfile( Application.ExecutablePath );

                return HasConsoleBeenAllocated;
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return false;
        }

        /// <summary>
        ///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
        ///     <para>
        ///         See also: <seealso cref="Message" />, <seealso cref="Info" />,
        ///         <seealso cref="Warning" />, and <seealso cref="Error" />.
        ///     </para>
        /// </summary>
        /// <param name="message"></param>
        [DebuggerStepThrough]
        public static void Warning( this String message ) => $"Warning: {message}".WriteLine();

        /// <summary>
        ///     <para>Write the <paramref name="message" /> out to the <see cref="ConsoleListener" />.</para>
        /// </summary>
        /// <param name="message"></param>
        [DebuggerStepThrough]
        public static void Write( this String message ) => ConsoleListener.Write( message );

        /// <summary>
        ///     <para>Write the <paramref name="message" /> out to the <see cref="ConsoleListener" />.</para>
        /// </summary>
        /// <param name="message"></param>
        [DebuggerStepThrough]
        public static void WriteLine( this String message ) => ConsoleListener.WriteLine( message );
    }
}