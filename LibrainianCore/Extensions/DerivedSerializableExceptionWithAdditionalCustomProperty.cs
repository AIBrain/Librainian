// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DerivedSerializableExceptionWithAdditionalCustomProperty.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "DerivedSerializableExceptionWithAdditionalCustomProperty.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

namespace LibrainianCore.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [JsonObject]
    [Serializable]
    public sealed class DerivedSerializableExceptionWithAdditionalCustomProperty : SerializableExceptionWithCustomProperties {

        public String Username { get; }

        [SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]

        // Serialization constructor is private, as this class is sealed
        private DerivedSerializableExceptionWithAdditionalCustomProperty( [NotNull] SerializationInfo info, StreamingContext context ) : base( info, context ) =>
            this.Username = info.GetString( "Username" );

        public DerivedSerializableExceptionWithAdditionalCustomProperty() { }

        public DerivedSerializableExceptionWithAdditionalCustomProperty( [CanBeNull] String message ) : base( message ) { }

        public DerivedSerializableExceptionWithAdditionalCustomProperty( [CanBeNull] String message, [CanBeNull] Exception innerException ) :
            base( message, innerException ) { }

        public DerivedSerializableExceptionWithAdditionalCustomProperty( [CanBeNull] String message, [CanBeNull] String username, [CanBeNull] String resourceName,
            [CanBeNull] IList<String> validationErrors ) : base( message, resourceName, validationErrors ) =>
            this.Username = username;

        public DerivedSerializableExceptionWithAdditionalCustomProperty( [CanBeNull] String message, [CanBeNull] String username, [CanBeNull] String resourceName,
            [CanBeNull] IList<String> validationErrors, [CanBeNull] Exception innerException ) : base( message, resourceName, validationErrors, innerException ) =>
            this.Username = username;

        public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
            if ( info is null ) {
                throw new ArgumentNullException( nameof( info ) );
            }

            info.AddValue( "Username", this.Username );
            base.GetObjectData( info, context );
        }
    }
}