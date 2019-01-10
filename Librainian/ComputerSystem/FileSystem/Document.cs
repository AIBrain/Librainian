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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "Document.cs" was last formatted by Protiguous on 2018/11/23 at 12:21 AM.

namespace Librainian.ComputerSystem.FileSystem {

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
	using Logging;
	using Maths;
	using Maths.Hashings;
	using Maths.Numbers;
	using Measurement.Time;
	using Microsoft.VisualBasic.Devices;
	using Microsoft.VisualBasic.FileIO;
	using Newtonsoft.Json;
	using Parsing;
	using Persistence;
	using Security;
	using Threading;

	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	public class Document : Αρχείο,  IComparable<Document> {

		/// <summary>
		///     Compares this. <see cref="FullPath" /> against other <see cref="FullPath" />.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Int32 CompareTo( [NotNull] Document other ) => String.Compare( strA: this.FullPath, strB: other.FullPath, comparisonType: StringComparison.Ordinal );


		public Document( [NotNull] String fullPath, [NotNull] String filename, Boolean deleteAfterClose = false ) : this( fullPathWithFilename: Path.Combine( path1: fullPath, path2: filename ),
			deleteAfterClose: deleteAfterClose ) { }

	

		public Document( [NotNull] FileSystemInfo info, Boolean deleteAfterClose = false ) : this( fullPathWithFilename: info.FullName, deleteAfterClose: deleteAfterClose ) { }

		public Document( [NotNull] IFolder folder, [NotNull] String filename, Boolean deleteAfterClose = false ) : this( fullPath: folder.FullName, filename: filename, deleteAfterClose: deleteAfterClose ) { }

		public Document( [NotNull] IFolder folder, [NotNull] Document document, Boolean deleteAfterClose = false ) : this( fullPathWithFilename: Path.Combine( path1: folder.FullName, path2: document.FileName() ),
			deleteAfterClose: deleteAfterClose ) { }

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
			if ( !this.CurrentFolder.Create() ) {
				throw new DirectoryNotFoundException( this.FullPath );
			}

			if ( this.Exists() ) {
				using ( var writer = File.AppendText( this.FullPath ) ) {
					writer.WriteLine( text );
					writer.Flush();
				}
			}
			else {
				using ( var writer = File.CreateText( this.FullPath ) ) {
					writer.WriteLine( text );
					writer.Flush();
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
		public Int32 CalcHashInt32( Boolean inParallel = false ) {

			var result = 0;

			if ( !inParallel ) {
				foreach ( var b in this.AsInt32() ) {
					unchecked {
						result += b.Deterministic();
					}
				}
			}
			else {
				throw new NotImplementedException( "Not Implemented Yet" );
			}

			return result;
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
					if ( Uri.TryCreate( this.FullPath, UriKind.Absolute, out var sourceAddress ) ) {
						using ( var client = new WebClient().Add( token ) ) {

							await client.DownloadFileTaskAsync( sourceAddress, destination.FullPath ).ConfigureAwait( false );

							return (true, stopwatch.Elapsed);
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
		public Task Copy( Document destination, IProgress<Single> progress, IProgress<TimeSpan> eta ) =>
			Task.Run( () => {
				var computer = new Computer();

				//TODO file monitor/watcher?
				computer.FileSystem.CopyFile( sourceFileName: this.FullPath, destinationFileName: destination.FullPath, showUI: UIOption.AllDialogs, onUserCancel: UICancelOption.DoNothing );
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
				var percentage = new Percentage( numerator: ( BigInteger )args.BytesReceived, denominator: args.TotalBytesToReceive );
				onProgress?.Invoke( percentage );
			};

			webClient.DownloadFileCompleted += ( sender, args ) => onCompleted?.Invoke();

			webClient.DownloadFileAsync( address: new Uri( uriString: this.FullPath ), fileName: destination.FullPath );

			return webClient;
		}

		public Int32? CRC32() {
			if ( !this.Exists() ) {
				return null;
			}

			try {
				using ( var fileStream = File.OpenRead( this.FullPath ) ) {
					var size = ( UInt32 )this.Size();

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

		[NotNull]
		public Task<Int32?> CRC32Async( CancellationToken token ) => Task.Run( this.CRC32, token );

		public String CRC32Hex() {
			try {
				if ( !this.Exists() ) {
					return null;
				}

				using ( var fileStream = File.OpenRead( this.FullPath ) ) {
					var size = ( UInt32 )this.Size();

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
		public Task<String> CRC32HexAsync( CancellationToken token ) => Task.Run( this.CRC32Hex, token );

		public Int64? CRC64() {
			try {
				if ( !this.Exists() ) {
					return null;
				}

				using ( var fileStream = File.OpenRead( this.FullPath ) ) {
					var size = ( UInt64 )this.Size();
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

		[NotNull]
		public Task<Int64?> CRC64Async( CancellationToken token ) => Task.Run( this.CRC64, token );

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

				var size = ( UInt64 )this.Size();

				var crc64 = new Crc64( polynomial: size, seed: size );

				using ( var fileStream = File.OpenRead( this.FullPath ) ) {
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
		[NotNull]
		public Task<String> CRC64HexAsync( CancellationToken token ) => Task.Run( this.CRC64Hex, token );

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
					return (new DownloadException( $"Could not use source Uri '{source}'." ), null);
				}

				using ( var webClient = new WebClient() ) {
					await webClient.DownloadFileTaskAsync( source, this.FullPath ).ConfigureAwait( false );

					return (null, webClient.ResponseHeaders);
				}
			}
			catch ( Exception exception ) {
				return (exception, null);
			}
		}

		/// <summary>
		///     Returns true if the <see cref="Document" /> was known to exist.
		/// </summary>
		/// <exception cref="IOException"></exception>
		public Boolean? Exists() => this.Info.Exists();

		/// <summary>
		///     <para>Computes the extension of the <see cref="FileName" />, including the prefix ".".</para>
		/// </summary>
		[NotNull]
		public String Extension() => Path.GetExtension( this.FullPath ).Trim().NullIfEmptyOrWhiteSpace() ?? String.Empty;

		/// <summary>
		///     <para>Just the file's name, including the extension.</para>
		/// </summary>
		/// <see cref="Path.GetFileName" />
		[NotNull]
		public String FileName() => Path.GetFileName( this.FullPath );

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
				return ( Int32 )oursize;
			}

			return Int32.MaxValue;
		}

		/// <summary>
		///     (file name, not contents)
		/// </summary>
		/// <returns></returns>
		public override Int32 GetHashCode() => this.FullPath.GetHashCode();

		public Boolean HavePermission( FileIOPermissionAccess access ) {
			try {
				var bob = new FileIOPermission( access: access, this.FullPath );
				bob.Demand();

				return true;
			}
			catch ( ArgumentException exception ) {
				exception.Log();
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

				return Task.Run( () => Process.Start( startInfo: info ) );
			}
			catch ( Exception exception ) {
				exception.Log();

				return Task.FromException<Process>( exception );
			}
		}

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
				using ( var textReader = File.OpenText( this.FullPath ) ) {
					using ( var jsonReader = new JsonTextReader( textReader ) ) {
						return new JsonSerializer {
							ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
							PreserveReferencesHandling = PreserveReferencesHandling.All
						}.Deserialize<T>( jsonReader );
					}
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		[NotNull]
		public Task<T> LoadJSONAsync<T>( CancellationToken token ) => Task.Run( this.LoadJSON<T>, token );

		/// <summary>
		///     <para>Starts a task to <see cref="MoveAsync" /> a file to the <paramref name="destination" />.</para>
		///     <para>Returns -1 if an exception happened.</para>
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="token"></param>
		/// <param name="exact">If true, the file creation and lastwrite dates are set after the <see cref="MoveAsync" />.</param>
		/// <returns></returns>
		[NotNull]
		public Task<Int64> MoveAsync( [NotNull] Document destination, CancellationToken token, Boolean exact = true ) {
			if ( destination == null ) {
				throw new ArgumentNullException( paramName: nameof( destination ) );
			}

			var jane = Task.Run( () => {
				try {
					var computer = new Computer();

					computer.FileSystem.MoveFile( sourceFileName: this.FullPath, destinationFileName: destination.FullPath, showUI: UIOption.AllDialogs, onUserCancel: UICancelOption.DoNothing );

					if ( exact ) {
						if ( destination.Exists() ) {
							var data = new DocumentInfo( this );

							if ( data.CreationTimeUtc.HasValue ) {
								destination.Info.CreationTime = data.CreationTimeUtc.Value;
							}

							if ( data.LastWriteTimeUtc.HasValue ) {
								destination.Info.CreationTime = data.LastWriteTimeUtc.Value;
							}
						}
					}

					return destination.Size();
				}
				catch ( FileNotFoundException exception ) {
					return Task.FromException<Int64>( exception ).Result;
				}
				catch ( DirectoryNotFoundException exception ) {
					return Task.FromException<Int64>( exception ).Result;
				}
				catch ( PathTooLongException exception ) {
					return Task.FromException<Int64>( exception ).Result;
				}
				catch ( IOException exception ) {
					return Task.FromException<Int64>( exception ).Result;
				}
				catch ( UnauthorizedAccessException exception ) {
					return Task.FromException<Int64>( exception ).Result;
				}
			}, token );

			return jane;
		}

		/// <summary>
		///     <para>Just the file's name, including the extension.</para>
		/// </summary>
		/// <see cref="Path.GetFileNameWithoutExtension" />
		[NotNull]
		public String Name() => this.FileName();

		public async Task<String> ReadStringAsync() {
			using ( var reader = new StreamReader( this.FullPath ) ) {
				return await reader.ReadToEndAsync().ConfigureAwait( false );
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

			if ( right == null || !this.Exists() || !right.Exists() ) {
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

				File.SetCreationTime( this.FullPath, creationTime: when );

				return true;
			}, tryFor: Seconds.Five, token: cancellationToken );
		}

		/// <summary>
		///     Open the file for reading and return a <see cref="StreamReader" />.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public StreamReader StreamReader() => new StreamReader( File.OpenRead( this.FullPath ) );

		/// <summary>
		///     Open the file for writing and return a <see cref="StreamWriter" />.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public StreamWriter StreamWriter() => new StreamWriter( File.OpenWrite( this.FullPath ) );

		/// <summary>
		///     Return this <see cref="Document" /> as a JSON string.
		/// </summary>
		/// <returns></returns>
		public async Task<String> ToJSON() {
			using ( var reader = new StreamReader( this.FullPath ) ) {
				return await reader.ReadToEndAsync().ConfigureAwait( false );
			}
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override String ToString() => this.FullPath;

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
				return (new ArgumentException( $"Destination address '{destination.OriginalString}' is not well formed.", nameof( destination ) ), null);
			}

			try {
				using ( var webClient = new WebClient() ) {
					await webClient.UploadFileTaskAsync( destination, this.FullPath ).ConfigureAwait( false );

					return (null, webClient.ResponseHeaders);
				}
			}
			catch ( Exception exception ) {
				return (exception, null);
			}
		}

	}

}