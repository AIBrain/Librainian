namespace Librainian.FileSystem.Physical {

    using System;
    using System.Collections.Generic;
    using System.Text;
    using OperatingSystem;

    /// <summary>
    ///     The device class for volume devices.
    /// </summary>
    /// <remarks>UsbEject version 1.0 March 2006</remarks>
    /// <remarks>written by Simon Mourier &lt;email: simon [underscore] mourier [at] hotmail [dot] com&gt;</remarks>
    public class VolumeDeviceClass : DeviceClass {

        /// <summary>
        ///     Initializes a new instance of the VolumeDeviceClass class.
        /// </summary>
        public VolumeDeviceClass() : base( new Guid( NativeMethods.GUID_DEVINTERFACE_VOLUME ) ) {
            var sb = new StringBuilder( 1024 );
            foreach ( var drive in Environment.GetLogicalDrives() ) {
                sb.Clear();
                if ( !NativeMethods.GetVolumeNameForVolumeMountPoint( drive, sb, ( UInt32 )sb.Capacity ) ) {
                    continue;
                }

                this.LogicalDrives[ sb.ToString() ] = drive.Replace( "\\", "" );

                //Debug.WriteLine( drive + " ==> " + sb );
            }
        }

        protected internal SortedDictionary<String, String> LogicalDrives { get; } = new SortedDictionary<String, String>();

        protected override Device CreateDevice( DeviceClass deviceClass, NativeMethods.SP_DEVINFO_DATA deviceInfoData, String path, Int32 index, Int32 disknum = -1 ) => new Volume( deviceClass, deviceInfoData, path, index );

    }

}