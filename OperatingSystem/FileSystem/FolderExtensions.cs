// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/FolderExtensions.cs" was last cleaned by Rick on 2015/11/13 at 11:30 PM

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using Measurement.Time;

    public static class FolderExtensions {

        public static readonly Char[] InvalidPathChars = Path.GetInvalidPathChars();

        public static String CleanupForFolder( this String foldername ) {
            foldername = foldername ?? String.Empty;

            do {
                var idx = foldername.IndexOfAny( InvalidPathChars );
                if ( idx >= 0 ) {
                    foldername = foldername.Remove( idx, 1 );
                }
            } while ( foldername.IndexOfAny( InvalidPathChars ) >= 0 );

            return foldername.Trim();
        }

        /// <summary>
        ///     <para>Returns true if the <see cref="Document" /> no longer seems to exist.</para>
        ///     <para>Returns null if existance cannot be determined.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="tryFor"></param>
        /// <returns></returns>
        public static Boolean? TryDeleting( this Folder folder, TimeSpan tryFor ) {
            var stopwatch = StopWatch.StartNew();
            TryAgain:
            try {
                if ( !folder.Exists() ) {
                    return true;
                }
                Directory.Delete( folder.FullName );
                return !Directory.Exists( folder.FullName );
            }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) {
                // IOExcception is thrown when the file is in use by any process.
                if ( stopwatch.Elapsed <= tryFor ) {
                    Thread.Yield();
                    Application.DoEvents();
                    goto TryAgain;
                }
            }
            catch ( UnauthorizedAccessException ) { }
            catch ( ArgumentNullException ) { }
            finally {
                stopwatch.Stop();
            }
            return null;
        }

    }

}
