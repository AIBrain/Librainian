// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Logging.cs" last touched on 2021-03-07 at 7:56 AM by Protiguous.

#nullable enable

namespace Librainian.Logging {

	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Threading;
	using JetBrains.Annotations;
	using Microsoft.Extensions.Logging;
	using Parsing;
	using Persistence;

	public interface IMessageWriter {
		void Write( String message );
		void WriteLine( String message, params Object[] args );
	}


	public class LoggingMessageWriter : IMessageWriter {
		private readonly ILogger<LoggingMessageWriter> _logger;

		public LoggingMessageWriter( ILogger<LoggingMessageWriter> logger ) => this._logger = logger;

		public void Write( String message ) => this._logger.LogInformation( message );

		public void WriteLine( String message, params Object[] args ) {
			this._logger.LogInformation( message, args );
		}

	}

	public static class Logging {

		//See Also: Microsoft.Extensions.Logging.Console

		//private static readonly ILogger<Type>? _logger;

		[DebuggerStepThrough]
		[Conditional( "DEBUG" )]
		public static void BreakIfDebug<T>( [CanBeNull] this T? _, [CanBeNull] String? breakReason = null ) {
			if ( breakReason is not null ) {
				$"Break reason: {breakReason}".DebugLine();
			}

			if ( Debugger.IsAttached ) {
				Debugger.Break();
			}
		}

		/// <summary>
		///     <para>Prints the <paramref name="breakReason" /></para>
		///     <para>Then calls <see cref="Debugger.Break" />.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="s"></param>
		/// <param name="breakReason"></param>
		[CanBeNull]
		[DebuggerStepThrough]
		public static String? Break<T>( [CanBeNull] this T s, [CanBeNull] String? breakReason = null ) {
			if ( !String.IsNullOrEmpty( breakReason ) ) {
				breakReason.DebugLine();
			}

			s.BreakIfDebug( breakReason );

			return breakReason;
		}

		[DebuggerStepThrough]
		public static void BreakIf( this Boolean condition, [CanBeNull] String? message = null ) {
			if ( condition ) {
				message.Break();
			}
		}

		[DebuggerStepThrough]
		public static void BreakIfTrue( this Boolean condition, [CanBeNull] String? message = null ) {
			if ( condition ) {
				message.Break();
			}
		}

		[DebuggerStepThrough]
		public static void BreakIfFalse( this Boolean condition, [CanBeNull] String? message = null ) {
			if ( !condition ) {
				message.Break();
			}
		}

		[DebuggerStepThrough]
		public static (Color fore, Color back) Colors( this LogLevel loggingLevel ) =>
			loggingLevel switch {
				LogLevel.Trace => (Color.Green, Color.White),
				LogLevel.Debug => (Color.DarkSeaGreen, Color.White),
				LogLevel.Information => (Color.Green, Color.White),
				LogLevel.Warning => (Color.Goldenrod, Color.White),
				LogLevel.Error => (Color.Red, Color.White),
				LogLevel.Critical => (Color.DarkRed, Color.Aqua),
				LogLevel.None => (Color.White, Color.DarkBlue),
				var _ => throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null )
			};

		/// <summary>Write line to <see cref="System.Diagnostics.Debug" />.</summary>
		/// <typeparam name="T"></typeparam>
		[DebuggerStepThrough]
		public static void DebugLine<T>( [CanBeNull] this T? self ) => Debug.WriteLine( self );

		/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
		/// <typeparam name="T"></typeparam>
		[DebuggerStepThrough]
		public static void DebugNoLine<T>( [CanBeNull] this T? self ) => Debug.Write( self );

		/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
		/// <typeparam name="T"></typeparam>
		[DebuggerStepThrough]
		public static void Error<T>( [CanBeNull] this T? self ) => self.DebugLine();

		/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
		/// <typeparam name="T"></typeparam>
		[DebuggerStepThrough]
		public static void Fatal<T>( [CanBeNull] this T? self ) => self.DebugLine();

		/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
		/// <typeparam name="T"></typeparam>
		[DebuggerStepThrough]
		public static void Info<T>( [CanBeNull] this T? self ) => self.DebugLine();

		[DebuggerStepThrough]
		[NotNull]
		public static String LevelName( this LogLevel loggingLevel ) =>
			loggingLevel switch {
				LogLevel.Trace => nameof( LogLevel.Trace ),
				LogLevel.Debug => nameof( LogLevel.Debug ),
				LogLevel.Information => nameof( LogLevel.Information ),
				LogLevel.Warning => nameof( LogLevel.Warning ),
				LogLevel.Error => nameof( LogLevel.Error ),
				LogLevel.Critical => nameof( LogLevel.Critical ),
				LogLevel.None => nameof( LogLevel.None ),
				var _ => throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null )
			};

		/// <summary>Prefix <paramref name="message" /> with the datetime and write out to the attached debugger and/or trace.</summary>
		/// <param name="message"></param>
		/// <param name="breakinto"></param>
		[Conditional( "DEBUG" )]
		[Conditional( "TRACE" )]
		[DebuggerStepThrough]
		public static void LogTimeMessage( [CanBeNull] this String? message, BreakOrDontBreak? breakinto = BreakOrDontBreak.DontBreak ) {
			$"[{DateTime.Now:t}] {message ?? Symbols.Null}".DebugLine();

			if ( breakinto == BreakOrDontBreak.Break && Debugger.IsAttached ) {
				Debugger.Break();
			}
		}

		[DebuggerStepThrough]
		[CanBeNull]
		public static Exception Log<T>( [CanBeNull] this T? message, BreakOrDontBreak? breakinto = null ) {
			if ( message is null ) {
				if ( breakinto == BreakOrDontBreak.Break && Debugger.IsAttached ) {
					Debugger.Break();
				}
			}
			else {
				if ( message is Exception exception ) {
					exception.ToStringDemystified().LogTimeMessage( breakinto );
					return exception;
				}

				message.ToString().LogTimeMessage( breakinto );
			}

			return new Exception( message?.ToString() );
		}

		/// <summary>
		///     Write
		///     <param name="self"></param>
		///     as JSON to debug.
		///     <para>Append <paramref name="more" /> if it has text.</para>
		/// </summary>
		/// <typeparam name="TT"></typeparam>
		/// <typeparam name="TM"></typeparam>
		/// <param name="self"></param>
		/// <param name="more"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[CanBeNull]
		public static TT? Log<TT, TM>( [CanBeNull] this TT? self, [CanBeNull] TM? more ) {
			var o = $"{self.ToJSON()}";
			o.DebugLine();

			if ( more is not null ) {
				$"Error={self.SmartQuote()}; {more.ToJSON()}".BreakIfDebug();
			}
			else {
				$"Error={self.SmartQuote()}".BreakIfDebug();
			}

			return self;
		}

		[Conditional( "VERBOSE" )]
		[DebuggerStepThrough]
		public static void Verbose( [NotNull] this String message ) => System.Diagnostics.Trace.WriteLine( message );

		/// <summary>
		///     Write a message to System.Diagnostics.Trace.
		///     <para>See also <see cref="TraceLine" />.</para>
		/// </summary>
		/// <param name="message"></param>
		[Conditional( "TRACE" )]
		[DebuggerStepThrough]
		public static void Trace( [NotNull] this String message ) => System.Diagnostics.Trace.Write( message );

		/// <summary>
		///     Write a message to System.Diagnostics.TraceLine.
		///     <para>See also <see cref="Trace" />.</para>
		/// </summary>
		/// <param name="message"></param>
		[Conditional( "TRACE" )]
		[DebuggerStepThrough]
		public static void TraceLine( [NotNull] this String message ) => System.Diagnostics.Trace.WriteLine( message );

		[Conditional( "DEBUG" )]
		[DebuggerStepThrough]
		public static void TimeDebug( [NotNull] this String message, Boolean newline = true, Boolean showThread = false ) {
			if ( newline ) {
				var m = showThread ? $"[{DateTime.UtcNow:s}].({Thread.CurrentThread.ManagedThreadId}) {message}" : $"[{DateTime.UtcNow:s}] {message}";
				Debug.WriteLine( m );
			}
			else {
				Debug.Write( message );
			}
		}

		[Conditional( "DEBUG" )]
		[DebuggerStepThrough]
		public static void TraceWithTime( [NotNull] this String message, Boolean newline = true, Boolean showThread = false ) {
			if ( newline ) {
				var m = showThread ? $"[{DateTime.UtcNow:s}].({Thread.CurrentThread.ManagedThreadId}) {message}" : $"[{DateTime.UtcNow:s}] {message}";
				m.TraceLine();
			}
			else {
				message.Trace();
			}
		}
	}

}