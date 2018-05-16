// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "SerializableExceptionWithCustomProperties.cs",
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
// "Librainian/Librainian/SerializableExceptionWithCustomProperties.cs" was last cleaned by Protiguous on 2018/05/15 at 10:40 PM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using Newtonsoft.Json;

    // Important: This attribute is NOT inherited from Exception, and MUST be specified otherwise serialization will fail with a SerializationException stating that "Type X in Assembly Y is not marked as serializable."
    [JsonObject]
    [Serializable]
    public class SerializableExceptionWithCustomProperties : Exception {

        /// <summary>
        ///     Pulled from
        /// </summary>
        /// <param name="info">   </param>
        /// <param name="context"></param>
        [SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]

        // Constructor should be protected for unsealed classes, private for sealed classes. (The Serializer invokes this constructor through reflection, so it can be private)
        protected SerializableExceptionWithCustomProperties( SerializationInfo info, StreamingContext context ) : base( info, context ) {
            this.ResourceName = info.GetString( "ResourceName" );
            this.ValidationErrors = ( IList<String> )info.GetValue( "ValidationErrors", typeof( IList<String> ) );
        }

        public SerializableExceptionWithCustomProperties() { }

        public SerializableExceptionWithCustomProperties( String message ) : base( message ) { }

        public SerializableExceptionWithCustomProperties( String message, Exception innerException ) : base( message, innerException ) { }

        public SerializableExceptionWithCustomProperties( String message, String resourceName, IList<String> validationErrors ) : base( message ) {
            this.ResourceName = resourceName;
            this.ValidationErrors = validationErrors;
        }

        public SerializableExceptionWithCustomProperties( String message, String resourceName, IList<String> validationErrors, Exception innerException ) : base( message, innerException ) {
            this.ResourceName = resourceName;
            this.ValidationErrors = validationErrors;
        }

        public String ResourceName { get; }

        public IList<String> ValidationErrors { get; }

        [SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]
        public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
            if ( info is null ) { throw new ArgumentNullException( nameof( info ) ); }

            info.AddValue( "ResourceName", this.ResourceName );

            // Note: if "List<T>" isn't serializable you may need to work out another method of adding your list, this is just for show...
            info.AddValue( "ValidationErrors", this.ValidationErrors, typeof( IList<String> ) );

            // MUST call through to the base class to let it save its own state
            base.GetObjectData( info, context );
        }
    }
}