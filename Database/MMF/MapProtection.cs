// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "MapProtection.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/MapProtection.cs" was last formatted by Protiguous on 2018/05/24 at 7:06 PM.

namespace Librainian.Database.MMF {

    using System;

    /// <summary>
    ///     Specifies page protection for the mapped file These correspond to the PAGE_XXX set of flags passed to
    ///     CreateFileMapping()
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