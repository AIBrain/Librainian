#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Class1.cs" was last cleaned by Rick on 2014/08/29 at 9:03 AM

#endregion License & Information

namespace Librainian.Magic.Abodit {
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using ImpromptuInterface;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    ///     A very simple document object
    /// </summary>
    public class SimpleDocument : DynamicObject {

        /// <summary>
        ///     Interfaces that have been added to this object
        /// </summary>
        [BsonElement( "int" )]
        protected readonly HashSet<String> Interfaces = new HashSet<String>();

        [BsonElement( "prop" )]
        protected readonly BsonDocument Properties = new BsonDocument();

        public ObjectId Id {
            get;
            set;
        }

        /// <summary>
        ///     Add support for an interface to this document if it doesn't already have it
        /// </summary>
        public T AddLike<T>() where T : class {
            this.Interfaces.Add( typeof( T ).Name );
            foreach ( var @interface in typeof( T ).GetInterfaces() ) {
                this.Interfaces.Add( @interface.Name );
            }
            return new Proxy( this.Properties ).ActLike<T>();
        }

        /// <summary>
        ///     Cast this object to an interface only if it has previously been created as one of that kind
        /// </summary>
        public T AsLike<T>() where T : class {
            if ( !this.Interfaces.Contains( typeof( T ).Name ) ) {
                return null;
            }
            return new Proxy( this.Properties ).ActLike<T>();
        }
    }
}