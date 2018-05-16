// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "VolumeDeviceClass.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/VolumeDeviceClass.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

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

                if ( !NativeMethods.GetVolumeNameForVolumeMountPoint( drive, sb, ( UInt32 )sb.Capacity ) ) { continue; }

                this.LogicalDrives[sb.ToString()] = drive.Replace( "\\", "" );

                //Debug.WriteLine( drive + " ==> " + sb );
            }
        }

        protected internal SortedDictionary<String, String> LogicalDrives { get; } = new SortedDictionary<String, String>();

        protected override Device CreateDevice( DeviceClass deviceClass, NativeMethods.SP_DEVINFO_DATA deviceInfoData, String path, Int32 index, Int32 disknum = -1 ) =>
            new Volume( deviceClass, deviceInfoData, path, index );
    }
}