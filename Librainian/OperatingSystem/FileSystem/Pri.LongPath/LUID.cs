namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.Runtime.InteropServices;

    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
    public struct LUID {

        public UInt32 LowPart;

        public UInt32 HighPart;
    }
}