// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Logging.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Logging.cs" was last formatted by Protiguous on 2018/06/04 at 4:02 PM.

namespace Librainian {

	using System;
	using System.Diagnostics;
	using System.Diagnostics.Contracts;
	using System.Runtime;
	using System.Runtime.CompilerServices;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;
	using Extensions;
	using JetBrains.Annotations;
	using Measurement.Frequency;
	using Measurement.Time;
	using OperatingSystem;
	using Parsing;
	using Threading;

	/// <summary>
	///     A class to help with exception handling and plain ol' simple time+logging to the Console.
	///     <para>I feel like this is a rereinvented wheel..</para>
	///     UPDATE: Can we use the NLOG nuget pagkage? It looks clean..
	/// </summary>
	public static class Logging {

		public static Boolean HasConsoleBeenAllocated { get; private set; }

		/// <summary>
		///     <seealso cref="Before" />
		/// </summary>
		/// <param name="method"></param>
		[DebuggerStepThrough]
		public static void After( [CallerMemberName] String method = null ) {
			$"After - {method ?? String.Empty}".WriteLine();
			ConsoleListener.IndentLevel--;
		}

		/// <summary>
		///     <seealso cref="After" />
		/// </summary>
		/// <param name="method"></param>
		[DebuggerStepThrough]
		public static void Before( [CallerMemberName] String method = null ) {
			ConsoleListener.IndentLevel++;
			$"Before - {method ?? String.Empty}".WriteLine();
		}

		[DebuggerStepThrough]
		public static void Break( this Exception exception ) {
			if ( !String.IsNullOrEmpty( exception.Message ) ) { Debug.WriteLine( exception.ToString() ); }

			Break();
		}

		/// <summary>
		///     <see cref="Debugger.Break" /> if a <see cref="Debugger" /> is attached.
		/// </summary>
		public static void Break() {
			if ( Debugger.IsAttached ) { Debugger.Break(); }
		}

		[DebuggerStepThrough]
		public static void BreakIfFalse( this Boolean condition, String message = "" ) {
			if ( condition ) { return; }

			if ( !String.IsNullOrEmpty( message ) ) { Debug.WriteLine( message ); }

			Break();
		}

		[DebuggerStepThrough]
		public static void BreakIfTrue( this Boolean condition, String message = "" ) {
			if ( !condition ) { return; }

			if ( !String.IsNullOrEmpty( message ) ) { Debug.WriteLine( message ); }

			Break();
		}

		/// <summary>
		///     <seealso cref="Exit" />.
		/// </summary>
		/// <param name="method"></param>
		[DebuggerStepThrough]
		public static void Enter( [CallerMemberName] String method = null ) {
			$"Entering {method ?? String.Empty}".WriteLine();
			ConsoleListener.IndentLevel++;
		}

		/// <summary>
		///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
		///     <para>
		///         See also: <seealso cref="Message" />, <seealso cref="Info" />, <seealso cref="Warning" />, and
		///         <seealso cref="Error" />.
		///     </para>
		/// </summary>
		/// <param name="message"></param>
		[DebuggerStepThrough]
		public static void Error( [CanBeNull] this String message ) {
			if ( message != null ) { $"Error: {message}".WriteLine(); }
		}

		/// <summary>
		///     <seealso cref="Enter" />
		/// </summary>
		/// <param name="method"></param>
		[DebuggerStepThrough]
		public static void Exit( [CallerMemberName] String method = "" ) {
			ConsoleListener.IndentLevel--;
			$"Exited {method ?? String.Empty}".WriteLine();
		}

		[DebuggerStepThrough]
		public static void Finalized( [CallerMemberName] String method = "" ) => $"Finalized: {method ?? String.Empty}".WriteLine();

		/// <summary>
		///     Gets the number of frames in the <see cref="StackTrace" />
		/// </summary>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static Int32 FrameCount<T>( this T _ ) => new StackTrace( false ).FrameCount;

		/// <summary>
		///     Force a memory garbage collection on generation0 and generation1 objects.
		/// </summary>
		/// <seealso
		///     cref="http://programmers.stackexchange.com/questions/276585/when-is-it-a-good-idea-to-force-garbage-collection" />
		[DebuggerStepThrough]
		public static void Garbage() {
			var before = GC.GetTotalMemory( forceFullCollection: false );
			GC.Collect( generation: 1, mode: GCCollectionMode.Forced, blocking: true );
			var after = GC.GetTotalMemory( forceFullCollection: false );

			if ( after < before ) { $"{before - after} bytes freed by the garbage collector.".Info(); }
		}

		/// <summary>
		///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
		///     <para>
		///         See also: <seealso cref="Message" />, <seealso cref="Info" />, <seealso cref="Warning" />, and
		///         <seealso cref="Error" />.
		///     </para>
		/// </summary>
		/// <param name="message"></param>
		[DebuggerStepThrough]
		public static String Info( [CanBeNull] this String message ) {
			$"Info: {message ?? String.Empty}".WriteLine();

			return message ?? String.Empty;
		}

		/// <summary>
		///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
		///     <para>
		///         See also: <seealso cref="Message" />, <seealso cref="Info" />, <seealso cref="Warning" />, and
		///         <seealso cref="Error" />.
		///     </para>
		/// </summary>
		/// <param name="message"></param>
		/// <param name="method"> </param>
		[DebuggerStepThrough]
		public static String Message( [CanBeNull] this String message, [CallerMemberName] String method = "" ) {
			$"{method.NullIfEmpty() ?? "?"}: {message}".WriteLine();

			return message ?? String.Empty;
		}

		/// <summary>
		///     "Bring out yer' dead! Bring out yer' dead!"
		/// </summary>
		/// <param name="exception">       </param>
		/// <param name="method">          </param>
		/// <param name="sourceFilePath">  </param>
		/// <param name="sourceLineNumber"></param>
		/// <remarks>My catchall for exceptions I don't want to deal with, but where I still want to see the exception.</remarks>
		[DebuggerStepThrough]
		public static Exception More( [NotNull] this Exception exception, [CanBeNull] [CallerMemberName] String method = "", [CanBeNull] [CallerFilePath] String sourceFilePath = "",
			[CallerLineNumber] Int32 sourceLineNumber = 0 ) {
			var message = new StringBuilder();
			message.Append( $" [Exception: {exception.Message}]\r\n" );
			message.Append( $" [In: {exception.Source}]\r\n" );
			message.Append( $" [When: {DateTime.Now}]\r\n" );
			message.Append( $" [Msg: {exception.Message}]\r\n" );
			message.Append( $" [Source: {sourceFilePath}]\r\n" );
			message.Append( $" [Line: {sourceLineNumber}]\r\n" );

			if ( method != null ) { ConsoleListener.Fail( method, message.ToString() ); }

			Break();

			return exception;
		}

		public static void Shutdown( Boolean linger = true ) {
			if ( !HasConsoleBeenAllocated ) { return; }

			"Shutting down".WriteColor( ConsoleColor.White, ConsoleColor.Blue );

			if ( linger ) {
				var stopwatch = StopWatch.StartNew();

				while ( stopwatch.Elapsed < Seconds.One ) {
					Thread.Sleep( Hertz.OneHundredTwenty );
					".".WriteColor( ConsoleColor.White, ConsoleColor.Blue );
				}

				stopwatch.Stop();
			}

			NativeMethods.FreeConsole();
		}

		/// <summary>
		///     <para>Allocate a Console.</para>
		///     <para>Start <see cref="ProfileOptimization" />.</para>
		/// </summary>
		/// <returns></returns>
		public static Boolean Startup() {
			try {
				HasConsoleBeenAllocated = NativeMethods.AllocConsole();

				Debug.Listeners.Add( ConsoleListener );

				Contract.ContractFailed += ( sender, e ) => {
					var message = $"Caught Uncaught Contract Failure:\r\nCondition:{e.Condition}\r\nFailureKind:{e.FailureKind}\r\nHandled:{e.Handled}\r\nMessage:{e.Message}";
					Debugger.IsAttached.BreakIfTrue( message );
					e.OriginalException.More();
				};

				ProfileOptimization.SetProfileRoot( Application.ExecutablePath );
				ProfileOptimization.StartProfile( Application.ExecutablePath );

				return HasConsoleBeenAllocated;
			}
			catch ( Exception exception ) { exception.More(); }

			return false;
		}

		/// <summary>
		///     <para>Write the <paramref name="message" /> with <see cref="WriteLine" /></para>
		///     <para>
		///         See also: <seealso cref="Message" />, <seealso cref="Info" />, <seealso cref="Warning" />, and
		///         <seealso cref="Error" />.
		///     </para>
		/// </summary>
		/// <param name="message"></param>
		[DebuggerStepThrough]
		public static void Warning( this String message ) => $"Warning: {message}".WriteLine();

		/// <summary>
		///     <para>Write the <paramref name="message" /> out to the <see cref="ConsoleListener" />.</para>
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="category"></param>
		[DebuggerStepThrough]
		public static void Write( this String message, [CanBeNull] String category = null ) => ConsoleListener.Write( message, category );

		/// <summary>
		///     <para>Write the <paramref name="message" /> out to the <see cref="ConsoleListener" />.</para>
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="category"></param>
		[DebuggerStepThrough]
		public static void WriteLine( this String message, [CanBeNull] String category = null ) => ConsoleListener.WriteLine( message, category );

		private static readonly ConsoleListenerWithTimePrefix ConsoleListener;

		static Logging() => ConsoleListener = new ConsoleListenerWithTimePrefix { IndentSize = 1 };

	}

}