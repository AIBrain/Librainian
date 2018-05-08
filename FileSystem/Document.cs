// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Document.cs" was last cleaned by Protiguous on 2018/05/06 at 4:10 PM

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
    using Internet;
    using JetBrains.Annotations;
    using Magic;
    using Maths;
    using Maths.Numbers;
    using Measurement.Time;
    using Microsoft.VisualBasic.Devices;
    using Microsoft.VisualBasic.FileIO;
    using Newtonsoft.Json;
    using Parsing;
    using Persistence;
    using Security;
    using Threading;

    /// <summary>
    /// <para>An immutable wrapper for a file, the extension, the [parent] folder, and the file's size all from a given full path.</para>
    /// <para>Also contains static String versions from <see cref="Path"/></para>
    /// </summary>
    [Immutable]
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public class Document : ABetterClassDispose, IEquatable<Document>, IEnumerable<Byte>, IComparable<Document> {

        /// <inheritdoc />
        public override void DisposeManaged() {
            if ( this.DeleteAfterClose ) {
                this.Delete();
            }
            base.DisposeManaged();
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private Document() => throw new NotImplementedException( message: "Private contructor is not allowed." );

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
            if ( String.IsNullOrWhiteSpace( value: fullPathWithFilename ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fullPathWithFilename ) );
            }

            this.DeleteAfterClose = deleteAfterClose;

            this.Info = new FileInfo( fileName: fullPathWithFilename );
        }

        public Document( [NotNull] FileSystemInfo info, Boolean deleteAfterClose = false ) : this( fullPathWithFilename: info.FullName, deleteAfterClose: deleteAfterClose ) { }

        public Document( [NotNull] Folder folder, [NotNull] String filename, Boolean deleteAfterClose = false ) : this( fullPath: folder.FullName, filename: filename, deleteAfterClose: deleteAfterClose ) { }

        public Document( Folder folder, Document document, Boolean deleteAfterClose = false ) : this( fullPathWithFilename: Path.Combine( path1: folder.FullName, path2: document.FileName() ),
            deleteAfterClose: deleteAfterClose ) { }

        public Boolean DeleteAfterClose { get; }

        /// <summary>
        /// <para>The <see cref="Folder"/> where this <see cref="Document"/> is stored.</para>
        /// </summary>
        [NotNull]
        public Folder Folder => new Folder( fileSystemInfo: this.Info.Directory );

        [NotNull]
        public FileInfo Info { get; }

        /// <summary>
        /// <para>The <see cref="Folder"/> combined with the <see cref="FileName"/>.</para>
        /// </summary>
        [JsonProperty]
        [NotNull]
        public String FullPathWithFileName => this.Info.FullName;

        /// <summary>
        /// <para>Static case sensitive comparison of the file names and file sizes for equality.</para>
        /// <para>To compare the contents of two <see cref="Document"/> use <see cref="SameContent(Document)"/>.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Document left, [CanBeNull] Document right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return false;
            }

            return left.Size() == right.Size() && left.FullPathWithFileName.Same( compare: right.FullPathWithFileName );
        }

        /// <summary>
        /// Returns a unique file in the user's temp folder.
        /// <para>If an extension is not provided, a random extension (a <see cref="Guid"/>) will be used.</para>
        /// <para><b>Note</b>: Does not create a 0-byte file like <see cref="Path.GetTempFileName"/>.</para>
        /// <para>If the temp folder is not found, one attempt will be made to create it.</para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        [NotNull]
        public static Document GetTempDocument( String extension = null ) {
            if ( extension != null ) {
                extension = extension.Trim();
                while ( extension.StartsWith( value: "." ) ) {
                    extension = extension.Substring( startIndex: 1 );
                }
            }

            if ( String.IsNullOrWhiteSpace( value: extension ) ) {
                extension = Guid.NewGuid().ToString();
            }

            return new Document( folder: Folder.GetTempFolder(), filename: Guid.NewGuid() + "." + extension.Trim() );
        }

        public static implicit operator FileInfo( Document document ) => document.Info;

        /// <summary>
        /// <para>Compares the file names (case insensitive) and file sizes for inequality.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=( Document left, Document right ) => !Equals( left: left, right: right );

        /// <summary>
        /// <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==( Document left, Document right ) => Equals( left: left, right: right );

        /// <summary>
        /// <para>If the file does not exist, it is created.</para>
        /// <para>Then the <paramref name="text"/> is appended to the file.</para>
        /// </summary>
        /// <param name="text"></param>
        public void AppendText( String text ) {
            if ( !this.Folder.Create() ) {
                throw new DirectoryNotFoundException( message: this.FullPathWithFileName );
            }

            if ( this.Exists() ) {
                using ( var writer = File.AppendText( path: this.FullPathWithFileName ) ) {
                    writer.WriteLine( value: text );
                    writer.Flush();
                }
            }
            else {
                using ( var writer = File.CreateText( path: this.FullPathWithFileName ) ) {
                    writer.WriteLine( value: text );
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Enumerates the <see cref="Document"/> as a sequence of <see cref="Byte"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Byte> AsByteArray() {
            if ( !this.Exists() ) {
                yield break;
            }

            var stream = IOExtensions.ReTry( ioFunction: () => new FileStream( path: this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ), tryFor: Seconds.Seven,
                token: CancellationToken.None );

            if ( null == stream ) {
                yield break;
            }

            if ( !stream.CanRead ) {
                throw new NotSupportedException( message: $"Cannot read from file {this.FullPathWithFileName}" );
            }

            using ( stream ) {
                using ( var buffered = new BufferedStream( stream: stream ) ) {
                    var b = buffered.ReadByte();
                    if ( b == -1 ) {
                        yield break;
                    }

                    yield return ( Byte )b;
                }
            }
        }

        /// <summary>
        /// Compares this. <see cref="FullPathWithFileName"/> against other <see cref="FullPathWithFileName"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo( Document other ) => String.Compare( strA: this.FullPathWithFileName, strB: other.FullPathWithFileName, comparisonType: StringComparison.Ordinal );

        /// <summary>
        /// Starts a task to copy a file
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="progress">   </param>
        /// <param name="eta">        </param>
        /// <returns></returns>
        public Task Copy( Document destination, Action<Double> progress, Action<TimeSpan> eta ) =>
            Task.Run( action: () => {
                var computer = new Computer();

                //TODO file monitor/watcher?
                computer.FileSystem.CopyFile( sourceFileName: this.FullPathWithFileName, destinationFileName: destination.FullPathWithFileName, showUI: UIOption.AllDialogs, onUserCancel: UICancelOption.DoNothing );
            } );

        /// <summary>
        /// Returns the <see cref="WebClient"/> if a file copy was started.
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

        [CanBeNull]
        public String Crc32() {
            if ( !this.Folder.Exists() ) {
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
                var crc32 = new Crc32( polynomial: ( UInt32 )size.Value, seed: ( UInt32 )size.Value );

                using ( var fileStream = File.Open( path: this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
                    return crc32.ComputeHash( inputStream: fileStream ).Aggregate( seed: String.Empty, func: ( current, b ) => current + b.ToString( format: "x2" ).ToLower() );
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
            if ( !this.Folder.Exists() ) {
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
                var crc64 = new Crc64( polynomial: size.Value, seed: size.Value );

                using ( var fileStream = File.Open( path: this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
                    return crc64.ComputeHash( inputStream: fileStream ).Aggregate( seed: String.Empty, func: ( current, b ) => current + b.ToString( format: "x2" ).ToLower() );
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
        /// <para>Returns true if the <see cref="Document"/> no longer exists.</para>
        /// <para>Defaults to 3 retries</para>
        /// </summary>
        /// <returns></returns>
        public Boolean Delete( Int32 retries = 3 ) {
            TryAgain:
            try {
                if ( !this.Exists() ) {
                    return true;
                }

                if ( this.Info.IsReadOnly ) {
                    this.Info.IsReadOnly = false;
                }

                if ( this.Exists() ) {
                    File.Delete( path: this.FullPathWithFileName );
                }

                return !this.Exists();
            }
            catch ( IOException ) {

                //file in use?
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
        /// <para>Downloads (replaces) the local document with the specified <paramref name="source"/>.</para>
        /// <para>Note: will replace the content of the this <see cref="Document"/>.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public async Task<(Exception exception, WebHeaderCollection responseHeaders)> DownloadFile( [NotNull] Uri source ) {
            if ( source == null ) {
                throw new ArgumentNullException( paramName: nameof( source ) );
            }

            try {
                if ( !source.IsWellFormedOriginalString() ) {
                    return ( new DownloadException( message: $"Could not use source Uri '{source}'." ), null );
                }

                using ( var webClient = new WebClient() ) {
                    await webClient.DownloadFileTaskAsync( source, this.FullPathWithFileName ).ConfigureAwait( continueOnCapturedContext: false );
                    return (null, webClient.ResponseHeaders);
                }
            }
            catch ( Exception exception ) {
                return ( exception , null);
            }
        }

        /// <summary>
        /// Uploads this <see cref="Document"/> to the given <paramref name="destination"/>.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public async Task<(Exception exception, WebHeaderCollection responseHeaders)> UploadFile( [NotNull] Uri destination ) {
            if ( destination == null ) {
                throw new ArgumentNullException( paramName: nameof( destination ) );
            }

            if ( !destination.IsWellFormedOriginalString() ) {
                return ( new ArgumentException( $"Destination address '{destination.OriginalString}' is not well formed.", nameof( destination ) ), null );
            }

            try {
                using ( var webClient = new WebClient() ) {
                    await webClient.UploadFileTaskAsync( destination, this.FullPathWithFileName ).ConfigureAwait( continueOnCapturedContext: false );
                    return (null, webClient.ResponseHeaders);
                }
            }
            catch ( Exception exception ) {
                return ( exception, null );
            }
        }

        

        /// <summary>
        /// <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        /// <para>To compare the contents of two <see cref="Document"/> use <see cref="SameContent(Document)"/>.</para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( Document other ) => !( other is null ) && Equals( left: this, right: other );

        /// <summary>
        /// <para>To compare the contents of two <see cref="Document"/> use SameContent( Document,Document).</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals( Object obj ) => obj is Document && Equals( left: this, right: ( Document )obj );

        /// <summary>
        /// Returns true if the <see cref="Document"/> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        public Boolean Exists() => this.Info.Exists;

        /// <summary>
        /// <para>Computes the extension of the <see cref="FileName"/>, including the prefix ".".</para>
        /// </summary>
        [NotNull]
        public String Extension() => Path.GetExtension( path: this.FullPathWithFileName ).Trim().NullIfEmptyOrWhiteSpace() ?? String.Empty;

        /// <summary>
        /// <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <seealso cref="Path.GetFileName"/>
        [NotNull]
        public String FileName() => Path.GetFileName( path: this.FullPathWithFileName );

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<Byte> GetEnumerator() => this.AsByteArray().GetEnumerator();

        /// <summary>
        /// (file name, not contents)
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode() => this.FullPathWithFileName.GetHashCode();

        /// <summary>
        /// <para>Gets the current size of the <see cref="Document"/>.</para>
        /// </summary>
        [CanBeNull]
        public UInt64? GetLength() {
            try {
                if ( this.Exists() ) {
                    return ( UInt64 )this.Info.Length;
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
        /// Attempt to start the process.
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
                catch ( Exception exception ) {
                    exception.More();
                }

                return null;
            } );

        /// <summary>
        /// Starts a task to copy a file
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public Task<Boolean> Move( [NotNull] Document destination ) {
            if ( destination is null || String.IsNullOrWhiteSpace( value: destination.FullPathWithFileName ) ) {
                throw new ArgumentNullException( paramName: nameof( destination ) );
            }

            return Task.Run( function: () => {
                try {
                    var computer = new Computer();
                    computer.FileSystem.MoveFile( sourceFileName: this.FullPathWithFileName, destinationFileName: destination.FullPathWithFileName, showUI: UIOption.AllDialogs, onUserCancel: UICancelOption.DoNothing );
                    return computer.FileSystem.FileExists( file: destination.FullPathWithFileName );
                }
                catch ( FileNotFoundException ) {
                    return false;
                }
                catch ( OperationCanceledException ) {
                    return false;
                }
                catch ( PathTooLongException ) {
                    return false;
                }
                catch ( NotSupportedException ) {
                    return false;
                }
                catch ( SecurityException ) {
                    return false;
                }
                catch ( IOException ) {
                    return false;
                }
            } );
        }

        /// <summary>
        /// <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <seealso cref="Path.GetFileNameWithoutExtension"/>
        [NotNull]
        public String Name() => this.FileName();

        /// <summary>
        /// Reads the entire file into a <see cref="String"/>.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public async Task<String> ReadTextAsync() {
            try {
                return await Task.Run( function: () => this.Exists() ? File.ReadAllText( path: this.FullPathWithFileName ) : String.Empty );
            }
            catch ( Exception ) {
                return null;
            }
        }

        public async Task<String> ReadToEndAsync() {
            using ( var reader = new StreamReader( path: this.FullPathWithFileName ) ) {
                return await reader.ReadToEndAsync().ConfigureAwait( continueOnCapturedContext: false );
            }
        }

        /// <summary>
        /// <para>Performs a byte by byte file comparison, but ignores the <see cref="Document"/> file names.</para>
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
            if ( right is null ) {
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

            return ll.Value == rl.Value && this.AsByteArray().SequenceEqual( second: right.AsByteArray() );
        }

        public void SetCreationTime( DateTime when, CancellationToken cancellationToken ) {
            IOExtensions.ReTry( ioFunction: () => {
                if ( !this.Exists() ) {
                    return false;
                }

                this.Info.IsReadOnly = false;
                return !this.Info.IsReadOnly;
            }, tryFor: Seconds.Five, token: cancellationToken );

            IOExtensions.ReTry( ioFunction: () => {
                if ( !this.Exists() ) {
                    return false;
                }

                File.SetCreationTime( path: this.FullPathWithFileName, creationTime: when );
                return true;
            }, tryFor: Seconds.Five, token: cancellationToken );
        }

        /// <summary>
        /// <para>Gets the current size of the <see cref="Document"/>.</para>
        /// </summary>
        /// <seealso cref="GetLength"/>
        [CanBeNull]
        public UInt64? Size() => this.GetLength();

        /// <summary>
        /// Return this <see cref="Document"/> as a JSON string.
        /// </summary>
        /// <returns></returns>
        public async Task<String> ToJSON() {
            Ini bob = new Ini();
            bob.
            using ( var reader = new StreamReader( path: this.FullPathWithFileName ) ) {
                using ( var writer = new StreamWriter( outfile.FullPathWithFileName ) ) {
                    await writer.WriteAsync( buffer ).ConfigureAwait(false);
                }

                return await reader.ReadToEndAsync().ConfigureAwait( continueOnCapturedContext: false );
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => this.FullPathWithFileName;

        /// <summary>
        /// <para>Returns true if the <see cref="Document"/> no longer seems to exist.</para>
        /// <para>Returns null if existence cannot be determined.</para>
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

                this.Delete();

                return !File.Exists( path: this.FullPathWithFileName );
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
            finally {
                stopwatch.Stop();
            }

            return null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /*
        public static String IndexToPath( Folder baseFolder, UInt64 index ) {
            if ( baseFolder is null ) {
                throw new ArgumentNullException( paramName: nameof( baseFolder ) );
            }

            var path = baseFolder.FullName;
            foreach ( var c in index.ToString() ) {
                path = @"\" + c;
            }
            return path;
        }
        */
        /*
        [Test]
        public static void Test_IndexToPath() {
            var largestEmptiestDrive = IOExtensions.GetLargestEmptiestDrive();
            var baseFolder = new Folder( largestEmptiestDrive.RootDirectory.FullName + @"\test\" );
        }
        */
        /*

        /// <summary>
        /// </summary>
        /// <param name="uri">     </param>
        /// <param name="download"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="WebException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public Document( [NotNull] Uri uri, Boolean download = true ) {
            if ( uri is null ) {
                throw new ArgumentNullException( nameof( uri ) );
            }

            var tempFolder = Folder.GetTempFolder();
            if ( null == tempFolder ) {
                throw new DirectoryNotFoundException( "Unable to find user's temp folder." );
            }

            var webClient = new WebClient();
            webClient.DownloadFileAsync( uri, downloadLocation.FullPathWithFileName );

            this.Info = new FileInfo( downloadLocation.FullPathWithFileName );
        }
        */

        /*

        //https://stackoverflow.com/questions/21661798/how-do-we-access-mft-through-c-sharp
        public SafeFileHandle GetVolumeHandle( NativeMethods.EFileAccess access = NativeMethods.EFileAccess.AccessSystemSecurity | NativeMethods.EFileAccess.GenericRead | NativeMethods.EFileAccess.ReadControl ) {
            var attributes = ( UInt32 )NativeMethods.EFileAttributes.BackupSemantics;
            var handle = NativeMethods.CreateFile( this.FullPathWithFileName, access, 7U, IntPtr.Zero, ( UInt32 )NativeMethods.ECreationDisposition.OpenExisting, attributes, IntPtr.Zero );
            if ( handle.IsInvalid ) {
                throw new IOException( "Bad path" );
            }

            return handle;
        }
        */
        /// <summary>
        ///     Return an object loaded from a JSON text file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [CanBeNull]
        public T LoadJSON<T>() {

            if ( !this.Exists() ) {
                return default;
            }

            try {
                using ( var textReader = File.OpenText( this.FullPathWithFileName ) ) {
                    using ( var jsonReader = new JsonTextReader( textReader ) ) {
                        return PersistenceExtensions.LocalJsonSerializers.Value.Deserialize<T>( jsonReader );
                    }
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return default;
        }

        public async Task<T> LoadJSONAsync<T>() {
          return  await Task.Run( () => this.LoadJSON<T>() ).ConfigureAwait(false);
        }
    }


}