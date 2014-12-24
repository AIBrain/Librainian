namespace Librainian.Extensions {
    using System;
    using IO;

    public delegate CopyProgressResult CopyProgressRoutine( long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, uint dwStreamNumber, CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData );
}