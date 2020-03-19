﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Win32DiskPartition.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Win32DiskPartition.cs" was last formatted by Protiguous on 2020/03/18 at 10:26 AM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Diagnostics;

    /// <summary></summary>

    // ReSharper disable once InconsistentNaming
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class Win32DiskPartition {

        public UInt16? Access;

        public UInt16? Availability;

        public UInt64? BlockSize;

        public Boolean? Bootable;

        public Boolean? BootPartition;

        public String Caption;

        public UInt32? ConfigManagerErrorCode;

        public Boolean? ConfigManagerUserConfig;

        public String CreationClassName;

        public String Description;

        public String DeviceID;

        public UInt32? DiskIndex;

        public Boolean? ErrorCleared;

        public String ErrorDescription;

        public String ErrorMethodology;

        public UInt32? HiddenSectors;

        public UInt32? Index;

        public DateTime? InstallDate;

        public UInt32? LastErrorCode;

        public String Name;

        public UInt64? NumberOfBlocks;

        public String PNPDeviceID;

        public UInt16[] PowerManagementCapabilities;

        public Boolean? PowerManagementSupported;

        public Boolean? PrimaryPartition;

        public String Purpose;

        public Boolean? RewritePartition;

        public UInt64? Size;

        public UInt64? StartingOffset;

        public String Status;

        public UInt16? StatusInfo;

        public String SystemCreationClassName;

        public String SystemName;

        public String Type;

        public override String ToString() => this.Name;

    }

}