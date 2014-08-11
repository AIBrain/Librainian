#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/ReflectionHelper.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    [Serializable]
    internal static class ReflectionHelper {
        /// <summary>
        ///     All types across multiple assemblies
        /// </summary>
        public static IEnumerable< Type > GetTypes( this IEnumerable< Assembly > assemblies ) {
            return from assembly in assemblies
                   from type in assembly.GetTypes()
                   select type;
        }

        /// <summary>
        ///     Find all types in 'assembly' that derive from 'baseType'
        /// </summary>
        /// <owner>jayBaz</owner>
        internal static IEnumerable< Type > FindAllTypesThatDeriveFrom< TBase >( Assembly assembly ) {
            return from type in assembly.GetTypes()
                   where type.IsSubclassOf( typeof ( TBase ) )
                   select type;
        }

        // I find that the default GetFields behavior is not suitable to my needs
        internal static IEnumerable< FieldInfo > GetAllDeclaredInstanceFields( Type type ) {
            return type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly );
        }

        /// <summary>
        ///     A typesafe wrapper for Attribute.GetCustomAttribute
        /// </summary>
        /// <remarks> TODO:add overloads for Assembly, Module, and ParameterInfo</remarks>
        internal static TAttribute GetCustomAttribute< TAttribute >( MemberInfo element ) where TAttribute : Attribute {
            return ( TAttribute ) Attribute.GetCustomAttribute( element, typeof ( TAttribute ) );
        }

        /// <summary>
        ///     Check if the given type has the given attribute on it. Don't look at base classes.
        /// </summary>
        /// <owner>jayBaz</owner>
        internal static Boolean TypeHasAttribute< TAttribute >( Type type ) where TAttribute : Attribute {
            return Attribute.IsDefined( type, typeof ( TAttribute ) );
        }
    }
}
