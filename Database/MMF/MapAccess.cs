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
// "Librainian/MapAccess.cs" was last cleaned by Protiguous on 2018/05/12 at 1:22 AM

namespace Librainian.Database.MMF {

    /// <summary>
    /// Specifies access for the mapped file. These correspond to the FILE_MAP_XXX constants used by MapViewOfFile[Ex]()
    /// </summary>
    public enum MapAccess {

        FileMapCopy = 0x0001,

        FileMapWrite = 0x0002,

        FileMapRead = 0x0004,

        FileMapAllAccess = 0x001f
    }
}