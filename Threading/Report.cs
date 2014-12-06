namespace Librainian.Threading {

    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Annotations;
    using Parsing;

    public static class Log {
        private static readonly ConsoleListenerWithTimePrefix ConsoleListener;

        [DebuggerStepThrough]
        public static void Write( this String message ) {
            ConsoleListener.Write( message );
        }

        [DebuggerStepThrough]
        public static void WriteLine( this String message, [CallerMemberName] String method = "" ) {
            ConsoleListener.WriteLine( String.Format( "({0}) {1}", method, message ) );
        }

        [DebuggerStepThrough]
        public static void Catch( this Exception exception, [CallerMemberName] String method = "" ) {
            ConsoleListener.Fail( method, exception.Message );
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }


        static Log() {
            ConsoleListener = new ConsoleListenerWithTimePrefix();
            "ConsoleListener.Listener started".WriteLine();
        }

        [DebuggerStepThrough]
        public static void Enter( [CallerMemberName] String method = "" ) {
            ConsoleListener.IndentLevel++;
            String.Format( "Entering {0}", method ?? String.Empty ).WriteLine();
        }

        [DebuggerStepThrough]
        public static void Before( [CallerMemberName] String method = "" ) {
            ConsoleListener.IndentLevel++;
            String.Format( "Before {0}", method ?? String.Empty ).WriteLine();
        }

        [DebuggerStepThrough]
        public static void After( [CallerMemberName] String method = "" ) {
            String.Format( "After {0}", method ?? String.Empty ).WriteLine();
            ConsoleListener.IndentLevel--;
        }

        [DebuggerStepThrough]
        public static void Exit( [CallerMemberName] String method = "" ) {
            String.Format( "Exited {0}", method ?? String.Empty ).WriteLine();
            ConsoleListener.IndentLevel--;
        }

      

        [DebuggerStepThrough]
        public static void Message( String message, [CallerMemberName] String method = "" ) {
            String.Format( "{0}: {1}", method.NullIfEmpty() ?? "?", message ).WriteLine();
        }

        [DebuggerStepThrough]
        public static void Info( String message ) {
            String.Format( "{0}:{1}", Thread.CurrentThread.ManagedThreadId, message ).WriteLine();
        }

        [DebuggerStepThrough]
        public static void Finalized( [CallerMemberName] String method = "" ) {
            String.Format( "{0}: {1}", "Finalized", method ?? String.Empty ).WriteLine();
        }
    }

}