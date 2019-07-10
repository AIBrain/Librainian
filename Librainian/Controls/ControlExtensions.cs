// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ControlExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ControlExtensions.cs" was last formatted by Protiguous on 2019/04/28 at 9:49 AM.

namespace Librainian.Controls {

    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Measurement.Time;
    using Microsoft.Win32;
    using Parsing;
    using Persistence;
    using Persistence.InIFiles;
    using Threading;

    public static class ControlExtensions {

        public static ConcurrentDictionary<Control, Int32> TurnOnOrOffReqests { get; } = new ConcurrentDictionary<Control, Int32>();

        public static void AppendLine( [NotNull] this RichTextBox box, String text, Color color, [NotNull] params Object[] args ) =>
            box.AppendText( $"\n{text}", color == Color.Empty ? box.ForeColor : color, args );

        public static void AppendText( [NotNull] this RichTextBox box, String text, Color color, [NotNull] params Object[] args ) =>
            box.InvokeAction( () => {
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
            } );

        public static void AppendTextInvoke( [NotNull] this RichTextBox box, String text, Color color, params Object[] args ) =>
            box.Invoke( ( MethodInvoker ) ( () => box.AppendText( text, color, args ) ) );

        public static Color Blend( this Color thisColor, Color blendToColor, Double blendToPercent ) {
            blendToPercent = ( 1 - blendToPercent ).ForceBounds( 0, 1 );

            var r = ( Byte ) ( (thisColor.R * blendToPercent) + (blendToColor.R * ( 1 - blendToPercent )) );
            var g = ( Byte ) ( (thisColor.G * blendToPercent) + (blendToColor.G * ( 1 - blendToPercent )) );
            var b = ( Byte ) ( (thisColor.B * blendToPercent) + (blendToColor.B * ( 1 - blendToPercent )) );

            return Color.FromArgb( r, g, b );
        }

        /// <summary>
        ///     Just changes the cursor to the <see cref="Cursors.WaitCursor" />.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static void BusyCursor( [NotNull] this Control control ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            control.InvokeAction( () => control.Cursor = Cursors.WaitCursor );
        }

        /// <summary>
        ///     Threadsafe <see cref="CheckBox.Checked" /> check.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Boolean Checked( [NotNull] this CheckBox control ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            return control.InvokeRequired ? ( Boolean ) control.Invoke( new Func<Boolean>( () => control.Checked ) ) : control.Checked;
        }

        /// <summary>
        ///     Safely set the <see cref="CheckBox.Checked" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Checked( [NotNull] this CheckBox control, Boolean value ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
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
        ///     <para>A threadsafe <see cref="Button.PerformClick" />.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="delay">  </param>
        /// <returns></returns>
        /// <see cref="Push" />
        public static void Click( [NotNull] this Button control, TimeSpan? delay = null ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            control.Push( delay );
        }

        /// <summary>
        ///     Returns a contrasting ForeColor for the specified BackColor. If the source BackColor is dark, then the
        ///     lightForeColor is returned. If the BackColor is light, then the darkForeColor is returned.
        /// </summary>
        public static Color DetermineForecolor( this Color thisColor, Color lightForeColor, Color darkForeColor ) {

            // Counting the perceptive luminance - human eye favors green color...
            var a = 1 - (( (0.299 * thisColor.R) + (0.587 * thisColor.G) + (0.114 * thisColor.B) ) / 255);

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
        public static void Enabled( [NotNull] this Control control, Boolean value, Boolean refresh = true ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            control.InvokeAction( () => {
                if ( control.IsDisposed ) {
                    return;
                }

                control.Enabled = value;

                if ( refresh ) {
                    control.Refresh();
                }
                else {
                    control.Invalidate();
                }
            } );
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Enabled" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Enabled( [NotNull] this ToolStripProgressBar control, Boolean value ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            if ( control.ProgressBar?.InvokeRequired == true ) {
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
                control.ProgressBar?.Refresh();
            }
        }

        /// <summary>
        ///     Flashes the control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="spanOff">How long to keep the control off before it resets.</param>
        public static void Flash( [NotNull] this Control control, [NotNull] TimeSpan? spanOff = null ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            if ( !spanOff.HasValue ) {
                spanOff = Milliseconds.One;
            }

            void Action() {
                var foreColor = control.ForeColor;
                control.ForeColor = control.BackColor;
                control.BackColor = foreColor;
                control.Refresh();
            }

            FluentTimer.Start( ( spanOff ?? Milliseconds.One ).Create( () => control.InvokeAction( Action ) ).Once() );
        }

        [NotNull]
        public static Task FlashWhileBlank( this Control input, [NotNull] Control control, CancellationToken token ) {
            if ( control == null ) {
                throw new ArgumentNullException( nameof( control ) );
            }

            return Seconds.Five.Then( async () => {
                if ( !input.Text().IsNullOrWhiteSpace() ) {
                    return;
                }

                control.Flash( Seconds.One );
                await input.FlashWhileBlank( control, token ).ConfigureAwait( true );
            }, token );
        }

        /// <summary>
        ///     Set <see cref="Control.Focus" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        public static void Fokus( [NotNull] this Control control ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            control.InvokeAction( () => {
                if ( !control.IsDisposed && control.CanFocus ) {
                    control.Focus();
                }
            } );
        }

        /// <summary>
        ///     Threadsafe <see cref="Control.ForeColor" /> check.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Color ForeColor( [NotNull] this Control control ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            return control.InvokeRequired ? ( Color ) control.Invoke( new Func<Color>( () => control.ForeColor ) ) : control.ForeColor;
        }

        /// <summary>
        ///     Safely set the <see cref="Control.ForeColor" /> of the control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void ForeColor( [NotNull] this Control control, Color value ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
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

        /*
        public static void FullScreen( [NotNull] this Window window ) {
            window.WindowState = WindowState.Maximized;
            window.WindowStyle = WindowStyle.None;
        }
        */

        /// <summary>
        ///     <para>Perform an <see cref="Action" /> on the control's thread.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"> </param>
        /// <seealso />
        public static void InvokeAction( [NotNull] this Control control, [NotNull] Action action ) {
            if ( control == null ) {
                throw new ArgumentNullException( nameof( control ) );
            }

            if ( action == null ) {
                throw new ArgumentNullException( nameof( action ) );
            }

            if ( control.InvokeRequired ) {
                if ( !control.IsDisposed ) {
                    control.Invoke( action );
                }
            }
            else {
                action();
            }
        }

        /// <summary>
        ///     <para>Perform an <see cref="Action" /> on the control's thread.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"> </param>
        /// <param name="thing"></param>
        /// <seealso />
        public static void InvokeAction<T>( [NotNull] this Control control, [NotNull] Action<T> action, T thing ) {
            if ( control == null ) {
                throw new ArgumentNullException( nameof( control ) );
            }

            if ( action == null ) {
                throw new ArgumentNullException( nameof( action ) );
            }

            if ( control.InvokeRequired ) {
                if ( !control.IsDisposed ) {
                    control.Invoke( action, thing );
                }
            }
            else {
                action( thing );
            }
        }

        [NotNull]
        public static T InvokeFunction<T>( [NotNull] this T invokable, Func<T> function, [NotNull] Object[] arguments = null ) where T : class, ISynchronizeInvoke =>
            invokable.Invoke( function, arguments ) as T;


        /// <summary>
        ///     <seealso cref="SaveSize(Form)" />
        /// </summary>
        /// <param name="form"></param>
        public static void LoadSize( [NotNull] this Form form ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( AppRegistry.TheApplication == null ) {
                throw new InvalidOperationException( "Application registry not set up." );
            }

            $"Loading form {form.Name} position from registry.".Log();

            var width = AppRegistry.GetInt32( nameof( form.Size ), form.Name, nameof( form.Size.Width ) );
            var height = AppRegistry.GetInt32( nameof( form.Size ), form.Name, nameof( form.Size.Height ) );

            if ( width.HasValue && height.HasValue ) {
                form.InvokeAction( () => {
                    form.SuspendLayout();
                    form.Size( new Size( width.Value, height.Value ) );
                    form.ResumeLayout();
                } );
            }
        }

        public static void LoadLocation( [NotNull] this Form form, [NotNull] String name, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            if ( String.IsNullOrEmpty( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( name ) );
            }

            if ( Int32.TryParse( settings[ name, nameof( Point.X ) ], out var x ) && Int32.TryParse( settings[ name, nameof( Point.Y ) ], out var y ) ) {
                form.InvokeAction( () => {
                    form.SuspendLayout();
                    form.Location( new Point( x, y ) );
                    form.ResumeLayout();
                } );
            }
        }

        public static void LoadSize( [NotNull] this Form form, [NotNull] String name, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            if ( String.IsNullOrEmpty( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( name ) );
            }

            if ( Int32.TryParse( settings[ name, nameof( form.Size.Width ) ], out var width ) &&
                 Int32.TryParse( settings[ name, nameof( form.Size.Height ) ], out var height ) ) {
                form.InvokeAction( () => {
                    form.SuspendLayout();
                    form.Size( new Size( width, height ) );
                    form.ResumeLayout();
                } );
            }
        }

        public static void LoadPosition( [NotNull] this Form form, [NotNull] String name, [NotNull] PersistTable<String, String> settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( String.IsNullOrEmpty( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( name ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            var key = Cache.BuildKey( name, nameof( form.Location ) );
            var point = settings[ key ].Deserialize<Point>();

            if ( point.X != 0 || point.Y != 0 ) {
                form.Location( point );
            }

            key = Cache.BuildKey( name, nameof( form.Size ) );
            var size = settings[ key ].Deserialize<Size>();

            if ( size.Height != 0 || size.Width != 0 ) {
                form.Size( size );
            }
        }

        public static void LoadPosition( [NotNull] this Form form, [NotNull] String name, [NotNull] StringKVPTable settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( String.IsNullOrEmpty( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( name ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            var key = Cache.BuildKey( name, nameof( form.Location ) );
            var point = settings[ key ].Deserialize<Point>();

            if ( point.X != 0 || point.Y != 0 ) {
                form.Location( point );
            }

            key = Cache.BuildKey( name, nameof( form.Size ) );
            var size = settings[ key ].Deserialize<Size>();

            if ( size.Height != 0 || size.Width != 0 ) {
                form.Size( size );
            }
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Location" /> of a <see cref="Form" /> across threads.
        /// </summary>
        /// <remarks></remarks>
        public static void Location( [NotNull] this Form form, Point location ) {
            if ( form == null ) {
                throw new ArgumentNullException( paramName: nameof( form ) );
            }

            form.InvokeAction( () => {
                form.SuspendLayout();
                form.Location = location;
                form.ResumeLayout();
            } );
        }

        public static Color MakeDarker( this Color thisColor, Double darknessPercent ) {
            darknessPercent = darknessPercent.ForceBounds( 0, 1 );

            return Blend( thisColor, Color.Black, darknessPercent );
        }

        public static Color MakeLighter( this Color thisColor, Double lightnessPercent ) {
            lightnessPercent = lightnessPercent.ForceBounds( 0, 1 );

            return Blend( thisColor, Color.White, lightnessPercent );
        }

        public static Color MakeTransparent( this Color thisColor, Double transparentPercent ) {
            transparentPercent = 255 - (transparentPercent.ForceBounds( 0, 1 ) * 255);

            return Color.FromArgb( thisColor.ToArgb() + (( Int32 ) transparentPercent * 0x1000000) );
        }

        [NotNull]
        public static Task MarqueeAsync( [NotNull] this Control control, TimeSpan timeSpan, [NotNull] String message ) {
            control.Text( message );
            var until = DateTime.Now.Add( timeSpan );

            return Task.Run( () => {
                var stopwatch = Stopwatch.StartNew();

                do {
                    stopwatch.Restart();
                    control.Flash();
                    stopwatch.Stop();
                } while ( DateTime.Now < until );
            } );
        }

        /// <summary>
        ///     Threadsafe get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Int32 Maximum( [NotNull] this ProgressBar control ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            return control.InvokeRequired ? ( Int32 ) control.Invoke( new Func<Int32>( () => control.Maximum ) ) : control.Maximum;
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Maximum" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Maximum( [NotNull] this ProgressBar control, Int32 value ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            void Action() {
                if ( control.IsDisposed ) {
                    return;
                }

                control.Maximum = value;
                control.Refresh();
            }

            control.InvokeAction( Action );
        }

        /// <summary>
        ///     Threadsafe get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Int32 Minimum( [NotNull] this ProgressBar control ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            return control.InvokeRequired ? ( Int32 ) control.Invoke( new Func<Int32>( () => control.Minimum ) ) : control.Minimum;
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Minimum" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Minimum( [NotNull] this ProgressBar control, Int32 value ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            control.InvokeAction( () => {
                if ( control.IsDisposed ) {
                    return;
                }

                control.Minimum = value;
                control.Refresh();
            } );
        }

        /// <summary>
        ///     <para>A threadsafe <see cref="Button.PerformClick" />.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="delay">  </param>
        /// <returns></returns>
        /// <see cref="Push" />
        public static void PerformClickThreadSafe( [NotNull] this Button control, TimeSpan? delay = null ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            control.Push( delay );
        }

        /// <summary>
        ///     Threadsafe <see cref="Button.PerformClick" />.
        /// </summary>
        /// <param name="control"></param>
        public static void Press( [NotNull] this Button control ) => control.InvokeAction( control.PerformClick );

        /// <summary>
        ///     <para>A threadsafe <see cref="Button.PerformClick" />.</para>
        /// </summary>
        /// <param name="control">   </param>
        /// <param name="delay">     </param>
        /// <param name="afterDelay"></param>
        /// <returns></returns>
        [NotNull]
        public static System.Timers.Timer Push( [NotNull] this Button control, TimeSpan? delay = null, [NotNull] Action afterDelay = null ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            return FluentTimer.Start( ( delay ?? Milliseconds.One ).Create( () => control.InvokeAction( () => {
                control.PerformClick();
                afterDelay?.Invoke();
            } ) ) );
        }

        /// <summary>
        ///     Threadsafe <see cref="Control.Refresh" />.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static void Redraw( [NotNull] this Control control ) => control.InvokeAction( control.Refresh );

        public static Boolean RemoveTags( [NotNull] this WebBrowser browser, String tagName, Int32 keepAtMost = 50 ) {
            if ( browser.Document == null ) {
                return false;
            }

            while ( null != browser.Document && browser.Document.GetElementsByTagName( tagName ).Count > keepAtMost ) {
                var item = browser.Document.GetElementsByTagName( tagName )[ 0 ];
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
        public static void Reset( [NotNull] this ProgressBar control, Int32? value = null ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            control.Value( value ?? control.Minimum() );
        }

        /// <summary>
        ///     Just changes the cursor to the <see cref="Cursors.Default" />.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static void ResetCursor( [NotNull] this Control control ) {
            if ( control == null ) {
                throw new ArgumentNullException( nameof( control ) );
            }

            void Action() {
                control.ResetCursor();
                control.Invalidate( invalidateChildren: false );
            }

            control.InvokeAction( Action );
        }

        /// <summary>
        ///     <seealso cref="LoadLocation" />
        /// </summary>
        /// <param name="form"></param>
        public static void SaveLocation( [NotNull] this Form form ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( AppRegistry.TheApplication == null ) {
                throw new InvalidOperationException( "Application registry not set up." );
            }

            $"Saving form {form.Name} position to registry.".Log();

            AppRegistry.Set( nameof( form.Location ), form.Name, nameof( form.Location.X ),
                form.WindowState == FormWindowState.Normal ? form.Location.X : form.RestoreBounds.Location.X, RegistryValueKind.DWord );

            AppRegistry.Set( nameof( form.Location ), form.Name, nameof( form.Location.Y ),
                form.WindowState == FormWindowState.Normal ? form.Location.Y : form.RestoreBounds.Location.Y, RegistryValueKind.DWord );
        }

        /// <summary>
        ///     <seealso cref="LoadSize(Form)" />
        /// </summary>
        /// <param name="form"></param>
        public static void SaveSize( [NotNull] this Form form ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( AppRegistry.TheApplication == null ) {
                throw new InvalidOperationException( "Application registry not set up." );
            }

            $"Saving form {form.Name} position to registry.".Log();

            AppRegistry.Set( nameof( form.Size ), form.Name, nameof( form.Size.Width ),
                form.WindowState == FormWindowState.Normal ? form.Size.Width : form.RestoreBounds.Size.Width, RegistryValueKind.DWord );

            AppRegistry.Set( nameof( form.Size ), form.Name, nameof( form.Size.Height ),
                form.WindowState == FormWindowState.Normal ? form.Size.Height : form.RestoreBounds.Size.Height, RegistryValueKind.DWord );
        }

        public static void SaveLocation( [NotNull] this Form form, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            var name = form.Name ?? "UnknownForm";

            settings[ name, nameof( form.Location.X ) ] = form.WindowState == FormWindowState.Normal ? form.Location.X.ToString() : form.RestoreBounds.Location.X.ToString();
            settings[ name, nameof( form.Location.Y ) ] = form.WindowState == FormWindowState.Normal ? form.Location.Y.ToString() : form.RestoreBounds.Location.Y.ToString();
        }

        public static void SaveSize( [NotNull] this Form form, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            var name = form.Name ?? "UnknownForm";

            settings[ name, nameof( form.Size.Width ) ] = form.WindowState == FormWindowState.Normal ? form.Size.Width.ToString() : form.RestoreBounds.Size.Width.ToString();

            settings[ name, nameof( form.Size.Height ) ] =
                form.WindowState == FormWindowState.Normal ? form.Size.Height.ToString() : form.RestoreBounds.Size.Height.ToString();
        }

        public static void SaveLocation( [NotNull] this Form form, String name, [NotNull] PersistTable<String, String> settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ Cache.BuildKey( name, nameof( form.Location ) ) ] = form.Location.ToJSON();
        }

        public static void SaveSize( [NotNull] this Form form, String name, [NotNull] PersistTable<String, String> settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ Cache.BuildKey( name, nameof( form.Size ) ) ] = form.Size.ToJSON();
        }

        public static void SaveLocation( [NotNull] this Form form, String name, [NotNull] StringKVPTable settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ Cache.BuildKey( name, nameof( form.Location ) ) ] = form.Location.ToJSON();
        }

        public static void SaveSize( [NotNull] this Form form, String name, [NotNull] StringKVPTable settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ Cache.BuildKey( name, nameof( form.Size ) ) ] = form.Size.ToJSON();
        }

        public static void SaveLocation( [NotNull] this Form form, String name, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ nameof( form.Location ), Cache.BuildKey( name, nameof( form.Location ) ) ] = form.Location.ToJSON();
        }

        public static void SaveSize( [NotNull] this Form form, String name, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ nameof( form.Size ), Cache.BuildKey( name, nameof( form.Size ) ) ] = form.Size.ToJSON();
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="minimum"></param>
        /// <param name="value">  </param>
        /// <param name="maximum"></param>
        /// <see cref="Values" />
        public static void Set( [NotNull] this ProgressBar control, Int32 minimum, Int32 value, Int32 maximum ) => control.Values( minimum: minimum, value, maximum: maximum );

        /// <summary>
        ///     Safely get the <see cref="Form.Size" />() of a <see cref="Form" /> across threads.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Size Size( [NotNull] this Form form ) {
            if ( form == null ) {
                throw new ArgumentNullException( paramName: nameof( form ) );
            }

            return form.InvokeRequired ? ( Size ) form.Invoke( new Func<Size>( () => form.Size ) ) : form.Size;
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Text" /> of a control across threads.
        /// </summary>
        /// <remarks></remarks>
        public static void Size( [NotNull] this Form form, Size size ) {
            if ( form == null ) {
                throw new ArgumentNullException( paramName: nameof( form ) );
            }

            form.InvokeAction( () => {
                if ( form.IsDisposed ) {
                    return;
                }

                form.SuspendLayout();
                form.Size = size;
                form.ResumeLayout();
            } );
        }

        /// <summary>
        ///     Safely perform the <see cref="ProgressBar.PerformStep" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        public static void Step( [NotNull] this ProgressBar control ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            void Action() {
                if ( control.IsDisposed ) {
                    return;
                }

                if ( control.Style != ProgressBarStyle.Marquee ) {
                    control.PerformStep();
                }

                control.Refresh();
            }

            control.InvokeAction( Action );
        }

        /// <summary>
        ///     Safely perform the <see cref="ProgressBar.PerformStep" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        public static void Step( [NotNull] this ToolStripProgressBar control ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            void Action() {
                if ( control.IsDisposed ) {
                    return;
                }

                control.PerformStep();

                if ( !control.IsDisposed ) {
                    control.ProgressBar?.Refresh();
                }
            }

            control.GetCurrentParent().InvokeAction( Action );
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Step" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Step( [NotNull] this ProgressBar control, Int32 value ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            void Action() {
                if ( control.IsDisposed ) {
                    return;
                }

                control.Step = value;
                control.Refresh();
            }

            control.InvokeAction( Action );
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Style" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Style( [NotNull] this ProgressBar control, ProgressBarStyle value ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            void Action() {
                if ( control.IsDisposed ) {
                    return;
                }

                control.Style = value;
                control.Refresh();
            }

            control.InvokeAction( Action );
        }

        /// <summary>
        ///     <para>Safely get the <see cref="Control.Text" /> of a <see cref="Control" /> across threads.</para>
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static String Text( [NotNull] this Control control ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            return control.InvokeRequired ? control.Invoke( new Func<String>( () => control.Text ) ) as String ?? String.Empty : control.Text;
        }

        /// <summary>
        ///     Safely set the <see cref="ToolStripItem.Text" /> of the control across threads.
        /// </summary>
        /// <param name="toolStripItem"></param>
        /// <param name="value">        </param>
        public static void Text( [NotNull] this ToolStripItem toolStripItem, [CanBeNull] String value ) {
            if ( toolStripItem == null ) {
                throw new ArgumentNullException( paramName: nameof( toolStripItem ) );
            }

            void Action() {
                if ( !toolStripItem.IsDisposed ) {
                    toolStripItem.Text = value;
                    toolStripItem.Invalidate();
                }
            }

            toolStripItem.GetCurrentParent().InvokeAction( Action );
        }

        /// <summary>
        ///     <para>Safely set the <see cref="Control.Text" /> of a control across threads.</para>
        /// </summary>
        /// <remarks></remarks>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Text( [NotNull] this Control control, [NotNull] String value ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            control.InvokeAction( () => {
                if ( control.IsDisposed ) {
                    return;
                }

                control.Text = value;
                control.Invalidate();
            } );
        }

        public static void TextAdd( [NotNull] this RichTextBox textBox, [NotNull] String message ) {
            if ( textBox == null ) {
                throw new ArgumentNullException( paramName: nameof( textBox ) );
            }

            if ( message == null ) {
                throw new ArgumentNullException( paramName: nameof( message ) );
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

        public static void TextAdd( [NotNull] this RichTextBox textBox, [NotNull] String text, Color color ) {
            if ( textBox == null ) {
                throw new ArgumentNullException( nameof( textBox ) );
            }

            if ( text == null ) {
                throw new ArgumentNullException( nameof( text ) );
            }

            textBox.SelectionStart = textBox.TextLength;
            textBox.SelectionLength = 0;

            textBox.SelectionColor = color;
            textBox.AppendText( text );
            textBox.SelectionColor = textBox.ForeColor;
        }

        public static Int32 ToBGR( this Color thisColor ) => ( thisColor.B << 16 ) | ( thisColor.G << 8 ) | ( thisColor.R << 0 );

        /// <summary>
        ///     Returns <see cref="CheckState.Checked" /> if true, on, set, checked, or 1.
        ///     Otherwise <see cref="CheckState.Unchecked" />.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static CheckState ToCheckState( [NotNull] this String s ) {
            if ( !String.IsNullOrWhiteSpace( s ) ) {
                s = s.Trim().ToLower();

                if ( s.Like( Boolean.TrueString ) || s.Like( "true" ) || s.Like( "on" ) || s.Like( "checked" ) || s.Like( "set" ) || s.Like( "1" ) ) {
                    return CheckState.Checked;
                }

                return Boolean.TryParse( s, out _ ) ? CheckState.Checked : CheckState.Unchecked;
            }

            return CheckState.Unchecked;
        }

        public static Int32 ToRGB( this Color thisColor ) => thisColor.ToArgb() & 0xFFFFFF;

        /// <summary>
        ///     Safely set the <see cref="Control.Enabled" /> and <see cref="Control.Visible" /> of a control across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Usable( [NotNull] this Control control, Boolean value ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            control.InvokeAction( () => {
                control.Visible = value;
                control.Enabled = value;
                control.Refresh();
            } );
        }

        /// <summary>
        ///     Threadsafe Value get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Decimal Value( [NotNull] this NumericUpDown control ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            return control.InvokeRequired ? ( Decimal ) control.Invoke( new Func<Decimal>( () => control.Value ) ) : control.Value;
        }

        /// <summary>
        ///     Threadsafe Value get.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Int32 Value( [NotNull] this ProgressBar control ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            return control.InvokeRequired ? ( Int32 ) control.Invoke( new Func<Int32>( () => control.Value ) ) : control.Value;
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value">  </param>
        public static void Value( [NotNull] this ProgressBar control, Int32 value ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

            void Action() {
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
            }

            control.InvokeAction( Action );
        }

        /// <summary>
        ///     Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="minimum"></param>
        /// <param name="value">  </param>
        /// <param name="maximum"></param>
        public static void Values( [NotNull] this ProgressBar control, Int32 minimum, Int32 value, Int32 maximum ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
            }

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
        public static void Visible( [NotNull] this Control control, Boolean value ) {
            if ( control == null ) {
                throw new ArgumentNullException( paramName: nameof( control ) );
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

    }

}