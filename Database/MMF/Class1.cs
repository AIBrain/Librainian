using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librainian.Database.MMF {
    using System.IO;
    using System.IO.MemoryMappedFiles;

    class Class1 {

        public void Test() {

            const long size32 = sizeof ( UInt32 );
            const long multiplier = UInt32.MaxValue;
            const long biteSize = size32*multiplier;    //that's a 17.18 GB !!

            using ( var bob = MemoryMappedFile.CreateOrOpen( "test.$$$", biteSize, MemoryMappedFileAccess.ReadWrite ) ) { 
            bob.CreateViewAccessor
            }
        }


        var localFilePath = "complete_path_to_large_file";
using (var memoryMappedFile = MemoryMappedFile.CreateFromFile(localFilePath, FileMode.Open))
{

}

        public string GetContent( MemoryMappedFile memoryMappedFile, long beginningByteLocation, long bytesToReadIn ) {
            string content;

            using ( var memoryMappedViewStream = memoryMappedFile.CreateViewStream( beginningByteLocation, bytesToReadIn, MemoryMappedFileAccess.Read ) ) {
                var contentArray = new byte[ bytesToReadIn ];
                memoryMappedViewStream.Read( contentArray, 0, contentArray.Length );
                content = Encoding.UTF8.GetString( contentArray );
            }

            return content;
        }

    }
}
