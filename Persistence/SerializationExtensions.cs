namespace Librainian.Persistence {
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Text;
    using System.Threading;
    using Annotations;

    public static class SerializationExtensions {
        internal static readonly ThreadLocal<StreamingContext> StreamingContexts = new ThreadLocal<StreamingContext>( () => new StreamingContext( StreamingContextStates.All ) );

        internal static readonly ThreadLocal<NetDataContractSerializer> Serializers = new ThreadLocal<NetDataContractSerializer>( () => new NetDataContractSerializer( context: StreamingContexts.Value, maxItemsInObjectGraph: Int32.MaxValue, ignoreExtensionDataObject: false, assemblyFormat: FormatterAssemblyStyle.Simple, surrogateSelector: null ) );

        [CanBeNull]
        public static String Serialize<TType>( this TType obj ) where TType : class {
            try {
                using ( var stream = new MemoryStream() ) {
                    var serializer = Serializers.Value;
                    serializer.WriteObject( stream, obj );
                    return stream.ReadToEnd();
                }
            }
            catch ( SerializationException exception ) {
                exception.Error();
            }
            return null;
        }

        [CanBeNull]
        public static TType Deserialize<TType>( this String storedAsString ) where TType : class {
            try {
                var byteArray = Encoding.Unicode.GetBytes( storedAsString );    //we can .Base64Encode() if we need.

                using ( var ms = new MemoryStream( byteArray ) ) {
                    ms.Position = 0;
                    var serializer = Serializers.Value;
                    var deSerialized = serializer.ReadObject( ms ) as TType;
                    return deSerialized;
                }
            }
            catch ( SerializationException exception ) {
                exception.Error();
            }
            return null;
        }
    }
}