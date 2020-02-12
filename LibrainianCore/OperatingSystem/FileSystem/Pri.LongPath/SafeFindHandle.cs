namespace LibrainianCore.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using Microsoft.Win32.SafeHandles;

    public sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid {

        public SafeFindHandle() : base( true ) { }

        protected override Boolean ReleaseHandle() => NativeMethods.FindClose( this.handle );
    }
}