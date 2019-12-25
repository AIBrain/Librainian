// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AutoLayoutWindow.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "AutoLayoutWindow.cs" was last formatted by Protiguous on 2019/08/08 at 6:43 AM.

namespace Librainian.Controls {

    using System;
    using System.Collections.Concurrent;
    using System.Reflection.Emit;
    using System.Windows.Forms;

    public partial class AutoLayoutWindow : Form {

        private ConcurrentBag<Label> Labels { get; } = new ConcurrentBag<Label>();

        private ConcurrentQueue<String> Messages { get; } = new ConcurrentQueue<String>();

        public AutoLayoutWindow() => this.InitializeComponent();

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