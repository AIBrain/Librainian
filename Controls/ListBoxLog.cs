namespace Librainian.Controls {

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Pulled from http://stackoverflow.com/a/6587172/956364
    /// </summary>
    public sealed class ListBoxLog : IDisposable {
        private const Int32 DEFAULT_MAX_LINES_IN_LISTBOX = 2000;
        private const String DEFAULT_MESSAGE_FORMAT = "{0} [{5}] : {8}";

        public ListBoxLog( ListBox listBox, String messageFormat ) : this( listBox, messageFormat, DEFAULT_MAX_LINES_IN_LISTBOX ) {
        }

        public ListBoxLog( ListBox listBox, String messageFormat = DEFAULT_MESSAGE_FORMAT, Int32 maxLinesInListbox = DEFAULT_MAX_LINES_IN_LISTBOX ) {
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

        public Boolean Paused { get; }

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
            this.Log( LoggingLevel.Debug, message );
        }

        public void Log( String format, params Object[] args ) {
            this.Log( LoggingLevel.Debug, format == null ? null : String.Format( format, args ) );
        }

        public void Log( LoggingLevel loggingLevel, String format, params Object[] args ) {
            this.Log( loggingLevel, format == null ? null : String.Format( format, args ) );
        }

        public void Log( LoggingLevel loggingLevel, String message ) {
            this.WriteEvent( new LogEvent( loggingLevel, message ) );
        }

        private static String FormatALogEventMessage( LogEvent logEvent, String messageFormat ) {
            var message = logEvent.Message ?? "<NULL>";
            return String.Format( messageFormat,
                                  /* {0} */ logEvent.EventTime.ToString( "yyyy-MM-dd HH:mm:ss.fff" ),
                                  /* {1} */ logEvent.EventTime.ToString( "yyyy-MM-dd HH:mm:ss" ),
                                  /* {2} */ logEvent.EventTime.ToString( "yyyy-MM-dd" ),
                                  /* {3} */ logEvent.EventTime.ToString( "HH:mm:ss.fff" ),
                                  /* {4} */ logEvent.EventTime.ToString( "HH:mm:ss" ),

                                  /* {5} */ LevelName( logEvent.LoggingLevel )[ 0 ],
                                  /* {6} */ LevelName( logEvent.LoggingLevel ),
                                  /* {7} */ ( Int32 )logEvent.LoggingLevel,

                                  /* {8} */ message );
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
            this.Box.Items.Add( item );

            if ( this.Box.Items.Count > this.MaxEntriesInListBox ) {
                this.Box.Items.RemoveAt( 0 );
            }

            if ( !this.Paused )
                this.Box.TopIndex = this.Box.Items.Count - 1;
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
            if ( this.Box.SelectedItems.Count > 0 ) {
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
            if ( e.Index >= 0 ) {
                e.DrawBackground();
                e.DrawFocusRectangle();

                var logEvent = ( ( ListBox )sender ).Items[ e.Index ] as LogEvent;

                // SafeGuard against wrong configuration of list box
                if ( logEvent == null ) {
                    logEvent = new LogEvent( LoggingLevel.Critical, ( ( ListBox )sender ).Items[ e.Index ].ToString() );
                }

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
                e.Graphics.DrawString( FormatALogEventMessage( logEvent, this.MessageFormat ), new Font( "Lucida Console", 8.25f, FontStyle.Regular ), new SolidBrush( color ), e.Bounds );
            }
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