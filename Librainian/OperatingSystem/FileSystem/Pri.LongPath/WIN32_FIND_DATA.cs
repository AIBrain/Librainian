namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;
    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
    public struct WIN32_FIND_DATA {

        public readonly FileAttributes dwFileAttributes;

        public FILETIME ftCreationTime;

        public FILETIME ftLastAccessTime;

        public FILETIME ftLastWriteTime;

        public readonly Int32 nFileSizeHigh;

        public readonly Int32 nFileSizeLow;

        public readonly Int32 dwReserved0;

        public readonly Int32 dwReserved1;

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = NativeMethods.MAX_PATH )]
        [NotNull]
        public readonly String cFileName;

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = NativeMethods.MAX_ALTERNATE )]
        [CanBeNull]
        public readonly String cAlternate;
    }
}