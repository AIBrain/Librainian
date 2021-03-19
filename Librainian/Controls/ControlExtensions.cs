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
// File "ControlExtensions.cs" last formatted on 2020-08-14 at 8:32 PM.

#nullable enable



namespace Librainian.Controls {

	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Drawing;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using Collections.Extensions;
	using Converters;
	using JetBrains.Annotations;
	using Maths;
	using Measurement.Time;
	using Threading;

	public static class ControlExtensions {

		[DebuggerStepThrough]
		public static void Append( [NotNull] this RichTextBox box, [NotNull] String text, Color color, [CanBeNull] params Object[] args ) =>
			box.AppendText( $"{text}", color == Color.Empty ? box.ForeColor : color, args );

		[DebuggerStepThrough]
		public static void AppendLine( [NotNull] this RichTextBox box, [NotNull] String text, Color color, [NotNull] params Object[] args ) =>
			box.AppendText( $"{text}\n", color == Color.Empty ? box.ForeColor : color, args );

		[DebuggerStepThrough]
		public static void AppendText( [NotNull] this RichTextBox box, [NotNull] String text, Color color, [CanBeNull] params Object[]? args ) {
			if ( args != null ) {
				text = String.Format( text, args );
			}

			box.InvokeAction( () => {
				box.SelectionStart = box.TextLength;
				box.SelectionLength = 0;

				if ( color != Color.Empty ) {
					box.SelectionColor = color;
				}

				box.AppendText( text );

				if ( color != Color.Empty ) {
					box.SelectionColor = box.ForeColor;
				}

				box.SelectionStart = box.TextLength;
				box.ScrollToCaret();
			} );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Color Blend( this Color thisColor, Color blendToColor, Double blendToPercent ) {
			blendToPercent = I().ForceBounds( 0, 1 );

			return Color.FromArgb( Red(), Green(), Blue() );

			Double I() => 1 - blendToPercent;

			Byte Red() => ( Byte )( thisColor.R * blendToPercent + blendToColor.R * I() );

			Byte Green() => ( Byte )( thisColor.G * blendToPercent + blendToColor.G * I() );

			Byte Blue() => ( Byte )( thisColor.B * blendToPercent + blendToColor.B * I() );
		}

		/// <summary>Just changes the cursor to the <see cref="Cursors.WaitCursor" />.</summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static void BusyCursor( [NotNull] this Control control ) => control.InvokeAction( () => control.Cursor = Cursors.WaitCursor );

		/// <summary>Threadsafe <see cref="CheckBox.Checked" /> check.</summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static Boolean? Checked( [NotNull] this CheckBox control ) {
			if ( control.InvokeRequired ) {
				return ( Boolean? )control.Invoke( new Func<Boolean?>( Target ) );
			}

			return Target();

			Boolean? Target() {
				if ( control.CheckState == CheckState.Indeterminate ) {
					return default( Boolean? );
				}

				return control.Checked;
			}
		}

		/// <summary>Safely set the <see cref="CheckBox.Checked" /> of the control across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		[NotNull]
		public static CheckBox Checked( [NotNull] this CheckBox control, Boolean? value ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			control.InvokeAction( () => {
				if ( !value.HasValue ) {
					if ( control.CheckState != CheckState.Indeterminate ) {
						control.CheckState = CheckState.Indeterminate;
						control.Refresh();
					}
				}
				else {
					control.CheckState = value.Value ? CheckState.Checked : CheckState.Unchecked;
					control.Checked = value.Value;
					control.Refresh();
				}
			} );

			return control;
		}

		/// <summary>
		///     <para>A threadsafe <see cref="Button.PerformClick" />.</para>
		/// </summary>
		/// <param name="control"></param>
		/// <param name="delay">  </param>
		/// <returns></returns>
		/// <see cref="Push" />
		public static void Click( [NotNull] this Button control, TimeSpan? delay = null ) => control.Push( delay );

		/// <summary>
		///     Returns a contrasting ForeColor for the specified BackColor. If the source BackColor is dark, then the
		///     lightForeColor is returned. If the BackColor is light, then the
		///     darkForeColor is returned.
		/// </summary>
		public static Color DetermineForecolor( this Color thisColor, Color lightForeColor, Color darkForeColor ) {
			// Counting the perceptive luminance - human eye favors green color...
			return A() < 0.5 ? darkForeColor : lightForeColor;

			Double A() {
				var r = 0.299 * thisColor.R;
				var g = 0.587 * thisColor.G;
				var b = 0.114 * thisColor.B;
				var i = 1 - ( r + g + b );
				return i / 255;
			}
		}

		/// <summary>
		///     Returns a contrasting ForeColor for the specified BackColor. If the source BackColor is dark, then the White is
		///     returned. If the BackColor is light, then the Black is
		///     returned.
		/// </summary>
		public static Color DetermineForecolor( this Color thisColor ) => DetermineForecolor( thisColor, Color.White, Color.Black );

		/// <summary>Safely set the <see cref="Control.Enabled" /> of the control across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		/// <param name="refresh"></param>
		public static void Enabled( this Control control, Boolean value, Boolean refresh = true ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
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

		/// <summary>Safely set the <see cref="Control.Enabled" /> of the control across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		public static void Enabled( [NotNull] this ToolStripProgressBar control, Boolean value ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
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

		/// <summary>Flashes the control.</summary>
		/// <param name="control"></param>
		/// <param name="spanOff">How long to keep the control off before it resets.</param>
		public static void Flash( [NotNull] this Control control, TimeSpan? spanOff = null ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			spanOff ??= Milliseconds.One;
			spanOff.Value.CreateTimer( OnTick ).Once().Start();

			void Action() {
				var foreColor = control.ForeColor;
				control.ForeColor = control.BackColor;
				control.BackColor = foreColor;
				control.Refresh();
			}

			void OnTick() => control.InvokeAction( Action );

		}

		[NotNull]
		[DebuggerStepThrough]
		public static Task FlashWhileBlank( [NotNull] this Control input, [NotNull] Control control,  CancellationToken cancellationToken  ) =>
			Seconds.Five.Then( async () => {
				if ( String.IsNullOrWhiteSpace( input.Text() ) ) {
					control.Flash( Seconds.One );
					await input.FlashWhileBlank( control, cancellationToken ).ConfigureAwait( true );
				}
			}, cancellationToken );

		/// <summary>Set <see cref="Control.Focus" /> across threads.</summary>
		/// <param name="control"></param>
		[DebuggerStepThrough]
		public static void Fokus( [NotNull] this Control control ) =>
			control.InvokeAction( () => {
				if ( !control.IsDisposed && control.CanFocus ) {
					_ = control.Focus();
				}
			} );

		/// <summary>Threadsafe <see cref="Control.ForeColor" /> check.</summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static Color ForeColor( [NotNull] this Control control ) {
			if ( control.InvokeRequired ) {
				var func = new Func<Color>( () => control.ForeColor );
				var invoke = control.Invoke( func );

				if ( invoke != null ) {
					return ( Color )invoke;
				}
			}

			return control.ForeColor;
		}

		/// <summary>Safely set the <see cref="Control.ForeColor" /> of the control across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		[DebuggerStepThrough]
		public static void ForeColor( [NotNull] this Control control, Color value ) {
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
		[DebuggerStepThrough]
		public static void InvokeAction( [NotNull] this Control control, [NotNull] Action action ) {
			if ( control.IsDisposed ) {
				return;
			}

			if ( control.InvokeRequired ) {
				control.Invoke( action );
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
		public static void InvokeAction<T>( [NotNull] this Control control, [NotNull] Action<T> action, [CanBeNull] T thing ) {
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
		public static T InvokeFunction<T>( [NotNull] this T invokable, [NotNull] Func<T> function, [CanBeNull] Object[] arguments = null )
			where T : class, ISynchronizeInvoke {
			if ( invokable == null ) {
				throw new ArgumentNullException( nameof( invokable ) );
			}

			if ( function == null ) {
				throw new ArgumentNullException( nameof( function ) );
			}

			return invokable.Invoke( function, arguments ) as T ?? throw new InvalidOperationException();
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
			transparentPercent = 255 - transparentPercent.ForceBounds( 0, 1 ) * 255;

			return Color.FromArgb( thisColor.ToArgb() + ( Int32 )transparentPercent * 0x1000000 );
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

		/// <summary>Threadsafe get.</summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static Int32 Maximum( [NotNull] this ProgressBar control ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			return control.InvokeRequired ? ( Int32 )control.Invoke( new Func<Int32>( () => control.Maximum ) ) : control.Maximum;
		}

		/// <summary>Safely set the <see cref="ProgressBar.Maximum" /> of the <see cref="ProgressBar" /> across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		public static void Maximum( [NotNull] this ProgressBar control, Int32 value ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
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

		/// <summary>Threadsafe get.</summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static Int32 Minimum( [NotNull] this ProgressBar control ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			return control.InvokeRequired ? ( Int32 )control.Invoke( new Func<Int32>( () => control.Minimum ) ) : control.Minimum;
		}

		/// <summary>Safely set the <see cref="ProgressBar.Minimum" /> of the <see cref="ProgressBar" /> across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		public static void Minimum( [NotNull] this ProgressBar control, Int32 value ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			control.InvokeAction( () => {
				if ( control.IsDisposed ) {
					return;
				}

				control.Minimum = value;
				control.Refresh();
			} );
		}

		public static Boolean Nope( this DialogResult result ) => result.In( DialogResult.Abort, DialogResult.Cancel );

		/// <summary>
		///     <para>A threadsafe <see cref="Button.PerformClick" />.</para>
		/// </summary>
		/// <param name="control"></param>
		/// <param name="delay">  </param>
		/// <returns></returns>
		/// <see cref="Push" />
		public static void PerformClick( [NotNull] this Button control, TimeSpan? delay = null ) => control.Push( delay );

		/// <summary>Threadsafe <see cref="Button.PerformClick" />.</summary>
		/// <param name="control"></param>
		public static void Press( [NotNull] this Button control ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			control.InvokeAction( control.PerformClick );
		}

		/// <summary>
		///     <para>A threadsafe <see cref="Button.PerformClick" />.</para>
		/// </summary>
		/// <param name="control">   </param>
		/// <param name="delay">     </param>
		/// <param name="afterClick"></param>
		/// <returns></returns>
		[NotNull]
		public static FluentTimer Push( this Button control, TimeSpan? delay = null, [CanBeNull] Action? afterClick = null ) {
			return ( delay ?? Milliseconds.One ).CreateTimer( () => control.InvokeAction( () => {
				control.PerformClick();
				afterClick?.Invoke();
			} ) ).Start();
		}

		/// <summary>Threadsafe <see cref="Control.Refresh" />.</summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static void Redraw( [NotNull] this Control control ) => control.InvokeAction( control.Refresh );

		public static Boolean RemoveTags( [NotNull] this WebBrowser browser, [CanBeNull] String? tagName, Int32 keepAtMost = 50 ) {
			if ( browser == null ) {
				throw new ArgumentNullException( nameof( browser ) );
			}

			if ( browser.Document == null ) {
				return false;
			}

			while ( null != browser.Document && browser.Document.GetElementsByTagName( tagName ).Count > keepAtMost ) {
				var item = browser.Document.GetElementsByTagName( tagName )[ 0 ];

				if ( item is not null ) {
					item.OuterHtml = String.Empty;
				}

				browser.BeginInvoke( new Action( browser.Update ) );
			}

			return true;
		}

		/// <summary>Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		public static void Reset( [NotNull] this ProgressBar control, Int32? value = null ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			control.Value( value ?? control.Minimum() );
		}

		/// <summary>Just changes the cursor to the <see cref="Cursors.Default" />.</summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static void ResetCursor( [NotNull] this Control control ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			void Action() {
				control.ResetCursor();
				control.Invalidate( false );
			}

			control.InvokeAction( Action );
		}

		/// <summary>Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.</summary>
		/// <param name="control"></param>
		/// <param name="minimum"></param>
		/// <param name="value">  </param>
		/// <param name="maximum"></param>
		/// <see cref="Values" />
		public static void Set( [NotNull] this ProgressBar control, Int32 minimum, Int32 value, Int32 maximum ) => control.Values( minimum, value, maximum );

		/// <summary>Safely perform the <see cref="ProgressBar.PerformStep" /> across threads.</summary>
		/// <param name="control"></param>
		public static void Step( [NotNull] this ProgressBar control ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
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

		/// <summary>Safely perform the <see cref="ProgressBar.PerformStep" /> across threads.</summary>
		/// <param name="control"></param>
		public static void Step( [NotNull] this ToolStripProgressBar control ) =>
			control.GetCurrentParent()?.InvokeAction( () => {
				if ( control.IsDisposed ) {
					return;
				}

				control.PerformStep();

				if ( !control.IsDisposed ) {
					control.ProgressBar?.Refresh();
				}
			} );

		/// <summary>Safely set the <see cref="ProgressBar.Step" /> of the <see cref="ProgressBar" /> across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		public static void Step( [NotNull] this ProgressBar control, Int32 value ) =>
			control.InvokeAction( () => {
				control.Step = value;
				control.Refresh();
			} );

		/// <summary>Safely set the <see cref="ProgressBar.Style" /> of the <see cref="ProgressBar" /> across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		public static void Style( [NotNull] this ProgressBar control, ProgressBarStyle value ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			control.InvokeAction( () => {
				control.Style = value;
				control.Refresh();
			} );
		}

		/// <summary>
		///     <para>Safely get the <see cref="Control.Text" /> of a <see cref="Control" /> across threads.</para>
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		[CanBeNull]
		public static String? Text( [NotNull] this Control control ) {
			if ( control.InvokeRequired ) {
				return control.Invoke( new Func<String>( () => control.Text ) ) as String ?? String.Empty;
			}

			return control.Text;
		}

		/// <summary>Safely set the <see cref="ToolStripItem.Text" /> of the control across threads.</summary>
		/// <param name="toolStripItem"></param>
		/// <param name="value">        </param>
		public static void Text( [NotNull] this ToolStripItem toolStripItem, [CanBeNull] String? value ) {
			void Action() {
				if ( toolStripItem.IsDisposed ) {
					return;
				}

				toolStripItem.Text = value;
				toolStripItem.Invalidate();
			}

			toolStripItem.GetCurrentParent()?.InvokeAction( Action );
		}

		/// <summary>
		///     <para>Safely set the <see cref="Control.Text" /> of a control across threads.</para>
		/// </summary>
		/// <remarks></remarks>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		public static void Text( [NotNull] this Control control, [CanBeNull] String? value ) =>
			control.InvokeAction( () => {
				control.Text = value;
				control.Invalidate();
			} );

		public static void TextAdd( [NotNull] this RichTextBox textBox, [NotNull] String message ) {
			if ( textBox == null ) {
				throw new ArgumentNullException( nameof( textBox ) );
			}

			if ( message == null ) {
				throw new ArgumentNullException( nameof( message ) );
			}

			textBox.InvokeAction( () => {
				textBox.AppendText( message );

				while ( textBox.Lines?.Length > 20 ) {
					( ( IList )textBox.Lines ).RemoveAt( 0 );
				}

				textBox.Invalidate();
			} );
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

		public static Int32 ToBgr( this Color thisColor ) => ( thisColor.B << 16 ) | ( thisColor.G << 8 ) | ( thisColor.R << 0 );

		/// <summary>
		///     Returns <see cref="CheckState.Checked" /> if true, on, set, checked, or 1. Otherwise
		///     <see cref="CheckState.Unchecked" />.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static CheckState ToCheckState( [NotNull] this String s ) =>
			!String.IsNullOrWhiteSpace( s ) ? s.ToBoolean() ? CheckState.Checked : Boolean.TryParse( s, out var _ ) ? CheckState.Checked : CheckState.Unchecked :
				CheckState.Unchecked;

		public static Int32 ToRgb( this Color thisColor ) => thisColor.ToArgb() & 0xFFFFFF;

		/// <summary>Safely set the <see cref="Control.Enabled" /> and <see cref="Control.Visible" /> of a control across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		public static void Usable( this Control control, Boolean value ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			control.InvokeAction( () => {
				control.Visible = value;
				control.Enabled = value;
				control.Refresh();
			} );
		}

		/// <summary>Threadsafe Value get.</summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static Decimal Value( this NumericUpDown control ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			return control.InvokeRequired ? control.Invoke( new Func<Decimal>( () => control.Value ) ).ToDecimalOrThrow() : control.Value;
		}

		/// <summary>Threadsafe Value get.</summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static Int32 Value( this ProgressBar control ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			return control.InvokeRequired ? control.Invoke( new Func<Int32>( () => control.Value ) ).ToIntOrThrow() : control.Value;
		}

		/// <summary>Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		public static void Value( [NotNull] this ProgressBar control, Int32 value ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
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

		/// <summary>Safely set the <see cref="ProgressBar.Value" /> of the <see cref="ProgressBar" /> across threads.</summary>
		/// <param name="control"></param>
		/// <param name="minimum"></param>
		/// <param name="value">  </param>
		/// <param name="maximum"></param>
		public static void Values( [NotNull] this ProgressBar control, Int32 minimum, Int32 value, Int32 maximum ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
			}

			var lowEnd = Math.Min( minimum, maximum );
			var highEnd = Math.Max( minimum, maximum );
			control.Minimum( lowEnd );
			control.Maximum( highEnd );
			control.Value( value );
		}

		/// <summary>Safely set the <see cref="Control.Visible" /> of the control across threads.</summary>
		/// <param name="control"></param>
		/// <param name="value">  </param>
		public static void Visible( [NotNull] this Control control, Boolean value ) {
			if ( control == null ) {
				throw new ArgumentNullException( nameof( control ) );
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

		public static Boolean Yup( this DialogResult result ) => result.In( DialogResult.Yes, DialogResult.OK );

	}

}

