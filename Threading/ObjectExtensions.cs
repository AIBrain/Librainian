// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ObjectExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    ///     Code pulled from https://raw.githubusercontent.com/Burtsev-Alexey/net-object-deep-copy/master/ObjectExtensions.cs
    /// </summary>
    public static class ObjectExtensions {
        private static readonly MethodInfo CloneMethod = typeof( Object ).GetMethod( "MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance );

        /// <summary>
        ///     Returns a deep copy of this object.
        /// </summary>
        /// <param name="originalObject"></param>
        /// <returns></returns>
        public static Object Copy( this Object originalObject ) => InternalCopy( originalObject, new Dictionary<Object, Object>( new ReferenceEqualityComparer() ) );

	    public static T Copy<T>( this T original ) => ( T )Copy( ( Object )original );

	    public static Boolean IsPrimitive( this Type type ) {
            if ( type == typeof( String ) ) {
                return true;
            }
            return type.IsValueType && type.IsPrimitive;
        }

        private static void CopyFields( Object originalObject, IDictionary<Object, Object> visited, Object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, Boolean> filter = null ) {
            foreach ( var fieldInfo in typeToReflect.GetFields( bindingFlags ) ) {
                if ( filter != null && filter( fieldInfo ) == false ) {
                    continue;
                }
                if ( IsPrimitive( fieldInfo.FieldType ) ) {
                    continue;
                }
                var originalFieldValue = fieldInfo.GetValue( originalObject );
                var clonedFieldValue = InternalCopy( originalFieldValue, visited );
                fieldInfo.SetValue( cloneObject, clonedFieldValue );
            }
        }

        private static Object InternalCopy( Object originalObject, IDictionary<Object, Object> visited ) {
            if ( originalObject == null ) {
                return null;
            }
            var typeToReflect = originalObject.GetType();
            if ( IsPrimitive( typeToReflect ) ) {
                return originalObject;
            }
            if ( visited.ContainsKey( originalObject ) ) {
                return visited[ originalObject ];
            }
            if ( typeof( Delegate ).IsAssignableFrom( typeToReflect ) ) {
                return null;
            }
            var cloneObject = CloneMethod.Invoke( originalObject, null );
            if ( typeToReflect.IsArray ) {
                var arrayType = typeToReflect.GetElementType();
                if ( IsPrimitive( arrayType ) == false ) {
                    var clonedArray = ( Array )cloneObject;
                    clonedArray.ForEach( ( array, indices ) => array.SetValue( InternalCopy( clonedArray.GetValue( indices ), visited ), indices ) );
                }
            }
            visited.Add( originalObject, cloneObject );
            CopyFields( originalObject, visited, cloneObject, typeToReflect );
            RecursiveCopyBaseTypePrivateFields( originalObject, visited, cloneObject, typeToReflect );
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields( Object originalObject, IDictionary<Object, Object> visited, Object cloneObject, Type typeToReflect ) {
            if ( null == typeToReflect.BaseType ) {
                return;
            }
            RecursiveCopyBaseTypePrivateFields( originalObject, visited, cloneObject, typeToReflect.BaseType );
            CopyFields( originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate );
        }
    }
}