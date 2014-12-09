namespace Librainian.Threading {
    using System;
    using System.Diagnostics;
    using System.Threading;

    public class ConsoleListenerWithTimePrefix : ConsoleTraceListener {
        //TODO  http://msdn.microsoft.com/en-us/library/system.diagnostics.consoletracelistener(v=vs.110).aspx

        public ConsoleListenerWithTimePrefix() : base( true ) {
            
        }

        private static String HeaderTimeThread() => String.Format( "[{0:yyyy-MM-dd HH:mm:ss} ({1})] ", DateTime.Now, Thread.CurrentThread.ManagedThreadId );

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
        public override bool IsThreadSafe => true;

        /// <summary>
        /// Writes a message to this instance's <see cref="P:System.Diagnostics.TextWriterTraceListener.Writer"/>.
        /// </summary>
        /// <param name="message">A message to write. </param><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public override void Write( string message ) {
            Console.Write( message );
            Flush();
        }

        /// <summary>
        /// Writes a message to this instance's <see cref="P:System.Diagnostics.TextWriterTraceListener.Writer"/> followed by a line terminator. The default line terminator is a carriage return followed by a line feed (\r\n).
        /// </summary>
        /// <param name="message">A message to write. </param><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public override void WriteLine( String message ) {
            Write( HeaderTimeThread() );
            Console.WriteLine( message );
            Flush();
        }
    }
}