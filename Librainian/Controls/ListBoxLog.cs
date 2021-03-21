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
// File "ListBoxLog.cs" last formatted on 2020-08-14 at 8:32 PM.

#nullable enable

namespace Librainian.Controls {

	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;
	using JetBrains.Annotations;
	using Logging;
	using Maths;
	using Utilities;

	/// <summary>Pulled from http://stackoverflow.com/a/6587172/956364</summary>
	public sealed class ListBoxLog : ABetterClassDispose {

		private const Int32 DefaultMaxLinesInListbox = 2048;

		/// <summary>
		///     <see cref="FormatALogEventMessage" />
		/// </summary>
		private const String DefaultMessageFormat = "{4}>{8}";

		public ListBoxLog( [NotNull] ListBox listBox, [NotNull] String messageFormat ) : this( listBox, messageFormat, DefaultMaxLinesInListbox ) {
			if ( listBox is null ) {
				throw new ArgumentNullException( nameof( listBox ) );
			}

			if ( messageFormat is null ) {
				throw new ArgumentNullException( nameof( messageFormat ) );
			}
		}

		public ListBoxLog( [NotNull] ListBox listBox, [NotNull] String messageFormat = DefaultMessageFormat, Int32 maxLinesInListbox = DefaultMaxLinesInListbox ) {
			if ( String.IsNullOrWhiteSpace( messageFormat ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( messageFormat ) );
			}

			this.Box = listBox ?? throw new ArgumentNullException( nameof( listBox ) );
			this.Box.SelectionMode = SelectionMode.MultiExtended;

			this.Box.HandleCreated += this.OnHandleCreated;
			this.Box.HandleDestroyed += this.OnHandleDestroyed;
			this.Box.DrawItem += this.DrawItemHandler;
			this.Box.KeyDown += this.KeyDownHandler;

#if NET48
			this.Box.ContextMenu = new ContextMenu( new[] {
				new MenuItem( "Copy", this.CopyMenuOnClickHandler )
			} );

			if ( this.Box.ContextMenu != null ) {
				this.Box.ContextMenu.Popup += this.CopyMenuPopupHandler;
			}
#endif
#if NET50
			//TODO add back in once NET50 has the contextmenu
#endif

			this.Box.DrawMode = DrawMode.OwnerDrawFixed;

			this.MessageFormat = messageFormat;
			this.MaxEntriesInListBox = maxLinesInListbox;

			this.Paused = false;

			this.CanAdd = listBox.IsHandleCreated;
		}


		[NotNull]
		private ListBox Box { get; }

		private Boolean CanAdd { get; set; }

		private Int32 MaxEntriesInListBox { get; }

		private String MessageFormat { get; }

		public Boolean Paused { get; }

		[NotNull]
		private static String FormatALogEventMessage( [NotNull] LogEvent logEvent, [NotNull] String messageFormat ) {
			var message = logEvent.Message ?? "*null*";

			return String.Format( messageFormat, /* {0} */ logEvent.EventTime.ToString( "yyyy-MM-dd HH:mm:ss.fff" ),                    /* {1} */
								  logEvent.EventTime.ToString( "yyyy-MM-dd HH:mm:ss" ),                                                 /* {2} */
								  logEvent.EventTime.ToString( "yyyy-MM-dd" ), /* {3} */ logEvent.EventTime.ToString( "HH:mm:ss.fff" ), /* {4} */
								  logEvent.EventTime.ToString( "HH:mm" ),                                                               /* {5} */
								  logEvent.LoggingLevel.LevelName()[0],                                                                 /* {6} */
								  logEvent.LoggingLevel.LevelName(), /* {7} */ ( Int32 )logEvent.LoggingLevel, /* {8} */ message );
		}

		private void AddALogEntry( [CanBeNull] Object? item ) {
			if ( item == null ) {
				return;
			}

			var items = this.Box.Items;

			if ( items.Count == 0 ) {
				this.AddALogEntryLine( item );

				return;
			}

			var currentText = items[^1] as String ?? String.Empty;
			currentText += item as String ?? String.Empty;
			this.Box.Items[items.Count - 1] = currentText;
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

		private void CopyMenuOnClickHandler( [CanBeNull] Object? sender, [CanBeNull] EventArgs? e ) => this.CopyToClipboard();

		private void CopyMenuPopupHandler( [CanBeNull] Object? sender, [CanBeNull] EventArgs? e ) {
#if NET48
			if ( sender is ContextMenu menu ) {
				menu.MenuItems[ 0 ].Enabled = this.Box.SelectedItems.Count > 0;
			}
#endif
		}

		private void CopyToClipboard() {
			if ( !this.Box.SelectedItems.Count.Any() ) {
				return;
			}

			var selectedItemsAsRTFText = new StringBuilder();
			selectedItemsAsRTFText.AppendLine( @"{\rtf1\ansi\deff0{\fonttbl{\f0\fcharset0 Courier;}}" );

			selectedItemsAsRTFText.AppendLine(
				@"{\colortbl;\red255\green255\blue255;\red255\green0\blue0;\red218\green165\blue32;\red0\green128\blue0;\red0\green0\blue255;\red0\green0\blue0}" );

			foreach ( LogEvent logEvent in this.Box.SelectedItems ) {
				selectedItemsAsRTFText.AppendFormat( @"{{\f0\fs16\chshdng0\chcbpat{0}\cb{0}\cf{1} ", logEvent.LoggingLevel == LoggingLevel.Critical ? 2 : 1,
													 logEvent.LoggingLevel == LoggingLevel.Critical ? 1 : ( Int32 )logEvent.LoggingLevel > 5 ? 6 :
													 ( Int32 )logEvent.LoggingLevel + 1 );

				selectedItemsAsRTFText.Append( FormatALogEventMessage( logEvent, this.MessageFormat ) );
				selectedItemsAsRTFText.AppendLine( @"\par}" );
			}

			selectedItemsAsRTFText.AppendLine( @"}" );
			Debug.WriteLine( selectedItemsAsRTFText.ToString() );

			if ( DataFormats.Rtf != null ) {
				Clipboard.SetData( DataFormats.Rtf, selectedItemsAsRTFText.ToString() );
			}
		}

		private void DrawItemHandler( [CanBeNull] Object? sender, [NotNull] DrawItemEventArgs e ) {
			if ( e.Index < 0 ) {
				return;
			}

			if ( sender is ListBox listbox ) {
				e.DrawBackground();
				e.DrawFocusRectangle();

				var listboxItem = listbox.Items[e.Index];

				var logEvent = listboxItem is LogEvent item ? item : new LogEvent( LoggingLevel.Critical, listboxItem.ToString() );

				( var fore, var back ) = logEvent.LoggingLevel.Colors();

				using var solidBrush = new SolidBrush( back );

				using var brush = new SolidBrush( fore );

				using var font = new Font( "Hack", 8.25f, FontStyle.Regular );

				e.Graphics.FillRectangle( solidBrush, e.Bounds );
				e.Graphics.DrawString( FormatALogEventMessage( logEvent, this.MessageFormat ), font, brush, e.Bounds );
			}
			else {
				String.Empty.Break();
			}
		}

		private void KeyDownHandler( [CanBeNull] Object? sender, [NotNull] KeyEventArgs e ) {
			if ( e.Modifiers == Keys.Control && e.KeyCode == Keys.C ) {
				this.CopyToClipboard();
			}
		}

		private void OnHandleCreated( [CanBeNull] Object? sender, [CanBeNull] EventArgs? e ) => this.CanAdd = true;

		private void OnHandleDestroyed( [CanBeNull] Object? sender, [CanBeNull] EventArgs? e ) => this.CanAdd = false;

		private void WriteEvent( [NotNull] LogEvent logEvent ) {
			if ( this.CanAdd ) {
				this.Box.BeginInvoke( new AddALogEntryDelegate( this.AddALogEntry ), logEvent );
			}
		}

		private void WriteEventLine( [NotNull] LogEvent logEvent ) {
			if ( this.CanAdd ) {
				this.Box.BeginInvoke( new AddALogEntryDelegate( this.AddALogEntryLine ), logEvent );
			}
		}

		public override void DisposeManaged() {
			this.CanAdd = false;

			using ( this.Box ) {
				this.Box.HandleCreated -= this.OnHandleCreated;
				this.Box.HandleCreated -= this.OnHandleDestroyed;
				this.Box.DrawItem -= this.DrawItemHandler;
				this.Box.KeyDown -= this.KeyDownHandler;

#if NET48
				using var boxContextMenu = this.Box.ContextMenu;

				if ( boxContextMenu != null ) {
					boxContextMenu.MenuItems.Clear();
					boxContextMenu.Popup -= this.CopyMenuPopupHandler;
				}
#endif

				this.Box.Items.Clear();
				this.Box.DrawMode = DrawMode.Normal;
			}
		}

		public void Log( [CanBeNull] String? message ) => this.WriteEvent( new LogEvent( LoggingLevel.Critical, message ) );

		public void LogLine( [CanBeNull] String? message ) => this.LogLine( LoggingLevel.Debug, message );

		public void LogLine( [CanBeNull] String? format, [NotNull] params Object[] args ) =>
			this.LogLine( LoggingLevel.Debug, format is null ? null : String.Format( format, args ) );

		public void LogLine( LoggingLevel loggingLevel, [CanBeNull] String? format, [NotNull] params Object[] args ) =>
			this.LogLine( loggingLevel, format is null ? null : String.Format( format, args ) );

		public void LogLine( LoggingLevel loggingLevel, [CanBeNull] String? message ) => this.WriteEventLine( new LogEvent( loggingLevel, message ) );

		private delegate void AddALogEntryDelegate( Object item );

		private class LogEvent {

			public LogEvent( LoggingLevel loggingLevel, [CanBeNull] String? message ) {
				this.EventTime = DateTime.Now;
				this.LoggingLevel = loggingLevel;
				this.Message = message;
			}

			public DateTime EventTime { get; }

			public LoggingLevel LoggingLevel { get; }

			[CanBeNull]
			public String? Message { get; }

		}

	}

}
