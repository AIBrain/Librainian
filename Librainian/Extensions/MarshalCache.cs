// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "MarshalCache.cs" belongs to Rick@AIBrain.org and
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
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "MarshalCache.cs" was last formatted by Protiguous on 2018/06/04 at 3:52 PM.

namespace Librainian.Extensions {

	/*
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Pulled from https://github.com/lolp1/Process.NET/blob/master/src/Process.NET/Marshaling/MarshalCache.cs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class MarshalCache<T> {

        public static readonly GetUnsafePtrDelegate GetUnsafePtr;

        /// <summary>
        ///     The real, underlying type.
        /// </summary>
        public static readonly Type RealType;

        /// <summary>
        ///     The size of the Type
        /// </summary>
        public static Int32 Size;

        /// <summary>
        ///     The type code
        /// </summary>
        public static TypeCode TypeCode;

        /// <summary>
        ///     True if this type requires the Marshaler to map variables. (No direct pointer dereferencing)
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
                        if ( attr[0] is FixedBufferAttribute fba ) { totalSize += GetSizeOf( fba.ElementType ) * fba.Length; }
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
                if ( t == typeof( IntPtr ) || t == typeof( String ) ) { continue; }

                // If it's a custom object, then check it separately for marshaling requirements.
                if ( Type.GetTypeCode( t ) == TypeCode.Object ) { requires |= RequiresMarshal( fieldInfo.FieldType ); }

                // if anything requires a marshal, period, no matter where/what it is. just return true. Hop out of this func as early as possible.
                if ( !requires ) { continue; }

                Debug.WriteLine( fieldInfo.FieldType.Name + " requires marshaling." );

                return true;
            }

            return false;
        }
    }
    */

}