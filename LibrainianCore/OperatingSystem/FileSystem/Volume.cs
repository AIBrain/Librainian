// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Volume.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "Volume.cs" was last formatted by Protiguous on 2020/03/16 at 3:10 PM.

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

    /// <summary>A volume device.</summary>
    public class Volume : Device {

        internal Volume( [NotNull] DeviceClass deviceClass, NativeMethods.SP_DEVINFO_DATA deviceInfoData, [CanBeNull] String path, Int32 index ) : base(
            deviceClass: deviceClass, deviceInfoData: deviceInfoData, path: path, index: index ) { }

        /// <summary>Compares the current instance with another object of the same type.</summary>
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

            return String.Compare( strA: this.GetLogicalDrive(), strB: device.GetLogicalDrive(), comparisonType: StringComparison.Ordinal );
        }

        [NotNull]
        public IEnumerable<Int32> GetDiskNumbers() {
            var numbers = new List<Int32>();

            if ( this.GetLogicalDrive() != null ) {

                Trace.WriteLine( message: $"Finding disk extents for volume: {this.GetLogicalDrive()}" );

                var hFile = NativeMethods.CreateFile( lpFileName: $@"\\.\{this.GetLogicalDrive()}", dwDesiredAccess: 0, dwShareMode: FileShare.ReadWrite,
                    lpSecurityAttributes: IntPtr.Zero, dwCreationDisposition: FileMode.Open, dwFlagsAndAttributes: 0, hTemplateFile: IntPtr.Zero );

                if ( hFile.IsInvalid ) {
                    throw new Win32Exception( error: Marshal.GetLastWin32Error() );
                }

                const Int32 size = 0x400; // some big size
                var buffer = Marshal.AllocHGlobal( cb: size );
                UInt32 bytesReturned;

                try {
                    if ( !NativeMethods.DeviceIoControl( hDevice: hFile.DangerousGetHandle(), dwIoControlCode: NativeMethods.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS,
                        lpInBuffer: IntPtr.Zero, nInBufferSize: 0, lpOutBuffer: buffer, nOutBufferSize: size, lpBytesReturned: out bytesReturned,
                        lpOverlapped: IntPtr.Zero ) ) {

                        // do nothing here on purpose
                    }
                }
                finally {
                    hFile.DangerousGetHandle().CloseHandle();
                }

                if ( bytesReturned > 0 ) {
                    var numberOfDiskExtents = ( Int32 ) Marshal.PtrToStructure( ptr: buffer, structureType: typeof( Int32 ) );

                    for ( var i = 0; i < numberOfDiskExtents; i++ ) {
                        var extentPtr = new IntPtr( value: buffer.ToInt32() + Marshal.SizeOf( t: typeof( Int64 ) ) +
                                                           i * Marshal.SizeOf( t: typeof( NativeMethods.DISK_EXTENT ) ) );

                        var extent = ( NativeMethods.DISK_EXTENT ) Marshal.PtrToStructure( ptr: extentPtr, structureType: typeof( NativeMethods.DISK_EXTENT ) );
                        numbers.Add( item: extent.DiskNumber );
                    }
                }

                Marshal.FreeHGlobal( hglobal: buffer );
            }

            return numbers;
        }

        /// <summary>Gets a list of underlying disks for this volume.</summary>
        [ItemNotNull]
        public IEnumerable<Device> GetDisks() {

            using var disks = new DiskDeviceClass();

            foreach ( var device in disks.GetDevices() ) {
                if ( !( device is null ) ) {
                    foreach ( var _ in this.GetDiskNumbers().Where( predicate: index => device.DiskNumber == index ) ) {
                        yield return device;
                    }
                }
            }
        }

        /// <summary>Gets the volume's logical drive in the form [letter]:\</summary>
        [CanBeNull]
        public String? GetLogicalDrive() {
            var volumeName = this.GetVolumeName();
            String logicalDrive = null;

            if ( volumeName != null ) {
                ( this.DeviceClass as VolumeDeviceClass )?.LogicalDrives.TryGetValue( key: volumeName, value: out logicalDrive );
            }

            return logicalDrive;
        }

        /// <summary>Gets a list of removable devices for this volume.</summary>
        [ItemNotNull]
        [NotNull]
        public override IEnumerable<Device> GetRemovableDevices() => this.GetDisks().SelectMany( selector: disk => disk.GetRemovableDevices() );

        /// <summary>Gets the volume's name.</summary>
        [CanBeNull]
        public String? GetVolumeName() {
            var sb = new StringBuilder( capacity: 1024 );

            if ( !NativeMethods.GetVolumeNameForVolumeMountPoint( volumeName: $@"{this.Path}\", uniqueVolumeName: sb, uniqueNameBufferCapacity: ( UInt32 ) sb.Capacity ) ) {

                // throw new Win32Exception(Marshal.GetLastWin32Error());
                return null;
            }

            return sb.ToString();
        }

        /// <summary>Gets a value indicating whether this volume is a based on USB devices.</summary>
        public override Boolean IsUsb() => this.GetDisks().Any( predicate: disk => disk.IsUsb() );

    }

}