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
// "Librainian/LogicalDisk.cs" was last cleaned by Protiguous on 2016/07/11 at 6:18 AM

namespace Librainian.FileSystem.Logical {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using OperatingSystem.WMI;

    public class LogicalDisk {

        public String DriveLetter {
            get; set;
        }

        public String VolumeName {
            get; set;
        }

        public UInt64 TotalSpace {
            get; set;
        }

        public UInt64 FreeSpace {
            get; set;
        }

        public DriveType DriveType {
            get; set;
        }

        public static IEnumerable<LogicalDisk> GetLogicalDisks( String machineName ) {
            foreach ( var o in WMIExtensions.QueryWMI( machineName, "cimv2", "SELECT * FROM Win32_LogicalDisk" ) ) {
                var disk = new LogicalDisk {
                    DriveLetter = o[ "Name" ].ToString(),
                    FreeSpace = Convert.ToUInt64( o[ "FreeSpace" ].ToString() ),
                    TotalSpace = Convert.ToUInt64( o[ "Size" ].ToString() ),
                    VolumeName = o[ "VolumeName" ].ToString(),
                    DriveType = ( DriveType )Convert.ToByte( o[ "DriveType" ].ToString() )
                };
                yield return disk;
            }
        }

    }

}
