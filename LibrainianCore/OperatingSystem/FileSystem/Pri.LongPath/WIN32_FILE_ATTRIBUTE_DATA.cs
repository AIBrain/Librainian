// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "WIN32_FILE_ATTRIBUTE_DATA.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "WIN32_FILE_ATTRIBUTE_DATA.cs" was last formatted by Protiguous on 2020/03/16 at 3:09 PM.

namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.IO;

    [Serializable]
    public struct WIN32_FILE_ATTRIBUTE_DATA {

        public FileAttributes fileAttributes;

        public Int32 fileSizeHigh;

        public Int32 fileSizeLow;

        public UInt32 ftCreationTimeHigh;

        public UInt32 ftCreationTimeLow;

        public UInt32 ftLastAccessTimeHigh;

        public UInt32 ftLastAccessTimeLow;

        public UInt32 ftLastWriteTimeHigh;

        public UInt32 ftLastWriteTimeLow;

        public void PopulateFrom( WIN32_FIND_DATA findData ) {
            this.fileAttributes = findData.dwFileAttributes;
            this.ftCreationTimeLow = ( UInt32 ) findData.ftCreationTime.dwLowDateTime;
            this.ftCreationTimeHigh = ( UInt32 ) findData.ftCreationTime.dwHighDateTime;
            this.ftLastAccessTimeLow = ( UInt32 ) findData.ftLastAccessTime.dwLowDateTime;
            this.ftLastAccessTimeHigh = ( UInt32 ) findData.ftLastAccessTime.dwHighDateTime;
            this.ftLastWriteTimeLow = ( UInt32 ) findData.ftLastWriteTime.dwLowDateTime;
            this.ftLastWriteTimeHigh = ( UInt32 ) findData.ftLastWriteTime.dwHighDateTime;
            this.fileSizeHigh = findData.nFileSizeHigh;
            this.fileSizeLow = findData.nFileSizeLow;
        }

    }

}