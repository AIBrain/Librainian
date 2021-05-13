// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Types.cs" last formatted on 2020-08-14 at 8:33 PM.

#nullable enable

namespace Librainian.Extensions {

	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Runtime.CompilerServices;
	using JetBrains.Annotations;
	using Logging;

	public static class Types {

		private static readonly IDictionary<Type, ObjectActivator> ObjectActivators = new Dictionary<Type, ObjectActivator>();

		public static Lazy<Assembly[]> CurrentDomainGetAssemblies { get; } = new( () => AppDomain.CurrentDomain.GetAssemblies() );

		public static ConcurrentDictionary<Type, IList<Type>> EnumerableOfTypeCache { get; } = new();

		private delegate Object ObjectActivator();

		public static Boolean CanAssignValue( [NotNull] this PropertyInfo p, [CanBeNull] Object? value ) {
			if ( p is null ) {
				throw new ArgumentNullException( nameof( p ) );
			}

			return value is null ? p.IsNullable() : p.PropertyType.IsInstanceOfType( value );
		}

		/// <summary>Creates a new <see cref="IList{T}" /> with a clone of each item.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		[NotNull]
		public static IList<T> Clone<T>( [NotNull] this IEnumerable<T> list ) where T : ICloneable =>
			list.Select( item => ( T )item.Clone() ).ToList();

		public static void CopyField<TSource>( [NotNull] this TSource source, [NotNull] TSource destination, [NotNull] FieldInfo field, Boolean mergeDictionaries = true ) {
			try {
				var sourceValue = field.GetValue( source );

				if ( mergeDictionaries && sourceValue is IDictionary dictionary && dictionary.MergeDictionaries( field, destination ) ) {
					return;
				}

				if ( !field.IsLiteral ) {
					field.SetValue( destination, sourceValue );
				}
			}
			catch ( TargetException exception ) {
				exception.Log();
			}
			catch ( NotSupportedException exception ) {
				exception.Log();
			}
			catch ( FieldAccessException exception ) {
				exception.Log();
			}
			catch ( ArgumentException exception ) {
				exception.Log();
			}
		}

		/// <summary>
		///     Copy the value of each field of the <paramref name="source" /> to the matching field in the
		///     <paramref name="destination" /> .
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source">     </param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static Boolean CopyFields<TSource>( [NotNull] this TSource source, [NotNull] TSource destination ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			if ( destination is null ) {
				throw new ArgumentNullException( nameof( destination ) );
			}

			try {
				var sourceFields = source.GetType().GetAllFields();
				var destFields = destination.GetType().GetAllFields();

				foreach ( var field in sourceFields.Where( destFields.Contains ) ) {
					CopyField( source, destination, field );
				}

				return true;
			}
			catch ( Exception ) {
				return false;
			}
		}

		/// <summary>
		///     Copy the value of each get property of the <paramref name="source" /> to each set property of the
		///     <paramref name="destination" /> .
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source">     </param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static Boolean CopyProperties<TSource>( [CanBeNull] this TSource source, [CanBeNull] TSource destination ) {
			try {
				var sourceProps = source.GetType().GetAllProperties().Where( prop => prop.CanRead );
				var destProps = destination.GetType().GetAllProperties().Where( prop => prop.CanWrite );

				foreach ( var prop in sourceProps.Where( destProps.Contains ) ) {
					CopyProperty( source, destination, prop );
				}

				return true;
			}
			catch ( Exception ) {
				return false;
			}
		}

		public static void CopyProperty<TSource>( [NotNull] this TSource source, [NotNull] TSource destination, [NotNull] PropertyInfo prop ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			if ( destination is null ) {
				throw new ArgumentNullException( nameof( destination ) );
			}

			if ( prop is null ) {
				throw new ArgumentNullException( nameof( prop ) );
			}

			try {
				var sourceValue = prop.GetValue( source, null );
				prop.SetValue( destination, sourceValue, null );
			}
			catch ( TargetParameterCountException exception ) {
				exception.Log();
			}
			catch ( TargetException exception ) {
				exception.Log();
			}
			catch ( NotSupportedException exception ) {
				exception.Log();
			}
			catch ( FieldAccessException exception ) {
				exception.Log();
			}
			catch ( ArgumentException exception ) {
				exception.Log();
			}
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
		public static Boolean DeepClone<TSource>( [CanBeNull] this TSource source, [CanBeNull] TSource destination ) {
			if ( ReferenceEquals( source, destination ) ) {
				return true;
			}

			if ( source is null ) {
				return false;
			}

			if ( destination is null ) {
				return false;
			}

			//copy all settable fields
			// then
			//copy all settable properties (going on the assumption that properties should be modifiying their private fields).
			return CopyFields( source, destination ) && CopyProperties( source, destination );
		}

		/// <summary>Enumerate all fields of the <paramref name="type" /></summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<FieldInfo> GetAllFields( [CanBeNull] this Type type ) {
			if ( type == null ) {
				return Enumerable.Empty<FieldInfo>();
			}

			const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

			return type.GetFields( flags ).Union( type.BaseType?.GetAllFields() ?? Enumerable.Empty<FieldInfo>() );
		}

		/// <summary>Enumerate all properties of the <paramref name="type" /></summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<PropertyInfo> GetAllProperties( [CanBeNull] this Type? type ) {
			if ( type is null ) {
				return Enumerable.Empty<PropertyInfo>();
			}

			const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

			return type.GetProperties( flags ).Union( GetAllProperties( type.BaseType ) );
		}

		[ItemCanBeNull]
		public static IEnumerable<T?> GetEnumerableOfType<T>( [CanBeNull] params Object[]? constructorArgs ) where T : class, IComparable<T> {
			if ( !EnumerableOfTypeCache.TryGetValue( typeof( T ), out var list ) ) {
				list = Assembly.GetAssembly( typeof( T ) )?.GetTypes().ToList();

				if ( list is not null ) {
					EnumerableOfTypeCache[ typeof( T ) ] = list;
				}
			}

			if ( list is null ) {
				yield break;
			}

			foreach ( var myType in list.Where( myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf( typeof( T ) ) ) ) {
				if ( constructorArgs?.Any() == true ) {
					yield return Activator.CreateInstance( myType, constructorArgs ) as T;
				}
				else {
					var declaredCtor = myType.GetConstructors();

					foreach ( ConstructorInfo constructorInfo in declaredCtor ) {
						ParameterInfo[] parms = constructorInfo.GetParameters();
						foreach ( var _ in from parameterInfo1 in parms where parameterInfo1.ParameterType == typeof( Guid ) select parms ) {
							yield return Activator.CreateInstance( myType, Guid.NewGuid() ) as T;
						}
					}
				}
			}
		}

		/// <summary>Get all <see cref="GetSealedClassesDerivedFrom" /><paramref name="baseType" />.</summary>
		/// <param name="baseType"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<Type> GetSealedClassesDerivedFrom( [CanBeNull] this Type? baseType ) {
			if ( baseType is null ) {
				throw new ArgumentNullException( nameof( baseType ) );
			}

			return baseType.Assembly.GetTypes().Where( type => type.IsAssignableFrom( baseType ) && type.IsSealed );
		}

		/// <summary>
		///     Get all <see cref="Type" /> from <see cref="AppDomain.CurrentDomain" /> that should be able to be created via
		///     <see cref="Activator.CreateInstance(Type,BindingFlags,Binder,System.Object[],CultureInfo) " />.
		/// </summary>
		/// <param name="baseType"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<Type> GetTypesDerivedFrom( [CanBeNull] this Type? baseType ) {
			if ( baseType is null ) {
				throw new ArgumentNullException( nameof( baseType ) );
			}

			foreach ( Assembly assembly in CurrentDomainGetAssemblies.Value ) {
				foreach ( Type type in assembly.GetTypes() ) {
					if ( baseType.IsAssignableFrom( type ) && type.IsClass && !type.IsAbstract ) {
						yield return type;
					}
				}
			}
		}

		public static Boolean HasDefaultConstructor( [CanBeNull] this Type? type ) {
			if ( type is null ) {
				throw new ArgumentNullException( nameof( type ) );
			}

			return type.IsValueType || type.GetConstructor( Type.EmptyTypes ) != null;
		}

		/// <summary>
		///     Returns whether or not objects of this type can be copied byte-for-byte in to another part of the system memory
		///     without potential segmentation faults (i.e. the type contains no managed references such as <see cref="String" /> s).
		/// This function will always return <c>false</c> for non- <see cref="ValueType" /> s.
		/// </summary>
		/// <returns>True if the type can be copied (blitted), or false if not.</returns>
		public static Boolean IsBlittable( [NotNull] this Type? self ) {
			if ( self is null ) {
				throw new ArgumentNullException( nameof( self ), "IsBlittable called on a null Type." );
			}

			if ( self.IsValueType ) {
				return false;
			}

			if ( self.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).Any( fieldInfo => !fieldInfo.FieldType.IsValueType && !fieldInfo.FieldType.IsPointer ) ) {
				return false;
			}

			return self.IsExplicitLayout || self.IsLayoutSequential;
		}

		/// <summary>
		/// only quickly/simply tested. Seems to work.. is it useful now that we have pattern matching?
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="_"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		public static Boolean IsDerivedFrom<TType>( this TType _, Type c ) {
			var p = typeof( TType );

			do {
				if ( p == c ) {
					return true;
				}

				p = p.BaseType;
			} while ( p != null );

			return false;
		}

		public static Boolean IsNullable( [NotNull] this PropertyInfo p ) => p.PropertyType.IsNullable();

		public static Boolean IsNullable( [NotNull] this Type t ) => !t.IsValueType || Nullable.GetUnderlyingType( t ) != null;

		/// <summary>Ascertains if the given type is a numeric type (e.g. <see cref="Int32" />).</summary>
		/// <returns>True if the type represents a numeric type, false if not.</returns>
		public static Boolean IsNumeric( [NotNull] this Type self ) =>
			self == typeof( Double ) || self == typeof( Single ) || self == typeof( Int64 ) || self == typeof( Int16 ) || self == typeof( Byte ) ||
			self == typeof( SByte ) || self == typeof( UInt32 ) || self == typeof( UInt64 ) || self == typeof( UInt16 ) || self == typeof( Decimal ) ||
			self == typeof( Int32 );

		/// <summary>Ascertains if the given type is a numeric type (e.g. <see cref="Int32" />).</summary>
		/// <returns>True if the type represents a numeric type, false if not.</returns>
		public static Boolean IsNumeric<T>( [NotNull] this T self ) =>
			self is Int32 or Double or Single or Int64 or Int16 or Byte or SByte or UInt32 or UInt64 or
			UInt16 or Decimal;

		/// <summary>
		///     <para>Checks a type to see if it derives from a raw generic (e.g. List[[]])</para>
		/// </summary>
		/// <param name="type">   </param>
		/// <param name="generic"></param>
		/// <returns></returns>
		public static Boolean IsSubclassOfRawGeneric( [CanBeNull] this Type? type, [CanBeNull] Type generic ) {
			while ( type != typeof( Object ) ) {
				var cur = type != null && type.IsGenericType ? type.GetGenericTypeDefinition() : type;

				if ( generic == cur ) {
					return true;
				}

				type = type?.BaseType;
			}

			return false;
		}

		public static Boolean MergeDictionaries<TSource>( [NotNull][ItemCanBeNull] this IDictionary sourceValue, [NotNull] FieldInfo field, [CanBeNull] TSource destination ) {
			if ( field.GetValue( destination ) is not IDictionary destAsDictionary ) {
				return false;
			}

			foreach ( DictionaryEntry pair in sourceValue ) {
				try {
					destAsDictionary[ pair.Key ] = pair.Value;
				}
				catch ( Exception exception ) {
					exception.Log();
				}
			}

			return true;
		}

		[NotNull]
		public static String Name<T>( [NotNull] this Expression<Func<T>> propertyExpression ) => ( propertyExpression.Body as MemberExpression )?.Member.Name ?? String.Empty;

		/// <summary>Alternate method of creating an object of type. Not proven to be faster than new().</summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[NotNull]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static T Newby<T>() where T : class => typeof( T ).Newby<T>();

		/// <summary>Alternate method of creating an object of type. Not proven to be faster than new().</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		[NotNull]
		public static T Newby<T>( this Type type ) where T : class {
			if ( !ObjectActivators.TryGetValue( type, out var activator ) ) {
				var dynamicMethod = new DynamicMethod( "CreateInstance", type, Type.EmptyTypes, true );

				var ilGenerator = dynamicMethod.GetILGenerator(); //can this be optimized any further?
				ilGenerator.Emit( OpCodes.Nop );                  //why nop here? alignment?

				ilGenerator.Emit( OpCodes.Newobj,
								  type.GetConstructor( Type.EmptyTypes ) ??
								  throw new InvalidOperationException( $"Could not {nameof( type.GetConstructor )} for type {type.FullName}." ) );

				ilGenerator.Emit( OpCodes.Ret );

				activator = ( ObjectActivator )dynamicMethod.CreateDelegate( typeof( ObjectActivator ) );
				ObjectActivators.Add( type, activator );
			}

			return ( T )activator.Invoke();
		}

		[NotNull]
		public static Func<Object?> NewInstanceByCreate( [NotNull] this Type type ) => () => Activator.CreateInstance( type );

		[NotNull]
		public static Func<Object> NewInstanceByLambda( [NotNull] this Type type ) => Expression.Lambda<Func<Object>>( Expression.New( type ) ).Compile();

		/// <summary>Get the <see cref="Type" /> associated with the subject <see cref="TypeCode" />.</summary>
		/// <param name="self">The extended TypeCode.</param>
		/// <returns>A <see cref="Type" /> that <paramref name="self" /> represents.</returns>
		[NotNull]
		public static Type ToType( this TypeCode self ) =>
			self switch {
				TypeCode.Boolean => typeof( Boolean ),
				TypeCode.Byte => typeof( Byte ),
				TypeCode.Char => typeof( Char ),
				TypeCode.DBNull => typeof( DBNull ),
				TypeCode.DateTime => typeof( DateTime ),
				TypeCode.Decimal => typeof( Decimal ),
				TypeCode.Double => typeof( Double ),
				TypeCode.Int16 => typeof( Int16 ),
				TypeCode.Int32 => typeof( Int32 ),
				TypeCode.Int64 => typeof( Int64 ),
				TypeCode.SByte => typeof( SByte ),
				TypeCode.Single => typeof( Single ),
				TypeCode.String => typeof( String ),
				TypeCode.UInt16 => typeof( UInt16 ),
				TypeCode.UInt32 => typeof( UInt32 ),
				TypeCode.UInt64 => typeof( UInt64 ),
				TypeCode.Empty => typeof( Object ),
				TypeCode.Object => typeof( Object ),
				var _ => typeof( Object )
			};

		public static Boolean TryCast<T>( this Object value, [CanBeNull] out T result ) {
			var type = typeof( T );

			// If the type is nullable and the result should be null, set a null value.
			if ( type.IsNullable() && value == DBNull.Value ) {
				result = default( T )!;

				return true;
			}

			// Convert.ChangeType fails on Nullable<T> types. We want to try to cast to the underlying type anyway.
			var underlyingType = Nullable.GetUnderlyingType( type ) ?? type;

			try {

				// Just one edge case you might want to handle.
				if ( underlyingType == typeof( Guid ) ) {
					value = value switch {
						String s => new Guid( s ),
						Byte[] bytes => new Guid( bytes ),
						var _ => value
					};
				}

				result = ( T )Convert.ChangeType( value, underlyingType );

				return true;
			}
			catch ( Exception ) {
				result = default( T )!;

				return false;
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