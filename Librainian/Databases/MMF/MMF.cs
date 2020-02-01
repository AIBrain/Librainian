// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "MMF.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "MMF.cs" was last formatted by Protiguous on 2020/01/31 at 12:24 AM.

namespace Librainian.Databases.MMF {

    using System;
    using System.IO.MemoryMappedFiles;
    using System.Text;
    using JetBrains.Annotations;

    internal class MMF {

        //private var localFilePath = "complete_path_to_large_file";

        [NotNull]
        public String GetContent( [NotNull] MemoryMappedFile memoryMappedFile, Int64 beginningByteLocation, Int64 bytesToReadIn ) {
            String content;

            using ( var memoryMappedViewStream = memoryMappedFile.CreateViewStream( beginningByteLocation, bytesToReadIn, MemoryMappedFileAccess.Read ) ) {
                var contentArray = new Byte[ bytesToReadIn ];
                memoryMappedViewStream.Read( contentArray, 0, contentArray.Length );
                content = Encoding.Unicode.GetString( contentArray );
            }

            return content;
        }

        public void Test() {
            const Int64 size32 = sizeof( UInt32 );
            const Int64 multiplier = UInt32.MaxValue;
            const Int64 biteSize = size32 * multiplier; //that's a 17.18 GB !!

            using ( var _ = MemoryMappedFile.CreateOrOpen( "test.$$$", biteSize, MemoryMappedFileAccess.ReadWrite ) ) {

                //bob.CreateViewAccessor
            }
        }

        //using (var memoryMappedFile = MemoryMappedFile.CreateFromFile(localFilePath, FileMode.Open)){}
    }
}