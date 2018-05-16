// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Document.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Document.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Linq;
    using System.Net;
    using System.Numerics;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Extensions;
    using Internet;
    using JetBrains.Annotations;
    using Magic;
    using Maths.Numbers;
    using Measurement.Time;
    using Microsoft.VisualBasic.Devices;
    using Microsoft.VisualBasic.FileIO;
    using Newtonsoft.Json;
    using Persistence;
    using Security;
    using Threading;

    /// <summary>
    ///     <para>
    ///         An immutable wrapper for a file, the extension, the [parent] folder, and the file's size all from a given
    ///         full path.
    ///     </para>
    ///     <para>Also contains static String versions from <see cref="Path" /></para>
    /// </summary>
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public class Document : ABetterClassDispose, IEquatable<Document>, IEnumerable<Byte>, IComparable<Document> {

        // ReSharper disable once NotNullMemberIsNotInitialized
        private Document() => throw new NotImplementedException( "Private contructor is not allowed." );

        public Document( [NotNull] String fullPath, String filename, Boolean deleteAfterClose = false ) :
            this( fullPathWithFilename: Path.Combine( path1: fullPath, path2: filename ), deleteAfterClose: deleteAfterClose ) { }

        /// <summary>
        /// </summary>
        /// <param name="fullPathWithFilename"></param>
        /// <param name="deleteAfterClose">    </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Document( [NotNull] String fullPathWithFilename, Boolean deleteAfterClose = false ) {
            if ( String.IsNullOrWhiteSpace( fullPathWithFilename ) ) { throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fullPathWithFilename ) ); }

            this.DeleteAfterClose = deleteAfterClose;

            this.Info = new FileInfo( fileName: fullPathWithFilename );
        }

        public Document( [NotNull] FileSystemInfo info, Boolean deleteAfterClose = false ) : this( fullPathWithFilename: info.FullName, deleteAfterClose: deleteAfterClose ) { }

        public Document( [NotNull] Folder folder, [NotNull] String filename, Boolean deleteAfterClose = false ) : this( fullPath: folder.FullName, filename: filename, deleteAfterClose: deleteAfterClose ) { }

        public Document( Folder folder, Document document, Boolean deleteAfterClose = false ) : this( fullPathWithFilename: Path.Combine( path1: folder.FullName, path2: document.FileName() ),
            deleteAfterClose: deleteAfterClose ) { }

        public Boolean DeleteAfterClose { get; }

        /// <summary>
        ///     <para>The <see cref="Folder" /> where this <see cref="Document" /> is stored.</para>
        /// </summary>
        [NotNull]
        public Folder Folder => new Folder( fileSystemInfo: this.Info.Directory );

        /// <summary>
        ///     <para>The <see cref="Folder" /> combined with the <see cref="FileName" />.</para>
        /// </summary>
        [JsonProperty]
        [NotNull]
        public String FullPathWithFileName => this.Info.FullName;

        /// <summary>
        ///     The heart of the <see cref="Document" /> class.
        /// </summary>
        [JsonProperty]
        [NotNull]
        public FileInfo Info { get; }

        /// <summary>
        ///     <para>Gets the current <seealso cref="Size" /> of the <see cref="Document" />.</para>
        /// </summary>
        public Int64 Length {
            get {
                try {
                    if ( this.Exists() ) { return this.Info.Length; }
                }
                catch ( FileNotFoundException exception ) { exception.More(); }
                catch ( IOException exception ) { exception.More(); }

                return default;
            }
        }

        /// <summary>
        ///     <para>Static case sensitive comparison of the file names and file sizes for equality.</para>
        ///     <para>To compare the contents of two <see cref="Document" /> use <see cref="SameContent(Document)" />.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Document left, [CanBeNull] Document right ) {
            if ( ReferenceEquals( left, right ) ) { return true; }

            if ( left is null || right is null ) { return false; }

            return left.Size() == right.Size() && left.FullPathWithFileName.Is( right: right.FullPathWithFileName );
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

                while ( extension.StartsWith( "." ) ) { extension = extension.Substring( startIndex: 1 ); }
            }

            if ( String.IsNullOrWhiteSpace( extension ) ) { extension = Guid.NewGuid().ToString(); }

            return new Document( folder: Folder.GetTempFolder(), filename: Guid.NewGuid() + "." + extension.Trim() );
        }

        public static implicit operator FileInfo( Document document ) => document.Info;

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for inequality.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=( Document left, Document right ) => !Equals( left: left, right: right );

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==( Document left, Document right ) => Equals( left: left, right: right );

        /// <summary>
        ///     <para>If the file does not exist, it is created.</para>
        ///     <para>Then the <paramref name="text" /> is appended to the file.</para>
        /// </summary>
        /// <param name="text"></param>
        public void AppendText( String text ) {
            if ( !this.Folder.Create() ) { throw new DirectoryNotFoundException( this.FullPathWithFileName ); }

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
        public IEnumerable<Byte> AsBytes() {
            if ( !this.Exists() ) { yield break; }

            using ( var stream = new FileStream( this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {

                if ( !stream.CanRead ) { throw new NotSupportedException( $"Cannot read from file stream on {this.FullPathWithFileName}" ); }

                using ( var buffered = new BufferedStream( stream: stream ) ) {
                    var b = buffered.ReadByte();

                    if ( b == -1 ) { yield break; }

                    yield return ( Byte )b;
                }
            }
        }

        /// <summary>
        ///     <para>Clone the entire document to the <paramref name="destination" /> as quickly as possible.</para>
        ///     <para>This will OVERWRITE any <see cref="destination" /> file.</para>
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="progress">   </param>
        /// <param name="eta">        </param>
        /// <returns></returns>
        public async Task<(Boolean success, TimeSpan timeElapsed)> Clone( [NotNull] Document destination, [CanBeNull] IProgress<Single> progress = null, [CanBeNull] IProgress<TimeSpan> eta = null ) {
            if ( destination == null ) { throw new ArgumentNullException( nameof( destination ) ); }

            var stopwatch = StopWatch.StartNew();

            var fileSize = this.Length;

            if ( !fileSize.Any() ) { return (false, stopwatch.Elapsed); }

            if ( fileSize <= ( UInt64 )this.GetBufferSize() ) {
                await this.Copy( destination, progress, eta ).ConfigureAwait( false );

                return (destination.Exists() && destination.Length == fileSize, stopwatch.Elapsed);
            }

            var processorCount = ( UInt64 )Environment.ProcessorCount;

            var chunksNeeded = fileSize / processorCount;

            var buffers = new ThreadLocal<Byte[]>( () => new Byte[this.GetBufferSize()], trackAllValues: true );

            var sourceStream = new FileStream( this.FullPathWithFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, this.GetBufferSize(), FileOptions.Asynchronous | FileOptions.RandomAccess );
            var sourceBuffer = new BufferedStream( sourceStream, this.GetBufferSize() );
            var sourceBinary = new BinaryReader( sourceBuffer, Encoding.Unicode );

            var destinationStream = new FileStream( destination.FullPathWithFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, this.GetBufferSize(), FileOptions.Asynchronous | FileOptions.RandomAccess );
            var destinationBuffer = new BufferedStream( destinationStream, this.GetBufferSize() );
            var destinationBinary = new BinaryWriter( destinationBuffer, Encoding.Unicode );

            using ( var memoryOut = MemoryMappedFile.CreateOrOpen( destination.JustName(), fileSize, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.DelayAllocatePages, HandleInheritability.Inheritable ) ) {
                var viewStream = memoryMapped.CreateViewStream();
                viewStream.Write( fileBytes, 0, fileBytes.Length );
            }
        }

        /// <summary>
        ///     Compares this. <see cref="FullPathWithFileName" /> against other <see cref="FullPathWithFileName" />.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo( Document other ) => String.Compare( strA: this.FullPathWithFileName, strB: other.FullPathWithFileName, comparisonType: StringComparison.Ordinal );

        /// <summary>
        ///     Starts a task to copy a file
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="progress">   </param>
        /// <param name="eta">        </param>
        /// <returns></returns>
        public Task Copy( Document destination, IProgress<Single> progress, IProgress<TimeSpan> eta ) =>
            Task.Run( () => {
                var computer = new Computer();

                //TODO file monitor/watcher?
                computer.FileSystem.CopyFile( sourceFileName: this.FullPathWithFileName, destinationFileName: destination.FullPathWithFileName, showUI: UIOption.AllDialogs, onUserCancel: UICancelOption.DoNothing );
            } );

        /// <summary>
        ///     Returns the <see cref="WebClient" /> if a file copy was started.
        /// </summary>
        /// <param name="destination">can this be a folder or a file?!?!</param>
        /// <param name="onProgress"> </param>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        public WebClient CopyFileWithProgress( Document destination, Action<Percentage> onProgress, Action onCompleted ) {

            // ReSharper disable once UseObjectOrCollectionInitializer
            var webClient = new WebClient();

            webClient.DownloadProgressChanged += ( sender, args ) => {
                var percentage = new Percentage( numerator: ( BigInteger )args.BytesReceived, denominator: args.TotalBytesToReceive );
                onProgress?.Invoke( percentage );
            };

            webClient.DownloadFileCompleted += ( sender, args ) => onCompleted?.Invoke();

            webClient.DownloadFileAsync( address: new Uri( uriString: this.FullPathWithFileName ), fileName: destination.FullPathWithFileName );

            return webClient;
        }

        /// <summary>
        ///     Returns a lowercase hex-string of the hash.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public async Task<String> Crc32() {
            try {
                if ( !this.Exists() ) { return null; }

                var size = this.Size();

                var crc32 = new Crc32( polynomial: ( UInt32 )size, seed: ( UInt32 )size );

                return await Task.Run( () => {
                    using ( var fileStream = File.Open( this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
                        return crc32.ComputeHash( fileStream ).Aggregate( seed: String.Empty, func: ( current, b ) => current + b.ToString( format: "x2" ).ToLower() );
                    }
                } ).ConfigureAwait( false );
            }
            catch ( FileNotFoundException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) { }
            catch ( UnauthorizedAccessException ) { }

            return null;
        }

        public async Task<UInt32?> CRC32() {
            try {
                if ( !this.Exists() ) { return null; }

                return await Task.Run( () => {
                    var size = this.Size();

                    var crc32 = new Crc32( polynomial: ( UInt32 )size, seed: ( UInt32 )size );

                    using ( var fileStream = File.Open( this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
                        var result = crc32.ComputeHash( fileStream );

                        return BitConverter.ToUInt32( result, 0 );
                    }
                } ).ConfigureAwait( false );
            }
            catch ( FileNotFoundException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) { }
            catch ( UnauthorizedAccessException ) { }

            return null;
        }

        public async Task<UInt64?> CRC64() {
            try {
                if ( !this.Exists() ) { return null; }

                await Task.Run( () => {
                    var size = this.Size();

                    var crc64 = new Crc64( polynomial: size, seed: size );

                    using ( var fileStream = File.Open( this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
                        return BitConverter.ToUInt64( crc64.ComputeHash( fileStream ), 0 );
                    }
                } );
            }
            catch ( FileNotFoundException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) { }
            catch ( UnauthorizedAccessException ) { }

            return null;
        }

        [CanBeNull]
        public String CRC64Hex() {
            try {
                if ( !this.Exists() ) { return null; }

                var size = this.Size();

                var crc64 = new Crc64( polynomial: size, seed: size );

                using ( var fileStream = File.Open( this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
                    return crc64.ComputeHash( fileStream ).Aggregate( seed: String.Empty, func: ( current, b ) => current + b.ToString( format: "x2" ).ToLower() );
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
        ///     <para>Defaults to 3 retries</para>
        /// </summary>
        /// <returns></returns>
        public Boolean Delete( Int32 retries = 3 ) {
            TryAgain:

            try {
                if ( !this.Exists() ) { return true; }

                if ( this.Info.IsReadOnly ) { this.Info.IsReadOnly = false; }

                if ( this.Exists() ) { this.Info.Delete(); }

                return !this.Exists();
            }
            catch ( IOException ) {

                //file in use?
                retries--;
                Application.DoEvents();

                if ( retries.Any() ) { goto TryAgain; }
            }

            return !this.Exists();
        }

        public Boolean DemandPermission( FileIOPermissionAccess access ) {
            try {
                var bob = new FileIOPermission( access: access, this.FullPathWithFileName );
                bob.Demand();

                return true;
            }
            catch ( ArgumentException exception ) { exception.More(); }
            catch ( SecurityException ) { }

            return false;
        }

        /// <inheritdoc />
        public override void DisposeManaged() {
            if ( this.DeleteAfterClose ) { this.Delete(); }

            base.DisposeManaged();
        }

        /// <summary>
        ///     <para>Downloads (replaces) the local document with the specified <paramref name="source" />.</para>
        ///     <para>Note: will replace the content of the this <see cref="Document" />.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public async Task<(Exception exception, WebHeaderCollection responseHeaders)> DownloadFile( [NotNull] Uri source ) {
            if ( source == null ) { throw new ArgumentNullException( nameof( source ) ); }

            try {
                if ( !source.IsWellFormedOriginalString() ) { return (new DownloadException( $"Could not use source Uri '{source}'." ), null); }

                using ( var webClient = new WebClient() ) {
                    await webClient.DownloadFileTaskAsync( source, this.FullPathWithFileName ).ConfigureAwait( false );

                    return (null, webClient.ResponseHeaders);
                }
            }
            catch ( Exception exception ) { return (exception, null); }
        }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        ///     <para>To compare the contents of two <see cref="Document" /> use <see cref="SameContent(Document)" />.</para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( Document other ) => !( other is null ) && Equals( left: this, right: other );

        /// <summary>
        ///     <para>To compare the contents of two <see cref="Document" /> use SameContent( Document,Document).</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals( Object obj ) => obj is Document document && Equals( left: this, right: document );

        /// <summary>
        ///     Returns true if the <see cref="Document" /> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        public Boolean Exists() => this.Info.Exists;

        /// <summary>
        ///     <para>Computes the extension of the <see cref="FileName" />, including the prefix ".".</para>
        /// </summary>
        [NotNull]
        public String Extension() => Path.GetExtension( this.FullPathWithFileName ).Trim().NullIfEmptyOrWhiteSpace() ?? String.Empty;

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <seealso cref="Path.GetFileName" />
        [NotNull]
        public String FileName() => Path.GetFileName( this.FullPathWithFileName );

        /// <summary>
        ///     <para>
        ///         Could we allocate a full 2GB buffer if we wanted? that'd be really nice for the <see cref="Document" /> copy
        ///         routines...
        ///     </para>
        ///     <para>See the file "App.config" for setting gcAllowVeryLargeObjects to true.</para>
        /// </summary>
        public Int32 GetBufferSize() {
            var oursize = this.Size();

            if ( !oursize.Any() ) { return default; }

            if ( oursize <= Int32.MaxValue ) { return ( Int32 )oursize; }

            return BufferSize1;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the
        ///     collection.
        /// </returns>
        public IEnumerator<Byte> GetEnumerator() => this.AsBytes().GetEnumerator();

        /// <summary>
        ///     (file name, not contents)
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode() => this.FullPathWithFileName.GetHashCode();

        public String JustName() => Path.GetFileNameWithoutExtension( this.FileName() );

        /// <summary>
        ///     Attempt to start the process.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="verb">     "runas" is elevated</param>
        /// <returns></returns>
        [CanBeNull]
        public async Task<Process> Launch( String arguments = null, String verb = "runas" ) =>
            await Task.Run( function: () => {
                try {
                    var info = new ProcessStartInfo( fileName: this.FullPathWithFileName ) { Arguments = arguments ?? String.Empty, UseShellExecute = false, Verb = verb };

                    return Process.Start( startInfo: info );
                }
                catch ( Exception exception ) { exception.More(); }

                return null;
            } );

        /// <summary>
        ///     Attempt to return an object Deserialized from this JSON text file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [CanBeNull]
        public T LoadJSON<T>() {
            if ( !this.Exists() ) { return default; }

            try {
                using ( var textReader = File.OpenText( this.FullPathWithFileName ) ) {
                    using ( var jsonReader = new JsonTextReader( textReader ) ) { return PersistenceExtensions.LocalJsonSerializers.Value.Deserialize<T>( jsonReader ); }
                }
            }
            catch ( Exception exception ) { exception.More(); }

            return default;
        }

        public async Task<T> LoadJSONAsync<T>() { return await Task.Run( () => this.LoadJSON<T>() ).ConfigureAwait( false ); }

        /// <summary>
        ///     Starts a task to copy a file
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public Task<Boolean> Move( [NotNull] Document destination ) {
            if ( destination is null || String.IsNullOrWhiteSpace( destination.FullPathWithFileName ) ) { throw new ArgumentNullException( nameof( destination ) ); }

            return Task.Run( function: () => {
                try {
                    var computer = new Computer();
                    computer.FileSystem.MoveFile( sourceFileName: this.FullPathWithFileName, destinationFileName: destination.FullPathWithFileName, showUI: UIOption.AllDialogs, onUserCancel: UICancelOption.DoNothing );

                    return computer.FileSystem.FileExists( file: destination.FullPathWithFileName );
                }
                catch ( FileNotFoundException ) { return false; }
                catch ( OperationCanceledException ) { return false; }
                catch ( PathTooLongException ) { return false; }
                catch ( NotSupportedException ) { return false; }
                catch ( SecurityException ) { return false; }
                catch ( IOException ) { return false; }
            } );
        }

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <seealso cref="Path.GetFileNameWithoutExtension" />
        [NotNull]
        public String Name() => this.FileName();

        public async Task<String> ReadStringAsync() {
            using ( var reader = new StreamReader( this.FullPathWithFileName ) ) { return await reader.ReadToEndAsync().ConfigureAwait( false ); }
        }

        public void Refresh() => this.Info.Refresh();

        /// <summary>
        ///     <para>Performs a byte by byte file comparison, but ignores the <see cref="Document" /> file names.</para>
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

            if ( right is null || !this.Exists() || !right.Exists() ) { return false; }

            return this.Length == right.Length && this.AsBytes().SequenceEqual( second: right.AsBytes() );
        }

        public void SetCreationTime( DateTime when, CancellationToken cancellationToken ) {
            IOExtensions.ReTry( ioFunction: () => {
                if ( !this.Exists() ) { return false; }

                this.Info.IsReadOnly = false;

                return !this.Info.IsReadOnly;
            }, tryFor: Seconds.Five, token: cancellationToken );

            IOExtensions.ReTry( ioFunction: () => {
                if ( !this.Exists() ) { return false; }

                File.SetCreationTime( this.FullPathWithFileName, creationTime: when );

                return true;
            }, tryFor: Seconds.Five, token: cancellationToken );
        }

        /// <summary>
        ///     <para>Gets the current <seealso cref="Length" /> of the <see cref="Document" />.</para>
        /// </summary>
        public Int64 Size() => this.Length;

        /// <summary>
        ///     Open the file for reading and return a <see cref="StreamReader" />.
        /// </summary>
        /// <returns></returns>
        public StreamReader StreamReader() => new StreamReader( File.OpenRead( this.FullPathWithFileName ) );

        /// <summary>
        ///     Open the file for writing and return a <see cref="StreamWriter" />.
        /// </summary>
        /// <returns></returns>
        public StreamWriter StreamWriter() => new StreamWriter( File.OpenWrite( this.FullPathWithFileName ) );

        /// <summary>
        ///     Return this <see cref="Document" /> as a JSON string.
        /// </summary>
        /// <returns></returns>
        public async Task<String> ToJSON() {
            using ( var reader = new StreamReader( this.FullPathWithFileName ) ) { return await reader.ReadToEndAsync().ConfigureAwait( false ); }
        }

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
                if ( !this.Exists() ) { return true; }

                this.Delete();
                this.Refresh();

                return !this.Exists();
            }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) {

                // IOExcception is thrown when the file is in use by any process.
                if ( stopwatch.Elapsed <= tryFor ) {
                    Thread.CurrentThread.Fraggle( timeSpan: Seconds.One );

                    goto TryAgain;
                }
            }
            catch ( UnauthorizedAccessException ) { }
            catch ( ArgumentNullException ) { }
            finally { stopwatch.Stop(); }

            return null;
        }

        /// <summary>
        ///     Uploads this <see cref="Document" /> to the given <paramref name="destination" />.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public async Task<(Exception exception, WebHeaderCollection responseHeaders)> UploadFile( [NotNull] Uri destination ) {
            if ( destination == null ) { throw new ArgumentNullException( nameof( destination ) ); }

            if ( !destination.IsWellFormedOriginalString() ) { return (new ArgumentException( $"Destination address '{destination.OriginalString}' is not well formed.", nameof( destination ) ), null); }

            try {
                using ( var webClient = new WebClient() ) {
                    await webClient.UploadFileTaskAsync( destination, this.FullPathWithFileName ).ConfigureAwait( false );

                    return (null, webClient.ResponseHeaders);
                }
            }
            catch ( Exception exception ) { return (exception, null); }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}