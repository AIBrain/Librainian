// Copyright � 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "JunctionPoint.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", File: "JunctionPoint.cs" was last formatted by Protiguous on 2020/03/16 at 2:58 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using JetBrains.Annotations;
    using Microsoft.Win32.SafeHandles;

    /// <summary>Provides access to NTFS junction points in .Net.</summary>
    public static class JunctionPoint {

        /// <summary>The data present in the reparse point buffer is invalid.</summary>
        private const Int32 ErrorInvalidReparseData = 4392;

        /// <summary>The file or directory is not a reparse point.</summary>
        private const Int32 ErrorNotAReparsePoint = 4390;

        /// <summary>The reparse point attribute cannot be set because it conflicts with an existing attribute.</summary>
        private const Int32 ErrorReparseAttributeConflict = 4391;

        /// <summary>The tag present in the reparse point buffer is invalid.</summary>
        private const Int32 ErrorReparseTagInvalid = 4393;

        /// <summary>There is a mismatch between the tag specified in the request and the tag present in the reparse point.</summary>
        private const Int32 ErrorReparseTagMismatch = 4394;

        /// <summary>Command to delete the reparse point data base.</summary>
        private const Int32 FsctlDeleteReparsePoint = 0x000900AC;

        /// <summary>Command to get the reparse point data block.</summary>
        private const Int32 FsctlGetReparsePoint = 0x000900A8;

        /// <summary>Command to set the reparse point data block.</summary>
        private const Int32 FsctlSetReparsePoint = 0x000900A4;

        /// <summary>Reparse point tag used to identify mount points and junction points.</summary>
        private const UInt32 IOReparseTagMountPoint = 0xA0000003;

        /// <summary>This prefix indicates to NTFS that the path is to be treated as a non-interpreted path in the virtual file system.</summary>
        private const String NonInterpretedPathPrefix = @"\??\";

        public enum ECreationDisposition : UInt32 {

            New = 1,

            CreateAlways = 2,

            OpenExisting = 3,

            OpenAlways = 4,

            TruncateExisting = 5
        }

        [Flags]
        public enum EFileAccess : UInt32 {

            GenericRead = 0x80000000,

            GenericWrite = 0x40000000,

            GenericExecute = 0x20000000,

            GenericAll = 0x10000000
        }

        [Flags]
        public enum EFileAttributes : UInt32 {

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
        public enum EFileShare : UInt32 {

            None = 0x00000000,

            Read = 0x00000001,

            Write = 0x00000002,

            Delete = 0x00000004
        }

        [CanBeNull]
        private static String? InternalGetTarget( [NotNull] SafeHandle handle ) {
            var outBufferSize = Marshal.SizeOf( t: typeof( ReparseDataBuffer ) );
            var outBuffer = Marshal.AllocHGlobal( cb: outBufferSize );

            try {
                var result = NativeMethods.DeviceIoControl( hDevice: handle.DangerousGetHandle(), dwIoControlCode: FsctlGetReparsePoint, inBuffer: IntPtr.Zero,
                    nInBufferSize: 0, outBuffer: outBuffer, nOutBufferSize: outBufferSize, pBytesReturned: out var bytesReturned, lpOverlapped: IntPtr.Zero );

                if ( !result ) {
                    var error = Marshal.GetLastWin32Error();

                    if ( error == ErrorNotAReparsePoint ) {
                        return null;
                    }

                    ThrowLastWin32Error( message: "Unable to get information about junction point." );
                }

                var reparseDataBuffer = ( ReparseDataBuffer )Marshal.PtrToStructure( ptr: outBuffer, structureType: typeof( ReparseDataBuffer ) );

                if ( reparseDataBuffer.ReparseTag != IOReparseTagMountPoint ) {
                    return null;
                }

                var targetDir = Encoding.Unicode.GetString( bytes: reparseDataBuffer.PathBuffer, index: reparseDataBuffer.SubstituteNameOffset,
                    count: reparseDataBuffer.SubstituteNameLength );

                if ( targetDir.StartsWith( value: NonInterpretedPathPrefix ) ) {
                    targetDir = targetDir.Substring( startIndex: NonInterpretedPathPrefix.Length );
                }

                return targetDir;
            }
            finally {
                Marshal.FreeHGlobal( hglobal: outBuffer );
            }
        }

        [NotNull]
        private static SafeFileHandle OpenReparsePoint( [CanBeNull] String? reparsePoint, FileAccess accessMode ) {
            var bob = NativeMethods.CreateFile( lpFileName: reparsePoint, dwDesiredAccess: accessMode, dwShareMode: FileShare.Read | FileShare.Write | FileShare.Delete,
                lpSecurityAttributes: IntPtr.Zero, dwCreationDisposition: FileMode.Open, dwFlagsAndAttributes: FileAttributes.Archive | FileAttributes.ReparsePoint,
                hTemplateFile: IntPtr.Zero );

            if ( Marshal.GetLastWin32Error() != 0 ) {
                ThrowLastWin32Error( message: "Unable to open reparse point." );
            }

            var reparsePointHandle = new SafeFileHandle( preexistingHandle: bob.DangerousGetHandle(), ownsHandle: true );

            return reparsePointHandle;
        }

        private static void ThrowLastWin32Error( String message ) =>
            throw new IOException( message: message, innerException: Marshal.GetExceptionForHR( errorCode: Marshal.GetHRForLastWin32Error() ) );

        /// <summary>Creates a junction point from the specified directory to the specified target directory.</summary>
        /// <remarks>Only works on NTFS.</remarks>
        /// <param name="junctionPoint">The junction point path</param>
        /// <param name="targetDir">    The target directory</param>
        /// <param name="overwrite">    If true overwrites an existing reparse point or empty directory</param>
        /// <exception cref="IOException">Thrown when the junction point could not be created or when an existing directory was found and <paramref name="overwrite" /> if false</exception>
        public static void Create( [NotNull] String junctionPoint, String targetDir, Boolean overwrite ) {
            targetDir = Path.GetFullPath( path: targetDir );

            if ( !Directory.Exists( path: targetDir ) ) {
                throw new IOException( message: "Target path does not exist or is not a directory." );
            }

            if ( Directory.Exists( path: junctionPoint ) ) {
                if ( !overwrite ) {
                    throw new IOException( message: "Directory already exists and overwrite parameter is false." );
                }
            }
            else {
                Directory.CreateDirectory( path: junctionPoint );
            }

            using ( var handle = OpenReparsePoint( reparsePoint: junctionPoint, accessMode: FileAccess.Write ) ) {
                var targetDirBytes = Encoding.Unicode.GetBytes( s: NonInterpretedPathPrefix + Path.GetFullPath( path: targetDir ) );

                var reparseDataBuffer = new ReparseDataBuffer {
                    ReparseTag = IOReparseTagMountPoint,
                    ReparseDataLength = ( UInt16 )( targetDirBytes.Length + 12 ),
                    SubstituteNameOffset = 0,
                    SubstituteNameLength = ( UInt16 )targetDirBytes.Length,
                    PrintNameOffset = ( UInt16 )( targetDirBytes.Length + 2 ),
                    PrintNameLength = 0,
                    PathBuffer = new Byte[ 0x3ff0 ]
                };

                Buffer.BlockCopy( src: targetDirBytes, srcOffset: 0, dst: reparseDataBuffer.PathBuffer, dstOffset: 0, count: targetDirBytes.Length );

                var inBufferSize = Marshal.SizeOf( structure: reparseDataBuffer );
                var inBuffer = Marshal.AllocHGlobal( cb: inBufferSize );

                try {
                    Marshal.StructureToPtr( structure: reparseDataBuffer, ptr: inBuffer, fDeleteOld: false );

                    var result = NativeMethods.DeviceIoControl( hDevice: handle.DangerousGetHandle(), dwIoControlCode: FsctlSetReparsePoint, inBuffer: inBuffer,
                        nInBufferSize: targetDirBytes.Length + 20, outBuffer: IntPtr.Zero, nOutBufferSize: 0, pBytesReturned: out var bytesReturned,
                        lpOverlapped: IntPtr.Zero );

                    if ( !result ) {
                        ThrowLastWin32Error( message: "Unable to create junction point." );
                    }
                }
                finally {
                    Marshal.FreeHGlobal( hglobal: inBuffer );
                }
            }
        }

        /// <summary>Deletes a junction point at the specified source directory along with the directory itself. Does nothing if the junction point does not exist.</summary>
        /// <remarks>Only works on NTFS.</remarks>
        /// <param name="junctionPoint">The junction point path</param>
        public static void Delete( [CanBeNull] String? junctionPoint ) {
            if ( !Directory.Exists( path: junctionPoint ) ) {
                if ( File.Exists( path: junctionPoint ) ) {
                    throw new IOException( message: "Path is not a junction point." );
                }

                return;
            }

            using ( var handle = OpenReparsePoint( reparsePoint: junctionPoint, accessMode: FileAccess.Write ) ) {
                var reparseDataBuffer = new ReparseDataBuffer {
                    ReparseTag = IOReparseTagMountPoint,
                    ReparseDataLength = 0,
                    PathBuffer = new Byte[ 0x3ff0 ]
                };

                var inBufferSize = Marshal.SizeOf( structure: reparseDataBuffer );
                var inBuffer = Marshal.AllocHGlobal( cb: inBufferSize );

                try {
                    Marshal.StructureToPtr( structure: reparseDataBuffer, ptr: inBuffer, fDeleteOld: false );

                    var result = NativeMethods.DeviceIoControl( hDevice: handle.DangerousGetHandle(), dwIoControlCode: FsctlDeleteReparsePoint, inBuffer: inBuffer,
                        nInBufferSize: 8, outBuffer: IntPtr.Zero, nOutBufferSize: 0, pBytesReturned: out var bytesReturned, lpOverlapped: IntPtr.Zero );

                    if ( !result ) {
                        ThrowLastWin32Error( message: "Unable to delete junction point." );
                    }
                }
                finally {
                    Marshal.FreeHGlobal( hglobal: inBuffer );
                }

                try {
                    Directory.Delete( path: junctionPoint );
                }
                catch ( IOException ex ) {
                    throw new IOException( message: "Unable to delete junction point.", innerException: ex );
                }
            }
        }

        /// <summary>Determines whether the specified path exists and refers to a junction point.</summary>
        /// <param name="path">The junction point path</param>
        /// <returns>True if the specified path represents a junction point</returns>
        /// <exception cref="IOException">Thrown if the specified path is invalid or some other error occurs</exception>
        public static Boolean Exists( [CanBeNull] String? path ) {
            if ( !Directory.Exists( path: path ) ) {
                return default;
            }

            using ( var handle = OpenReparsePoint( reparsePoint: path, accessMode: FileAccess.Read ) ) {
                var target = InternalGetTarget( handle: handle );

                return target != null;
            }
        }

        /// <summary>Gets the target of the specified junction point.</summary>
        /// <remarks>Only works on NTFS.</remarks>
        /// <param name="junctionPoint">The junction point path</param>
        /// <returns>The target of the junction point</returns>
        /// <exception cref="IOException">Thrown when the specified path does not exist, is invalid, is not a junction point, or some other error occurs</exception>
        [NotNull]
        public static String GetTarget( [CanBeNull] String? junctionPoint ) {
            using ( var handle = OpenReparsePoint( reparsePoint: junctionPoint, accessMode: FileAccess.Read ) ) {
                var target = InternalGetTarget( handle: handle );

                if ( target is null ) {
                    throw new IOException( message: "Path is not a junction point." );
                }

                return target;
            }
        }

        [StructLayout( layoutKind: LayoutKind.Sequential )]
        private struct ReparseDataBuffer {

            /// <summary>Reparse point tag. Must be a Microsoft reparse point tag.</summary>
            public UInt32 ReparseTag;

            /// <summary>
            /// Size, in bytes, of the data after the Reserved member. This can be calculated by: (4 * sizeof(UInt16)) + SubstituteNameLength + PrintNameLength + (namesAreNullTerminated
            /// ? 2 * sizeof(char) : 0);
            /// </summary>
            public UInt16 ReparseDataLength;

            /// <summary>Reserved; do not use.</summary>
            public UInt16 Reserved { get; }

            /// <summary>Offset, in bytes, of the substitute name String in the PathBuffer array.</summary>
            public UInt16 SubstituteNameOffset;

            /// <summary>Length, in bytes, of the substitute name String. If this String is null-terminated, SubstituteNameLength does not include space for the null character.</summary>
            public UInt16 SubstituteNameLength;

            /// <summary>Offset, in bytes, of the print name String in the PathBuffer array.</summary>
            public UInt16 PrintNameOffset;

            /// <summary>Length, in bytes, of the print name String. If this String is null-terminated, PrintNameLength does not include space for the null character.</summary>
            public UInt16 PrintNameLength;

            /// <summary>A buffer containing the unicode-encoded path String. The path String contains the substitute name String and print name String.</summary>
            [MarshalAs( unmanagedType: UnmanagedType.ByValArray, SizeConst = 0x3FF0 )]
            public Byte[] PathBuffer;
        }
    }
}