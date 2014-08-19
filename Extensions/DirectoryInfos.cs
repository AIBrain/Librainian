#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/DirectoryInfos.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using Annotations;
    using NUnit.Framework;
    using Threading;

    public static class DirectoryInfos {
        public static readonly HashSet< DirectoryInfo > SystemFolders = new HashSet< DirectoryInfo >();

        /// <summary>
        ///     No guarantee of return order. Also, because of the way the operating system works
        ///     (random-access), a directory may be created or deleted even after a search.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static IEnumerable< DirectoryInfo > BetterEnumerateDirectories( this DirectoryInfo target, String searchPattern = "*" ) {
            if ( null == target ) {
                yield break;
            }
            var searchPath = Path.Combine( target.FullName, searchPattern );
            NativeWin32.Win32FindData findData;
            using ( var hFindFile = NativeWin32.FindFirstFile( searchPath, out findData ) ) {
                do {
                    if ( hFindFile.IsInvalid ) {
                        break;
                    }

                    if ( IsParentOrCurrent( findData ) ) {
                        continue;
                    }

                    if ( IsReparsePoint( findData ) ) {
                        continue;
                    }

                    if ( !IsDirectory( findData ) ) {
                        continue;
                    }

                    if ( IsIgnoreFolder( findData ) ) {
                        continue;
                    }

                    var subFolder = Path.Combine( target.FullName, findData.cFileName );

                    // @"\\?\" +System.IO.PathTooLongException
                    if ( subFolder.Length >= 260 ) {
                        continue; //HACK
                    }

                    var subInfo = new DirectoryInfo( subFolder );

                    if ( IsProtected( subInfo ) ) {
                        continue;
                    }

                    yield return subInfo;

                    foreach ( var info in subInfo.BetterEnumerateDirectories( searchPattern ) ) {
                        yield return info;
                    }
                } while ( NativeWin32.FindNextFile( hFindFile, out findData ) );
            }
        }

        public static IEnumerable< FileInfo > BetterEnumerateFiles( [NotNull] this DirectoryInfo target, [NotNull] String searchPattern = "*" ) {
            if ( target == null ) {
                throw new ArgumentNullException( "target" );
            }
            if ( searchPattern == null ) {
                throw new ArgumentNullException( "searchPattern" );
            }

            //if ( null == target ) {
            //    yield break;
            //}
            var searchPath = Path.Combine( target.FullName, searchPattern );
            NativeWin32.Win32FindData findData;
            using ( var hFindFile = NativeWin32.FindFirstFile( searchPath, out findData ) ) {
                do {
                    //Application.DoEvents();

                    if ( hFindFile.IsInvalid ) {
                        break;
                    }

                    if ( IsParentOrCurrent( findData ) ) {
                        continue;
                    }
                    if ( IsReparsePoint( findData ) ) {
                        continue;
                    }

                    if ( !IsFile( findData ) ) {
                        continue;
                    }

                    var newfName = Path.Combine( target.FullName, findData.cFileName );
                    yield return new FileInfo( newfName );
                } while ( NativeWin32.FindNextFile( hFindFile, out findData ) );
            }
        }

        /// <summary>
        ///     Before: @"c:\hello\world".
        ///     After: @"c:\hello\world\23468923475634836.extension"
        /// </summary>
        /// <param name="info"></param>
        /// <param name="withExtension"></param>
        /// <param name="toBase"></param>
        /// <returns></returns>
        public static FileInfo DateAndTimeAsFile( this DirectoryInfo info, String withExtension, int toBase = 16 ) {
            if ( info == null ) {
                throw new ArgumentNullException( "info" );
            }

            var now = Convert.ToString( value: DateTime.UtcNow.ToBinary(), toBase: toBase );
            var fileName = String.Format( "{0}{1}", now, withExtension ?? info.Extension );
            var path = Path.Combine( info.FullName, fileName );
            return new FileInfo( path );
        }

        /// <summary>
        ///     If the <paramref name="directoryInfo" /> does not exist, attempt to create it.
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="changeCompressionTo">
        ///     Suggest if folder comperssion be Enabled or Disabled. Defaults to null.
        /// </param>
        /// <param name="requestReadAccess"></param>
        /// <param name="requestWriteAccess"></param>
        /// <returns></returns>
        public static DirectoryInfo Ensure( this DirectoryInfo directoryInfo, Boolean? changeCompressionTo = null, Boolean? requestReadAccess = null, Boolean? requestWriteAccess = null ) {
            Assert.NotNull( directoryInfo );
            if ( directoryInfo == null ) {
                throw new ArgumentNullException( "directoryInfo" );
            }
            try {
                Assert.False( String.IsNullOrWhiteSpace( directoryInfo.FullName ) );
                directoryInfo.Refresh();
                if ( !directoryInfo.Exists ) {
                    directoryInfo.Create();
                    directoryInfo.Refresh();
                }

                if ( changeCompressionTo.HasValue ) {
                    directoryInfo.SetCompression( changeCompressionTo.Value );
                    directoryInfo.Refresh();
                }

                if ( requestReadAccess.HasValue ) {
                    directoryInfo.Refresh();
                }

                if ( requestWriteAccess.HasValue ) {
                    var temp = Path.Combine( directoryInfo.FullName, Path.GetRandomFileName() );
                    File.WriteAllText( temp, "Delete Me!" );
                    File.Delete( temp );
                    directoryInfo.Refresh();
                }
                Assert.True( directoryInfo.Exists );
            }
            catch ( Exception exception ) {
                exception.Error();
                return null;
            }
            return directoryInfo;
        }

        public static DriveInfo GetDriveWithLargestAvailableFreeSpace() {
            return DriveInfo.GetDrives().AsParallel().Where( info => info.IsReady ).FirstOrDefault( driveInfo => driveInfo.AvailableFreeSpace >= DriveInfo.GetDrives().AsParallel().Where( info => info.IsReady ).Max( info => info.AvailableFreeSpace ) );
        }

        public static Boolean IsDirectory( this NativeWin32.Win32FindData data ) {
            return ( data.dwFileAttributes & FileAttributes.Directory ) == FileAttributes.Directory;
        }

        public static Boolean IsFile( this NativeWin32.Win32FindData data ) {
            return !IsDirectory( data );
        }

        public static Boolean IsIgnoreFolder( this NativeWin32.Win32FindData data ) {
            return data.cFileName.EndsWith( "$RECYCLE.BIN", StringComparison.InvariantCultureIgnoreCase ) || data.cFileName.Equals( "TEMP", StringComparison.InvariantCultureIgnoreCase ) || data.cFileName.Equals( "TMP", StringComparison.InvariantCultureIgnoreCase ) || SystemFolders.Contains( new DirectoryInfo( data.cFileName ) );
        }

        public static Boolean IsParentOrCurrent( this NativeWin32.Win32FindData data ) {
            return data.cFileName == "." || data.cFileName == "..";
        }

        public static Boolean IsProtected( [NotNull] this FileSystemInfo fileSystemInfo ) {
            if ( fileSystemInfo == null ) {
                throw new ArgumentNullException( "fileSystemInfo" );
            }
            if ( !fileSystemInfo.Exists ) {
                return false;
            }
            var ds = new DirectorySecurity( fileSystemInfo.FullName, AccessControlSections.Access );
            if ( ds.AreAccessRulesProtected ) {
                using ( var wi = WindowsIdentity.GetCurrent() ) {
                    if ( wi != null ) {
                        var wp = new WindowsPrincipal( wi );
                        var isProtected = !wp.IsInRole( WindowsBuiltInRole.Administrator ); // Not running as admin
                        return isProtected;
                    }
                }
            }

            return false;
        }

        public static Boolean IsReparsePoint( this NativeWin32.Win32FindData data ) {
            return ( data.dwFileAttributes & FileAttributes.ReparsePoint ) == FileAttributes.ReparsePoint;
        }

        public static Boolean SetCompression( this DirectoryInfo directoryInfo, Boolean compressed = true ) {
            try {
                if ( directoryInfo.Exists ) {
                    using ( var dir = new ManagementObject( directoryInfo.ToManagementPath() ) ) {
                        var outParams = dir.InvokeMethod( compressed ? "Compress" : "Uncompress", null, null );
                        if ( null == outParams ) {
                            return false;
                        }
                        var result = Convert.ToUInt32( outParams.Properties[ "ReturnValue" ].Value );
                        return result == 0;
                    }
                }
            }
            catch ( ManagementException exception ) {
                exception.Error();
            }
            return false;
        }

        public static ManagementPath ToManagementPath( this DirectoryInfo systemPath ) {
            var fullPath = systemPath.FullName;
            while ( fullPath.EndsWith( "\\" ) ) {
                fullPath = fullPath.Substring( 0, fullPath.Length - 1 );
            }
            fullPath = String.Format( "Win32_Directory.Name=\"{0}\"", fullPath.Replace( "\\", "\\\\" ) );
            var managed = new ManagementPath( fullPath );
            return managed;
        }

        public static IEnumerable< string > ToPaths( [NotNull] this DirectoryInfo directoryInfo ) {
            if ( directoryInfo == null ) {
                throw new ArgumentNullException( "directoryInfo" );
            }
            return directoryInfo.ToString().Split( Path.DirectorySeparatorChar );
        }

        /// <summary>
        ///     (does not create path)
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DirectoryInfo WithShortDatePath( this DirectoryInfo basePath, DateTime d ) {
            var path = Path.Combine( basePath.FullName, d.Year.ToString(), d.DayOfYear.ToString(), d.Hour.ToString() );
            return new DirectoryInfo( path );
        }
    }
}
