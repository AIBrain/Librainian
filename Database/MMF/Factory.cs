// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Factory.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Factory.cs" was last formatted by Protiguous on 2018/06/04 at 3:50 PM.

namespace Librainian.Database.MMF {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Measurement.Time;

	public class Factory<T> {

		private static HashSet<Type> CompiledUnsafeSerializer { get; } = new HashSet<Type>();

		private static Dictionary<Type, ISerializeDeserialize<T>> DictionaryCache { get; } = new Dictionary<Type, ISerializeDeserialize<T>>();

		private static Int32 BenchMarkSerializer( ISerializeDeserialize<T> serDeser ) {
			Object[] args = null;

			if ( typeof( T ) == typeof( String ) ) { args = new Object[] { new[] { 'T', 'e', 's', 't', 'T', 'e', 's', 't', 'T', 'e', 's', 't' } }; }

			try {
				var classInstance = ( T ) Activator.CreateInstance( typeof( T ), args );
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

				if ( !serializer.CanSerializeType() ) { continue; }

				var count = BenchMarkSerializer( serializer );

				if ( count > 0 ) { benchmarkTimes.Add( count, serializer ); }
			}

			foreach ( var valuePair in benchmarkTimes ) { Debug.WriteLine( $"{valuePair.Key} : {valuePair.Value.GetType()}" ); }

			return benchmarkTimes;
		}

		private static void CompileAndRegisterUnsafeSerializer() {
			try {
				if ( CompiledUnsafeSerializer.Contains( typeof( T ) ) ) { return; }

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

			return ( ISerializeDeserialize<T> ) Activator.CreateInstance( instType );
		}

		private static ISerializeDeserialize<T> PickOptimalSerializer() {
			CompileAndRegisterUnsafeSerializer();

			var listOfSerializers = GetListOfGenericSerializers().ToList();
			listOfSerializers.AddRange( GetListOfImplementedSerializers() );

			var benchmarkTimes = BenchmarkSerializers( listOfSerializers );

			if ( benchmarkTimes.Count == 0 ) { throw new SerializerException( "No serializer available for the type" ); }

			return benchmarkTimes.Last().Value;
		}

		public ISerializeDeserialize<T> GetSerializer() {
			var objectType = typeof( T );

			if ( !DictionaryCache.TryGetValue( objectType, out var result ) ) { DictionaryCache[ objectType ] = result = PickOptimalSerializer(); }

			Debug.WriteLine( $"{typeof( T )} uses {result.GetType()}" );

			return result;
		}

		public ISerializeDeserialize<T> GetSerializer( String name ) => ( from pair in DictionaryCache where pair.Value.GetType().AssemblyQualifiedName == name select pair.Value ).FirstOrDefault();

		public List<ISerializeDeserialize<T>> GetValidSerializers() {
			CompileAndRegisterUnsafeSerializer();

			var listOfSerializers = GetListOfGenericSerializers().ToList();
			listOfSerializers.AddRange( GetListOfImplementedSerializers() );

			var benchmarkTimes = BenchmarkSerializers( listOfSerializers );

			if ( benchmarkTimes.Count == 0 ) { throw new SerializerException( "No serializer available for the type" ); }

			return benchmarkTimes.Values.ToList();
		}

	}

}