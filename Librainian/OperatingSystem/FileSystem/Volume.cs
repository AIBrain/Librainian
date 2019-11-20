// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Volume.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Volume.cs" was last formatted by Protiguous on 2019/08/08 at 9:19 AM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using ComputerSystem.Devices;
    using JetBrains.Annotations;

    /// <summary>
    ///     A volume device.
    /// </summary>
    public class Volume : Device {

        internal Volume( [NotNull] DeviceClass deviceClass, NativeMethods.SP_DEVINFO_DATA deviceInfoData, String path, Int32 index ) : base( deviceClass, deviceInfoData, path,
            index ) { }

        /// <summary>
        ///     Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the comparands.</returns>
        public override Int32 CompareTo( Object obj ) {
            if ( !( obj is Volume device ) ) {
                throw new ArgumentException();
            }

            if ( this.GetLogicalDrive() is null ) {
                return 1;
            }

            if ( device.GetLogicalDrive() is null ) {
                return -1;
            }

            return String.Compare( this.GetLogicalDrive(), device.GetLogicalDrive(), StringComparison.Ordinal );
        }

        [NotNull]
        public IEnumerable<Int32> GetDiskNumbers() {
            var numbers = new List<Int32>();

            if ( this.GetLogicalDrive() != null ) {

                Trace.WriteLine( $"Finding disk extents for volume: {this.GetLogicalDrive()}" );

                var hFile = NativeMethods.CreateFile( $@"\\.\{this.GetLogicalDrive()}", 0, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero );

                if ( hFile.IsInvalid ) {
                    throw new Win32Exception( Marshal.GetLastWin32Error() );
                }

                const Int32 size = 0x400; // some big size
                var buffer = Marshal.AllocHGlobal( size );
                UInt32 bytesReturned;

                try {
                    if ( !NativeMethods.DeviceIoControl( hFile.DangerousGetHandle(), NativeMethods.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, IntPtr.Zero, 0, buffer, size,
                        out bytesReturned, IntPtr.Zero ) ) {

                        // do nothing here on purpose
                    }
                }
                finally {
                    hFile.DangerousGetHandle().CloseHandle();
                }

                if ( bytesReturned > 0 ) {
                    var numberOfDiskExtents = ( Int32 )Marshal.PtrToStructure( buffer, typeof( Int32 ) );

                    for ( var i = 0; i < numberOfDiskExtents; i++ ) {
                        var extentPtr = new IntPtr( buffer.ToInt32() + Marshal.SizeOf( typeof( Int64 ) ) + ( i * Marshal.SizeOf( typeof( NativeMethods.DISK_EXTENT ) ) ) );
                        var extent = ( NativeMethods.DISK_EXTENT )Marshal.PtrToStructure( extentPtr, typeof( NativeMethods.DISK_EXTENT ) );
                        numbers.Add( extent.DiskNumber );
                    }
                }

                Marshal.FreeHGlobal( buffer );
            }

            return numbers;
        }

        /// <summary>
        ///     Gets a list of underlying disks for this volume.
        /// </summary>
        [ItemNotNull]
        public IEnumerable<Device> GetDisks() {

            var disks = new DiskDeviceClass();
            var devices = disks.GetDevices().ToList();

            foreach ( var disk in devices ) {
                foreach ( var _ in this.GetDiskNumbers().Where( index => disk.DiskNumber == index ) ) {
                    yield return disk;
                }
            }
        }

        /// <summary>
        ///     Gets the volume's logical drive in the form [letter]:\
        /// </summary>
        [CanBeNull]
        public String GetLogicalDrive() {
            var volumeName = this.GetVolumeName();
            String logicalDrive = null;

            if ( volumeName != null ) {
                ( this.DeviceClass as VolumeDeviceClass )?.LogicalDrives.TryGetValue( volumeName, out logicalDrive );
            }

            return logicalDrive;
        }

        /// <summary>
        ///     Gets a list of removable devices for this volume.
        /// </summary>
        public override IEnumerable<Device> GetRemovableDevices() {
            if ( this.GetDisks() is null ) {
                foreach ( var removableDevice in base.GetRemovableDevices() ) {
                    yield return removableDevice;
                }
            }
            else {
                foreach ( var disk in this.GetDisks() ) {
                    foreach ( var device in disk.GetRemovableDevices() ) {
                        yield return device;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the volume's name.
        /// </summary>
        [CanBeNull]
        public String GetVolumeName() {
            var sb = new StringBuilder( 1024 );

            if ( !NativeMethods.GetVolumeNameForVolumeMountPoint( $@"{this.Path}\", sb, ( UInt32 )sb.Capacity ) ) {

                // throw new Win32Exception(Marshal.GetLastWin32Error());
                return null;
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Gets a value indicating whether this volume is a based on USB devices.
        /// </summary>
        public override Boolean IsUsb() => this.GetDisks()?.Any( disk => disk.IsUsb() ) == true;
    }
}