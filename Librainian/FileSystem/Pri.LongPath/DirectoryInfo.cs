// Copyright � Protiguous. All Rights Reserved.
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
// File "DirectoryInfo.cs" last formatted on 2020-08-14 at 8:39 PM.

namespace Librainian.FileSystem.Pri.LongPath {

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using JetBrains.Annotations;

	public class DirectoryInfo : FileSystemInfo {

		public DirectoryInfo( [NotNull] String path ) : base( path.GetFullPath() ) => this.Name = path.Length != 2 || path[1] != ':' ? GetDirName( this.FullPath ) : ".";

		[NotNull]
		private System.IO.DirectoryInfo SysDirectoryInfo => new( this.FullPath );

		public override Boolean Exists {
			get {
				if ( this.state == State.Uninitialized ) {
					this.Refresh();
				}

				return this.state == State.Initialized && this.data?.fileAttributes.HasFlag( FileAttributes.Directory ) == true;
			}
		}

		public override String Name { get; }

		[NotNull]
		public DirectoryInfo Parent {
			get {
				var fullPath = this.FullPath;

				if ( fullPath.Length > 3 && fullPath.EndsWith( Path.DirectorySeparatorChar ) ) {
					fullPath = this.FullPath[ ..^1 ];
				}

				var directoryName = fullPath.GetDirectoryName();

				return new DirectoryInfo( directoryName );
			}
		}

		[NotNull]
		public DirectoryInfo Root {
			get {
				var rootLength = this.FullPath.GetRootLength();
				var str = this.FullPath.Substring( 0, rootLength - ( this.FullPath.IsPathUnc() ? 1 : 0 ) );

				return new DirectoryInfo( str );
			}
		}

		[NotNull]
		public override System.IO.FileSystemInfo SystemInfo => this.SysDirectoryInfo;

		[NotNull]
		public static String GetDirName( [NotNull] String fullPath ) {
			fullPath = fullPath.ThrowIfBlank();

			if ( fullPath.Length <= 3 ) {
				return fullPath;
			}

			var s = fullPath;

			if ( s.EndsWith( Path.DirectorySeparatorChar ) ) {
				s = s[ ..^1 ];
			}

			return s.GetFileName();
		}

		public void Create() => this.FullPath.CreateDirectory();

		[NotNull]
		public DirectoryInfo CreateSubdirectory( [NotNull] String path ) {
			var newDir = this.FullPath.Combine( path );
			var newFullPath = newDir.GetFullPath();

			if ( String.Compare( this.FullPath, 0, newFullPath, 0, this.FullPath.Length, StringComparison.OrdinalIgnoreCase ) != 0 ) {
				throw new ArgumentException( "Invalid subpath", path );
			}

			newDir.CreateDirectory();

			return new DirectoryInfo( newDir );
		}

		public override void Delete() => Directory.Delete( this.FullPath );

		public void Delete( Boolean recursive ) => Directory.Delete( this.FullPath, recursive );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<DirectoryInfo> EnumerateDirectories( [NotNull] String searchPattern ) =>
			Directory.EnumerateFileSystemEntries( this.FullPath, searchPattern, true, false, SearchOption.TopDirectoryOnly )
					 .Select( directory => new DirectoryInfo( directory ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<DirectoryInfo> EnumerateDirectories( [NotNull] String searchPattern, SearchOption searchOption ) =>
			Directory.EnumerateFileSystemEntries( this.FullPath, searchPattern, true, false, searchOption ).Select( directory => new DirectoryInfo( directory ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<DirectoryInfo> EnumerateDirectories() =>
			Directory.EnumerateFileSystemEntries( this.FullPath, "*", true, false, SearchOption.TopDirectoryOnly ).Select( directory => new DirectoryInfo( directory ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileInfo> EnumerateFiles() => Directory.EnumerateFiles( this.FullPath ).Select( e => new FileInfo( e ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileInfo> EnumerateFiles( [NotNull] String searchPattern ) =>
			Directory.EnumerateFileSystemEntries( this.FullPath, searchPattern, false, true, SearchOption.TopDirectoryOnly ).Select( e => new FileInfo( e ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileInfo> EnumerateFiles( [NotNull] String searchPattern, SearchOption searchOption ) =>
			Directory.EnumerateFileSystemEntries( this.FullPath, searchPattern, false, true, searchOption ).Select( e => new FileInfo( e ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos() =>
			Directory.EnumerateFileSystemEntries( this.FullPath ).Select( e => {
				if ( e.Exists() ) {
					return new DirectoryInfo( e );
				}

				return (FileSystemInfo)new FileInfo( e );
			} );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos( [NotNull] String searchPattern ) =>
			Directory.EnumerateFileSystemEntries( this.FullPath, searchPattern, true, true, SearchOption.TopDirectoryOnly )
					 .Select( e => e.Exists() ? new DirectoryInfo( e ) : ( FileSystemInfo )new FileInfo( e ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos( [NotNull] String searchPattern, SearchOption searchOption ) =>
			Directory.EnumerateFileSystemEntries( this.FullPath, searchPattern, searchOption )
					 .Select( e => e.Exists() ? new DirectoryInfo( e ) : ( FileSystemInfo )new FileInfo( e ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<DirectoryInfo> GetDirectories() => Directory.GetDirectories( this.FullPath ).Select( path => new DirectoryInfo( path ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<DirectoryInfo> GetDirectories( [NotNull] String searchPattern ) =>
			Directory.GetDirectories( this.FullPath, searchPattern ).Select( path => new DirectoryInfo( path ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<DirectoryInfo> GetDirectories( [NotNull] String searchPattern, SearchOption searchOption ) =>
			this.FullPath.GetDirectories( searchPattern, searchOption ).Select( path => new DirectoryInfo( path ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileInfo> GetFiles( [NotNull] String searchPattern ) => Directory.GetFiles( this.FullPath, searchPattern ).Select( path => new FileInfo( path ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileInfo> GetFiles( [NotNull] String searchPattern, SearchOption searchOption ) =>
			Directory.GetFiles( this.FullPath, searchPattern, searchOption ).Select( path => new FileInfo( path ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileInfo> GetFiles() =>
			Directory.EnumerateFileSystemEntries( this.FullPath, "*", false, true, SearchOption.TopDirectoryOnly ).Select( path => new FileInfo( path ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileSystemInfo> GetFileSystemInfos( [NotNull] String searchPattern ) =>
			Directory.EnumerateFileSystemEntries( this.FullPath, searchPattern, true, true, SearchOption.TopDirectoryOnly )
					 .Select( e => e.Exists() ? new DirectoryInfo( e ) : ( FileSystemInfo )new FileInfo( e ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileSystemInfo> GetFileSystemInfos( [NotNull] String searchPattern, SearchOption searchOption ) =>
			Directory.EnumerateFileSystemEntries( this.FullPath, searchPattern, true, true, searchOption )
					 .Select( e => e.Exists() ? new DirectoryInfo( e ) : ( FileSystemInfo )new FileInfo( e ) );

		[NotNull]
		[ItemNotNull]
		public IEnumerable<FileSystemInfo> GetFileSystemInfos() =>
			Directory.EnumerateFileSystemEntries( this.FullPath, "*", true, true, SearchOption.TopDirectoryOnly )
					 .Select( e => e.Exists() ? new DirectoryInfo( e ) : ( FileSystemInfo )new FileInfo( e ) );

		public void MoveTo( [NotNull] String destDirName ) {
			var fullDestDirName = destDirName.ThrowIfBlank().GetFullPath();

			if ( !fullDestDirName.EndsWith( Path.DirectorySeparatorChar ) ) {
				fullDestDirName += Path.DirectorySeparatorChar;
			}

			String fullSourcePath;

			if ( this.FullPath.EndsWith( Path.DirectorySeparatorChar ) ) {
				fullSourcePath = this.FullPath;
			}
			else {
				fullSourcePath = this.FullPath + Path.DirectorySeparatorChar;
			}

			if ( String.Compare( fullSourcePath, fullDestDirName, StringComparison.OrdinalIgnoreCase ) == 0 ) {
				throw new IOException( "source and destination directories must be different" );
			}

			var sourceRoot = fullSourcePath.GetPathRoot();
			var destinationRoot = fullDestDirName.GetPathRoot();

			if ( String.Compare( sourceRoot, destinationRoot, StringComparison.OrdinalIgnoreCase ) != 0 ) {
				throw new IOException( "Source and destination directories must have same root" );
			}

			File.Move( fullSourcePath, fullDestDirName );
		}

		[NotNull]
		public override String ToString() => this.FullPath;

	}

}