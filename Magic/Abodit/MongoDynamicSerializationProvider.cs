
namespace Librainian.Magic.Abodit {
    using System;
    using MongoDB.Bson.Serialization;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="http://blog.abodit.com/2011/09/dynamic-persistence-with-mongodb-look-no-classes-polymorphism-in-c/"/>
    public class MongoDynamicSerializationProvider : IBsonSerializationProvider {

        public IBsonSerializer GetSerializer( Type type ) {
            if ( typeof( MongoDynamic ).IsAssignableFrom( type ) )
                return MongoDynamicBsonSerializer.Instance;
            return null;
        }
    }
}
