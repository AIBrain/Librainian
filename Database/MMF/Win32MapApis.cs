#region License & Information

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
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/Win32MapApis.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM

#endregion License & Information

namespace Librainian.Database.MMF {

    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Win32 APIs used by the library
    /// </summary>
    /// <remarks>Defines the PInvoke functions we use to access the FileMapping Win32 APIs</remarks>
    internal class Win32MapApis {

        [DllImport( "kernel32", SetLastError = true )]
        public static extern Boolean CloseHandle( IntPtr handle );

        [DllImport( "kernel32", SetLastError = true )]
        public static extern Boolean UnmapViewOfFile( IntPtr lpBaseAddress );

        [DllImport( "kernel32", SetLastError = true )]
        public static extern Boolean FlushViewOfFile( IntPtr lpBaseAddress, IntPtr dwNumBytesToFlush );

        [DllImport( "kernel32", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern IntPtr OpenFileMapping( int dwDesiredAccess, Boolean bInheritHandle, String lpName );

        [DllImport( "kernel32", SetLastError = true )]
        public static extern IntPtr MapViewOfFile( IntPtr hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, IntPtr dwNumBytesToMap );

        [DllImport( "kernel32", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern IntPtr CreateFileMapping( IntPtr hFile, IntPtr lpAttributes, int flProtect, int dwMaximumSizeLow, int dwMaximumSizeHigh, String lpName );

        [DllImport( "kernel32", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern IntPtr CreateFile( String lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile );
    }
}