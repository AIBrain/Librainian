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
// "Librainian2/FileSystemExtensions.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.IO;
    using System.Linq;
    using Threading;

    public static class FileSystemExtensions {
        static FileSystemExtensions() {
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.System ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.SystemX86 ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.AdminTools ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.CDBurning ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.Windows ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.Cookies ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.History ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.InternetCache ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.PrinterShortcuts ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86 ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.Programs ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.SendTo ) ) );
            DirectoryInfos.SystemFolders.Add( new DirectoryInfo( Path.GetTempPath() ) );
        }

        public static Boolean SetCompression( this String folderPath, Boolean compressed = true ) {
            if ( String.IsNullOrWhiteSpace( folderPath ) ) {
                return false;
            }

            try {
                var dirInfo = new DirectoryInfo( folderPath );
                return dirInfo.SetCompression( compressed );
            }
            catch ( Exception exception ) {
                exception.Log();
            }
            return false;
        }

        /// <summary>
        ///     Given the <paramref name="path" /> and <paramref name="searchPattern" /> pick any one file and return the
        ///     <see
        ///         cref="FileSystemInfo.FullName" />
        ///     .
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static String GetRandomFile( String path, String searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly ) {
            if ( !Directory.Exists( path ) ) {
                return String.Empty;
            }
            var dir = new DirectoryInfo( path );
            if ( !dir.Exists ) {
                return String.Empty;
            }
            var files = Directory.EnumerateFiles( path: dir.FullName, searchPattern: searchPattern, searchOption: searchOption );
            var pickedfile = files.OrderBy( r => Randem.Next() ).FirstOrDefault();
            if ( pickedfile != null && File.Exists( pickedfile ) ) {
                return new FileInfo( pickedfile ).FullName;
            }
            return String.Empty;
        }
    }
}
