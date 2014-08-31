namespace Librainian.Threading {
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public static class Report {

        /// <summary>
        ///     TODO add in the threadID
        /// </summary>
        /// <param name="method"></param>
        /// <param name="fullMethodPath"></param>
        [DebuggerStepThrough]
        public static void Enter( [CallerMemberName] String method = "", [Custom] String fullMethodPath = "" ) {
            //if ( String.IsNullOrWhiteSpace( method ) ) {
            //    return;
            //}
            Debug.Indent();
            String.Format( "{0}: {1}  {2}", "enter", method, fullMethodPath ).TimeDebug();
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