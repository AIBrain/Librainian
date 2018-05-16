// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Partitions.cs",
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
// "Librainian/Librainian/Partitions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

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
        public static IEnumerable<Win32DiskPartition> GetDiskPartitions() {

            //var items = new List<Win32_DiskPartition>();

            using ( var searcher = new ManagementObjectSearcher( "SELECT * FROM Win32_DiskPartition" ) ) {
                var collection = searcher.Get();

                foreach ( var obj in collection.OfType<ManagementObject>() ) {
                    var item = new Win32DiskPartition {
                        Access = ( UInt16? )obj["Access"],
                        Availability = ( UInt16? )obj["Availability"],
                        BlockSize = ( UInt64? )obj["BlockSize"],
                        Bootable = ( Boolean? )obj["Bootable"],
                        BootPartition = ( Boolean? )obj["BootPartition"],
                        Caption = ( String )obj["Caption"],
                        ConfigManagerErrorCode = ( UInt32? )obj["ConfigManagerErrorCode"],
                        ConfigManagerUserConfig = ( Boolean? )obj["ConfigManagerUserConfig"],
                        CreationClassName = ( String )obj["CreationClassName"],
                        Description = ( String )obj["Description"],
                        DeviceID = ( String )obj["DeviceID"],
                        DiskIndex = ( UInt32? )obj["DiskIndex"],
                        ErrorCleared = ( Boolean? )obj["ErrorCleared"],
                        ErrorDescription = ( String )obj["ErrorDescription"],
                        ErrorMethodology = ( String )obj["ErrorMethodology"],
                        HiddenSectors = ( UInt32? )obj["HiddenSectors"],
                        Index = ( UInt32? )obj["Index"],
                        InstallDate = ( DateTime? )obj["InstallDate"],
                        LastErrorCode = ( UInt32? )obj["LastErrorCode"],
                        Name = ( String )obj["Name"],
                        NumberOfBlocks = ( UInt64? )obj["NumberOfBlocks"],
                        PNPDeviceID = ( String )obj["PNPDeviceID"],
                        PowerManagementCapabilities = ( UInt16[] )obj["PowerManagementCapabilities"],
                        PowerManagementSupported = ( Boolean? )obj["PowerManagementSupported"],
                        PrimaryPartition = ( Boolean? )obj["PrimaryPartition"],
                        Purpose = ( String )obj["Purpose"],
                        RewritePartition = ( Boolean? )obj["RewritePartition"],
                        Size = ( UInt64? )obj["Size"],
                        StartingOffset = ( UInt64? )obj["StartingOffset"],
                        Status = ( String )obj["Status"],
                        StatusInfo = ( UInt16? )obj["StatusInfo"],
                        SystemCreationClassName = ( String )obj["SystemCreationClassName"],
                        SystemName = ( String )obj["SystemName"],
                        Type = ( String )obj["Type"]
                    };

                    //items.Add( item );
                    yield return item;
                }

                //return items.OrderBy( partition => partition.Name ).ToList();
            }
        }
    }

    [TestFixture]
    public static class TestPartitions {

        [Test]
        public static void ListAllPartitions() {
            var partitions = Partitions.GetDiskPartitions().OrderBy( partition => partition.DiskIndex ).ThenBy( partition => partition.StartingOffset ).ToList();

            foreach ( var partition in partitions ) { Console.WriteLine( partition ); }
        }
    }
}