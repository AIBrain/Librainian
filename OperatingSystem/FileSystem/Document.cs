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
// "Librainian/Document.cs" was last cleaned by Rick on 2015/11/13 at 11:30 PM

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Numerics;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Extensions;
    using IO;
    using JetBrains.Annotations;
    using Magic;
    using Maths;
    using Measurement.Time;
    using Parsing;
    using Security;

    /// <summary>
    ///     <para>
    ///         An immutable wrapper for a file, the extension, the [parent] folder, and the file's size all
    ///         from a given full path.
    ///     </para>
    ///     <para>Also contains static String versions from <see cref="Path" /></para>
    /// </summary>
    [DataContract( IsReference = true )]
    [Immutable]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    public class Document : BetterDisposableClass, IEquatable< Document >, IEnumerable< Byte >, IComparable< Document > {

        public Document( [NotNull] String fullPath, String filename ) : this( Path.Combine( fullPath, filename ) ) { }

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
            var folder = ( Path.GetDirectoryName( fullPathWithFilename ) ?? String.Empty ).CleanupForFolder();

            if ( String.IsNullOrWhiteSpace( folder ) ) {
                throw new ArgumentNullException( nameof( folder ) );
            }

            var filename = Path.GetFileName( fullPathWithFilename )
                               .CleanupForFileName();
            if ( String.IsNullOrWhiteSpace( filename ) ) {
                throw new ArgumentNullException( nameof( filename ) );
            }

            this.FullPathWithFileName = Path.Combine( folder, filename );
        }

        public Document( FileSystemInfo info ) : this( info.FullName ) { }

        public Document( [NotNull] Folder folder, String filename ) : this( folder.FullName, filename ) { }

        public Document( Folder folder, Document document ) : this( Path.Combine( folder.FullName, document.FileName ) ) { }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private Document() {
            throw new NotImplementedException();
        }

        public String DebuggerDisplay => this.FullPathWithFileName;

        /// <summary>
        ///     <para>Returns the extension of the <see cref="FileName" />, including the prefix ".".</para>
        /// </summary>
        [NotNull]
        public String Extension => Path.GetExtension( this.FullPathWithFileName )
                                       .Trim()
                                       .NullIfEmptyOrWhiteSpace() ?? String.Empty;

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <seealso cref="Path.GetFileName" />
        [NotNull]
        public String FileName => Path.GetFileName( this.FullPathWithFileName );

        /// <summary>
        ///     <para>The <see cref="Folder" /> this <see cref="Document" /> is stored.</para>
        /// </summary>
        [NotNull]
        public Folder Folder => new Folder( Path.GetDirectoryName( this.FullPathWithFileName ) );

        /// <summary>
        ///     <para>The <see cref="Folder" /> combined with the <see cref="FileName" />.</para>
        /// </summary>
        [NotNull]
        public String FullPathWithFileName { get; }

        [NotNull]
        public FileInfo Info => new FileInfo( this.FullPathWithFileName );

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <seealso cref="Path.GetFileNameWithoutExtension" />
        [NotNull]
        public String Name => this.FileName;

        /// <summary>
        ///     <para>Gets the current size of the <see cref="Document" />.</para>
        /// </summary>
        /// <seealso cref="GetLength" />
        [CanBeNull]
        public UInt64? Size => this.GetLength();

        /// <summary>
        ///     Compares this. <see cref="FullPathWithFileName" /> against other <see cref="FullPathWithFileName" />.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo( Document other ) {
            return String.Compare( this.FullPathWithFileName, other.FullPathWithFileName, StringComparison.Ordinal );
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        ///     through the collection.
        /// </returns>
        public IEnumerator< Byte > GetEnumerator() => this.AsByteArray()
                                                          .GetEnumerator();

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
        ///     through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        ///     <para>To compare the contents of two <see cref="Document" /> use <see cref="SameContent(Document)" />.</para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( [CanBeNull] Document other ) => !ReferenceEquals( null, other ) && Equals( this, other );

        /// <summary>
        ///     <para>Static case sensitive comparison of the file names and file sizes for equality.</para>
        ///     <para>To compare the contents of two <see cref="Document" /> use <see cref="SameContent(Document)" />.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Document left, [CanBeNull] Document right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }
            if ( ReferenceEquals( left, null ) || ReferenceEquals( right, null ) ) {
                return false;
            }
            return ( left.Size == right.Size ) && left.FullPathWithFileName.Same( right.FullPathWithFileName );
        }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for inequality.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=( Document left, Document right ) => !Equals( left, right );

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==( Document left, Document right ) => Equals( left, right );

        public static implicit operator FileInfo( Document document ) => document.Info;

        /// <summary>
        ///     <para>If the file does not exist, it is created.</para>
        ///     <para>Then the <paramref name="text" /> is appended to the file.</para>
        /// </summary>
        /// <param name="text"></param>
        public void AppendText( String text ) {
            if ( !this.Folder.Create() ) {
                throw new DirectoryNotFoundException( this.FullPathWithFileName );
            }

            if ( this.Exists() ) {
                using ( var writer = File.AppendText( this.FullPathWithFileName ) ) {
                    writer.WriteLine( text );
                    writer.Flush();
                    writer.Close();
                }
            }
            else {
                using ( var writer = File.CreateText( this.FullPathWithFileName ) ) {
                    writer.WriteLine( text );
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        /// <summary>
        ///     Enumerates the <see cref="Document" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <returns></returns>
        public IEnumerable< Byte > AsByteArray() {
            if ( !this.Exists() ) {
                yield break;
            }

            var stream = IOExtensions.Try( () => new FileStream( path: this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read ), Seconds.Seven, CancellationToken.None );

            if ( null == stream ) {
                yield break;
            }

            if ( !stream.CanRead ) {
                throw new NotSupportedException( $"Cannot read from file {this.FullPathWithFileName}" );
            }

            using ( stream ) {
                using ( var buffered = new BufferedStream( stream ) ) {
                    var b = buffered.ReadByte();
                    if ( b == -1 ) {
                        yield break;
                    }
                    yield return ( Byte ) b;
                }

                stream.Close();
            }
        }

        /// <summary>
        ///     Returns true if a file copy was started.
        /// </summary>
        /// <param name="destination">can this be a folder or a file?!?!</param>
        /// <param name="onProgress"></param>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        public WebClient CopyFileWithProgress( Document destination, Action< Percentage > onProgress, Action onCompleted ) {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var webClient = new WebClient();

            webClient.DownloadProgressChanged += ( sender, args ) => {
                                                     var percentage = new Percentage( ( BigInteger ) args.BytesReceived, args.TotalBytesToReceive );
                                                     onProgress?.Invoke( percentage );
                                                 };
            webClient.DownloadFileCompleted += ( sender, args ) => onCompleted?.Invoke();

            webClient.DownloadFileAsync( new Uri( this.FullPathWithFileName ), destination.FullPathWithFileName );

            return webClient;
        }

        public String Crc32() {
            if ( !this.Folder.Exists() ) {
                return String.Empty;
            }
            if ( !this.Exists() ) {
                return String.Empty;
            }

            var size = this.Size;
            if ( !size.HasValue ) {
                return String.Empty;
            }

            try {
                var crc32 = new Crc32( ( UInt32 ) size.Value, ( UInt32 ) size.Value ); //HACK why not use size?

                var hash = String.Empty;
                using ( var fileStream = File.Open( this.FullPathWithFileName, FileMode.Open, FileAccess.Read, FileShare.Read ) ) {
                    hash = crc32.ComputeHash( fileStream )
                                .Aggregate( hash, ( current, b ) => current + b.ToString( "x2" )
                                                                               .ToLower() );
                    fileStream.Close();
                }
                return hash;
            }
            catch ( FileNotFoundException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) { }
            catch ( UnauthorizedAccessException ) { }
            return String.Empty;
        }

        public String Crc64() {
            if ( !this.Folder.Exists() ) {
                return String.Empty;
            }
            if ( !this.Exists() ) {
                return String.Empty;
            }

            var size = this.Size;
            if ( !size.HasValue ) {
                return String.Empty;
            }

            try {
                var crc64 = new Crc64( polynomial: size.Value, seed: size.Value ); //HACK why not use size?

                var hash = String.Empty;
                using ( var fileStream = File.Open( this.FullPathWithFileName, FileMode.Open, FileAccess.Read, FileShare.Read ) ) {
                    hash = crc64.ComputeHash( fileStream )
                                .Aggregate( hash, ( current, b ) => current + b.ToString( "x2" )
                                                                               .ToLower() );
                    fileStream.Close();
                }
                return hash;
            }
            catch ( FileNotFoundException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) { }
            catch ( UnauthorizedAccessException ) { }
            return String.Empty;
        }

        /// <summary>
        ///     <para>Returns true if the <see cref="Document" /> no longer exists.</para>
        /// </summary>
        /// <returns></returns>
        public Boolean Delete() {
            var retries = 100;
            TryAgain:
            try {
                this.Info.IsReadOnly = false;
                if ( this.Exists() ) {
                    File.Delete( this.FullPathWithFileName );
                }
                return !this.Exists();
            }
            catch ( IOException ) { //file in use
                retries--;
                Application.DoEvents();
                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }
            return !this.Exists();
        }

        public Boolean DemandPermission( FileIOPermissionAccess access ) {
            try {
                var bob = new FileIOPermission( access: access, path: this.FullPathWithFileName );
                bob.Demand();
                return true;
            }
            catch ( ArgumentException exception ) {
                exception.More();
            }
            catch ( SecurityException ) { }
            return false;
        }

        /// <summary>
        ///     <para>
        ///         To compare the contents of two <see cref="Document" /> use
        ///         <see cref="IOExtensions.SameContent(Document,Document)" />.
        ///     </para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals( [CanBeNull] Object obj ) => obj is Document && Equals( this, ( Document ) obj );

        /// <summary>
        ///     Returns true if the <see cref="Document" /> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        public Boolean Exists() => this.Info.Exists;

        /// <summary>
        ///     (file name, not contents)
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode() => this.FullPathWithFileName.GetHashCode();

        /// <summary>
        ///     <para>Gets the current size of the <see cref="Document" />.</para>
        /// </summary>
        [CanBeNull]
        public UInt64? GetLength() {
            try {
                if ( this.Exists() ) {
                    return ( UInt64 ) this.Info.Length;
                }
            }
            catch ( FileNotFoundException exception ) {
                exception.More();
            }
            catch ( IOException exception ) {
                exception.More();
            }
            return null;
        }

        /// <summary>
        ///     Attempt to start the process.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="verb">"runas" is elevated</param>
        /// <returns></returns>
        [CanBeNull]
        public async Task< Process > Launch( String arguments = null, String verb = "runas" ) {
            return await Task.Run( () => {
                                       try {
                                           var info = new ProcessStartInfo( this.FullPathWithFileName ) {Arguments = arguments ?? String.Empty, UseShellExecute = false, Verb = verb};

                                           var proc = Process.Start( info );

                                           return proc;
                                       }
                                       catch ( Exception exception ) {
                                           exception.More();
                                       }
                                       return null;
                                   } );
        }

        /// <summary>
        ///     Reads the entire file into a <see cref="String" />.
        /// </summary>
        /// <returns></returns>
        public async Task< String > ReadTextAsync() => await Task.Run( () => this.Exists() ? File.ReadAllText( this.FullPathWithFileName ) : String.Empty );

        /// <summary>
        ///     <para>
        ///         Performs a byte by byte file comparison, but ignores the <see cref="Document" /> file names.
        ///     </para>
        /// </summary>
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
        public Boolean SameContent( [CanBeNull] Document right ) {
            if ( right == null ) {
                return false;
            }

            if ( !this.Exists() ) {
                return false;
            }
            if ( !right.Exists() ) {
                return false;
            }

            var ll = this.GetLength();
            var rl = right.GetLength();

            if ( !ll.HasValue || !rl.HasValue ) {
                return false;
            }

            return ( ll.Value == rl.Value ) && this.AsByteArray()
                                                   .SequenceEqual( right.AsByteArray() );
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => this.DebuggerDisplay;

        protected override void CleanUpManagedResources() {
            base.CleanUpManagedResources();
        }

        protected override void CleanUpNativeResources() {
            base.CleanUpNativeResources();
        }

        /// <summary>
        /// Returns a unique file in the user's temp folder.
        /// <para>(Does not create a 0-byte file like <see cref="Path.GetTempFileName"/>.)</para>
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public static Document GetTempDocument() {
            return new Document( Path.GetTempPath(), Guid.NewGuid() + "." + Guid.NewGuid() );
        }

    }

}
