// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FSConstants.cs" belongs to Rick@AIBrain.org and
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
//
// "Librainian/Librainian/FSConstants.cs" was last formatted by Protiguous on 2018/05/21 at 9:55 PM.

namespace Librainian.ComputerSystems.FileSystem {

    using System;

    /// <summary>
    ///     constants lifted from winioctl.h from platform sdk
    /// </summary>
    internal class FSConstants {

        private const UInt32 FileAnyAccess = 0;

        private const UInt32 FileDeviceFileSystem = 0x00000009;

        private const UInt32 FileSpecialAccess = FileAnyAccess;

        private const UInt32 MethodBuffered = 0;

        private const UInt32 MethodNeither = 3;

        public static UInt32 FsctlGetRetrievalPointers = CTL_CODE( FileDeviceFileSystem, 28, MethodNeither, FileAnyAccess );

        public static UInt32 FsctlGetVolumeBitmap = CTL_CODE( FileDeviceFileSystem, 27, MethodNeither, FileAnyAccess );

        public static UInt32 FsctlMoveFile = CTL_CODE( FileDeviceFileSystem, 29, MethodBuffered, FileSpecialAccess );

        private static UInt32 CTL_CODE( UInt32 deviceType, UInt32 function, UInt32 method, UInt32 access ) => ( deviceType << 16 ) | ( access << 14 ) | ( function << 2 ) | method;
    }
}