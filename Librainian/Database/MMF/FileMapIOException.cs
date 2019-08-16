// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FileMapIOException.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "FileMapIOException.cs" was last formatted by Protiguous on 2019/08/08 at 6:57 AM.

namespace Librainian.Database.MMF {

    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     An exception occured as a result of an invalid IO operation on any of the File mapping classes. It wraps the error
    ///     message and the underlying Win32 error code that caused the error.
    /// </summary>
    [JsonObject]
    [Serializable]
    public class FileMapIOException : IOException {

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

        public Int32 Win32ErrorCode { get; }

        protected FileMapIOException( [NotNull] SerializationInfo info, StreamingContext context ) : base( info, context ) { }

        // construction
        public FileMapIOException( Int32 error ) => this.Win32ErrorCode = error;

        public FileMapIOException( String message ) : base( message ) { }

        public FileMapIOException( String message, Exception innerException ) : base( message, innerException ) { }
    }
}