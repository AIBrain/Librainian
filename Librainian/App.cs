// Copyright � Protiguous. All Rights Reserved.
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
// File "App.cs" last formatted on 2020-08-26 at 4:52 AM.

#nullable enable

namespace Librainian {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime;
	using System.Threading;
	using System.Windows.Forms;
	using CommandLine;
	using Controls;
	using Extensions;
	using JetBrains.Annotations;
	using Logging;
	using Parsing;
	using Error = CommandLine.Error;

	public static class App {

		/// <summary>
		///     <para>Adds program-wide exception handlers.</para>
		///     <para>Optimizes program startup.</para>
		///     <para>Starts logging, Debug and Trace.</para>
		///     <para>Performs a garbage cleanup.</para>
		///     <para>And starts the form thread-loop.</para>
		/// </summary>
		public static Status Run<TOpts>( [NotNull] Action<TOpts> runParsedOptions, String[] arguments ) where TOpts : IOptions {
			if ( runParsedOptions is null ) {
				throw new ArgumentNullException( nameof( runParsedOptions ) );
			}

			if ( arguments is null ) {
				throw new ArgumentNullException( nameof( arguments ) );
			}

			try {
				RunInternalCommon();

				var parsed = Parser.Default?.ParseArguments<TOpts>( arguments );

				if ( parsed is null ) {
					return TellError();
				}

				var with = parsed.WithParsed( runParsedOptions );

				if ( with == null ) {
					return TellError();
				}

				var withnow = with.WithNotParsed( HandleErrors );

				if ( withnow is null ) {
					return TellError();
				}

				return Status.Success;
			}
			catch ( Exception exception ) {
				exception.Log();

				return Status.Exception;
			}

			static void HandleErrors( IEnumerable<Error?>? errors ) {
				try {
					if ( errors is null ) {
						if ( Debugger.IsAttached ) {
							"Unknown error.".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );
							Debugger.Break();
						}
					}
					else {
						var message = errors.Select( error => error?.ToString() ).ToStrings( Environment.NewLine );

						if ( Debugger.IsAttached ) {
							Debug.WriteLine( message );
							Debugger.Break();
						}
						else {
							$"Error parsing command line options.{Environment.NewLine}{message}".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );
						}
					}
				}
				catch ( Exception exception ) {
					exception.Log();
				}
			}

			static Status TellError() {
				ConsoleWindow.AttachConsoleWindow();
				ConsoleWindow.ShowWindow();
				"Error parsing command line.".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );

				return Status.Failure;
			}

		}

		private static void RunInternalCommon() {
			Debug.AutoFlush = true;
			Trace.AutoFlush = true;

			AppDomain.CurrentDomain.UnhandledException += ( sender, e ) => ( e.ExceptionObject as Exception )?.Log();

			ProfileOptimization.SetProfileRoot( Application.ExecutablePath );

			ProfileOptimization.StartProfile( Application.ExecutablePath );
			Application.ThreadException += ( sender, e ) => e.Exception.Log();

			try {
				Application.SetCompatibleTextRenderingDefault( false );
			}
			catch ( InvalidOperationException exception ) {
				exception.Log();
			}

			Application.SetHighDpiMode( HighDpiMode.SystemAware );
			Application.EnableVisualStyles();

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

		public static void Run<TForm>( [CanBeNull] IEnumerable<String>? arguments ) where TForm : Form, new() {
			RunInternalCommon();

			using var form = new TForm();

			if ( arguments != null ) {
				form.Tag = arguments.Where( s => !String.IsNullOrWhiteSpace( s ) ).ToArray();
			}

			form.SuspendLayout();
			form.WindowState = FormWindowState.Normal;
			form.StartPosition = FormStartPosition.WindowsDefaultBounds;
			//form.LoadLocation();
			//form.LoadSize();

			/*
			if ( !form.IsFullyVisibleOnAnyScreen() ) {
				form.WindowState = FormWindowState.Normal;
				form.StartPosition = FormStartPosition.CenterScreen;
			}
			*/

			form.ResumeLayout( true );

			//var frm = form;
			//form.LocationChanged += ( sender, args ) => frm.SaveLocation();
			//form.SizeChanged += ( sender, args ) => frm.SaveSize();

			Application.Run( form );
		}

	}

}