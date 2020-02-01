// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FlexibleMessageBox.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "FlexibleMessageBox.cs" was last formatted by Protiguous on 2020/01/31 at 12:24 AM.

namespace Librainian.Controls {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;
    using JetBrains.Annotations;

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

    [SuppressMessage( "ReSharper", "InconsistentNaming" )]
    public static class FlexibleMessageBox {

        /// <summary>Defines the font for all FlexibleMessageBox instances. Default is: SystemFonts.MessageBoxFont</summary>
        public static Font FONT { get; } = SystemFonts.MessageBoxFont;

        /// <summary>
        /// Defines the maximum height for all FlexibleMessageBox instances in percent of the working area. Allowed values are 0.2 - 1.0 where: 0.2 means: The FlexibleMessageBox can
        /// be at most half as high as the working area. 1.0 means: The FlexibleMessageBox can be as high as the working area. Default is: 90% of the working area height.
        /// </summary>
        public static Double MAX_HEIGHT_FACTOR { get; } = 0.9;

        /// <summary>
        /// Defines the maximum width for all FlexibleMessageBox instances in percent of the working area. Allowed values are 0.2 - 1.0 where: 0.2 means: The FlexibleMessageBox can
        /// be at most half as wide as the working area. 1.0 means: The FlexibleMessageBox can be as wide as the working area. Default is: 70% of the working area width.
        /// </summary>
        public static Double MAX_WIDTH_FACTOR { get; } = 0.7;

        /// <summary>Shows the specified message box.</summary>
        /// <param name="text">The text.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Show( [CanBeNull] String text ) =>
            FlexibleMessageBoxForm.ShowDialog( null, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

        /// <summary>Shows the specified message box.</summary>
        /// <param name="owner">The owner.</param>
        /// <param name="text"> The text.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Show( [CanBeNull] IWin32Window owner, [CanBeNull] String text ) =>
            FlexibleMessageBoxForm.ShowDialog( owner, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

        /// <summary>Shows the specified message box.</summary>
        /// <param name="text">   The text.</param>
        /// <param name="caption">The caption.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Show( [CanBeNull] String text, [CanBeNull] String caption ) =>
            FlexibleMessageBoxForm.ShowDialog( null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

        /// <summary>Shows the specified message box.</summary>
        /// <param name="owner">  The owner.</param>
        /// <param name="text">   The text.</param>
        /// <param name="caption">The caption.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Show( [CanBeNull] IWin32Window owner, [CanBeNull] String text, [CanBeNull] String caption ) =>
            FlexibleMessageBoxForm.ShowDialog( owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

        /// <summary>Shows the specified message box.</summary>
        /// <param name="text">   The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Show( [CanBeNull] String text, [CanBeNull] String caption, MessageBoxButtons buttons ) =>
            FlexibleMessageBoxForm.ShowDialog( null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

        /// <summary>Shows the specified message box.</summary>
        /// <param name="owner">  The owner.</param>
        /// <param name="text">   The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Show( [CanBeNull] IWin32Window owner, [CanBeNull] String text, [CanBeNull] String caption, MessageBoxButtons buttons ) =>
            FlexibleMessageBoxForm.ShowDialog( owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 );

        /// <summary>Shows the specified message box.</summary>
        /// <param name="text">   The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">   The icon.</param>
        /// <returns></returns>
        public static DialogResult Show( [CanBeNull] String text, [CanBeNull] String caption, MessageBoxButtons buttons, MessageBoxIcon icon ) =>
            FlexibleMessageBoxForm.ShowDialog( null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1 );

        /// <summary>Shows the specified message box.</summary>
        /// <param name="owner">  The owner.</param>
        /// <param name="text">   The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">   The icon.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult
            Show( [CanBeNull] IWin32Window owner, [CanBeNull] String text, [CanBeNull] String caption, MessageBoxButtons buttons, MessageBoxIcon icon ) =>
            FlexibleMessageBoxForm.ShowDialog( owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1 );

        /// <summary>Shows the specified message box.</summary>
        /// <param name="text">         The text.</param>
        /// <param name="caption">      The caption.</param>
        /// <param name="buttons">      The buttons.</param>
        /// <param name="icon">         The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Show( [CanBeNull] String text, [CanBeNull] String caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton ) =>
            FlexibleMessageBoxForm.ShowDialog( null, text, caption, buttons, icon, defaultButton );

        /// <summary>Shows the specified message box.</summary>
        /// <param name="owner">        The owner.</param>
        /// <param name="text">         The text.</param>
        /// <param name="caption">      The caption.</param>
        /// <param name="buttons">      The buttons.</param>
        /// <param name="icon">         The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Show( [CanBeNull] IWin32Window owner, [CanBeNull] String text, [CanBeNull] String caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton ) =>
            FlexibleMessageBoxForm.ShowDialog( owner, text, caption, buttons, icon, defaultButton );

        /// <summary>The form to show the customized message box. It is defined as an internal class to keep the public interface of the FlexibleMessageBox clean.</summary>
        private class FlexibleMessageBoxForm : Form {

            //These separators are used for the "copy to clipboard" standard operation, triggered by Ctrl + C (behavior and clipboard format is like in a standard MessageBox)
            private const String STANDARD_MESSAGEBOX_SEPARATOR_LINES = "---------------------------\n";

            private const String STANDARD_MESSAGEBOX_SEPARATOR_SPACES = "   ";

            private static readonly String[] BUTTON_TEXTS_ENGLISH_EN = {
                "&OK", "&Cancel", "&Yes", "&No", "&Abort", "&Retry", "&Ignore"
            };

            private Button _button1;

            private Button _button2;

            private Button _button3;

            private BindingSource _flexibleMessageBoxFormBindingSource;

            private Panel _panel1;

            private PictureBox _pictureBoxForIcon;

            private RichTextBox _richTextBoxMessage;

            /// <summary>Erforderliche Designervariable.</summary>
            private IContainer components;

            private MessageBoxDefaultButton defaultButton;

            private Int32 visibleButtonsCount;

            /// <summary>The text that is been used for the heading.</summary>
            public String CaptionText { get; set; }

            /// <summary>The text that is been used in the FlexibleMessageBoxForm.</summary>
            public String MessageText { get; set; }

            //Note: This is also the fallback language

            /// <summary>Initializes a new instance of the <see cref="FlexibleMessageBoxForm" /> class.</summary>
            private FlexibleMessageBoxForm() {
                this.InitializeComponent();

                //Try to evaluate the language. If this fails, the fallback language English will be used
                Enum.TryParse( CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, out TwoLetterISOLanguageID _ );

                this.KeyPreview = true;
                this.KeyUp += this.FlexibleMessageBoxForm_KeyUp;
            }

            //These are the possible buttons (in a standard MessageBox)
            private enum ButtonID {

                OK = 0,

                CANCEL,

                YES,

                NO,

                ABORT,

                RETRY,

                IGNORE
            }

            //These are the buttons texts for different languages.
            //If you want to add a new language, add it here and in the GetButtonText-Function
            private enum TwoLetterISOLanguageID {

                en,

                de,

                es,

                it
            }

            /// <summary>Gets the button text for the CurrentUICulture language. Note: The fallback language is English</summary>
            /// <param name="buttonID">The ID of the button.</param>
            /// <returns>The button text</returns>
            [CanBeNull]
            private static String GetButtonText( ButtonID buttonID ) {
                var buttonTextArrayIndex = Convert.ToInt32( buttonID );

                return BUTTON_TEXTS_ENGLISH_EN[ buttonTextArrayIndex ];
            }

            /// <summary>
            /// Ensure the given working area factor in the range of 0.2 - 1.0 where: 0.2 means: 20 percent of the working area height or width. 1.0 means: 100 percent of the working
            /// area height or width.
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
            [CanBeNull]
            private static IEnumerable<String> GetStringRows( [CanBeNull] String message ) {
                if ( String.IsNullOrEmpty( message ) ) {
                    return null;
                }

                return message.Split( new[] {
                    '\n'
                }, StringSplitOptions.None );
            }

            /// <summary>Handles the LinkClicked event of the richTextBoxMessage control.</summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">     The <see cref="LinkClickedEventArgs" /> instance containing the event data.</param>
            private static void richTextBoxMessage_LinkClicked( [CanBeNull] Object sender, [NotNull] LinkClickedEventArgs e ) {
                try {
                    Cursor.Current = Cursors.WaitCursor;
                    Process.Start( e.LinkText );
                }
                finally {
                    Cursor.Current = Cursors.Default;
                }
            }

            /// <summary>Set dialog buttons visibilities and texts. Also set a default button.</summary>
            /// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
            /// <param name="buttons">               The buttons.</param>
            /// <param name="defaultButton">         The default button.</param>
            private static void SetDialogButtons( [NotNull] FlexibleMessageBoxForm flexibleMessageBoxForm, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton ) {

                //Set the buttons visibilities and texts
                switch ( buttons ) {
                    case MessageBoxButtons.AbortRetryIgnore:
                        flexibleMessageBoxForm.visibleButtonsCount = 3;

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
                        flexibleMessageBoxForm.visibleButtonsCount = 2;

                        flexibleMessageBoxForm._button2.Visible = true;
                        flexibleMessageBoxForm._button2.Text = GetButtonText( ButtonID.OK );
                        flexibleMessageBoxForm._button2.DialogResult = DialogResult.OK;

                        flexibleMessageBoxForm._button3.Visible = true;
                        flexibleMessageBoxForm._button3.Text = GetButtonText( ButtonID.CANCEL );
                        flexibleMessageBoxForm._button3.DialogResult = DialogResult.Cancel;

                        flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm._button3;

                        break;

                    case MessageBoxButtons.RetryCancel:
                        flexibleMessageBoxForm.visibleButtonsCount = 2;

                        flexibleMessageBoxForm._button2.Visible = true;
                        flexibleMessageBoxForm._button2.Text = GetButtonText( ButtonID.RETRY );
                        flexibleMessageBoxForm._button2.DialogResult = DialogResult.Retry;

                        flexibleMessageBoxForm._button3.Visible = true;
                        flexibleMessageBoxForm._button3.Text = GetButtonText( ButtonID.CANCEL );
                        flexibleMessageBoxForm._button3.DialogResult = DialogResult.Cancel;

                        flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm._button3;

                        break;

                    case MessageBoxButtons.YesNo:
                        flexibleMessageBoxForm.visibleButtonsCount = 2;

                        flexibleMessageBoxForm._button2.Visible = true;
                        flexibleMessageBoxForm._button2.Text = GetButtonText( ButtonID.YES );
                        flexibleMessageBoxForm._button2.DialogResult = DialogResult.Yes;

                        flexibleMessageBoxForm._button3.Visible = true;
                        flexibleMessageBoxForm._button3.Text = GetButtonText( ButtonID.NO );
                        flexibleMessageBoxForm._button3.DialogResult = DialogResult.No;

                        flexibleMessageBoxForm.ControlBox = false;

                        break;

                    case MessageBoxButtons.YesNoCancel:
                        flexibleMessageBoxForm.visibleButtonsCount = 3;

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
                        flexibleMessageBoxForm.visibleButtonsCount = 1;
                        flexibleMessageBoxForm._button3.Visible = true;
                        flexibleMessageBoxForm._button3.Text = GetButtonText( ButtonID.OK );
                        flexibleMessageBoxForm._button3.DialogResult = DialogResult.OK;

                        flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm._button3;

                        break;
                }

                //Set default button (used in FlexibleMessageBoxForm_Shown)
                flexibleMessageBoxForm.defaultButton = defaultButton;
            }

            /// <summary>Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.</summary>
            /// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
            /// <param name="icon">                  The MessageBoxIcon.</param>
            private static void SetDialogIcon( [NotNull] FlexibleMessageBoxForm flexibleMessageBoxForm, MessageBoxIcon icon ) {
                switch ( icon ) {
                    case MessageBoxIcon.Information:
                        flexibleMessageBoxForm._pictureBoxForIcon.Image = SystemIcons.Information.ToBitmap();

                        break;

                    case MessageBoxIcon.Warning:
                        flexibleMessageBoxForm._pictureBoxForIcon.Image = SystemIcons.Warning.ToBitmap();

                        break;

                    case MessageBoxIcon.Error:
                        flexibleMessageBoxForm._pictureBoxForIcon.Image = SystemIcons.Error.ToBitmap();

                        break;

                    case MessageBoxIcon.Question:
                        flexibleMessageBoxForm._pictureBoxForIcon.Image = SystemIcons.Question.ToBitmap();

                        break;

                    default:

                        //When no icon is used: Correct placement and width of rich text box.
                        flexibleMessageBoxForm._pictureBoxForIcon.Visible = false;
                        flexibleMessageBoxForm._richTextBoxMessage.Left -= flexibleMessageBoxForm._pictureBoxForIcon.Width;
                        flexibleMessageBoxForm._richTextBoxMessage.Width += flexibleMessageBoxForm._pictureBoxForIcon.Width;

                        break;
                }
            }

            /// <summary>Calculate the dialogs start size (Try to auto-size width to show longest text row). Also set the maximum dialog size.</summary>
            /// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
            /// <param name="text">                  The text (the longest text row is used to calculate the dialog width).</param>
            /// <param name="caption">               The caption (this can also affect the dialog width).</param>
            private static void SetDialogSizes( [NotNull] FlexibleMessageBoxForm flexibleMessageBoxForm, [CanBeNull] String text, [CanBeNull] String caption ) {

                //First set the bounds for the maximum dialog size
                flexibleMessageBoxForm.MaximumSize = new Size( Convert.ToInt32( SystemInformation.WorkingArea.Width * GetCorrectedWorkingAreaFactor( MAX_WIDTH_FACTOR ) ),
                    Convert.ToInt32( SystemInformation.WorkingArea.Height * GetCorrectedWorkingAreaFactor( MAX_HEIGHT_FACTOR ) ) );

                //Get rows. Exit if there are no rows to render...
                var stringRows = GetStringRows( text );

                if ( stringRows is null ) {
                    return;
                }

                //Calculate whole text height
                var textHeight = TextRenderer.MeasureText( text, FONT ).Height;

                //Calculate width for longest text line
                const Int32 SCROLLBAR_WIDTH_OFFSET = 15;
                var longestTextRowWidth = stringRows.Max( textForRow => TextRenderer.MeasureText( textForRow, FONT ).Width );
                var captionWidth = TextRenderer.MeasureText( caption, SystemFonts.CaptionFont ).Width;
                var textWidth = Math.Max( longestTextRowWidth + SCROLLBAR_WIDTH_OFFSET, captionWidth );

                //Calculate margins
                var marginWidth = flexibleMessageBoxForm.Width - flexibleMessageBoxForm._richTextBoxMessage.Width;
                var marginHeight = flexibleMessageBoxForm.Height - flexibleMessageBoxForm._richTextBoxMessage.Height;

                //Set calculated dialog size (if the calculated values exceed the maximums, they were cut by windows forms automatically)
                flexibleMessageBoxForm.Size = new Size( textWidth + marginWidth, textHeight + marginHeight );
            }

            /// <summary>Set the dialogs start position when given. Otherwise center the dialog on the current screen.</summary>
            /// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
            /// <param name="owner">                 The owner.</param>
            private static void SetDialogStartPosition( [CanBeNull] FlexibleMessageBoxForm flexibleMessageBoxForm, [CanBeNull] IWin32Window owner ) {

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
            private void FlexibleMessageBoxForm_KeyUp( [CanBeNull] Object sender, [NotNull] KeyEventArgs e ) {

                //Handle standard key strikes for clipboard copy: "Ctrl + C" and "Ctrl + Insert"
                if ( e.Control && ( e.KeyCode == Keys.C || e.KeyCode == Keys.Insert ) ) {
                    var buttonsTextLine = ( this._button1.Visible ? this._button1.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : String.Empty ) +
                                          ( this._button2.Visible ? this._button2.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : String.Empty ) +
                                          ( this._button3.Visible ? this._button3.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : String.Empty );

                    //Build same clipboard text like the standard .Net MessageBox
                    var textForClipboard = STANDARD_MESSAGEBOX_SEPARATOR_LINES + this.Text + Environment.NewLine + STANDARD_MESSAGEBOX_SEPARATOR_LINES +
                                           this._richTextBoxMessage.Text + Environment.NewLine + STANDARD_MESSAGEBOX_SEPARATOR_LINES +
                                           buttonsTextLine.Replace( "&", String.Empty ) + Environment.NewLine + STANDARD_MESSAGEBOX_SEPARATOR_LINES;

                    //Set text in clipboard
                    Clipboard.SetText( textForClipboard );
                }
            }

            /// <summary>Handles the Shown event of the FlexibleMessageBoxForm control.</summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">     The <see cref="EventArgs" /> instance containing the event data.</param>
            private void FlexibleMessageBoxForm_Shown( [CanBeNull] Object sender, [CanBeNull] EventArgs e ) {
                Int32 buttonIndexToFocus;
                Button buttonToFocus;

                //Set the default button...
                switch ( this.defaultButton ) {
                    case MessageBoxDefaultButton.Button1:
                        buttonIndexToFocus = 1;

                        break;

                    case MessageBoxDefaultButton.Button2:
                        buttonIndexToFocus = 2;

                        break;

                    case MessageBoxDefaultButton.Button3:
                        buttonIndexToFocus = 3;

                        break;

                    default:
                        buttonIndexToFocus = 1;

                        break;
                }

                if ( buttonIndexToFocus > this.visibleButtonsCount ) {
                    buttonIndexToFocus = this.visibleButtonsCount;
                }

                switch ( buttonIndexToFocus ) {
                    case 3: {
                            buttonToFocus = this._button3;

                            break;
                        }

                    case 2: {
                            buttonToFocus = this._button2;

                            break;
                        }

                    default: {
                            buttonToFocus = this._button1;

                            break;
                        }
                }

                buttonToFocus.Focus();
            }

            /// <summary>Erforderliche Methode für die Designerunterstützung. Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.</summary>
            private void InitializeComponent() {
                this.components = new Container();
                this._button1 = new Button();
                this._richTextBoxMessage = new RichTextBox();
                this._flexibleMessageBoxFormBindingSource = new BindingSource( this.components );
                this._panel1 = new Panel();
                this._pictureBoxForIcon = new PictureBox();
                this._button2 = new Button();
                this._button3 = new Button();
                ( ( ISupportInitialize )this._flexibleMessageBoxFormBindingSource ).BeginInit();
                this._panel1.SuspendLayout();
                ( ( ISupportInitialize )this._pictureBoxForIcon ).BeginInit();
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

                // richTextBoxMessage
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
                this._panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                this._panel1.BackColor = Color.White;
                this._panel1.Controls.Add( this._pictureBoxForIcon );
                this._panel1.Controls.Add( this._richTextBoxMessage );
                this._panel1.Location = new Point( -3, -4 );
                this._panel1.Name = "_panel1";
                this._panel1.Size = new Size( 268, 59 );
                this._panel1.TabIndex = 1;

                // pictureBoxForIcon
                this._pictureBoxForIcon.BackColor = Color.Transparent;
                this._pictureBoxForIcon.Location = new Point( 15, 19 );
                this._pictureBoxForIcon.Name = "_pictureBoxForIcon";
                this._pictureBoxForIcon.Size = new Size( 32, 32 );
                this._pictureBoxForIcon.TabIndex = 8;
                this._pictureBoxForIcon.TabStop = false;

                // button2
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

                // button3
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

                // FlexibleMessageBoxForm
                this.AutoScaleDimensions = new SizeF( 6F, 13F );
                this.AutoScaleMode = AutoScaleMode.Font;
                this.ClientSize = new Size( 260, 102 );
                this.Controls.Add( this._button3 );
                this.Controls.Add( this._button2 );
                this.Controls.Add( this._panel1 );
                this.Controls.Add( this._button1 );
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
                ( ( ISupportInitialize )this._flexibleMessageBoxFormBindingSource ).EndInit();
                this._panel1.ResumeLayout( false );
                ( ( ISupportInitialize )this._pictureBoxForIcon ).EndInit();
                this.ResumeLayout( false );
                this.PerformLayout();
            }

            /// <summary>Verwendete Ressourcen bereinigen.</summary>
            /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
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
            public static DialogResult ShowDialog( [CanBeNull] IWin32Window owner, [CanBeNull] String text, [CanBeNull] String caption, MessageBoxButtons buttons,
                MessageBoxIcon icon, MessageBoxDefaultButton defaultButton ) {
                var func = new Func<DialogResult>( () => {

                    //Create a new instance of the FlexibleMessageBox form
                    var flexibleMessageBoxForm = new FlexibleMessageBoxForm {
                        ShowInTaskbar = false,
                        CaptionText = caption,
                        MessageText = text
                    };

                    flexibleMessageBoxForm._flexibleMessageBoxFormBindingSource.DataSource = flexibleMessageBoxForm;

                    //Set the buttons visibilities and texts. Also set a default button.
                    SetDialogButtons( flexibleMessageBoxForm, buttons, defaultButton );

                    //Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.
                    SetDialogIcon( flexibleMessageBoxForm, icon );

                    //Set the font for all controls
                    flexibleMessageBoxForm.Font = FONT;
                    flexibleMessageBoxForm._richTextBoxMessage.Font = FONT;

                    //Calculate the dialogs start size (Try to auto-size width to show longest text row). Also set the maximum dialog size.
                    SetDialogSizes( flexibleMessageBoxForm, text, caption );

                    //Set the dialogs start position when given. Otherwise center the dialog on the current screen.
                    SetDialogStartPosition( flexibleMessageBoxForm, owner );

                    //Show the dialog
                    return flexibleMessageBoxForm.ShowDialog( owner );
                } );

                if ( owner is Control control && control.InvokeRequired ) {
                    return ( DialogResult )control.Invoke( func );
                }

                return func();
            }
        }
    }
}