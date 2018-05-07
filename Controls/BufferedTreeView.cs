// Copyright 2016 Protiguous.
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
// "Librainian/BufferedTreeView.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

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