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
// "Librainian2/MMFExt2.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.IO;
    using System.IO.MemoryMappedFiles;

    public class MMFExt2 {
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
