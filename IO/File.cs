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
// "Librainian2/File.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.IO {
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security;
    using Annotations;
    using Librainian.Extensions;

    /// <summary>
    ///     <para>Container for a file, the parent folder, and the file size from the given fullpath.</para>
    ///     <seealso cref="Extensions.SameContent(File,File)" />
    /// </summary>
    [DataContract( IsReference = true )]
    [Immutable]
    public sealed class File : IEquatable< File > {
        /// <summary>
        ///     <para>The extension of the file parsed in the ctor.</para>
        /// </summary>
        [NotNull] public readonly String Extension;

        /// <summary>
        ///     <para>The <see cref="FileInfo" /> parsed in the ctor.</para>
        /// </summary>
        [NotNull] public readonly FileInfo Info;

        /// <summary>
        ///     The last known size of the file.
        /// </summary>
        public readonly UInt64 Size;

        [NotNull] private readonly Lazy< DirectoryInfo > _lazyFolder;

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
        public File( [NotNull] String fullPathWithFilename ) {
            if ( String.IsNullOrWhiteSpace( fullPathWithFilename ) ) {
                throw new ArgumentNullException( "fullPathWithFilename" );
            }

            fullPathWithFilename = fullPathWithFilename.Trim();
            if ( String.IsNullOrWhiteSpace( fullPathWithFilename ) ) {
                throw new ArgumentNullException( "fullPathWithFilename" );
            }

            this.Extension = Path.GetExtension( path: fullPathWithFilename );
            this.Info = new FileInfo( fileName: fullPathWithFilename );
            this.Size = ( UInt64 ) this.Info.Length;
            this._lazyFolder = new Lazy< DirectoryInfo >( () => this.Info.Directory );
        }

        [NotNull]
        public DirectoryInfo Folder { get { return this._lazyFolder.Value; } }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( [CanBeNull] File other ) {
            return !ReferenceEquals( null, other ) && Equals( this, other );
        }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] File left, [CanBeNull] File right ) {
            if ( left == null ) {
                throw new ArgumentNullException( "left" );
            }
            if ( right == null ) {
                throw new ArgumentNullException( "right" );
            }
            return String.Equals( left.Info.FullName, right.Info.FullName, StringComparison.InvariantCultureIgnoreCase ) && left.Size == right.Size;
        }

        public override int GetHashCode() {
            unchecked {
                return ( this.Info.GetHashCode()*397 ) ^ this.Size.GetHashCode();
            }
        }

        public override Boolean Equals( [CanBeNull] object obj ) {
            return obj is File && Equals( this, ( File ) obj );
        }

        [NotNull]
        public static implicit operator DirectoryInfo( [NotNull] File file ) {
            if ( file == null ) {
                throw new ArgumentNullException( "file" );
            }
            return file.Folder;
        }

        [NotNull]
        public static implicit operator FileInfo( [NotNull] File file ) {
            if ( file == null ) {
                throw new ArgumentNullException( "file" );
            }
            return file.Info;
        }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for inequality.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=( File left, File right ) {
            return !Equals( left, right );
        }

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==( File left, File right ) {
            return Equals( left, right );
        }
    }
}
