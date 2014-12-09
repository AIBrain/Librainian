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
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/WindowsAPI.cs" was last cleaned by Rick on 2014/12/09 at 5:56 AM

namespace Librainian.IO {

    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal delegate CopyProgressResult CopyProgressRoutine( long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, uint dwStreamNumber, CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData );

    /// <summary>
    /// Win32 APIs used by the library
    /// </summary>
    /// <remarks>Defines the PInvoke functions we use to access the FileMapping Win32 APIs</remarks>
    public class WindowsAPI {

        [DllImport("kernel32", SetLastError = true)]
        public static extern Boolean CloseHandle( IntPtr handle );

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile( String lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping( IntPtr hFile, IntPtr lpAttributes, int flProtect, int dwMaximumSizeLow, int dwMaximumSizeHigh, String lpName );

        [DllImport("kernel32", SetLastError = true)]
        public static extern Boolean FlushViewOfFile( IntPtr lpBaseAddress, IntPtr dwNumBytesToFlush );

        /// <summary>
        /// <para>
        /// Retrieves the actual number of bytes of disk storage used to store a specified file as a
        /// transacted operation.
        /// </para>
        /// <para>
        /// If the file is located on a volume that supports compression and the file is compressed,
        /// the value obtained is the compressed size of the specified file.
        /// </para>
        /// <para>
        /// If the file is located on a volume that supports sparse files and the file is a sparse
        /// file, the value obtained is the sparse size of the specified file.
        /// </para>
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="lpFileSizeHigh"></param>
        /// <returns></returns>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa364930(v=vs.85).aspx"/>
        [DllImport("kernel32.dll")]
        public static extern uint GetCompressedFileSizeW( [In] [MarshalAs(UnmanagedType.LPWStr)] String lpFileName, [Out] [MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh );

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        public static extern uint GetDiskFreeSpaceW( [In] [MarshalAs(UnmanagedType.LPWStr)] String lpRootPathName, out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters, out uint lpTotalNumberOfClusters );

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr MapViewOfFile( IntPtr hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, IntPtr dwNumBytesToMap );

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping( int dwDesiredAccess, Boolean bInheritHandle, String lpName );

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern bool PathCompactPathEx( [Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags );

        [DllImport("kernel32", SetLastError = true)]
        public static extern Boolean UnmapViewOfFile( IntPtr lpBaseAddress );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool MoveFileWithProgress( String lpExistingFileName, String lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, MoveFileFlags dwFlags );
    }
}