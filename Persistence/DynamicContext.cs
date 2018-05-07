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
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/DynamicContext.cs" was last cleaned by Protiguous on 2016/06/18 at 10:56 PM

namespace Librainian.Persistence {

    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using Newtonsoft.Json;

    /// <summary></summary>
    /// <seealso cref="http://stackoverflow.com/a/4857322/956364" />
    [JsonObject]
    [Serializable]
    public class DynamicContext : DynamicObject, ISerializable {
        private readonly Dictionary<String, Object> _dynamicContext = new Dictionary<String, Object>();

        public DynamicContext() {
        }

        protected DynamicContext( SerializationInfo info, StreamingContext context ) {

            // TODO: validate inputs before deserializing. See http://msdn.microsoft.com/en-us/Library/ty01x675(VS.80).aspx
            foreach ( var entry in info ) {
                this._dynamicContext.Add( entry.Name, entry.Value );
            }
        }

        [SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]
        public virtual void GetObjectData( SerializationInfo info, StreamingContext context ) {
            foreach ( var kvp in this._dynamicContext ) {
                info.AddValue( kvp.Key, kvp.Value );
            }
        }

        public override Boolean TryGetMember( GetMemberBinder binder, out Object result ) => this._dynamicContext.TryGetValue( binder.Name, out result );

        public override Boolean TrySetMember( SetMemberBinder binder, Object value ) {
            this._dynamicContext.Add( binder.Name, value );
            return true;
        }
    }
}