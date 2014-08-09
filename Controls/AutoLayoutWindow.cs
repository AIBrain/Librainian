#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/AutoLayoutWindow.cs" was last cleaned by Rick on 2014/08/08 at 2:25 PM

#endregion License & Information

namespace Librainian.Controls {

    using System;
    using System.Collections.Concurrent;
    using System.Windows.Forms;

    public partial class AutoLayoutWindow : Form {
        private readonly ConcurrentBag<Label> Labels = new ConcurrentBag<Label>();

        private readonly ConcurrentQueue<String> Messages = new ConcurrentQueue<String>();

        public AutoLayoutWindow() {
            this.InitializeComponent();
        }

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
                return false;
            }
        }
    }
}