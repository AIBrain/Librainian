// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Logging.cs" was last formatted by Protiguous on 2018/11/16 at 1:51 AM.

namespace Librainian.Logging {

	using System;
	using System.Collections.Concurrent;
	using System.Diagnostics;
	using System.Drawing;
	using System.Windows.Forms;
	using JetBrains.Annotations;
	using NLog;
	using NLog.Common;
	using NLog.Config;
	using NLog.Layouts;
	using NLog.Targets;
	using NLog.Windows.Forms;

	public static class Logging {

		[NotNull]
		public static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

		[NotNull]
		public static ConcurrentDictionary<Control, Target> OurTargets { get; } = new ConcurrentDictionary<Control, Target>();

		public static Boolean Start() {

			InternalLogger.Reset();
#if DEBUG
			LogManager.ThrowConfigExceptions = true;
			LogManager.ThrowExceptions = true;
#endif
			if ( LogManager.Configuration is null ) {
				InternalLogger.Trace( $"{nameof( Start )} creating a new {nameof( LoggingConfiguration )}..." );
				LogManager.Configuration = new LoggingConfiguration();
				InternalLogger.Trace( $"{nameof( Start )} created a new {nameof( LoggingConfiguration )}." );
			}

			return Setup( LogLevel.Trace, LogLevel.Fatal, Targets.TraceTarget.Value );
		}

		/// <summary>
		///     Add a <see cref="Target" /> or a <see cref="RichTextBox" />.
		/// </summary>
		/// <param name="minLogLevel"></param>
		/// <param name="maxLogLevel"></param>
		/// <param name="target">     </param>
		/// <param name="rtb">        </param>
		public static Boolean Setup( LogLevel minLogLevel, LogLevel maxLogLevel, Target target = null, [CanBeNull] RichTextBox rtb = null ) {

			switch ( target ) {
				case null when rtb is null:
					"No target or control specified to setup!".Break();

					return false;

				case null:
					target = rtb.ToTarget();

					break;
			}

			if ( !( target is null ) ) {
				LogManager.Configuration.AddTarget( target );
				LogManager.Configuration.AddRule( minLevel: minLogLevel, maxLevel: maxLogLevel, target: target, loggerNamePattern: "*" );

				return LogManager.Configuration.AllTargets.Contains( target );
			}

			$"Unable to set up target for {minLogLevel} to {maxLogLevel}".Break();

			return false;
		}

		public static void Break<T>( this T _, [CanBeNull] String message = null ) {
			if ( !String.IsNullOrEmpty( message ) ) {
				message.Debug();
			}

			if ( Debugger.IsAttached ) {
				Debugger.Break();
			}
		}

		public static void BreakIfFalse( this Boolean condition, [CanBeNull] String message = null ) {
			if ( !condition ) {
				Break( message );
			}
		}

		public static (Color fore, Color back) Colors( this LoggingLevel loggingLevel ) {

			switch ( loggingLevel ) {
				case LoggingLevel.Divine: {
						return (Color.Blue, Color.Aqua);
					}
				case LoggingLevel.SubspaceTear: {
						return (Color.HotPink, Color.Aqua); //hotpink might actually look okay..
					}
				case LoggingLevel.Fatal: {

						return (Color.DarkRed, Color.Aqua);
					}
				case LoggingLevel.Critical: {

						return (Color.Red, Color.Aqua);
					}
				case LoggingLevel.Error: {

						return (Color.Red, Color.White);
					}
				case LoggingLevel.Warning: {

						return (Color.Goldenrod, Color.White);
					}
				case LoggingLevel.Diagnostic: {

						return (Color.Green, Color.White);
					}

				case LoggingLevel.Debug: {

						return (Color.DarkSeaGreen, Color.White);
					}
				case LoggingLevel.Exception: {

						return (Color.DarkOliveGreen, Color.AntiqueWhite);
					}
				default: throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null );
			}
		}

		public static void Debug<T>( this T obj ) => Logger.Debug( obj );

		public static void Info<T>( this T obj ) => Logger.Info( obj );

		public static void Error<T>( this T obj ) => Logger.Error( obj );

		public static void Fatal<T>( this T obj ) => Logger.Fatal( obj );

		public static void Info( [CanBeNull] this String message ) {
			if ( !String.IsNullOrEmpty( message ) ) {
				Logger.Info( message );
			}
		}

		[NotNull]
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

		public static void Log( this String message, Boolean breakinto = false ) {
			Logger.Debug( message );

			if ( breakinto && Debugger.IsAttached ) {
				Debugger.Break();
			}
		}

		public static void Log( this Exception exception, Boolean breakinto = false ) {
			Logger.Debug( exception );

			if ( breakinto && Debugger.IsAttached ) {
				Debugger.Break();
			}
		}

		public static T Log<T>( this T message, Boolean breakinto = false ) {
			Logger.Debug( message );

			if ( breakinto && Debugger.IsAttached ) {
				Debugger.Break();
			}

			return message;
		}

		public static void Trace( this String message ) => Logger.Trace( message );

		public static void Trace( this Exception exception ) => Logger.Trace( exception );

		public static void Trace<T>( this T message ) => Logger.Trace( message );

		public static void Warn( this String message ) => Logger.Warn( message );

		public static void Warn( this Exception exception ) => Logger.Warn( exception );

		public static void Warn<T>( this T message ) => Logger.Warn( message );

		[CanBeNull]
		public static Target ToTarget( [CanBeNull] this RichTextBox rtb ) {
			if ( rtb is null ) {

				//throw new ArgumentNullException(nameof( rtb ),"The paramter 'rtb' was null." );
				return null;
			}

			if ( String.IsNullOrWhiteSpace( rtb.Name ) ) {
				throw new ArgumentNullException( nameof( rtb ), "No name given on this RichTextBox control." );
			}

			if ( OurTargets.TryGetValue( rtb, out var toTarget ) ) {
				return toTarget;
			}

			var controlName = rtb.Name.Trim();

			if ( LogManager.Configuration.FindTargetByName( controlName ) is RichTextBoxTarget target ) {
				OurTargets[ rtb ] = target;

				return target;
			}

			var form = rtb.FindForm();
			var formName = form?.Name ?? "Form_" + rtb.Name;

			target = new RichTextBoxTarget {
				Name = controlName,
				Layout = new CsvLayout {
					WithHeader = false,
					Delimiter = CsvColumnDelimiterMode.Tab,
					Columns = {
						new CsvColumn( "Time", layout: "${longdate}" ), new CsvColumn( "Message", layout: "${message}" )
					}
				},
				ControlName = controlName,
				AutoScroll = true,
				FormName = formName,
				MaxLines = 256,
				UseDefaultRowColoringRules = true,
				AllowAccessoryFormCreation = true,
				MessageRetention = RichTextBoxTargetMessageRetentionStrategy.All,
				ShowMinimized = false,
				SupportLinks = true,
				TargetForm = form,
				TargetRichTextBox = rtb,
				ToolWindow = true
			};

			OurTargets[ rtb ] = target;

			return target;
		}
	}
}