
namespace Librainian.Magic.Abodit {
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using ImpromptuInterface;
    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.IdGenerators;
    using MongoDB.Bson.Serialization.Serializers;

    public class MongoDynamicBsonSerializer : BsonBaseSerializer {
        private static readonly MongoDynamicBsonSerializer instance = new MongoDynamicBsonSerializer();

        public static MongoDynamicBsonSerializer Instance {
            get {
                return instance;
            }
        }

        public override object Deserialize( BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options ) {
            var bsonType = bsonReader.CurrentBsonType;
            switch ( bsonType ) {
                case BsonType.Null:
                    bsonReader.ReadNull();
                    return null;
                case BsonType.Document: {
                    var os = new ObjectSerializer();
                    var md = new MongoDynamic();
                    bsonReader.ReadStartDocument();

                    Dictionary<string, Type> typeMap;

                    // scan document first to find interfaces
                    {
                        var bookMark = bsonReader.GetBookmark();
                        if ( bsonReader.FindElement( MongoDynamic.InterfacesField ) ) {
                            md[ MongoDynamic.InterfacesField ] = BsonValue.ReadFrom( bsonReader ).AsBsonArray.Select( x => x.AsString );
                            typeMap = md.GetTypeMap();
                        }
                        else {
                            throw new FormatException( "No interfaces defined for this dynamic object - can't deserialize it" );
                        }
                        bsonReader.ReturnToBookmark( bookMark );
                    }

                    while ( bsonReader.ReadBsonType() != BsonType.EndOfDocument ) {
                        var name = bsonReader.ReadName();

                        switch ( name ) {
                            case "_id":
                                md[ name ] = BsonValue.ReadFrom( bsonReader ).AsObjectId;
                                break;
                            case MongoDynamic.InterfacesField:
                                BsonValue.ReadFrom( bsonReader );
                                break;
                            default: {
                                if ( typeMap == null )
                                    throw new FormatException( "No interfaces define for this dynamic object - can't deserialize" );
                                // lookup the type for this element according to the interfaces
                                Type elementType;
                                if ( typeMap.TryGetValue( name, out elementType ) ) {
                                    var value = BsonSerializer.Deserialize( bsonReader, elementType );
                                    md[ name ] = value;
                                }
                                else {
                                    // This is a value that is no longer in the interface, maybe a column you removed
                                    // not really much we can do with it ... but we need to read it and move on
                                    var value = BsonSerializer.Deserialize( bsonReader, typeof( object ) );
                                    md[ name ] = value;

                                    // As with all databases, removing elements from the schema is always going to cause problems ... 
                                }
                            }
                                break;
                        }
                    }
                    bsonReader.ReadEndDocument();
                    return md;
                }
                default: {
                    var message = string.Format( "Can't deserialize a {0} from BsonType {1}.", nominalType.FullName, bsonType );
                    throw new FormatException( message );
                }
            }
        }

        public bool GetDocumentId( object document, out object id, out Type idNominalType, out IIdGenerator idGenerator ) {
            var x = document as MongoDynamic;
            id = x.ID;
            idNominalType = typeof( ObjectId );
            idGenerator = new ObjectIdGenerator();
            return true;
        }

        public override void SetDocumentId( object document, object id ) {
            MongoDynamic x = ( MongoDynamic )document;
            x.ID = ( ObjectId )id;
        }

        public override void Serialize( BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options ) {
            if ( value == null ) {
                bsonWriter.WriteNull();
                return;
            }
            var metaObject = ( ( IDynamicMetaObjectProvider )value ).GetMetaObject( Expression.Constant( value ) );
            var memberNames = metaObject.GetDynamicMemberNames().ToList();
            if ( memberNames.Count == 0 ) {
                bsonWriter.WriteNull();
                return;
            }

            bsonWriter.WriteStartDocument();
            foreach ( var memberName in memberNames ) {
                // ToDo: handle all those _id Id id variants?
                bsonWriter.WriteName( memberName );

                object memberValue;
                if ( memberName == "_id" )
                    memberValue = ( ( MongoDynamic )value ).ID;
                else if ( memberName == "int" )
                    memberValue = ( ( MongoDynamic )value ).Int;
                else
                    memberValue = Impromptu.InvokeGet( value, memberName );

                if ( memberValue == null )
                    bsonWriter.WriteNull();
                else {
                    var memberType = memberValue.GetType();
                    var serializer = BsonSerializer.LookupSerializer( memberType );
                    serializer.Serialize( bsonWriter, memberType, memberValue, null );
                }
            }
            bsonWriter.WriteEndDocument();
        }
    }
}