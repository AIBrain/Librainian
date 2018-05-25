// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DeviceCapabilities.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/DeviceCapabilities.cs" was last formatted by Protiguous on 2018/05/24 at 7:01 PM.

namespace Librainian.ComputerSystems.Devices {

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