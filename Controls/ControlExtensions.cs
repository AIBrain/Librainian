// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ControlExtensions.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/ControlExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:39 PM.

namespace Librainian.Controls {

    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Persistence;
    using Threading;
    using Application = System.Windows.Forms.Application;
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;
    using Timer = System.Timers.Timer;

    public static class ControlExtensions {

        public static ConcurrentDictionary<Control, Int32> TurnOnOrOffReqests { get; } = new ConcurrentDictionary<Control, Int32>();

        public static void AppendLine( this RichTextBox box, String text, Color color, params Object[] args ) => box.AppendText( $"\n{text}", color == Color.Empty ? box.ForeColor : color, args );

        public static void AppendText( this RichTextBox box, String text, Color color, params Object[] args ) {
            text = String.Format( text, args );

            if ( color == Color.Empty ) {
                box.AppendText( text );

                return;
            }

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText( text );
            box.SelectionColor = box.ForeColor;

            box.SelectionStart = box.TextLength;
            box.ScrollToCaret();
        }

        public static Color Blend( this Color thisColor, Color blendToColor, Double blendToPercent ) {
            blendToPercent = ( 1 - blendToPercent ).ForceBounds( 0, 1 );

            var r = ( Byte )( thisColor.R * blendToPercent + blendToColor.R * ( 1 - blendToPercent ) );
            var g = ( Byte )( thisColor.G * blendToPercent + blendToColor.G * ( 1 - blendToPercent ) );
            var b = ( Byte )( thisColor.B * blendToPercent + blendToColor.B * ( 1 - blendToPercent ) );

            return Color.FromArgb( r, g, b );
        }

        /// <summary>
        ///     Just changes the cursor to the <see cref="Cursors.WaitCursor" />.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static void BusyCursor( [CanBeNull] this Control control ) => ThreadingExtensions.Wrap( () => control?.InvokeIfRequired( () => control.Cursor = Cursors.WaitCursor ) );

        /// <summary>
        ///     Threadsafe <see cref="CheckBox.Checked" /> check.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Boolean Checked( [CanBeNull] this CheckBox control ) {
            if ( null == control ) { return false; }

            return control.InvokeRequired ? ( Boolean )control.Invoke( new Func<Boolean>( () => control.Checked ) ) : control.Checked;
        }

        /// <summary>
        ///     Safely set the <see cref="CheckBox.Checked" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Checked( [CanBeNull] this CheckBox control, Boolean value ) {
            if ( null == control ) { return; }

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

        public static Boolean CreateDivInsideBrowser( ref WebBrowser browser, String message ) {
            try {
                if ( null == browser ) { return false; }

                while ( null == browser.Document ) { Application.DoEvents(); }

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
                    if ( span != null ) { div.AppendChild( span ); }

                    while ( null == browser.Document.Body ) { Application.DoEvents(); }

                    browser.Document.Body.AppendChild( div );
                    div.ScrollIntoView( false );
                }

                browser.Update();

                //Application.DoEvents();

                return true;
            }
            catch ( Exception exception ) { exception.More(); }

            return false;
        }

        /// <summary>
        ///     Returns a contrasting ForeColor for the specified BackColor. If the source BackColor is dark, then the
        ///     lightForeColor is returned. If the BackColor is light, then the darkForeColor is returned.
        /// </summary>
        public static Color DetermineForecolor( this Color thisColor, Color lightForeColor, Color darkForeColor ) {

            // Counting the perceptive luminance - human eye favors green color...
            var a = 1 - ( 0.299 * thisColor.R + 0.587 * thisColor.G + 0.114 * thisColor.B ) / 255;

            return a < 0.5 ? darkForeColor : lightForeColor;
        }

        /// <summary>
        ///     Returns a contrasting ForeColor for the specified BackColor. If the source BackColor is dark, then the White is
        ///     returned. If the BackColor is light, then the Black is returned.
        /// </summary>
        public static Color DetermineForecolor( this Color thisColor ) => DetermineForecolor( thisColor, Color.White, Color.Black );

        /// <summary>
        ///     Safely set the <see cref="Control.Enabled" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        /// <param name="refresh"></param>
        public static void Enabled( this Control control, Boolean value, Boolean refresh = true ) =>
            control?.InvokeIfRequired( () => {
                if ( control.IsDisposed ) { return; }

                control.Enabled = value;

                if ( refresh ) { control.Refresh(); }
            } );

        /// <summary>
        ///     Safely set the <see cref="Control.Enabled" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Enabled( this ToolStripProgressBar control, Boolean value ) {
            if ( control?.ProgressBar is null ) { return; }

            if ( control.ProgressBar.InvokeRequired ) {
                control.ProgressBar.BeginInvoke( new Action( () => {
                    if ( control.IsDisposed ) { return; }

                    control.Enabled = value;
                    control.ProgressBar.Refresh();
                } ) );
            }
            else {
                control.Enabled = value;
                control.ProgressBar.Refresh();
            }
        }

        /// <summary>
        ///     Flashes the control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="spanOff">How long to keep the control off before it resets.</param>
        [CanBeNull]
        public static Timer Flash( [CanBeNull] this Control control, [CanBeNull] TimeSpan? spanOff = null ) {
            if ( null == control ) { return null; }

            if ( !spanOff.HasValue ) { spanOff = Milliseconds.One; }

            control.OnThread( () => {
                var foreColor = control.ForeColor;
                control.ForeColor = control.BackColor;
                control.BackColor = foreColor;
                control.Refresh();
            } );

            return ( spanOff ?? Milliseconds.One ).CreateTimer( () => control.OnThread( () => {
                control.ResetForeColor();
                control.ResetBackColor();
                control.Refresh();
            } ) ).Once().AndStart();
        }

        public static async Task FlashWhileBlank( this Control input, [NotNull] Control control ) {
            if ( control is null ) { throw new ArgumentNullException( nameof( control ) ); }

            await Seconds.Five.Then( async () => {
                if ( !input.Text().IsNullOrWhiteSpace() ) { return; }

                control.Flash( Seconds.One );
                await input.FlashWhileBlank( control );
            } );
        }

        /// <summary>
        ///     Set <see cref="Control.Focus" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        public static void Fokus( [CanBeNull] this Control control ) =>
            control?.InvokeIfRequired( () => {
                if ( control.IsDisposed ) { return; }

                control.Focus();
            } );

        /// <summary>
        ///     Threadsafe <see cref="Control.ForeColor" /> check.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Color ForeColor( [CanBeNull] this Control control ) {
            if ( null == control ) { return default; }

            return control.InvokeRequired ? ( Color )control.Invoke( new Func<Color>( () => control.ForeColor ) ) : control.ForeColor;
        }

        /// <summary>
        ///     Safely set the <see cref="Control.ForeColor" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void ForeColor( [CanBeNull] this Control control, Color value ) {
            if ( null == control ) { return; }

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

        public static void FullScreen( this Window window ) {
            window.WindowState = WindowState.Maximized;
            window.WindowStyle = WindowStyle.None;
        }

        public static void InvokeA<T>( this T invokable, Action<T> action, T argument = default ) where T : ISynchronizeInvoke {
            try {
                if ( Equals( invokable, default ) ) { return; }

                if ( invokable is Control && ( invokable as Control ).IsDisposed ) { return; }

                if ( invokable.InvokeRequired ) { invokable.Invoke( action, new Object[] { argument } ); }
                else { action( argument ); }
            }
            catch ( ObjectDisposedException exception ) { exception.More(); }
        }

        public static void InvokeAppendLine( this RichTextBox box, String text, Color color, params Object[] args ) => box.Invoke( ( MethodInvoker )delegate { box.AppendLine( text, color, args ); } );

        public static void InvokeAppendText( this RichTextBox box, String text, Color color, params Object[] args ) => box.Invoke( ( MethodInvoker )delegate { box.AppendText( text, color, args ); } );

        public static T InvokeF<T>( this T invokable, Func<T> function, T argument = default ) where T : class, ISynchronizeInvoke {
            if ( invokable.InvokeRequired ) {
                if ( invokable is Control && ( invokable as Control ).IsDisposed ) { }
                else { return invokable.Invoke( function, new Object[] { argument } ) as T; }
            }

            return function();
        }

        /// <summary>
        ///     <para>Perform an <see cref="Action" /> on the control's thread and then <see cref="Control.Refresh" />.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"> </param>
        /// <param name="refresh"></param>
        /// <seealso />
        public static void InvokeIfRequired( [NotNull] this Control control, [NotNull] Action action, Boolean refresh = true ) {
            if ( control is null ) { throw new ArgumentNullException( nameof( control ) ); }

            if ( action is null ) { throw new ArgumentNullException( nameof( action ) ); }

            if ( control.IsDisposed ) { return; }

            if ( control.InvokeRequired ) {
                control.Invoke( action );

                if ( !refresh ) { return; }

                if ( control.IsDisposed ) { return; }

                action = control.Refresh;
                control.Invoke( action );
            }
            else {
                if ( control.IsDisposed ) { return; }

                action();

                if ( !control.IsDisposed && refresh ) { control.Refresh(); }
            }
        }

        public static Boolean IsFullScreen( this Window window ) => window.WindowState == WindowState.Maximized && window.WindowStyle == WindowStyle.None;

        public static Boolean IsMinimized( this Window window ) => window.WindowState == WindowState.Minimized;

        public static Boolean IsNormal( this Window window ) => window.WindowState == WindowState.Normal && window.WindowStyle != WindowStyle.None;

        public static void LoadPosition( [NotNull] this Form form, [CanBeNull] Ini settings ) {
            if ( form is null ) { throw new ArgumentNullException( nameof( form ) ); }

            if ( settings is null ) { throw new ArgumentNullException( nameof( settings ) ); }

            var name = form.Name ?? "UnknownForm";

            if ( Int32.TryParse( settings[name, nameof( Point.X )], out var x ) && Int32.TryParse( settings[name, nameof( Point.Y )], out var y ) ) {
                form.SuspendLayout();
                form.Location( new Point( x, y ) );
                form.ResumeLayout();
            }

            if ( Int32.TryParse( settings[name, nameof( form.Size.Width )], out var width ) && Int32.TryParse( settings[name, nameof( form.Size.Height )], out var height ) ) {
                form.SuspendLayout();
                form.Size( new Size( width, height ) );
                form.ResumeLayout();
            }
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Text" /> of a control across threads.
        /// </summary>
        /// <remarks></remarks>
        public static void Location( [CanBeNull] this Form form, Point location ) =>
            form?.InvokeIfRequired( () => {
                if ( form.IsDisposed ) { return; }

                form.Location = location;
            } );

        public static Color MakeDarker( this Color thisColor, Double darknessPercent ) {
            darknessPercent = darknessPercent.ForceBounds( 0, 1 );

            return Blend( thisColor, Color.Black, darknessPercent );
        }

        public static Color MakeLighter( this Color thisColor, Double lightnessPercent ) {
            lightnessPercent = lightnessPercent.ForceBounds( 0, 1 );

            return Blend( thisColor, Color.White, lightnessPercent );
        }

        public static Color MakeTransparent( this Color thisColor, Double transparentPercent ) {
            transparentPercent = 255 - transparentPercent.ForceBounds( 0, 1 ) * 255;

            return Color.FromArgb( thisColor.ToArgb() + ( Int32 )transparentPercent * 0x1000000 );
        }

        public static async Task MarqueeAsync( [CanBeNull] this Control control, TimeSpan timeSpan, [CanBeNull] String message ) {
            control.Text( message );
            var until = DateTime.Now.Add( timeSpan );

            await Task.Run( () => {
                var stopwatch = StopWatch.StartNew();

                do {
                    stopwatch.Restart();
                    control.Flash();
                    stopwatch.Stop();

                    //await Task.Delay( stopwatch.Elapsed );
                } while ( DateTime.Now < until );
            } );
        }

        /// <summary>
        ///     Threadsafe get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Int32 Maximum( [CanBeNull] this ProgressBar control ) {
            if ( null == control ) { return 0; }

            return control.InvokeRequired ? ( Int32 )control.Invoke( new Func<Int32>( () => control.Maximum ) ) : control.Maximum;
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Maximum" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Maximum( [CanBeNull] this ProgressBar control, Int32 value ) =>
            control?.OnThread( () => {
                if ( control.IsDisposed ) { return; }

                control.Maximum = value;
                control.Refresh();
            } );

        /// <summary>
        ///     Threadsafe get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Int32 Minimum( [CanBeNull] this ProgressBar control ) {
            if ( null == control ) { return 0; }

            return control.InvokeRequired ? ( Int32 )control.Invoke( new Func<Int32>( () => control.Minimum ) ) : control.Minimum;
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Minimum" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Minimum( [CanBeNull] this ProgressBar control, Int32 value ) =>
            control?.InvokeIfRequired( () => {
                if ( control.IsDisposed ) { return; }

                control.Minimum = value;
                control.Refresh();
            } );

        /// <summary>
        ///     <para>Perform an <see cref="Action" /> on the control's thread and then <see cref="Control.Refresh" />.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"> </param>
        /// <param name="refresh"></param>
        /// <seealso cref="InvokeIfRequired" />
        public static void OnThread( [CanBeNull] this Control control, [CanBeNull] Action action, Boolean refresh = true ) {
            if ( null == control ) { return; }

            if ( null == action ) { return; }

            control.InvokeIfRequired( action, refresh );
        }

        /// <summary>
        ///     Perform an <see cref="Action" /> on a <see cref="ToolStripItem" />'s thread.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"> </param>
        public static void OnThread( [CanBeNull] this ToolStripItem control, [CanBeNull] Action action ) {
            if ( null == control ) { return; }

            if ( null == action ) { return; }

            var parent = control.GetCurrentParent() as Control;
            parent?.OnThread( action );
        }

        public static void Output( this WebBrowser browser, String message ) {
            if ( browser is null ) { return; }

            if ( browser.InvokeRequired ) { browser.BeginInvoke( new Action( () => CreateDivInsideBrowser( ref browser, message ) ) ); }
            else { CreateDivInsideBrowser( ref browser, message ); }
        }

        /// <summary>
        ///     <para>A threadsafe <see cref="Button.PerformClick" />.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="delay">  </param>
        /// <returns></returns>
        /// <seealso cref="Push" />
        public static void PerformClickThreadSafe( [CanBeNull] this Button control, TimeSpan? delay = null ) => control?.Push( delay );

        /// <summary>
        ///     <para>A threadsafe <see cref="Button.PerformClick" />.</para>
        /// </summary>
        /// <param name="control">   </param>
        /// <param name="delay">     </param>
        /// <param name="afterDelay"></param>
        /// <returns></returns>
        public static Timer Push( [NotNull] this Button control, TimeSpan? delay = null, Action afterDelay = null ) {
            if ( control is null ) { throw new ArgumentNullException( nameof( control ) ); }

            return ( delay ?? Milliseconds.One ).CreateTimer( () => control.InvokeIfRequired( () => {
                control.PerformClick();
                afterDelay?.Invoke();
            } ) ).AndStart();
        }

        /// <summary>
        ///     Threadsafe <see cref="Control.Refresh" />.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static void Redraw( [CanBeNull] this Control control ) => control?.InvokeIfRequired( control.Refresh );

        public static Boolean RemoveTags( this WebBrowser browser, String tagName, Int32 keepAtMost = 50 ) {
            if ( browser?.Document is null ) { return false; }

            while ( null != browser.Document && browser.Document.GetElementsByTagName( tagName ).Count > keepAtMost ) {
                var item = browser.Document.GetElementsByTagName( tagName )[0];
                item.OuterHtml = String.Empty;
                browser.BeginInvoke( new Action( browser.Update ) );
            }

            return true;
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Reset( [CanBeNull] this ProgressBar control, Int32? value = null ) => control?.Value( value ?? control.Minimum() );

        /// <summary>
        ///     Just changes the cursor to the <see cref="Cursors.Default" />.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static void ResetCursor( [NotNull] this Control control ) {
            if ( control is null ) { throw new ArgumentNullException( nameof( control ) ); }

            ThreadingExtensions.Wrap( () => control.OnThread( () => {
                control.ResetCursor();
                control.Invalidate( invalidateChildren: false );
            } ) );
        }

        public static void SavePosition( [NotNull] this Form form, [CanBeNull] Ini settings ) {
            if ( form is null ) { throw new ArgumentNullException( nameof( form ) ); }

            if ( settings is null ) { throw new ArgumentNullException( nameof( settings ) ); }

            var name = form.Name ?? "UnknownForm";

            settings[name, nameof( form.Size.Width )] = form.WindowState == FormWindowState.Normal ? form.Size.Width.ToString() : form.RestoreBounds.Size.Width.ToString();
            settings[name, nameof( form.Size.Height )] = form.WindowState == FormWindowState.Normal ? form.Size.Height.ToString() : form.RestoreBounds.Size.Height.ToString();

            settings[name, nameof( form.Location.X )] = form.WindowState == FormWindowState.Normal ? form.Location.X.ToString() : form.RestoreBounds.Location.X.ToString();
            settings[name, nameof( form.Location.Y )] = form.WindowState == FormWindowState.Normal ? form.Location.Y.ToString() : form.RestoreBounds.Location.Y.ToString();
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="minimum"></param>
        /// <param name="value">  </param>
        /// <param name="maximum"></param>
        /// <seealso cref="Values" />
        public static void Set( [CanBeNull] this ProgressBar control, Int32 minimum, Int32 value, Int32 maximum ) => control.Values( minimum: minimum, value, maximum: maximum );

        /// <summary>
        ///     Safely get the <see cref="Form.Size" />() of a <see cref="Form" /> across threads.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Size Size( [CanBeNull] this Form form ) {
            if ( null == form ) { return new Size(); }

            return form.InvokeRequired ? ( Size )form.Invoke( new Func<Size>( () => form.Size ) ) : form.Size;
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Text" /> of a control across threads.
        /// </summary>
        /// <remarks></remarks>
        public static void Size( [CanBeNull] this Form form, Size size ) =>
            form?.InvokeIfRequired( () => {
                if ( form.IsDisposed ) { return; }

                form.Size = size;
            } );

        /// <summary>
        ///     Safely perform the <see cref="ProgressBar.PerformStep" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        public static void Step( [CanBeNull] this ProgressBar control ) =>
            control?.OnThread( () => {
                if ( control.IsDisposed ) { return; }

                if ( control.Style != ProgressBarStyle.Marquee ) { control.PerformStep(); }

                control.Refresh();
            } );

        /// <summary>
        ///     Safely perform the <see cref="ProgressBar.PerformStep" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        public static void Step( [CanBeNull] this ToolStripProgressBar control ) =>
            control?.OnThread( () => {
                if ( control.IsDisposed ) { return; }

                control.PerformStep();

                if ( !control.IsDisposed ) { control.ProgressBar?.Refresh(); }
            } );

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Step" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Step( [CanBeNull] this ProgressBar control, Int32 value ) =>
            control?.OnThread( () => {
                if ( control.IsDisposed ) { return; }

                control.Step = value;
                control.Refresh();
            } );

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Style" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Style( [CanBeNull] this ProgressBar control, ProgressBarStyle value ) =>
            control?.OnThread( () => {
                if ( control.IsDisposed ) { return; }

                control.Style = value;
                control.Refresh();
            } );

        /// <summary>
        ///     <para>Safely get the <see cref="Control.Text" /> of a <see cref="Control" /> across threads.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static String Text( [CanBeNull] this Control control ) {
            if ( null == control ) { return String.Empty; }

            return control.InvokeRequired ? control.Invoke( new Func<String>( () => control.Text ) ) as String ?? String.Empty : control.Text;
        }

        /// <summary>
        ///     Safely set the <see cref="ToolStripItem.Text" /> of the control across threads.
        /// </summary>
        /// <param name="toolStripItem"></param>
        /// <param name="value">        </param>
        public static void Text( [CanBeNull] this ToolStripItem toolStripItem, [CanBeNull] String value ) {
            if ( null == toolStripItem ) { return; }

            if ( toolStripItem.IsDisposed ) { return; }

            toolStripItem.OnThread( () => {
                if ( toolStripItem.IsDisposed ) { return; }

                toolStripItem.Text = value;
                toolStripItem.Invalidate();
            } );
        }

        /// <summary>
        ///     <para>Safely set the <see cref="Control.Text" /> of a control across threads.</para>
        /// </summary>
        /// <remarks></remarks>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        /// <seealso cref="http://kristofverbiest.blogspot.com/2007/02/don-confuse-controlbegininvoke-with.html" />
        /// <seealso
        ///     cref="http://programmers.stackexchange.com/questions/114605/how-will-c-5-async-support-help-ui-thread-synchronization-issues" />
        public static void Text( [CanBeNull] this Control control, [CanBeNull] String value ) =>
            control?.InvokeIfRequired( () => {
                if ( control.IsDisposed ) { return; }

                control.Text = value;
                control.Invalidate();
            } );

        public static void TextAdd( [CanBeNull] this RichTextBox textBox, [CanBeNull] String message ) {
            if ( textBox is null ) { return; }

            if ( message is null ) { return; }

            var method = new Action( () => {
                if ( textBox.IsDisposed ) { return; }

                textBox.AppendText( message );

                var lines = textBox.Lines.ToList();

                if ( lines.Count > 20 ) {
                    while ( lines.Count > 20 ) { lines.RemoveAt( 0 ); }

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

            if ( textBox.IsDisposed ) { return; }

            if ( textBox.InvokeRequired ) { textBox.BeginInvoke( method ); }
            else { method(); }
        }

        public static void TextAdd( [NotNull] this RichTextBox textBox, [NotNull] String text, Color color ) {
            if ( textBox is null ) { throw new ArgumentNullException( nameof( textBox ) ); }

            if ( text is null ) { throw new ArgumentNullException( nameof( text ) ); }

            textBox.SelectionStart = textBox.TextLength;
            textBox.SelectionLength = 0;

            textBox.SelectionColor = color;
            textBox.AppendText( text );
            textBox.SelectionColor = textBox.ForeColor;
        }

        public static Int32 ToBGR( this Color thisColor ) => ( thisColor.B << 16 ) | ( thisColor.G << 8 ) | ( thisColor.R << 0 );

        public static Int32 ToRGB( this Color thisColor ) => thisColor.ToArgb() & 0xFFFFFF;

        /// <summary>
        ///     <para>Make this
        ///         <param name="control"></param>
        ///         not <see cref="Usable" />.
        ///     </para>
        /// </summary>
        /// <param name="control"></param>
        public static void TurnOff( this Control control ) {
            if ( !TurnOnOrOffReqests.ContainsKey( control ) ) { TurnOnOrOffReqests[control] = 0; }

            TurnOnOrOffReqests[control]--;
            control.Usable( TurnOnOrOffReqests[control] <= 0 );
        }

        /// <summary>
        ///     <para>Make this
        ///         <param name="control"></param>
        ///         <see cref="Usable" />.
        ///     </para>
        /// </summary>
        /// <param name="control"></param>
        public static void TurnOn( [CanBeNull] this Control control ) {
            if ( null == control ) { return; }

            if ( !TurnOnOrOffReqests.ContainsKey( control ) ) { TurnOnOrOffReqests[control] = 0; }

            TurnOnOrOffReqests[control]++;
            control.Usable( TurnOnOrOffReqests[control] > 0 );
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Enabled" /> and <see cref="Control.Visible" /> of a control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Usable( this Control control, Boolean value ) {
            if ( null == control ) { return; }

            if ( control.InvokeRequired ) {
                control.BeginInvoke( new Action( () => {
                    if ( control.IsDisposed ) { return; }

                    var anyChange = control.Visible != value || control.Enabled != value;

                    if ( !anyChange ) { return; }

                    control.Visible = value;
                    control.Enabled = value;
                    control.Refresh();
                } ) );
            }
            else {
                var anyChange = control.Visible != value || control.Enabled != value;

                if ( !anyChange ) { return; }

                control.Visible = value;
                control.Enabled = value;
                control.Refresh();
            }
        }

        /// <summary>
        ///     Threadsafe Value get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Decimal Value( [CanBeNull] this NumericUpDown control ) {
            if ( null == control ) { return Decimal.Zero; }

            return control.InvokeRequired ? ( Decimal )control.Invoke( new Func<Decimal>( () => control.Value ) ) : control.Value;
        }

        /// <summary>
        ///     Threadsafe Value get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Int32 Value( [CanBeNull] this ProgressBar control ) {
            if ( null == control ) { return 0; }

            return control.InvokeRequired ? ( Int32 )control.Invoke( new Func<Int32>( () => control.Value ) ) : control.Value;
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Value( [CanBeNull] this ProgressBar control, Int32 value ) =>
            control?.OnThread( () => {
                if ( control.IsDisposed ) { return; }

                if ( value > control.Maximum ) { control.Maximum = value; }
                else if ( value < control.Minimum ) { control.Minimum = value; }

                control.Value = value;
                control.Refresh();
            } );

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="minimum"></param>
        /// <param name="value">  </param>
        /// <param name="maximum"></param>
        public static void Values( [CanBeNull] this ProgressBar control, Int32 minimum, Int32 value, Int32 maximum ) {
            if ( null == control ) { return; }

            minimum.Should().BeLessOrEqualTo( maximum );
            value.Should().BeLessOrEqualTo( maximum );
            var lowEnd = Math.Min( minimum, maximum );
            var highEnd = Math.Max( minimum, maximum );
            control.Minimum( lowEnd );
            control.Maximum( highEnd );
            control.Value( value );
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Visible" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Visible( this Control control, Boolean value ) {
            if ( null == control ) { return; }

            if ( control.InvokeRequired ) {
                control.BeginInvoke( new Action( () => {
                    if ( control.IsDisposed ) { return; }

                    control.Visible = value;
                    control.Refresh();
                } ) );
            }
            else {
                control.Visible = value;
                control.Refresh();
            }
        }
    }
}