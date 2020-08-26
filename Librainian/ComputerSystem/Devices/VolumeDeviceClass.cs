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
// File "VolumeDeviceClass.cs" last formatted on 2020-08-14 at 8:31 PM.

namespace Librainian.ComputerSystem.Devices {

	using System;
	using System.Collections.Generic;
	using System.Text;
	using JetBrains.Annotations;
	using OperatingSystem;
	using OperatingSystem.FileSystem;

	/// <summary>The device class for volume devices.</summary>
	/// <remarks>UsbEject version 1.0 March 2006</remarks>
	/// <remarks>written by Simon Mourier &lt;email: simon [underscore] mourier [at] hotmail [dot] com&gt;</remarks>
	public class VolumeDeviceClass : DeviceClass {

		/// <summary>Initializes a new instance of the VolumeDeviceClass class.</summary>
		public VolumeDeviceClass() : base( new Guid( NativeMethods.GUID_DEVINTERFACE_VOLUME ) ) {
			var sb = new StringBuilder( 1024 );

			foreach ( var drive in Environment.GetLogicalDrives() ) {
				sb.Clear();

				if ( !NativeMethods.GetVolumeNameForVolumeMountPoint( drive, sb, ( UInt32 )sb.Capacity ) ) {
					continue;
				}

				this.LogicalDrives[sb.ToString()] = drive.Replace( @"\", "" );
			}
		}

		[NotNull]
		protected internal SortedDictionary<String, String> LogicalDrives { get; } = new SortedDictionary<String, String>();

		protected override Device CreateDevice( DeviceClass deviceClass, NativeMethods.SP_DEVINFO_DATA deviceInfoData, String path, Int32 index, Int32 disknum = -1 ) =>
			new Volume( deviceClass, deviceInfoData, path, index );

	}

}