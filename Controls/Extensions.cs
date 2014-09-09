#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// "Librainian/Extensions.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM

#endregion License & Information

namespace Librainian.Controls {

    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Annotations;
    using FluentAssertions;
    using Measurement.Time;
    using Threading;
    using Timer = System.Timers.Timer;

    public static class Extensions {

        /// <summary>
        ///     Just changes the cursor to the <see cref="Cursors.WaitCursor" />.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static void BusyCursor( [CanBeNull] this Control control ) {
            ThreadingExtensions.Wrap( () => {
                if ( control != null ) {
                    control.InvokeIfRequired( () => {
                        control.Cursor = Cursors.WaitCursor;
                    } );
                }
            } );
        }

        /// <summary>
        ///    <para>Perform an <see cref="Action" /> on the control's thread and then <see cref="Control.Refresh" />.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        /// <param name="refresh"></param>
        /// <seealso cref="InvokeIfRequired"/>
        public static void OnThread( [CanBeNull] this Control control, [CanBeNull] Action action, Boolean refresh = true ) {
            if ( null == control ) {
                return;
            }
            if ( null == action ) {
                return;
            }
            control.InvokeIfRequired( () => {
                action();
                if ( refresh ) {
                    control.Refresh();
                }
            } );
        }

        /// <summary>
        /// <para>Perform an <see cref="Action" /> on the control's thread and then <see cref="Control.Refresh" />.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        /// <param name="refresh"></param>
        /// <seealso />
        public static void InvokeIfRequired( [NotNull] this Control control, [NotNull] Action action, Boolean refresh = true ) {
            if ( control == null ) {
                throw new ArgumentNullException( "control" );
            }
            if ( action == null ) {
                throw new ArgumentNullException( "action" );
            }
            if ( control.IsDisposed ) {
                return;
            }
            if ( control.InvokeRequired ) {
                if ( refresh ) {
                    action += control.Refresh;  //BUG does this work like this?
                }
                control.BeginInvoke( action );
            }
            else {
                if ( control.IsDisposed ) {
                    return;
                }
                action();
                if ( !control.IsDisposed && refresh ) {
                    control.Refresh();
                }
            }
        }

        /// <summary>
        ///     Threadsafe <see cref="CheckBox.Checked" /> check.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Boolean Checked( [CanBeNull] this CheckBox control ) {
            if ( null == control ) {
                return false;
            }
            return control.InvokeRequired ? ( Boolean )control.Invoke( new Func<bool>( () => control.Checked ) ) : control.Checked;
        }

        /// <summary>
        ///     Threadsafe <see cref="Control.ForeColor" /> check.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Color ForeColor( [CanBeNull] this Control control ) {
            if ( null == control ) {
                return default( Color );
            }
            return control.InvokeRequired ? ( Color )control.Invoke( new Func<Color>( () => control.ForeColor ) ) : control.ForeColor;
        }

        /// <summary>
        ///     Threadsafe get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static int Maximum( [CanBeNull] this ProgressBar control ) {
            if ( null == control ) {
                return 0;
            }
            return control.InvokeRequired ? ( int )control.Invoke( new Func<int>( () => control.Maximum ) ) : control.Maximum;
        }

        /// <summary>
        ///     Threadsafe <see cref="Control.Refresh" />.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static void Redraw( [CanBeNull] this Control control ) {
            if ( null == control ) {
                return;
            }
            control.InvokeIfRequired( control.Refresh );
        }

        /// <summary>
        ///     Just changes the cursor to the <see cref="Cursors.Default" />.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static void ResetCursor( [NotNull] this Control control ) {
            if ( control == null ) {
                throw new ArgumentNullException( "control" );
            }
            ThreadingExtensions.Wrap( () => control.OnThread( () => {
                control.ResetCursor();
                control.Invalidate( invalidateChildren: false );
            } ) );
        }

        /// <summary>
        ///     Safely get the <see cref="Form.Size" />() of a <see cref="Form" /> across threads.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Size Size( [CanBeNull] this Form form ) {
            if ( null == form ) {
                return new Size();
            }
            return form.InvokeRequired ? ( Size )form.Invoke( new Func<Size>( () => form.Size ) ) : form.Size;
        }

        /// <summary>
        ///     Safely perform the <see cref="ProgressBar.PerformStep" />  across threads.
        /// </summary>
        /// <param name="control"></param>
        public static void Step( [CanBeNull] this ProgressBar control ) {
            if ( null == control ) {
                return;
            }
            control.OnThread( () => {
                if ( control.IsDisposed ) {
                    return;
                }
                control.PerformStep();
                control.Refresh();
            } );
        }

        /// <summary>
        ///     Safely perform the <see cref="ProgressBar.PerformStep" />  across threads.
        /// </summary>
        /// <param name="control"></param>
        public static void Step( [CanBeNull] this ToolStripProgressBar control ) {
            if ( null == control ) {
                return;
            }
            control.OnThread( () => {
                if ( control.IsDisposed ) {
                    return;
                }
                control.PerformStep();
                if ( !control.IsDisposed && null != control.ProgressBar ) {
                    control.ProgressBar.Refresh();
                }
            } );
        }

        /// <summary>
        ///     Perform an <see cref="Action" /> on a <see cref="ToolStripItem" />'s thread.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static void OnThread( [CanBeNull] this ToolStripItem control, [CanBeNull] Action action ) {
            if ( null == control ) {
                return;
            }
            if ( null == action ) {
                return;
            }
            var parent = control.GetCurrentParent() as Control;
            if ( null != parent ) {
                parent.OnThread( action );
            }
        }

        /// <summary>
        ///     <para>Safely get the <see cref="Control.Text" /> of a <see cref="Control" /> across threads.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static String Text( [CanBeNull] this Control control ) {
            if ( null == control ) {
                return String.Empty;
            }
            return control.InvokeRequired ? control.Invoke( new Func<string>( () => control.Text ) ) as String ?? String.Empty : control.Text;
        }

        /// <summary>
        ///     Threadsafe Value get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Decimal Value( [CanBeNull] this NumericUpDown control ) {
            if ( null == control ) {
                return Decimal.Zero;
            }
            return control.InvokeRequired ? ( Decimal )control.Invoke( new Func<decimal>( () => control.Value ) ) : control.Value;
        }

        /// <summary>
        ///     Threadsafe Value get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static int Value( [CanBeNull] this ProgressBar control ) {
            if ( null == control ) {
                return 0;
            }
            return control.InvokeRequired ? ( int )control.Invoke( new Func<int>( () => control.Value ) ) : control.Value;
        }

        /// <summary>
        ///     Safely set the <see cref="CheckBox.Checked" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Checked( [CanBeNull] this CheckBox control, Boolean value ) {
            if ( null == control ) {
                return;
            }
            if ( control.InvokeRequired ) {
                control.BeginInvoke( new Action( () => {
                    control.Checked = value;
                    control.Refresh();
                } ) );
            }
            else {
                control.Checked = value;
                control.Refresh();
            }
        }

        /// <summary>
        ///     Safely set the <see cref="Control.ForeColor" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void ForeColor( [CanBeNull] this Control control, Color value ) {
            if ( null == control ) {
                return;
            }
            if ( control.InvokeRequired ) {
                control.BeginInvoke( new Action( () => {
                    control.ForeColor = value;
                    control.Refresh();
                } ) );
            }
            else {
                control.ForeColor = value;
                control.Refresh();
            }
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Enabled" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        /// <param name="refresh"></param>
        public static void Enabled( this Control control, Boolean value, Boolean refresh = true ) {
            if ( null == control ) {
                return;
            }
            control.InvokeIfRequired( () => {
                if ( control.IsDisposed ) {
                    return;
                }
                control.Enabled = value;
                if ( refresh ) {
                    control.Refresh();
                }
            } );
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Enabled" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Enabled( this ToolStripProgressBar control, Boolean value ) {
            if ( null == control ) {
                return;
            }
            if ( null == control.ProgressBar ) {
                return;
            }
            if ( control.ProgressBar.InvokeRequired ) {
                control.ProgressBar.BeginInvoke( new Action( () => {
                    if ( control.IsDisposed ) {
                        return;
                    }
                    control.Enabled = value;
                    control.ProgressBar.Refresh();
                } ) );
            }
            else {
                control.Enabled = value;
                control.ProgressBar.Refresh();
            }
        }

        [DllImport( "user32.dll" )]
        public static extern IntPtr GetSystemMenu( this IntPtr hwndValue, Boolean isRevert );

        /// <summary>
        ///     Safely set the <see cref="Control.Text" /> of a control across threads.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void Location( [CanBeNull] this Form form, Point location ) {
            if ( null == form ) {
                return;
            }
            form.InvokeIfRequired( () => {
                if ( form.IsDisposed ) {
                    return;
                }
                form.Location = location;
            } );
        }

        /*
                public static void OnThread( [CanBeNull] this Form form, [CanBeNull] Action action ) {
                    if ( null == form ) {
                        return;
                    }
                    if ( null == action ) {
                        return;
                    }
                    form.InvokeIfRequired( action );
                }
        */

        /*
                public static void OnThread( [CanBeNull] this Control control, [CanBeNull] Action<Control> action ) {
                    if ( null == control ) {
                        return;
                    }
                    if ( null == action ) {
                        return;
                    }
                    if ( control.InvokeRequired ) {
                        control.BeginInvoke( action, control );
                        control.Invalidate();
                    }
                    else {
                        action( control );
                        control.Invalidate();
                    }
                }
        */

        public static void Output( this WebBrowser browser, String message ) {
            if ( browser == null ) {
                return;
            }
            if ( browser.InvokeRequired ) {
                browser.BeginInvoke( new Action( () => CreateDivInsideBrowser( ref browser, message ) ) );
            }
            else {
                CreateDivInsideBrowser( ref browser, message );
            }
        }

        private static Boolean CreateDivInsideBrowser( ref WebBrowser browser, String message ) {
            try {
                if ( null == browser ) {
                    return false;
                }

                while ( null == browser.Document ) {
                    Application.DoEvents();
                }

                var div = browser.Document.CreateElement( "DIV" );

                var span = browser.Document.CreateElement( "SPAN" );
                if ( message.StartsWith( "ECHO:" ) ) {
                    if ( span != null ) {
                        span.InnerText = message.Replace( "ECHO:", String.Empty );
                        span.Style = "font-variant:small-caps; font-size:small";
                    }
                }
                else if ( message.StartsWith( "INFO:" ) ) {
                    message = message.Replace( "INFO:", String.Empty );
                    if ( message.StartsWith( "<" ) ) {
                        if ( span != null ) {
                            span.InnerHtml = message;
                            span.Style = "font-style: oblique; font-size:xx-small";
                        }
                    }
                    else {
                        if ( span != null ) {
                            span.InnerText = message;
                            span.Style = "font-style: oblique; font-size:xx-small";
                        }
                    }
                }
                else {
                    if ( span != null ) {
                        span.InnerText = message;
                        span.Style = "font-style:normal;font-size:small;font-family:Comic Sans MS;";
                    }
                }

                if ( div != null ) {
                    if ( span != null ) {
                        div.AppendChild( span );
                    }
                    while ( null == browser.Document.Body ) {
                        Application.DoEvents();
                    }
                    browser.Document.Body.AppendChild( div );
                    div.ScrollIntoView( false );
                }
                browser.Update();

                //Application.DoEvents();

                return true;
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            return false;
        }

        /// <summary>
        ///     <para>A threadsafe <see cref="Button.PerformClick" />.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        /// <seealso cref="Push"/>
        public static void PerformClickThreadSafe( [CanBeNull] this Button control, TimeSpan? delay = null ) {
            control.Push( delay );
        }

        /// <summary>
        ///     <para>A threadsafe <see cref="Button.PerformClick" />.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static void Push( [CanBeNull] this Button control, TimeSpan? delay = null ) {
            if ( control == null ) {
                return;
            }
            if ( !delay.HasValue ) {
                delay = Milliseconds.One;
            }
            ( ( Span )delay.Value ).Create( () => control.InvokeIfRequired( control.PerformClick ) ).AndStart();
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Reset( [CanBeNull] this ProgressBar control, int? value = null ) {
            if ( null == control ) {
                return;
            }
            control.Value( value ?? control.Minimum() );
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Value( [CanBeNull] this ProgressBar control, int value ) {
            if ( null == control ) {
                return;
            }
            control.OnThread( () => {
                if ( control.IsDisposed ) {
                    return;
                }
                if ( value > control.Maximum ) {
                    control.Maximum = value;
                }
                else if ( value < control.Minimum ) {
                    control.Minimum = value;
                }
                control.Value = value;
                control.Refresh();
            } );
        }

        /// <summary>
        ///     Threadsafe  get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static int Minimum( [CanBeNull] this ProgressBar control ) {
            if ( null == control ) {
                return 0;
            }
            return control.InvokeRequired ? ( int )control.Invoke( new Func<int>( () => control.Minimum ) ) : control.Minimum;
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Text" /> of a control across threads.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void Size( [CanBeNull] this Form form, Size size ) {
            if ( null == form ) {
                return;
            }
            form.InvokeIfRequired( () => {
                if ( form.IsDisposed ) {
                    return;
                }
                form.Size = size;
            } );
        }

        /// <summary>
        ///     Safely set the <see cref="ToolStripItem.Text" /> of the control across threads.
        /// </summary>
        /// <param name="toolStripItem"></param>
        /// <param name="value"></param>
        public static void Text( [CanBeNull] this ToolStripItem toolStripItem, [CanBeNull] String value ) {
            if ( null == toolStripItem ) {
                return;
            }
            if ( toolStripItem.IsDisposed ) {
                return;
            }

            toolStripItem.OnThread( () => {
                if ( toolStripItem.IsDisposed ) {
                    return;
                }
                toolStripItem.Text = value;
                toolStripItem.Invalidate();
            } );
        }

        public static void TextAdd( [CanBeNull] this RichTextBox textBox, [CanBeNull] String message ) {
            if ( textBox == null ) {
                return;
            }
            if ( message == null ) {
                return;
            }
            var method = new Action( () => {
                if ( textBox.IsDisposed ) {
                    return;
                }

                textBox.AppendText( message );

                var lines = textBox.Lines.ToList();
                if ( lines.Count > 20 ) {
                    while ( lines.Count > 20 ) {
                        lines.RemoveAt( 0 );
                    }
                    textBox.Lines = lines.ToArray();
                }

                //if ( textBox.Text.Length > 0 ) {textBox.SelectionStart = textBox.Text.Length - 1;}
                //textBox.SelectionLength = message.Length;
                //textBox.SelectionBackColor = Color.BlanchedAlmond;
                //textBox.ShowSelectionMargin = true;

                //if ( italic ) {
                //    var style = textBox.SelectionFont.Style;
                //    style |= FontStyle.Italic;
                //    textBox.SelectionFont = new Font( textBox.SelectionFont, style );
                //}
                //textBox.ScrollToCaret();
                textBox.Invalidate();
            } );

            if ( textBox.IsDisposed ) {
                return;
            }
            if ( textBox.InvokeRequired ) {
                textBox.BeginInvoke( method );
            }
            else {
                method();
            }
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Enabled" /> and <see cref="Control.Visible" /> of a control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Usable( this Control control, Boolean value ) {
            if ( null == control ) {
                return;
            }
            if ( control.InvokeRequired ) {
                control.BeginInvoke( new Action( () => {
                    if ( control.IsDisposed ) {
                        return;
                    }
                    control.Visible = value;
                    control.Enabled = value;
                    control.Refresh();
                } ) );
            }
            else {
                control.Visible = value;
                control.Enabled = value;
                control.Refresh();
            }
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Visible" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Visible( this Control control, Boolean value ) {
            if ( null == control ) {
                return;
            }
            if ( control.InvokeRequired ) {
                control.BeginInvoke( new Action( () => {
                    if ( control.IsDisposed ) {
                        return;
                    }
                    control.Visible = value;
                    control.Refresh();
                } ) );
            }
            else {
                control.Visible = value;
                control.Refresh();
            }
        }

        [DllImport( "user32.dll" )]
        public static extern int EnableMenuItem( this IntPtr tMenu, int targetItem, int targetStatus );

        public static async void Marquee( [CanBeNull] this Control control, TimeSpan timeSpan, [CanBeNull] String message ) {
            control.Text( message );
            var until = DateTime.Now.Add( timeSpan );
            await Task.Run( () => {
                var stopwatch = Stopwatch.StartNew();
                do {
                    stopwatch.Restart();
                    control.Flash();
                    stopwatch.Stop();
                    //await Task.Delay( stopwatch.Elapsed );
                } while ( DateTime.Now < until );
            } );
        }

        /// <summary>
        ///     <para>Safely set the <see cref="Control.Text" /> of a control across threads.</para>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="control"></param>
        /// <param name="value"></param>
        /// <seealso cref="http://kristofverbiest.blogspot.com/2007/02/don-confuse-controlbegininvoke-with.html" />
        /// <seealso
        ///     cref="http://programmers.stackexchange.com/questions/114605/how-will-c-5-async-support-help-ui-thread-synchronization-issues" />
        public static void Text( [CanBeNull] this Control control, [CanBeNull] String value ) {
            if ( null == control ) {
                return;
            }
            control.InvokeIfRequired( () => {
                if ( control.IsDisposed ) {
                    return;
                }
                control.Text = value;
            } );
        }

        /// <summary>
        ///     Flashes the control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="spanOff">How long to keep the control off before it resets.</param>
        [CanBeNull]
        public static Timer Flash( [CanBeNull] this Control control, [CanBeNull] Span? spanOff = null ) {
            if ( null == control ) {
                return null;
            }
            if ( !spanOff.HasValue ) {
                spanOff = Milliseconds.Hertz111;
            }
            control.OnThread( () => {
                var foreColor = control.ForeColor;
                control.ForeColor = control.BackColor;
                control.BackColor = foreColor;
                control.Refresh();
            } );
            return spanOff.Value.Create( () => control.OnThread( () => {
                control.ResetForeColor();
                control.ResetBackColor();
                control.Refresh();
            } ) ).Once().AndStart();
        }

        public static Boolean RemoveTags( this WebBrowser browser, String tagName, int keepAtMost = 50 ) {
            if ( null == browser ) {
                return false;
            }
            if ( null == browser.Document ) {
                return false;
            }
            while ( null != browser.Document && browser.Document.GetElementsByTagName( tagName ).Count > keepAtMost ) {
                var item = browser.Document.GetElementsByTagName( tagName )[ 0 ];
                item.OuterHtml = String.Empty;
                browser.BeginInvoke( new Action( browser.Update ) );
            }

            return true;
        }

        public static void TextAdd( [NotNull] this RichTextBox textBox, [NotNull] String text, Color color ) {
            if ( textBox == null ) {
                throw new ArgumentNullException( "textBox" );
            }
            if ( text == null ) {
                throw new ArgumentNullException( "text" );
            }
            textBox.SelectionStart = textBox.TextLength;
            textBox.SelectionLength = 0;

            textBox.SelectionColor = color;
            textBox.AppendText( text );
            textBox.SelectionColor = textBox.ForeColor;
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="minimum"></param>
        /// <param name="value"></param>
        /// <param name="maximum"></param>
        public static void Values( [CanBeNull] this ProgressBar control, int minimum, int value, int maximum ) {
            if ( null == control ) {
                return;
            }
            minimum.Should().BeLessOrEqualTo( maximum );
            value.Should().BeLessOrEqualTo( maximum );
            var lowEnd = Math.Min( minimum, maximum );
            var highEnd = Math.Max( minimum, maximum );
            control.Minimum( lowEnd );
            control.Maximum( highEnd );
            control.Value( value );
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="minimum"></param>
        /// <param name="value"></param>
        /// <param name="maximum"></param>
        /// <seealso cref="Values"/>
        public static void Set( [CanBeNull] this ProgressBar control, int minimum, int value, int maximum ) {
            control.Values( minimum: minimum, value: value, maximum: maximum );
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Minimum" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Minimum( [CanBeNull] this ProgressBar control, int value ) {
            if ( null == control ) {
                return;
            }
            control.InvokeIfRequired( () => {
                if ( control.IsDisposed ) {
                    return;
                }
                control.Minimum = value;
                control.Refresh();
            } );
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Step" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Step( [CanBeNull] this ProgressBar control, int value ) {
            if ( null == control ) {
                return;
            }
            control.OnThread( () => {
                if ( control.IsDisposed ) {
                    return;
                }
                control.Step = value;
                control.Refresh();
            } );
        }


        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Style" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Style( [CanBeNull] this ProgressBar control, ProgressBarStyle value ) {
            if ( null == control ) {
                return;
            }
            control.OnThread( () => {
                if ( control.IsDisposed ) {
                    return;
                }
                control.Style = value;
                control.Refresh();
            } );
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Maximum" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Maximum( [CanBeNull] this ProgressBar control, int value ) {
            if ( null == control ) {
                return;
            }
            control.OnThread( () => {
                if ( control.IsDisposed ) {
                    return;
                }
                control.Maximum = value;
                control.Refresh();
            } );
        }

        //[Obsolete( "Untested" )]

        /// <summary>
        ///     Why?
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static async Task<string> StringAsync( String a ) {
            return await Task<string>.Factory.StartNew( () => a );
        }

        /*
        private static void CreateLabelControlInsideFlow( ref FlowLayoutPanel flow, String message ) {
            try {
                if ( flow == null ) {
                    return;
                }
                if ( flow.VerticalScroll.Visible && flow.Controls.Count > 50 ) {
                    Control oldest = null;
                    foreach ( Control control in flow.Controls ) {
                        if ( !( control.Tag is DateTime ) ) { continue; }
                        var when = ( DateTime )control.Tag;
                        if ( null == oldest ) {
                            oldest = control;
                        }
                        else {
                            if ( when < ( DateTime )oldest.Tag ) {
                                oldest = control;
                            }
                        }
                    }
                    if ( null != oldest ) {
                        flow.Controls.Remove( oldest );
                    }
                }

                var temp = new Label {
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding( 0 ),
                    Margin = new Padding( 0 ),
                    Font = new Font( FontFamily.GenericSansSerif, 3, FontStyle.Regular, GraphicsUnit.Millimeter ),
                    BorderStyle = BorderStyle.None,
                    AutoSize = false,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                    Tag = DateTime.UtcNow
                };
                temp.Width = temp.Parent.Width; // -( temp.Parent.Width / 5 );
                flow.Controls.Add( temp );
                temp.SetText( message );
                flow.AutoScroll = true;
                flow.ScrollControlIntoView( temp );
                flow.Update();

                return;
            }
            catch ( Exception error ) {
                Utility.LogException( error );
                return;
            }
        }
        */

        /*
        public static void Output( this FlowLayoutPanel flow, String message ) {
            var mainForm = flow.FindForm() as MainForm;
            if ( mainForm == null ) return;
            if ( flow.InvokeRequired ) {
                flow.BeginInvoke( new Action( () => CreateLabelControlInsideFlow( ref flow, message ) ) );
            }
            else {
                CreateLabelControlInsideFlow( ref flow, message );
            }
        }
        */

        //public static Boolean RemoveTags( this WebControl browser, String tagName, int keepAtMost = 50 ) {
        //    if ( null == browser ) {
        //        return false;
        //    }
        //    //TODO
        //    //if ( null == browser.rem ) {
        //    //    return false;
        //    //}
        //    //while ( null != browser.Document && browser.Document.GetElementsByTagName( tagName ).Count > keepAtMost ) {
        //    //    var item = browser.Document.GetElementsByTagName( tagName )[ 0 ];
        //    //    item.OuterHtml = String.Empty;
        //    //    browser.BeginInvoke( new Action( browser.Update ) );
        //    //}

        //    return true;
        //}

        //private static Boolean CreateDivInsideBrowser( ref WebControl webbrowser, String message ) {
        //    try {
        //        if ( null == webbrowser ) {
        //            return false;
        //        }

        //        //webbrowser.cl
        //        webbrowser.LoadHTML( String.Format( "<div><span>{0}</span></div><br/>{1}", message, Environment.NewLine ) );
        //        //webbrowser.LoadCompleted += ( sender, args ) => {  };
        //        while ( webbrowser.IsLoadingPage ) { WebCore.Update(); }

        //        var script =
        //            "for (var i = 0; i < 30; i++) {" +
        //                 "var newdiv = document.createElement('div');" +
        //                 "var txt = document.createTextNode('This text was added to the DIV for i = ' + i + '.');" +
        //                 "newdiv.appendChild(txt);" +
        //                 "document.getElementById('myDiv').appendChild(newdiv); " +
        //             "}";
        //        webbrowser.ExecuteJavascript( script );
        //        //webbrowser.Focus();
        //        while ( webbrowser.IsLoadingPage ) { WebCore.Update(); }

        //        //webbrowser.LoadHTML( message );
        //        //var html = webbrowser.Text;
        //        //var html = webbrowser.PageContents;

        //        //while ( !webbrowser.IsDomReady ) { Thread.Yield(); Thread.Sleep( 0 ); }
        //        //if ( !webbrowser.IsLive ) { return false; }
        //        //if ( !webbrowser.IsLoadingPage ) { return false; }

        //        //bob.ToString().TimeDebug();

        //        //while ( null == webbrowser.Document ) {
        //        //    Application.DoEvents();
        //        //}

        //        //var div = webbrowser.LoadHTML( .Document.CreateElement( "DIV" );

        //        //var span = webbrowser.Document.CreateElement( "SPAN" );
        //        //if ( message.StartsWith( "ECHO:" ) ) {
        //        //    if ( span != null ) {
        //        //        span.InnerText = message.Replace( "ECHO:", String.Empty );
        //        //        span.Style = "font-variant:small-caps; font-size:small";
        //        //    }
        //        //}
        //        //else if ( message.StartsWith( "INFO:" ) ) {
        //        //    message = message.Replace( "INFO:", String.Empty );
        //        //    if ( message.StartsWith( "<" ) ) {
        //        //        if ( span != null ) {
        //        //            span.InnerHtml = message;
        //        //            span.Style = "font-style: oblique; font-size:xx-small";
        //        //        }
        //        //    }
        //        //    else {
        //        //        if ( span != null ) {
        //        //            span.InnerText = message;
        //        //            span.Style = "font-style: oblique; font-size:xx-small";
        //        //        }
        //        //    }
        //        //}
        //        //else {
        //        //    if ( span != null ) {
        //        //        span.InnerText = message;
        //        //        span.Style = "font-style:normal;font-size:small;font-family:Comic Sans MS;";
        //        //    }
        //        //}

        //        //if ( div != null ) {
        //        //    if ( span != null ) {
        //        //        div.AppendChild( span );
        //        //    }
        //        //    while ( null == webbrowser.Document.Body ) {
        //        //        Application.DoEvents();
        //        //    }
        //        //    webbrowser.Document.Body.AppendChild( div );
        //        //    div.ScrollIntoView( false );
        //        //}
        //        //webbrowser.Update();

        //        //Application.DoEvents();

        //        return true;
        //    }
        //    catch ( Exception exception ) {
        //        exception.Error();
        //    }
        //    return false;
        //}

        //public static void Write( this RichTextBox textBox, String message, Boolean italic = false ) {
        //    if ( textBox == null ) {
        //        return;
        //    }
        //    var method = new Action( () => {
        //        if ( italic ) {
        //            textBox.SelectionStart = textBox.Text.Length;
        //            textBox.SelectionLength = message.Length;
        //            var style = textBox.SelectionFont.Style;
        //            style |= FontStyle.Italic;
        //            textBox.SelectionFont = new Font( textBox.SelectionFont, style );
        //        }
        //        textBox.AppendText( message );
        //        textBox.SelectionStart = textBox.Text.Length;
        //        textBox.ScrollToCaret();
        //        textBox.Invalidate();
        //    } );
        //    if ( textBox.InvokeRequired ) {
        //        textBox.BeginInvoke( method );
        //    }
        //    else {
        //        method();
        //    }
        //}

        //public static void WriteLine( this RichTextBox textBox, String message ) { AppendToTextBox( textBox, message, false ); }

        //public static void Output( this WebControl webbrowser, String message ) {
        //    if ( webbrowser == null ) {
        //        return;
        //    }
        //    //webbrowser.on
        //    if ( webbrowser.InvokeRequired ) {
        //        webbrowser.BeginInvoke( new Action( () => CreateDivInsideBrowser( ref webbrowser, message ) ) );
        //    }
        //    else {
        //        CreateDivInsideBrowser( ref webbrowser, message );
        //    }
        //}

        public static void InvokeA<T>( this T invokable, Action<T> action, T argument = default( T ) ) where T : ISynchronizeInvoke {
            try {
                if ( Equals( invokable, default( T ) ) ) {
                    return;
                }
                if ( invokable is Control && ( invokable as Control ).IsDisposed ) {
                    return;
                }
                if ( invokable.InvokeRequired ) {
                    invokable.Invoke( action, new object[] { argument } );
                }
                else {
                    action( argument );
                }
            }
            catch ( ObjectDisposedException exception ) {
                exception.Error();
            }
        }

        public static T InvokeF<T>( this T invokable, Func<T> function, T argument = default( T ) ) where T : class, ISynchronizeInvoke {
            if ( invokable.InvokeRequired ) {
                if ( invokable is Control && ( invokable as Control ).IsDisposed ) {
                }
                else {
                    return invokable.Invoke( function, new object[] { argument } ) as T;
                }
            }
            return function();
        }
    }
}