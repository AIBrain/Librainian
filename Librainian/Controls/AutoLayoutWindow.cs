#if NET48 || NET50
namespace Librainian.Controls {

	using System;
	using System.Collections.Concurrent;
	using System.Windows.Forms;

	public partial class AutoLayoutWindow : Form {

		private AutoLayoutWindow() => this.InitializeComponent();

		private ConcurrentBag<Label> Labels { get; } = new ConcurrentBag<Label>();

		private ConcurrentQueue<String> Messages { get; } = new ConcurrentQueue<String>();
		public static AutoLayoutWindow CreateInstance() => new AutoLayoutWindow();

		public Boolean Add( String message ) {
			try {
				this.Messages.Enqueue( message );

				var label = new Label {
					Text = message
				};

				this.Labels.Add( label );

				this.Panel.Controls.Add( label );
				this.Panel.Update();

				return true;
			}
			catch ( Exception ) {
				return default;
			}
		}
	}

}
#endif