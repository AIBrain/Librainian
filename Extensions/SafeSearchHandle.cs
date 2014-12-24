namespace Librainian.Extensions {
    using System;
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// Class to encapsulate a seach handle returned from FindFirstFile. Using a wrapper like
    /// this ensures that the handle is properly cleaned up with FindClose.
    /// </summary>
    public class SafeSearchHandle : SafeHandleZeroOrMinusOneIsInvalid {

        public SafeSearchHandle() : base( true ) {
        }

        protected override Boolean ReleaseHandle() => NativeWin32.FindClose( this.handle );
    }
}