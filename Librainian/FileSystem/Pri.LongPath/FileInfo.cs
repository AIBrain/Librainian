// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "FileInfo.cs" last formatted on 2020-08-14 at 8:39 PM.

namespace Librainian.FileSystem.Pri.LongPath {

	using System;
	using System.IO;
	using System.Text;
	using JetBrains.Annotations;

	public class FileInfo : FileSystemInfo {

		public FileInfo( [NotNull] String fileName ) : base( fileName.GetFullPath() ) => this.Name = this.FullPath.GetFileName();

		[NotNull]
		public String DirectoryName => this.FullPath.GetDirectoryName();

		public override Boolean Exists {
			get {
				if ( this.state == State.Uninitialized ) {
					this.Refresh();
				}

				var fileAttributeData = this.data;

				return fileAttributeData != null && this.state == State.Initialized &&
					   ( fileAttributeData.fileAttributes & FileAttributes.Directory ) != FileAttributes.Directory;
			}
		}

		public Boolean IsReadOnly {
			get => this.Attributes.HasFlag( FileAttributes.ReadOnly );

			set {
				if ( value ) {
					this.Attributes |= FileAttributes.ReadOnly;

					return;
				}

				this.Attributes &= ~FileAttributes.ReadOnly;
			}
		}

		public Int64 Length => this.GetFileLength();

		[NotNull]
		public override String Name { get; }

		[NotNull]
		public System.IO.FileInfo SysFileInfo => new( this.FullPath );

		[NotNull]
		public override System.IO.FileSystemInfo SystemInfo => this.SysFileInfo;

		[NotNull]
		public DirectoryInfo Directory => new( this.DirectoryName );

		private Int64 GetFileLength() {
			if ( this.state == State.Uninitialized ) {
				this.Refresh();
			}

			if ( this.data is null ) {
				throw new IOException( $"Unable to obtain {nameof( FileAttributeData )} on {this.FullPath}." );
			}

			if ( this.state == State.Error ) {
				Common.ThrowIOError( this.errorCode, this.FullPath );
			}

			return ( ( Int64 )this.data.fileSizeHigh << 32 ) | ( this.data.fileSizeLow & 0xFFFFFFFFL );
		}

		[NotNull]
		public StreamWriter AppendText() => File.CreateStreamWriter( this.FullPath, true );

		[NotNull]
		public FileInfo CopyTo( [NotNull] String destFileName ) => this.CopyTo( destFileName, false );

		[NotNull]
		public FileInfo CopyTo( [NotNull] String destFileName, Boolean overwrite ) {
			File.Copy( this.FullPath, destFileName, overwrite );

			return new FileInfo( destFileName );
		}

		[NotNull]
		public FileStream Create() => File.Create( this.FullPath );

		[NotNull]
		public StreamWriter CreateText() => File.CreateStreamWriter( this.FullPath, false );

		public void Decrypt() => File.Decrypt( this.FullPath );

		public override void Delete() => File.Delete( this.FullPath );

		public void Encrypt() => File.Encrypt( this.FullPath );

		public void MoveTo( [NotNull] String destFileName ) => File.Move( this.FullPath, destFileName );

		[NotNull]
		public FileStream Open( FileMode mode ) => this.Open( mode, FileAccess.ReadWrite, FileShare.None );

		[NotNull]
		public FileStream Open( FileMode mode, FileAccess access ) => this.Open( mode, access, FileShare.None );

		[NotNull]
		public FileStream Open( FileMode mode, FileAccess access, FileShare share ) => File.Open( this.FullPath, mode, access, share, 4096, FileOptions.SequentialScan );

		[NotNull]
		public FileStream OpenRead() => File.Open( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None );

		[NotNull]
		public StreamReader OpenText() => File.CreateStreamReader( this.FullPath, Encoding.UTF8, true, 1024 );

		[NotNull]
		public FileStream OpenWrite() => File.Open( this.FullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None );

		[NotNull]
		public FileInfo Replace( [NotNull] String destinationFilename, [NotNull] String backupFilename ) => this.Replace( destinationFilename, backupFilename, false );

		[NotNull]
		public FileInfo Replace( [NotNull] String destinationFilename, [NotNull] String backupFilename, Boolean ignoreMetadataErrors ) {
			File.Replace( this.FullPath, destinationFilename, backupFilename, ignoreMetadataErrors );

			return new FileInfo( destinationFilename );
		}

		[NotNull]
		public override String ToString() => this.FullPath;

	}

}