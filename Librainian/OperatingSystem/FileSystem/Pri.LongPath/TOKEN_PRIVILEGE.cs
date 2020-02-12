namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.Runtime.InteropServices;

    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
    public struct TOKEN_PRIVILEGE {

        public UInt32 PrivilegeCount;

        public LUID_AND_ATTRIBUTES Privilege;
    }
}