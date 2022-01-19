// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "NativeMethods.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

#nullable enable

namespace Librainian.OperatingSystem;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using System.Threading;
using ComputerSystem.Devices;
using FileSystem;
using Graphics;
using Microsoft.Win32.SafeHandles;

[SuppressMessage( "ReSharper", "InconsistentNaming" )]
[SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Global" )]
public static class NativeMethods {

	/// <summary>
	///     https://msdn.microsoft.com/en-us/library/windows/desktop/aa363854(v=vs.85).aspx
	/// </summary>
	/// <param name="TotalFileSize"></param>
	/// <param name="TotalBytesTransferred"></param>
	/// <param name="StreamSize"></param>
	/// <param name="StreamBytesTransferred"></param>
	/// <param name="dwStreamNumber"></param>
	/// <param name="dwCallbackReason"></param>
	/// <param name="hSourceFile"></param>
	/// <param name="hDestinationFile"></param>
	/// <param name="lpData"></param>
	[UnmanagedFunctionPointer( CallingConvention.Winapi )]
	public delegate UInt32 CopyProgressRoutine(
		Int64 TotalFileSize,
		Int64 TotalBytesTransferred,
		Int64 StreamSize,
		Int64 StreamBytesTransferred,
		UInt32 dwStreamNumber,
		COPY_CALLBACK_REASON dwCallbackReason,
		[In] IntPtr hSourceFile,
		[In] IntPtr hDestinationFile,
		[In] IntPtr lpData
	);

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

	/// <summary>The reason that CopyProgressRoutine was called.</summary>
	public enum COPY_CALLBACK_REASON : UInt32 {

		/// <summary>Another part of the data file was copied.</summary>
		CALLBACK_CHUNK_FINISHED = 0x00000000,

		/// <summary>
		///     Another stream was created and is about to be copied. this is the callback reason given when the callback
		///     routine is first invoked.
		/// </summary>
		CALLBACK_STREAM_SWITCH = 0x00000001

	}

	public enum ErrorCodes {

		ERROR_FILE_NOT_FOUND = 2,

		ERROR_PATH_NOT_FOUND = 3,

		ERROR_ACCESS_DENIED = 5

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
	public enum FINDEX_ADDITIONAL_FLAGS {

		FindFirstExCaseSensitive,

		FindFirstExLargeFetch

	}

	public enum FINDEX_INFO_LEVELS {

		FindExInfoStandard = 0,

		FindExInfoBasic = 1,

		FindExInfoMaxInfoLevel = 2

	}

	public enum FINDEX_SEARCH_OPS {

		FindExSearchNameMatch,

		FindExSearchLimitToDirectories,

		FindExSearchLimitToDevices,

		FindExSearchMaxSearchOp

	}

	[Flags]
	public enum HeapFlags {

		HEAP_NO_SERIALIZE = 0x1,

		HEAP_GENERATE_EXCEPTIONS = 0x4,

		HEAP_ZERO_MEMORY = 0x8

	}

	public enum IconSize : Byte {

		Small = ICON_SMALL,

		Big = ICON_BIG

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

	internal const Int32 CREATE_ALWAYS = 2;

	internal const Int32 CREATE_NEW = 1;

	internal const Int32 FILE_APPEND_DATA = 0x0004;

	internal const Int32 FILE_ATTRIBUTE_ARCHIVE = 0x20;

	internal const Int32 FILE_ATTRIBUTE_DIRECTORY = 0x10;

	internal const Int64 FILE_GENERIC_READ = STANDARD_RIGHTS_READ | FILE_READ_DATA | FILE_READ_ATTRIBUTES | FILE_READ_EA | SYNCHRONIZE;

	internal const Int64 FILE_GENERIC_WRITE = STANDARD_RIGHTS_WRITE | FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | SYNCHRONIZE;

	internal const Int32 FILE_READ_ATTRIBUTES = 0x0080;

	internal const Int32 FILE_READ_DATA = 0x0001;

	internal const Int32 FILE_READ_EA = 0x0008;

	internal const Int32 FILE_SHARE_NONE = 0x00000000;

	internal const Int32 FILE_WRITE_ATTRIBUTES = 0x0100;

	internal const Int32 FILE_WRITE_DATA = 0x0002;

	internal const Int32 FILE_WRITE_EA = 0x0010;

	internal const Int32 INVALID_FILE_ATTRIBUTES = -1;

	internal const Int32 MAX_ALTERNATE = 14;

	internal const Int32 MAX_PATH = 260;

	internal const Int64 READ_CONTROL = 0x00020000L;

	internal const Int64 STANDARD_RIGHTS_READ = READ_CONTROL;

	internal const Int64 STANDARD_RIGHTS_WRITE = READ_CONTROL;

	internal const Int64 SYNCHRONIZE = 0x00100000L;

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

	public const Int32 ICON_BIG = 1;

	public const Int32 ICON_SMALL = 0;

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

	public const Int32 WM_SETICON = 0x80;

	private static readonly IntPtr NegativeOneIntPtr = new(-1);

	[DllImport( "shlwapi.dll", CharSet = CharSet.Unicode )]
	public static extern Boolean PathMatchSpec( [In] String pszFileParam, [In] String pszSpec );

	/*
	[DllImport( "Mpr.dll", EntryPoint = "WNetAddConnection2", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true )]
	private static extern Int32 WNetAddConnection2( NETRESOURCE lpNetResource, ref String lpPassword, ref String lpUsername, UInt32 dwFlags );
	*/

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean AllocConsole();

	/*
	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIFileCreateStream( Int32 pfile, out IntPtr ppavi, ref Avi.Avistreaminfo ptrStreaminfo );
	*/

	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern void AVIFileExit();

	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIFileGetStream( Int32 pfile, out IntPtr ppavi, Int32 fccType, Int32 lParam );

	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern void AVIFileInit();

	[DllImport( "avifil32.dll", PreserveSig = true, BestFitMapping = false, ThrowOnUnmappableChar = true, CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIFileOpen( ref Int32 ppfile, [MarshalAs( UnmanagedType.LPWStr )] String szFile, Int32 uMode, Int32 pclsidHandler );

	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIFileRelease( Int32 pfile );

	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIStreamGetFrame( Int32 pGetFrameObj, Int32 lPos );

	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIStreamGetFrameClose( Int32 pGetFrameObj );

	[DllImport( "mpr.dll" )]
	public static extern Int32 WNetAddConnection2( ref NetResource netResource, String password, String username, UInt32 flags );

	[DllImport( "mpr.dll" )]
	public static extern Int32 WNetAddConnection2( NETRESOURCE netResource, String password, String username, UInt32 flags );

	/*
	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIStreamGetFrameOpen( IntPtr pAviStream, ref Avi.Bitmapinfoheader bih );
	*/

	/*
	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIStreamInfo( Int32 pAviStream, ref Avi.Avistreaminfo psi, Int32 lSize );
	*/

	[DllImport( "avifil32.dll", PreserveSig = true, CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIStreamLength( Int32 pavi );

	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIStreamRelease( IntPtr aviStream );

	/*
	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIStreamSetFormat( IntPtr aviStream, Int32 lPos, ref Avi.Bitmapinfoheader lpFormat, Int32 cbFormat );
	*/

	[DllImport( "avifil32.dll", PreserveSig = true, CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIStreamStart( Int32 pavi );

	[DllImport( "avifil32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 AVIStreamWrite( IntPtr aviStream, Int32 lStart, Int32 lSamples, IntPtr lpBuffer, Int32 cbBuffer, Int32 dwFlags, Int32 dummy1, Int32 dummy2 );

	[DllImport( "User32.Dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean ClientToScreen( IntPtr hWnd, ref Point point );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	[SuppressUnmanagedCodeSecurity]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern Boolean CloseHandle( this IntPtr handle );

	[DllImport( "setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 CM_Get_Device_ID( Int32 dnDevInst, [MarshalAs( UnmanagedType.LPWStr )] StringBuilder buffer, Int32 bufferLen, Int32 ulFlags );

	[DllImport( "setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 CM_Get_Parent( ref Int32 pdnDevInst, UInt32 dnDevInst, Int32 ulFlags );

	[DllImport( "setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 CM_Request_Device_Eject(
		UInt32 dnDevInst,
		out PNP_VETO_TYPE pVetoType,
		[MarshalAs( UnmanagedType.LPWStr )] StringBuilder pszVetoName,
		Int32 ulNameLength,
		Int32 ulFlags
	);

	[DllImport( "setupapi.dll", EntryPoint = "CM_Request_Device_Eject", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 CM_Request_Device_Eject_NoUi(
		UInt32 dnDevInst,
		IntPtr pVetoType,
		[MarshalAs( UnmanagedType.LPWStr )] StringBuilder? pszVetoName,
		Int32 ulNameLength,
		Int32 ulFlags
	);

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean CopyFileW( String lpExistingFileName, String lpNewFileName, Boolean bFailIfExists );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean CreateDirectory( String lpPathName, IntPtr lpSecurityAttributes );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern SafeFileHandle CreateFile(
		String lpFileName,
		[MarshalAs( UnmanagedType.U4 )] FileAccess dwDesiredAccess,
		[MarshalAs( UnmanagedType.U4 )] FileShare dwShareMode,
		IntPtr lpSecurityAttributes,
		[MarshalAs( UnmanagedType.U4 )] FileMode dwCreationDisposition,
		[MarshalAs( UnmanagedType.U4 )] FileAttributes dwFlagsAndAttributes,
		IntPtr hTemplateFile
	);

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern SafeFileHandle CreateFile(
		String lpFileName,
		Int32 dwDesiredAccess,
		Int32 dwShareMode,
		IntPtr lpSecurityAttributes,
		Int32 dwCreationDisposition,
		Int32 dwFlagsAndAttributes,
		IntPtr hTemplateFile
	);

	[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
	public static extern IntPtr CreateFileMapping( IntPtr hFile, IntPtr lpAttributes, Int32 flProtect, Int32 dwMaximumSizeLow, Int32 dwMaximumSizeHigh, String lpName );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern SafeFileHandle CreateFileW(
		[MarshalAs( UnmanagedType.LPWStr )] String lpFileName,
		UInt32 dwDesiredAccess,
		UInt32 dwShareMode,
		IntPtr lpSecurityAttributes,
		UInt32 dwCreationDisposition,
		UInt32 dwFlagsAndAttributes,
		IntPtr hTemplateFile
	);

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern Boolean DeleteFile( String path );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern Boolean DeleteFileW( String lpFileName );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean DeviceIoControl(
		IntPtr hDevice,
		UInt32 dwIoControlCode,
		IntPtr inBuffer,
		Int32 nInBufferSize,
		IntPtr outBuffer,
		Int32 nOutBufferSize,
		out Int32 pBytesReturned,
		IntPtr lpOverlapped
	);

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 DeviceIoControl(
		IntPtr hDevice,
		Int32 dwIoControlCode,
		ref Int16 lpInBuffer,
		Int32 nInBufferSize,
		IntPtr lpOutBuffer,
		Int32 nOutBufferSize,
		ref Int32 lpBytesReturned,
		IntPtr lpOverlapped
	);

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean DeviceIoControl(
		IntPtr hDevice,
		UInt32 dwIoControlCode,
		IntPtr lpInBuffer,
		UInt32 nInBufferSize,
		[Out] IntPtr lpOutBuffer,
		UInt32 nOutBufferSize,
		out UInt32 lpBytesReturned,
		IntPtr lpOverlapped
	);

	[DllImport( "Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean DeviceIoControl(
		IntPtr hDevice,
		UInt32 dwIoControlCode,
		ref Int64 inBuffer,
		Int32 inBufferSize,
		ref Int64 outBuffer,
		Int32 outBufferSize,
		ref Int32 bytesReturned,
		[In] ref NativeOverlapped overlapped
	);

	[DllImport( "kernel32.dll", EntryPoint = "DeviceIoControl", CharSet = CharSet.Unicode, SetLastError = true )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern Boolean DeviceIoControl(
		SafeFileHandle hDevice,
		UInt32 dwIoControlCode,
		ref STORAGE_PROPERTY_QUERY lpInBuffer,
		UInt32 nInBufferSize,
		ref DEVICE_SEEK_PENALTY_DESCRIPTOR lpOutBuffer,
		UInt32 nOutBufferSize,
		out UInt32 lpBytesReturned,
		IntPtr lpOverlapped
	);

	[DllImport( "kernel32.dll", EntryPoint = "DeviceIoControl", CharSet = CharSet.Unicode, SetLastError = true )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern Boolean DeviceIoControl(
		SafeFileHandle hDevice,
		UInt32 dwIoControlCode,
		ref ATAIdentifyDeviceQuery lpInBuffer,
		UInt32 nInBufferSize,
		ref ATAIdentifyDeviceQuery lpOutBuffer,
		UInt32 nOutBufferSize,
		out UInt32 lpBytesReturned,
		IntPtr lpOverlapped
	);

	[DllImport( "advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 DuplicateToken( IntPtr hToken, Int32 impersonationLevel, ref IntPtr hNewToken );

	[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 EnableMenuItem( this IntPtr tMenu, Int32 targetItem, Int32 targetStatus );

	/// <summary>Closes a file search handle opened by the FindFirstFile, FindFirstFileEx, or FindFirstStreamW function.</summary>
	/// <param name="hFindFile">The file search handle.</param>
	/// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
	/// <see cref="http://msdn.microsoft.com/en-us/Library/aa364413%28VS.85%29.aspx" />
	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean FindClose( IntPtr hFindFile );

	/// <summary>
	///     Closes a file search handle opened by the FindFirstFile, FindFirstFileEx, FindFirstFileNameW,
	///     FindFirstFileNameTransactedW, FindFirstFileTransacted,
	///     FindFirstStreamTransactedW, or FindFirstStreamW functions.
	/// </summary>
	/// <param name="hFindFile">The file search handle.</param>
	/// <returns>
	///     If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get
	///     extended error information, call GetLastError.
	/// </returns>
	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern Boolean FindClose( [In] SafeSearchHandle hFindFile );

	/// <summary>
	///     Searches a directory for a file or subdirectory with a name that matches a specific name (or partial name if
	///     wildcards are used).
	/// </summary>
	/// <param name="lpFileName">
	///     The directory or path, and the file name, which can include wildcard characters, for example,
	///     an asterisk (*) or a question mark (?).
	/// </param>
	/// <param name="lpFindData">
	///     A pointer to the WIN32_FIND_DATA structure that receives information about a found file or
	///     directory.
	/// </param>
	/// <returns>
	///     If the function succeeds, the return value is a search handle used in a subsequent call to FindNextFile or
	///     FindClose, and the lpFindFileData parameter contains
	///     information about the first file or directory found. If the function fails or fails to locate files from the search
	///     String in the lpFileName parameter, the return value is
	///     INVALID_HANDLE_VALUE and the contents of lpFindFileData are indeterminate.
	/// </returns>
	/// <see cref="http://msdn.microsoft.com/en-us/Library/aa364418%28VS.85%29.aspx" />
	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false )]
	public static extern SafeSearchHandle? FindFirstFile( String lpFileName, out Win32FindData lpFindData );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern SafeFindHandle FindFirstFileExW(
		String lpFileName,
		FINDEX_INFO_LEVELS fInfoLevelId,
		out WIN32_FIND_DATAW lpFindFileData,
		FINDEX_SEARCH_OPS fSearchOp,
		IntPtr lpSearchFilter,
		FINDEX_ADDITIONAL_FLAGS dwAdditionalFlags
	);

	/// <summary>Continues a file search from a previous call to the FindFirstFile or FindFirstFileEx function.</summary>
	/// <param name="hFindFile">The search handle returned by a previous call to the FindFirstFile or FindFirstFileEx function.</param>
	/// <param name="lpFindData">
	///     A pointer to the WIN32_FIND_DATA structure that receives information about the found file or subdirectory. The
	///     structure can be used in subsequent calls
	///     to FindNextFile to indicate from which file to continue the search.
	/// </param>
	/// <returns>
	///     If the function succeeds, the return value is nonzero and the lpFindFileData parameter contains information about
	///     the next file or directory found. If the function fails,
	///     the return value is zero and the contents of lpFindFileData are indeterminate.
	/// </returns>
	/// <see cref="http://msdn.microsoft.com/en-us/Library/aa364428%28VS.85%29.aspx" />
	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false )]
	public static extern Boolean FindNextFile( SafeSearchHandle hFindFile, out Win32FindData lpFindData );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
	public static extern Boolean FindNextFile( SafeFindHandle hFindFile, out WIN32_FIND_DATAW lpFindFileData );

	[DllImport( "kernel32", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean FindNextFile( IntPtr hFindFile, out Win32FindData lpFindFileData );

	[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr FindWindow( String cls, String win );

	[DllImport( "user32.dll", ThrowOnUnmappableChar = true, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr FindWindowEx( IntPtr hwndParent, IntPtr hwndChildAfter, [MarshalAs( UnmanagedType.LPWStr )] String lpszClass, String lpszWindow );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
	public static extern Boolean FreeConsole();

	[DllImport( "iphlpapi.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 GetAdaptersInfo( IntPtr pAdapterInfo, ref Int64 pBufOutLen );

	[DllImport( "iphlpapi.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 GetBestInterface( UInt32 destAddr, out UInt32 bestIfIndex );

	/// <summary>
	///     <para>
	///         Retrieves the actual number of bytes of disk storage used to store a specified file as a transacted
	///         operation.
	///     </para>
	///     <para>
	///         If the file is located on a volume that supports compression and the file is compressed, the value obtained
	///         is the compressed size of the specified file.
	///     </para>
	///     <para>
	///         If the file is located on a volume that supports sparse files and the file is a sparse file, the value
	///         obtained is the sparse size of the specified file.
	///     </para>
	/// </summary>
	/// <param name="lpFileName">    </param>
	/// <param name="lpFileSizeHigh"></param>
	/// <see cref="http://msdn.microsoft.com/en-us/Library/windows/desktop/aa364930(v=vs.85).aspx" />
	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern UInt32 GetCompressedFileSizeW(
		[In] [MarshalAs( UnmanagedType.LPWStr )] String lpFileName,
		[Out] [MarshalAs( UnmanagedType.U4 )] out UInt32 lpFileSizeHigh
	);

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr GetCurrentThread();

	[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr GetDC( IntPtr hWnd );

	public static IntPtr GetDesktopHandle() {
		var desktop = GetDesktopWindow();
		var progMan = FindWindowEx( desktop, IntPtr.Zero, "Progman", "Program Manager" );
		var defView = FindWindowEx( progMan, IntPtr.Zero, "SHELLDLL_DefView", String.Empty );

		//var listView = FindWindowEx( defView, IntPtr.Zero, "SysListView32", "FolderView" );

		return defView;
	}

	[DllImport( "user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr GetDesktopWindow();

	[DllImport( "gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean GetDeviceGammaRamp( IntPtr hDC, ref GraphicsExtensions.RAMP lpRamp );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true )]
	public static extern UInt32 GetDiskFreeSpaceW(
		[In] [MarshalAs( UnmanagedType.LPWStr )] String lpRootPathName,
		out UInt32 lpSectorsPerCluster,
		out UInt32 lpBytesPerSector,
		out UInt32 lpNumberOfFreeClusters,
		out UInt32 lpTotalNumberOfClusters
	);

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 GetFileAttributesW( String lpFileName );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean GetFileInformationByHandleEx( IntPtr hFile, FILE_INFO_BY_HANDLE_CLASS infoClass, out FILE_ID_BOTH_DIR_INFO dirInfo, UInt32 dwBufferSize );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean GetFileTime( SafeFileHandle hFile, ref Int64 lpCreationTime, ref Int64 lpLastAccessTime, ref Int64 lpLastWriteTime );

	[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
	public static extern IntPtr GetForegroundWindow();

	/// <summary>https://msdn.microsoft.com/en-us/library/windows/desktop/aa364963.aspx</summary>
	/// <param name="lpFileName"></param>
	/// <param name="nBufferLength"></param>
	/// <param name="lpBuffer"></param>
	/// <param name="lpFilePart"></param>
	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
	public static extern UInt32 GetFullPathNameW( String lpFileName, UInt32 nBufferLength, StringBuilder lpBuffer, IntPtr lpFilePart );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr GetProcessHeap();

	[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr GetSystemMenu( IntPtr hwndValue, Boolean isRevert );

	[DllImport( "Kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern void GetSystemTimePreciseAsFileTime( out Int64 filetime );

	[DllImport( "kernel32", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean GetVolumeNameForVolumeMountPoint(
		[MarshalAs( UnmanagedType.LPWStr )] String volumeName,
		[MarshalAs( UnmanagedType.LPWStr )] StringBuilder uniqueVolumeName,
		UInt32 uniqueNameBufferCapacity
	);

	[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr GetWindowDC( IntPtr hwnd );

	public static void HandleLastError( String fullPath ) {
		var lastWin32Error = Marshal.GetLastWin32Error();

		HandleLastError( fullPath, lastWin32Error );
	}

	[DoesNotReturn]
	public static void HandleLastError( String fullPath, Int32 lastWin32Error ) {
		switch ( lastWin32Error ) {
			case ( Int32 ) ErrorCodes.ERROR_FILE_NOT_FOUND: {
				ThrowFileNotFound( fullPath );

				break;
			}

			case ( Int32 ) ErrorCodes.ERROR_PATH_NOT_FOUND: {
				ThrowPathNotFound( fullPath );

				break;
			}

			case ( Int32 ) ErrorCodes.ERROR_ACCESS_DENIED: {
				ThrowAccessDenied( fullPath );

				break;
			}

			default: {
				ThrowExceptionForHR( lastWin32Error );

				break;
			}
		}

		throw new Exception( "wtf" );
	}

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr HeapAlloc( IntPtr hHeap, HeapFlags dwFlags, UInt32 dwSize );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr HeapCreate( UInt32 flOptions, UIntPtr dwInitialSize, UIntPtr dwMaximumSize );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr HeapCreate( HeapFlags flOptions, UInt64 dwInitialsize, UInt64 dwMaximumSize );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean HeapDestroy( IntPtr hHeap );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean HeapFree( IntPtr hHeap, HeapFlags dwFlags, IntPtr lpMem );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.Winapi )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern Boolean IsWow64Process( [In] IntPtr process, [Out] out Boolean wow64Process );

	[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean LockWorkStation();

	[DllImport( "advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true )]
	public static extern Int32 LogonUser( String lpszUserName, String lpszDomain, String lpszPassword, Int32 dwLogonType, Int32 dwLogonProvider, ref IntPtr phToken );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr MapViewOfFile( IntPtr hFileMappingObject, Int32 dwDesiredAccess, Int32 dwFileOffsetHigh, Int32 dwFileOffsetLow, IntPtr dwNumBytesToMap );

	[DllImport( "winmm.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int64 mciSendString( String strCommand, StringBuilder strReturn, Int32 iReturnLength, IntPtr hwndCallback );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean MoveFileW( String lpExistingFileName, String lpNewFileName );

	[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
	public static extern Boolean MoveFileWithProgress(
		[MarshalAs( UnmanagedType.LPWStr )] String lpExistingFileName,
		String lpNewFileName,
		CopyProgressRoutine lpProgressRoutine,
		IntPtr lpData,
		MoveFileFlags dwFlags
	);

	/// <summary>
	///     Netapi32.dll : The NetApiBufferFree function frees the memory that the NetApiBufferAllocate function allocates.
	///     Call NetApiBufferFree to free the memory that other
	///     network management functions return.
	/// </summary>
	[DllImport( "netapi32.dll", EntryPoint = "NetApiBufferFree", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 NetApiBufferFree( IntPtr buffer );

	/// <summary>The NetServerEnum function lists all servers of the specified type that are visible in a domain.</summary>
	/// <param name="servername">  </param>
	/// <param name="level">       </param>
	/// <param name="bufptr">      </param>
	/// <param name="prefmaxlen">  </param>
	/// <param name="entriesread"> </param>
	/// <param name="totalentries"></param>
	/// <param name="servertype">  </param>
	/// <param name="domain">      </param>
	/// <param name="resumeHandle"></param>
	/// <see cref="http://www.pinvoke.net/default.aspx/netapi32.netserverenum" />
	[DllImport( "netapi32.dll", EntryPoint = "NetServerEnum", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 NetServerEnum(
		[MarshalAs( UnmanagedType.LPWStr )] String servername,
		Int32 level,
		out IntPtr bufptr,
		Int32 prefmaxlen,
		ref Int32 entriesread,
		ref Int32 totalentries,
		Sv101Types servertype,
		[MarshalAs( UnmanagedType.LPWStr )] String domain,
		IntPtr resumeHandle
	);

	[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
	public static extern IntPtr OpenFileMapping( Int32 dwDesiredAccess, Boolean bInheritHandle, [MarshalAs( UnmanagedType.LPWStr )] String lpName );

	[DllImport( "shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean PathCompactPathEx( [MarshalAs( UnmanagedType.LPWStr )] [Out] StringBuilder pszOut, String szPath, Int32 cchMax, Int32 dwFlags );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean QueryPerformanceCounter( out Int64 value );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean QueryPerformanceFrequency( out Int64 value );

	[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 ReleaseDC( IntPtr hwnd, IntPtr dc );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean RemoveDirectory( String path );

	[DllImport( "advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean RevertToSelf();

	[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr SendMessage( IntPtr hWnd, Int32 msg, IntPtr wp, IntPtr lp );

	/// <summary>
	///     <code>
	/// Process process = Process.Start("notepad");
	/// Icon icon = new Icon( @"C:\Icons\FilePath.ico" );
	/// process.WaitForInputIdle();
	/// SendMessage( process.MainWindowHandle, WM_SETICON, ICON_BIG, icon.Handle);
	/// </code>
	/// </summary>
	/// <param name="hwnd"></param>
	/// <param name="message"></param>
	/// <param name="iconSize"></param>
	/// <param name="iconHandle"></param>
	[DllImport( "user32.dll", ExactSpelling = false )]
	public static extern IntPtr SendMessage( this IntPtr hwnd, Int32 message, IconSize iconSize, IntPtr iconHandle );

	[DllImport( "User32.Dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int64 SetCursorPos( Int32 x, Int32 y );

	[DllImport( "gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean SetDeviceGammaRamp( IntPtr hDC, ref GraphicsExtensions.RAMP lpRamp );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false )]
	public static extern Boolean SetFileAttributes( String fileName, UInt32 dwFileAttributes );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 SetFileAttributesW( String lpFileName, Int32 fileAttributes );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean SetFileTime( SafeFileHandle hFile, ref Int64 lpCreationTime, ref Int64 lpLastAccessTime, ref Int64 lpLastWriteTime );

	/// <summary>
	/// </summary>
	/// <param name="hThread">             </param>
	/// <param name="dwThreadAffinityMask"></param>
	/// <example>SetThreadAffinityMask( GetCurrentThread(), new IntPtr( 1 &lt;&lt; processor ) );</example>
	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr SetThreadAffinityMask( IntPtr hThread, IntPtr dwThreadAffinityMask );

	[DllImport( "setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern UInt32 SetupDiDestroyDeviceInfoList( IntPtr deviceInfoSet );

	[DllImport( "setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode )]
	public static extern Boolean SetupDiEnumDeviceInterfaces(
		IntPtr deviceInfoSet,
		SP_DEVINFO_DATA deviceInfoData,
		ref Guid interfaceClassGuid,
		Int32 memberIndex,
		SP_DEVICE_INTERFACE_DATA deviceInterfaceData
	);

	[DllImport( "setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean SetupDiEnumDeviceInterfaces(
		IntPtr hDevInfo,
		ref SP_DEVINFO_DATA devInfo,
		ref Guid interfaceClassGuid,
		UInt32 memberIndex,
		ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
	);

	[DllImport( "setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true )]
	public static extern IntPtr SetupDiGetClassDevs( ref Guid classGuid, [MarshalAs( UnmanagedType.LPTStr )] String enumerator, IntPtr hwndParent, UInt32 flags );

	[DllImport( "setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode )]
	public static extern Boolean SetupDiGetDeviceInterfaceDetail(
		IntPtr deviceInfoSet,
		SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
		IntPtr deviceInterfaceDetailData,
		Int32 deviceInterfaceDetailDataSize,
		ref Int32 requiredSize,
		SP_DEVINFO_DATA deviceInfoData
	);

	[DllImport( "setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean SetupDiGetDeviceRegistryProperty(
		IntPtr deviceInfoSet,
		ref SP_DEVINFO_DATA deviceInfoData,
		UInt32 property,
		out UInt32 propertyRegDataType,
		Byte[] propertyBuffer,
		UInt32 propertyBufferSize,
		out UInt32 requiredSize
	);

	[DllImport( "setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true )]
	public static extern Boolean SetupDiOpenDeviceInfo(
		IntPtr deviceInfoSet,
		[MarshalAs( UnmanagedType.LPWStr )] String deviceInstanceId,
		IntPtr hwndParent,
		Int32 openFlags,
		SP_DEVINFO_DATA deviceInfoData
	);

	/// <summary>
	///     <code>
	/// Process process = Process.Start("notepad");
	/// process.WaitForInputIdle();
	/// SetWindowText(process.MainWindowHandle, "Hello!");
	/// </code>
	/// </summary>
	[DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false )]
	public static extern Boolean SetWindowText( this IntPtr hwnd, String lpString );

	/// <summary>Throw a useful <see cref="IOException" />.</summary>
	[DebuggerStepThrough]
	[DoesNotReturn]
	public static void ThrowAccessDenied( String fullPath ) => throw new IOException( $"Access denied to file \"{fullPath}\"." );

	[DebuggerStepThrough]
	public static void ThrowExceptionForHR( Int32 errorCode ) => Marshal.ThrowExceptionForHR( errorCode, NegativeOneIntPtr );

	/// <summary>Throw a useful <see cref="FileNotFoundException" />.</summary>
	/// <exception cref="FileNotFoundException"></exception>
	[DebuggerStepThrough]
	[DoesNotReturn]
	public static void ThrowFileNotFound( String fullPath ) => throw new FileNotFoundException( $"The file \"{fullPath}\" was not found.", fullPath );

	/// <summary>Throw a useful <see cref="DirectoryNotFoundException" />.</summary>
	[DebuggerStepThrough]
	[DoesNotReturn]
	public static void ThrowPathNotFound( String fullPath ) => throw new DirectoryNotFoundException( $"The path for file \"{fullPath}\" was not found." );

	[DebuggerStepThrough]
	public static DateTime ToDateTime( this Filetime time ) {
		try {
			var high = ( UInt64 ) time.dwHighDateTime;
			var low = time.dwLowDateTime;
			var fileTime = ( Int64 ) ( ( high << 32 ) + low );

			return DateTime.FromFileTimeUtc( fileTime );
		}
		catch ( ArgumentOutOfRangeException ) {
			return DateTime.FromFileTimeUtc( 0xFFFFFFFF ); //shouldn't this actually be null or something?
		}
	}

	[DebuggerStepThrough]
	public static DateTime? ToDateTime( this FILETIME time ) {
		try {
			var high = ( UInt64 ) time.dwHighDateTime;
			var low = ( UInt32 ) time.dwLowDateTime;
			var fileTime = ( Int64 ) ( ( high << 32 ) + low );

			return DateTime.FromFileTimeUtc( fileTime );
		}
		catch ( ArgumentOutOfRangeException ) {
			//return DateTime.FromFileTimeUtc( 0xFFFFFFFF ); //shouldn't this actually be null or something?
			return default( DateTime? );
		}
	}

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean UnmapViewOfFile( IntPtr lpBaseAddress );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern IntPtr VirtualAlloc( IntPtr lpAddress, UIntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect );

	[DllImport( "kernel32", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean VirtualFree( IntPtr lpAddress, UInt32 dwSize, UInt32 dwFreeType );

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Boolean VirtualFree( UIntPtr lpAddress, UIntPtr dwSize, UInt32 dwFreeType );

	/*
	[DllImport( "mpr.dll", BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true, CharSet = CharSet.Unicode )]
	public static extern Int32 WNetAddConnection2( NetResource netResource, [MarshalAs( UnmanagedType.LPWStr )] String password,
		[MarshalAs( UnmanagedType.LPWStr )] String username, Int32 flags );
	*/

	/*
	/// <summary>this must be used if NETRESOURCE is defined as a struct???</summary>
	/// <param name="netResource"></param>
	/// <param name="password">   </param>
	/// <param name="username">   </param>
	/// <param name="flags">      </param>
	/// <returns></returns>
	[DllImport( "mpr.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 WNetAddConnection2( ref NetResource netResource, [MarshalAs( UnmanagedType.LPWStr )] String password,
		[MarshalAs( UnmanagedType.LPWStr )] String username, UInt32 flags );
	*/

	[DllImport( "mpr.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern Int32 WNetCancelConnection2( [MarshalAs( UnmanagedType.LPWStr )] String name, Int32 flags, Boolean force );

	/*
	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
	internal struct WIN32_FIND_DATA {
		public System.IO.FileAttributes dwFileAttributes;
		public FILETIME ftCreationTime;
		public FILETIME ftLastAccessTime;
		public FILETIME ftLastWriteTime;
		public UInt32 nFileSizeHigh; //changed all to uint, otherwise you run into unexpected overflow
		public UInt32 nFileSizeLow;  //|
		public UInt32 dwReserved0;   //|
		public UInt32 dwReserved1;   //v
		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = MAX_PATH )]
		public String cFileName;
		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE )]
		public String cAlternate;
	}
	*/

	[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
	public static extern IntPtr CreateFile(
		String lpFileName,
		JunctionPoint.EFileAccess dwDesiredAccess,
		JunctionPoint.EFileShare dwShareMode,
		IntPtr lpSecurityAttributes,
		JunctionPoint.ECreationDisposition dwCreationDisposition,
		JunctionPoint.EFileAttributes dwFlagsAndAttributes,
		IntPtr hTemplateFile
	);

	[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
	public static extern IntPtr CreateFile(
		String lpFileName,
		UInt32 dwDesiredAccess,
		UInt32 dwShareMode,
		IntPtr lpSecurityAttributes,
		UInt32 dwCreationDisposition,
		UInt32 dwFlagsAndAttributes,
		IntPtr hTemplateFile
	);

	[DllImport( "kernel32.dll", SetLastError = true )]
	public static extern Boolean FlushViewOfFile( IntPtr lpBaseAddress, IntPtr dwNumBytesToFlush );

	[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
	public static extern Int32 FormatMessage(
		UInt32 dwFlags,
		IntPtr lpSource,
		UInt32 dwMessageId,
		UInt32 dwLanguageId,
		StringBuilder lpBuffer,
		UInt32 nSize,
		IntPtr arguments
	);

	public static String GetErrorMessage( Int32 code ) {
		var message = new StringBuilder( 255 );

#pragma warning disable CA1806 // Do not ignore method results
		FormatMessage( FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, ( UInt32 ) code, 0, message, ( UInt32 ) message.Capacity, IntPtr.Zero );
#pragma warning restore CA1806 // Do not ignore method results

		return message.ToString();
	}

	[SecurityCritical]
	public class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid {

		[SecurityCritical]
		public SafeFindHandle() : base( true ) { }

		[SecurityCritical]
		protected override Boolean ReleaseHandle() => FindClose( this.handle );

	}

	public interface IHandle {

		/// <summary>Returns the value of the handle field.</summary>
		/// <returns>An IntPtr representing the value of the handle field.</returns>
		IntPtr DangerousGetHandle();

	}

	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
	public struct WIN32_FIND_DATAW {

		public FileAttributes dwFileAttributes;

		internal FILETIME ftCreationTime;

		internal FILETIME ftLastAccessTime;

		internal FILETIME ftLastWriteTime;

		public UInt32 nFileSizeHigh;

		public UInt32 nFileSizeLow;

		public UInt32 dwReserved0;

		public UInt32 dwReserved1;

		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
		public String cFileName;

		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 14 )]
		public String cAlternateFileName;

	}

	[StructLayout( LayoutKind.Sequential )]
	public struct ATA_PASS_THROUGH_EX {

		public UInt16 Length;

		public UInt16 AtaFlags;

		public readonly Byte PathId;

		public readonly Byte TargetId;

		public readonly Byte Lun;

		public readonly Byte ReservedAsUchar;

		public UInt32 DataTransferLength;

		public UInt32 TimeOutValue;

		public readonly UInt32 ReservedAsUlong;

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
	public readonly struct DEVICE_SEEK_PENALTY_DESCRIPTOR {

		public readonly UInt32 Version;

		public readonly UInt32 Size;

		[MarshalAs( UnmanagedType.U1 )]
		public readonly Boolean IncursSeekPenalty;

	}

	[StructLayout( LayoutKind.Sequential )]
	public readonly struct DISK_EXTENT {

		public readonly Int32 DiskNumber;

		public readonly Int64 StartingOffset;

		public readonly Int64 ExtentLength;

	}

	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
	public struct FILE_ID_BOTH_DIR_INFO {

		public readonly UInt32 NextEntryOffset;

		public readonly UInt32 FileIndex;

		public LargeInteger CreationTime;

		public LargeInteger LastAccessTime;

		public LargeInteger LastWriteTime;

		public LargeInteger ChangeTime;

		public LargeInteger EndOfFile;

		public LargeInteger AllocationSize;

		public readonly UInt32 FileAttributes;

		public readonly UInt32 FileNameLength;

		public readonly UInt32 EaSize;

		public readonly Char ShortNameLength;

		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 12 )]
		public readonly String ShortName;

		public LargeInteger FileId;

		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 1 )]
		public readonly String FileName;

	}

	/// <summary>
	///     Win32 FILETIME structure. The win32 documentation says: "Contains a 64-bit value representing the number of
	///     100-nanosecond intervals since January 1, 1601 (UTC)."
	/// </summary>
	/// <see cref="http://msdn.microsoft.com/en-us/Library/ms724284%28VS.85%29.aspx" />
	[StructLayout( LayoutKind.Sequential )]
	public readonly struct Filetime {

		public readonly UInt32 dwLowDateTime;

		public readonly UInt32 dwHighDateTime;

	}

	[StructLayout( LayoutKind.Explicit, Pack = 0 )]
	public struct LargeInteger {

		[FieldOffset( 0 )]
		public Int32 Low;

		[FieldOffset( 4 )]
		public Int32 High;

		[FieldOffset( 0 )]
		public readonly Int64 QuadPart;

		/// <summary>use only when QuadPart cannot be passed</summary>
		public Int64 ToInt64() => ( ( Int64 ) this.High << 32 ) | ( UInt32 ) this.Low;

		// just for demonstration
		public static LargeInteger FromInt64( Int64 value ) =>
			new() {
				Low = ( Int32 ) value,
				High = ( Int32 ) ( value >> 32 )
			};

	}

	[StructLayout( LayoutKind.Sequential )]
	public readonly struct ServerInfo101 {

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

	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
	public struct SP_DEVICE_INTERFACE_DATA {

		public UInt32 cbSize;

		public readonly UInt32 Flags;

		public Guid InterfaceClassGuid;

		private readonly IntPtr Reserved;

	}

	[StructLayout( LayoutKind.Sequential, Pack = 2 )]
	public struct SP_DEVICE_INTERFACE_DETAIL_DATA {

		public Int32 cbSize;

		public readonly Int16 devicePath;

	}

	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
	public struct SP_DEVINFO_DATA {

		public UInt32 cbSize;

		public Guid classGuid;

		public readonly UInt32 devInst;

		public IntPtr reserved;

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
		public readonly Byte[] AdditionalParameters;

	}

	[StructLayout( LayoutKind.Sequential )]
	public struct WIN32_FILE_ATTRIBUTE_DATA {

		public FileAttributes dwFileAttributes;

		public Filetime ftCreationTime;

		public Filetime ftLastAccessTime;

		public Filetime ftLastWriteTime;

		public UInt32 nFileSizeHigh;

		public UInt32 nFileSizeLow;

		[SecurityCritical]
		public void PopulateFrom( ref Win32FindData findData ) {
			this.dwFileAttributes = findData.dwFileAttributes;
			this.ftCreationTime = findData.ftCreationTime;
			this.ftLastAccessTime = findData.ftLastAccessTime;
			this.ftLastWriteTime = findData.ftLastWriteTime;
			this.nFileSizeHigh = findData.nFileSizeHigh;
			this.nFileSizeLow = findData.nFileSizeLow;
		}

	}

	/// <summary>
	///     The Win32 find data structure. The documentation says: "Contains information about the file that is found by the
	///     FindFirstFile, FindFirstFileEx, or FindNextFile
	///     function."
	/// </summary>
	/// <see cref="http://msdn.microsoft.com/en-us/Library/aa365740%28VS.85%29.aspx" />
	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
	public struct Win32FindData {

		public readonly FileAttributes dwFileAttributes;

		public Filetime ftCreationTime;

		public Filetime ftLastAccessTime;

		public Filetime ftLastWriteTime;

		public readonly UInt32 nFileSizeHigh;

		public readonly UInt32 nFileSizeLow;

		public readonly UInt32 dwReserved0;

		public readonly UInt32 dwReserved1;

		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = MaxPath )]
		public readonly String cFileName;

		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 14 )]
		public readonly String cAlternateFileName;

	}

	/// <summary>Base class for all native handles.</summary>
	/// <seealso cref="SafeHandleZeroOrMinusOneIsInvalid" />
	/// <seealso cref="IEquatable{T}" />
	/// <seealso cref="IHandle" />
	public class HANDLE : SafeHandleZeroOrMinusOneIsInvalid, IEquatable<HANDLE>, IHandle {

		/// <summary>Initializes a new instance of the <see cref="HANDLE" /> class and assigns an existing handle.</summary>
		/// <param name="preexistingHandle">An <see cref="IntPtr" /> object that represents the pre-existing handle to use.</param>
		/// <param name="ownsHandle">
		///     <see langword="true" /> to reliably release the handle during the finalization phase;
		///     otherwise, <see langword="false" /> (not recommended).
		/// </param>
		protected HANDLE( IntPtr preexistingHandle, Boolean ownsHandle = true ) : base( ownsHandle ) => this.SetHandle( preexistingHandle );

		[DebuggerStepThrough]
		protected HANDLE() : base( true ) { }

		/// <summary>Gets a value indicating whether this instance is null.</summary>
		/// <value><c>true</c> if this instance is null; otherwise, <c>false</c>.</value>
		protected Boolean IsNull => this.handle == IntPtr.Zero;

		/// <summary>Determines whether the specified <see cref="HANDLE" />, is equal to this instance.</summary>
		/// <param name="other">The <see cref="HANDLE" /> to compare with this instance.</param>
		/// <returns><c>true</c> if the specified <see cref="HANDLE" /> is equal to this instance; otherwise, <c>false</c>.</returns>
		public Boolean Equals( HANDLE? other ) => Equals( this, other );

		/// <summary>
		///     Internal method that actually releases the handle. this is called by <see cref="ReleaseHandle" /> for valid
		///     handles and afterwards zeros the handle.
		/// </summary>
		/// <returns><c>true</c> to indicate successful release of the handle; <c>false</c> otherwise.</returns>
		protected virtual Boolean InternalReleaseHandle() => true;

		/// <inheritdoc />
		protected override Boolean ReleaseHandle() {
			if ( this.IsInvalid ) {
				return true;
			}

			if ( !this.InternalReleaseHandle() ) {
				return false;
			}

			this.handle = IntPtr.Zero;

			return true;
		}

		/// <summary>
		///     <para>Static compirson method.</para>
		///     <para>True if <paramref name="left" /> and <paramref name="right" /> are the same instance.</para>
		///     <para>False if either <see cref="HANDLE" /> is default (null).</para>
		///     <para>True if handles are equal, false otherwise.</para>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public static Boolean Equals( HANDLE? left, HANDLE? right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return false;
			}

			return left.handle == right.handle;
		}

		/// <summary>Implements the operator !=.</summary>
		/// <param name="h1">The first handle.</param>
		/// <param name="h2">The second handle.</param>
		/// <returns>The result of the operator.</returns>
		public static Boolean operator !=( HANDLE? h1, HANDLE? h2 ) => !( h1 == h2 );

		/// <summary>Implements the operator ==.</summary>
		/// <param name="h1">The first handle.</param>
		/// <param name="h2">The second handle.</param>
		/// <returns>The result of the operator.</returns>
		public static Boolean operator ==( HANDLE? h1, HANDLE? h2 ) => h1 is not null && h2 is not null && h1.Equals( h2 );

		/// <summary>Determines whether the specified <see cref="Object" />, is equal to this instance.</summary>
		/// <param name="obj">The <see cref="Object" /> to compare with this instance.</param>
		/// <returns><c>true</c> if the specified <see cref="Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
		public override Boolean Equals( Object? obj ) => Equals( this, obj as HANDLE );

		// ReSharper disable once NonReadonlyMemberInGetHashCode
		public override Int32 GetHashCode() => this.handle.GetHashCode();

	}

	/// <summary>
	///     Represents a self-closing file search handle opened by the FindFirstFile, FindFirstFileEx, FindFirstFileNameW,
	///     FindFirstFileNameTransactedW, FindFirstFileTransacted,
	///     FindFirstStreamTransactedW, or FindFirstStreamW functions.
	/// </summary>
	public class SafeSearchHandle : HANDLE {

		[DebuggerStepThrough]
		private SafeSearchHandle() { }

		/// <summary>Initializes a new instance of the <see cref="SafeSearchHandle" /> class.</summary>
		/// <param name="handle">The handle.</param>
		public SafeSearchHandle( IntPtr handle ) : base( handle ) { }

		/// <inheritdoc />
		protected override Boolean InternalReleaseHandle() {
			if ( this.IsClosed || this.IsInvalid || this.IsNull ) {
				return true;
			}

			return FindClose( this );
		}

	}

}