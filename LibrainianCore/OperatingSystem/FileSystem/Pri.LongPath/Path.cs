namespace LibrainianCore.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using JetBrains.Annotations;

    public static class Path {

        [NotNull]
        public const String LongPathPrefix = @"\\?\";

        [NotNull]
        public const String UNCLongPathPrefix = @"\\?\UNC\";

        public const Char VolumeSeparatorChar = ':';

        public static readonly Char AltDirectorySeparatorChar = System.IO.Path.AltDirectorySeparatorChar;

        public static readonly Char DirectorySeparatorChar = System.IO.Path.DirectorySeparatorChar;

        [NotNull]
        public static readonly Char[] InvalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();

        [NotNull]
        public static readonly Char[] InvalidPathChars = System.IO.Path.GetInvalidPathChars();

        public static readonly Char PathSeparator = System.IO.Path.PathSeparator;

        private static Int32 GetUncRootLength( [NotNull]  this String path ) {

            var components = path.ThrowIfBlank().Split( new[] {
                DirectorySeparatorChar
            }, StringSplitOptions.RemoveEmptyEntries );

            return components.Length >= 2 ? $@"\\{components[ 0 ]}\{components[ 1 ]}\".Length : throw new InvalidOperationException( "Invalid path components." );
        }

        [NotNull]
        public static String AddLongPathPrefix( [NotNull]  this String path ) {
            path = path.ThrowIfBlank();

            if ( path.StartsWith( LongPathPrefix ) ) {
                return path;
            }

            // http://msdn.microsoft.com/en-us/library/aa365247.aspx
            if ( path.StartsWith( @"\\" ) ) {

                // UNC.
                return $"{UNCLongPathPrefix}{path.Substring( 2 )}";
            }

            return $"{LongPathPrefix}{path}";
        }

        [NotNull]
        public static String ChangeExtension( [NotNull]  this String filename, [CanBeNull] String extension ) => System.IO.Path.ChangeExtension( filename.ThrowIfBlank(), extension );

        [NotNull]
        public static String CheckAddLongPathPrefix( [NotNull]  this String path ) {
            path = path.ThrowIfBlank();

            if ( path.StartsWith( LongPathPrefix ) ) {
                return path;
            }

            var maxPathLimit = NativeMethods.MAX_PATH;

            if ( Uri.TryCreate( path.ThrowIfBlank(), UriKind.Absolute, out var uri ) && uri.IsUnc ) {

                // What's going on here?  Empirical evidence shows that Windows has trouble dealing with UNC paths
                // longer than MAX_PATH *minus* the length of the "\\hostname\" prefix.  See the following tests:
                //  - UncDirectoryTests.TestDirectoryCreateNearMaxPathLimit
                //  - UncDirectoryTests.TestDirectoryEnumerateDirectoriesNearMaxPathLimit
                var rootPathLength = 3 + uri.Host.Length;
                maxPathLimit -= rootPathLength;
            }

            if ( path.Length < maxPathLimit ) {
                return path;
            }

            return path.AddLongPathPrefix();
        }

        /// <summary></summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentNullException">Thrown if any invalid chars found in path.</exception>
        [NotNull]
        [Pure]
        public static String CheckInvalidPathChars( [NotNull] this String path ) {
            path = path.ThrowIfBlank();

            if ( path.HasIllegalCharacters() ) {
                throw new ArgumentNullException( nameof( path ), "Invalid characters in path" );
            }

            return path;
        }

        [NotNull]
        public static String Combine( [NotNull]  this String path1, [NotNull] String path2 ) {

            path1 = path1.CheckInvalidPathChars();

            path2 = path2.CheckInvalidPathChars();

            if ( path2.Length == 0 ) {
                return path1;
            }

            if ( path1.Length == 0 || path2.IsPathRooted() ) {
                return path2;
            }

            var ch = path1[ ^1 ];

            if ( ch.IsDirectorySeparator() || ch == VolumeSeparatorChar ) {
                return path1 + path2;
            }

            return $"{path1}{DirectorySeparatorChar}{path2}";
        }

        [NotNull]
        public static String Combine( [NotNull]  this String path1, [NotNull] String path2, [NotNull] String path3 ) => Combine( path1, path2 ).Combine( path3 );

        [NotNull]
        public static String Combine( [NotNull]  this String path1, [NotNull] String path2, [NotNull] String path3, [NotNull] String path4 ) => Combine( path1.Combine( path2 ), path3 ).Combine( path4 );

        [NotNull]
        public static String Combine( [NotNull] [ItemNotNull] params String[] paths ) {
            if ( paths == null ) {
                throw new ArgumentNullException( nameof( paths ) );
            }

            switch ( paths.Length ) {
                case 0: return String.Empty;
                case 1: {
                        var z = paths[ 0 ];

                        if ( z == null ) {
                            throw new ArgumentException( "Value cannot be null or whitespace." );
                        }

                        return z.CheckInvalidPathChars().ThrowIfBlank();
                    }
                default: {
                        var z = paths[ 0 ];

                        if ( z == null ) {
                            throw new ArgumentException( "Value cannot be null or whitespace." );
                        }

                        var path = z.CheckInvalidPathChars().ThrowIfBlank();

                        for ( var i = 1; i < paths.Length; ++i ) {
                            path = path.Combine( paths[ i ] );
                        }

                        return path;
                    }
            }
        }

        [NotNull]
        public static String GetDirectoryName( [NotNull]  this String path ) {
            path = path.ThrowIfBlank().CheckInvalidPathChars();

            String basePath = null;

            if ( !path.IsPathRooted() ) {
                basePath = System.IO.Directory.GetCurrentDirectory();
            }

            path = path.NormalizeLongPath().RemoveLongPathPrefix();
            var rootLength = path.GetRootLength();

            if ( path.Length <= rootLength ) {
                return String.Empty;
            }

            var length = path.Length;

            do { } while ( length > rootLength && !path[ --length ].IsDirectorySeparator() );

            if ( basePath == null ) {
                return path.Substring( 0, length );
            }

            path = path.Substring( basePath.Length + 1 );
            length = length - basePath.Length - 1;

            if ( length < 0 ) {
                length = 0;
            }

            return path.Substring( 0, length );
        }

        [CanBeNull]
        public static String GetExtension( [NotNull]  this String path ) => System.IO.Path.GetExtension( path.ThrowIfBlank() );

        [NotNull]
        public static String GetFileName( [NotNull]  this String path ) => System.IO.Path.GetFileName( path.NormalizeLongPath() );

        [CanBeNull]
        public static String GetFileNameWithoutExtension( [NotNull]  this String path ) => System.IO.Path.GetFileNameWithoutExtension( path.ThrowIfBlank() );

        [NotNull]
        public static String GetFullPath( [NotNull] this String path ) {
            path = path.ThrowIfBlank();

            return path.IsPathUnc() ? path : path.NormalizeLongPath().RemoveLongPathPrefix();
        }

        [NotNull]
        public static IEnumerable<Char> GetInvalidFileNameChars() => InvalidFileNameChars;

        [NotNull]
        public static IEnumerable<Char> GetInvalidPathChars() => InvalidPathChars;

        [NotNull]
        public static String GetPathRoot( [NotNull]  this String path ) {
            path = path.ThrowIfBlank();

            if ( !path.IsPathRooted() ) {
                return String.Empty;
            }

            if ( !path.IsPathUnc() ) {
                path = path.NormalizeLongPath().RemoveLongPathPrefix();
            }

            return path.Substring( 0, path.GetRootLength() );
        }

        /// <summary>Returns just the unique random file name (no path) with an optional <paramref name="extension"/>.
        /// <remarks>Does not create a file.</remarks>
        /// </summary>
        /// <param name="extension"></param>
        [NotNull]
        public static String GetRandomFileName( String? extension = null ) {
            if ( String.IsNullOrEmpty( extension ) ) {
                extension = $"{Guid.NewGuid():D}";
            }

            return $"{Guid.NewGuid():D}.{extension}";
        }

        public static Int32 GetRootLength( [NotNull]  this String path ) {
            path = path.ThrowIfBlank();

            if ( path.IsPathUnc() ) {
                return path.GetUncRootLength();
            }

            path = path.GetFullPath().CheckInvalidPathChars();

            var rootLength = 0;
            var length = path.Length;

            if ( length >= 1 && path[ 0 ].IsDirectorySeparator() ) {
                rootLength = 1;

                if ( length >= 2 && path[ 1 ].IsDirectorySeparator() ) {
                    rootLength = 2;
                    var num = 2;

                    while ( rootLength >= length ||
                            ( ( path[ rootLength ] == System.IO.Path.DirectorySeparatorChar || path[ rootLength ] == System.IO.Path.AltDirectorySeparatorChar ) && --num <= 0 ) ) {
                        ++rootLength;
                    }
                }
            }
            else if ( length >= 2 && path[ 1 ] == System.IO.Path.VolumeSeparatorChar ) {
                rootLength = 2;

                if ( length >= 3 && path[ 2 ].IsDirectorySeparator() ) {
                    ++rootLength;
                }
            }

            return rootLength;
        }

        [NotNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String GetTempFileName( [CanBeNull] String? extension = null ) => GetRandomFileName( extension );

        [NotNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String GetTempPath() => System.IO.Path.GetTempPath().ThrowIfBlank();

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean HasExtension( [NotNull]  this String path ) => System.IO.Path.HasExtension( path.ThrowIfBlank() );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean HasIllegalCharacters( [NotNull]  this String path ) => path.ThrowIfBlank().Any( InvalidPathChars.Contains );

        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsDirectorySeparator( this Char c ) => c == DirectorySeparatorChar || c == AltDirectorySeparatorChar;

        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsPathRooted( [NotNull]  this String path ) => System.IO.Path.IsPathRooted( path.ThrowIfBlank() );

        /// <summary>Normalizes path (can be longer than MAX_PATH) and adds \\?\ long path prefix</summary>
        /// <param name="path"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        [NotNull]
        public static String NormalizeLongPath( [NotNull]  this String path, [NotNull] String parameterName = "path" ) {
            path = path.ThrowIfBlank();

            if ( path.IsPathUnc() ) {
                return path.CheckAddLongPathPrefix();
            }

            var buffer = new StringBuilder( path.Length + 1 ); // Add 1 for NULL
            var length = NativeMethods.GetFullPathNameW( path.ThrowIfBlank(), ( UInt32 )buffer.Capacity, buffer, IntPtr.Zero );

            if ( length > buffer.Capacity ) {

                // Resulting path longer than our buffer, so increase it

                buffer.Capacity = ( Int32 )length;
                length = NativeMethods.GetFullPathNameW( path.ThrowIfBlank(), length, buffer, IntPtr.Zero );
            }

            if ( length == 0 ) {
                throw Common.GetExceptionFromLastWin32Error( parameterName );
            }

            if ( length > NativeMethods.MAX_LONG_PATH ) {
                throw Common.GetExceptionFromWin32Error( NativeMethods.ERROR_FILENAME_EXCED_RANGE, parameterName );
            }

            if ( length > 1 && buffer[ 0 ].IsDirectorySeparator() && buffer[ 1 ].IsDirectorySeparator() ) {
                if ( length < 2 ) {
                    throw new ArgumentException( @"The UNC path should be of the form \\server\share." );
                }

                var parts = buffer.ToString().Split( new[] {
                    DirectorySeparatorChar
                }, StringSplitOptions.RemoveEmptyEntries );

                if ( parts.Length < 2 ) {
                    throw new ArgumentException( @"The UNC path should be of the form \\server\share." );
                }
            }

            return buffer.ToString().AddLongPathPrefix();
        }

        [NotNull]
        public static String RemoveLongPathPrefix( [NotNull]  this String normalizedPath ) {

            if ( String.IsNullOrWhiteSpace( normalizedPath ) || !normalizedPath.StartsWith( LongPathPrefix ) ) {
                return normalizedPath;
            }

            if ( normalizedPath.StartsWith( UNCLongPathPrefix, StringComparison.Ordinal ) ) {
                return $@"\\{normalizedPath.Substring( UNCLongPathPrefix.Length )}";
            }

            return normalizedPath.Substring( LongPathPrefix.Length );
        }

        public static Boolean TryNormalizeLongPath( [NotNull]  this String path, [CanBeNull] out String result ) {
            if ( !String.IsNullOrWhiteSpace( path ) ) {
                try {
                    result = path.NormalizeLongPath();

                    return true;
                }
                catch ( ArgumentException ) { }
                catch ( PathTooLongException ) { }
            }

            result = null;

            return false;
        }
    }
}