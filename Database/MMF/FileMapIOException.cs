// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FileMapIOException.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/FileMapIOException.cs" was last cleaned by Protiguous on 2018/05/15 at 10:39 PM.

namespace Librainian.Database.MMF {

    using System;

    using System.IO;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    ///     Exception class thrown by the Library
    /// </summary>
    /// <remarks>
    ///     Represents an exception occured as a result of an invalid IO operation on any of the File mapping classes It
    ///     wraps the error message and the underlying Win32 error code that caused the error.
    /// </remarks>
    [SuppressMessage( "Microsoft.Usage", "CA2240:ImplementISerializableCorrectly" )]
    [JsonObject]
    [Serializable]
    public class FileMapIOException : IOException {

        protected FileMapIOException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }

        // construction
        public FileMapIOException( Int32 error ) => this.Win32ErrorCode = error;

        public FileMapIOException( String message ) : base( message ) { }

        public FileMapIOException( String message, Exception innerException ) : base( message, innerException ) { }

        public override String Message {
            get {
                if ( this.Win32ErrorCode == 0 ) { return base.Message; }

                //if ( this.Win32ErrorCode == 0x80070008 ) {
                //    return base.Message + " Not enough address space available (" + this.Win32ErrorCode + ")";
                //}
                return base.Message + " (" + this.Win32ErrorCode.ToString( "X" ) + ")";
            }
        }

        public Int32 Win32ErrorCode { get; }
    }
}