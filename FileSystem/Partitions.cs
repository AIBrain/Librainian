// Copyright 2016 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Partitions.cs" was last cleaned by Rick on 2016/07/29 at 9:37 AM

namespace Librainian.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using NUnit.Framework;

    public static class Partitions {

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks>http://www.csharpdeveloping.net/Snippet/how_to_enumerate_disk_partitions</remarks>
        public static IEnumerable< Win32DiskPartition > GetDiskPartitions() {
            //var items = new List<Win32_DiskPartition>();

            using ( var searcher = new ManagementObjectSearcher( "SELECT * FROM Win32_DiskPartition" ) ) {
                var collection = searcher.Get();

                foreach ( var obj in collection.OfType< ManagementObject >() ) {
                    var item = new Win32DiskPartition {
                        Access = ( UInt16? )obj[ "Access" ],
                        Availability = ( UInt16? )obj[ "Availability" ],
                        BlockSize = ( UInt64? )obj[ "BlockSize" ],
                        Bootable = ( Boolean? )obj[ "Bootable" ],
                        BootPartition = ( Boolean? )obj[ "BootPartition" ],
                        Caption = ( String )obj[ "Caption" ],
                        ConfigManagerErrorCode = ( UInt32? )obj[ "ConfigManagerErrorCode" ],
                        ConfigManagerUserConfig = ( Boolean? )obj[ "ConfigManagerUserConfig" ],
                        CreationClassName = ( String )obj[ "CreationClassName" ],
                        Description = ( String )obj[ "Description" ],
                        DeviceID = ( String )obj[ "DeviceID" ],
                        DiskIndex = ( UInt32? )obj[ "DiskIndex" ],
                        ErrorCleared = ( Boolean? )obj[ "ErrorCleared" ],
                        ErrorDescription = ( String )obj[ "ErrorDescription" ],
                        ErrorMethodology = ( String )obj[ "ErrorMethodology" ],
                        HiddenSectors = ( UInt32? )obj[ "HiddenSectors" ],
                        Index = ( UInt32? )obj[ "Index" ],
                        InstallDate = ( DateTime? )obj[ "InstallDate" ],
                        LastErrorCode = ( UInt32? )obj[ "LastErrorCode" ],
                        Name = ( String )obj[ "Name" ],
                        NumberOfBlocks = ( UInt64? )obj[ "NumberOfBlocks" ],
                        PNPDeviceID = ( String )obj[ "PNPDeviceID" ],
                        PowerManagementCapabilities = ( UInt16[] )obj[ "PowerManagementCapabilities" ],
                        PowerManagementSupported = ( Boolean? )obj[ "PowerManagementSupported" ],
                        PrimaryPartition = ( Boolean? )obj[ "PrimaryPartition" ],
                        Purpose = ( String )obj[ "Purpose" ],
                        RewritePartition = ( Boolean? )obj[ "RewritePartition" ],
                        Size = ( UInt64? )obj[ "Size" ],
                        StartingOffset = ( UInt64? )obj[ "StartingOffset" ],
                        Status = ( String )obj[ "Status" ],
                        StatusInfo = ( UInt16? )obj[ "StatusInfo" ],
                        SystemCreationClassName = ( String )obj[ "SystemCreationClassName" ],
                        SystemName = ( String )obj[ "SystemName" ],
                        Type = ( String )obj[ "Type" ]
                    };

                    //items.Add( item );
                    yield return item;
                }

                //return items.OrderBy( partition => partition.Name ).ToList();
            }
        }

    }

    [ TestFixture ]
    public static class TestPartitions {

        [ Test ]
        public static void ListAllPartitions() {
            var partitions = Partitions.GetDiskPartitions().OrderBy( partition => partition.DiskIndex ).ThenBy( partition => partition.StartingOffset ).ToList();
            foreach ( var partition in partitions ) {
                Console.WriteLine( partition );
            }
        }

    }

}
