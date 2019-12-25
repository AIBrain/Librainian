// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Tests.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
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
// Project: "LibrainianCoreTests", "Tests.cs" was last formatted by Protiguous on 2019/12/22 at 6:39 AM.

namespace LibrainianCoreTests {

    using System;
    using Xunit;

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

        internal struct MyStruct { }

        internal class MyClass { }
    }

}