// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IOWrapper.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "IOWrapper.cs" was last formatted by Protiguous on 2019/08/08 at 9:17 AM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;

    /// <summary>
    ///     defrag stuff
    /// </summary>
    /// <see cref="http://blogs.msdn.com/b/jeffrey_wall/archive/2004/09/13/229137.aspx" />
    public static class IOWrapper {

        /// <summary>Access flags.</summary>
        [Flags]
        [SuppressMessage( "ReSharper", "InconsistentNaming" )]
        public enum ACCESS_MASK : UInt32 {

            /// <summary>The right to delete the object.</summary>
            DELETE = 0x00010000,

            /// <summary>
            ///     The right to read the information in the object's security descriptor, not including the information in the system
            ///     access control
            ///     list (SACL).
            /// </summary>
            READ_CONTROL = 0x00020000,

            /// <summary>The right to modify the discretionary access control list (DACL) in the object's security descriptor.</summary>
            WRITE_DAC = 0x00040000,

            /// <summary>The right to change the owner in the object's security descriptor.</summary>
            WRITE_OWNER = 0x00080000,

            /// <summary>
            ///     The right to use the object for synchronization. this enables a thread to wait until the object is in the signaled
            ///     state. Some
            ///     object types do not support this access right.
            /// </summary>
            SYNCHRONIZE = 0x00100000,

            /// <summary>Combines DELETE, READ_CONTROL, WRITE_DAC, and WRITE_OWNER access.</summary>
            STANDARD_RIGHTS_REQUIRED = 0x000F0000,

            /// <summary>Currently defined to equal READ_CONTROL.</summary>
            STANDARD_RIGHTS_READ = 0x00020000,

            /// <summary>Currently defined to equal READ_CONTROL.</summary>
            STANDARD_RIGHTS_WRITE = 0x00020000,

            /// <summary>Currently defined to equal READ_CONTROL.</summary>
            STANDARD_RIGHTS_EXECUTE = 0x00020000,

            /// <summary>Combines DELETE, READ_CONTROL, WRITE_DAC, WRITE_OWNER, and SYNCHRONIZE access.</summary>
            STANDARD_RIGHTS_ALL = 0x001F0000,

            /// <summary>The specific rights all</summary>
            SPECIFIC_RIGHTS_ALL = 0x0000FFFF,

            /// <summary>
            ///     Controls the ability to get or set the SACL in an object's security descriptor. The system grants this access right
            ///     only if the
            ///     SE_SECURITY_NAME privilege is enabled in the access token of the requesting thread.
            /// </summary>
            ACCESS_SYSTEM_SECURITY = 0x01000000,

            /// <summary>Request that the object be opened with all the access rights that are valid for the caller.</summary>
            MAXIMUM_ALLOWED = 0x02000000,

            /// <summary>Read access.</summary>
            GENERIC_READ = 0x80000000,

            /// <summary>Write access.</summary>
            GENERIC_WRITE = 0x40000000,

            /// <summary>Execute access.</summary>
            GENERIC_EXECUTE = 0x20000000,

            /// <summary>All possible access rights.</summary>
            GENERIC_ALL = 0x10000000
        }

        /// <summary>Enumerates the that may apply to files.</summary>
        /// <remarks>These flags may be passed to CreateFile.</remarks>
        [Flags]
        [SuppressMessage( "ReSharper", "InconsistentNaming" )]
        public enum FileAccess : UInt32 {

            /// <summary>Read access.</summary>
            GENERIC_READ = ACCESS_MASK.GENERIC_READ,

            /// <summary>Write access.</summary>
            GENERIC_WRITE = ACCESS_MASK.GENERIC_WRITE,

            /// <summary>Execute access.</summary>
            GENERIC_EXECUTE = ACCESS_MASK.GENERIC_EXECUTE,

            /// <summary>All possible access rights.</summary>
            GENERIC_ALL = ACCESS_MASK.GENERIC_ALL,

            /// <summary>
            ///     For a file object, the right to read the corresponding file data. For a directory object, the right to read the
            ///     corresponding directory data.
            /// </summary>
            FILE_READ_DATA = 0x0001, // file & pipe

            /// <summary>For a directory, the right to list the contents of the directory.</summary>
            FILE_LIST_DIRECTORY = 0x0001, // directory

            /// <summary>
            ///     For a file object, the right to write data to the file. For a directory object, the right to create a file in the
            ///     directory ( <see cref="FILE_ADD_FILE" />).
            /// </summary>
            FILE_WRITE_DATA = 0x0002, // file & pipe

            /// <summary>For a directory, the right to create a file in the directory.</summary>
            FILE_ADD_FILE = 0x0002, // directory

            /// <summary>
            ///     For a file object, the right to append data to the file. (For local files, write operations will not overwrite
            ///     existing data if this flag is
            ///     specified without <see cref="FILE_WRITE_DATA" />.) For a directory object, the right to create a subdirectory (
            ///     <see cref="FILE_ADD_SUBDIRECTORY" />).
            /// </summary>
            FILE_APPEND_DATA = 0x0004, // file

            /// <summary>For a directory, the right to create a subdirectory.</summary>
            FILE_ADD_SUBDIRECTORY = 0x0004, // directory

            /// <summary>For a named pipe, the right to create a pipe.</summary>
            FILE_CREATE_PIPE_INSTANCE = 0x0004, // named pipe

            /// <summary>The right to read extended file attributes.</summary>
            FILE_READ_EA = 0x0008, // file & directory

            /// <summary>The right to write extended file attributes.</summary>
            FILE_WRITE_EA = 0x0010, // file & directory

            /// <summary>
            ///     For a native code file, the right to execute the file. this access right given to scripts may cause the script to
            ///     be executable, depending on the
            ///     script interpreter.
            /// </summary>
            FILE_EXECUTE = 0x0020, // file

            /// <summary>
            ///     For a directory, the right to traverse the directory. By default, users are assigned the BYPASS_TRAVERSE_CHECKING
            ///     privilege, which ignores the
            ///     FILE_TRAVERSE access right.
            /// </summary>
            FILE_TRAVERSE = 0x0020, // directory

            /// <summary>For a directory, the right to delete a directory and all the files it contains, including read-only files.</summary>
            FILE_DELETE_CHILD = 0x0040, // directory

            /// <summary>The right to read file attributes.</summary>
            FILE_READ_ATTRIBUTES = 0x0080, // all

            /// <summary>The right to write file attributes.</summary>
            FILE_WRITE_ATTRIBUTES = 0x0100, // all

            SPECIFIC_RIGHTS_ALL = 0x00FFFF,

            FILE_ALL_ACCESS = ACCESS_MASK.STANDARD_RIGHTS_REQUIRED | ACCESS_MASK.SYNCHRONIZE | 0x1FF,

            FILE_GENERIC_READ = ACCESS_MASK.STANDARD_RIGHTS_READ | FILE_READ_DATA | FILE_READ_ATTRIBUTES | FILE_READ_EA | ACCESS_MASK.SYNCHRONIZE,

            FILE_GENERIC_WRITE = ACCESS_MASK.STANDARD_RIGHTS_WRITE | FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | ACCESS_MASK.SYNCHRONIZE,

            FILE_GENERIC_EXECUTE = ACCESS_MASK.STANDARD_RIGHTS_EXECUTE | FILE_READ_ATTRIBUTES | FILE_EXECUTE | ACCESS_MASK.SYNCHRONIZE
        }

        private const UInt32 ErrorInsufficientBuffer = 122;

        private const UInt32 FileFlagNoBuffering = 0x20000000;

        private const UInt32 FileReadAttributes = 0x0080;

        private const UInt32 FileShareDelete = 0x00000004;

        // CreateFile constants
        private const UInt32 FileShareRead = 0x00000001;

        private const UInt32 FileShareWrite = 0x00000002;

        private const UInt32 FileWriteAttributes = 0x0100;

        private const UInt32 GenericRead = 0x80000000;

        private const UInt32 GenericWrite = 0x40000000;

        private const UInt32 OpenExisting = 3;

        /// <summary>
        ///     returns a 2*number of extents array - the vcn and the lcn as pairs
        /// </summary>
        /// <param name="path">file to get the map for ex: "c:\windows\explorer.exe"</param>
        /// <returns>An array of [virtual cluster, physical cluster]</returns>
        public static Array GetFileMap( String path ) {
            var hFile = IntPtr.Zero;
            var pAlloc = IntPtr.Zero;

            try {
                hFile = OpenFile( path );

                const Int64 i64 = 0;

                var handle = GCHandle.Alloc( i64, GCHandleType.Pinned );
                var p = handle.AddrOfPinnedObject();

                const Int32 q = 1024 * 1024 * 64; // 1024 bytes == 1k * 1024 == 1 meg * 64 == 64 megs

                pAlloc = Marshal.AllocHGlobal( q );
                var pDest = pAlloc;
                var fResult = NativeMethods.DeviceIoControl( hFile, FSConstants.FsctlGetRetrievalPointers, p, Marshal.SizeOf( i64 ), pDest, q, out var size, IntPtr.Zero );

                if ( !fResult ) {
                    throw new Exception( Marshal.GetLastWin32Error().ToString() );
                }

                handle.Free();

                /*
                returned back one of...
                    typedef struct RETRIEVAL_POINTERS_BUFFER {
                    DWORD ExtentCount;
                    LARGE_INTEGER StartingVcn;
                    struct {
                        LARGE_INTEGER NextVcn;
                        LARGE_INTEGER Lcn;
                    } Extents[1];
                    } RETRIEVAL_POINTERS_BUFFER, *PRETRIEVAL_POINTERS_BUFFER;
                */

                var extentCount = ( Int32 ) Marshal.PtrToStructure( pDest, typeof( Int32 ) );

                pDest = ( IntPtr ) ( ( Int64 ) pDest + 4 );

                var startingVcn = ( Int64 ) Marshal.PtrToStructure( pDest, typeof( Int64 ) );

                Debug.Assert( startingVcn == 0 );

                pDest = ( IntPtr ) ( ( Int64 ) pDest + 8 );

                // now pDest points at an array of pairs of Int64s.

                var retVal = Array.CreateInstance( typeof( Int64 ), new[] {
                    extentCount, 2
                } );

                for ( var i = 0; i < extentCount; i++ ) {
                    for ( var j = 0; j < 2; j++ ) {
                        var v = ( Int64 ) Marshal.PtrToStructure( pDest, typeof( Int64 ) );

                        retVal.SetValue( v, new[] {
                            i, j
                        } );

                        pDest = ( IntPtr ) ( ( Int64 ) pDest + 8 );
                    }
                }

                return retVal;
            }
            finally {
                hFile.CloseHandle();

                // ReSharper disable once RedundantAssignment
                hFile = IntPtr.Zero;

                Marshal.FreeHGlobal( pAlloc );

                // ReSharper disable once RedundantAssignment
                pAlloc = IntPtr.Zero;
            }
        }

        /// <summary>
        ///     Get cluster usage for a device
        /// </summary>
        /// <param name="deviceName">use "c:"</param>
        /// <returns>a bitarray for each cluster</returns>
        [NotNull]
        public static BitArray GetVolumeMap( [NotNull] String deviceName ) {
            if ( String.IsNullOrWhiteSpace( value: deviceName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( deviceName ) );
            }

            var pAlloc = IntPtr.Zero;
            var hDevice = IntPtr.Zero;

            try {
                hDevice = OpenVolume( deviceName );

                const Int64 i64 = 0;

                var handle = GCHandle.Alloc( i64, GCHandleType.Pinned );
                var p = handle.AddrOfPinnedObject();

                // alloc off more than enough for my machine 64 megs == 67108864 bytes == 536870912
                // bits == cluster count NTFS 4k clusters == 2147483648 k of storage == 2097152 megs
                // == 2048 gig disk storage
                const Int32 q = 1024 * 1024 * 64; // 1024 bytes == 1k * 1024 == 1 meg * 64 == 64 megs

                pAlloc = Marshal.AllocHGlobal( q );
                var pDest = pAlloc;

                var result = NativeMethods.DeviceIoControl( hDevice, FSConstants.FsctlGetVolumeBitmap, p, Marshal.SizeOf( i64 ), pDest, q, out var size, IntPtr.Zero );

                if ( !result ) {
                    throw new Exception( Marshal.GetLastWin32Error().ToString() );
                }

                handle.Free();

                /*
                object returned was...
                  typedef struct
                  {
                       LARGE_INTEGER StartingLcn;
                       LARGE_INTEGER BitmapSize;
                       BYTE Buffer[1];
                  } VOLUME_BITMAP_BUFFER, *PVOLUME_BITMAP_BUFFER;
                */
                var startingLcn = ( Int64 ) Marshal.PtrToStructure( pDest, typeof( Int64 ) );

                Debug.Assert( startingLcn == 0 );

                pDest = ( IntPtr ) ( ( Int64 ) pDest + 8 );
                var bitmapSize = ( Int64 ) Marshal.PtrToStructure( pDest, typeof( Int64 ) );

                var byteSize = ( Int32 ) ( bitmapSize / 8 );
                byteSize++; // round up - even with no remainder

                var bitmapBegin = ( IntPtr ) ( ( Int64 ) pDest + 8 );

                var byteArr = new Byte[ byteSize ];

                Marshal.Copy( bitmapBegin, byteArr, 0, byteSize );

                var retVal = new BitArray( byteArr ) {
                    Length = ( Int32 ) bitmapSize
                };

                // truncate to exact cluster count
                return retVal;
            }
            finally {
                hDevice.CloseHandle();

                // ReSharper disable once RedundantAssignment
                hDevice = IntPtr.Zero;

                Marshal.FreeHGlobal( pAlloc );

                // ReSharper disable once RedundantAssignment
                pAlloc = IntPtr.Zero;
            }
        }

        /// <summary>
        ///     move a virtual cluster for a file to a logical cluster on disk, repeat for count clusters
        /// </summary>
        /// <param name="deviceName">device to move on"c:"</param>
        /// <param name="path">file to muck with "c:\windows\explorer.exe"</param>
        /// <param name="vcn">cluster number in file to move</param>
        /// <param name="lcn">cluster on disk to move to</param>
        /// <param name="count">for how many clusters</param>
        public static void MoveFile( String deviceName, String path, Int64 vcn, Int64 lcn, Int32 count ) {
            var hVol = IntPtr.Zero;
            var hFile = IntPtr.Zero;

            try {
                hVol = OpenVolume( deviceName );

                hFile = OpenFile( path );

                var mfd = new MoveFileData {
                    HFile = hFile, StartingVcn = vcn, StartingLcn = lcn, ClusterCount = count
                };

                var handle = GCHandle.Alloc( mfd, GCHandleType.Pinned );
                var p = handle.AddrOfPinnedObject();
                var bufSize = ( UInt32 ) Marshal.SizeOf( mfd );

                var fResult = NativeMethods.DeviceIoControl( hVol, FSConstants.FsctlMoveFile, p, bufSize, IntPtr.Zero, /* no output data from this FSCTL*/ 0, out var size,
                    IntPtr.Zero );

                if ( !fResult ) {
                    throw new Exception( Marshal.GetLastWin32Error().ToString() );
                }

                handle.Free();
            }
            finally {
                hVol.CloseHandle();
                hFile.CloseHandle();
            }
        }

        public static IntPtr OpenFile( String path ) {
            var hFile = NativeMethods.CreateFile( path, ( System.IO.FileAccess ) ( FileAccess.FILE_READ_DATA | FileAccess.FILE_WRITE_DATA ), FileShare.ReadWrite, IntPtr.Zero,
                FileMode.Open, 0, IntPtr.Zero );

            if ( hFile.IsInvalid ) {
                throw new Exception( Marshal.GetLastWin32Error().ToString() );
            }

            return hFile.DangerousGetHandle();
        }

        public static IntPtr OpenVolume( String deviceName ) {
            var hDevice = NativeMethods.CreateFile( @"\\.\" + deviceName, ( System.IO.FileAccess ) ( FileAccess.FILE_READ_DATA | FileAccess.FILE_WRITE_DATA ), FileShare.Write,
                IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero );

            if ( hDevice.IsInvalid ) {
                throw new Exception( Marshal.GetLastWin32Error().ToString() );
            }

            return hDevice.DangerousGetHandle();
        }

        /// <summary>
        ///     input structure for use in MoveFile
        /// </summary>
        public struct MoveFileData {

            public Int32 ClusterCount;

            public IntPtr HFile;

            public Int64 StartingLcn;

            public Int64 StartingVcn;
        }
    }
}