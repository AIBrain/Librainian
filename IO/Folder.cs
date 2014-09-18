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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Folder.cs" was last cleaned by Rick on 2014/08/23 at 12:37 AM

#endregion License & Information

namespace Librainian.IO {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Windows.Forms;
    using Annotations;
    using Extensions;
    using Parsing;

    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [DataContract( IsReference = true )]
    [Immutable]
    public class Folder {

        //TODO add in long name (unc) support. Like 'ZedLongPaths' ?

        private String DebuggerDisplay { get { return this.FullName; } }

        /// <summary>
        /// Returns a String that represents the current object.
        /// </summary>
        /// <returns>
        /// A String that represents the current object.
        /// </returns>
        public override String ToString() {
            return DebuggerDisplay;
        }

        /// <summary>
        ///     "\"
        /// </summary>
        [NotNull]
        public static readonly String FolderSeparator = new String( new[] { Path.DirectorySeparatorChar } );

        /// <summary>
        ///     "/"
        /// </summary>
        [NotNull]
        public static readonly String FolderAltSeparator = new String( new[] { Path.AltDirectorySeparatorChar } );

        [NotNull]
        protected readonly DirectoryInfo DirectoryInfo;

        [NotNull]
        public String FullName { get { return this.DirectoryInfo.FullName; } }

        /// <summary>
        ///     <para>The <see cref="Folder" />.</para>
        /// </summary>
        [NotNull]
        public readonly String OriginalFullPath;

        [NotNull]
        public readonly Uri Uri;

        /// <summary>
        /// </summary>
        /// <param name="fullPath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( [NotNull] String fullPath ) {
            if ( String.IsNullOrWhiteSpace( fullPath ) ) {
                throw new ArgumentNullException( "path" );
            }

            this.OriginalFullPath = fullPath;

            if ( !IOExtensions.TryGetFolderFromPath( fullPath, out this.DirectoryInfo, out this.Uri ) ) {
                throw new InvalidOperationException( String.Format( "Unable to parse a valid path from `{0}`", fullPath ) );
            }
        }


        /// <summary>
        ///     <para>Static comparison of the file names (case insensitive) and file sizes for equality.</para>
        ///     <para>
        ///         To compare the contents of two <see cref="Document" /> use
        ///         <see cref="IOExtensions.SameContent(Document,Document)" />.
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
            return left.FullName.Like( right.FullName );
        }

        public override int GetHashCode() {
            return this.FullName.GetHashCode();
        }

        /// <summary>
        /// <para>Check if this <see cref="Folder"/> contains any <see cref="Folder"/> or <see cref="Document"/>.</para>
        /// </summary>
        /// <returns></returns>
        public Boolean IsEmpty() {
            return !this.GetFolders("*.*").Any() && !this.GetDocuments("*.*").Any();
        }

        public Folder( Environment.SpecialFolder specialFolder )
            : this( Environment.GetFolderPath( specialFolder ) ) {
        }

        public Folder( Environment.SpecialFolder specialFolder, String appName, String subFolder )
            : this( Path.Combine( Environment.GetFolderPath( specialFolder ), appName, subFolder ) ) {
        }

        /// <summary>
        /// <para>Pass null to automatically fill in <paramref name="companyName"/> and <paramref name="applicationName"/>.</para>
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <param name="companyName"></param>
        /// <param name="applicationName"></param>
        /// <param name="subFolder"></param>
        public Folder( Environment.SpecialFolder specialFolder, String companyName, String applicationName, String subFolder )
            : this( Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder ) ) {
        }

        public Folder( String fullPath, String subFolder )
            : this( Path.Combine( fullPath, subFolder ) ) {
        }

        /// <summary>
        /// <para>Returns an enumerable collection of <see cref="Document"/> in the current directory.</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Document> GetDocuments() {
            if ( !this.DirectoryInfo.Exists ) {
                this.Refresh();
                if ( !this.DirectoryInfo.Exists ) {
                    return Enumerable.Empty<Document>();
                }
            }
            return this.DirectoryInfo.EnumerateFiles( ).Select( fileInfo => new Document( fileInfo.FullName ) );
        }

        public IEnumerable<Folder> GetFolders() {
            if ( !this.DirectoryInfo.Exists ) {
                this.Refresh();
                if ( !this.DirectoryInfo.Exists ) {
                    return Enumerable.Empty<Folder>();
                }
            }
            return this.DirectoryInfo.EnumerateDirectories().Select( fileInfo => new Folder( fileInfo.FullName ) );
        }

        public IEnumerable<Folder> GetFolders( String searchPattern ) {
            if ( String.IsNullOrEmpty( searchPattern ) ) {
                yield break;
            }
            foreach ( var fileInfo in this.DirectoryInfo.EnumerateDirectories( searchPattern ) ) {
                yield return new Folder( fileInfo.FullName );
            }
        }

        public IEnumerable<Folder> GetFolders( String searchPattern, SearchOption searchOption ) {
            if ( String.IsNullOrEmpty( searchPattern ) ) {
                yield break;
            }
            foreach ( var fileInfo in this.DirectoryInfo.EnumerateDirectories( searchPattern, searchOption ) ) {
                yield return new Folder( fileInfo.FullName );
            }
        }

        public IEnumerable<Document> GetDocuments( String searchPattern ) {
            return this.DirectoryInfo.EnumerateFiles( searchPattern ).Select( fileInfo => new Document( fileInfo.FullName ) );
        }

        public IEnumerable<Document> GetDocuments( IEnumerable<String> searchPatterns ) {
            return searchPatterns.SelectMany( this.GetDocuments );
        }

        public IEnumerable<Document> GetDocuments( IEnumerable<String> searchPatterns, SearchOption searchOption ) {
            return searchPatterns.SelectMany( searchPattern => this.GetDocuments( searchPattern, searchOption ) );
        }

        public IEnumerable<Document> GetDocuments( String searchPattern, SearchOption searchOption ) {
            return this.DirectoryInfo.EnumerateFiles( searchPattern, searchOption ).Select( fileInfo => new Document( fileInfo.FullName ) );
        }

        /// <summary>
        ///     Returns true if the <see cref="Folder" /> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        public bool Exists() {
            this.Refresh();
            return this.DirectoryInfo.Exists;
        }

        /// <summary>
        /// <para>Returns True if the folder now exists.</para>
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Delete"/>
        public Boolean Create() {
            try {
                this.DirectoryInfo.Create();
                return this.Exists();
            }
            catch ( IOException ) {
                return false;
            }
        }

        /// <summary>
        /// <para>Returns True if the folder no longers exists.</para>
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Create"/>
        public Boolean Delete() {
            try {
                //safety checks
                if ( this.IsEmpty() ) {
                    this.DirectoryInfo.Delete();
                    return !this.Exists();
                }
            }
            catch ( IOException ) {
            }
            return false;
        }

        //TODO
       //public Task FindFilesAsync(
       //     IEnumerable<String> fileSearchPatterns
       //     , DirectoryInfo startingFolder
       //     , CancellationToken cancellationToken
       //     , Action<FileInfo> onFindFile = null
       //     , Action<DirectoryInfo> onEachDirectory = null
       //     , SearchStyle searchStyle = SearchStyle.FilesFirst
       //     )


        public void Refresh() {
            this.DirectoryInfo.Refresh();

        }
    }
}