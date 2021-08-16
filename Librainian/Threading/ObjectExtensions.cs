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
// File "ObjectExtensions.cs" last formatted on 2020-08-14 at 8:46 PM.

#nullable enable

namespace Librainian.Threading {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Reflection;
	using Exceptions;
	using Utilities;

	/// <summary>
	///     Code pulled from
	///     <see cref="http://raw.githubusercontent.com/Burtsev-Alexey/net-object-deep-copy/master/ObjectExtensions.cs" />
	/// </summary>
	/// <remarks>
	///     TODO Needs some serious testing.
	/// </remarks>
	[NeedsTesting]
	public static class ObjectExtensions {

		[NeedsTesting]
		private static MethodInfo? MemberwiseCloneMethod { get; } = typeof( Object ).GetMethod( "MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance );

		[NeedsTesting]
		private static void CopyFields(
			this Object original,
			IDictionary<Object, Object?> visited,
			Object? destination,
			IReflect reflect,
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,
			Func<FieldInfo, Boolean>? filter = null
		) {
			var fields = reflect.GetFields( bindingFlags );

			foreach ( var field in fields ) {
				if ( filter?.Invoke( field ) == false ) {
					continue;
				}

				//if ( fieldInfo.FieldType.IsPrimitive() ) { continue; }	//why skip a primitive?

				var value = field.GetValue( original );
				field.SetValue( destination, InternalCopy( value, visited ) );
			}
		}

		[NeedsTesting]
		private static Object? InternalCopy( Object? originalObject, IDictionary<Object, Object?> visits ) {
			if ( originalObject is null ) {
				return null;
			}

			var reflect = originalObject.GetType();

			if ( reflect.IsPrimitive() ) {
				return originalObject;
			}

			if ( visits.ContainsKey( originalObject ) ) {
				return visits[ originalObject ];
			}

			if ( typeof( Delegate ).IsAssignableFrom( reflect ) ) {
				return null;
			}

			var copy = MemberwiseCloneMethod?.Invoke( originalObject, null );

			if ( reflect.IsArray ) {
				var elementsType = reflect.GetElementType();

				if ( elementsType != null /*&& !elementsType.IsPrimitive()*/ ) {

					//TODO why skip primitives?

					if ( copy is Array clonedArray ) {
						for ( var index = 0; index < clonedArray.Length; index++ ) {
							clonedArray.SetValue( InternalCopy( clonedArray.GetValue( index ), visits ), index );
						}
					}
				}
			}

			visits.Add( originalObject, copy );
			originalObject.CopyFields( visits, copy, reflect );
			RecursiveCopyBaseTypePrivateFields( originalObject, visits, copy, reflect );

			return copy;
		}

		[NeedsTesting]
		private static void RecursiveCopyBaseTypePrivateFields(
			Object? originalObject,
			IDictionary<Object, Object?> visited,
			Object? destination,
			Type typeToReflect
		) {
			if ( originalObject is null ) {
				return;
			}
			if ( destination is null ) {
				return;
			}
			if ( typeToReflect.BaseType is null ) {
				return;
			}

			RecursiveCopyBaseTypePrivateFields( originalObject, visited, destination, typeToReflect.BaseType );

			originalObject.CopyFields( visited, destination, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate );
		}

		[NeedsTesting]
		public static T? Copy<T>( this T? original ) =>
					( T? )( DeepCopy( original ) ?? throw new NullReferenceException( nameof( original ) ) );

		/// <summary>Returns a deep copy of this object.</summary>
		/// <param name="original"></param>
		[NeedsTesting]
		public static Object? DeepCopy<T>( this T? original ) => InternalCopy( original, new Dictionary<Object, Object?>( new ReferenceEqualComparer<Object?>() ) );

		[NeedsTesting]
		public static Object? GetPrivateFieldValue<T>( [DisallowNull] this T instance, String fieldName ) {
			if ( instance is null ) {
				throw new ArgumentEmptyException( nameof( instance ) );
			}

			if ( String.IsNullOrWhiteSpace( fieldName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fieldName ) );
			}

			var type = instance.GetType();
			var info = type.GetField( fieldName, BindingFlags.NonPublic | BindingFlags.Instance );

			if ( info is null ) {
				throw new ArgumentException( $"{type.FullName} does not contain the private field '{fieldName}'." );
			}

			return info.GetValue( instance );
		}

		[NeedsTesting]
		public static Boolean IsPrimitive<T>( this T? type ) =>
			type switch {
				null => false,
				String _ => true,
				var _ => type.GetType().IsPrimitive
			};
	}
}