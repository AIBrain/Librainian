// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "$FILENAME$" last touched on $CURRENT_YEAR$-$CURRENT_MONTH$-$CURRENT_DAY$ at $CURRENT_TIME$ by Protiguous.

#nullable enable

namespace Librainian.FileSystem;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using JetBrains.Annotations;
using Maths;
using Maths.Numbers;
using PooledAwait;

public interface IDocument : IEquatable<IDocument>, IAsyncEnumerable<Byte> {

	/// <summary>
	///     Largest amount of memory that will be allocated for file reads.
	/// <para>1 gibibyte</para>
	/// </summary>
	public const Int32 MaximumBufferSize = 1024 * 1024 * 1024;

	public Byte[]? Buffer { get; set; }

	/// <summary>Local file creation <see cref="DateTime" />.</summary>
	public DateTime? CreationTime { get; set; }

	/// <summary>Gets or sets the file creation time, in coordinated universal time (UTC).</summary>
	public DateTime? CreationTimeUtc { get; set; }

	//FileAttributeData FileAttributeData { get; }

	public Boolean DeleteAfterClose { get; set; }

	/// <summary>
	///     <para>Just the file's name, including the extension.</para>
	/// </summary>
	/// <example>
	///     <code>new Document("C:\Temp\Test.text").FileName() == "Test.text"</code>
	/// </example>
	/// <see cref="Pri.LongPath.Path.GetFileName" />
	public String FileName { get; }

	/// <summary>
	///     Represents the fully qualified path of the file.
	///     <para>Fully qualified "Drive:\Path\Folder\Filename.Ext"</para>
	/// </summary>
	public String FullPath { get; }

	Boolean IsBufferLoaded { get; }

	/// <summary>Gets or sets the time the current file was last accessed.</summary>
	public DateTime? LastAccessTime { get; set; }

	/// <summary>Gets or sets the UTC time the file was last accessed.</summary>
	public DateTime? LastAccessTimeUtc { get; set; }

	/// <summary>Gets or sets the time when the current file or directory was last written to.</summary>
	public DateTime? LastWriteTime { get; set; }

	/// <summary>Gets or sets the UTC datetime when the file was last written to.</summary>
	public DateTime? LastWriteTimeUtc { get; set; }

	/// <summary>
	///     <para>Just the file's name, including the extension.</para>
	/// </summary>
	/// <see cref="Pri.LongPath.Path.GetFileNameWithoutExtension" />
	public String Name { get; }

	public PathTypeAttributes PathTypeAttributes { get; }

	/// <summary>Anything that can be temp stored can go in this. Not serialized. Defaults to be used for internal locking.</summary>
	public Object? Tag { get; set; }

	FileStream? Writer { get; set; }

	StreamWriter? WriterStream { get; set; }

	/// <summary>
	///     <para>If the file does not exist, it is created.</para>
	///     <para>Then the <paramref name="text" /> is appended to the file.</para>
	/// </summary>
	/// <param name="text"></param>
	/// <param name="cancellationToken"></param>
	public PooledValueTask<IDocument> AppendText( String text, CancellationToken cancellationToken );

	/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Byte" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	public IAsyncEnumerable<Byte> AsBytes( CancellationToken cancellationToken );

	/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	IAsyncEnumerable<Decimal> AsDecimal( CancellationToken cancellationToken );

	/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Guid" />.</summary>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	public IAsyncEnumerable<Guid> AsGuids( CancellationToken cancellationToken );

	/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.</summary>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	public IAsyncEnumerable<Int32> AsInt32( CancellationToken cancellationToken );

	/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	public IAsyncEnumerable<Int64> AsInt64( CancellationToken cancellationToken );

	/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="UInt64" />.</summary>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	public IAsyncEnumerable<UInt64> AsUInt64( CancellationToken cancellationToken );

	/// <summary>
	///     <para>Clone the entire document to the <paramref name="destination" /> as quickly as possible.</para>
	///     <para>this will OVERWRITE any <see cref="destination" /> file.</para>
	/// </summary>
	/// <param name="destination"></param>
	/// <param name="progress">   </param>
	/// <param name="eta">        </param>
	/// <param name="cancellationToken"></param>
	public PooledValueTask<(Status success, TimeSpan timeElapsed)> CloneDocument(
		IDocument destination,
		IProgress<Single> progress,
		IProgress<TimeSpan> eta,
		CancellationToken cancellationToken
	);

	public IFolder ContainingingFolder();

	Task<FileCopyData> Copy( FileCopyData fileCopyData, CancellationToken cancellationToken );

	public PooledValueTask<Int32?> CRC32( CancellationToken cancellationToken );

	/// <summary>Returns a lowercase hex-string of the hash.</summary>
	public PooledValueTask<String?> CRC32Hex( CancellationToken cancellationToken );

	public PooledValueTask<Int64?> CRC64( CancellationToken cancellationToken );

	/// <summary>Returns a lowercase hex-string of the hash.</summary>
	public PooledValueTask<String?> CRC64Hex( CancellationToken cancellationToken );

	/// <summary>Deletes the file.</summary>
	public PooledValueTask Delete( CancellationToken cancellationToken );

	/// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
	void DisposeManaged();

	/// <summary>
	///     <para>Downloads (replaces) the local document with the specified <paramref name="source" />.</para>
	///     <para>Note: will replace the content of the this <see cref="IDocument" />.</para>
	/// </summary>
	/// <param name="source"></param>
	public PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> DownloadFile( Uri source );

	//TODO PooledValueTask<UInt64?> RealSizeOnDisk( CancellationToken cancellationToken );
	//TODO PooledValueTask<UInt64?> AllocatedSizeOnDisk( CancellationToken cancellationToken );
	/// <summary>
	///     <para>To compare the contents of two <see cref="IDocument" /> use SameContent( IDocument,IDocument).</para>
	/// </summary>
	/// <param name="other"></param>
	public Boolean Equals( Object other );

	/// <summary>Returns whether the file exists.</summary>
	[Pure]
	public PooledValueTask<Boolean> Exists( CancellationToken cancellationToken );

	/// <summary>
	///     <para>Computes the extension of the <see cref="FileName" />, including the prefix ".".</para>
	/// </summary>
	public String Extension();

	/// <summary>Returns an enumerator that iterates through the collection.</summary>
	/// <returns>A <see cref="IEnumerator" /> that can be used to iterate through the collection.</returns>
	IAsyncEnumerator<Byte> GetEnumerator();

	/// <summary>
	///     Synchronous version.
	/// </summary>
	Boolean GetExists();

	/// <summary>Create and returns a new <see cref="FileInfo" /> object for <see cref="Document.FullPath" />.</summary>
	/// <see cref="Document.op_Implicit" />
	/// <see cref="Document.ToFileInfo" />
	PooledValueTask<FileInfo> GetFreshInfo( CancellationToken cancellationToken );

	/// <summary>(file name, not contents)</summary>
	public Int32 GetHashCode();

	/// <summary>
	///     Synchronous version.
	/// </summary>
	/// <returns></returns>
	UInt64? GetLength();

	void GetObjectData( SerializationInfo info, StreamingContext context );

	/// <summary>
	///     <para>
	///         Can we allocate a full 2GB buffer?
	///     </para>
	///     <para>See the file "App.config" for setting gcAllowVeryLargeObjects to true.</para>
	/// </summary>
	public PooledValueTask<Int32?> GetOptimalBufferSize( CancellationToken cancellationToken );

	/// <summary>HarkerHash (hash-by-addition)</summary>
	PooledValueTask<Int32> HarkerHash32( CancellationToken cancellationToken );

	PooledValueTask<Int64> HarkerHash64( CancellationToken cancellationToken );

	/// <summary>"poor mans Decimal hash"</summary>
	PooledValueTask<Decimal> HarkerHashDecimal( CancellationToken cancellationToken );

	PooledValueTask<Boolean> IsAll( Byte number, CancellationToken cancellationToken );

	/// <summary>Returns the filename, without the extension.</summary>
	public String JustName();

	/// <summary>Attempt to start the process.</summary>
	/// <param name="arguments"></param>
	/// <param name="verb">     "runas" is elevated</param>
	/// <param name="useShell"></param>
	public PooledValueTask<Process?> Launch( String? arguments = null, String verb = "runas", Boolean useShell = false );

	/// <summary>Returns the length of the file (if it exists).</summary>
	public PooledValueTask<UInt64?> Length( CancellationToken cancellationToken );

	/*

    /// <summary>Returns the <see cref="WebClient" /> if a file copy was started.</summary>
    /// <param name="destination"></param>
    /// <param name="onProgress"> </param>
    /// <param name="onCompleted"></param>
    /// <returns></returns>
    (PooledValueTask? task, Exception? exception, Status Exception) Copy( [NotNull] IDocument destination,
        [NotNull] Action<(IDocument, UInt64 bytesReceived, UInt64 totalBytesToReceive)> onProgress, [NotNull] Action onCompleted );
    */

	/// <summary>Attempt to load the entire file into memory. If it throws, it throws..</summary>
	PooledValueTask<Status> LoadDocumentIntoBuffer( CancellationToken cancellationToken );

	/// <summary>
	///     Attempt to return an object Deserialized from a JSON text file.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="progress"></param>
	/// <param name="cancellationToken"></param>
	public PooledValueTask<(Status status, T? obj)> LoadJSON<T>( IProgress<ZeroToOne>? progress, CancellationToken cancellationToken );

	/// <summary>
	///     Opens an existing file or creates a new file for writing.
	///     <para>Should be able to read and write from <see cref="FileStream" />.</para>
	///     <para>If there is any error opening or creating the file, <see cref="Document.Writer" /> will be null.</para>
	/// </summary>
	public PooledValueTask<FileStream?> OpenWriter( Boolean deleteIfAlreadyExists, CancellationToken cancellationToken, FileShare sharingOptions = FileShare.None );

	IAsyncEnumerable<String> ReadLines( CancellationToken cancellationToken );

	public PooledValueTask<String> ReadStringAsync();

	/// <summary>
	///     Releases the <see cref="FileStream" /> opened by <see cref="OpenWriter" />.
	/// </summary>
	public void ReleaseWriter();

	/// <summary>
	///     <para>Performs a byte by byte file comparison, but ignores the <see cref="IDocument" /> file names.</para>
	/// </summary>
	/// <param name="right"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	/// <exception cref="SecurityException"></exception>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="UnauthorizedAccessException"></exception>
	/// <exception cref="PathTooLongException"></exception>
	/// <exception cref="NotSupportedException"></exception>
	/// <exception cref="IOException"></exception>
	/// <exception cref="DirectoryNotFoundException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	public PooledValueTask<Boolean> SameContent( Document? right, CancellationToken cancellationToken );

	/// <summary>
	///     <para>If the file does not exist, return <see cref="Status.Error" />.</para>
	///     <para>If an exception happens, return <see cref="Status.Exception" />.</para>
	///     <para>Otherwise, return <see cref="Status.Success" />.</para>
	/// </summary>
	/// <param name="value"></param>
	/// <param name="cancellationToken"></param>
	PooledValueTask<Status> SetReadOnly( Boolean value, CancellationToken cancellationToken );

	/// <summary>Returns the size of the file, if it exists.</summary>
	public PooledValueTask<UInt64?> Size( CancellationToken cancellationToken );

	/*
	 *	//TODO Move: copy to dest under guid.guid, delete old dest, rename new dest, delete source
	 *	//TODO PooledValueTask<FileCopyData> Move( FileCopyData fileData, CancellationToken cancellationToken );
	 */

	/// <summary>Open the file for reading and return a <see cref="Document.StreamReader" />.</summary>
	StreamReader StreamReader();

	/// <summary>
	///     Open the file for writing and return a <see cref="Document.StreamWriter" />.
	///     <para>Optional <paramref name="encoding" />. Defaults to <see cref="Encoding.Unicode" />.</para>
	///     <para>Optional buffersize. Defaults to 1 MB.</para>
	/// </summary>
	Task<StreamWriter?> StreamWriter( CancellationToken cancellationToken, Encoding? encoding = null, UInt32 bufferSize = MathConstants.Sizes.OneMegaByte );

	/// <summary>Return this <see cref="IDocument" /> as a JSON string.</summary>
	PooledValueTask<String?> ToJSON();

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	String ToString();

	/// <summary>
	///     <para>Returns true if this <see cref="Document" /> no longer seems to exist.</para>
	/// </summary>
	/// <param name="delayBetweenRetries"></param>
	/// <param name="cancellationToken"></param>
	PooledValueTask<Boolean?> TryDeleting( TimeSpan delayBetweenRetries, CancellationToken cancellationToken );

	PooledValueTask<Status> TurnOffReadonly( CancellationToken cancellationToken );

	PooledValueTask<Status> TurnOnReadonly( CancellationToken cancellationToken );

	/// <summary>Uploads this <see cref="IDocument" /> to the given <paramref name="destination" />.</summary>
	/// <param name="destination"></param>
	PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> UploadFile( Uri destination );
}