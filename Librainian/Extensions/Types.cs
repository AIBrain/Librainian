// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Types.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Types.cs" was last formatted by Protiguous on 2018/07/10 at 9:03 PM.

namespace Librainian.Extensions {

	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Runtime.Serialization;
	using JetBrains.Annotations;

	//using System.Runtime.InteropServices;

	public static class Types {

		public static Lazy<Assembly[]> CurrentDomainGetAssemblies { get; } = new Lazy<Assembly[]>( () => AppDomain.CurrentDomain.GetAssemblies() );

		public static ConcurrentDictionary<Type, IList<Type>> EnumerableOfTypeCache { get; } = new ConcurrentDictionary<Type, IList<Type>>();

		public static Boolean CanAssignValue( this PropertyInfo p, [CanBeNull] Object value ) => value is null ? p.IsNullable() : p.PropertyType.IsInstanceOfType( value );

		[NotNull]
		public static IList<T> Clone<T>( [NotNull] this IEnumerable<T> listToClone ) where T : ICloneable => listToClone.Select( item => ( T ) item.Clone() ).ToList();

		public static void CopyField<TSource>( this TSource source, TSource destination, [NotNull] FieldInfo field, Boolean mergeDictionaries = true ) {
			if ( field is null ) { throw new ArgumentNullException( nameof( field ) ); }

			try {
				var sourceValue = field.GetValue( source );

				if ( mergeDictionaries && sourceValue is IDictionary dictionary && dictionary.MergeDictionaries( field, destination ) ) { return; }

				if ( !field.IsLiteral ) { field.SetValue( destination, sourceValue ); }
			}
			catch ( TargetException exception ) { exception.Log(); }
			catch ( NotSupportedException exception ) { exception.Log(); }
			catch ( FieldAccessException exception ) { exception.Log(); }
			catch ( ArgumentException exception ) { exception.Log(); }
		}

		/// <summary>
		///     Copy the value of each field of the <paramref name="source" /> to the matching field in the
		///     <paramref name="destination" /> .
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source">     </param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static Boolean CopyFields<TSource>( this TSource source, TSource destination ) {
			try {
				var sourceFields = source.GetType().GetAllFields();
				var destFields = destination.GetType().GetAllFields();

				foreach ( var field in sourceFields.Where( destFields.Contains ) ) { CopyField( source: source, destination: destination, field: field ); }

				return true;
			}
			catch ( Exception ) { return false; }
		}

		/// <summary>
		///     Copy the value of each get property of the <paramref name="source" /> to each set property of the
		///     <paramref name="destination" /> .
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source">     </param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static Boolean CopyProperties<TSource>( this TSource source, TSource destination ) {
			try {
				var sourceProps = source.GetType().GetAllProperties().Where( prop => prop.CanRead );
				var destProps = destination.GetType().GetAllProperties().Where( prop => prop.CanWrite );

				foreach ( var prop in sourceProps.Where( destProps.Contains ) ) { CopyProperty( source: source, destination: destination, prop: prop ); }

				return true;
			}
			catch ( Exception ) { return false; }
		}

		public static void CopyProperty<TSource>( this TSource source, TSource destination, [NotNull] PropertyInfo prop ) {
			if ( prop is null ) { throw new ArgumentNullException( nameof( prop ) ); }

			try {
				var sourceValue = prop.GetValue( source, null );
				prop.SetValue( destination, sourceValue, null );
			}
			catch ( TargetParameterCountException exception ) { exception.Log(); }
			catch ( TargetException exception ) { exception.Log(); }
			catch ( NotSupportedException exception ) { exception.Log(); }
			catch ( FieldAccessException exception ) { exception.Log(); }
			catch ( ArgumentException exception ) { exception.Log(); }
		}

		/// <summary>
		///     <para>
		///         Copy each field in the <paramref name="source" /> to the matching field in the
		///         <paramref name="destination" />.
		///     </para>
		///     <para>then</para>
		///     <para>
		///         Copy each property in the <paramref name="source" /> to the matching property in the
		///         <paramref name="destination" />.
		///     </para>
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source">     </param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static Boolean DeepClone<TSource>( this TSource source, TSource destination ) {
			if ( ReferenceEquals( source, destination ) ) { return true; }

			if ( Equals( source, default ) ) { return false; }

			if ( Equals( destination, default ) ) { return false; }

			//copy all settable fields
			// then
			//copy all settable properties (going on the assumption that properties should be modifiying their private fields).
			return CopyFields( source: source, destination: destination ) && CopyProperties( source: source, destination: destination );
		}

		/// <summary>
		///     Enumerate all fields of the <paramref name="type" />
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<FieldInfo> GetAllFields( [CanBeNull] this Type type ) {
			if ( null == type ) { return Enumerable.Empty<FieldInfo>(); }

			const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

			return type.GetFields( flags ).Union( GetAllFields( type.BaseType ) );
		}

		/// <summary>
		///     Enumerate all properties of the <paramref name="type" />
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<PropertyInfo> GetAllProperties( [CanBeNull] this Type type ) {
			if ( null == type ) { return Enumerable.Empty<PropertyInfo>(); }

			const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

			return type.GetProperties( flags ).Union( GetAllProperties( type.BaseType ) );
		}

		[ItemCanBeNull]
		public static IEnumerable<T> GetEnumerableOfType<T>( params Object[] constructorArgs ) where T : class, IComparable<T> {
			if ( !EnumerableOfTypeCache.TryGetValue( typeof( T ), out var list ) ) {
				list = Assembly.GetAssembly( typeof( T ) ).GetTypes().ToList();
				EnumerableOfTypeCache[ typeof( T ) ] = list;
			}

			if ( null == list ) { yield break; }

			foreach ( var myType in list.Where( myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf( typeof( T ) ) ) ) {
				if ( constructorArgs?.Any() == true ) { yield return ( T ) Activator.CreateInstance( myType, constructorArgs ); }
				else {
					var declaredCtor = myType.GetConstructors();

					foreach ( var _ in declaredCtor.Select( constructorInfo => constructorInfo.GetParameters() )
						.SelectMany( parms => parms.Where( parameterInfo => parameterInfo.ParameterType == typeof( Guid ) ), ( parms, parameterInfo ) => parms ) ) {
						yield return Activator.CreateInstance( myType, Guid.NewGuid() ) as T;
					}
				}
			}
		}

		/// <summary>
		///     Get all <see cref="GetSealedClassesDerivedFrom" /><paramref name="baseType" />.
		/// </summary>
		/// <param name="baseType"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<Type> GetSealedClassesDerivedFrom( [CanBeNull] this Type baseType ) {
			if ( baseType is null ) { throw new ArgumentNullException( nameof( baseType ) ); }

			return baseType.Assembly.GetTypes().Where( type => type.IsAssignableFrom( baseType ) && type.IsSealed );
		}

		/// <summary>
		///     Get all <see cref="Type" /> from <see cref="AppDomain.CurrentDomain" /> that should be able to be created via
		///     <see cref="Activator.CreateInstance(Type,BindingFlags,Binder,System.Object[],CultureInfo) " />.
		/// </summary>
		/// <param name="baseType"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<Type> GetTypesDerivedFrom( [NotNull] this Type baseType ) {
			if ( baseType is null ) { throw new ArgumentNullException( nameof( baseType ) ); }

			return CurrentDomainGetAssemblies.Value.SelectMany( assembly => assembly.GetTypes(), ( assembly, type ) => type ).Where( arg => baseType.IsAssignableFrom( arg ) && arg.IsClass && !arg.IsAbstract );
		}

		public static Boolean HasDefaultConstructor( [NotNull] this Type t ) => t.IsValueType || t.GetConstructor( Type.EmptyTypes ) != null;

		/// <summary>
		///     Returns whether or not objects of this type can be copied byte-for-byte in to another part of the system memory
		///     without potential segmentation faults (i.e. the type contains no managed references such as
		///     <see
		///         cref="String" />
		///     s). This function will always return <c>false</c> for non- <see cref="ValueType" /> s.
		/// </summary>
		/// <param name="this">The extended Type.</param>
		/// <returns>True if the type can be copied (blitted), or false if not.</returns>
		public static Boolean IsBlittable( [NotNull] this Type @this ) {
			if ( @this is null ) { throw new ArgumentNullException( nameof( @this ), "IsBlittable called on a null Type." ); }

			return @this.IsValueType && @this.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).All( fieldInfo => fieldInfo.FieldType.IsValueType || fieldInfo.FieldType.IsPointer ) &&
			       ( @this.IsExplicitLayout || @this.IsLayoutSequential );
		}

		public static Boolean IsNullable( [NotNull] this PropertyInfo p ) => p.PropertyType.IsNullable();

		public static Boolean IsNullable( [NotNull] this Type t ) => !t.IsValueType || Nullable.GetUnderlyingType( t ) != null;

		/// <summary>
		///     Ascertains if the given type is a numeric type (e.g. <see cref="Int32" />).
		/// </summary>
		/// <param name="this">The extended Type.</param>
		/// <returns>True if the type represents a numeric type, false if not.</returns>
		public static Boolean IsNumeric( [NotNull] this Type @this ) {
			if ( @this is null ) { throw new ArgumentNullException( nameof( @this ), "IsNumeric called on a null Type." ); }

			return @this == typeof( Double ) || @this == typeof( Single ) || @this == typeof( Int64 ) || @this == typeof( Int16 ) || @this == typeof( Byte ) || @this == typeof( SByte ) || @this == typeof( UInt32 ) ||
			       @this == typeof( UInt64 ) || @this == typeof( UInt16 ) || @this == typeof( Decimal ) || @this == typeof( Int32 );
		}

		/// <summary>
		///     <para>Checks a type to see if it derives from a raw generic (e.g. List[[]])</para>
		/// </summary>
		/// <param name="type">   </param>
		/// <param name="generic"></param>
		/// <returns></returns>
		public static Boolean IsSubclassOfRawGeneric( this Type type, Type generic ) {
			while ( type != typeof( Object ) ) {
				var cur = type != null && type.IsGenericType ? type.GetGenericTypeDefinition() : type;

				if ( generic == cur ) { return true; }

				type = type?.BaseType;
			}

			return false;
		}

		public static Boolean MergeDictionaries<TSource>( [CanBeNull] this IDictionary sourceValue, FieldInfo field, TSource destination ) {
			if ( null == sourceValue ) { return false; }

			if ( !( field.GetValue( destination ) is IDictionary destAsDictionary ) ) { return false; }

			foreach ( var key in sourceValue.Keys ) {
				try { destAsDictionary[ key ] = sourceValue[ key ]; }
				catch ( Exception exception ) { exception.Log(); }
			}

			return true;
		}

		[NotNull]
		public static String Name<T>( [NotNull] this Expression<Func<T>> propertyExpression ) {
			if ( propertyExpression is null ) { throw new ArgumentNullException( nameof( propertyExpression ) ); }

			var memberExpression = propertyExpression.Body as MemberExpression;

			return memberExpression?.Member.Name ?? String.Empty;
		}

		[NotNull]
		public static Func<Object> NewInstanceByCreate( [NotNull] this Type type ) {
			if ( type is null ) { throw new ArgumentNullException( nameof( type ) ); }

			var localType = type; // create a local copy to prevent adverse effects of closure

			Object Func() => Activator.CreateInstance( localType );

			return Func;
		}

		[NotNull]
		public static Func<Object> NewInstanceByLambda( [NotNull] this Type type ) {
			if ( type is null ) { throw new ArgumentNullException( nameof( type ) ); }

			return Expression.Lambda<Func<Object>>( Expression.New( type ) ).Compile();
		}

		/// <summary>
		///     Get the <see cref="Type" /> associated with the subject <see cref="TypeCode" />.
		/// </summary>
		/// <param name="this">The extended TypeCode.</param>
		/// <returns>A <see cref="Type" /> that <paramref name="this" /> represents.</returns>
		[NotNull]
		public static Type ToType( this TypeCode @this ) {
			switch ( @this ) {
				case TypeCode.Boolean: return typeof( Boolean );

				case TypeCode.Byte: return typeof( Byte );

				case TypeCode.Char: return typeof( Char );

				case TypeCode.DBNull: return typeof( DBNull );

				case TypeCode.DateTime: return typeof( DateTime );

				case TypeCode.Decimal: return typeof( Decimal );

				case TypeCode.Double: return typeof( Double );

				case TypeCode.Int16: return typeof( Int16 );

				case TypeCode.Int32: return typeof( Int32 );

				case TypeCode.Int64: return typeof( Int64 );

				case TypeCode.SByte: return typeof( SByte );

				case TypeCode.Single: return typeof( Single );

				case TypeCode.String: return typeof( String );

				case TypeCode.UInt16: return typeof( UInt16 );

				case TypeCode.UInt32: return typeof( UInt32 );

				case TypeCode.UInt64: return typeof( UInt64 );

				case TypeCode.Empty: return typeof( Object );

				case TypeCode.Object: return typeof( Object );

				default: return typeof( Object );
			}
		}

		public static Boolean TryCast<T>( this Object value, out T result ) {
			var type = typeof( T );

			// If the type is nullable and the result should be null, set a null value.
			if ( type.IsNullable() && ( value is null || value == DBNull.Value ) ) {
				result = default;

				return true;
			}

			// Convert.ChangeType fails on Nullable<T> types. We want to try to cast to the underlying type anyway.
			var underlyingType = Nullable.GetUnderlyingType( type ) ?? type;

			try {

				// Just one edge case you might want to handle.
				if ( underlyingType == typeof( Guid ) ) {
					if ( value is String ) { value = new Guid( ( String ) value ); }

					if ( value is Byte[] ) { value = new Guid( ( Byte[] ) value ); }

					result = ( T ) Convert.ChangeType( value, underlyingType );

					return true;
				}

				result = ( T ) Convert.ChangeType( value, underlyingType );

				return true;
			}
			catch ( Exception ) {
				result = default;

				return false;
			}
		}

		public static class New<T> {

			public static readonly Func<T> Instance = Creator();

			[NotNull]
			private static Func<T> Creator() {
				var t = typeof( T );

				if ( t == typeof( String ) ) { return Expression.Lambda<Func<T>>( Expression.Constant( String.Empty ) ).Compile(); }

				if ( t.HasDefaultConstructor() ) { return Expression.Lambda<Func<T>>( Expression.New( t ) ).Compile(); }

				return () => ( T ) FormatterServices.GetUninitializedObject( t );
			}
		}

		/*
                public static String GetName<T>( [CanBeNull] this Expression<Func<T>> propertyExpression ) {
                    if ( null == propertyExpression ) {
                        return String.Empty;
                    }
                    var memberExpression = propertyExpression.Body as MemberExpression;
                    return memberExpression != null ? memberExpression.Member.Name : String.Empty;
                }
        */

		///// <summary>
		///// <para>Returns the name of the instance (variable/property).</para>
		///// Doesn't seem to work
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="item"></param>
		///// <returns></returns>
		//public static string Name<T>( this T item ) where T : class {
		//    if ( item is null ) {
		//        return string.Empty;
		//    }
		//    else {
		//        var props = typeof ( T ).GetProperties().ToList();
		//        return props[ 0 ].Name;
		//    }
		//}
		/*

                /// <summary>
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <param name="getMethod"></param>
                /// <returns></returns>
                /// <see cref="http://stackoverflow.com/a/557711"/>
                public static T GetProperty<T>( MethodBase getMethod ) {
                    if ( !getMethod.Name.StartsWith( "get_" ) ) {
                        throw new ArgumentException(
                            "GetProperty must be called from a property" );
                    }
                    return GetValue<T>( getMethod.Name.Substring( 4 ) );
                }
        */

		// case TypeCode.Double: return sizeof( Single ); case TypeCode.Single: return sizeof( UInt64 ); case TypeCode.UInt64: return sizeof( Int64 ); case TypeCode.Int64: return sizeof( UInt32 ); case TypeCode.UInt32:
		// return sizeof( Int32 ); case TypeCode.Int32: return sizeof( UInt16 ); case TypeCode.UInt16: return sizeof( Int16 ); case TypeCode.Int16: return sizeof( Byte ); case TypeCode.Byte: return sizeof( SByte ); case
		// TypeCode.SByte: return sizeof( Char ); case TypeCode.Char: return sizeof( Boolean ); case TypeCode.Boolean: switch ( typeCode ) { var typeCode = Type.GetTypeCode( type ); var type = typeof( T );

		//public static Int32 SizeOf<T>() where T : struct {
		//            return sizeof( Double );
		//        case TypeCode.Decimal:
		//            return sizeof( Decimal );
		//        //case TypeCode.DateTime: return sizeof( DateTime );
		//        default:
		//            var tArray = new T[ 2 ];
		//            var tArrayPinned = GCHandle.Alloc( tArray, GCHandleType.Pinned );
		//            try {
		//                unsafe {
		//                    TypedReference tRef0 = __makeref( tArray[ 0 ] );
		//                    TypedReference tRef1 = __makeref( tArray[ 1 ] );
		//                    var ptrToT0 = *( IntPtr* )&tRef0;
		//                    var ptrToT1 = *( IntPtr* )&tRef1;

		//                    return ( Int32 )( ( ( Byte* )ptrToT1 ) - ( ( Byte* )ptrToT0 ) );
		//                }
		//            }
		//            finally {
		//                tArrayPinned.Free();
		//            }
		//    }
		//}
	}
}