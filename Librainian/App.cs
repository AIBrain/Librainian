﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "App.cs" last formatted on 2021-11-30 at 7:23 PM by Protiguous.

#nullable enable

namespace Librainian;

using System;
using System.Diagnostics;
using System.Runtime;
using System.Threading;
using System.Windows.Forms;
using Controls;
using Exceptions;
using Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class App {

	private static void CommonPreRun() {
		Debug.AutoFlush = true;
		Trace.AutoFlush = true;

		AppDomain.CurrentDomain.UnhandledException += ( _, e ) => ( e.ExceptionObject as Exception )?.Log( BreakOrDontBreak.Break );

		ProfileOptimization.SetProfileRoot( Application.ExecutablePath );
		ProfileOptimization.StartProfile( Application.ExecutablePath );

		Application.ThreadException += ( _, e ) => e.Exception.Log( BreakOrDontBreak.Break );

		try {
			var highDpiMode = Application.SetHighDpiMode( HighDpiMode.SystemAware );
			if ( highDpiMode ) {
				$"{nameof( Application.SetHighDpiMode )} has been set.".TraceLine();
			}

			Application.SetCompatibleTextRenderingDefault( false );
			Application.EnableVisualStyles();
		}
		catch ( InvalidOperationException exception ) {
			exception.Log();
		}

		try {
			Thread.CurrentThread.Name = "UI";
		}
		catch ( InvalidOperationException exception ) {
			exception.Log();
		}

		Compact();
		Thread.Yield();
		Compact();

		static void Compact() {
			GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
			GC.Collect( 2, GCCollectionMode.Forced, true, true );
		}
	}

	/*

	/// <summary>
	/// <para>Creates a console window.</para>
	/// <para>Adds program-wide exception handlers.</para>
	/// <para>Optimizes program startup.</para>
	/// <para>Starts logging, Debug and Trace.</para>
	/// <para>Performs a garbage cleanup.</para>
	/// <para>And then runs the <see cref="Action" /><paramref name="runMe" />.</para>
	/// </summary>
	public static Status Run<TOpts>( [NotNull] Action<TOpts> runMe, params String[] arguments ) where TOpts : IOptions {
		if ( runMe is null ) {
			throw new NullException( nameof( runMe ) );
		}

		if ( arguments is null ) {
			throw new NullException( nameof( arguments ) );
		}

		try {
			ConsoleWindow.ShowWindow();

			ConsoleWindow.SetTitle( "Loading.." );
			RunInternalCommon();

			ConsoleWindow.SetTitle( $"Parsing {nameof( arguments )}.." );
			var parsed = Parser.Default?.ParseArguments<TOpts>( arguments );

			if ( parsed is null ) {
				return TellError();
			}

			var result = parsed.WithNotParsed( HandleParseErrors );
			if ( result == null || result.Tag == ParserResultType.NotParsed ) {
				return TellError();
			}

			parsed.WithParsed( runMe );

			ConsoleWindow.SetTitle( "Exiting.." );

			ConsoleWindow.HideWindow();

			return Status.Done;
		}
		catch ( Exception exception ) {
			exception.Log();

			return Status.Exception;
		}

		static void HandleParseErrors( IEnumerable<Error?>? errors ) {
			try {
				if ( errors is null ) {

					//"Unknown error.".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );
					//                Logging.Logging.BreakIfDebug();

					return;
				}

				var message = errors.Select( error => error?.ToString() ).ToStrings( Environment.NewLine );

				if ( Debugger.IsAttached ) {
					message.Break();
				}
				else {
					$"Error parsing command line options.{Environment.NewLine}{message}".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}
		}

		static Status TellError() {
			"Error parsing command line.".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );

			return Status.Failure;
		}
	}
	*/

	public static void Run<TForm>( IHost host ) where TForm : Form {
		try {
			CommonPreRun();

			if ( host.Services is null ) {
				throw new NullException( nameof( host.Services ) );
			}

			var form = host.Services.GetRequiredService<TForm>();

			form.Tag = host;
			form.PersistPlacement();

			if ( form.InvokeRequired ) {
				$"{nameof( form.InvokeRequired )} on {nameof( form.Name )}.".DebugLine();
			}

			form.InvokeAction( () => Application.Run( form ), RefreshOrInvalidate.Refresh );
		}
		catch ( Exception exception ) {
			exception.Log( BreakOrDontBreak.Break );
		}
	}
}