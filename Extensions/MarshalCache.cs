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
// "Librainian/MarshalCache.cs" was last cleaned by Protiguous on 2018/05/12 at 1:22 AM

namespace Librainian.Extensions {

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Pulled from https://github.com/lolp1/Process.NET/blob/master/src/Process.NET/Marshaling/MarshalCache.cs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SuppressMessage( "ReSharper", "StaticMemberInGenericType" )]
    public static class MarshalCache<T> {

        public static readonly GetUnsafePtrDelegate GetUnsafePtr;

        /// <summary>
        /// The real, underlying type.
        /// </summary>
        public static readonly Type RealType;

        /// <summary>
        /// The size of the Type
        /// </summary>
        public static Int32 Size;

        /// <summary>
        /// The type code
        /// </summary>
        public static TypeCode TypeCode;

        /// <summary>
        /// True if this type requires the Marshaler to map variables. (No direct pointer dereferencing)
        /// </summary>
        public static Boolean TypeRequiresMarshal;

        static MarshalCache() {
            TypeCode = Type.GetTypeCode( typeof( T ) );

            // Bools = 1 char.
            if ( typeof( T ) == typeof( Boolean ) ) {
                Size = 1;
                RealType = typeof( T );
            }
            else if ( typeof( T ).IsEnum ) {
                var underlying = typeof( T ).GetEnumUnderlyingType();
                Size = GetSizeOf( underlying );
                RealType = underlying;
                TypeCode = Type.GetTypeCode( underlying );
            }
            else {
                Size = GetSizeOf( typeof( T ) );
                RealType = typeof( T );
            }

            // Basically, if any members of the type have a MarshalAs attrib, then we can't just pointer deref. :( This literally means any kind of MarshalAs. Strings, arrays, custom type sizes, etc. Ideally, we want to
            // avoid the Marshaler as much as possible. It causes a lot of overhead, and for a memory reading lib where we need the best speed possible, we do things manually when possible!
            TypeRequiresMarshal = RequiresMarshal( RealType );

            //Debug.WriteLine("Type " + typeof(T).Name + " requires marshaling: " + TypeRequiresMarshal);

            // Generate a method to get the address of a generic type. We'll be using this for RtlMoveMemory later for much faster structure reads.
            var method = new DynamicMethod( $"GetPinnedPtr<{typeof( T ).FullName.Replace( ".", "<>" )}>", typeof( void* ), new[] { typeof( T ).MakeByRefType() }, typeof( MarshalCache<> ).Module );
            var generator = method.GetILGenerator();
            generator.Emit( OpCodes.Ldarg_0 );
            generator.Emit( OpCodes.Conv_U );
            generator.Emit( OpCodes.Ret );
            GetUnsafePtr = ( GetUnsafePtrDelegate )method.CreateDelegate( typeof( GetUnsafePtrDelegate ) );
        }

        public unsafe delegate void* GetUnsafePtrDelegate( ref T value );

        private static Int32 GetSizeOf( Type t ) {
            try {

                // Note: This is in a try/catch for a reason.

                // A structure doesn't have to be marked as generic, to have generic types INSIDE of it. Marshal.SizeOf will toss an exception when it can't find a size due to a generic type inside it. Also... this just
                // makes sure we can handle any other shenanigans the marshaler does.
                return Marshal.SizeOf( t );
            }
            catch {

                // So, chances are, we're using generic sub-types. This is a good, and bad thing. Good for STL implementations, bad for most everything else. But for the sake of completeness, lets make this work.

                var totalSize = 0;

                foreach ( var field in t.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance ) ) {

                    // Check if its a fixed-size-buffer. Eg; fixed byte Pad[50];
                    var attr = field.GetCustomAttributes( typeof( FixedBufferAttribute ), false );

                    if ( attr.Length > 0 ) {
                        if ( attr[0] is FixedBufferAttribute fba ) {
                            totalSize += GetSizeOf( fba.ElementType ) * fba.Length;
                        }
                    }

                    // Recursive. We want to allow ourselves to dive back into this function if we need to!
                    totalSize += GetSizeOf( field.FieldType );
                }

                return totalSize;
            }
        }

        private static Boolean RequiresMarshal( Type t ) {
            foreach ( var fieldInfo in t.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) ) {
                var requires = fieldInfo.GetCustomAttributes( typeof( MarshalAsAttribute ), true ).Any();

                if ( requires ) {
                    Debug.WriteLine( fieldInfo.FieldType.Name + " requires marshaling." );

                    return true;
                }

                // Nope
                if ( t == typeof( IntPtr ) || t == typeof( String ) ) {
                    continue;
                }

                // If it's a custom object, then check it separately for marshaling requirements.
                if ( Type.GetTypeCode( t ) == TypeCode.Object ) {
                    requires |= RequiresMarshal( fieldInfo.FieldType );
                }

                // if anything requires a marshal, period, no matter where/what it is. just return true. Hop out of this func as early as possible.
                if ( !requires ) {
                    continue;
                }

                Debug.WriteLine( fieldInfo.FieldType.Name + " requires marshaling." );

                return true;
            }

            return false;
        }
    }
}