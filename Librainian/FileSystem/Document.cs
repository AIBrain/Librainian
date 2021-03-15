// Copyright � Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

#nullable enable

namespace Librainian.FileSystem {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Runtime.CompilerServices;
	using System.Runtime.Serialization;
	using System.Security;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions.Warnings;
	using Internet;
	using JetBrains.Annotations;
	using Logging;
	using Maths;
	using Maths.Numbers;
    using Newtonsoft.Json;
	using Parsing;
	using PooledAwait;
	using Security;
    using Threading;
    using Utilities;
	using Directory = Pri.LongPath.Directory;
	using File = Pri.LongPath.File;
	using FileInfo = Pri.LongPath.FileInfo;
	using FileSystemInfo = Pri.LongPath.FileSystemInfo;
	using Path = Pri.LongPath.Path;

	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	public class Document : ABetterClassDispose, IDocument {
		/// <summary>
		///     Largest amount of memory that will be allocated for file reads.
		/// </summary>
		/// <remarks>About 1.8GB (90% of 2GB)</remarks>
		public const Int32 MaximumBufferSize = ( Int32 )(Int32.MaxValue * 0.9);

		[CanBeNull]
		private Folder? _containingFolder;

		protected Document( [NotNull] SerializationInfo info ) {
			if ( info is null ) {
				throw new ArgumentNullException( nameof( info ) );
			}

			this.FullPath = ( info.GetString( nameof( this.FullPath ) ) ?? throw new InvalidOperationException() ).TrimAndThrowIfBlank();
		}

		/// <summary></summary>
		/// <param name="fullPath"></param>
		/// <param name="deleteAfterClose"></param>
		/// <param name="watchFile"></param>
		/// <exception cref="InvalidOperationException">Unable to parse the given path.</exception>
		/// <exception cref="FileNotFoundException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="IOException"></exception>
		public Document( [NotNull] String fullPath, Boolean deleteAfterClose = false, Boolean watchFile = false ) {
			if ( String.IsNullOrWhiteSpace( fullPath ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fullPath ) );
			}

			this.FullPath = Path.Combine( Path.GetFullPath( fullPath ), Path.GetFileName( fullPath ) ).TrimAndThrowIfBlank();

			if ( Uri.TryCreate( fullPath, UriKind.Absolute, out var uri ) ) {
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
					throw new InvalidOperationException( $"Could not parse \"{fullPath}\"." );
#endif
				}
			}
			else {
				throw new InvalidOperationException( $"Could not parse \"{fullPath}\"." );
			}

			if ( deleteAfterClose ) {
				this.PathTypeAttributes &= PathTypeAttributes.DeleteAfterClose;
				Debug.Assert( this.PathTypeAttributes.HasFlag( PathTypeAttributes.DeleteAfterClose ) );
			}

			if ( watchFile ) {
				this.Watcher = new Lazy<FileSystemWatcher>( () => new FileSystemWatcher( this.ContainingingFolder().FullPath, this.FileName ) {
					IncludeSubdirectories = false,
					EnableRaisingEvents = true
				} );

				this.WatchEvents = new Lazy<FileWatchingEvents>( () => new FileWatchingEvents(), false );

				var watcher = this.Watcher.Value;

				watcher.Created += ( _, e ) => this.WatchEvents.Value.OnCreated( e );
				watcher.Changed += ( _, e ) => this.WatchEvents.Value.OnChanged( e );
				watcher.Deleted += ( _, e ) => this.WatchEvents.Value.OnDeleted( e );
				watcher.Renamed += ( _, e ) => this.WatchEvents.Value.OnRenamed( e );
				watcher.Error += ( _, e ) => this.WatchEvents.Value.OnError( e );
			}
		}

		private Document() => throw new NotImplementedException( "Private contructor is not allowed." );

		public Document( [NotNull] String justPath, [NotNull] String filename, Boolean deleteAfterClose = false ) : this( Path.Combine( justPath, filename ),
			deleteAfterClose ) { }

		public Document( [NotNull] FileSystemInfo info, Boolean deleteAfterClose = false ) : this( info.FullPath, deleteAfterClose ) { }

		public Document( [NotNull] IFolder folder, [NotNull] String filename, Boolean deleteAfterClose = false ) : this( folder.FullPath, filename, deleteAfterClose ) { }

		public Document( [NotNull] IFolder folder, [NotNull] IDocument document, Boolean deleteAfterClose = false ) : this( Path.Combine( folder.FullPath, document.FileName ),
			deleteAfterClose ) { }

		[CanBeNull]
		public Byte[]? Buffer { get; set; }

		public Boolean IsBufferLoaded {
			get;
			[Pure]
			private set;
		}

		[CanBeNull]
		public FileStream? Writer { get; set; }

		[CanBeNull]
		public StreamWriter? WriterStream { get; set; }

		[CanBeNull]
		private Lazy<FileSystemWatcher>? Watcher { get; }

		[CanBeNull]
		private Lazy<FileWatchingEvents>? WatchEvents { get; }

		[NotNull]
		public static String InvalidFileNameCharacters { get; } = new( Path.InvalidFileNameChars );

		[NotNull]
		public static Lazy<Regex> RegexForInvalidFileNameCharacters { get; } = new( () =>
			new Regex( $"[{Regex.Escape( InvalidFileNameCharacters )}]", RegexOptions.Compiled | RegexOptions.Singleline ) );

		[NotNull]
		private ThreadLocal<JsonSerializer> JsonSerializers { get; } = new( () => new JsonSerializer {
			ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
			PreserveReferencesHandling = PreserveReferencesHandling.All
		} );

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>A <see cref="IEnumerator" /> that can be used to iterate through the collection.</returns>
		[Pure]
		public IEnumerator<Byte> GetEnumerator() => this.AsBytes().GetEnumerator();

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		/// <summary>
		///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
		///     <para>To compare the contents of two <see cref="IDocument" /> use <see cref="IDocument.SameContent" />.</para>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		[Pure]
		public Boolean Equals( [CanBeNull] IDocument? other ) => Equals( this, other );

		/// <summary>
		///     Represents the fully qualified path of the file.
		///     <para>Fully qualified "Drive:\Path\Folder\Filename.Ext"</para>
		/// </summary>
		[JsonProperty]
		[NotNull]
		public String FullPath { get; }

		/// <summary>Local file creation <see cref="DateTime" />.</summary>
		[JsonIgnore]
		public DateTime? CreationTime {
			get => this.CreationTimeUtc?.ToLocalTime();
			set => this.CreationTimeUtc = value?.ToUniversalTime();
		}

		/// <summary>Gets or sets the file creation time, in coordinated universal time (UTC).</summary>
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

		/// <summary>Gets or sets the UTC time the file was last accessed.</summary>
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

		/// <summary>Gets or sets the UTC datetime when the file was last written to.</summary>
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

		/// <summary>Returns the length of the file (default if it doesn't exists).</summary>
		[JsonIgnore]
		public UInt64? Length {
			get {
				var info = this.GetFreshInfo();

				if ( info.Exists ) {
					return ( UInt64 )info.Length;
				}

				return null;
			}
		}

		/// <summary>Anything that can be temp stored can go in this. Not serialized. Defaults to be used for internal locking.</summary>
		[JsonIgnore]
		[CanBeNull]
		public Object? Tag { get; set; }

		public Boolean DeleteAfterClose {
			get => this.PathTypeAttributes.HasFlag( PathTypeAttributes.DeleteAfterClose );

			set {
				if ( value ) {
					this.PathTypeAttributes |= PathTypeAttributes.DeleteAfterClose;
					Debug.Assert( this.PathTypeAttributes.HasFlag( PathTypeAttributes.DeleteAfterClose ) );
				}
				else {
					this.PathTypeAttributes &= ~PathTypeAttributes.DeleteAfterClose;
					Debug.Assert( this.PathTypeAttributes.HasFlag( PathTypeAttributes.DeleteAfterClose ) );
				}
			}
		}

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Byte" />.</summary>
		/// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
		/// <returns></returns>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		[NotNull]
		[Pure]
		public IEnumerable<Byte> AsBytes( FileOptions options = FileOptions.SequentialScan ) {
			if ( !this.Exists() ) {
				yield break;
			}

			using var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, MathConstants.Sizes.OneMegaByte, options );

			if ( !stream.CanRead ) {
				throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath}" );
			}

			var buffer = new Byte[ sizeof( Byte ) ];

			using var buffered = new BufferedStream( stream );

			while ( buffered.Read( buffer, 0, buffer.Length ).Any() ) {
				yield return buffer[ 0 ];
			}
		}

		/// <summary>"simple Int32 hash"</summary>
		/// <returns></returns>
		[Pure]
		public Int32 CalculateHarkerHashInt32() => this.AsInt32().AsParallel().AsUnordered().WithMergeOptions( ParallelMergeOptions.AutoBuffered )
			.Aggregate( 0, ( current, i ) => unchecked(current + i) );

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.</summary>
		/// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		/// <returns></returns>
		[NotNull]
		[Pure]
		public IEnumerable<Int32> AsInt32( FileOptions options = FileOptions.SequentialScan ) {
			if ( !this.Exists() ) {
				yield break;
			}

			using var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, MathConstants.Sizes.OneGigaByte, options );

			if ( !stream.CanRead ) {
				throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath}." );
			}

			var buffer = new Byte[ sizeof( Int32 ) ];

			using var buffered = new BufferedStream( stream, sizeof( Int32 ) );

			while ( buffered.Read( buffer, 0, buffer.Length ).Any() ) {
				yield return BitConverter.ToInt32( buffer, 0 );
			}
		}

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
		/// <param name="token"></param>
		/// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		/// <returns></returns>
		[Pure]
		public async IAsyncEnumerable<Int64> AsInt64( [EnumeratorCancellation] CancellationToken token,
			FileOptions options = FileOptions.Asynchronous | FileOptions.SequentialScan ) {
			if ( !this.Exists() ) {
				yield break;
			}

			await using var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, MathConstants.Sizes.OneGigaByte, options );

			if ( !stream.CanRead ) {
				throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath}." );
			}

			var buffer = new Byte[ sizeof( Int64 ) ];
			var length = buffer.Length;

			await using var buffered = new BufferedStream( stream, MathConstants.Sizes.OneGigaByte );

			while ( ( await buffered.ReadAsync( buffer.AsMemory( 0, length ), token ) ).Any() ) {
				yield return BitConverter.ToInt64( buffer, 0 );
			}
		}

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.</summary>
		/// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		/// <returns></returns>
		[NotNull]
		[Pure]
		public IEnumerable<Guid> AsGuids( FileOptions options = FileOptions.SequentialScan ) {
			if ( this.Exists() is false ) {
				yield break;
			}

			using var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, MathConstants.Sizes.OneGigaByte, options );

			if ( !stream.CanRead ) {
				throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath}." );
			}

			var buffer = new Byte[ sizeof( Decimal ) ]; //sizeof( Decimal ) == sizeof( Guid ) right?

			using var buffered = new BufferedStream( stream, sizeof( Decimal ) );

			while ( buffered.Read( buffer, 0, buffer.Length ).Any() ) {
				yield return new Guid( buffer );
			}
		}

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="UInt64" />.</summary>
		/// <returns></returns>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		[Pure]
		public IEnumerable<UInt64> AsUInt64( FileOptions options = FileOptions.SequentialScan ) {
			if ( this.Exists() is false ) {
				yield break;
			}

			using var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, MathConstants.Sizes.OneGigaByte, options );

			if ( !stream.CanRead ) {
				throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath}" );
			}

			var bytes = new Byte[ sizeof( UInt64 ) ];

			using var buffered = new BufferedStream( stream, sizeof( UInt64 ) );

			while ( buffered.Read( bytes, 0, bytes.Length ).Any() ) {
				yield return BitConverter.ToUInt64( bytes, 0 );
			}
		}

		/// <summary>HarkerHash (hash-by-addition) ( )</summary>
		/// <remarks>
		///     <para>The result is the same, No Matter What Order the bytes are read in. Right?</para>
		///     <para>So it should be able to be read in parallel..</para>
		/// </remarks>
		/// <returns></returns>
		[Pure]
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
			var fileInfo = this.GetFreshInfo();

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
		[Pure]
		public Boolean Exists() => this.GetFreshInfo().Exists;

		/// <summary>
		///     <para>Clone the entire IDocument to the <paramref name="destination" /> as quickly as possible.</para>
		///     <para>this will OVERWRITE any <see cref="destination" /> file.</para>
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="token"></param>
		/// <param name="progress">   </param>
		/// <param name="eta">        </param>
		/// <returns></returns>
		[Pure]
		public async PooledValueTask<(Status success, TimeSpan timeElapsed)> Clone( [NotNull] IDocument destination, CancellationToken token,
			[CanBeNull] IProgress<Single>? progress = null, [CanBeNull] IProgress<TimeSpan>? eta = null ) {
			if ( destination is null ) {
				throw new ArgumentNullException( nameof( destination ) );
			}

			var stopwatch = Stopwatch.StartNew();

			try {
				if ( this.Length.Any() ) {
					if ( Uri.TryCreate( this.FullPath, UriKind.Absolute, out var sourceAddress ) ) {
						using var client = new WebClient().Add( token );

						await client.DownloadFileTaskAsync( sourceAddress, destination.FullPath ).ConfigureAwait( false );

						return (Status.Success, stopwatch.Elapsed);
					}
				}
			}
			catch ( WebException exception ) {
				exception.Log();
				return (Status.Exception, stopwatch.Elapsed);
			}

			return (Status.Failure, stopwatch.Elapsed);
		}

		[Pure]
		public async PooledValueTask<Int32?> CRC32( CancellationToken token ) {
			try {
				var size = this.Size();

				if ( size?.Any() is true ) {
					await using var fileStream = File.OpenRead( this.FullPath );

					using var crc32 = new CRC32( ( UInt32 )size, ( UInt32 )size );

					var hash = await crc32.ComputeHashAsync( fileStream, token );

					return BitConverter.ToInt32( hash, 0 );
				}

				return null;
			}
			catch ( FileNotFoundException exception ) {
				exception.Log();
			}
			catch ( DirectoryNotFoundException exception ) {
				exception.Log();
			}
			catch ( PathTooLongException exception ) {
				exception.Log();
			}
			catch ( IOException exception ) {
				exception.Log();
			}
			catch ( UnauthorizedAccessException exception ) {
				exception.Log();
			}

			return null;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		[Pure]
		public async PooledValueTask<String?> CRC32Hex( CancellationToken token ) {
			try {
				/*
				if ( this.Exists() is false ) {
					return default;
				}
				*/

				var size = this.Size();

				switch ( size ) {
					case null:
					case not > 0: {
						return null;
					}
					case > Int32.MaxValue / 2: {
						throw new InvalidOperationException( "File too large to convert to hex." );
					}
				}

				await using var fileStream = File.OpenRead( this.FullPath );

				using var crc32 = new CRC32( ( UInt32 )size.Value, ( UInt32 )size.Value );

				var hash = await crc32.ComputeHashAsync( fileStream, token ).ConfigureAwait( false );

				return hash.Aggregate( String.Empty, ( current, b ) => current + $"{b:X}" );
			}
			catch ( FileNotFoundException exception ) {
				exception.Log();
			}
			catch ( DirectoryNotFoundException exception ) {
				exception.Log();
			}
			catch ( PathTooLongException exception ) {
				exception.Log();
			}
			catch ( IOException exception ) {
				exception.Log();
			}
			catch ( UnauthorizedAccessException exception ) {
				exception.Break();
			}

			return null;
		}


		[Pure]
		public async PooledValueTask<Int64?> CRC64( CancellationToken token ) {
			try {
				if ( this.Exists() is false ) {
					return null;
				}

				await using var fileStream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read );

				var size = this.Size();

				if ( size > 0 ) {
					using var crc64 = new CRC64( size.Value, size.Value );

					var hash = await crc64.ComputeHashAsync( fileStream, token ).ConfigureAwait( false );

					return BitConverter.ToInt64( hash, 0 );
				}
			}
			catch ( FileNotFoundException exception ) {
				exception.Log();
			}
			catch ( DirectoryNotFoundException exception ) {
				exception.Log();
			}
			catch ( PathTooLongException exception ) {
				exception.Log();
			}
			catch ( IOException exception ) {
				exception.Log();
			}
			catch ( UnauthorizedAccessException exception ) {
				exception.Log();
			}

			return null;
		}

		/// <summary>Returns a lowercase hex-string of the hash.</summary>
		/// <returns></returns>
		[Pure]
		public async PooledValueTask<String?> CRC64Hex( CancellationToken token ) {
			try {
				if ( this.Exists() is false ) {
					return null;
				}

				await using var fileStream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read );

				var size = this.Size();

				if ( size is > 0 ) {
					using var crc64 = new CRC64( size.Value, size.Value );

					var hash = await crc64.ComputeHashAsync( fileStream, token ).ConfigureAwait( false );

					return hash.Aggregate( String.Empty, ( current, b ) => current + $"{b:X}" );
				}
			}
			catch ( FileNotFoundException exception ) {
				exception.Log();
			}
			catch ( DirectoryNotFoundException exception ) {
				exception.Log();
			}
			catch ( PathTooLongException exception ) {
				exception.Log();
			}
			catch ( IOException exception ) {
				exception.Log();
			}
			catch ( UnauthorizedAccessException exception ) {
				exception.Log();
			}

			return null;
		}

		[Pure]
		public async PooledValueTask<Boolean> IsAll( Byte number, CancellationToken token ) {
			if ( !this.IsBufferLoaded ) {
				var result = await this.LoadDocumentIntoBuffer( token ).ConfigureAwait( false );

				if ( !result.IsGood() ) {
					return false;
				}
			}

			if ( !this.IsBufferLoaded ) {
				return false;
			}

			var buffer = this.Buffer;

			if ( buffer is null ) {
				return false;
			}

			var max = buffer.Length;

			for ( var i = 0; i < max; i++ ) {
				if ( buffer[ i ] != number ) {
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///     <para>Downloads (replaces) the local IDocument with the specified <paramref name="source" />.</para>
		///     <para>Note: will replace the content of the this <see cref="IDocument" />.</para>
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		[Pure]
		public async PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> DownloadFile( [NotNull] Uri source ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			//TODO possibly download entire file, delete original version, then rename the newly downloaded file?

			try {
				if ( !source.IsWellFormedOriginalString() ) {
					return (new Exception( $"Could not use source Uri '{source}'." ), null);
				}

				using var webClient = new WebClient(); //from what I've read, Dispose should NOT be being called on a WebClient???

				await webClient.DownloadFileTaskAsync( source, this.FullPath ).ConfigureAwait( false );

				return (null, webClient.ResponseHeaders);
			}
			catch ( Exception exception ) {
				return (exception, null);
			}
		}

		/// <summary>
		///     <para>Computes the extension of the <see cref="FileName" />, including the prefix ".".</para>
		/// </summary>
		[NotNull]
		[Pure]
		public String Extension() => Path.GetExtension( this.FullPath ).Trim().NullIfEmptyOrWhiteSpace() ?? String.Empty;

		/// <summary>
		///     <para>Just the file's name, including the extension (no path).</para>
		/// </summary>
		/// <example>
		///     <code>new Document("C:\Temp\Test.text").FileName() == "Test.text"</code>
		/// </example>
		/// <see cref="Path.GetFileName" />
		[NotNull]
		public String FileName => Path.GetFileName( this.FullPath );

		/// <summary>Returns the size of the file, if it exists.</summary>
		/// <returns></returns>
		[Pure]
		public UInt64? Size() => this.Length;

		/// <summary>
		///     <para>If the file does not exist, it is created.</para>
		///     <para>Then the <paramref name="text" /> is appended to the file.</para>
		/// </summary>
		/// <param name="text"></param>
		[NotNull]
		public IDocument AppendText( [NotNull] String text ) {
			var folder = this.ContainingingFolder();

			if ( !folder.Exists() ) {
				if ( !Directory.CreateDirectory( folder.FullPath ).Exists ) {
					throw new DirectoryNotFoundException( $"Could not create folder \"{folder.FullPath}\"." );
				}
			}

			this.SetReadOnly( false );

			File.AppendAllText( this.FullPath, text );

			return this;
		}

		/// <summary>
		///     <para>To compare the contents of two <see cref="IDocument" /> use SameContent( IDocument,IDocument).</para>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		[Pure]
		public override Boolean Equals( Object? other ) => other is IDocument document && Equals( this, document );

		/// <summary>(file name, not contents)</summary>
		/// <returns></returns>
		[Pure]
		public override Int32 GetHashCode() => this.FullPath.GetHashCode();

		/// <summary>Returns the filename, without the extension.</summary>
		/// <returns></returns>
		[NotNull]
		[Pure]
		public String JustName() => Path.GetFileNameWithoutExtension( this.FileName );

		/// <summary>
		///     <para>
		///         Could we allocate a full 2GB buffer if we wanted? that'd be really nice for the <see cref="Document.Copy" />
		///         routines.
		///     </para>
		///     <para>See the file "App.config" for setting gcAllowVeryLargeObjects to true.</para>
		/// </summary>
		[Pure]
		public Int32 GetOptimalBufferSize() {
            var size = this.Size();

            return size switch {
                >= MaximumBufferSize => MaximumBufferSize,
                var _ => ( Int32 )( size ?? 4096 )
            };
        }

		/// <summary>Attempt to start the process.</summary>
		/// <param name="arguments"></param>
		/// <param name="verb">     "runas" is elevated</param>
		/// <param name="useShell"></param>
		/// <returns></returns>
		[Pure]
		public PooledValueTask<Process?> Launch( [CanBeNull] String? arguments = null, [CanBeNull] String? verb = "runas", Boolean useShell = false ) {
			try {
				var info = new ProcessStartInfo( this.FullPath ) {
					Arguments = arguments ?? String.Empty,
					UseShellExecute = useShell,
					Verb = verb ?? String.Empty
				};

				var process = Process.Start( info );
				if ( process != null ) {
					return new PooledValueTask<Process?>( process );
				}
			}
			catch ( Exception exception ) {
				exception.Log();
				throw;
			}

			return new PooledValueTask<Process?>( null );
		}

		/// <summary>
		///     <para>Just the file's name, including the extension.</para>
		/// </summary>
		/// <see cref="Path.GetFileNameWithoutExtension" />
		[NotNull]
		public String Name => this.FileName;

		[Pure]
		[NotNull]
		public Task<String> ReadStringAsync() {
			using var reader = new StreamReader( this.FullPath );

			return reader.ReadToEndAsync();
		}

		/// <summary>
		///     <para>Performs a byte by byte file comparison, but ignores the <see cref="IDocument" /> file names.</para>
		/// </summary>
		/// <param name="right"></param>
		/// <param name="token"></param>
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
		[Pure]
		public async PooledValueTask<Boolean> SameContent( [CanBeNull] Document? right, CancellationToken token ) {
			if ( right is null || !this.Exists() || !right.Exists() || this.Size() != right.Size() ) {
				return false;
			}

			if ( !this.IsBufferLoaded ) {
				await this.LoadDocumentIntoBuffer( token ).ConfigureAwait( false );
			}

			if ( !right.IsBufferLoaded ) {
				await right.LoadDocumentIntoBuffer( token ).ConfigureAwait( false );
			}

			if ( this.IsBufferLoaded && !( this.Buffer is null ) && right.IsBufferLoaded && !( right.Buffer is null ) ) {
				return this.Buffer.SequenceEqual( right.Buffer );
			}

			return this.Length == right.Length && this.AsGuids().SequenceEqual( right.AsGuids() );
		}

		/// <summary>Open the file for reading and return a <see cref="StreamReader" />.</summary>
		/// <returns></returns>
		[NotNull]
		[Pure]
		public StreamReader StreamReader() => new( File.OpenRead( this.FullPath ) );

		/// <summary>
		///     Open the file for writing and return a <see cref="StreamWriter" />.
		///     <para>Optionally the <paramref name="encoding" /> can be given. Defaults to <see cref="Encoding.UTF8" />.</para>
		///     <para>Optionally the buffersze can be given. Defaults to 1 MB.</para>
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		public StreamWriter? StreamWriter( [CanBeNull] Encoding? encoding = null, UInt32 bufferSize = MathConstants.Sizes.OneMegaByte ) {
			try {
				this.ReleaseWriterStream();

				this.OpenWriter();

				if ( this.Writer is null ) {
					return default( StreamWriter? );
				}

				return this.WriterStream = new StreamWriter( this.Writer, encoding ?? Encoding.UTF8, ( Int32 )bufferSize, false );
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			this.ReleaseWriterStream();

			return default( StreamWriter? );
		}

		/// <summary>Return this <see cref="IDocument" /> as a JSON string.</summary>
		/// <returns></returns>
		[Pure]
		public async PooledValueTask<String?> ToJSON() {
			using var reader = new StreamReader( this.FullPath );

			var readToEndAsync = await reader.ReadToEndAsync().ConfigureAwait( false );

			return readToEndAsync;
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		[Pure]
		[NotNull]
		public override String ToString() => this.FullPath;

		/// <summary>
		///     <para>Returns true if this <see cref="Document" /> no longer seems to exist.</para>
		/// </summary>
		/// <param name="delayBetweenRetries"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async PooledValueTask<Boolean?> TryDeleting( TimeSpan delayBetweenRetries, CancellationToken token ) {
			await Task.Run( async () => {
				while ( !token.IsCancellationRequested && this.Exists() ) {
					try {
						if ( this.Exists() ) {
							this.Delete();
						}
					}
					catch ( DirectoryNotFoundException ) { }
					catch ( PathTooLongException ) { }
					catch ( IOException ) {
						// IOException is thrown when the file is in use by any process.
						await Task.Delay( delayBetweenRetries, token ).ConfigureAwait( false );
					}
					catch ( UnauthorizedAccessException ) { }
					catch ( ArgumentNullException ) { }
				}
			}, token ).ConfigureAwait( false );

			return this.Exists();
		}

		/// <summary>Uploads this <see cref="IDocument" /> to the given <paramref name="destination" />.</summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		[Pure]
		public async PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> UploadFile( [NotNull] Uri destination ) {
			if ( destination is null ) {
				throw new ArgumentNullException( nameof( destination ) );
			}

			if ( !destination.IsWellFormedOriginalString() ) {
				return (new ArgumentException( $"Destination address '{destination.OriginalString}' is not well formed.", nameof( destination ) ), null);
			}

			try {
				using var webClient = new WebClient();

				await webClient.UploadFileTaskAsync( destination, this.FullPath ).ConfigureAwait( false );

				return (null, webClient.ResponseHeaders);
			}
			catch ( Exception exception ) {
				return (exception, null);
			}
		}

		/// <summary>Create and returns a new <see cref="FileInfo" /> object for <see cref="FullPath" />.</summary>
		/// <see cref="op_Implicit" />
		/// <returns></returns>
		[NotNull]
		[Pure]
		public FileInfo GetFreshInfo() => this; //use the implicit operator. see op_Implicit(FileInfo) above.

		/// <summary>Attempt to return an object Deserialized from this JSON text file.</summary>
		/// <param name="progress"></param>
		/// <param name="token"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public async PooledValueTask<(Status status, T? obj)> LoadJSON<T>( IProgress<ZeroToOne>? progress = null, CancellationToken? token = null ) {
			var i = 0.0;
			const Double maxsteps = 6.0;

			try {
				progress?.Report( ++i / maxsteps );

				if ( !this.Exists() ) {
					progress?.Report( 1 );

					return (Status.Bad, default( T? ));
				}

				progress?.Report( ++i / maxsteps );

				using var textReader = File.OpenText( this.FullPath );
				progress?.Report( ++i / maxsteps );

				var jsonReader = new JsonTextReader( textReader );
				progress?.Report( ++i / maxsteps );

				try {
					var run = await Task.Run( () => this.JsonSerializers.Value!.Deserialize<T>( jsonReader ), token ?? CancellationToken.None ).ConfigureAwait( false );

					progress?.Report( ++i / maxsteps );

					return (Status.Success, run);
				}
				finally {
					progress?.Report( ++i / maxsteps );
				}
			}
			catch ( TaskCanceledException ) {
				progress?.Report( 1 );
			}
			catch ( Exception exception ) {
				progress?.Report( 1 );
				exception.Log();
			}

			return (Status.Exception, default( T? ));
		}

		[NotNull]
		[Pure]
		public IFolder ContainingingFolder() {
			if ( this._containingFolder is null ) {
				var directoryName = Path.GetDirectoryName( this.FullPath );

				if ( String.IsNullOrWhiteSpace( directoryName ) ) {
					//empty means a root-level folder (C:\) was found. Right?
					directoryName = Path.GetPathRoot( this.FullPath );
				}

				return this._containingFolder = new Folder( directoryName );
			}

			return this._containingFolder;
		}

		[Pure]
		public async PooledValueTask<FileCopyData> Copy( FileCopyData fileCopyData, CancellationToken cancellationToken ) {

			if ( !Uri.TryCreate( fileCopyData.Source.FullPath, UriKind.RelativeOrAbsolute, out var uri ) || uri is null ) {
				throw new UriFormatException( $"Unable to parse {this.FullPath.DoubleQuote()} into a Uri." );
			}

			try {
				if ( DoesSourceFileExist() is false ) {
                    return fileCopyData with {Status = Status.Bad};
                }

				if ( CheckSourceSize( out var size ) is false || size is null ) {
                    return fileCopyData with {Status = Status.Failure};
                }

                fileCopyData.SourceSize = size.Value;

                var bytes = size.Value;
                Single bits = bytes * 8;
                const Int32 bitsPerSecond = 11 * 1024 * 1014;
                var guessSeconds = bits / bitsPerSecond;

				var estimatedTimeToCopy = TimeSpan.FromSeconds(guessSeconds * 2);

				//TODO Add in capability to pause/resume?
                await DidDownloadWork().WithTimeout( estimatedTimeToCopy, cancellationToken ).ConfigureAwait(false);
            }
            catch ( TaskCanceledException exception ) {
				//what is thrown when a Task<T> times out or is cancelled? TaskCanceledException or OperationCanceledException?
				RecordException( exception );
			}
            catch ( OperationCanceledException exception ) {
                RecordException( exception );
            }
			catch ( WebException exception ) {
				RecordException( exception );
			}
			catch ( Exception exception ) {
				RecordException( exception );
			}

			return fileCopyData;

			void RecordException( [CanBeNull] Exception? exception ) {
				if ( exception is null ) {
					return;
				}

				fileCopyData.Exceptions ??= new List<Exception>();
				fileCopyData.Exceptions.Add( exception );
			}

			Boolean DoesSourceFileExist() {
				$"Checking for existance of source file {fileCopyData.Source.FullPath.DoubleQuote()}".Verbose();
				var exists = fileCopyData.Source.Exists();
				if ( exists is not true ) {
					RecordException( new FileNotFoundException( "Missing file.", fileCopyData.Source.FullPath ) );

					return false;

				}

				return true;
			}

            Boolean CheckSourceSize( out UInt64? size ) {
                $"Checking for size of source file {fileCopyData.Source.FullPath.DoubleQuote()}".Verbose();
                size = fileCopyData.Source.Size();

                if ( size is > 0 ) {
                    return true;
                }

                RecordException( new FileNotFoundException( "Empty file.", fileCopyData.Source.FullPath ) );

                return false;

            }

            async Task<Boolean> DidDownloadWork() {
                using var webClient = new WebClient();
                $"{nameof( webClient )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()} instantiated.".Verbose();

                webClient.Disposed += ( _, _ ) => $"{nameof( webClient )} {nameof( webClient.Disposed )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()}.".Verbose();

                webClient.DownloadFileCompleted += ( _, args ) => {
                    fileCopyData.WhenCompleted = DateTime.UtcNow;

                    RecordException( args.Error );

                    if ( args.Error is null ) {
                        fileCopyData.OnCompleted?.Invoke( fileCopyData );
                    }
                };

                webClient.DownloadProgressChanged += ( _, args ) => fileCopyData.DataCopied?.Report( fileCopyData with {BytesCopied = ( UInt64 )args.BytesReceived} );

                $"{nameof( webClient )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()} started.".Verbose();
                await webClient.DownloadFileTaskAsync( uri, fileCopyData.Destination.FullPath ).ConfigureAwait( false );


                $"Checking existance of destination file {fileCopyData.Destination.FullPath.DoubleQuote()}.".Verbose();
                if ( fileCopyData.Destination.Exists() is false ) {
                    $"Could not find destination file {fileCopyData.Destination.FullPath.DoubleQuote()}.".Verbose();

                    return false;
                }

                $"Checking size of destination file {fileCopyData.Destination.FullPath.DoubleQuote()}.".Verbose();
                var destinationSize = fileCopyData.Destination.Size();

                if ( destinationSize is null ) {
                    RecordException( new Warning(
                        $"Unknown error occurred copying file {fileCopyData.Source.FullPath.DoubleQuote()} to {fileCopyData.Destination.ContainingingFolder().FullPath}." ) );

                    return false;
                }

                if ( destinationSize != fileCopyData.SourceSize ) {
                    RecordException( new Warning(
                        $"Unknown error occurred copying file {fileCopyData.Source.FullPath.DoubleQuote()} to {fileCopyData.Destination.ContainingingFolder().FullPath}." ) );

                    return false;
                }

                $"{nameof( webClient )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()} done.".Verbose();

                return true;
            }
        }

		/// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
		public override void DisposeManaged() {
			this.Buffer = default( Byte[]? );

			this.ReleaseWriterStream();

			this.ReleaseWriter();

			if ( this.DeleteAfterClose ) {
				this.Delete();
			}
		}

		/// <summary>Attempt to load the entire file into memory. If it throws, it throws..</summary>
		/// <returns></returns>
		public async PooledValueTask<Status> LoadDocumentIntoBuffer( CancellationToken token ) {
			var size = this.Size();

			switch ( size ) {
				case null: {
					return Status.Exception;
				}
				default: {
					if ( size > 0 ) {
						if ( size.Value > Int32.MaxValue ) {
							return Status.Exception;
						}
					}
					else {
						return Status.No;
					}

					break;
				}
			}

			var bytesLeft = size.Value;

			var filelength = ( Int32 )size.Value; //will we EVER have an image (or whatever) larger than Int32? (yes, probably)
			this.Buffer = new Byte[ filelength ];

			var offset = 0;

			await using var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, filelength, FileOptions.SequentialScan );

			if ( !stream.CanRead ) {
				//throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}" );
				return Status.Exception;
			}

			await using var buffered = new BufferedStream( stream );

			Int32 bytesRead;

			do {
				bytesRead = await buffered.ReadAsync( this.Buffer.AsMemory( offset, filelength ), token ).ConfigureAwait( false );
				bytesLeft -= bytesRead.Positive();

				if ( !bytesRead.Any() || !bytesLeft.Any() ) {
					this.IsBufferLoaded = true;

					return Status.Success;
				}

				offset += bytesRead;
			} while ( bytesRead.Any() && bytesLeft.Any() );

			return Status.Failure;
		}

		/// <summary>"poor mans Int64 hash"</summary>
		/// <returns></returns>
		[Pure]
		public async Task<Int64> CalculateHarkerHashInt64( CancellationToken token ) {
			this.ThrowIfNotExists();

			var total = 0L;
			await foreach ( var l in this.AsInt64( token ) ) {
				total += l;
			}

			return total;
		}

		/// <summary>"poor mans Int64 hash"</summary>
		/// <returns></returns>
		[Pure]
		public Decimal CalculateHarkerHashDecimal() {
			this.ThrowIfNotExists();

			return this.AsDecimal().AsParallel().AsUnordered().WithMergeOptions( ParallelMergeOptions.AutoBuffered ).Aggregate( 0M, ( current, i ) => current + i );
		}

		private void ThrowIfNotExists() {
			if ( !this.Exists() ) {
				throw new FileNotFoundException( $"Could find document {this.FullPath.DoubleQuote()}." );
			}
		}


		[Pure]
		public PooledValueTask<Decimal> CalculateHarkerHashDecimalAsync() {
			this.ThrowIfNotExists();

			return new PooledValueTask<Decimal>( this.CalculateHarkerHashDecimal() );
		}

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
		/// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		/// <returns></returns>
		[NotNull]
		[Pure]
		public IEnumerable<Decimal> AsDecimal( FileOptions options = FileOptions.SequentialScan ) {
			var fileLength = this.Length;

			if ( !fileLength.HasValue ) {
				yield break;
			}

			//Span<Byte> inBuffer = stackalloc Byte[ MathConstants.Sizes.OneGigaByte / sizeof( Byte ) ];
			//var inLength = inBuffer.Length;

			//Span<Decimal> outBuffer = stackalloc Decimal[ MathConstants.Sizes.OneGigaByte / sizeof( Decimal ) ];
			//var outLength = outBuffer.Length;

			using var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, MathConstants.Sizes.OneGigaByte, options );

			if ( !stream.CanRead ) {
				throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath}." );
			}

			using var buffered = new BufferedStream( stream, MathConstants.Sizes.OneGigaByte ); //TODO Is this buffering twice??

			using var br = new BinaryReader( buffered );

			while ( true ) {
				Decimal d;

				try {
					d = br.ReadDecimal();
				}
				catch ( EndOfStreamException ) {
					yield break;
				}
				catch ( IOException ) {
					yield break;
				}

				yield return d;
			}
		}

		private void ReleaseWriterStream() {
			using ( this.WriterStream ) {
				this.WriterStream = null;
			}
		}

		/// <summary>
		///     Opens an existing file or creates a new file for writing.
		///     <para>Should be able to read and write from <see cref="FileStream" />.</para>
		///     <para>If there is any error opening or creating the file, <see cref="Writer" /> will be default.</para>
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		public FileStream? OpenWriter() {
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

			return null;
		}

		private void ReleaseWriter() {
			using ( this.Writer ) {
				this.Writer = null;
			}
		}

		/*
		/// <summary>Returns true if this IDocument was copied to the <paramref name="destination" />.</summary>
		/// <param name="destination"></param>
		/// <param name="onEachProgress"></param>
		/// <param name="progress"></param>
		/// <param name="onComplete"></param>
		/// <returns></returns>
		[Pure]
		public async PooledValueTask<(WebClient? downloader, Boolean? exists)> Copy(
			[NotNull] IDocument destination,
			[NotNull] Action<(IDocument, UInt64 bytesReceived, UInt64 totalBytesToReceive)> onEachProgress,
			[NotNull] Action<DownloadProgressChangedEventArgs>? progress ,
			CancellationToken onComplete  ) {
			if ( destination is null ) {
				throw new ArgumentNullException( nameof( destination ) );
			}

			if ( !this.Exists() ) {
				return ( default, default );
			}

			if ( destination.Exists() ) {
				destination.Delete();

				if ( destination.Exists() ) {
					return ( default, default );
				}
			}

			if ( !this.Length.HasValue || !this.Length.Any() ) {
#if NET48
				using var stream = File.Create( destination.FullPath, 1, FileOptions.None );
#else
				await using var stream = File.Create( destination.FullPath, 1, FileOptions.None );
#endif

				return ( default, true ); //just create an empty file?
			}

			var webClient = new WebClient {
				DownloadProgressChanged += ( sender, args ) => progress?.Invoke( args ),
				DownloadFileCompleted += ( sender, args ) => onComplete?.Invoke( args, ( this, destination ) )
			};

			await webClient.DownloadFileTaskAsync( this.FullPath, destination.FullPath ).ConfigureAwait( false );

			return ( webClient, destination.Exists() && destination.Size() == this.Size() );
		}
		*/

		/// <summary>
		///     Pull a new <see cref="FileInfo" /> for the <paramref name="document" />.
		/// </summary>
		/// <param name="document"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		[NotNull]
		[Pure]
		public static implicit operator FileInfo( [NotNull] Document document ) {
			if ( document is null ) {
				throw new ArgumentNullException( nameof( document ) );
			}

			var info = new FileInfo( document.FullPath );

			if ( info is null ) {
				throw new FileNotFoundException( $"Unable to find file {document.DoubleQuote()}." );
			}

			return info;
		}

		[NotNull]
		[Pure]
		public FileInfo ToFileInfo() => this;

		/// <summary>
		///     <para>If the file does not exist, return <see cref="Status.Error" />.</para>
		///     <para>If an exception happens, return <see cref="Status.Exception" />.</para>
		///     <para>Otherwise, return <see cref="Status.Success" />.</para>
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Status SetReadOnly( Boolean value ) {
			var info = this.GetFreshInfo();

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

		[Pure]
		public Status TurnOnReadonly() => this.SetReadOnly( true );

		[Pure]
		public Status TurnOffReadonly() => this.SetReadOnly( false );

		public virtual void GetObjectData( [NotNull] SerializationInfo info, StreamingContext context ) =>
			info.AddValue( nameof( this.FullPath ), this.FullPath, typeof( String ) );


		/// <summary>this seems to work great!</summary>
		/// <param name="address"></param>
		/// <param name="fileName"></param>
		/// <param name="progress"></param>
		/// <returns></returns>
		[Pure]
		public static async PooledValueTask<WebClient> DownloadFileTaskAsync( [NotNull] Uri address, [NotNull] String fileName,
			[CanBeNull] IProgress<(Int64 BytesReceived, Int32 ProgressPercentage, Int64 TotalBytesToReceive)>? progress ) {
			if ( address is null ) {
				throw new ArgumentNullException( nameof( address ) );
			}

			if ( String.IsNullOrWhiteSpace( fileName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fileName ) );
			}

			var tcs = new TaskCompletionSource<Object?>( address, TaskCreationOptions.RunContinuationsAsynchronously );

			void CompletedHandler( Object cs, AsyncCompletedEventArgs ce ) {
				if ( ce.UserState != tcs ) {
					return;
				}

				if ( ce.Error != null ) {
					tcs.TrySetException( ce.Error );
				}
				else if ( ce.Cancelled ) {
					tcs.TrySetCanceled();
				}
				else {
					tcs.TrySetResult( null );
				}
			}

			void ProgressChangedHandler( Object? ps, DownloadProgressChangedEventArgs? pe ) {
				if ( pe?.UserState == tcs ) {
					progress?.Report( (pe.BytesReceived, pe.ProgressPercentage, pe.TotalBytesToReceive) );
				}
			}

			var webClient = new WebClient();

			try {
				webClient.DownloadFileCompleted += CompletedHandler;
				webClient.DownloadProgressChanged += ProgressChangedHandler;
				webClient.DownloadFileAsync( address, fileName, tcs );

				await tcs.Task.ConfigureAwait( false );
			}
			finally {
				webClient.DownloadFileCompleted -= CompletedHandler;
				webClient.DownloadProgressChanged -= ProgressChangedHandler;
			}

			return webClient;
		}

		[NotNull]
		[Pure]
		public static IDocument GetTempDocument( [CanBeNull] String? extension = null ) {
			if ( String.IsNullOrEmpty( extension ) ) {
				extension = Guid.NewGuid().ToString();
			}

			extension = extension.TrimLeading( ".", StringComparison.OrdinalIgnoreCase );

			return new Document( Folder.GetTempFolder(), $"{Guid.NewGuid()}.{extension}" );
		}

		/// <summary>
		///     <para>Compares the file names (case sensitive) and file sizes for inequality.</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		[Pure]
		public static Boolean operator !=( [CanBeNull] Document? left, [CanBeNull] IDocument? right ) => !Equals( left, right );

		/// <summary>
		///     <para>Compares the file names (case sensitive) and file sizes for equality.</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		[Pure]
		public static Boolean operator ==( [CanBeNull] Document? left, [CanBeNull] IDocument? right ) => Equals( left, right );

		/// <summary>
		///     <para>Static case sensitive comparison of the file names and file sizes for equality.</para>
		///     <para>To compare the contents of two <see cref="Document" /> use <see cref="IDocument.SameContent" />.</para>
		///     <para>
		///         To quickly compare the contents of two <see cref="Document" /> use <see cref="CRC32" /> or <see cref="CRC64" />.
		///     </para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		[Pure]
		public static Boolean Equals( [CanBeNull] IDocument? left, [CanBeNull] IDocument? right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return false;
			}

			return left.FullPath.Is( right.FullPath ); //&& left.Size() == right.Size();
		}

		public async IAsyncEnumerable<String> ReadLines() {

			if ( !this.Exists() ) {
				yield break;
			}

			await using var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 32768, FileOptions.SequentialScan | FileOptions.Asynchronous );
			using var reader = new StreamReader( stream );

			while ( true ) {
				var line = await reader.ReadLineAsync().ConfigureAwait( false );

				if ( line is null ) {
					break;
				}

				yield return line;
			}
		}
	}
}