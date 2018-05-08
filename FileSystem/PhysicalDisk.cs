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
// "Librainian/PhysicalDisk.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.Management;
    using Extensions;

    /// <summary>
    ///     A physical storage medium. HD, usb, dvd, etc...
    /// </summary>
    /// <remarks>http://superuser.com/questions/341497/whats-the-difference-between-a-disk-and-a-drive</remarks>
    [Immutable]
    public class PhysicalDisk {

        public PhysicalDisk( Int32 diskNumber ) {
            //this.SerialNumber = GetSerialNumber( diskNumber );
        }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public String SerialNumber {
            get;
        }

        [Obsolete("not complete at all")]
        public static String GetSerialNumber( Int32 diskNumber ) {

            //TODO
            var mosDisks = new ManagementObjectSearcher( "SELECT * FROM Win32_DiskDrive" );

            // Loop through each object (disk) retrieved by WMI
            foreach ( var o in mosDisks.Get() ) {
	            // Add the HDD to the list (use the Model field as the item's caption)
                if ( o is ManagementObject moDisk ) {
                    return moDisk[ "Model" ].ToString();
                }
            }
            return String.Empty;
        }

    }

}