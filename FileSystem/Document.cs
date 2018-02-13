// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Document.cs" was last cleaned by Rick on 2016/08/06 at 11:19 PM

namespace Librainian.FileSystem {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Numerics;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Maths.Numbers;
    using Measurement.Time;
    using Microsoft.VisualBasic.Devices;
    using Microsoft.VisualBasic.FileIO;
    using Newtonsoft.Json;
    using Parsing;
    using Security;
    using Threading;

    /// <summary>
    ///     <para>
    ///         An immutable wrapper for a file, the extension, the [parent] folder, and the file's size all
    ///         from a given full path.
    ///     </para>
    ///     <para>Also contains static String versions from <see cref="Path" /></para>
    /// </summary>
    [Immutable]
    [DebuggerDisplay( "{ToString(),nq}" )]
    [JsonObject]
    public class Document : IEquatable<Document>, IEnumerable<Byte>, IComparable<Document> {

        public Document( [NotNull] String fullPath, String filename ) : this( Path.Combine( fullPath, filename ) ) {
        }

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
            if ( String.IsNullOrEmpty( Path.GetDirectoryName( fullPathWithFilename ) ) ) {
                fullPathWithFilename = Path.Combine( Environment.CurrentDirectory, fullPathWithFilename );
            }

            //return new Folder( Path.GetDirectoryName( this.FullPathWithFileName ) );

            var folder = ( Path.GetDirectoryName( fullPathWithFilename ) ?? String.Empty ).CleanupForFolder();

            if ( String.IsNullOrWhiteSpace( folder ) ) {
                throw new ArgumentNullException( nameof( folder ) );
            }

            var filename = Path.GetFileName( fullPathWithFilename ).CleanupForFileName();
            if ( String.IsNullOrWhiteSpace( filename ) ) {
                throw new ArgumentNullException( nameof( filename ) );
            }

            this.FullPathWithFileName = Path.Combine( folder, filename );

            //this.Folder = new Folder( folder );
        }

        public Document( FileSystemInfo info ) : this( info.FullName ) {
        }

        public Document( [NotNull] Folder folder, String filename ) : this( folder.FullName, filename ) {
        }

        public Document( Folder folder, Document document ) : this( Path.Combine( folder.FullName, document.FileName() ) ) {
        }

        /// <summary>
        /// </summary>
        /// <param name="internetAddress"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="WebException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public Document( [NotNull] Uri internetAddress ) {
            if ( internetAddress == null ) {
                throw new ArgumentNullException( nameof( internetAddress ) );
            }

            var tempFolder = Librainian.FileSystem.Folder.GetTempFolder();
            if ( null == tempFolder ) {
                throw new DirectoryNotFoundException( "Unable to find user's temp folder." );
            }

            var tempFilename = Path.GetFileName( internetAddress.AbsolutePath );

            var downloadLocation = new Document( tempFolder, tempFilename );

            var webClient = new WebClient();
            webClient.DownloadFile( internetAddress, downloadLocation.FullPathWithFileName );

            this.FullPathWithFileName = downloadLocation.FullPathWithFileName;

            //this.Folder = tempFolder;
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private Document() {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     <para>Returns the extension of the <see cref="FileName" />, including the prefix ".".</para>
        /// </summary>
        [ NotNull ]
        public String Extension() => Path.GetExtension( this.FullPathWithFileName ).Trim().NullIfEmptyOrWhiteSpace() ?? String.Empty;

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <seealso cref="Path.GetFileName" />
        [ NotNull ]
        public String FileName() => Path.GetFileName( this.FullPathWithFileName );

        /// <summary>
        ///     <para>The <see cref="Folder" /> this <see cref="Document" /> is stored.</para>
        /// </summary>
        [ NotNull ]
        public Folder Folder() => new Folder( this.Info().Directory );

        /// <summary>
        ///     <para>The <see cref="Folder" /> combined with the <see cref="FileName" />.</para>
        /// </summary>
        [JsonProperty]
        [NotNull]
        public String FullPathWithFileName {
            get;
        }

        [ NotNull ]
        public FileInfo Info() => new FileInfo( this.FullPathWithFileName );

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

            return ( left.Size() == right.Size() ) && left.FullPathWithFileName.Same( right.FullPathWithFileName );
        }

        /// <summary>
        ///     Returns a unique file in the user's temp folder.
        ///     <para>If an extension is not provided, a random extension (a <see cref="Guid" />) will be used.</para>
        ///     <para><b>Note</b>: Does not create a 0-byte file like <see cref="Path.GetTempFileName" />.</para>
        ///     <para>If the temp folder is not found, one attempt will be made to create it.</para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        [NotNull]
        public static Document GetTempDocument( String extension = null ) {
            if ( extension != null ) {
                extension = extension.Trim();
                while ( extension.StartsWith( "." ) ) {
                    extension = extension.Substring( 1 );
                }
            }

            if ( String.IsNullOrWhiteSpace( extension ) ) {
                extension = Guid.NewGuid().ToString();
            }
            return new Document( Librainian.FileSystem.Folder.GetTempFolder(), Guid.NewGuid() + "." + extension.Trim() );
        }

        public static implicit operator FileInfo( Document document ) => document.Info();

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

        /// <summary>
        ///     <para>If the file does not exist, it is created.</para>
        ///     <para>Then the <paramref name="text" /> is appended to the file.</para>
        /// </summary>
        /// <param name="text"></param>
        public void AppendText( String text ) {
            if ( !this.Folder().Create() ) {
                throw new DirectoryNotFoundException( this.FullPathWithFileName );
            }

            if ( this.Exists() ) {
                using ( var writer = File.AppendText( this.FullPathWithFileName ) ) {
                    writer.WriteLine( text );
                    writer.Flush();
                }
            }
            else {
                using ( var writer = File.CreateText( this.FullPathWithFileName ) ) {
                    writer.WriteLine( text );
                    writer.Flush();
                }
            }
        }

        /// <summary>
        ///     Enumerates the <see cref="Document" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Byte> AsByteArray() {
            if ( !this.Exists() ) {
                yield break;
            }

            var stream = IOExtensions.Try( () => new FileStream( path: this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ), Seconds.Seven, CancellationToken.None );

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

                    yield return ( Byte )b;
                }
            }
        }

        /// <summary>
        ///     Compares this. <see cref="FullPathWithFileName" /> against other <see cref="FullPathWithFileName" />.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo( Document other ) {
            return String.Compare( this.FullPathWithFileName, other.FullPathWithFileName, StringComparison.Ordinal );
        }

        /// <summary>
        ///     Starts a task to copy a file
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="progress"></param>
        /// <param name="eta"></param>
        /// <returns></returns>
        public Task Copy( Document destination, Action<Double> progress, Action<TimeSpan> eta ) {
            return Task.Run( () => {
                var computer = new Computer();

                //TODO file monitor/watcher?
                computer.FileSystem.CopyFile( this.FullPathWithFileName, destination.FullPathWithFileName, UIOption.AllDialogs, UICancelOption.DoNothing );
            } );
        }

        /// <summary>
        ///     Returns the <see cref="WebClient" /> if a file copy was started.
        /// </summary>
        /// <param name="destination">can this be a folder or a file?!?!</param>
        /// <param name="onProgress"></param>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        public WebClient CopyFileWithProgress( Document destination, Action<Percentage> onProgress, Action onCompleted ) {
            var webClient = new WebClient();

            webClient.DownloadProgressChanged += ( sender, args ) => {
                var percentage = new Percentage( ( BigInteger )args.BytesReceived, args.TotalBytesToReceive );
                onProgress?.Invoke( percentage );
            };
            webClient.DownloadFileCompleted += ( sender, args ) => onCompleted?.Invoke();

            webClient.DownloadFileAsync( new Uri( this.FullPathWithFileName ), destination.FullPathWithFileName );

            return webClient;
        }

        [CanBeNull]
        public String Crc32() {
            if ( !this.Folder().Exists() ) {
                return null;
            }
            if ( !this.Exists() ) {
                return null;
            }

            var size = this.Size();
            if ( !size.HasValue ) {
                return null;
            }

            try {
                var crc32 = new Crc32( ( UInt32 )size.Value, ( UInt32 )size.Value ); //HACK why not use size?

                using ( var fileStream = File.Open( this.FullPathWithFileName, FileMode.Open, FileAccess.Read, FileShare.Read ) ) {
                    return crc32.ComputeHash( fileStream ).Aggregate( String.Empty, ( current, b ) => current + b.ToString( "x2" ).ToLower() );
                }
            }
            catch ( FileNotFoundException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) { }
            catch ( UnauthorizedAccessException ) { }

            return null;
        }

        [CanBeNull]
        public String Crc64() {
            if ( !this.Folder().Exists() ) {
                return null;
            }
            if ( !this.Exists() ) {
                return null;
            }

            var size = this.Size();
            if ( !size.HasValue ) {
                return null;
            }

            try {
                var crc64 = new Crc64( polynomial: size.Value, seed: size.Value ); //HACK why not use size?

                using ( var fileStream = File.Open( this.FullPathWithFileName, FileMode.Open, FileAccess.Read, FileShare.Read ) ) {
                    return crc64.ComputeHash( fileStream ).Aggregate( String.Empty, ( current, b ) => current + b.ToString( "x2" ).ToLower() );
                }
            }
            catch ( FileNotFoundException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) { }
            catch ( UnauthorizedAccessException ) { }

            return null;
        }

        /// <summary>
        ///     <para>Returns true if the <see cref="Document" /> no longer exists.</para>
        /// </summary>
        /// <returns></returns>
        public Boolean Delete() {
            var retries = 100;
            TryAgain:
            try {
                if ( !this.Exists() ) {
                    return true;
                }

                if ( this.Info().IsReadOnly ) {
                    this.Info().IsReadOnly = false;
                }

                if ( this.Exists() ) {
                    File.Delete( this.FullPathWithFileName );
                }
                return !this.Exists();
            }
            catch ( IOException ) {

                //file in use
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
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        ///     <para>To compare the contents of two <see cref="Document" /> use <see cref="SameContent(Document)" />.</para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( [CanBeNull] Document other ) => !ReferenceEquals( null, other ) && Equals( this, other );

        /// <summary>
        ///     <para>
        ///         To compare the contents of two <see cref="Document" /> use SameContent( Document,Document).
        ///     </para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals( [CanBeNull] Object obj ) => obj is Document && Equals( this, ( Document )obj );

        /// <summary>
        ///     Returns true if the <see cref="Document" /> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        public Boolean Exists() => this.Info().Exists;

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        ///     through the collection.
        /// </returns>
        public IEnumerator<Byte> GetEnumerator() => this.AsByteArray().GetEnumerator();

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
                    return ( UInt64 )this.Info().Length;
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
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
        ///     through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        ///     Attempt to start the process.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="verb">"runas" is elevated</param>
        /// <returns></returns>
        [CanBeNull]
        public async Task<Process> Launch( String arguments = null, String verb = "runas" ) {
            return await Task.Run( () => {
                try {
                    var info = new ProcessStartInfo( this.FullPathWithFileName ) {
                        Arguments = arguments ?? String.Empty,
                        UseShellExecute = false,
                        Verb = verb
                    };

                    return Process.Start( info );
                }
                catch ( Exception exception ) {
                    exception.More();
                }

                return null;
            } );
        }

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <seealso cref="Path.GetFileNameWithoutExtension" />
        [NotNull]
        public String Name() => this.FileName();

        /// <summary>
        ///     Reads the entire file into a <see cref="String" />.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public async Task<String> ReadTextAsync() {
            try {
                return await Task.Run( () => this.Exists() ? File.ReadAllText( this.FullPathWithFileName ) : String.Empty );
            }
            catch ( Exception ) {
                return null;
            }
        }

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

            return ( ll.Value == rl.Value ) && this.AsByteArray().SequenceEqual( right.AsByteArray() );
        }

        public void SetCreationTime( DateTime when, CancellationToken cancellationToken ) {
            IOExtensions.Try( () => {
                if ( !Exists() ) {
                    return false;
                }

                Info().IsReadOnly = false;
                return !Info().IsReadOnly;
            }, Seconds.Thirty, cancellationToken );

            IOExtensions.Try( () => {
                if ( !Exists() ) {
                    return false;
                }

                File.SetCreationTime( path: FullPathWithFileName, creationTime: when );
                return true;
            }, Seconds.Thirty, cancellationToken );
        }

        /// <summary>
        ///     <para>Gets the current size of the <see cref="Document" />.</para>
        /// </summary>
        /// <seealso cref="GetLength" />
        [CanBeNull]
        public UInt64? Size() => this.GetLength();

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => this.FullPathWithFileName;

        /// <summary>
        ///     <para>Returns true if the <see cref="Document" /> no longer seems to exist.</para>
        ///     <para>Returns null if existence cannot be determined.</para>
        /// </summary>
        /// <param name="tryFor"></param>
        /// <returns></returns>
        public Boolean? TryDeleting( TimeSpan tryFor ) {
            var stopwatch = StopWatch.StartNew();
            TryAgain:
            try {
                if ( !this.Exists() ) {
                    return true;
                }

                File.Delete( path: this.FullPathWithFileName );
                return !File.Exists( this.FullPathWithFileName );
            }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) {

                // IOExcception is thrown when the file is in use by any process.
                if ( stopwatch.Elapsed <= tryFor ) {
                    Thread.CurrentThread.Fraggle( Seconds.One );
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
