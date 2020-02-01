// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "LoggingOld.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "LoggingOld.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

/*
namespace LibrainianCore.Logging {

    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using Extensions;
    using JetBrains.Annotations;
    using Measurement.Frequency;
    using Measurement.Time;
    using Parsing;
    using Vanara.PInvoke;

    [Obsolete("Use LoggingExtensions instead.")]
    public static class LoggingOld {

        public static Boolean HasConsoleBeenAllocated { get; private set; }

        private static readonly ConsoleListenerWithTimePrefix ConsoleListener;

        static LoggingOld() =>
            ConsoleListener = new ConsoleListenerWithTimePrefix {
                IndentSize = 1
            };

        /// <summary>
        ///     <see cref="Before" />
        /// </summary>
        /// <param name="method"></param>
        /// <param name="loggingLevel"></param>
        [DebuggerStepThrough]
        public static void After( [CanBeNull] [CallerMemberName] String method = null, LoggingLevel loggingLevel = LoggingLevel.Diagnostic ) {
            $"After - {method ?? String.Empty}".Diagnostic();
            ConsoleListener.IndentLevel--;
        }

        /// <summary>
        ///     <see cref="After" />
        /// </summary>
        /// <param name="method"></param>
        [DebuggerStepThrough]
        public static void Before( [CanBeNull] [CallerMemberName] String method = null ) {
            ConsoleListener.IndentLevel++;
            $"Before - {method ?? String.Empty}".Info();
        }

        [DebuggerStepThrough]
        public static void Break( [NotNull] this Exception exception ) {
            if ( exception is null ) {
                throw new ArgumentNullException(  nameof( exception ) );
            }

            if ( !String.IsNullOrEmpty( exception.Message ) ) {
                Debug.WriteLine( exception.ToString() );
            }

            Break();
        }

        /// <summary>
        ///     <see cref="Debugger.Break" /> if a <see cref="Debugger" /> is attached.
        /// </summary>
        public static void Break() {
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        public static void Break( TrimmedString message ) {
            Debug.WriteLine( message );
            Break();
        }

        public static void Break( TrimmedString message, [NotNull] params Object[] args ) {
            Debug.WriteLine( message, args );
            Break();
        }

        [DebuggerStepThrough]
        public static void BreakIfFalse( this Boolean condition, String message = "" ) {
            if ( condition ) {
                return;
            }

            if ( !String.IsNullOrEmpty( message ) ) {
                Debug.WriteLine( message );
            }

            Break();
        }

        [DebuggerStepThrough]
        public static void BreakIfTrue( this Boolean condition, String message = "" ) {
            if ( !condition ) {
                return;
            }

            if ( !String.IsNullOrEmpty( message ) ) {
                Debug.WriteLine( message );
            }

            Break();
        }

        /// <summary>
        ///     <see cref="Exit" />.
        /// </summary>
        /// <param name="method"></param>
        [DebuggerStepThrough]
        public static void Enter( [CanBeNull] [CallerMemberName] String method = null ) {
            $"Entering {method ?? String.Empty}".Info();
            ConsoleListener.IndentLevel++;
        }

        /// <summary>
        ///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
        ///     <para>
        ///         See also: <see cref="Message" />, <see cref="Info" />, <see cref="Warning" />, and
        ///         <see cref="Error" />.
        ///     </para>
        /// </summary>
        /// <param name="message"></param>
        [DebuggerStepThrough]
        public static void Error( [CanBeNull] this String message ) {
            if ( message != null ) {
                $"Error: {message}".Info();
            }
        }

        /// <summary>
        ///     <see cref="Enter" />
        /// </summary>
        /// <param name="method"></param>
        [DebuggerStepThrough]
        public static void Exit( [CanBeNull] [CallerMemberName] String method = "" ) {
            ConsoleListener.IndentLevel--;
            $"Exited {method ?? String.Empty}".Info();
        }

        [DebuggerStepThrough]
        public static void Finalized( [CanBeNull] [CallerMemberName] String method = "" ) => $"Finalized: {method ?? String.Empty}".Info();

        /// <summary>
        ///     Gets the number of frames in the <see cref="StackTrace" />
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Int32 FrameCount<T>( this T _ ) => new StackTrace( false ).FrameCount;

        /// <summary>
        ///     Force a memory garbage collection on generation0 and generation1 objects.
        /// </summary>
        /// <seealso
        ///     cref="http://programmers.stackexchange.com/questions/276585/when-is-it-a-good-idea-to-force-garbage-collection" />
        [DebuggerStepThrough]
        public static void Garbage() {
            var before = GC.GetTotalMemory( forceFullCollection: false );
            GC.Collect( generation: 1, mode: GCCollectionMode.Forced, blocking: true );
            var after = GC.GetTotalMemory( forceFullCollection: false );

            if ( after < before ) {
                $"{before - after} bytes freed by the garbage collector.".Info();
            }
        }

        /// <summary>
        ///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
        ///     <para>
        ///         See also: <see cref="Message" />, <see cref="Info" />, <see cref="Warning" />, and
        ///         <see cref="Error" />.
        ///     </para>
        /// </summary>
        /// <param name="message"></param>
        [NotNull]
        [DebuggerStepThrough]
        public static String Info( [CanBeNull] this String message ) {
            $"Info: {message ?? String.Empty}".Info();

            return message ?? String.Empty;
        }

        /// <summary>
        ///     "Bring out yer' dead! Bring out yer' dead!"
        /// </summary>
        /// <param name="exception">       </param>
        /// <param name="method">          </param>
        /// <param name="sourceFilePath">  </param>
        /// <param name="sourceLineNumber"></param>
        /// <remarks>My catchall for exceptions I don't want to deal with, but where I still want to see the exception.</remarks>
        [NotNull]
        [DebuggerStepThrough]
        public static Exception Log( [NotNull] this Exception exception, [CanBeNull] [CallerMemberName] String method = "", [CanBeNull] [CallerFilePath] String sourceFilePath = "",
            [CallerLineNumber] Int32 sourceLineNumber = 0 ) {
            var message = new StringBuilder();
            message.Append( $" [Exception: {exception.Message}]\r\n" );
            message.Append( $" [In: {exception.Source}]\r\n" );
            message.Append( $" [When: {DateTime.Now}]\r\n" );
            message.Append( $" [Msg: {exception.Message}]\r\n" );
            message.Append( $" [Source: {sourceFilePath}]\r\n" );
            message.Append( $" [Line: {sourceLineNumber}]\r\n" );

            if ( method != null ) {
                ConsoleListener.Fail( method, message.ToString() );
            }

            Break();

            return exception;
        }

        /// <summary>
        ///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
        ///     <para>
        ///         See also: <see cref="Message" />, <see cref="Info" />, <see cref="Warning" />, and
        ///         <see cref="Error" />.
        ///     </para>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="method"> </param>
        [NotNull]
        [DebuggerStepThrough]
        public static String Message( [CanBeNull] this String message, [CallerMemberName] String method = "" ) {
            $"{method.NullIfEmpty() ?? "?"}: {message}".Info();

            return message ?? String.Empty;
        }

        [Obsolete( "Won't allocate a console in higher .NET frameworks." )]
        public static void Shutdown( Boolean linger = true ) {
            if ( !HasConsoleBeenAllocated ) {
                return;
            }

            "Shutting down".WriteColor( ConsoleColor.White, ConsoleColor.Blue );

            if ( linger ) {
                var stopwatch = Stopwatch.StartNew();

                while ( stopwatch.Elapsed < Seconds.One ) {
                    Thread.Sleep( Hertz.OneHundredTwenty );
                    ".".WriteColor( ConsoleColor.White, ConsoleColor.Blue );
                }

                stopwatch.Stop();
            }

            Kernel32.FreeConsole();
        }

        /*

        /// <summary>
        ///     <para>Allocate a Console.</para>
        ///     <para>Start <see cref="ProfileOptimization" />.</para>
        /// </summary>
        /// <returns></returns>
        [Obsolete( "Won't allocate a console in higher .NET frameworks." )]
        public static Boolean Startup() {
            try {
                HasConsoleBeenAllocated = NativeMethods.AllocConsole();

                Debug.Listeners.Add( ConsoleListener );

                Contract.ContractFailed += ( sender, e ) => {
                    var message = $"Caught Uncaught Contract Failure:\r\nCondition:{e.Condition}\r\nFailureKind:{e.FailureKind}\r\nHandled:{e.Handled}\r\nMessage:{e.Message}";
                    Debugger.IsAttached.BreakIfTrue( message );
                    e.OriginalException.Log();
                };

                ProfileOptimization.SetProfileRoot( Application.ExecutablePath );
                ProfileOptimization.StartProfile( Application.ExecutablePath );

                return HasConsoleBeenAllocated;
            }
            catch ( Exception exception ) { exception.Log(); }

            return default;
        }

        //

        /// <summary>
        ///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
        ///     <para>
        ///         See also: <see cref="Message" />, <see cref="Info" />, <see cref="Warning" />, and
        ///         <see cref="Error" />.
        ///     </para>
        /// </summary>
        /// <param name="message"></param>
        [DebuggerStepThrough]
        public static void Warning( this String message ) => $"Warning: {message}".Info();

        /// <summary>
        ///     <para>Write the <paramref name="message" /> out to the <see cref="ConsoleListener" />.</para>
        /// </summary>
        /// <param name="message"> </param>
        /// <param name="category"></param>
        [DebuggerStepThrough]
        public static void Write( this String message, [CanBeNull] String category = null ) => ConsoleListener.Write( message, category );

        /// <summary>
        ///     <para>Write the <paramref name="message" /> out to the <see cref="ConsoleListener" />.</para>
        /// </summary>
        /// <param name="message"> </param>
        /// <param name="category"></param>
        [DebuggerStepThrough]
        public static void WriteLine( this String message, [CanBeNull] String category = null ) => ConsoleListener.WriteLine( message, category );
    }
}
*/