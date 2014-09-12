namespace Librainian.Threading {

    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Annotations;
    using Parsing;

    public static class Report {

        /// <summary>
        ///     TODO add in the threadID
        /// </summary>
        /// <param name="method"></param>
        /// <param name="fullMethodPath"></param>
        [DebuggerStepThrough]
        public static void Enter( [CallerMemberName] String method = "", [Custom] String fullMethodPath = "" ) {
            Debug.Indent();
            String.Format( "{0}: {1} {2}", "enter", method ?? String.Empty, fullMethodPath ?? String.Empty ).TimeDebug();
        }

        /// <summary>
        ///     TODO add in the threadID
        /// </summary>
        /// <param name="method"></param>
        [DebuggerStepThrough]
        public static void Exit( [CallerMemberName] String method = "" ) {
            String.Format( "{0}: {1}", "exit", method ?? String.Empty ).TimeDebug();
            Debug.Unindent();
        }

        [DebuggerStepThrough]
        public static void Info( String message, [CallerMemberName] String method = "" ) {
            String.Format( "{0}: {1}", method.NullIfEmpty() ?? "?", message ).TimeDebug();
        }

        [DebuggerStepThrough]
        public static void Message( String message, [CallerMemberName] String method = "" ) {
            String.Format( "{0}: {1}", method.NullIfEmpty() ?? "?", message ).TimeDebug();
        }

        [DebuggerStepThrough]
        public static void TimeDebug( [CanBeNull] this String message, Boolean newline = true ) {
            if ( message == null ) {
                return;
            }
#if DEBUG
            if ( newline ) {
                Debug.WriteLine( "[{0:s}].({1}) {2}", DateTime.Now, Thread.CurrentThread.ManagedThreadId, message );
            }
            else {
                Debug.Write( String.Format( "[] {0}", message ) );
            }
#endif
#if TRACE
            if ( newline ) {
                Trace.WriteLine( "[{0:s}].({1}) {2}", DateTime.Now, Thread.CurrentThread.ManagedThreadId, message );
            }
            else {
                Trace.Write( String.Format( "[] {0}", message ) );
            }
#endif
        }
    }
}