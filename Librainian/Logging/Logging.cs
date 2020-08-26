// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "Logging.cs" last formatted on 2020-08-14 at 8:35 PM.

#nullable enable

namespace Librainian.Logging {

	using System;
	using System.Diagnostics;
	using System.Drawing;
	using JetBrains.Annotations;
	using Parsing;
	using Persistence;

	public static class Logging {

		/// <summary>
		///     <para>Prints the <paramref name="message" /></para>
		///     <para>Then calls <see cref="Debugger.Break" />.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="s"></param>
		/// <param name="message"></param>
		[CanBeNull]
		[DebuggerStepThrough]
		public static String? Break<T>( [CanBeNull] this T s, [CanBeNull] String? message = null ) {
			if ( !String.IsNullOrEmpty( message ) ) {
				message.Debug();
			}

			s.BreakIfDebug();

			return message;
		}

		[DebuggerStepThrough]
		public static void BreakIfFalse( this Boolean condition, [CanBeNull] String? message = null ) {
			if ( !condition ) {
				Break( message );
			}
		}

		[DebuggerStepThrough]
		public static (Color fore, Color back) Colors( this LoggingLevel loggingLevel ) =>
			loggingLevel switch {
				LoggingLevel.Divine       => ( Color.Blue, Color.Aqua ),
				LoggingLevel.SubspaceTear => ( Color.HotPink, Color.Aqua ), //hotpink might actually look okay..
				LoggingLevel.Fatal        => ( Color.DarkRed, Color.Aqua ),
				LoggingLevel.Critical     => ( Color.Red, Color.Aqua ),
				LoggingLevel.Error        => ( Color.Red, Color.White ),
				LoggingLevel.Warning      => ( Color.Goldenrod, Color.White ),
				LoggingLevel.Diagnostic   => ( Color.Green, Color.White ),
				LoggingLevel.Debug        => ( Color.DarkSeaGreen, Color.White ),
				LoggingLevel.Exception    => ( Color.DarkOliveGreen, Color.AntiqueWhite ),
				_                         => throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null )
			};

		/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
		/// <typeparam name="T"></typeparam>
		[DebuggerStepThrough]
		public static void Debug<T>( [NotNull] this T self ) => System.Diagnostics.Debug.WriteLine( self );

		/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
		/// <typeparam name="T"></typeparam>
		[DebuggerStepThrough]
		public static void Error<T>( [NotNull] this T self ) => System.Diagnostics.Debug.WriteLine( self );

		/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
		/// <typeparam name="T"></typeparam>
		[DebuggerStepThrough]
		public static void Fatal<T>( [NotNull] this T self ) => System.Diagnostics.Debug.WriteLine( self );

		/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
		/// <typeparam name="T"></typeparam>
		[DebuggerStepThrough]
		public static void Info<T>( [NotNull] this T self ) => System.Diagnostics.Debug.WriteLine( self );

		[DebuggerStepThrough]
		[NotNull]
		public static String LevelName( this LoggingLevel loggingLevel ) =>
			loggingLevel switch {
				LoggingLevel.Diagnostic   => nameof( LoggingLevel.Diagnostic ),
				LoggingLevel.Debug        => nameof( LoggingLevel.Debug ),
				LoggingLevel.Warning      => nameof( LoggingLevel.Warning ),
				LoggingLevel.Error        => nameof( LoggingLevel.Error ),
				LoggingLevel.Exception    => nameof( LoggingLevel.Exception ),
				LoggingLevel.Critical     => nameof( LoggingLevel.Critical ),
				LoggingLevel.Fatal        => nameof( LoggingLevel.Fatal ),
				LoggingLevel.SubspaceTear => nameof( LoggingLevel.SubspaceTear ),
				LoggingLevel.Divine       => nameof( LoggingLevel.Divine ),
				_                         => throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null )
			};

		/// <summary>Prefix <paramref name="message" /> with the datetime and write out to the attached debugger and/or trace.</summary>
		/// <param name="message"></param>
		/// <param name="breakinto"></param>
		[Conditional( "DEBUG" )]
		[Conditional( "TRACE" )]
		[DebuggerStepThrough]
		public static void Log( [CanBeNull] this String? message, Boolean breakinto = false ) {
			$"[{DateTime.Now:t}] {message ?? Symbols.Null}".Debug();

			if ( breakinto && Debugger.IsAttached ) {
				Debugger.Break();
			}
		}

		/// <summary></summary>
		/// <param name="exception"></param>
		/// <param name="breakinto"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[NotNull]
		public static Exception Log( [NotNull] this Exception exception, Boolean? breakinto = default ) {
			if ( !breakinto.HasValue && Debugger.IsAttached ) {
				breakinto = true;
			}

			var log = exception.ToStringDemystified().Log( breakinto );

			return exception;
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
				message.ToString().Log( breakinto );
			}

			return message;
		}

		/// <summary>
		///     Write
		///     <param name="self"></param>
		///     as JSON to debug.
		///     <para>Append <paramref name="more" /> if it has text.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="M"></typeparam>
		/// <param name="self"></param>
		/// <param name="more"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[CanBeNull]
		public static T Log<T, M>( [CanBeNull] this T self, [CanBeNull] M more ) {
			//Console.Beep( 14000, 100 );
			var o = $"{self.ToJSON()}";

			if ( more is null ) {
				o.Debug();

				if ( Debugger.IsAttached ) {
					System.Diagnostics.Debug.WriteLine( $"Error={self.DoubleQuote()}" );
					Debugger.Break();
				}
			}
			else {
				var m = more.ToJSON();
				$"{o}; {m}".Debug();

				if ( Debugger.IsAttached ) {
					System.Diagnostics.Debug.WriteLine( $"Error={self.DoubleQuote()}; {m}" );
					Debugger.Break();
				}
			}

			return self;
		}

		[Conditional( "VERBOSE" )]
		[DebuggerStepThrough]
		public static void Verbose( [NotNull] this String message ) => System.Diagnostics.Trace.WriteLine( message );

		[Conditional( "VERBOSE" )]
		[DebuggerStepThrough]
		public static void Trace( [NotNull] this String message ) => System.Diagnostics.Trace.WriteLine( message );

	}

}