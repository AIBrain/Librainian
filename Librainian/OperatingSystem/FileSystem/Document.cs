// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Document.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Document.cs" was last formatted by Protiguous on 2019/08/19 at 7:22 AM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Internet;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Maths.Numbers;
    using Measurement.Time;
    using Microsoft.VisualBasic.Devices;
    using Microsoft.VisualBasic.FileIO;
    using Microsoft.Win32.SafeHandles;
    using Newtonsoft.Json;
    using Parsing;
    using Persistence;
    using Security;
    using Threading;

    public interface IDocument : IDisposable, IComparable<IDocument>, IEquatable<IDocument>, IEnumerable<Byte> {

        String FullPath { get; }

        /*

        /// <summary>
        ///     Gets or sets the <see cref="System.IO.FileAttributes" /> for <see cref="FullPath" />.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        FileAttributes? FileAttributes { get; set; }
        */

        /// <summary>
        ///     Local file creation <see cref="DateTime" />.
        /// </summary>
        DateTime? CreationTime { get; set; }

        /// <summary>
        ///     Gets or sets the file creation time, in coordinated universal time (UTC).
        /// </summary>
        DateTime? CreationTimeUtc { get; set; }

        //FileAttributeData FileAttributeData { get; }

        /// <summary>Gets or sets the time the current file was last accessed.</summary>
        DateTime? LastAccessTime { get; set; }

        /// <summary>
        ///     Gets or sets the UTC time the file was last accessed.
        /// </summary>
        DateTime? LastAccessTimeUtc { get; set; }

        /// <summary>Gets or sets the time when the current file or directory was last written to.</summary>
        DateTime? LastWriteTime { get; set; }

        /// <summary>
        ///     Gets or sets the UTC datetime when the file was last written to.
        /// </summary>
        DateTime? LastWriteTimeUtc { get; set; }

        PathTypeAttributes PathTypeAttributes { get; }

        /// <summary>
        ///     Returns the length of the file (if it exists).
        /// </summary>
        UInt64? Length { get; }

        /// <summary>
        ///     Anything that can be temp stored can go in this. Not serialized. Defaults to be used for internal locking.
        /// </summary>
        Object Tag { get; set; }

        Boolean DeleteAfterClose { get; set; }

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <example>
        ///     <code>new Document("C:\Temp\Test.text").FileName() == "Test.text"</code>
        /// </example>
        /// <see cref="Path.GetFileName" />
        String FileName { get; }

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <see cref="Path.GetFileNameWithoutExtension" />
        String Name { get; }

        /// <summary>
        ///     Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        IEnumerable<Byte> AsBytes( FileOptions options = FileOptions.SequentialScan );

        /// <summary>
        ///     "poor mans hash" heh
        /// </summary>
        /// <returns></returns>
        Int32 CalculateHarkerHashInt32();

        /// <summary>
        ///     Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.
        /// </summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        IEnumerable<Int32> AsInt32( FileOptions options = FileOptions.SequentialScan );

        /// <summary>
        ///     Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.
        /// </summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        IEnumerable<Int64> AsInt64( FileOptions options = FileOptions.SequentialScan );

        /// <summary>
        ///     Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Guid" />.
        /// </summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        IEnumerable<Guid> AsGuid( FileOptions options = FileOptions.SequentialScan );

        /// <summary>
        ///     Enumerates the <see cref="IDocument" /> as a sequence of <see cref="UInt64" />.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        IEnumerable<UInt64> AsUInt64( FileOptions options = FileOptions.SequentialScan );

        /// <summary>
        ///     HarkerHash (hash-by-addition) ( )
        /// </summary>
        /// <remarks>
        ///     <para>The result is the same, No Matter What Order the bytes are read in. Right?</para>
        ///     <para>So it should be able to be read in parallel..</para>
        /// </remarks>
        /// <returns></returns>
        Int32 CalcHashInt32( Boolean inParallel = true );

        /// <summary>Deletes the file.</summary>
        void Delete();

        /// <summary>Returns whether the file exists.</summary>
        Boolean Exists();

        Folder ContainingingFolder();

        /// <summary>
        ///     <para>Clone the entire document to the <paramref name="destination" /> as quickly as possible.</para>
        ///     <para>this will OVERWRITE any <see cref="destination" /> file.</para>
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="token"></param>
        /// <param name="progress">   </param>
        /// <param name="eta">        </param>
        /// <returns></returns>
        Task<(Boolean success, TimeSpan timeElapsed)> Clone( [NotNull] IDocument destination, CancellationToken token, [CanBeNull] IProgress<Single> progress = null,
            [CanBeNull] IProgress<TimeSpan> eta = null );

        /// <summary>
        ///     Starts a task to copy a file
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="progress">   </param>
        /// <param name="eta">        </param>
        /// <returns></returns>
        Task Copy( IDocument destination, IProgress<Single> progress, IProgress<TimeSpan> eta );

        /// <summary>
        ///     Returns the <see cref="WebClient" /> if a file copy was started.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="onProgress"> </param>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        WebClient CopyFileWithProgress( [NotNull] IDocument destination, Action<Percentage> onProgress, Action onCompleted );

        Int32? CRC32();

        Task<Int32?> CRC32Async( CancellationToken token );

        String CRC32Hex();

        /// <summary>
        ///     Returns a lowercase hex-string of the hash.
        /// </summary>
        /// <returns></returns>
        Task<String> CRC32HexAsync( CancellationToken token );

        Int64? CRC64();

        Task<Int64?> CRC64Async( CancellationToken token );

        /// <summary>
        ///     Returns a lowercase hex-string of the hash.
        /// </summary>
        /// <returns></returns>
        String CRC64Hex();

        /// <summary>
        ///     Returns a lowercase hex-string of the hash.
        /// </summary>
        /// <returns></returns>
        Task<String> CRC64HexAsync( CancellationToken token );

        /// <summary>
        ///     <para>Downloads (replaces) the local document with the specified <paramref name="source" />.</para>
        ///     <para>Note: will replace the content of the this <see cref="IDocument" />.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        Task<(Exception exception, WebHeaderCollection responseHeaders)> DownloadFile( [NotNull] Uri source );

        /// <summary>
        ///     <para>Computes the extension of the <see cref="IDocument.FileName" />, including the prefix ".".</para>
        /// </summary>
        String Extension();

        /// <summary>
        ///     Returns the size of the file, if it exists.
        /// </summary>
        /// <returns></returns>
        UInt64? Size();

        /// <summary>
        ///     <para>If the file does not exist, it is created.</para>
        ///     <para>Then the <paramref name="text" /> is appended to the file.</para>
        /// </summary>
        /// <param name="text"></param>
        IDocument AppendText( String text );

        /// <summary>
        ///     <para>To compare the contents of two <see cref="IDocument" /> use SameContent( IDocument,IDocument).</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        Boolean Equals( Object obj );

        /// <summary>
        ///     (file name, not contents)
        /// </summary>
        /// <returns></returns>
        Int32 GetHashCode();

        Boolean HavePermission( FileIOPermissionAccess access );

        /// <summary>
        ///     Returns the filename, without the extension.
        /// </summary>
        /// <returns></returns>
        String JustName();

        /// <summary>
        ///     <para>
        ///         Could we allocate a full 2GB buffer if we wanted? that'd be really nice for the <see cref="IDocument" /> copy
        ///         routines...
        ///     </para>
        ///     <para>See the file "App.config" for setting gcAllowVeryLargeObjects to true.</para>
        /// </summary>
        Int32 GetBufferSize();

        /// <summary>
        ///     Attempt to start the process.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="verb">     "runas" is elevated</param>
        /// <param name="useShell"></param>
        /// <returns></returns>
        Task<Process> Launch( [CanBeNull] String arguments = null, String verb = "runas", Boolean useShell = false );

        /// <summary>
        ///     Attempt to return an object Deserialized from this JSON text file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T LoadJSON<T>();

        Task<T> LoadJSONAsync<T>( CancellationToken token );

        /// <summary>
        ///     <para>Starts a task to <see cref="IDocument.MoveAsync" /> a file to the <paramref name="destination" />.</para>
        ///     <para>Returns -1 if an exception happened.</para>
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="token"></param>
        /// <param name="exact">
        ///     If true, the file creation and lastwrite dates are set after the <see cref="IDocument.MoveAsync" />
        ///     .
        /// </param>
        /// <returns></returns>
        Task<UInt64?> MoveAsync( [NotNull] IDocument destination, CancellationToken token, Boolean exact = true );

        Task<String> ReadStringAsync();

        /// <summary>
        ///     <para>Performs a byte by byte file comparison, but ignores the <see cref="IDocument" /> file names.</para>
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
        Boolean SameContent( [CanBeNull] IDocument right );

        /// <summary>
        ///     Open the file for reading and return a <see cref="IDocument.StreamReader" />.
        /// </summary>
        /// <returns></returns>
        StreamReader StreamReader();

        /// <summary>
        ///     Open the file for writing and return a <see cref="IDocument.StreamWriter" />.
        /// </summary>
        /// <returns></returns>
        StreamWriter StreamWriter( [CanBeNull] Encoding encoding = null, UInt32 bufferSize = MathConstants.Sizes.OneMegaByte );

        /// <summary>
        ///     Return this <see cref="IDocument" /> as a JSON string.
        /// </summary>
        /// <returns></returns>
        Task<String> ToJSON();

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        String ToString();

        /// <summary>
        ///     <para>Returns true if the <see cref="IDocument" /> no longer seems to exist.</para>
        ///     <para>Returns null if existence cannot be determined.</para>
        /// </summary>
        /// <param name="tryFor"></param>
        /// <returns></returns>
        Boolean? TryDeleting( TimeSpan tryFor );

        /// <summary>
        ///     Uploads this <see cref="IDocument" /> to the given <paramref name="destination" />.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        Task<(Exception exception, WebHeaderCollection responseHeaders)> UploadFile( [NotNull] Uri destination );

        Task<Boolean> IsAll( Byte number );
    }

    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public class Document : IDocument {

        /// <summary>
        ///     Compares this. <see cref="FullPath" /> against other <see cref="FullPath" />.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [Pure]
        public Int32 CompareTo( [NotNull] IDocument other ) => String.Compare( strA: this.FullPath, strB: other.FullPath, comparisonType: StringComparison.Ordinal );

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the
        ///     collection.
        /// </returns>
        public IEnumerator<Byte> GetEnumerator() => this.AsBytes().GetEnumerator();

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        ///     <para>To compare the contents of two <see cref="IDocument" /> use <see cref="IDocument.SameContent" />.</para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( IDocument other ) => Equals( left: this, right: other );

        /// <summary>
        ///     Represents the fully qualified path of the file.
        ///     <para>Fully qualified "Drive:\Path\Folder\Filename.Ext"</para>
        /// </summary>
        [JsonProperty]
        [NotNull]
        public String FullPath { get; }

        /*

        /// <summary>
        ///     Gets or sets the <see cref="System.IO.FileAttributes" /> for <see cref="FullPath" />.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        [JsonIgnore]
        public FileAttributes? FileAttributes {
            get => this.FileAttributeData.FileAttributes;

            set {
                if ( value.HasValue ) {
                    if ( !NativeMethods.SetFileAttributes( fileName: this.FullPath, dwFileAttributes: ( UInt32 )value.Value ) ) {
                        NativeMethods.HandleLastError( fullPath: this.FullPath );
                    }
                }

                this._fileAttributeData.FileAttributes = value;
            }
        }
        */

        /// <summary>
        ///     Local file creation <see cref="DateTime" />.
        /// </summary>
        [JsonIgnore]
        public DateTime? CreationTime {
            get => this.CreationTimeUtc?.ToLocalTime();
            set => this.CreationTimeUtc = value?.ToUniversalTime();
        }

        /// <summary>
        ///     Gets or sets the file creation time, in coordinated universal time (UTC).
        /// </summary>
        [JsonIgnore]
        public DateTime? CreationTimeUtc {
            get => File.Exists( this.FullPath ) ? File.GetCreationTimeUtc( this.FullPath ) : default( DateTime? );

            set {
                if ( value.HasValue && File.Exists( this.FullPath ) ) {
                    File.SetCreationTimeUtc( this.FullPath, value.Value );
                }
            }
        }

        /// <summary>Gets or sets the time the current file was last accessed.</summary>
        [JsonIgnore]
        public DateTime? LastAccessTime {
            get => this.LastAccessTimeUtc?.ToLocalTime();
            set => this.LastAccessTimeUtc = value?.ToUniversalTime();
        }

        /// <summary>
        ///     Gets or sets the UTC time the file was last accessed.
        /// </summary>
        [JsonIgnore]
        public DateTime? LastAccessTimeUtc {
            get => File.Exists( this.FullPath ) ? File.GetLastAccessTimeUtc( this.FullPath ) : default( DateTime? );

            set {
                if ( value.HasValue && File.Exists( this.FullPath ) ) {
                    File.SetLastAccessTimeUtc( this.FullPath, value.Value );
                }
            }
        }

        /// <summary>Gets or sets the time when the current file or directory was last written to.</summary>
        [JsonIgnore]
        public DateTime? LastWriteTime {
            get => this.LastWriteTimeUtc?.ToLocalTime();
            set => this.LastWriteTimeUtc = value?.ToUniversalTime();
        }

        /// <summary>
        ///     Gets or sets the UTC datetime when the file was last written to.
        /// </summary>
        [JsonIgnore]
        public DateTime? LastWriteTimeUtc {
            get => File.Exists( this.FullPath ) ? File.GetLastWriteTimeUtc( this.FullPath ) : default( DateTime? );

            set {
                if ( value.HasValue && File.Exists( this.FullPath ) ) {
                    File.SetLastWriteTimeUtc( this.FullPath, value.Value );
                }
            }
        }

        [JsonIgnore]
        public PathTypeAttributes PathTypeAttributes { get; set; } = PathTypeAttributes.Unknown;

        /// <summary>
        ///     Returns the length of the file (if it exists).
        /// </summary>
        [JsonIgnore]
        public UInt64? Length {
            get {
                var info = this.GetInfo();

                return info.Exists ? ( UInt64? )info.Length : default;
            }
        }

        /// <summary>
        ///     Anything that can be temp stored can go in this. Not serialized. Defaults to be used for internal locking.
        /// </summary>
        [JsonIgnore]
        [CanBeNull]
        public Object Tag { get; set; }

        public Boolean DeleteAfterClose {
            get => this.PathTypeAttributes.HasFlag( PathTypeAttributes.DeleteAfterClose );

            set {
                if ( value ) {
                    this.PathTypeAttributes |= PathTypeAttributes.DeleteAfterClose;
                }
                else {
                    this.PathTypeAttributes &= ~PathTypeAttributes.DeleteAfterClose;
                }
            }
        }

        /// <summary>
        ///     Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        public IEnumerable<Byte> AsBytes( FileOptions options = FileOptions.SequentialScan ) {
            if ( this.Exists() == false ) {
                yield break;
            }

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneMegaByte, options: options ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}" );
                }

                var buffer = new Byte[ sizeof( Byte ) ];

                using ( var buffered = new BufferedStream( stream: stream ) ) {
                    while ( buffered.Read( array: buffer, offset: 0, count: buffer.Length ).Any() ) {
                        yield return buffer[ 0 ];
                    }
                }
            }
        }

        /// <summary>
        ///     "poor mans Int32 hash"
        /// </summary>
        /// <returns></returns>
        public Int32 CalculateHarkerHashInt32() =>
            this.AsInt32().AsParallel().WithMergeOptions( mergeOptions: ParallelMergeOptions.NotBuffered )
                .Aggregate( seed: 0, func: ( current, i ) => unchecked(current + i) );

        [NotNull]
        public Task<Int32> CalculateHarkerHashInt32Async() => Task.Run( this.CalculateHarkerHashInt32 );

        /// <summary>
        ///     "poor mans Int64 hash"
        /// </summary>
        /// <returns></returns>
        public Int64 CalculateHarkerHashInt64() =>
            this.AsInt64().AsParallel().WithMergeOptions( mergeOptions: ParallelMergeOptions.NotBuffered )
                .Aggregate( seed: 0L, func: ( current, i ) => unchecked(current + i) );

        [NotNull]
        public Task<Int64> CalculateHarkerHashInt64Async() => Task.Run( this.CalculateHarkerHashInt64 );

        /// <summary>
        ///     Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.
        /// </summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        public IEnumerable<Int32> AsInt32( FileOptions options = FileOptions.SequentialScan ) {
            if ( this.Exists() == false ) {
                yield break;
            }

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneGigaByte, options: options ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}." );
                }

                var buffer = new Byte[ sizeof( Int32 ) ];

                using ( var buffered = new BufferedStream( stream: stream, sizeof( Int32 ) ) ) {
                    while ( buffered.Read( array: buffer, offset: 0, count: buffer.Length ).Any() ) {
                        yield return BitConverter.ToInt32( value: buffer, startIndex: 0 );
                    }
                }
            }
        }

        /// <summary>
        ///     Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.
        /// </summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        public IEnumerable<Int64> AsInt64( FileOptions options = FileOptions.SequentialScan ) {
            if ( this.Exists() == false ) {
                yield break;
            }

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneGigaByte, options: options ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}." );
                }

                var buffer = new Byte[ sizeof( Int64 ) ];

                using ( var buffered = new BufferedStream( stream: stream, sizeof( Int64 ) ) ) {
                    while ( buffered.Read( array: buffer, offset: 0, count: buffer.Length ).Any() ) {
                        yield return BitConverter.ToInt64( value: buffer, startIndex: 0 );
                    }
                }
            }
        }

        /// <summary>
        ///     Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.
        /// </summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        public IEnumerable<Guid> AsGuid( FileOptions options = FileOptions.SequentialScan ) {
            if ( this.Exists() == false ) {
                yield break;
            }

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneGigaByte, options: options ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}." );
                }

                var buffer = new Byte[ sizeof( Decimal ) ];

                using ( var buffered = new BufferedStream( stream: stream, sizeof( Decimal ) ) ) {
                    while ( buffered.Read( array: buffer, offset: 0, count: buffer.Length ).Any() ) {
                        yield return new Guid( buffer );
                    }
                }
            }
        }

        /// <summary>
        ///     Enumerates the <see cref="IDocument" /> as a sequence of <see cref="UInt64" />.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        public IEnumerable<UInt64> AsUInt64( FileOptions options = FileOptions.SequentialScan ) {
            if ( this.Exists() == false ) {
                yield break;
            }

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneGigaByte, options: options ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}" );
                }

                var bytes = new Byte[ sizeof( UInt64 ) ];

                using ( var buffered = new BufferedStream( stream: stream, sizeof( UInt64 ) ) ) {
                    while ( buffered.Read( array: bytes, offset: 0, count: bytes.Length ).Any() ) {
                        yield return BitConverter.ToUInt64( value: bytes, startIndex: 0 );
                    }
                }
            }
        }

        /// <summary>
        ///     HarkerHash (hash-by-addition) ( )
        /// </summary>
        /// <remarks>
        ///     <para>The result is the same, No Matter What Order the bytes are read in. Right?</para>
        ///     <para>So it should be able to be read in parallel..</para>
        /// </remarks>
        /// <returns></returns>
        public Int32 CalcHashInt32( Boolean inParallel = true ) {

            var result = 0;

            if ( inParallel ) {
                result = this.CalculateHarkerHashInt32();
            }
            else {
                foreach ( var b in this.AsInt32() ) {
                    unchecked {
                        result += b == 0 ? 1 : b;
                    }
                }
            }

            return result;
        }

        /// <summary>Deletes the file.</summary>
        public void Delete() {
            var fileInfo = this.GetInfo();

            if ( !fileInfo.Exists ) {
                return;
            }

            if ( fileInfo.IsReadOnly ) {
                fileInfo.IsReadOnly = false;
            }

            fileInfo.Delete();
        }

        /// <summary>Returns whether the file exists.</summary>
        [DebuggerStepThrough]
        public Boolean Exists() => this.GetInfo().Exists;

        [NotNull]
        public Folder ContainingingFolder() {
            if ( this._containingFolder == default ) {
                var dir = Path.GetDirectoryName( path: this.FullPath );

                if ( String.IsNullOrEmpty( value: dir ) ) {

                    //empty means a root-level folder (C:\) was found. Right?
                    dir = Path.GetPathRoot( path: this.FullPath );
                }

                this._containingFolder = new Folder( fullPath: dir );
            }

            return this._containingFolder;
        }

        /// <summary>
        ///     <para>Clone the entire IDocument to the <paramref name="destination" /> as quickly as possible.</para>
        ///     <para>this will OVERWRITE any <see cref="destination" /> file.</para>
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="token"></param>
        /// <param name="progress">   </param>
        /// <param name="eta">        </param>
        /// <returns></returns>
        public async Task<(Boolean success, TimeSpan timeElapsed)> Clone( [NotNull] IDocument destination, CancellationToken token,
            [CanBeNull] IProgress<Single> progress = null, [CanBeNull] IProgress<TimeSpan> eta = null ) {
            if ( destination == null ) {
                throw new ArgumentNullException( paramName: nameof( destination ) );
            }

            var stopwatch = Stopwatch.StartNew();

            try {
                if ( this.Length.Any() ) {
                    if ( Uri.TryCreate( uriString: this.FullPath, uriKind: UriKind.Absolute, result: out var sourceAddress ) ) {
                        using ( var client = new WebClient().Add( token: token ) ) {

                            await client.DownloadFileTaskAsync( address: sourceAddress, fileName: destination.FullPath ).ConfigureAwait( false );

                            return (true, stopwatch.Elapsed);
                        }
                    }
                }
            }
            catch ( WebException exception ) {
                exception.Log();
            }

            return (false, stopwatch.Elapsed);
        }

        /// <summary>
        ///     Starts a task to copy a file
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="progress">   </param>
        /// <param name="eta">        </param>
        /// <returns></returns>
        [NotNull]
        public Task Copy( IDocument destination, IProgress<Single> progress, IProgress<TimeSpan> eta ) =>
            Task.Run( action: () => {
                var computer = new Computer();

                //TODO file monitor/watcher?
                computer.FileSystem.CopyFile( sourceFileName: this.FullPath, destinationFileName: destination.FullPath, showUI: UIOption.AllDialogs,
                    onUserCancel: UICancelOption.DoNothing );
            } );

        /// <summary>
        ///     Returns the <see cref="WebClient" /> if a file copy was started.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="onProgress"> </param>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        [NotNull]
        public WebClient CopyFileWithProgress( [NotNull] IDocument destination, Action<Percentage> onProgress, Action onCompleted ) {

            var webClient = new WebClient();

            webClient.DownloadProgressChanged += ( sender, args ) => {
                var percentage = new Percentage( numerator: ( BigInteger )args.BytesReceived, denominator: args.TotalBytesToReceive );
                onProgress?.Invoke( obj: percentage );
            };

            webClient.DownloadFileCompleted += ( sender, args ) => onCompleted?.Invoke();

            webClient.DownloadFileAsync( address: new Uri( uriString: this.FullPath ), fileName: destination.FullPath );

            return webClient;
        }

        public Int32? CRC32() {
            try {

                var size = this.Size();

                if ( size?.Any() == true ) {
                    using ( var fileStream = File.OpenRead( path: this.FullPath ) ) {
                        var crc32 = new CRC32( polynomial: ( UInt32 )size, seed: ( UInt32 )size );

                        var result = crc32.ComputeHash( inputStream: fileStream );

                        return BitConverter.ToInt32( value: result, startIndex: 0 );
                    }
                }

                return null;
            }
            catch ( FileNotFoundException exception ) {
                exception.Break();
            }
            catch ( DirectoryNotFoundException exception ) {
                exception.Break();
            }
            catch ( PathTooLongException exception ) {
                exception.Break();
            }
            catch ( IOException exception ) {
                exception.Break();
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.Break();
            }

            return null;
        }

        [NotNull]
        public Task<Int32?> CRC32Async( CancellationToken token ) => Task.Run( function: this.CRC32, cancellationToken: token );

        public String CRC32Hex() {
            try {
                if ( this.Exists() == false ) {
                    return null;
                }

                using ( var fileStream = File.OpenRead( path: this.FullPath ) ) {
                    var size = this.Size();

                    if ( !size.HasValue ) {
                        return null;
                    }

                    var crc32 = new CRC32( polynomial: ( UInt32 )size.Value, seed: ( UInt32 )size.Value );

                    return crc32.ComputeHash( inputStream: fileStream )
                        .Aggregate( seed: String.Empty, func: ( current, b ) => current + b.ToString( format: "x2" ).ToLower() );
                }
            }
            catch ( FileNotFoundException exception ) {
                exception.Break();
            }
            catch ( DirectoryNotFoundException exception ) {
                exception.Break();
            }
            catch ( PathTooLongException exception ) {
                exception.Break();
            }
            catch ( IOException exception ) {
                exception.Break();
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.Break();
            }

            return null;
        }

        /// <summary>
        ///     Returns a lowercase hex-string of the hash.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public Task<String> CRC32HexAsync( CancellationToken token ) => Task.Run( function: this.CRC32Hex, cancellationToken: token );

        public Int64? CRC64() {
            try {
                if ( this.Exists() == false ) {
                    return null;
                }

                using ( var fileStream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
                    var size = this.Size();

                    if ( !size.HasValue || !size.Any() ) {
                        return null;
                    }

                    var crc64 = new CRC64( polynomial: size.Value, seed: size.Value );

                    return BitConverter.ToInt64( value: crc64.ComputeHash( inputStream: fileStream ), startIndex: 0 );
                }
            }
            catch ( FileNotFoundException exception ) {
                exception.Break();
            }
            catch ( DirectoryNotFoundException exception ) {
                exception.Break();
            }
            catch ( PathTooLongException exception ) {
                exception.Break();
            }
            catch ( IOException exception ) {
                exception.Break();
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.Break();
            }

            return null;
        }

        [NotNull]
        public Task<Int64?> CRC64Async( CancellationToken token ) => Task.Run( function: this.CRC64, cancellationToken: token );

        /// <summary>
        ///     Returns a lowercase hex-string of the hash.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public String CRC64Hex() {
            try {
                if ( this.Exists() == false ) {
                    return null;
                }

                var size = this.Size();

                if ( !size.HasValue || !size.Any() ) {
                    return null;
                }

                var crc64 = new CRC64( polynomial: size.Value, seed: size.Value );

                using ( var fileStream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                    bufferSize: MathConstants.Sizes.OneMegaByte ) ) {
                    return crc64.ComputeHash( inputStream: fileStream )
                        .Aggregate( seed: String.Empty, func: ( current, b ) => current + b.ToString( format: "x2" ).ToLower() );
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
        ///     Returns a lowercase hex-string of the hash.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public Task<String> CRC64HexAsync( CancellationToken token ) => Task.Run( function: this.CRC64Hex, cancellationToken: token );

        public void Dispose() {
            this.ReleaseWriterStream();

            this.ReleaseWriter();

            if ( this.DeleteAfterClose ) {
                this.Delete();
            }
        }

        private void ReleaseWriterStream() {
            using ( this.WriterStream ) {
                this.WriterStream = default;
            }
        }

        /// <summary>
        ///     <para>Downloads (replaces) the local IDocument with the specified <paramref name="source" />.</para>
        ///     <para>Note: will replace the content of the this <see cref="IDocument" />.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public async Task<(Exception exception, WebHeaderCollection responseHeaders)> DownloadFile( [NotNull] Uri source ) {
            if ( source == null ) {
                throw new ArgumentNullException( paramName: nameof( source ) );
            }

            try {
                if ( !source.IsWellFormedOriginalString() ) {
                    return (new DownloadException( message: $"Could not use source Uri '{source}'." ), null);
                }

                using ( var webClient = new WebClient() ) {
                    await webClient.DownloadFileTaskAsync( address: source, fileName: this.FullPath ).ConfigureAwait( false );

                    return (null, webClient.ResponseHeaders);
                }
            }
            catch ( Exception exception ) {
                return (exception, null);
            }
        }

        /// <summary>
        ///     <para>Computes the extension of the <see cref="FileName" />, including the prefix ".".</para>
        /// </summary>
        [NotNull]
        public String Extension() => Path.GetExtension( path: this.FullPath ).Trim().NullIfEmptyOrWhiteSpace() ?? String.Empty;

        /// <summary>
        ///     <para>Just the file's name, including the extension (no path).</para>
        /// </summary>
        /// <example>
        ///     <code>new Document("C:\Temp\Test.text").FileName() == "Test.text"</code>
        /// </example>
        /// <see cref="Path.GetFileName" />
        [NotNull]
        public String FileName => Path.GetFileName( path: this.FullPath );

        /// <summary>
        ///     Returns the size of the file, if it exists.
        /// </summary>
        /// <returns></returns>
        public UInt64? Size() => this.Length;

        /// <summary>
        ///     <para>If the file does not exist, it is created.</para>
        ///     <para>Then the <paramref name="text" /> is appended to the file.</para>
        /// </summary>
        /// <param name="text"></param>
        [NotNull]
        public IDocument AppendText( String text ) {
            var folder = this.ContainingingFolder();

            if ( !folder.Exists() ) {
                if ( !Directory.CreateDirectory( path: folder.FullName ).Exists ) {
                    throw new DirectoryNotFoundException( message: $"Could not create folder \"{folder.FullName}\"." );
                }
            }

            this.SetReadOnly( false );

            using ( var writer = File.AppendText( path: this.FullPath ) ) {
                writer.WriteLine( value: text );
            }

            return this;
        }

        /// <summary>
        ///     <para>To compare the contents of two <see cref="IDocument" /> use SameContent( IDocument,IDocument).</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals( Object obj ) => obj is IDocument document && Equals( left: this, right: document );

        /// <summary>
        ///     (file name, not contents)
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode() => this.FullPath.GetHashCode();

        public Boolean HavePermission( FileIOPermissionAccess access ) {
            try {
                var bob = new FileIOPermission( access: access, path: this.FullPath );
                bob.Demand();

                return true;
            }
            catch ( ArgumentException exception ) {
                exception.Log();
            }
            catch ( SecurityException ) { }

            return false;
        }

        /// <summary>
        ///     Returns the filename, without the extension.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public String JustName() => Path.GetFileNameWithoutExtension( path: this.FileName );

        /// <summary>
        ///     <para>
        ///         Could we allocate a full 2GB buffer if we wanted? that'd be really nice for the <see cref="IDocument" /> copy
        ///         routines...
        ///     </para>
        ///     <para>See the file "App.config" for setting gcAllowVeryLargeObjects to true.</para>
        /// </summary>
        public Int32 GetBufferSize() {
            var oursize = this.Size();

            if ( !oursize.Any() ) {

                //empty document? no buffer!
                return default;
            }

            if ( oursize <= Int32.MaxValue ) {
                return ( Int32 )oursize;
            }

            return Int32.MaxValue;
        }

        /// <summary>
        ///     Attempt to start the process.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="verb">     "runas" is elevated</param>
        /// <param name="useShell"></param>
        /// <returns></returns>
        [NotNull]
        public Task<Process> Launch( [CanBeNull] String arguments = null, String verb = "runas", Boolean useShell = false ) {
            try {
                var info = new ProcessStartInfo( fileName: this.FullPath ) {
                    Arguments = arguments ?? String.Empty,
                    UseShellExecute = useShell,
                    Verb = verb
                };

                return Task.Run( function: () => Process.Start( startInfo: info ) );
            }
            catch ( Exception exception ) {
                exception.Log();

                return Task.FromException<Process>( exception: exception );
            }
        }

        /// <summary>
        ///     Attempt to return an object Deserialized from this JSON text file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [CanBeNull]
        public T LoadJSON<T>() {
            if ( this.Exists() == false ) {
                return default;
            }

            try {
                using ( var textReader = File.OpenText( path: this.FullPath ) ) {
                    using ( var jsonReader = new JsonTextReader( reader: textReader ) ) {
                        return new JsonSerializer {
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                            PreserveReferencesHandling = PreserveReferencesHandling.All
                        }.Deserialize<T>( reader: jsonReader );
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        [NotNull]
        public Task<T> LoadJSONAsync<T>( CancellationToken token ) => Task.Run( function: this.LoadJSON<T>, cancellationToken: token );

        /// <summary>
        ///     <para>Starts a task to <see cref="MoveAsync" /> a file to the <paramref name="destination" />.</para>
        ///     <para>Returns -1 if an exception happened.</para>
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="token"></param>
        /// <param name="exact">If true, the file creation and lastwrite dates are set after the <see cref="MoveAsync" />.</param>
        /// <returns></returns>
        [NotNull]
        public Task<UInt64?> MoveAsync( [NotNull] IDocument destination, CancellationToken token, Boolean exact = true ) {
            if ( destination == null ) {
                throw new ArgumentNullException( paramName: nameof( destination ) );
            }

            return Task.Run( () => {
                try {
                    if ( destination.Exists() ) {
                        if ( destination.SameContent( this ) ) {

                            if ( exact ) {
                                if ( destination.Exists() ) {
                                    var documentInfo = new DocumentInfo( document: this );

                                    if ( documentInfo.CreationTimeUtc.HasValue ) {
                                        destination.CreationTimeUtc = documentInfo.CreationTimeUtc.Value;
                                    }

                                    if ( documentInfo.LastWriteTimeUtc.HasValue ) {
                                        destination.LastWriteTimeUtc = documentInfo.LastWriteTimeUtc.Value;
                                    }
                                }
                            }

                            this.Delete();

                            return destination.Size();
                        }
                    }

                    thisComputer.FileSystem.MoveFile( sourceFileName: this.FullPath, destinationFileName: destination.FullPath, showUI: UIOption.OnlyErrorDialogs,
                        onUserCancel: UICancelOption.DoNothing );

                    if ( exact ) {
                        if ( destination.Exists() ) {
                            var documentInfo = new DocumentInfo( document: this );

                            if ( documentInfo.CreationTimeUtc.HasValue ) {
                                destination.CreationTimeUtc = documentInfo.CreationTimeUtc.Value;
                            }

                            if ( documentInfo.LastWriteTimeUtc.HasValue ) {
                                destination.LastWriteTimeUtc = documentInfo.LastWriteTimeUtc.Value;
                            }
                        }
                    }

                    return destination.Size();
                }
                catch ( FileNotFoundException exception ) {
                    exception.Break();

                    return default;
                }
                catch ( DirectoryNotFoundException exception ) {
                    exception.Break();

                    return default;
                }
                catch ( PathTooLongException exception ) {
                    exception.Break();

                    return default;
                }
                catch ( IOException exception ) {
                    exception.Break();

                    return default;
                }
                catch ( UnauthorizedAccessException exception ) {
                    exception.Break();

                    return default;
                }
            }, token );
        }

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <see cref="Path.GetFileNameWithoutExtension" />
        [NotNull]
        public String Name => this.FileName;

        public async Task<String> ReadStringAsync() {
            using ( var reader = new StreamReader( path: this.FullPath ) ) {
                return await reader.ReadToEndAsync().ConfigureAwait( false );
            }
        }

        /// <summary>
        ///     <para>Performs a byte by byte file comparison, but ignores the <see cref="IDocument" /> file names.</para>
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
        public Boolean SameContent( [CanBeNull] IDocument right ) {

            if ( right == null || this.Exists() == false || right.Exists() == false ) {
                return false;
            }

            return this.Length == right.Length && this.AsGuid().SequenceEqual( second: right.AsGuid() );
        }

        /// <summary>
        ///     Open the file for reading and return a <see cref="StreamReader" />.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public StreamReader StreamReader() => new StreamReader( stream: File.OpenRead( path: this.FullPath ) );

        /// <summary>
        /// Opens an existing file or creates a new file for writing.
        /// <para>Should be able to read and write from <see cref="FileStream"/>.</para>
        /// <para>If there is any error opening or creating the file, <see cref="Writer"/> will be default.</para>
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public FileStream OpenWriter() {
            try {
                this.ReleaseWriter();

                if ( this.Exists() ) {
                    this.SetReadOnly( false );
                }

                return this.Writer = new FileStream( this.FullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None );
            }
            catch ( Exception exception ) {
                exception.Log();
            }
            this.ReleaseWriter();

            return default;
        }

        private void ReleaseWriter() {
            if ( this.Writer != default ) {
                using ( this.Writer ) {
                    this.Writer = default;
                }
            }
        }

        [CanBeNull]
        public FileStream Writer { get; set; }

        [CanBeNull]
        public StreamWriter WriterStream { get; set; }

        /// <summary>
        ///     Open the file for writing and return a <see cref="StreamWriter" />.
        /// <para>Optionally the <paramref name="encoding"/> can be given. Defaults to <see cref="Encoding.UTF8"/>.</para>
        /// <para>Optionally the buffersze can be given. Defaults to 1 MB.</para>
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public StreamWriter StreamWriter( [CanBeNull] Encoding encoding = null, UInt32 bufferSize = MathConstants.Sizes.OneMegaByte ) {
            this.ReleaseWriterStream();

            this.OpenWriter();

            if ( this.Writer == default ) {
                return default;
            }

            try {
                return this.WriterStream = new StreamWriter( stream: this.Writer, encoding: encoding ?? Encoding.UTF8, bufferSize: ( Int32 )bufferSize, leaveOpen: false );
            }
            catch ( Exception exception) {
                exception.Log();
            }

            this.ReleaseWriterStream();

            return default;
        }

        /// <summary>
        ///     Return this <see cref="IDocument" /> as a JSON string.
        /// </summary>
        /// <returns></returns>
        public async Task<String> ToJSON() {
            using ( var reader = new StreamReader( path: this.FullPath ) ) {
                return await reader.ReadToEndAsync().ConfigureAwait( false );
            }
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => this.FullPath;

        /// <summary>
        ///     <para>Returns true if the <see cref="IDocument" /> no longer seems to exist.</para>
        ///     <para>Returns null if existence cannot be determined.</para>
        /// </summary>
        /// <param name="tryFor"></param>
        /// <returns></returns>
        public Boolean? TryDeleting( TimeSpan tryFor ) {
            var stopwatch = Stopwatch.StartNew();
            TryAgain:

            try {
                if ( this.Exists() == false ) {
                    return true;
                }

                this.Delete();

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
            finally {
                stopwatch.Stop();
            }

            return null;
        }

        /// <summary>
        ///     Uploads this <see cref="IDocument" /> to the given <paramref name="destination" />.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public async Task<(Exception exception, WebHeaderCollection responseHeaders)> UploadFile( [NotNull] Uri destination ) {
            if ( destination == null ) {
                throw new ArgumentNullException( paramName: nameof( destination ) );
            }

            if ( !destination.IsWellFormedOriginalString() ) {
                return (new ArgumentException( message: $"Destination address '{destination.OriginalString}' is not well formed.", paramName: nameof( destination ) ), null);
            }

            try {
                using ( var webClient = new WebClient() ) {
                    await webClient.UploadFileTaskAsync( address: destination, fileName: this.FullPath ).ConfigureAwait( false );

                    return (null, webClient.ResponseHeaders);
                }
            }
            catch ( Exception exception ) {
                return (exception, null);
            }
        }

        [NotNull]
        public async Task<Boolean> IsAll( Byte number ) {

            if ( !this.Exists() ) {
                return false;
            }

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneGigaByte, options: FileOptions.SequentialScan ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}" );
                }

                var buffer = new Byte[ MathConstants.Sizes.OneGigaByte ];

                using ( var buffered = new BufferedStream( stream: stream ) ) {
                    Int32 bytesRead;

                    do {
                        bytesRead = await buffered.ReadAsync( buffer, offset: 0, count: buffer.Length ).ConfigureAwait( false );

                        if ( !bytesRead.Any() || buffer.Any( b => b != number ) ) {
                            return false;
                        }
                    } while ( bytesRead.Any() );
                }

                return true;
            }
        }

        private Folder _containingFolder;

        [CanBeNull]
        private Lazy<FileSystemWatcher> Watcher { get; }

        [CanBeNull]
        private Lazy<FileWatchingEvents> WatchEvents { get; }

        public static Computer thisComputer { get; } = new Computer();

        private static ThreadLocal<Lazy<WebClient>> WebClients =
            new ThreadLocal<Lazy<WebClient>>( valueFactory: () => new Lazy<WebClient>( valueFactory: () => new WebClient(), isThreadSafe: false ), trackAllValues: true );

        protected Document( [NotNull] SerializationInfo info, StreamingContext context ) {
            if ( info == null ) {
                throw new ArgumentNullException( paramName: nameof( info ) );
            }

            this.FullPath = info.GetString( name: nameof( this.FullPath ) ).TrimAndThrowIfBlank();
        }

        /// <summary>
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="deleteAfterClose"></param>
        /// <param name="watchFile"></param>
        /// <exception cref="InvalidOperationException">Unable to parse the given path.</exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        public Document( [NotNull] String fullPath, Boolean deleteAfterClose = false, Boolean watchFile = false ) {
            this.FullPath = fullPath.TrimAndThrowIfBlank();

            if ( Uri.TryCreate( uriString: fullPath, uriKind: UriKind.Absolute, result: out var uri ) ) {
                if ( uri.IsFile ) {

                    //this.FullPath = Path.GetFullPath( uri.AbsolutePath );
                    this.FullPath = Path.GetFullPath( fullPath );
                    this.PathTypeAttributes = PathTypeAttributes.Document;
                }
                else if ( uri.IsAbsoluteUri ) {
                    this.FullPath = uri.AbsolutePath; //how long can a URI be?
                    this.PathTypeAttributes = PathTypeAttributes.Uri;
                }
                else if ( uri.IsUnc ) {
                    this.FullPath = uri.AbsolutePath; //TODO verify this is the "long" path?
                    this.PathTypeAttributes = PathTypeAttributes.UNC;
                }
                else {
                    this.FullPath = fullPath;
                    this.PathTypeAttributes = PathTypeAttributes.Unknown;
#if DEBUG
                    throw new InvalidOperationException( message: $"Could not parse \"{fullPath}\"." );
#endif
                }
            }
            else {
                throw new InvalidOperationException( message: $"Could not parse \"{fullPath}\"." );
            }

            if ( deleteAfterClose ) {
                this.PathTypeAttributes &= PathTypeAttributes.DeleteAfterClose;
                Debug.Assert( this.PathTypeAttributes.HasFlag( PathTypeAttributes.DeleteAfterClose ) );
            }

            if ( watchFile ) {
                this.Watcher = new Lazy<FileSystemWatcher>( valueFactory: () => new FileSystemWatcher( path: this.ContainingingFolder().FullName, filter: this.FileName ) {
                    IncludeSubdirectories = false,
                    EnableRaisingEvents = true
                } );

                this.WatchEvents = new Lazy<FileWatchingEvents>( valueFactory: () => new FileWatchingEvents(), isThreadSafe: false );
                this.Watcher.Value.Created += ( sender, e ) => this.WatchEvents.Value.OnCreated?.Invoke( obj: e );
                this.Watcher.Value.Changed += ( sender, e ) => this.WatchEvents.Value.OnChanged?.Invoke( obj: e );
                this.Watcher.Value.Deleted += ( sender, e ) => this.WatchEvents.Value.OnDeleted?.Invoke( obj: e );
                this.Watcher.Value.Renamed += ( sender, e ) => this.WatchEvents.Value.OnRenamed?.Invoke( obj: e );
                this.Watcher.Value.Error += ( sender, e ) => this.WatchEvents.Value.OnError?.Invoke( obj: e );
            }
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private Document() => throw new NotImplementedException( message: "Private contructor is not allowed." );

        public Document( [NotNull] String fullPath, [NotNull] String filename, Boolean deleteAfterClose = false ) : this(
            fullPath: Path.Combine( path1: fullPath, path2: filename ), deleteAfterClose: deleteAfterClose ) { }

        public Document( [NotNull] FileSystemInfo info, Boolean deleteAfterClose = false ) : this( fullPath: info.FullName, deleteAfterClose: deleteAfterClose ) { }

        public Document( [NotNull] IFolder folder, [NotNull] String filename, Boolean deleteAfterClose = false ) : this( fullPath: folder.FullName, filename: filename,
            deleteAfterClose: deleteAfterClose ) { }

        public Document( [NotNull] IFolder folder, [NotNull] IDocument document, Boolean deleteAfterClose = false ) : this(
            fullPath: Path.Combine( path1: folder.FullName, path2: document.FileName ), deleteAfterClose: deleteAfterClose ) { }

        /// <summary>
        ///     Returns true if this IDocument was copied to the <paramref name="destination" />.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="progress"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public async Task<Boolean> Copy( [NotNull] IDocument destination, [CanBeNull] Action<DownloadProgressChangedEventArgs> progress = null,
            [CanBeNull] Action<AsyncCompletedEventArgs, (IDocument source, IDocument destination)> onComplete = null ) {
            if ( destination == null ) {
                throw new ArgumentNullException( paramName: nameof( destination ) );
            }

            if ( !this.Exists() ) {
                return false;
            }

            if ( destination.Exists() ) {
                destination.Delete();

                if ( destination.Exists() ) {
                    return false;
                }
            }

            if ( !this.Length.HasValue || !this.Length.Any() ) {
                using ( File.Create( destination.FullPath, 1, FileOptions.None ) ) {
                    return true; //just create an empty file
                }
            }

            var webClient = new WebClient();
            webClient.DownloadProgressChanged += ( sender, args ) => progress?.Invoke( args );
            webClient.DownloadFileCompleted += ( sender, args ) => onComplete?.Invoke( args, (this, destination) );
            await webClient.DownloadFileTaskAsync( this.FullPath, destination.FullPath ).ConfigureAwait( false );

            return destination.Exists() && destination.Size() == this.Size();
        }

        /// <summary>
        ///     I *really* dislike filenames starting with a period. Here's looking at you java..
        /// </summary>
        /// <returns></returns>
        public Boolean BadlyNamedFile() => Path.GetFileName( this.FullPath ).Like( Path.GetExtension( this.FullPath ) );

        /*
        [DebuggerStepThrough]
        public void Refresh( Boolean throwOnError = false ) {
            this._fileAttributeData.Reset();

            var handle = NativeMethods.FindFirstFile( lpFileName: this.FullPath, lpFindData: out var data );

            if ( handle.IsInvalid ) {
                if ( throwOnError ) {
                    NativeMethods.HandleLastError( fullPath: this.FullPath );

                    return;
                }

                return;
            }

            var fileAttributeData = new FileAttributeData( findData: data );
            this._fileAttributeData.FileAttributes = fileAttributeData.FileAttributes;
            this._fileAttributeData.CreationTime = fileAttributeData.CreationTime;
            this._fileAttributeData.Exists = fileAttributeData.Exists;
            this._fileAttributeData.FileSize = fileAttributeData.FileSize;
            this._fileAttributeData.LastAccessTime = fileAttributeData.LastAccessTime;
            this._fileAttributeData.LastWriteTime = fileAttributeData.LastWriteTime;
        }
        */

        /// <summary>
        ///     Create and returns a new <see cref="FileInfo" /> object for <see cref="FullPath" />.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public FileInfo GetInfo() => this;  //use the implicit operator

        [NotNull]
        public static implicit operator FileInfo( [NotNull] Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            return new FileInfo( fileName: document.FullPath );
        }

        /// <summary>
        /// <para>If the file does not exist, return <see cref="Status.Error"/>.</para>
        /// <para>If an exception happens, return <see cref="Status.Exception"/>.</para>
        /// <para>Otherwise, return <see cref="Status.Success"/>.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Status SetReadOnly( Boolean value ) {
            var info = this.GetInfo();

            if ( !info.Exists ) {
                return Status.Error;
            }

            try {
                if ( info.IsReadOnly != value ) {
                    info.IsReadOnly = value;
                }
                return Status.Success;
            }
            catch ( Exception ) {
                return Status.Exception;
            }
        }

        public Status TurnOnReadonly() => this.SetReadOnly( true );

        public Status TurnOffReadonly() => this.SetReadOnly( false );

        public virtual void GetObjectData( [NotNull] SerializationInfo info, StreamingContext context ) =>
            info.AddValue( name: "FullPath", value: this.FullPath, type: typeof( String ) );

        [StructLayout( layoutKind: LayoutKind.Sequential )]
        internal class SECURITY_ATTRIBUTES {

            internal Int32 bInheritHandle;

            internal Int32 nLength;

            internal unsafe Byte* pSecurityDescriptor = null;
        }

        /// <summary>
        ///     Maybe someday rewrite <see cref="IDocument" /> to use long path names, with faster creation, opening, and saving.
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="securityAttrs"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport( dllName: "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false )]
        private static extern SafeFileHandle CreateFile( String lpFileName, Int32 dwDesiredAccess, FileShare dwShareMode, SECURITY_ATTRIBUTES securityAttrs,
            FileMode dwCreationDisposition, Int32 dwFlagsAndAttributes, IntPtr hTemplateFile );

        /// <summary>
        ///     this seems to work great!
        /// </summary>
        /// <param name="address"></param>
        /// <param name="fileName"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static async Task DownloadFileTaskAsync( [NotNull] Uri address, [NotNull] String fileName,
            IProgress<(Int64 BytesReceived, Int32 ProgressPercentage, Int64 TotalBytesToReceive)> progress ) {

            if ( address == null ) {
                throw new ArgumentNullException( paramName: nameof( address ) );
            }

            if ( String.IsNullOrWhiteSpace( value: fileName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fileName ) );
            }

            var tcs = new TaskCompletionSource<Object>( state: address );

            void CompletedHandler( Object cs, AsyncCompletedEventArgs ce ) {
                if ( ce.UserState != tcs ) {
                    return;
                }

                if ( ce.Error != null ) {
                    tcs.TrySetException( exception: ce.Error );
                }
                else if ( ce.Cancelled ) {
                    tcs.TrySetCanceled();
                }
                else {
                    tcs.TrySetResult( result: null );
                }
            }

            void ProgressChangedHandler( Object ps, DownloadProgressChangedEventArgs pe ) {
                if ( pe.UserState == tcs ) {
                    progress.Report( value: (pe.BytesReceived, pe.ProgressPercentage, pe.TotalBytesToReceive) );
                }
            }

            var webClient = WebClientInstance();
            Debug.Assert( condition: !webClient.IsBusy );

            try {
                webClient.DownloadFileCompleted += CompletedHandler;
                webClient.DownloadProgressChanged += ProgressChangedHandler;
                webClient.DownloadFileAsync( address: address, fileName: fileName, userToken: tcs );
                await tcs.Task.ConfigureAwait( false );
            }
            finally {
                webClient.DownloadFileCompleted -= CompletedHandler;
                webClient.DownloadProgressChanged -= ProgressChangedHandler;
            }
        }

        private static WebClient WebClientInstance() => WebClients.Value.Value;

        /// <summary>
        ///     Returns a unique file in the user's temp folder.
        ///     <para>If an extension is not provided, a random extension (a <see cref="System.Guid" />) will be used.</para>
        ///     <para><b>Note</b>: Does not create a 0-byte file like <see cref="Path.GetTempFileName" />.</para>
        ///     <para>If the temp folder is not found, one attempt will be made to create it.</para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        [NotNull]
        public static IDocument GetTempDocument( String extension = null ) {
            if ( String.IsNullOrEmpty( extension ) ) {
                extension = Guid.NewGuid().ToString();
            }

            extension = extension.Trim();

            while ( extension.StartsWith( value: "." ) ) {
                extension = extension.Substring( startIndex: 1 ).Trim();
            }

            return new Document( folder: Folder.GetTempFolder(), filename: $"{Guid.NewGuid()}.{extension}" );
        }

        /// <summary>
        ///     <para>Compares the file names (case sensitive) and file sizes for inequality.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=( [CanBeNull] Document left, [CanBeNull] IDocument right ) => !Equals( left: left, right: right );

        /// <summary>
        ///     <para>Compares the file names (case sensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==( [CanBeNull] Document left, [CanBeNull] IDocument right ) => Equals( left: left, right: right );

        /// <summary>
        ///     <para>Static case sensitive comparison of the file names and file sizes for equality.</para>
        ///     <para>To compare the contents of two <see cref="IDocument" /> use <see cref="IDocument.SameContent" />.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] IDocument left, [CanBeNull] IDocument right ) {
            if ( ReferenceEquals( objA: left, objB: right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return false;
            }

            return left.FullPath.Is( right: right.FullPath ); //&& left.Size() == right.Size();
        }

        [Pure]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        internal static Boolean IsExtended( [NotNull] String path ) =>
            path.Length >= 4 && path[ index: 0 ] == PathInternal.Constants.Backslash && ( path[ index: 1 ] == PathInternal.Constants.Backslash || path[ index: 1 ] == '?' ) &&
            path[ index: 2 ] == '?' && path[ index: 3 ] == PathInternal.Constants.Backslash;
    }

}