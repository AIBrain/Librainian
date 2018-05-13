// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Factory.cs" was last cleaned by Protiguous on 2018/05/12 at 1:22 AM

namespace Librainian.Database.MMF {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Measurement.Time;

    public class Factory<T> {

        // ReSharper disable once StaticMemberInGenericType
        private static readonly HashSet<Type> CompiledUnsafeSerializer = new HashSet<Type>();

        private static readonly Dictionary<Type, ISerializeDeserialize<T>> DictionaryCache = new Dictionary<Type, ISerializeDeserialize<T>>();

        private static Int32 BenchMarkSerializer( ISerializeDeserialize<T> serDeser ) {
            Object[] args = null;

            if ( typeof( T ) == typeof( String ) ) {
                args = new Object[] { new[] { 'T', 'e', 's', 't', 'T', 'e', 's', 't', 'T', 'e', 's', 't' } };
            }

            try {
                var classInstance = ( T )Activator.CreateInstance( typeof( T ), args );
                var sw = StopWatch.StartNew();
                var count = 0;

                while ( sw.ElapsedMilliseconds < 500 ) {
                    var bytes = serDeser.ObjectToBytes( classInstance );
                    serDeser.BytesToObject( bytes );
                    count++;
                }

                sw.Stop();

                return count;
            }
            catch ( MissingMethodException ) {

                // Missing default constructor
                return 0;
            }
        }

        private static SortedDictionary<Int32, ISerializeDeserialize<T>> BenchmarkSerializers( IEnumerable<Type> listOfSerializers ) {
            var benchmarkTimes = new SortedDictionary<Int32, ISerializeDeserialize<T>>();

            foreach ( var type in listOfSerializers ) {
                var serializer = InstantiateSerializer( type );

                if ( !serializer.CanSerializeType() ) {
                    continue;
                }

                var count = BenchMarkSerializer( serializer );

                if ( count > 0 ) {
                    benchmarkTimes.Add( count, serializer );
                }
            }

            foreach ( var valuePair in benchmarkTimes ) {
                Debug.WriteLine( $"{valuePair.Key} : {valuePair.Value.GetType()}" );
            }

            return benchmarkTimes;
        }

        private static void CompileAndRegisterUnsafeSerializer() {
            try {
                if ( CompiledUnsafeSerializer.Contains( typeof( T ) ) ) {
                    return;
                }

                var createUnsafeSerializer = new CreateUnsafeSerializer<T>();
                createUnsafeSerializer.GetSerializer();
                CompiledUnsafeSerializer.Add( typeof( T ) );
            }
            catch ( SerializerException ) {

                // ignore errors
            }
        }

        private static IEnumerable<Type> GetListOfGenericSerializers() {
            var interfaceGenricType = typeof( ISerializeDeserialize<T> );

            var serializers =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from genericType in assembly.GetTypes()
                from interfaceType in genericType.GetInterfaces().Where( iType => iType.Name == interfaceGenricType.Name && genericType.IsGenericTypeDefinition )
                select genericType;

            return serializers; //.ToList();
        }

        private static IEnumerable<Type> GetListOfImplementedSerializers() {
            var interfaceGenricType = typeof( ISerializeDeserialize<T> );

            var serializers =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from implementedType in assembly.GetTypes()
                from interfaceType in implementedType.GetInterfaces().Where( iType => iType == interfaceGenricType )
                select implementedType;

            return serializers; //.ToList();
        }

        private static ISerializeDeserialize<T> InstantiateSerializer( Type type ) {
            var instType = type.IsGenericTypeDefinition ? type.MakeGenericType( typeof( T ) ) : type;

            return ( ISerializeDeserialize<T> )Activator.CreateInstance( instType );
        }

        private static ISerializeDeserialize<T> PickOptimalSerializer() {
            CompileAndRegisterUnsafeSerializer();

            var listOfSerializers = GetListOfGenericSerializers().ToList();
            listOfSerializers.AddRange( GetListOfImplementedSerializers() );

            var benchmarkTimes = BenchmarkSerializers( listOfSerializers );

            if ( benchmarkTimes.Count == 0 ) {
                throw new SerializerException( "No serializer available for the type" );
            }

            return benchmarkTimes.Last().Value;
        }

        public ISerializeDeserialize<T> GetSerializer() {
            var objectType = typeof( T );

            if ( !DictionaryCache.TryGetValue( objectType, out var result ) ) {
                DictionaryCache[objectType] = result = PickOptimalSerializer();
            }

            Debug.WriteLine( $"{typeof( T )} uses {result.GetType()}" );

            return result;
        }

        public ISerializeDeserialize<T> GetSerializer( String name ) => ( from pair in DictionaryCache where pair.Value.GetType().AssemblyQualifiedName == name select pair.Value ).FirstOrDefault();

        public List<ISerializeDeserialize<T>> GetValidSerializers() {
            CompileAndRegisterUnsafeSerializer();

            var listOfSerializers = GetListOfGenericSerializers().ToList();
            listOfSerializers.AddRange( GetListOfImplementedSerializers() );

            var benchmarkTimes = BenchmarkSerializers( listOfSerializers );

            if ( benchmarkTimes.Count == 0 ) {
                throw new SerializerException( "No serializer available for the type" );
            }

            return benchmarkTimes.Values.ToList();
        }
    }
}