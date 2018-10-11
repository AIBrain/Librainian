namespace Librainian.Controls {
    partial class QuestionBox {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if ( disposing && ( this.components != null ) ) {
                this.components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonOkay = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.richTextBoxQuestion = new System.Windows.Forms.RichTextBox();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonOkay
            // 
            this.buttonOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOkay.Location = new System.Drawing.Point(499, 239);
            this.buttonOkay.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonOkay.Name = "buttonOkay";
            this.buttonOkay.Size = new System.Drawing.Size(112, 35);
            this.buttonOkay.TabIndex = 0;
            this.buttonOkay.Text = "Okay";
            this.buttonOkay.UseVisualStyleBackColor = true;
            this.buttonOkay.Click += new System.EventHandler(this.buttonOkay_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(619, 239);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 35);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // richTextBoxQuestion
            // 
            this.richTextBoxQuestion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxQuestion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxQuestion.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxQuestion.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBoxQuestion.Name = "richTextBoxQuestion";
            this.richTextBoxQuestion.ReadOnly = true;
            this.richTextBoxQuestion.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxQuestion.Size = new System.Drawing.Size(744, 288);
            this.richTextBoxQuestion.TabIndex = 3;
            this.richTextBoxQuestion.Text = "";
            // 
            // textBoxInput
            // 
            this.textBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInput.Location = new System.Drawing.Point(13, 243);
            this.textBoxInput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(463, 26);
            this.textBoxInput.TabIndex = 4;
            this.textBoxInput.VisibleChanged += new System.EventHandler(this.textBoxInput_VisibleChanged);
            // 
            // QuestionBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(744, 288);
            this.Controls.Add(this.textBoxInput);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOkay);
            this.Controls.Add(this.richTextBoxQuestion);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "QuestionBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Question";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Question_FormClosed);
            this.Load += new System.EventHandler(this.QuestionBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOkay;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.RichTextBox richTextBoxQuestion;
        private System.Windows.Forms.TextBox textBoxInput;
    }
}