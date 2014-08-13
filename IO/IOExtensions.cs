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
// "Librainian/IOExtensions.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.IO {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using Annotations;
    using Maths;
    using Measurement.Time;

    public static class IOExtensions {
        /// <summary>
        ///     poor mans crc
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static int CalcHash( [NotNull] this FileInfo fileInfo ) {
            if ( fileInfo == null ) {
                throw new ArgumentNullException( "fileInfo" );
            }

            using ( var stream = File.OpenRead( fileInfo.FullName ) ) {
                if ( !stream.CanRead ) {
                    throw new NotSupportedException( String.Format( "Cannot read from file {0}", fileInfo.FullName ) );
                }

                var result = fileInfo.FullName.GetHashCode();
                int b;
                do {
                    b = stream.ReadByte();
                    if ( b != -1 ) {
                        result = result.GetHashMerge( b );
                    }
                } while ( b != -1 );
                return result;
            }
        }

        /// <summary>
        ///     poor mans <see cref="Document" /> compare. byte by byte.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static Boolean SameContent( [CanBeNull] this Document left, [CanBeNull] Document right ) {
            if ( left == null || right == null ) {
                return false;
            }
            return SameContent( left.File, right.File );
        }

        /// <summary>
        ///     poor mans file compare. byte by byte.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static Boolean SameContent( [CanBeNull] this FileInfo left, [CanBeNull] FileInfo right ) {
            if ( left == null || right == null ) {
                return false;
            }

            return left.Length == right.Length && left.AsByteArray().SequenceEqual( right.AsByteArray() );
        }

        /// <summary>
        ///     Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static IEnumerable< byte > AsByteArray( [NotNull] this FileInfo fileInfo ) {
            if ( fileInfo == null ) {
                throw new ArgumentNullException( "fileInfo" );
            }
            if ( !fileInfo.Exists ) {
                fileInfo.Refresh(); //check one more time
                if ( !fileInfo.Exists ) {
                    yield break;
                }
            }

            using ( var stream = new FileStream( fileInfo.FullName, FileMode.Open ) ) {
                if ( !stream.CanRead ) {
                    throw new NotSupportedException( String.Format( "Cannot read from file {0}", fileInfo.FullName ) );
                }

                using ( var buffered = new BufferedStream( stream ) ) {
                    var b = buffered.ReadByte();
                    if ( b == -1 ) {
                        yield break;
                    }
                    yield return ( Byte ) b;
                }
            }
        }

        /// <summary>
        ///     Starts a task to provides
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="progress"></param>
        /// <param name="eta"></param>
        /// <returns></returns>
        public static Task Copy( Document source, Document destination, Action<double> progress, Action<TimeSpan> eta ) {
            return Task.Run( () => {
            } );
        }

        public static MemoryStream TryCopyStream( String filePath, Boolean bePatient = true, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite ) {
        TryAgain:
            var memoryStream = new MemoryStream();
            try {
                if ( File.Exists( filePath ) ) {
                    using ( var fileStream = File.Open( path: filePath, mode: fileMode, access: fileAccess, share: fileShare ) ) {
                        var length = ( int )fileStream.Length;
                        if ( length > 0 ) {
                            fileStream.CopyTo( memoryStream, length ); //int-long possible issue.
                            memoryStream.Seek( 0, SeekOrigin.Begin );
                        }
                    }
                }
            }
            catch ( IOException ) {
                // IOExcception is thrown if the file is in use by another process.
                if ( bePatient ) {
                    if ( !Thread.Yield() ) {
                        Thread.Sleep( 0 );
                    }
                    goto TryAgain;
                }
            }
            return memoryStream;
        }

        /// <summary>
        /// <para>Returns true if the <see cref="Document"/> no longer seems to exist.</para>
        /// <para>Returns null if existance cannot be determined.</para>
        /// </summary>
        /// <param name="document"></param>
        /// <param name="bePatient">The delete will retry for a default of <see cref="Seconds.Five"/>.</param>
        /// <returns></returns>
        public static Boolean? TryDeleting( this Document document, Boolean bePatient = true ) {
            var stopwatch = Stopwatch.StartNew();
        TryAgain:
            try {
                if ( !document.Exists ) {
                    return true;
                }
                File.Delete( path: document.FullName );
                document.Refresh();
                if ( !document.Exists ) {
                    return true;
                }
            }
            catch ( DirectoryNotFoundException ) {
            }
            catch ( PathTooLongException ) {
            }
            catch ( IOException ) {
                // IOExcception is thrown if the file is in use by another process.
                if ( bePatient && stopwatch.Elapsed <= Seconds.Five ) {
                    if ( !Thread.Yield() ) {
                        Thread.Sleep( Milliseconds.ThreeHundredThirtyThree );
                    }
                    goto TryAgain;
                }
            }
            catch ( UnauthorizedAccessException ) {
            }
            catch ( ArgumentNullException ) {
            }
            finally {
                stopwatch.Stop();
            }
            return null;
        }

        /// <summary>
        ///     Tries to open a file, with a user defined number of attempt and Sleep delay between attempts.
        /// </summary>
        /// <param name="filePath">The full file path to be opened</param>
        /// <param name="fileMode">Required file mode enum value(see MSDN documentation)</param>
        /// <param name="fileAccess">Required file access enum value(see MSDN documentation)</param>
        /// <param name="fileShare">Required file share enum value(see MSDN documentation)</param>
        /// <returns>
        ///     A valid FileStream object for the opened file, or null if the File could not be opened after the required
        ///     attempts
        /// </returns>
        public static FileStream TryOpen( String filePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare ) {
            try {
                return File.Open( path: filePath, mode: fileMode, access: fileAccess, share: fileShare );
            }
            catch ( IOException ) {
                // IOExcception is thrown if the file is in use by another process.
            }
            return null;
        }

        public static FileStream TryOpenForReading( String filePath, Boolean bePatient = true, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite ) {
        TryAgain:
            try {
                if ( File.Exists( filePath ) ) {
                    return File.Open( path: filePath, mode: fileMode, access: fileAccess, share: fileShare );
                }
            }
            catch ( IOException ) {
                // IOExcception is thrown if the file is in use by another process.
                if ( bePatient ) {
                    if ( !Thread.Yield() ) {
                        Thread.Sleep( 0 );
                    }
                    goto TryAgain;
                }
            }
            return null;
        }

        public static FileStream TryOpenForWriting( String filePath, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.Write, FileShare fileShare = FileShare.ReadWrite ) {
            try {
                return File.Open( path: filePath, mode: fileMode, access: fileAccess, share: fileShare );
            }
            catch ( IOException ) {
                // IOExcception is thrown if the file is in use by another process.
            }
            return null;
        }
    }
}
