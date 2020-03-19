// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Factory.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "LibrainianCore", File: "Factory.cs" was last formatted by Protiguous on 2020/03/16 at 3:03 PM.

namespace Librainian.Databases.MMF {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using JetBrains.Annotations;

    public class Factory<T> {

        [NotNull]
        private static HashSet<Type> CompiledUnsafeSerializer { get; } = new HashSet<Type>();

        [NotNull]
        private static Dictionary<Type, ISerializeDeserialize<T>> DictionaryCache { get; } = new Dictionary<Type, ISerializeDeserialize<T>>();

        private static Int32 BenchMarkSerializer( [NotNull] ISerializeDeserialize<T> serDeser ) {
            if ( serDeser == null ) {
                throw new ArgumentNullException( paramName: nameof( serDeser ) );
            }

            Object[] args = null;

            if ( typeof( T ) == typeof( String ) ) {
                args = new Object[] {
                    new[] {
                        'T', 'e', 's', 't', 'T', 'e', 's', 't', 'T', 'e', 's', 't'
                    }
                };
            }

            try {
                var classInstance = ( T )Activator.CreateInstance( type: typeof( T ), args: args );
                var sw = Stopwatch.StartNew();
                var count = 0;

                while ( sw.ElapsedMilliseconds < 500 ) {
                    var bytes = serDeser.ObjectToBytes( data: classInstance );
                    serDeser.BytesToObject( bytes: bytes );
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
                var serializer = InstantiateSerializer( type: type );

                if ( !serializer.CanSerializeType() ) {
                    continue;
                }

                var count = BenchMarkSerializer( serDeser: serializer );

                if ( count > 0 ) {
                    benchmarkTimes.Add( key: count, value: serializer );
                }
            }

            foreach ( var valuePair in benchmarkTimes ) {
                Debug.WriteLine( message: $"{valuePair.Key} : {valuePair.Value.GetType()}" );
            }

            return benchmarkTimes;
        }

        [NotNull]
        private static IEnumerable<Type> GetListOfGenericSerializers() {
            var interfaceGenricType = typeof( ISerializeDeserialize<T> );

            var serializers =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from genericType in assembly.GetTypes()
                where genericType != default
                where genericType != null
                from interfaceType in genericType
                                      ?.GetInterfaces().Where( predicate: iType => iType.Name == interfaceGenricType.Name && genericType?.IsGenericTypeDefinition == true )
                select genericType;

            return serializers; //.ToList();
        }

        [NotNull]
        private static IEnumerable<Type> GetListOfImplementedSerializers() {
            var interfaceGenricType = typeof( ISerializeDeserialize<T> );

            var serializers =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from implementedType in assembly.GetTypes()
                from interfaceType in implementedType.GetInterfaces().Where( predicate: iType => iType == interfaceGenricType )
                select implementedType;

            return serializers; //.ToList();
        }

        [NotNull]
        private static ISerializeDeserialize<T> InstantiateSerializer( [NotNull] Type type ) {
            var instType = type.IsGenericTypeDefinition ? type.MakeGenericType( typeof( T ) ) : type;

            return ( ISerializeDeserialize<T> )Activator.CreateInstance( type: instType );
        }

        [CanBeNull]
        public ISerializeDeserialize<T> GetSerializer( [CanBeNull] String name ) =>
            ( from pair in DictionaryCache where pair.Value.GetType().AssemblyQualifiedName == name select pair.Value ).FirstOrDefault();
    }
}