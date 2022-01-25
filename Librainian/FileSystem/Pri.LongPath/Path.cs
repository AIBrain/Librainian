// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Path.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.

#nullable enable

namespace Librainian.FileSystem.Pri.LongPath;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Exceptions;
using Utilities;

public static class Path {

	public const String LongPathPrefix = @"\\?\";

	public const String UNCLongPathPrefix = @"\\?\UNC\";

	public const Char VolumeSeparatorChar = ':';

	public static readonly Char AltDirectorySeparatorChar = System.IO.Path.AltDirectorySeparatorChar;

	public static readonly Char DirectorySeparatorChar = System.IO.Path.DirectorySeparatorChar;

	public static readonly Char[] InvalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();

	public static readonly Char[] InvalidPathChars = System.IO.Path.GetInvalidPathChars();

	public static readonly Char PathSeparator = System.IO.Path.PathSeparator;

	private static Int32 GetUncRootLength( this String path ) {
		var components = path.ThrowIfBlank().Split( DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries );

		return components.Length >= 2 ? $@"\\{components[ 0 ]}\{components[ 1 ]}\".Length : throw new InvalidOperationException( "Invalid path components." );
	}

	/// <summary>
	///     <para>Throws if the <paramref name="path" /> is blank.</para>
	///     <para>Checks if the <paramref name="path" /> starts with \\?\</para>
	/// </summary>
	/// <param name="path"></param>
	[DebuggerStepThrough]
	public static String AddLongPathPrefix( this String path ) {
		path = path.ThrowIfBlank();

		if ( path.StartsWith( LongPathPrefix, StringComparison.OrdinalIgnoreCase ) ) {
			return path;
		}

		// http://msdn.microsoft.com/en-us/library/aa365247.aspx
		return path.StartsWith( @"\\", StringComparison.OrdinalIgnoreCase ) ? $"{UNCLongPathPrefix}{path[ 2.. ]}" : $"{LongPathPrefix}{path}";
	}

	[DebuggerStepThrough]
	public static String ChangeExtension( this String filename, String? extension ) => System.IO.Path.ChangeExtension( filename.ThrowIfBlank(), extension );

	/// <summary>
	///     <para>Throws if blank.</para>
	///     <para>Checks if the <paramref name="path" /> starts with \\?\</para>
	///     <para>If it doesn't, it calls <see cref="AddLongPathPrefix" />.</para>
	/// </summary>
	/// <param name="path"></param>
	public static String CheckAddLongPathPrefix( this String path ) {
		path = path.ThrowIfBlank();

		if ( path.StartsWith( LongPathPrefix, StringComparison.OrdinalIgnoreCase ) ) {
			return path;
		}

		var maxPathLimit = PriNativeMethods.MAX_PATH;

		if ( Uri.TryCreate( path, UriKind.Absolute, out var uri ) && uri.IsUnc ) {

			// What's going on here?
			// Empirical evidence shows that Windows has trouble dealing with UNC paths
			// longer than MAX_PATH *minus* the length of the "\\hostname\" prefix.
			// See the following tests:
			//  - UncDirectoryTests.TestDirectoryCreateNearMaxPathLimit
			//  - UncDirectoryTests.TestDirectoryEnumerateDirectoriesNearMaxPathLimit
			var rootPathLength = 3 + uri.Host.Length;
			maxPathLimit -= rootPathLength;
		}

		return path.Length < maxPathLimit ? path : path.AddLongPathPrefix();
	}

	public static String CombinePaths( this String path1, String path2 ) {
		ThrowIfInvalidPathChars( ref path1 );
		ThrowIfInvalidPathChars( ref path2 );

		if ( String.IsNullOrEmpty( path2 ) ) {
			return path1;
		}

		if ( String.IsNullOrEmpty( path1 ) || path2.IsPathRooted() ) {
			return path2;
		}

		var ch = path1[ ^1 ];

		if ( ch.IsDirectorySeparator() || ch == VolumeSeparatorChar ) {
			return path1 + path2;
		}

		return $"{path1}{DirectorySeparatorChar}{path2}";
	}

	public static String CombinePaths( this String path1, String path2, String path3 ) => CombinePaths( path1, path2 ).CombinePaths( path3 );

	public static String CombinePaths( this String path1, String path2, String path3, String path4 ) =>
		CombinePaths( path1.CombinePaths( path2 ), path3 ).CombinePaths( path4 );

	public static String CombinePaths( params String[] paths ) {
		if ( paths == null ) {
			throw new ArgumentEmptyException( nameof( paths ) );
		}

		switch ( paths.Length ) {
			case 0:
				return String.Empty;

			case 1: {
				var z = paths[ 0 ];

				//if ( z == null ) {
				//throw new ArgumentException( "Value cannot be null or whitespace." );
				//}

				return z.ThrowIfInvalidPathChars().ThrowIfBlank();
			}

			default: {
				var z = paths[ 0 ];

				//if ( z == null ) {
				//throw new ArgumentException( "Value cannot be null or whitespace." );
				//}

				var path = z.ThrowIfInvalidPathChars().ThrowIfBlank();

				for ( var i = 1; i < paths.Length; ++i ) {
					path = path.CombinePaths( paths[ i ] );
				}

				return path;
			}
		}
	}

	public static String GetDirectoryName( this String path ) {
		path = path.ThrowIfInvalidPathChars();

		String? basePath = null;

		if ( !path.IsPathRooted() ) {
			basePath = Directory.GetCurrentDirectory();
		}

		path = path.NormalizeLongPath().RemoveLongPathPrefix();
		var rootLength = path.GetRootLength();

		if ( path.Length <= rootLength ) {
			return String.Empty;
		}

		var length = path.Length;

		do { } while ( length > rootLength && !path[ --length ].IsDirectorySeparator() );

		if ( basePath == null ) {
			return path[ ..length ];
		}

		path = path[ ( basePath.Length + 1 ).. ];
		length = length - basePath.Length - 1;

		if ( length < 0 ) {
			length = 0;
		}

		return path[ ..length ];
	}

	public static String GetExtension( this String path ) => System.IO.Path.GetExtension( path.ThrowIfBlank() );

	public static String GetFileName( this String path ) => System.IO.Path.GetFileName( path.NormalizeLongPath() );

	[return: NotNullIfNotNull( "path" )]
	public static String? GetFileNameWithoutExtension( this String? path ) => System.IO.Path.GetFileNameWithoutExtension( path );

	/// <summary>
	///     <para>If the <paramref name="path" /> is UNC, then leave it UNC.</para>
	///     <para>Otherwise, normalize it, and then remove the long path prefix.</para>
	/// </summary>
	/// <param name="path"></param>
	[DebuggerStepThrough]
	public static String GetFullPath( this String path ) => path.IsPathUnc() ? path : path.NormalizeLongPath().RemoveLongPathPrefix();

	[DebuggerStepThrough]
	public static IEnumerable<Char> GetInvalidFileNameChars() => InvalidFileNameChars;

	[DebuggerStepThrough]
	public static IEnumerable<Char> GetInvalidPathChars() => InvalidPathChars;

	public static String GetPathRoot( this String path ) {
		if ( !path.IsPathRooted() ) {
			return String.Empty;
		}

		if ( !path.IsPathUnc() ) {
			path = path.NormalizeLongPath().RemoveLongPathPrefix();
		}

		return path[ ..path.GetRootLength() ];
	}

	/// <summary>
	///     Returns just the unique random file name (no path) with an optional <paramref name="extension" />.
	///     <remarks>Does not create a file.</remarks>
	/// </summary>
	/// <param name="extension"></param>
	public static String GetRandomFileName( String? extension = null ) {
		if ( String.IsNullOrEmpty( extension ) ) {
			extension = $"{Guid.NewGuid():D}";
		}

		return $"{Guid.NewGuid():D}.{extension}";
	}

	public static Int32 GetRootLength( this String path ) {
		if ( path.IsPathUnc() ) {
			return path.GetUncRootLength();
		}

		path = path.GetFullPath().ThrowIfInvalidPathChars();

		var rootLength = 0;
		var length = path.Length;

		switch ( length ) {
			case >= 1 when path[ 0 ].IsDirectorySeparator(): {
				rootLength = 1;

				if ( length >= 2 && path[ 1 ].IsDirectorySeparator() ) {
					rootLength = 2;
					var num = 2;

					while ( rootLength >= length ||
							( path[ rootLength ] == System.IO.Path.DirectorySeparatorChar || path[ rootLength ] == System.IO.Path.AltDirectorySeparatorChar ) && --num <= 0 ) {
						++rootLength;
					}
				}

				break;
			}

			case >= 2 when path[ 1 ] == System.IO.Path.VolumeSeparatorChar: {
				rootLength = 2;

				if ( length >= 3 && path[ 2 ].IsDirectorySeparator() ) {
					++rootLength;
				}

				break;
			}
		}

		return rootLength;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static String GetTempFileName( String? extension = null ) => GetRandomFileName( extension );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	public static String GetTempPath() => System.IO.Path.GetTempPath().ThrowIfBlank();

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	public static Boolean HasExtension( this String path ) => System.IO.Path.HasExtension( path.ThrowIfBlank() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	public static Boolean HasIllegalCharacters( this String? path ) {
		PriCommon.ThrowIfBlank( ref path );
		return path?.Any( InvalidPathChars.Contains ) == true;
	}

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	public static Boolean IsDirectorySeparator( this Char c ) => c == DirectorySeparatorChar || c == AltDirectorySeparatorChar;

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	public static Boolean IsPathRooted( this String path ) => System.IO.Path.IsPathRooted( path.ThrowIfBlank() );

	/// <summary>
	///     Normalizes path and adds the \\?\ long path prefix.
	///     <para>
	///         <remarks>Makes a DLL call to kernel32.dll.GetFullPathNameW.</remarks>
	///     </para>
	/// </summary>
	/// <param name="path"></param>
	/// <param name="parameterName"></param>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static String NormalizeLongPath( this String path, String parameterName = "path" ) => path.CheckAddLongPathPrefix();

	/// <summary>
	///     <para>Trim whitespace from the <paramref name="path" />.</para>
	///     <para>If the <paramref name="path" /> is null, empty or whitespace then return <see cref="String.Empty" />.</para>
	///     <para>If the <paramref name="path" /> doesn't start with "\\?\UNC\", then return the rest of the path.</para>
	///     <para>
	///         If the <paramref name="path" /> starts with "\\?\UNC\", then return the rest of the \\
	///         <paramref name="path" />.
	///     </para>
	///     <para>Otherwise just return the path as is.</para>
	/// </summary>
	/// <param name="path"></param>
	public static String RemoveLongPathPrefix( this String path ) {
		if ( String.IsNullOrWhiteSpace( path ) ) {
			return String.Empty;
		}

		path = path.Trim();

		if ( String.IsNullOrEmpty( path ) ) {
			return String.Empty;
		}

		if ( !path.StartsWith( LongPathPrefix, StringComparison.OrdinalIgnoreCase ) ) {

			// \\?\
			return path;
		}

		if ( path.StartsWith( UNCLongPathPrefix, StringComparison.OrdinalIgnoreCase ) ) {

			// \\?\UNC\
			return $@"\\{path[ UNCLongPathPrefix.Length.. ]}";
		}

		if ( path.Length > LongPathPrefix.Length ) {
			return path[ LongPathPrefix.Length.. ]!;
		}

		return path;
	}

	/// <summary>
	/// </summary>
	/// <param name="path"></param>
	/// <exception cref="ArgumentEmptyException">Thrown if any invalid chars found in path.</exception>
	[NeedsTesting]
	public static String ThrowIfInvalidPathChars( this String path ) {
		path = path.ThrowIfBlank();

		if ( path.HasIllegalCharacters() ) {
			throw new InvalidOperationException( $"Invalid characters in {nameof( path )}" );
		}

		return path;
	}

	/// <summary>
	/// </summary>
	/// <param name="path"></param>
	/// <exception cref="InvalidOperationException"></exception>
	public static void ThrowIfInvalidPathChars( ref String path ) {
		if ( path.Any( InvalidPathChars.Contains ) ) {
			throw new InvalidOperationException( $"Invalid characters in {nameof( path )}" );
		}
	}

	/*
		if ( path.IsPathUnc() ) {
			return path.CheckAddLongPathPrefix();
		}
		*/
	/*
		var buffer = new StringBuilder( PriNativeMethods.MAX_LONG_PATH + 1 ); // Add 1 for NULL
		var length = PriNativeMethods.GetFullPathNameW( path, ( UInt32 )buffer.Capacity, buffer, IntPtr.Zero );

		switch ( length ) {
			case 0: {
				throw Common.GetExceptionFromLastWin32Error( parameterName );
			}

			case > PriNativeMethods.MAX_LONG_PATH: {
				throw Common.GetExceptionFromWin32Error( PriNativeMethods.ERROR.ERROR_FILENAME_EXCED_RANGE, parameterName );
			}

			case > 1 when buffer[ 0 ].IsDirectorySeparator() && buffer[ 1 ].IsDirectorySeparator(): {
				if ( length < 2 ) {
					throw new ArgumentException( @"The UNC path should be of the form \\server\share." );
				}

				var parts = buffer.ToString().Split( DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries );

				if ( parts.Length < 2 ) {
					throw new ArgumentException( @"The UNC path should be of the form \\server\share." );
				}

				break;
			}
		}
		*/
	/*
		return path.AddLongPathPrefix();
		*/

	public static Boolean TryNormalizeLongPath( this String path, out String? result ) {
		if ( String.IsNullOrWhiteSpace( path ) ) {
			result = null;

			return false;
		}

		try {
			result = path.NormalizeLongPath();

			return true;
		}
		catch ( ArgumentException ) { }
		catch ( PathTooLongException ) { }

		result = null;

		return false;
	}
}