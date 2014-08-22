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
// "Librainian/Document.cs" was last cleaned by Rick on 2014/08/19 at 1:27 PM
#endregion

namespace Librainian.IO {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security;
    using Annotations;
    using Collections;
    using Extensions;

    /// <summary>
    ///     <para>A wrapper for a file, the extension, the [parent] folder, and the file's size all from a given full path.</para>
    ///     <para>Also contains static string versions from <see cref="Path" /></para>
    /// </summary>
    /// <seealso cref="IOExtensions.SameContent(Document,Document)" />
    [DataContract( IsReference = true )]
    [Immutable]
    public class Document : IEquatable< Document > {
        /// <summary>
        ///     "\"
        /// </summary>
        [NotNull] public static readonly String FolderSeparator = new String( new[] { Path.DirectorySeparatorChar } );

        /// <summary>
        ///     "/"
        /// </summary>
        [NotNull] public static readonly String FolderAltSeparator = new String( new[] { Path.AltDirectorySeparatorChar } );

        /// <summary>
        ///     "/"
        /// </summary>
        [NotNull] public static readonly List< char > InvalidPathChars = new List< char >( Path.GetInvalidPathChars() );

        /// <summary>
        ///     <para>The extension of the <see cref="FileName" />, including the ".".</para>
        /// </summary>
        [NotNull] public readonly String Extension;

        /// <summary>
        ///     <para>The file's name, including the extension.</para>
        /// </summary>
        /// <seealso cref="Path.GetFileNameWithoutExtension" />
        [NotNull] public readonly String FileName;

        /// <summary>
        ///     <para>FYI: A folder always ends with the <see cref="FolderSeparator" />.</para>
        /// </summary>
        [NotNull] public readonly String Folder;

        /// <summary>
        ///     <para>The <see cref="Folder" /> combined with the <see cref="FileName" />.</para>
        /// </summary>
        [NotNull] public readonly String FullPathWithFileName;

        /// <summary>
        ///     The last known size of the file.
        /// </summary>
        public readonly UInt64 Size;

        static Document() {
            InvalidPathChars.Fix();
        }

        /// <summary>
        /// </summary>
        /// <param name="fullPathWithFilename"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Document( [NotNull] String fullPathWithFilename ) {
            if ( String.IsNullOrWhiteSpace( fullPathWithFilename ) ) {
                throw new ArgumentNullException( "fullPathWithFilename" );
            }

            fullPathWithFilename = fullPathWithFilename.Trim();
            if ( String.IsNullOrWhiteSpace( fullPathWithFilename ) ) {
                throw new ArgumentNullException( "fullPathWithFilename" );
            }

            var file = new FileInfo( fullPathWithFilename );

            this.Folder = Path.GetDirectoryName( fullPathWithFilename ) ?? String.Empty;
            while ( !this.Folder.EndsWith( FolderAltSeparator ) ) {
                this.Folder = this.Folder.Substring( 0, this.Folder.Length - 1 ); //trim off extra "/"
            }

            if ( !this.Folder.EndsWith( FolderSeparator ) ) {
                this.Folder += FolderSeparator;
            }

            this.FileName = Path.GetFileName( fullPathWithFilename );
            this.Extension = Path.GetExtension( fullPathWithFilename );

            this.Size = ( UInt64 ) file.Length;

            this.FullPathWithFileName = Path.Combine( this.Folder, this.FileName );
        }

        /// <summary>
        ///     Returns true if the <see cref="Document" /> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        public Boolean FileExists { get { return File.Exists( this.FullPathWithFileName ); } }

        /// <summary>
        ///     Returns true if the <see cref="Document.Folder" /> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        public Boolean FolderExists { get { return Directory.Exists( this.Folder ); } }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        ///     <para>
        ///         To compare the contents of two <see cref="Document" /> use
        ///         <see cref="IOExtensions.SameContent(Document,Document)" />.
        ///     </para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( [CanBeNull] Document other ) {
            return !ReferenceEquals( null, other ) && Equals( this, other );
        }

        public UInt64 GetSize() {
            var info = new FileInfo( this.FullPathWithFileName );
            return !info.Exists ? UInt64.MinValue : ( UInt64 ) info.Length;
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
        public static Boolean Equals( [CanBeNull] Document left, [CanBeNull] Document right ) {
            if ( left == null ) {
                throw new ArgumentNullException( "left" );
            }
            if ( right == null ) {
                throw new ArgumentNullException( "right" );
            }
            return left.Size == right.Size && String.Equals( left.FullPathWithFileName, right.FullPathWithFileName, StringComparison.InvariantCultureIgnoreCase );
        }

        public override int GetHashCode() {
            unchecked {
                return this.FullPathWithFileName.GetHashCode();
            }
        }

        /// <summary>
        ///     <para>
        ///         To compare the contents of two <see cref="Document" /> use
        ///         <see cref="IOExtensions.SameContent(Document,Document)" />.
        ///     </para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals( [CanBeNull] object obj ) {
            return obj is Document && Equals( this, ( Document ) obj );
        }

        //[NotNull]
        //public static implicit operator DirectoryInfo( [NotNull] Document document ) {
        //    if ( document == null ) {
        //        throw new ArgumentNullException( "document" );
        //    }
        //    return document.Folder;
        //}

        //[NotNull]
        //public static implicit operator FileInfo( [NotNull] Document document ) {
        //    if ( document == null ) {
        //        throw new ArgumentNullException( "document" );
        //    }
        //    return document._file;
        //}

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for inequality.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=( Document left, Document right ) {
            return !Equals( left, right );
        }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==( Document left, Document right ) {
            return Equals( left, right );
        }

        /// <summary>
        ///     Enumerates a <see cref="Document" /> as a sequence of <see cref="Byte" />.
        /// </summary>
        /// <returns></returns>
        public IEnumerable< byte > AsByteArray() {
            var info = new FileInfo( this.FullPathWithFileName );
            return info.AsByteArray();
        }
    }
}
