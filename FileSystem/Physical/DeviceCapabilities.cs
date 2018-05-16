// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DeviceCapabilities.cs",
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
// "Librainian/Librainian/DeviceCapabilities.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

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