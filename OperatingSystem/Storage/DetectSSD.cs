// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DetectSSD.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/DetectSSD.cs" was last formatted by Protiguous on 2018/05/24 at 7:29 PM.

namespace Librainian.OperatingSystem.Storage {

    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Maths;
    using NUnit.Framework;

    public static class DetectSSD {

        private static UInt32 CTL_CODE( UInt32 deviceType, UInt32 function, UInt32 method, UInt32 access ) => ( deviceType << 16 ) | ( access << 14 ) | ( function << 2 ) | method;

        /// <summary>
        ///     Returns true if the disk/drive has seek penalty.
        /// </summary>
        /// <param name="diskNumber"></param>
        /// <returns></returns>
        public static Boolean? IncursSeekPenalty( this Byte diskNumber ) {
            var sDrive = @"\\.\PhysicalDrive" + diskNumber;

            var hDrive = NativeMethods.CreateFileW( sDrive, 0, // No access to drive
                NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, NativeMethods.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero );

            if ( hDrive is null || hDrive.IsInvalid ) {

                //Debug.WriteLine( "CreateFile failed. " + NativeMethods.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            var IOCTL_STORAGE_QUERY_PROPERTY = CTL_CODE( NativeMethods.IOCTL_STORAGE_BASE, 0x500, NativeMethods.METHOD_BUFFERED, NativeMethods.FILE_ANY_ACCESS ); // From winioctl.h

            var query_seek_penalty = new NativeMethods.STORAGE_PROPERTY_QUERY { PropertyId = NativeMethods.StorageDeviceSeekPenaltyProperty, QueryType = NativeMethods.PropertyStandardQuery };

            var query_seek_penalty_desc = new NativeMethods.DEVICE_SEEK_PENALTY_DESCRIPTOR();

            var querySeekPenaltyResult = NativeMethods.DeviceIoControl( hDrive, IOCTL_STORAGE_QUERY_PROPERTY, ref query_seek_penalty, ( UInt32 )Marshal.SizeOf( query_seek_penalty ), ref query_seek_penalty_desc,
                ( UInt32 )Marshal.SizeOf( query_seek_penalty_desc ), out var returned_query_seek_penalty_size, IntPtr.Zero );

            hDrive.Close();

            if ( !querySeekPenaltyResult ) {

                //Debug.WriteLine( "DeviceIoControl failed: " + NativeMethods.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            return query_seek_penalty_desc.IncursSeekPenalty;
        }

        public static Boolean? IsDiskSSD( this Byte diskNumber ) {

            //test 1
            var incursSeekPenalty = diskNumber.IncursSeekPenalty();

            if ( incursSeekPenalty != null && !incursSeekPenalty.Value ) { return true; }

            //test 2 (must be admin)
            var isARotateDevice = diskNumber.IsRotateDevice();

            if ( isARotateDevice != null && !isARotateDevice.Value ) { return true; }

            return null; //could not determine
        }

        /// <summary>
        ///     Method for nominal media rotation rate (Administrative privilege is required)
        /// </summary>
        /// <param name="diskNumber"></param>
        public static Boolean? IsRotateDevice( this Byte diskNumber ) {
            var sDrive = @"\\.\PhysicalDrive" + diskNumber;

            var hDrive = NativeMethods.CreateFileW( sDrive, NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, // Administrative privilege is required
                NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, NativeMethods.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero );

            if ( hDrive is null || hDrive.IsInvalid ) {

                //Debug.WriteLine( "CreateFile failed. " + NativeMethods.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            var ioctlAtaPassThrough = CTL_CODE( NativeMethods.IOCTL_SCSI_BASE, 0x040b, NativeMethods.METHOD_BUFFERED, NativeMethods.FILE_READ_ACCESS | NativeMethods.FILE_WRITE_ACCESS ); // From ntddscsi.h

            var idQuery = new NativeMethods.ATAIdentifyDeviceQuery { data = new UInt16[256] };

            idQuery.header.Length = ( UInt16 )Marshal.SizeOf( idQuery.header );
            idQuery.header.AtaFlags = ( UInt16 )NativeMethods.ATA_FLAGS_DATA_IN;
            idQuery.header.DataTransferLength = ( UInt32 )( idQuery.data.Length * 2 ); // Size of "data" in bytes
            idQuery.header.TimeOutValue = 3; // Sec
            idQuery.header.DataBufferOffset = Marshal.OffsetOf( typeof( NativeMethods.ATAIdentifyDeviceQuery ), "data" );
            idQuery.header.PreviousTaskFile = new Byte[8];
            idQuery.header.CurrentTaskFile = new Byte[8];
            idQuery.header.CurrentTaskFile[6] = 0xec; // ATA IDENTIFY DEVICE

            var result = NativeMethods.DeviceIoControl( hDrive, ioctlAtaPassThrough, ref idQuery, ( UInt32 )Marshal.SizeOf( idQuery ), ref idQuery, ( UInt32 )Marshal.SizeOf( idQuery ), out var retvalSize,
                IntPtr.Zero );

            hDrive.Close();

            if ( !result ) {

                //Debug.WriteLine( "DeviceIoControl failed. " + NativeMethods.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            // Word index of nominal media rotation rate
            const Int32 kNominalMediaRotRateWordIndex = 217;
            const Int32 nonRotateDevice = 1;

            if ( idQuery.data[kNominalMediaRotRateWordIndex] == nonRotateDevice ) {

                //Debug.WriteLine( $"The disk #{diskNumber} is a NON-ROTATE device." );
                return false;
            }

            //Debug.WriteLine( "This disk is ROTATE device." );
            return true;
        }

        [Test]
        public static void Test_Search_For_SSD() {
            foreach ( var disk in Byte.MinValue.To( 10 ) ) {
                var isp = disk.IncursSeekPenalty();

                if ( isp.HasValue && !isp.Value ) { Debug.WriteLine( $"Disk {disk} is an SSD." ); }
                else { Debug.WriteLine( $"Disk {disk} is not an SSD." ); }
            }
        }
    }
}