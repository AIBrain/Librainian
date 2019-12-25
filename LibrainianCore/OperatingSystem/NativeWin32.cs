// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: Protiguous@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/NativeWin32.cs" was last cleaned by Rick on 2016/06/23 at 10:11 PM

namespace LibrainianCore.OperatingSystem {

    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using FileSystem;
    using Microsoft.Win32.SafeHandles;

    public static class NativeWin32 {
        public const UInt32 ATA_FLAGS_DATA_IN = 0x02;

        public const UInt32 ErrorMoreData = 234;

        public const UInt32 ErrorSuccess = 0;

        public const UInt32 FILE_ANY_ACCESS = 0;

        public const UInt32 FILE_ATTRIBUTE_NORMAL = 0x00000080;

        public const UInt32 FILE_DEVICE_CONTROLLER = 0x00000004;

        public const UInt32 FILE_DEVICE_MASS_STORAGE = 0x0000002d;

        public const UInt32 FILE_READ_ACCESS = 0x00000001;

        public const UInt32 FILE_SHARE_READ = 0x00000001;

        public const UInt32 FILE_SHARE_WRITE = 0x00000002;

        public const UInt32 FILE_WRITE_ACCESS = 0x00000002;

        public const UInt32 FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

        public const UInt32 GENERIC_READ = 0x80000000;

        public const UInt32 GENERIC_WRITE = 0x40000000;

        public const UInt32 IOCTL_SCSI_BASE = FILE_DEVICE_CONTROLLER;

        public const UInt32 IOCTL_STORAGE_BASE = FILE_DEVICE_MASS_STORAGE;

        public const Int32 MaxPath = 260;

        public const UInt32 METHOD_BUFFERED = 0;

        public const UInt32 OPEN_EXISTING = 3;

        public const UInt32 PropertyStandardQuery = 0;

        public const UInt32 StorageDeviceSeekPenaltyProperty = 7;

        public enum FILE_INFO_BY_HANDLE_CLASS {
            FileBasicInfo = 0,

            FileStandardInfo = 1,

            FileNameInfo = 2,

            FileRenameInfo = 3,

            FileDispositionInfo = 4,

            FileAllocationInfo = 5,

            FileEndOfFileInfo = 6,

            FileStreamInfo = 7,

            FileCompressionInfo = 8,

            FileAttributeTagInfo = 9,

            FileIdBothDirectoryInfo = 10, // 0x0A

            FileIdBothDirectoryRestartInfo = 11, // 0xB

            FileIoPriorityHintInfo = 12, // 0xC

            FileRemoteProtocolInfo = 13, // 0xD

            FileFullDirectoryInfo = 14, // 0xE

            FileFullDirectoryRestartInfo = 15, // 0xF

            FileStorageInfo = 16, // 0x10

            FileAlignmentInfo = 17, // 0x11

            FileIdInfo = 18, // 0x12

            FileIdExtdDirectoryInfo = 19, // 0x13

            FileIdExtdDirectoryRestartInfo = 20, // 0x14

            MaximumFileInfoByHandlesClass
        }

        public enum PLATFORM_ID {
            PlatformIDDos = 300,

            PlatformIDOs2 = 400,

            PlatformIDNt = 500,

            PlatformIDOsf = 600,

            PlatformIDVms = 700
        }

        public enum Sv101Types : uint {
            SvTypeWorkstation = 0x00000001,

            SvTypeServer = 0x00000002,

            SvTypeSqlserver = 0x00000004,

            SvTypeDomainCtrl = 0x00000008,

            SvTypeDomainBakctrl = 0x00000010,

            SvTypeTimeSource = 0x00000020,

            SvTypeAfp = 0x00000040,

            SvTypeNovell = 0x00000080,

            SvTypeDomainMember = 0x00000100,

            SvTypePrintqServer = 0x00000200,

            SvTypeDialinServer = 0x00000400,

            SvTypeXenixServer = 0x00000800,

            SvTypeServerUnix = 0x00000800,

            SvTypeNt = 0x00001000,

            SvTypeWfw = 0x00002000,

            SvTypeServerMfpn = 0x00004000,

            SvTypeServerNt = 0x00008000,

            SvTypePotentialBrowser = 0x00010000,

            SvTypeBackupBrowser = 0x00020000,

            SvTypeMasterBrowser = 0x00040000,

            SvTypeDomainMaster = 0x00080000,

            SvTypeServerOsf = 0x00100000,

            SvTypeServerVms = 0x00200000,

            SvTypeWindows = 0x00400000,

            SvTypeDfs = 0x00800000,

            SvTypeClusterNt = 0x01000000,

            SvTypeTerminalserver = 0x02000000,

            SvTypeClusterVsNt = 0x04000000,

            SvTypeDce = 0x10000000,

            SvTypeAlternateXport = 0x20000000,

            SvTypeLocalListOnly = 0x40000000,

            SvTypeDomainEnum = 0x80000000,

            SvTypeAll = 0xFFFFFFFF
        }

        [DllImport( "kernel32.dll" )]
        public static extern Boolean AllocConsole();

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern Boolean CloseHandle( IntPtr handle );

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern IntPtr CreateFile( String lpFileName, JunctionPoint.EFileAccess dwDesiredAccess, JunctionPoint.EFileShare dwShareMode, IntPtr lpSecurityAttributes, JunctionPoint.ECreationDisposition dwCreationDisposition, JunctionPoint.EFileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern IntPtr CreateFile( String lpFileName, Int32 dwDesiredAccess, Int32 dwShareMode, IntPtr lpSecurityAttributes, Int32 dwCreationDisposition, Int32 dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern IntPtr CreateFile( String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern IntPtr CreateFileMapping( IntPtr hFile, IntPtr lpAttributes, Int32 flProtect, Int32 dwMaximumSizeLow, Int32 dwMaximumSizeHigh, String lpName );

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern SafeFileHandle CreateFileW( [MarshalAs( UnmanagedType.LPWStr )] String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile );

        public static UInt32 CTL_CODE( UInt32 deviceType, UInt32 function, UInt32 method, UInt32 access ) {
            return deviceType << 16 | access << 14 | function << 2 | method;
        }

        [DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern Boolean DeviceIoControl( IntPtr hDevice, UInt32 dwIoControlCode, IntPtr inBuffer, Int32 nInBufferSize, IntPtr outBuffer, Int32 nOutBufferSize, out Int32 pBytesReturned, IntPtr lpOverlapped );

        [DllImport( "kernel32.dll" )]
        public static extern Int32 DeviceIoControl( IntPtr hDevice, Int32 dwIoControlCode, ref Int16 lpInBuffer, Int32 nInBufferSize, IntPtr lpOutBuffer, Int32 nOutBufferSize, ref Int32 lpBytesReturned, IntPtr lpOverlapped );

        [DllImport( "kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto )]
        public static extern Boolean DeviceIoControl( IntPtr hDevice, UInt32 dwIoControlCode, IntPtr lpInBuffer, UInt32 nInBufferSize, IntPtr lpOutBuffer, UInt32 nOutBufferSize, out UInt32 lpBytesReturned, IntPtr lpOverlapped );

        [DllImport( "Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern Boolean DeviceIoControl( IntPtr hDevice, UInt32 dwIoControlCode, ref Int64 inBuffer, Int32 inBufferSize, ref Int64 outBuffer, Int32 outBufferSize, ref Int32 bytesReturned, [In] ref NativeOverlapped overlapped );

        [DllImport( "kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern Boolean DeviceIoControl( SafeFileHandle hDevice, UInt32 dwIoControlCode, ref STORAGE_PROPERTY_QUERY lpInBuffer, UInt32 nInBufferSize, ref DEVICE_SEEK_PENALTY_DESCRIPTOR lpOutBuffer, UInt32 nOutBufferSize, out UInt32 lpBytesReturned, IntPtr lpOverlapped );

        [DllImport( "kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern Boolean DeviceIoControl( SafeFileHandle hDevice, UInt32 dwIoControlCode, ref ATAIdentifyDeviceQuery lpInBuffer, UInt32 nInBufferSize, ref ATAIdentifyDeviceQuery lpOutBuffer, UInt32 nOutBufferSize, out UInt32 lpBytesReturned, IntPtr lpOverlapped );

        [DllImport( "user32.dll" )]
        public static extern Int32 EnableMenuItem( this IntPtr tMenu, Int32 targetItem, Int32 targetStatus );

        /// <summary>
        ///     Closes a file search handle opened by the FindFirstFile, FindFirstFileEx, or
        ///     FindFirstStreamW function.
        /// </summary>
        /// <param name="hFindFile">The file search handle.</param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero. If the function fails, the return
        ///     value is zero.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/aa364413%28VS.85%29.aspx" />
        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern Boolean FindClose( IntPtr hFindFile );

        /// <summary>
        ///     Searches a directory for a file or subdirectory with a name that matches a specific name
        ///     (or partial name if wildcards are used).
        /// </summary>
        /// <param name="lpFileName">
        ///     The directory or path, and the file name, which can include wildcard characters, for
        ///     example, an asterisk (*) or a question mark (?).
        /// </param>
        /// <param name="lpFindData">
        ///     A pointer to the WIN32_FIND_DATA structure that receives information about a found file
        ///     or directory.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is a search handle used in a subsequent call
        ///     to FindNextFile or FindClose, and the lpFindFileData parameter contains information
        ///     about the first file or directory found. If the function fails or fails to locate files
        ///     from the search String in the lpFileName parameter, the return value is
        ///     INVALID_HANDLE_VALUE and the contents of lpFindFileData are indeterminate.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/aa364418%28VS.85%29.aspx" />
        [DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false )]
        public static extern NativeMethods.SafeSearchHandle FindFirstFile( String lpFileName, out Win32FindData lpFindData );

        /// <summary>
        ///     Continues a file search from a previous call to the FindFirstFile or FindFirstFileEx function.
        /// </summary>
        /// <param name="hFindFile">
        ///     The search handle returned by a previous call to the FindFirstFile or FindFirstFileEx function.
        /// </param>
        /// <param name="lpFindData">
        ///     A pointer to the WIN32_FIND_DATA structure that receives information about the found
        ///     file or subdirectory. The structure can be used in subsequent calls to FindNextFile to
        ///     indicate from which file to continue the search.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero and the lpFindFileData parameter
        ///     contains information about the next file or directory found. If the function fails, the
        ///     return value is zero and the contents of lpFindFileData are indeterminate.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/aa364428%28VS.85%29.aspx" />
        [DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false )]
        public static extern Boolean FindNextFile( NativeMethods.SafeSearchHandle hFindFile, out Win32FindData lpFindData );

        [DllImport( "user32.dll" )]
        public static extern IntPtr FindWindowEx( IntPtr hwndParent, IntPtr hwndChildAfter, String lpszClass, String lpszWindow );

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern Boolean FlushViewOfFile( IntPtr lpBaseAddress, IntPtr dwNumBytesToFlush );

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern UInt32 FormatMessage( UInt32 dwFlags, IntPtr lpSource, UInt32 dwMessageId, UInt32 dwLanguageId, StringBuilder lpBuffer, UInt32 nSize, IntPtr arguments );

        [DllImport( "kernel32.dll", SetLastError = true, ExactSpelling = true )]
        public static extern Boolean FreeConsole();

        /// <summary>
        ///     <para>
        ///         Retrieves the actual number of bytes of disk storage used to store a specified file as a
        ///         transacted operation.
        ///     </para>
        ///     <para>
        ///         If the file is located on a volume that supports compression and the file is compressed,
        ///         the value obtained is the compressed size of the specified file.
        ///     </para>
        ///     <para>
        ///         If the file is located on a volume that supports sparse files and the file is a sparse
        ///         file, the value obtained is the sparse size of the specified file.
        ///     </para>
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="lpFileSizeHigh"></param>
        /// <returns></returns>
        /// <seealso cref="http://msdn.microsoft.com/en-us/Library/windows/desktop/aa364930(v=vs.85).aspx" />
        [DllImport( "kernel32.dll" )]
        public static extern UInt32 GetCompressedFileSizeW( [In] [MarshalAs( UnmanagedType.LPWStr )] String lpFileName, [Out] [MarshalAs( UnmanagedType.U4 )] out UInt32 lpFileSizeHigh );

        [DllImport( "kernel32.dll" )]
        public static extern IntPtr GetCurrentThread();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="http://www.facepunch.com/showthread.php?t=1312991" />
        public static IntPtr GetDesktopHandle() {
            var desktop = GetDesktopWindow();
            var progMan = FindWindowEx( desktop, IntPtr.Zero, "Progman", "Program Manager" );
            var defView = FindWindowEx( progMan, IntPtr.Zero, "SHELLDLL_DefView", String.Empty );

            //var listView = FindWindowEx( defView, IntPtr.Zero, "SysListView32", "FolderView" );

            return defView;
        }

        [DllImport( "user32.dll", EntryPoint = "GetDesktopWindow" )]
        public static extern IntPtr GetDesktopWindow();

        [DllImport( "kernel32.dll", SetLastError = true, PreserveSig = true )]
        public static extern UInt32 GetDiskFreeSpaceW( [In] [MarshalAs( UnmanagedType.LPWStr )] String lpRootPathName, out UInt32 lpSectorsPerCluster, out UInt32 lpBytesPerSector, out UInt32 lpNumberOfFreeClusters, out UInt32 lpTotalNumberOfClusters );

        public static String GetErrorMessage( Int32 code ) {
            var message = new StringBuilder( 255 );

            FormatMessage( FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, ( UInt32 )code, 0, message, ( UInt32 )message.Capacity, IntPtr.Zero );

            return message.ToString();
        }

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern Boolean GetFileInformationByHandleEx( IntPtr hFile, FILE_INFO_BY_HANDLE_CLASS infoClass, out FILE_ID_BOTH_DIR_INFO dirInfo, UInt32 dwBufferSize );

        [DllImport( "user32.dll" )]
        public static extern IntPtr GetSystemMenu( IntPtr hwndValue, Boolean isRevert );

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern IntPtr MapViewOfFile( IntPtr hFileMappingObject, Int32 dwDesiredAccess, Int32 dwFileOffsetHigh, Int32 dwFileOffsetLow, IntPtr dwNumBytesToMap );

        [DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern Boolean MoveFileWithProgress( String lpExistingFileName, String lpNewFileName, NativeMethods.CopyProgressRoutine lpProgressRoutine, IntPtr lpData, MoveFileFlags dwFlags );

        /// <summary>
        ///     Netapi32.dll : The NetApiBufferFree function frees the memory that the
        ///     NetApiBufferAllocate function allocates. Call NetApiBufferFree to free the memory that
        ///     other network management functions return.
        /// </summary>
        [DllImport( "netapi32.dll", EntryPoint = "NetApiBufferFree" )]
        public static extern Int32 NetApiBufferFree( IntPtr buffer );

        /// <summary>
        ///     The NetServerEnum function lists all servers of the specified type that are visible in a domain.
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="level"></param>
        /// <param name="bufptr"></param>
        /// <param name="prefmaxlen"></param>
        /// <param name="entriesread"></param>
        /// <param name="totalentries"></param>
        /// <param name="servertype"></param>
        /// <param name="domain"></param>
        /// <param name="resumeHandle"></param>
        /// <returns></returns>
        /// <seealso cref="http://www.pinvoke.net/default.aspx/netapi32.netserverenum" />
        [DllImport( "netapi32.dll", EntryPoint = "NetServerEnum" )]
        public static extern Int32 NetServerEnum( [MarshalAs( UnmanagedType.LPWStr )] String servername, Int32 level, out IntPtr bufptr, Int32 prefmaxlen, ref Int32 entriesread, ref Int32 totalentries, Sv101Types servertype, [MarshalAs( UnmanagedType.LPWStr )] String domain, IntPtr resumeHandle );

        [DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern IntPtr OpenFileMapping( Int32 dwDesiredAccess, Boolean bInheritHandle, String lpName );

        [DllImport( "shlwapi.dll", CharSet = CharSet.Auto )]
        public static extern Boolean PathCompactPathEx( [Out] StringBuilder pszOut, String szPath, Int32 cchMax, Int32 dwFlags );

        /// <summary>
        /// </summary>
        /// <param name="hThread"></param>
        /// <param name="dwThreadAffinityMask"></param>
        /// <returns></returns>
        /// <example>
        ///     SetThreadAffinityMask( GetCurrentThread(), new IntPtr( 1 &lt;&lt; processor ) );
        /// </example>
        [DllImport( "kernel32.dll" )]
        public static extern IntPtr SetThreadAffinityMask( IntPtr hThread, IntPtr dwThreadAffinityMask );

        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern Boolean UnmapViewOfFile( IntPtr lpBaseAddress );

        [StructLayout( LayoutKind.Sequential )]
        public struct ATA_PASS_THROUGH_EX {
            public UInt16 Length;

            public UInt16 AtaFlags;

            public Byte PathId;

            public Byte TargetId;

            public Byte Lun;

            public Byte ReservedAsUchar;

            public UInt32 DataTransferLength;

            public UInt32 TimeOutValue;

            public UInt32 ReservedAsUlong;

            public IntPtr DataBufferOffset;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 8 )]
            public Byte[] PreviousTaskFile;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 8 )]
            public Byte[] CurrentTaskFile;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct ATAIdentifyDeviceQuery {
            public ATA_PASS_THROUGH_EX header;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
            public UInt16[] data;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DEVICE_SEEK_PENALTY_DESCRIPTOR {
            public UInt32 Version;

            public UInt32 Size;

            [MarshalAs( UnmanagedType.U1 )]
            public Boolean IncursSeekPenalty;
        }

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
        public struct FILE_ID_BOTH_DIR_INFO {
            public UInt32 NextEntryOffset;

            public UInt32 FileIndex;

            public LargeInteger CreationTime;

            public LargeInteger LastAccessTime;

            public LargeInteger LastWriteTime;

            public LargeInteger ChangeTime;

            public LargeInteger EndOfFile;

            public LargeInteger AllocationSize;

            public UInt32 FileAttributes;

            public UInt32 FileNameLength;

            public UInt32 EaSize;

            public Char ShortNameLength;

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 12 )]
            public String ShortName;

            public LargeInteger FileId;

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 1 )]
            public String FileName;
        }

        /// <summary>
        ///     Win32 FILETIME structure. The win32 documentation says this: "Contains a 64-bit value
        ///     representing the number of 100-nanosecond intervals since January 1, 1601 (UTC)."
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/ms724284%28VS.85%29.aspx" />
        [StructLayout( LayoutKind.Sequential )]
        public struct Filetime {
            public UInt32 dwLowDateTime;

            public UInt32 dwHighDateTime;
        }

        [StructLayout( LayoutKind.Explicit )]
        public struct LargeInteger {

            [FieldOffset( 0 )]
            public Int32 Low;

            [FieldOffset( 4 )]
            public Int32 High;

            [FieldOffset( 0 )]
            public Int64 QuadPart;

            /// <summary>
            ///     use only when QuadPart cannot be passed
            /// </summary>
            /// <returns></returns>
            public Int64 ToInt64() {
                return ( ( Int64 )this.High << 32 ) | ( UInt32 )this.Low;
            }

            // just for demonstration
            public static LargeInteger FromInt64( Int64 value ) {
                return new LargeInteger { Low = ( Int32 )value, High = ( Int32 )( value >> 32 ) };
            }
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct ServerInfo101 {

            [MarshalAs( UnmanagedType.U4 )]
            public readonly UInt32 sv101_platform_id;

            [MarshalAs( UnmanagedType.LPWStr )]
            public readonly String sv101_name;

            [MarshalAs( UnmanagedType.U4 )]
            public readonly UInt32 sv101_version_major;

            [MarshalAs( UnmanagedType.U4 )]
            public readonly UInt32 sv101_version_minor;

            [MarshalAs( UnmanagedType.U4 )]
            public readonly UInt32 sv101_type;

            [MarshalAs( UnmanagedType.LPWStr )]
            public readonly String sv101_comment;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct STORAGE_PROPERTY_QUERY {
            public UInt32 PropertyId;

            public UInt32 QueryType;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1 )]
            public Byte[] AdditionalParameters;
        }

        /// <summary>
        ///     The Win32 find data structure. The documentation says: "Contains information about the
        ///     file that is found by the FindFirstFile, FindFirstFileEx, or FindNextFile function."
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/aa365740%28VS.85%29.aspx" />
        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
        public struct Win32FindData {
            public FileAttributes dwFileAttributes;

            public Filetime ftCreationTime;

            public Filetime ftLastAccessTime;

            public Filetime ftLastWriteTime;

            public UInt32 nFileSizeHigh;

            public UInt32 nFileSizeLow;

            public UInt32 dwReserved0;

            public UInt32 dwReserved1;

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = MaxPath )]
            public String cFileName;

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 14 )]
            public String cAlternateFileName;
        }
    }
}