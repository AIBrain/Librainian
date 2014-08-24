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

#endregion License & Information

namespace Librainian.IO {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Numerics;
    using System.Runtime.Serialization;
    using System.Security;
    using Annotations;
    using Collections;
    using Extensions;
    using Magic;
    using Maths;
    using Parsing;

    /// <summary>
    ///     <para>A wrapper for a file, the extension, the [parent] folder, and the file's size all from a given full path.</para>
    ///     <para>Also contains static string versions from <see cref="Path" /></para>
    /// </summary>
    /// <seealso cref="IOExtensions.SameContent(Document,Document)" />
    [DataContract( IsReference = true )]
    [Immutable]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    public class Document : IEquatable<Document>, IEnumerable<Byte> {

        [UsedImplicitly]
        private String DebuggerDisplay { get { return this.FullPathWithFileName; } }

        public static readonly Document Empty = new Document();

        /// <summary>
        ///     "/"
        /// </summary>
        [NotNull]
        public static readonly List<char> InvalidPathChars = new List<char>( Path.GetInvalidPathChars() );

        /// <summary>
        ///     <para>The extension of the <see cref="FileName" />, including the ".".</para>
        /// </summary>
        [NotNull]
        public readonly String Extension;

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <seealso cref="Path.GetFileNameWithoutExtension" />
        [NotNull]
        public readonly String FileName;

        [NotNull]
        private readonly FileInfo _fileInfo;

        /// <summary>
        ///     <para>FYI: A folder does not always ends with a <see cref="IO.Folder.FolderSeparator" />... .. . / \</para>
        /// </summary>
        [NotNull]
        public readonly Folder Folder;

        /// <summary>
        ///     <para>The <see cref="Folder" /> combined with the <see cref="FileName" />.</para>
        /// </summary>
        [NotNull]
        public readonly String FullPathWithFileName;

        /// <summary>
        ///     <para>The last known size of the file.</para>
        /// </summary>
        public UInt64 Size { get { return this.Length; } }

        /// <summary>
        /// <para>The last known size of the file.</para>
        /// </summary>
        public UInt64 Length {
            get {
                this.Refresh();
                return ( UInt64 )this._fileInfo.Length;
            }
        }

        static Document() {
            InvalidPathChars.Fix();
        }

        public void Refresh() {
            this._fileInfo.Refresh();
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
            this.OriginalPathWithFileName = fullPathWithFilename;

            this._fileInfo = new FileInfo( fullPathWithFilename );

            this.Folder = new Folder( Path.GetDirectoryName( fullPathWithFilename ) );

            this.FileName = Path.GetFileName( fullPathWithFilename );
            this.Extension = Path.GetExtension( fullPathWithFilename );

            this.FullPathWithFileName = Path.Combine( this.Folder.FullName, this.FileName );
        }

        public readonly String OriginalPathWithFileName;

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref=""></exception>
        private Document() {
            this.Folder = new Folder( Directory.GetCurrentDirectory() );
            Document document;
            if ( !this.Folder.TryGetTempDocument( out document ) ) {
                throw new DirectoryNotFoundException();
            }
            document.DeepClone( this );
        }

        /// <summary>
        ///     Returns true if the <see cref="Document" /> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        public Boolean Exists { get { return File.Exists( this.FullPathWithFileName ); } }

        /*
                /// <summary>
                ///     Returns true if the <see cref="Document.Folder" /> currently exists.
                /// </summary>
                /// <exception cref="IOException"></exception>
                public Boolean FolderExists { get { return this.Folder.Exists; } }
        */

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

        /// <summary>
        /// <para>Returns the file's size or 0.</para>
        /// </summary>
        /// <returns></returns>
        public UBigInteger GetSize() {
            var info = new FileInfo( this.FullPathWithFileName );
            return !info.Exists ? UBigInteger.Zero : info.Length;
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
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }
            if ( ReferenceEquals( left, null ) || ReferenceEquals( right, null )  ) {
                return false;
            }
            return left.Size == right.Size && left.FullPathWithFileName.Like(right.FullPathWithFileName );
        }

        public override int GetHashCode() {
            return this.FullPathWithFileName.GetHashCode();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<byte> GetEnumerator() {
            return this.AsByteArray().GetEnumerator();
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
            return obj is Document && Equals( this, ( Document )obj );
        }

        //public delegate void IntDelegate( int Int );

        //public event IntDelegate FileCopyProgress = i => { };

        /// <summary>
        /// Returns true if a file copy was started.
        /// </summary>
        /// <param name="destination">can this be a folder or a file?!?!</param>
        /// <param name="onProgress"></param>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        public Boolean CopyFileWithProgress( string destination, Action<Percentage> onProgress, Action onCompleted ) {
            var webClient = Ioc.Container.TryGet<WebClient>();

            if ( webClient == null ) {
                return false;
            }
            webClient.DownloadProgressChanged += ( sender, args ) => {
                var percentage = new Percentage( ( BigInteger )args.BytesReceived, args.TotalBytesToReceive );
                if ( onProgress != null ) {
                    onProgress( percentage );
                }
            };
            webClient.DownloadFileCompleted += ( sender, args ) => {
                if ( onCompleted != null ) {
                    onCompleted();
                }
            };
            webClient.DownloadFileAsync( new Uri( this.FullPathWithFileName ), destination );

            return true;
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
        public IEnumerable<byte> AsByteArray() {
            var info = new FileInfo( this.FullPathWithFileName );
            return info.AsByteArray();
        }

        /// <summary>
        /// <para>If the file does not exist, it is created.</para>
        /// <para>Then the <paramref name="text"/> is appended to the file.</para>
        /// </summary>
        /// <param name="text"></param>
        public void AppendText( String text ) {
            if ( this.Exists ) {
                using ( var writer = File.AppendText( this.FullPathWithFileName ) ) {
                    writer.WriteLine( text );
                    writer.Flush();
                    writer.Close();
                }
            }
            else {
                using ( var writer = File.CreateText( this.FullPathWithFileName ) ) {
                    writer.WriteLine( text );
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        /// <summary>
        /// <para>Returns true if the <see cref="Document"/> no longer exists.</para>
        /// </summary>
        /// <returns></returns>
        public Boolean Delete() {
            this._fileInfo.Delete();
            return Exists;
        }
    }
}