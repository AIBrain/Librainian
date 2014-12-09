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
// "Librainian/SimpleHttpLogger.cs" was last cleaned by Rick on 2014/09/08 at 3:51 AM

#endregion License & Information

namespace Librainian.Internet.Servers {

    using System;

    /// <summary>
    ///     A class which handles error logging by the http server.  It allows you to (optionally) register an ILogger instance
    ///     to use for logging.
    /// </summary>
    public static class SimpleHttpLogger {
        private static ILogger logger;
        private static bool logVerbose;

        /// <summary>
        ///     (OPTIONAL) Keeps a static reference to the specified ILogger and uses it for http server error logging.  Only one
        ///     logger can be registered at a time; attempting to register a second logger simply replaces the first one.
        /// </summary>
        /// <param name="loggerToRegister">
        ///     The logger that should be used when an error message needs logged.  If null, logging
        ///     will be disabled.
        /// </param>
        /// <param name="logVerboseMessages">
        ///     If true, additional error reporting will be enabled.  These errors include things that
        ///     can occur frequently during normal operation, so it may be spammy.
        /// </param>
        public static void RegisterLogger( ILogger loggerToRegister, bool logVerboseMessages = false ) {
            logger = loggerToRegister;
            logVerbose = logVerboseMessages;
        }

        /// <summary>
        ///     Unregisters the currently registered logger (if any) by calling RegisterLogger(null);
        /// </summary>
        public static void UnregisterLogger() => RegisterLogger( null );

        internal static void Log( Exception ex, String additionalInformation = "" ) {
            try {
                if ( logger != null ) {
                    logger.Log( ex, additionalInformation );
                }
            }
            catch ( Exception ) {
            }
        }

        internal static void Log( String str ) {
            try {
                if ( logger != null ) {
                    logger.Log( str );
                }
            }
            catch ( Exception ) {
            }
        }

        internal static void LogVerbose( Exception ex, String additionalInformation = "" ) {
            if ( logVerbose ) {
                Log( ex, additionalInformation );
            }
        }

        internal static void LogVerbose( String str ) {
            if ( logVerbose ) {
                Log( str );
            }
        }
    }
}