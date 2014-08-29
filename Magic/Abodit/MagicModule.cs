namespace Librainian.Magic.Abodit {
    using MongoDB.Bson.Serialization;
    using Ninject.Modules;

    public class MagicModule : NinjectModule {

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load() {
            BsonSerializer.RegisterSerializationProvider( new MongoDynamicSerializationProvider() );

        }
    }
}