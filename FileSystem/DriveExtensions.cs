// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks goes
// to the Authors.
//
// Donations and royalties can be paid via
// 
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// 
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/DriveExtensions.cs" was last cleaned by Protiguous on 2016/12/02 at 8:45 PM

namespace Librainian.FileSystem {

	using Collections;
	using System;
	using System.Collections.Generic;
	using System.IO;

	public static class DriveExtensions {
		public static readonly List<DriveType> FixedDriveTypes = new List<DriveType>( new[] { DriveType.Fixed } );
		public static readonly List<DriveType> FleetingDriveTypes = new List<DriveType>( new[] { DriveType.Ram, DriveType.Network, DriveType.CDRom, DriveType.Removable } );

		static DriveExtensions() {
			FleetingDriveTypes.Fix();
			FixedDriveTypes.Fix();
		}

		public static Boolean IsFixed( this Drive drive ) => FixedDriveTypes.Contains( drive.Info.DriveType );

		public static Boolean IsFixed( this DriveInfo drive ) => FixedDriveTypes.Contains( drive.DriveType );

		public static Boolean IsFleeting( this Drive drive ) => FleetingDriveTypes.Contains( drive.Info.DriveType );

		public static Boolean IsFleeting( this DriveInfo drive ) => FleetingDriveTypes.Contains( drive.DriveType );
	}
}