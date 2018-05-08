// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/AutoLayoutWindow.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

namespace Librainian.Controls {

    using System;
    using System.Collections.Concurrent;
    using System.Windows.Forms;

    public partial class AutoLayoutWindow : Form {
        private readonly ConcurrentBag<Label> _labels = new ConcurrentBag<Label>();
        private readonly ConcurrentQueue<String> _messages = new ConcurrentQueue<String>();

		public AutoLayoutWindow() => this.InitializeComponent();

		public Boolean Add( String message ) {
            try {
                this._messages.Enqueue( message );

                var label = new Label { Text = message };
                this._labels.Add( label );

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