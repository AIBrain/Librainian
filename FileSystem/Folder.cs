// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Folder.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows.Forms;
    using Extensions;
    using JetBrains.Annotations;
    using Magic;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using OperatingSystem;
    using Parsing;
    using Persistence;

    [TestFixture]
    public static class FolderTests {

        [Test]
        public static void TestSerialize() {
            var expected = Windows.WindowsSystem32Folder.Value;

            var json = expected.ToJSON();

            var actual = json.FromJSON< Folder >();

            Assert.AreEqual( expected, actual );
        }

    }

    /// <summary>
    ///     //TODO add in long name (unc) support. Like 'ZedLongPaths' does
    /// </summary>
    [DebuggerDisplay( "{ToString()}" )]
    [JsonObject(MemberSerialization.Fields)]
    [Immutable]
    public class Folder : BetterDisposableClass, IEquatable<Folder> {

        /// <summary>
        /// </summary>
        /// <param name="fullPath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( [NotNull] String fullPath ) {
            if ( fullPath.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( fullPath ) );
            }

            DirectoryInfo directoryInfo;
            Uri uri;
            if ( !fullPath.TryGetFolderFromPath( out directoryInfo, out uri ) ) {
                throw new InvalidOperationException( $"Unable to parse a valid path from `{fullPath}`" );
            }

            if ( directoryInfo == null ) {
                throw new InvalidOperationException( $"Unable to parse a valid path from `{fullPath}`" );
            }

            this.Info = directoryInfo;
        }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( Environment.SpecialFolder specialFolder ) : this( Environment.GetFolderPath( specialFolder ) ) { }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( Environment.SpecialFolder specialFolder, String subFolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), subFolder ) ) { }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( Environment.SpecialFolder specialFolder, String applicationName, String subFolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder ) ) { }

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

        /// <summary>
        ///     <para>
        ///         Pass null to automatically fill in <paramref name="companyName" /> and
        ///         <paramref name="applicationName" /> .
        ///     </para>
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <param name="companyName"></param>
        /// <param name="applicationName"></param>
        /// <param name="subFolder"></param>
        /// <param name="subSubfolder"></param>
        /// <param name="subSubSubfolder"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( Environment.SpecialFolder specialFolder, String companyName, String applicationName, String subFolder, String subSubfolder, String subSubSubfolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder, subSubfolder, subSubSubfolder ) ) { }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( String fullPath, String subFolder ) : this( Path.Combine( fullPath, subFolder ) ) { }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( Folder folder, String subFolder ) : this( Path.Combine( folder.FullName, subFolder ) ) { }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( FileSystemInfo fileSystemInfo ) : this( fileSystemInfo.FullName ) { } //-V3073

        /// <summary>
        ///     "/"
        /// </summary>
        [NotNull]
        public static String FolderAltSeparator { get; } = new String( new[] { Path.AltDirectorySeparatorChar } );

        /// <summary>
        ///     "\"
        /// </summary>
        [NotNull]
        public static String FolderSeparator { get; } = new String( new[] { Path.DirectorySeparatorChar } );

        public static Char[] FolderSeparatorChar { get; } = { Path.DirectorySeparatorChar };

        [NotNull]
        public String FullName => this.Info.FullName;

        [NotNull]
        public DirectoryInfo Info {
            get;
        }

        [NotNull]
        public String Name => this.Info.Name;

        [CanBeNull]
        public Folder GetParent() {
            var info = this.Info;
            if ( info.Parent != null ) {
                return new Folder( info.Parent );
            }
            return null;
        }

        /// <summary>
        ///     <para>Static comparison of the folder names (case sensitive) for equality.</para>
        ///     <para>
        ///         To compare the path of two <see cref="Folder" /> use
        ///         <seealso cref="IOExtensions.SameContent(Folder,Folder)" /> .
        ///     </para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Folder left, [CanBeNull] Folder right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }
            if ( ReferenceEquals( left, null ) || ReferenceEquals( right, null ) ) {
                return false;
            }
            return left.FullName.Same( right.FullName );
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public static Folder GetTempFolder() {
            var tempFolder = new Folder( Path.GetTempPath() );
            if ( tempFolder.Exists() ) {
                return tempFolder;
            }
            if ( tempFolder.Create() ) {
                return tempFolder;
            }
            throw new DirectoryNotFoundException( $"Unable to create the user's temp folder `{tempFolder.FullName}`." );
        }

        public static implicit operator DirectoryInfo( Folder folder ) => folder.Info;

        public static Boolean TryParse( [CanBeNull] String path, [CanBeNull] out Folder folder ) {
            folder = null;

            try {
                if ( String.IsNullOrWhiteSpace( path ) ) {
                    return false;
                }
                path = path.Trim();
                if ( String.IsNullOrWhiteSpace( path ) ) {
                    return false;
                }
                DirectoryInfo dirInfo;
                Uri uri;
                if ( Uri.TryCreate( path, UriKind.Absolute, out uri ) ) {
                    dirInfo = new DirectoryInfo( uri.LocalPath );
                    folder = new Folder( dirInfo );
                    return true;
                }

                dirInfo = new DirectoryInfo( path ); //try it anyways
                folder = new Folder( dirInfo );
                return true;
            }
            catch ( ArgumentException ) { }
            catch ( UriFormatException ) { }
            catch ( SecurityException ) { }
            catch ( PathTooLongException ) { }
            catch ( InvalidOperationException ) { }
            return false;
        }

        public IEnumerable<Folder> BetterGetFolders( String searchPattern = "*" ) {
            if ( String.IsNullOrEmpty( searchPattern ) ) {
                yield break;
            }
            foreach ( var fileInfo in this.Info.BetterEnumerateDirectories( searchPattern ) ) {
                yield return new Folder( fileInfo.FullName );
            }
        }

        /// <summary>
        ///     Returns a copy of the folder instance.
        /// </summary>
        /// <returns></returns>
        public Folder Clone() {
            return new Folder( this );
        }

        /// <summary>
        ///     <para>Returns True if the folder exists.</para>
        /// </summary>
        /// <returns></returns>
        /// See also:
        /// <seealso cref="Delete"></seealso>
        public Boolean Create() {
            try {
                if ( this.Exists() ) {
                    return true;
                }
                this.Info.Create();
                return this.Exists();
            }
            catch ( IOException ) {
                return false;
            }
        }

        /// <summary>
        ///     <para>Returns True if the folder no longer exists.</para>
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Create"></seealso>
        public Boolean Delete() {
            try {

                //safety checks
                if ( this.IsEmpty() ) {
                    this.Info.Delete();
                    return !this.Exists();
                }
            }
            catch ( IOException ) { }
            return false;
        }

        public Boolean DemandPermission( FileIOPermissionAccess access ) {
            try {
                var bob = new FileIOPermission( access: access, path: this.FullName );
                bob.Demand();
                return true;
            }
            catch ( ArgumentException exception ) {
                exception.More();
            }
            catch ( SecurityException ) { }
            return false;
        }

        public Boolean Equals( Folder other ) {
            return Equals( this, other );
        }

        /// <summary>
        ///     Returns true if the <see cref="Folder" /> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        public Boolean Exists() {
            this.Refresh();
            return this.Info.Exists;
        }

        /// <summary>
        ///     Free space available to the current user.
        /// </summary>
        /// <returns></returns>
        public UInt64 GetAvailableFreeSpace() {
            var driveLetter = this.GetDrive().ToString();
            var driveInfo = new DriveInfo( driveLetter );
            return ( UInt64 )driveInfo.AvailableFreeSpace;
        }

        /// <summary>
        ///     <para>Returns an enumerable collection of <see cref="Document" /> in the current directory.</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Document> GetDocuments() {
            if ( !this.Info.Exists ) {
                this.Refresh();
                if ( !this.Info.Exists ) {
                    return Enumerable.Empty<Document>();
                }
            }
            return this.Info.BetterEnumerateFiles().Select( fileInfo => new Document( fileInfo.FullName ) );
        }

        public IEnumerable<Document> GetDocuments( String searchPattern ) => this.Info.BetterEnumerateFiles( searchPattern ).Select( fileInfo => new Document( fileInfo.FullName ) );

        public IEnumerable<Document> GetDocuments( IEnumerable<String> searchPatterns ) => searchPatterns.SelectMany( this.GetDocuments );

        public Drive GetDrive() {
            return new Drive( this.Info.Root.FullName );
        }

        public IEnumerable<Folder> GetFolders() {
            if ( !this.Info.Exists ) {
                this.Refresh();
                if ( !this.Info.Exists ) {
                    return Enumerable.Empty<Folder>();
                }
            }
            return this.Info.EnumerateDirectories().Select( fileInfo => new Folder( fileInfo.FullName ) );
        }

        public IEnumerable<Folder> GetFolders( String searchPattern ) {
            if ( String.IsNullOrEmpty( searchPattern ) ) {
                yield break;
            }
            foreach ( var fileInfo in this.Info.EnumerateDirectories( searchPattern ) ) {
                yield return new Folder( fileInfo.FullName );
            }
        }

        public IEnumerable<Folder> GetFolders( String searchPattern, SearchOption searchOption ) {
            if ( String.IsNullOrEmpty( searchPattern ) ) {
                yield break;
            }
            foreach ( var fileInfo in this.Info.EnumerateDirectories( searchPattern, searchOption ) ) {
                yield return new Folder( fileInfo.FullName );
            }
        }

        public override Int32 GetHashCode() => this.FullName.GetHashCode();

        /// <summary>
        ///     <para>
        ///         Check if this <see cref="Folder" /> contains any <see cref="Folder" /> or
        ///         <see cref="Document" /> .
        ///     </para>
        /// </summary>
        /// <returns></returns>
        public Boolean IsEmpty() => !this.GetFolders( "*.*" ).Any() && !this.GetDocuments( "*.*" ).Any();

        public void OpenWithExplorer() {
            Windows.ExecuteExplorer( arguments: this.FullName );
        }

        public void Refresh() => this.Info.Refresh();

        /// <summary>
        ///     <para>Shorten the full path with "..."</para>
        /// </summary>
        /// <returns></returns>
        public String ToCompactFormat() {
            var sb = new StringBuilder();
            NativeWin32.PathCompactPathEx( sb, this.FullName, this.FullName.Length, 0 ); //TODO untested. //HACK may be buggy on extensions also
            return sb.ToString();
        }

        /// <summary>
        ///     Returns a String that represents the current object.
        /// </summary>
        /// <returns>A String that represents the current object.</returns>
        public override String ToString() => this.FullName;

        protected override void CleanUpManagedResources() {
            base.CleanUpManagedResources();
        }

        protected override void CleanUpNativeResources() {
            base.CleanUpNativeResources();
        }
    }
}