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
// File "JunctionPoint.cs" last formatted on 2020-08-14 at 8:39 PM.

// ReSharper disable All

#nullable enable
namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

	using System;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Text;
	using JetBrains.Annotations;
	using Microsoft.Win32.SafeHandles;

	/// <summary>PRELIMINARY Provides access to NTFS junction points in .Net.</summary>
	public static class JunctionPoint {

		/// <summary>The data present in the reparse point buffer is invalid.</summary>
		private const Int32 ERROR_INVALID_REPARSE_DATA = 4392;

		/// <summary>The file or directory is not a reparse point.</summary>
		private const Int32 ERROR_NOT_A_REPARSE_POINT = 4390;

		/// <summary>The reparse point attribute cannot be set because it conflicts with an existing attribute.</summary>
		private const Int32 ERROR_REPARSE_ATTRIBUTE_CONFLICT = 4391;

		/// <summary>The tag present in the reparse point buffer is invalid.</summary>
		private const Int32 ERROR_REPARSE_TAG_INVALID = 4393;

		/// <summary>There is a mismatch between the tag specified in the request and the tag present in the reparse point.</summary>
		private const Int32 ERROR_REPARSE_TAG_MISMATCH = 4394;

		/// <summary>Command to delete the reparse point data base.</summary>
		private const Int32 FSCTL_DELETE_REPARSE_POINT = 0x000900AC;

		/// <summary>Command to get the reparse point data block.</summary>
		private const Int32 FSCTL_GET_REPARSE_POINT = 0x000900A8;

		/// <summary>Command to set the reparse point data block.</summary>
		private const Int32 FSCTL_SET_REPARSE_POINT = 0x000900A4;

		/// <summary>Reparse point tag used to identify mount points and junction points.</summary>
		private const UInt32 IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;

		/// <summary>
		///     This prefix indicates to NTFS that the path is to be treated as a non-interpreted path in the virtual file
		///     system.
		/// </summary>
		private const String NonInterpretedPathPrefix = @"\??\";

		[DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true )]
		private static extern IntPtr CreateFile(
			[NotNull] String lpFileName,
			EFileAccess dwDesiredAccess,
			EFileShare dwShareMode,
			IntPtr lpSecurityAttributes,
			ECreationDisposition dwCreationDisposition,
			EFileAttributes dwFlagsAndAttributes,
			IntPtr hTemplateFile
		);

		[DllImport( DLL.kernel32, BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true )]
		private static extern Boolean DeviceIoControl(
			IntPtr hDevice,
			UInt32 dwIoControlCode,
			IntPtr InBuffer,
			Int32 nInBufferSize,
			IntPtr OutBuffer,
			Int32 nOutBufferSize,
			out Int32 pBytesReturned,
			IntPtr lpOverlapped
		);

		[CanBeNull]
		private static String? InternalGetTarget( SafeFileHandle handle ) {
			var outBufferSize = Marshal.SizeOf( typeof( REPARSE_DATA_BUFFER ) );
			var outBuffer = Marshal.AllocHGlobal( outBufferSize );

			try {
				var result = DeviceIoControl( handle.DangerousGetHandle(), FSCTL_GET_REPARSE_POINT, IntPtr.Zero, 0, outBuffer, outBufferSize, out var bytesReturned,
											  IntPtr.Zero );

				if ( !result ) {
					var error = Marshal.GetLastWin32Error();

					if ( error == ERROR_NOT_A_REPARSE_POINT ) {
						return default;
					}

					ThrowLastWin32Error( "Unable to get information about junction point." );
				}

				var reparseDataBuffer = ( REPARSE_DATA_BUFFER )Marshal.PtrToStructure( outBuffer, typeof( REPARSE_DATA_BUFFER ) )!;

				if ( reparseDataBuffer.ReparseTag != IO_REPARSE_TAG_MOUNT_POINT ) {
					return default;
				}

				var targetDir = Encoding.Unicode.GetString( reparseDataBuffer.PathBuffer, reparseDataBuffer.SubstituteNameOffset, reparseDataBuffer.SubstituteNameLength );

				if ( targetDir.StartsWith( NonInterpretedPathPrefix ) ) {
					targetDir = targetDir.Substring( NonInterpretedPathPrefix.Length );
				}

				return targetDir;
			}
			finally {
				Marshal.FreeHGlobal( outBuffer );
			}
		}

		[NotNull]
		private static SafeFileHandle OpenReparsePoint( [NotNull] String reparsePoint, EFileAccess accessMode ) {
			var reparsePointHandle = new SafeFileHandle(
				CreateFile( reparsePoint, accessMode, EFileShare.Read | EFileShare.Write | EFileShare.Delete, IntPtr.Zero, ECreationDisposition.OpenExisting,
							EFileAttributes.BackupSemantics | EFileAttributes.OpenReparsePoint, IntPtr.Zero ), true );

			if ( Marshal.GetLastWin32Error() != 0 ) {
				ThrowLastWin32Error( "Unable to open reparse point." );
			}

			return reparsePointHandle;
		}

		private static void ThrowLastWin32Error( [NotNull] String message ) => throw new IOException( message, Marshal.GetExceptionForHR( Marshal.GetHRForLastWin32Error() ) );

		/// <summary>Creates a junction point from the specified directory to the specified target directory.</summary>
		/// <remarks>Only works on NTFS.</remarks>
		/// <param name="junctionPoint">The junction point path</param>
		/// <param name="targetDir">The target directory</param>
		/// <param name="overwrite">If true overwrites an existing reparse point or empty directory</param>
		/// <exception cref="IOException">
		///     Thrown when the junction point could not be created or when an existing directory was
		///     found and <paramref name="overwrite" /> if false
		/// </exception>
		public static void Create( [NotNull] String junctionPoint, [NotNull] String targetDir, Boolean overwrite ) {
			targetDir = targetDir.GetFullPath();

			if ( !targetDir.Exists() ) {
				throw new IOException( "Target path does not exist or is not a directory." );
			}

			if ( junctionPoint.Exists() ) {
				if ( !overwrite ) {
					throw new IOException( "Directory already exists and overwrite parameter is false." );
				}
			}
			else {
				junctionPoint.CreateDirectory();
			}

			using var handle = OpenReparsePoint( junctionPoint, EFileAccess.GenericWrite );

			var targetDirBytes = Encoding.Unicode.GetBytes( NonInterpretedPathPrefix + targetDir.GetFullPath() );

			var reparseDataBuffer = new REPARSE_DATA_BUFFER {
				ReparseTag = IO_REPARSE_TAG_MOUNT_POINT, ReparseDataLength = ( UInt16 )( targetDirBytes.Length + 12 ), SubstituteNameOffset = 0,
				SubstituteNameLength = ( UInt16 )targetDirBytes.Length, PrintNameOffset = ( UInt16 )( targetDirBytes.Length + 2 ), PrintNameLength = 0,
				PathBuffer = new Byte[0x3ff0]
			};

			Array.Copy( targetDirBytes, reparseDataBuffer.PathBuffer, targetDirBytes.Length );

			var inBufferSize = Marshal.SizeOf( reparseDataBuffer );
			var inBuffer = Marshal.AllocHGlobal( inBufferSize );

			try {
				Marshal.StructureToPtr( reparseDataBuffer, inBuffer, false );

				var result = DeviceIoControl( handle.DangerousGetHandle(), FSCTL_SET_REPARSE_POINT, inBuffer, targetDirBytes.Length + 20, IntPtr.Zero, 0,
											  out var bytesReturned, IntPtr.Zero );

				if ( !result ) {
					ThrowLastWin32Error( "Unable to create junction point." );
				}
			}
			finally {
				Marshal.FreeHGlobal( inBuffer );
			}
		}

		/// <summary>
		///     Deletes a junction point at the specified source directory along with the directory itself. Does nothing if
		///     the junction point does not exist.
		/// </summary>
		/// <remarks>Only works on NTFS.</remarks>
		/// <param name="junctionPoint">The junction point path</param>
		public static void Delete( [NotNull] String junctionPoint ) {
			if ( !junctionPoint.Exists() ) {
				if ( File.Exists( junctionPoint ) ) {
					throw new IOException( "Path is not a junction point." );
				}

				return;
			}

			using var handle = OpenReparsePoint( junctionPoint, EFileAccess.GenericWrite );

			var reparseDataBuffer = new REPARSE_DATA_BUFFER {
				ReparseTag = IO_REPARSE_TAG_MOUNT_POINT, ReparseDataLength = 0, PathBuffer = new Byte[0x3ff0]
			};

			var inBufferSize = Marshal.SizeOf( reparseDataBuffer );
			var inBuffer = Marshal.AllocHGlobal( inBufferSize );

			try {
				Marshal.StructureToPtr( reparseDataBuffer, inBuffer, false );

				var result = DeviceIoControl( handle.DangerousGetHandle(), FSCTL_DELETE_REPARSE_POINT, inBuffer, 8, IntPtr.Zero, 0, out var bytesReturned, IntPtr.Zero );

				if ( !result ) {
					ThrowLastWin32Error( "Unable to delete junction point." );
				}
			}
			finally {
				Marshal.FreeHGlobal( inBuffer );
			}

			try {
				Directory.Delete( junctionPoint );
			}
			catch ( IOException ex ) {
				throw new IOException( "Unable to delete junction point.", ex );
			}
		}

		/// <summary>Determines whether the specified path exists and refers to a junction point.</summary>
		/// <param name="path">The junction point path</param>
		/// <returns>True if the specified path represents a junction point</returns>
		/// <exception cref="IOException">Thrown if the specified path is invalid or some other error occurs</exception>
		public static Boolean Exists( [NotNull] String path ) {
			path = path.ThrowIfBlank();

			if ( !path.Exists() ) {
				return false;
			}

			using var handle = OpenReparsePoint( path.ThrowIfBlank(), EFileAccess.GenericRead );

			var target = InternalGetTarget( handle );

			return target != null;
		}

		/// <summary>Gets the target of the specified junction point.</summary>
		/// <remarks>Only works on NTFS.</remarks>
		/// <param name="junctionPoint">The junction point path</param>
		/// <returns>The target of the junction point</returns>
		/// <exception cref="IOException">
		///     Thrown when the specified path does not exist, is invalid, is not a junction point, or
		///     some other error occurs
		/// </exception>
		[NotNull]
		public static String GetTarget( [NotNull] String junctionPoint ) {
			using var handle = OpenReparsePoint( junctionPoint, EFileAccess.GenericRead );

			var target = InternalGetTarget( handle );

			if ( target == null ) {
				throw new IOException( "Path is not a junction point." );
			}

			return target;
		}

		private enum ECreationDisposition : UInt32 {

			New = 1,
			CreateAlways = 2,
			OpenExisting = 3,
			OpenAlways = 4,
			TruncateExisting = 5

		}

		[Flags]
		private enum EFileAccess : UInt32 {

			GenericRead = 0x80000000,
			GenericWrite = 0x40000000,
			GenericExecute = 0x20000000,
			GenericAll = 0x10000000

		}

		[Flags]
		private enum EFileAttributes : UInt32 {

			Readonly = 0x00000001,
			Hidden = 0x00000002,
			System = 0x00000004,
			Directory = 0x00000010,
			Archive = 0x00000020,
			Device = 0x00000040,
			Normal = 0x00000080,
			Temporary = 0x00000100,
			SparseFile = 0x00000200,
			ReparsePoint = 0x00000400,
			Compressed = 0x00000800,
			Offline = 0x00001000,
			NotContentIndexed = 0x00002000,
			Encrypted = 0x00004000,
			WriteThrough = 0x80000000,
			Overlapped = 0x40000000,
			NoBuffering = 0x20000000,
			RandomAccess = 0x10000000,
			SequentialScan = 0x08000000,
			DeleteOnClose = 0x04000000,
			BackupSemantics = 0x02000000,
			PosixSemantics = 0x01000000,
			OpenReparsePoint = 0x00200000,
			OpenNoRecall = 0x00100000,
			FirstPipeInstance = 0x00080000

		}

		[Flags]
		private enum EFileShare : UInt32 {

			None = 0x00000000,
			Read = 0x00000001,
			Write = 0x00000002,
			Delete = 0x00000004

		}

		[StructLayout( LayoutKind.Sequential )]
		private struct REPARSE_DATA_BUFFER {

			/// <summary>Reparse point tag. Must be a Microsoft reparse point tag.</summary>
			public UInt32 ReparseTag;

			/// <summary>
			///     Size, in bytes, of the data after the Reserved member. This can be calculated by: (4 * sizeof(ushort)) +
			///     SubstituteNameLength + PrintNameLength + (namesAreNullTerminated
			///     ? 2 * sizeof(char) : 0);
			/// </summary>
			public UInt16 ReparseDataLength;

			/// <summary>Reserved; do not use.</summary>

			// ReSharper disable once MemberCanBePrivate.Local
			// ReSharper disable once FieldCanBeMadeReadOnly.Local
			public UInt16 Reserved;

			/// <summary>Offset, in bytes, of the substitute name string in the PathBuffer array.</summary>
			public UInt16 SubstituteNameOffset;

			/// <summary>
			///     Length, in bytes, of the substitute name string. If this string is null-terminated, SubstituteNameLength does
			///     not include space for the null character.
			/// </summary>
			public UInt16 SubstituteNameLength;

			/// <summary>Offset, in bytes, of the print name string in the PathBuffer array.</summary>
			public UInt16 PrintNameOffset;

			/// <summary>
			///     Length, in bytes, of the print name string. If this string is null-terminated, PrintNameLength does not
			///     include space for the null character.
			/// </summary>
			public UInt16 PrintNameLength;

			/// <summary>
			///     A buffer containing the unicode-encoded path string. The path string contains the substitute name string and
			///     print name string.
			/// </summary>
			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 0x3FF0 )]
			public Byte[] PathBuffer;

		}

	}

}