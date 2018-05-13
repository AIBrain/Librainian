// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ReflectionHelper.cs" was last cleaned by Protiguous on 2018/05/12 at 1:23 AM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ReflectionHelper {

        /// <summary>
        /// Find all types in 'assembly' that derive from 'baseType'
        /// </summary>
        /// <owner>jayBaz</owner>
        public static IEnumerable<Type> FindAllTypesThatDeriveFrom<TBase>( this Assembly assembly ) => from type in assembly.GetTypes() where type.IsSubclassOf( typeof( TBase ) ) select type;

        // I find that the default GetFields behavior is not suitable to my needs
        public static IEnumerable<FieldInfo> GetAllDeclaredInstanceFields( this Type type ) => type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly );

        /// <summary>
        /// A typesafe wrapper for Attribute.GetCustomAttribute
        /// </summary>
        /// <remarks>TODO: add overloads for Assembly, Module, and ParameterInfo</remarks>
        public static TAttribute GetCustomAttribute<TAttribute>( this MemberInfo element ) where TAttribute : Attribute => ( TAttribute )Attribute.GetCustomAttribute( element, typeof( TAttribute ) );

        /// <summary>
        /// All types across multiple assemblies
        /// </summary>
        public static IEnumerable<Type> GetTypes( this IEnumerable<Assembly> assemblies ) => assemblies.SelectMany( assembly => assembly.GetTypes() );

        /// <summary>
        /// Check if the given type has the given attribute on it. Don't look at base classes.
        /// </summary>
        /// <owner>jayBaz</owner>
        public static Boolean TypeHasAttribute<TAttribute>( this Type type ) where TAttribute : Attribute => Attribute.IsDefined( type, typeof( TAttribute ) );
    }
}