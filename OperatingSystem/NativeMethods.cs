// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/NativeMethods.cs" was last cleaned by Protiguous on 2018/05/14 at 6:26 PM

namespace Librainian.OperatingSystem {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading;
    using Controls;
    using Extensions;
    using FileSystem;
    using Graphics;
    using Graphics.Video;
    using Microsoft.Win32.SafeHandles;

    [SuppressMessage( "ReSharper", "InconsistentNaming" )]
    public static class NativeMethods {

        public const UInt32 ATA_FLAGS_DATA_IN = 0x02;

        public const Int32 DIGCF_DEVICEINTERFACE = 0x00000010;

        public const Int32 DIGCF_PRESENT = 0x00000002;

        public const Int32 ERROR_INSUFFICIENT_BUFFER = 122;

        public const Int32 ERROR_INVALID_DATA = 13;

        public const Int32 ERROR_NO_MORE_ITEMS = 259;

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

        public const String GUID_DEVINTERFACE_DISK = "53f56307-b6bf-11d0-94f2-00a0c91efb8b";

        public const String GUID_DEVINTERFACE_VOLUME = "53f5630d-b6bf-11d0-94f2-00a0c91efb8b";

        public const UInt32 IOCTL_SCSI_BASE = FILE_DEVICE_CONTROLLER;

        public const UInt32 IOCTL_STORAGE_BASE = FILE_DEVICE_MASS_STORAGE;

        public const Int32 IOCTL_STORAGE_GET_DEVICE_NUMBER = 0x002d1080;

        public const Int32 IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS = 0x00560000;

        public const Int32 MaxPath = 260;

        public const UInt32 METHOD_BUFFERED = 0;

        public const UInt32 OPEN_EXISTING = 3;

        public const UInt32 PropertyStandardQuery = 0;

        public const Int32 SPDRP_CAPABILITIES = 0x0000000F;

        public const Int32 SPDRP_CLASS = 0x00000007;

        public const Int32 SPDRP_CLASSGUID = 0x00000008;

        public const Int32 SPDRP_DEVICEDESC = 0x00000000;

        public const Int32 SPDRP_FRIENDLYNAME = 0x0000000C;

        public const UInt32 StorageDeviceSeekPenaltyProperty = 7;

        public const Int32 WM_DEVICECHANGE = 0x0219;

        [Flags]
        public enum AllocationType : UInt32 {

            COMMIT = 0x1000,

            RESERVE = 0x2000,

            RESET = 0x80000,

            LARGE_PAGES = 0x20000000,

            PHYSICAL = 0x400000,

            TOP_DOWN = 0x100000,

            WRITE_WATCH = 0x200000
        }

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

        [Flags]
        public enum HeapFlags {

            HEAP_NO_SERIALIZE = 0x1,

            HEAP_GENERATE_EXCEPTIONS = 0x4,

            HEAP_ZERO_MEMORY = 0x8
        }

        [Flags]
        public enum MemoryProtection : UInt32 {

            EXECUTE = 0x10,

            EXECUTE_READ = 0x20,

            EXECUTE_READWRITE = 0x40,

            EXECUTE_WRITECOPY = 0x80,

            NOACCESS = 0x01,

            READONLY = 0x02,

            READWRITE = 0x04,

            WRITECOPY = 0x08,

            GUARD_Modifierflag = 0x100,

            NOCACHE_Modifierflag = 0x200,

            WRITECOMBINE_Modifierflag = 0x400
        }

        public enum PLATFORM_ID {

            PlatformIDDos = 300,

            PlatformIDOs2 = 400,

            PlatformIDNt = 500,

            PlatformIDOsf = 600,

            PlatformIDVms = 700
        }

        public enum PNP_VETO_TYPE {

            Ok,

            TypeUnknown,

            LegacyDevice,

            PendingClose,

            WindowsApp,

            WindowsService,

            OutstandingOpen,

            Device,

            Driver,

            IllegalDeviceRequest,

            InsufficientPower,

            NonDisableable,

            LegacyDriver
        }

        [Flags]
        public enum Sv101Types : UInt32 {

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
        internal static extern Boolean AllocConsole();

        //[DllImport( "kernel32.dll", SetLastError = true )]
        //internal static extern IntPtr CreateFile( String lpFileName, JunctionPoint.EFileAccess dwDesiredAccess, EFileShare dwShareMode, IntPtr lpSecurityAttributes, JunctionPoint.ECreationDisposition dwCreationDisposition, JunctionPoint.EFileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile );

        //[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        //internal static extern IntPtr CreateFile( String lpFileName, Int32 dwDesiredAccess, Int32 dwShareMode, IntPtr lpSecurityAttributes, Int32 dwCreationDisposition, Int32 dwFlagsAndAttributes, IntPtr hTemplateFile );

        //[DllImport( "kernel32.dll", SetLastError = true )]
        //internal static extern IntPtr CreateFile( String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport( "avifil32.dll" )]
        internal static extern Int32 AVIFileCreateStream( Int32 pfile, out IntPtr ppavi, ref Avi.Avistreaminfo ptrStreaminfo );

        [DllImport( "avifil32.dll" )]
        internal static extern void AVIFileExit();

        [DllImport( "avifil32.dll" )]
        internal static extern Int32 AVIFileGetStream( Int32 pfile, out IntPtr ppavi, Int32 fccType, Int32 lParam );

        [DllImport( "avifil32.dll" )]
        internal static extern void AVIFileInit();

        [DllImport( "avifil32.dll", PreserveSig = true, BestFitMapping = false, ThrowOnUnmappableChar = true )]
        internal static extern Int32 AVIFileOpen( ref Int32 ppfile, [MarshalAs( UnmanagedType.LPWStr )] String szFile, Int32 uMode, Int32 pclsidHandler );

        [DllImport( "avifil32.dll" )]
        internal static extern Int32 AVIFileRelease( Int32 pfile );

        [DllImport( "avifil32.dll" )]
        internal static extern Int32 AVIStreamGetFrame( Int32 pGetFrameObj, Int32 lPos );

        [DllImport( "avifil32.dll" )]
        internal static extern Int32 AVIStreamGetFrameClose( Int32 pGetFrameObj );

        [DllImport( "avifil32.dll" )]
        internal static extern Int32 AVIStreamGetFrameOpen( IntPtr pAviStream, ref Avi.Bitmapinfoheader bih );

        [DllImport( "avifil32.dll" )]
        internal static extern Int32 AVIStreamInfo( Int32 pAviStream, ref Avi.Avistreaminfo psi, Int32 lSize );

        [DllImport( "avifil32.dll", PreserveSig = true )]
        internal static extern Int32 AVIStreamLength( Int32 pavi );

        [DllImport( "avifil32.dll" )]
        internal static extern Int32 AVIStreamRelease( IntPtr aviStream );

        [DllImport( "avifil32.dll" )]
        internal static extern Int32 AVIStreamSetFormat( IntPtr aviStream, Int32 lPos, ref Avi.Bitmapinfoheader lpFormat, Int32 cbFormat );

        [DllImport( "avifil32.dll", PreserveSig = true )]
        internal static extern Int32 AVIStreamStart( Int32 pavi );

        [DllImport( "avifil32.dll" )]
        internal static extern Int32 AVIStreamWrite( IntPtr aviStream, Int32 lStart, Int32 lSamples, IntPtr lpBuffer, Int32 cbBuffer, Int32 dwFlags, Int32 dummy1, Int32 dummy2 );

        [DllImport( "User32.Dll" )]
        internal static extern Boolean ClientToScreen( IntPtr hWnd, ref Win32.POINT point );

        [DllImport( "kernel32.dll", SetLastError = true )]
        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs( UnmanagedType.Bool )]
        internal static extern Boolean CloseHandle( IntPtr handle );

        [DllImport( "setupapi.dll" )]
        internal static extern Int32 CM_Get_Device_ID( Int32 dnDevInst, [MarshalAs( UnmanagedType.LPWStr )] StringBuilder buffer, Int32 bufferLen, Int32 ulFlags );

        [DllImport( "setupapi.dll" )]
        internal static extern Int32 CM_Get_Parent( ref Int32 pdnDevInst, UInt32 dnDevInst, Int32 ulFlags );

        [DllImport( "setupapi.dll", CharSet = CharSet.Unicode )]
        internal static extern Int32 CM_Request_Device_Eject( UInt32 dnDevInst, out PNP_VETO_TYPE pVetoType, [MarshalAs( UnmanagedType.LPWStr )] StringBuilder pszVetoName, Int32 ulNameLength, Int32 ulFlags );

        [DllImport( "setupapi.dll", EntryPoint = "CM_Request_Device_Eject", CharSet = CharSet.Unicode )]
        internal static extern Int32 CM_Request_Device_Eject_NoUi( UInt32 dnDevInst, IntPtr pVetoType, [MarshalAs( UnmanagedType.LPWStr )] StringBuilder pszVetoName, Int32 ulNameLength, Int32 ulFlags );

        [DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
        internal static extern SafeFileHandle CreateFile( String lpFileName, [MarshalAs( UnmanagedType.U4 )] FileAccess dwDesiredAccess, [MarshalAs( UnmanagedType.U4 )] FileShare dwShareMode, IntPtr lpSecurityAttributes,
            [MarshalAs( UnmanagedType.U4 )] FileMode dwCreationDisposition, [MarshalAs( UnmanagedType.U4 )] FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
        internal static extern IntPtr CreateFileMapping( IntPtr hFile, IntPtr lpAttributes, Int32 flProtect, Int32 dwMaximumSizeLow, Int32 dwMaximumSizeHigh, String lpName );

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern SafeFileHandle CreateFileW( [MarshalAs( UnmanagedType.LPWStr )] String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition,
            UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        internal static extern Boolean DeviceIoControl( IntPtr hDevice, UInt32 dwIoControlCode, IntPtr inBuffer, Int32 nInBufferSize, IntPtr outBuffer, Int32 nOutBufferSize, out Int32 pBytesReturned,
            IntPtr lpOverlapped );

        [DllImport( "kernel32.dll" )]
        internal static extern Int32 DeviceIoControl( IntPtr hDevice, Int32 dwIoControlCode, ref Int16 lpInBuffer, Int32 nInBufferSize, IntPtr lpOutBuffer, Int32 nOutBufferSize, ref Int32 lpBytesReturned,
            IntPtr lpOverlapped );

        //[DllImport( "kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto )]
        //internal static extern Boolean DeviceIoControl( IntPtr hDevice, UInt32 dwIoControlCode, IntPtr lpInBuffer, UInt32 nInBufferSize, IntPtr lpOutBuffer, UInt32 nOutBufferSize, out UInt32 lpBytesReturned, IntPtr lpOverlapped );

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern Boolean DeviceIoControl( IntPtr hDevice, UInt32 dwIoControlCode, IntPtr lpInBuffer, UInt32 nInBufferSize, [Out] IntPtr lpOutBuffer, UInt32 nOutBufferSize, out UInt32 lpBytesReturned,
            IntPtr lpOverlapped );

        [DllImport( "Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        internal static extern Boolean DeviceIoControl( IntPtr hDevice, UInt32 dwIoControlCode, ref Int64 inBuffer, Int32 inBufferSize, ref Int64 outBuffer, Int32 outBufferSize, ref Int32 bytesReturned,
            [In] ref NativeOverlapped overlapped );

        [DllImport( "kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        internal static extern Boolean DeviceIoControl( SafeFileHandle hDevice, UInt32 dwIoControlCode, ref STORAGE_PROPERTY_QUERY lpInBuffer, UInt32 nInBufferSize, ref DEVICE_SEEK_PENALTY_DESCRIPTOR lpOutBuffer,
            UInt32 nOutBufferSize, out UInt32 lpBytesReturned, IntPtr lpOverlapped );

        [DllImport( "kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        internal static extern Boolean DeviceIoControl( SafeFileHandle hDevice, UInt32 dwIoControlCode, ref ATAIdentifyDeviceQuery lpInBuffer, UInt32 nInBufferSize, ref ATAIdentifyDeviceQuery lpOutBuffer,
            UInt32 nOutBufferSize, out UInt32 lpBytesReturned, IntPtr lpOverlapped );

        [DllImport( "advapi32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        internal static extern Int32 DuplicateToken( IntPtr hToken, Int32 impersonationLevel, ref IntPtr hNewToken );

        [DllImport( "user32.dll" )]
        internal static extern Int32 EnableMenuItem( this IntPtr tMenu, Int32 targetItem, Int32 targetStatus );

        /// <summary>
        /// Closes a file search handle opened by the FindFirstFile, FindFirstFileEx, or FindFirstStreamW function.
        /// </summary>
        /// <param name="hFindFile">The file search handle.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/aa364413%28VS.85%29.aspx"/>
        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern Boolean FindClose( IntPtr hFindFile );

        /// <summary>
        /// Searches a directory for a file or subdirectory with a name that matches a specific name (or partial name if wildcards are used).
        /// </summary>
        /// <param name="lpFileName">The directory or path, and the file name, which can include wildcard characters, for example, an asterisk (*) or a question mark (?).</param>
        /// <param name="lpFindData">A pointer to the WIN32_FIND_DATA structure that receives information about a found file or directory.</param>
        /// <returns>
        /// If the function succeeds, the return value is a search handle used in a subsequent call to FindNextFile or FindClose, and the lpFindFileData parameter contains information about the first file or directory
        /// found. If the function fails or fails to locate files from the search String in the lpFileName parameter, the return value is INVALID_HANDLE_VALUE and the contents of lpFindFileData are indeterminate.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/aa364418%28VS.85%29.aspx"/>
        [DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false )]
        internal static extern SafeSearchHandle FindFirstFile( String lpFileName, out Win32FindData lpFindData );

        /// <summary>
        /// Continues a file search from a previous call to the FindFirstFile or FindFirstFileEx function.
        /// </summary>
        /// <param name="hFindFile"> The search handle returned by a previous call to the FindFirstFile or FindFirstFileEx function.</param>
        /// <param name="lpFindData">
        /// A pointer to the WIN32_FIND_DATA structure that receives information about the found file or subdirectory. The structure can be used in subsequent calls to FindNextFile to indicate from which file to continue
        /// the search.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero and the lpFindFileData parameter contains information about the next file or directory found. If the function fails, the return value is zero and the
        /// contents of lpFindFileData are indeterminate.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/aa364428%28VS.85%29.aspx"/>
        [DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false )]
        internal static extern Boolean FindNextFile( SafeSearchHandle hFindFile, out Win32FindData lpFindData );

        [DllImport( "user32.dll", CharSet = CharSet.Unicode )]
        internal static extern IntPtr FindWindow( String cls, String win );

        [DllImport( "user32.dll", ThrowOnUnmappableChar = true, BestFitMapping = false )]
        internal static extern IntPtr FindWindowEx( IntPtr hwndParent, IntPtr hwndChildAfter, [MarshalAs( UnmanagedType.LPWStr )] String lpszClass, String lpszWindow );

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern Boolean FlushViewOfFile( IntPtr lpBaseAddress, IntPtr dwNumBytesToFlush );

        [DllImport( "kernel32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true )]
        internal static extern UInt32 FormatMessage( UInt32 dwFlags, IntPtr lpSource, UInt32 dwMessageId, UInt32 dwLanguageId, [MarshalAs( UnmanagedType.LPWStr )] StringBuilder lpBuffer, UInt32 nSize, IntPtr arguments );

        [DllImport( "kernel32.dll", SetLastError = true, ExactSpelling = true )]
        internal static extern Boolean FreeConsole();

        [DllImport( "iphlpapi.dll", CharSet = CharSet.Ansi )]
        internal static extern Int32 GetAdaptersInfo( IntPtr pAdapterInfo, ref Int64 pBufOutLen );

        [DllImport( "iphlpapi.dll", SetLastError = true )]
        internal static extern Int32 GetBestInterface( UInt32 destAddr, out UInt32 bestIfIndex );

        /// <summary>
        /// <para>Retrieves the actual number of bytes of disk storage used to store a specified file as a transacted operation.</para>
        /// <para>If the file is located on a volume that supports compression and the file is compressed, the value obtained is the compressed size of the specified file.</para>
        /// <para>If the file is located on a volume that supports sparse files and the file is a sparse file, the value obtained is the sparse size of the specified file.</para>
        /// </summary>
        /// <param name="lpFileName">    </param>
        /// <param name="lpFileSizeHigh"></param>
        /// <returns></returns>
        /// <seealso cref="http://msdn.microsoft.com/en-us/Library/windows/desktop/aa364930(v=vs.85).aspx"/>
        [DllImport( "kernel32.dll" )]
        internal static extern UInt32 GetCompressedFileSizeW( [In] [MarshalAs( UnmanagedType.LPWStr )]
            String lpFileName, [Out] [MarshalAs( UnmanagedType.U4 )] out UInt32 lpFileSizeHigh );

        [DllImport( "kernel32.dll" )]
        internal static extern IntPtr GetCurrentThread();

        [DllImport( "user32.dll" )]
        internal static extern IntPtr GetDC( IntPtr hWnd );

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="http://www.facepunch.com/showthread.php?t=1312991"/>
        internal static IntPtr GetDesktopHandle() {
            var desktop = GetDesktopWindow();
            var progMan = FindWindowEx( desktop, IntPtr.Zero, "Progman", "Program Manager" );
            var defView = FindWindowEx( progMan, IntPtr.Zero, "SHELLDLL_DefView", String.Empty );

            //var listView = FindWindowEx( defView, IntPtr.Zero, "SysListView32", "FolderView" );

            return defView;
        }

        [DllImport( "user32.dll", EntryPoint = "GetDesktopWindow" )]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport( "gdi32.dll" )]
        internal static extern Boolean GetDeviceGammaRamp( IntPtr hDC, ref GraphicsExtensions.RAMP lpRamp );

        [DllImport( "kernel32.dll", SetLastError = true, PreserveSig = true )]
        internal static extern UInt32 GetDiskFreeSpaceW( [In] [MarshalAs( UnmanagedType.LPWStr )]
            String lpRootPathName, out UInt32 lpSectorsPerCluster, out UInt32 lpBytesPerSector, out UInt32 lpNumberOfFreeClusters, out UInt32 lpTotalNumberOfClusters );

        internal static String GetErrorMessage( Int32 code ) {
            var message = new StringBuilder( 255 );

            FormatMessage( FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, ( UInt32 )code, 0, message, ( UInt32 )message.Capacity, IntPtr.Zero );

            return message.ToString();
        }

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern Boolean GetFileInformationByHandleEx( IntPtr hFile, FILE_INFO_BY_HANDLE_CLASS infoClass, out FILE_ID_BOTH_DIR_INFO dirInfo, UInt32 dwBufferSize );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern IntPtr GetProcessHeap();

        [DllImport( "user32.dll" )]
        internal static extern IntPtr GetSystemMenu( IntPtr hwndValue, Boolean isRevert );

        [DllImport( "Kernel32.dll", CallingConvention = CallingConvention.Winapi )]
        internal static extern void GetSystemTimePreciseAsFileTime( out Int64 filetime );

        [DllImport( "kernel32", CharSet = CharSet.Unicode, SetLastError = true )]
        internal static extern Boolean GetVolumeNameForVolumeMountPoint( [MarshalAs( UnmanagedType.LPWStr )] String volumeName, [MarshalAs( UnmanagedType.LPWStr )] StringBuilder uniqueVolumeName,
            UInt32 uniqueNameBufferCapacity );

        [DllImport( "user32.dll" )]
        internal static extern IntPtr GetWindowDC( IntPtr hwnd );

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern IntPtr HeapAlloc( IntPtr hHeap, HeapFlags dwFlags, UInt32 dwSize );

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern IntPtr HeapCreate( UInt32 flOptions, UIntPtr dwInitialSize, UIntPtr dwMaximumSize );

        //[DllImport( "kernel32.dll", SetLastError = true )]
        //internal static extern IntPtr HeapCreate( HeapFlags flOptions, UInt64 dwInitialsize, UInt64 dwMaximumSize );
        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern Boolean HeapDestroy( IntPtr hHeap );

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern Boolean HeapFree( IntPtr hHeap, HeapFlags dwFlags, IntPtr lpMem );

        [DllImport( "kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi )]
        [return: MarshalAs( UnmanagedType.Bool )]
        internal static extern Boolean IsWow64Process( [In] IntPtr process, [Out] out Boolean wow64Process );

        [DllImport( "user32.dll", SetLastError = true )]
        internal static extern Boolean LockWorkStation();

        [DllImport( "advapi32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true )]
        internal static extern Int32 LogonUser( String lpszUserName, String lpszDomain, String lpszPassword, Int32 dwLogonType, Int32 dwLogonProvider, ref IntPtr phToken );

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern IntPtr MapViewOfFile( IntPtr hFileMappingObject, Int32 dwDesiredAccess, Int32 dwFileOffsetHigh, Int32 dwFileOffsetLow, IntPtr dwNumBytesToMap );

        [DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
        internal static extern Boolean MoveFileWithProgress( [MarshalAs( UnmanagedType.LPWStr )] String lpExistingFileName, String lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData,
            MoveFileFlags dwFlags );

        /// <summary>
        /// Netapi32.dll : The NetApiBufferFree function frees the memory that the NetApiBufferAllocate function allocates. Call NetApiBufferFree to free the memory that other network management functions return.
        /// </summary>
        [DllImport( "netapi32.dll", EntryPoint = "NetApiBufferFree" )]
        internal static extern Int32 NetApiBufferFree( IntPtr buffer );

        /// <summary>
        /// The NetServerEnum function lists all servers of the specified type that are visible in a domain.
        /// </summary>
        /// <param name="servername">  </param>
        /// <param name="level">       </param>
        /// <param name="bufptr">      </param>
        /// <param name="prefmaxlen">  </param>
        /// <param name="entriesread"> </param>
        /// <param name="totalentries"></param>
        /// <param name="servertype">  </param>
        /// <param name="domain">      </param>
        /// <param name="resumeHandle"></param>
        /// <returns></returns>
        /// <seealso cref="http://www.pinvoke.net/default.aspx/netapi32.netserverenum"/>
        [DllImport( "netapi32.dll", EntryPoint = "NetServerEnum" )]
        internal static extern Int32 NetServerEnum( [MarshalAs( UnmanagedType.LPWStr )] String servername, Int32 level, out IntPtr bufptr, Int32 prefmaxlen, ref Int32 entriesread, ref Int32 totalentries,
            Sv101Types servertype, [MarshalAs( UnmanagedType.LPWStr )] String domain, IntPtr resumeHandle );

        [DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
        internal static extern IntPtr OpenFileMapping( Int32 dwDesiredAccess, Boolean bInheritHandle, [MarshalAs( UnmanagedType.LPWStr )] String lpName );

        [DllImport( "shlwapi.dll", CharSet = CharSet.Unicode )]
        internal static extern Boolean PathCompactPathEx( [MarshalAs( UnmanagedType.LPWStr )] [Out]
            StringBuilder pszOut, String szPath, Int32 cchMax, Int32 dwFlags );

        [DllImport( "kernel32.dll" )]
        internal static extern Boolean QueryPerformanceCounter( out Int64 value );

        [DllImport( "kernel32.dll" )]
        internal static extern Boolean QueryPerformanceFrequency( out Int64 value );

        [DllImport( "user32.dll" )]
        internal static extern Int32 ReleaseDC( IntPtr hwnd, IntPtr dc );

        [DllImport( "advapi32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        internal static extern Boolean RevertToSelf();

        [DllImport( "user32.dll" )]
        internal static extern IntPtr SendMessage( IntPtr hWnd, Int32 msg, IntPtr wp, IntPtr lp );

        [DllImport( "User32.Dll" )]
        internal static extern Int64 SetCursorPos( Int32 x, Int32 y );

        [DllImport( "gdi32.dll" )]
        internal static extern Boolean SetDeviceGammaRamp( IntPtr hDC, ref GraphicsExtensions.RAMP lpRamp );

        /// <summary>
        /// </summary>
        /// <param name="hThread">             </param>
        /// <param name="dwThreadAffinityMask"></param>
        /// <returns></returns>
        /// <example>SetThreadAffinityMask( GetCurrentThread(), new IntPtr( 1 &lt;&lt; processor ) );</example>
        [DllImport( "kernel32.dll" )]
        internal static extern IntPtr SetThreadAffinityMask( IntPtr hThread, IntPtr dwThreadAffinityMask );

        //[DllImport( "kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto )]
        //internal static extern Boolean DeviceIoControl( IntPtr hDevice, UInt32 dwIoControlCode, IntPtr lpInBuffer, UInt32 nInBufferSize, IntPtr lpOutBuffer, UInt32 nOutBufferSize, out UInt32 lpBytesReturned, IntPtr lpOverlapped );
        [DllImport( "setupapi.dll" )]
        internal static extern UInt32 SetupDiDestroyDeviceInfoList( IntPtr deviceInfoSet );

        //[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        //internal static extern SafeFileHandle CreateFile( String lpFileName, [MarshalAs( UnmanagedType.U4 )] FileAccess dwDesiredAccess, [MarshalAs( UnmanagedType.U4 )] FileShare dwShareMode, IntPtr lpSecurityAttributes, [MarshalAs( UnmanagedType.U4 )] FileMode dwCreationDisposition, [MarshalAs( UnmanagedType.U4 )] FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile );
        [DllImport( "setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode )]
        internal static extern Boolean SetupDiEnumDeviceInterfaces( IntPtr deviceInfoSet, SP_DEVINFO_DATA deviceInfoData, ref Guid interfaceClassGuid, Int32 memberIndex, SP_DEVICE_INTERFACE_DATA deviceInterfaceData );

        [DllImport( "setupapi.dll", CharSet = CharSet.Auto, SetLastError = true )]
        internal static extern Boolean SetupDiEnumDeviceInterfaces( IntPtr hDevInfo, ref SP_DEVINFO_DATA devInfo, ref Guid interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData );

        [DllImport( "setupapi.dll", CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true )]
        internal static extern IntPtr SetupDiGetClassDevs( ref Guid classGuid, [MarshalAs( UnmanagedType.LPTStr )] String enumerator, IntPtr hwndParent, UInt32 flags );

        //[DllImport( "setupapi.dll" )]
        //internal static extern IntPtr SetupDiGetClassDevs( ref Guid classGuid, Int64 enumerator, IntPtr hwndParent, Int32 flags );
        [DllImport( "setupapi.dll", SetLastError = true, CharSet = CharSet.Auto )]
        internal static extern Boolean SetupDiGetDeviceInterfaceDetail( IntPtr deviceInfoSet, SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, Int32 deviceInterfaceDetailDataSize,
            ref Int32 requiredSize, SP_DEVINFO_DATA deviceInfoData );

        [DllImport( "setupapi.dll", CharSet = CharSet.Auto, SetLastError = true )]
        internal static extern Boolean SetupDiGetDeviceRegistryProperty( IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, UInt32 property, out UInt32 propertyRegDataType, Byte[] propertyBuffer,
            UInt32 propertyBufferSize, out UInt32 requiredSize );

        [DllImport( "setupapi.dll", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true )]
        internal static extern Boolean SetupDiOpenDeviceInfo( IntPtr deviceInfoSet, [MarshalAs( UnmanagedType.LPWStr )] String deviceInstanceId, IntPtr hwndParent, Int32 openFlags, SP_DEVINFO_DATA deviceInfoData );

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern Boolean UnmapViewOfFile( IntPtr lpBaseAddress );

        [DllImport( "mpr.dll", BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true, CharSet = CharSet.Unicode )]
        internal static extern Int32 WNetAddConnection2( NetResource netResource, [MarshalAs( UnmanagedType.LPWStr )] String password, [MarshalAs( UnmanagedType.LPWStr )] String username, Int32 flags );

        //[DllImport("Mpr.dll", EntryPoint="WNetAddConnection2", CallingConvention=CallingConvention.Winapi)]
        //private static extern ErrorCodes WNetAddConnection2( NETRESOURCE lpNetResource,ref String lpPassword,ref     String lpUsername, UInt32 dwFlags );

        /// <summary>
        /// This must be used if NETRESOURCE is defined as a struct???
        /// </summary>
        /// <param name="netResource"></param>
        /// <param name="password">   </param>
        /// <param name="username">   </param>
        /// <param name="flags">      </param>
        /// <returns></returns>
        [DllImport( "mpr.dll" )]
        internal static extern Int32 WNetAddConnection2( ref NetResource netResource, [MarshalAs( UnmanagedType.LPWStr )] String password, [MarshalAs( UnmanagedType.LPWStr )] String username, UInt32 flags );

        [DllImport( "mpr.dll" )]
        internal static extern Int32 WNetCancelConnection2( [MarshalAs( UnmanagedType.LPWStr )] String name, Int32 flags, Boolean force );

        [SuppressMessage( "Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible" )]
        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern IntPtr VirtualAlloc( IntPtr lpAddress, UIntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect );

        [SuppressMessage( "Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "1" )]
        [SuppressMessage( "Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible" )] //BUG here's a bug waiting to happen...
        [DllImport( "kernel32" )]
        public static extern Boolean VirtualFree( IntPtr lpAddress, UInt32 dwSize, UInt32 dwFreeType );

        [SuppressMessage( "Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible" )]
        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern Boolean VirtualFree( UIntPtr lpAddress, UIntPtr dwSize, UInt32 dwFreeType );

        [SuppressMessage( "Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable" )]
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

            internal IntPtr DataBufferOffset;

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

        [StructLayout( LayoutKind.Sequential )]
        public struct DISK_EXTENT {

            public Int32 DiskNumber;

            public Int64 StartingOffset;

            public Int64 ExtentLength;
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
        /// Win32 FILETIME structure. The win32 documentation says this: "Contains a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601 (UTC)."
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/ms724284%28VS.85%29.aspx"/>
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
            /// use only when QuadPart cannot be passed
            /// </summary>
            /// <returns></returns>
            public Int64 ToInt64() => ( ( Int64 )this.High << 32 ) | ( UInt32 )this.Low;

            // just for demonstration
            internal static LargeInteger FromInt64( Int64 value ) => new LargeInteger { Low = ( Int32 )value, High = ( Int32 )( value >> 32 ) };
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
        public struct STORAGE_DEVICE_NUMBER {

            public Int32 DeviceType;

            public Int32 DeviceNumber;

            public Int32 PartitionNumber;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct STORAGE_PROPERTY_QUERY {

            public UInt32 PropertyId;

            public UInt32 QueryType;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1 )]
            public Byte[] AdditionalParameters;
        }

        /// <summary>
        /// The Win32 find data structure. The documentation says: "Contains information about the file that is found by the FindFirstFile, FindFirstFileEx, or FindNextFile function."
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/aa365740%28VS.85%29.aspx"/>
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

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
        public class SP_DEVICE_INTERFACE_DATA {

            public UInt32 cbSize;

            public UInt32 Flags;

            public Guid InterfaceClassGuid;

            private IntPtr Reserved;
        }

        [StructLayout( LayoutKind.Sequential, Pack = 2 )]
        public class SP_DEVICE_INTERFACE_DETAIL_DATA {

            public Int32 cbSize;

            public Int16 devicePath;
        }

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
        public class SP_DEVINFO_DATA {

            public UInt32 cbSize;

            public Guid classGuid;

            public UInt32 devInst;

            private IntPtr reserved;
        }
    }
}