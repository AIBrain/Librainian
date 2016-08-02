// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/SimpleHttpLogger.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Internet.Servers {

    using System;

    /// <summary>
    ///     A class which handles error logging by the http Server. It allows you to (optionally)
    ///     register an ILogger instance to use for logging.
    /// </summary>
    public static class SimpleHttpLogger {
        private static ILogger _logger;
        private static Boolean _logVerbose;

        /// <summary>
        ///     (OPTIONAL) Keeps a static reference to the specified ILogger and uses it for http Server
        ///     error logging. Only one logger can be registered at a time; attempting to register a
        ///     second logger simply replaces the first one.
        /// </summary>
        /// <param name="loggerToRegister">
        ///     The logger that should be used when an error message needs logged. If null, logging will
        ///     be disabled.
        /// </param>
        /// <param name="logVerboseMessages">
        ///     If true, additional error reporting will be enabled. These errors include things that
        ///     can occur frequently during normal operation, so it may be spammy.
        /// </param>
        public static void RegisterLogger( ILogger loggerToRegister, Boolean logVerboseMessages = false ) {
            _logger = loggerToRegister;
            _logVerbose = logVerboseMessages;
        }

        /// <summary>Unregisters the currently registered logger (if any) by calling RegisterLogger(null);</summary>
        public static void UnregisterLogger() => RegisterLogger( null );

        internal static void Log( Exception ex, String additionalInformation = "" ) {
            try {
                if ( _logger != null ) {
                    _logger.Log( ex, additionalInformation );
                }
            }
            catch ( Exception ) { }
        }

        internal static void Log( String str ) {
            try {
                if ( _logger != null ) {
                    _logger.Log( str );
                }
            }
            catch ( Exception ) { }
        }

        internal static void LogVerbose( Exception ex, String additionalInformation = "" ) {
            if ( _logVerbose ) {
                Log( ex, additionalInformation );
            }
        }

        internal static void LogVerbose( String str ) {
            if ( _logVerbose ) {
                Log( str );
            }
        }
    }
}