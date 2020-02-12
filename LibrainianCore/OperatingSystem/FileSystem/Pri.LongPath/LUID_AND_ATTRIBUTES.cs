namespace LibrainianCore.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.Runtime.InteropServices;

    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
    public struct LUID_AND_ATTRIBUTES {

        public LUID Luid;

        public UInt32 Attributes;
    }
}