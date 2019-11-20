// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ClassAndStructExt.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "LibrainianCore", "ClassAndStructExt.cs" was last formatted by Protiguous on 2019/11/20 at 5:01 AM.

namespace LibrainianCore {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Xunit;

    /// <summary>Used directly from LanguageExt source with my modifications.</summary>
    public static class ClassAndStructExt {

        /// <summary>Returns true if the value is equal to this type's default value.</summary>
        /// <example>0.IsDefault()  // true 1.IsDefault()  // false</example>
        /// <returns>True if the value is equal to this type's default value</returns>
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsDefault<A>( this A value ) => Check<A>.IsDefault( value );

        /// <summary>Returns true if the value is null, and does so without boxing of any value-types.  Value-types will always return false.</summary>
        /// <example>int x = 0; string y = null; x.IsNull()  // false y.IsNull()  // true</example>
        /// <returns>True if the value is null, and does so without boxing of any value-types.  Value-types will always return false.</returns>
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsNull<A>( this A value ) => Check<A>.IsNull( value );

        internal static class Check<A> {

            private static EqualityComparer<A> DefaultEqualityComparer { get; }

            private static NullableOrRefFlag flags { get; }

            static Check() {
                if ( Nullable.GetUnderlyingType( typeof( A ) ) != null ) {
                    flags |= NullableOrRefFlag.Nullable;
                }

                if ( !typeof( A ).GetTypeInfo()?.IsValueType == true ) {
                    flags |= NullableOrRefFlag.IsRef;
                }

                DefaultEqualityComparer = EqualityComparer<A>.Default;
            }

            [Flags]
            private enum NullableOrRefFlag : Byte {

                None = 0,

                Nullable = 0b1,

                IsRef = 0b10
            }

            //private static  Boolean IsNullable;

            //private static  Boolean IsReferenceType;

            //trade a byte for cpu time?
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            internal static Boolean IsDefault( A value ) => DefaultEqualityComparer.Equals( value, default );

            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            internal static Boolean IsNull( A value ) => value is null && ( flags.HasFlag( NullableOrRefFlag.IsRef ) || flags.HasFlag( NullableOrRefFlag.Nullable ) );
        }

        public static class Tests {

            [Theory]
            [InlineData( typeof( MyClass ) )]
            public static void TestClassHasDefaults<T>( T cls ) {

                //Is.TypeOf
                if ( cls is null ) {
                    throw new ArgumentNullException( paramName: nameof( cls ) );
                }

                Assert.False( cls.IsDefault() );
            }

            [Fact]
            public static void TestClassNulls() {
                Assert.False( 0.IsNull() );
                Assert.False( 1.IsNull() );

                MyClass none = default;
                Assert.True( none.IsNull() );

                var test = new MyClass();
                Assert.False( test.IsNull() );
            }

            [Fact]
            public static void TestStructDefaults() {
                Assert.True( 0.IsDefault() );
                Assert.False( 1.IsDefault() );

                MyClass none = default;
                Assert.True( none.IsDefault() );

                var test = new MyClass();
                Assert.False( test.IsDefault() );
            }

            [Fact]
            public static void TestStructNulls() {
                Assert.False( 0.IsNull() );
                Assert.False( 1.IsNull() );

                MyClass none = default;
                Assert.True( none.IsNull() );

                var test = new MyClass();
                Assert.False( test.IsNull() );
            }

            [Theory]
            [InlineData( Int32.MinValue )]
            [InlineData( 0 )]
            [InlineData( 1 )]
            [InlineData( Int32.MaxValue )]
            public static void TestValuesHaveDefaults( Int32 value ) {
                if ( value == 0 ) {
                    Assert.True( value.IsDefault() );
                }
                else {
                    Assert.False( value.IsDefault() );
                }
            }

            public struct MyStruct { }

            public class MyClass { }
        }
    }
}