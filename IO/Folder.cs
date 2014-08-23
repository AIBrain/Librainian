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
    using System.IO;
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
        public readonly DirectoryInfo DirectoryInfo;

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

            var cleanUpPath = GetFolderFromPath( path, out this.Uri );
            if ( null == cleanUpPath ) {
                throw new InvalidOperationException( String.Format( "Unable to parse path {0}", path ) );
            }

            this.DirectoryInfo = cleanUpPath;
        }

        /// <summary>
        ///     Returns true if the <see cref="Folder" /> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        public Boolean Exists { get { return Directory.Exists( this.DirectoryInfo.FullName ); } }

        [CanBeNull]
        public static DirectoryInfo GetFolderFromPath( String path, out Uri uri ) {
            uri = null;
            try {
                if ( String.IsNullOrWhiteSpace( path ) ) {
                    return null;
                }
                path = path.Trim();
                if ( String.IsNullOrWhiteSpace( path ) ) {
                    return null;
                }
                if ( Uri.TryCreate( path, UriKind.Absolute, out uri ) ) {
                    return new DirectoryInfo( uri.LocalPath );
                }
            }
            catch ( UriFormatException ) { }
            catch ( SecurityException ) { }
            catch ( PathTooLongException ) { }
            catch ( InvalidOperationException ) { }
            return null;
        }
    }
}