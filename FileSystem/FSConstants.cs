// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FSConstants.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;

    /// <summary>
    ///     constants lifted from winioctl.h from platform sdk
    /// </summary>
    internal class FSConstants {
        public static UInt32 FsctlGetRetrievalPointers = CTL_CODE( FileDeviceFileSystem, 28, MethodNeither, FileAnyAccess );
        public static UInt32 FsctlGetVolumeBitmap = CTL_CODE( FileDeviceFileSystem, 27, MethodNeither, FileAnyAccess );
        public static UInt32 FsctlMoveFile = CTL_CODE( FileDeviceFileSystem, 29, MethodBuffered, FileSpecialAccess );
        private const UInt32 FileAnyAccess = 0;

        private const UInt32 FileDeviceFileSystem = 0x00000009;

        private const UInt32 FileSpecialAccess = FileAnyAccess;

        private const UInt32 MethodBuffered = 0;

        private const UInt32 MethodNeither = 3;

        private static UInt32 CTL_CODE( UInt32 deviceType, UInt32 function, UInt32 method, UInt32 access ) => ( deviceType << 16 ) | ( access << 14 ) | ( function << 2 ) | method;

    }
}