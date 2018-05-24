// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AutoLayoutWindow.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/AutoLayoutWindow.cs" was last formatted by Protiguous on 2018/05/22 at 6:35 PM.

namespace Librainian.Controls {

    using System;
    using System.Collections.Concurrent;
    using System.Windows.Forms;

    public partial class AutoLayoutWindow : Form {

        private ConcurrentBag<Label> Labels { get; } = new ConcurrentBag<Label>();

        private ConcurrentQueue<String> Messages { get; } = new ConcurrentQueue<String>();

        public AutoLayoutWindow() => this.InitializeComponent();

        public Boolean Add( String message ) {
            try {
                this.Messages.Enqueue( message );

                var label = new Label { Text = message };
                this.Labels.Add( label );

                this.Panel.Controls.Add( label );
                this.Panel.Update();

                return true;
            }
            catch ( Exception ) { return false; }
        }
    }
}