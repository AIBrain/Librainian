// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Document.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "Document.cs" was last formatted by Protiguous on 2018/06/26 at 12:54 AM.

namespace Librainian.ComputerSystems.FileSystem {

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
		///     The <see cref="Document" /> class is built around <see cref="FileInfo" />.
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
					if ( this.Exists() ) {
						return this.Info.Length;
					}
				}
				catch ( FileNotFoundException exception ) {
					exception.More();
				}
				catch ( IOException exception ) {
					exception.More();
				}

				return default;
			}
		}

		// ReSharper disable once NotNullMemberIsNotInitialized
		private Document() => throw new NotImplementedException( "Private contructor is not allowed." );

		public Document( [NotNull] String fullPath, [NotNull] String filename, Boolean deleteAfterClose = false ) : this( fullPathWithFilename: Path.Combine( path1: fullPath, path2: filename ),
			deleteAfterClose: deleteAfterClose ) { }

		/// <summary>
		/// </summary>
		/// <param name="fullPathWithFilename"></param>
		/// <param name="deleteAfterClose">    </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="SecurityException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.UnauthorizedAccessException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="System.NotSupportedException"></exception>
		/// <exception cref="IOException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Document( [NotNull] String fullPathWithFilename, Boolean deleteAfterClose = false ) {
			if ( String.IsNullOrWhiteSpace( fullPathWithFilename ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fullPathWithFilename ) );
			}

			this.DeleteAfterClose = deleteAfterClose;

			this.Info = new FileInfo( fileName: fullPathWithFilename );
		}

		public Document( [NotNull] FileSystemInfo info, Boolean deleteAfterClose = false ) : this( fullPathWithFilename: info.FullName, deleteAfterClose: deleteAfterClose ) { }

		public Document( [NotNull] Folder folder, [NotNull] String filename, Boolean deleteAfterClose = false ) : this( fullPath: folder.FullName, filename: filename, deleteAfterClose: deleteAfterClose ) { }

		public Document( [NotNull] Folder folder, [NotNull] Document document, Boolean deleteAfterClose = false ) : this(
			fullPathWithFilename: Path.Combine( path1: folder.FullName, path2: document.FileName() ), deleteAfterClose: deleteAfterClose ) { }

		/// <summary>
		///     <para>Static case sensitive comparison of the file names and file sizes for equality.</para>
		///     <para>To compare the contents of two <see cref="Document" /> use <see cref="SameContent(Document)" />.</para>
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

			return left.Size() == right.Size() && left.FullPathWithFileName.Is( right: right.FullPathWithFileName );
		}

		/// <summary>
		///     Returns a unique file in the user's temp folder.
		///     <para>If an extension is not provided, a random extension (a <see cref="System.Guid" />) will be used.</para>
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
					extension = extension.Substring( startIndex: 1 );
				}
			}

			if ( String.IsNullOrWhiteSpace( extension ) ) {
				extension = Guid.NewGuid().ToString();
			}

			return new Document( folder: Folder.GetTempFolder(), filename: Guid.NewGuid() + "." + extension.Trim() );
		}

		[NotNull]
		public static implicit operator FileInfo( [NotNull] Document document ) => document.Info;

		/// <summary>
		///     <para>Compares the file names (case insensitive) and file sizes for inequality.</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean operator !=( [CanBeNull] Document left, [CanBeNull] Document right ) => !Equals( left: left, right: right );

		/// <summary>
		///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean operator ==( [CanBeNull] Document left, [CanBeNull] Document right ) => Equals( left: left, right: right );

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
			if ( !this.Exists() ) {
				yield break;
			}

			using ( var stream = new FileStream( this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {

				if ( !stream.CanRead ) {
					throw new NotSupportedException( $"Cannot read from file stream on {this.FullPathWithFileName}" );
				}

				using ( var buffered = new BufferedStream( stream: stream ) ) {
					var a = buffered.ReadByte();

					if ( a == -1 ) {
						yield break;
					}

					yield return ( Byte ) a;
				}
			}
		}

		/// <summary>
		///     Enumerates the <see cref="Document" /> as a sequence of <see cref="Int32" />.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Int32> AsInt32() {
			if ( !this.Exists() ) {
				yield break;
			}

			//TODO will wrapping this in a BufferedStream be any faster? Or is the buffersize okay?

			using ( var stream = new FileStream( this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, ( Int32 ) Constants.Sizes.OneGigaByte ) ) {

				if ( !stream.CanRead ) {
					throw new NotSupportedException( $"Cannot read from file stream on {this.FullPathWithFileName}" );
				}

				using ( var buffered = new BufferedStream( stream: stream ) ) {
					var a = buffered.ReadByte();

					if ( a == -1 ) {
						yield break;
					}

					var b = buffered.ReadByte();

					if ( b == -1 ) {
						yield return BitConverter.ToInt32( new[] {
							( Byte ) a, Byte.MinValue, Byte.MinValue, Byte.MinValue
						}, 0 );

						yield break;
					}

					var c = buffered.ReadByte();

					if ( c == -1 ) {
						yield return BitConverter.ToInt32( new[] {
							( Byte ) a, ( Byte ) b, Byte.MinValue, Byte.MinValue
						}, 0 );

						yield break;
					}

					var d = buffered.ReadByte();

					if ( d == -1 ) {
						yield return BitConverter.ToInt32( new[] {
							( Byte ) a, ( Byte ) b, ( Byte ) c, Byte.MinValue
						}, 0 );

						yield break;
					}

					yield return BitConverter.ToInt32( new[] {
						( Byte ) a, ( Byte ) b, ( Byte ) c, ( Byte ) d
					}, 0 );
				}
			}
		}

		/// <summary>
		///     <para>Clone the entire document to the <paramref name="destination" /> as quickly as possible.</para>
		///     <para>This will OVERWRITE any <see cref="destination" /> file.</para>
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="token"></param>
		/// <param name="progress">   </param>
		/// <param name="eta">        </param>
		/// <returns></returns>
		public async Task<(Boolean success, TimeSpan timeElapsed)> Clone( [NotNull] Document destination, CancellationToken token, [CanBeNull] IProgress<Single> progress = null,
			[CanBeNull] IProgress<TimeSpan> eta = null ) {
			if ( destination == null ) {
				throw new ArgumentNullException( nameof( destination ) );
			}

			var stopwatch = Stopwatch.StartNew();

			try {
				if ( this.Length.Any() ) {
					if ( Uri.TryCreate( this.FullPathWithFileName, UriKind.Absolute, out var sourceAddress ) ) {
						using ( var client = new WebClient().Add( token ) ) {

							await client.DownloadFileTaskAsync( sourceAddress, destination.FullPathWithFileName ).NoUI();

							return ( true, stopwatch.Elapsed );
						}
					}
				}

				//if ( fileSize <= this.GetBufferSize() ) {
				//    await this.Copy( destination, progress, eta ).NoUI();

				//    return (destination.Exists() && destination.Length == fileSize, stopwatch.Elapsed);
				//}

				//var processorCount = ( Int64 )Environment.ProcessorCount;

				//var chunksNeeded = fileSize / processorCount;

				//var buffers = new ThreadLocal<Byte[]>( () => new Byte[this.GetBufferSize()], trackAllValues: true );

				//var sourceStream = new FileStream( this.FullPathWithFileName, FileMode.Open, FileAccess.Read, FileShare.Read, this.GetBufferSize(), FileOptions.Asynchronous | FileOptions.RandomAccess );
				//var sourceBuffer = new BufferedStream( sourceStream, this.GetBufferSize() );
				//var sourceBinary = new BinaryReader( sourceBuffer, Encoding.Unicode );
				//var destinationStream = new FileStream( destination.FullPathWithFileName, FileMode.Create, FileAccess.Write, FileShare.Write, this.GetBufferSize(), FileOptions.Asynchronous | FileOptions.RandomAccess );
				//var destinationBuffer = new BufferedStream( destinationStream, this.GetBufferSize() );
				//var destinationBinary = new BinaryWriter( destinationBuffer, Encoding.Unicode );
			}
			catch ( WebException exception ) {
				exception.More();
			}

			return ( false, stopwatch.Elapsed );
		}

		/// <summary>
		///     Compares this. <see cref="FullPathWithFileName" /> against other <see cref="FullPathWithFileName" />.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Int32 CompareTo( [NotNull] Document other ) => String.Compare( strA: this.FullPathWithFileName, strB: other.FullPathWithFileName, comparisonType: StringComparison.Ordinal );

		/// <summary>
		///     Starts a task to copy a file
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="progress">   </param>
		/// <param name="eta">        </param>
		/// <returns></returns>
		public async Task Copy( Document destination, IProgress<Single> progress, IProgress<TimeSpan> eta ) =>
			await Task.Run( () => {
				var computer = new Computer();

				//TODO file monitor/watcher?
				computer.FileSystem.CopyFile( sourceFileName: this.FullPathWithFileName, destinationFileName: destination.FullPathWithFileName, showUI: UIOption.AllDialogs,
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
		public WebClient CopyFileWithProgress( [NotNull] Document destination, Action<Percentage> onProgress, Action onCompleted ) {

			var webClient = new WebClient();

			webClient.DownloadProgressChanged += ( sender, args ) => {
				var percentage = new Percentage( numerator: ( BigInteger ) args.BytesReceived, denominator: args.TotalBytesToReceive );
				onProgress?.Invoke( percentage );
			};

			webClient.DownloadFileCompleted += ( sender, args ) => onCompleted?.Invoke();

			webClient.DownloadFileAsync( address: new Uri( uriString: this.FullPathWithFileName ), fileName: destination.FullPathWithFileName );

			return webClient;
		}

		public Int32? CRC32() {
			if ( !this.Exists() ) {
				return null;
			}

			try {
				using ( var fileStream = File.Open( this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
					var size = ( UInt32 ) this.Size();

					var crc32 = new Crc32( polynomial: size, seed: size );

					var result = crc32.ComputeHash( fileStream );

					return BitConverter.ToInt32( result, 0 );
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

		public async Task<Int32?> CRC32Async( CancellationToken token ) => await Task.Run( () => this.CRC32(), token ).NoUI();

		public String CRC32Hex() {
			try {
				if ( !this.Exists() ) {
					return null;
				}

				using ( var fileStream = File.Open( this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
					var size = ( UInt32 ) this.Size();

					var crc32 = new Crc32( polynomial: size, seed: size );

					return crc32.ComputeHash( fileStream ).Aggregate( seed: String.Empty, func: ( current, b ) => current + b.ToString( format: "x2" ).ToLower() );
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
		public async Task<String> CRC32HexAsync( CancellationToken token ) => await Task.Run( () => this.CRC32Hex(), token ).NoUI();

		public Int64? CRC64() {
			try {
				if ( !this.Exists() ) {
					return null;
				}

				using ( var fileStream = File.Open( this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
					var size = ( UInt64 ) this.Size();
					var crc64 = new Crc64( polynomial: size, seed: size );

					return BitConverter.ToInt64( crc64.ComputeHash( fileStream ), 0 );
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

		public async Task<Int64?> CRC64Async( CancellationToken token ) => await Task.Run( () => this.CRC64(), token ).NoUI();

		/// <summary>
		///     Returns a lowercase hex-string of the hash.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		public String CRC64Hex() {
			try {
				if ( !this.Exists() ) {
					return null;
				}

				var size = ( UInt64 ) this.Size();

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
		///     Returns a lowercase hex-string of the hash.
		/// </summary>
		/// <returns></returns>
		public async Task<String> CRC64HexAsync( CancellationToken token ) => await Task.Run( () => this.CRC64Hex(), token ).NoUI();

		/// <summary>
		///     <para>Returns true if the <see cref="Document" /> no longer exists.</para>
		///     <para>Defaults to 3 retries</para>
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
					this.Info.Delete();
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

		/// <inheritdoc />
		public override void DisposeManaged() {
			if ( this.DeleteAfterClose ) {
				this.Delete();
			}

			base.DisposeManaged();
		}

		/// <summary>
		///     <para>Downloads (replaces) the local document with the specified <paramref name="source" />.</para>
		///     <para>Note: will replace the content of the this <see cref="Document" />.</para>
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public async Task<(Exception exception, WebHeaderCollection responseHeaders)> DownloadFile( [NotNull] Uri source ) {
			if ( source == null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			try {
				if ( !source.IsWellFormedOriginalString() ) {
					return ( new DownloadException( $"Could not use source Uri '{source}'." ), null );
				}

				using ( var webClient = new WebClient() ) {
					await webClient.DownloadFileTaskAsync( source, this.FullPathWithFileName ).NoUI();

					return ( null, webClient.ResponseHeaders );
				}
			}
			catch ( Exception exception ) {
				return ( exception, null );
			}
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

			if ( !oursize.Any() ) {

				//empty document? no buffer!
				return default;
			}

			if ( oursize <= Int32.MaxValue ) {
				return ( Int32 ) oursize;
			}

			return Int32.MaxValue;
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

		public Boolean HavePermission( FileIOPermissionAccess access ) {
			try {
				var bob = new FileIOPermission( access: access, this.FullPathWithFileName );
				bob.Demand();

				return true;
			}
			catch ( ArgumentException exception ) {
				exception.More();
			}
			catch ( SecurityException ) { }

			return false;
		}

		[NotNull]
		public String JustName() => Path.GetFileNameWithoutExtension( this.FileName() );

		/// <summary>
		///     Attempt to start the process.
		/// </summary>
		/// <param name="arguments"></param>
		/// <param name="verb">     "runas" is elevated</param>
		/// <returns></returns>
		[CanBeNull]
		public async Task<Process> Launch( [CanBeNull] String arguments = null, String verb = "runas" ) =>
			await Task.Run( function: () => {
				try {
					var info = new ProcessStartInfo( fileName: this.FullPathWithFileName ) {
						Arguments = arguments ?? String.Empty,
						UseShellExecute = false,
						Verb = verb
					};

					return Process.Start( startInfo: info );
				}
				catch ( Exception exception ) {
					exception.More();
				}

				return null;
			} );

		/// <summary>
		///     Attempt to return an object Deserialized from this JSON text file.
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
						return new JsonSerializer {
							ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
							PreserveReferencesHandling = PreserveReferencesHandling.All
						}.Deserialize<T>( jsonReader );
					}
				}
			}
			catch ( Exception exception ) {
				exception.More();
			}

			return default;
		}

		public async Task<T> LoadJSONAsync<T>( CancellationToken token ) => await Task.Run( () => this.LoadJSON<T>(), token ).NoUI();

		/// <summary>
		///     <para>Starts a task to <see cref="Move" /> a file to the <paramref name="destination" />.</para>
		///     <para>Returns -1 if an exception happened.</para>
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="token"></param>
		/// <param name="exact">If true, the file creation and lastwrite dates are set after the <see cref="Move" />.</param>
		/// <returns></returns>
		public async Task<Int64?> Move( [NotNull] Document destination, CancellationToken token, Boolean exact = true ) {
			if ( destination is null ) {
				throw new ArgumentNullException( paramName: nameof( destination ) );
			}

			DocumentInfo data = null;

			if ( exact ) {
				data = new DocumentInfo( this );
				await data.Scan( token ).NoUI();
			}

			return await Task.Run( () => {
				try {
					var computer = new Computer();

					computer.FileSystem.MoveFile( sourceFileName: this.FullPathWithFileName, destinationFileName: destination.FullPathWithFileName, showUI: UIOption.AllDialogs,
						onUserCancel: UICancelOption.DoNothing );

					return destination.Length();
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
				finally {
					if ( exact && destination.Exists() ) {

						// ReSharper disable once UseNullPropagationWhenPossible //WTF, this produces the wrong code.
						if ( data != null ) {
							if ( data.CreationTimeUtc.HasValue ) {
								destination.Info.CreationTime = data.CreationTimeUtc.Value;
							}

							if ( data.LastWriteTimeUtc.HasValue ) {
								destination.Info.CreationTime = data.LastWriteTimeUtc.Value;
							}
						}
					}
				}

				return -1;
			}, token ).NoUI();
		}

		/// <summary>
		///     <para>Just the file's name, including the extension.</para>
		/// </summary>
		/// <seealso cref="Path.GetFileNameWithoutExtension" />
		[NotNull]
		public String Name() => this.FileName();

		public async Task<String> ReadStringAsync() {
			using ( var reader = new StreamReader( this.FullPathWithFileName ) ) {
				return await reader.ReadToEndAsync().NoUI();
			}
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

			if ( right is null || !this.Exists() || !right.Exists() ) {
				return false;
			}

			return this.Length == right.Length && this.AsBytes().SequenceEqual( second: right.AsBytes() );
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
		[NotNull]
		public StreamReader StreamReader() => new StreamReader( File.OpenRead( this.FullPathWithFileName ) );

		/// <summary>
		///     Open the file for writing and return a <see cref="StreamWriter" />.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public StreamWriter StreamWriter() => new StreamWriter( File.OpenWrite( this.FullPathWithFileName ) );

		/// <summary>
		///     Return this <see cref="Document" /> as a JSON string.
		/// </summary>
		/// <returns></returns>
		public async Task<String> ToJSON() {
			using ( var reader = new StreamReader( this.FullPathWithFileName ) ) {
				return await reader.ReadToEndAsync().NoUI();
			}
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
			var stopwatch = Stopwatch.StartNew();
			TryAgain:

			try {
				if ( !this.Exists() ) {
					return true;
				}

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
			finally {
				stopwatch.Stop();
			}

			return null;
		}

		/// <summary>
		///     Uploads this <see cref="Document" /> to the given <paramref name="destination" />.
		/// </summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		public async Task<(Exception exception, WebHeaderCollection responseHeaders)> UploadFile( [NotNull] Uri destination ) {
			if ( destination == null ) {
				throw new ArgumentNullException( nameof( destination ) );
			}

			if ( !destination.IsWellFormedOriginalString() ) {
				return ( new ArgumentException( $"Destination address '{destination.OriginalString}' is not well formed.", nameof( destination ) ), null );
			}

			try {
				using ( var webClient = new WebClient() ) {
					await webClient.UploadFileTaskAsync( destination, this.FullPathWithFileName ).NoUI();

					return ( null, webClient.ResponseHeaders );
				}
			}
			catch ( Exception exception ) {
				return ( exception, null );
			}
		}

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}