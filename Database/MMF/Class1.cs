#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/Class1.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Database.MMF {
    using System;
    using System.IO.MemoryMappedFiles;
    using System.Text;

    internal class Class1 {
        //private var localFilePath = "complete_path_to_large_file";

        public void Test() {
            const long size32 = sizeof ( UInt32 );
            const long multiplier = UInt32.MaxValue;
            const long biteSize = size32*multiplier; //that's a 17.18 GB !!

            using ( var bob = MemoryMappedFile.CreateOrOpen( "test.$$$", biteSize, MemoryMappedFileAccess.ReadWrite ) ) {
                //bob.CreateViewAccessor
            }
        }

        //using (var memoryMappedFile = MemoryMappedFile.CreateFromFile(localFilePath, FileMode.Open)){}

        public string GetContent( MemoryMappedFile memoryMappedFile, long beginningByteLocation, long bytesToReadIn ) {
            string content;

            using ( var memoryMappedViewStream = memoryMappedFile.CreateViewStream( beginningByteLocation, bytesToReadIn, MemoryMappedFileAccess.Read ) ) {
                var contentArray = new byte[bytesToReadIn];
                memoryMappedViewStream.Read( contentArray, 0, contentArray.Length );
                content = Encoding.UTF8.GetString( contentArray );
            }

            return content;
        }
    }
}
