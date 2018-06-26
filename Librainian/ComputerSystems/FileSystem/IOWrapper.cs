// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "IOWrapper.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "IOWrapper.cs" was last formatted by Protiguous on 2018/06/26 at 12:55 AM.

namespace Librainian.ComputerSystems.FileSystem {

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

				var mfd = new MoveFileData {
					HFile = hFile,
					StartingVcn = vcn,
					StartingLcn = lcn,
					ClusterCount = count
				};

				var handle = GCHandle.Alloc( mfd, GCHandleType.Pinned );
				var p = handle.AddrOfPinnedObject();
				var bufSize = ( UInt32 ) Marshal.SizeOf( mfd );

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
	}
}