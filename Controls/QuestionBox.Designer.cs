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
            this.buttonOkay.Location = new System.Drawing.Point(328, 152);
            this.buttonOkay.Name = "buttonOkay";
            this.buttonOkay.Size = new System.Drawing.Size(75, 23);
            this.buttonOkay.TabIndex = 0;
            this.buttonOkay.Text = "Okay";
            this.buttonOkay.UseVisualStyleBackColor = true;
            this.buttonOkay.Click += new System.EventHandler(this.buttonOkay_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(409, 152);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
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
            this.richTextBoxQuestion.Name = "richTextBoxQuestion";
            this.richTextBoxQuestion.ReadOnly = true;
            this.richTextBoxQuestion.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxQuestion.Size = new System.Drawing.Size(496, 187);
            this.richTextBoxQuestion.TabIndex = 3;
            this.richTextBoxQuestion.Text = "";
            // 
            // textBoxInput
            // 
            this.textBoxInput.Location = new System.Drawing.Point(12, 152);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(310, 20);
            this.textBoxInput.TabIndex = 4;
            this.textBoxInput.VisibleChanged += new System.EventHandler(this.textBoxInput_VisibleChanged);
            // 
            // QuestionBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(496, 187);
            this.Controls.Add(this.textBoxInput);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOkay);
            this.Controls.Add(this.richTextBoxQuestion);
            this.DoubleBuffered = true;
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