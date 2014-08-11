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
// "Librainian/Heap.cs" was last cleaned by Rick on 2014/08/11 at 12:41 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Runtime.InteropServices;

    public class Heap {
        [Flags]
        public enum HeapFlags {
            // ReSharper disable InconsistentNaming
            HEAP_NO_SERIALIZE = 0x1,

            HEAP_GENERATE_EXCEPTIONS = 0x4,
            HEAP_ZERO_MEMORY = 0x8
            // ReSharper restore InconsistentNaming
        }

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern IntPtr GetProcessHeap();

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern IntPtr HeapAlloc( IntPtr hHeap, HeapFlags dwFlags, uint dwSize );

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern IntPtr HeapCreate( HeapFlags flOptions, uint dwInitialsize, uint dwMaximumSize );

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern Boolean HeapDestroy( IntPtr hHeap );

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern Boolean HeapFree( IntPtr hHeap, HeapFlags dwFlags, IntPtr lpMem );
    }
}
