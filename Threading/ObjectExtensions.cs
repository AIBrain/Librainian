// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "ObjectExtensions.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license
// has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/ObjectExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 4:23 AM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Code pulled from https://raw.githubusercontent.com/Burtsev-Alexey/net-object-deep-copy/master/ObjectExtensions.cs
    /// </summary>
    public static class ObjectExtensions {

        private static readonly MethodInfo CloneMethod = typeof( Object ).GetMethod( "MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance );

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

            if ( visited.ContainsKey( originalObject ) ) { return visited[originalObject]; }

            if ( typeof( Delegate ).IsAssignableFrom( typeToReflect ) ) { return null; }

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
            if ( null == typeToReflect.BaseType ) { return; }

            RecursiveCopyBaseTypePrivateFields( originalObject, visited, cloneObject, typeToReflect.BaseType );
            CopyFields( originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate );
        }

        /// <summary>
        /// Returns a deep copy of this object.
        /// </summary>
        /// <param name="originalObject"></param>
        /// <returns></returns>
        public static Object Copy( this Object originalObject ) => InternalCopy( originalObject, new Dictionary<Object, Object>( new ReferenceEqualityComparer() ) );

        public static T Copy<T>( this T original ) => ( T )Copy( ( Object )original );

        public static Boolean IsPrimitive( this Type type ) {
            if ( type == typeof( String ) ) { return true; }

            return type.IsValueType && type.IsPrimitive;
        }
    }
}