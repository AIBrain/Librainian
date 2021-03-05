// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "NativeMethods.cs" last formatted on 2020-08-14 at 8:39 PM.

namespace Librainian.FileSystem.Pri.LongPath {

	using System;
	using System.IO;
	
	using System.Runtime.InteropServices;
	using System.Text;
	using JetBrains.Annotations;
	using Microsoft.Win32.SafeHandles;
	using DWORD = System.UInt32;

	public static class NativeMethods {

		public delegate CopyProgressResult CopyProgressRoutine(
			Int64 TotalFileSize,
			Int64 TotalBytesTransferred,
			Int64 StreamSize,
			Int64 StreamBytesTransferred,
			DWORD dwStreamNumber,
			CopyProgressCallbackReason dwCallbackReason,
			IntPtr hSourceFile,
			IntPtr hDestinationFile,
			IntPtr lpData
		);

		public enum CopyProgressCallbackReason : DWORD {

			CALLBACK_CHUNK_FINISHED = 0x00000000,
			CALLBACK_STREAM_SWITCH = 0x00000001

		}

		public enum CopyProgressResult : DWORD {

			PROGRESS_CONTINUE = 0,
			PROGRESS_CANCEL = 1,
			PROGRESS_STOP = 2,
			PROGRESS_QUIET = 3

		}

		[Flags]
		public enum EFileAccess : DWORD {

			GenericRead = 0x80000000,
			GenericWrite = 0x40000000,
			GenericExecute = 0x20000000,
			GenericAll = 0x10000000

		}

		[Flags]
		public enum MoveFileFlags : DWORD {

			MOVE_FILE_REPLACE_EXISTSING = 0x00000001,
			MOVE_FILE_COPY_ALLOWED = 0x00000002,
			MOVE_FILE_DELAY_UNTIL_REBOOT = 0x00000004,
			MOVE_FILE_WRITE_THROUGH = 0x00000008,
			MOVE_FILE_CREATE_HARDLINK = 0x00000010,
			MOVE_FILE_FAIL_IF_NOT_TRACKABLE = 0x00000020

		}

		public enum SecurityImpersonationLevel {

			Anonymous = 0,
			Identification = 1,
			Impersonation = 2,
			Delegation = 3

		}

		public enum TokenType {

			Primary = 1,
			Impersonation = 2

		}

		public enum ERROR : Int32 {
			ERROR_SUCCESS = 0,
			ERROR_ACCESS_DENIED = 0x5,




			ERROR_ALREADY_EXISTS = 0xB7,

			ERROR_BAD_NETPATH = 0x35,

			ERROR_BAD_PATHNAME = 0xA1,

			ERROR_CANT_OPEN_ANONYMOUS = 0x543,

			ERROR_DIRECTORY = 0x10B,

			ERROR_FILE_EXISTS = 0x50,

			ERROR_FILE_NOT_FOUND = 0x2,

			ERROR_FILENAME_EXCED_RANGE = 0xCE,

			ERROR_INVALID_DRIVE = 0xf,

			ERROR_INVALID_HANDLE = 0x6,

			ERROR_INVALID_NAME = 0x7B,

			ERROR_INVALID_OWNER = 0x51B,

			ERROR_INVALID_PARAMETER = 0x57,

			ERROR_INVALID_PRIMARY_GROUP = 0x51C,

			ERROR_LOGON_FAILURE = 0x52E,

			ERROR_NETNAME_DELETED = 0x40,

			ERROR_NO_MORE_FILES = 0x12,

			ERROR_NO_SECURITY_ON_OBJECT = 0x546,

			ERROR_NO_SUCH_PRIVILEGE = 0x521,

			ERROR_NO_TOKEN = 0x3f0,

			ERROR_NOT_ALL_ASSIGNED = 0x514,

			ERROR_NOT_ENOUGH_MEMORY = 0x8,

			ERROR_NOT_READY = 0x15,

			// filename too long.
			ERROR_OPERATION_ABORTED = 0x3e3,

			ERROR_PATH_NOT_FOUND = 0x3,

			ERROR_PRIVILEGE_NOT_HELD = 0x522,

			ERROR_SHARING_VIOLATION = 0x20,

		}

		public const Int32 FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

		//success starts with ERROR? lol
		public const Int32 FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

		public const Int32 FILE_WRITE_ATTRIBUTES = 0x0100;

		public const Int32 FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000;

		public const Int32 FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

		public const Int32 FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;

		public const Int32 INVALID_FILE_ATTRIBUTES = -1;

		public const Int32 MAX_ALTERNATE = 14;

		/// <summary>
		///     While Windows allows larger paths up to a maximum of 32767 characters, because this is only an approximation and
		///     can vary across systems and OS versions, we choose a limit well under so that we can give a consistent behavior.
		/// </summary>
		public const Int32 MAX_LONG_PATH = 32000;

		public const Int32 MAX_PATH = 260;

		public const Int32 REPLACEFILE_IGNORE_MERGE_ERRORS = 0x2;

		public const Int32 REPLACEFILE_WRITE_THROUGH = 0x1;

		public const DWORD SE_PRIVILEGE_DISABLED = 0x00000000;

		public const DWORD SE_PRIVILEGE_ENABLED = 0x00000002;

		private static readonly Version ThreadErrorModeMinOsVersion = new( 6, 1, 7600 );

		[DllImport( DLL.kernel32, BestFitMapping = false, CharSet = CharSet.None, EntryPoint = "SetErrorMode", ExactSpelling = true )]
		private static extern Int32 SetErrorMode_VistaAndOlder( Int32 newMode );

		[DllImport( DLL.kernel32, BestFitMapping = false, CharSet = CharSet.None, EntryPoint = "SetThreadErrorMode", ExactSpelling = false, SetLastError = true )]
		private static extern Boolean SetErrorMode_Win7AndNewer( Int32 newMode, out Int32 oldMode );

		[DllImport( DLL.advapi32, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true )]
		public static extern Boolean AdjustTokenPrivileges(
			[In] SafeTokenHandle TokenHandle,
			[In] Boolean DisableAllPrivileges,
			[In] ref TOKEN_PRIVILEGE NewState,
			[In] DWORD BufferLength,
			[In] [Out] ref TOKEN_PRIVILEGE PreviousState,
			[In] [Out] ref DWORD ReturnLength
		);

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true )]
		public static extern Boolean CloseHandle( IntPtr handle );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern Boolean CopyFile( [NotNull] String src, [NotNull] String dst, [MarshalAs( UnmanagedType.Bool )] Boolean failIfExists );

		[DllImport( DLL.coredll, BestFitMapping = false, SetLastError = true )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern Boolean CopyFileEx(
			[NotNull] String lpExistingFileName,
			[NotNull] String lpNewFileName,
			[CanBeNull] CopyProgressRoutine lpProgressRoutine,
			IntPtr lpData,
			ref Int32 IsCancelled,
			MoveFileFlags dwCopyFlags
		);

		/// <summary>Default handler for <see cref="XCopy" />.</summary>
		/// <param name="total"></param>
		/// <param name="transferred"></param>
		/// <param name="streamSize"></param>
		/// <param name="StreamByteTrans"></param>
		/// <param name="dwStreamNumber"></param>
		/// <param name="reason"></param>
		/// <param name="hSourceFile"></param>
		/// <param name="hDestinationFile"></param>
		/// <param name="lpData"></param>
		/// <returns></returns>
		public static CopyProgressResult CopyProgressHandler(
			Int64 total,
			Int64 transferred,
			Int64 streamSize,
			Int64 StreamByteTrans,
			DWORD dwStreamNumber,
			CopyProgressCallbackReason reason,
			IntPtr hSourceFile,
			IntPtr hDestinationFile,
			IntPtr lpData
		) =>
			CopyProgressResult.PROGRESS_CONTINUE;

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern Boolean CreateDirectory( [NotNull] this String lpPathName, IntPtr lpSecurityAttributes );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		public static extern SafeFileHandle CreateFile(
			[NotNull] String lpFileName,
			EFileAccess dwDesiredAccess,
			DWORD dwShareMode,
			IntPtr lpSecurityAttributes,
			DWORD dwCreationDisposition,
			DWORD dwFlagsAndAttributes,
			IntPtr hTemplateFile
		);

		[DllImport( DLL.advapi32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Auto )]
		public static extern Boolean DecryptFile( [NotNull] String path, Int32 reservedMustBeZero );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern Boolean DeleteFile( [NotNull] String lpFileName );

		/*
		[DllImport( DLL.advapi32, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true )]
		[ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
		public static extern Boolean DuplicateTokenEx(
			[In] SafeTokenHandle ExistingToken,
			[In] TokenAccessLevels DesiredAccess,
			[In] IntPtr TokenAttributes,
			[In] SecurityImpersonationLevel ImpersonationLevel,
			[In] TokenType TokenType,
			[In] [Out] ref SafeTokenHandle NewToken
		);
		*/

		[DllImport( DLL.advapi32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Auto )]
		public static extern Boolean EncryptFile( [NotNull] String path );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern Boolean FindClose( IntPtr hFindFile );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		public static extern SafeFindHandle FindFirstFile( [NotNull] String lpFileName, out WIN32_FIND_DATA lpFindFileData );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern Boolean FindNextFile( [NotNull] this SafeFindHandle hFindFile, out WIN32_FIND_DATA lpFindFileData );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		public static extern Int32 FormatMessage(
			Int32 dwFlags,
			IntPtr lpSource,
			Int32 dwMessageId,
			Int32 dwLanguageId,
			StringBuilder lpBuffer,
			Int32 nSize,
			IntPtr va_list_arguments
		);

		[DllImport( DLL.kernel32, BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true )]
		public static extern IntPtr GetCurrentProcess();

		[DllImport( DLL.kernel32, BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true )]
		public static extern IntPtr GetCurrentThread();

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		public static extern FileAttributes GetFileAttributes( [NotNull] String lpFileName );

		[DllImport( DLL.kernel32, BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true )]
		public static extern Boolean GetFileAttributesEx( [NotNull] String name, Int32 fileInfoLevel, ref WIN32_FILE_ATTRIBUTE_DATA lpFileInformation );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		public static extern DWORD GetFullPathName( [NotNull] String lpFileName, DWORD nBufferLength, [NotNull] StringBuilder lpBuffer, IntPtr mustBeNull = default );

		/// <summary></summary>
		/// <param name="lpFileName"></param>
		/// <param name="nBufferLength"></param>
		/// <param name="lpBuffer"></param>
		/// <param name="lpFilePart">
		///     <para>
		///         A pointer to a buffer that receives the address (within lpBuffer) of the final file name component in the
		///         path.
		///     </para>
		///     <para>This parameter can be NULL.</para>
		///     <para>If lpBuffer refers to a directory and not a file, lpFilePart receives zero.</para>
		/// </param>
		/// <returns></returns>
		[DllImport( DLL.kernel32, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
		public static extern DWORD GetFullPathNameW( [NotNull] String lpFileName, DWORD nBufferLength, StringBuilder lpBuffer, IntPtr lpFilePart = default );

		[NotNull]
		public static String GetMessage( ERROR errorCode ) {
			var sb = new StringBuilder( 1024 );

			// result is the # of characters copied to the StringBuilder.
			var result = FormatMessage( FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_ARGUMENT_ARRAY, IntPtr.Zero, ( Int32 )errorCode, 0, sb, sb.Capacity,
										IntPtr.Zero );

			return result == 0 ? $"Unknown error: {errorCode}" : sb.ToString();
		}

		[DllImport( DLL.advapi32, BestFitMapping = false, EntryPoint = "GetSecurityDescriptorLength", CallingConvention = CallingConvention.Winapi, SetLastError = true,
					ExactSpelling = true, CharSet = CharSet.Unicode )]
		public static extern DWORD GetSecurityDescriptorLength( IntPtr byteArray );

		[DllImport( DLL.advapi32, BestFitMapping = false, EntryPoint = "GetNamedSecurityInfoW", CallingConvention = CallingConvention.Winapi, SetLastError = true,
					ExactSpelling = true, CharSet = CharSet.Unicode )]
		public static extern DWORD GetSecurityInfoByName(
			[NotNull] String name,
			DWORD objectType,
			DWORD securityInformation,
			out IntPtr sidOwner,
			out IntPtr sidGroup,
			out IntPtr dacl,
			out IntPtr sacl,
			out IntPtr securityDescriptor
		);

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true )]
		public static extern IntPtr LocalFree( IntPtr handle );

		[DllImport( DLL.advapi32, BestFitMapping = false, EntryPoint = "LookupPrivilegeValueW", CharSet = CharSet.Auto, SetLastError = true )]
		public static extern Boolean LookupPrivilegeValue( [In] String lpSystemName, [In] String lpName, [In] [Out] ref LUID Luid );

		public static Int32 MakeHRFromErrorCode( ERROR errorCode ) => unchecked( ( Int32 )0x80070000 | ( Int32 )errorCode );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern Boolean MoveFile( [NotNull] String lpPathNameFrom, [NotNull] String lpPathNameTo );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Auto )]
		public static extern Boolean MoveFileWithProgress(
			[NotNull] String lpExistingFileName,
			[NotNull] String lpNewFileName,
			CopyProgressRoutine lpProgressRoutine,
			IntPtr lpData,
			MoveFileFlags dwCopyFlags
		);

		/*
		[DllImport( DLL.advapi32, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true )]
		[ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
		public static extern Boolean OpenProcessToken( [In] IntPtr ProcessToken, [In] TokenAccessLevels DesiredAccess, [In] [Out] ref SafeTokenHandle TokenHandle );
		*/

		/*
		[DllImport( DLL.advapi32, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true )]
		[ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
		public static extern Boolean OpenThreadToken(
			[In] IntPtr ThreadToken,
			[In] TokenAccessLevels DesiredAccess,
			[In] Boolean OpenAsSelf,
			[In] [Out] ref SafeTokenHandle TokenHandle
		);
		*/

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern Boolean RemoveDirectory( [NotNull] String lpPathName );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Auto )]
		public static extern Boolean ReplaceFile(
			[NotNull] String replacedFileName,
			[NotNull] String replacementFileName,
			[NotNull] String backupFileName,
			Int32 dwReplaceFlags,
			IntPtr lpExclude,
			IntPtr lpReserved
		);

		[DllImport( DLL.advapi32, BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true )]
		public static extern Boolean RevertToSelf();

		[DllImport( DLL.kernel32, BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true )]
		public static extern Boolean SetCurrentDirectory( [NotNull] String path );

		public static Int32 SetErrorMode( Int32 newMode ) {
			if ( Environment.OSVersion.Version < ThreadErrorModeMinOsVersion ) {
				return SetErrorMode_VistaAndOlder( newMode );
			}

			SetErrorMode_Win7AndNewer( newMode, out var num );

			return num;
		}

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true, CharSet = CharSet.Unicode )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern Boolean SetFileAttributes( [NotNull] String lpFileName, [MarshalAs( UnmanagedType.U4 )] FileAttributes dwFileAttributes );

		/// <summary>
		///     Calls <see cref="SetFilePointerEx" />.
		///     <para>Throws <see cref="IOException" /> if unable to set file pointer.</para>
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="origin"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		/// <exception cref="IOException"></exception>
		public static UInt64 SetFilePointer( [CanBeNull] SafeFileHandle handle, SeekOrigin origin, UInt64 offset ) {
			var result = SetFilePointerEx( handle, offset, out var filePointer, origin );

			if ( !result && Marshal.GetLastWin32Error() != 0 ) {
				throw new IOException( $"Error seeking to position {offset}" );
			}

			return filePointer;
		}

		[DllImport( DLL.kernel32, BestFitMapping = false, EntryPoint = "SetFilePointer", SetLastError = true )]
		public static extern Int32 SetFilePointer( SafeFileHandle handle, Int32 distanceToMove, ref Int32 distanceToMoveHigh, Int32 origin );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true )]
		public static extern Boolean SetFilePointerEx( SafeFileHandle hFile, UInt64 distanceToMove, out UInt64 lpNewFilePointer, SeekOrigin dwMoveMethod );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true )]
		public static extern unsafe Boolean SetFileTime( SafeFileHandle hFile, FILE_TIME* creationTime, FILE_TIME* lastAccessTime, FILE_TIME* lastWriteTime );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern Boolean SetFileTime( SafeFileHandle hFile, Int64 creationTime, Int64 lastAccessTime, Int64 lastWriteTime );

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern Boolean SetFileTime( IntPtr hFile, Int64 creationTime, Int64 lastAccessTime, Int64 lastWriteTime );

		[DllImport( DLL.advapi32, BestFitMapping = false, EntryPoint = "SetSecurityInfo", CallingConvention = CallingConvention.Winapi, SetLastError = true,
					ExactSpelling = true, CharSet = CharSet.Unicode )]
		public static extern DWORD SetSecurityInfoByHandle(
			SafeHandle handle,
			DWORD objectType,
			DWORD securityInformation,
			Byte[] owner,
			Byte[] group,
			Byte[] dacl,
			Byte[] sacl
		);

		[DllImport( DLL.advapi32, BestFitMapping = false, EntryPoint = "SetNamedSecurityInfoW", CallingConvention = CallingConvention.Winapi, SetLastError = true,
					ExactSpelling = true, CharSet = CharSet.Unicode )]
		public static extern DWORD SetSecurityInfoByName(
			[NotNull] String name,
			DWORD objectType,
			DWORD securityInformation,
			Byte[] owner,
			Byte[] group,
			Byte[] dacl,
			Byte[] sacl
		);

		[DllImport( DLL.advapi32, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true )]
		public static extern Boolean SetThreadToken( [In] IntPtr Thread, [In] SafeTokenHandle Token );

		/// <summary></summary>
		/// <param name="oldFile"></param>
		/// <param name="newFile"></param>
		/// <param name="progress"></param>
		/// <param name="isCancelled">I believe this becomes 1 if the file copy was cancelled, and 0 otherwise.</param>
		public static void XCopy( [NotNull] this String oldFile, [NotNull] String newFile, [CanBeNull] CopyProgressRoutine? progress, ref Int32 isCancelled ) {
			if ( String.IsNullOrWhiteSpace( oldFile ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( oldFile ) );
			}

			if ( String.IsNullOrWhiteSpace( newFile ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( newFile ) );
			}

			if ( progress is null ) {
				progress = CopyProgressHandler;
			}

			CopyFileEx( oldFile, newFile, progress, IntPtr.Zero, ref isCancelled,
						MoveFileFlags.MOVE_FILE_REPLACE_EXISTSING | MoveFileFlags.MOVE_FILE_WRITE_THROUGH | MoveFileFlags.MOVE_FILE_COPY_ALLOWED );
		}

	}

}