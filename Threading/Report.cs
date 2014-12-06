namespace Librainian.Threading {

    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Annotations;
    using Parsing;

    public class ConsoleListenerWithTimePrefix : ConsoleTraceListener {
        //TODO  http://msdn.microsoft.com/en-us/library/system.diagnostics.consoletracelistener(v=vs.110).aspx

        private static String HeaderTimeThread() {
            return String.Format( "[{0:yyyy-MM-dd HH:mm:ss} ({1})] ", DateTime.Now, Thread.CurrentThread.ManagedThreadId );
        }

        /// <summary>
        /// Emits an error message and a detailed error message to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"/> class.
        /// </summary>
        /// <param name="message">A message to emit. </param><param name="detailMessage">A detailed message to emit. </param>
        public override void Fail( String message, String detailMessage ) {
            base.Fail( message, detailMessage );
            Flush();
        }

        /// <summary>
        /// Gets a value indicating whether the trace listener is thread safe. 
        /// </summary>
        /// <returns>
        /// true if the trace listener is thread safe; otherwise, false. The default is false.
        /// </returns>
        public override bool IsThreadSafe {
            get {
                return true;
            }
        }

        /// <summary>
        /// Writes a message to this instance's <see cref="P:System.Diagnostics.TextWriterTraceListener.Writer"/>.
        /// </summary>
        /// <param name="message">A message to write. </param><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public override void Write( string message ) {
            base.Write( message );
            Flush();
        }

        /// <summary>
        /// Writes a message to this instance's <see cref="P:System.Diagnostics.TextWriterTraceListener.Writer"/> followed by a line terminator. The default line terminator is a carriage return followed by a line feed (\r\n).
        /// </summary>
        /// <param name="message">A message to write. </param><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public override void WriteLine( String message ) {
            Write( HeaderTimeThread() );
            base.WriteLine( message );
            Flush();
        }
    }

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

        static Log() {
            ConsoleListener = new ConsoleListenerWithTimePrefix();
            "ConsoleListener.Listener started".WriteLine();
        }
    }

    public static class Report {

        /// <summary>
        ///     TODO add in the threadID
        /// </summary>
        /// <param name="method"></param>
        /// <param name="fullMethodPath"></param>
        [DebuggerStepThrough]
        public static void Enter( [CallerMemberName] String method = "", [Custom] String fullMethodPath = "" ) {
            Debug.Indent();
            String.Format( "{0} {1} {2}", "enter", method ?? String.Empty, fullMethodPath ?? String.Empty ).WriteLine();
        }

        /// <summary>
        ///     TODO add in the threadID
        /// </summary>
        /// <param name="method"></param>
        [DebuggerStepThrough]
        public static void Exit( [CallerMemberName] String method = "" ) {
            String.Format( "{0} {1}", "exit", method ?? String.Empty ).WriteLine();
            Debug.Unindent();
        }

        //[DebuggerStepThrough]
        public static void Before( String message, [CallerMemberName] String method = "" ) {
            message = String.Format( "[{0} {1}.{2}.{3}] {4}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), Thread.CurrentThread.ManagedThreadId, method.NullIfEmpty() ?? "?", message );
#if DEBUG
            Debug.Write( message );
#endif
#if TRACE
            Trace.Write( message );
#endif
        }

        [DebuggerStepThrough]
        public static void After( String message, [CallerMemberName] String method = "" ) {
#if DEBUG
            Debug.WriteLine( message );
#endif
#if TRACE
            Trace.WriteLine( message );
#endif
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