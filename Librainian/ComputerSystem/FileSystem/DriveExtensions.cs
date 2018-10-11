﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DriveExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "DriveExtensions.cs" was last formatted by Protiguous on 2018/10/05 at 6:03 PM.

namespace Librainian.ComputerSystem.FileSystem {

    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class DriveExtensions {

        public static List<DriveType> FixedDriveTypes {
            get;
        } = new List<DriveType>(new[] {
            DriveType.Fixed
        });

        /// <summary>
        ///     Drives that have "disks" able to be "removed".
        /// </summary>
        public static List<DriveType> FleetingDriveTypes {
            get;
        } = new List<DriveType>(new[] {
            DriveType.Ram, DriveType.Network, DriveType.CDRom, DriveType.Removable
        });

        static DriveExtensions() {
            FleetingDriveTypes.TrimExcess();
            FixedDriveTypes.TrimExcess();
        }

        public static Boolean IsFixed([NotNull] this Disk disk) {
            if (disk == null) {
                throw new ArgumentNullException(paramName: nameof(disk));
            }

            return FixedDriveTypes.Contains(disk.Info.DriveType);
        }

        public static Boolean IsFixed([NotNull] this DriveInfo drive) {
            if (drive == null) {
                throw new ArgumentNullException(paramName: nameof(drive));
            }

            return FixedDriveTypes.Contains(drive.DriveType);
        }

        public static Boolean IsFleeting([NotNull] this Disk disk) {
            if (disk == null) {
                throw new ArgumentNullException(paramName: nameof(disk));
            }

            return FleetingDriveTypes.Contains(disk.Info.DriveType);
        }

        public static Boolean IsFleeting([NotNull] this DriveInfo drive) {
            if (drive == null) {
                throw new ArgumentNullException(paramName: nameof(drive));
            }

            return FleetingDriveTypes.Contains(drive.DriveType);
        }
    }
}