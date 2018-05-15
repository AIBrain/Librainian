// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "MapProtection.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license
// has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/MapProtection.cs" was last cleaned by Protiguous on 2018/05/15 at 1:34 AM.

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