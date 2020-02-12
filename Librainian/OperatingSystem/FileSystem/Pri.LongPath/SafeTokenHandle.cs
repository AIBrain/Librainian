namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using JetBrains.Annotations;
    using Microsoft.Win32.SafeHandles;

    public class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid {

        [NotNull]
        public static SafeTokenHandle InvalidHandle => new SafeTokenHandle( IntPtr.Zero );

        private SafeTokenHandle() : base( true ) { }

        // 0 is an Invalid Handle
        public SafeTokenHandle( IntPtr handle ) : base( true ) => this.SetHandle( handle );

        [DllImport( DLL.kernel32, BestFitMapping = false, SetLastError = true )]
        [SuppressUnmanagedCodeSecurity]
        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        private static extern Boolean CloseHandle( IntPtr handle );

        protected override Boolean ReleaseHandle() => CloseHandle( this.handle );
    }
}