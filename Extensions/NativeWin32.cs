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
// "Librainian2/NativeWin32.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;

    public static class NativeWin32 {
        public const int MaxPath = 260;

        /// <summary>
        ///     Closes a file search handle opened by the FindFirstFile, FindFirstFileEx, or FindFirstStreamW function.
        /// </summary>
        /// <param name="hFindFile">The file search handle.</param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa364413%28VS.85%29.aspx" />
        [DllImport( "kernel32", SetLastError = true )]
        public static extern Boolean FindClose( IntPtr hFindFile );

        /// <summary>
        ///     Searches a directory for a file or subdirectory with a name that matches a specific name (or partial name if
        ///     wildcards are used).
        /// </summary>
        /// <param name="lpFileName">
        ///     The directory or path, and the file name, which can include wildcard characters, for example,
        ///     an asterisk (*) or a question mark (?).
        /// </param>
        /// <param name="lpFindData">
        ///     A pointer to the WIN32_FIND_DATA structure that receives information about a found file or
        ///     directory.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is a search handle used in a subsequent call to FindNextFile or
        ///     FindClose, and the lpFindFileData parameter contains information about the first file or directory found.
        ///     If the function fails or fails to locate files from the search String in the lpFileName parameter, the return value
        ///     is INVALID_HANDLE_VALUE and the contents of lpFindFileData are indeterminate.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa364418%28VS.85%29.aspx" />
        [DllImport( "kernel32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false )]
        public static extern SafeSearchHandle FindFirstFile( String lpFileName, out Win32FindData lpFindData );

        /// <summary>
        ///     Continues a file search from a previous call to the FindFirstFile or FindFirstFileEx function.
        /// </summary>
        /// <param name="hFindFile">The search handle returned by a previous call to the FindFirstFile or FindFirstFileEx function.</param>
        /// <param name="lpFindData">
        ///     A pointer to the WIN32_FIND_DATA structure that receives information about the found file or subdirectory.
        ///     The structure can be used in subsequent calls to FindNextFile to indicate from which file to continue the search.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero and the lpFindFileData parameter contains information about
        ///     the next file or directory found.
        ///     If the function fails, the return value is zero and the contents of lpFindFileData are indeterminate.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa364428%28VS.85%29.aspx" />
        [DllImport( "kernel32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false )]
        public static extern Boolean FindNextFile( SafeSearchHandle hFindFile, out Win32FindData lpFindData );
        /// <summary>
        ///     Win32 FILETIME structure.  The win32 documentation says this:
        ///     "Contains a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601 (UTC)."
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms724284%28VS.85%29.aspx" />
        [StructLayout( LayoutKind.Sequential )]
        public struct FILETIME {
            public uint dwLowDateTime;

            public uint dwHighDateTime;
        }

        /// <summary>
        ///     The Win32 find data structure.  The documentation says:
        ///     "Contains information about the file that is found by the FindFirstFile, FindFirstFileEx, or FindNextFile
        ///     function."
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa365740%28VS.85%29.aspx" />
        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
        public struct Win32FindData {
            public FileAttributes dwFileAttributes;

            public FILETIME ftCreationTime;

            public FILETIME ftLastAccessTime;

            public FILETIME ftLastWriteTime;

            public uint nFileSizeHigh;

            public uint nFileSizeLow;

            public uint dwReserved0;

            public uint dwReserved1;

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = MaxPath )]
            public String cFileName;

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 14 )]
            public String cAlternateFileName;
        }

        /// <summary>
        ///     Class to encapsulate a seach handle returned from FindFirstFile.  Using a wrapper
        ///     like this ensures that the handle is properly cleaned up with FindClose.
        /// </summary>
        public class SafeSearchHandle : SafeHandleZeroOrMinusOneIsInvalid {
            public SafeSearchHandle() : base( true ) { }

            protected override Boolean ReleaseHandle() {
                return FindClose( this.handle );
            }
        }
    }
}
