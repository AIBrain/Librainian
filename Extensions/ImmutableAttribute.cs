// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/ImmutableAttribute.cs" was last cleaned by Rick on 2015/08/05 at 1:51 PM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;

    /// <summary>Without further ado, here's the ImmutableAttribute itself.</summary>
    /// <seealso cref="http://blogs.msdn.com/b/kevinpilchbisson/archive/2007/11/20/enforcing-immutability-in-code.aspx" />
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
    [Serializable]
    [MeansImplicitUse]
    public sealed class ImmutableAttribute : Attribute {

        [SuppressMessage( "Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable" )]
        [SuppressMessage( "Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors" )]
        [Serializable]
        public class ImmutableFailureException : Exception {

            public readonly Type Type;

            internal ImmutableFailureException( Type type, String message, Exception inner ) : base( message, inner ) {
                this.Type = type;
            }

            internal ImmutableFailureException( Type type, String message ) : base( message ) {
                this.Type = type;
            }

            protected ImmutableFailureException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) {
            }

        }

        [Serializable]
        private class MutableBaseException : ImmutableFailureException {

            internal MutableBaseException( Type type, Exception inner ) : base( type, FormatMessage( type ), inner ) {
            }

            protected MutableBaseException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) {
            }

            [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)" )]
            private static String FormatMessage( Type type ) => $"'{type}' is mutable because its base type ('[{type.BaseType}]') is mutable.";

        }

        [Serializable]
        private class MutableFieldException : ImmutableFailureException {

            internal MutableFieldException( FieldInfo fieldInfo, Exception inner ) : base( fieldInfo.DeclaringType, FormatMessage( fieldInfo ), inner ) {
            }

            protected MutableFieldException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) {
            }

            [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)" )]
            private static String FormatMessage( FieldInfo fieldInfo ) => $"'{fieldInfo.DeclaringType}' is mutable because '{fieldInfo.Name}' of type '{fieldInfo.FieldType}' is mutable.";

        }

        [Serializable]
        private class WritableFieldException : ImmutableFailureException {

            internal WritableFieldException( FieldInfo fieldInfo ) : base( fieldInfo.DeclaringType, FormatMessage( fieldInfo ) ) {
            }

            protected WritableFieldException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) {
            }

            [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)" )]
            private static String FormatMessage( FieldInfo fieldInfo ) => $"'{fieldInfo.DeclaringType}' is mutable because field '{fieldInfo.Name}' is not marked 'readonly'.";

        }

        // in some cases, a type is immutable but can't be proven as such. in these cases, the
        // developer can mark the type with [Immutable(true)] and the code below will take it on
        // faith that the type is immutable, instead of testing explicitly.
        // 
        // A common example is a type that contains a List<T>, but doesn't modify it after construction.
        // 
        // TODO: replace this with a per-field attribute, to allow the immutability test to run over
        // the rest of the type.

        public Boolean OnFaith;

        /// <summary>Ensures that 'type' follows the rules for immutability</summary>
        /// <exception cref="ImmutableFailureException">Thrown if a mutability issue appears.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1002:DoNotExposeGenericLists" )]
        public static void VerifyTypeIsImmutable( [NotNull] Type type, [NotNull] IEnumerable<Type> whiteList ) {
            if ( type == null ) {
                throw new ArgumentNullException( nameof( type ) );
            }
            if ( type.BaseType == null ) {
                throw new ArgumentNullException( nameof( type ) );
            }

            if ( whiteList == null ) {
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

            foreach ( var fieldInfo in ReflectionHelper.GetAllDeclaredInstanceFields( type ) ) {
                if ( ( fieldInfo.Attributes & FieldAttributes.InitOnly ) == 0 ) {
                    throw new WritableFieldException( fieldInfo );
                }

                // if it's marked with [Immutable], that's good enough, as we can be sure that these
                // tests will all be applied to this type
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

        /// <summary>
        /// Ensures that all types in 'assemblies' that are marked [Immutable] follow the rules for immutability.
        /// </summary>
        /// <exception cref="ImmutableFailureException">Thrown if a mutability issue appears.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1002:DoNotExposeGenericLists" )]
        public static void VerifyTypesAreImmutable( IEnumerable<Assembly> assemblies, params Type[] whiteList ) {
            var typesMarkedImmutable = from type in assemblies.GetTypes() where IsMarkedImmutable( type ) select type;

            foreach ( var type in typesMarkedImmutable ) {
                VerifyTypeIsImmutable( type, whiteList );
            }
        }

        private static Boolean IsMarkedImmutable( Type type ) => ReflectionHelper.TypeHasAttribute<ImmutableAttribute>( type );

        private static Boolean IsWhiteListed( Type type ) {

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
            return ( immutableAttribute != null ) && immutableAttribute.OnFaith;
        }

    }

}
