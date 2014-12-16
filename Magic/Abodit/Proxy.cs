namespace Librainian.Magic.Abodit {
    using System.Dynamic;
    using MongoDB.Bson;

    public class Proxy : DynamicObject {

        public Proxy( BsonDocument document ) {
            this.Document = document;
        }

        public BsonDocument Document {
            get;
            set;
        }

        public override bool TryGetMember( GetMemberBinder binder, out object result ) {
            BsonValue res;
            this.Document.TryGetValue( binder.Name, out res );
#pragma warning disable 618
            result = res.RawValue;  //Obsolete warning here.
#pragma warning restore 618

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