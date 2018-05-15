// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "QuestionBox.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license has
// been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/QuestionBox.cs" was last cleaned by Protiguous on 2018/05/15 at 1:34 AM.

namespace Librainian.Controls {

    using System;
    using System.Windows.Forms;
    using JetBrains.Annotations;

    /// <summary>
    /// Better than a messagebox?
    /// </summary>
    public partial class QuestionBox : Form {

        /// <summary>
        /// <para>fix per http://stackoverflow.com/a/18619181/956364</para>
        /// <para>questionBox.Load += ( sender, e ) =&gt; ( sender as QuestionBox ).Visible = true;</para>
        /// </summary>
        /// <param name="question"></param>
        /// <param name="parent">  </param>
        public QuestionBox( [NotNull] String question, Control parent = null ) {
            if ( question is null ) { throw new ArgumentNullException( nameof( question ) ); }

            if ( parent != null ) { this.Parent = parent; }

            this.InitializeComponent();
            this.Visible( false );
            this.richTextBoxQuestion.Text( question.Trim() );
        }

        [CanBeNull]
        public String Response { get; private set; }

        private void buttonCancel_Click( Object sender, EventArgs e ) {
            this.Response = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonOkay_Click( Object sender, EventArgs e ) {
            this.Response = this.textBoxInput.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Question_FormClosed( Object sender, FormClosedEventArgs e ) => this.Response = this.textBoxInput.Text.Trim();

        private void QuestionBox_Load( Object sender, EventArgs e ) {
            this.Visible( true ); //fix per http://stackoverflow.com/a/18619181/956364
            this.richTextBoxQuestion.Focus(); //BUG not working under unit test
        }

        private void textBoxInput_VisibleChanged( Object sender, EventArgs e ) {
            if ( this.Visible ) {
                this.richTextBoxQuestion.Focus(); //BUG not working under unit test
            }
        }
    }
}