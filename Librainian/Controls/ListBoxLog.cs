// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "ListBoxLog.cs" belongs to Protiguous@Protiguous.com
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
// File "ListBoxLog.cs" was last formatted by Protiguous on 2018/06/26 at 12:58 AM.

namespace Librainian.Controls {

	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;
	using Extensions;
	using JetBrains.Annotations;
	using Magic;

	/// <summary>
	///     Pulled from http://stackoverflow.com/a/6587172/956364
	/// </summary>
	public sealed class ListBoxLog : ABetterClassDispose {

		private const Int32 DefaultMaxLinesInListbox = 2000;

		/// <summary>
		///     <seealso cref="FormatALogEventMessage" />
		/// </summary>
		private const String DefaultMessageFormat = "{4}>{8}";

		private ListBox Box { get; set; }

		private Boolean CanAdd { get; set; }

		private Int32 MaxEntriesInListBox { get; }

		private String MessageFormat { get; }

		public Boolean Paused { get; }

		public ListBoxLog( ListBox listBox, String messageFormat ) : this( listBox, messageFormat, DefaultMaxLinesInListbox ) { }

		public ListBoxLog( [NotNull] ListBox listBox, String messageFormat = DefaultMessageFormat, Int32 maxLinesInListbox = DefaultMaxLinesInListbox ) {

			this.Box = listBox;
			this.MessageFormat = messageFormat;
			this.MaxEntriesInListBox = maxLinesInListbox;

			this.Paused = false;

			this.CanAdd = listBox.IsHandleCreated;

			this.Box.SelectionMode = SelectionMode.MultiExtended;

			this.Box.HandleCreated += this.OnHandleCreated;
			this.Box.HandleDestroyed += this.OnHandleDestroyed;
			this.Box.DrawItem += this.DrawItemHandler;
			this.Box.KeyDown += this.KeyDownHandler;

			var menuItems = new[] {
				new MenuItem( "Copy", this.CopyMenuOnClickHandler )
			};

			this.Box.ContextMenu = new ContextMenu( menuItems );
			this.Box.ContextMenu.Popup += this.CopyMenuPopupHandler;

			this.Box.DrawMode = DrawMode.OwnerDrawFixed;
		}

		~ListBoxLog() => this.Dispose();

		private delegate void AddALogEntryDelegate( Object item );

		[NotNull]
		private static String FormatALogEventMessage( [NotNull] LogEvent logEvent, [NotNull] String messageFormat ) {
			var message = logEvent.Message ?? "<NULL>";

			return String.Format( messageFormat, /* {0} */ logEvent.EventTime.ToString( "yyyy-MM-dd HH:mm:ss.fff" ), /* {1} */ logEvent.EventTime.ToString( "yyyy-MM-dd HH:mm:ss" ), /* {2} */
				logEvent.EventTime.ToString( "yyyy-MM-dd" ), /* {3} */ logEvent.EventTime.ToString( "HH:mm:ss.fff" ), /* {4} */ logEvent.EventTime.ToString( "HH:mm" ), /* {5} */
				LevelName( logEvent.LoggingLevel )[ 0 ], /* {6} */ LevelName( logEvent.LoggingLevel ), /* {7} */ ( Int32 ) logEvent.LoggingLevel, /* {8} */ message );
		}

		[NotNull]
		private static String LevelName( LoggingLevel loggingLevel ) {
			switch ( loggingLevel ) {
				case LoggingLevel.Critical: return "Critical";

				case LoggingLevel.Error: return "Error";

				case LoggingLevel.Warning: return "Warning";

				case LoggingLevel.Info: return "Info";

				case LoggingLevel.Verbose: return "Verbose";

				case LoggingLevel.Debug: return "Debug";

				default: return $"<value={( Int32 ) loggingLevel}>";
			}
		}

		private void AddALogEntry( Object item ) {
			var items = this.Box.Items;

			if ( items.Count == 0 ) {
				this.AddALogEntryLine( item );

				return;
			}

			var currentText = items[ items.Count - 1 ] as String ?? String.Empty;
			currentText += item as String ?? String.Empty;
			this.Box.Items[ items.Count - 1 ] = currentText;
		}

		private void AddALogEntryLine( [NotNull] Object item ) {
			this.Box.Items.Add( item );

			if ( this.Box.Items.Count > this.MaxEntriesInListBox ) {
				this.Box.Items.RemoveAt( 0 );
			}

			if ( !this.Paused ) {
				this.Box.TopIndex = this.Box.Items.Count - 1;
			}
		}

		private void CopyMenuOnClickHandler( Object sender, EventArgs e ) => this.CopyToClipboard();

		private void CopyMenuPopupHandler( Object sender, EventArgs e ) {
			if ( sender is ContextMenu menu ) {
				menu.MenuItems[ 0 ].Enabled = this.Box.SelectedItems.Count > 0;
			}
		}

		private void CopyToClipboard() {
			if ( !this.Box.SelectedItems.Count.Any() ) {
				return;
			}

			var selectedItemsAsRTFText = new StringBuilder();
			selectedItemsAsRTFText.AppendLine( @"{\rtf1\ansi\deff0{\fonttbl{\f0\fcharset0 Courier;}}" );
			selectedItemsAsRTFText.AppendLine( @"{\colortbl;\red255\green255\blue255;\red255\green0\blue0;\red218\green165\blue32;\red0\green128\blue0;\red0\green0\blue255;\red0\green0\blue0}" );

			foreach ( LogEvent logEvent in this.Box.SelectedItems ) {

				selectedItemsAsRTFText.AppendFormat( @"{{\f0\fs16\chshdng0\chcbpat{0}\cb{0}\cf{1} ", logEvent.LoggingLevel == LoggingLevel.Critical ? 2 : 1,
					logEvent.LoggingLevel == LoggingLevel.Critical ? 1 : ( Int32 ) logEvent.LoggingLevel > 5 ? 6 : ( Int32 ) logEvent.LoggingLevel + 1 );

				selectedItemsAsRTFText.Append( FormatALogEventMessage( logEvent, this.MessageFormat ) );
				selectedItemsAsRTFText.AppendLine( @"\par}" );
			}

			selectedItemsAsRTFText.AppendLine( @"}" );
			Debug.WriteLine( selectedItemsAsRTFText.ToString() );
			Clipboard.SetData( DataFormats.Rtf, selectedItemsAsRTFText.ToString() );
		}

		private void DrawItemHandler( Object sender, [NotNull] DrawItemEventArgs e ) {
			if ( e.Index < 0 ) {
				return;
			}

			e.DrawBackground();
			e.DrawFocusRectangle();

			var logEvent = ( ( ListBox ) sender ).Items[ e.Index ] as LogEvent ?? new LogEvent( LoggingLevel.Critical, ( ( ListBox ) sender ).Items[ e.Index ].ToString() );

			// SafeGuard against wrong configuration of list box

			Color color;

			switch ( logEvent.LoggingLevel ) {
				case LoggingLevel.Critical:
					color = Color.White;

					break;

				case LoggingLevel.Error:
					color = Color.Red;

					break;

				case LoggingLevel.Warning:
					color = Color.Goldenrod;

					break;

				case LoggingLevel.Info:
					color = Color.Green;

					break;

				case LoggingLevel.Verbose:
					color = Color.Blue;

					break;

				default:
					color = Color.Black;

					break;
			}

			if ( logEvent.LoggingLevel == LoggingLevel.Critical ) {
				e.Graphics.FillRectangle( new SolidBrush( Color.Red ), e.Bounds );
			}

			e.Graphics.DrawString( FormatALogEventMessage( logEvent, this.MessageFormat ), new Font( "Hack", 8.25f, FontStyle.Regular ), new SolidBrush( color ), e.Bounds );
		}

		private void KeyDownHandler( Object sender, [NotNull] KeyEventArgs e ) {
			if ( e.Modifiers == Keys.Control && e.KeyCode == Keys.C ) {
				this.CopyToClipboard();
			}
		}

		private void OnHandleCreated( Object sender, EventArgs e ) => this.CanAdd = true;

		private void OnHandleDestroyed( Object sender, EventArgs e ) => this.CanAdd = false;

		private void WriteEvent( [CanBeNull] LogEvent logEvent ) {
			if ( logEvent != null && this.CanAdd ) {
				this.Box.BeginInvoke( new AddALogEntryDelegate( this.AddALogEntry ), logEvent );
			}
		}

		private void WriteEventLine( [CanBeNull] LogEvent logEvent ) {
			if ( logEvent != null && this.CanAdd ) {
				this.Box.BeginInvoke( new AddALogEntryDelegate( this.AddALogEntryLine ), logEvent );
			}
		}

		public override void DisposeManaged() {
			if ( this.Box is null ) {
				return;
			}

			this.CanAdd = false;

			this.Box.HandleCreated -= this.OnHandleCreated;
			this.Box.HandleCreated -= this.OnHandleDestroyed;
			this.Box.DrawItem -= this.DrawItemHandler;
			this.Box.KeyDown -= this.KeyDownHandler;

			this.Box.ContextMenu.MenuItems.Clear();
			this.Box.ContextMenu.Popup -= this.CopyMenuPopupHandler;
			this.Box.ContextMenu = null;

			this.Box.Items.Clear();
			this.Box.DrawMode = DrawMode.Normal;
			this.Box = null;

			base.DisposeManaged();
		}

		public void Log( String message ) => this.WriteEvent( new LogEvent( LoggingLevel.Critical, message ) );

		public void LogLine( String message ) => this.LogLine( LoggingLevel.Debug, message );

		public void LogLine( [CanBeNull] String format, params Object[] args ) => this.LogLine( LoggingLevel.Debug, format is null ? null : String.Format( format, args ) );

		public void LogLine( LoggingLevel loggingLevel, [CanBeNull] String format, params Object[] args ) => this.LogLine( loggingLevel, format is null ? null : String.Format( format, args ) );

		public void LogLine( LoggingLevel loggingLevel, String message ) => this.WriteEventLine( new LogEvent( loggingLevel, message ) );

		private class LogEvent {

			public DateTime EventTime { get; }

			public LoggingLevel LoggingLevel { get; }

			public String Message { get; }

			public LogEvent( LoggingLevel loggingLevel, String message ) {
				this.EventTime = DateTime.Now;
				this.LoggingLevel = loggingLevel;
				this.Message = message;
			}
		}
	}
}