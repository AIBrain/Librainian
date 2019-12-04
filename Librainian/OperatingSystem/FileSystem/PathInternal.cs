// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "PathInternal.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "PathInternal.cs" was last formatted by Protiguous on 2019/08/08 at 9:18 AM.

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

        [DllImport( dllName: "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false )]
        private static extern UInt32 GetLongPathNameW( this String lpszShortPath, StringBuilder lpszLongPath, UInt32 cchBuffer );

        [Pure]
        [NotNull]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static String EnsureExtendedPrefix( [NotNull] this String path ) {
            path = path.TrimAndThrowIfBlank();

            if ( path.IsPartiallyQualified() || path.IsDevice() ) {
                return path;
            }

            if ( path.StartsWith( value: Constants.UncPathPrefix, comparisonType: StringComparison.OrdinalIgnoreCase ) ) {
                return path.Insert( startIndex: 2, value: @"?\UNC\" );
            }

            return $"{Constants.ExtendedPathPrefix}{path}";
        }

        [NotNull]
        public static String GetLongPathName( [NotNull] this String path ) {
            path = path.TrimAndThrowIfBlank();

            var stringBuffer = new StringBuilder( capacity: Constants.MaxPathLength );
            path.GetLongPathNameW( lpszLongPath: stringBuffer, cchBuffer: ( UInt32 ) stringBuffer.Capacity );

            return stringBuffer.ToString();
        }

        [Pure]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean IsDevice( [NotNull] this String path ) {
            path = path.TrimAndThrowIfBlank();

            if ( Document.IsExtended( path: path ) ) {
                return true;
            }

            if ( path.Length >= 4 && path[ index: 0 ].IsDirectorySeparator() && path[ index: 1 ].IsDirectorySeparator() &&
                 ( path[ index: 2 ] == '.' || path[ index: 2 ] == '?' ) ) {
                return path[ index: 3 ].IsDirectorySeparator();
            }

            return false;
        }

        [Pure]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean IsDirectorySeparator( this Char c ) => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;

        [Pure]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean IsPartiallyQualified( [NotNull] this String path ) {
            path = path.TrimAndThrowIfBlank();

            if ( path.Length < 2 ) {
                return true;
            }

            if ( path[ index: 0 ].IsDirectorySeparator() ) {
                if ( path[ index: 1 ] != '?' ) {
                    return !path[ index: 1 ].IsDirectorySeparator();
                }

                return false;
            }

            if ( path.Length >= 3 && path[ index: 1 ] == Path.VolumeSeparatorChar && path[ index: 2 ].IsDirectorySeparator() ) {
                return !path[ index: 0 ].IsValidDriveChar();
            }

            return true;
        }

        /// <summary>
        ///     Returns true if the path is too long
        /// </summary>
        [DebuggerStepThrough]
        public static Boolean IsPathTooLong( [NotNull] this String fullPath ) => fullPath.TrimAndThrowIfBlank().Length >= Constants.MaxPathLength;

        [DebuggerStepThrough]
        [Pure]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean IsValidDriveChar( this Char value ) => (value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z');

        /// <summary>
        ///     Returns the trimmed <paramref name="path" /> or throws <see cref="ArgumentException" /> if null, empty, or
        ///     whitespace.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentException">Gets thrown if the <paramref name="path" /> is null, empty, or whitespace.</exception>
        [DebuggerStepThrough]
        [NotNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String TrimAndThrowIfBlank( [NotNull] this String path ) {

            if ( String.IsNullOrWhiteSpace( value: path ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( path ) );
            }

            path = path.Trimmed();

            if ( String.IsNullOrEmpty( value: path ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( path ) );
            }

            return path;
        }

        public static class Constants {

            public const Char Backslash = '\\';

            public const String DevicePathPrefix = @"\\.\";

            public const String ExtendedPathPrefix = @"\\?\";

            public const Char Forwardslash = '/';

            public const UInt32 MaxComponentLength = Byte.MaxValue;

            public const UInt16 MaxPathLength = ( UInt16 ) Int16.MaxValue;

            public const String TwoBackslashes = @"\\";

            public const String UncExtendedPathPrefix = @"\\?\UNC\";

            public const String UncExtendedPrefixToInsert = @"?\UNC\";

            public const String UncPathPrefix = @"\\";
        }
    }
}