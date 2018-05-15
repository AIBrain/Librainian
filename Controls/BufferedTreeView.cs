// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "BufferedTreeView.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license
// has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/BufferedTreeView.cs" was last cleaned by Protiguous on 2018/05/15 at 1:34 AM.

namespace Librainian.Controls {

    using System;
    using System.Windows.Forms;
    using OperatingSystem;

    public class BufferedTreeView : TreeView {

        private const Int32 TVM_GETEXTENDEDSTYLE = 0x1100 + 45;

        // Pinvoke:
        private const Int32 TVM_SETEXTENDEDSTYLE = 0x1100 + 44;

        private const Int32 TVS_EX_DOUBLEBUFFER = 0x0004;

        protected override void OnHandleCreated( EventArgs e ) {
            NativeMethods.SendMessage( this.Handle, TVM_SETEXTENDEDSTYLE, ( IntPtr )TVS_EX_DOUBLEBUFFER, ( IntPtr )TVS_EX_DOUBLEBUFFER );
            base.OnHandleCreated( e );
        }
    }
}