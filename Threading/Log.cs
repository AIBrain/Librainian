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
// "Librainian/Report.cs" was last cleaned by Rick on 2014/12/06 at 8:43 AM

#endregion License & Information

namespace Librainian.Threading {

    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Parsing;

    public static class Log {
        private static readonly ConsoleListenerWithTimePrefix ConsoleListener;

        static Log() {
            ConsoleListener = new ConsoleListenerWithTimePrefix();
            "ConsoleListener.Listener started".WriteLine();
        }

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