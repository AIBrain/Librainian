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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "FileSystemInfo.cs" last touched on 2021-03-07 at 9:52 AM by Protiguous.

#nullable enable

namespace Librainian.FileSystem.Pri.LongPath {

	using System;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;
	using System.Runtime.Serialization;
	using JetBrains.Annotations;

	public abstract class FileSystemInfo {

		[NotNull]
		public readonly String FullPath;

		[CanBeNull]
		protected FileAttributeData? Data;

		protected PriNativeMethods.ERROR ErrorCode;

		protected State state;

		protected FileSystemInfo( [NotNull] String fullPath ) => this.FullPath = fullPath.ThrowIfBlank();

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
				if ( this.state == State.Uninitialized || this.Data is null ) {
					this.Refresh();
				}

				if ( this.state == State.Error ) {
					Common.ThrowIOError( this.ErrorCode, this.FullPath );
				}

				var fileTime = ( ( Int64 ) this.Data!.CreationTime.dwHighDateTime << 32 ) | ( this.Data.CreationTime.dwLowDateTime & 0xffffffff );

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
				if ( this.state == State.Uninitialized || this.Data is null ) {
					this.Refresh();
				}

				if ( this.state == State.Error ) {
					Common.ThrowIOError( this.ErrorCode, this.FullPath );
				}

				var fileTime = ( ( Int64 ) this.Data!.LastAccessTime.dwHighDateTime << 32 ) | ( this.Data.LastAccessTime.dwLowDateTime & 0xffffffff );

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
				if ( this.state == State.Uninitialized || this.Data is null ) {
					this.Refresh();
				}

				if ( this.state == State.Error ) {
					ThrowLastWriteTimeUtcIOError( this.ErrorCode, this.FullPath );
				}

				var fileTime = ( ( Int64 ) this.Data!.LastWriteTime.dwHighDateTime << 32 ) | ( this.Data.LastWriteTime.dwLowDateTime & 0xffffffff );

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

		private static void ThrowLastWriteTimeUtcIOError( PriNativeMethods.ERROR errorCode, [NotNull] String maybeFullPath ) {
			// This doesn't have to be perfect, but is a perf optimization.
			var isInvalidPath = errorCode is PriNativeMethods.ERROR.ERROR_INVALID_NAME or PriNativeMethods.ERROR.ERROR_BAD_PATHNAME;
			var str = isInvalidPath ? maybeFullPath.GetFileName() : maybeFullPath;

			switch ( errorCode ) {
				case PriNativeMethods.ERROR.ERROR_FILE_NOT_FOUND:
					break;

				case PriNativeMethods.ERROR.ERROR_PATH_NOT_FOUND:
					break;

				case PriNativeMethods.ERROR.ERROR_ACCESS_DENIED:

					if ( str.Length == 0 ) {
						throw new UnauthorizedAccessException( "Empty path" );
					}
					else {
						throw new UnauthorizedAccessException( $"Access denied accessing {str}" );
					}

				case PriNativeMethods.ERROR.ERROR_ALREADY_EXISTS:

					if ( str.Length == 0 ) {
						goto default;
					}

					throw new IOException( $"File {str}", PriNativeMethods.MakeHRFromErrorCode( errorCode ) );

				case PriNativeMethods.ERROR.ERROR_FILENAME_EXCED_RANGE:
					throw new PathTooLongException( "Path too long" );

				case PriNativeMethods.ERROR.ERROR_INVALID_DRIVE:
					throw new DriveNotFoundException( $"Drive {str} not found" );

				case PriNativeMethods.ERROR.ERROR_INVALID_PARAMETER:
					throw new IOException( PriNativeMethods.GetMessage( errorCode ), PriNativeMethods.MakeHRFromErrorCode( errorCode ) );

				case PriNativeMethods.ERROR.ERROR_SHARING_VIOLATION:

					if ( str.Length == 0 ) {
						throw new IOException( "Sharing violation with empty filename", PriNativeMethods.MakeHRFromErrorCode( errorCode ) );
					}
					else {
						throw new IOException( $"Sharing violation: {str}", PriNativeMethods.MakeHRFromErrorCode( errorCode ) );
					}

				case PriNativeMethods.ERROR.ERROR_FILE_EXISTS:

					if ( str.Length == 0 ) {
						goto default;
					}

					throw new IOException( $"File exists {str}", PriNativeMethods.MakeHRFromErrorCode( errorCode ) );

				case PriNativeMethods.ERROR.ERROR_OPERATION_ABORTED:
					throw new OperationCanceledException();

				default:
					throw new IOException( PriNativeMethods.GetMessage( errorCode ), PriNativeMethods.MakeHRFromErrorCode( errorCode ) );
			}
		}

		public abstract void Delete();

		public virtual void GetObjectData( [NotNull] SerializationInfo info, StreamingContext context ) =>
			info.AddValue( nameof( this.FullPath ), this.FullPath, typeof( String ) );

		public void Refresh() {
			try {
				this.Data = null;

				//BUG BeginFind fails on "\\?\c:\"

				var normalizedPathWithSearchPattern = this.FullPath.NormalizeLongPath();

				using var handle = Directory.BeginFind( normalizedPathWithSearchPattern, out var findData );

				this.Data = new FileAttributeData( findData );
				this.state = State.Initialized;

				
			}
			catch ( DirectoryNotFoundException ) {
				this.state = State.Error;
				this.ErrorCode = PriNativeMethods.ERROR.ERROR_PATH_NOT_FOUND;
			}
			catch ( Exception ) {
				if ( this.state != State.Error ) {
					Common.ThrowIOError( ( PriNativeMethods.ERROR ) Marshal.GetLastWin32Error(), this.FullPath );
				}
			}
		}

		protected enum State {

			Uninitialized,

			Initialized,

			Error

		}

		protected class FileAttributeData {

			public readonly FileAttributes FileAttributes;

			public readonly Int32 FileSizeHigh;

			public readonly Int32 FileSizeLow;

			public FILETIME CreationTime;

			public FILETIME LastAccessTime;

			public FILETIME LastWriteTime;

			public Boolean? Exists { get; }
			

			public FileAttributeData( WIN32_FIND_DATA findData ) {
				this.FileAttributes = findData.dwFileAttributes;
				this.CreationTime = findData.ftCreationTime;
				this.LastAccessTime = findData.ftLastAccessTime;
				this.LastWriteTime = findData.ftLastWriteTime;
				this.FileSizeHigh = findData.nFileSizeHigh;
				this.FileSizeLow = findData.nFileSizeLow;
				this.Exists = findData.Exists;
			}

			private FileAttributeData() { }

		}

	}

}