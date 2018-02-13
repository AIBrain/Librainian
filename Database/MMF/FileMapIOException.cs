// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FileMapIOException.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Database.MMF {

    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>Exception class thrown by the Library</summary>
    /// <remarks>
    ///     Represents an exception occured as a result of an invalid IO operation on any of the File
    ///     mapping classes It wraps the error message and the underlying Win32 error code that caused
    ///     the error.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2240:ImplementISerializableCorrectly" )]
    [JsonObject]
    [Serializable]
    public class FileMapIOException : IOException {

        // construction
        public FileMapIOException( Int32 error ) => this.Win32ErrorCode = error;

	    public FileMapIOException( String message ) : base( message ) {
        }

        public FileMapIOException( String message, Exception innerException ) : base( message, innerException ) {
        }

        protected FileMapIOException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }

        public override String Message {
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

        public Int32 Win32ErrorCode {
            get;
        }
    }
}