// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "SimpleHttpLogger.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "SimpleHttpLogger.cs" was last formatted by Protiguous on 2018/06/26 at 1:12 AM.

namespace Librainian.Internet.Servers {

	using System;

	/// <summary>
	///     A class which handles error logging by the http Server. It allows you to (optionally)
	///     register an ILogger instance to use for logging.
	/// </summary>
	public static class SimpleHttpLogger {

		private static ILogger _logger;

		private static Boolean _logVerbose;

		internal static void Log( Exception ex, String additionalInformation = "" ) {
			try {
				_logger?.Log( ex, additionalInformation );
			}
			catch ( Exception ) {

				// ignored
			}
		}

		internal static void Log( String str ) {
			try {
				_logger?.Log( str );
			}
			catch ( Exception ) {

				// ignored
			}
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
	}
}