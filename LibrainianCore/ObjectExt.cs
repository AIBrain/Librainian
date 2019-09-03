// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ObjectExt.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", "ObjectExt.cs" was last formatted by Protiguous on 2019/08/27 at 4:05 PM.

namespace LanguageExt {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Xunit;

    /// <summary>
    ///     Used directly from LanguageExt source with my modifications.
    /// </summary>
    public static class ObjectExt {

        public class MyClass {}

        [Fact]
        public static void TestObjectExtDefaults() {
            Assert.True( 0.IsDefault() );
            Assert.False( 1.IsDefault() );

            MyClass none = default;
            Assert.True( none.IsDefault() );

            var test = new MyClass();
            Assert.False( test.IsDefault() );
        }

        [Fact]
        public static void TestObjectExtNulls() {
            Assert.False( 0.IsNull() );
            Assert.False( 1.IsNull() );

            MyClass none = default;
            Assert.True( none.IsNull() );

            var test = new MyClass();
            Assert.False( test.IsNull() );
        }

        /// <summary>
        ///     Returns true if the value is equal to this type's
        ///     default value.
        /// </summary>
        /// <example>
        ///     0.IsDefault()  // true
        ///     1.IsDefault()  // false
        /// </example>
        /// <returns>
        ///     True if the value is equal to this type's
        ///     default value
        /// </returns>
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsDefault<A>( this A value ) => Check<A>.IsDefault( value );

        /// <summary>
        ///     Returns true if the value is null, and does so without
        ///     boxing of any value-types.  Value-types will always
        ///     return false.
        /// </summary>
        /// <example>
        ///     int x = 0;
        ///     string y = null;
        ///     x.IsNull()  // false
        ///     y.IsNull()  // true
        /// </example>
        /// <returns>
        ///     True if the value is null, and does so without
        ///     boxing of any value-types.  Value-types will always
        ///     return false.
        /// </returns>
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsNull<A>( this A value ) => Check<A>.IsNull( value );

        internal static class Check<A> {

            private static readonly EqualityComparer<A> DefaultEqualityComparer;

            private static readonly myFlags flags;

            static Check() {
                if ( Nullable.GetUnderlyingType( typeof( A ) ) != null ) {
                    flags |= myFlags.Nullable;
                }

                if ( !typeof( A ).GetTypeInfo().IsValueType ) {
                    flags |= myFlags.IsRef;
                }

                DefaultEqualityComparer = EqualityComparer<A>.Default;
            }

            [Flags]
            private enum myFlags : Byte {

                None = 0,

                Nullable = 0b1,

                IsRef = 0b10

            }

            //private static readonly Boolean IsNullable;

            //private static readonly Boolean IsReferenceType;

            //trade a byte for cpu time?
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            internal static Boolean IsDefault( A value ) => DefaultEqualityComparer.Equals( value, default );

            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            internal static Boolean IsNull( A value ) =>
                flags.HasFlag( myFlags.IsRef ) && ReferenceEquals( value, null ) || flags.HasFlag( myFlags.Nullable ) && value.Equals( default );

        }

    }

}