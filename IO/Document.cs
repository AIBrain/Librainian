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
// "Librainian/File.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.IO {
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using Annotations;
    using Extensions;

    /// <summary>
    ///     <para>Container for a file, the parent folder, and the file size from the given fullpath.</para>
    ///     <seealso cref="IOExtensions.SameContent(Document,Document)" />
    /// </summary>
    [DataContract( IsReference = true )]
    [Immutable]
    public sealed class Document : IEquatable< Document > {
        /// <summary>
        ///     <para>The extension of the file parsed in the ctor.</para>
        /// </summary>
        [NotNull]
        public readonly String Extension;

        /// <summary>
        ///     <para>The <see cref="FileInfo" /> parsed in the ctor.</para>
        /// </summary>
        [NotNull]
        public readonly FileInfo Info;

        /// <summary>
        ///     The last known size of the file.
        /// </summary>
        public readonly UInt64 Size;

        [NotNull]
        private readonly Lazy< DirectoryInfo > _lazyFolder;

        /// <summary>
        /// </summary>
        /// <param name="fullPathWithFilename"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Document( [NotNull] String fullPathWithFilename ) {
            if ( String.IsNullOrWhiteSpace( fullPathWithFilename ) ) {
                throw new ArgumentNullException( "fullPathWithFilename" );
            }

            fullPathWithFilename = fullPathWithFilename.Trim();
            if ( String.IsNullOrWhiteSpace( fullPathWithFilename ) ) {
                throw new ArgumentNullException( "fullPathWithFilename" );
            }

            this.Extension = Path.GetExtension( path: fullPathWithFilename );
            this.Info = new FileInfo( fileName: fullPathWithFilename );
            this.Size = ( UInt64 )this.Info.Length;
            this._lazyFolder = new Lazy< DirectoryInfo >( () => this.Info.Directory );
        }

        [NotNull]
        public DirectoryInfo Folder { get { return this._lazyFolder.Value; } }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( [CanBeNull] Document other ) {
            return !ReferenceEquals( null, other ) && Equals( this, other );
        }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Document left, [CanBeNull] Document right ) {
            if ( left == null ) {
                throw new ArgumentNullException( "left" );
            }
            if ( right == null ) {
                throw new ArgumentNullException( "right" );
            }
            return String.Equals( left.Info.FullName, right.Info.FullName, StringComparison.InvariantCultureIgnoreCase ) && left.Size == right.Size;
        }

        public override int GetHashCode() {
            unchecked {
                return ( this.Info.GetHashCode() * 397 ) ^ this.Size.GetHashCode();
            }
        }

        public override Boolean Equals( [CanBeNull] object obj ) {
            return obj is Document && Equals( this, ( Document )obj );
        }

        [NotNull]
        public static implicit operator DirectoryInfo( [NotNull] Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( "document" );
            }
            return document.Folder;
        }

        [NotNull]
        public static implicit operator FileInfo( [NotNull] Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( "document" );
            }
            return document.Info;
        }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for inequality.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=( Document left, Document right ) {
            return !Equals( left, right );
        }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==( Document left, Document right ) {
            return Equals( left, right );
        }

        /// <summary>
        ///     Starts a task to provides
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="progress"></param>
        /// <param name="eta"></param>
        /// <returns></returns>
        public static Task Copy( Document source, Document destination, Action< double > progress, Action< TimeSpan > eta ) {
            return Task.Run( () => { } );
        }

        public static MemoryStream TryCopyStream( String filePath, Boolean bePatient = true, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite ) {
            TryAgain:
            var memoryStream = new MemoryStream();
            try {
                if ( File.Exists( filePath ) ) {
                    using ( var fileStream = File.Open( path: filePath, mode: fileMode, access: fileAccess, share: fileShare ) ) {
                        var length = ( int ) fileStream.Length;
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

        public static FileStream TryDeletingFile( String filePath, Boolean bePatient = true ) {
            TryAgain:
            try {
                File.Delete( path: filePath );
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
