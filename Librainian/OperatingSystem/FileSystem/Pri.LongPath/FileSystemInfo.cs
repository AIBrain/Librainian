// Copyright © Protiguous. All Rights Reserved.
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

namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.IO;
	
	using System.Runtime.InteropServices.ComTypes;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;

    public abstract class FileSystemInfo {

        [NotNull]
        public readonly String FullPath;

        [CanBeNull]
        protected FileAttributeData? data;

        protected Int32 errorCode;

        protected State state;

        // Summary:
        //     Gets or sets the attributes for the current file or directory.
        //
        // Returns:
        //     System.IO.FileAttributes of the current System.IO.FileSystemInfo.
        //
        // Exceptions:
        //   System.IO.FileNotFoundException:
        //     The specified file does not exist.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid; for example, it is on an unmapped drive.
        //
        //   System.Security.SecurityException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentException:
        //     The caller attempts to set an invalid file attribute. -or-The user attempts
        //     to set an attribute value but does not have write permission.
        //
        //   System.IO.IOException:
        //     System.IO.FileSystemInfo.Refresh() cannot initialize the data.
        public FileAttributes Attributes {
            get => this.FullPath.GetAttributes();
            set => this.FullPath.SetAttributes( value );
        }

        //
        // Summary:
        //     Gets or sets the creation time of the current file or directory.
        //
        // Returns:
        //     The creation date and time of the current System.IO.FileSystemInfo object.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     System.IO.FileSystemInfo.Refresh() cannot initialize the data.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid; for example, it is on an unmapped drive.
        //
        //   System.PlatformNotSupportedException:
        //     The current operating system is not Windows NT or later.
        //
        //   System.ArgumentOutOfRangeException:
        //     The caller attempts to set an invalid creation time.
        public DateTime CreationTime {
            get => this.CreationTimeUtc.ToLocalTime();

            set => this.CreationTimeUtc = value.ToUniversalTime();
        }

        //
        // Summary:
        //     Gets or sets the creation time, in coordinated universal time (UTC), of the
        //     current file or directory.
        //
        // Returns:
        //     The creation date and time in UTC format of the current System.IO.FileSystemInfo
        //     object.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     System.IO.FileSystemInfo.Refresh() cannot initialize the data.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid; for example, it is on an unmapped drive.
        //
        //   System.PlatformNotSupportedException:
        //     The current operating system is not Windows NT or later.
        //
        //   System.ArgumentOutOfRangeException:
        //     The caller attempts to set an invalid access time.
        public DateTime CreationTimeUtc {
            get {

                if ( this.state == State.Uninitialized ) {
                    this.Refresh();
                }

                if ( this.state == State.Error ) {
                    Common.ThrowIOError( this.errorCode, this.FullPath );
                }

                var fileTime = ( ( Int64 ) this.data.ftCreationTime.dwHighDateTime << 32 ) | ( this.data.ftCreationTime.dwLowDateTime & 0xffffffff );

                return DateTime.FromFileTimeUtc( fileTime );
            }

            set {

                if ( this is DirectoryInfo ) {
                    Directory.SetCreationTimeUtc( this.FullPath, value );
                }
                else {
                    File.SetCreationTimeUtc( this.FullPath, value );
                }

                this.state = State.Uninitialized;
            }
        }

        public abstract Boolean Exists { get; }

        [CanBeNull]
        public String Extension => this.FullPath.GetExtension();

        public DateTime LastAccessTime {
            get => this.LastAccessTimeUtc.ToLocalTime();
            set => this.LastAccessTimeUtc = value.ToUniversalTime();
        }

        public DateTime LastAccessTimeUtc {
            get {

                if ( this.state == State.Uninitialized ) {
                    this.Refresh();
                }

                if ( this.state == State.Error ) {
                    Common.ThrowIOError( this.errorCode, this.FullPath );
                }

                var fileTime = ( ( Int64 ) this.data.ftLastAccessTime.dwHighDateTime << 32 ) | ( this.data.ftLastAccessTime.dwLowDateTime & 0xffffffff );

                return DateTime.FromFileTimeUtc( fileTime );
            }

            set {

                if ( this is DirectoryInfo ) {
                    this.FullPath.SetLastAccessTimeUtc( value );
                }
                else {
                    File.SetLastAccessTimeUtc( this.FullPath, value );
                }

                this.state = State.Uninitialized;
            }
        }

        public DateTime LastWriteTime {
            get => this.LastWriteTimeUtc.ToLocalTime();
            set => this.LastWriteTimeUtc = value.ToUniversalTime();
        }

        public DateTime LastWriteTimeUtc {
            get {

                if ( this.state == State.Uninitialized ) {
                    this.Refresh();
                }

                if ( this.state == State.Error ) {
                    ThrowLastWriteTimeUtcIOError( this.errorCode, this.FullPath );
                }

                var fileTime = ( ( Int64 ) this.data.ftLastWriteTime.dwHighDateTime << 32 ) | ( this.data.ftLastWriteTime.dwLowDateTime & 0xffffffff );

                return DateTime.FromFileTimeUtc( fileTime );
            }

            set {

                if ( this is DirectoryInfo ) {
                    this.FullPath.SetLastWriteTimeUtc( value ); //which is better?
                }
                else {
                    File.SetLastWriteTimeUtc( this.FullPath, value );
                }

                this.state = State.Uninitialized;
            }
        }

        public abstract String Name { get; }

        public abstract System.IO.FileSystemInfo SystemInfo { get; }

        protected FileSystemInfo( [NotNull] String fullPath ) => this.FullPath = fullPath.ThrowIfBlank();

        protected enum State {

            Uninitialized,

            Initialized,

            Error

        }

        private static void ThrowLastWriteTimeUtcIOError( Int32 errorCode, [NotNull] String maybeFullPath ) {

            // This doesn't have to be perfect, but is a perf optimization.
            var isInvalidPath = errorCode == NativeMethods.ERROR_INVALID_NAME || errorCode == NativeMethods.ERROR_BAD_PATHNAME;
            var str = isInvalidPath ? maybeFullPath.GetFileName() : maybeFullPath;

            switch ( errorCode ) {
                case NativeMethods.ERROR_FILE_NOT_FOUND: break;

                case NativeMethods.ERROR_PATH_NOT_FOUND: break;

                case NativeMethods.ERROR_ACCESS_DENIED:

                    if ( str.Length == 0 ) {
                        throw new UnauthorizedAccessException( "Empty path" );
                    }
                    else {
                        throw new UnauthorizedAccessException( $"Access denied accessing {str}" );
                    }

                case NativeMethods.ERROR_ALREADY_EXISTS:

                    if ( str.Length == 0 ) {
                        goto default;
                    }

                    throw new IOException( $"File {str}", NativeMethods.MakeHRFromErrorCode( errorCode ) );

                case NativeMethods.ERROR_FILENAME_EXCED_RANGE: throw new PathTooLongException( "Path too long" );

                case NativeMethods.ERROR_INVALID_DRIVE: throw new DriveNotFoundException( $"Drive {str} not found" );

                case NativeMethods.ERROR_INVALID_PARAMETER: throw new IOException( NativeMethods.GetMessage( errorCode ), NativeMethods.MakeHRFromErrorCode( errorCode ) );

                case NativeMethods.ERROR_SHARING_VIOLATION:

                    if ( str.Length == 0 ) {
                        throw new IOException( "Sharing violation with empty filename", NativeMethods.MakeHRFromErrorCode( errorCode ) );
                    }
                    else {
                        throw new IOException( $"Sharing violation: {str}", NativeMethods.MakeHRFromErrorCode( errorCode ) );
                    }

                case NativeMethods.ERROR_FILE_EXISTS:

                    if ( str.Length == 0 ) {
                        goto default;
                    }

                    throw new IOException( $"File exists {str}", NativeMethods.MakeHRFromErrorCode( errorCode ) );

                case NativeMethods.ERROR_OPERATION_ABORTED: throw new OperationCanceledException();

                default: throw new IOException( NativeMethods.GetMessage( errorCode ), NativeMethods.MakeHRFromErrorCode( errorCode ) );
            }
        }

        public abstract void Delete();

        // ReSharper disable once UnusedParameter.Global
        public virtual void GetObjectData( [NotNull] SerializationInfo info, StreamingContext context ) =>
            info.AddValue( nameof( this.FullPath ), this.FullPath, typeof( String ) );

        public void Refresh() {
            try {
                this.data = default;

                // TODO: BeginFind fails on "\\?\c:\"

                var normalizedPathWithSearchPattern = this.FullPath.NormalizeLongPath();

                using var handle = Directory.BeginFind( normalizedPathWithSearchPattern, out var findData );

                this.data = new FileAttributeData( findData );
                this.state = State.Initialized;

            }
            catch ( DirectoryNotFoundException ) {
                this.state = State.Error;
                this.errorCode = NativeMethods.ERROR_PATH_NOT_FOUND;
            }
            catch ( Exception ) {
                if ( this.state != State.Error ) {
                    Common.ThrowIOError( System.Runtime.InteropServices.Marshal.GetLastWin32Error(), this.FullPath );
                }
            }
        }

        protected class FileAttributeData {

            public readonly FileAttributes fileAttributes;

            public readonly Int32 fileSizeHigh;

            public readonly Int32 fileSizeLow;

            public FILETIME ftCreationTime;

            public FILETIME ftLastAccessTime;

            public FILETIME ftLastWriteTime;

            public FileAttributeData( WIN32_FIND_DATA findData ) {
                this.fileAttributes = findData.dwFileAttributes;
                this.ftCreationTime = findData.ftCreationTime;
                this.ftLastAccessTime = findData.ftLastAccessTime;
                this.ftLastWriteTime = findData.ftLastWriteTime;
                this.fileSizeHigh = findData.nFileSizeHigh;
                this.fileSizeLow = findData.nFileSizeLow;
            }

            public FileAttributeData() { }

        }

    }

}