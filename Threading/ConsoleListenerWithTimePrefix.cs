// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ConsoleListenerWithTimePrefix.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/ConsoleListenerWithTimePrefix.cs" was last cleaned by Protiguous on 2018/05/15 at 10:50 PM.

namespace Librainian.Threading {

    using System;
    using System.Diagnostics;

    public class ConsoleListenerWithTimePrefix : ConsoleTraceListener {

        public ConsoleListenerWithTimePrefix() : base( useErrorStream: true ) { }

        //TODO  http://msdn.microsoft.com/en-us/Library/system.diagnostics.consoletracelistener(v=vs.110).aspx

        /// <summary>
        ///     Gets a value indicating whether the trace listener is thread safe.
        /// </summary>
        /// <returns>true if the trace listener is thread safe; otherwise, false. The default is false.</returns>
        public override Boolean IsThreadSafe => true;

        /// <summary>
        ///     Emits an error message and a detailed error message to the listener you create when you implement the
        ///     <see cref="T:System.Diagnostics.TraceListener" /> class.
        /// </summary>
        /// <param name="message">      A message to emit.</param>
        /// <param name="detailMessage">A detailed message to emit.</param>
        public override void Fail( String message, String detailMessage ) {
            base.Fail( message, detailMessage );
            this.Flush();
        }

        /// <summary>
        ///     Writes a message to this instance's <see cref="P:System.Diagnostics.TextWriterTraceListener.Writer" />.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DebuggerStepThrough]
        public override void Write( String message ) {
            Console.Write( message );
            this.Flush();
        }

        /// <summary>
        ///     Writes a message to this instance's <see cref="P:System.Diagnostics.TextWriterTraceListener.Writer" /> followed by
        ///     a line terminator. The default line terminator is a carriage return followed by a line feed (\r\n).
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DebuggerStepThrough]
        public override void WriteLine( String message ) {
            Console.WriteLine( message );
            this.Flush();
        }

        //private static String HeaderTimeThread() => $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} ({Thread.CurrentThread.ManagedThreadId})] ";
    }
}