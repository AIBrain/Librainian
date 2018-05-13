// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/SerializableExceptionWithCustomProperties.cs" was last cleaned by Protiguous on 2018/05/12 at 1:23 AM

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
        /// Pulled from
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
            if ( info is null ) {
                throw new ArgumentNullException( nameof( info ) );
            }

            info.AddValue( "ResourceName", this.ResourceName );

            // Note: if "List<T>" isn't serializable you may need to work out another method of adding your list, this is just for show...
            info.AddValue( "ValidationErrors", this.ValidationErrors, typeof( IList<String> ) );

            // MUST call through to the base class to let it save its own state
            base.GetObjectData( info, context );
        }
    }
}