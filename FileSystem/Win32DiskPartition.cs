// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Win32DiskPartition.cs",
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
// "Librainian/Librainian/Win32DiskPartition.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

    using System;
    using System.Diagnostics;

    /// <summary>
    /// </summary>
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