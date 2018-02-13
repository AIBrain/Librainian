
namespace Librainian.FileSystem {

    using System;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable once InconsistentNaming
    [DebuggerDisplay("{" + nameof( ToString ) + "(),nq}")]
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