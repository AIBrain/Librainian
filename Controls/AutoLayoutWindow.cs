// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "AutoLayoutWindow.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/AutoLayoutWindow.cs" was last cleaned by Protiguous on 2018/05/15 at 10:39 PM.

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
            catch ( Exception ) { return false; }
        }
    }
}