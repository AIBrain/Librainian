// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DirectoryInfo.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "DirectoryInfo.cs" was last formatted by Protiguous on 2020/03/16 at 3:08 PM.

namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using JetBrains.Annotations;

    public class DirectoryInfo : FileSystemInfo {

        [NotNull]
        private System.IO.DirectoryInfo SysDirectoryInfo => new System.IO.DirectoryInfo( path: this.FullPath );

        public override Boolean Exists {
            get {
                if ( this.state == State.Uninitialized ) {
                    this.Refresh();
                }

                return this.state == State.Initialized && this.data?.fileAttributes.HasFlag( flag: FileAttributes.Directory ) == true;
            }
        }

        public override String Name { get; }

        [NotNull]
        public DirectoryInfo Parent {
            get {
                var fullPath = this.FullPath;

                if ( fullPath.Length > 3 && fullPath.EndsWith( value: Path.DirectorySeparatorChar ) ) {
                    fullPath = this.FullPath.Substring( startIndex: 0, length: this.FullPath.Length - 1 );
                }

                var directoryName = fullPath.GetDirectoryName();

                return new DirectoryInfo( path: directoryName );
            }
        }

        [NotNull]
        public DirectoryInfo Root {
            get {
                var rootLength = this.FullPath.GetRootLength();
                var str = this.FullPath.Substring( startIndex: 0, length: rootLength - ( this.FullPath.IsPathUnc() ? 1 : 0 ) );

                return new DirectoryInfo( path: str );
            }
        }

        [NotNull]
        public override System.IO.FileSystemInfo SystemInfo => this.SysDirectoryInfo;

        public DirectoryInfo( [NotNull] String path ) : base( fullPath: path.GetFullPath() ) =>
            this.Name = path.Length != 2 || path[ index: 1 ] != ':' ? GetDirName( fullPath: this.FullPath ) : ".";

        [NotNull]
        public static String GetDirName( [NotNull] String fullPath ) {
            fullPath = fullPath.ThrowIfBlank();

            if ( fullPath.Length <= 3 ) {
                return fullPath;
            }

            var s = fullPath;

            if ( s.EndsWith( value: Path.DirectorySeparatorChar ) ) {
                s = s.Substring( startIndex: 0, length: s.Length - 1 );
            }

            return s.GetFileName();
        }

        public void Create() => this.FullPath.CreateDirectory();

        [NotNull]
        public DirectoryInfo CreateSubdirectory( [NotNull] String path ) {
            var newDir = this.FullPath.Combine( path2: path );
            var newFullPath = newDir.GetFullPath();

            if ( String.Compare( strA: this.FullPath, indexA: 0, strB: newFullPath, indexB: 0, length: this.FullPath.Length,
                comparisonType: StringComparison.OrdinalIgnoreCase ) != 0 ) {
                throw new ArgumentException( message: "Invalid subpath", paramName: path );
            }

            newDir.CreateDirectory();

            return new DirectoryInfo( path: newDir );
        }

        public override void Delete() => Directory.Delete( path: this.FullPath );

        public void Delete( Boolean recursive ) => Directory.Delete( path: this.FullPath, recursive: recursive );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<DirectoryInfo> EnumerateDirectories( [NotNull] String searchPattern ) =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath, searchPattern: searchPattern, includeDirectories: true, includeFiles: false,
                option: SearchOption.TopDirectoryOnly ).Select( selector: directory => new DirectoryInfo( path: directory ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<DirectoryInfo> EnumerateDirectories( [NotNull] String searchPattern, SearchOption searchOption ) =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath, searchPattern: searchPattern, includeDirectories: true, includeFiles: false, option: searchOption )
                     .Select( selector: directory => new DirectoryInfo( path: directory ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<DirectoryInfo> EnumerateDirectories() =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath, searchPattern: "*", includeDirectories: true, includeFiles: false,
                option: SearchOption.TopDirectoryOnly ).Select( selector: directory => new DirectoryInfo( path: directory ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileInfo> EnumerateFiles() => Directory.EnumerateFiles( path: this.FullPath ).Select( selector: e => new FileInfo( fileName: e ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileInfo> EnumerateFiles( [NotNull] String searchPattern ) =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath, searchPattern: searchPattern, includeDirectories: false, includeFiles: true,
                option: SearchOption.TopDirectoryOnly ).Select( selector: e => new FileInfo( fileName: e ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileInfo> EnumerateFiles( [NotNull] String searchPattern, SearchOption searchOption ) =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath, searchPattern: searchPattern, includeDirectories: false, includeFiles: true, option: searchOption )
                     .Select( selector: e => new FileInfo( fileName: e ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos() =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath )
                     .Select( selector: e => e.Exists() ? new DirectoryInfo( path: e ) : ( FileSystemInfo ) new FileInfo( fileName: e ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos( [NotNull] String searchPattern ) =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath, searchPattern: searchPattern, includeDirectories: true, includeFiles: true,
                         option: SearchOption.TopDirectoryOnly )
                     .Select( selector: e => e.Exists() ? new DirectoryInfo( path: e ) : ( FileSystemInfo ) new FileInfo( fileName: e ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos( [NotNull] String searchPattern, SearchOption searchOption ) =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath, searchPattern: searchPattern, options: searchOption ).Select( selector: e =>
                e.Exists() ? new DirectoryInfo( path: e ) : ( FileSystemInfo ) new FileInfo( fileName: e ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<DirectoryInfo> GetDirectories() => Directory.GetDirectories( path: this.FullPath ).Select( selector: path => new DirectoryInfo( path: path ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<DirectoryInfo> GetDirectories( [NotNull] String searchPattern ) =>
            Directory.GetDirectories( path: this.FullPath, searchPattern: searchPattern ).Select( selector: path => new DirectoryInfo( path: path ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<DirectoryInfo> GetDirectories( [NotNull] String searchPattern, SearchOption searchOption ) =>
            this.FullPath.GetDirectories( searchPattern: searchPattern, searchOption: searchOption ).Select( selector: path => new DirectoryInfo( path: path ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileInfo> GetFiles( [NotNull] String searchPattern ) =>
            Directory.GetFiles( path: this.FullPath, searchPattern: searchPattern ).Select( selector: path => new FileInfo( fileName: path ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileInfo> GetFiles( [NotNull] String searchPattern, SearchOption searchOption ) =>
            Directory.GetFiles( path: this.FullPath, searchPattern: searchPattern, options: searchOption ).Select( selector: path => new FileInfo( fileName: path ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileInfo> GetFiles() =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath, searchPattern: "*", includeDirectories: false, includeFiles: true,
                option: SearchOption.TopDirectoryOnly ).Select( selector: path => new FileInfo( fileName: path ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileSystemInfo> GetFileSystemInfos( [NotNull] String searchPattern ) =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath, searchPattern: searchPattern, includeDirectories: true, includeFiles: true,
                         option: SearchOption.TopDirectoryOnly )
                     .Select( selector: e => e.Exists() ? new DirectoryInfo( path: e ) : ( FileSystemInfo ) new FileInfo( fileName: e ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileSystemInfo> GetFileSystemInfos( [NotNull] String searchPattern, SearchOption searchOption ) =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath, searchPattern: searchPattern, includeDirectories: true, includeFiles: true, option: searchOption )
                     .Select( selector: e => e.Exists() ? new DirectoryInfo( path: e ) : ( FileSystemInfo ) new FileInfo( fileName: e ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileSystemInfo> GetFileSystemInfos() =>
            Directory.EnumerateFileSystemEntries( path: this.FullPath, searchPattern: "*", includeDirectories: true, includeFiles: true,
                         option: SearchOption.TopDirectoryOnly )
                     .Select( selector: e => e.Exists() ? new DirectoryInfo( path: e ) : ( FileSystemInfo ) new FileInfo( fileName: e ) );

        public void MoveTo( [NotNull] String destDirName ) {

            var fullDestDirName = destDirName.ThrowIfBlank().GetFullPath();

            if ( !fullDestDirName.EndsWith( value: Path.DirectorySeparatorChar ) ) {
                fullDestDirName += Path.DirectorySeparatorChar;
            }

            String fullSourcePath;

            if ( this.FullPath.EndsWith( value: Path.DirectorySeparatorChar ) ) {
                fullSourcePath = this.FullPath;
            }
            else {
                fullSourcePath = this.FullPath + Path.DirectorySeparatorChar;
            }

            if ( String.Compare( strA: fullSourcePath, strB: fullDestDirName, comparisonType: StringComparison.OrdinalIgnoreCase ) == 0 ) {
                throw new IOException( message: "source and destination directories must be different" );
            }

            var sourceRoot = fullSourcePath.GetPathRoot();
            var destinationRoot = fullDestDirName.GetPathRoot();

            if ( String.Compare( strA: sourceRoot, strB: destinationRoot, comparisonType: StringComparison.OrdinalIgnoreCase ) != 0 ) {
                throw new IOException( message: "Source and destination directories must have same root" );
            }

            File.Move( sourcePath: fullSourcePath, destinationPath: fullDestDirName );
        }

        [NotNull]
        public override String ToString() => this.FullPath;

    }

}