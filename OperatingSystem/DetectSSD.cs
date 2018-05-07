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
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/DetectSSD.cs" was last cleaned by Protiguous on 2016/06/23 at 9:32 PM

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

            var hDrive = NativeMethods.CreateFileW( sDrive, 0, // No access to drive
                                                  NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, NativeMethods.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero );

            if ( hDrive is null || hDrive.IsInvalid ) {
                //Debug.WriteLine( "CreateFile failed. " + NativeMethods.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            var IOCTL_STORAGE_QUERY_PROPERTY = CTL_CODE( NativeMethods.IOCTL_STORAGE_BASE, 0x500, NativeMethods.METHOD_BUFFERED, NativeMethods.FILE_ANY_ACCESS ); // From winioctl.h

            var query_seek_penalty = new NativeMethods.STORAGE_PROPERTY_QUERY { PropertyId = NativeMethods.StorageDeviceSeekPenaltyProperty, QueryType = NativeMethods.PropertyStandardQuery };

            var query_seek_penalty_desc = new NativeMethods.DEVICE_SEEK_PENALTY_DESCRIPTOR();


			var querySeekPenaltyResult = NativeMethods.DeviceIoControl( hDrive, IOCTL_STORAGE_QUERY_PROPERTY, ref query_seek_penalty, ( UInt32 )Marshal.SizeOf( query_seek_penalty ), ref query_seek_penalty_desc, ( UInt32 )Marshal.SizeOf( query_seek_penalty_desc ), out var returned_query_seek_penalty_size, IntPtr.Zero );

			hDrive.Close();

            if ( !querySeekPenaltyResult ) {
                //Debug.WriteLine( "DeviceIoControl failed: " + NativeMethods.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            return query_seek_penalty_desc.IncursSeekPenalty;
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

            var idQuery = new NativeMethods.ATAIdentifyDeviceQuery { data = new UInt16[ 256 ] };

            idQuery.header.Length = ( UInt16 )Marshal.SizeOf( idQuery.header );
            idQuery.header.AtaFlags = ( UInt16 )NativeMethods.ATA_FLAGS_DATA_IN;
            idQuery.header.DataTransferLength = ( UInt32 )( idQuery.data.Length * 2 ); // Size of "data" in bytes
            idQuery.header.TimeOutValue = 3; // Sec
            idQuery.header.DataBufferOffset = Marshal.OffsetOf( typeof( NativeMethods.ATAIdentifyDeviceQuery ), "data" );
            idQuery.header.PreviousTaskFile = new Byte[ 8 ];
            idQuery.header.CurrentTaskFile = new Byte[ 8 ];
            idQuery.header.CurrentTaskFile[ 6 ] = 0xec; // ATA IDENTIFY DEVICE


			var result = NativeMethods.DeviceIoControl( hDrive, ioctlAtaPassThrough, ref idQuery, ( UInt32 )Marshal.SizeOf( idQuery ), ref idQuery, ( UInt32 )Marshal.SizeOf( idQuery ), out var retvalSize, IntPtr.Zero );

			hDrive.Close();

            if ( !result ) {
                //Debug.WriteLine( "DeviceIoControl failed. " + NativeMethods.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            // Word index of nominal media rotation rate
            const Int32 kNominalMediaRotRateWordIndex = 217;
            const Int32 nonRotateDevice = 1;

            if ( idQuery.data[ kNominalMediaRotRateWordIndex ] == nonRotateDevice ) {
                //Debug.WriteLine( $"The disk #{diskNumber} is a NON-ROTATE device." );
                return false;
            }

            //Debug.WriteLine( "This disk is ROTATE device." );
            return true;
        }

        public static Boolean? IsDiskSSD( this Byte diskNumber ) {

            //test 1
            var incursSeekPenalty = diskNumber.IncursSeekPenalty();
            if ( incursSeekPenalty != null && !incursSeekPenalty.Value ) {
                return true;
            }

            //test 2 (must be admin)
            var isARotateDevice = diskNumber.IsRotateDevice();
            if ( isARotateDevice != null && !isARotateDevice.Value ) {
                return true;
            }


            return null;    //could not determine
        }

        [Test]
        public static void Test_Search_For_SSD() {
            foreach ( var disk in Byte.MinValue.To( 10 ) ) {
                var isp = disk.IncursSeekPenalty();
                if ( isp.HasValue && !isp.Value ) {
                    Debug.WriteLine( $"Disk {disk} is an SSD." );
                }
                else {
                    Debug.WriteLine( $"Disk {disk} is not an SSD." );
                }
            }
        }

        private static UInt32 CTL_CODE( UInt32 deviceType, UInt32 function, UInt32 method, UInt32 access ) => ( deviceType << 16 ) | ( access << 14 ) | ( function << 2 ) | method;

    }
}