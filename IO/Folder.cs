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
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security;
    using Annotations;
    using Extensions;

    [DataContract( IsReference = true )]
    [Immutable]
    public class Folder {

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
        private readonly DirectoryInfo _directoryInfo;

        [NotNull]
        public string FullName { get { return this._directoryInfo.FullName; } }

        /// <summary>
        ///     <para>The <see cref="Folder" />.</para>
        /// </summary>
        [NotNull]
        public readonly String OriginalPath;

        [NotNull]
        public readonly Uri Uri;

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public Folder( [NotNull] String path ) {
            if ( String.IsNullOrWhiteSpace( path ) ) {
                throw new ArgumentNullException( "path" );
            }

            this.OriginalPath = path;

            if ( !IOExtensions.TryGetFolderFromPath( path, out this._directoryInfo, out this.Uri ) ) {
                throw new InvalidOperationException( String.Format( "Unable to parse path {0}", path ) );
            }
        }

        /// <summary>
        /// <para>Returns an enumerable collection of <see cref="Document"/> in the current directory.</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerable< Document > GetDocuments() {
            return this._directoryInfo.EnumerateFiles().Select( fileInfo => new Document( fileInfo.FullName ) );
        }

        public IEnumerable< Folder > GetFolders() {
            return this._directoryInfo.EnumerateDirectories().Select( fileInfo => new Folder( fileInfo.FullName ) );
        }

        public IEnumerable<Folder> GetFolders( String searchPattern ) {
            if ( String.IsNullOrEmpty( searchPattern) ) {
                yield break;
            }
            foreach ( var fileInfo in this._directoryInfo.EnumerateDirectories( searchPattern ) ) {
                yield return new Folder( fileInfo.FullName );
            }
        }
        
        public IEnumerable<Folder> GetFolders( String searchPattern, SearchOption searchOption ) {
            if ( String.IsNullOrEmpty( searchPattern) ) {
                yield break;
            }
            foreach ( var fileInfo in this._directoryInfo.EnumerateDirectories( searchPattern, searchOption ) ) {
                yield return new Folder( fileInfo.FullName );
            }
        }

        public IEnumerable<Document> GetDocuments( String searchPattern ) {
            return this._directoryInfo.EnumerateFiles( searchPattern ).Select( fileInfo => new Document( fileInfo.FullName ) );
        }

        public IEnumerable<Document> GetDocuments(IEnumerable< String> searchPatterns ) {
            return searchPatterns.SelectMany( this.GetDocuments );
        }

        public IEnumerable<Document> GetDocuments( IEnumerable<String> searchPatterns, SearchOption searchOption ) {
            return searchPatterns.SelectMany( searchPattern => this.GetDocuments( searchPattern, searchOption ) );
        }

        public IEnumerable<Document> GetDocuments( String searchPattern, SearchOption searchOption ) {
            return this._directoryInfo.EnumerateFiles( searchPattern, searchOption ).Select( fileInfo => new Document( fileInfo.FullName ) );
        }

        /// <summary>
        ///     Returns true if the <see cref="Folder" /> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        public Boolean Exists {
            get {
                this._directoryInfo.Refresh();
                return this._directoryInfo.Exists;
            }
        }

        /// <summary>
        /// <para>Returns True if the folder now exists.</para>
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Delete"/>
        public Boolean Create() {
            try {
                this._directoryInfo.Create();
                return this.Exists;
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
                this._directoryInfo.Delete();
                return !this.Exists;
            }
            catch ( IOException ) {
                return false;
            }
        }
    }
}