// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Partitions.cs" last formatted on 2020-08-14 at 8:40 PM.

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
			using var searcher = new ManagementObjectSearcher( "SELECT * FROM Win32_DiskPartition" );

			var collection = searcher.Get();

			foreach ( var item in collection.OfType<ManagementObject>().Select( obj => new Win32DiskPartition {
				Access = ( UInt16? )obj["Access"], Availability = ( UInt16? )obj["Availability"], BlockSize = ( UInt64? )obj["BlockSize"],
				Bootable = ( Boolean? )obj["Bootable"], BootPartition = ( Boolean? )obj["BootPartition"], Caption = ( String )obj["Caption"],
				ConfigManagerErrorCode = ( UInt32? )obj["ConfigManagerErrorCode"], ConfigManagerUserConfig = ( Boolean? )obj["ConfigManagerUserConfig"],
				CreationClassName = ( String )obj["CreationClassName"], Description = ( String )obj["Description"], DeviceID = ( String )obj["DeviceID"],
				DiskIndex = ( UInt32? )obj["DiskIndex"], ErrorCleared = ( Boolean? )obj["ErrorCleared"], ErrorDescription = ( String )obj["ErrorDescription"],
				ErrorMethodology = ( String )obj["ErrorMethodology"], HiddenSectors = ( UInt32? )obj["HiddenSectors"], Index = ( UInt32? )obj["Index"],
				InstallDate = ( DateTime? )obj["InstallDate"], LastErrorCode = ( UInt32? )obj["LastErrorCode"], Name = ( String )obj["Name"],
				NumberOfBlocks = ( UInt64? )obj["NumberOfBlocks"], PNPDeviceID = ( String )obj["PNPDeviceID"],
				PowerManagementCapabilities = ( UInt16[] )obj["PowerManagementCapabilities"], PowerManagementSupported = ( Boolean? )obj["PowerManagementSupported"],
				PrimaryPartition = ( Boolean? )obj["PrimaryPartition"], Purpose = ( String )obj["Purpose"], RewritePartition = ( Boolean? )obj["RewritePartition"],
				Size = ( UInt64? )obj["Size"], StartingOffset = ( UInt64? )obj["StartingOffset"], Status = ( String )obj["Status"], StatusInfo = ( UInt16? )obj["StatusInfo"],
				SystemCreationClassName = ( String )obj["SystemCreationClassName"], SystemName = ( String )obj["SystemName"], Type = ( String )obj["Type"]
			} ) ) {
				yield return item;
			}
		}

	}

}