// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Directory.cs" last formatted on 2020-08-14 at 8:39 PM.

namespace Librainian.FileSystem.Pri.LongPath {

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.IO;
	using System.Runtime.InteropServices;
	using JetBrains.Annotations;
	using Microsoft.Win32.SafeHandles;

	public static class Directory {

		[NotNull]
		private static DirectoryInfo CreateDirectoryUnc( [NotNull] String path ) {
			path = path.ThrowIfBlank();
			var length = path.Length;

			if ( length >= 2 && path[length - 1].IsDirectorySeparator() ) {
				--length;
			}

			var rootLength = path.GetRootLength();

			var pathComponents = new List<String>();

			if ( length > rootLength ) {
				for ( var index = length - 1; index >= rootLength; --index ) {
					var subPath = path.Substring( 0, index + 1 );

					if ( !subPath.Exists() ) {
						pathComponents.Add( subPath );
					}

					while ( index > rootLength && path[index] != System.IO.Path.DirectorySeparatorChar && path[index] != System.IO.Path.AltDirectorySeparatorChar ) {
						--index;
					}
				}
			}

			while ( pathComponents.Count > 0 ) {
				var str = pathComponents[^1];

				if ( !String.IsNullOrWhiteSpace( str ) ) {
					str = str.NormalizeLongPath();
					pathComponents.RemoveAt( pathComponents.Count - 1 );

					if ( str.Exists() || str.CreateDirectory( IntPtr.Zero ) ) {
						continue;
					}
				}

				// To mimic Directory.CreateDirectory, we don't throw if the directory (not a file) already exists
				var errorCode = ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error();

				if ( errorCode != PriNativeMethods.ERROR.ERROR_ALREADY_EXISTS || !path.Exists() ) {
					throw Common.GetExceptionFromWin32Error( errorCode );
				}
			}

			return new DirectoryInfo( path );
		}

		[NotNull]
		[ItemNotNull]
		private static IEnumerable<String> EnumerateFileSystemIterator(
			[NotNull] String normalizedPath,
			[NotNull] String normalizedSearchPattern,
			Boolean includeDirectories,
			Boolean includeFiles
		) {
			if ( String.IsNullOrWhiteSpace( normalizedPath ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( normalizedPath ) );
			}

			// NOTE: Any exceptions thrown from this method are thrown on a call to IEnumerator<string>.MoveNext()

			var path = normalizedPath.IsPathUnc() ? normalizedPath : normalizedPath.RemoveLongPathPrefix();

			using var handle = BeginFind( normalizedPath.Combine( normalizedSearchPattern ), out var findData );

			do {
				if ( findData.dwFileAttributes.IsDirectory() ) {
					if ( findData.cFileName.IsCurrentOrParentDirectory() ) {
						continue;
					}

					if ( includeDirectories ) {
						yield return path.RemoveLongPathPrefix().Combine( findData.cFileName );
					}
				}
				else {
					if ( includeFiles ) {
						yield return path.RemoveLongPathPrefix().Combine( findData.cFileName );
					}
				}
			} while ( handle.FindNextFile( out findData ) );

			var errorCode = (PriNativeMethods.ERROR)Marshal.GetLastWin32Error();

			if ( errorCode != PriNativeMethods.ERROR.ERROR_NO_MORE_FILES ) {
				throw Common.GetExceptionFromWin32Error( errorCode );
			}
		}

		[NotNull]
		[ItemNotNull]
		private static IEnumerable<String> EnumerateFileSystemIteratorRecursive(
			[NotNull] String normalizedPath,
			[NotNull] String normalizedSearchPattern,
			Boolean includeDirectories,
			Boolean includeFiles
		) {
			normalizedPath = normalizedPath.ThrowIfBlank();

			// NOTE: Any exceptions thrown from this method are thrown on a call to IEnumerator<string>.MoveNext()
			var pendingDirectories = new Queue<String>();
			pendingDirectories.Enqueue( normalizedPath );

			while ( pendingDirectories.Count > 0 ) {
                normalizedPath = pendingDirectories.Dequeue();

				// get all subdirs to recurse in the next iteration
				foreach ( var subdir in EnumerateNormalizedFileSystemEntries( true, false, SearchOption.TopDirectoryOnly, normalizedPath, "*" ) ) {
					pendingDirectories.Enqueue( subdir.NormalizeLongPath() );
				}

				var path = normalizedPath.IsPathUnc() ? normalizedPath : normalizedPath.RemoveLongPathPrefix();

				using var handle = BeginFind( normalizedPath.Combine( normalizedSearchPattern ), out var findData );

				do {
					var fullPath = path.Combine( findData.cFileName );

					if ( findData.dwFileAttributes.IsDirectory() ) {
						if ( findData.cFileName.IsCurrentOrParentDirectory() ) {
							continue;
						}

						//var fullNormalizedPath = normalizedPath.Combine( findData.cFileName );

						//Debug.Assert( Common.Exists( fullPath ) );
						//Debug.Assert( Common.Exists( fullNormalizedPath.IsPathUnc() ? fullNormalizedPath : fullNormalizedPath.RemoveLongPathPrefix() ) );

						if ( includeDirectories ) {
							yield return fullPath.RemoveLongPathPrefix();
						}
					}
					else if ( includeFiles ) {
						yield return fullPath.RemoveLongPathPrefix();
					}
				} while ( handle.FindNextFile( out findData ) );

				var errorCode = ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error();

				if ( errorCode != PriNativeMethods.ERROR.ERROR_NO_MORE_FILES ) {
					throw Common.GetExceptionFromWin32Error( errorCode );
				}
			}
		}

		[NotNull]
		[ItemNotNull]
		private static IEnumerable<String> EnumerateNormalizedFileSystemEntries(
			Boolean includeDirectories,
			Boolean includeFiles,
			SearchOption option,
			[NotNull] String normalizedPath,
			[NotNull] String normalizedSearchPattern
		) {
			// First check whether the specified path refers to a directory and exists
			var errorCode = normalizedPath.TryGetDirectoryAttributes( out _ );

			if ( errorCode != 0 ) {
				throw Common.GetExceptionFromWin32Error( errorCode );
			}

			if ( option == SearchOption.AllDirectories ) {
				return EnumerateFileSystemIteratorRecursive( normalizedPath, normalizedSearchPattern, includeDirectories, includeFiles );
			}

			return EnumerateFileSystemIterator( normalizedPath, normalizedSearchPattern, includeDirectories, includeFiles );
		}

		private static Boolean IsCurrentOrParentDirectory( [NotNull] this String directoryName ) {
			if ( String.IsNullOrEmpty( directoryName ) ) {
				throw new ArgumentException( "Value cannot be null or empty.", nameof( directoryName ) );
			}

			return directoryName.Equals( ".", StringComparison.OrdinalIgnoreCase ) || directoryName.Equals( "..", StringComparison.OrdinalIgnoreCase );
		}

		[CanBeNull]
		public static Librainian.OperatingSystem.NativeMethods.SafeFindHandle BeginFind( [NotNull] String normalizedPathWithSearchPattern, out WIN32_FIND_DATA findData ) {
			normalizedPathWithSearchPattern = normalizedPathWithSearchPattern.TrimEnd( '\\' ).ThrowIfBlank();
			var handle = PriNativeMethods.FindFirstFile( normalizedPathWithSearchPattern, out findData );

			if ( !handle.IsInvalid ) {
				return handle;
			}

			var errorCode = ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error();

			if ( errorCode is not PriNativeMethods.ERROR.ERROR_FILE_NOT_FOUND and not PriNativeMethods.ERROR.ERROR_PATH_NOT_FOUND and not PriNativeMethods.ERROR.ERROR_NOT_READY) {
				throw Common.GetExceptionFromWin32Error( errorCode );
			}

			return default( Librainian.OperatingSystem.NativeMethods.SafeFindHandle );
		}

		/// <summary>Creates the specified directory.</summary>
		/// <param name="path">A <see cref="String" /> containing the path of the directory to create.</param>
		/// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">
		///     <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid
		///     characters as defined in <see cref="Path.GetInvalidPathChars" />.
		///     <para>-or-</para>
		///     <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example,
		///     on Windows-based platforms, components must not exceed 255
		///     characters.
		/// </exception>
		/// <exception cref="PathTooLongException">
		///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths
		///     must not exceed 32,000
		///     characters.
		/// </exception>
		/// <exception cref="DirectoryNotFoundException">
		///     <paramref name="path" /> contains one or more directories that could not
		///     be found.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">The caller does not have the required access permissions.</exception>
		/// <exception cref="IOException">
		///     <paramref name="path" /> is a file.
		///     <para>-or-</para>
		///     <paramref name="path" /> specifies a device that is not ready.
		/// </exception>
		/// <remarks>
		///     Note: Unlike <see cref="CreateDirectory(String)" />, this method only creates the last directory in
		///     <paramref name="path" />.
		/// </remarks>
		[NotNull]
		public static DirectoryInfo CreateDirectory( [NotNull] this String path ) {
			if ( String.IsNullOrWhiteSpace( path ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( path ) );
			}

			if ( path.IsPathUnc() ) {
				return CreateDirectoryUnc( path );
			}

			var normalizedPath = path.NormalizeLongPath();
			var fullPath = normalizedPath.RemoveLongPathPrefix();

			var length = fullPath.Length;

			if ( length >= 2 && fullPath[length - 1].IsDirectorySeparator() ) {
				--length;
			}

			var rootLength = fullPath.GetRootLength();

			var pathComponents = new List<String>();

			if ( length > rootLength ) {
				for ( var index = length - 1; index >= rootLength; --index ) {
					var subPath = fullPath.Substring( 0, index + 1 );

					if ( !subPath.Exists() ) {
						pathComponents.Add( subPath.NormalizeLongPath() );
					}

					while ( index > rootLength && fullPath[index].IsDirectorySeparator() ) {
						--index;
					}
				}
			}

			while ( pathComponents.Count > 0 ) {
				var str = pathComponents[^1].ThrowIfBlank();
				pathComponents.RemoveAt( pathComponents.Count - 1 );

				if ( str.CreateDirectory( IntPtr.Zero ) ) {
					continue;
				}

				// To mimic Directory.CreateDirectory, we don't throw if the directory (not a file) already exists
				var errorCode = ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error();

				if ( errorCode != PriNativeMethods.ERROR.ERROR_ALREADY_EXISTS || !path.Exists() ) {
					throw Common.GetExceptionFromWin32Error( errorCode );
				}
			}

			return new DirectoryInfo( fullPath );
		}

		public static void Delete( [NotNull] String path, Boolean recursive ) {
			path = path.ThrowIfBlank();

			/* MSDN: https://msdn.microsoft.com/en-us/library/fxeahc5f.aspx
			   The behavior of this method differs slightly when deleting a directory that contains a reparse point,
			   such as a symbolic link or a mount point.
			   (1) If the reparse point is a directory, such as a mount point, it is unmounted and the mount point is deleted.
			   This method does not recurse through the reparse point.
			   (2) If the reparse point is a symbolic link to a file, the reparse point is deleted and not the target of
			   the symbolic link.
			*/

			try {
				const FileAttributes reparseFlags = FileAttributes.Directory | FileAttributes.ReparsePoint;
				var isDirectoryReparsePoint = path.GetAttributes().HasFlag( reparseFlags );

				if ( isDirectoryReparsePoint ) {
					Delete( path );

					return;
				}
			}
			catch ( FileNotFoundException ) {
				// ignore: not there when we try to delete, it doesn't matter
			}

			if ( !recursive ) {
				Delete( path );

				return;
			}

			try {
				foreach ( var file in EnumerateFileSystemEntries( path.ThrowIfBlank(), "*", false, true, SearchOption.TopDirectoryOnly ) ) {
					File.Delete( file );
				}
			}
			catch ( FileNotFoundException ) {
				// ignore: not there when we try to delete, it doesn't matter
			}

			try {
				foreach ( var subPath in EnumerateFileSystemEntries( path.ThrowIfBlank(), "*", true, false, SearchOption.TopDirectoryOnly ) ) {
					Delete( subPath, true );
				}
			}
			catch ( FileNotFoundException ) {
				// ignore: not there when we try to delete, it doesn't matter
			}

			try {
				Delete( path );
			}
			catch ( FileNotFoundException ) {
				// ignore: not there when we try to delete, it doesn't matter
			}
		}

		/// <summary>Deletes the specified empty directory.</summary>
		/// <param name="path">A <see cref="String" /> containing the path of the directory to delete.</param>
		/// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">
		///     <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid
		///     characters as defined in <see cref="Path.GetInvalidPathChars" />.
		///     <para>-or-</para>
		///     <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example,
		///     on Windows-based platforms, components must not exceed 255
		///     characters.
		/// </exception>
		/// <exception cref="PathTooLongException">
		///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths
		///     must not exceed 32,000
		///     characters.
		/// </exception>
		/// <exception cref="DirectoryNotFoundException"><paramref name="path" /> could not be found.</exception>
		/// <exception cref="UnauthorizedAccessException">
		///     The caller does not have the required access permissions.
		///     <para>-or-</para>
		///     <paramref name="path" /> refers to a directory that is read-only.
		/// </exception>
		/// <exception cref="IOException">
		///     <paramref name="path" /> is a file.
		///     <para>-or-</para>
		///     <paramref name="path" /> refers to a directory that is not empty.
		///     <para>-or-</para>
		///     <paramref name="path" /> refers to a directory that is in use.
		///     <para>-or-</para>
		///     <paramref name="path" /> specifies a device that is not ready.
		/// </exception>
		public static void Delete( [NotNull] String path ) {
			var normalizedPath = path.NormalizeLongPath();

			if ( !PriNativeMethods.RemoveDirectory( normalizedPath ) ) {
				throw Common.GetExceptionFromLastWin32Error();
			}
		}

		/// <summary>Returns a enumerable containing the directory names of the specified directory.</summary>
		/// <param name="path">A <see cref="String" /> containing the path of the directory to search.</param>
		/// <returns>A <see cref="IEnumerable{T}" /> containing the directory names within <paramref name="path" />.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">
		///     <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid
		///     characters as defined in <see cref="Path.GetInvalidPathChars" />.
		///     <para>-or-</para>
		///     <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example,
		///     on Windows-based platforms, components must not exceed 255
		///     characters.
		/// </exception>
		/// <exception cref="PathTooLongException">
		///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths
		///     must not exceed 32,000
		///     characters.
		/// </exception>
		/// <exception cref="DirectoryNotFoundException">
		///     <paramref name="path" /> contains one or more directories that could not
		///     be found.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">The caller does not have the required access permissions.</exception>
		/// <exception cref="IOException">
		///     <paramref name="path" /> is a file.
		///     <para>-or-</para>
		///     <paramref name="path" /> specifies a device that is not ready.
		/// </exception>
		[NotNull]
		public static IEnumerable<String> EnumerateDirectories( [NotNull] String path ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), "*", true, false, SearchOption.TopDirectoryOnly );

		/// <summary>
		///     Returns a enumerable containing the directory names of the specified directory that match the specified search
		///     pattern.
		/// </summary>
		/// <param name="path">A <see cref="String" /> containing the path of the directory to search.</param>
		/// <param name="searchPattern">
		///     A <see cref="String" /> containing search pattern to match against the names of the directories in
		///     <paramref name="path" />, otherwise,
		///     <see langword="null" /> or an empty string ("") to use the default search pattern, "*".
		/// </param>
		/// <returns>
		///     A <see cref="IEnumerable{T}" /> containing the directory names within <paramref name="path" /> that match
		///     <paramref name="searchPattern" />.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">
		///     <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid
		///     characters as defined in <see cref="Path.GetInvalidPathChars" />.
		///     <para>-or-</para>
		///     <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example,
		///     on Windows-based platforms, components must not exceed 255
		///     characters.
		/// </exception>
		/// <exception cref="PathTooLongException">
		///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths
		///     must not exceed 32,000
		///     characters.
		/// </exception>
		/// <exception cref="DirectoryNotFoundException">
		///     <paramref name="path" /> contains one or more directories that could not
		///     be found.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">The caller does not have the required access permissions.</exception>
		/// <exception cref="IOException">
		///     <paramref name="path" /> is a file.
		///     <para>-or-</para>
		///     <paramref name="path" /> specifies a device that is not ready.
		/// </exception>
		[NotNull]
		public static IEnumerable<String> EnumerateDirectories( [NotNull] String path, [NotNull] String searchPattern ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, true, false, SearchOption.TopDirectoryOnly );

		[NotNull]
		public static IEnumerable<String> EnumerateDirectories( [NotNull] String path, [NotNull] String searchPattern, SearchOption options ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, true, false, options );

		/// <summary>Returns a enumerable containing the file names of the specified directory.</summary>
		/// <param name="path">A <see cref="String" /> containing the path of the directory to search.</param>
		/// <returns>A <see cref="IEnumerable{T}" /> containing the file names within <paramref name="path" />.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">
		///     <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid
		///     characters as defined in <see cref="Path.GetInvalidPathChars" />.
		///     <para>-or-</para>
		///     <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example,
		///     on Windows-based platforms, components must not exceed 255
		///     characters.
		/// </exception>
		/// <exception cref="PathTooLongException">
		///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths
		///     must not exceed 32,000
		///     characters.
		/// </exception>
		/// <exception cref="DirectoryNotFoundException">
		///     <paramref name="path" /> contains one or more directories that could not
		///     be found.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">The caller does not have the required access permissions.</exception>
		/// <exception cref="IOException">
		///     <paramref name="path" /> is a file.
		///     <para>-or-</para>
		///     <paramref name="path" /> specifies a device that is not ready.
		/// </exception>
		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> EnumerateFiles( [NotNull] String path ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), "*", false, true, SearchOption.TopDirectoryOnly );

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> EnumerateFiles( [NotNull] String path, [NotNull] String searchPattern, SearchOption options ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, false, true, options );

		/// <summary>
		///     Returns a enumerable containing the file names of the specified directory that match the specified search
		///     pattern.
		/// </summary>
		/// <param name="path">A <see cref="String" /> containing the path of the directory to search.</param>
		/// <param name="searchPattern">
		///     A <see cref="String" /> containing search pattern to match against the names of the files in
		///     <paramref name="path" />, otherwise,
		///     <see langword="null" /> or an empty string ("") to use the default search pattern, "*".
		/// </param>
		/// <returns>
		///     A <see cref="IEnumerable{T}" /> containing the file names within <paramref name="path" /> that match
		///     <paramref name="searchPattern" />.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">
		///     <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid
		///     characters as defined in <see cref="Path.GetInvalidPathChars" />.
		///     <para>-or-</para>
		///     <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example,
		///     on Windows-based platforms, components must not exceed 255
		///     characters.
		/// </exception>
		/// <exception cref="PathTooLongException">
		///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths
		///     must not exceed 32,000
		///     characters.
		/// </exception>
		/// <exception cref="DirectoryNotFoundException">
		///     <paramref name="path" /> contains one or more directories that could not
		///     be found.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">The caller does not have the required access permissions.</exception>
		/// <exception cref="IOException">
		///     <paramref name="path" /> is a file.
		///     <para>-or-</para>
		///     <paramref name="path" /> specifies a device that is not ready.
		/// </exception>
		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> EnumerateFiles( [NotNull] String path, [NotNull] String searchPattern ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, false, true, SearchOption.TopDirectoryOnly );

		/// <summary>Returns a enumerable containing the file and directory names of the specified directory.</summary>
		/// <param name="path">A <see cref="String" /> containing the path of the directory to search.</param>
		/// <returns>A <see cref="IEnumerable{T}" /> containing the file and directory names within <paramref name="path" />.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">
		///     <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid
		///     characters as defined in <see cref="Path.GetInvalidPathChars" />.
		///     <para>-or-</para>
		///     <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example,
		///     on Windows-based platforms, components must not exceed 255
		///     characters.
		/// </exception>
		/// <exception cref="PathTooLongException">
		///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths
		///     must not exceed 32,000
		///     characters.
		/// </exception>
		/// <exception cref="DirectoryNotFoundException">
		///     <paramref name="path" /> contains one or more directories that could not
		///     be found.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">The caller does not have the required access permissions.</exception>
		/// <exception cref="IOException">
		///     <paramref name="path" /> is a file.
		///     <para>-or-</para>
		///     <paramref name="path" /> specifies a device that is not ready.
		/// </exception>
		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> EnumerateFileSystemEntries( [NotNull] String path ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), null, true, true, SearchOption.TopDirectoryOnly );

		/// <summary>
		///     Returns a enumerable containing the file and directory names of the specified directory that match the
		///     specified search pattern.
		/// </summary>
		/// <param name="path">A <see cref="String" /> containing the path of the directory to search.</param>
		/// <param name="searchPattern">
		///     A <see cref="String" /> containing search pattern to match against the names of the files and directories in
		///     <paramref name="path" />, otherwise,
		///     <see langword="null" /> or an empty string ("") to use the default search pattern, "*".
		/// </param>
		/// <returns>
		///     A <see cref="IEnumerable{T}" /> containing the file and directory names within <paramref name="path" />that
		///     match <paramref name="searchPattern" />.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">
		///     <paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid
		///     characters as defined in <see cref="Path.GetInvalidPathChars" />.
		///     <para>-or-</para>
		///     <paramref name="path" /> contains one or more components that exceed the drive-defined maximum length. For example,
		///     on Windows-based platforms, components must not exceed 255
		///     characters.
		/// </exception>
		/// <exception cref="PathTooLongException">
		///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths
		///     must not exceed 32,000
		///     characters.
		/// </exception>
		/// <exception cref="DirectoryNotFoundException">
		///     <paramref name="path" /> contains one or more directories that could not
		///     be found.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">The caller does not have the required access permissions.</exception>
		/// <exception cref="IOException">
		///     <paramref name="path" /> is a file.
		///     <para>-or-</para>
		///     <paramref name="path" /> specifies a device that is not ready.
		/// </exception>
		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> EnumerateFileSystemEntries( [NotNull] String path, [NotNull] String searchPattern ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, true, true, SearchOption.TopDirectoryOnly );

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> EnumerateFileSystemEntries( [NotNull] String path, [NotNull] String searchPattern, SearchOption options ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, true, true, options );

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> EnumerateFileSystemEntries(
			[NotNull] String path,
			[CanBeNull] String? searchPattern,
			Boolean includeDirectories,
			Boolean includeFiles,
			SearchOption option
		) {
			var normalizedSearchPattern = searchPattern.NormalizeSearchPattern();
			var normalizedPath = path.NormalizeLongPath();

			return EnumerateNormalizedFileSystemEntries( includeDirectories, includeFiles, option, normalizedPath, normalizedSearchPattern );
		}

		/// <summary>Returns a value indicating whether the specified path refers to an existing directory.</summary>
		/// <param name="path">A <see cref="String" /> containing the path to check.</param>
		/// <returns>
		///     <see langword="true" /> if <paramref name="path" /> refers to an existing directory; otherwise,
		///     <see langword="false" />.
		/// </returns>
		/// <remarks>
		///     Note that this method will return false if any error occurs while trying to determine if the specified directory
		///     exists. This includes situations that would normally
		///     result in thrown exceptions including (but not limited to); passing in a directory name with invalid or too many
		///     characters, an I/O error such as a failing or missing disk, or if
		///     the caller does not have Windows or Code Access Security (CAS) permissions to to read the directory.
		/// </remarks>
		public static Boolean Exists( [NotNull] this String path ) => path.ThrowIfBlank().Exists( out var isDirectory ) && isDirectory;

		public static FileAttributes GetAttributes( [NotNull] String path ) => path.ThrowIfBlank().GetAttributes();

		public static DateTime GetCreationTime( [NotNull] String path ) => GetCreationTimeUtc( path ).ToLocalTime();

		public static DateTime GetCreationTimeUtc( [NotNull] String path ) => new DirectoryInfo( path.ThrowIfBlank() ).CreationTimeUtc;

		[NotNull]
		public static String GetCurrentDirectory() => ".".NormalizeLongPath().RemoveLongPathPrefix();

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> GetDirectories( [NotNull] this String path, [NotNull] String searchPattern, SearchOption searchOption ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, true, false, searchOption );

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> GetDirectories( [NotNull] String path ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), "*", true, false, SearchOption.TopDirectoryOnly );

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> GetDirectories( [NotNull] String path, [NotNull] String searchPattern ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, true, false, SearchOption.TopDirectoryOnly );

		[NotNull]
		public static SafeFileHandle GetDirectoryHandle( [NotNull] this String normalizedPath ) {
			var handle = PriNativeMethods.CreateFile( normalizedPath, PriNativeMethods.EFileAccess.GenericWrite, ( UInt32 )( FileShare.Write | FileShare.Delete ), IntPtr.Zero,
												   ( Int32 )FileMode.Open, PriNativeMethods.FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero );

			if ( !handle.IsInvalid ) {
				return handle;
			}

			var ex = Common.GetExceptionFromLastWin32Error();
			Debug.WriteLine( "error {0} with {1}\n{2}", ex.Message, normalizedPath, ex.StackTrace );

			throw ex;
		}

		[NotNull]
		public static String GetDirectoryRoot( [NotNull] String path ) {
			path = path.ThrowIfBlank().GetFullPath();

			return path.Substring( 0, path.GetRootLength() );
		}

		[NotNull]
		public static IEnumerable<String> GetFiles( [NotNull] this String path ) {
			if ( String.IsNullOrWhiteSpace( path ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( path ) );
			}

			return EnumerateFileSystemEntries( path.ThrowIfBlank(), "*", false, true, SearchOption.TopDirectoryOnly );
		}

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> GetFiles( [NotNull] String path, [NotNull] String searchPattern ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, false, true, SearchOption.TopDirectoryOnly );

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> GetFiles( [NotNull] String path, [NotNull] String searchPattern, SearchOption options ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, false, true, options );

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> GetFileSystemEntries( [NotNull] String path ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), null, true, true, SearchOption.TopDirectoryOnly );

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> GetFileSystemEntries( [NotNull] String path, [NotNull] String searchPattern ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, true, true, SearchOption.TopDirectoryOnly );

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<String> GetFileSystemEntries( [NotNull] String path, [NotNull] String searchPattern, SearchOption options ) =>
			EnumerateFileSystemEntries( path.ThrowIfBlank(), searchPattern, true, true, options );

		public static DateTime GetLastAccessTime( [NotNull] String path ) => GetLastAccessTimeUtc( path ).ToLocalTime();

		public static DateTime GetLastAccessTimeUtc( [NotNull] String path ) => new DirectoryInfo( path ).LastAccessTimeUtc;

		public static DateTime GetLastWriteTime( [NotNull] String path ) => GetLastWriteTimeUtc( path ).ToLocalTime();

		public static DateTime GetLastWriteTimeUtc( [NotNull] String path ) => new DirectoryInfo( path ).LastWriteTimeUtc;

		//[NotNull] public static IEnumerable<String> GetLogicalDrives() => System.IO.Directory.GetLogicalDrives();

		[NotNull]
		public static DirectoryInfo GetParent( [NotNull] String path ) => new( path.ThrowIfBlank().GetDirectoryName() );

		public static Boolean IsDirectory( this FileAttributes attributes ) => attributes.HasFlag( FileAttributes.Directory );

		public static void Move( [NotNull] String sourcePath, [NotNull] String destinationPath ) {
			var normalizedSourcePath = sourcePath.ThrowIfBlank().NormalizeLongPath( "sourcePath" );
			var normalizedDestinationPath = destinationPath.ThrowIfBlank().NormalizeLongPath( "destinationPath" );

			if ( PriNativeMethods.MoveFile( normalizedSourcePath, normalizedDestinationPath ) ) {
				return;
			}

			var lastWin32Error = ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error();

			if ( lastWin32Error == PriNativeMethods.ERROR.ERROR_ACCESS_DENIED ) {
				throw new IOException( $"Access to the path '{sourcePath}'is denied.", PriNativeMethods.MakeHRFromErrorCode( lastWin32Error ) );
			}

			throw Common.GetExceptionFromWin32Error( lastWin32Error, "path" );
		}

		public static void SetAttributes( [NotNull] String path, FileAttributes fileAttributes ) {
			if ( !Enum.IsDefined( typeof( FileAttributes ), fileAttributes ) ) {
				throw new InvalidEnumArgumentException( nameof( fileAttributes ), ( Int32 )fileAttributes, typeof( FileAttributes ) );
			}

			path.ThrowIfBlank().SetAttributes( fileAttributes );
		}

		public static void SetCreationTime( [NotNull] this String path, DateTime creationTime ) => SetCreationTimeUtc( path.ThrowIfBlank(), creationTime.ToUniversalTime() );

		public static void SetCreationTimeUtc( [NotNull] this String path, DateTime creationTimeUtc ) {
			var normalizedPath = path.GetFullPath().NormalizeLongPath();

			using var handle = normalizedPath.GetDirectoryHandle();

			unsafe {
				var fileTime = new FILE_TIME( creationTimeUtc.ToFileTimeUtc() );

				if ( PriNativeMethods.SetFileTime( handle, &fileTime, null, null ) ) {
					return;
				}

				var errorCode = ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error();
				Common.ThrowIOError( errorCode, path );
			}
		}

		/// <summary>
		///     Author's remark: NotSupportedException("Windows does not support setting the current directory to a long
		///     path");
		/// </summary>
		/// <param name="path"></param>
		public static void SetCurrentDirectory( [NotNull] this String path ) {
			var normalizedPath = path.ThrowIfBlank().GetFullPath().NormalizeLongPath();

			if ( !PriNativeMethods.SetCurrentDirectory( normalizedPath ) ) {
				var lastWin32Error = ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error();

				if ( lastWin32Error == PriNativeMethods.ERROR.ERROR_FILE_NOT_FOUND ) {
					lastWin32Error = PriNativeMethods.ERROR.ERROR_PATH_NOT_FOUND;
				}

				Common.ThrowIOError( lastWin32Error, normalizedPath );
			}
		}

		public static Boolean SetFileTimes( this IntPtr hFile, DateTime creationTime, DateTime accessTime, DateTime writeTime ) =>
			PriNativeMethods.SetFileTime( hFile, creationTime.ToFileTimeUtc(), accessTime.ToFileTimeUtc(), writeTime.ToFileTimeUtc() );

		public static void SetLastAccessTime( [NotNull] this String path, DateTime lastAccessTime ) =>
			path.ThrowIfBlank().SetLastAccessTimeUtc( lastAccessTime.ToUniversalTime() );

		public static unsafe void SetLastAccessTimeUtc( [NotNull] this String path, DateTime lastWriteTimeUtc ) {
			var normalizedPath = path.ThrowIfBlank().GetFullPath().NormalizeLongPath();

			using var handle = normalizedPath.GetDirectoryHandle();

			var fileTime = new FILE_TIME( lastWriteTimeUtc.ToFileTimeUtc() );

			if ( PriNativeMethods.SetFileTime( handle, null, &fileTime, null ) ) {
				return;
			}

			var errorCode = ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error();
			Common.ThrowIOError( errorCode, path );
		}

		public static void SetLastWriteTime( [NotNull] this String path, DateTime lastWriteTimeUtc ) {
			unsafe {
				var normalizedPath = path.GetFullPath().NormalizeLongPath();

				using var handle = normalizedPath.GetDirectoryHandle();

				var fileTime = new FILE_TIME( lastWriteTimeUtc.ToFileTimeUtc() );
				var r = PriNativeMethods.SetFileTime( handle, null, null, &fileTime );

				if ( r ) {
					return;
				}

				var errorCode = ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error();
				Common.ThrowIOError( errorCode, path );
			}
		}

		public static unsafe void SetLastWriteTimeUtc( [NotNull] this String path, DateTime lastWriteTimeUtc ) {
			var normalizedPath = path.GetFullPath().NormalizeLongPath();

			using var handle = normalizedPath.GetDirectoryHandle();

			var fileTime = new FILE_TIME( lastWriteTimeUtc.ToFileTimeUtc() );

			if ( PriNativeMethods.SetFileTime( handle, null, null, &fileTime ) ) {
				return;
			}

			var errorCode = ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error();
			Common.ThrowIOError( errorCode, path );
		}

	}

}