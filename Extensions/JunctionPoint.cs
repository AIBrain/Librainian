// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/JunctionPoint.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

namespace Librainian.Extensions {

    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.Win32.SafeHandles;
    using OperatingSystem;

    [Flags]
    public enum EFileShare : UInt32 {
        None = 0x00000000,
        Read = 0x00000001,
        Write = 0x00000002,
        Delete = 0x00000004
    }

    /// <summary>Provides access to NTFS junction points in .Net.</summary>
    public static class JunctionPoint {

        /// <summary>The data present in the reparse point buffer is invalid.</summary>
        private const Int32 ErrorInvalidReparseData = 4392;

        /// <summary>The file or directory is not a reparse point.</summary>
        private const Int32 ErrorNotAReparsePoint = 4390;

        /// <summary>
        ///     The reparse point attribute cannot be set because it conflicts with an existing attribute.
        /// </summary>
        private const Int32 ErrorReparseAttributeConflict = 4391;

        /// <summary>The tag present in the reparse point buffer is invalid.</summary>
        private const Int32 ErrorReparseTagInvalid = 4393;

        /// <summary>
        ///     There is a mismatch between the tag specified in the request and the tag present in the
        ///     reparse point.
        /// </summary>
        private const Int32 ErrorReparseTagMismatch = 4394;

        /// <summary>Command to delete the reparse point data base.</summary>
        private const Int32 FsctlDeleteReparsePoint = 0x000900AC;

        /// <summary>Command to get the reparse point data block.</summary>
        private const Int32 FsctlGetReparsePoint = 0x000900A8;

        /// <summary>Command to set the reparse point data block.</summary>
        private const Int32 FsctlSetReparsePoint = 0x000900A4;

        /// <summary>Reparse point tag used to identify mount points and junction points.</summary>
        private const UInt32 IOReparseTagMountPoint = 0xA0000003;

        /// <summary>
        ///     This prefix indicates to NTFS that the path is to be treated as a non-interpreted path
        ///     in the virtual file system.
        /// </summary>
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

        /// <summary>
        ///     Creates a junction point from the specified directory to the specified target directory.
        /// </summary>
        /// <remarks>Only works on NTFS.</remarks>
        /// <param name="junctionPoint">The junction point path</param>
        /// <param name="targetDir">The target directory</param>
        /// <param name="overwrite">If true overwrites an existing reparse point or empty directory</param>
        /// <exception cref="IOException">
        ///     Thrown when the junction point could not be created or when an existing directory was
        ///     found and <paramref name="overwrite" /> if false
        /// </exception>
        public static void Create( String junctionPoint, String targetDir, Boolean overwrite ) {
            targetDir = Path.GetFullPath( targetDir );

            if ( !Directory.Exists( targetDir ) ) {
                throw new IOException( "Target path does not exist or is not a directory." );
            }

            if ( Directory.Exists( junctionPoint ) ) {
                if ( !overwrite ) {
                    throw new IOException( "Directory already exists and overwrite parameter is false." );
                }
            }
            else {
                Directory.CreateDirectory( junctionPoint );
            }

            using ( var handle = OpenReparsePoint( junctionPoint, FileAccess.Write ) ) {
                var targetDirBytes = Encoding.Unicode.GetBytes( NonInterpretedPathPrefix + Path.GetFullPath( targetDir ) );

                var reparseDataBuffer = new ReparseDataBuffer { ReparseTag = IOReparseTagMountPoint, ReparseDataLength = ( UInt16 )( targetDirBytes.Length + 12 ), SubstituteNameOffset = 0, SubstituteNameLength = ( UInt16 )targetDirBytes.Length, PrintNameOffset = ( UInt16 )( targetDirBytes.Length + 2 ), PrintNameLength = 0, PathBuffer = new Byte[ 0x3ff0 ] };

                Array.Copy( targetDirBytes, reparseDataBuffer.PathBuffer, targetDirBytes.Length );

                var inBufferSize = Marshal.SizeOf( reparseDataBuffer );
                var inBuffer = Marshal.AllocHGlobal( inBufferSize );

                try {
                    Marshal.StructureToPtr( reparseDataBuffer, inBuffer, false );

					var result = NativeMethods.DeviceIoControl( handle.DangerousGetHandle(), FsctlSetReparsePoint, inBuffer, targetDirBytes.Length + 20, IntPtr.Zero, 0, out var bytesReturned, IntPtr.Zero );

					if ( !result ) {
                        ThrowLastWin32Error( "Unable to create junction point." );
                    }
                }
                finally {
                    Marshal.FreeHGlobal( inBuffer );
                }
            }
        }

        /// <summary>
        ///     Deletes a junction point at the specified source directory along with the directory
        ///     itself. Does nothing if the junction point does not exist.
        /// </summary>
        /// <remarks>Only works on NTFS.</remarks>
        /// <param name="junctionPoint">The junction point path</param>
        public static void Delete( String junctionPoint ) {
            if ( !Directory.Exists( junctionPoint ) ) {
                if ( File.Exists( junctionPoint ) ) {
                    throw new IOException( "Path is not a junction point." );
                }

                return;
            }

            using ( var handle = OpenReparsePoint( junctionPoint, FileAccess.Write) ) {
                var reparseDataBuffer = new ReparseDataBuffer { ReparseTag = IOReparseTagMountPoint, ReparseDataLength = 0, PathBuffer = new Byte[ 0x3ff0 ] };

                var inBufferSize = Marshal.SizeOf( reparseDataBuffer );
                var inBuffer = Marshal.AllocHGlobal( inBufferSize );
                try {
                    Marshal.StructureToPtr( reparseDataBuffer, inBuffer, false );

					var result = NativeMethods.DeviceIoControl( handle.DangerousGetHandle(), FsctlDeleteReparsePoint, inBuffer, 8, IntPtr.Zero, 0, out var bytesReturned, IntPtr.Zero );

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
        }

        /// <summary>
        ///     Determines whether the specified path exists and refers to a junction point.
        /// </summary>
        /// <param name="path">The junction point path</param>
        /// <returns>True if the specified path represents a junction point</returns>
        /// <exception cref="IOException">
        ///     Thrown if the specified path is invalid or some other error occurs
        /// </exception>
        public static Boolean Exists( String path ) {
            if ( !Directory.Exists( path ) ) {
                return false;
            }

            using ( var handle = OpenReparsePoint( path, FileAccess.Read) ) {
                var target = InternalGetTarget( handle );
                return target != null;
            }
        }

        /// <summary>Gets the target of the specified junction point.</summary>
        /// <remarks>Only works on NTFS.</remarks>
        /// <param name="junctionPoint">The junction point path</param>
        /// <returns>The target of the junction point</returns>
        /// <exception cref="IOException">
        ///     Thrown when the specified path does not exist, is invalid, is not a junction point, or
        ///     some other error occurs
        /// </exception>
        public static String GetTarget( String junctionPoint ) {
            using ( var handle = OpenReparsePoint( junctionPoint, FileAccess.Read ) ) {
                var target = InternalGetTarget( handle );
                if ( target is null ) {
                    throw new IOException( "Path is not a junction point." );
                }

                return target;
            }
        }

        private static String InternalGetTarget( SafeHandle handle ) {
            var outBufferSize = Marshal.SizeOf( typeof( ReparseDataBuffer ) );
            var outBuffer = Marshal.AllocHGlobal( outBufferSize );

            try {
				var result = NativeMethods.DeviceIoControl( handle.DangerousGetHandle(), FsctlGetReparsePoint, IntPtr.Zero, 0, outBuffer, outBufferSize, out var bytesReturned, IntPtr.Zero );

				if ( !result ) {
                    var error = Marshal.GetLastWin32Error();
                    if ( error == ErrorNotAReparsePoint ) {
                        return null;
                    }

                    ThrowLastWin32Error( "Unable to get information about junction point." );
                }

                var reparseDataBuffer = ( ReparseDataBuffer )Marshal.PtrToStructure( outBuffer, typeof( ReparseDataBuffer ) );

                if ( reparseDataBuffer.ReparseTag != IOReparseTagMountPoint ) {
                    return null;
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

        private static SafeFileHandle OpenReparsePoint( String reparsePoint, FileAccess accessMode ) {
            var bob = NativeMethods.CreateFile( reparsePoint, accessMode, FileShare.Read | FileShare.Write | FileShare.Delete, IntPtr.Zero, FileMode.Open, FileAttributes.Archive | FileAttributes.ReparsePoint, IntPtr.Zero );

            if ( Marshal.GetLastWin32Error() != 0 ) {
                ThrowLastWin32Error( "Unable to open reparse point." );
            }

            var reparsePointHandle = new SafeFileHandle( bob.DangerousGetHandle(), true );

            return reparsePointHandle;
        }

		private static void ThrowLastWin32Error( String message ) => throw new IOException( message, Marshal.GetExceptionForHR( Marshal.GetHRForLastWin32Error() ) );

		[StructLayout( LayoutKind.Sequential )]
        private struct ReparseDataBuffer {

            /// <summary>Reparse point tag. Must be a Microsoft reparse point tag.</summary>
            public UInt32 ReparseTag;

            /// <summary>
            ///     Size, in bytes, of the data after the Reserved member. This can be calculated by: (4
            ///     * sizeof(UInt16)) + SubstituteNameLength + PrintNameLength + (namesAreNullTerminated
            ///     ? 2 * sizeof(char) : 0);
            /// </summary>
            public UInt16 ReparseDataLength;

            /// <summary>Reserved; do not use.</summary>
            public readonly UInt16 Reserved;

            /// <summary>
            ///     Offset, in bytes, of the substitute name String in the PathBuffer array.
            /// </summary>
            public UInt16 SubstituteNameOffset;

            /// <summary>
            ///     Length, in bytes, of the substitute name String. If this String is null-terminated,
            ///     SubstituteNameLength does not include space for the null character.
            /// </summary>
            public UInt16 SubstituteNameLength;

            /// <summary>Offset, in bytes, of the print name String in the PathBuffer array.</summary>
            public UInt16 PrintNameOffset;

            /// <summary>
            ///     Length, in bytes, of the print name String. If this String is null-terminated,
            ///     PrintNameLength does not include space for the null character.
            /// </summary>
            public UInt16 PrintNameLength;

            /// <summary>
            ///     A buffer containing the unicode-encoded path String. The path String contains the
            ///     substitute name String and print name String.
            /// </summary>
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 0x3FF0 )]
            public Byte[] PathBuffer;
        }
    }
}