// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "IniFileTests.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/LibrainianTests/IniFileTests.cs" was last cleaned by Protiguous on 2018/05/15 at 10:51 PM.

namespace LibrainianTests.Persistence {

    using System;
    using FluentAssertions;
    using Librainian.FileSystem;
    using Librainian.Persistence;
    using NUnit.Framework;

    [TestFixture]
    public static class IniFileTests {

        public const String ini_test_data = @"
[ Section 1  ]
;This is a comment
data1=value1
data2 =value2
data3= value3
data4 = value4
data5   =   value5

[ Section 2  ]

//This is also a comment
data11=value11
data22 = value22
data33   =   value33
data44 =value44
data55= value55

[ Section 2  ]

//This is also a comment
data11=value11b
data22 = value22b
data33   =   value33b

[ Section 3  ]

//This is also a comment
data11=1
data22 = 2
data33   =   3

";

        public static IniFile Ini1;

        public static IniFile Ini2;

        public static IniFile Ini3;

        //[OneTimeSetUp]
        public static void Setup() { }

        [Test]
        public static void test_load_from_file() {

            //prepare file
            var config = Document.GetTempDocument( "config" );
            config.AppendText( ini_test_data );

            Ini2 = new IniFile( config ) { ["Greetings", "Hello"] = "world1!", ["Greetings", "Hello"] = "world2!" };
            Ini2["Greetings", "Hello"].Should().Be( "world2!" );
        }

        [Test]
        public static void test_load_from_string() {
            Ini1 = new IniFile( ini_test_data );
            Ini1.Save( Document.GetTempDocument( "config" ) );
        }
    }
}