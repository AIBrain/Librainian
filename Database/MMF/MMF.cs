// Copyright 2018 Protiguous.
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
// "Librainian/MMF.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

namespace Librainian.Database.MMF {

    using System;
    using System.IO.MemoryMappedFiles;
    using System.Text;

    internal class MMF {

        //private var localFilePath = "complete_path_to_large_file";

        public String GetContent( MemoryMappedFile memoryMappedFile, Int64 beginningByteLocation, Int64 bytesToReadIn ) {
            String content;

            using ( var memoryMappedViewStream = memoryMappedFile.CreateViewStream( beginningByteLocation, bytesToReadIn, MemoryMappedFileAccess.Read ) ) {
                var contentArray = new Byte[ bytesToReadIn ];
                memoryMappedViewStream.Read( contentArray, 0, contentArray.Length );
                content = Encoding.UTF8.GetString( contentArray );
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