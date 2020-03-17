// Copyright � 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Folder.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Folder.cs" was last formatted by Protiguous on 2020/03/16 at 2:58 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Collections.Extensions;
    using ComputerSystem.Devices;
    using Extensions;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using DirectoryInfo = Pri.LongPath.DirectoryInfo;
    using FileSystemInfo = Pri.LongPath.FileSystemInfo;

    // ReSharper disable RedundantUsingDirective
    using Path = Pri.LongPath.Path;

    // ReSharper restore RedundantUsingDirective

    public interface IFolder : IEquatable<IFolder> {

        [NotNull]
        String FullPath { get; }

        /// <summary>The <see cref="IFolder" /> class is built around <see cref="Pri.LongPath.DirectoryInfo" />.</summary>
        [NotNull]
        DirectoryInfo Info { get; }

        [NotNull]
        String Name { get; }

        /// <summary></summary>
        /// <param name="searchPattern"></param>
        /// <param name="randomize">    </param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        [NotNull]
        [ItemNotNull]
        IEnumerable<IFolder> BetterGetFolders( [CanBeNull] String? searchPattern = "*", Boolean randomize = false, SearchOption searchOption = SearchOption.AllDirectories );

        /// <summary>Return a list of all <see cref="IFolder" /> matching the <paramref name="searchPattern" />.</summary>
        /// <param name="token"></param>
        /// <param name="searchPattern"></param>
        /// <param name="randomize">Return the folders in random order.</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        [NotNull]
        [ItemNotNull]
        Task<List<Folder>> BetterGetFoldersAsync( CancellationToken token, [CanBeNull] String? searchPattern = "*", Boolean randomize = true,
            SearchOption searchOption = SearchOption.AllDirectories );

        /// <summary>Returns a copy of the folder instance.</summary>
        /// <returns></returns>
        [NotNull]
        IFolder Clone();

        /// <summary>
        ///     <para>Returns True if the folder exists.</para>
        /// </summary>
        /// <returns></returns>
        /// See also:
        /// <see cref="Delete"></see>
        Boolean Create();

        /// <summary>
        ///     <para>Returns True if the folder no longer exists.</para>
        /// </summary>
        /// <returns></returns>
        /// <see cref="Create"></see>
        Boolean Delete();

        /// <summary>Returns true if the <see cref="IFolder" /> currently exists.</summary>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        Boolean Exists();

        /// <summary>Free space available to the current user.</summary>
        /// <returns></returns>
        UInt64 GetAvailableFreeSpace();

        /// <summary>
        ///     <para>Returns an enumerable collection of <see cref="Document" /> in the current directory.</para>
        /// </summary>
        /// <returns></returns>
        [NotNull]
        [ItemNotNull]
        IEnumerable<Document> GetDocuments();

        [NotNull]
        [ItemNotNull]
        IEnumerable<Document> GetDocuments( [NotNull] String searchPattern );

        [NotNull]
        [ItemNotNull]
        IEnumerable<Document> GetDocuments( [NotNull] IEnumerable<String> searchPatterns );

        [NotNull]
        Disk GetDrive();

        [NotNull]
        [ItemNotNull]
        IEnumerable<IFolder> GetFolders( [CanBeNull] String? searchPattern, SearchOption searchOption = SearchOption.AllDirectories );

        Int32 GetHashCode();

        [CanBeNull]
        IFolder GetParent();

        Boolean HavePermission( FileIOPermissionAccess access );

        /// <summary>
        ///     <para>Check if this <see cref="IFolder" /> contains any <see cref="IFolder" /> or <see cref="Document" /> .</para>
        /// </summary>
        /// <returns></returns>
        Boolean IsEmpty();

        void OpenWithExplorer();

        void Refresh();

        /// <summary>
        ///     <para>Shorten the full path with "..."</para>
        /// </summary>
        /// <returns></returns>
        String ToCompactFormat();

        /// <summary>Returns a String that represents the current object.</summary>
        /// <returns>A String that represents the current object.</returns>
        [NotNull]
        String ToString();
    }

    /// <summary>//TODO add in long name (unc) support. Like 'ZedLongPaths' does</summary>
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "()}" )]
    [JsonObject]
    [Immutable]
    [Serializable]
    public class Folder : IFolder {

        /// <summary>String of invalid characters in a path or filename.</summary>
        [NotNull]
        private static readonly String InvalidPathCharacters = new String( value: ( Char[] )Path.GetInvalidPathChars() );

        [NotNull]
        public static readonly Regex RegexForInvalidPathCharacters = new Regex( pattern: $"[{Regex.Escape( str: InvalidPathCharacters )}]", options: RegexOptions.Compiled );

        /// <summary>"/"</summary>
        [JsonIgnore]
        [NotNull]
        public static String FolderAltSeparator { get; } = new String( value: new[] {
            Path.AltDirectorySeparatorChar
        } );

        /// <summary>"\"</summary>
        [JsonIgnore]
        [NotNull]
        public static String FolderSeparator { get; } = new String( value: new[] {
            Path.DirectorySeparatorChar
        } );

        [JsonIgnore]
        public static Char FolderSeparatorChar { get; } = Path.DirectorySeparatorChar;

        /// <summary>The <see cref="IFolder" /> class is built around <see cref="DirectoryInfo" />.</summary>
        [JsonProperty]
        [NotNull]
        public DirectoryInfo Info { get; }

        [JsonIgnore]
        [NotNull]
        public String FullPath => this.Info.FullPath;

        [JsonIgnore]
        [NotNull]
        public String Name => this.Info.Name;

        /// <summary></summary>
        /// <param name="fullPath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( String fullPath ) {
            if ( String.IsNullOrWhiteSpace( value: fullPath ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fullPath ) );
            }

            fullPath = CleanPath( fullpath: fullPath ); //replace any invalid path chars with a separator

            if ( String.IsNullOrWhiteSpace( value: fullPath ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fullPath ) );
            }

            if ( !fullPath.TryGetFolderFromPath( directoryInfo: out var directoryInfo, uri: out _ ) ) {
                throw new InvalidOperationException( message: $"Unable to parse a valid path from `{fullPath}`" );
            }

            this.Info = directoryInfo ?? throw new InvalidOperationException( message: $"Unable to parse a valid path from `{fullPath}`" );
        }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( Environment.SpecialFolder specialFolder ) : this( fullPath: Environment.GetFolderPath( folder: specialFolder ) ) { }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( Environment.SpecialFolder specialFolder, [NotNull] String subFolder ) : this(
            fullPath: Path.Combine( path1: Environment.GetFolderPath( folder: specialFolder ), path2: subFolder ) ) { }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( Environment.SpecialFolder specialFolder, [CanBeNull] String? applicationName, [NotNull] String subFolder ) : this(
            fullPath: Path.Combine( path1: Environment.GetFolderPath( folder: specialFolder ),
                path2: applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, path3: subFolder ) ) { }

        /// <summary>
        ///     <para>Pass null to automatically fill in <paramref name="companyName" /> and <paramref name="applicationName" /> .</para>
        /// </summary>
        /// <param name="specialFolder">  </param>
        /// <param name="companyName">    </param>
        /// <param name="applicationName"></param>
        /// <param name="subFolders"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        [DebuggerStepThrough]
        public Folder( Environment.SpecialFolder specialFolder, [CanBeNull] String? companyName, [CanBeNull] String? applicationName, [NotNull] params String[] subFolders ) :
            this( fullPath: Path.Combine( path1: Environment.GetFolderPath( folder: specialFolder ),
                path2: companyName ?? Application.CompanyName ?? throw new InvalidOperationException( message: $"Empty {nameof( Application )}.{Application.CompanyName}." ),
                path3: applicationName ?? Application.ProductName ??
                throw new InvalidOperationException( message: $"Empty {nameof( Application )}.{Application.ProductName}." ),
                path4: subFolders.ToStrings( separator: @"\" ) ) ) { }

        [DebuggerStepThrough]
        public Folder( Environment.SpecialFolder specialFolder, [NotNull] params String[] subFolders ) : this( fullPath: Path.Combine(
                    path1: Environment.GetFolderPath( folder: specialFolder ),
                    path2: subFolders.Select( selector: fullpath => CleanPath( fullpath: fullpath ) ).ToStrings( separator: @"\" ) ) ) { }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        [DebuggerStepThrough]
        public Folder( [NotNull] String fullPath, [NotNull] String subFolder ) : this( fullPath: Path.Combine( path1: fullPath, path2: subFolder ) ) { }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        [DebuggerStepThrough]
        public Folder( [NotNull] IFolder folder, [NotNull] String subFolder ) : this( fullPath: Path.Combine( path1: folder.FullPath, path2: subFolder ) ) { }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        [DebuggerStepThrough]
        public Folder( [NotNull] IDocument document, [NotNull] String subFolder ) : this( fullPath: Path.Combine( path1: document.ContainingingFolder().FullPath,
            path2: subFolder ) ) { }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        [DebuggerStepThrough]
        public Folder( [NotNull] FileSystemInfo fileSystemInfo ) : this( fullPath: fileSystemInfo.FullPath ) { }

        /// <summary>Returns the path with any invalid characters replaced with <paramref name="replacement" /> and then trimmed. (Defaults to "" />.)</summary>
        /// <param name="fullpath"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [NotNull]
        public static String CleanPath( [NotNull] String fullpath, [CanBeNull] String? replacement = null ) {
            if ( fullpath is null ) {
                throw new ArgumentNullException( paramName: nameof( fullpath ) );
            }

            return RegexForInvalidPathCharacters.Replace( input: fullpath, replacement: replacement ?? String.Empty ).Trim();
        }

        ///// <summary>
        /////     <para>
        /////         Pass null to automatically fill in <paramref name="companyName" /> and
        /////         <paramref name="applicationName" /> .
        /////     </para>
        ///// </summary>
        ///// <param name="specialFolder"></param>
        ///// <param name="companyName"></param>
        ///// <param name="applicationName"></param>
        ///// <param name="subFolder"></param>
        ///// <param name="subSubfolder"></param>
        //public Folder( Environment.SpecialFolder specialFolder, String companyName, String applicationName, String subFolder, String subSubfolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder, subSubfolder ) ) {
        //}
        ///// <summary>
        /////     <para>
        /////         Pass null to automatically fill in <paramref name="companyName" /> and
        /////         <paramref name="applicationName" /> .
        /////     </para>
        ///// </summary>
        ///// <param name="specialFolder"></param>
        ///// <param name="companyName"></param>
        ///// <param name="applicationName"></param>
        ///// <param name="subFolder"></param>
        //public Folder( Environment.SpecialFolder specialFolder, String companyName, String applicationName, String subFolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder ) ) {
        //}
        /// <summary>
        ///     <para>Static comparison of the folder names (case sensitive) for equality.</para>
        ///     <para>To compare the path of two <see cref="IFolder" /> use
        ///     <param name="left">todo: describe left parameter on Equals</param>
        ///     <param name="right">todo: describe right parameter on Equals</param>
        ///     <seealso /> .</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] IFolder left, [CanBeNull] IFolder right ) {
            if ( ReferenceEquals( objA: left, objB: right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            return left.FullPath.Is( right: right.FullPath );
        }

        /// <summary>Throws Exception if unable to obtain the Temp path.</summary>
        /// <returns></returns>
        [NotNull]
        public static IFolder GetTempFolder() => new Folder( fullPath: Path.GetTempPath() );

        [NotNull]
        public static implicit operator DirectoryInfo( [NotNull] Folder folder ) => folder.Info;

        /// <summary>Opens a folder in file explorer.</summary>
        public static void OpenWithExplorer( [CanBeNull] IFolder folder ) {
            if ( folder is null ) {
                throw new ArgumentNullException( paramName: nameof( folder ) );
            }

            var windowsFolder = Environment.GetFolderPath( folder: Environment.SpecialFolder.Windows );

            Process.Start( fileName: $@"{windowsFolder}\explorer.exe", arguments: $"/e,\"{folder.FullPath}\"" );
        }

        public static Boolean TryParse( [CanBeNull] String? path, [CanBeNull] out IFolder folder ) {
            folder = null;

            try {
                if ( String.IsNullOrWhiteSpace( value: path ) ) {
                    return default;
                }

                path = CleanPath( fullpath: path );

                if ( String.IsNullOrEmpty( value: path ) ) {
                    return default;
                }

                DirectoryInfo dirInfo;

                if ( Uri.TryCreate( uriString: path, uriKind: UriKind.Absolute, result: out var uri ) ) {
                    dirInfo = new DirectoryInfo( path: uri.LocalPath );
                    folder = new Folder( fileSystemInfo: dirInfo );

                    return true;
                }

                dirInfo = new DirectoryInfo( path: path );      //try it anyways
                folder = new Folder( fileSystemInfo: dirInfo ); //eh? //TODO

                return true;
            }
            catch ( ArgumentException ) { }
            catch ( UriFormatException ) { }
            catch ( SecurityException ) { }
            catch ( PathTooLongException ) { }
            catch ( InvalidOperationException ) { }

            return default;
        }

        /// <summary></summary>
        /// <param name="searchPattern"></param>
        /// <param name="randomize">    </param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        [ItemNotNull]
        public IEnumerable<IFolder> BetterGetFolders( [CanBeNull] String? searchPattern = "*", Boolean randomize = false,
            SearchOption searchOption = SearchOption.AllDirectories ) {
            if ( String.IsNullOrEmpty( value: searchPattern ) ) {
                yield break;
            }

            if ( randomize ) {
                foreach ( var fileInfo in this.Info.BetterEnumerateDirectories( searchPattern: searchPattern, searchOption: searchOption )
                                              .OrderBy( keySelector: info => Randem.Next() ) ) {
                    if ( fileInfo != null ) {
                        yield return new Folder( fullPath: fileInfo.FullPath );
                    }
                }
            }
            else {
                foreach ( var fileInfo in this.Info.BetterEnumerateDirectories( searchPattern: searchPattern, searchOption: searchOption ) ) {
                    yield return new Folder( fullPath: fileInfo.FullPath );
                }
            }
        }

        /// <summary>Return a list of all <see cref="IFolder" /> matching the <paramref name="searchPattern" />.</summary>
        /// <param name="token"></param>
        /// <param name="searchPattern"></param>
        /// <param name="randomize">Return the folders in random order.</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        [NotNull]
        [ItemNotNull]
        public Task<List<Folder>> BetterGetFoldersAsync( CancellationToken token, [CanBeNull] String? searchPattern = "*", Boolean randomize = false,
            SearchOption searchOption = SearchOption.AllDirectories ) =>
            Task.Run( function: () => {
                var folders = new List<Folder>( collection: this.Info.BetterEnumerateDirectories( searchPattern: searchPattern, searchOption: searchOption )
                                                                .Select( selector: info => new Folder( fullPath: info.FullPath ) ) );

                if ( randomize ) {
                    folders.ShuffleByHarker( token: token );
                }

                return folders;
            }, cancellationToken: token );

        /// <summary>Returns a copy of this folder instance. (Doesn't change anything in the file system.)</summary>
        /// <returns></returns>
        [NotNull]
        public IFolder Clone() => new Folder( fileSystemInfo: this );

        /// <summary>
        ///     <para>Returns True if the folder exists.</para>
        /// </summary>
        /// <returns></returns>
        /// See also:
        /// <see cref="Delete"></see>
        public Boolean Create() {
            try {
                if ( this.Exists() ) {
                    return true;
                }

                try {
                    if ( this.Info.Parent.Exists == false ) {
                        new Folder( fullPath: this.Info.Parent.FullPath ).Create();
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();
                }

                this.Info.Create();

                return this.Exists();
            }
            catch ( IOException ) {
                return default;
            }
        }

        /// <summary>
        ///     <para>Returns True if the folder no longer exists.</para>
        /// </summary>
        /// <returns></returns>
        /// <see cref="Create"></see>
        public Boolean Delete() {
            try {

                //safety checks
                if ( this.IsEmpty() ) {
                    this.Info.Delete();

                    return !this.Exists();
                }
            }
            catch ( IOException ) { }

            return default;
        }

        public Boolean Equals( IFolder other ) => Equals( left: this, right: other );

        /// <summary>Returns true if the <see cref="IFolder" /> currently exists.</summary>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        public Boolean Exists() {
            this.Refresh();

            return this.Info.Exists;
        }

        public Boolean Explore() => this.Info.OpenWithExplorer();

        /// <summary>Free space available to the current user.</summary>
        /// <returns></returns>
        public UInt64 GetAvailableFreeSpace() {
            var driveLetter = this.GetDrive().ToString();
            var driveInfo = new DriveInfo( driveName: driveLetter );

            return ( UInt64 )driveInfo.AvailableFreeSpace;
        }

        /// <summary>
        ///     <para>Returns an enumerable collection of <see cref="Document" /> in the current directory.</para>
        /// </summary>
        /// <returns></returns>
        [NotNull]
        [ItemNotNull]
        public IEnumerable<Document> GetDocuments() {
            if ( !this.Info.Exists ) {
                this.Refresh();

                if ( !this.Info.Exists ) {
                    return Enumerable.Empty<Document>();
                }
            }

            return this.Info.BetterEnumerateFiles().Select( selector: fileInfo => new Document( fullPath: fileInfo.FullPath ) );
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<Document> GetDocuments( [NotNull] String searchPattern ) =>
            this.Info.BetterEnumerateFiles( searchPattern: searchPattern ).Select( selector: fileInfo => new Document( fullPath: fileInfo.FullPath ) );

        [NotNull]
        [ItemNotNull]
        public IEnumerable<Document> GetDocuments( [NotNull] IEnumerable<String> searchPatterns ) => searchPatterns.SelectMany( selector: this.GetDocuments );

        [NotNull]
        public Disk GetDrive() => new Disk( fullpath: this.Info.Root.FullPath );

        [ItemNotNull]
        [NotNull]
        public IEnumerable<IFolder> GetFolders( [CanBeNull] String? searchPattern, SearchOption searchOption = SearchOption.AllDirectories ) {
            if ( String.IsNullOrEmpty( value: searchPattern ) ) {
                yield break;
            }

            foreach ( var fileInfo in this.Info.BetterEnumerateDirectories( searchPattern: searchPattern, searchOption: searchOption ) ) {
                yield return new Folder( fullPath: fileInfo.FullPath );
            }
        }

        public override Int32 GetHashCode() => this.FullPath.GetHashCode();

        [CanBeNull]
        public IFolder GetParent() => new Folder( fileSystemInfo: this.Info.Parent );

        public Boolean HavePermission( FileIOPermissionAccess access ) {
            try {
                var bob = new FileIOPermission( access: access, path: this.FullPath );
                bob.Demand();

                return true;
            }
            catch ( ArgumentException exception ) {
                exception.Log();
            }
            catch ( SecurityException ) { }

            return default;
        }

        /// <summary>
        ///     <para>Check if this <see cref="IFolder" /> contains any <see cref="IFolder" /> or <see cref="Document" /> .</para>
        /// </summary>
        /// <returns></returns>
        public Boolean IsEmpty() => !this.GetFolders( searchPattern: "*.*" ).Any() && !this.GetDocuments( searchPattern: "*.*" ).Any();

        public void OpenWithExplorer() {
            using var _ = Windows.OpenWithExplorer( value: this.FullPath );
        }

        public void Refresh() => this.Info.Refresh();

        /// <summary>
        ///     <para>Shorten the full path with "..."</para>
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public String ToCompactFormat() {
            var sb = new StringBuilder();

            NativeMethods.PathCompactPathEx( pszOut: sb, szPath: this.FullPath, cchMax: this.FullPath.Length,
                dwFlags: 0 ); //TODO untested. //HACK may be buggy on extensions also

            return sb.ToString();
        }

        [NotNull]
        public DirectoryInfo ToDirectoryInfo() => this;

        /// <summary>Returns a String that represents the current object.</summary>
        /// <returns>A String that represents the current object.</returns>
        [NotNull]
        public override String ToString() => this.FullPath;
    }
}