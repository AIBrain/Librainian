// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Logging.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", "Logging.cs" was last formatted by Protiguous on 2019/11/25 at 4:08 PM.

namespace LibrainianCore.Logging {

    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Drawing;
    using Extensions;
    using JetBrains.Annotations;
    using NLog;
    using NLog.Targets;
    using Parsing;
    using Persistence;

    public static class Logging {

        [System.Diagnostics.CodeAnalysis.NotNull]
        public static Logger Logger { get; } = LogManager.GetCurrentClassLogger() ?? throw new InvalidOperationException( $"{nameof( Logger )} is invalid!" );

        [System.Diagnostics.CodeAnalysis.NotNull]
        public static ConcurrentDictionary<Control, Target> Targets { get; } = new ConcurrentDictionary<Control, Target>();

        /// <summary>
        ///     <para>Prints the <paramref name="message" /></para>
        ///     <para>Then calls <see cref="Debugger.Break" />.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="message"></param>
        [CanBeNull]
        [DebuggerStepThrough]
        public static String Break<T>( [CanBeNull] this T s, [CanBeNull] String message = null ) {
            if ( !String.IsNullOrEmpty( message ) ) {
                message.Debug();
            }

            s.BreakIfDebug();

            return message;
        }

        [DebuggerStepThrough]
        public static void BreakIfFalse( this Boolean condition, [CanBeNull] String message = null ) {
            if ( !condition ) {
                Break( message );
            }
        }

        [DebuggerStepThrough]
        public static (Color fore, Color back) Colors( this LoggingLevel loggingLevel ) {

            switch ( loggingLevel ) {
                case LoggingLevel.Divine: {
                    return ( Color.Blue, Color.Aqua );
                }

                case LoggingLevel.SubspaceTear: {
                    return ( Color.HotPink, Color.Aqua ); //hotpink might actually look okay..
                }

                case LoggingLevel.Fatal: {

                    return ( Color.DarkRed, Color.Aqua );
                }

                case LoggingLevel.Critical: {

                    return ( Color.Red, Color.Aqua );
                }

                case LoggingLevel.Error: {

                    return ( Color.Red, Color.White );
                }

                case LoggingLevel.Warning: {

                    return ( Color.Goldenrod, Color.White );
                }

                case LoggingLevel.Diagnostic: {

                    return ( Color.Green, Color.White );
                }

                case LoggingLevel.Debug: {

                    return ( Color.DarkSeaGreen, Color.White );
                }

                case LoggingLevel.Exception: {

                    return ( Color.DarkOliveGreen, Color.AntiqueWhite );
                }

                default: throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null );
            }
        }

        /// <summary>Write <paramref name="self" /> to the <see cref="Logger" />.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        [DebuggerStepThrough]
        public static void Debug<T>( [CanBeNull] this T self ) => Logger.Debug( self );

        [DebuggerStepThrough]
        public static void Error<T>( [CanBeNull] this T self ) => Logger.Error( self );

        [DebuggerStepThrough]
        public static void Fatal<T>( [CanBeNull] this T self ) => Logger.Fatal( self );

        [DebuggerStepThrough]
        public static void Info<T>( [CanBeNull] this T self ) => Logger.Info( self );

        [DebuggerStepThrough]
        public static void Info( [CanBeNull] this String message ) => Logger.Info( message );

        [DebuggerStepThrough]
        [System.Diagnostics.CodeAnalysis.NotNull]
        public static String LevelName( this LoggingLevel loggingLevel ) {
            switch ( loggingLevel ) {
                case LoggingLevel.Diagnostic: return nameof( LoggingLevel.Diagnostic );
                case LoggingLevel.Debug: return nameof( LoggingLevel.Debug );
                case LoggingLevel.Warning: return nameof( LoggingLevel.Warning );
                case LoggingLevel.Error: return nameof( LoggingLevel.Error );
                case LoggingLevel.Exception: return nameof( LoggingLevel.Exception );
                case LoggingLevel.Critical: return nameof( LoggingLevel.Critical );
                case LoggingLevel.Fatal: return nameof( LoggingLevel.Fatal );
                case LoggingLevel.SubspaceTear: return nameof( LoggingLevel.SubspaceTear );
                case LoggingLevel.Divine: return nameof( LoggingLevel.Divine );
                default: throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null );
            }
        }

        /// <summary>Prefix <paramref name="message" /> with the datetime and write out to the attached debugger and/or trace <see cref="Logger" />.</summary>
        /// <param name="message"></param>
        /// <param name="breakinto"></param>
        [Conditional( "DEBUG" )]
        [Conditional( "TRACE" )]
        [DebuggerStepThrough]
        public static void Log( this String message, Boolean breakinto = false ) {
            message = $"[{DateTime.Now:t}] {message ?? Symbols.Null}";

            if ( Debugger.IsAttached ) {
                System.Diagnostics.Debug.WriteLine( message );
            }

            if ( Logger.IsTraceEnabled ) {
                Logger.Trace( message );
            }
            else if ( Logger.IsDebugEnabled ) {
                Logger.Debug( message );
            }

            if ( breakinto && Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        /// <summary></summary>
        /// <param name="exception"></param>
        /// <param name="breakinto"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [System.Diagnostics.CodeAnalysis.NotNull]
        public static Exception Log( [CanBeNull] this Exception exception, Boolean breakinto = false ) {
            if ( exception is null ) {
                $"Null {nameof( Exception )}".Log( breakinto: true );
            }

            exception.ToStringDemystified().Log( breakinto: breakinto );

            return exception ?? new NullReferenceException( $"Null {nameof( Exception )}" );
        }

        [DebuggerStepThrough]
        [CanBeNull]
        public static T Log<T>( [CanBeNull] this T message, Boolean breakinto ) {
            if ( message is null ) {
                if ( breakinto && Debugger.IsAttached ) {
                    Debugger.Break();
                }
            }
            else {
                message.ToString().Log( breakinto: breakinto );
            }

            return message;
        }

        /// <summary>Write
        /// <param name="self"></param>
        /// as JSON to the <see cref="Logger" />.
        /// <para>Append <paramref name="more" /> if it has text.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <param name="self"></param>
        /// <param name="more"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [CanBeNull]
        public static T Log<T, M>( [CanBeNull] this T self, [CanBeNull] M more ) {
            Console.Beep( 14000, 100 );
            var o = $"{self.ToJSON()}";

            if ( more is null ) {
                Logger.Debug( o );

                if ( Debugger.IsAttached ) {
                    System.Diagnostics.Debug.WriteLine( $"Error={self.DoubleQuote()}" );
                    Debugger.Break();
                }
            }
            else {
                var m = more.ToJSON();
                Logger.Debug( $"{o}; {m}" );

                if ( Debugger.IsAttached ) {
                    System.Diagnostics.Debug.WriteLine( $"Error={self.DoubleQuote()}; {m}" );
                    Debugger.Break();
                }
            }

            return self;
        }

        /// <summary>Add a <see cref="Target" /> or a <see cref="RichTextBox" />.</summary>
        /// <param name="minLogLevel"></param>
        /// <param name="maxLogLevel"></param>
        /// <param name="target">     </param>
        /// <param name="rtb">        </param>
        [DebuggerStepThrough]
        public static Boolean Setup( [System.Diagnostics.CodeAnalysis.NotNull] LogLevel minLogLevel, [System.Diagnostics.CodeAnalysis.NotNull] LogLevel maxLogLevel, [CanBeNull] Target target = null, [CanBeNull] RichTextBox rtb = null ) {
            if ( minLogLevel is null ) {
                throw new ArgumentNullException( paramName: nameof( minLogLevel ) );
            }

            if ( maxLogLevel is null ) {
                throw new ArgumentNullException( paramName: nameof( maxLogLevel ) );
            }

            switch ( target ) {
                case null when rtb is null:
                    "No target or control specified to setup!".Break();

                    return false;

                case null:
                    target = rtb.ToTarget();

                    break;
            }

            if ( target is null ) {
                $"Unable to set up target for {minLogLevel} to {maxLogLevel}".Break();

                return false;
            }

            LogManager.Configuration?.AddTarget( target );
            LogManager.Configuration?.AddRule( minLevel: minLogLevel, maxLevel: maxLogLevel, target: target, loggerNamePattern: "*" );

            return LogManager.Configuration?.AllTargets?.Contains( target ) == true;
        }


        [DebuggerStepThrough]
        public static void Trace( [CanBeNull] this String message ) => Logger.Trace( message );

        [DebuggerStepThrough]
        public static void Trace( [CanBeNull] this Exception exception ) => Logger.Trace( exception );

        [DebuggerStepThrough]
        public static void Trace<T>( [CanBeNull] this T message ) => Logger.Trace( message );

        [Conditional( "VERBOSE" )]
        [DebuggerStepThrough]
        public static void Verbose( [CanBeNull] this String message ) {
            if ( Logger.IsTraceEnabled ) {
                Logger.Trace( message );
            } else if ( Logger.IsDebugEnabled ) {
                Logger.Debug( message );
            }
        }

        [DebuggerStepThrough]
        public static void Warn( [CanBeNull] this String message ) => Logger.Warn( message );

        [DebuggerStepThrough]
        public static void Warn( [CanBeNull] this Exception exception ) => Logger.Warn( exception );

        [DebuggerStepThrough]
        public static void Warn<T>( [CanBeNull] this T message ) => Logger.Warn( message );

    }

}