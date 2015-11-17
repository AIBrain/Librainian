// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/VerticalProgressBar.cs" was last cleaned by Rick on 2015/06/12 at 2:52 PM

namespace Librainian.Controls {

    using System.Windows.Forms;

    public partial class VerticalProgressBar : ProgressBar {

        protected override CreateParams CreateParams {
            get {
                var cp = base.CreateParams;
                cp.Style |= 0x04; //PBS_VERTICAL
                return cp;
            }
        }
    }
}