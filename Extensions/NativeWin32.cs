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
// "Librainian/NativeWin32.cs" was last cleaned by Rick on 2014/12/24 at 4:29 AM

namespace Librainian.Extensions {

    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using IO;
    

    public static class NativeWin32 {
        public const uint ERROR_MORE_DATA = 234;

        public const uint ERROR_SUCCESS = 0;

        public const int MaxPath = 260;

        public enum PLATFORM_ID {
            PLATFORM_ID_DOS = 300,
            PLATFORM_ID_OS2 = 400,
            PLATFORM_ID_NT = 500,
            PLATFORM_ID_OSF = 600,
            PLATFORM_ID_VMS = 700
        }

        public enum SV_101_TYPES : uint {
            SV_TYPE_WORKSTATION = 0x00000001,
            SV_TYPE_SERVER = 0x00000002,
            SV_TYPE_SQLSERVER = 0x00000004,
            SV_TYPE_DOMAIN_CTRL = 0x00000008,
            SV_TYPE_DOMAIN_BAKCTRL = 0x00000010,
            SV_TYPE_TIME_SOURCE = 0x00000020,
            SV_TYPE_AFP = 0x00000040,
            SV_TYPE_NOVELL = 0x00000080,
            SV_TYPE_DOMAIN_MEMBER = 0x00000100,
            SV_TYPE_PRINTQ_SERVER = 0x00000200,
            SV_TYPE_DIALIN_SERVER = 0x00000400,
            SV_TYPE_XENIX_SERVER = 0x00000800,
            SV_TYPE_SERVER_UNIX = 0x00000800,
            SV_TYPE_NT = 0x00001000,
            SV_TYPE_WFW = 0x00002000,
            SV_TYPE_SERVER_MFPN = 0x00004000,
            SV_TYPE_SERVER_NT = 0x00008000,
            SV_TYPE_POTENTIAL_BROWSER = 0x00010000,
            SV_TYPE_BACKUP_BROWSER = 0x00020000,
            SV_TYPE_MASTER_BROWSER = 0x00040000,
            SV_TYPE_DOMAIN_MASTER = 0x00080000,
            SV_TYPE_SERVER_OSF = 0x00100000,
            SV_TYPE_SERVER_VMS = 0x00200000,
            SV_TYPE_WINDOWS = 0x00400000,
            SV_TYPE_DFS = 0x00800000,
            SV_TYPE_CLUSTER_NT = 0x01000000,
            SV_TYPE_TERMINALSERVER = 0x02000000,
            SV_TYPE_CLUSTER_VS_NT = 0x04000000,
            SV_TYPE_DCE = 0x10000000,
            SV_TYPE_ALTERNATE_XPORT = 0x20000000,
            SV_TYPE_LOCAL_LIST_ONLY = 0x40000000,
            SV_TYPE_DOMAIN_ENUM = 0x80000000,
            SV_TYPE_ALL = 0xFFFFFFFF
        };

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("kernel32", SetLastError = true)]
        public static extern Boolean CloseHandle( IntPtr handle );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile( String lpFileName, JunctionPoint.EFileAccess dwDesiredAccess, EFileShare dwShareMode, IntPtr lpSecurityAttributes, JunctionPoint.ECreationDisposition dwCreationDisposition, JunctionPoint.EFileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile( String lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping( IntPtr hFile, IntPtr lpAttributes, int flProtect, int dwMaximumSizeLow, int dwMaximumSizeHigh, String lpName );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean DeviceIoControl( IntPtr hDevice, uint dwIoControlCode, IntPtr InBuffer, int nInBufferSize, IntPtr OutBuffer, int nOutBufferSize, out int pBytesReturned, IntPtr lpOverlapped );

        [DllImport("kernel32.dll")]
        public static extern int DeviceIoControl( IntPtr hDevice, int dwIoControlCode, ref short lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped );

        [DllImport("user32.dll")]
        public static extern int EnableMenuItem( this IntPtr tMenu, int targetItem, int targetStatus );

        /// <summary>
        /// Closes a file search handle opened by the FindFirstFile, FindFirstFileEx, or
        /// FindFirstStreamW function.
        /// </summary>
        /// <param name="hFindFile">The file search handle.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the return
        /// value is zero.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa364413%28VS.85%29.aspx"/>
        [DllImport("kernel32", SetLastError = true)]
        public static extern Boolean FindClose( IntPtr hFindFile );

        /// <summary>
        /// Searches a directory for a file or subdirectory with a name that matches a specific name
        /// (or partial name if wildcards are used).
        /// </summary>
        /// <param name="lpFileName">
        /// The directory or path, and the file name, which can include wildcard characters, for
        /// example, an asterisk (*) or a question mark (?).
        /// </param>
        /// <param name="lpFindData">
        /// A pointer to the WIN32_FIND_DATA structure that receives information about a found file
        /// or directory.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a search handle used in a subsequent call
        /// to FindNextFile or FindClose, and the lpFindFileData parameter contains information
        /// about the first file or directory found. If the function fails or fails to locate files
        /// from the search String in the lpFileName parameter, the return value is
        /// INVALID_HANDLE_VALUE and the contents of lpFindFileData are indeterminate.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa364418%28VS.85%29.aspx"/>
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        public static extern SafeSearchHandle FindFirstFile( String lpFileName, out Win32FindData lpFindData );

        /// <summary>
        /// Continues a file search from a previous call to the FindFirstFile or FindFirstFileEx function.
        /// </summary>
        /// <param name="hFindFile">
        /// The search handle returned by a previous call to the FindFirstFile or FindFirstFileEx function.
        /// </param>
        /// <param name="lpFindData">
        /// A pointer to the WIN32_FIND_DATA structure that receives information about the found
        /// file or subdirectory. The structure can be used in subsequent calls to FindNextFile to
        /// indicate from which file to continue the search.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero and the lpFindFileData parameter
        /// contains information about the next file or directory found. If the function fails, the
        /// return value is zero and the contents of lpFindFileData are indeterminate.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa364428%28VS.85%29.aspx"/>
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        public static extern Boolean FindNextFile( SafeSearchHandle hFindFile, out Win32FindData lpFindData );

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx( IntPtr hwndParent, IntPtr hwndChildAfter, String lpszClass, String lpszWindow );

        [DllImport("kernel32", SetLastError = true)]
        public static extern Boolean FlushViewOfFile( IntPtr lpBaseAddress, IntPtr dwNumBytesToFlush );

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool FreeConsole();

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

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThread();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="http://www.facepunch.com/showthread.php?t=1312991"/>
        public static IntPtr GetDesktopHandle() {
            var desktop = GetDesktopWindow();
            var progMan = FindWindowEx( desktop, IntPtr.Zero, "Progman", "Program Manager" );
            var defView = FindWindowEx( progMan, IntPtr.Zero, "SHELLDLL_DefView", String.Empty );

            //var listView = FindWindowEx( defView, IntPtr.Zero, "SysListView32", "FolderView" );

            return defView;
        }

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        public static extern uint GetDiskFreeSpaceW( [In] [MarshalAs(UnmanagedType.LPWStr)] String lpRootPathName, out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters, out uint lpTotalNumberOfClusters );

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu( IntPtr hwndValue, Boolean isRevert );

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr MapViewOfFile( IntPtr hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, IntPtr dwNumBytesToMap );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool MoveFileWithProgress( String lpExistingFileName, String lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, MoveFileFlags dwFlags );

        /// <summary>
        /// Netapi32.dll : The NetApiBufferFree function frees the memory that the
        /// NetApiBufferAllocate function allocates. Call NetApiBufferFree to free the memory that
        /// other network management functions return.
        /// </summary>
        [DllImport("netapi32.dll", EntryPoint = "NetApiBufferFree")]
        public static extern int NetApiBufferFree( IntPtr buffer );

        /// <summary>
        /// The NetServerEnum function lists all servers of the specified type that are visible in a domain.
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="level"></param>
        /// <param name="bufptr"></param>
        /// <param name="prefmaxlen"></param>
        /// <param name="entriesread"></param>
        /// <param name="totalentries"></param>
        /// <param name="servertype"></param>
        /// <param name="domain"></param>
        /// <param name="resume_handle"></param>
        /// <returns></returns>
        /// <seealso cref="http://www.pinvoke.net/default.aspx/netapi32.netserverenum"/>
        [DllImport("netapi32.dll", EntryPoint = "NetServerEnum")]
        public static extern int NetServerEnum( [MarshalAs(UnmanagedType.LPWStr)] String servername, int level, out IntPtr bufptr, int prefmaxlen, ref int entriesread, ref int totalentries, SV_101_TYPES servertype, [MarshalAs(UnmanagedType.LPWStr)] String domain, IntPtr resume_handle );

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping( int dwDesiredAccess, Boolean bInheritHandle, String lpName );

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern bool PathCompactPathEx( [Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags );

        /// <summary>
        /// </summary>
        /// <param name="hThread"></param>
        /// <param name="dwThreadAffinityMask"></param>
        /// <returns></returns>
        /// <example>
        /// SetThreadAffinityMask( GetCurrentThread(), new IntPtr( 1 &lt;&lt; processor ) );
        /// </example>
        [DllImport("kernel32.dll")]
        public static extern IntPtr SetThreadAffinityMask( IntPtr hThread, IntPtr dwThreadAffinityMask );

        [DllImport("kernel32", SetLastError = true)]
        public static extern Boolean UnmapViewOfFile( IntPtr lpBaseAddress );

        /// <summary>
        /// Win32 FILETIME structure. The win32 documentation says this: "Contains a 64-bit value
        /// representing the number of 100-nanosecond intervals since January 1, 1601 (UTC)."
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms724284%28VS.85%29.aspx"/>
        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME {
            public uint dwLowDateTime;

            public uint dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVER_INFO_101 {

            [MarshalAs(UnmanagedType.U4)]
            public readonly UInt32 sv101_platform_id;

            [MarshalAs(UnmanagedType.LPWStr)]
            public readonly String sv101_name;

            [MarshalAs(UnmanagedType.U4)]
            public readonly UInt32 sv101_version_major;

            [MarshalAs(UnmanagedType.U4)]
            public readonly UInt32 sv101_version_minor;

            [MarshalAs(UnmanagedType.U4)]
            public readonly UInt32 sv101_type;

            [MarshalAs(UnmanagedType.LPWStr)]
            public readonly String sv101_comment;
        };

        /// <summary>
        /// The Win32 find data structure. The documentation says: "Contains information about the
        /// file that is found by the FindFirstFile, FindFirstFileEx, or FindNextFile function."
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa365740%28VS.85%29.aspx"/>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct Win32FindData {
            public FileAttributes dwFileAttributes;

            public FILETIME ftCreationTime;

            public FILETIME ftLastAccessTime;

            public FILETIME ftLastWriteTime;

            public uint nFileSizeHigh;

            public uint nFileSizeLow;

            public uint dwReserved0;

            public uint dwReserved1;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
            public String cFileName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public String cAlternateFileName;
        }
    }
}