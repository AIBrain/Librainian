// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ObjectExtensions.cs" belongs to Rick@AIBrain.org and
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
// File "ObjectExtensions.cs" was last formatted by Protiguous on 2018/06/04 at 4:28 PM.

namespace Librainian.Threading {

	using System;
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	///     Code pulled from https://raw.githubusercontent.com/Burtsev-Alexey/net-object-deep-copy/master/ObjectExtensions.cs
	/// </summary>
	public static class ObjectExtensions {

		private static void CopyFields( Object originalObject, IDictionary<Object, Object> visited, Object cloneObject, Type typeToReflect,
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, Boolean> filter = null ) {
			foreach ( var fieldInfo in typeToReflect.GetFields( bindingFlags ) ) {
				if ( filter != null && filter( fieldInfo ) == false ) { continue; }

				if ( IsPrimitive( fieldInfo.FieldType ) ) { continue; }

				var originalFieldValue = fieldInfo.GetValue( originalObject );
				var clonedFieldValue = InternalCopy( originalFieldValue, visited );
				fieldInfo.SetValue( cloneObject, clonedFieldValue );
			}
		}

		private static Object InternalCopy( Object originalObject, IDictionary<Object, Object> visited ) {
			if ( originalObject is null ) { return null; }

			var typeToReflect = originalObject.GetType();

			if ( IsPrimitive( typeToReflect ) ) { return originalObject; }

			if ( visited.ContainsKey( originalObject ) ) { return visited[ originalObject ]; }

			if ( typeof( Delegate ).IsAssignableFrom( typeToReflect ) ) { return null; }

			var cloneObject = CloneMethod.Invoke( originalObject, null );

			if ( typeToReflect.IsArray ) {
				var arrayType = typeToReflect.GetElementType();

				if ( IsPrimitive( arrayType ) == false ) {
					var clonedArray = ( Array ) cloneObject;
					clonedArray.ForEach( ( array, indices ) => array.SetValue( InternalCopy( clonedArray.GetValue( indices ), visited ), indices ) );
				}
			}

			visited.Add( originalObject, cloneObject );
			CopyFields( originalObject, visited, cloneObject, typeToReflect );
			RecursiveCopyBaseTypePrivateFields( originalObject, visited, cloneObject, typeToReflect );

			return cloneObject;
		}

		private static void RecursiveCopyBaseTypePrivateFields( Object originalObject, IDictionary<Object, Object> visited, Object cloneObject, Type typeToReflect ) {
			if ( null == typeToReflect.BaseType ) { return; }

			RecursiveCopyBaseTypePrivateFields( originalObject, visited, cloneObject, typeToReflect.BaseType );
			CopyFields( originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate );
		}

		/// <summary>
		///     Returns a deep copy of this object.
		/// </summary>
		/// <param name="originalObject"></param>
		/// <returns></returns>
		public static Object Copy( this Object originalObject ) => InternalCopy( originalObject, new Dictionary<Object, Object>( new ReferenceEqualityComparer() ) );

		public static T Copy<T>( this T original ) => ( T ) Copy( ( Object ) original );

		public static Boolean IsPrimitive( this Type type ) {
			if ( type == typeof( String ) ) { return true; }

			return type.IsValueType && type.IsPrimitive;
		}

		private static readonly MethodInfo CloneMethod = typeof( Object ).GetMethod( "MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance );

	}

}