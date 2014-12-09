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
// "Librainian/Class2.cs" was last cleaned by Rick on 2014/08/29 at 9:13 AM
#endregion

namespace Librainian.Magic.Abodit {
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using ImpromptuInterface;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    ///     MongoDynamic is like an ExpandoObject that also understands document Ids and uses Improptu interface
    ///     to act like any other collection of interfaces ...
    ///     It can be serialized and deserialized from BSon and thus stored in a MongoDB database.
    /// </summary>
    /// <remarks>
    ///     This simple class gives you the ability to define database objects using only .NET interfaces - no classes!
    ///     Those objects can be dynamically extended to support any interface you want to add to them - polymorphism!
    ///     When loaded back from the database the object will support all of the interfaces that were ever applied to it.
    ///     Adding a new field is easy.  Removing one works too.
    ///     All fields must be nullable since they may not be present on earlier instances of an object type.
    /// </remarks>
    /// <seealso cref="http://blog.abodit.com/2011/09/dynamic-persistence-with-mongodb-look-no-classes-polymorphism-in-c/"/>
    public class MongoDynamic : DynamicObject, IId {

        /// <summary>
        /// Dumb name for a property - which is why I chose it - very unlikely it will ever conflict with a real property name
        /// </summary>
        public const String InterfacesField = "int";

        /// <summary>
        /// 
        /// </summary>
        private static List<Type> cacheOfTypes;

        /// <summary>
        /// A cache of the interface types corresponding to a given 'key' of interface names
        /// </summary>
        private static readonly Dictionary<String, Type[]> CacheOfInterfaces = new Dictionary<String, Type[]>();

        /// <summary>
        /// BsonIgnore because Bson serialization will happen on the dynamic interface this class exposes not on this dictionary
        /// </summary>
        [BsonIgnore]
        private readonly Dictionary<String, object> _children = new Dictionary<String, object>();

        /// <summary>
        ///     Interfaces that have been added to this object
        /// </summary>
        /// <remarks>
        ///     We always begin by supporting the _id interface
        ///     Order is important, we need to see this field before we can deserialize any others
        /// </remarks>
        [BsonElement( InterfacesField, Order = 2 )]
        internal HashSet<String> Int = new HashSet<String> { typeof( IId ).FullName };

        /// <summary>
        ///     A text version of all interfaces - mostly for debugging purposes, stored in alphabetical order
        /// </summary>
        [BsonIgnore]
        public String InterfacesAsText => String.Join( ",", this.Int.OrderBy( s => s ) );

        /// <summary>
        ///     An indexer for use by serialization code
        /// </summary>
        internal object this[ String key ] {
            get {
                switch ( key ) {
                    case "_id":
                        return this.ID;
                    case InterfacesField:
                        return this.Int;
                    default:
                        return this._children[ key ];
                }
            }

            set {
                switch ( key ) {
                    case "_id":
                        this.ID = ( value as BsonObjectId )?.Value ?? ( ObjectId )value;
                        break;
                    case InterfacesField:
                        this.Int = new HashSet<String>( ( IEnumerable<String> )value );
                        break;
                    default:
                        this._children[ key ] = value;
                        break;
                }

            }
        }

        [BsonId( Order = 1 )]
        public ObjectId ID {
            get;
            set;
        }

        /// <summary>
        ///     Add support for an interface to this document if it doesn't already have it
        /// </summary>
        public T AddLike<T>() where T : class {
            this.Int.Add( typeof( T ).FullName );
            // And also act like any interfaces that interface implements (which will include ones they represent too)
            foreach ( var @interface in typeof( T ).GetInterfaces() ) {
                this.Int.Add( @interface.FullName );
            }
            return this.ActLike<T>( this.GetAllInterfaces() );
        }

        /// <summary>
        ///     Add support for multiple interfaces
        /// </summary>
        public T AddLike<T>( IEnumerable<Type> otherInterfaces ) where T : class {
            var allInterfaces = otherInterfaces.Concat( new[] { typeof( T ) } );
            var interfaces = allInterfaces as IList<Type> ?? allInterfaces.ToList();
            var allInterfacesAndDescendants = interfaces.Concat( interfaces.SelectMany( x => x.GetInterfaces() ) );
            foreach ( var @interface in allInterfacesAndDescendants ) {
                this.Int.Add( @interface.FullName );
            }
            return this.ActLike<T>( this.GetAllInterfaces() );
        }

        /// <summary>
        ///     Cast this object to an interface only if it has previously been created as one of that kind
        /// </summary>
        public T AsLike<T>() where T : class {
            if ( !this.Int.Contains( typeof( T ).FullName ) ) {
                return null;
            }
            return this.ActLike<T>( this.GetAllInterfaces() );
        }

        // A rather large cache of all interface types loaded into the App Domain

        public Type[] GetAllInterfaces() {
            // We always behave like an object with an Id plus any other interfaces we have
            var key = String.Join( ",", this.Int.OrderBy( i => i ) );
            if ( CacheOfInterfaces.ContainsKey( key ) ) {
                return CacheOfInterfaces[ key ];
            }

            if ( cacheOfTypes == null ) {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                cacheOfTypes = assemblies.SelectMany( ass => ass.GetTypes() ).Where( t => t.IsInterface ).ToList();
            }

            var interfaces = cacheOfTypes.Where( t => this.Int.Any( i => i == t.FullName ) );

            // Could trim the interfaces to remove any that are inherited from others ...
            CacheOfInterfaces.Add( key, interfaces.ToArray() );
            return CacheOfInterfaces[ key ];
        }

        /// <summary>
        ///     Get a mapping from a field name to a type according to the interfaces on this object
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, Type> GetTypeMap() {
            var typeMap = new Dictionary<String, Type>();
            var interfaces = this.GetAllInterfaces();
            foreach ( var mi in interfaces.SelectMany( intf => intf.GetProperties() ) ) {
                typeMap[ mi.Name ] = mi.PropertyType;
            }
            return typeMap;
        }

        /// <summary>
        ///     Becomes a Proxy object that acts like it implements all of the interfaces listed as being supported by this Entity
        /// </summary>
        /// <remarks>
        ///     Because the returned object supports ALL of the interfaces that have ever been added to this object
        ///     you can cast it to any of them.  This enables a type of polymorphism.
        /// </remarks>
        public object ActLikeAllInterfacesPresent() => Impromptu.DynamicActLike( this, this.GetAllInterfaces() );

        /// <summary>
        ///     Fetch a property by name
        /// </summary>
        public override bool TryGetMember( GetMemberBinder binder, out object result ) {
            switch ( binder.Name ) {
                case "_id":
                    result = this.ID;
                    return true;
                case InterfacesField:
                    result = this.Int;
                    return true;
                default:
                    this._children.TryGetValue( binder.Name, out result );
                    result = null; // we hope that it's nullable!  If not you have an issue 
                    return true; // when you do a database migration or query a nullable field it won't be in 'children'
            }
        }

        /// <summary>
        ///     Set a property (e.g. person1.Name = "Smith")
        /// </summary>
        public override bool TrySetMember( SetMemberBinder binder, object value ) {
            switch ( binder.Name ) {
                case "_id":
                    this.ID = ( ObjectId )value;
                    return true;
                case InterfacesField:
                    throw new AccessViolationException( "You cannot set the interfaces directly, use AddLike() instead" );
            }
            if ( !this.GetTypeMap().ContainsKey( binder.Name ) ) {
                throw new ArgumentException( String.Format( "Property '{0}' not found.  You need to call AddLike to specify the interfaces you want to support.", binder.Name ) );
            }
            this._children[ binder.Name ] = value;
            return true;
        }

        public override IEnumerable<String> GetDynamicMemberNames() => new[] { "_id", InterfacesField }.Concat( this._children.Keys );
    }
}
