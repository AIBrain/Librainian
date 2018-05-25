// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ImmutableAttribute.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/ImmutableAttribute.cs" was last formatted by Protiguous on 2018/05/24 at 7:08 PM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     Without further ado, here's the ImmutableAttribute itself.
    /// </summary>
    /// <seealso cref="http://blogs.msdn.com/b/kevinpilchbisson/archive/2007/11/20/enforcing-immutability-in-code.aspx" />
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
    [JsonObject]
    [MeansImplicitUse]
    public sealed class ImmutableAttribute : Attribute {

        public Boolean OnFaith;

        private static Boolean IsMarkedImmutable( Type type ) => type.TypeHasAttribute<ImmutableAttribute>();

        private static Boolean IsWhiteListed( Type type ) {

            // Boolean, int, etc.
            if ( type.IsPrimitive ) { return true; }

            if ( type == typeof( Object ) ) { return true; }

            if ( type == typeof( String ) ) { return true; }

            if ( type == typeof( Guid ) ) { return true; }

            if ( type.IsEnum ) { return true; }

            // override all checks on this type if [ImmutableAttribute(OnFaith=true)] is set
            var immutableAttribute = ReflectionHelper.GetCustomAttribute<ImmutableAttribute>( type );

            return immutableAttribute.OnFaith;
        }

        // in some cases, a type is immutable but can't be proven as such. in these cases, the developer can mark the type with [Immutable(true)] and the code below will take it on faith that the type is immutable,
        // instead of testing explicitly.
        //
        // A common example is a type that contains a List<T>, but doesn't modify it after construction.
        //
        // TODO: replace this with a per-field attribute, to allow the immutability test to run over the rest of the type.
        /// <summary>
        ///     Ensures that 'type' follows the rules for immutability
        /// </summary>
        /// <exception cref="ImmutableFailureException">Thrown if a mutability issue appears.</exception>
        public static void VerifyTypeIsImmutable( [NotNull] Type type, [NotNull] IEnumerable<Type> whiteList ) {
            if ( type is null ) { throw new ArgumentNullException( nameof( type ) ); }

            if ( type.BaseType is null ) { throw new ArgumentNullException( nameof( type ) ); }

            if ( whiteList is null ) { throw new ArgumentNullException( nameof( whiteList ) ); }

            var enumerable = whiteList as IList<Type> ?? whiteList.ToList();

            if ( enumerable.Contains( type ) ) { return; }

            if ( IsWhiteListed( type ) ) { return; }

            try { VerifyTypeIsImmutable( type.BaseType, enumerable ); }
            catch ( ImmutableFailureException ex ) { throw new MutableBaseException( type, ex ); }

            foreach ( var fieldInfo in type.GetAllDeclaredInstanceFields() ) {
                if ( ( fieldInfo.Attributes & FieldAttributes.InitOnly ) == 0 ) { throw new WritableFieldException( fieldInfo ); }

                // if it's marked with [Immutable], that's good enough, as we can be sure that these tests will all be applied to this type
                if ( IsMarkedImmutable( fieldInfo.FieldType ) ) { continue; }

                try { VerifyTypeIsImmutable( fieldInfo.FieldType, enumerable ); }
                catch ( ImmutableFailureException ex ) { throw new MutableFieldException( fieldInfo, ex ); }
            }
        }

        /// <summary>
        ///     Ensures that all types in 'assemblies' that are marked [Immutable] follow the rules for immutability.
        /// </summary>
        /// <exception cref="ImmutableFailureException">Thrown if a mutability issue appears.</exception>
        public static void VerifyTypesAreImmutable( IEnumerable<Assembly> assemblies, params Type[] whiteList ) {
            var typesMarkedImmutable = from type in assemblies.GetTypes() where IsMarkedImmutable( type ) select type;

            foreach ( var type in typesMarkedImmutable ) { VerifyTypeIsImmutable( type, whiteList ); }
        }

        [JsonObject]
        [Serializable]
        private class MutableBaseException : ImmutableFailureException {

            protected MutableBaseException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) { }

            internal MutableBaseException( Type type, Exception inner ) : base( type, FormatMessage( type ), inner ) { }

            private static String FormatMessage( Type type ) => $"'{type}' is mutable because its base type ('[{type.BaseType}]') is mutable.";
        }

        [JsonObject]
        [Serializable]
        private class MutableFieldException : ImmutableFailureException {

            protected MutableFieldException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) { }

            internal MutableFieldException( FieldInfo fieldInfo, Exception inner ) : base( fieldInfo.DeclaringType, FormatMessage( fieldInfo ), inner ) { }

            [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)" )]
            private static String FormatMessage( FieldInfo fieldInfo ) => $"'{fieldInfo.DeclaringType}' is mutable because '{fieldInfo.Name}' of type '{fieldInfo.FieldType}' is mutable.";
        }

        [JsonObject]
        [Serializable]
        private class WritableFieldException : ImmutableFailureException {

            protected WritableFieldException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) { }

            internal WritableFieldException( FieldInfo fieldInfo ) : base( fieldInfo.DeclaringType, FormatMessage( fieldInfo ) ) { }

            private static String FormatMessage( FieldInfo fieldInfo ) => $"'{fieldInfo.DeclaringType}' is mutable because field '{fieldInfo.Name}' is not marked 'makeitget'.";
        }

        [JsonObject]
        [Serializable]
        public class ImmutableFailureException : Exception {

            public Type Type { get; }

            protected ImmutableFailureException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) { }

            internal ImmutableFailureException( Type type, String message, Exception inner ) : base( message, inner ) => this.Type = type;

            internal ImmutableFailureException( Type type, String message ) : base( message ) => this.Type = type;
        }
    }
}