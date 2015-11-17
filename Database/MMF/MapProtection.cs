// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/MapProtection.cs" was last cleaned by Rick on 2015/06/12 at 2:53 PM

namespace Librainian.Database.MMF {

    using System;

    /// <summary>
    /// Specifies page protection for the mapped file These correspond to the PAGE_XXX set of flags
    /// passed to CreateFileMapping()
    /// </summary>
    [Flags]
    public enum MapProtection {
        PageNone = 0x00000000,

        // protection - mutually exclusive, do not or
        PageReadOnly = 0x00000002,

        PageReadWrite = 0x00000004,

        PageWriteCopy = 0x00000008,

        // attributes - or-able with protection
        SecImage = 0x01000000,

        SecReserve = 0x04000000,

        SecCommit = 0x08000000,

        SecNoCache = 0x10000000,
    }
}