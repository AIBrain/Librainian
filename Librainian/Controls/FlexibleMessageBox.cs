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
// File "FlexibleMessageBox.cs" last formatted on 2020-08-14 at 8:32 PM.

#nullable enable

namespace Librainian.Controls {

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;
	using Exceptions;

	/*  FlexibleMessageBox – A flexible replacement for the .NET MessageBox
     *
     *  Author:         Jörg Reichert (public@jreichert.de)
     *  Contributors:   Thanks to: David Hall, Roink
     *  Version:        1.3
     *  Published at:   http://www.codeproject.com/Articles/601900/FlexibleMessageBox
     *
     ************************************************************************************************************
     * Features:
     *  - It can be simply used instead of MessageBox since all important static "Show"-Functions are supported
     *  - It is small, only one source file, which could be added easily to each solution
     *  - It can be resized and the content is correctly word-wrapped
     *  - It tries to auto-size the width to show the longest text row
     *  - It never exceeds the current desktop working area
     *  - It displays a vertical scrollbar when needed
     *  - It does support hyperlinks in text
     *
     *  Because the interface is identical to MessageBox, you can add this single source file to your project
     *  and use the FlexibleMessageBox almost everywhere you use a standard MessageBox.
     *  The goal was NOT to produce as many features as possible but to provide a simple replacement to fit my
     *  own needs. Feel free to add additional features on your own, but please left my credits in this class.
     *
     ************************************************************************************************************
     * Usage examples:
     *
     *  FlexibleMessageBox.Show("Just a text");
     *
     *  FlexibleMessageBox.Show("A text",
     *                          "A caption");
     *
     *  FlexibleMessageBox.Show("Some text with a link: www.google.com",
     *                          "Some caption",
     *                          MessageBoxButtons.AbortRetryIgnore,
     *                          MessageBoxIcon.Information,
     *                          MessageBoxDefaultButton.Button2);
     *
     *  var dialogResult = FlexibleMessageBox.Show("Do you know the answer to life the universe and everything?",
     *                                             "One short question",
     *                                             MessageBoxButtons.YesNo);
     *
     ************************************************************************************************************
     *  THE SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS", WITHOUT WARRANTY
     *  OF ANY KIND, EXPRESS OR IMPLIED. IN NO EVENT SHALL THE AUTHOR BE
     *  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY ARISING FROM,
     *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OF THIS
     *  SOFTWARE.
     ************************************************************************************************************
    */

	public static class FlexibleMessageBox {

		/// <summary>Defines the font for all FlexibleMessageBox instances. Default is: SystemFonts.MessageBoxFont</summary>
		public static Font? Font { get; } = SystemFonts.MessageBoxFont;

		/// <summary>
		///     Defines the maximum height for all FlexibleMessageBox instances in percent of the working area. Allowed values are
		///     0.2 - 1.0 where: 0.2 means: The FlexibleMessageBox can
		///     be at most half as high as the working area. 1.0 means: The FlexibleMessageBox can be as high as the working area.
		///     Default is: 90% of the working area height.
		/// </summary>
		public static Double MaxHeightFactor => 0.9;

		/// <summary>
		///     Defines the maximum width for all FlexibleMessageBox instances in percent of the working area. Allowed values are
		///     0.2 - 1.0 where: 0.2 means: The FlexibleMessageBox can
		///     be at most half as wide as the working area. 1.0 means: The FlexibleMessageBox can be as wide as the working area.
		///     Default is: 70% of the working area width.
		/// </summary>
		public static Double MaxWidthFactor => 0.7;

		/// <summary>Shows the specified message box.</summary>
		/// <param name="text">The text.</param>
		/// <returns>The dialog result.</returns>
		public static DialogResult? Show( String? text ) =>
			FlexibleMessageBoxForm.ShowDialog( null, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

		/// <summary>Shows the specified message box.</summary>
		/// <param name="owner">The owner.</param>
		/// <param name="text"> The text.</param>
		/// <returns>The dialog result.</returns>
		public static DialogResult? Show( IWin32Window? owner, String? text ) =>
			FlexibleMessageBoxForm.ShowDialog( owner, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

		/// <summary>Shows the specified message box.</summary>
		/// <param name="text">   The text.</param>
		/// <param name="caption">The caption.</param>
		/// <returns>The dialog result.</returns>
		public static DialogResult? Show( String? text, String? caption ) =>
			FlexibleMessageBoxForm.ShowDialog( null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

		/// <summary>Shows the specified message box.</summary>
		/// <param name="owner">  The owner.</param>
		/// <param name="text">   The text.</param>
		/// <param name="caption">The caption.</param>
		/// <returns>The dialog result.</returns>
		public static DialogResult? Show( IWin32Window? owner, String? text, String? caption ) =>
			FlexibleMessageBoxForm.ShowDialog( owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

		/// <summary>Shows the specified message box.</summary>
		/// <param name="text">   The text.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">The buttons.</param>
		/// <returns>The dialog result.</returns>
		public static DialogResult? Show( String? text, String? caption, MessageBoxButtons buttons ) =>
			FlexibleMessageBoxForm.ShowDialog( null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

		/// <summary>Shows the specified message box.</summary>
		/// <param name="owner">  The owner.</param>
		/// <param name="text">   The text.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">The buttons.</param>
		/// <returns>The dialog result.</returns>
		public static DialogResult? Show( IWin32Window? owner, String? text, String? caption, MessageBoxButtons buttons ) =>
			FlexibleMessageBoxForm.ShowDialog( owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

		/// <summary>Shows the specified message box.</summary>
		/// <param name="text">   The text.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">The buttons.</param>
		/// <param name="icon">   The icon.</param>
		public static DialogResult? Show( String? text, String? caption, MessageBoxButtons buttons, MessageBoxIcon icon ) =>
			FlexibleMessageBoxForm.ShowDialog( null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1 );

		/// <summary>Shows the specified message box.</summary>
		/// <param name="owner">  The owner.</param>
		/// <param name="text">   The text.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">The buttons.</param>
		/// <param name="icon">   The icon.</param>
		/// <returns>The dialog result.</returns>
		public static DialogResult? Show(
			IWin32Window? owner,
			String? text,
			String? caption,
			MessageBoxButtons buttons,
			MessageBoxIcon icon
		) =>
			FlexibleMessageBoxForm.ShowDialog( owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1 );

		/// <summary>Shows the specified message box.</summary>
		/// <param name="text">         The text.</param>
		/// <param name="caption">      The caption.</param>
		/// <param name="buttons">      The buttons.</param>
		/// <param name="icon">         The icon.</param>
		/// <param name="defaultButton">The default button.</param>
		/// <returns>The dialog result.</returns>
		public static DialogResult? Show(
			String? text,
			String? caption,
			MessageBoxButtons buttons,
			MessageBoxIcon icon,
			MessageBoxDefaultButton defaultButton
		) =>
			FlexibleMessageBoxForm.ShowDialog( null, text, caption, buttons, icon, defaultButton );

		/// <summary>Shows the specified message box.</summary>
		/// <param name="owner">        The owner.</param>
		/// <param name="text">         The text.</param>
		/// <param name="caption">      The caption.</param>
		/// <param name="buttons">      The buttons.</param>
		/// <param name="icon">         The icon.</param>
		/// <param name="defaultButton">The default button.</param>
		/// <returns>The dialog result.</returns>
		public static DialogResult? Show(
			IWin32Window? owner,
			String? text,
			String? caption,
			MessageBoxButtons buttons,
			MessageBoxIcon icon,
			MessageBoxDefaultButton defaultButton
		) =>
			FlexibleMessageBoxForm.ShowDialog( owner, text, caption, buttons, icon, defaultButton );

		/// <summary>
		///     The form to show the customized message box. It is defined as an internal class to keep the public interface
		///     of the FlexibleMessageBox clean.
		/// </summary>
		private class FlexibleMessageBoxForm : Form {

			//These separators are used for the "copy to clipboard" standard operation, triggered by Ctrl + C (behavior and clipboard format is like in a standard MessageBox)
			private const String StandardMessageboxSeparatorLines = "---------------------------\n";

			private const String StandardMessageboxSeparatorSpaces = "   ";

			private static readonly String[] ButtonTextsEnglish = {
				"&OK", "&Cancel", "&Yes", "&No", "&Abort", "&Retry", "&Ignore"
			};

			private Button _button1;

			private Button _button2;

			private Button _button3;

			private MessageBoxDefaultButton _defaultButton;

			private BindingSource? _flexibleMessageBoxFormBindingSource;

			private Panel? _panel1;

			private RichTextBox? _richTextBoxMessage;

			private Int32 _visibleButtonsCount;

			/// <summary>Erforderliche Designervariable.</summary>
			private IContainer? components;

			/// <summary>The text that is been used for the heading.</summary>
			public String? CaptionText { get; set; }

			/// <summary>The text that is been used in the FlexibleMessageBoxForm.</summary>
			public String? MessageText { get; set; }

			/// <summary>Initializes a new instance of the <see cref="FlexibleMessageBoxForm" /> class.</summary>
			private FlexibleMessageBoxForm() {
				this.InitializeComponent();
				this.KeyPreview = true;
				this.KeyUp += this.FlexibleMessageBoxForm_KeyUp;
			}

			//These are the possible buttons (in a standard MessageBox)
			[SuppressMessage( "ReSharper", "InconsistentNaming" )]
			private enum ButtonID {

				OK = 0,

				CANCEL,

				YES,

				NO,

				ABORT,

				RETRY,

				IGNORE
			}

			/// <summary>Gets the button text for the CurrentUICulture language. Note: The fallback language is English</summary>
			/// <param name="buttonID">The ID of the button.</param>
			/// <returns>The button text</returns>
			private static String GetButtonText( ButtonID buttonID ) {
				var buttonTextArrayIndex = Convert.ToInt32( buttonID );

				return ButtonTextsEnglish[buttonTextArrayIndex];
			}

			/// <summary>
			///     Ensure the given working area factor in the range of 0.2 - 1.0 where: 0.2 means: 20 percent of the working area
			///     height or width. 1.0 means: 100 percent of the working
			///     area height or width.
			/// </summary>
			/// <param name="workingAreaFactor">The given working area factor.</param>
			/// <returns>The corrected given working area factor.</returns>
			private static Double GetCorrectedWorkingAreaFactor( Double workingAreaFactor ) {
				const Double minFactor = 0.2;
				const Double maxFactor = 1.0;

				if ( workingAreaFactor < minFactor ) {
					return minFactor;
				}

				if ( workingAreaFactor > maxFactor ) {
					return maxFactor;
				}

				return workingAreaFactor;
			}

			/// <summary>Gets the string rows.</summary>
			/// <param name="message">The message.</param>
			/// <returns>The string rows as 1-dimensional array</returns>
			private static IEnumerable<String?>? GetStringRows( String? message ) {
				if ( String.IsNullOrEmpty( message ) ) {
					return default( IEnumerable<String?>? );
				}

				return message.Split( '\n', StringSplitOptions.None );
			}

			/// <summary>Handles the LinkClicked event of the richTextBoxMessage control.</summary>
			/// <param name="sender">The source of the event.</param>
			/// <param name="e">     The <see cref="LinkClickedEventArgs" /> instance containing the event data.</param>
			private static void richTextBoxMessage_LinkClicked( Object? sender, LinkClickedEventArgs e ) {
				try {
					Cursor.Current = Cursors.WaitCursor;

					if ( !String.IsNullOrEmpty( e.LinkText ) ) {
						Process.Start( e.LinkText );
					}
				}
				finally {
					Cursor.Current = Cursors.Default;
				}
			}

			/// <summary>Set dialog buttons visibilities and texts. Also set a default button.</summary>
			/// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
			/// <param name="buttons">               The buttons.</param>
			/// <param name="defaultButton">         The default button.</param>
			private static void SetDialogButtons(
				FlexibleMessageBoxForm flexibleMessageBoxForm,
				MessageBoxButtons buttons,
				MessageBoxDefaultButton defaultButton
			) {

				//Set the buttons visibilities and texts
				switch ( buttons ) {
					case MessageBoxButtons.AbortRetryIgnore:
						flexibleMessageBoxForm._visibleButtonsCount = 3;

						flexibleMessageBoxForm._button1.Visible = true;
						flexibleMessageBoxForm._button1.Text = GetButtonText( ButtonID.ABORT );
						flexibleMessageBoxForm._button1.DialogResult = DialogResult.Abort;

						flexibleMessageBoxForm._button2.Visible = true;
						flexibleMessageBoxForm._button2.Text = GetButtonText( ButtonID.RETRY );
						flexibleMessageBoxForm._button2.DialogResult = DialogResult.Retry;

						flexibleMessageBoxForm._button3.Visible = true;
						flexibleMessageBoxForm._button3.Text = GetButtonText( ButtonID.IGNORE );
						flexibleMessageBoxForm._button3.DialogResult = DialogResult.Ignore;

						flexibleMessageBoxForm.ControlBox = false;

						break;

					case MessageBoxButtons.OKCancel:
						flexibleMessageBoxForm._visibleButtonsCount = 2;

						flexibleMessageBoxForm._button2.Visible = true;
						flexibleMessageBoxForm._button2.Text = GetButtonText( ButtonID.OK );
						flexibleMessageBoxForm._button2.DialogResult = DialogResult.OK;

						flexibleMessageBoxForm._button3.Visible = true;
						flexibleMessageBoxForm._button3.Text = GetButtonText( ButtonID.CANCEL );
						flexibleMessageBoxForm._button3.DialogResult = DialogResult.Cancel;

						flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm._button3;

						break;

					case MessageBoxButtons.RetryCancel:
						flexibleMessageBoxForm._visibleButtonsCount = 2;

						flexibleMessageBoxForm._button2.Visible = true;
						flexibleMessageBoxForm._button2.Text = GetButtonText( ButtonID.RETRY );
						flexibleMessageBoxForm._button2.DialogResult = DialogResult.Retry;

						flexibleMessageBoxForm._button3.Visible = true;
						flexibleMessageBoxForm._button3.Text = GetButtonText( ButtonID.CANCEL );
						flexibleMessageBoxForm._button3.DialogResult = DialogResult.Cancel;

						flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm._button3;

						break;

					case MessageBoxButtons.YesNo:
						flexibleMessageBoxForm._visibleButtonsCount = 2;

						flexibleMessageBoxForm._button2.Visible = true;
						flexibleMessageBoxForm._button2.Text = GetButtonText( ButtonID.YES );
						flexibleMessageBoxForm._button2.DialogResult = DialogResult.Yes;

						flexibleMessageBoxForm._button3.Visible = true;
						flexibleMessageBoxForm._button3.Text = GetButtonText( ButtonID.NO );
						flexibleMessageBoxForm._button3.DialogResult = DialogResult.No;

						flexibleMessageBoxForm.ControlBox = false;

						break;

					case MessageBoxButtons.YesNoCancel:
						flexibleMessageBoxForm._visibleButtonsCount = 3;

						flexibleMessageBoxForm._button1.Visible = true;
						flexibleMessageBoxForm._button1.Text = GetButtonText( ButtonID.YES );
						flexibleMessageBoxForm._button1.DialogResult = DialogResult.Yes;

						flexibleMessageBoxForm._button2.Visible = true;
						flexibleMessageBoxForm._button2.Text = GetButtonText( ButtonID.NO );
						flexibleMessageBoxForm._button2.DialogResult = DialogResult.No;

						flexibleMessageBoxForm._button3.Visible = true;
						flexibleMessageBoxForm._button3.Text = GetButtonText( ButtonID.CANCEL );
						flexibleMessageBoxForm._button3.DialogResult = DialogResult.Cancel;

						flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm._button3;

						break;

					case MessageBoxButtons.OK:
					default:
						flexibleMessageBoxForm._visibleButtonsCount = 1;
						flexibleMessageBoxForm._button3.Visible = true;
						flexibleMessageBoxForm._button3.Text = GetButtonText( ButtonID.OK );
						flexibleMessageBoxForm._button3.DialogResult = DialogResult.OK;

						flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm._button3;

						break;
				}

				//Set default button (used in FlexibleMessageBoxForm_Shown)
				flexibleMessageBoxForm._defaultButton = defaultButton;
			}

			/// <summary>Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.</summary>
			/// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
			/// <param name="icon">                  The MessageBoxIcon.</param>
			private static void SetDialogIcon( FlexibleMessageBoxForm flexibleMessageBoxForm, MessageBoxIcon icon ) {
				var pictureBoxForIcon = new PictureBox();
				switch ( icon ) {
					case MessageBoxIcon.Information: {
							pictureBoxForIcon.Image = SystemIcons.Information.ToBitmap();

							break;
						}
					case MessageBoxIcon.Warning: {
							pictureBoxForIcon.Image = SystemIcons.Warning.ToBitmap();

							break;
						}

					case MessageBoxIcon.Error: {
							pictureBoxForIcon.Image = SystemIcons.Error.ToBitmap();

							break;
						}

					case MessageBoxIcon.Question: {
							pictureBoxForIcon.Image = SystemIcons.Question.ToBitmap();

							break;
						}

					default: {

							//When no icon is used: Correct placement and width of rich text box.
							pictureBoxForIcon.Visible = false;
							if ( flexibleMessageBoxForm._richTextBoxMessage != null ) {
								flexibleMessageBoxForm._richTextBoxMessage.Left -= pictureBoxForIcon.Width;
								flexibleMessageBoxForm._richTextBoxMessage.Width += pictureBoxForIcon.Width;
							}

							break;
						}
				}
			}

			/// <summary>
			///     Calculate the dialogs start size (Try to auto-size width to show longest text row). Also set the maximum
			///     dialog size.
			/// </summary>
			/// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
			/// <param name="text">                  The text (the longest text row is used to calculate the dialog width).</param>
			/// <param name="caption">               The caption (this can also affect the dialog width).</param>
			private static void SetDialogSizes(
				FlexibleMessageBoxForm flexibleMessageBoxForm,
				String? text,
				String? caption
			) {

				//First set the bounds for the maximum dialog size
				flexibleMessageBoxForm.MaximumSize = new Size( Convert.ToInt32( SystemInformation.WorkingArea.Width * GetCorrectedWorkingAreaFactor( MaxWidthFactor ) ),
					Convert.ToInt32( SystemInformation.WorkingArea.Height * GetCorrectedWorkingAreaFactor( MaxHeightFactor ) ) );

				//Get rows. Exit if there are no rows to render...
				var stringRows = GetStringRows( text );

				if ( stringRows is null ) {
					return;
				}

				//Calculate whole text height
				var textHeight = TextRenderer.MeasureText( text, FlexibleMessageBox.Font ).Height;

				//Calculate width for longest text line
				const Int32 scrollbarWidthOffset = 15;
				var longestTextRowWidth = stringRows.Max( textForRow => TextRenderer.MeasureText( textForRow, FlexibleMessageBox.Font ).Width );
				var captionWidth = TextRenderer.MeasureText( caption, SystemFonts.CaptionFont ).Width;
				var textWidth = Math.Max( longestTextRowWidth + scrollbarWidthOffset, captionWidth );

				//Calculate margins
				if ( flexibleMessageBoxForm._richTextBoxMessage != null ) {
					var marginWidth = flexibleMessageBoxForm.Width - flexibleMessageBoxForm._richTextBoxMessage.Width;
					var marginHeight = flexibleMessageBoxForm.Height - flexibleMessageBoxForm._richTextBoxMessage.Height;

					//Set calculated dialog size (if the calculated values exceed the maximums, they were cut by windows forms automatically)
					flexibleMessageBoxForm.Size = new Size( textWidth + marginWidth, textHeight + marginHeight );
				}
			}

			/// <summary>Set the dialogs start position when given. Otherwise center the dialog on the current screen.</summary>
			/// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
			/// <param name="owner">                 The owner.</param>
			private static void SetDialogStartPosition( Form flexibleMessageBoxForm, IWin32Window? owner ) {

				//If no owner given: Center on current screen
				if ( owner is null ) {
					var screen = Screen.FromPoint( Cursor.Position );

					flexibleMessageBoxForm.StartPosition = FormStartPosition.Manual;
					flexibleMessageBoxForm.Left = screen.Bounds.Left + screen.Bounds.Width / 2 - flexibleMessageBoxForm.Width / 2;
					flexibleMessageBoxForm.Top = screen.Bounds.Top + screen.Bounds.Height / 2 - flexibleMessageBoxForm.Height / 2;
				}
			}

			/// <summary>Handles the KeyUp event of the richTextBoxMessage control.</summary>
			/// <param name="sender">The source of the event.</param>
			/// <param name="e">     The <see cref="KeyEventArgs" /> instance containing the event data.</param>
			private void FlexibleMessageBoxForm_KeyUp( Object? sender, KeyEventArgs? e ) {

				//Handle standard key strikes for clipboard copy: "Ctrl + C" and "Ctrl + Insert"
				if ( e?.Control == true && e.KeyCode is Keys.C or Keys.Insert ) {
					var buttonsTextLine = ( this._button1.Visible ? this._button1.Text + StandardMessageboxSeparatorSpaces : String.Empty ) +
										  ( this._button2.Visible ? this._button2.Text + StandardMessageboxSeparatorSpaces : String.Empty ) +
										  ( this._button3.Visible ? this._button3.Text + StandardMessageboxSeparatorSpaces : String.Empty );

					//Build same clipboard text like the standard .Net MessageBox
					var textForClipboard =
						$"{StandardMessageboxSeparatorLines}{this.Text}{Environment.NewLine}{StandardMessageboxSeparatorLines}{this._richTextBoxMessage?.Text}{Environment.NewLine}{StandardMessageboxSeparatorLines}{buttonsTextLine.Replace( "&", String.Empty )}{Environment.NewLine}{StandardMessageboxSeparatorLines}";

					//Set text in clipboard
					Clipboard.SetText( textForClipboard );
				}
			}

			/// <summary>Handles the Shown event of the FlexibleMessageBoxForm control.</summary>
			/// <param name="sender">The source of the event.</param>
			/// <param name="e">     The <see cref="EventArgs" /> instance containing the event data.</param>
			private void FlexibleMessageBoxForm_Shown( Object? sender, EventArgs? e ) {

				//Set the default button...
				var buttonIndexToFocus = this._defaultButton switch {
					MessageBoxDefaultButton.Button1 => 1,
					MessageBoxDefaultButton.Button2 => 2,
					MessageBoxDefaultButton.Button3 => 3,
					var _ => 1
				};

				if ( buttonIndexToFocus > this._visibleButtonsCount ) {
					buttonIndexToFocus = this._visibleButtonsCount;
				}

				var buttonToFocus = buttonIndexToFocus switch {
					3 => this._button3,
					2 => this._button2,
					var _ => this._button1
				};

				buttonToFocus.Focus();
			}

			/// <summary>
			///     Erforderliche Methode für die Designerunterstützung. Der Inhalt der Methode darf nicht mit dem Code-Editor
			///     geändert werden.
			/// </summary>
			private void InitializeComponent() {
				this.components = new Container();
				this._button1 = new Button();
				this._richTextBoxMessage = new RichTextBox();
				this._flexibleMessageBoxFormBindingSource = new BindingSource( this.components );
				this._panel1 = new Panel();
				var pictureBoxForIcon = new PictureBox();
				this._button2 = new Button();
				this._button3 = new Button();
				( ( ISupportInitialize )this._flexibleMessageBoxFormBindingSource ).BeginInit();
				this._panel1.SuspendLayout();
				( ( ISupportInitialize )pictureBoxForIcon ).BeginInit();
				this.SuspendLayout();

				// button1
				this._button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
				this._button1.AutoSize = true;
				this._button1.DialogResult = DialogResult.OK;
				this._button1.Location = new Point( 11, 67 );
				this._button1.MinimumSize = new Size( 0, 24 );
				this._button1.Name = "_button1";
				this._button1.Size = new Size( 75, 24 );
				this._button1.TabIndex = 2;
				this._button1.Text = "OK";
				this._button1.UseVisualStyleBackColor = true;
				this._button1.Visible = false;
				this.Controls.Add( this._button1 );

				// richTextBoxMessage
				if ( this._richTextBoxMessage != null ) {
					this._richTextBoxMessage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
					this._richTextBoxMessage.BackColor = Color.White;
					this._richTextBoxMessage.BorderStyle = BorderStyle.None;

					this._richTextBoxMessage.DataBindings.Add( new Binding( "Text", this._flexibleMessageBoxFormBindingSource, "MessageText", true,
						DataSourceUpdateMode.OnPropertyChanged ) );

					this._richTextBoxMessage.Font = new Font( "Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0 );
					this._richTextBoxMessage.Location = new Point( 50, 26 );
					this._richTextBoxMessage.Margin = new Padding( 0 );
					this._richTextBoxMessage.Name = "_richTextBoxMessage";
					this._richTextBoxMessage.ReadOnly = true;
					this._richTextBoxMessage.ScrollBars = RichTextBoxScrollBars.Vertical;
					this._richTextBoxMessage.Size = new Size( 200, 20 );
					this._richTextBoxMessage.TabIndex = 0;
					this._richTextBoxMessage.TabStop = false;
					this._richTextBoxMessage.Text = "<Message>";
					this._richTextBoxMessage.LinkClicked += richTextBoxMessage_LinkClicked;

					// panel1
					if ( this._panel1 != null ) {
						this._panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
						this._panel1.BackColor = Color.White;
						this._panel1.Controls.Add( pictureBoxForIcon );
						this._panel1.Controls.Add( this._richTextBoxMessage );
					}
				}

				if ( this._panel1 != null ) {
					this._panel1.Location = new Point( -3, -4 );
					this._panel1.Name = "_panel1";
					this._panel1.Size = new Size( 268, 59 );
					this._panel1.TabIndex = 1;
					this.Controls.Add( this._panel1 );
				}

				pictureBoxForIcon.BackColor = Color.Transparent;
				pictureBoxForIcon.Location = new Point( 15, 19 );
				pictureBoxForIcon.Name = nameof( pictureBoxForIcon );
				pictureBoxForIcon.Size = new Size( 32, 32 );
				pictureBoxForIcon.TabIndex = 8;
				pictureBoxForIcon.TabStop = false;

				this._button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
				this._button2.DialogResult = DialogResult.OK;
				this._button2.Location = new Point( 92, 67 );
				this._button2.MinimumSize = new Size( 0, 24 );
				this._button2.Name = "_button2";
				this._button2.Size = new Size( 75, 24 );
				this._button2.TabIndex = 3;
				this._button2.Text = "OK";
				this._button2.UseVisualStyleBackColor = true;
				this._button2.Visible = false;

				this.Controls.Add( this._button2 );

				this._button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
				this._button3.AutoSize = true;
				this._button3.DialogResult = DialogResult.OK;
				this._button3.Location = new Point( 173, 67 );
				this._button3.MinimumSize = new Size( 0, 24 );
				this._button3.Name = "_button3";
				this._button3.Size = new Size( 75, 24 );
				this._button3.TabIndex = 0;
				this._button3.Text = "OK";
				this._button3.UseVisualStyleBackColor = true;
				this._button3.Visible = false;

				this.AutoScaleDimensions = new SizeF( 6F, 13F );
				this.AutoScaleMode = AutoScaleMode.Font;
				this.ClientSize = new Size( 260, 102 );
				this.Controls.Add( this._button3 );

				this.DataBindings.Add( new Binding( "Text", this._flexibleMessageBoxFormBindingSource, "CaptionText", true ) );

				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.MinimumSize = new Size( 276, 140 );
				this.Name = "FlexibleMessageBoxForm";
				this.ShowIcon = false;
				this.SizeGripStyle = SizeGripStyle.Show;
				this.StartPosition = FormStartPosition.CenterParent;
				this.Text = "<Caption>";
				this.Shown += this.FlexibleMessageBoxForm_Shown;
				( ( ISupportInitialize )this._flexibleMessageBoxFormBindingSource )?.EndInit();
				this._panel1?.ResumeLayout( false );
				( ( ISupportInitialize )pictureBoxForIcon ).EndInit();
				this.ResumeLayout( false );
				this.PerformLayout();
			}

			protected override void Dispose( Boolean disposing ) {
				if ( disposing ) {
					this.components?.Dispose();
				}

				base.Dispose( disposing );
			}

			/// <summary>Shows the specified message box.</summary>
			/// <param name="owner">        The owner.</param>
			/// <param name="text">         The text.</param>
			/// <param name="caption">      The caption.</param>
			/// <param name="buttons">      The buttons.</param>
			/// <param name="icon">         The icon.</param>
			/// <param name="defaultButton">The default button.</param>
			/// <returns>The dialog result.</returns>
			public static DialogResult? ShowDialog(
				IWin32Window? owner,
				String? text,
				String? caption,
				MessageBoxButtons buttons,
				MessageBoxIcon icon,
				MessageBoxDefaultButton defaultButton
			) {
				var func = new Func<DialogResult?>( () => {

					//Create a new instance of the FlexibleMessageBox form
					var flexibleMessageBoxForm = new FlexibleMessageBoxForm {
						ShowInTaskbar = false,
						CaptionText = caption,
						MessageText = text
					};

					if ( flexibleMessageBoxForm._flexibleMessageBoxFormBindingSource != null ) {
						flexibleMessageBoxForm._flexibleMessageBoxFormBindingSource.DataSource = flexibleMessageBoxForm;
					}

					//Set the buttons visibilities and texts. Also set a default button.
					SetDialogButtons( flexibleMessageBoxForm, buttons, defaultButton );

					//Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.
					SetDialogIcon( flexibleMessageBoxForm, icon );

					//Set the font for all controls
					flexibleMessageBoxForm.Font = FlexibleMessageBox.Font ?? throw new InvalidOperationException( "Invalid font." );
					if ( flexibleMessageBoxForm._richTextBoxMessage != null ) {
						flexibleMessageBoxForm._richTextBoxMessage.Font = FlexibleMessageBox.Font;
					}

					//Calculate the dialogs start size (Try to auto-size width to show longest text row). Also set the maximum dialog size.
					SetDialogSizes( flexibleMessageBoxForm, text, caption );

					//Set the dialogs start position when given. Otherwise center the dialog on the current screen.
					SetDialogStartPosition( flexibleMessageBoxForm, owner );

					//Show the dialog
					return flexibleMessageBoxForm.ShowDialog( owner ?? throw new ArgumentEmptyException( nameof( owner ) ) );
				} );

				if ( owner is Control { InvokeRequired: true } control ) {
					return control.Invoke( func );
				}

				return func();
			}

			//These are the buttons texts for different languages.
			//If you want to add a new language, add it here and in the GetButtonText-Function
		}
	}
}