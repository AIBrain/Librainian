namespace Librainian.FileSystem.Physical {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using JetBrains.Annotations;
    using OperatingSystem;

    /// <summary>
    ///     A volume device.
    /// </summary>
    public class Volume : Device {

        internal Volume( DeviceClass deviceClass, NativeMethods.SP_DEVINFO_DATA deviceInfoData, String path, Int32 index ) : base( deviceClass, deviceInfoData, path, index ) { }

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

        public IEnumerable<Int32> GetDiskNumbers() {
            var numbers = new List<Int32>();

            if ( this.GetLogicalDrive() != null ) {

                Debug.WriteLine( "Finding disk extents for volume: " + this.GetLogicalDrive() );

                var hFile = NativeMethods.CreateFile( @"\\.\" + this.GetLogicalDrive(), 0, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero );
                if ( hFile.IsInvalid ) {
                    throw new Win32Exception( Marshal.GetLastWin32Error() );
                }

                const Int32 size = 0x400; // some big size
                var buffer = Marshal.AllocHGlobal( size );
                UInt32 bytesReturned;
                try {
                    if ( !NativeMethods.DeviceIoControl( hFile.DangerousGetHandle(), NativeMethods.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, IntPtr.Zero, 0, buffer, size, out bytesReturned, IntPtr.Zero ) ) {
                        // do nothing here on purpose
                    }
                }
                finally {
                    NativeMethods.CloseHandle( hFile.DangerousGetHandle() );
                }

                if ( bytesReturned > 0 ) {
                    var numberOfDiskExtents = ( Int32 )Marshal.PtrToStructure( buffer, typeof( Int32 ) );
                    for ( var i = 0; i < numberOfDiskExtents; i++ ) {
                        var extentPtr = new IntPtr( buffer.ToInt32() + Marshal.SizeOf( typeof( Int64 ) ) + i * Marshal.SizeOf( typeof( NativeMethods.DISK_EXTENT ) ) );
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
        public IEnumerable<Device> GetDisks() {
            if ( this.GetDiskNumbers() is null ) {
                yield break;
            }

            var disks = new DiskDeviceClass();
            foreach ( var index in this.GetDiskNumbers() ) {
                foreach ( var disk in disks.GetDevices() ) {
                    if ( disk.DiskNumber == index ) {
                        yield return disk;
                    }
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
            if ( !NativeMethods.GetVolumeNameForVolumeMountPoint( this.Path + "\\", sb, ( UInt32 )sb.Capacity ) ) {
                // throw new Win32Exception(Marshal.GetLastWin32Error());
                return null;
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Gets a value indicating whether this volume is a based on USB devices.
        /// </summary>
        public override Boolean IsUsb() {
            if ( this.GetDisks() != null ) {
                foreach ( var disk in this.GetDisks() ) {
                    //TODO
                    if ( disk.IsUsb() ) {
                        return true;
                    }
                }
            }

            return false;
        }

    }

}