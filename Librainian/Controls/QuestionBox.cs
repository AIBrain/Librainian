// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "QuestionBox.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "QuestionBox.cs" was last formatted by Protiguous on 2020/01/31 at 12:24 AM.

namespace Librainian.Controls {

    using System;
    using System.Windows.Forms;
    using JetBrains.Annotations;
    using Parsing;

    /// <summary>Better than a messagebox?</summary>
    public partial class QuestionBox : Form {

        [CanBeNull]
        public String Response { get; private set; }

        /// <summary>
        ///     <para>fix per http://stackoverflow.com/a/18619181/956364</para>
        ///     <para>questionBox.Load += ( sender, e ) =&gt; ( sender as QuestionBox ).Visible = true;</para>
        /// </summary>
        /// <param name="question"></param>
        public QuestionBox( [NotNull] String question ) {
            if ( question is null ) {
                throw new ArgumentNullException( nameof( question ) );
            }

            this.InitializeComponent();
            this.Visible( false );
            this.richTextBoxQuestion.Text( question.Limit( this.richTextBoxQuestion.MaxLength ) );
        }

        private void buttonCancel_Click( [CanBeNull] Object sender, [CanBeNull] EventArgs e ) {
            this.Response = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonOkay_Click( [CanBeNull] Object sender, [CanBeNull] EventArgs e ) {
            this.Response = this.textBoxInput.Text()?.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Question_FormClosed( [CanBeNull] Object sender, [CanBeNull] FormClosedEventArgs e ) => this.Response = this.textBoxInput.Text.Trim();

        private void QuestionBox_Load( [CanBeNull] Object sender, [CanBeNull] EventArgs e ) {
            this.Visible( true ); //fix per http://stackoverflow.com/a/18619181/956364
            this.richTextBoxQuestion.Focus(); //BUG not working under unit test
        }

        private void textBoxInput_VisibleChanged( [CanBeNull] Object sender, [CanBeNull] EventArgs e ) {
            if ( this.Visible ) {
                this.richTextBoxQuestion.Focus(); //BUG not working under unit test
            }
        }

    }

}