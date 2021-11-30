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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Document.cs" last touched on 2021-10-13 at 4:25 PM by Protiguous.

#nullable enable

namespace Librainian.FileSystem;

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
using Exceptions;
using Exceptions.Warnings;
using Internet;
using JetBrains.Annotations;
using Logging;
using Maths;
using Maths.Numbers;
using Measurement.Time;
using Newtonsoft.Json;
using Parsing;
using PooledAwait;
using Security;
using Threading;
using Utilities.Disposables;

[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
public class Document : ABetterClassDispose, IDocument {

	private Folder? _containingFolder;

	protected Document( SerializationInfo info ) : base( nameof( Document ) ) {
		if ( info is null ) {
			throw new NullException( nameof( info ) );
		}

		this.FullPath = ( info.GetString( nameof( this.FullPath ) ) ?? throw new InvalidOperationException() ).TrimAndThrowIfBlank();
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
	public Document( String fullPath, Boolean deleteAfterClose = false, Boolean watchFile = false ) : base( nameof( Document ) ) {
		if ( String.IsNullOrWhiteSpace( fullPath ) ) {
			throw new NullException( nameof( fullPath ) );
		}

		this.FullPath = Path.Combine( Path.GetFullPath( fullPath ), Path.GetFileName( fullPath ) ).TrimAndThrowIfBlank();

		//TODO What about just keeping the full path along with the long path UNC prefix?

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
				IncludeSubdirectories = false, EnableRaisingEvents = true
			} );

			this.WatchEvents = new Lazy<FileWatchingEvents>( () => new FileWatchingEvents(), false );

			var watcher = this.Watcher.Value;

			watcher.Created += ( _, e ) => this.WatchEvents.Value.OnCreated?.Invoke( e );
			watcher.Changed += ( _, e ) => this.WatchEvents.Value.OnChanged?.Invoke( e );
			watcher.Deleted += ( _, e ) => this.WatchEvents.Value.OnDeleted?.Invoke( e );
			watcher.Renamed += ( _, e ) => this.WatchEvents.Value.OnRenamed?.Invoke( e );
			watcher.Error += ( _, e ) => this.WatchEvents.Value.OnError?.Invoke( e );
		}
	}

	private Document() : base( nameof( Document ) ) => throw new NotAllowedWarning( "Private contructor is not allowed." );

	public Document( String justPath, String filename, Boolean deleteAfterClose = false ) : this( Path.Combine( justPath, filename ), deleteAfterClose ) { }

	public Document( FileSystemInfo info, Boolean deleteAfterClose = false ) : this( info.FullName, deleteAfterClose ) { }

	public Document( IFolder folder, String filename, Boolean deleteAfterClose = false ) : this( folder.FullPath, filename, deleteAfterClose ) { }

	public Document( IFolder folder, IDocument document, Boolean deleteAfterClose = false ) : this( Path.Combine( folder.FullPath, document.FileName ), deleteAfterClose ) { }

	private Lazy<FileSystemWatcher>? Watcher { get; }

	private Lazy<FileWatchingEvents>? WatchEvents { get; }

	public static String InvalidFileNameCharacters { get; } = new(Path.GetInvalidFileNameChars());

	public static Lazy<Regex> RegexForInvalidFileNameCharacters { get; } = new(() =>
		new Regex( $"[{Regex.Escape( InvalidFileNameCharacters )}]", RegexOptions.Compiled | RegexOptions.Singleline ));

	private ThreadLocal<JsonSerializer> JsonSerializers { get; } = new(() => new JsonSerializer {
		ReferenceLoopHandling = ReferenceLoopHandling.Serialize, PreserveReferencesHandling = PreserveReferencesHandling.All
	});

	/// <summary>
	///     Get or sets the <see cref="TimeSpan" /> used when getting a fresh <see cref="CancellationToken" /> via
	///     <see cref="GetCancelToken" />.
	/// </summary>
	public static TimeSpan DefaultDocumentTimeout { get; set; } = Seconds.Thirty;

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
			throw new NullException( nameof( destination ) );
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

	public CancellationTokenSource? CancellationTokenSource { get; set; }

	public Byte[]? Buffer { get; set; }

	public Boolean IsBufferLoaded {
		get;

		[Pure]
		private set;
	}

	public FileStream? Writer { get; set; }

	public StreamWriter? WriterStream { get; set; }

	/// <summary>
	///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
	///     <para>To compare the contents of two <see cref="IDocument" /> use <see cref="IDocument.SameContent" />.</para>
	/// </summary>
	/// <param name="other"></param>
	[Pure]
	public Boolean Equals( IDocument? other ) => Equals( this, other );

	/// <summary>
	///     Represents the fully qualified path of the file.
	///     <para>Fully qualified "Drive:\Path\Folder\Filename.Ext"</para>
	/// </summary>
	[JsonProperty]
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
	public async PooledValueTask<UInt64?> Length( CancellationToken cancellationToken ) {
		var info = await this.GetFreshInfo( cancellationToken ).ConfigureAwait( false );

		return info.Exists ? ( UInt64? )info.Length : default( UInt64? );
	}

	/// <summary>Anything that can be temp stored can go in this. Not serialized. Defaults to be used for internal locking.</summary>
	[JsonIgnore]
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
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	[Pure]
	public async IAsyncEnumerable<Byte> AsBytes( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffer = new Byte[ sizeof( Byte ) ];
		var length = buffer.Length;

		var buffered = new BufferedStream( stream, optimal.Value );
		await using var __ = buffered.ConfigureAwait( false );

		while ( ( await buffered.ReadAsync( buffer.AsMemory( 0, length ), cancellationToken ).ConfigureAwait( false ) ).Any() ) {
			yield return buffer[ 0 ];
		}
	}

	/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.</summary>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	[Pure]
	public async IAsyncEnumerable<Int32> AsInt32( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffer = new Byte[ sizeof( Int32 ) ];
		var length = buffer.Length;

		var buffered = new BufferedStream( stream, optimal.Value );
		await using var __ = buffered.ConfigureAwait( false );

		while ( ( await buffered.ReadAsync( buffer.AsMemory( 0, length ), cancellationToken ).ConfigureAwait( false ) ).Any() ) {
			yield return BitConverter.ToInt32( buffer, 0 );
		}
	}

	/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	[Pure]
	public async IAsyncEnumerable<Int64> AsInt64( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffer = new Byte[ sizeof( Int64 ) ];
		var length = buffer.Length;

		var buffered = new BufferedStream( stream, optimal.Value );
		await using var __ = buffered.ConfigureAwait( false );

		while ( ( await buffered.ReadAsync( buffer.AsMemory( 0, length ), cancellationToken ).ConfigureAwait( false ) ).Any() ) {
			yield return BitConverter.ToInt64( buffer, 0 );
		}
	}

	/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Guid" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	public async IAsyncEnumerable<Guid> AsGuids( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffer = new Byte[ sizeof( Decimal ) ];
		var length = buffer.Length;

		var buffered = new BufferedStream( stream, optimal.Value );
		await using var __ = buffered.ConfigureAwait( false );

		while ( ( await buffered.ReadAsync( buffer.AsMemory( 0, length ), cancellationToken ).ConfigureAwait( false ) ).Any() ) {
			yield return new Guid( buffer );
		}
	}

	/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="UInt64" />.</summary>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	[Pure]
	public async IAsyncEnumerable<UInt64> AsUInt64( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffer = new Byte[ sizeof( UInt64 ) ];
		var length = buffer.Length;

		var buffered = new BufferedStream( stream, sizeof( UInt64 ) );
		await using var __ = buffered.ConfigureAwait( false );

		while ( ( await buffered.ReadAsync( buffer.AsMemory( 0, length ), cancellationToken ).ConfigureAwait( false ) ).Any() ) {
			yield return BitConverter.ToUInt64( buffer, 0 );
		}
	}

	/// <summary>HarkerHash (hash-by-addition)</summary>
	[Pure]
	public async PooledValueTask<Int32> HarkerHash32( CancellationToken cancellationToken ) =>
		await this.AsInt32( cancellationToken ).Select( i => i == 0 ? 1 : i ).SumAsync( cancellationToken ).ConfigureAwait( false );

	/// <summary>Deletes the file.</summary>
	public async PooledValueTask Delete( CancellationToken cancellationToken ) {
		var fileInfo = await this.GetFreshInfo( cancellationToken ).ConfigureAwait( false );

		if ( fileInfo.Exists ) {
			if ( fileInfo.IsReadOnly ) {
				fileInfo.IsReadOnly = false;
			}

			fileInfo.Delete();
		}
	}

	/// <summary>Returns whether the file exists.</summary>
	[DebuggerStepThrough]
	[Pure]
	public async PooledValueTask<Boolean> Exists( CancellationToken cancellationToken ) {
		var info = await this.GetFreshInfo( cancellationToken ).ConfigureAwait( false );
		return info.Exists;
	}

	/// <summary>
	///     <para>Clone the entire IDocument to the <paramref name="destination" /> as quickly as possible.</para>
	///     <para>this will OVERWRITE any <see cref="destination" /> file.</para>
	/// </summary>
	/// <param name="destination"></param>
	/// <param name="progress">   </param>
	/// <param name="eta">        </param>
	/// <param name="cancellationToken"></param>
	[Pure]
	public async PooledValueTask<(Status success, TimeSpan timeElapsed)> CloneDocument(
		IDocument destination,
		IProgress<Single> progress,
		IProgress<TimeSpan> eta,
		CancellationToken cancellationToken
	) {
		if ( destination is null ) {
			throw new NullException( nameof( destination ) );
		}

		var stopwatch = Stopwatch.StartNew();

		try {
			if ( ( await this.Length( cancellationToken ).ConfigureAwait( false ) ).Any() ) {
				if ( Uri.TryCreate( this.FullPath, UriKind.Absolute, out var sourceAddress ) ) {
					//BUG Obsolete
					using var client = new WebClient().Add( cancellationToken );

					await client.DownloadFileTaskAsync( sourceAddress, destination.FullPath ).ConfigureAwait( false );

					return ( Status.Success, stopwatch.Elapsed );
				}
			}
		}
		catch ( WebException exception ) {
			exception.Log();
			return ( Status.Exception, stopwatch.Elapsed );
		}

		return ( Status.Failure, stopwatch.Elapsed );
	}

	[Pure]
	public async PooledValueTask<Int32?> CRC32( CancellationToken cancellationToken ) {
		try {
			var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
			if ( optimal is null ) {
				return null;
			}

			using var crc32 = new CRC32( ( UInt32 )optimal, ( UInt32 )optimal );

			await using var fileStream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );

			await using var buffered = new BufferedStream( fileStream, optimal.Value );

			var hash = await crc32.ComputeHashAsync( buffered, cancellationToken ).ConfigureAwait( false );

			return BitConverter.ToInt32( hash, 0 );
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

	/// <exception cref="InvalidOperationException"></exception>
	[Pure]
	public async PooledValueTask<String?> CRC32Hex( CancellationToken cancellationToken ) {
		try {
			var size = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );

			//var size = await this.Size().ConfigureAwait( false );

			switch ( size ) {
				case null:
				case not > 0: {
					return null;
				}
				case > Int32.MaxValue / 2: {
					throw new InvalidOperationException( "File too large to convert to hex." );
				}
			}

			using var crc32 = new CRC32( ( UInt32 )size.Value, ( UInt32 )size.Value );

			//TODO Would BufferedStream be any faster here?
			var fileStream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, size.Value, FileOptions.SequentialScan );
			await using var _ = fileStream.ConfigureAwait( false );

			var buffered = new BufferedStream( fileStream, size.Value );
			await using var __ = buffered.ConfigureAwait( false );

			var hash = await crc32.ComputeHashAsync( buffered, cancellationToken ).ConfigureAwait( false );

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
			exception.Break( );
		}

		return null;
	}

	[Pure]
	public async PooledValueTask<Int64?> CRC64( CancellationToken cancellationToken ) {
		try {
			var size = await this.Size( cancellationToken ).ConfigureAwait( false );

			if ( size?.Any() is true ) {
				var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );

				using var crc64 = new CRC64( size.Value, size.Value );

				//TODO Would BufferedStream be any faster here?
				var fileStream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
				await using var _ = fileStream.ConfigureAwait( false );
				var buffered = new BufferedStream( fileStream, optimal.Value );
				await using var __ = buffered.ConfigureAwait( false );

				var hash = await crc64.ComputeHashAsync( buffered, cancellationToken ).ConfigureAwait( false );

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
	[Pure]
	public async PooledValueTask<String?> CRC64Hex( CancellationToken cancellationToken ) {
		try {
			var size = await this.Size( cancellationToken ).ConfigureAwait( false );

			if ( size?.Any() is true ) {
				var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
				using var crc64 = new CRC64( size.Value, size.Value );

				var fileStream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
				await using var _ = fileStream.ConfigureAwait( false );
				var buffered = new BufferedStream( fileStream, optimal.Value );
				await using var __ = buffered.ConfigureAwait( false );

				var hash = await crc64.ComputeHashAsync( buffered, cancellationToken ).ConfigureAwait( false );

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
	public async PooledValueTask<Boolean> IsAll( Byte number, CancellationToken cancellationToken ) {
		if ( !this.IsBufferLoaded ) {
			var result = await this.LoadDocumentIntoBuffer( cancellationToken ).ConfigureAwait( false );

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
	[Pure]
	public async PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> DownloadFile( Uri source ) {
		if ( source is null ) {
			throw new NullException( nameof( source ) );
		}

		//TODO possibly download entire file, delete original version, then rename the newly downloaded file?

		try {
			if ( !source.IsWellFormedOriginalString() ) {
				return ( new Exception( $"Could not use source Uri '{source}'." ), null );
			}

			using var webClient = new WebClient(); //from what I've read, Dispose should NOT be being called on a WebClient???

			await webClient.DownloadFileTaskAsync( source, this.FullPath ).ConfigureAwait( false );

			return ( null, webClient.ResponseHeaders );
		}
		catch ( Exception exception ) {
			return ( exception, null );
		}
	}

	/// <summary>
	///     <para>Computes the extension of the <see cref="FileName" />, including the prefix ".".</para>
	/// </summary>
	[Pure]
	public String Extension() => Path.GetExtension( this.FullPath ).Trim().NullIfEmptyOrWhiteSpace() ?? String.Empty;

	/// <summary>
	///     <para>Just the file's name, including the extension (no path).</para>
	/// </summary>
	/// <example>
	///     <code>new Document("C:\Temp\Test.text").FileName() == "Test.text"</code>
	/// </example>
	/// <see cref="Path.GetFileName" />
	public String FileName => Path.GetFileName( this.FullPath );

	/// <summary>Returns the size of the file, if it exists.</summary>
	[Pure]
	public PooledValueTask<UInt64?> Size( CancellationToken cancellationToken ) => this.Length( cancellationToken );

	/// <summary>
	///     <para>If the file does not exist, it is created.</para>
	///     <para>Then the <paramref name="text" /> is appended to the file.</para>
	/// </summary>
	/// <param name="text"></param>
	/// <param name="cancellationToken"></param>
	public async PooledValueTask<IDocument> AppendText( String text, CancellationToken cancellationToken ) {
		var folder = this.ContainingingFolder();

		if ( !await folder.Exists( cancellationToken ).ConfigureAwait( false ) ) {
			if ( !Directory.CreateDirectory( folder.FullPath ).Exists ) {
				throw new DirectoryNotFoundException( $"Could not create folder \"{folder.FullPath}\"." );
			}
		}

		await this.SetReadOnly( false, cancellationToken ).ConfigureAwait( false );

		await File.AppendAllTextAsync( this.FullPath, text, cancellationToken ).ConfigureAwait( false );

		return this;
	}

	public IAsyncEnumerator<Byte> GetAsyncEnumerator( CancellationToken cancellationToken ) => this.AsBytes( cancellationToken ).GetAsyncEnumerator( cancellationToken );

	/// <summary>
	///     <para>To compare the contents of two <see cref="IDocument" /> use SameContent( IDocument,IDocument).</para>
	/// </summary>
	/// <param name="other"></param>
	[Pure]
	public override Boolean Equals( Object? other ) => other is IDocument document && Equals( this, document );

	/// <summary>(file name, not contents)</summary>
	[Pure]
	public override Int32 GetHashCode() => this.FullPath.GetHashCode();

	/// <summary>Returns the filename, without the extension.</summary>
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
	public async PooledValueTask<Int32?> GetOptimalBufferSize( CancellationToken cancellationToken ) {
		var size = await this.Size( cancellationToken ).ConfigureAwait( false );

		//A null size means the file was not found.

		return size switch {
			null => null,
			>= IDocument.MaximumBufferSize => IDocument.MaximumBufferSize,
			var _ => ( Int32 )size
		};
	}

	/// <summary>Attempt to start the process.</summary>
	/// <param name="arguments"></param>
	/// <param name="verb">     "runas" is elevated</param>
	/// <param name="useShell"></param>
	[Pure]
	public PooledValueTask<Process?> Launch( String? arguments = null, String? verb = "runas", Boolean useShell = false ) {
		try {
			var info = new ProcessStartInfo( this.FullPath ) {
				Arguments = arguments ?? String.Empty, UseShellExecute = useShell, Verb = verb ?? String.Empty
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
	/// <see cref="Path.GetFileNameWithoutExtension(System.ReadOnlySpan{Char})" />
	public String Name => this.FileName;

	[Pure]
	public async PooledValueTask<String> ReadStringAsync() {
		using var reader = new StreamReader( this.FullPath );

		return await reader.ReadToEndAsync().ConfigureAwait( false );
	}

	/// <summary>
	///     <para>Performs a byte by byte file comparison, but ignores the <see cref="IDocument" /> file names.</para>
	/// </summary>
	/// <param name="right"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NullException"></exception>
	/// <exception cref="SecurityException"></exception>
	/// <exception cref="NullException"></exception>
	/// <exception cref="UnauthorizedAccessException"></exception>
	/// <exception cref="PathTooLongException"></exception>
	/// <exception cref="NotSupportedException"></exception>
	/// <exception cref="IOException"></exception>
	/// <exception cref="DirectoryNotFoundException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	[Pure]
	public async PooledValueTask<Boolean> SameContent( Document? right, CancellationToken cancellationToken ) {
		if ( right is null ) {
			return false;
		}

		if ( !await this.Exists( cancellationToken ).ConfigureAwait( false ) || !await right.Exists( cancellationToken ).ConfigureAwait( false ) ) {
			return false;
		}

		if ( await this.Size( cancellationToken ).ConfigureAwait( false ) != await right.Size( cancellationToken ).ConfigureAwait( false ) ) {
			return false;
		}

		var lefts = this.AsDecimal( cancellationToken ); //Could use AsBytes.. any performance difference?
		var rights = right.AsDecimal( cancellationToken ); //Could use AsGuids also?

		return await lefts.SequenceEqualAsync( rights, cancellationToken ).ConfigureAwait( false );
	}

	/// <summary>Open the file for reading and return a <see cref="StreamReader" />.</summary>
	[Pure]
	public StreamReader StreamReader() => new(File.OpenRead( this.FullPath ));

	/// <summary>
	///     Open the file for writing and return a <see cref="StreamWriter" />.
	///     <para>Optional <paramref name="encoding" />. Defaults to <see cref="Encoding.Unicode" />.</para>
	///     <para>Optional buffersize. Defaults to 1 MB.</para>
	/// </summary>
	public async Task<StreamWriter?> StreamWriter( CancellationToken cancellationToken, Encoding? encoding = null, UInt32 bufferSize = MathConstants.Sizes.OneMegaByte ) {
		try {
			this.ReleaseWriterStream();

			await this.OpenWriter( false, cancellationToken ).ConfigureAwait( false );

			if ( this.Writer is null ) {
				return default( StreamWriter? );
			}

			return this.WriterStream = new StreamWriter( this.Writer, encoding ?? Encoding.Unicode, ( Int32 )bufferSize, false );
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		this.ReleaseWriterStream();

		return default( StreamWriter? );
	}

	/// <summary>Return this <see cref="IDocument" /> as a JSON string.</summary>
	[Pure]
	public async PooledValueTask<String?> ToJSON() {
		using var reader = new StreamReader( this.FullPath );

		var readToEndAsync = await reader.ReadToEndAsync().ConfigureAwait( false );

		return readToEndAsync;
	}

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	[Pure]
	public override String ToString() => this.FullPath;

	/// <summary>
	///     <para>Returns true if this <see cref="Document" /> no longer seems to exist.</para>
	/// </summary>
	/// <param name="delayBetweenRetries"></param>
	/// <param name="cancellationToken"></param>
	public async PooledValueTask<Boolean?> TryDeleting( TimeSpan delayBetweenRetries, CancellationToken cancellationToken ) {
		while ( !cancellationToken.IsCancellationRequested && await this.Exists( cancellationToken ).ConfigureAwait( false ) ) {
			try {
				if ( await this.Exists( cancellationToken ).ConfigureAwait( false ) ) {
					await this.Delete( cancellationToken ).ConfigureAwait( false );
				}
			}
			catch ( DirectoryNotFoundException ) { }
			catch ( PathTooLongException ) { }
			catch ( IOException ) {
				// IOException is thrown when the file is in use by any process.
				await Task.Delay( delayBetweenRetries, cancellationToken ).ConfigureAwait( false );
			}
			catch ( UnauthorizedAccessException ) { }
			catch ( NullException ) { }
		}

		return await this.Exists( cancellationToken ).ConfigureAwait( false );
	}

	/// <summary>Uploads this <see cref="IDocument" /> to the given <paramref name="destination" />.</summary>
	/// <param name="destination"></param>
	[Pure]
	public async PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> UploadFile( Uri destination ) {
		if ( destination is null ) {
			throw new NullException( nameof( destination ) );
		}

		if ( !destination.IsWellFormedOriginalString() ) {
			return ( new InvalidOperationException( $"Destination address '{destination.OriginalString}' is not well formed." ), null );
		}

		try {
			using var webClient = new WebClient();

			await webClient.UploadFileTaskAsync( destination, this.FullPath ).ConfigureAwait( false );

			return ( null, webClient.ResponseHeaders );
		}
		catch ( Exception exception ) {
			return ( exception, null );
		}
	}

	/// <summary>Create and returns a new <see cref="FileInfo" /> object for <see cref="FullPath" />.</summary>
	/// <see cref="op_Implicit" />
	/// <see cref="ToFileInfo" />
	[Pure]
	public PooledValueTask<FileInfo> GetFreshInfo( CancellationToken cancellationToken ) => ToFileInfo( this );

	/// <summary>Attempt to return an object Deserialized from this JSON text file.</summary>
	/// <param name="progress"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="T"></typeparam>
	public async PooledValueTask<(Status status, T? obj)> LoadJSON<T>( IProgress<ZeroToOne>? progress, CancellationToken cancellationToken ) {
		var i = 0.0;
		const Double maxsteps = 6.0;

		try {
			progress?.Report( ++i / maxsteps );

			if ( !await this.Exists( cancellationToken ).ConfigureAwait( false ) ) {
				ZeroToOne bob;
				progress?.Report( new ZeroToOne( ZeroToOne.MaximumValue ) );

				return ( Status.Bad, default( T? ) );
			}

			progress?.Report( ++i / maxsteps );

			using var textReader = File.OpenText( this.FullPath );
			progress?.Report( ++i / maxsteps );

			var jsonReader = new JsonTextReader( textReader );
			progress?.Report( ++i / maxsteps );

			try {
				var run = await Task.Run( () => this.JsonSerializers.Value!.Deserialize<T>( jsonReader ), cancellationToken ).ConfigureAwait( false );

				progress?.Report( ++i / maxsteps );

				return ( Status.Success, run );
			}
			finally {
				progress?.Report( ++i / maxsteps );
			}
		}
		catch ( TaskCanceledException ) {
			progress?.Report( new ZeroToOne( ZeroToOne.MaximumValue ) );
		}
		catch ( Exception exception ) {
			progress?.Report( new ZeroToOne( ZeroToOne.MaximumValue ) );
			exception.Log();
		}

		return ( Status.Exception, default( T? ) );
	}

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
	public async Task<FileCopyData> Copy( FileCopyData fileCopyData, CancellationToken cancellationToken ) {
		if ( !Uri.TryCreate( fileCopyData.Source.FullPath, UriKind.RelativeOrAbsolute, out var uri ) ) {
			throw new UriFormatException( $"Unable to parse {this.FullPath.DoubleQuote()} into a Uri." );
		}

		try {
			if ( await DoesSourceFileExist().ConfigureAwait( false ) is false ) {
				return fileCopyData with {
					Status = Status.Bad
				};
			}

			( var exists, var size ) = await CheckSourceSize().ConfigureAwait( false );

			if ( exists is false || size is null ) {
				return fileCopyData with {
					Status = Status.Failure
				};
			}

			fileCopyData.SourceSize = size.Value;

			var bytes = size.Value;
			Single bits = bytes * 8;
			const Int32 bitsPerSecond = 11 * MathConstants.Sizes.OneMegaByte;
			var guessSeconds = bits / bitsPerSecond;

			var estimatedTimeToCopy = TimeSpan.FromSeconds( guessSeconds * 2 );

			//TODO Add in capability to pause/resume?
			await DownloadAndVerifySize().WithTimeout( estimatedTimeToCopy, fileCopyData.CancellationTokenSource.Token ).ConfigureAwait( false );
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

		void RecordException( Exception? exception ) {
			if ( exception is null ) {
				return;
			}

			fileCopyData.Exceptions ??= new List<Exception>();
			fileCopyData.Exceptions.Add( exception );
		}

		async PooledValueTask<Boolean> DoesSourceFileExist() {
			$"Checking for existance of source file {fileCopyData.Source.FullPath.DoubleQuote()}".Verbose();
			var exists = await fileCopyData.Source.Exists( cancellationToken ).ConfigureAwait( false );
			if ( exists is not true ) {
				RecordException( new FileNotFoundException( "Missing file.", fileCopyData.Source.FullPath ) );

				return false;
			}

			return true;
		}

		async PooledValueTask<(Boolean exists, UInt64? size)> CheckSourceSize() {
			$"Checking for size of source file {fileCopyData.Source.FullPath.DoubleQuote()}".Verbose();
			var size = await fileCopyData.Source.Size( cancellationToken ).ConfigureAwait( false );

			if ( size is > 0 ) {
				return ( true, size );
			}

			RecordException( new FileNotFoundException( "Empty file.", fileCopyData.Source.FullPath ) );

			return ( false, default( UInt64? ) );
		}

		async Task<Boolean> DownloadAndVerifySize() {
			using var webClient = new WebClient();
			$"{nameof( webClient )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()} instantiated.".Verbose();

			webClient.Disposed += ( _, _ ) => $"{nameof( webClient )} {nameof( webClient.Disposed )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()}."
				.Verbose();

			webClient.DownloadFileCompleted += ( _, args ) => {
				fileCopyData.WhenCompleted = DateTime.UtcNow;

				RecordException( args.Error );

				if ( args.Error is null ) {
					fileCopyData.OnCompleted?.Invoke( fileCopyData );
				}
			};

			webClient.DownloadProgressChanged += ( _, args ) => fileCopyData.DataCopied?.Report( fileCopyData with {
				BytesCopied = ( UInt64 )args.BytesReceived
			} );

			$"{nameof( webClient )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()} started.".Verbose();
			await webClient.DownloadFileTaskAsync( uri, fileCopyData.Destination.FullPath ).ConfigureAwait( false );

			$"Checking existance of destination file {fileCopyData.Destination.FullPath.DoubleQuote()}.".Verbose();
			if ( await fileCopyData.Destination.Exists( cancellationToken ).ConfigureAwait( false ) is false ) {
				$"Could not find destination file {fileCopyData.Destination.FullPath.DoubleQuote()}.".Verbose();

				return false;
			}

			$"Checking size of destination file {fileCopyData.Destination.FullPath.DoubleQuote()}.".Verbose();
			var destinationSize = await fileCopyData.Destination.Size( cancellationToken ).ConfigureAwait( false );

			if ( destinationSize is null ) {
				RecordException( new UnknownWarning(
					$"Unknown error occurred copying file {fileCopyData.Source.FullPath.DoubleQuote()} to {fileCopyData.Destination.ContainingingFolder().FullPath}." ) );

				return false;
			}

			if ( destinationSize != fileCopyData.SourceSize ) {
				RecordException( new UnknownWarning(
					$"Unknown error occurred copying file {fileCopyData.Source.FullPath.DoubleQuote()} to {fileCopyData.Destination.ContainingingFolder().FullPath}." ) );

				return false;
			}

			$"{nameof( webClient )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()} done.".Verbose();

			return true;
		}
	}

	[Pure]
	public async PooledValueTask<Int64> HarkerHash64( CancellationToken cancellationToken ) =>
		await this.AsInt64( cancellationToken ).Select( i => i == 0L ? 1L : i ).SumAsync( cancellationToken ).ConfigureAwait( false );

	/// <summary>"poor mans Decimal hash"</summary>
	[Pure]
	public async PooledValueTask<Decimal> HarkerHashDecimal( CancellationToken cancellationToken ) =>
		await this.AsDecimal( cancellationToken ).Select( i => i == 0M ? 1M : i ).SumAsync( cancellationToken ).ConfigureAwait( false );

	/// <summary>Returns an enumerator that iterates through the collection.</summary>
	/// <returns>A <see cref="IEnumerator" /> that can be used to iterate through the collection.</returns>
	[Pure]
	public IAsyncEnumerator<Byte> GetEnumerator() => this.AsBytes( CancellationToken.None ).GetAsyncEnumerator();

	/// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
	public override void DisposeManaged() {
		this.Buffer = default( Byte[]? );

		this.ReleaseWriterStream();

		this.ReleaseWriter();

		if ( this.DeleteAfterClose ) {
			var _ = this.Delete( this.GetCancelToken() );
		}
	}

	/// <summary>Attempt to load the entire file into memory. If it throws, it throws..</summary>
	public async PooledValueTask<Status> LoadDocumentIntoBuffer( CancellationToken cancellationToken ) {
		var size = await this.Size( cancellationToken ).ConfigureAwait( false );

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

		var filelength = ( Int32 )size.Value; //will we EVER have an image (or whatever) larger than Int32? (answer: yes)
		this.Buffer = new Byte[ filelength ];

		var offset = 0;

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, filelength, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			//throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}" );
			return Status.Exception;
		}

		var buffered = new BufferedStream( stream );
		await using var __ = buffered.ConfigureAwait( false );

		Int32 bytesRead;

		do {
			bytesRead = await buffered.ReadAsync( this.Buffer.AsMemory( offset, filelength ), cancellationToken ).ConfigureAwait( false );
			bytesLeft -= bytesRead.Positive();

			if ( !bytesRead.Any() || !bytesLeft.Any() ) {
				this.IsBufferLoaded = true;

				return Status.Success;
			}

			offset += bytesRead;
		} while ( bytesRead.Any() && bytesLeft.Any() );

		return Status.Failure;
	}

	/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	[Pure]
	public async IAsyncEnumerable<Decimal> AsDecimal( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var fileLength = await this.Length( cancellationToken ).ConfigureAwait( false );

		if ( !fileLength.HasValue ) {
			yield break;
		}

		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffered = new BufferedStream( stream, optimal.Value ); //TODO Is this buffering twice??
		await using var __ = buffered.ConfigureAwait( false );

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

	/// <summary>
	///     Opens an existing file or creates a new file for writing.
	///     <para>Should be able to read and write from <see cref="FileStream" />.</para>
	///     <para>If there is any error opening or creating the file, <see cref="Writer" /> will be null.</para>
	/// </summary>
	public async PooledValueTask<FileStream?> OpenWriter( Boolean deleteIfAlreadyExists, CancellationToken cancellationToken, FileShare sharingOptions = FileShare.None ) {
		try {
			this.ReleaseWriter();

			if ( deleteIfAlreadyExists ) {
				if ( await this.Exists( cancellationToken ).ConfigureAwait( false ) ) {
					await this.Delete( cancellationToken ).ConfigureAwait( false );
				}
			}

			return this.Writer = new FileStream( this.FullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, sharingOptions );
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		this.ReleaseWriter();

		return default( FileStream? );
	}

	/// <summary>
	///     Releases the <see cref="FileStream" /> opened by <see cref="OpenWriter" />.
	/// </summary>
	public void ReleaseWriter() {
		using ( this.Writer ) {
			this.Writer = default( FileStream? );
		}
	}

	/// <summary>
	///     <para>If the file does not exist, return <see cref="Status.Error" />.</para>
	///     <para>If an exception happens, return <see cref="Status.Exception" />.</para>
	///     <para>Otherwise, return <see cref="Status.Success" />.</para>
	/// </summary>
	/// <param name="value"></param>
	/// <param name="cancellationToken"></param>
	public async PooledValueTask<Status> SetReadOnly( Boolean value, CancellationToken cancellationToken ) {
		var info = await this.GetFreshInfo( cancellationToken ).ConfigureAwait( false );

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
	public PooledValueTask<Status> TurnOnReadonly( CancellationToken cancellationToken ) => this.SetReadOnly( true, cancellationToken );

	[Pure]
	public PooledValueTask<Status> TurnOffReadonly( CancellationToken cancellationToken ) => this.SetReadOnly( false, cancellationToken );

	public virtual void GetObjectData( SerializationInfo info, StreamingContext context ) => info.AddValue( nameof( this.FullPath ), this.FullPath, typeof( String ) );

	public async IAsyncEnumerable<String> ReadLines( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var size = await this.Size( cancellationToken ).ConfigureAwait( false );

		if ( !size.Any() ) {
			yield break;
		}

		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false ) ?? 4096;

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );
		var buffered = new BufferedStream( stream, optimal );
		await using var __ = buffered.ConfigureAwait( false );

		using var reader = new StreamReader( buffered );

		while ( !cancellationToken.IsCancellationRequested ) {
			var line = await reader.ReadLineAsync().ConfigureAwait( false );

			if ( line is null ) {
				break;
			}

			yield return line;
		}
	}

	/// <summary>
	///     Synchronous version.
	/// </summary>
	public UInt64? GetLength() {
		var info = new FileInfo( this.FullPath );
		info.Refresh();
		return info.Exists ? ( UInt64 )info.Length : null;
	}

	/// <summary>
	///     Synchronous version.
	/// </summary>
	public Boolean GetExists() {
		var info = new FileInfo( this.FullPath );
		info.Refresh();
		return info.Exists;
	}

	/// <summary>
	///     Synchronous version.
	/// </summary>
	public UInt64? GetSize() {
		var info = new FileInfo( this.FullPath );
		info.Refresh();
		return ( UInt64? )info.Length;
	}

	/*
	private async PooledValueTask ThrowIfNotExists() {
		if ( !await this.Exists( this.GetCancelToken() ).ConfigureAwait( false ) ) {
			throw new FileNotFoundException( $"Could find document {this.FullPath.SmartQuote()}." );
		}
	}
	*/

	private void ReleaseWriterStream() {
		using ( this.WriterStream ) {
			this.WriterStream = null;
		}
	}

	private CancellationToken GetCancelToken() {
		this.CancellationTokenSource ??= new CancellationTokenSource();
		return this.CancellationTokenSource.Token;
	}

	/// <summary>
	///     Pull a new <see cref="FileInfo" /> for the <paramref name="document" />.
	/// </summary>
	/// <param name="document"></param>
	/// <exception cref="NullException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	[Pure]
	public static implicit operator FileInfo( Document document ) => ToFileInfo( document ).AsValueTask().AsTask().Result;

	[Pure]
	public static async PooledValueTask<FileInfo> ToFileInfo( Document document ) {
		if ( document is null ) {
			throw new NullException( nameof( document ) );
		}

		return await Task.Run( () => {
			var info = new FileInfo( document.FullPath );

			info.Refresh();

			return info;
		} ).ConfigureAwait( false );
	}

	/// <summary>this seems to work great!</summary>
	/// <param name="address"></param>
	/// <param name="fileName"></param>
	/// <param name="progress"></param>
	[Pure]
	public static async PooledValueTask<WebClient> DownloadFileTaskAsync(
		Uri address,
		String fileName,
		IProgress<(Int64 BytesReceived, Int32 ProgressPercentage, Int64 TotalBytesToReceive)>? progress
	) {
		if ( address is null ) {
			throw new NullException( nameof( address ) );
		}

		if ( String.IsNullOrWhiteSpace( fileName ) ) {
			throw new NullException( nameof( fileName ) );
		}

		var tcs = new TaskCompletionSource<Object?>( address, TaskCreationOptions.RunContinuationsAsynchronously );

		void CompletedHandler( Object? cs, AsyncCompletedEventArgs ce ) {
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
				progress?.Report( ( pe.BytesReceived, pe.ProgressPercentage, pe.TotalBytesToReceive ) );
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

	/// <summary>
	/// Any leading periods will be trimmed off of <paramref name="extension"/>.
	/// <para>If <paramref name="extension"/> is not supplied, a random <see cref="Guid"/> will be used.</para>
	/// </summary>
	/// <param name="extension"></param>
	/// <returns></returns>
	[Pure]
	public static IDocument GetTempDocument( String? extension = null ) {
		extension = String.IsNullOrEmpty( extension ) ? Guid.NewGuid().ToString() : extension.TrimLeading( ".", StringComparison.OrdinalIgnoreCase );

		return new Document( GetTempFolder(), $"{Guid.NewGuid()}.{extension}" );
	}

	/// <summary>Throws Exception if unable to obtain the Temp path.</summary>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="PathTooLongException"></exception>
	/// <exception cref="DirectoryNotFoundException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	public static IFolder GetTempFolder() => Folder.GetTempFolder();


	/// <summary>
	///     <para>Compares the file names (case sensitive) and file sizes for inequality.</para>
	/// </summary>
	/// <param name="left"> </param>
	/// <param name="right"></param>
	[Pure]
	public static Boolean operator !=( Document? left, IDocument? right ) => !Equals( left, right );

	/// <summary>
	///     <para>Compares the file names (case sensitive) and file sizes for equality.</para>
	/// </summary>
	/// <param name="left"> </param>
	/// <param name="right"></param>
	[Pure]
	public static Boolean operator ==( Document? left, IDocument? right ) => Equals( left, right );

	/// <summary>
	///     <para>Static case sensitive comparison of the file names and file sizes for equality.</para>
	///     <para>To compare the contents of two <see cref="Document" /> use <see cref="IDocument.SameContent" />.</para>
	///     <para>
	///         To quickly compare the contents of two <see cref="Document" /> use <see cref="CRC32" /> or <see cref="CRC64" />
	///         .
	///     </para>
	/// </summary>
	/// <param name="left"> </param>
	/// <param name="right"></param>
	[Pure]
	public static Boolean Equals( IDocument? left, IDocument? right ) {
		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		if ( left is null || right is null ) {
			return false;
		}

		return left.FullPath.Is( right.FullPath ); //&& left.Size() == right.Size();
	}

}