// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "FileInfo.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "FileInfo.cs" was last formatted by Protiguous on 2020/03/16 at 3:09 PM.

namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.IO;
    using System.Text;
    using JetBrains.Annotations;

    public class FileInfo : FileSystemInfo {

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
            get => ( this.Attributes & FileAttributes.ReadOnly ) != 0;

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
        public System.IO.FileInfo SysFileInfo => new System.IO.FileInfo( fileName: this.FullPath );

        [NotNull]
        public override System.IO.FileSystemInfo SystemInfo => this.SysFileInfo;

        [NotNull]
        public DirectoryInfo Directory => new DirectoryInfo( path: this.DirectoryName );

        public FileInfo( [NotNull] String fileName ) : base( fullPath: fileName.GetFullPath() ) => this.Name = this.FullPath.GetFileName();

        private Int64 GetFileLength() {
            if ( this.state == State.Uninitialized ) {
                this.Refresh();
            }

            if ( this.data is null ) {
                throw new IOException( message: $"Unable to obtain {nameof( FileAttributeData )} on {this.FullPath}." );
            }

            if ( this.state == State.Error ) {
                Common.ThrowIOError( errorCode: this.errorCode, maybeFullPath: this.FullPath );
            }

            return ( ( Int64 ) this.data.fileSizeHigh << 32 ) | ( this.data.fileSizeLow & 0xFFFFFFFFL );
        }

        [NotNull]
        public StreamWriter AppendText() => File.CreateStreamWriter( path: this.FullPath, append: true );

        [NotNull]
        public FileInfo CopyTo( [NotNull] String destFileName ) => this.CopyTo( destFileName: destFileName, overwrite: false );

        [NotNull]
        public FileInfo CopyTo( [NotNull] String destFileName, Boolean overwrite ) {
            File.Copy( sourcePath: this.FullPath, destinationPath: destFileName, overwrite: overwrite );

            return new FileInfo( fileName: destFileName );
        }

        [NotNull]
        public FileStream Create() => File.Create( path: this.FullPath );

        [NotNull]
        public StreamWriter CreateText() => File.CreateStreamWriter( path: this.FullPath, append: false );

        public void Decrypt() => File.Decrypt( path: this.FullPath );

        public override void Delete() => File.Delete( path: this.FullPath );

        public void Encrypt() => File.Encrypt( path: this.FullPath );

        public void MoveTo( [NotNull] String destFileName ) => File.Move( sourcePath: this.FullPath, destinationPath: destFileName );

        [NotNull]
        public FileStream Open( FileMode mode ) => this.Open( mode: mode, access: FileAccess.ReadWrite, share: FileShare.None );

        [NotNull]
        public FileStream Open( FileMode mode, FileAccess access ) => this.Open( mode: mode, access: access, share: FileShare.None );

        [NotNull]
        public FileStream Open( FileMode mode, FileAccess access, FileShare share ) =>
            File.Open( path: this.FullPath, mode: mode, access: access, share: share, bufferSize: 4096, options: FileOptions.SequentialScan );

        [NotNull]
        public FileStream OpenRead() =>
            File.Open( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: 4096, options: FileOptions.None );

        [NotNull]
        public StreamReader OpenText() => File.CreateStreamReader( path: this.FullPath, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024 );

        [NotNull]
        public FileStream OpenWrite() => File.Open( path: this.FullPath, mode: FileMode.OpenOrCreate, access: FileAccess.Write, share: FileShare.None );

        [NotNull]
        public FileInfo Replace( [NotNull] String destinationFilename, [NotNull] String backupFilename ) =>
            this.Replace( destinationFilename: destinationFilename, backupFilename: backupFilename, ignoreMetadataErrors: false );

        [NotNull]
        public FileInfo Replace( [NotNull] String destinationFilename, [NotNull] String backupFilename, Boolean ignoreMetadataErrors ) {
            File.Replace( sourceFileName: this.FullPath, destinationFileName: destinationFilename, destinationBackupFileName: backupFilename,
                ignoreMetadataErrors: ignoreMetadataErrors );

            return new FileInfo( fileName: destinationFilename );
        }

        [NotNull]
        public override String ToString() => this.FullPath;

    }

}