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
// File "ListBoxLog.cs" last touched on 2021-07-03 at 12:19 PM by Protiguous.

#nullable enable

namespace Librainian.Controls {

	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;
	using Exceptions;
	using Logging;
	using Maths;
	using Microsoft.Extensions.Logging;
	using Parsing;
	using Utilities;

	/// <summary>Pulled from http://stackoverflow.com/a/6587172/956364</summary>
	public sealed class ListBoxLog : ABetterClassDispose {

		private const Int32 DefaultMaxLinesInListbox = 4096;

		/// <summary>
		///     <see cref="FormatALogEventMessage" />
		/// </summary>
		private const String DefaultMessageFormat = "{4}>{8}";

		public ListBoxLog( ListBox listBox, String messageFormat ) : this( listBox, messageFormat, DefaultMaxLinesInListbox ) {
			if ( listBox is null ) {
				throw new ArgumentEmptyException( nameof( listBox ) );
			}

			if ( messageFormat is null ) {
				throw new ArgumentEmptyException( nameof( messageFormat ) );
			}
		}

		public ListBoxLog( ListBox listBox, String messageFormat = DefaultMessageFormat, Int32 maxLinesInListbox = DefaultMaxLinesInListbox ) {
			if ( String.IsNullOrWhiteSpace( messageFormat ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( messageFormat ) );
			}

			this.Box = listBox ?? throw new ArgumentEmptyException( nameof( listBox ) );
			this.Box.SelectionMode = SelectionMode.MultiExtended;

			this.Box.HandleCreated += this.OnHandleCreated;
			this.Box.HandleDestroyed += this.OnHandleDestroyed;
			this.Box.DrawItem += this.DrawItemHandler;
			this.Box.KeyDown += this.KeyDownHandler;

			this.Box.ContextMenuStrip = new ContextMenuStrip( 
				//new[] { new MenuItem( "Copy", this.CopyMenuOnClickHandler ) } //TODO
			) {
				AutoClose = true, AutoSize = true
			};

			if ( this.Box.ContextMenuStrip != null ) {
				//this.Box.ContextMenuStrip.Popup += this.CopyMenuPopupHandler;	//TODO
			}

			this.Box.DrawMode = DrawMode.OwnerDrawFixed;

			this.MessageFormat = messageFormat;
			this.MaxEntriesInListBox = maxLinesInListbox;

			this.Paused = false;

			this.CanAdd = listBox.IsHandleCreated;
		}

		private ListBox Box { get; }

		private Boolean CanAdd { get; set; }

		/// <summary>
		///     Used during every <see cref="DrawItemHandler" />.
		/// </summary>
		private Font HackFont { get; } = new("Hack", 8.25f, FontStyle.Regular);

		private Int32 MaxEntriesInListBox { get; }

		private String MessageFormat { get; }

		public Boolean Paused { get; }

		private static String? FormatALogEventMessage( LogEvent logEvent, String messageFormat ) {
			var message = logEvent.Message ?? "*null*";

			var f = String.Format( messageFormat, /* {0} */ logEvent.EventTime.ToString( "yyyy-MM-dd HH:mm:ss.fff" ), /* {1} */
				logEvent.EventTime.ToString( "yyyy-MM-dd HH:mm:ss" ), /* {2} */
				logEvent.EventTime.ToString( "yyyy-MM-dd" ), /* {3} */ logEvent.EventTime.ToString( "HH:mm:ss.fff" ), /* {4} */
				logEvent.EventTime.ToString( "HH:mm" ), /* {5} */
				logEvent.LogLevel.LevelName()[ 0 ], /* {6} */
				logEvent.LogLevel.LevelName(), /* {7} */ ( Int32 )logEvent.LogLevel, /* {8} */ message );
			return f.Trimmed();
		}

		private void AddALogEntry( Object? item ) {
			if ( item == null ) {
				return;
			}

			var items = this.Box.Items;

			if ( items.Count == 0 ) {
				this.AddALogEntryLine( item );

				return;
			}

			var currentText = items[ ^1 ] as String ?? String.Empty;
			currentText += item as String ?? String.Empty;
			this.Box.Items[ items.Count - 1 ] = currentText;
		}

		private void AddALogEntryLine( Object item ) {
			this.Box.Items.Add( item );

			if ( this.Box.Items.Count > this.MaxEntriesInListBox ) {
				this.Box.Items.RemoveAt( 0 );
			}

			if ( !this.Paused ) {
				this.Box.TopIndex = this.Box.Items.Count - 1;
			}
		}

		private void CopyMenuOnClickHandler( Object? sender, EventArgs? e ) => this.CopyToClipboard();

		private void CopyMenuPopupHandler( Object? sender, EventArgs? e ) {
			if ( sender is ContextMenuStrip menu ) {
				menu.Items[ 0 ].Enabled = this.Box.SelectedItems.Count > 0;
			}
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
				selectedItemsAsRTFText.AppendFormat( @"{{\f0\fs16\chshdng0\chcbpat{0}\cb{0}\cf{1} ", logEvent.LogLevel == LogLevel.Critical ? 2 : 1,
					logEvent.LogLevel == LogLevel.Critical ? 1 :
					( Int32 )logEvent.LogLevel > 5 ? 6 : ( Int32 )logEvent.LogLevel + 1 );

				selectedItemsAsRTFText.Append( FormatALogEventMessage( logEvent, this.MessageFormat ) );
				selectedItemsAsRTFText.AppendLine( @"\par}" );
			}

			selectedItemsAsRTFText.AppendLine( @"}" );
			Debug.WriteLine( selectedItemsAsRTFText.ToString() );

			if ( DataFormats.Rtf != null ) {
				Clipboard.SetData( DataFormats.Rtf, selectedItemsAsRTFText.ToString() );
			}
		}

		private void DrawItemHandler( Object? sender, DrawItemEventArgs e ) {
			if ( e.Index < 0 ) {
				return;
			}

			if ( sender is ListBox listbox ) {
				e.DrawBackground();
				e.DrawFocusRectangle();

				var listboxItem = listbox.Items[ e.Index ];

				var logEvent = listboxItem is LogEvent item ? item : new LogEvent( LogLevel.Critical, listboxItem.ToString() );

				( var fore, var back ) = logEvent.LogLevel.Colors();

				using ( var solidBrush = new SolidBrush( back ) ) {
					e.Graphics.FillRectangle( solidBrush, e.Bounds );
				}

				using var brush = new SolidBrush( fore );
				e.Graphics.DrawString( FormatALogEventMessage( logEvent, this.MessageFormat ), this.HackFont, brush, e.Bounds );
			}
			else {
				String.Empty.Break();
			}
		}

		private void KeyDownHandler( Object? sender, KeyEventArgs e ) {
			if ( e.Modifiers == Keys.Control && e.KeyCode == Keys.C ) {
				this.CopyToClipboard();
			}
		}

		private void OnHandleCreated( Object? sender, EventArgs? e ) => this.CanAdd = true;

		private void OnHandleDestroyed( Object? sender, EventArgs? e ) => this.CanAdd = false;

		//private delegate void AddALogEntryDelegate( Object item );

		public override void DisposeManaged() {
			this.CanAdd = false;

			using ( this.Box ) {
				this.Box.HandleCreated -= this.OnHandleCreated;
				this.Box.HandleCreated -= this.OnHandleDestroyed;
				this.Box.DrawItem -= this.DrawItemHandler;
				this.Box.KeyDown -= this.KeyDownHandler;

				using var boxContextMenuStrip = this.Box.ContextMenuStrip;

				if ( boxContextMenuStrip != null ) {
					boxContextMenuStrip.Nop();
					boxContextMenuStrip.Items.Clear();

					//TODO boxContextMenu.Popup -= this.CopyMenuPopupHandler;
				}

				this.Box.Items.Clear();
				this.Box.DrawMode = DrawMode.Normal;
				using ( this.HackFont ) { }
			}
		}

		public void LogCritical( String? message ) {
			if ( this.CanAdd ) {
				LogEvent logEvent = new(LogLevel.Critical, message);
				this.Box.InvokeAction( () => this.AddALogEntry( logEvent ), RefreshOrInvalidate.Refresh );
			}
		}

		/// <summary>
		///     Write a <paramref name="message" /> with <see cref="LogLevel.Debug" />.
		/// </summary>
		/// <param name="message"></param>
		public void LogLine( String? message ) {
			if ( this.CanAdd ) {
				LogEvent logEvent = new(LogLevel.Debug, message);
				this.Box.InvokeAction( () => this.AddALogEntryLine( logEvent ), RefreshOrInvalidate.Refresh );
			}
		}

		/// <summary>
		///     ABetterRecordDispose is just fluff. (just want to see it in "action"..)
		/// </summary>
		private record LogEvent( LogLevel LogLevel, String? Message ) : ABetterRecordDispose {

			public DateTime EventTime { get; } = DateTime.Now;

		}

	}

}