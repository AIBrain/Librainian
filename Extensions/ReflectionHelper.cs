// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ReflectionHelper.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/ReflectionHelper.cs" was last cleaned by Protiguous on 2018/05/15 at 10:40 PM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ReflectionHelper {

        /// <summary>
        ///     Find all types in 'assembly' that derive from 'baseType'
        /// </summary>
        /// <owner>jayBaz</owner>
        public static IEnumerable<Type> FindAllTypesThatDeriveFrom<TBase>( this Assembly assembly ) => from type in assembly.GetTypes() where type.IsSubclassOf( typeof( TBase ) ) select type;

        // I find that the default GetFields behavior is not suitable to my needs
        public static IEnumerable<FieldInfo> GetAllDeclaredInstanceFields( this Type type ) => type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly );

        /// <summary>
        ///     A typesafe wrapper for Attribute.GetCustomAttribute
        /// </summary>
        /// <remarks>TODO: add overloads for Assembly, Module, and ParameterInfo</remarks>
        public static TAttribute GetCustomAttribute<TAttribute>( this MemberInfo element ) where TAttribute : Attribute => ( TAttribute )Attribute.GetCustomAttribute( element, typeof( TAttribute ) );

        /// <summary>
        ///     All types across multiple assemblies
        /// </summary>
        public static IEnumerable<Type> GetTypes( this IEnumerable<Assembly> assemblies ) => assemblies.SelectMany( assembly => assembly.GetTypes() );

        /// <summary>
        ///     Check if the given type has the given attribute on it. Don't look at base classes.
        /// </summary>
        /// <owner>jayBaz</owner>
        public static Boolean TypeHasAttribute<TAttribute>( this Type type ) where TAttribute : Attribute => Attribute.IsDefined( type, typeof( TAttribute ) );
    }
}