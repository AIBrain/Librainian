// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "LogicalDisk.cs",
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
// "Librainian/Librainian/LogicalDisk.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem.Logical {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using OperatingSystem.WMI;

    public class LogicalDisk {

        public String DriveLetter { get; set; }

        public DriveType DriveType { get; set; }

        public UInt64 FreeSpace { get; set; }

        public UInt64 TotalSpace { get; set; }

        public String VolumeName { get; set; }

        public static IEnumerable<LogicalDisk> GetLogicalDisks( String machineName ) {
            foreach ( var o in WMIExtensions.QueryWMI( machineName, "cimv2", "SELECT * FROM Win32_LogicalDisk" ) ) {
                var disk = new LogicalDisk {
                    DriveLetter = o["Name"].ToString(),
                    FreeSpace = Convert.ToUInt64( o["FreeSpace"].ToString() ),
                    TotalSpace = Convert.ToUInt64( o["Size"].ToString() ),
                    VolumeName = o["VolumeName"].ToString(),
                    DriveType = ( DriveType )Convert.ToByte( o["DriveType"].ToString() )
                };

                yield return disk;
            }
        }
    }
}