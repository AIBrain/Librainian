// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "PhysicalDisk.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "PhysicalDisk.cs" was last formatted by Protiguous on 2018/07/13 at 1:33 AM.

namespace Librainian.OperatingSystem.Storage {

	using System;
	using System.Management;
	using Extensions;
	using JetBrains.Annotations;

	/// <summary>
	///     A physical storage medium. HD, usb, dvd, etc...
	/// </summary>
	/// <remarks>http://superuser.com/questions/341497/whats-the-difference-between-a-disk-and-a-drive</remarks>
	[Immutable]
	public class PhysicalDisk {

		// ReSharper disable once UnassignedGetOnlyAutoProperty
		public String SerialNumber { get; }

		public PhysicalDisk( Int32 diskNumber ) {

			//this.SerialNumber = GetSerialNumber( diskNumber );
		}

		[NotNull]
		[Obsolete( "not complete at all" )]
		public static String GetSerialNumber( Int32 diskNumber ) {

			//TODO
			var mosDisks = new ManagementObjectSearcher( "SELECT * FROM Win32_DiskDrive" );

			// Loop through each object (disk) retrieved by WMI
			foreach ( var o in mosDisks.Get() ) {

				// Add the HDD to the list (use the Model field as the item's caption)
				if ( o is ManagementObject moDisk ) { return moDisk[ "Model" ].ToString(); }
			}

			return String.Empty;
		}
	}
}