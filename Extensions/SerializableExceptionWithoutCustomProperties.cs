// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/SerializableExceptionWithoutCustomProperties.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Extensions {

    using System;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    // Important: This attribute is NOT inherited from Exception, and MUST be specified otherwise
    // serialization will fail with a SerializationException stating that "Type X in Assembly Y is
    // not marked as serializable."
    [JsonObject]
    [Serializable]
    public class SerializableExceptionWithoutCustomProperties : Exception {

        public SerializableExceptionWithoutCustomProperties() {
        }

        public SerializableExceptionWithoutCustomProperties( String message ) : base( message ) {
        }

        public SerializableExceptionWithoutCustomProperties( String message, Exception innerException ) : base( message, innerException ) {
        }

        // Without this constructor, deserialization will fail
        protected SerializableExceptionWithoutCustomProperties( SerializationInfo info, StreamingContext context ) : base( info, context ) { }
    }
}