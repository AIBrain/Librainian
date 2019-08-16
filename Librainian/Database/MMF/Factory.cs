// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Factory.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Factory.cs" was last formatted by Protiguous on 2019/08/08 at 6:57 AM.

namespace Librainian.Database.MMF {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using JetBrains.Annotations;

    public class Factory<T> {

        private static HashSet<Type> CompiledUnsafeSerializer { get; } = new HashSet<Type>();

        private static Dictionary<Type, ISerializeDeserialize<T>> DictionaryCache { get; } = new Dictionary<Type, ISerializeDeserialize<T>>();

        private static Int32 BenchMarkSerializer( ISerializeDeserialize<T> serDeser ) {
            Object[] args = null;

            if ( typeof( T ) == typeof( String ) ) {
                args = new Object[] {
                    new[] {
                        'T', 'e', 's', 't', 'T', 'e', 's', 't', 'T', 'e', 's', 't'
                    }
                };
            }

            try {
                var classInstance = ( T ) Activator.CreateInstance( typeof( T ), args );
                var sw = Stopwatch.StartNew();
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

        [NotNull]
        private static SortedDictionary<Int32, ISerializeDeserialize<T>> BenchmarkSerializers( [NotNull] IEnumerable<Type> listOfSerializers ) {
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

        [NotNull]
        private static IEnumerable<Type> GetListOfGenericSerializers() {
            var interfaceGenricType = typeof( ISerializeDeserialize<T> );

            var serializers =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from genericType in assembly.GetTypes()
                from interfaceType in genericType.GetInterfaces().Where( iType => iType.Name == interfaceGenricType.Name && genericType.IsGenericTypeDefinition )
                select genericType;

            return serializers; //.ToList();
        }

        [NotNull]
        private static IEnumerable<Type> GetListOfImplementedSerializers() {
            var interfaceGenricType = typeof( ISerializeDeserialize<T> );

            var serializers =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from implementedType in assembly.GetTypes()
                from interfaceType in implementedType.GetInterfaces().Where( iType => iType == interfaceGenricType )
                select implementedType;

            return serializers; //.ToList();
        }

        [NotNull]
        private static ISerializeDeserialize<T> InstantiateSerializer( [NotNull] Type type ) {
            var instType = type.IsGenericTypeDefinition ? type.MakeGenericType( typeof( T ) ) : type;

            return ( ISerializeDeserialize<T> ) Activator.CreateInstance( instType );
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

        [NotNull]
        public ISerializeDeserialize<T> GetSerializer() {
            var objectType = typeof( T );

            if ( !DictionaryCache.TryGetValue( objectType, out var result ) ) {
                DictionaryCache[ objectType ] = result = PickOptimalSerializer();
            }

            Debug.WriteLine( $"{typeof( T )} uses {result.GetType()}" );

            return result;
        }

        [CanBeNull]
        public ISerializeDeserialize<T> GetSerializer( String name ) =>
            ( from pair in DictionaryCache where pair.Value.GetType().AssemblyQualifiedName == name select pair.Value ).FirstOrDefault();

        [NotNull]
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