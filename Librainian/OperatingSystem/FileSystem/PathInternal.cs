// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "PathInternal.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "PathInternal.cs" was last formatted by Protiguous on 2020/03/16 at 2:58 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using JetBrains.Annotations;
    using Parsing;

    public static class PathInternal {

        [DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false )]
        private static extern UInt32 GetLongPathNameW( this String lpszShortPath, StringBuilder lpszLongPath, UInt32 cchBuffer );

        [Pure]
        [NotNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String EnsureExtendedPrefix( [NotNull] this String path ) {
            path = path.TrimAndThrowIfBlank();

            if ( path.IsPartiallyQualified() || path.IsDevice() ) {
                return path;
            }

            if ( path.StartsWith( Constants.UncPathPrefix, StringComparison.OrdinalIgnoreCase ) ) {
                return path.Insert( 2, @"?\UNC\" );
            }

            return $"{Constants.ExtendedPathPrefix}{path}";
        }

        [NotNull]
        public static String GetLongPathName( [NotNull] this String path ) {
            path = path.TrimAndThrowIfBlank();

            var stringBuffer = new StringBuilder( Constants.MaxPathLength );
            path.GetLongPathNameW( stringBuffer, ( UInt32 )stringBuffer.Capacity );

            return stringBuffer.ToString();
        }

        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsDevice( [NotNull] this String path ) {
            path = path.TrimAndThrowIfBlank();

            if ( Document.IsExtended( path ) ) {
                return true;
            }

            if ( path.Length >= 4 && path[ 0 ].IsDirectorySeparator() && path[ 1 ].IsDirectorySeparator() && ( path[ 2 ] == '.' || path[ 2 ] == '?' ) ) {
                return path[ 3 ].IsDirectorySeparator();
            }

            return default;
        }

        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsDirectorySeparator( this Char c ) => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;

        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsPartiallyQualified( [NotNull] this String path ) {
            path = path.TrimAndThrowIfBlank();

            if ( path.Length < 2 ) {
                return true;
            }

            if ( path[ 0 ].IsDirectorySeparator() ) {
                if ( path[ 1 ] != '?' ) {
                    return !path[ 1 ].IsDirectorySeparator();
                }

                return default;
            }

            if ( path.Length >= 3 && path[ 1 ] == Path.VolumeSeparatorChar && path[ 2 ].IsDirectorySeparator() ) {
                return !path[ 0 ].IsValidDriveChar();
            }

            return true;
        }

        /// <summary>Returns true if the path is too long</summary>
        [DebuggerStepThrough]
        public static Boolean IsPathTooLong( [NotNull] this String fullPath ) => fullPath.TrimAndThrowIfBlank().Length >= Constants.MaxPathLength;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsValidDriveChar( this Char value ) => value >= 'A' && value <= 'Z' || value >= 'a' && value <= 'z';

        /// <summary>Returns the trimmed <paramref name="path" /> or throws <see cref="ArgumentException" /> if null, empty, or whitespace.</summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentException">Gets thrown if the <paramref name="path" /> is null, empty, or whitespace.</exception>
        [DebuggerStepThrough]
        [NotNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String TrimAndThrowIfBlank( [NotNull] this String path ) {

            if ( String.IsNullOrWhiteSpace( path ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( path ) );
            }

            path = path.Trimmed();

            if ( String.IsNullOrEmpty( path ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( path ) );
            }

            return path;
        }

        public static class Constants {

            public const Char Backslash = '\\';

            public const String DevicePathPrefix = @"\\.\";

            public const String ExtendedPathPrefix = @"\\?\";

            public const Char Forwardslash = '/';

            public const UInt32 MaxComponentLength = Byte.MaxValue;

            public const UInt16 MaxPathLength = ( UInt16 )Int16.MaxValue;

            public const String TwoBackslashes = @"\\";

            public const String UncExtendedPathPrefix = @"\\?\UNC\";

            public const String UncExtendedPrefixToInsert = @"?\UNC\";

            public const String UncPathPrefix = @"\\";
        }
    }
}