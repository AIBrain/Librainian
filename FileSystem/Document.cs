// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks goes
// to the Authors.
//
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
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
	using Microsoft.VisualBasic.FileIO;
	using Newtonsoft.Json;
	using NUnit.Framework;
	using Parsing;
	using Security;
	using Threading;
	using Computer = Microsoft.VisualBasic.Devices.Computer;

	/// <summary>
	/// <para>
	/// An immutable wrapper for a file, the extension, the [parent] folder, and the file's size all
	/// from a given full path.
	/// </para>
	/// <para>Also contains static String versions from <see cref="Path"/></para>
	/// </summary>
	[Immutable]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	public class Document : IEquatable<Document>, IEnumerable<Byte>, IComparable<Document> {

		[NotNull]
		public readonly FileInfo Info;

		// ReSharper disable once NotNullMemberIsNotInitialized
		private Document() => throw new NotImplementedException();

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
			if ( String.IsNullOrWhiteSpace( value: fullPathWithFilename ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", paramName: nameof( fullPathWithFilename ) );
			}

			this.Info = new FileInfo( fullPathWithFilename );
		}

		public Document( [NotNull] FileSystemInfo info ) : this( info.FullName ) {
			if ( info is null ) {
				throw new ArgumentNullException( paramName: nameof( info ) );
			}
		}

		public Document( [NotNull] Folder folder, [NotNull] String filename ) : this( folder.FullName, filename ) {
			if ( folder is null ) {
				throw new ArgumentNullException( paramName: nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( value: filename ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", paramName: nameof( filename ) );
			}
		}

		public Document( [NotNull] Folder baseFolder, UInt64 index ) {
			var path = IndexToPath( baseFolder: baseFolder, index: index );
			this.Info = new FileInfo( path );
			//TODO this.Info.Exists.ShouldBeTrue();
		}

		public Document( Folder folder, Document document ) : this( Path.Combine( folder.FullName, document.FileName() ) ) {
		}

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

			var tempFolder = Librainian.FileSystem.Folder.GetTempFolder();
			if ( null == tempFolder ) {
				throw new DirectoryNotFoundException( "Unable to find user's temp folder." );
			}

			var tempFilename = Path.GetFileName( uri.AbsolutePath );

			var downloadLocation = new Document( tempFolder, tempFilename );

			var webClient = new WebClient();
			webClient.DownloadFileAsync( uri, downloadLocation.FullPathWithFileName );

			this.Info = new FileInfo( downloadLocation.FullPathWithFileName );
		}

		/// <summary>
		/// <para>The <see cref="Folder"/> where this <see cref="Document"/> is stored.</para>
		/// </summary>
		[NotNull]
		public Folder Folder => new Folder( this.Info.Directory );

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

			return left.Size() == right.Size() && left.FullPathWithFileName.Same( right.FullPathWithFileName );
		}

		/// <summary>
		/// Returns a unique file in the user's temp folder.
		/// <para>
		/// If an extension is not provided, a random extension (a <see cref="Guid"/>) will be used.
		/// </para>
		/// <para><b>Note</b>: Does not create a 0-byte file like <see cref="Path.GetTempFileName"/>.</para>
		/// <para>If the temp folder is not found, one attempt will be made to create it.</para>
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

		public static implicit operator FileInfo( Document document ) => document.Info;

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

		/// <summary>
		/// <para>Compares the file names (case insensitive) and file sizes for inequality.</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean operator !=( Document left, Document right ) => !Equals( left, right );

		/// <summary>
		/// <para>Compares the file names (case insensitive) and file sizes for equality.</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean operator ==( Document left, Document right ) => Equals( left, right );

		[Test]
		public static void Test_IndexToPath() {
			var largestEmptiestDrive = IOExtensions.GetLargestEmptiestDrive();
			var baseFolder = new Folder( largestEmptiestDrive.RootDirectory.FullName + @"\test\" );
		}

		/// <summary>
		/// <para>If the file does not exist, it is created.</para>
		/// <para>Then the <paramref name="text"/> is appended to the file.</para>
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
		/// Enumerates the <see cref="Document"/> as a sequence of <see cref="Byte"/>.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Byte> AsByteArray() {
			if ( !this.Exists() ) {
				yield break;
			}

			var stream = IOExtensions.ReTry( () => new FileStream( path: this.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ), Seconds.Seven, CancellationToken.None );

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
		/// Compares this. <see cref="FullPathWithFileName"/> against other <see cref="FullPathWithFileName"/>.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Int32 CompareTo( Document other ) => String.Compare( this.FullPathWithFileName, other.FullPathWithFileName, StringComparison.Ordinal );

		/// <summary>
		/// Starts a task to copy a file
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="progress">   </param>
		/// <param name="eta">        </param>
		/// <returns></returns>
		public Task Copy( Document destination, Action<Double> progress, Action<TimeSpan> eta ) => Task.Run( () => {
			var computer = new Computer();

			//TODO file monitor/watcher?
			computer.FileSystem.CopyFile( this.FullPathWithFileName, destination.FullPathWithFileName, UIOption.AllDialogs, UICancelOption.DoNothing );
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
				var percentage = new Percentage( ( BigInteger )args.BytesReceived, args.TotalBytesToReceive );
				onProgress?.Invoke( percentage );
			};
			webClient.DownloadFileCompleted += ( sender, args ) => onCompleted?.Invoke();

			webClient.DownloadFileAsync( new Uri( this.FullPathWithFileName ), destination.FullPathWithFileName );

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
				var crc32 = new Crc32( ( UInt32 )size.Value, ( UInt32 )size.Value );

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
		/// <para>Returns true if the <see cref="Document"/> no longer exists.</para>
		/// </summary>
		/// <returns></returns>
		public Boolean Delete() {
			var retries = 10;
			TryAgain:
			try {
				if ( !this.Exists() ) {
					return true;
				}

				if ( this.Info.IsReadOnly ) {
					this.Info.IsReadOnly = false;
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
		/// <para>Compares the file names (case insensitive) and file sizes for equality.</para>
		/// <para>To compare the contents of two <see cref="Document"/> use <see cref="SameContent(Document)"/>.</para>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Boolean Equals( Document other ) => !( other is null ) && Equals( this, other );

		/// <summary>
		/// <para>To compare the contents of two <see cref="Document"/> use SameContent( Document,Document).</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override Boolean Equals( Object obj ) => obj is Document && Equals( this, ( Document )obj );

		/// <summary>
		/// Returns true if the <see cref="Document"/> currently exists.
		/// </summary>
		/// <exception cref="IOException"></exception>
		public Boolean Exists() => this.Info.Exists;

		/// <summary>
		/// <para>Computes the extension of the <see cref="FileName"/>, including the prefix ".".</para>
		/// </summary>
		[NotNull]
		public String Extension() => Path.GetExtension( this.FullPathWithFileName ).Trim().NullIfEmptyOrWhiteSpace() ?? String.Empty;

		/// <summary>
		/// <para>Just the file's name, including the extension.</para>
		/// </summary>
		/// <seealso cref="Path.GetFileName"/>
		[NotNull]
		public String FileName() => Path.GetFileName( this.FullPathWithFileName );

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate
		/// through the collection.
		/// </returns>
		public IEnumerator<Byte> GetEnumerator() => this.AsByteArray().GetEnumerator();

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate
		/// through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

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
		public async Task<Process> Launch( String arguments = null, String verb = "runas" ) => await Task.Run( () => {
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

		/// <summary>
		/// Starts a task to copy a file
		/// </summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		public Task<Boolean> Move( [NotNull] Document destination ) {
			if ( destination is null || String.IsNullOrWhiteSpace( destination.FullPathWithFileName ) ) {
				throw new ArgumentNullException( paramName: nameof( destination ) );
			}
			return Task.Run( () => {
				try {
					var computer = new Computer();
					computer.FileSystem.MoveFile( this.FullPathWithFileName, destination.FullPathWithFileName, UIOption.AllDialogs, UICancelOption.DoNothing );
					return computer.FileSystem.FileExists( destination.FullPathWithFileName );
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
				return await Task.Run( () => this.Exists() ? File.ReadAllText( this.FullPathWithFileName ) : String.Empty );
			}
			catch ( Exception ) {
				return null;
			}
		}

		public async Task<String> ReadToEndAsync() {
			using ( var reader = new StreamReader( this.FullPathWithFileName ) ) {
				return await reader.ReadToEndAsync().ConfigureAwait( false );
			}
		}

		/// <summary>
		/// <para>
		/// Performs a byte by byte file comparison, but ignores the <see cref="Document"/> file names.
		/// </para>
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

			return ll.Value == rl.Value && this.AsByteArray().SequenceEqual( right.AsByteArray() );
		}

		public void SetCreationTime( DateTime when, CancellationToken cancellationToken ) {
			IOExtensions.ReTry( () => {
				if ( !Exists() ) {
					return false;
				}

				this.Info.IsReadOnly = false;
				return !this.Info.IsReadOnly;
			}, Seconds.Five, cancellationToken );

			IOExtensions.ReTry( () => {
				if ( !Exists() ) {
					return false;
				}

				File.SetCreationTime( path: this.FullPathWithFileName, creationTime: when );
				return true;
			}, Seconds.Five, cancellationToken );
		}

		/// <summary>
		/// <para>Gets the current size of the <see cref="Document"/>.</para>
		/// </summary>
		/// <seealso cref="GetLength"/>
		[CanBeNull]
		public UInt64? Size() => this.GetLength();

		public async Task<String> ToJSON( Document outfile ) {
			using ( var reader = new StreamReader( this.FullPathWithFileName ) ) {
				return await reader.ReadToEndAsync().ConfigureAwait( false );
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
	}
}
