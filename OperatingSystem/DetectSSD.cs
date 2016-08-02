// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/DetectSSD.cs" was last cleaned by Rick on 2016/06/23 at 9:32 PM

namespace Librainian.OperatingSystem {

    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Maths;
    using NUnit.Framework;

    public static class DetectSSD {

        /// <summary>
        ///     Returns true if the disk/drive has seek penalty.
        /// </summary>
        /// <param name="diskNumber"></param>
        /// <returns></returns>
        public static Boolean? IncursSeekPenalty( this Byte diskNumber ) {
            var sDrive = @"\\.\PhysicalDrive" + diskNumber;

            var hDrive = NativeWin32.CreateFileW( sDrive, 0, // No access to drive
                                                  NativeWin32.FILE_SHARE_READ | NativeWin32.FILE_SHARE_WRITE, IntPtr.Zero, NativeWin32.OPEN_EXISTING, NativeWin32.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero );

            if ( hDrive == null || hDrive.IsInvalid ) {
                //Debug.WriteLine( "CreateFile failed. " + NativeWin32.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            var IOCTL_STORAGE_QUERY_PROPERTY = NativeWin32.CTL_CODE( NativeWin32.IOCTL_STORAGE_BASE, 0x500, NativeWin32.METHOD_BUFFERED, NativeWin32.FILE_ANY_ACCESS ); // From winioctl.h

            var query_seek_penalty = new NativeWin32.STORAGE_PROPERTY_QUERY { PropertyId = NativeWin32.StorageDeviceSeekPenaltyProperty, QueryType = NativeWin32.PropertyStandardQuery };

            var query_seek_penalty_desc = new NativeWin32.DEVICE_SEEK_PENALTY_DESCRIPTOR();

            UInt32 returned_query_seek_penalty_size;

            var query_seek_penalty_result = NativeWin32.DeviceIoControl( hDrive, IOCTL_STORAGE_QUERY_PROPERTY, ref query_seek_penalty, ( UInt32 )Marshal.SizeOf( query_seek_penalty ), ref query_seek_penalty_desc, ( UInt32 )Marshal.SizeOf( query_seek_penalty_desc ), out returned_query_seek_penalty_size, IntPtr.Zero );

            hDrive.Close();

            if ( !query_seek_penalty_result ) {
                //Debug.WriteLine( "DeviceIoControl failed: " + NativeWin32.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            return query_seek_penalty_desc.IncursSeekPenalty;
        }

        /// <summary>
        ///     Method for nominal media rotation rate (Administrative privilege is required)
        /// </summary>
        /// <param name="diskNumber"></param>
        public static Boolean? IsARotateDevice( this Byte diskNumber ) {
            var sDrive = @"\\.\PhysicalDrive" + diskNumber;

            var hDrive = NativeWin32.CreateFileW( sDrive, NativeWin32.GENERIC_READ | NativeWin32.GENERIC_WRITE, // Administrative privilege is required
                                                  NativeWin32.FILE_SHARE_READ | NativeWin32.FILE_SHARE_WRITE, IntPtr.Zero, NativeWin32.OPEN_EXISTING, NativeWin32.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero );

            if ( hDrive == null || hDrive.IsInvalid ) {
                //Debug.WriteLine( "CreateFile failed. " + NativeWin32.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            var IOCTL_ATA_PASS_THROUGH = NativeWin32.CTL_CODE( NativeWin32.IOCTL_SCSI_BASE, 0x040b, NativeWin32.METHOD_BUFFERED, NativeWin32.FILE_READ_ACCESS | NativeWin32.FILE_WRITE_ACCESS ); // From ntddscsi.h

            var idQuery = new NativeWin32.ATAIdentifyDeviceQuery { data = new UInt16[ 256 ] };

            idQuery.header.Length = ( UInt16 )Marshal.SizeOf( idQuery.header );
            idQuery.header.AtaFlags = ( UInt16 )NativeWin32.ATA_FLAGS_DATA_IN;
            idQuery.header.DataTransferLength = ( UInt32 )( idQuery.data.Length * 2 ); // Size of "data" in bytes
            idQuery.header.TimeOutValue = 3; // Sec
            idQuery.header.DataBufferOffset = Marshal.OffsetOf( typeof( NativeWin32.ATAIdentifyDeviceQuery ), "data" );
            idQuery.header.PreviousTaskFile = new Byte[ 8 ];
            idQuery.header.CurrentTaskFile = new Byte[ 8 ];
            idQuery.header.CurrentTaskFile[ 6 ] = 0xec; // ATA IDENTIFY DEVICE

            UInt32 retvalSize;

            var result = NativeWin32.DeviceIoControl( hDrive, IOCTL_ATA_PASS_THROUGH, ref idQuery, ( UInt32 )Marshal.SizeOf( idQuery ), ref idQuery, ( UInt32 )Marshal.SizeOf( idQuery ), out retvalSize, IntPtr.Zero );

            hDrive.Close();

            if ( result == false ) {
                //Debug.WriteLine( "DeviceIoControl failed. " + NativeWin32.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            // Word index of nominal media rotation rate
            const Int32 kNominalMediaRotRateWordIndex = 217;
            const Int32 NonRotateDevice = 1;

            if ( idQuery.data[ kNominalMediaRotRateWordIndex ] == NonRotateDevice ) {
                //Debug.WriteLine( $"The disk #{diskNumber} is a NON-ROTATE device." );
                return false;
            }

            //Debug.WriteLine( "This disk is ROTATE device." );
            return true;
        }

        public static Boolean? IsDiskSSD( this Byte diskNumber ) {
            var incursSeekPenalty = diskNumber.IncursSeekPenalty();
            var isARotateDevice = diskNumber.IsARotateDevice();
            return incursSeekPenalty != null && !incursSeekPenalty.Value && isARotateDevice != null && !isARotateDevice.Value;
        }

        [Test]
        public static void Test_Search_For_SSD() {
            foreach ( var disk in Byte.MinValue.To( 10 ) ) {
                Debug.WriteLine( $"Disk {disk}: SeekPenalty={disk.IncursSeekPenalty()} rotates={disk.IsARotateDevice()}, so IsSSD={disk.IsDiskSSD()}\r\n" );
            }
        }
    }
}