namespace Librainian.FileSystem.Physical {

    using System;

    /// <summary>
    ///     Contains constants for determining devices capabilities.
    ///     This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.
    /// </summary>
    [Flags]
    public enum DeviceCapabilities {
        Unknown = 0x00000000,

        // matches cfmgr32.h CM_DEVCAP_* definitions

        LockSupported = 0x00000001,
        EjectSupported = 0x00000002,
        Removable = 0x00000004,
        DockDevice = 0x00000008,
        UniqueId = 0x00000010,
        SilentInstall = 0x00000020,
        RawDeviceOk = 0x00000040,
        SurpriseRemovalOk = 0x00000080,
        HardwareDisabled = 0x00000100,
        NonDynamic = 0x00000200
    }

}