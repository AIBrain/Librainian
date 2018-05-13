// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/MapProtection.cs" was last cleaned by Protiguous on 2018/05/12 at 1:22 AM

namespace Librainian.Database.MMF {

    using System;

    /// <summary>
    /// Specifies page protection for the mapped file These correspond to the PAGE_XXX set of flags passed to CreateFileMapping()
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

        SecNoCache = 0x10000000
    }
}