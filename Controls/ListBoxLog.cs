// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ListBoxLog.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Controls {

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using Maths;

    /// <summary>
    ///     Pulled from http://stackoverflow.com/a/6587172/956364
    /// </summary>
    public sealed class ListBoxLog : IDisposable {
        private const Int32 DefaultMaxLinesInListbox = 2000;

        /// <summary>
        ///     <seealso cref="FormatALogEventMessage" />
        /// </summary>
        private const String DefaultMessageFormat = "{4}>{8}";

        public ListBoxLog( ListBox listBox, String messageFormat ) : this( listBox, messageFormat, DefaultMaxLinesInListbox ) {
        }

        public ListBoxLog( ListBox listBox, String messageFormat = DefaultMessageFormat, Int32 maxLinesInListbox = DefaultMaxLinesInListbox ) {
            this.Disposed = false;

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

            var menuItems = new[] { new MenuItem( "Copy", this.CopyMenuOnClickHandler ) };
            this.Box.ContextMenu = new ContextMenu( menuItems );
            this.Box.ContextMenu.Popup += this.CopyMenuPopupHandler;

            this.Box.DrawMode = DrawMode.OwnerDrawFixed;
        }

        ~ListBoxLog() {
            if ( this.Disposed ) {
                return;
            }
            this.Dispose( false );
            this.Disposed = true;
        }

        private delegate void AddALogEntryDelegate( Object item );

        public Boolean Paused {
            get;
        }

        private ListBox Box {
            get; set;
        }

        private Boolean CanAdd {
            get; set;
        }

        private Boolean Disposed {
            get; set;
        }

        private Int32 MaxEntriesInListBox {
            get;
        }

        private String MessageFormat {
            get;
        }

        public void Dispose() {
            if ( this.Disposed ) {
                return;
            }
            this.Dispose( true );
            GC.SuppressFinalize( this );
            this.Disposed = true;
        }

        public void Log( String message ) {
            this.WriteEvent( new LogEvent( LoggingLevel.Critical, message ) );
        }

        public void LogLine( String message ) {
            this.LogLine( LoggingLevel.Debug, message );
        }

        public void LogLine( String format, params Object[] args ) {
            this.LogLine( LoggingLevel.Debug, format == null ? null : String.Format( format, args ) );
        }

        public void LogLine( LoggingLevel loggingLevel, String format, params Object[] args ) {
            this.LogLine( loggingLevel, format == null ? null : String.Format( format, args ) );
        }

        public void LogLine( LoggingLevel loggingLevel, String message ) {
            this.WriteEventLine( new LogEvent( loggingLevel, message ) );
        }

        private static String FormatALogEventMessage( LogEvent logEvent, String messageFormat ) {
            var message = logEvent.Message ?? "<NULL>";
            return String.Format( messageFormat, /* {0} */ logEvent.EventTime.ToString( "yyyy-MM-dd HH:mm:ss.fff" ), /* {1} */ logEvent.EventTime.ToString( "yyyy-MM-dd HH:mm:ss" ), /* {2} */ logEvent.EventTime.ToString( "yyyy-MM-dd" ), /* {3} */ logEvent.EventTime.ToString( "HH:mm:ss.fff" ), /* {4} */ logEvent.EventTime.ToString( "HH:mm" ), /* {5} */ LevelName( logEvent.LoggingLevel )[ 0 ], /* {6} */ LevelName( logEvent.LoggingLevel ), /* {7} */ ( Int32 )logEvent.LoggingLevel, /* {8} */ message );
        }

        private static String LevelName( LoggingLevel loggingLevel ) {
            switch ( loggingLevel ) {
                case LoggingLevel.Critical:
                    return "Critical";

                case LoggingLevel.Error:
                    return "Error";

                case LoggingLevel.Warning:
                    return "Warning";

                case LoggingLevel.Info:
                    return "Info";

                case LoggingLevel.Verbose:
                    return "Verbose";

                case LoggingLevel.Debug:
                    return "Debug";

                default:
                    return $"<value={( Int32 )loggingLevel}>";
            }
        }

        private void AddALogEntry( Object item ) {
            var items = this.Box.Items;
            if ( items.Count == 0 ) {
                AddALogEntryLine( item );
                return;
            }
            var currentText = items[ items.Count - 1 ] as String ?? String.Empty;
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

        private void CopyMenuOnClickHandler( Object sender, EventArgs e ) {
            this.CopyToClipboard();
        }

        private void CopyMenuPopupHandler( Object sender, EventArgs e ) {
            var menu = sender as ContextMenu;
            if ( menu != null ) {
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
                selectedItemsAsRTFText.AppendFormat( @"{{\f0\fs16\chshdng0\chcbpat{0}\cb{0}\cf{1} ", logEvent.LoggingLevel == LoggingLevel.Critical ? 2 : 1, logEvent.LoggingLevel == LoggingLevel.Critical ? 1 : ( Int32 )logEvent.LoggingLevel > 5 ? 6 : ( Int32 )logEvent.LoggingLevel + 1 );
                selectedItemsAsRTFText.Append( FormatALogEventMessage( logEvent, this.MessageFormat ) );
                selectedItemsAsRTFText.AppendLine( @"\par}" );
            }

            selectedItemsAsRTFText.AppendLine( @"}" );
            Debug.WriteLine( selectedItemsAsRTFText.ToString() );
            Clipboard.SetData( DataFormats.Rtf, selectedItemsAsRTFText.ToString() );
        }

        private void Dispose( Boolean disposing ) {
            if ( this.Box == null ) {
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
        }

        private void DrawItemHandler( Object sender, DrawItemEventArgs e ) {
            if ( e.Index < 0 ) {
                return;
            }

            e.DrawBackground();
            e.DrawFocusRectangle();

            var logEvent = ( ( ListBox )sender ).Items[ e.Index ] as LogEvent ?? new LogEvent( LoggingLevel.Critical, ( ( ListBox )sender ).Items[ e.Index ].ToString() );

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

        private void KeyDownHandler( Object sender, KeyEventArgs e ) {
            if ( ( e.Modifiers == Keys.Control ) && ( e.KeyCode == Keys.C ) ) {
                this.CopyToClipboard();
            }
        }

        private void OnHandleCreated( Object sender, EventArgs e ) {
            this.CanAdd = true;
        }

        private void OnHandleDestroyed( Object sender, EventArgs e ) {
            this.CanAdd = false;
        }

        private void WriteEvent( LogEvent logEvent ) {
            if ( ( logEvent != null ) && this.CanAdd ) {
                this.Box.BeginInvoke( new AddALogEntryDelegate( this.AddALogEntry ), logEvent );
            }
        }

        private void WriteEventLine( LogEvent logEvent ) {
            if ( ( logEvent != null ) && this.CanAdd ) {
                this.Box.BeginInvoke( new AddALogEntryDelegate( this.AddALogEntryLine ), logEvent );
            }
        }

        private class LogEvent {

            public LogEvent( LoggingLevel loggingLevel, String message ) {
                this.EventTime = DateTime.Now;
                this.LoggingLevel = loggingLevel;
                this.Message = message;
            }

            public DateTime EventTime {
                get;
            }

            public LoggingLevel LoggingLevel {
                get;
            }

            public String Message {
                get;
            }
        }
    }
}