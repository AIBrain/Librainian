namespace Librainian.Magic.Abodit {
    using System.Dynamic;
    using MongoDB.Bson;

    public class Proxy : DynamicObject {
        public BsonDocument Document {
            get;
            set;
        }

        public Proxy( BsonDocument document ) {
            this.Document = document;
        }
        public override bool TryGetMember( GetMemberBinder binder, out object result ) {
            BsonValue res = null;
            this.Document.TryGetValue( binder.Name, out res );
            result = res.RawValue;  //Obsolete warning here.
            //result = BsonTypeMapper.MapToDotNetValue();
            return true;            // We always support a member even if we don't have it in the dictionary
        }

        /// <summary>
        /// Set a property (e.g. person1.Name = "Smith")
        /// </summary>     
        public override bool TrySetMember( SetMemberBinder binder, object value ) {
            this.Document.Add( binder.Name, BsonValue.Create( value ) );
            return true;
        }
    }
}