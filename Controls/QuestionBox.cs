// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/QuestionBox.cs" was last cleaned by Protiguous on 2018/05/12 at 1:21 AM

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
            if ( question is null ) {
                throw new ArgumentNullException( nameof( question ) );
            }

            if ( parent != null ) {
                Parent = parent;
            }

            this.InitializeComponent();
            this.Visible( false );
            this.richTextBoxQuestion.Text( question.Trim() );
        }

        [CanBeNull]
        public String Response { get; private set; }

        private void buttonCancel_Click( Object sender, EventArgs e ) {
            this.Response = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOkay_Click( Object sender, EventArgs e ) {
            this.Response = this.textBoxInput.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Question_FormClosed( Object sender, FormClosedEventArgs e ) => this.Response = this.textBoxInput.Text.Trim();

        private void QuestionBox_Load( Object sender, EventArgs e ) {
            this.Visible( true ); //fix per http://stackoverflow.com/a/18619181/956364
            this.richTextBoxQuestion.Focus(); //BUG not working under unit test
        }

        private void textBoxInput_VisibleChanged( Object sender, EventArgs e ) {
            if ( Visible ) {
                this.richTextBoxQuestion.Focus(); //BUG not working under unit test
            }
        }
    }
}