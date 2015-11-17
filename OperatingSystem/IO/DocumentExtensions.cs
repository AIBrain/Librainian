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
// "Librainian/DocumentExtensions.cs" was last cleaned by Rick on 2015/11/13 at 11:30 PM

namespace Librainian.OperatingSystem.IO {

    using System;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using FileSystem;
    using JetBrains.Annotations;
    using Measurement.Time;

    public static class DocumentExtensions {

        /// <summary>
        ///     The characters not allowed in file names.
        /// </summary>
        [NotNull] public static readonly Char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        /// <summary>
        ///     Returns the <paramref name="filename" /> will any invalid chars removed.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static String CleanupForFileName( this String filename ) {
            filename = filename ?? String.Empty;

            do {
                var idx = filename.IndexOfAny( InvalidFileNameChars );
                if ( idx >= 0 ) {
                    filename = filename.Remove( idx, 1 );
                }
            } while ( filename.IndexOfAny( InvalidFileNameChars ) >= 0 );

            return filename.Trim();
        }

        /// <summary>
        ///     <para>Returns true if the <see cref="Document" /> no longer seems to exist.</para>
        ///     <para>Returns null if existance cannot be determined.</para>
        /// </summary>
        /// <param name="document"></param>
        /// <param name="tryFor"></param>
        /// <returns></returns>
        public static Boolean? TryDeleting( this Document document, TimeSpan tryFor ) {
            var stopwatch = StopWatch.StartNew();
            TryAgain:
            try {
                if ( !document.Exists() ) {
                    return true;
                }
                File.Delete( path: document.FullPathWithFileName );
                return !File.Exists( document.FullPathWithFileName );
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
