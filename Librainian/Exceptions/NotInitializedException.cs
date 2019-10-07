// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "NotInitializedException.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "NotInitializedException.cs" was last formatted by Protiguous on 2019/10/03 at 12:10 PM.

namespace Librainian.Exceptions {

    using System;
    using System.Runtime.Serialization;
    using Logging;
    using Newtonsoft.Json;

    /// <summary>
    ///     Throw when the object has not been initialized.
    ///     <para><see cref="Logging.Log(string,bool)" /> gets called.</para>
    /// </summary>
    [Serializable]
    [JsonObject]
    public class NotInitializedException : Exception {

        public String Parameter { get; }

        private NotInitializedException() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotInitializedException" /> class with serialized data.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="SerializationInfo" /> instance that holds the serialized object data about the
        ///     exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="StreamingContext" /> instance that contains contextual information about the
        ///     source or destination.
        /// </param>
        /// <remarks>
        ///     This constructor overload is provided in order to adhere to custom exception design best practice guidelines.
        /// </remarks>
        protected NotInitializedException( SerializationInfo info, StreamingContext context ) : base( info, context ) {
            $"{nameof( NotInitializedException )} serialization exception.".Log( breakinto: true );
        }

        public NotInitializedException( String message ) : base( message ) => message.Log();

        public NotInitializedException( String message, String paramName ) : this( message ) => this.Parameter = paramName;

        public NotInitializedException( String message, Exception inner ) : base( message, inner ) => message.Log();

        public NotInitializedException( String message, String paramName, Exception inner ) : this( message, inner ) => this.Parameter = paramName;

    }

}