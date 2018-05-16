// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DynamicContext.cs",
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
// "Librainian/Librainian/DynamicContext.cs" was last cleaned by Protiguous on 2018/05/15 at 10:49 PM.

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

        protected DynamicContext( SerializationInfo info, StreamingContext context ) {

            // TODO: validate inputs before deserializing. See http://msdn.microsoft.com/en-us/Library/ty01x675(VS.80).aspx
            foreach ( var entry in info ) { this._dynamicContext.Add( entry.Name, entry.Value ); }
        }

        public DynamicContext() { }

        [SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]
        public virtual void GetObjectData( SerializationInfo info, StreamingContext context ) {
            foreach ( var kvp in this._dynamicContext ) { info.AddValue( kvp.Key, kvp.Value ); }
        }

        public override Boolean TryGetMember( GetMemberBinder binder, out Object result ) => this._dynamicContext.TryGetValue( binder.Name, out result );

        public override Boolean TrySetMember( SetMemberBinder binder, Object value ) {
            this._dynamicContext.Add( binder.Name, value );

            return true;
        }
    }
}