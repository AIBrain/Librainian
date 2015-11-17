// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/DriveExtensions.cs" was last cleaned by Rick on 2015/11/13 at 11:30 PM

namespace Librainian.OperatingSystem.IO {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using FileSystem;

    public static class DriveExtensions {

        public static readonly IList< DriveType > FleetingDriveTypes = new List< DriveType >( new[] {DriveType.Ram, DriveType.Network, DriveType.CDRom, DriveType.Removable} );

        public static readonly IList< DriveType > PermanentDriveTypes = new List< DriveType >( new[] {DriveType.Fixed} );

        public static Boolean IsFleeting( this Drive drive ) => FleetingDriveTypes.Contains( drive.Info.DriveType );

        public static Boolean IsFleeting( this DriveInfo drive ) => FleetingDriveTypes.Contains( drive.DriveType );

        public static Boolean IsMostlyPermanent( this Drive drive ) => PermanentDriveTypes.Contains( drive.Info.DriveType );

        public static Boolean IsMostlyPermanent( this DriveInfo drive ) => PermanentDriveTypes.Contains( drive.DriveType );

    }

}
