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
// File "QuestionBox.cs" last formatted on 2020-08-18 at 12:18 AM.

#nullable enable



namespace Librainian.Controls {

	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	public partial class QuestionBox : Form {

		public String Question { get; set; }
		public String? Response { get; set; }

		public QuestionBox( String question ) {
			this.Question = question;
			this.InitializeComponent();
			this.AddControls();
		}


		private void AddControls() {
			var flow = new FlowLayoutPanel {
				Dock = DockStyle.Fill
			};

			this.Controls.Add( flow );

			AddCancelButton();

			var OkayButton = new Button {
				Text = "OK", Dock = DockStyle.Right
			};
			flow.Controls.Add( OkayButton );

			AddOkayLabel();

			AddUserInput();

			this.PerformLayout();

			void AddCancelButton() {
				var cancelButton = new Button {
					Text = "Cancel", Dock = DockStyle.Right
				};
				cancelButton.MouseClick += ( sender, args ) => {
					this.Response = null;
					this.DialogResult = DialogResult.Cancel;
					this.Close();
				};

				flow.Controls.Add( cancelButton );
			}

			void AddOkayLabel() {
				var question = new Label {
					Text = this.Question, Dock = DockStyle.Top
				};

				flow.Controls.Add( question );
			}

			void AddUserInput() {
				var input = new TextBox {
					Text = String.Empty, Dock = DockStyle.Bottom
				};

				OkayButton.MouseClick += ( sender, args ) => {
					this.Response = input.Text();
					this.Close();
				};

				OkayButton.KeyPress += ( sender, args ) => {
					this.Response = input.Text();
					this.DialogResult = DialogResult.OK;
					this.Close();
				};
				flow.Controls.Add( input );
				input.Fokus();
			}


		}

		public QuestionBox( IContainer container ) {
			container.Add( this );

			this.InitializeComponent();
		}

	}

}

