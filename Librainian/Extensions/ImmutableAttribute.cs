// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ImmutableAttribute.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "ImmutableAttribute.cs" was last formatted by Protiguous on 2020/03/18 at 10:23 AM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>Without further ado, here's the ImmutableAttribute itself. Now.. does it work?</summary>
    /// <see cref="http://blogs.msdn.com/b/kevinpilchbisson/archive/2007/11/20/enforcing-immutability-in-code.aspx" />
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
    [JsonObject]
    [MeansImplicitUse]
    public sealed class ImmutableAttribute : Attribute {

        public Boolean OnFaith;

        private static Boolean IsMarkedImmutable( [NotNull] Type type ) {
            if ( type is null ) {
                throw new ArgumentNullException( nameof( type ) );
            }

            return type.TypeHasAttribute<ImmutableAttribute>();
        }

        private static Boolean IsWhiteListed( [NotNull] Type type ) {
            if ( type is null ) {
                throw new ArgumentNullException( nameof( type ) );
            }

            // Boolean, int, etc.
            if ( type.IsPrimitive ) {
                return true;
            }

            if ( type == typeof( Object ) ) {
                return true;
            }

            if ( type == typeof( String ) ) {
                return true;
            }

            if ( type == typeof( Guid ) ) {
                return true;
            }

            if ( type.IsEnum ) {
                return true;
            }

            // override all checks on this type if [ImmutableAttribute(OnFaith=true)] is set
            var immutableAttribute = ReflectionHelper.GetCustomAttribute<ImmutableAttribute>( type );

            return immutableAttribute?.OnFaith == true;
        }

        // in some cases, a type is immutable but can't be proven as such. in these cases, the developer can mark the type with [Immutable(true)] and the code below will take it on faith that the type is immutable,
        // instead of testing explicitly.
        //
        // A common example is a type that contains a List<T>, but doesn't modify it after construction.
        //
        // TODO: replace this with a per-field attribute, to allow the immutability test to run over the rest of the type.
        /// <summary>Ensures that 'type' follows the rules for immutability</summary>
        /// <exception cref="ImmutableFailureException">Thrown if a mutability issue appears.</exception>
        public static void VerifyTypeIsImmutable( [NotNull] Type type, [NotNull] IEnumerable<Type> whiteList ) {
            if ( type is null ) {
                throw new ArgumentNullException( nameof( type ) );
            }

            if ( type.BaseType is null ) {
                throw new ArgumentNullException( nameof( type ) );
            }

            if ( whiteList is null ) {
                throw new ArgumentNullException( nameof( whiteList ) );
            }

            var enumerable = whiteList as IList<Type> ?? whiteList.ToList();

            if ( enumerable.Contains( type ) ) {
                return;
            }

            if ( IsWhiteListed( type ) ) {
                return;
            }

            try {
                VerifyTypeIsImmutable( type.BaseType, enumerable );
            }
            catch ( ImmutableFailureException ex ) {
                throw new MutableBaseException( type, ex );
            }

            foreach ( var fieldInfo in type.GetAllDeclaredInstanceFields() ) {
                if ( ( fieldInfo.Attributes & FieldAttributes.InitOnly ) == 0 ) {
                    throw new WritableFieldException( fieldInfo );
                }

                // if it's marked with [Immutable], that's good enough, as we can be sure that these tests will all be applied to this type
                if ( IsMarkedImmutable( fieldInfo.FieldType ) ) {
                    continue;
                }

                try {
                    VerifyTypeIsImmutable( fieldInfo.FieldType, enumerable );
                }
                catch ( ImmutableFailureException ex ) {
                    throw new MutableFieldException( fieldInfo, ex );
                }
            }
        }

        /// <summary>Ensures that all types in 'assemblies' that are marked [Immutable] follow the rules for immutability.</summary>
        /// <exception cref="ImmutableFailureException">Thrown if a mutability issue appears.</exception>
        public static void VerifyTypesAreImmutable( [NotNull] IEnumerable<Assembly> assemblies, [NotNull] params Type[] whiteList ) {
            if ( assemblies is null ) {
                throw new ArgumentNullException( nameof( assemblies ) );
            }

            if ( whiteList == null ) {
                throw new ArgumentNullException( nameof( whiteList ) );
            }

            var typesMarkedImmutable = from type in assemblies.GetTypes() where IsMarkedImmutable( type ) select type;

            foreach ( var type in typesMarkedImmutable ) {
                VerifyTypeIsImmutable( type, whiteList );
            }
        }

    }

}