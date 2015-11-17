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
// "Librainian/MoveFileFlags.cs" was last cleaned by Rick on 2015/11/13 at 11:31 PM

namespace Librainian.OperatingSystem.IO {

    using System;

    [Flags]
    public enum MoveFileFlags {

        MovefileReplaceExisting = 0x00000001,

        MovefileCopyAllowed = 0x00000002,

        MovefileDelayUntilReboot = 0x00000004,

        MovefileWriteThrough = 0x00000008,

        MovefileCreateHardlink = 0x00000010,

        MovefileFailIfNotTrackable = 0x00000020

    }

}
