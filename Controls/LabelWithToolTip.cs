// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "LabelWithToolTip.cs",
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
// "Librainian/Librainian/LabelWithToolTip.cs" was last cleaned by Protiguous on 2018/05/15 at 10:39 PM.

namespace Librainian.Controls {

    using System;
    using System.Windows.Forms;

    public class LabelWithToolTip : Label {

        public LabelWithToolTip() {
            this.Tt.AutoPopDelay = 1500;
            this.Tt.InitialDelay = 400;
            this.Tt.IsBalloon = true;
            this.Tt.UseAnimation = true;
            this.Tt.UseFading = true;
            this.Tt.Active = true;

            this.MouseEnter += ( sender1, ea ) => {
                if ( String.IsNullOrEmpty( this.TooltipText ) ) { return; }

                this.Tt.SetToolTip( this, this.TooltipText );
                this.Tt.Show( this.TooltipText, this.Parent );
            };

            this.MouseLeave += ( sender, args ) => this.Tt.Hide( this.Parent );
        }

        private ToolTip Tt { get; } = new ToolTip();

        public String TooltipText { get; set; }
    }
}