// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "File.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "File.cs" was last formatted by Protiguous on 2020/03/18 at 10:26 AM.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo( "Tests" )]

namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using JetBrains.Annotations;
    using Microsoft.Win32.SafeHandles;

    public static class File {

        [NotNull]
        public static Encoding UTF8NoBOM => _UTF8NoBOM ??= new UTF8Encoding( false, true );

        private static Encoding _UTF8NoBOM;

        public static void AppendAllLines( [NotNull] String path, [NotNull] IEnumerable<String> contents ) => AppendAllLines( path.ThrowIfBlank(), contents, Encoding.UTF8 );

        public static void AppendAllLines( [NotNull] String path, [NotNull] IEnumerable<String> contents, [NotNull] Encoding encoding ) {

            const Boolean append = true;

            using var writer = CreateStreamWriter( path.ThrowIfBlank(), append, encoding );

            foreach ( var line in contents ) {
                writer.WriteLine( line );
            }
        }

        public static void AppendAllText( [NotNull] String path, [CanBeNull] String? contents ) => AppendAllText( path.ThrowIfBlank(), contents, Encoding.UTF8 );

        public static void AppendAllText( [NotNull] String path, [CanBeNull] String? contents, [NotNull] Encoding encoding ) {

            const Boolean append = true;

            using var writer = CreateStreamWriter( path.ThrowIfBlank(), append, encoding );

            writer.Write( contents );
        }

        [NotNull]
        public static StreamWriter AppendText( [NotNull] String path ) => CreateStreamWriter( path.ThrowIfBlank(), true );

        public static void Copy( [NotNull] String sourceFileName, [NotNull] String destFileName ) => Copy( sourceFileName.ThrowIfBlank(), destFileName.ThrowIfBlank(), false );

        /// <summary>Copies the specified file to a specified new file, indicating whether to overwrite an existing file.</summary>
        /// <param name="sourcePath">A <see cref="String" /> containing the path of the file to copy.</param>
        /// <param name="destinationPath">A <see cref="String" /> containing the new path of the file.</param>
        /// <param name="overwrite"><see langword="true" /> if <paramref name="destinationPath" /> should be overwritten if it refers to an existing file, otherwise, <see langword="false" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> is an empty string (""), contains only white space, or contains one
        /// or more invalid characters as defined in <see cref="Librainian.OperatingSystem.FileSystem.Pri.LongPath.Path.GetInvalidPathChars()" />.
        /// <para>-or-</para>
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> contains one or more components that exceed the drive-defined maximum length. For example, on
        /// Windows-based platforms, components must not exceed 255 characters.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> exceeds the system-defined maximum length. For example, on
        /// Windows-based platforms, paths must not exceed 32,000 characters.
        /// </exception>
        /// <exception cref="FileNotFoundException"><paramref name="sourcePath" /> could not be found.</exception>
        /// <exception cref="DirectoryNotFoundException">One or more directories in <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> could not be found.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required access permissions.
        /// <para>-or-</para>
        /// <paramref name="overwrite" /> is true and <paramref name="destinationPath" /> refers to a file that is read-only.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="overwrite" /> is false and <paramref name="destinationPath" /> refers to a file that already exists.
        /// <para>-or-</para>
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> is a directory.
        /// <para>-or-</para>
        /// <paramref name="overwrite" /> is true and <paramref name="destinationPath" /> refers to a file that already exists and is in use.
        /// <para>-or-</para>
        /// <paramref name="sourcePath" /> refers to a file that is in use.
        /// <para>-or-</para>
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> specifies a device that is not ready.
        /// </exception>
        public static void Copy( [NotNull] String sourcePath, [NotNull] String destinationPath, Boolean overwrite ) {
            var normalizedSourcePath = sourcePath.ThrowIfBlank().NormalizeLongPath( "sourcePath" );
            var normalizedDestinationPath = destinationPath.ThrowIfBlank().NormalizeLongPath( "destinationPath" );

            if ( !NativeMethods.CopyFile( normalizedSourcePath, normalizedDestinationPath, !overwrite ) ) {
                throw Common.GetExceptionFromLastWin32Error();
            }
        }

        [NotNull]
        public static FileStream Create( [NotNull] String path ) => Create( path.ThrowIfBlank(), Common.DefaultBufferSize );

        [NotNull]
        public static FileStream Create( [NotNull] String path, Int32 bufferSize ) =>
            Open( path.ThrowIfBlank(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize, FileOptions.None );

        [NotNull]
        public static FileStream Create( [NotNull] String path, Int32 bufferSize, FileOptions options ) =>
            Open( path.ThrowIfBlank(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize, options );

        /// <remarks>replaces "new StreamReader(path, true|false)"</remarks>
        [NotNull]
        public static StreamReader CreateStreamReader( [NotNull] String path, [NotNull] Encoding encoding, Boolean detectEncodingFromByteOrderMarks, Int32 bufferSize ) {

            var fileStream = Open( path.ThrowIfBlank(), FileMode.Open, FileAccess.Read, FileShare.Read, Common.DefaultBufferSize, FileOptions.SequentialScan );

            return new StreamReader( fileStream, encoding, detectEncodingFromByteOrderMarks, bufferSize );
        }

        /// <remarks>replaces "new StreamWriter(path, true|false)"</remarks>
        [NotNull]
        public static StreamWriter CreateStreamWriter( [NotNull] String path, Boolean append ) {

            var fileMode = append ? FileMode.Append : FileMode.Create;

            var fileStream = Open( path.ThrowIfBlank(), fileMode, FileAccess.Write, FileShare.Read, Common.DefaultBufferSize, FileOptions.SequentialScan );

            return new StreamWriter( fileStream, UTF8NoBOM, Common.DefaultBufferSize );
        }

        [NotNull]
        public static StreamWriter CreateStreamWriter( [NotNull] String path, Boolean append, [NotNull] Encoding encoding ) {

            var fileMode = append ? FileMode.Append : FileMode.Create;

            var fileStream = Open( path.ThrowIfBlank(), fileMode, FileAccess.Write, FileShare.Read, Common.DefaultBufferSize, FileOptions.SequentialScan );

            return new StreamWriter( fileStream, encoding, Common.DefaultBufferSize );
        }

        [NotNull]
        public static StreamWriter CreateText( [NotNull] String path ) {

            var fileStream = Open( path.ThrowIfBlank(), FileMode.Create, FileAccess.Write, FileShare.Read, Common.DefaultBufferSize, FileOptions.SequentialScan );

            return new StreamWriter( fileStream, UTF8NoBOM, Common.DefaultBufferSize );
        }

        [NotNull]
        public static StreamWriter CreateText( [NotNull] String path, [NotNull] Encoding encoding ) => CreateStreamWriter( path.ThrowIfBlank(), false, encoding );

        public static void Decrypt( [NotNull] String path ) {

            var fullPath = path.ThrowIfBlank().GetFullPath();
            var normalizedPath = fullPath.NormalizeLongPath();

            if ( NativeMethods.DecryptFile( normalizedPath, 0 ) ) {
                return;
            }

            var errorCode = Marshal.GetLastWin32Error();

            if ( errorCode == NativeMethods.ERROR_ACCESS_DENIED ) {
                var di = new DriveInfo( normalizedPath.GetPathRoot() );

                if ( !String.Equals( "NTFS", di.DriveFormat ) ) {
                    throw new NotSupportedException( "NTFS drive required for file encryption" );
                }
            }

            Common.ThrowIOError( errorCode, fullPath );
        }

        /// <summary>Deletes the specified file.</summary>
        /// <param name="path">A <see cref="String" /> containing the path of the file to delete.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid characters as defined in
        /// <see cref="Librainian.OperatingSystem.FileSystem.Pri.LongPath.Path.GetInvalidPathChars()" />.
        /// <para>-or-</para>
        /// <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example, on Windows-based platforms, components must not exceed 255
        /// characters.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must not exceed 32,000
        /// characters.
        /// </exception>
        /// <exception cref="FileNotFoundException"><paramref name="path" /> could not be found.</exception>
        /// <exception cref="DirectoryNotFoundException">One or more directories in <paramref name="path" /> could not be found.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required access permissions.
        /// <para>-or-</para>
        /// <paramref name="path" /> refers to a file that is read-only.
        /// <para>-or-</para>
        /// <paramref name="path" /> is a directory.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="path" /> refers to a file that is in use.
        /// <para>-or-</para>
        /// <paramref name="path" /> specifies a device that is not ready.
        /// </exception>
        public static void Delete( [NotNull] String path ) {

            var normalizedPath = path.ThrowIfBlank().NormalizeLongPath();

            if ( !path.Exists() ) {
                return;
            }

            if ( !NativeMethods.DeleteFile( normalizedPath ) ) {
                throw Common.GetExceptionFromLastWin32Error();
            }
        }

        public static void Encrypt( [NotNull] String path ) {

            var fullPath = path.ThrowIfBlank().GetFullPath();
            var normalizedPath = fullPath.NormalizeLongPath();

            if ( NativeMethods.EncryptFile( normalizedPath ) ) {
                return;
            }

            var errorCode = Marshal.GetLastWin32Error();

            if ( errorCode == NativeMethods.ERROR_ACCESS_DENIED ) {
                var di = new DriveInfo( normalizedPath.GetPathRoot() );

                if ( !String.Equals( "NTFS", di.DriveFormat ) ) {
                    throw new NotSupportedException( "NTFS drive required for file encryption" );
                }
            }

            Common.ThrowIOError( errorCode, fullPath );
        }

        /// <summary>Returns a value indicating whether the specified path refers to an existing file.</summary>
        /// <param name="path">A <see cref="String" /> containing the path to check.</param>
        /// <returns><see langword="true" /> if <paramref name="path" /> refers to an existing file; otherwise, <see langword="false" />.</returns>
        /// <remarks>
        /// Note that this method will return false if any error occurs while trying to determine if the specified file exists. This includes situations that would normally result in
        /// thrown exceptions including (but not limited to); passing in a file name with invalid or too many characters, an I/O error such as a failing or missing disk, or if the caller does
        /// not have Windows or Code Access Security (CAS) permissions to to read the file.
        /// </remarks>
        public static Boolean Exists( [NotNull] String path ) => path.ThrowIfBlank().Exists( out var isDirectory ) && !isDirectory;

        public static FileAttributes GetAttributes( [NotNull] String path ) => path.ThrowIfBlank().GetFileAttributes();

        public static DateTime GetCreationTime( [NotNull] this String path ) => path.ThrowIfBlank().GetCreationTimeUtc().ToLocalTime();

        public static DateTime GetCreationTimeUtc( [NotNull] this String path ) => new FileInfo( path ).CreationTimeUtc;

        [NotNull]

        //[SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "handle is stored by caller" )]
        public static SafeFileHandle GetFileHandle( [NotNull] String normalizedPath, FileMode mode, FileAccess access, FileShare share, FileOptions options ) {

            var append = mode == FileMode.Append;

            if ( append ) {
                mode = FileMode.OpenOrCreate;
            }

            var underlyingAccess = GetUnderlyingAccess( access );

            var handle = NativeMethods.CreateFile( normalizedPath.ThrowIfBlank(), underlyingAccess, ( UInt32 ) share, IntPtr.Zero, ( UInt32 ) mode, ( UInt32 ) options,
                IntPtr.Zero );

            if ( handle.IsInvalid ) {
                var ex = Common.GetExceptionFromLastWin32Error();
                Debug.WriteLine( $"error {ex.Message} with {normalizedPath}{Environment.NewLine}{ex.StackTrace}" );
                Debug.WriteLine( $"{mode} {access} {share} {options}" );

                throw ex;
            }

            if ( append ) {
                NativeMethods.SetFilePointer( handle, SeekOrigin.End, 0 );
            }

            return handle;
        }

        public static DateTime GetLastAccessTime( [NotNull] this String path ) => path.ThrowIfBlank().GetLastAccessTimeUtc().ToLocalTime();

        public static DateTime GetLastAccessTimeUtc( [NotNull] this String path ) => new FileInfo( path.ThrowIfBlank() ).LastAccessTimeUtc;

        public static DateTime GetLastWriteTime( [NotNull] this String path ) => path.ThrowIfBlank().GetLastWriteTimeUtc().ToLocalTime();

        public static DateTime GetLastWriteTimeUtc( [NotNull] this String path ) => new FileInfo( path.ThrowIfBlank() ).LastWriteTimeUtc;

        public static NativeMethods.EFileAccess GetUnderlyingAccess( FileAccess access ) {
            switch ( access ) {
                case FileAccess.Read: return NativeMethods.EFileAccess.GenericRead;

                case FileAccess.Write: return NativeMethods.EFileAccess.GenericWrite;

                case FileAccess.ReadWrite: return NativeMethods.EFileAccess.GenericRead | NativeMethods.EFileAccess.GenericWrite;

                default: throw new ArgumentOutOfRangeException( nameof( access ) );
            }
        }

        /// <summary>Moves the specified file to a new location.</summary>
        /// <param name="sourcePath">A <see cref="String" /> containing the path of the file to move.</param>
        /// <param name="destinationPath">A <see cref="String" /> containing the new path of the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> is an empty string (""), contains only white space, or contains one
        /// or more invalid characters as defined in <see cref="Librainian.OperatingSystem.FileSystem.Pri.LongPath.Path.GetInvalidPathChars()" />.
        /// <para>-or-</para>
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> contains one or more components that exceed the drive-defined maximum length. For example, on
        /// Windows-based platforms, components must not exceed 255 characters.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> exceeds the system-defined maximum length. For example, on
        /// Windows-based platforms, paths must not exceed 32,000 characters.
        /// </exception>
        /// <exception cref="FileNotFoundException"><paramref name="sourcePath" /> could not be found.</exception>
        /// <exception cref="DirectoryNotFoundException">One or more directories in <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> could not be found.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required access permissions.</exception>
        /// <exception cref="IOException">
        /// <paramref name="destinationPath" /> refers to a file that already exists.
        /// <para>-or-</para>
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> is a directory.
        /// <para>-or-</para>
        /// <paramref name="sourcePath" /> refers to a file that is in use.
        /// <para>-or-</para>
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> specifies a device that is not ready.
        /// </exception>
        public static void Move( [NotNull] String sourcePath, [NotNull] String destinationPath ) {
            var normalizedSourcePath = sourcePath.ThrowIfBlank().NormalizeLongPath( "sourcePath" );
            var normalizedDestinationPath = destinationPath.ThrowIfBlank().NormalizeLongPath( "destinationPath" );

            if ( !NativeMethods.MoveFile( normalizedSourcePath, normalizedDestinationPath ) ) {
                throw Common.GetExceptionFromLastWin32Error();
            }
        }

        [NotNull]
        public static FileStream Open( [NotNull] String path, FileMode mode ) =>
            Open( path.ThrowIfBlank(), mode, mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite, FileShare.None );

        /// <summary>Opens the specified file.</summary>
        /// <param name="path">A <see cref="String" /> containing the path of the file to open.</param>
        /// <param name="access">One of the <see cref="FileAccess" /> value that specifies the operations that can be performed on the file.</param>
        /// <param name="mode">
        /// One of the <see cref="FileMode" /> values that specifies whether a file is created if one does not exist, and determines whether the contents of existing files
        /// are retained or overwritten.
        /// </param>
        /// <returns>A <see cref="FileStream" /> that provides access to the file specified in <paramref name="path" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid characters as defined in
        /// <see cref="Librainian.OperatingSystem.FileSystem.Pri.LongPath.Path.GetInvalidPathChars()" />.
        /// <para>-or-</para>
        /// <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example, on Windows-based platforms, components must not exceed 255
        /// characters.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must not exceed 32,000
        /// characters.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">One or more directories in <paramref name="path" /> could not be found.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required access permissions.
        /// <para>-or-</para>
        /// <paramref name="path" /> refers to a file that is read-only and <paramref name="access" /> is not <see cref="FileAccess.Read" />.
        /// <para>-or-</para>
        /// <paramref name="path" /> is a directory.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="path" /> refers to a file that is in use.
        /// <para>-or-</para>
        /// <paramref name="path" /> specifies a device that is not ready.
        /// </exception>
        [NotNull]
        public static FileStream Open( [NotNull] String path, FileMode mode, FileAccess access ) =>
            Open( path.ThrowIfBlank(), mode, access, FileShare.None, 0, FileOptions.None );

        /// <summary>Opens the specified file.</summary>
        /// <param name="path">A <see cref="String" /> containing the path of the file to open.</param>
        /// <param name="access">One of the <see cref="FileAccess" /> value that specifies the operations that can be performed on the file.</param>
        /// <param name="mode">
        /// One of the <see cref="FileMode" /> values that specifies whether a file is created if one does not exist, and determines whether the contents of existing files
        /// are retained or overwritten.
        /// </param>
        /// <param name="share">One of the <see cref="FileShare" /> values specifying the type of access other threads have to the file.</param>
        /// <returns>A <see cref="FileStream" /> that provides access to the file specified in <paramref name="path" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid characters as defined in
        /// <see cref="Librainian.OperatingSystem.FileSystem.Pri.LongPath.Path.GetInvalidPathChars()" />.
        /// <para>-or-</para>
        /// <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example, on Windows-based platforms, components must not exceed 255
        /// characters.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must not exceed 32,000
        /// characters.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">One or more directories in <paramref name="path" /> could not be found.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required access permissions.
        /// <para>-or-</para>
        /// <paramref name="path" /> refers to a file that is read-only and <paramref name="access" /> is not <see cref="FileAccess.Read" />.
        /// <para>-or-</para>
        /// <paramref name="path" /> is a directory.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="path" /> refers to a file that is in use.
        /// <para>-or-</para>
        /// <paramref name="path" /> specifies a device that is not ready.
        /// </exception>
        [NotNull]
        public static FileStream Open( [NotNull] String path, FileMode mode, FileAccess access, FileShare share ) =>
            Open( path.ThrowIfBlank(), mode, access, share, 0, FileOptions.None );

        /// <summary>Opens the specified file.</summary>
        /// <param name="path">A <see cref="String" /> containing the path of the file to open.</param>
        /// <param name="access">One of the <see cref="FileAccess" /> value that specifies the operations that can be performed on the file.</param>
        /// <param name="mode">
        /// One of the <see cref="FileMode" /> values that specifies whether a file is created if one does not exist, and determines whether the contents of existing files
        /// are retained or overwritten.
        /// </param>
        /// <param name="share">One of the <see cref="FileShare" /> values specifying the type of access other threads have to the file.</param>
        /// <param name="bufferSize">
        /// An <see cref="Int32" /> containing the number of bytes to buffer for reads and writes to the file, or 0 to specified the default buffer size,
        /// DefaultBufferSize.
        /// </param>
        /// <param name="options">One or more of the <see cref="FileOptions" /> values that describes how to create or overwrite the file.</param>
        /// <returns>A <see cref="FileStream" /> that provides access to the file specified in <paramref name="path" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid characters as defined in
        /// <see cref="Librainian.OperatingSystem.FileSystem.Pri.LongPath.Path.GetInvalidPathChars()" />.
        /// <para>-or-</para>
        /// <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example, on Windows-based platforms, components must not exceed 255
        /// characters.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize" /> is less than 0.</exception>
        /// <exception cref="PathTooLongException">
        /// <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must not exceed 32,000
        /// characters.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">One or more directories in <paramref name="path" /> could not be found.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required access permissions.
        /// <para>-or-</para>
        /// <paramref name="path" /> refers to a file that is read-only and <paramref name="access" /> is not <see cref="FileAccess.Read" />.
        /// <para>-or-</para>
        /// <paramref name="path" /> is a directory.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="path" /> refers to a file that is in use.
        /// <para>-or-</para>
        /// <paramref name="path" /> specifies a device that is not ready.
        /// </exception>
        [NotNull]
        public static FileStream Open( [NotNull] String path, FileMode mode, FileAccess access, FileShare share, Int32 bufferSize, FileOptions options ) {

            const Int32 defaultBufferSize = Common.DefaultBufferSize;

            if ( bufferSize == 0 ) {
                bufferSize = defaultBufferSize;
            }

            var normalizedPath = path.ThrowIfBlank().NormalizeLongPath();

            var handle = GetFileHandle( normalizedPath, mode, access, share, options );

            return new FileStream( handle, access, bufferSize, options.HasFlag( FileOptions.Asynchronous ) );
        }

        [NotNull]
        public static FileStream OpenRead( [NotNull] String path ) => Open( path.ThrowIfBlank(), FileMode.Open, FileAccess.Read, FileShare.Read );

        [NotNull]
        public static StreamReader OpenText( [NotNull] String path, [NotNull] Encoding encoding ) {

            var stream = Open( path.ThrowIfBlank(), FileMode.Open, FileAccess.Read, FileShare.Read, Common.DefaultBufferSize, FileOptions.SequentialScan );

            return new StreamReader( stream, encoding, true, Common.DefaultBufferSize );
        }

        [NotNull]
        public static StreamReader OpenText( [NotNull] String path ) {

            var stream = Open( path.ThrowIfBlank(), FileMode.Open, FileAccess.Read, FileShare.Read, Common.DefaultBufferSize, FileOptions.SequentialScan );

            return new StreamReader( stream, Encoding.UTF8, true, Common.DefaultBufferSize );
        }

        [NotNull]
        public static FileStream OpenWrite( [NotNull] String path ) => Open( path.ThrowIfBlank(), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None );

        [NotNull]
        public static Byte[] ReadAllBytes( [NotNull] String path ) {

            using var fileStream = Open( path.ThrowIfBlank(), FileMode.Open, FileAccess.Read, FileShare.Read );

            var length = fileStream.Length;

            if ( length > Int32.MaxValue ) {
                throw new IOException( "File length greater than 2GB." );
            }

            var bytes = new Byte[ length ];
            var offset = 0;

            while ( length > 0 ) {
                var read = fileStream.Read( bytes, offset, ( Int32 ) length );

                if ( read == 0 ) {
                    throw new EndOfStreamException( "Read beyond end of file." );
                }

                offset += read;
                length -= read;
            }

            return bytes;
        }

        [NotNull]
        public static IEnumerable<String> ReadAllLines( [NotNull] String path ) => ReadLines( path.ThrowIfBlank() ).ToArray();

        [NotNull]
        public static IEnumerable<String> ReadAllLines( [NotNull] String path, [NotNull] Encoding encoding ) => ReadLines( path.ThrowIfBlank(), encoding ).ToArray();

        [NotNull]
        public static String ReadAllText( [NotNull] String path ) => ReadAllText( path.ThrowIfBlank(), Encoding.UTF8 );

        [NotNull]
        public static String ReadAllText( [NotNull] String path, [NotNull] Encoding encoding ) {

            using var streamReader = OpenText( path.ThrowIfBlank(), encoding );

            return streamReader.ReadToEnd();
        }

        [NotNull]
        public static IEnumerable<String> ReadLines( [NotNull] String path ) => ReadAllLines( path.ThrowIfBlank(), Encoding.UTF8 );

        [NotNull]
        [ItemCanBeNull]
        public static IEnumerable<String> ReadLines( [NotNull] String path, [NotNull] Encoding encoding ) {

            var stream = Open( path.ThrowIfBlank(), FileMode.Open, FileAccess.Read, FileShare.Read, Common.DefaultBufferSize, FileOptions.SequentialScan );

            using var sr = new StreamReader( stream, encoding, true, Common.DefaultBufferSize );

            while ( !sr.EndOfStream ) {
                yield return sr.ReadLine();
            }
        }

        public static void Replace( [NotNull] String sourceFileName, [NotNull] String destinationFileName, [NotNull] String destinationBackupFileName ) =>
            Replace( sourceFileName.ThrowIfBlank(), destinationFileName.ThrowIfBlank(), destinationBackupFileName.ThrowIfBlank(), false );

        public static void Replace( [NotNull] String sourceFileName, [NotNull] String destinationFileName, [NotNull] String destinationBackupFileName,
            Boolean ignoreMetadataErrors ) {

            var fullSrcPath = sourceFileName.ThrowIfBlank().GetFullPath().NormalizeLongPath();
            var fullDestPath = destinationFileName.ThrowIfBlank().GetFullPath().NormalizeLongPath();
            var fullBackupPath = destinationBackupFileName.ThrowIfBlank().GetFullPath().NormalizeLongPath();

            var flags = NativeMethods.REPLACEFILE_WRITE_THROUGH;

            if ( ignoreMetadataErrors ) {
                flags |= NativeMethods.REPLACEFILE_IGNORE_MERGE_ERRORS;
            }

            var r = NativeMethods.ReplaceFile( fullDestPath, fullSrcPath, fullBackupPath, flags, IntPtr.Zero, IntPtr.Zero );

            if ( !r ) {
                Common.ThrowIOError( Marshal.GetLastWin32Error(), String.Empty );
            }
        }

        public static void SetAttributes( [NotNull] String path, FileAttributes fileAttributes ) => path.ThrowIfBlank().SetAttributes( fileAttributes );

        public static void SetCreationTime( [NotNull] this String path, DateTime creationTime ) => SetCreationTimeUtc( path.ThrowIfBlank(), creationTime.ToUniversalTime() );

        public static unsafe void SetCreationTimeUtc( [NotNull] this String path, DateTime creationTimeUtc ) {

            var normalizedPath = path.ThrowIfBlank().NormalizeLongPath();

            using ( var handle = GetFileHandle( normalizedPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite, FileOptions.None ) ) {
                var fileTime = new FILE_TIME( creationTimeUtc.ToFileTimeUtc() );
                var r = NativeMethods.SetFileTime( handle, &fileTime, null, null );

                if ( !r ) {
                    var errorCode = Marshal.GetLastWin32Error();
                    Common.ThrowIOError( errorCode, path );
                }
            }
        }

        public static void SetLastAccessTime( [NotNull] String path, DateTime lastAccessTime ) =>
            SetLastAccessTimeUtc( path.ThrowIfBlank(), lastAccessTime.ToUniversalTime() );

        public static unsafe void SetLastAccessTimeUtc( [NotNull] String path, DateTime lastAccessTimeUtc ) {

            var normalizedPath = path.ThrowIfBlank().NormalizeLongPath();

            using var handle = GetFileHandle( normalizedPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite, FileOptions.None );

            var fileTime = new FILE_TIME( lastAccessTimeUtc.ToFileTimeUtc() );
            var r = NativeMethods.SetFileTime( handle, null, &fileTime, null );

            if ( !r ) {
                var errorCode = Marshal.GetLastWin32Error();
                Common.ThrowIOError( errorCode, path );
            }
        }

        public static void SetLastWriteTime( [NotNull] this String path, DateTime lastWriteTime ) =>
            SetLastWriteTimeUtc( path.ThrowIfBlank(), lastWriteTime.ToUniversalTime() );

        public static unsafe void SetLastWriteTimeUtc( [NotNull] String path, DateTime lastWriteTimeUtc ) {

            var normalizedPath = path.ThrowIfBlank().NormalizeLongPath();

            using var handle = GetFileHandle( normalizedPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite, FileOptions.None );

            var fileTime = new FILE_TIME( lastWriteTimeUtc.ToFileTimeUtc() );
            var r = NativeMethods.SetFileTime( handle, null, null, &fileTime );

            if ( !r ) {
                var errorCode = Marshal.GetLastWin32Error();
                Common.ThrowIOError( errorCode, path );
            }
        }

        public static void WriteAllBytes( [NotNull] String path, [NotNull] Byte[] bytes ) {

            using var fileStream = Open( path.ThrowIfBlank(), FileMode.Create, FileAccess.Write, FileShare.Read );

            fileStream.Write( bytes, 0, bytes.Length );
        }

        public static void WriteAllLines( [NotNull] String path, [NotNull] String[] contents ) {

            if ( contents == null ) {
                throw new ArgumentNullException( nameof( contents ) );
            }

            WriteAllLines( path.ThrowIfBlank(), contents, Encoding.UTF8 );
        }

        public static void WriteAllLines( [NotNull] String path, [NotNull] String[] contents, [NotNull] Encoding encoding ) {

            using var writer = CreateStreamWriter( path.ThrowIfBlank(), false, encoding );

            foreach ( var line in contents ) {
                writer.WriteLine( line );
            }
        }

        public static void WriteAllLines( [NotNull] String path, [NotNull] IEnumerable<String> contents ) => WriteAllLines( path.ThrowIfBlank(), contents, Encoding.UTF8 );

        public static void WriteAllLines( [NotNull] String path, [NotNull] IEnumerable<String> contents, [NotNull] Encoding encoding ) {

            const Boolean doNotAppend = false;

            using var writer = CreateStreamWriter( path.ThrowIfBlank(), doNotAppend, encoding );

            foreach ( var line in contents ) {
                writer.WriteLine( line );
            }
        }

        public static void WriteAllText( [NotNull] String path, [NotNull] String contents ) => WriteAllText( path.ThrowIfBlank(), contents, UTF8NoBOM );

        public static void WriteAllText( [NotNull] String path, [NotNull] String contents, [NotNull] Encoding encoding ) {

            const Boolean doNotAppend = false;

            using var sw = CreateStreamWriter( path.ThrowIfBlank(), doNotAppend, encoding );

            sw.Write( contents );
        }

    }

}