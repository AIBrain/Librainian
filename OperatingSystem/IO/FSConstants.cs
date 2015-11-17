// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/FSConstants.cs" was last cleaned by Rick on 2015/11/13 at 11:30 PM

namespace Librainian.OperatingSystem.IO {

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

        private static UInt32 CTL_CODE( UInt32 deviceType, UInt32 function, UInt32 method, UInt32 access ) {
            return ( deviceType << 16 ) | ( access << 14 ) | ( function << 2 ) | method;
        }

    }

}
