// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "App.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "App.cs" was last formatted by Protiguous on 2020/03/16 at 3:13 PM.

namespace Librainian {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime;
    using System.Threading;
    using Collections.Extensions;
    using CommandLine;
    using JetBrains.Annotations;
    using Logging;
    using NLog;
    using NLog.Common;
    using NLog.Config;
    using Error = CommandLine.Error;

    public static class App {

        /// <summary>
        ///     <para>Adds program-wide exception handlers.</para>
        ///     <para>Optimizes program startup.</para>
        ///     <para>Starts logging, Debug and Trace.</para>
        ///     <para>Performs a garbage cleanup.</para>
        ///     <para>And starts the form thread-loop.</para>
        /// </summary>
        public static void Run<TOpts>( [NotNull] Action<TOpts> runParsedOptions, String[] arguments ) where TOpts : IOptions {

            if ( runParsedOptions is null ) {
                throw new ArgumentNullException( paramName: nameof( runParsedOptions ) );
            }

            if ( arguments is null ) {
                throw new ArgumentNullException( paramName: nameof( arguments ) );
            }

            try {
                RunInternalCommon();

                Parser.Default?.ParseArguments<TOpts>( args: arguments ).WithParsed( action: runParsedOptions ).WithNotParsed( action: HandleErrors );
            }
            catch ( Exception exception ) {
                exception.Log( breakinto: true );
            }

            static void HandleErrors( IEnumerable<Error> errors ) {
                try {
                    if ( errors is null ) {
                        if ( Debugger.IsAttached ) {
                            Debug.WriteLine( message: "Unknown error." );
                            Debugger.Break();
                        }
                    }
                    else {
                        var message = errors.Select( selector: error => error?.ToString() ).ToStrings( separator: Environment.NewLine );

                        if ( Debugger.IsAttached ) {
                            Debug.WriteLine( message: message );
                            Debugger.Break();
                        }
                        else {
                            Console.WriteLine( value: $"Error parsing command line options.{Environment.NewLine}{message}" );
                        }
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }
        }

        private static void RunInternalCommon() {
            AppDomain.CurrentDomain.UnhandledException += ( sender, e ) => ( e?.ExceptionObject as Exception )?.Log( breakinto: true );

            Debug.AutoFlush = true;
            Trace.AutoFlush = true;

            InternalLogger.Reset();

            if ( LogManager.Configuration is null ) {
                LogManager.Configuration = new LoggingConfiguration();
                InternalLogger.Trace( message: $"{nameof( Run )} created a new {nameof( LoggingConfiguration )}." );
            }

            Debug.Assert( condition: LogLevel.Trace != null, message: "LogLevel.Trace != null" );
            Debug.Assert( condition: LogLevel.Fatal != null, message: "LogLevel.Fatal != null" );
            Logging.Logging.Setup( minLogLevel: LogLevel.Trace, maxLogLevel: LogLevel.Fatal, target: SomeTargets.TraceTarget.Value );

#if DEBUG
            LogManager.ThrowConfigExceptions = true;
            LogManager.ThrowExceptions = true;
#endif

            Compact();
            Thread.Yield();
            Compact();

            static void Compact() {
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect( generation: 2, mode: GCCollectionMode.Forced, blocking: true, compacting: true );
            }
        }

    }

}