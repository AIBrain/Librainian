// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ObjectExtensions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "ObjectExtensions.cs" was last formatted by Protiguous on 2020/03/16 at 3:02 PM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Collections.Extensions;
    using JetBrains.Annotations;

    /// <summary>Code pulled from https://raw.githubusercontent.com/Burtsev-Alexey/net-object-deep-copy/master/ObjectExtensions.cs</summary>
    public static class ObjectExtensions {

        private static MethodInfo CloneMethod { get; } = typeof( Object ).GetMethod( name: "MemberwiseClone", bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance );

        private static void CopyFields( [CanBeNull] Object originalObject, [CanBeNull] IDictionary<Object, Object> visited, [CanBeNull] Object cloneObject,
            [NotNull] Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,
            [CanBeNull] Func<FieldInfo, Boolean> filter = null ) {
            foreach ( var fieldInfo in typeToReflect.GetFields( bindingAttr: bindingFlags ) ) {
                if ( filter?.Invoke( arg: fieldInfo ) == false ) {
                    continue;
                }

                if ( IsPrimitive( type: fieldInfo.FieldType ) ) {
                    continue;
                }

                var originalFieldValue = fieldInfo.GetValue( obj: originalObject );
                var clonedFieldValue = InternalCopy( originalObject: originalFieldValue, visited: visited );
                fieldInfo.SetValue( obj: cloneObject, value: clonedFieldValue );
            }
        }

        [CanBeNull]
        private static Object InternalCopy( [CanBeNull] Object originalObject, [CanBeNull] IDictionary<Object, Object> visited ) {
            if ( originalObject is null ) {
                return null;
            }

            var typeToReflect = originalObject.GetType();

            if ( IsPrimitive( type: typeToReflect ) ) {
                return originalObject;
            }

            if ( visited.ContainsKey( key: originalObject ) ) {
                return visited[ key: originalObject ];
            }

            if ( typeof( Delegate ).IsAssignableFrom( c: typeToReflect ) ) {
                return null;
            }

            var cloneObject = CloneMethod.Invoke( obj: originalObject, parameters: null );

            if ( typeToReflect.IsArray ) {
                var arrayType = typeToReflect.GetElementType();

                if ( arrayType != null && IsPrimitive( type: arrayType ) == false ) {
                    var clonedArray = ( Array ) cloneObject;

                    clonedArray.ForEach( action: ( array, indices ) =>
                        array.SetValue( value: InternalCopy( originalObject: clonedArray.GetValue( indices: indices ), visited: visited ), indices: indices ) );
                }
            }

            visited.Add( key: originalObject, value: cloneObject );
            CopyFields( originalObject: originalObject, visited: visited, cloneObject: cloneObject, typeToReflect: typeToReflect );
            RecursiveCopyBaseTypePrivateFields( originalObject: originalObject, visited: visited, cloneObject: cloneObject, typeToReflect: typeToReflect );

            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields( [CanBeNull] Object originalObject, [CanBeNull] IDictionary<Object, Object> visited,
            [CanBeNull] Object cloneObject, [NotNull] Type typeToReflect ) {
            if ( null == typeToReflect.BaseType ) {
                return;
            }

            RecursiveCopyBaseTypePrivateFields( originalObject: originalObject, visited: visited, cloneObject: cloneObject, typeToReflect: typeToReflect.BaseType );

            CopyFields( originalObject: originalObject, visited: visited, cloneObject: cloneObject, typeToReflect: typeToReflect.BaseType,
                bindingFlags: BindingFlags.Instance | BindingFlags.NonPublic, filter: info => info.IsPrivate );
        }

        /// <summary>Returns a deep copy of this object.</summary>
        /// <param name="originalObject"></param>
        /// <returns></returns>
        [CanBeNull]
        public static Object Copy( [CanBeNull] this Object originalObject ) =>
            InternalCopy( originalObject: originalObject, visited: new Dictionary<Object, Object>( comparer: new ReferenceEqualityComparer() ) );

        [CanBeNull]
        public static T Copy<T>( [CanBeNull] this T original ) => ( T ) Copy( originalObject: ( Object ) original );

        [CanBeNull]
        public static Object GetPrivateFieldValue<T>( [NotNull] this T instance, [NotNull] String fieldName ) {
            if ( instance is null ) {
                throw new ArgumentNullException( paramName: nameof( instance ) );
            }

            if ( String.IsNullOrWhiteSpace( value: fieldName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fieldName ) );
            }

            var type = instance.GetType();
            var info = type.GetField( name: fieldName, bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance );

            if ( info is null ) {
                throw new ArgumentException( message: $"{type.FullName} does not contain the private field '{fieldName}'." );
            }

            return info.GetValue( obj: instance );
        }

        public static Boolean IsPrimitive<T>( [NotNull] this T type ) {
            if ( type is String ) {
                return true;
            }

            var gt = type.GetType();

            return gt.IsValueType && gt.IsPrimitive;
        }

    }

}