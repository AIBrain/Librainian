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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Logging.cs" last formatted on 2021-11-10 at 8:05 AM by Protiguous.

#nullable enable

namespace Librainian.Logging;

using System;
using System.Diagnostics;
using System.Drawing;
using Microsoft.Extensions.Logging;
using Parsing;

public static class Logging {

	//See Also: Microsoft.Extensions.Logging.Console

	//private static readonly ILogger<Type>? _logger;

	[DebuggerStepThrough]
	[Conditional( "DEBUG" )]
	public static void BreakIf( this Boolean condition, String? message = null ) => condition.BreakIfTrue( message );

	/// <summary>
	///     <para>Write to debug the first 50 chars of the object we broke on.</para>
	///     <para>Then write to debug the reason we broke on.</para>
	///     <para>Then Debugger.Break if a Debugger is attached.</para>
	///     <remarks>This method is Conditional on Debug.</remarks>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="brokeOn"></param>
	/// <param name="breakReason"></param>
	[DebuggerStepThrough]
	[Conditional( "DEBUG" )]
	public static void Break<T>( this T? brokeOn, String? breakReason = null ) {
		if ( brokeOn is not null ) {
			$"Break on object: {brokeOn.ToString().Left( 50 )}".DebugLine();
		}

		if ( breakReason is not null ) {
			$"Break reason: {breakReason}".LogTimeMessage();
			return;
		}

		if ( Debugger.IsAttached ) {
			Debugger.Break();
		}
	}

	[DebuggerStepThrough]
	public static void BreakIfFalse( this Boolean condition, String? message = null ) {
		if ( !condition ) {
			message.Break( $"{nameof( condition )} was false." );
		}
	}

	[DebuggerStepThrough]
	public static void BreakIfTrue( this Boolean condition, String? message = null ) {
		if ( condition ) {
			message.Break( $"{nameof( condition )} was true." );
		}
	}

	[DebuggerStepThrough]
	public static (Color fore, Color back) Colors( this LogLevel loggingLevel ) =>
		loggingLevel switch {
			LogLevel.Trace => ( Color.Green, Color.White ),
			LogLevel.Debug => ( Color.DarkSeaGreen, Color.White ),
			LogLevel.Information => ( Color.Black, Color.White ),
			LogLevel.Warning => ( Color.Goldenrod, Color.White ),
			LogLevel.Error => ( Color.Red, Color.White ),
			LogLevel.Critical => ( Color.DarkRed, Color.Aqua ),
			LogLevel.None => ( Color.White, Color.DarkBlue ),
			var _ => throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null )
		};

	/// <summary>Write line to <see cref="System.Diagnostics.Debug" />.</summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerStepThrough]
	public static void DebugLine<T>( this T? self ) => Debug.WriteLine( self );

	/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerStepThrough]
	public static void DebugNoLine<T>( this T? self ) => Debug.Write( self );

	/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerStepThrough]
	public static void Error<T>( this T? self ) => self.DebugLine();

	/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerStepThrough]
	public static void Fatal<T>( this T? self ) => self.DebugLine();

	/// <summary>Write to <see cref="System.Diagnostics.Debug" />.</summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerStepThrough]
	public static void Info<T>( this T? self ) => self.DebugLine();

	[DebuggerStepThrough]
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

	[DebuggerStepThrough]
	public static Exception Log<T>( this T? obj, BreakOrDontBreak? breakinto = null ) where T : Exception {
		if ( obj is Exception exception ) {
			exception.ToStringDemystified().LogTimeMessage( breakinto );
			return exception;
		}

		if ( breakinto == BreakOrDontBreak.Break && Debugger.IsAttached ) {
			Debugger.Break();
		}

		return new Exception( obj?.ToString() );
	}

	/*
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
	/// <param name="asJSON"></param>
	[DebuggerStepThrough]
	public static TT? Log<TT, TM>( this TT? self, TM? more, Boolean asJSON = false ) {
		var o = asJSON ? $"{self.ToJSON( Formatting.Indented )}" : self?.ToString();

		o.DebugLine();

		if ( more is null ) {
			$"Error={self.SmartQuote()}".BreakIfDebug();
		}
		else {
			$"Error={self.SmartQuote()}; {more.ToJSON( Formatting.Indented )}".BreakIfDebug();
		}

		return self;
	}
	*/

	/// <summary>Prefix <paramref name="message" /> with the datetime and write out to the attached debugger and/or trace.</summary>
	/// <param name="message"></param>
	/// <param name="breakinto"></param>
	[Conditional( "DEBUG" )]
	[Conditional( "TRACE" )]
	[DebuggerStepThrough]
	public static void LogTimeMessage( this String? message, BreakOrDontBreak? breakinto = BreakOrDontBreak.DontBreak ) {
		$"[{DateTime.Now:t}] {message ?? Symbols.Null}".DebugLine();

		if ( breakinto == BreakOrDontBreak.Break && Debugger.IsAttached ) {
			Debugger.Break();
		}
	}

	[Conditional( "DEBUG" )]
	[DebuggerStepThrough]
	public static void TimeDebug( this String message, Boolean newline = true, Boolean showThread = false ) {
		if ( newline ) {
			Debug.WriteLine( showThread ? $"[{DateTime.UtcNow:s}].({Environment.CurrentManagedThreadId}) {message}" : $"[{DateTime.UtcNow:s}] {message}" );
		}
		else {
			Debug.Write( message );
		}
	}

	/// <summary>
	///     Write a message to System.Diagnostics.Trace.
	///     <para>See also <see cref="TraceLine" />.</para>
	/// </summary>
	/// <param name="message"></param>
	[Conditional( "TRACE" )]
	[DebuggerStepThrough]
	public static void Trace( this String message ) => System.Diagnostics.Trace.Write( message );

	/// <summary>
	///     Write a message to System.Diagnostics.TraceLine.
	///     <para>See also <see cref="Trace" />.</para>
	/// </summary>
	/// <param name="message"></param>
	[Conditional( "TRACE" )]
	[DebuggerStepThrough]
	public static void TraceLine( this String message ) => System.Diagnostics.Trace.WriteLine( message );

	[Conditional( "TRACE" )]
	[DebuggerStepThrough]
	public static void TraceWithTime( this String message, Boolean newline = true, Boolean showThread = false ) {
		if ( newline ) {
			( showThread ? $"[{DateTime.UtcNow:s}].({Environment.CurrentManagedThreadId}) {message}" : $"[{DateTime.UtcNow:s}] {message}" ).TraceLine();
		}
		else {
			message.Trace();
		}
	}

	[Conditional( "VERBOSE" )]
	[DebuggerStepThrough]
	public static void Verbose( this String message ) => System.Diagnostics.Trace.WriteLine( message );

}