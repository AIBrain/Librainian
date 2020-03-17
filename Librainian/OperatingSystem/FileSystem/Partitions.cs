// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Partitions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Partitions.cs" was last formatted by Protiguous on 2020/03/16 at 5:04 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using JetBrains.Annotations;

    public static class Partitions {

        /// <summary></summary>
        /// <returns></returns>
        /// <remarks>http://www.csharpdeveloping.net/Snippet/how_to_enumerate_disk_partitions</remarks>
        [ItemNotNull]
        public static IEnumerable<Win32DiskPartition> GetDiskPartitions() {

            using ( var searcher = new ManagementObjectSearcher( queryString: "SELECT * FROM Win32_DiskPartition" ) ) {
                var collection = searcher.Get();

                foreach ( var item in collection.OfType<ManagementObject>().Select( selector: obj => new Win32DiskPartition {
                    Access = ( UInt16? ) obj[ propertyName: "Access" ],
                    Availability = ( UInt16? ) obj[ propertyName: "Availability" ],
                    BlockSize = ( UInt64? ) obj[ propertyName: "BlockSize" ],
                    Bootable = ( Boolean? ) obj[ propertyName: "Bootable" ],
                    BootPartition = ( Boolean? ) obj[ propertyName: "BootPartition" ],
                    Caption = ( String ) obj[ propertyName: "Caption" ],
                    ConfigManagerErrorCode = ( UInt32? ) obj[ propertyName: "ConfigManagerErrorCode" ],
                    ConfigManagerUserConfig = ( Boolean? ) obj[ propertyName: "ConfigManagerUserConfig" ],
                    CreationClassName = ( String ) obj[ propertyName: "CreationClassName" ],
                    Description = ( String ) obj[ propertyName: "Description" ],
                    DeviceID = ( String ) obj[ propertyName: "DeviceID" ],
                    DiskIndex = ( UInt32? ) obj[ propertyName: "DiskIndex" ],
                    ErrorCleared = ( Boolean? ) obj[ propertyName: "ErrorCleared" ],
                    ErrorDescription = ( String ) obj[ propertyName: "ErrorDescription" ],
                    ErrorMethodology = ( String ) obj[ propertyName: "ErrorMethodology" ],
                    HiddenSectors = ( UInt32? ) obj[ propertyName: "HiddenSectors" ],
                    Index = ( UInt32? ) obj[ propertyName: "Index" ],
                    InstallDate = ( DateTime? ) obj[ propertyName: "InstallDate" ],
                    LastErrorCode = ( UInt32? ) obj[ propertyName: "LastErrorCode" ],
                    Name = ( String ) obj[ propertyName: "Name" ],
                    NumberOfBlocks = ( UInt64? ) obj[ propertyName: "NumberOfBlocks" ],
                    PNPDeviceID = ( String ) obj[ propertyName: "PNPDeviceID" ],
                    PowerManagementCapabilities = ( UInt16[] ) obj[ propertyName: "PowerManagementCapabilities" ],
                    PowerManagementSupported = ( Boolean? ) obj[ propertyName: "PowerManagementSupported" ],
                    PrimaryPartition = ( Boolean? ) obj[ propertyName: "PrimaryPartition" ],
                    Purpose = ( String ) obj[ propertyName: "Purpose" ],
                    RewritePartition = ( Boolean? ) obj[ propertyName: "RewritePartition" ],
                    Size = ( UInt64? ) obj[ propertyName: "Size" ],
                    StartingOffset = ( UInt64? ) obj[ propertyName: "StartingOffset" ],
                    Status = ( String ) obj[ propertyName: "Status" ],
                    StatusInfo = ( UInt16? ) obj[ propertyName: "StatusInfo" ],
                    SystemCreationClassName = ( String ) obj[ propertyName: "SystemCreationClassName" ],
                    SystemName = ( String ) obj[ propertyName: "SystemName" ],
                    Type = ( String ) obj[ propertyName: "Type" ]
                } ) ) {
                    yield return item;
                }
            }
        }

    }

}