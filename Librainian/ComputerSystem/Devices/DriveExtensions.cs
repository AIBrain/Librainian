// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "DriveExtensions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "DriveExtensions.cs" was last formatted by Protiguous on 2020/03/16 at 2:53 PM.

namespace Librainian.ComputerSystem.Devices {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using JetBrains.Annotations;

    public static class DriveExtensions {

        public static List<DriveType> FixedDriveTypes { get; } = new List<DriveType>( collection: new[] {
            DriveType.Fixed
        } );

        /// <summary>Drives that have "disks" able to be "removed".</summary>
        public static List<DriveType> FleetingDriveTypes { get; } = new List<DriveType>( collection: new[] {
            DriveType.Ram, DriveType.Network, DriveType.CDRom, DriveType.Removable
        } );

        static DriveExtensions() {
            FleetingDriveTypes.TrimExcess();
            FixedDriveTypes.TrimExcess();
        }

        public static Boolean IsFixed( [NotNull] this Disk disk ) {
            if ( disk is null ) {
                throw new ArgumentNullException( paramName: nameof( disk ) );
            }

            return FixedDriveTypes.Contains( item: disk.Info.DriveType );
        }

        public static Boolean IsFixed( [NotNull] this DriveInfo drive ) {
            if ( drive is null ) {
                throw new ArgumentNullException( paramName: nameof( drive ) );
            }

            return FixedDriveTypes.Contains( item: drive.DriveType );
        }

        public static Boolean IsFleeting( [NotNull] this Disk disk ) {
            if ( disk is null ) {
                throw new ArgumentNullException( paramName: nameof( disk ) );
            }

            return FleetingDriveTypes.Contains( item: disk.Info.DriveType );
        }

        public static Boolean IsFleeting( [NotNull] this DriveInfo drive ) {
            if ( drive is null ) {
                throw new ArgumentNullException( paramName: nameof( drive ) );
            }

            return FleetingDriveTypes.Contains( item: drive.DriveType );
        }
    }
}