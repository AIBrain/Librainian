#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/Win32.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Controls {
    public static class Win32 {
        // ReSharper disable InconsistentNaming

        public const int MF_DISABLED = 0x00000002;
        public const int MF_ENABLED = 0x00000000;
        public const int MF_GRAYED = 0x1;
        public const int SC_CLOSE = 0xF060; //close button's code in Windows API

        //enabled button status

        //disabled button status (enabled = false)

        //disabled button status

        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_MINIMIZE = 0xF020; //for minimize button on forms

        //for maximize button on forms

        // ReSharper restore InconsistentNaming
    }
}
