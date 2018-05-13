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
// "Librainian/MMFExt2.cs" was last cleaned by Protiguous on 2018/05/12 at 1:22 AM

namespace Librainian.Extensions {

    using System;
    using System.IO;
    using System.IO.MemoryMappedFiles;

    public class MmfExt2 {

        public static Boolean Resize( FileInfo source, FileInfo destination, Boolean overwriteDestination = true, Boolean findRoom = true ) {
            source.Refresh();

            if ( !source.Exists ) {
                return false;
            }

            destination.Refresh();

            if ( destination.Exists ) {
                if ( overwriteDestination ) {
                    destination.Delete();
                }
                else {
                    return false;
                }
            }

            // ReSharper disable once UnusedVariable
            using ( var sourceMappedFile = MemoryMappedFile.CreateFromFile( source.FullName, FileMode.Open, "why?", source.Length, MemoryMappedFileAccess.Read ) ) { }

            return false;
        }
    }
}