namespace Librainian.Magic.Abodit {
    using MongoDB.Bson.Serialization;

    public class MagicModule  {

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public void Load() {
            BsonSerializer.RegisterSerializationProvider( new MongoDynamicSerializationProvider() );

        }
    }
}