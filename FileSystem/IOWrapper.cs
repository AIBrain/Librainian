// Copyright 2016 Protiguous.
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
// "Librainian/IOWrapper.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using OperatingSystem;

    /// <summary>
    ///     defrag stuff
    /// </summary>
    /// <seealso cref="http://blogs.msdn.com/b/jeffrey_wall/archive/2004/09/13/229137.aspx" />
    public class IOWrapper {
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

                var extentCount = ( Int32 )Marshal.PtrToStructure( pDest, typeof( Int32 ) );

                pDest = ( IntPtr )( ( Int64 )pDest + 4 );

                var startingVcn = ( Int64 )Marshal.PtrToStructure( pDest, typeof( Int64 ) );

                Debug.Assert( startingVcn == 0 );

                pDest = ( IntPtr )( ( Int64 )pDest + 8 );

                // now pDest points at an array of pairs of Int64s.

                var retVal = Array.CreateInstance( typeof( Int64 ), new[] { extentCount, 2 } );

                for ( var i = 0; i < extentCount; i++ ) {
                    for ( var j = 0; j < 2; j++ ) {
                        var v = ( Int64 )Marshal.PtrToStructure( pDest, typeof( Int64 ) );
                        retVal.SetValue( v, new[] { i, j } );
                        pDest = ( IntPtr )( ( Int64 )pDest + 8 );
                    }
                }

                return retVal;
            }
            finally {
                NativeMethods.CloseHandle( hFile );

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
        public static BitArray GetVolumeMap( String deviceName ) {
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
                var startingLcn = ( Int64 )Marshal.PtrToStructure( pDest, typeof( Int64 ) );

                Debug.Assert( startingLcn == 0 );

                pDest = ( IntPtr )( ( Int64 )pDest + 8 );
                var bitmapSize = ( Int64 )Marshal.PtrToStructure( pDest, typeof( Int64 ) );

                var byteSize = ( Int32 )( bitmapSize / 8 );
                byteSize++; // round up - even with no remainder

                var bitmapBegin = ( IntPtr )( ( Int64 )pDest + 8 );

                var byteArr = new Byte[ byteSize ];

                Marshal.Copy( bitmapBegin, byteArr, 0, byteSize );

                var retVal = new BitArray( byteArr ) { Length = ( Int32 )bitmapSize };

                // truncate to exact cluster count
                return retVal;
            }
            finally {
                NativeMethods.CloseHandle( hDevice );

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

                var mfd = new MoveFileData { HFile = hFile, StartingVcn = vcn, StartingLcn = lcn, ClusterCount = count };

                var handle = GCHandle.Alloc( mfd, GCHandleType.Pinned );
                var p = handle.AddrOfPinnedObject();
                var bufSize = ( UInt32 )Marshal.SizeOf( mfd );

				var fResult = NativeMethods.DeviceIoControl( hVol, FSConstants.FsctlMoveFile, p, bufSize, IntPtr.Zero, /* no output data from this FSCTL*/ 0, out var size, IntPtr.Zero );
				if ( !fResult ) {
                    throw new Exception( Marshal.GetLastWin32Error().ToString() );
                }
                handle.Free();

            }
            finally {
                NativeMethods.CloseHandle( hVol );
                NativeMethods.CloseHandle( hFile );
            }
        }

        public static IntPtr OpenFile( String path ) {
            var hFile = NativeMethods.CreateFile( path, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero );
            if ( hFile.IsInvalid ) {
                throw new Exception( Marshal.GetLastWin32Error().ToString() );
            }
            return hFile.DangerousGetHandle();
        }

        public static IntPtr OpenVolume( String deviceName ) {
            var hDevice = NativeMethods.CreateFile( @"\\.\" + deviceName, FileAccess.ReadWrite, FileShare.Write, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero );
            if ( hDevice.IsInvalid ) {
                throw new Exception( Marshal.GetLastWin32Error().ToString() );
            }
            return hDevice.DangerousGetHandle();
        }


        /// <summary>
        ///     input structure for use in MoveFile
        /// </summary>
        private struct MoveFileData {
#pragma warning disable 414
            public Int32 ClusterCount;
#pragma warning restore 414

#pragma warning disable 414
            public IntPtr HFile;
#pragma warning restore 414

#pragma warning disable 414
            public Int64 StartingLcn;
#pragma warning restore 414

#pragma warning disable 414
            public Int64 StartingVcn;
#pragma warning restore 414
        }
    }
}