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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Log.cs" was last cleaned by Rick on 2014/12/06 at 10:45 AM

#endregion License & Information

namespace Librainian.Threading {

    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Runtime;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using Annotations;
    using Extensions;
    using Parsing;

    /// <summary>
    ///     A class to help with exception handling and plain ol' simple time+logging to the Console.
    /// </summary>
    public static class Log {
        private static readonly ConsoleListenerWithTimePrefix ConsoleListener;

        /// <summary>
        /// Assumes the first <see cref="SynchronizationContext"/> will be the same as the UI thread.
        /// </summary>
        public static readonly SynchronizationContext UIContext;

        static Log() {
            ConsoleListener = new ConsoleListenerWithTimePrefix();
            UIContext = SynchronizationContext.Current; //assumption.
        }

        public static Boolean HasConsoleBeenAllocated {
            get;
            private set;
        }

        public static Boolean Startup() {
            HasConsoleBeenAllocated = NativeWin32.AllocConsole();

            Debug.Listeners.Add( ConsoleListener );

            Contract.ContractFailed += ( sender, e ) => {
                var message = String.Format( "Caught Uncaught Contract Failure:\r\nCondition:{0}\r\nFailureKind:{1}\r\nHandled:{2}\r\nMessage:{3}", e.Condition, e.FailureKind, e.Handled, e.Message );
                Debugger.IsAttached.BreakIfTrue( message );
                e.OriginalException.More();
            };

            ProfileOptimization.SetProfileRoot( Application.ExecutablePath );
            ProfileOptimization.StartProfile( Application.ExecutablePath );

            return true;
        }

        public static void Shutdown() {
            if ( HasConsoleBeenAllocated ) {
                NativeWin32.FreeConsole();
            }
        }

        [DebuggerStepThrough]
        public static void Write( this String message ) => ConsoleListener.Write( message );

        /// <summary>
        ///     <para>Write the <paramref name="message" /> out to the console.</para>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="method"></param>
        [DebuggerStepThrough]
        public static void WriteLine( this String message, [CallerMemberName] String method = "" ) => ConsoleListener.WriteLine( String.Format( "({0}) {1}", method, message ) );

        [DebuggerStepThrough]
        public static void Enter( [CallerMemberName] String method = "" ) {
            ConsoleListener.IndentLevel++;
            String.Format( "Entering {0}", method ?? String.Empty ).WriteLine();
        }

        [DebuggerStepThrough]
        public static void Before( [CallerMemberName] String method = "" ) {
            ConsoleListener.IndentLevel++;
            String.Format( "Before - {0}", method ?? String.Empty ).WriteLine();
        }

        [DebuggerStepThrough]
        public static void After( [CallerMemberName] String method = "" ) {
            String.Format( "After - {0}", method ?? String.Empty ).WriteLine();
            ConsoleListener.IndentLevel--;
        }

        [DebuggerStepThrough]
        public static void Exit( [CallerMemberName] String method = "" ) {
            String.Format( "Exited {0}", method ?? String.Empty ).WriteLine();
            ConsoleListener.IndentLevel--;
        }

        [DebuggerStepThrough]
        public static void Message( String message, [CallerMemberName] String method = "" ) => String.Format( "{0}: {1}", method.NullIfEmpty() ?? "?", message ).WriteLine();

        [DebuggerStepThrough]
        public static void Info( String message ) => String.Format( "{0}", message ).WriteLine();

        [DebuggerStepThrough]
        public static void Finalized( [CallerMemberName] String method = "" ) => String.Format( "{0}: {1}", "Finalized", method ?? String.Empty ).WriteLine();

        /// <param name="exception"></param>
        /// <param name="method"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        [DebuggerStepThrough]
        public static void More( [NotNull] this Exception exception, [CanBeNull] [CallerMemberName] String method = "", [CanBeNull] [CallerFilePath] String sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0 ) {
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
            var message = new StringBuilder();
            message.AppendFormat( " [Exception: {0}]\r\n", exception.Message );
            message.AppendFormat( " [In: {0}]\r\n", exception.Source );
            message.AppendFormat( " [Msg: {0}]\r\n", exception.Message );
            message.AppendFormat( " [Source: {0}]\r\n", sourceFilePath );
            message.AppendFormat( " [Line: {0}]\r\n", sourceLineNumber );
            ConsoleListener.Fail( method, message.ToString() );
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
        ///     Gets the number of frames in the <see cref="StackTrace" />
        /// </summary>
        /// <param name="obj"> </param>
        /// <returns> </returns>
        // ReSharper disable once UnusedParameter.Global
        public static int FrameCount( this Object obj ) => ( new StackTrace( false ) ).FrameCount;

        /// <summary>
        ///     Force a memory garbage collection on generation0 and generation1 objects.
        /// </summary>
        public static void Garbage() {
            var before = GC.GetTotalMemory( forceFullCollection: false );
            GC.Collect( generation: 1, mode: GCCollectionMode.Optimized, blocking: false );
            var after = GC.GetTotalMemory( forceFullCollection: false );

            if ( after < before ) {
                Info( String.Format( "{0} bytes freed by the GC.", before - after ) );
            }
        }
    }
}