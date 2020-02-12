namespace LibrainianCore.OperatingSystem.FileSystem.Pri.LongPath {

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

                return fileAttributeData != null && ( this.state == State.Initialized && ( fileAttributeData.fileAttributes & FileAttributes.Directory ) != FileAttributes.Directory );
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
        public System.IO.FileInfo SysFileInfo => new System.IO.FileInfo( this.FullPath );

        [NotNull]
        public override System.IO.FileSystemInfo SystemInfo => this.SysFileInfo;

        [NotNull]
        public DirectoryInfo Directory => new DirectoryInfo( this.DirectoryName );

        public FileInfo( [NotNull] String fileName ) : base(fileName.GetFullPath()) => this.Name = this.FullPath.GetFileName();

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