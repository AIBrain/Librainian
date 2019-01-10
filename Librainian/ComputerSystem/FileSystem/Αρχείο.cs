// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Αρχείο.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Αρχείο.cs" was last formatted by Protiguous on 2019/01/06 at 12:53 PM.

namespace Librainian.ComputerSystem.FileSystem {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;
	using System.Security.Permissions;
	using JetBrains.Annotations;
	using Maths;
	using Microsoft.Win32;
	using Microsoft.Win32.SafeHandles;
	using Newtonsoft.Json;
	using OperatingSystem;
	using Parsing;

	/// <summary>
	///     A lighterweight replacement for <see cref="FileInfo" />.
	/// </summary>
	/// <remarks>
	///     <para>Αρχείο is Greek for "File".</para>
	///     <para>
	///         This code is originally based from <see cref="FileSystemInfo" />, but updated for modern computer file
	///         systems.
	///     </para>
	/// <para>Attempting to ensure paths (longer than 260 chars!) up to 32767 chars are supported.</para>
	/// </remarks>
	[Serializable]
	public class Αρχείο : ISerializable, IEquatable<Αρχείο>, IEnumerable<Byte> {

		public virtual void GetObjectData( SerializationInfo info, StreamingContext context ) {
			info.AddValue( "FullPath", this.FullPath, typeof( String ) );
			this.Refresh( throwOnError: false );
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
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		/// <summary>
		///     Enumerates the <see cref="Document" /> as a sequence of <see cref="Byte" />.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Byte> AsBytes() {
			if ( this.Exists() == false ) {
				yield break;
			}

			using ( var stream = new FileStream( this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {

				if ( !stream.CanRead ) {
					throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath}" );
				}

				using ( var buffered = new BufferedStream( stream: stream ) ) {
					var a = buffered.ReadByte();

					if ( a == -1 ) {
						yield break;
					}

					yield return ( Byte )a;
				}
			}
		}

		[StructLayout( LayoutKind.Sequential )]
		internal class SECURITY_ATTRIBUTES {
			internal unsafe Byte* pSecurityDescriptor = null;
			internal Int32 nLength;
			internal Int32 bInheritHandle;
		}

		/// <summary>
		/// Maybe someday rewrite <see cref="Αρχείο"/> to use long path names, with faster creation, opening, and saving.
		/// </summary>
		/// <param name="lpFileName"></param>
		/// <param name="dwDesiredAccess"></param>
		/// <param name="dwShareMode"></param>
		/// <param name="securityAttrs"></param>
		/// <param name="dwCreationDisposition"></param>
		/// <param name="dwFlagsAndAttributes"></param>
		/// <param name="hTemplateFile"></param>
		/// <returns></returns>
		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false )]
		private static extern SafeFileHandle CreateFile(
			String lpFileName,
			Int32 dwDesiredAccess,
			FileShare dwShareMode,
			SECURITY_ATTRIBUTES securityAttrs,
			FileMode dwCreationDisposition,
			Int32 dwFlagsAndAttributes,
			IntPtr hTemplateFile );


		/// <summary>
		///     Enumerates the <see cref="Document" /> as a sequence of <see cref="Int32" />.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Int32> AsInt32() {
			if ( this.Exists() == false ) {
				yield break;
			}

			using ( var stream = new FileStream( this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, ( Int32 )Constants.Sizes.OneGigaByte ) ) {

				if ( !stream.CanRead ) {
					throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath}" );
				}

				var four = new FourBytes(); //reuse this buffer to avoid GC

				using ( var buffered = new BufferedStream( stream: stream ) ) {
					four.W = 0; //reset

					var a = buffered.ReadByte();

					if ( a == -1 ) {
						yield break;
					}

					four.A = ( Byte )a;

					var b = buffered.ReadByte();

					if ( b == -1 ) {

						//yield return BitConverter.ToInt32( new[] { ( Byte ) a, Byte.MinValue, Byte.MinValue, Byte.MinValue }, 0 );
						yield return four.W;

						yield break;
					}

					four.B = ( Byte )b;

					var c = buffered.ReadByte();

					if ( c == -1 ) {

						//yield return BitConverter.ToInt32( new[] {( Byte ) a, ( Byte ) b, Byte.MinValue, Byte.MinValue}, 0 );
						yield return four.W;

						yield break;
					}

					four.C = ( Byte )c;

					var d = buffered.ReadByte();

					if ( d == -1 ) {

						//yield return BitConverter.ToInt32( new[] {( Byte ) a, ( Byte ) b, ( Byte ) c, Byte.MinValue}, 0 );
						yield return four.W;

						yield break;
					}

					four.D = ( Byte )d;

					//yield return BitConverter.ToInt32( new[] {( Byte ) a, ( Byte ) b, ( Byte ) c, ( Byte ) d}, 0 );
					yield return four.W;
				}
			}
		}

		/// <summary>Represents the fully qualified path of the file.</summary>
		[JsonProperty]
		[NotNull]
		public readonly String FullPath;

		/// <summary>
		///     Gets or sets the <see cref="FileAttributes" /> for <see cref="FullPath" />.
		/// </summary>
		/// <exception cref="FileNotFoundException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="IOException"></exception>
		[JsonIgnore]
		public FileAttributes? Attributes {
			get => this.FileAttributeData.FileAttributes;

			set {
				if ( !value.HasValue ) {
					return; //TODO eh?
				}

				if ( !NativeMethods.SetFileAttributes( this.FullPath, ( UInt32 ) value.Value ) ) {
					NativeMethods.HandleLastError( this.FullPath);
				}
			}
		}

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
			get => this.FileAttributeData.CreationTime;

			set {
				try {
					if ( value.HasValue ) {
						File.SetCreationTimeUtc( this.FullPath, value.Value );
					}
				}
				finally {
					this.Refresh();
				}
			}
		}

		/// <summary>Gets the string representing the extension part of the file.</summary>
		[JsonIgnore]
		[NotNull]
		public String Extension => Path.GetExtension( this.FullPath );

		[JsonIgnore]
		public FileAttributeData FileAttributeData { get; private set; }

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
			get => this.FileAttributeData.LastAccessTime;

			set {
				try {
					if ( value.HasValue ) {
						File.SetLastAccessTimeUtc( this.FullPath, value.Value );
					}
				}
				finally {
					this.Refresh();
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
			get => this.FileAttributeData.LastWriteTime;

			set {
				try {
					if ( value.HasValue ) {
						File.SetLastWriteTimeUtc( this.FullPath, value.Value );
					}
				}
				finally {
					this.Refresh();
				}
			}
		}

		protected Αρχείο( [NotNull] SerializationInfo info, StreamingContext context ) {
			if ( info == null ) {
				throw new ArgumentNullException( nameof( info ) );
			}

			this.DeleteAfterClose = false;
			this.FullPath = info.GetString( nameof( this.FullPath ) );
			this.Refresh( throwOnError: true );
		}

		/// <summary>
		/// </summary>
		/// <param name="fullPath"></param>
		/// <param name="throwOnError"></param>
		/// <param name="deleteAfterClose"></param>
		/// <exception cref="FileNotFoundException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="IOException"></exception>
		public Αρχείο( [NotNull] String fullPath, Boolean throwOnError = true, Boolean deleteAfterClose = false ) {
			if ( String.IsNullOrWhiteSpace( value: fullPath ) ) {
				throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fullPath ) );
			}

			this.DeleteAfterClose = deleteAfterClose;

			this.FullPath = fullPath;
			this.Refresh( throwOnError: throwOnError );

			var filename = this.FileName();

			if ( filename != null ) {
				var bob = new FileSystemWatcher( this.FullPath, filename ); //TODO
			}
		}

		private Αρχείο() {
			throw new NotImplementedException( "Private contructor is not allowed." );
		}

		public void Refresh( Boolean throwOnError = true ) {
			var handle = NativeMethods.FindFirstFile( this.FullPath, out var data );

			if ( handle.IsInvalid ) {
				if ( throwOnError ) {
					NativeMethods.HandleLastError(this.FullPath);
				}
				else {
					this.FileAttributeData = new FileAttributeData();
				}
			}

			this.FileAttributeData = new FileAttributeData( data );
		}

		/// <summary>Deletes the file.</summary>
		public void Delete() {
			if ( this.Exists() == true ) {
				if ( !NativeMethods.DeleteFile( this.FullPath ) ) {
					NativeMethods.HandleLastError(this.FullPath);
				}
			}
		}

		/// <summary>Returns whether the file exists.</summary>
		public Boolean? Exists() {
			if ( !this.FileAttributeData.Exists.HasValue ) {
				this.Refresh();
			}
			return this.FileAttributeData.Exists;
		}

		/// <summary>
		///     Returns just the filename portion of <see cref="FullPath" />.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		public String FileName() => Path.GetFileName( this.FullPath );

		[NotNull]
		public Folder ContaingingFolder() {
			var dir = Path.GetDirectoryName( this.FullPath );

			if ( String.IsNullOrEmpty(dir) ) {
				//empty means a root-level folder (C:\) was found. Right?
				dir = Path.GetPathRoot( this.FullPath );
			}

			return new Folder( dir );
		}

		/// <summary>
		/// Returns the length of the file (if it exists).
		/// <para>Remember to call <see cref="Refresh"/> if the file has changed.</para>
		/// </summary>
		[JsonIgnore]
		public UInt64? Length => this.FileAttributeData.FileSize;

		public Boolean DeleteAfterClose { get; }

		/// <summary>
		/// Returns the size of the file, if it exists.
		/// <para>Remember to call <see cref="Refresh"/> if the file has changed.</para>
		/// </summary>
		/// <returns></returns>
		public UInt64? Size() => this.FileAttributeData.FileSize;

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

			return new Document( folder: Folder.GetTempFolder(), filename: $"{Guid.NewGuid()}.{extension.Trim()}" );
		}

		/// <summary>
		///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
		///     <para>To compare the contents of two <see cref="Document" /> use <see cref="Document.SameContent" />.</para>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Boolean Equals( Αρχείο other ) => Equals( left: this, right: other );

		/// <summary>
		///     <para>Static case sensitive comparison of the file names and file sizes for equality.</para>
		///     <para>To compare the contents of two <see cref="Document" /> use <see cref="Document.SameContent" />.</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] Αρχείο left, [CanBeNull] Αρχείο right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left == null || right == null ) {
				return false;
			}

			return left.FullPath.Is( right: right.FullPath ) && left.Size() == right.Size();
		}

		/// <summary>
		///     <para>To compare the contents of two <see cref="Document" /> use SameContent( Document,Document).</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override Boolean Equals( Object obj ) => obj is Document document && Equals( left: this, right: document );

	}

}