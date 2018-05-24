// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DerivedSerializableExceptionWithAdditionalCustomProperty.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/DerivedSerializableExceptionWithAdditionalCustomProperty.cs" was last formatted by Protiguous on 2018/05/22 at 5:53 PM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using Newtonsoft.Json;

    [JsonObject]
    [Serializable]
    public sealed class DerivedSerializableExceptionWithAdditionalCustomProperty : SerializableExceptionWithCustomProperties {

        public String Username { get; }

        [SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]

        // Serialization constructor is private, as this class is sealed
        private DerivedSerializableExceptionWithAdditionalCustomProperty( SerializationInfo info, StreamingContext context ) : base( info, context ) => this.Username = info.GetString( "Username" );

        public DerivedSerializableExceptionWithAdditionalCustomProperty() { }

        public DerivedSerializableExceptionWithAdditionalCustomProperty( String message ) : base( message ) { }

        public DerivedSerializableExceptionWithAdditionalCustomProperty( String message, Exception innerException ) : base( message, innerException ) { }

        public DerivedSerializableExceptionWithAdditionalCustomProperty( String message, String username, String resourceName, IList<String> validationErrors ) : base( message, resourceName, validationErrors ) =>
            this.Username = username;

        public DerivedSerializableExceptionWithAdditionalCustomProperty( String message, String username, String resourceName, IList<String> validationErrors, Exception innerException ) : base( message, resourceName,
            validationErrors, innerException ) =>
            this.Username = username;

        public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
            if ( info is null ) { throw new ArgumentNullException( nameof( info ) ); }

            info.AddValue( "Username", this.Username );
            base.GetObjectData( info, context );
        }
    }
}