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
// Project: "Librainian", File: "File.cs" was last formatted by Protiguous on 2020/03/16 at 2:57 PM.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo( assemblyName: "Tests" )]

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

        private static Encoding _UTF8NoBOM;

        [NotNull]
        public static Encoding UTF8NoBOM => _UTF8NoBOM ??= new UTF8Encoding( encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true );

        public static void AppendAllLines( [NotNull] String path, [NotNull] IEnumerable<String> contents ) =>
            AppendAllLines( path: path.ThrowIfBlank(), contents: contents, encoding: Encoding.UTF8 );

        public static void AppendAllLines( [NotNull] String path, [NotNull] IEnumerable<String> contents, [NotNull] Encoding encoding ) {

            const Boolean append = true;

            using var writer = CreateStreamWriter( path: path.ThrowIfBlank(), append: append, encoding: encoding );

            foreach ( var line in contents ) {
                writer.WriteLine( value: line );
            }
        }

        public static void AppendAllText( [NotNull] String path, [CanBeNull] String? contents ) =>
            AppendAllText( path: path.ThrowIfBlank(), contents: contents, encoding: Encoding.UTF8 );

        public static void AppendAllText( [NotNull] String path, [CanBeNull] String? contents, [NotNull] Encoding encoding ) {

            const Boolean append = true;

            using var writer = CreateStreamWriter( path: path.ThrowIfBlank(), append: append, encoding: encoding );

            writer.Write( value: contents );
        }

        [NotNull]
        public static StreamWriter AppendText( [NotNull] String path ) => CreateStreamWriter( path: path.ThrowIfBlank(), append: true );

        public static void Copy( [NotNull] String sourceFileName, [NotNull] String destFileName ) =>
            Copy( sourcePath: sourceFileName.ThrowIfBlank(), destinationPath: destFileName.ThrowIfBlank(), overwrite: false );

        /// <summary>Copies the specified file to a specified new file, indicating whether to overwrite an existing file.</summary>
        /// <param name="sourcePath">A <see cref="String" /> containing the path of the file to copy.</param>
        /// <param name="destinationPath">A <see cref="String" /> containing the new path of the file.</param>
        /// <param name="overwrite"><see langword="true" /> if <paramref name="destinationPath" /> should be overwritten if it refers to an existing file, otherwise, <see langword="false" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> is an empty string (""), contains only white space, or contains one or more invalid characters as defined
        /// in <see cref="Librainian.OperatingSystem.FileSystem.Pri.LongPath.Path.GetInvalidPathChars()" />.
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
            var normalizedSourcePath = sourcePath.ThrowIfBlank().NormalizeLongPath( parameterName: "sourcePath" );
            var normalizedDestinationPath = destinationPath.ThrowIfBlank().NormalizeLongPath( parameterName: "destinationPath" );

            if ( !NativeMethods.CopyFile( src: normalizedSourcePath, dst: normalizedDestinationPath, failIfExists: !overwrite ) ) {
                throw Common.GetExceptionFromLastWin32Error();
            }
        }

        [NotNull]
        public static FileStream Create( [NotNull] String path ) => Create( path: path.ThrowIfBlank(), bufferSize: Common.DefaultBufferSize );

        [NotNull]
        public static FileStream Create( [NotNull] String path, Int32 bufferSize ) =>
            Open( path: path.ThrowIfBlank(), mode: FileMode.Create, access: FileAccess.ReadWrite, share: FileShare.None, bufferSize: bufferSize, options: FileOptions.None );

        [NotNull]
        public static FileStream Create( [NotNull] String path, Int32 bufferSize, FileOptions options ) =>
            Open( path: path.ThrowIfBlank(), mode: FileMode.Create, access: FileAccess.ReadWrite, share: FileShare.None, bufferSize: bufferSize, options: options );

        /// <remarks>replaces "new StreamReader(path, true|false)"</remarks>
        [NotNull]
        public static StreamReader CreateStreamReader( [NotNull] String path, [NotNull] Encoding encoding, Boolean detectEncodingFromByteOrderMarks, Int32 bufferSize ) {

            var fileStream = Open( path: path.ThrowIfBlank(), mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: Common.DefaultBufferSize,
                options: FileOptions.SequentialScan );

            return new StreamReader( stream: fileStream, encoding: encoding, detectEncodingFromByteOrderMarks: detectEncodingFromByteOrderMarks, bufferSize: bufferSize );
        }

        /// <remarks>replaces "new StreamWriter(path, true|false)"</remarks>
        [NotNull]
        public static StreamWriter CreateStreamWriter( [NotNull] String path, Boolean append ) {

            var fileMode = append ? FileMode.Append : FileMode.Create;

            var fileStream = Open( path: path.ThrowIfBlank(), mode: fileMode, access: FileAccess.Write, share: FileShare.Read, bufferSize: Common.DefaultBufferSize,
                options: FileOptions.SequentialScan );

            return new StreamWriter( stream: fileStream, encoding: UTF8NoBOM, bufferSize: Common.DefaultBufferSize );
        }

        [NotNull]
        public static StreamWriter CreateStreamWriter( [NotNull] String path, Boolean append, [NotNull] Encoding encoding ) {

            var fileMode = append ? FileMode.Append : FileMode.Create;

            var fileStream = Open( path: path.ThrowIfBlank(), mode: fileMode, access: FileAccess.Write, share: FileShare.Read, bufferSize: Common.DefaultBufferSize,
                options: FileOptions.SequentialScan );

            return new StreamWriter( stream: fileStream, encoding: encoding, bufferSize: Common.DefaultBufferSize );
        }

        [NotNull]
        public static StreamWriter CreateText( [NotNull] String path ) {

            var fileStream = Open( path: path.ThrowIfBlank(), mode: FileMode.Create, access: FileAccess.Write, share: FileShare.Read, bufferSize: Common.DefaultBufferSize,
                options: FileOptions.SequentialScan );

            return new StreamWriter( stream: fileStream, encoding: UTF8NoBOM, bufferSize: Common.DefaultBufferSize );
        }

        [NotNull]
        public static StreamWriter CreateText( [NotNull] String path, [NotNull] Encoding encoding ) =>
            CreateStreamWriter( path: path.ThrowIfBlank(), append: false, encoding: encoding );

        public static void Decrypt( [NotNull] String path ) {

            var fullPath = path.ThrowIfBlank().GetFullPath();
            var normalizedPath = fullPath.NormalizeLongPath();

            if ( NativeMethods.DecryptFile( path: normalizedPath, reservedMustBeZero: 0 ) ) {
                return;
            }

            var errorCode = Marshal.GetLastWin32Error();

            if ( errorCode == NativeMethods.ERROR_ACCESS_DENIED ) {
                var di = new DriveInfo( driveName: normalizedPath.GetPathRoot() );

                if ( !String.Equals( a: "NTFS", b: di.DriveFormat ) ) {
                    throw new NotSupportedException( message: "NTFS drive required for file encryption" );
                }
            }

            Common.ThrowIOError( errorCode: errorCode, maybeFullPath: fullPath );
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

            if ( !NativeMethods.DeleteFile( lpFileName: normalizedPath ) ) {
                throw Common.GetExceptionFromLastWin32Error();
            }
        }

        public static void Encrypt( [NotNull] String path ) {

            var fullPath = path.ThrowIfBlank().GetFullPath();
            var normalizedPath = fullPath.NormalizeLongPath();

            if ( NativeMethods.EncryptFile( path: normalizedPath ) ) {
                return;
            }

            var errorCode = Marshal.GetLastWin32Error();

            if ( errorCode == NativeMethods.ERROR_ACCESS_DENIED ) {
                var di = new DriveInfo( driveName: normalizedPath.GetPathRoot() );

                if ( !String.Equals( a: "NTFS", b: di.DriveFormat ) ) {
                    throw new NotSupportedException( message: "NTFS drive required for file encryption" );
                }
            }

            Common.ThrowIOError( errorCode: errorCode, maybeFullPath: fullPath );
        }

        /// <summary>Returns a value indicating whether the specified path refers to an existing file.</summary>
        /// <param name="path">A <see cref="String" /> containing the path to check.</param>
        /// <returns><see langword="true" /> if <paramref name="path" /> refers to an existing file; otherwise, <see langword="false" />.</returns>
        /// <remarks>
        /// Note that this method will return false if any error occurs while trying to determine if the specified file exists. This includes situations that would normally result in
        /// thrown exceptions including (but not limited to); passing in a file name with invalid or too many characters, an I/O error such as a failing or missing disk, or if the caller does
        /// not have Windows or Code Access Security (CAS) permissions to to read the file.
        /// </remarks>
        public static Boolean Exists( [NotNull] String path ) => path.ThrowIfBlank().Exists( isDirectory: out var isDirectory ) && !isDirectory;

        public static FileAttributes GetAttributes( [NotNull] String path ) => path.ThrowIfBlank().GetFileAttributes();

        public static DateTime GetCreationTime( [NotNull] this String path ) => path.ThrowIfBlank().GetCreationTimeUtc().ToLocalTime();

        public static DateTime GetCreationTimeUtc( [NotNull] this String path ) => new FileInfo( fileName: path ).CreationTimeUtc;

        [NotNull]

        //[SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "handle is stored by caller" )]
        public static SafeFileHandle GetFileHandle( [NotNull] String normalizedPath, FileMode mode, FileAccess access, FileShare share, FileOptions options ) {

            var append = mode == FileMode.Append;

            if ( append ) {
                mode = FileMode.OpenOrCreate;
            }

            var underlyingAccess = GetUnderlyingAccess( access: access );

            var handle = NativeMethods.CreateFile( lpFileName: normalizedPath.ThrowIfBlank(), dwDesiredAccess: underlyingAccess, dwShareMode: ( UInt32 )share,
                lpSecurityAttributes: IntPtr.Zero, dwCreationDisposition: ( UInt32 )mode, dwFlagsAndAttributes: ( UInt32 )options, hTemplateFile: IntPtr.Zero );

            if ( handle.IsInvalid ) {
                var ex = Common.GetExceptionFromLastWin32Error();
                Debug.WriteLine( message: $"error {ex.Message} with {normalizedPath}{Environment.NewLine}{ex.StackTrace}" );
                Debug.WriteLine( message: $"{mode} {access} {share} {options}" );

                throw ex;
            }

            if ( append ) {
                NativeMethods.SetFilePointer( handle: handle, origin: SeekOrigin.End, offset: 0 );
            }

            return handle;
        }

        public static DateTime GetLastAccessTime( [NotNull] this String path ) => path.ThrowIfBlank().GetLastAccessTimeUtc().ToLocalTime();

        public static DateTime GetLastAccessTimeUtc( [NotNull] this String path ) => new FileInfo( fileName: path.ThrowIfBlank() ).LastAccessTimeUtc;

        public static DateTime GetLastWriteTime( [NotNull] this String path ) => path.ThrowIfBlank().GetLastWriteTimeUtc().ToLocalTime();

        public static DateTime GetLastWriteTimeUtc( [NotNull] this String path ) => new FileInfo( fileName: path.ThrowIfBlank() ).LastWriteTimeUtc;

        public static NativeMethods.EFileAccess GetUnderlyingAccess( FileAccess access ) {
            switch ( access ) {
                case FileAccess.Read: return NativeMethods.EFileAccess.GenericRead;

                case FileAccess.Write: return NativeMethods.EFileAccess.GenericWrite;

                case FileAccess.ReadWrite: return NativeMethods.EFileAccess.GenericRead | NativeMethods.EFileAccess.GenericWrite;

                default: throw new ArgumentOutOfRangeException( paramName: nameof( access ) );
            }
        }

        /// <summary>Moves the specified file to a new location.</summary>
        /// <param name="sourcePath">A <see cref="String" /> containing the path of the file to move.</param>
        /// <param name="destinationPath">A <see cref="String" /> containing the new path of the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="sourcePath" /> and/or <paramref name="destinationPath" /> is an empty string (""), contains only white space, or contains one or more invalid characters as defined
        /// in <see cref="Librainian.OperatingSystem.FileSystem.Pri.LongPath.Path.GetInvalidPathChars()" />.
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
            var normalizedSourcePath = sourcePath.ThrowIfBlank().NormalizeLongPath( parameterName: "sourcePath" );
            var normalizedDestinationPath = destinationPath.ThrowIfBlank().NormalizeLongPath( parameterName: "destinationPath" );

            if ( !NativeMethods.MoveFile( lpPathNameFrom: normalizedSourcePath, lpPathNameTo: normalizedDestinationPath ) ) {
                throw Common.GetExceptionFromLastWin32Error();
            }
        }

        [NotNull]
        public static FileStream Open( [NotNull] String path, FileMode mode ) =>
            Open( path: path.ThrowIfBlank(), mode: mode, access: mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite, share: FileShare.None );

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
            Open( path: path.ThrowIfBlank(), mode: mode, access: access, share: FileShare.None, bufferSize: 0, options: FileOptions.None );

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
            Open( path: path.ThrowIfBlank(), mode: mode, access: access, share: share, bufferSize: 0, options: FileOptions.None );

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

            var handle = GetFileHandle( normalizedPath: normalizedPath, mode: mode, access: access, share: share, options: options );

            return new FileStream( handle: handle, access: access, bufferSize: bufferSize, isAsync: options.HasFlag( flag: FileOptions.Asynchronous ) );
        }

        [NotNull]
        public static FileStream OpenRead( [NotNull] String path ) => Open( path: path.ThrowIfBlank(), mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read );

        [NotNull]
        public static StreamReader OpenText( [NotNull] String path, [NotNull] Encoding encoding ) {

            var stream = Open( path: path.ThrowIfBlank(), mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: Common.DefaultBufferSize,
                options: FileOptions.SequentialScan );

            return new StreamReader( stream: stream, encoding: encoding, detectEncodingFromByteOrderMarks: true, bufferSize: Common.DefaultBufferSize );
        }

        [NotNull]
        public static StreamReader OpenText( [NotNull] String path ) {

            var stream = Open( path: path.ThrowIfBlank(), mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: Common.DefaultBufferSize,
                options: FileOptions.SequentialScan );

            return new StreamReader( stream: stream, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: Common.DefaultBufferSize );
        }

        [NotNull]
        public static FileStream OpenWrite( [NotNull] String path ) =>
            Open( path: path.ThrowIfBlank(), mode: FileMode.OpenOrCreate, access: FileAccess.Write, share: FileShare.None );

        [NotNull]
        public static Byte[] ReadAllBytes( [NotNull] String path ) {

            using var fileStream = Open( path: path.ThrowIfBlank(), mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read );

            var length = fileStream.Length;

            if ( length > Int32.MaxValue ) {
                throw new IOException( message: "File length greater than 2GB." );
            }

            var bytes = new Byte[ length ];
            var offset = 0;

            while ( length > 0 ) {
                var read = fileStream.Read( array: bytes, offset: offset, count: ( Int32 )length );

                if ( read == 0 ) {
                    throw new EndOfStreamException( message: "Read beyond end of file." );
                }

                offset += read;
                length -= read;
            }

            return bytes;
        }

        [NotNull]
        public static IEnumerable<String> ReadAllLines( [NotNull] String path ) => ReadLines( path: path.ThrowIfBlank() ).ToArray();

        [NotNull]
        public static IEnumerable<String> ReadAllLines( [NotNull] String path, [NotNull] Encoding encoding ) =>
            ReadLines( path: path.ThrowIfBlank(), encoding: encoding ).ToArray();

        [NotNull]
        public static String ReadAllText( [NotNull] String path ) => ReadAllText( path: path.ThrowIfBlank(), encoding: Encoding.UTF8 );

        [NotNull]
        public static String ReadAllText( [NotNull] String path, [NotNull] Encoding encoding ) {

            using var streamReader = OpenText( path: path.ThrowIfBlank(), encoding: encoding );

            return streamReader.ReadToEnd();
        }

        [NotNull]
        public static IEnumerable<String> ReadLines( [NotNull] String path ) => ReadAllLines( path: path.ThrowIfBlank(), encoding: Encoding.UTF8 );

        [NotNull]
        [ItemCanBeNull]
        public static IEnumerable<String> ReadLines( [NotNull] String path, [NotNull] Encoding encoding ) {

            var stream = Open( path: path.ThrowIfBlank(), mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: Common.DefaultBufferSize,
                options: FileOptions.SequentialScan );

            using var sr = new StreamReader( stream: stream, encoding: encoding, detectEncodingFromByteOrderMarks: true, bufferSize: Common.DefaultBufferSize );

            while ( !sr.EndOfStream ) {
                yield return sr.ReadLine();
            }
        }

        public static void Replace( [NotNull] String sourceFileName, [NotNull] String destinationFileName, [NotNull] String destinationBackupFileName ) =>
            Replace( sourceFileName: sourceFileName.ThrowIfBlank(), destinationFileName: destinationFileName.ThrowIfBlank(),
                destinationBackupFileName: destinationBackupFileName.ThrowIfBlank(), ignoreMetadataErrors: false );

        public static void Replace( [NotNull] String sourceFileName, [NotNull] String destinationFileName, [NotNull] String destinationBackupFileName,
            Boolean ignoreMetadataErrors ) {

            var fullSrcPath = sourceFileName.ThrowIfBlank().GetFullPath().NormalizeLongPath();
            var fullDestPath = destinationFileName.ThrowIfBlank().GetFullPath().NormalizeLongPath();
            var fullBackupPath = destinationBackupFileName.ThrowIfBlank().GetFullPath().NormalizeLongPath();

            var flags = NativeMethods.REPLACEFILE_WRITE_THROUGH;

            if ( ignoreMetadataErrors ) {
                flags |= NativeMethods.REPLACEFILE_IGNORE_MERGE_ERRORS;
            }

            var r = NativeMethods.ReplaceFile( replacedFileName: fullDestPath, replacementFileName: fullSrcPath, backupFileName: fullBackupPath, dwReplaceFlags: flags,
                lpExclude: IntPtr.Zero, lpReserved: IntPtr.Zero );

            if ( !r ) {
                Common.ThrowIOError( errorCode: Marshal.GetLastWin32Error(), maybeFullPath: String.Empty );
            }
        }

        public static void SetAttributes( [NotNull] String path, FileAttributes fileAttributes ) => path.ThrowIfBlank().SetAttributes( fileAttributes: fileAttributes );

        public static void SetCreationTime( [NotNull] this String path, DateTime creationTime ) =>
            SetCreationTimeUtc( path: path.ThrowIfBlank(), creationTimeUtc: creationTime.ToUniversalTime() );

        public static unsafe void SetCreationTimeUtc( [NotNull] this String path, DateTime creationTimeUtc ) {

            var normalizedPath = path.ThrowIfBlank().NormalizeLongPath();

            using ( var handle = GetFileHandle( normalizedPath: normalizedPath, mode: FileMode.Open, access: FileAccess.Write, share: FileShare.ReadWrite,
                options: FileOptions.None ) ) {
                var fileTime = new FILE_TIME( fileTime: creationTimeUtc.ToFileTimeUtc() );
                var r = NativeMethods.SetFileTime( hFile: handle, creationTime: &fileTime, lastAccessTime: null, lastWriteTime: null );

                if ( !r ) {
                    var errorCode = Marshal.GetLastWin32Error();
                    Common.ThrowIOError( errorCode: errorCode, maybeFullPath: path );
                }
            }
        }

        public static void SetLastAccessTime( [NotNull] String path, DateTime lastAccessTime ) =>
            SetLastAccessTimeUtc( path: path.ThrowIfBlank(), lastAccessTimeUtc: lastAccessTime.ToUniversalTime() );

        public static unsafe void SetLastAccessTimeUtc( [NotNull] String path, DateTime lastAccessTimeUtc ) {

            var normalizedPath = path.ThrowIfBlank().NormalizeLongPath();

            using var handle = GetFileHandle( normalizedPath: normalizedPath, mode: FileMode.Open, access: FileAccess.Write, share: FileShare.ReadWrite,
                options: FileOptions.None );

            var fileTime = new FILE_TIME( fileTime: lastAccessTimeUtc.ToFileTimeUtc() );
            var r = NativeMethods.SetFileTime( hFile: handle, creationTime: null, lastAccessTime: &fileTime, lastWriteTime: null );

            if ( !r ) {
                var errorCode = Marshal.GetLastWin32Error();
                Common.ThrowIOError( errorCode: errorCode, maybeFullPath: path );
            }
        }

        public static void SetLastWriteTime( [NotNull] this String path, DateTime lastWriteTime ) =>
            SetLastWriteTimeUtc( path: path.ThrowIfBlank(), lastWriteTimeUtc: lastWriteTime.ToUniversalTime() );

        public static unsafe void SetLastWriteTimeUtc( [NotNull] String path, DateTime lastWriteTimeUtc ) {

            var normalizedPath = path.ThrowIfBlank().NormalizeLongPath();

            using var handle = GetFileHandle( normalizedPath: normalizedPath, mode: FileMode.Open, access: FileAccess.Write, share: FileShare.ReadWrite,
                options: FileOptions.None );

            var fileTime = new FILE_TIME( fileTime: lastWriteTimeUtc.ToFileTimeUtc() );
            var r = NativeMethods.SetFileTime( hFile: handle, creationTime: null, lastAccessTime: null, lastWriteTime: &fileTime );

            if ( !r ) {
                var errorCode = Marshal.GetLastWin32Error();
                Common.ThrowIOError( errorCode: errorCode, maybeFullPath: path );
            }
        }

        public static void WriteAllBytes( [NotNull] String path, [NotNull] Byte[] bytes ) {

            using var fileStream = Open( path: path.ThrowIfBlank(), mode: FileMode.Create, access: FileAccess.Write, share: FileShare.Read );

            fileStream.Write( array: bytes, offset: 0, count: bytes.Length );
        }

        public static void WriteAllLines( [NotNull] String path, [NotNull] String[] contents ) {

            if ( contents == null ) {
                throw new ArgumentNullException( paramName: nameof( contents ) );
            }

            WriteAllLines( path: path.ThrowIfBlank(), contents: contents, encoding: Encoding.UTF8 );
        }

        public static void WriteAllLines( [NotNull] String path, [NotNull] String[] contents, [NotNull] Encoding encoding ) {

            using var writer = CreateStreamWriter( path: path.ThrowIfBlank(), append: false, encoding: encoding );

            foreach ( var line in contents ) {
                writer.WriteLine( value: line );
            }
        }

        public static void WriteAllLines( [NotNull] String path, [NotNull] IEnumerable<String> contents ) =>
            WriteAllLines( path: path.ThrowIfBlank(), contents: contents, encoding: Encoding.UTF8 );

        public static void WriteAllLines( [NotNull] String path, [NotNull] IEnumerable<String> contents, [NotNull] Encoding encoding ) {

            const Boolean doNotAppend = false;

            using var writer = CreateStreamWriter( path: path.ThrowIfBlank(), append: doNotAppend, encoding: encoding );

            foreach ( var line in contents ) {
                writer.WriteLine( value: line );
            }
        }

        public static void WriteAllText( [NotNull] String path, [NotNull] String contents ) =>
            WriteAllText( path: path.ThrowIfBlank(), contents: contents, encoding: UTF8NoBOM );

        public static void WriteAllText( [NotNull] String path, [NotNull] String contents, [NotNull] Encoding encoding ) {

            const Boolean doNotAppend = false;

            using var sw = CreateStreamWriter( path: path.ThrowIfBlank(), append: doNotAppend, encoding: encoding );

            sw.Write( value: contents );
        }
    }
}