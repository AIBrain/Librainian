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
// File "PathInternal.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.

#nullable enable

namespace Librainian.FileSystem;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Utilities;

public static class PathInternal {

	[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false )]
	private static extern UInt32 GetLongPathNameW( this String lpszShortPath, StringBuilder lpszLongPath, UInt32 cchBuffer );

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static String EnsureExtendedPrefix( this String path ) {
		path = path.TrimAndThrowIfBlank();

		if ( path.IsPartiallyQualified() || path.IsDevice() ) {
			return path;
		}

		if ( path.StartsWith( Constants.UncPathPrefix, StringComparison.OrdinalIgnoreCase ) ) {
			return path.Insert( 2, @"?\UNC\" );
		}

		return $"{Constants.ExtendedPathPrefix}{path}";
	}

	public static String GetLongPathName( this String path ) {
		path = path.TrimAndThrowIfBlank();

		var stringBuffer = new StringBuilder( Constants.MaxPathLength );
		var hresult = path.GetLongPathNameW( stringBuffer, ( UInt32 )stringBuffer.Capacity );

		return stringBuffer.ToString();
	}

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean IsDevice( this String path ) {
		path = path.TrimAndThrowIfBlank();

		if ( path.IsExtended() ) {
			return true;
		}

		if ( path.Length >= 4 && path[ 0 ].IsDirectorySeparator() && path[ 1 ].IsDirectorySeparator() && ( path[ 2 ] == '.' || path[ 2 ] == '?' ) ) {
			return path[ 3 ].IsDirectorySeparator();
		}

		return false;
	}

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean IsDirectorySeparator( this Char c ) => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean IsPartiallyQualified( this String path ) {
		TrimAndThrowIfBlank( ref path );

		if ( path.Length < 2 ) {
			return true;
		}

		if ( path[ 0 ].IsDirectorySeparator() ) {
			if ( path[ 1 ] != '?' ) {
				return !path[ 1 ].IsDirectorySeparator();
			}

			return false;
		}

		if ( path.Length >= 3 && path[ 1 ] == Path.VolumeSeparatorChar && path[ 2 ].IsDirectorySeparator() ) {
			return !path[ 0 ].IsValidDriveChar();
		}

		return true;
	}

	/// <summary>Returns true if the path is too long</summary>
	[DebuggerStepThrough]
	public static Boolean IsPathTooLong( this String fullPath ) => fullPath.TrimAndThrowIfBlank().Length >= Constants.MaxPathLength;

	[DebuggerStepThrough]
	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean IsValidDriveChar( this Char value ) => value is >= 'A' and <= 'Z' or >= 'a' and <= 'z';

	/// <summary>
	///     Returns the trimmed <paramref name="path" /> or throws <see cref="ArgumentException" /> if null, empty, or
	///     whitespace.
	/// </summary>
	/// <param name="path"></param>
	/// <exception cref="ArgumentException">Gets thrown if the <paramref name="path" /> is null, empty, or whitespace.</exception>
	[DebuggerStepThrough]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static String TrimAndThrowIfBlank( this String? path ) {
		path = path?.Trim();

		if ( String.IsNullOrEmpty( path ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( path ) );
		}

		return path;
	}

	/// <summary>
	///     Trims <paramref name="path" /> and throws <see cref="ArgumentException" /> if null, empty, or whitespace.
	///     <para>Returns true if the <paramref name="path" /> was blank.</para>
	/// </summary>
	/// <param name="path"></param>
	/// <exception cref="ArgumentException">Gets thrown if the <paramref name="path" /> is null, empty, or whitespace.</exception>
	[DebuggerStepThrough]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean TrimAndThrowIfBlank( [DoesNotReturnIf( true )] ref String? path ) {
		path = path.Trim();

		if ( !String.IsNullOrEmpty( path ) ) {
			return false;
		}

		throw new ArgumentException( "Value cannot be null or whitespace.", nameof( path ) );
	}

	public static class Constants {

		/// <summary>
		///     \
		/// </summary>
		public const Char Backslash = '\\';

		/// <summary>
		///     \\.\
		/// </summary>
		public const String DevicePathPrefix = TwoBackslashes + @".\";

		/// <summary>
		///     \\?\
		/// </summary>
		public const String ExtendedPathPrefix = TwoBackslashes + @"?\";

		public const Char Forwardslash = '/';

		/// <summary>
		///     255
		/// </summary>
		public const UInt32 MaxComponentLength = Byte.MaxValue;

		/// <summary>
		///     [Maximum] value could be 32767, but this gives a little wiggle room of 32512.
		/// </summary>
		public const UInt16 MaxPathLength = ( UInt16 )( Int16.MaxValue - MaxComponentLength );

		/// <summary>
		///     \\
		/// </summary>
		public const String TwoBackslashes = @"\\";

		/// <summary>
		///     \\?\UNC\
		/// </summary>
		public const String UncExtendedPathPrefix = TwoBackslashes + UncExtendedPrefixToInsert;

		/// <summary>
		///     ?\UNC\
		/// </summary>
		public const String UncExtendedPrefixToInsert = @"?\UNC\";

		/// <summary>
		///     \\
		/// </summary>
		public const String UncPathPrefix = TwoBackslashes;
	}
}