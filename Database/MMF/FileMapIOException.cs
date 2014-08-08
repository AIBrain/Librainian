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
// "Librainian2/FileMapIOException.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM
#endregion

namespace Librainian.Database.MMF {
    using System;
    using System.IO;

    /// <summary>Exception class thrown by the library</summary>
    /// <remarks>
    ///     Represents an exception occured as a result of an
    ///     invalid IO operation on any of the File mapping classes
    ///     It wraps the error message and the underlying Win32 error
    ///     code that caused the error.
    /// </remarks>
    [Serializable]
    public class FileMapIOException : IOException {
        //
        // properties
        //

        // construction
        public FileMapIOException( int error ) {
            this.Win32ErrorCode = error;
        }

        public FileMapIOException( string message ) : base( message ) { }

        public FileMapIOException( string message, Exception innerException ) : base( message, innerException ) { }

        public int Win32ErrorCode { get; private set; }

        public override string Message {
            get {
                if ( this.Win32ErrorCode == 0 ) {
                    return base.Message;
                }
                //if ( this.Win32ErrorCode == 0x80070008 ) {
                //    return base.Message + " Not enough address space available (" + this.Win32ErrorCode + ")";
                //}
                return base.Message + " (" + this.Win32ErrorCode.ToString( "X" ) + ")";
            }
        }
    }
}
