// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// 
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// 
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Controls {
	using System.Windows.Forms;

	partial class QuestionBox {

		private FlowLayoutPanel flow;
		private TextBox textBoxQuestion;
		private TextBox textBoxUserInput;
		private Panel panel1;
		private Button buttonOkay;
		private Button buttonCancel;

		private void InitializeComponent() {
			this.flow = new System.Windows.Forms.FlowLayoutPanel();
			this.textBoxQuestion = new System.Windows.Forms.TextBox();
			this.textBoxUserInput = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonOkay = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.flow.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// flow
			// 
			this.flow.Controls.Add(this.textBoxQuestion);
			this.flow.Controls.Add(this.textBoxUserInput);
			this.flow.Controls.Add(this.panel1);
			this.flow.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flow.Location = new System.Drawing.Point(0, 0);
			this.flow.Name = "flow";
			this.flow.Size = new System.Drawing.Size(1025, 342);
			this.flow.TabIndex = 0;
			// 
			// textBoxQuestion
			// 
			this.textBoxQuestion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxQuestion.Dock = System.Windows.Forms.DockStyle.Top;
			this.textBoxQuestion.Location = new System.Drawing.Point(3, 3);
			this.textBoxQuestion.Multiline = true;
			this.textBoxQuestion.Name = "textBoxQuestion";
			this.textBoxQuestion.PlaceholderText = "Ask Question Here";
			this.textBoxQuestion.ReadOnly = true;
			this.textBoxQuestion.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxQuestion.Size = new System.Drawing.Size(1010, 213);
			this.textBoxQuestion.TabIndex = 0;
			// 
			// textBoxUserInput
			// 
			this.textBoxUserInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flow.SetFlowBreak(this.textBoxUserInput, true);
			this.textBoxUserInput.Location = new System.Drawing.Point(3, 222);
			this.textBoxUserInput.Name = "textBoxUserInput";
			this.textBoxUserInput.PlaceholderText = "Answer here..";
			this.textBoxUserInput.Size = new System.Drawing.Size(1010, 35);
			this.textBoxUserInput.TabIndex = 3;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonOkay);
			this.panel1.Controls.Add(this.buttonCancel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.flow.SetFlowBreak(this.panel1, true);
			this.panel1.Location = new System.Drawing.Point(3, 263);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1010, 70);
			this.panel1.TabIndex = 10;
			// 
			// buttonOkay
			// 
			this.buttonOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOkay.Dock = System.Windows.Forms.DockStyle.Right;
			this.buttonOkay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonOkay.Location = new System.Drawing.Point(748, 0);
			this.buttonOkay.Name = "buttonOkay";
			this.buttonOkay.Size = new System.Drawing.Size(131, 70);
			this.buttonOkay.TabIndex = 10;
			this.buttonOkay.Text = "Okay";
			this.buttonOkay.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Dock = System.Windows.Forms.DockStyle.Right;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonCancel.Location = new System.Drawing.Point(879, 0);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(131, 70);
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// QuestionBox
			// 
			this.ClientSize = new System.Drawing.Size(1025, 342);
			this.Controls.Add(this.flow);
			this.Name = "QuestionBox";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QuestionBox_FormClosing);
			this.flow.ResumeLayout(false);
			this.flow.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

	}
}
